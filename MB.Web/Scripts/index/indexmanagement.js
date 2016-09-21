/**
 * Created by DELL on 15-7-16.
 */
$(function () {
   var argus2={
    module:{
            moduleId:null,           //模块ID
            title:"",               //标题
            displayTitle:1,       //表示标题
            linkTarget:1,         //链接方式
            maxRow:1,             //显示条数       
            width:640,              //宽度
            height:350,             //高度
            type:1,               //表示类型
            position:"",        //位置
            defaultEfficiency:null,  //默认显示
            topDisplay:null,         //TOP
            topDisplayLine:null,     //显示曲线
            efficiencyValue:null,    //功效价值
            executiveForce:null,     //执行力
            objective:null,          //目标考核
            defaultLine:null         //默认显示曲线
    },
   target:[]
    }
    var imgargus={
        imageId:1      //图片ID
    }
   var newOrUpdate = 0;//1:更新操作，2:原来移除模块位置(不包括占了了一整行的新建模块)上的新建操作，3:在所有模块的最后进行新建操作 4原来移除模块位置上占了一整行的新建操作
   var maxPosition;//
   var argus3={
        moduleId:[],           //模块ID
        width:[],              //宽度
        position:[]        //位置
    }

   var arg={
      imageId:[],            //图片ID
      imageUrl:[],         //图片地址
      width:[],             //宽度
      height:[]            //高度
   }
   var photoarray = new Array();//用于保存轮播向里面的图片列表图片
   var photoChoosearray = new Array();//用于保存在图库中选中的图片信息，当图库点击确定时，将该值赋值给photoarray
   var Ztreearray = new Array();//用于保存ztree树中选中的值
   var ZtreeChecked = new Array();
  // $('#Setup_modal').modal('show');
   
 //  ztreeOnload("/Shared/GetOrganizationList", 5, "/IndexManagement/GetIndexStatistics", 183);
   photoLoad();
   Loadmodule();
   var defaultLine = 0;
  
   function ztreeOnload(url1, type, url2, module) {
       $.ajax({
           type: "post",
           url: url2,
           dataType: "json",
           data: { moduleId: module },
           success: rsHandler(function (data) {
               ZtreeChecked = data;
               console.log(ZtreeChecked);
           })

       })
     // 加载ztree
     //var ztree_lodi;
    $.ajax({
        type: "post",
        url:url1,
        dataType: "json",
        success: rsHandler(function (data) {
            var categoryTree = $.fn.zTree.init($("#catalogue"), $.extend({
                callback: {
                    onNodeCreated: function (event, treeId, treeNode) {
                        if (type == 5) {
                            var $checked = $(" <input id='" + treeNode.id + "'type='checkbox' class='haschild' style=' margin-left: 20px; margin-right: 3px; display:none; '/><a style='display:none;'>统计子组织</a>");
                        }
                        else { var $checked = $(" <input id='" + treeNode.id + "'type='checkbox' class='haschild'style=' margin-left: 20px; margin-right: 3px; display:none;'/><a style='display:none;'>包含子分类</a>"); }
                        var tid = '#' + treeNode.tId;
                        $(tid).append($checked);
                        //对于每一个新生成的节点，判断是否在原来选中数据中
                        var t = $.fn.zTree.getZTreeObj(treeId);
                        $.each(ZtreeChecked, function (i, v) {

                            if (v.targetId == treeNode.id) {
                                t.checkNode(treeNode, true, false);
                                console.log(treeNode.id);
                                $('#' + treeNode.tId).find(".haschild").first().css("display", "inline");
                                $('#' + treeNode.tId).find(".haschild").first().next("a").css("display", "inline");
                                if (v.withSub == 1) {
                                    $('#' + treeNode.tId).find(".haschild").first().attr("checked", true);
                                }
                                else {
                                    $('#' + treeNode.tId).find(".haschild").first().attr("checked", false);
                                }
                                return false;
                            }
                            

                        })
                        $(".haschild").click(function () {
                             
                            var targetargus = {
                                targetId: 1,        //组织ID
                                withSub: 1               //统计子组织
                            }
                            targetargus.targetId = parseInt($(this).attr("id"));
                            for (var i = 0; i < Ztreearray.length; i++) {
                                if (Ztreearray[i].targetId == parseInt($(this).attr("id"))) {
                                    Ztreearray.splice(i, 1);
                                    break;
                                }
                            }
                            if ($(this).is(":checked")) { targetargus.withSub = 1; }
                            else { targetargus.withSub = 0; }
                            Ztreearray.push(targetargus);
                        })
                        //ztreecheckedOnload(url2, module);
                           
                    },
                    onCheck: function (e, id, node) {
                        //如果节点被选中，则添加相应数据          
                        if (node.checked == true) {
                            var targetargus = {
                                targetId: 1,        //组织ID
                                withSub: 1               //统计子组织
                            }
                            targetargus.targetId = node.id;
                            targetargus.withSub = 0;
                            //if ($('#' + nodes[i].tId).find(".haschild").is(":checked")) {
                            //    targetargus.withSub = 1;
                           // } else { targetargus.withSub = 0; }
                            Ztreearray.push(targetargus);
                            //控制子组织的显示与隐藏      
                            $('#' + node.tId).find(".haschild").first().css("display", "inline");
                            $('#' + node.tId).find(".haschild").first().next("a").css("display", "inline");
                        }
                        else {
                            //如果节点选中状态被取消，则移除相应数据
                            for (var i = 0; i < Ztreearray.length; i++) {
                                if (Ztreearray[i].targetId == node.id) {                                   
                                    Ztreearray.splice(i, 1);
                                }
                            }
                            //控制子组织的显示与隐藏   
                            $('#' + node.tId).find(".haschild").first().css("display", "none");
                            $('#' + node.tId).find(".haschild").first().next("a").css("display", "none");
                            $('#' + node.tId).find(".haschild").first().prop("checked", false);
                        }                                               
                    } 
                }
            }, {
                view: {
                    showIcon: false,
                    showLine: false
                },
                check: {
                    autoCheckTrigger: false,
                    enable: true,
                    chkStyle: "checkbox",
                    chkboxType:{ "Y": "", "N": "" }//Y：勾选，p：关联父节点，s：关联子节点，N：取消勾选
                },
                async: {
                    enable: true,
                    url:url1,
                    autoParam: ["id=parent"],
                    dataFilter: function (treeId, parentNode, responseData) {
                        return responseData.data;
                    }
                }
            }), data);
           
        })
        //complete: function () {
        //    layer.close(ztree_lodi);
        //}
    });

   
    }
   function ztreecheckedData(url2, module) {
       //加载ztree选中部分
       $.ajax({
           type: "post",
           url: url2,
           dataType: "json",
           data: { moduleId: module },
           success: rsHandler(function (data) {
               Ztreearray = data;
           })

       })

   }
    function ztreecheckedOnload(url2, module) {
        //加载ztree选中部分
        $.ajax({
            type: "post",
            url:url2,
            dataType: "json",
            data: { moduleId: module },
            success: rsHandler(function (data) {               
                var zTree = $.fn.zTree.getZTreeObj("catalogue");
                $.each(data, function (i, v) {
                    var checknode = zTree.getNodeByParam("id",v.targetId,null);
                    if (checknode != null) {
                        zTree.checkNode(checknode, true, false);
                        $('#' + checknode.tId).find(".haschild").first().css("display", "inline");
                        $('#' + checknode.tId).find(".haschild").first().next("a").css("display", "inline");
                        if (v.withSub == 1) {
                            $('#' + checknode.tId).find(".haschild").first().attr("checked", true);
                        }
                        else {
                            $('#' + checknode.tId).find(".haschild").first().attr("checked", false);
                        }
                    }
                   

                })
            })
            
        })
       
    }
   
    /*部门列表加载 结束*/
   //显示图库时点击清除全部，取消全选
    $("#photo_modal .cancleAll").click(function(){
        $("#photo_modal .choose").each(function(){
            if ($(this).hasClass('chooseHit')) {
                $(this).removeClass('chooseHit');
                $('span', this).removeClass('spanHit');
            }
        })
    });
    /* 上传图片 确认上传 开始 */
    /* 确认上传 开始 */
    var lay_affirm;
    var html = '';
    var i = 1;
    var j;
    var k = 0;
    var parttern = /(\.|\/)(jpg|jpeg|tiff|bmp|png|gif)$/i;
    fileUpload();
    function fileUpload() {
        $('#fileupload').fileupload({
            url: '/NewsManagement/InsertNewsImage',
            dataType: 'text',
            add: function (e, data) {
                layer.closeTips();
                var isSubmit = true;
                var photoarry = [];
                $.each(data.files, function (index, value) {

                    if (!parttern.test(value.name)) {
                        ncUnits.alert("上传文件格式不对");
                         
                        isSubmit = false;
                        return;
                    } else if (value.size > 209715200) {
                        ncUnits.alert("上传文件过大(最大200M)");
 
                        isSubmit = false;
                        return;
                    } else {
                        var photoargs = {
                            imageId: null,         //图片ID
                            saveName: "",     //存储名
                            extension: "",    //后缀
                            width: null,           //宽度
                            height: null           //高度
                        }
                        var name = value.name.split(".");
                        photoargs.saveName = name[0];
                        photoargs.extension = name[1];
                        photoarry.push(photoargs);
                        $.ajax({
                            type: "post",
                            //url: "/NewsManagement/AddNewsImage",
                            dataType: "json",
                            data: { data: JSON.stringify(photoargs) },
                            success: rsHandler(function (data) {
                                ncUnits.alert("上传成功");
                                // photoLoad();
                            })
                        });
                    }
                });
                //showDelete();
                if (isSubmit) {
                    data.submit();
                }
            },
            complete: function (data, flag) {
                $('.up_progress').css('display', 'none');
                $('.files li').addClass('uploaded');
            },
            error: function (e, data) {
                ncUnits.alert('error');
            },
            done: function (e, data) {
                var result = JSON.parse(data.result);
                if (result.data.displayName != null) {
                    ncUnits.alert("上传成功");
                    photoLoad();

                } else {
                    ncUnits.alert("上传文件内容不能为空");
                    $(".file").parent().parent().remove();
                }
                if (data.result.status == 0) {
                    ncUnits.alert("上传失败");
                    $this.parent().parent().remove();

                } else {
                    ncUnits.alert("上传成功");
                    //$('.file').eq(k++).attr('term', $.parseJSON(data._response.result).data[0].attachmentId);
                }
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('.up_progress' + j + '').css('width', progress + '%');
            },
            always: function (e, data) {
            }
        });

    }
    /* 上传图片 确认上传 结束 */
   //图库模态框点击确定
    $("#photo_modal .photo-modalsure").click(function(){
        var choosecount=0;
        photoarray.length = 0;//如果点击如果点击选择图库中的确定，那么原有图片数据清零
        
        $("#photo_modal .choose").each(function(){
            if ($(this).hasClass('chooseHit')) {
                choosecount++;
                var photoarg={
                    imageId:[],            //图片ID
                    imageUrl:[],         //图片地址
                    width:[],             //宽度
                    height:[]            //高度
                }
                photoarg.imageId=$(this).closest(".col-xs-3").find(".img-rounded").attr("id");
                photoarg.imageUrl=$(this).closest(".col-xs-3").find(".img-rounded").attr("src");
                photoarray.push(photoarg);
            }
        })
        if (choosecount == 0) {
            ncUnits.alert("没有勾选图片，请勾选图片!");           
        }
        else if (choosecount > 10) {
            ncUnits.alert("勾选图片超过10张，请去掉多选图片!");           
        }
        else{
            /*将在图库选择的图片加载到图片轮播选项弹窗中开始*/
            //加载图片信息
            IndexphotoLoad();
           $("#photo_modal").modal('hide');
        }
    })
    //点击设置时出现设置框
    $(".setbutton").click(function(){
        $("#Setup_modal .content").css('display','block');
    });
    //新建时对radio模块的控制
    $(".modalRadio input").click(function () {
        $("#catalogue").empty();
        $("#Setup_modal .content").css('display', 'none');

        newModuleRadioChange();
            if($(this).attr("id")=="baz1"){
                $(".modalnews").css('display','block');
                $(".modal-size").css('display','block');
                $(".modaldocument").css('display','none');
                $(".efficiencyValue").css('display','none');
                $(".modal-phonto").css('display','none');
                ztreeOnload("/Shared/GetNewsTypeList", 1);
            }
            else  if($(this).attr("id")=="baz2"){
                $(".modalnews").css('display','block');
                $(".modal-size").css('display','block');
                $(".modaldocument").css('display','none');
                $(".efficiencyValue").css('display','none');
                $(".modal-phonto").css('display','none');
               
                ztreeOnload("/Shared/GetNoticeTypeList", 2);
            }
            else if($(this).attr("id")=="baz3"){
                $(".modaldocument").css('display','block');
                $(".modal-size").css('display','block');
                $(".modalnews").css('display','none');
                $(".efficiencyValue").css('display','none');
                $(".modal-phonto").css('display','none');
                
                ztreeOnload("/Shared/GetDocumentFolder", 3);
            }
            else if($(this).attr("id")=="baz4"){
                $(".modal-phonto").css('display','block');
                $(".modaldocument").css('display','none');
                $(".modal-size").css('display','none');
                $(".modalnews").css('display','none');
                $(".efficiencyValue").css('display','none');
                 
            }
            else if($(this).attr("id")=="baz5"){
                $(".modal-phonto").css('display','none');
                $(".modaldocument").css('display','none');
                $(".modal-size").css('display','none');
                $(".modalnews").css('display','none');
                $(".efficiencyValue").css('display','block');
                
                ztreeOnload("/Shared/GetOrganizationList", 5);

            }
            

    });
    //编辑时对radio模块的控制
    function defalutRadio(x) {
        $("#baz1").prop("disabled", false);
        $("#baz2").prop("disabled", false);
        $("#baz3").prop("disabled", false);
        $("#baz4").prop("disabled", false);
        $("#baz5").prop("disabled", false);
        $("#baz1").prop("checked", false);
        $("#baz2").prop("checked", false);
        $("#baz3").prop("checked", false);
        $("#baz4").prop("checked", false);
        $("#baz5").prop("checked", false);
        if(x==1){
            $(".modalnews").css('display','block');
            $(".modal-size").css('display','block');
            $(".modaldocument").css('display','none');
            $(".efficiencyValue").css('display','none');
            $(".modal-phonto").css('display', 'none');

            $("#baz1").prop("checked", true);

        }
        else if( x==2){
            $(".modalnews").css('display','block');
            $(".modal-size").css('display','block');
            $(".modaldocument").css('display','none');
            $(".efficiencyValue").css('display','none');
            $(".modal-phonto").css('display','none');          
            $("#baz2").prop("checked", true);
        }
        else if(x==3){
            $(".modaldocument").css('display','block');
            $(".modal-size").css('display','block');
            $(".modalnews").css('display','none');
            $(".efficiencyValue").css('display','none');
            $(".modal-phonto").css('display','none');         
            $("#baz3").prop("checked", true);
        }
        else if(x==4){
            $(".modal-phonto").css('display','block');
            $(".modaldocument").css('display','none');
            $(".modal-size").css('display','none');
            $(".modalnews").css('display','none');
            $(".efficiencyValue").css('display','none');
           
            $("#baz4").prop("checked", true);

        }
        else if(x==5){
            $(".modal-phonto").css('display','none');
            $(".modaldocument").css('display','none');
            $(".modal-size").css('display','none');
            $(".modalnews").css('display','none');
            $(".efficiencyValue").css('display','block');          
            $("#baz5").prop("checked", true);
        }
        Ztreearray.length = 0;//重新初始化Ztreearray
        $("#baz1").prop("disabled", true);
        $("#baz2").prop("disabled", true);
        $("#baz3").prop("disabled", true);
        $("#baz4").prop("disabled", true);
        $("#baz5").prop("disabled", true);
    };

//画面初始化。从数据库中加载模块
    function Loadmodule() {
        var add_load = getLoadingPosition(".all");
        $.ajax({
            type: "post",
            url: "/IndexManagement/GetModuleList",
            dataType: "json",
            complete: rcHandler(function () {
                add_load.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                var $documents = $(".all");
                $documents.empty();
                 
                var flag = 0;
                if (data == null) {
                    maxPosition = "0-0";
                    var $add = $("<div class='add'><span>还没有任何模块，点击</span><button class='addPicture'>新建</button></div>");
                    var firstnewmodule = $("<div class='firstnewmodule'></div>");
                    firstnewmodule.append($add);
                    $documents.append(firstnewmodule);
                    $(".addPicture").click(function () {
                        defaultLine = 0;//初始化defaultLine
                        Ztreearray.length = 0;//初始化Ztreearray
                        
                        newmoduleclearSetModal();
                        $("#Setup_modal").modal('show');                
                        $("#Setup_modal .content").css('display', 'none');
                        $("#catalogue").empty();
                        ztreeOnload("/Shared/GetNewsTypeList", 1);
                        $(".modalnews").css('display', 'block');
                        $(".modal-size").css('display', 'block');
                        $(".modaldocument").css('display', 'none');
                        $(".efficiencyValue").css('display', 'none');
                        $(".modal-phonto").css('display', 'none');
                        
                        $("#baz1").attr("checked", true);
                        $("#baz1").attr("disabled", false);
                        $("#baz2").attr("disabled", false);
                        $("#baz3").attr("disabled", false);
                        $("#baz4").attr("disabled", false);
                        $("#baz5").attr("disabled", false);
                        if ($(this).closest(".module").attr("id") != undefined && $(this).closest(".module").attr("id").substr(0, 1) == "R") {//如果此处点击的新建是之前移除的模块
                            //修改开始 考虑连续一行的新建按照默认大小处理                          
                            newOrUpdate = 2;
                            argus2.module.position = $(this).closest(".module").attr("id").split("R", 3)[1];
                            if ($(this).closest(".module").hasClass("fullline")) {//正确应该为fullline
                                newOrUpdate = 4;
                            }
                            if (newOrUpdate == 2) {
                                argus2.module.width = $(this).closest(".module").width();
                            }
                            //修改结束 考虑连续一行的新建按照默认大小处理
                        }
                        else { newOrUpdate = 3; }
                       // console.log("position" + argus2.module.position);
                       // console.log("newOrUpdate" + newOrUpdate);
                    });
                    return;
                }
                //从开头就要添加新建模块的情况
               else if(data.length>=1&&(data[0].module.position!='1-1'||data[0].module.position!='1-0')){
                   for (var i = 1; i < data[0].module.position.split("-", 1) ; i++) {
                       var $move = $("<div class='layout-operate cccs'><a href='javascript:void(0)' class='glyphicon glyphicon-trash'></a><a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");
                        var $box =$("<div class='box'style='border:0px;'></div>"); 
                        var $module = $("<div class='newmodual module fullline' style='width:1202px;margin-left:0px' id='" + 'R' + i + '-0' + "'></div>");
                        var $add = $(" <div class='add '><button class='addPicture'>新建模块</button></div>"); 
                        $module.append($add);
                        $box.append($module,$move);
                        $documents.append($box);
                    } 
                    if(data[0].module.position.split("-", 3)[1]==2){
                            var $box =$("<div class='box'style='border:0px;'></div>"); 
                            if(data[0].module.type==4){
                                var $news = $("<div class='news jz module move-module' style='width:" + (1202 - data[0].module.width) + "px;'id='" + 'R' + data[0].module.position.split("-", 1) + '-1' + "'></div>");
                                var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 100px;'
                                $news.append($add);
                                $box.append($news);
                                $documents.append($box);
                            }
                            else{
                                var $doc = $(" <div class='newmodual module move-module' style='margin-left:0px;width:" + (1192 - data[0].module.width) + "px;'id='" + 'R' + data[0].module.position.split("-", 1) + '-1' + "'></div>");
                                var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 200px;'
                                $doc.append($add);
                                $box.append($doc);
                                $documents.append($box);
                            }
                        }
                }
                $.each(data, function (i, v) {                    
                    var s, s1,s2             // 声明变量。
                    var s = v.module.position;
                    maxPosition=v.module.position;                   
                    s1= s.split("-", 3)[0]; //子字符串。
                    s2= s.split("-", 3)[1];//子字符串。                
                    var $dropdown = $(" <div class='dropdown dropdown-right' ></div>");
                    var $ul = $("<ul class='dropdown-menu' role='menu'></ul>");
                    var $remove = $("<li><a href='javascript:void(0)'>移除模块</a></li><li class='divider short'></li>");
                    var $edit = $("<li><a href='javascript:void(0)'>编辑</a></li>");
                
                    var $set = $("<a href='javascript:void(0)' class='dropdown-toggle'   data-toggle='dropdown' data-hover='dropdown' data-delay='50' role='button' aria-expanded='false'>设置<span class='caret'></span></a>");
                    $ul.append($remove, $edit);
                    $dropdown.append($set, $ul);                 
                    if (v.module.type == 1) { var $typeli = $("<li>模块类型:新闻</li>"); }
                    else if (v.module.type == 2) { var $typeli = $("<li>模块类型:通知</li>"); }
                    else if (v.module.type == 3) { var $typeli = $("<li>模块类型:文档</li>"); }
                    else if (v.module.type == 4) { var $typeli = $("<li>模块类型:图片轮播</li>"); }
                    else if (v.module.type == 1) { var $typeli = $("<li>模块类型:绩效统计</li>"); }
                    if (v.module.linkTarget == 0) { var $linkTargetli = $("<li>链接方式:当前页</li>"); }
                    else if (v.module.linkTarget == 1) { var $linkTargetli = $("<li>链接方式:新页面</li>"); }
                    var $maxRowli = $("<li>显示条数:" + v.module.maxRow + "</li>");                 
                   if(s2==1){
                       if(flag+2<data.length){
                          // console.log(" next type:"+data[flag+1].module.type);
                       }
                       if(v.module.type==4||(flag+2<=data.length)&&data[flag+1].module.type==4&&data[flag+1].module.position.split("-", 3)[1]==2){
                          var  $box =$("<div class='box'> </div>");
                        }
                       else{
                          var $box =$("<div class='box'style='border:0px;'></div>");
                        }
                    }
                    else if(s2==2){
                       var $box=$(".box:last");
                    }
                    if(v.module.type==5){
                        //<!-- 功效价值 开始 -->
                        var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");
                  
                        var $box = $(" <div class='box module effectvalue'id='" + v.module.moduleId + "' style='border:0px;'></div>");
                        var $effect = $("<div class='effect'></div>");
                        var $dropdown = $(" <div class='dropdown ' ></div>");
                        var $ul = $("<ul class='dropdown-menu' role='menu'></ul>");                        
                        var $a1 = $("<li><a href='javascript:void(0)'>功效价值</a></li><li class='divider short'></li>");
                        var $a2 = $("<li><a href='javascript:void(0)'>执行力</a></li><li class='divider short'></li>");
                        var $a3 = $("<li><a href='javascript:void(0)'>目标考核</a></li><li class='divider short'></li>");
                        if( v.module.efficiencyValue==1){
                            $ul.append($a1);
                        }
                        if(v.module.executiveForce==1){
                            $ul.append($a2);
                        }
                        if(v.module.objective==1){
                            $ul.append($a3);
                        }
                        if (v.module.defaultLine == 1) {
                            var $TOne = $("<a href='javascript:void(0)' class='dropdown-toggle'   data-toggle='dropdown' data-hover='dropdown' data-delay='50' role='button' aria-expanded='false'>功效价值<span class='caret'></span></a>");
                        }
                        else if (v.module.defaultLine == 2) {
                            var $TOne = $("<a href='javascript:void(0)' class='dropdown-toggle'   data-toggle='dropdown' data-hover='dropdown' data-delay='50' role='button' aria-expanded='false'>执行力<span class='caret'></span></a>");
                        }
                        else if (v.module.defaultLine == 3) {
                            var $TOne = $("<a href='javascript:void(0)' class='dropdown-toggle'   data-toggle='dropdown' data-hover='dropdown' data-delay='50' role='button' aria-expanded='false'>目标考核<span class='caret'></span></a>");
                        }
                        $dropdown.append($TOne,$ul);                       
                        var $title =$("<div class='title' id='statistics_title'></div>");                     
                        $title.append($dropdown);

                        var $list = $("<ul class='list'></ul>");
                        if (v.module.efficiencyValue == 1) { var $efficiencyValueli = $("<li>功效价值</li>"); }
                        if (v.module.executiveForce == 1) { var $executiveForceli = $("<li>执行力</li>"); }
                        if (v.module.objective == 1) { var $objectiveli = $("<li>目标考核</li>"); }

                        if (v.module.defaultLine == 1) { var $defaultLineli = $("<li>默认显示曲线:功效价值</li>"); }
                        else if (v.module.defaultLine == 2) { var $defaultLineli = $("<li>默认显示曲线:执行力</li>"); }
                        else if (v.module.defaultLine == 3) { var $defaultLineli = $("<li>默认显示曲线:目标考核</li>"); }

                        if (v.module.defaultEfficiency == 1) { var $defaultEfficiencyli = $("<li>默认显示功效:年工效</li>"); }
                        else if (v.module.defaultEfficiency == 2) { var $defaultEfficiencyli = $("<li>默认显示功效:月工效</li>"); }
                        var $topDisplayLineli = $("<li>显示曲线数:" + v.module.topDisplayLine + "</li>");
                        $list.append($efficiencyValueli, $executiveForceli, $objectiveli, $defaultLineli, $defaultEfficiencyli, $topDisplayLineli);
                        $effect.append($title, $list);
                        //<!-- 功效价值 结束 -->
                        var $dropdown = $(" <div class='dropdown dropdown-right' ></div>");
                        var $ul = $("<ul class='dropdown-menu' role='menu'></ul>");
                        var $remove = $("<li><a href='javascript:void(0)'>移除模块</a></li><li class='divider short'></li>");
                        var $edit = $("<li><a href='javascript:void(0)'>编辑</a></li>");
                  
                        var $set = $("<a href='javascript:void(0)' class='dropdown-toggle'   data-toggle='dropdown' data-hover='dropdown' data-delay='50' role='button' aria-expanded='false'>设置<span class='caret'></span></a>");
                        $ul.append($remove, $edit);
                        $dropdown.append($set, $ul);
                        
                        var $title =$("<div class='title'></div>");
                        var $span = $("<span> TOP" + v.module.topDisplay + "</span>");
                        $title.append($span,$dropdown);
                        var $list =$("<ul class='list' id='top10_list'> </ul>");
                        var $top = $("<div class='TOP'> </div>");
                        if (v.target != null && v.target.length > 0) {
                            var $targetSourceli = $("<li>来源：</li>");
                            $.each(v.target, function (i, v) {
                                var $targetli = $("</br>&nbsp;&nbsp;<span title='" + v.targetName + "'>" + v.targetName + "</span>");
                                $targetSourceli.append($targetli);
                            });

                            $list.append($targetSourceli);
                        }
                        $top.append( $title, $list);
                        $box.append($effect, $top, $move);
                        $documents.append($box);

                       
                        //<!-- top 结束 -->
                    }
                    else if(v.module.type==4){
                        var $title =$("<div class='title'></div>");
                        var $banner = $("<div class='banner jz module' id= '" + v.module.moduleId + "'></div>");
                        var $list = $("<ul class='list'></ul>");
                        $list.append($typeli);
                        $title.append($dropdown);
                        $banner.append($title,$list);
                        if (s2 == 2) {
                            var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a></div>");
                            $box.append($banner, $move);
                        }
                        else { $box.append($banner); }
                     
                        $documents.append($box);
                       // <!-- 图片轮播 结束 -->
                    }
                    else{
                        if ($(".box:last").find(".add").hasClass("add")) { var lastnew = 1; } else { lastnew = 0 };
                       
                      
                        //x-1时后一个类型为图片轮播(排除两个module之间还有一个新建模块的情况)   或x-2时前一个类型为图片轮播(排除两个module之间还有一个新建模块的情况)
                        if ((flag + 2 <= data.length) && s2 == 1 && data[flag + 1].module.type == 4 && data[flag + 1].module.position.split("-")[1] == 2 && s1 == data[flag + 1].module.position.split("-")[0]|| flag > 0 && s2 == 2 && data[flag - 1].module.type == 4 && lastnew == 0) {
                             
                            var $span = $("<span></span> ");
                            if(v.module.displayTitle==1){
                                $span =$("<span >"+v.module.title+"</span> ");
                            }
                            var $title = $("<div class='title'></div>");
                             
                            var $list = $("<ul class='list' id='news_list'></ul>");
                            var $news = $("<div class='news jhfl jz module move-module' style='width:415px;'></div>");//默认全部是415 "+v.module.width+"
                            var $tzcontent = $("<div class='tzcontent'id='" + v.module.moduleId + "'></div>");                            
                            $list.append($typeli, $linkTargetli, $maxRowli);
                            if (v.target != null && v.target.length>0) {        
                                var $targetSourceli = $("<li>来源：</li>");
                                $.each(v.target, function (i, v) {
                                    var $targetli = $("</br>&nbsp;&nbsp;<span title='" + v.targetName + "'>" + v.targetName + "</span>");
                                    $targetSourceli.append($targetli);
                                });
                                $list.append($targetSourceli);
                            }
                            $title.append($span,$dropdown);
                            $tzcontent.append($title,$list);
                            $news.append($tzcontent);
                            if (s2 == 2) {
                                var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a></div>");
                                $box.append($news, $move);
                            }
                            else { $box.append($news); }                          
                            $documents.append($box);
                        }
                        else if(s2==1){
                            var $span =$("<span></span> ");
                            if(v.module.displayTitle==1){
                                $span =$("<span >"+v.module.title+"</span> ");
                            }
                            var $tzcontent=$("<div class='tzcontent'id='"+v.module.moduleId+"'></div>");
                            var $title =$("<div class='title'></div>");
                            var $list =$("<ul id='message_list' class='list'></ul>");
                            var $message = $(" <div class='messageIndex jhfl module move-module'style='width:" + v.module.width + "px;'> </div>");
                            $list.append($typeli, $linkTargetli, $maxRowli);
                            if (v.target != null && v.target.length > 0) {
                                var $targetSourceli = $("<li>来源：</li>");
                                $.each(v.target, function (i, v) {
                                    var $targetli = $("</br>&nbsp;&nbsp;<span title='" + v.targetName + "'>" + v.targetName + "</span>");
                                    $targetSourceli.append($targetli);
                                });

                                $list.append($targetSourceli);
                            }
                            $title.append($span, $dropdown);                      
                            $tzcontent.append($title,$list);
                            $message.append($tzcontent);
                            $box.append($message);
                            $documents.append($box);
                        }
                        else{
                            var $span =$("<span></span> ");
                            if(v.module.displayTitle==1){
                                $span =$("<span >"+v.module.title+"</span> ");
                            }
                            var $title =$("<div class='title'></div>");
                            var $list =$("<ul id='docu_list' class='list'> </ul>");
                            var $doc = $(" <div class='docuInfor jhfl module move-module'style='width:" + v.module.width + "px;'></div>");
                            var $tzcontent = $("<div class='tzcontent'id='" + v.module.moduleId + "'></div>");
                            $list.append($typeli, $linkTargetli, $maxRowli);
                             
                            if (v.target != null && v.target.length > 0) {
                                var $targetSourceli = $("<li>来源：</li>");
                                $.each(v.target, function (i, v) {
                                    var $targetli = $("</br>&nbsp;&nbsp;<span title='" + v.targetName + "'>" + v.targetName + "</span>");
                                    $targetSourceli.append($targetli);
                                });

                                $list.append($targetSourceli);
                            }
                           
                            $title.append($span,$dropdown);
                            $tzcontent.append($title,$list);
                            $doc.append($tzcontent);
                            if (s2 == 2) {
                                var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a></div>");
                                $box.append($doc, $move);
                            }
                            else { $box.append($doc); }                          
                            $documents.append($box);
                        }
                    }
                    //如果中间有以前被移除的模块，用新建补齐
                    if(flag+2>data.length){
                    }
                    else if(parseInt(v.module.position.split("-",1))+1<parseInt(data[flag+1].module.position.split("-",1))){//如果前后连个模块之间中间空有多行
                        if(v.module.position.split("-")[1]==1){
                            var $box=$(".box:last");
                            if(data[flag].module.type==4){
                                var $news = $("<div class='news jz module move-module' style='width:415px;'id='" + 'R' + v.module.position.split("-", 1) + '-2' + "'></div>");//"+(1200-data[flag].module.width)+"
                                var $add = $("<div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 100px;'
                                $news.append($add);
                                $box.append($news);
                                $documents.append($box);
                            }
                            else{
                                var $doc = $(" <div class='newmodual module move-module'style='margin-left:10px;width:" + (1192 - data[flag].module.width) + "px;'id='" + 'R' + v.module.position.split("-", 1) + '-2' + "'></div>");
                                var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 200px;'
                                $doc.append($add);
                                $box.append($doc);
                                $documents.append($box);
                            }
                        }
                        var j=data[flag+1].module.position.split("-",1)-v.module.position.split("-",1);
                        for(var i=1;i<j;i++){
                            var $box =$("<div class='box'style='border:0px;'></div>"); 
                            var $module = $("<div class='newmodual module fullline' style='width:1202px;margin-left:0px'id='" + 'R' + (i + parseInt(v.module.position.split("-", 1))) + '-0' + "'></div>");//修改
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                            var $move = $("<div class='layout-operate cccs'><a href='javascript:void(0)' class='glyphicon glyphicon-trash'></a><a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");

                            $module.append($add);
                            $box.append($module,$move);
                            $documents.append($box);
                        }
                        if (data[flag + 1].module.position.split("-", 3)[1] == 2) {
                           
                            if(data[flag+1].module.type==4){
                                var $box =$("<div class='box'></div>");
                                var $news = $("<div class='news jz module move-module' style='width:415px;'id='" + 'R' + data[flag + 1].module.position.split("-", 1) + '-1' + "'></div>");//"+(1200-data[flag+1].module.width)+"
                                var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                                $news.append($add);
                                $box.append($news);
                                $documents.append($box);
                            }
                            else{
                                var $box =$("<div class='box'style='border:0px;'></div>"); 
                                var $doc = $(" <div class='newmodual module move-module' style='margin-left:0px;width:" + (1192 - data[flag + 1].module.width) + "px;'id='" + 'R' + data[flag + 1].module.position.split("-", 1) + '-2' + "'></div>");
                                var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                                $doc.append($add);
                                $box.append($doc);
                                $documents.append($box);
                            }
                        }
                    }
                else if(parseInt(v.module.position.split("-",1))+1==parseInt(data[flag+1].module.position.split("-",1))&&v.module.position.split("-",3)[1]==1&&data[flag+1].module.position.split("-",3)[1]==2){
                    //如果x-2 和 y-1模块不存在(x+1=y)的情况下，用新建补全
                    //建立x-2
                     
                    var $box=$(".box:last");
                    var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");

                        if (v.module.type == 4) {
                            var $news = $("<div class='news jz module move-module' style='width:" + (1202 - v.module.width) + "px;'id='" + 'R' + v.module.position.split("-", 1) + '-2' + "'></div>");//考虑替换成1200：   或固定大小
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                            $news.append($add);
                            $box.append($news,$move);
                            $documents.append($box);
                        }
                        else {
                             
                            var $doc = $(" <div class='newmodual module move-module'style='width:" + (1190 - v.module.width) + "px;'id='" + 'R' + v.module.position.split("-", 1) + '-2' + "'></div>");//一定要是1200否则会超出
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 200px;'
                            $doc.append($add);
                            $box.append($doc,$move);
                            $documents.append($box);
                        }
                        //建立x-1位置的新建
                        if(data[flag+1].module.type==4){
                            var $box =$("<div class='box'></div>");
                            var $news = $("<div class='news jz module move-module' style='margin-left:0px;width:415px;'id='" + 'R' + data[flag + 1].module.position.split("-", 1) + '-1' + "'></div>");
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 100px;'
                            $news.append($add);
                            $box.append($news);
                            $documents.append($box);
                        }
                        else{
                            var $box =$("<div class='box'style='border:0px;'></div>");// width:1202px; border:0px;
                            var $doc = $(" <div class='newmodual module move-module' style='margin-left:0px;width:" + (1192 - data[flag + 1].module.width) + "px;'id='" + 'R' + data[flag + 1].module.position.split("-", 1) + '-1' + "'></div>");
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 200px;'
                            $doc.append($add);
                            $box.append($doc);
                            $documents.append($box);
                        }
                    }
                    else if((v.module.position.split("-",3)[1]==0||v.module.position.split("-",3)[1]==2)&&data[flag+1].module.position.split("-",3)[1]==2){//x-1不存在(不包括1-1不存在的情况)
                        if(data[flag+1].module.type==4){
                            var $box =$("<div class='box'></div>");
                            var $news = $("<div class='news jz module move-module' style='width:" + (1200 - data[flag + 1].module.width) + "px;'id='" + 'R' + data[flag + 1].module.position.split("-", 1) + '-1' + "'></div>");//必须是1200
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 100px;'
                            $news.append($add);
                            $box.append($news);
                            $documents.append($box);
                        }
                        else{
                            var $box =$("<div class='box'style='border:0px;'></div>"); 
                            var $doc = $(" <div class='newmodual module move-module' style='margin-left:0px;width:" + (1192 - data[flag + 1].module.width) + "px;'id='" + 'R' + data[flag + 1].module.position.split("-", 1) + '-1' + "'></div>");
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");//style='margin: 100px 200px;'
                            $doc.append($add);
                            $box.append($doc);
                            $documents.append($box);
                        }
                    }
                    else if(v.module.position.split("-",3)[1]==1&&(data[flag+1].module.position.split("-",3)[1]==1||data[flag+1].module.position.split("-",3)[1]==0)){//x-2不存在,建立新建
                        var $box = $(".box:last");
                        var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");
                        if(v.module.type==4){
                            var $news = $("<div class='news  jz module move-module' style='width:" + (1200 - data[flag].module.width) + "px;'id='" + 'R' + v.module.position.split("-", 1) + '-2' + "'></div>");//必须是1200 因为width不包括border
                            
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                            $news.append($add);
                            $box.append($news,$move);
                            $documents.append($box);
                        }
                        else{
                            var $doc = $(" <div class='newmodual module move-module'style='margin-left:10px;width:" + (1192 - data[flag].module.width) + "px;'id='" + 'R' + v.module.position.split("-", 1) + '-2' + "'></div>");
                            var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                            $doc.append($add);
                            $box.append($doc,$move);
                            $documents.append($box);
                        }
                    }
                    flag++;
                    $edit.click(function () {
                        $("#personal_modal_label").text("编辑模块");
                        newOrUpdate = 1;
                        argus2.module.position = v.module.position;
                        argus2.module.width = v.module.width;
                         var  moduleId = v.module.moduleId;
                        argus2.module.moduleId = v.module.moduleId;
                        argus2.module.type = v.module.type;
                        //console.log("edit moduleId" + argus2.module.moduleId + "type" + argus2.module.type + "position" + argus2.module.position);
                         
                        $("#Setup_modal .content").css('display', 'none');
                       
                         
                         $.ajax({
                             type: "post",
                             url: "/IndexManagement/GetModuleInfo",
                             dataType: "json",
                             data: { moduleId: moduleId },
                             success: rsHandler(function (data) {
                                 
                                 $("#catalogue").empty();
                                 defalutRadio(data.module.type);
                                 if (data.module.type == 1 || data.module.type == 2) {
                                     $("#news-title").val(data.module.title);
                                     if (data.module.displayTitle == 1) {
                                         $(".modalmessage .displayTitle").prop("checked",true);
                                     }
                                     else{
                                         $(".modalmessage .displayTitle").prop("checked", false);
                                     }
                                     $(".modalmessage .maxRow").val(data.module.maxRow);
                                     if (data.module.linkTarget) {
                                         $("#linkTarget").attr('term', "1").text("弹出页");
                                     }
                                     else {
                                         $("#linkTarget").attr('term', "0").text("当前页");
                                     }
                                     
                                      
                                     //加载右侧ztree树
                                     if (data.module.type == 1) {
                                         ztreeOnload("/Shared/GetNewsTypeList", data.module.type, "/IndexManagement/GetIndexNews", data.module.moduleId);
                                         ztreecheckedData("/IndexManagement/GetIndexNews", data.module.moduleId);
                                     } else {
                                          ztreeOnload("/Shared/GetNoticeTypeList", data.module.type,"/IndexManagement/GetIndexNotice", data.module.moduleId);
                                          ztreecheckedData("/IndexManagement/GetIndexNotice", data.module.moduleId);
                                     }
                                 }
                                 else if (data.module.type == 3) {
                                     $("#document-title").val(data.module.title);
                                     if (data.module.displayTitle == 1) {
                                         $(".modaldocument .displayTitle").prop("checked", true);
                                     }
                                     else{
                                         $(".modaldocument .displayTitle").prop("checked", false);
                                     }
                                     $("#document-maxRow").val(data.module.maxRow);
                                      
                                     
                                     //加载右侧ztree树
                                     ztreeOnload("/Shared/GetDocumentFolder", data.module.type, "/IndexManagement/GetIndexDocument", data.module.moduleId);
                                     ztreecheckedData("/IndexManagement/GetIndexDocument", data.module.moduleId);
                                 }
                                 else if (data.module.type == 4) {
                                   
                                     //加载图片信息
                                     photoarray = data.image;
                                     
                                    // console.log(photoarray);
                                     IndexphotoLoad();
                                     $("#photo_modal .xxc_choose").each(function () {
                                         if ($(this).hasClass('chooseHit')) {
                                             $(this).removeClass('chooseHit');
                                             $('span',this).removeClass('spanHit');
                                         }
                                        
                                     })
                                     $.each(photoarray, function (i, v) {
                                         $("#photo_modal .img-rounded").each(function () {
                                             if ($(this).attr("id") == v.imageId) {
                                                 $(this).siblings(".xxc_choose").addClass('chooseHit');
                                                 $('span', $(this).siblings(".xxc_choose")).addClass('spanHit');
                                             }
                                            
                                         })
                                     })
                                   
                                      
                                 }
                                 else if (data.module.type == 5) {
                                     $("#efficiency-title").val(data.module.title);
                                     argus2.module.defaultLine = data.module.defaultLine;
                                     if (data.module.defaultLine == 1) {
                                         $("#defaultLine1").addClass("defaultLine");
                                         $("#defaultLine2").removeClass("defaultLine");
                                         $("#defaultLine3").removeClass("defaultLine");
                                     }
                                     else if (data.module.defaultLine == 2) {
                                         $("#defaultLine2").addClass("defaultLine");
                                         $("#defaultLine1").removeClass("defaultLine");
                                         $("#defaultLine3").removeClass("defaultLine");
                                     }
                                     else if (data.module.defaultLine == 3) {
                                         $("#defaultLine3").addClass("defaultLine");
                                         $("#defaultLine1").removeClass("defaultLine");
                                         $("#defaultLine2").removeClass("defaultLine");
                                     }
                                     if (data.module.displayTitle == 1) {
                                         $(".efficiencyValue .displayTitle").prop("checked", true);
                                     }
                                     else{
                                         $(".efficiencyValue .displayTitle").prop("checked", false);
                                     }
                                     if(data.module.defaultEfficiency){
                                         $("#defaultEfficiency").attr('term',"1").text("年功效");
                                     }
                                     else {
                                         $("#defaultEfficiency").attr('term',"2").text("月功效");
                                      }                                                                       
                                     $(".efficiencyValue #topselect").attr('term', data.module.topDisplay).text(data.module.topDisplay);
                                     $(".efficiencyValue #topDisplayLine").attr('term', data.module.topDisplayLine).text(data.module.topDisplayLine); 
                                     
                                     if (data.module.efficiencyValue || data.module.efficiencyValue == 1) {
                                         $(".efficiencyValue #efficiencyValue").prop("checked", true);
                                          $("#defaultLine1").css("display", "inline");
                                     }
                                     else{
                                         $(".efficiencyValue #efficiencyValue").prop("checked", false);
                                          $("#defaultLine1").css("display", "none");
                                     }
                                     if (data.module.executiveForce || data.module.executiveForce == 1) {
                                         $(".efficiencyValue #executiveForce").prop("checked", true);
                                          
                                         $("#defaultLine2").css("display", "inline");
                                     }
                                     else{
                                         $(".efficiencyValue #executiveForce").prop("checked", false);
                                         $("#defaultLine2").css("display", "none");
                                     }
                                     if (data.module.objective||data.module.objective == 1) {
                                         $(".efficiencyValue #objective").prop("checked", true);
                                         $("#defaultLine3").css("display", "inline");
                                     }
                                     else{
                                         $(".efficiencyValue #objective").prop("checked", false);
                                         $("#defaultLine3").css("display", "none");
                                     }                                    
                                     defaultLine = data.module.defaultLine;//修改-防止取消修改时defaultLine值消失
                                                                    
                                     //加载右侧ztree树
                                     console.log("data.module.moduleId" + data.module.moduleId);
                                     console.log("data.module.type" + data.module.type);
                                     ztreeOnload("/Shared/GetOrganizationList", data.module.type, "/IndexManagement/GetIndexStatistics", data.module.moduleId);
                                     ztreecheckedData("/IndexManagement/GetIndexStatistics", data.module.moduleId);
                                     
                                 }
                                 $('#Setup_modal').modal('show');
                             })

                         });
                         
                     });
                    $remove.click(function(){
                        var moduleId= v.module.moduleId;
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除这个模块吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/IndexManagement/DeleteModule",
                                    dataType: "json",
                                    data: { moduleId: moduleId },
                                    success: rsHandler(function (data) {
                                          Loadmodule();//删除后，根据数据库重新加载页面
                                        ncUnits.alert("删除成功!");
                                    })
                                })
                            },
                            no:function(layer_confirm){
                                layer.close(layer_confirm);
                            }
                        });

                    });
                    
                    
                });
                //在数据库数据加载完成后最后加上新建模块
                if (data == null) {
                    
                }
                else if(data[flag-1].module.position.split("-",3)[1]==2||data[flag-1].module.position.split("-",3)[1]==0){
                    var $box =$("<div class='box'style='border:0px;'></div>"); 
                    var $doc = $(" <div class='newmodual' style=' width:1202px;margin-left:0px;'></div>"); 
                    var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>");
                    var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");
                    $doc.append($add);
                    $box.append($doc,$move);
                    $documents.append($box);
                }
                else {
                    var $box=$(".box:last");
                    if(data[flag-1].module.type==4){
                        var $news = $("<div class='news jz module move-module' style='width:415px;'></div>");
                        var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                        var $move = $("<div class='layout-operate cccs'> <a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");
                        $news.append($add);
                        $box.append($news,$move);
                       
                    }
                    else{
                        var $move = $("<div class='layout-operate cccs'><a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a> </div>");
                        var $doc = $(" <div class='newmodual module move-module' style='width:" + (1192 - data[flag - 1].module.width) + "px;'></div>");
                        var $add = $(" <div class='add'><button class='addPicture'>新建模块</button></div>"); 
                        $doc.append($add);
                        $box.append($doc,$move);
                        $documents.append($box);
                    }
                }
                var qishi;
                //.module
                /*
                 $(".news,.docuInfor,.message").sortable({
                    cursor: "move",
                    items :".tzcontent",
                    opacity: 0.6,
                    revert: true,
                    connectWith: ".jhfl",
                    start: function (event, ui) {
                        qishi = $('#' + ui.item.attr("id")).closest(".module");                                                                             
                    },
                    update: function (event, ui) {                     
                        qishi.append(ui.item.siblings());
                        ui.item.siblings().remove();                     
                        positionUpdate();
                    }
                });
                */
                $(".module").sortable({
                    cursor: "move",
                    items: ".tzcontent",
                    opacity: 0.6,
                    revert: true,
                    scroll: true,
                    distance:100,
                    connectWith: ".move-module",//module
                    start: function (event, ui) {
                        qishi = $('#' + ui.item.attr("id")).closest(".module");
                    },
                    update: function (event, ui) {
                        qishi.append(ui.item.siblings());
                        ui.item.siblings().remove();
                        positionUpdate();
                    }
                });
                 
                //box拖动位置
                $(".all").sortable({
                    cursor: "move",
                    items :".box",
                    opacity: 0.6,
                    revert: true,
                    scroll: true,
                    update : function(event, ui){
                        positionUpdate();
                    }
                });
                /*
                  //左侧及其右侧交换位置
                $(".box").sortable({
                    cursor: "move",
                    items :".module",//.jz
                    opacity: 0.6,
                    revert: true,
                    update : function(event, ui){
                        positionUpdate();
                    }
                });
                */
              
                //轮播图片及其右侧交换位置
                $(".box").sortable({
                    cursor: "move",
                    items: ".jz",//.jz
                    opacity: 0.6,
                    revert: true,
                    update: function (event, ui) {
                        positionUpdate();
                    }
                });
                $( ".messageIndex" ).resizable({
                    maxHeight:350,
                    minHeight: 350,
                    maxWidth: 1000,                    
                    minWidth:200,
                    // handles: 'e, w',
                     
                    stop: function(event, ui) {
                        var owidth=0;
                        //owidth=$(".docuInfor").width();
                        owidth = $(this).siblings(".module").width();
                       
                        //console.log(ui.size.width - ui.originalSize.width);
                        $(this).siblings(".module").width(owidth - ui.size.width + ui.originalSize.width + 2);
                        positionUpdate();
                       
                    }
                });
                $( ".docuInfor" ).resizable({
                    maxHeight:350,
                    minHeight: 350,
                    maxWidth: 1000,
                    minWidth: 200,
                //    handles: 'e, w' ,
                    stop: function(event, ui) {
                        var owidth=0;
                        owidth = $(this).siblings(".module").width();                     
                       // console.log(ui.size.width - ui.originalSize.width);
                        $(this).siblings(".module").width(owidth - ui.size.width + ui.originalSize.width + 2);
                        positionUpdate();
                    }
                });
                $(".glyphicon-trash").click(function () {
                    var xx = $(this).closest(".cccs").siblings(".fullline").attr("id").split("R")[1];
                    ncUnits.confirm({
                        title: '提示',
                        html: '确认要删除这个模块吗？',
                        yes: function (layer_confirm) {


                            var arrayObj = new Array();
                            var positionx = 1;
                            var positiony = 1;
                            $(".all .module").each(function () {
                                var argus3 = {
                                    moduleId: [],           //模块ID
                                    width: [],              //宽度
                                    position: []        //位置
                                }//分类排序参数列表

                                argus3.width = $(this).outerWidth();
                                if (positiony == 1) {
                                    if ($(this).width() >= 1100) {
                                        if (positionx == xx.split("-")[0]) {
                                            positionx--;

                                        }
                                        argus3.position = positionx + '-0';//修改
                                        positiony = 1;
                                        positionx++;
                                    }
                                    else {
                                        argus3.position = positionx + '-' + positiony;
                                        positiony++;
                                    }
                                }
                                else {
                                    argus3.position = positionx + '-' + positiony;
                                    positionx++;
                                    positiony = 1;
                                }

                                if ($(this).find(".tzcontent").attr("id") != undefined) {
                                     
                                    argus3.moduleId = $(this).find(".tzcontent").attr("id");
                                    arrayObj.push(argus3);
                                }
                                else if ($(this).hasClass("banner") || $(this).hasClass("effectvalue")) {
                                    argus3.moduleId = $(this).attr("id");
                                    arrayObj.push(argus3);

                                }
                               // console.log("id" + argus3.moduleId + "position" + argus3.position + "position" + argus3.width);

                            });
                          //  console.log(arrayObj);
                            $.ajax({
                                type: "post",
                                url: "/IndexManagement/SaveModuleSize",
                                dataType: "json",
                                data: { data: JSON.stringify(arrayObj) },
                                success: rsHandler(function (data) {
                                  
                                    Loadmodule();
                                })
                            });
                            layer.close(layer_confirm);
                        } 
                    });
                   
                    
                })
                $(".addPicture").click(function () {
                    defaultLine = 0;//初始化defaultLine
                    Ztreearray.length = 0;//初始化Ztreearray           
                  
                    newmoduleclearSetModal();
                    $("#Setup_modal").modal('show');
                    $("#Setup_modal .content").css('display', 'none');                 
                    $("#catalogue").empty();
                    ztreeOnload("/Shared/GetNewsTypeList", 1);
                    $(".modalnews").css('display','block');
                    $(".modal-size").css('display','block');
                    $(".modaldocument").css('display','none');
                    $(".efficiencyValue").css('display','none');
                    $(".modal-phonto").css('display','none');
                    
                    $("#baz1").prop("checked", true);
                    $("#baz1").prop("disabled", false);
                    $("#baz2").prop("disabled", false);
                    $("#baz3").prop("disabled", false);
                    $("#baz4").prop("disabled", false);
                    $("#baz5").prop("disabled",false);
                    if ($(this).closest(".module").attr("id") != undefined && $(this).closest(".module").attr("id").substr(0, 1) == "R") {//如果此处点击的新建是之前移除的模块
                        //修改开始 考虑连续一行的新建按照默认大小处理
                         
                        newOrUpdate=2;
                        argus2.module.position = $(this).closest(".module").attr("id").split("R",3)[1];
                        if ($(this).closest(".module").hasClass("fullline")) {                         
                            newOrUpdate = 4;
                        }
                        if (newOrUpdate == 2) {
                            $("#baz4").prop("disabled", true);
                            $("#baz5").prop("disabled", true);
                            argus2.module.width = $(this).closest(".module").outerWidth();
                             
                        }
                        //修改结束 考虑连续一行的新建按照默认大小处理
                    }
                    else {                       
                        if ($(this).closest(".box").find(".tzcontent").attr("id")!= undefined) {//要考虑x-2的新建大小取决于于前面模块的大小,同时限制模块不能建立激励模块和图片轮播模块
                            argus2.module.width = 1190 - $(this).closest(".box").find(".module").width();
                             
                            $("#baz4").prop("disabled", true);
                            $("#baz5").prop("disabled", true);
                            newOrUpdate = 5;
                            
                        }                       
                        else if ($(this).closest(".box").find(".banner").attr("id") != undefined) {//如果前面一个是图片轮播，那么后面一个大小固定
                            argus2.module.width = 415;
                            $("#baz4").prop("disabled", true);
                            $("#baz5").prop("disabled", true);
                           newOrUpdate = 5;
                        }
                        else { newOrUpdate = 3; }                                      
                    }
                   // console.log("newOrUpdate"+newOrUpdate);
                });

            })
        });
       
       
    }
    function positionUpdate(){
        var arrayObj = new Array();
        var positionx=1;
        var positiony=1;
        $(".all .module").each(function(){
            var argus3={
                moduleId:[],           //模块ID
                width:[],              //宽度
                position:[]        //位置
            }//分类排序参数列表
            argus3.width=$(this).outerWidth();
            if(positiony==1){
                if($(this).width()>=1100){
                    argus3.position = positionx + '-0';//修改
                    positiony = 1;
                    positionx++;
                }
                else{
                    argus3.position=positionx+'-'+positiony;
                    positiony++;
                }
            }
            else {
                argus3.position=positionx+'-'+positiony;
                positionx++;
                positiony=1;
            }
            /*
            if($(this).attr("id")==undefined){
                if ($(this).find(".tzcontent").attr("id") != undefined) {
                    console.log("find tzcontent");
                    argus3.moduleId=$(this).find(".tzcontent").attr("id");
                    arrayObj.push(argus3);
                }
            }
            else if($(this).attr("id").substr(0,1)!="R"){
                argus3.moduleId=$(this).attr("id");
                arrayObj.push(argus3);
               
            }
            */
             
             if ($(this).find(".tzcontent").attr("id") != undefined) {
                   
                    argus3.moduleId = $(this).find(".tzcontent").attr("id");
                    arrayObj.push(argus3);
             }
             else if ($(this).hasClass("banner") || $(this).hasClass("effectvalue")) {
                argus3.moduleId = $(this).attr("id");
                arrayObj.push(argus3);

            }
           // console.log("id" + argus3.moduleId + "position" + argus3.position + "position" + argus3.width);
           
        });
       // console.log(arrayObj);
        $.ajax({
            type: "post",
            url: "/IndexManagement/SaveModuleSize",
            dataType: "json",
            data: { data: JSON.stringify(arrayObj) },
            success: rsHandler(function (data) {
                Loadmodule();
                ncUnits.alert("位置已保存");
            })
        });
    }

    $("#Setup_modal cancle-setup").click(function(){
        $("#Setup_modal .content").css('display','none');
        $("#Setup_modal").modal('hide');
    })

