//@ sourceURL=organizationManage.js
/**
 * Created by DELL on 15-7-17.
 */

$(function() {

 
    //---------------初始化 开始-----------//
    //组织类型数组
    var schemaNames = ["集团","公司/产业","部门", "科室"];
    schemaNames[8]="其他";
    var cycleDate = ["日", "周", "月", "年"];        //循环周期
    var weekArray = ["星期一", "星期二", "星期三", "星期四", "星期五", "星期六","星期日"]
 
    var parentOrganization = '',            //上级组织ID 默认无
          parentJob = null,                     //上级岗位ID
          organizationName,                     //岗位的组织名称
          parentStationName,                   //上级岗位名  
          rightOrgId = null,              //存储右边目录选取的组织ID
          rightOrgName = null,              //存储右边目录选取的组织Name
          JoborgTree = false;
 

    var orgOrJob = 1;             //orgOrJob=1表示架构操作,orgOrJob=2表示岗位操作,默认为架构操作
    var orgOrJobForTree = 1;          //仅仅用于点击目录时判断刷新的列表,只能通过  架构/岗位的tab 来改变
    var idList=[],
        dataLength,
        info,
        editMode = 0;

    //组织架构列表请求所用参数
    var argus = {
        name:String,         //组织名称
        parentId:Number     //父组织ID
    };
    argus.name = '';
    argus.parentId = null;


    //组织架构排序所用参数
    var argus_order=new Array();


    //岗位列表请求所用参数
    var argus_job = {
        name:String,         //名称
        orgId:Number     //组织ID
    };
    argus_job.name='';
    argus_job.orgId=null;


    //新建\更新组织架构所用参数
    var argus_newOrg = {
        organizationId : Number,       //组织ID
        parentOrganization : String,        //父组织ID
        schemaName : Number,                //组织类型
        organizationName : String,          //组织名
        withSub : Number,                    //有子组织
        orderNumber : Number,                //排序
        description : String                 //描述
    }



    //新建\更新岗位所用参数
    var argus_newJob = {
            stationId:Number,       //岗位ID
            parentStation:Number,   //上级岗位ID
            stationName:String,  //岗位名称
            organizationId:Number,  //组织ID
            comment:String,      //描述
            approval:Number,        //流程审批人
            skipApproval:Number     //越级审批
    }


    //-------------初始化 结束----------------//



    //--------页面初始加载 开始--------------//

    //加载组织架构列表
    loadOrganizationList();



    //加载右侧菜单列表
    treeRight();

    //--------页面初始加载 结束--------------//






    //----------------菜单脚本 开始--------------//

   $("#orgTab").click(function(){
       orgOrJob = 1;
       argus.parentId = rightOrgId;    //初始化架构列表加载的参数
       argus.name='';
       $("#search_input").val("");
       $("#search_input").addClass("invisible");
       $(".menuSearch").removeClass("menuClick");
       orgOrJobForTree = 1;
       loadOrganizationList();
   });

   $("#jobTab").click(function () {
       console.log("jobTab");
       orgOrJob = 2;
       argus_job.orgId = rightOrgId;  //初始化岗位列表加载的参数
       argus_job.name='';
       $("#search_input").val("");
       $("#search_input").addClass("invisible");
       $(".menuSearch").removeClass("menuClick");
       orgOrJobForTree = 2;
       loadingJobList();
   });




    //点击搜索按钮事件

    $(".menuSearch").click(function(){
        if( $("#search_input").hasClass("invisible") ){            //如果搜索框是隐藏的
            $("#search_input").val("");
            $("#search_input").removeClass("invisible");
            $(this).addClass("menuClick");
        }
        else{
            var searchText = $.trim($("#search_input").val());  
            if (orgOrJob == 1) {        //如果是架构搜索操作
                argus.name = searchText;
                argus.parentId = "";
                loadOrganizationList();
                if ($(".menuSearch").hasClass("menuClick")) {            //如果搜索框是隐藏的
                    //$(".menuSearch").val("");
                    $(".menuSearch").removeClass("menuClick");
                    $("#search_input").addClass("invisible");
                }
            }
            else {                  //如果是岗位搜索操作
                argus_job.name = searchText;
                argus_job.orgId = "";
                loadingJobList();
                if ($(".menuSearch").hasClass("menuClick")) {            //如果搜索框是隐藏的
                    //$(".search_input").val("");
                    $(".menuSearch").removeClass("menuClick");
                    $("#search_input").addClass("invisible");
                }
            }         
        }
    });


    //点击+号事件
    $(".menuAdd").click(function(){
        editMode=0;
        if(orgOrJob==1){                //如果选择了架构操作
            editOrgInfo();
        }
        else{                           //选择了岗位操作
            editJobInfo();
        }
    });


    //点击+号 弹出模态框单选按钮事件
    $("#new_modal input[type='radio']").click(function(){
        if( this.id=="orgNew" ){
            orgOrJob = 1;               //架构
            $("#newOrgOrJob").empty();
            $("#newOrgOrJob").load("/OrganizationManagement/GetNewOrg", function () {
                drawDowns();
                if (parentOrganization == null) {        //选择无 浏览组织架构
                    $("#orgParentName").text("无");
                } else {
                    parentOrganization = rightOrgId;
                    if (rightOrgName == null)
                        $("#orgParentName").text("无");
                    else
                        $("#orgParentName").text(rightOrgName);
                }
            });

        }
        if( this.id == "jobNew" ){
            orgOrJob = 2;                //岗位
            $("#newOrgOrJob").empty();
            $("#newOrgOrJob").load("/OrganizationManagement/GetNewJob", function () {
                drawDowns();
                if (parentOrganization == null) {        //选择无 浏览组织架构时
                    $("#orgParentId").val("");
                } else {
                    parentOrganization = rightOrgId;
                    if (rightOrgName == null)
                        $("#orgParentId").val("");
                    else
                        $("#orgParentId").val(rightOrgName);
                }
            });
        }
   });


    //点击批量删除事件
    $(".menuDel").off("click");
    $(".menuDel").click(function(){
        idList.length=0;
        var i =0 ,withSubS = 0,urls;
        var objArray;
        if(orgOrJob==1)
            objArray=$(".orgCheck");
        else
            objArray=$(".jobCheck");

        objArray.each(function(){
            if( $(this).prop("checked") ){          
                idList[i++] = $(this).val();
            }
        });
        if (idList.length != 0) {
            if (orgOrJob == 1) {            //如果是组织架构的批量删除
                var confirmInfo = '确定要删除所选' + idList.length + '个组织？';
                urls = "/OrganizationManagement/DeleteOrganization";
                var confirmText = "架构有子组织或岗位，无法删除!";
            } else if (orgOrJob == 2) {            //如果是岗位的批量删除
                var confirmInfo = '确定要删除所选' + idList.length + '个岗位？';
                urls = "/OrganizationManagement/DeleteStation";
                var confirmText = "存在人员或下级岗位，无法删除!";
            }
            ncUnits.confirm({
                title: '提示',
                html: confirmInfo,
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: urls,
                        dataType: "json",
                        data: { data: JSON.stringify(idList) },
                        success: rsHandler(function (data) {
                            if (data == "1") {
                                ncUnits.alert(confirmText);
                                if (orgOrJob == 1)
                                    loadOrganizationList();
                                else
                                    loadingJobList();
                            } else if (data == "2") {
                                ncUnits.alert(confirmText);
                                if (orgOrJob == 1)
                                    loadOrganizationList();
                                else
                                    loadingJobList();
                            } else {
                                ncUnits.alert("删除成功!");
                                if (orgOrJob == 1) {
                                    loadOrganizationList();
                                    treeRight();           //刷新右边菜单 
                                }
                                else
                                    loadingJobList();
                            }

                        })
                    });
                }
            });
        } else {
            if (orgOrJobForTree == 1)
                ncUnits.alert("请选择要删除的组织架构!");
            else
                ncUnits.alert("请选择要删除的岗位!");
        }

    });


    //----------------菜单脚本 结束--------------//






    //----------------目录脚本 开始--------------//

    //组织架构分类的收缩展开
    $(".directory-set a").click(function () {
        parentOrganization = null;
        rightOrgId = null,              //存储右边目录选取的组织ID
        rightOrgName = null;              //存储右边目录选取的组织Name

        $("#rightTree .curSelectedNode").removeClass("curSelectedNode");       //修改树结构颜色

        if (orgOrJob == 1) {
            argus.name = '';
             argus.parentId = null;
            loadOrganizationList();

        } else if (orgOrJob == 2) {
            argus_job.name = '';
            argus_job.orgId = null;
            loadingJobList();
        }
    });

    $(".directory-set .arrowDown").click(function () {
        if ($(this).hasClass("glyphicon-chevron-down")) {
            $(this).removeClass("glyphicon-chevron-down");
            $(this).addClass("glyphicon-chevron-up");
            $(".ztree").slideDown();
        } else {
            $(this).addClass("glyphicon-chevron-down");
            $(this).removeClass("glyphicon-chevron-up");
            $(".ztree").slideUp();
        }
    });

    //右侧目录加载
    function treeRight() {
        //rightOrgId = null;
        //rightOrgName = null;
        var folderTree;
        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetOrganizationList",
            dataType: "json",
            data: { parent: null },
            success: rsHandler(function (data) {
                    folderTree = $.fn.zTree.init($("#rightTree"), $.extend({
                    callback: { 
                        onClick: function (e, id, node) {
                            $("#search_input").val("");
                            $("#search_input").addClass("invisible");
                            $(".menuSearch").removeClass("menuClick");
                            rightOrgId = node.id;              //存储右边目录点击ID
                            rightOrgName = node.name;                //存储右边目录点击组织的Name
                            parentOrganization = node.id;
                            if (orgOrJobForTree == 1) {        //如果是架构搜索操作
                                 argus.parentId = node.id;           //加载组织架构列表参数赋值
                                 argus.name = "";
                                loadOrganizationList();
                            }
                            else{                  //如果是岗位搜索操作
                                 argus_job.orgId = node.id;
                                 argus_job.name = "";
                                loadingJobList();
                            }
                        },
                        onNodeCreated: function (event, treeId, treeNode) {
                            var ztree = $.fn.zTree.getZTreeObj("rightTree");
                            if ( rightOrgId!=null && treeNode.id == rightOrgId) {
                                var n = ztree.getNodeByParam("id", rightOrgId);
                                ztree.selectNode(n);
                            }
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    async: {
                        enable: true,
                        url: "/OrganizationManagement/GetOrganizationList",
                        autoParam: ["id=parent"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }


    //弹出框组织架构菜单的加载
    function treeOrgLoad() {
        var orgId = null;
        if ( editMode == 1 && orgOrJob == 1 && info!=null ) {        //只在编辑架构时 传递organizationId
            orgId = info.organizationId;
        }
        
        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: orgId },             //去除死循环
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($(".ztreeOrg"), $.extend({
                    callback: {
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {        //选中事件
                            if (node.checked) {
                                if (JoborgTree) {
                                    appendJob(node.id);
                                }
                                else {
                                    parentOrganization = node.id;
                                    if (orgOrJob == 1) {    //如果是架构
                                        $("#orgParentName").text(node.name);
                                    }
                                    else if (orgOrJob == 2) {            //岗位的所属架构
                                        $("#orgParentId").val(node.name);
                                    }
                                }   
                            }
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    check: {            //设置为单选按钮
                        enable: true,
                        chkStyle: "radio",
                        radioType: "all",
                        chkboxType: { "Y": "", "N": "" }
                    },

                    async: {
                        enable: true,
                        url: "/OrganizationManagement/GetOrganizationList",
                        autoParam: ["id=parent"], 
                        otherParam: ["organizationId", orgId],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }
  
    //浏览上级岗位
    function appendJob(id) {
        var staId = null;
        if (info != null) {
            staId = info.stationId;
        }
        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetStationList",
            dataType: "json",
            data: { name: "", organizationId: id, stationId: staId },         //去除死循环
            success: rsHandler(function (data) {
                $("#jobListUl").empty();
                $.each(data, function (i, v) {
                        var $li = $("<li></li>");
                        var $label = $("<label class='radio'></label>")
                        var $radio = $("<input type='radio' name='parentJob' value=" + v.stationId + ">");
                        var $span = $("<span title=" + v.stationName + " >" + v.stationName + "</span>");
                        $radio.click(function () {
                            $(".treeNames").text($(this).next().text());
                            parentJob = $(this).val();
                        })
                        $label.append([$radio, $span])
                        $li.append($label);
                        $("#jobListUl").append($li);                  
                })
            })
        });
    }
    

    //----------------目录脚本 结束--------------//






    //-----------------架构脚本 开始-------------------//

    /*加载组织架构列表 开始*/
    function loadOrganizationList() { 
        var $orgTable = $(".orgTable");
        $("#orgOrder").hide();
        $orgTable.empty();
        var lodi = getLoadingPosition($orgTable);//显示load层
        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetOrganizationListFiest",
            dataType: "json",
            data: { name: argus.name, parentId: argus.parentId },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $orgTable.empty();
           
                dataLength = data.length;
                if (dataLength == 0) {            //如果没有数据
                    var $noorg = $("<div class='noOrgView'></div>");
                    var $spanOne = $("<span>还没有任何架构，点击&nbsp;&nbsp;</span>");
                    var $spanTwo = $("<span>&nbsp;&nbsp;添加</span>");
                    var $addBig = $("<span class='add_big' data-toggle='modal' data-target='#new_modal'></span>");
                    $addBig.click(function () {
                        editMode = 0;
                        editOrgInfo();
                    });
                    $noorg.append([$spanOne,$addBig,$spanTwo]);
                    if ($("#orgContent").children('.noOrgView').length > 0) {
                        $("#orgContent .noOrgView").remove();
                    }                 
                    $("#orgContent").append($noorg);                  
                }
                else {
                    $("#orgContent .noOrgView").remove();
                    //console.log("架构加载参数：parentId= " + argus.parentId + " name=" + argus.name);                  //检测参数

                    $("#orgOrder").show();
                    var orgId = [];
                    var $head = $("<thead class='colorGray'></thead>");
                    var $title=$("<tr></tr>");
                    var $tCb = $("<th><input id='selectAll' type='checkbox'/></th>");
                    var $tOrgName =$("<th><span>组织名称</span></th>");
                    var $tSchema =$("<th><span>组织类型</span></th>");
                    var $tParentOrg =$("<th><span>上级组织</span></th>");
                    var $tOrgDes =$("<th><span>组织描述</span></th>");
                    var $tOperate =$("<th><span>操作选项</span></th>");
                    $title.append([$tCb,$tOrgName,$tSchema,$tParentOrg,$tOrgDes,$tOperate]);
                    $title.appendTo($head);
                    $head.appendTo($orgTable);
                    var $body =$("<tbody></tbody>");
                    $body.appendTo($orgTable);

                    $("#selectAll").click(function(){
                        selectAll(this);
                    });

                    $.each(data, function (i, v) {
                        var $tr = $("<tr></tr>");
                        var $cb = $("<td style='width:4%' ><input class='orgCheck' term=" + v.withSub + "  value=" + v.organizationId + " type='checkbox'/></td>");
                        var $orgName = $("<td style='width:16%' ><span   title=" + v.organizationName + ">" + v.organizationName + "</span></td>");
                        var $schema = $("<td style='width:16%' ><span>" + schemaNames[v.schemaName] + "</span></td>");
                        var $parentOrg = $("<td style='width:16%' ><span title="+v.parentName+">" + (v.parentName == null ? "" : v.parentName) + "</span></td>");
                        var $description = $("<td style='width:24%'><span   title=" + v.description + " >" + (v.description == null ? "" : v.description) + "</span></td>");
                        var $operate = $("<td style='width:25%'></td>");
                        var $edit = $("<a href='#' data-toggle='modal' data-target='#newOrg_modal'>编辑</a>");
                        var $addChild = $("<a href='#' data-toggle='modal' data-target='#newOrg_modal'>&nbsp;&nbsp;&nbsp;添加下级组织</a>");
                        var $delete = $("<a href='#'>&nbsp;&nbsp;&nbsp;删除</a>");

                       
                      //  $("[data-toggle='tooltip']").tooltip({ background: 'white' });

                        $operate.append([$edit,$addChild,$delete]);
                        $tr.append([$cb,$orgName,$schema,$parentOrg,$description,$operate]);
                        $tr.appendTo($body);


                        //复选框与全选按钮
                        $cb.find("input").click(function () {
                            var checkFlag = $(this).prop("checked");
                            if (checkFlag) {
                                var flag = true;
                                var checks = $("#orgContent input:checkbox");
                                checks.each( function (i,v) {
                                    if ( i>0 && !$(this).prop("checked") ) {
                                        flag = false;
                                    }
                                });  
                                if (flag) {
                                    $("#selectAll").prop("checked",true);
                                }
                            }
                            else {
                                $("#selectAll").prop("checked",false);
                            }
                        });

                        $edit.click(function(){
                            editMode =1;
                            info =v;
                            editOrgInfo();
                        });

                        $addChild.click(function(){
                            //TODO
                            editMode =2;
                            info =v;
                            editOrgInfo();
                        });

                        $delete.click(function(){                   //删除事件
                            var confirmInfo = '确认要删除？';
                            //if(v.withSub){
                            //    confirmInfo = '该组织有子组织,不可删除';
                            //}
                            ncUnits.confirm({
                                title: '提示',
                                html: confirmInfo,
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    idList=[v.organizationId];
                                    $.ajax({
                                        type: "post",
                                        url: "/OrganizationManagement/DeleteOrganization",
                                        dataType: "json",
                                        data: { data: JSON.stringify(idList) },
                                        success: rsHandler(function (data) {
                                            if (data == "1") {
                                                ncUnits.alert("组织架构下有子组织，无法删除!");          
                                            } else if (data == "2") {
                                                ncUnits.alert("组织架构下有岗位，无法删除!");
                                            } else {
                                                ncUnits.alert("删除成功!");
                                                loadOrganizationList();
                                                treeRight();           //刷新右边菜单
                                            }                                         
                                        })
                                    });
                                }
                            });
                        });
                    });


                    //右边目录的高度
                    var x = $("#aaa").height();
                    $("#menuTreeHeight").css({
                        "height": x
                    });
                }
            })
        });
    }


    //全选按钮事
    function selectAll(value){
        if (value.checked) {
            $("input:checkbox").prop("checked", true);
        } else {
            $("input:checkbox").prop("checked", false);
        }
    }

    //点击排序在弹窗中加载数据
    $("#orgOrder").off("click");
    $("#orgOrder").click(function(){
        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetOrganizationList",
            dataType: "json",
            data: { name: argus.name, parentId: argus.parentId },
            success: rsHandler(function (data) {
                var $orderList = $(".row-orderList");
                $orderList.empty();

                $.each(data, function (i, v) {
                    var $row = $("<div class='row  orderListOne'></div>");
                    var $orgName = $("<div class='col-xs-8'><span class='orderNum'>" + (i + 1) + "</span><span class='orderOrgName' title=" + v.name + " term=" + v.id + ">" + v.name + "</span></div>");
                    var $orgOrderMethod = $("<div class='orderMethod  pull-right'>" +
                        "<div><a href='#' class='up'></a>" +
                        "<a href='#' class='downs'></a>" +
                        "<a href='#' class='upTop'>顶部</a>" +
                        "<a href='#' class='downBottom' >底部</a></div></div>");
                    $row.append($orgName).append($orgOrderMethod);
                    $row.appendTo($orderList);
                });

                $("a.up").click(function(){
                    var obj1 = $(this).parent().parent().prev().children().last();
                    var obj2 = $(this).parent().parent().parent().prev().find(".orderOrgName");
                    var txt = obj1.text();
                    var id = obj1.attr("term");
                    obj1.attr("term",obj2.attr("term"));
                    obj1.text(obj2.text());
                    obj2.text(txt);
                    obj2.attr("term",id);
                });

                $("a.downs").click(function(){
                    var obj1 = $(this).parent().parent().prev().children().last();
                    var obj2 = $(this).parent().parent().parent().next().find(".orderOrgName");
                    var txt = obj1.text();
                    var id = obj1.attr("term");
                    obj1.attr("term",obj2.attr("term"));
                    obj1.text(obj2.text());
                    obj2.text(txt);
                    obj2.attr("term",id);
                });

                $("a.upTop").click(function(){
                    var obj1 = $(this).parent().parent().prev().children().last();
                    var obj2 =  $(".row-orderList").children().eq(0).find(".orderOrgName");
                    var txt = obj1.text();
                    var id = obj1.attr("term");
                    obj1.attr("term",obj2.attr("term"));
                    obj1.text(obj2.text());
                    obj2.text(txt);
                    obj2.attr("term",id);
                });

                $("a.downBottom").click(function(){
                    var obj1 = $(this).parent().parent().prev().children().last();
                    var obj2 =  $(".row-orderList").children().eq(data.length-1).find(".orderOrgName");
                    var txt = obj1.text();
                    var id = obj1.attr("term");
                    obj1.attr("term",obj2.attr("term"));
                    obj1.text(obj2.text());
                    obj2.text(txt);
                    obj2.attr("term",id);
                });

                $(".orderListOne").eq(0).find(".up").off("click");
                $(".orderListOne").eq(0).find(".upTop").off("click");
                $(".orderListOne").eq(data.length-1).find(".downs").off("click");
                $(".orderListOne").eq(data.length-1).find(".downBottom").off("click");

            })
        });

        //确定
        $('#order_modal_submit').off("click");
        $('#order_modal_submit').click(function () {
            var obj;
            argus_order.length = 0;
            $(".orderListOne").each(function(i,v){
                obj= $(v).find(".orderOrgName");
                var argorder = {
                    organizationId:Number,
                    orderNumber:Number
                };
                argorder.organizationId = obj.attr("term");         //组织的Id
                argorder.orderNumber = i+1;                          //组织的顺序
                argus_order[i] = argorder;
            });
            $.ajax({
                type: "post",
                url: "/OrganizationManagement/OrderOrganization",
                dataType: "json",
                data: {data:JSON.stringify(argus_order)},
                success: rsHandler(function(data){
                    console.log(argus_order);              //参数检查
                    $("#order_modal").modal("hide");
                    loadOrganizationList();            //按照原有的输入参数刷新组织架构列表
                    treeRight();
                }),
                complete:function() {
                    console.log("complete");
                }
            });

        });

    });



    //判断某变量是否具有非法字符
    function justifyByLetter(txt,name) {
        var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            ncUnits.alert(name);
            return false;
        }
        return true;
    }


    /*新建 更新组织架构事件*/
    function editOrgInfo() {

        //页面预处理部分
        $(".newOrg").empty();
        $("#newOrgOrJob").empty();
        $(".newJob").empty();

        if(editMode ==0){               //如果是新建 使单选框按钮选中架构 在新建模态框中加载新建架构页面
            $("#orgNew").prop("checked", true);
            $("#newOrgOrJob").load("/OrganizationManagement/GetNewOrg", function () {
                drawDowns();
                if (parentOrganization == null) {        //选择无 浏览组织架构
                    $("#orgParentName").text("无");
                } else {
                    parentOrganization = rightOrgId;
                    if (rightOrgName == null)
                        $("#orgParentName").text("无");
                    else
                        $("#orgParentName").text(rightOrgName);
                }
            });

        }else if(editMode==1 ){            //如果是编辑  在编辑模态框中加载新建架构页面  初始化值
            $(".newOrg").load("/OrganizationManagement/GetNewOrg", function () {
                drawDowns();
                var type = info.schemaName;
                $('.span-orgType span:eq(0)').attr("term",type+1).text(schemaNames[type]);
                $("#input_orgName").val(info.organizationName);
                $("#input_orgDes").val(info.description);
                if (info.parentOrganization != null) {
                    $("#orgParentName").text(info.parentName);
                    parentOrganization = info.parentOrganization;
                } else {
                    $("#orgParentName").text("无");
                    parentOrganization = null;
                }
            });
        } else if (editMode == 2) {          //如果是添加下级组织
         
            $(".newOrg").load("/OrganizationManagement/GetNewOrg", function () {
                drawDowns();
                if (info.organizationName != null) {
                    $("#orgParentName").text(info.organizationName);
                    parentOrganization = info.organizationId;
                } else {
                    $("#orgParentName").text("无");
                    parentOrganization = null;
                }
            });
        }

        $("#newOrg_modal_insert").off("click");
        $("#newOrg_modal_insert").click(function () {
            orgNewSubmit(this);
        });

        $("#new_modal_insert").off("click");
        $("#new_modal_insert").click(function(){
            if(orgOrJob==1){
                orgNewSubmit(this);
            }
            else{
                jobNewSubmit(this);
            }

        });

    }


    //更新\编辑组织架构提交按钮事件
    function orgNewSubmit(value) {

        var input_orgName = $.trim( $('#input_orgName').val() );
        if (input_orgName == "") {
            ncUnits.alert("组织架构名不能为空!");
            $('#input_orgName').focus();
            return;
        }
        else if (input_orgName.length > 50) {
            ncUnits.alert("组织架构名不能超过50字符!");
            $('#input_orgName').focus();
            return;
        }
        if ( justifyByLetter(input_orgName, '组织架构名')==false ) {
            $('#input_orgName').focus();
            return;
        }

        var input_orgDes = $.trim( $("#input_orgDes").val() );
        if (justifyByLetter(input_orgDes, '组织描述') == false) {         
            $("#input_orgDes").focus();
            return;
        }
        if (input_orgDes.length > 100 ) {
            ncUnits.alert("描述不能超过100字符!");
            $('#input_orgDes').focus();
            return;
        }


        var modelHidden;
        //获取参数
        if( editMode==1){                     //如果是编辑事件
            argus_newOrg.organizationId = info.organizationId;
            argus_newOrg.withSub = info.withSub;
           // argus_newOrg.orderNumber=[];
            modelHidden = $("#newOrg_modal");
        } else if (editMode == 2) {
            argus_newOrg.organizationId = null;
            argus_newOrg.withSub = false;
            // argus_newOrg.orderNumber=[];
            modelHidden = $("#newOrg_modal");
        }
        else if( editMode==0 ){                  //如果是新建事件
            argus_newOrg.organizationId = null;
            argus_newOrg.withSub = false;
            argus_newOrg.orderNumber= dataLength+1;
            modelHidden = $("#new_modal");
        }
        argus_newOrg.parentOrganization = parentOrganization;
        argus_newOrg.schemaName= $('.span-orgType span:eq(0)').attr('term')-1;
        argus_newOrg.organizationName = $('#input_orgName').val();
        argus_newOrg.description =$('#input_orgDes').val();

        $.ajax({
            type: "post",
            url: "/OrganizationManagement/SaveOrganization",
            dataType: "json",
            data: { data: JSON.stringify(argus_newOrg)},
            success: rsHandler(function (data) {
                if (editMode == 0 || editMode ==2 ) {
                    ncUnits.alert("组织架构添加成功");
                }
                else if (editMode == 1) {
                    ncUnits.alert("组织架构编辑成功");
                }
            
                var x = "架构ID:"+argus_newOrg.organizationId +" 上级架构Id:"+argus_newOrg.parentOrganization +" schemaName"+argus_newOrg.schemaName +
                    " organizationName "+argus_newOrg.organizationName+" description "+argus_newOrg.description +" orderNumber "+argus_newOrg.orderNumber+" withSub "+argus_newOrg.withSub;
                console.log(x);
                modelHidden.modal("hide");
                loadOrganizationList();
                treeRight();           //刷新右边菜单
            }),
            complete:function() {
                console.log("complete");
            }
        });
    }


    //模态框中下拉框事件函数
    function drawDowns() {

        $("span.dropdown-toggle").off("click");
        $("span.dropdown-toggle").click(function(){
            $(this).find(".upPic").toggleClass("upPicHit");
            $(".browse").removeClass("flowPic_hit");              //图片样式
            $(".browse").parent().removeClass("border_hit");
            $(".browse").parent().prev().removeClass("border_rHit");
        });

        //下拉列表事件
        $('.orgType-dropdown li').off("click");
        $('.orgType-dropdown li').click(function () {
            $(this).parent().prev().children().last().removeClass("upPicHit");           //改变下拉图片
            var type = $(this).find('a').attr('term');
            var text = $(this).find('a').text();
            $(this).parent().prev().children().first().attr('term', type).text(text);
        });

        //浏览组织结构点击触发事件
        $(".browse").click(function () {
            if (orgOrJob == 1) {
                parentOrganization = null;                    //没有选择组织架构,则默认组织架构为空
            }
            $(this).addClass("flowPic_hit");              //组织架构图片样式
            $(this).parent().addClass("border_hit");
            $(this).parent().prev().addClass("border_rHit");

            JoborgTree = false;
            var x = $(this).parents('.modal-dialog').width()-5;
            $(".browseTree").css({
                "left":x
            });
            var treeOrg = "<h4>组织架构</h4><hr class='hr' style='margin-bottom:0px'/><div class='ztree ztreeOrg' style='padding-top:10px'></div>";
            var obj = $(this).parents('.modal-body').nextAll(".browseTree");
            obj.empty();
            obj.append(treeOrg);
            treeOrgLoad();
            obj.show();
        });

        //浏览岗位结构点击触发事件
        $(".browseJob").off("click");
        $(".browseJob").click(function () {
            JoborgTree = true;
            var x = $(this).parents('.modal-dialog').width()-5;
            $(".browseTree").css({
                "left":x
            });
            var $title = $("<h4>上级岗位</h4><hr class='hr' style='margin-bottom:0px' />");
            var $content = $("<div style='background-color:#fff;'></div>");
            var $treeOrg = $("<div class='ztree ztreeOrg' style='padding-top:10px; height:200px;overflow-y:auto; margin-left: -10px' ></div>");
            var $jobList = $("<p style='line-height: 30px; margin-bottom: 0px;'>岗位列表</p><hr class='hr' style='margin-bottom:0px;margin-top:0px'/><ul id='jobListUl' ></ul>");
            var obj = $(this).parents('.modal-content').find(".browseTree");
            obj.empty();
            $content.append([$treeOrg, $jobList]);
            obj.append([$title,$content]);
            treeOrgLoad()
            obj.show();
        });    

        //点击下拉框架构无事件
        $(".noOrgParent").click(function(){
            parentOrganization = null;
        });

        //点击下拉框岗位无事件
        $(".noJobParent").click(function(){
            parentJob = null;
        });

    }


    //组织架构模态框的关闭事件
    $('#newOrg_modal').on('hide.bs.modal', function () {
        clearMenu(this);
    });
     
    //岗位模态框的关闭事件
    $('#newJob_modal').on('hide.bs.modal', function () {    
        clearMenu(this);
 
    });

    //新建模态框的关闭事件
    $('#new_modal').on('hide.bs.modal', function () {
        clearMenu(this);
    });

    //清除弹出的菜单
    function clearMenu(value) {
        parentOrganization = '';      //清除变量parentOrganization,即获得的nodeId值
        parentJob = null               //清除变量parentOrganization,即获得的nodeId值
        var obj = $(value).find(".browseTree");
        obj.empty();
        obj.hide();
    }

//-----------------架构脚本 结束-------------------//






//------------------岗位脚本开始--------------//
    //加载岗位函数
    function loadingJobList() {    
        $("#jobContent").empty();
        $("#loadding").removeClass("hidden");         //显示load的固定div
        var lodi = getLoadingPosition($("#loadding"));    //显示load层  
        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetStationList",
            dataType: "json",
            data: { name: argus_job.name, organizationId: argus_job.orgId },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $("#jobContent").empty();
                $("#loadding").addClass("hidden");
                console.log ("加载岗位列表参数:name="+argus_job.name+"  organizationId="+argus_job.orgId) ;             //输出检查

                if(data.length==0){            //如果没有数据
                    var $nojob = $("<div class='noJobView' style='margin:0 auto'></div>");
                    var $spanOne = $("<span>还没有任何岗位，点击&nbsp;&nbsp;</span>");
                    var $spanTwo = $("<span>&nbsp;&nbsp;添加</span>");
                    var $add_jobBigPic = $("<span class='add_big'  data-toggle='modal' data-target='#new_modal'></span>");
                    $add_jobBigPic.click(function () {
                        editMode = 0;
                        editJobInfo();
                    });
                    $nojob.append([$spanOne, $add_jobBigPic, $spanTwo]);
                    $("#jobContent").empty().append($nojob);
                }
                else {
                    var $jobTable = $("<table class='table table-hover jobTable'  role='tabpanel'></table>");
                    $("#jobContent").empty().append($jobTable);

                    var jobId = [];
                    var $head = $("<thead class='colorGray'></thead>");
                    var $title=$("<tr><th><input id='selectAllJob' type='checkbox'/></th>" +
                        "<th><span>岗位名称</span></th><th><span>所属组织</span></th><th><span>上级岗位</span></th>" +
                        "<th><span>负责人</span></th><th><span>岗位人员</span></th><th><span>操作选项</span></th></tr>");
                    $title.appendTo($head);
                    $head.appendTo($jobTable);
                    var $body =$("<tbody></tbody>");
                    $body.appendTo($jobTable);

                    $("#selectAllJob").click(function(){
                        selectAll(this);
                    });

                    $.each(data, function (i, v) {
                        var hasUser = false;
                        if(v.userName!=""){
                            hasUser = true;
                        }
                        var $tr = $("<tr></tr>");
                        var $cb = $("<td style='width:15px'><input class='jobCheck' term="+ hasUser +" value="+ v.stationId +"  type='checkbox'/></td>");
                        var $jobName = $("<td style='width:130px; max-width:130px'><span   title=" + v.stationName + ">" + (v.stationName == null ? "" : v.stationName) + "</span></td>");
                        var $orgName = $("<td style='width:100px; max-width:100px' ><span  title=" + v.organizationName + ">" + (v.organizationName == null ? "" : v.organizationName) + "</span></td>");
                        var $parentOrg = $("<td style='width:140px;max-width:140px' ><span   title=" + v.parentStationName + ">" + (v.parentStationName == null ? "" : v.parentStationName) + "</span></td>");
                        var $approval = $("<td style='width:57px;max-width:57px' ><span>" + (v.approval == true ? "是" : "否") + "</span></td>");
                     //  var $skipApproval = $("<td style='width:70px;max-width:70px' ><span>" + (v.skipApproval == true ? "是" : "否") + "</span></td>");
                        var $userName = $("<td style='width:150px;max-width:150px'><span  title=" + v.userName + ">" + (v.userName == null ? "" : v.userName) + "</span></td>");
                        var $operate = $("<td style='width:190px;max-width:200px'></td>");
                        var $edit = $("<a href='#' data-toggle='modal' data-target='#newJob_modal'>编辑</a>");
                        var $add = $("<a href='#' class='HR-add'  data-toggle='modal' data-target='#HR_modal' >&nbsp;&nbsp;&nbsp;管理人员</a>");
                        var $jobH = $("<a href='#'data-toggle='modal' data-target='#hand_modal'>&nbsp;&nbsp;&nbsp;岗位手册</a>");
                        var $delete = $("<a href='#'>&nbsp;&nbsp;&nbsp;删除</a>");

             
                

                        $operate.append([$edit,$add,$delete,$jobH,$delete]);
                        $tr.append([$cb,$jobName,$orgName,$parentOrg,$approval,$userName,$operate]);
                        $tr.appendTo($body);


                        $edit.click(function(){            //编辑
                            organizationName = v.organizationName;                 //存储岗位列表获取的组织名 以及 上级岗位名  详情获取没有这两个参数
                            parentStationName = v.parentStationName;
                            editMode =1;
                            $.ajax({                          //获取详情
                                type: "post",
                                url: "/OrganizationManagement/GetStationInfo",
                                dataType: "json",
                                data: { id: v.stationId },
                                success: rsHandler(function (data) {
                                    info  = data;
                                    console.log("岗位详情-->参数:" + v.stationId + "值:" + info.stationId);
                                    editJobInfo();
                                })
                            })


                        });

                        $jobH.click(function(){        //岗位手册事件
                            info = v;
                        });

                        $add.click(function () {
                            info = v;
                            HR_addFunc();
                        });

                        
                        //复选框与全选按钮
                        $cb.find("input").click(function () {
                            var checkFlag = $(this).prop("checked");
                            if (checkFlag) {
                                var flag = true;
                                var checks = $("#jobContent input:checkbox");
                                checks.each(function (i, v) {
                                    if (i > 0 && !$(this).prop("checked")) {
                                        flag = false;
                                    }
                                });
                                if (flag) {
                                    $("#selectAllJob").prop("checked", true);
                                }
                            }
                            else {
                                $("#selectAllJob").prop("checked", false);
                            }
                        });


                        $delete.click(function(){                   //删除事件  
                            //if (v.userName != "") {
                            //    ncUnits.alert("存在人员或下级岗位，无法删除!");
                            //}
                            //else {
                                ncUnits.confirm({
                                    title: '提示',
                                    html: '确定要删除?',
                                    yes: function (layer_confirm) {
                                        layer.close(layer_confirm);
                                        idList = [v.stationId];
                                        $.ajax({
                                            type: "post",
                                            url: "/OrganizationManagement/DeleteStation",
                                            dataType: "json",
                                            data: { data: JSON.stringify(idList) },
                                            success: rsHandler(function (data) {
                                                if (data == "1") {
                                                    ncUnits.alert("存在人员或下级岗位，无法删除!");
                                                    loadingJobList();
                                                } else {
                                                    ncUnits.alert("删除成功！");      //检测
                                                    loadingJobList();
                                                }
                                            })
                                        });
                                    }
                                });
                            //}                  
                        });
                    });
                }
                //改变目录的高度
                var x = $("#aaa").height();
                $("#menuTreeHeight").css({
                    "height": x
                });

            })
        });
    }

    //新建岗位函数
    function  editJobInfo(){

        $(".newJob").empty();
        $(".newOrg").empty();
        $("#newOrgOrJob").empty();
        if(editMode ==0){
            $("#jobNew").prop("checked", true);
            $("#newOrgOrJob").load("/OrganizationManagement/GetNewJob", function () {
                drawDowns();

                if (parentOrganization == null) {        //选择无 浏览组织架构时
                    $("#orgParentId").val("");
                } else {
                    parentOrganization = rightOrgId;
                    if (rightOrgName == null)
                        $("#orgParentId").val("");
                    else
                        $("#orgParentId").val(rightOrgName);
                }

                //parentOrganization = rightOrgId;
                //if (rightOrgId != null) {
                //    $("#orgParentId").val(rightOrgName);
                //}
            });
        }else if(editMode==1){
            $(".newJob").load("/OrganizationManagement/GetNewJob", function () {
                drawDowns();
                $("#jobName").val(info.stationName);
                parentOrganization=info.organizationId;           //上级组织ID
                $("#orgParentId").val(organizationName);
                parentJob = info.parentStation;

                if (parentJob != null) {                    //上级岗位为不为无时
                    $(".treeNames").text(parentStationName);
                }
                else {
                    $(".treeNames").text("无");
                }
                if(info.approval==1){
                    $(".approval").prop("checked",true);
                }

                if(info.skipApproval==1){
                    $(".skipApproval").prop("checked",true);
                }
                $("#jobDes").val(info.comment);
            });
        }

        $("#newJob_modal_insert").off("click");
        $("#newJob_modal_insert").click(function(){
            jobNewSubmit(this);
        });

        $("#new_modal_insert").off("click");
        $("#new_modal_insert").click(function(){
            if(orgOrJob==1){
                orgNewSubmit(this);
            }
            else{
                jobNewSubmit(this);
            }
        });

    };


    function jobNewSubmit(value) {

        //岗位名的输入判断
        var jobName = $.trim($('#jobName').val());
        if (jobName == "") {
            ncUnits.alert("岗位名不能为空!");
            $('#jobName').focus();
            return;
        }
        else if (jobName.length > 100) {
            ncUnits.alert("岗位名不能超过100字符!");
            $('#jobName').focus();
            return;
        }
        if (justifyByLetter(jobName, '岗位名') == false) {
            $('#jobName').focus();
            return;
        }


        //所属组织输入判断
        var orgParentInput = $.trim($('#orgParentId').val());
        if (orgParentInput == "") {
            ncUnits.alert("所属组织不能为空!");
            $('#orgParentId').focus();
            return;
        }      

        //岗位描述判断
        var jobDes = $.trim( $('#jobDes').val() );
        if (jobDes.length > 250) {
            ncUnits.alert("岗位描述不能超过250字符!");
            $('#jobDes').focus();
            return;
        }
        if (justifyByLetter(jobDes, '岗位描述') == false) {
            $('#jobDes').focus();
            return;
        }



        var modalHidden ;
       //参数的初始化
        if(editMode==1){
            argus_newJob.stationId = info.stationId;
            modalHidden = $("#newJob_modal");
        }
        else if(editMode==0){
            argus_newJob.stationId =null;
            modalHidden = $("#new_modal");
        }
        argus_newJob.parentStation = parentJob;
        argus_newJob.stationName = $("#jobName").val();
        argus_newJob.organizationId = parentOrganization;
        argus_newJob.comment = $("#jobDes").val();

        if( $(".checkbox-inline .skipApproval").is(':checked') ){
            argus_newJob.skipApproval = 1;
        }
        else{
            argus_newJob.skipApproval = 0;
        }
        if( $(".checkbox-inline .approval").is(':checked') ){
            argus_newJob.approval = 1;
        }
        else{
            argus_newJob.approval = 0;
        }

        $.ajax({
            type: "post",
            url: "/OrganizationManagement/SaveStation",
            dataType: "json",
            data: {data:JSON.stringify(argus_newJob)},
            success: rsHandler(function(data){
                if (editMode == 0 ) {
                    ncUnits.alert("岗位添加成功");
                }
                else if (editMode == 1) {
                    ncUnits.alert("岗位编辑成功");
                }
                modalHidden.modal("hide");       //隐藏模态框
                loadingJobList();
            }),
            complete:function() {
                console.log("complete");
            }
        });




    }









    /*人力资源 开始*/
    var personOrgId;
    var personWithSub = false;
    function HR_addFunc() {

       // var deleteUser = [];
   //     var delAll = [];

        $("#HR_modal .modal-content").load("/Shared/GetPersonHtml", function () {          
           
            //删除人员
            $.ajax({
                type: "post",
                url: "/OrganizationManagement/GetSharedUser",
                dataType: "json",
                data: { stationid: info.stationId },
                success: rsHandler(function (data) {
                    $.each(data, function (i, v) {
                       // delAll[delAll.length] = v.userId;
                        var $checked = $("<li  term=" + v.userId + "><span>" + v.userName + "</spam></li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $close.click(function () {
                            var nodeId = $(this).parent().attr("term");
                            $(this).parent().remove();
                            $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
                            $(".person_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                        //    deleteUser[deleteUser.length] = v.userId;        
                        });
                        $("#HR_modal_chosen").append($checked.append($close));
                        $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
                
                    })
                })
            });




            //组织架构树结构
            $.ajax({
                type: "post",
                url: "/OrganizationManagement/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    var folderTree = $.fn.zTree.init($("#HR_modal_folder"), $.extend({
                        callback: {
                            onClick: function (e, id, node) {
                                $("#HR-haschildren").prop("checked", false);
                                $("#person-selectall").prop("checked", false);
                                var checked = $("#HR-haschildren").prop('checked');
                                personWithSub = checked == true ? 1 : 0;
                                personOrgId = node.id;
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetUserList",
                                    dataType: "json",
                                    data: { withSub: personWithSub, organizationId: personOrgId },
                                    success: rsHandler(function (data) {
                                        $(".person_list ul").remove();
                                        if (data.length > 0) {
                                            $.each(data, function (i, v) {
                                                var $personHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                                $(".person_list").append($personHtml);
                                                $("#HR_modal_chosen li").each(function () {
                                                    if ($(this).attr('term') == v.userId) {
                                                        $personHtml.find("input[type='checkbox']").prop('checked', true);
                                                    }
                                                });
                                            });
                                            appendperson();
                                        }
                                    })
                                });
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false,
                            selectedMulti: false
                        },
                        async: {
                            enable: true,
                            url: "/OrganizationManagement/GetOrganizationList",
                            autoParam: ["id=parent"],
                            dataFilter: function (treeId, parentNode, responseData) {
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            });

            //其他
            var $noOrg = $("<a href='#' style='margin-left:10px'>编外人员</a>");
            $noOrg.off("click");
            $noOrg.click(function () {
                $.ajax({
                    type: "post",
                    url: "/OrganizationManagement/GetALLUser",
                    dataType: "json",
                    success: rsHandler(function (data) {
                        $(".person_list ul").remove();
                        if (data.length > 0) {
                            $.each(data, function (i, v) {
                                if (v.userName != "系统管理员") {
                                    var $personHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span></li></ul>");
                                    $(".person_list").append($personHtml);
                                    $("#HR_modal_chosen li").each(function () {
                                        if ($(this).attr('term') == v.userId) {
                                            $personHtml.find("input[type='checkbox']").prop('checked', true);
                                        }
                                    });
                                }
                            });
                            appendperson();
                        }
                    })
                });
            });
            $(".chosebox").append($noOrg);


            //人员搜索
            $("#HR_modal_search").selection({
                url: "/User/GetXXUser",
                hasImage: true,
                selectHandle: function (data) {
                    $("#HR_select").val(data.name);
                    var flag = true;
                    if ($("#HR_modal_chosen li").length > 0) {
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == data.id) {
                                flag = false;
                            }
                        });
                    }
                    if (flag == true) {
                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#HR_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var nodeId = $(this).parent().attr("term");
                            $(this).parent().remove();
                            $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
                            $(".person_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#person-selectall").prop("checked", false);
                        });
                    }
                    if ($(".person_list ul").length > 0) {
                        $(".person_list ul").each(function () {
                            if ($(this).find("li:eq(1)").attr('term') == data.id.toString()) {
                                $(this).find("input[type='checkbox']").prop("checked", true);
                            }
                        });
                    }
                    $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
                }
            });


            //人员复选框点击事件
            function appendperson() {
                $(".person_list input[type='checkbox']").click(function () {
                    var checked = $(this).prop('checked');
                    var personId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
                    var personName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
                    var showflag = true;
                    if (checked == true) {
                        $(this).attr('checked', true);
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr("term") == personId) {
                                showflag = false;
                            }
                        });
                        if (showflag) {
                            $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length + 1);
                            var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $("#HR_modal_chosen").append($checked.append($close));
                            $close.click(function () {
                                var $thisId = $(this).parent().attr('term');
                                $(this).parent().remove();
                                $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                                $(".person_list ul").each(function () {
                                    if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                        $(this).find("input[type='checkbox']").prop("checked", false);
                                    }
                                });
                                $("#person-selectall").prop("checked", false);
                            });
                        }
                    } else {

                        $(this).prop('checked', false);
                        $(this).parents(".person_list").find("li").each(function () {
                            if ($(this).attr('term') && $(this).attr('term') == personId) {
                                $(this).parents(".list-inline").find("li:eq(0) input").prop('checked', false);
                            }
                        });
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == personId) {
                                $(this).remove();

                                ////如果没有选中复选框 则判断是否是要删除的人
                                //if ($.inArray( parseInt(personId), delAll ) >= 0) {
                                //    deleteUser[deleteUser.length] = parseInt(personId);
                                //    delAll.splice($.inArray( parseInt(personId), delAll ), 1);
                                //}

                                if ($("#HR_modal_chosen li").length <= 0) {
                                    $("#person-selectall").prop("checked", false);
                                }
                                $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                            }
                        });
                    }
                });
            }

            //包含下级
            $("#HR-haschildren").click(function () {
                $(".person_list ul").remove();
                var checked = $(this).prop('checked');
                personWithSub = checked == true ? 1 : 0;
                $.ajax({
                    type: "post",
                    url: "/Shared/GetUserList",
                    dataType: "json",
                    data: { withSub: personWithSub, organizationId: personOrgId },
                    success: rsHandler(function (data) {
                        if (data.length > 0) {
                            $.each(data, function (i, v) {
                                var $personHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                $(".person_list").append($personHtml);
                                $("#HR_modal_chosen li").each(function () {
                                    if ($(this).attr('term') == v.userId) {
                                        $personHtml.find("input[type='checkbox']").attr('checked', true);
                                    }
                                });
                            });
                            appendperson();
                        }
                    })
                });
            });

            //选择全部
            $('#person-selectall').click(function () {
                var personAll = $(this).prop("checked");
                var showflag = true;
                if (personAll == true) {
                    $(".person_list ul").each(function () {
                        showflag = true;
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    $(".person_list ul input[type='checkbox']").prop('checked', true);

                    var length = $(".person_list input[type='checkbox']:checked").length
                    $('#HR_modal_chosen_count').text(length);
                    for (var i = 0; i < length; i++) {
                        showflag = true;
                        var personId = $(".person_list ul:eq(" + i + ")").find("li:eq(1)").attr('term');
                        var personName = $(".person_list ul:eq(" + i + ")").find("li:eq(1) span:eq(0)").text();
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == personId) {
                                showflag = false;
                            }
                        });
                        if (showflag) {
                            var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $("#share_modal_chosen li").each(function () {
                                if ($(this).attr('term') == stationId) {
                                    $(this).remove();
                                    $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
                                }
                            });
                            $("#HR_modal_chosen").append($checked.append($close));
                            $close.click(function () {
                                var $thisId = $(this).parent().attr('term');
                                $(this).parent().remove();
                                $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                                $(".person_list ul").each(function () {
                                    if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                        $(this).find("input[type='checkbox']").prop("checked", false);
                                    }
                                });
                                $("#person-selectall").prop("checked", false);
                            });
                        }

                    }
                    $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);

                }
                else {               //如果全选复选框全部被取消  则判断是否有删除元素
                    $(".person_list ul").each(function () {
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                                //if ( $.inArray( parseInt(term), delAll ) >= 0 ) {
                                //    deleteUser[deleteUser.length] = parseInt(term);
                                //    delAll.splice($.inArray( parseInt(term), delAll), 1);
                                //}
                            }
                        });
                    });
                    $(".person_list ul input[type='checkbox']").prop('checked', false);
                    var length = $("#HR_modal_chosen li").length
                    $('#HR_modal_chosen_count').text(length);
                }
            });



            //确定
            $("#HR_modal_submit").click(function () {     
                var addUser = [];       //添加岗位人员ID
                var i = 0;

                $(".depart_choose").remove();
                $(".station_choose").remove();
                $(".HR_choose").remove();
                $("#HR_modal_chosen li").each(function () {                  
                //    if (!$(this).hasClass("del")) {
                        addUser[i++] = $(this).attr("term");
                //    }s
                });

                console.log("addUser" + addUser);
                    $.ajax({
                        type: "post",
                        url: "/OrganizationManagement/AddUser",
                        dataType: "json",

                        data: { data: JSON.stringify({ addUser: addUser,stationid:info.stationId }) }, 
                        success: rsHandler(function (data) {
                            $("#manager_modal").modal("hide");       //隐藏模态框
                            loadingJobList();
                        }),
                        complete: function () {
                            console.log("complete");
                        }
                    });
                $("#HR_select").val('');
                $('#HR_modal').modal('hide');
            });

            //取消
            $("#HR_cancel").click(function () {
                $("#HR_select").val('');
                $("#person-haschildren").prop("checked", false);
                $("#HR_modal_chosen_count").text(0);
                $(".person_list input[type=checkbox]").prop("checked", false);
                $("#HR_modal_chosen li").remove();
            }); 

        });
    }
    /*人力资源 结束*/




    var hand_modalFirst = true;            //标志岗位手册是否是第一次加载
    var doMethod;
    //添加岗位手册所用到的参数
    var argus_addPlanArray = [];
    var argus_delPlanArray = [];
    //岗位手册
    $('#hand_modal').on('show.bs.modal', function () {
        argus_addPlanArray.length = 0;
        argus_delPlanArray.length = 0;
        $(this).find("#hand_jobName").text(info.stationName);
        $(this).find("#hand_jobName").attr("title", info.stationName);
        $(this).find("#hand_orgName").text(info.organizationName);
        $(this).find("#hand_orgName").attr("title", info.organizationName);
        var $handTable = $("#handList");
        var lodi = getLoadingPosition($handTable);//显示load层

        $.ajax({
            type: "post",
            url: "/OrganizationManagement/GetStationManual",
            dataType: "json",
 
            data: { stationId:info.stationId },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                    $handTable.empty();
                    var $tableHead = $("<thead class='colorGray'><tr><th style='width:110px'>执行方式</th><th style='width:221px' >事项的输出结果</th><th style='width:73px'>循环周期</th><th style='width:73px' >循环时间</th><th style='width:51px'></th></tr></thead>");
                    var $tableBody = $("<tbody></tbody>");
                    $.each(data, function (i, v) {
                        var $tr = $("<tr></tr>");
                        var $td = $("<td style='width:110px' >" + v.executionName + "</td><td class='tdEllipsis' title=" + v.eventOutput + ">" + v.eventOutput + "</td><td style='width:73px'>" + cycleDate[v.loopType] + "</td>");

                        var $time = $("<td style='width:73px' ></td>");
                        var timevalue;
                        if (v.loopType == 0) {              //日
                            timevalue = (v.loopTime[0] == '0' ? '' : v.loopTime[0]) + v.loopTime[1] + ":" + ( v.loopTime[2] == '0' ? '' : v.loopTime[2] ) + v.loopTime[3];
                        }
                        else if (v.loopType == 1) {                //周
                            timevalue = weekArray [v.loopWeek-1];
                        }
                        else if (v.loopType == 2) {              //月
                            timevalue = v.loopMonth;
                        }
                        if (v.loopType == 3) {                 //年
                            timevalue = (v.loopYear[0] == '0' ? '' : v.loopYear[0]) + v.loopYear[1] + "-" + (v.loopYear[2] == '0' ? '' : v.loopYear[2]) + v.loopYear[3];;
                        }
                        $time.text(timevalue);

                        var $del = $("<td style='width:51px'><span term=" + v.loopId + " class='handDel'></span></td>");

                        //删除循环计划1
                        $del.off("click");
                        $del.click(function () {

                            var $this = $(this);
                            ncUnits.confirm({
                                title: '提示',
                                html: '你确定要删除?',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    var loopId = $this.find("span").attr("term");
                                    // argus_delPlanArray.push(loopId);
                                    argus_delPlanArray[argus_delPlanArray.length] = loopId;
                                    $this.parents("tr").remove();
                                }
                            });
                              
                        });
                        console.log(argus_delPlanArray);
                        $tr.append($td, $time, $del).appendTo($tableBody);
                    });
                    $handTable.append($tableHead).append($tableBody);
            })
        });

        if (hand_modalFirst) {         //岗位手册模态框第一次加载时所添加的事件
            hand_modalFirst = false;
            //添加循环计划页面---增加循环计划按钮事件
            plan_addEvent();
            $("#hand_add").click(function(){
                $("#planModal").show();
            });

            //循环计划添加事件
            $("#planAdd").click(function () {

                //确认人不得为空
                var surePerson = $("#surePerson").val();
                if (surePerson == "") {
                    ncUnits.alert("确认人不能为空!");
                    $('#surePerson').focus();
                    return;
                }

                //循环时间不得为空的输入判断
                if ($("#circleDateIn").attr("term") == 1) {
                    var time = $.trim($("#timeIn").val());
                } else if ($("#circleDateIn").attr("term") == 2) {
                    var time = $.trim($("#weekIn").val());
                } else if ($("#circleDateIn").attr("term") == 3) {
                    var time = $.trim($("#dayIn").val());
                } else if ($("#circleDateIn").attr("term") == 4) {
                    var timeOne = $.trim($("#monthIn").val());
                    var time = $.trim($("#dayIn").val());                   
                }
                if ($("#circleDateIn").attr("term") != 4) {
                    if (time == "") {
                        ncUnits.alert("循环时间不得为空!");
                        return;
                    }
                }
                else {
                    if (timeOne == "" || time == "") {
                        ncUnits.alert("循环时间不得为空!");
                        return;
                    }
                }
                
                //完成时间
                var completeTime = $.trim( $('#completeTime').val() );
                if (completeTime == "") {
                    ncUnits.alert("单位完成时间不得为空!");
                    $('#completeTime').focus();
                    return;
                }   

                //输出事项的判断
                var result = $.trim($('#result').val());
                if (result == "") {
                    ncUnits.alert("事项输出结果不能为空!");
                    $('#result').focus();
                    return;
                }
                else if (result.length > 50) {
                    ncUnits.alert("事项输出结果不能超过50字符!");
                    $('#result').focus();
                    return;
                }
                if (justifyByLetter(result, '事项输出结果') == false) {
                    $('#result').focus();
                    return;
                }

             


                var argus_addPlan = {
                    loopId:Number,           //计划ID
                    stationId:Number,        //岗位ID
                    organizationId:Number,   //组织ID
                    executionModeId:Number,  //执行方式
                    eventOutput:String,   //事项输出结果
                    confirmUser:Number,      //确认人
                    loopMonth: Number,            //月
                    loopWeek: Number,             //周
                    loopYear: Number,              //日
                    loopTime: String,          //时间
                    loopType: Number,          //循环周期
                    unitTime :Number        //完成时间
                 };

                argus_addPlan.loopId="";
                argus_addPlan.stationId = info.stationId;
                argus_addPlan.organizationId = info.organizationId;
                argus_addPlan.executionModeId = $("#doMethodIn").attr("term");
                argus_addPlan.eventOutput = result;
                argus_addPlan.confirmUser = $("#surePerson").attr("term");     //默认为空值
                argus_addPlan.loopType = $("#circleDateIn").attr("term") - 1;
                argus_addPlan.unitTime = completeTime;

                //时间参数的传输问题
                argus_addPlan.loopMonth = '';                          
                argus_addPlan.loopYear = '';
                argus_addPlan.loopWeek ='';
                argus_addPlan.loopTime ='';
                if( argus_addPlan.loopType == 0 ) {
                    var times =$.trim( $("#timeIn").val() ).split(":");      
                    if( times[0].length == 1 ){
                        times[0] = "0" + times[0];
                    }
                    argus_addPlan.loopTime = times[0] + times[1]; 
                }else if (argus_addPlan.loopType == 1) {
                    argus_addPlan.loopWeek = $.trim( $("#weekIn").attr("term") );             
                }else if (argus_addPlan.loopType == 2) {
                    argus_addPlan.loopMonth = $.trim( $("#dayIn").val() );
                } else {
                    var month = $.trim( $("#monthIn").val() );
                    var day = $.trim( $("#dayIn").val() );
                    if (month.length == 1) {
                        month = "0" + month;
                    }
                    if ( day.length == 1) {
                        argus_addPlan.loopYear = month + "0" + day;
                    }else {
                        argus_addPlan.loopYear = month + day;
                    }
                }
                argus_addPlanArray.push( argus_addPlan );
             

                //页面添加一条计划
                var $tr = $("<tr><td style='width:110px'>" + $("#doMethodIn").text() + "</td><td class='tdEllipsis' title=" + $("#result").val() + ">" + $("#result").val() + "</td><td style='width:73px'>" + $("#circleDateIn").text() + "</td></tr>");
                var x;
                if( $("#circleDateIn").attr("term") == 4 ){       //如果是年历
                    var month = $("#monthIn").val();
                    var day = $("#dayIn").val();
                    x = month+"-"+day;
                }else{
                    x = $("#circleTime_down div:visible input").val();
                }
                var $del = $("<td style='width:51px'><span term=" + argus_addPlanArray.length + " class='handDel'></span></td>");


                //删除循环计划2
                $del.click(function () {
                    var $this = $(this);
                    ncUnits.confirm({
                        title: '提示',
                        html: '你确定要删除?',
                        yes: function (layer_confirm) {
                            layer.close(layer_confirm);
                            $this.parents("tr").remove();
                            var index = $this.attr("term");
                            argus_addPlanArray.splice(index - 1, 1);        //删除添加循环计划数组中的某一个指定下标的元素
                        }
                    });                
                });
                $tr.append($("<td style='width:73px'>" + x + "</td>"));
                $tr.append($del);
                $("#handList").append($tr);
                clearPlanModal();              //清空输入框
            });

            //循环计划取消事件
            $("#planCancel").click(function(){
                clearPlanModal();
                $("#planModal").hide();
            });

            //添加岗位手册
            $("#hand_sure").click(function(){
                $.ajax({
                    type: "post",
                    url: "/OrganizationManagement/AddStationManual",
                    dataType: "json",
 
                    data: { data: JSON.stringify({ "loopPlanList": argus_addPlanArray, "deleteStation": argus_delPlanArray, "stationId": info.stationId }) },

                    success: rsHandler(function (data) {
                        $.each(argus_addPlanArray,function(i,v){
                            console.log(i+"-"+v);
                        });
                        console.log("argus_delArray"+argus_delPlanArray);
                        ncUnits.alert("添加成功!");
                        argus_addPlanArray.length=0;
                        argus_delPlanArray.length =0;
                        $("#hand_modal").modal('hide');
                    })
                });
            });


        }

        //执行方式获取
        $.ajax({
            type: "post",
            url: "/BuildNewPlan/GetExecutionList",
            dataType: "json",
            success: rsHandler(function (data) {
                for (var i = 0, len = data.length; i < len ; i++) {
                    $("<li><a href='#' term=" + data[i].id + ">" + data[i].text + "</a></li>").appendTo($("#doMethod ul"));
                }
                doMethod = data[0].text;
                $("#doMethodIn").text(data[0].text);
                $("#doMethodIn").attr("term", data[0].id);
                //执行方式下拉框事件
                $("#doMethod").find("li a").off("click");
                $("#doMethod").find("li a").click(function () {
                    dropDownEvent(this, 1);
                });
            })
        });

       

   });
    var day  = []
    //循环计划页面中的事件
    function plan_addEvent(){
        var x = $("#hand_modal .modal-dialog").width()-5;
        $("#planModal").css({"left":x });

            //绘制日历表格
            var $tr = $("<tr></tr>");
            for(var i=1;i<=31;i++){
                var $td = $("<td><a herf='#'>"+i+"</a></td>");
                $tr.append($td)
                if(i%7==0 ){
                    $("#dayInput table").append($tr);
                    var $tr = $("<tr></tr>");
                }
            }
       //     $tr.append(  $("<td>1</td><td>2</td><td>3</td><td>4</td>").css("color","#c5c5c5")  );
            $("#dayInput table").append($tr);



            //绘制时间下拉框
            var $ul = $("#dropUl");
            for(var i=0;i<24;i++){
                var $li = $("<li><a herf='#'>" + i + "</a></li>");
                $li.off("click");
                $li.click(function () {
                    var $this = $(this);
                    $(".dropUlChild").remove();
                    var term = $this.find("a").text();
                    var $ulTwo = $("<ul class='dropUlChild'></ul>");
                    $this.append($ulTwo);
                    for(var j=0;j<=5;j++){
                        if(j==0){
                            var  $liTwo = $("<li><a herf='#'>"+ term  +":00"+"</a></li>");
                        }
                        else{
                            var  $liTwo = $("<li><a herf='#'>"+ term  +":"+j*10+"</a></li>");
                        }
                        $liTwo.off("click");
                        $liTwo.click(function(e){
                            $("#timeCandler input").val( $(this).find("a").text() );
                            $(this).parents(".dropUlChild").remove();
                            $("#dropUl").slideUp("fast");
                            e.stopPropagation();    //阻止事件冒泡
                        });
                        $ulTwo.append($liTwo);
                    }
                });
                $ul.append($li);
            }


            $("#circleTime_down > div").hide();
            $("#onehr").hide();
            $("#timeCandler").show();
            handEvent();

            //循环周期下拉框事件
            $("#circleDate_Input").find("li a").off("click");
            $("#circleDate_Input").find("li a").click(function () {
                $(".dateCorn").removeClass("dateCorn_hit");
                dropDownEvent(this,1);
                var term = $(this).attr("term");
                $("#circleTime_down > div").hide();
                $("#onehr").hide();
                if(term==1){
                    $("#timeCandler").show();
                    $("#timeCandler input").val("");
                }else if(term == 2 ){              //显示周历
                    $("#weekCandler input").val("");
                    $("#weekCandler input").attr("term","");
                    $("#weekCandler").show();
                }else if(term==3){        //显示月历
                    $("#dayCandler input").val("");
                    $("#dayCandler input").attr("term","");
                    $("#dayCandler").show();
                }else if(term == 4){          //显示年历
                    $("#monthCandler input").val("");
                    $("#monthCandler input").attr("term","");
                    $("#monthCandler").show();
                    $("#onehr").show();

                    $("#dayCandler input").val("");
                    $("#dayCandler input").attr("term","");
                    $("#dayCandler").show();
                }
            });
              
        //模糊查询
        //$("#surePerson").searchPopup({
        //    url: "/User/GetXXUser",
        //    defText: "常用联系人",
        //    hasImage: true,
        //    selectHandle: function (data) {
        //        $("#surePerson").attr("term",data.id);                 //获取确认的ID
        //        $(this).val(data.name);
        //        console.log("确认人id"+data.id);
        //    }
        //});
        //确认人选择
            $("#surePerson").searchPopup({
                url: "/BuildNewPlan/GetIpUserByUserId",
                defText: "常用联系人",
                hasImage: true,
                selectHandle: function (data) {
                    $("#surePerson").attr("term", data.id);                 //获取确认的ID
                    $(this).val(data.name);
                    console.log("确认人id" + data.id);

                }
            });
    }

  

    //岗位手册模态框隐藏事件
    $('#hand_modal').on('hide.bs.modal', function () {
        $("#handList").empty();
        clearPlanModal();

        $("#doMethod ul").empty();
        $("#result").val("");

        $("#planModal").hide();
    });

    //清空新增循环计划中输入框的值
    function clearPlanModal() {

        $(".dateCorn").removeClass("dateCorn_hit");
        $("#dropUl").slideUp();

        $("#doMethodIn").text(doMethod);
        $("#result").val("");
        $("#completeTime").val("");

        $("#circleDateIn").text("日");
        $("#circleDateIn").attr("term",1);

        $("#circleTime_down > div").hide();
        $("#onehr").hide();
        $("#timeCandler").show();

        $("#surePerson").val("");
        $("#surePerson").attr("term","");

        $("#weekIn").attr("term","");
        $("#weekIn").val("");
        $("#monthIn").val("");
        $("#dayIn").val("");
        $("#timeIn").val("");


    }

    //新增循环计划页面中的时间输入框事件
    function handEvent(){
        //时间下拉框
        $("#timeInput").off("click");
        $("#timeInput").click(function(){
            $("#dropUl").slideToggle("fast");
        });
        $(".dateCorn").parents(".dropdown-toggle").off("click");
        $(".dateCorn").parents(".dropdown-toggle").click(function(){
           $(this).find(".dateCorn").toggleClass("dateCorn_hit");
        });

        //日下拉框
        $("#dayInput table td").off("click");
        $("#dayInput table td").click(function(){
            $(this).parents("table").prev().find("input").val( $(this).text() );
            $(this).parents(".dropdown").find(".dateCorn").removeClass("dateCorn_hit");
        });

        //周下拉框
        $("#weekCandler li a").off("click");
        $("#weekCandler li a").click(function(){
               dropDownEvent(this,2);
        });

        //月下拉框
        $("#monthCandler li a").off("click");
        $("#monthCandler li a").click(function(){
              dropDownEvent(this,2);
             $(this).parents(".dropdown").find(".dateCorn").removeClass("dateCorn_hit");
        });

    }


    //dropdown事件
    function dropDownEvent(value,flag){
        if(flag==1){
            var x =  $(value).parents("ul").prev().find("span:eq(0)");
            x.text( $(value).text() );
        }else{
            var x =  $(value).parents("ul").prev().find("input:eq(0)");
            x.val( $(value).text() );
        }
        var term  = $(value).attr("term");
        x.attr("term",term);
    }

});