//点击模态框上的确定，提交数据进行更新或新建
    $("#newDoc_modal_submit").click(function(){
        argus2.module.height = 350;
        var rid = $('.modalRadio input:radio[name="modalRadio[1]"]:checked').attr("id");
        if (newOrUpdate != 1) {
            argus2.module.moduleId = null;
            if (rid == "baz1") { argus2.module.type = 1; } else if (rid == "baz2") { argus2.module.type = 2; }
            else if (rid == "baz3") { argus2.module.type = 3; }
            else if (rid == "baz4") { argus2.module.type = 4;} else if (rid == "baz5") { argus2.module.type = 5; }
        }
        
        if (argus2.module.type != 4) {
            argus2.target = Ztreearray;
            
        }
        if (newOrUpdate == 3 || newOrUpdate == 4) {//如果是点击最后的新建或中间独占一整行的新建模块 给模块赋默认高度值
            if (rid == "baz1") { argus2.module.width = 415; } else if (rid == "baz2") { argus2.module.width = 548; }
            else if (rid == "baz3") { argus2.module.width = 640; } else if (rid == "baz4") { argus2.module.width = 785; }
            else if (rid == "baz5") { argus2.module.width = 1202; }
        }
        if (newOrUpdate == 3||newOrUpdate == 5) {//如果是点击最后的新建建立的模块      计算position
            if (maxPosition == "0-0") {
                if (argus2.module.type == 5) {
                    argus2.module.position = "1-0";
                }
                else { argus2.module.position = "1-1"; }
            } else if (maxPosition.split("-", 3)[1] == 2 || maxPosition.split("-", 3)[1] == 0) {
                 
                if(rid=="baz5"){argus2.module.position=parseInt(maxPosition.split("-",1))+1+'-0';}
                else { argus2.module.position = parseInt(maxPosition.split("-", 1)) + 1 + '-1'; }
            }
            else if (maxPosition.split("-", 3)[1] == 1) {
                
                argus2.module.position = maxPosition.split("-", 1)+ '-2';
            }
        }
        else if(newOrUpdate==2){//如果是点击中间被移除的模块产生的新建
            if (rid == "baz5") { alert("不能在此位置建立激励模块"); return; }
        } else if (newOrUpdate == 4) {
            if (rid != "baz5") { argus2.module.position = argus2.module.position.split("-", 1)+ '-1'; }
        }
         
        if(rid=="baz1"||rid=="baz2"){
            argus2.module.title = $("#news-title").val();
            if ($(".modalmessage .displayTitle").is(":checked")) { argus2.module.displayTitle = 1; } else { argus2.module.displayTitle = 0; }             
            argus2.module.maxRow = $(".modalmessage .maxRow").val();
            argus2.module.linkTarget = parseInt($("#linkTarget").attr("term"));
            /*如果没有输入值，则出现提示框 开始 */          
            var Filename = $.trim($("#news-title").val());
            if (Filename == "") {
                ncUnits.alert("模块标题不能为空!");
                return;
            }
            else if (Filename.length > 20) {
                ncUnits.alert("模块标题不能超过20字符!");
                return;
            }
            var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
            if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                ncUnits.alert("模块标题存在非法字符!");
                return;
            }
            if ($(".modalmessage .maxRow").val() == "") {
                ncUnits.alert("显示条数输入为空!");
                return;
            }
                       
            if ($("#linkTarget").text() == "") {
                ncUnits.alert("链接方式未选!");
                return;
            }
            
            /*如果没有输入值，则出现提示框 结束 */
        }
        else if (rid == "baz3") {
            argus2.module.title = $("#document-title").val();           
            argus2.module.maxRow = $("#document-maxRow").val();
            if ($(".modaldocument .displayTitle").is(":checked")) { argus2.module.displayTitle = 1; } else { argus2.module.displayTitle = 0; }
            /*如果没有输入值，则出现提示框 开始 */
            var Filename = $.trim($("#document-title").val());
            if (Filename == "") {
                ncUnits.alert("模块标题不能为空!");
                return;
            }
            else if (Filename.length > 20) {
                ncUnits.alert("模块标题不能超过20字符!");
                return;
            }
            var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
            if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                ncUnits.alert("模块标题存在非法字符!");
                return;
            }
            if ($("#document-title").val() == "") {
                ncUnits.alert("模块标题输入为空!");
                return;
            }
            if ($("#document-maxRow").val() == "") {
                ncUnits.alert("显示条数输入为空!");
                return;
            }
             
            /*如果没有输入值，则出现提示框 结束 */
        }
        else if (rid == "baz4") {
            argus2.target.length = 0;//清空 argus2.target
            $("#Setup_modal .photochoose .img-rounded").each(function () {
                var imgargus = {
                    targetId: 1      //图片ID
                }
                imgargus.targetId = $(this).closest(".col-xs-3").find(".img-rounded").attr("id");
                argus2.target.push(imgargus);
            })
        }
        else if (rid == "baz5") {
            argus2.module.defaultLine = defaultLine;
            
            argus2.module.title = $("#efficiency-title").val();
            if ($(".fficiencyValue .displayTitle").is(":checked")) {
                argus2.module.displayTitle = 1;
            }
            else {
                argus2.module.displayTitle = 0;
            }
            argus2.module.defaultEfficiency = $("#defaultEfficiency").attr("term");
            argus2.module.topDisplay = $("#topselect").attr("term");
            argus2.module.topDisplayLine = $("#topDisplayLine").attr("term");
            if ($(".efficiencyValue #efficiencyValue").is(":checked")) {              
                argus2.module.efficiencyValue = 1;
            }
            else {         
                argus2.module.efficiencyValue = 0;
            }
            
            if ($(".efficiencyValue #executiveForce").is(":checked")) {
                argus2.module.executiveForce = 1;
            }
            else {
                argus2.module.executiveForce = 0;
            }
             
            if ($(".efficiencyValue #objective").is(":checked")) {
                argus2.module.objective = 1;
            }
            else {
                argus2.module.objective = 0;
            }
            
            /*如果没有输入值，则出现提示框 开始 修改*/
            var Filename = $.trim($("#efficiency-title").val());
            if (Filename == "") {
                ncUnits.alert("模块标题不能为空!");
                return;
            }
            else if (Filename.length > 20) {
                ncUnits.alert("模块标题不能超过20字符!");
                return;
            }
            var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
            if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                ncUnits.alert("模块标题存在非法字符!");
                return;
            }
            if (defaultLine == 0) {
                ncUnits.alert("默认显示曲线未选!");
                return;
            }
            if ($("#efficiency-title").val() == "") {
                ncUnits.alert("模块标题输入为空!");
                return;
            }           
            if ($("#defaultEfficiency").text() == "") {
                ncUnits.alert("默认功效未选!");
                return;
            }           
            if ($("#topselect").text() == "") {
                ncUnits.alert("TOP排名未选!");
                return;
            }           
            if ($("#topDisplayLine").text() == "") {
                ncUnits.alert("曲线显示未选!");
                return;
            }        
            /*如果没有输入值，则出现提示框 结束 修改*/
        }
        if (argus2.target.length <= 0) {
            if (argus2.module.type != 4) {
                ncUnits.alert("没有进行设置!");
            }
            else { ncUnits.alert("没有选择图片!"); }
            return;
        }
       
        $(".firstnewmodule").remove();

     //   console.log(argus2);
        if(argus2.module.moduleId==null){
            $.ajax({
                type: "post",
                url: "/IndexManagement/SaveIndexModule",
                dataType: "json",
                data: { data: JSON.stringify(argus2) },
                success: rsHandler(function () {
                    ncUnits.alert("新建成功!");
                    Loadmodule();///新建或更新成功后 根据数据库重新加载页面
                })
            });
        }
        else{
            $.ajax({
                type: "post",
                url: "/IndexManagement/SaveIndexModule",
                dataType: "json",
                data: { data: JSON.stringify(argus2) },
                success: rsHandler(function () {
                    ncUnits.alert("更新成功!");
                    Loadmodule();///新建或更新成功后 根据数据库重新加载页面
                })
            });
        }
        $("#Setup_modal .content").css('display', 'none');
        $("#Setup_modal").modal('hide');

    });
    
    //加载图片库图片
    function photoLoad() {
        var photo_lodi = getLoadingPosition("#photo_modal .photochoose");
        $.ajax({
            type: "post",
            url: "/NewsManagement/GetImageList",
            dataType: "json",
            complete: rcHandler(function () {
                photo_lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data){
                var  $photo=$("#photo_modal .photochoose");
                $photo.empty();
                $.each(data, function (i, v){                  
                    var $content =$("<div class='col-xs-3'></div>");
                    var $img =$("<img  class='img-rounded'src='"+ v.imageUrl + "'id='"+v.imageId+"'> </div>");
                    var $operate =$("<div class='operate'></div>");
                    var $operateDiv =$("<div class='operateDiv'></div>");
                    var $operateBg =$("<span class='operateBg'></span>");
                    var $operateText =$("<div class='operateText'></div>");
                    var $ul=$("<ul></ul>");
                    var $preview =$("<li class='checks'></li>");
                    var $delete =$("<li class='delete'></li>");
                    var $choose=$("<div class='xxc_choose choose' term='2' style='display:block;'><span ></span></div>")
                    $ul.append($delete, $preview);
                    $operateText.append($ul);
                    $operateDiv.append($operateBg,$operateText);
                    $operate.append($operateDiv);
                    $content.append($img,$operate,$choose);
                    $content.appendTo($photo);
                    $choose.click(function () {
                        if ($(this).hasClass('chooseHit')) {
                            // $(".choose").removeClass("chooseHit");
                            // $(".col-xs-3 span").removeClass("spanHit");
                            $(this).removeClass('chooseHit');
                            $('span', this).removeClass('spanHit');
                        }
                        else {
                            $(this).addClass('chooseHit');
                            $('span', this).addClass('spanHit');
                        }
                    });
                    /* 绿条hover效果开始 */
                    $content.hover(
                        function(){
                            $(this).find('.operate').css('display','block');
                            //当鼠标放上去的时候,程序处理
                        },
                        function(){
                            $(this).find('.operate').css('display','none');
                            //当鼠标离开的时候,程序处理
                        });
                    /* 绿条hover效果 结束 */
                    /* 点击绿条中的详情 预览图片  开始 */
                    $preview.click(function () {
                        $photoview = $("#photoviewModal");
                        $photoview.empty();
                        var $imgview = $("<img  class='view'src='" + v.imageUrl + "'> </div>");
                        $photoview.append($imgview);
                        $("#photoviewModal").modal('show');
                    });
                    /* 点击绿条中的详情 预览图片  结束 */
                    /* 删除 开始 */
                    $delete.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除这个图片吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                imageId = v.imageId;
                                //hai需要模块id   moduleId
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/DeleteImage",
                                    dataType: "json",
                                    data: { imageId: imageId },
                                    success: rsHandler(function (data) {
                                        photoLoad();//删除后，根据数据库重新加载页面
                                        ncUnits.alert("id:"+categoryId+"删除成功!");
                                    })
                                });
                            },
                            no:function(layer_confirm){      //修改
                                layer.close(layer_confirm);
                            }
                        });
                    });
                    /* 删除 结束 */
                })
            })
        });
    };
    //加载轮播图片的图片
    function IndexphotoLoad() {
        var $indexphoto = $("#Setup_modal .photochoose");
        $indexphoto.empty();
        $.each(photoarray, function (i, v) {
            var $content = $("<div class='col-xs-3'></div>");
            var $img = $("<img  class='img-rounded'src='" + v.imageUrl + "'id='" + v.imageId + "'> </div>");
            var $operate = $("<div class='operate'></div>");
            var $operateDiv = $("<div class='operateDiv'></div>");
            var $operateBg = $("<span class='operateBg'></span>");
            var $operateText = $("<div class='operateText'></div>");
            var $ul = $("<ul></ul>");
            var $check = $("<li class='checks'></li>");
            var $delete = $("<li class='delete'></li>");
            $ul.append($delete,$check);
            $operateText.append($ul);
            $operateDiv.append($operateBg, $operateText);
            $operate.append($operateDiv);
            $content.append($img, $operate);
            $content.appendTo($indexphoto);
            /* 绿条hover效果开始 */
            $content.hover(
                function () {
                    $(this).find('.operate').css('display', 'block');
                    //当鼠标放上去的时候,程序处理
                },
                function () {
                    $(this).find('.operate').css('display', 'none');
                    //当鼠标离开的时候,程序处理
                });
            /* 绿条hover效果 结束 */
            /* 点击绿条中的详情 预览图片  开始 */
            $check.click(function () {
                $photoview = $("#photoviewModal");
                $photoview.empty();
                var $imgview = $("<img  class='view'src='" + v.imageUrl + "'> </div>");
                $photoview.append($imgview);
                $("#photoviewModal").modal('show');
            });
            /* 点击绿条中的详情 预览图片  结束 */
            /* 删除 开始 */
            $delete.click(function () {
                if (photoarray.length <= 1) {
                    ncUnits.alert("已经是最后一张图片啦，不能删除");
                }
                else {
                    ncUnits.confirm({
                        title: '提示',
                        html: '确认要删除这个图片吗？',
                        yes: function (layer_confirm) {
                            layer.close(layer_confirm);
                            imageId = v.imageId;                          
                            for (var i = 0; i < photoarray.length; i++) {
                                if (photoarray[i].imageId == v.imageId) {
                                     
                                    photoarray.splice(i, 1);
                                }
                            }                           
                            IndexphotoLoad();//删除后，根据数组重新加载页面 
                            ncUnits.alert("删除成功!");
                        },
                        no: function (layer_confirm) {
                            layer.close(layer_confirm);
                        }
                    });
                }

            });
            /* 删除 结束 */
        })
    };
        
    function newmoduleclearSetModal() {
        $("#personal_modal_label").text("新建模块");//更换弹出框标题
        $("#baz1").prop("checked", true);//默认radio框选中到第一个
        //清除图库中的全部选中项
        photoarray.length = 0;//将原有的保存选中的图片数组清空
        $("#Setup_modal .photochoose").empty();
        //清除全部的价值激励类里面默认选中的defaultLine
        $("#defaultLine1").removeClass("defaultLine");
        $("#defaultLine2").removeClass("defaultLine");      
        $("#defaultLine3").removeClass("defaultLine");
        
        $("#news-title").val("");
        $(".modalmessage .displayTitle").prop("checked",false);
        $(".modalmessage .maxRow").val("");
        $("#linkTarget").attr('term', "").text("");
        $("#document-title").val("");
        $(".modaldocument .displayTitle").prop("checked", false);
        $("#document-maxRow").val("");
        $("#efficiency-title").val("");
        $("#defaultEfficiency").attr('term', "").text("");
        $("#topselect").attr('term', "").text("");
        $("#topDisplayLine").attr('term', "").text("");
        $("#efficiencyValue").prop("checked", false);
        $("#executiveForce").prop("checked", false);
        $("#objective").prop("checked", false);               
        $("#defaultLine1").css("display", "none");
        $("#defaultLine2").css("display", "none");
        $("#defaultLine3").css("display", "none");

    }

    function newModuleRadioChange() {
        //清除图库中的全部选中项
        photoarray.length = 0;//将原有的保存选中的图片数组清空
        $("#Setup_modal .photochoose").empty();
        //清除全部的价值激励类里面默认选中的defaultLine
        $("#defaultLine1").removeClass("defaultLine");
        $("#defaultLine2").removeClass("defaultLine");
        $("#defaultLine3").removeClass("defaultLine");

        $("#news-title").val("");
        $(".modalmessage .displayTitle").prop("checked", false);
        $(".modalmessage .maxRow").val("");
        $("#linkTarget").attr('term', "").text("");
        $("#document-title").val("");
        $(".modaldocument .displayTitle").prop("checked", false);
        $("#document-maxRow").val("");
        $("#efficiency-title").val("");
        $("#defaultEfficiency").attr('term', "").text("");
        $("#topselect").attr('term', "").text("");
        $("#topDisplayLine").attr('term', "").text("");
        $("#efficiencyValue").prop("checked", false);
        $("#executiveForce").prop("checked", false);
        $("#objective").prop("checked", false);
        $("#defaultLine1").css("display", "none");
        $("#defaultLine2").css("display", "none");
        $("#defaultLine3").css("display", "none");

    }
    //点击选择图片时根据数组内的数据进行勾选
    $(".choosephoto").click(function () {
        $("#photo_modal .xxc_choose").each(function () {
            if ($(this).hasClass('chooseHit')) {
                $(this).removeClass('chooseHit');
                $('span', this).removeClass('spanHit');
            }

        })//将选中项初始化全部取消
        $.each(photoarray, function (i, v) {
            $("#photo_modal .img-rounded").each(function () {
                if ($(this).attr("id") == v.imageId) {
                    $(this).siblings(".xxc_choose").addClass('chooseHit');
                    $('span', $(this).siblings(".xxc_choose")).addClass('spanHit');
                }
                
            })
        })
        $("#photo_modal").modal('show');
    })
   
    $("#efficiencyValue").click(function () {
        if ($(this).is(":checked")) {
            $("#defaultLine1").css("display", "inline");        
        }
        else {
            $("#defaultLine1").css("display", "none");
        }

    })
    $("#executiveForce").click(function () {
        if ($(this).is(":checked")) {
            $("#defaultLine2").css("display", "inline");
        }
        else {
            $("#defaultLine2").css("display", "none");
        }

    })
    $("#objective").click(function () {
        if ($(this).is(":checked")) {
            $("#defaultLine3").css("display", "inline");
        }
        else {
            $("#defaultLine3").css("display", "none");
        }

    })

    $("#defaultLine1").click(function () {
        defaultLine = 1;
        $("#defaultLine1").addClass("defaultLine");
        $("#defaultLine2").removeClass("defaultLine");
        $("#defaultLine3").removeClass("defaultLine");
    })
    $("#defaultLine2").click(function () {
        defaultLine = 2;
        $("#defaultLine2").addClass("defaultLine");
        $("#defaultLine1").removeClass("defaultLine");
        $("#defaultLine3").removeClass("defaultLine");
    })
    $("#defaultLine3").click(function () {
        defaultLine = 3;
        $("#defaultLine3").addClass("defaultLine");
        $("#defaultLine1").removeClass("defaultLine");
        $("#defaultLine2").removeClass("defaultLine");
    })
    //控制价值激励模块的默认按钮的显示和隐藏 结束
    //下拉列表事件
    // $('.orgType-dropdown li').off("click");
  
    $('.orgType-dropdown li').click(function () {
       // $(this).parent().prev().children().last().removeClass("upPicHit");           //改变下拉图片
        var type = $(this).find('a').attr('term');
        var text = $(this).find('a').text();
        $(this).parent().prev().children().first().attr('term', type).text(text);
    });
});