//@ sourceURL=addPlan.js
/**
 * Created by ZETA on 2015/4/7.
 */

function addPlan() {
    console.log("addPlan");
    var page_flag = page_flag ? page_flag : null;
    var layIndex_addPlan;
    //$(".addPlan").click(function(){
    //    initPlan();
    //    layIndex_addPlan = $.layer({
    //        type: 1,
    //        shade: [0.5, '#000'],
    //        area: ['auto', 'auto'],
    //        //title: ['新建计划','background:#58b456;color:#fff;'],
    //        title:false,
    //        border: [0],
    //        page: {dom : '#layer_addplan'},
    //        move:".drapdiv",
    //        closeBtn:false
    //    });
    //
    //    $("#layer_addplan .closeWCom").click(closePlan)
    //});

    /* 新建计划 开始 */
    //变量
    var department_v              //部门值
        , project_v               //项目值
        , runMode_v                //执行方式
        , output_v                   //事项输出结果
        , roundType_v         //循环类型
        , roundTime_v         //循环时间
        , endTime_v                  //完成时间
        , responsibleUser_v          //责任人
        , confirmUser_v             //确认人
        , isTmp_v                   //是否临时计划
        , workTime_v                //单位完成工时
        , partner_v = []                 //协作人
        , premise_v = []                  //前提计划
        , tab_v = []                      //标签
        , meetingId                 //会议室Id

        ,department_text
        ,project_text
        ,runMode_text
        ,roundType_text
        ,responsibleUser_name
        ,confirmUser_name
        ,partner_name = []
        ,premise_text = [];

    var setting = {
        view: {
            showIcon:false,
            showLine:false,
            selectedMulti:false
        }
    };

    function getNodeNameLine(node,nameLine){
        if(node){
            return getNodeNameLine(node.getParentNode(),nameLine ? node.name + " - " + nameLine : node.name);
        }else{
            return nameLine;
        }
    }
    //部门分类的tree
    $.ajax({
        type: "post",
        url: "/BuildNewPlan/GetOrgList",
        dataType: "json",
        success:rsHandler(function(data){
            $.fn.zTree.init($("#addplan_department"), $.extend({
                callback:{
                    onClick:function(e,id,node){
                        var nodeName = getNodeNameLine(node);
                        $("#addplan_department_v").html(nodeName);
                        department_v = node.id;
                        department_text = nodeName;
                        $("#addplan_department_icon").click();
                    }
                }
            },setting),data);
        })
    });

    //项目分类的tree
    $.ajax({
        type: "post",
        url: "/BuildNewPlan/GetAllProList",
        dataType: "json",
        success:rsHandler(function(data){
            $.fn.zTree.init($("#addplan_project"),$.extend({
                callback:{
                    onClick:function(e,id,node){
                        var nodeName = getNodeNameLine(node);
                        $("#addplan_project_v").html(nodeName);
                        project_v = node.id;
                        project_text = nodeName;
                        $("#addplan_project_icon").click();
                    }
                }
            },setting),data);

        })
    });

    //addplan部门分类的收缩展开
    $("#addplan_department_icon").unbind();
    $("#addplan_department_icon").click(function () {

        if($(this).hasClass("arrowsBBBCom")){
            $(this).removeClass("arrowsBBBCom");
            $(this).addClass("arrowsBBTCom");
            $("#addplan_department").slideDown();
        }else{
            $(this).addClass("arrowsBBBCom");
            $(this).removeClass("arrowsBBTCom");
            $("#addplan_department").slideUp();
        }
    });

    //addplan项目分类的收缩展开
    $("#addplan_project_icon").unbind();
    $("#addplan_project_icon").click(function(){
        if($(this).hasClass("arrowsBBBCom")){
            $(this).removeClass("arrowsBBBCom");
            $(this).addClass("arrowsBBTCom");
            $("#addplan_project").slideDown();
        }else{
            $(this).addClass("arrowsBBBCom");
            $(this).removeClass("arrowsBBTCom");
            $("#addplan_project").slideUp();
        }
    });

    //laydate.skin("huanglv");

    //addplan完成时间选项的弹层
    var end = {
        elem: '#addplan_endtime_v', //需显示日期的元素选择器
            event: 'click', //触发事件
        format: 'YYYY-MM-DD hh:mm', //日期格式
        istime: true, //是否开启时间选择
        isclear: true, //是否显示清空
        istoday: true, //是否显示今天
        issure: true, //是否显示确认
        festival: true, //是否显示节日
        //min: '1900-01-01 00:00:00', //最小日期
        //max: '2099-12-31 23:59:59', //最大日期
        start:new Date().toLocaleDateString() + ' 17:30:00',    //开始日期
        //fixed: false, //是否固定在可视区域
        //zIndex: 99999999, //css z-index
        choose: function (dates) { //选择好日期的回调
            
            endTime_v = dates;
            console.log("endtime_v:", endTime_v)
            start.max = dates;
        },
        clear: function () {
            console.log("clear endtime_v")
            endTime_v = undefined;
            start.max = undefined;
        }
    }

    $("#addplan_endtime").click(function(){
        laydate(end);
    });

    //addplan循环时间选项的弹层
    var start = {
        elem: '#addplan_roundtime_v',
        event: 'click',
        format: 'YYYY-MM-DD',
        isclear: true,
        istoday: true,
        issure: true,
        festival: true,
        choose: function(dates){
            roundTime_v = dates;
            end.start = dates;
            end.min = dates;
        },
        clear:function(){
            roundTime_v = undefined;
            end.min = undefined;
        }
    }

    $("#layer_addplan .select-trigger").each(function(index,el){
        var $options = $(this).nextAll(".select-options");
        $(el).click(function(e){
            e.stopPropagation();
            $("#layer_addplan .select-options").not($options).slideUp(100);
            $options.stop();
            $options.slideToggle(100);
        });
        $(document).click(function(){
            $options.slideUp(100);
        });
    });

    $("#addplan_roundtime").click(function(){
        laydate(start);
    });


    $("#runmode").change(function(){
        runMode_v = $(this).val();
        runMode_text = $("option:selected",$(this)).text();
    });

    $("#isTmp").change(function(){
        isTmp_v = $(this).val();
    });

    //title 新建计划or子计划
    //$(".popUp .title a").click(function(){
    //    if($(this).hasClass("sub")){
    //        $(this).siblings("a").addClass("sub");
    //        $(this).removeClass("sub");
    //        var $thisdiv = $($(this).attr("hrefto"));
    //        $thisdiv.siblings(".content").hide();
    //        $thisdiv.show();
    //    }
    //});


    //循环类型
    $("#roundtype").change(function(){
        roundType_v = $(this).val();
        roundType_text = $("option:selected",this).text();
        if( roundType_v == undefined || roundType_v == ""){
            $(this).css({color:"#a0a0a0"});
            $("#addplan_roundtime").hide();
            $("#worktimeDiv").hide();
        }else{
            $(this).css({color:"#686868"});
            if(roundType_v == 0){
                $("#addplan_roundtime").hide();
                $("#worktimeDiv").hide();
                $("#isTmpOC").show();
                //$("#layer_addplan .title .sub").show();
            }else{
                $("#addplan_roundtime").show();
                $("#worktimeDiv").show();
                $("#isTmpOC").hide();
                //$("#layer_addplan .title .sub").hide();
            }
        }
    });

    //责任人选择
    var selectUserTip;
    console.log(2222, $("#addplan_responsibleUser"))
    $("#addplan_responsibleUser").searchStaffPopup({
        url: "/BuildNewPlan/GetIpUserByUserId",
        defText: "常用联系人",
        hasImage: true,
        selectHandle: function (data) {
            layer.close(selectUserTip);
            if(data.id == confirmUser_v){
                selectUserTip = validate_reject('责任人与确认人不能为同一人', "#addplan_responsibleUser");
                return
            }
            layer.close(selectUserTip);
            for (var item = 0; item < partner_v.length; item++) {
                if (data.id == partner_v[item]) {
                    selectUserTip = validate_reject('责任人与协作人不能为同一人', "#addplan_responsibleUser");
                    return;
                }
            }
            responsibleUser_v = data.id;
            responsibleUser_name = data.name;
            $(this).val(responsibleUser_name);
        }
    });
    console.log(111, $("#addplan_responsibleUser"))

    //确认人选择
    $("#addplan_confirmUser").searchStaffPopup({
        url: "/BuildNewPlan/GetUserByUserId",
        defText: "常用联系人",
        hasImage: true,
        selectHandle: function (data) {
            layer.close(selectUserTip);
            if(responsibleUser_v == data.id){
                selectUserTip = validate_reject('确认人与责任人不能为同一人', "#addplan_confirmUser");
                return
            }
            confirmUser_v = data.id;
            confirmUser_name = data.name;
            $(this).val(data.name);
        }
    });

    //协作人选择
    $("#addplan_partner").searchStaffPopup({
        url: "/BuildNewPlan/GetIpUserByUserId",
        defText: "常用联系人",
        hasImage: true,
        selectHandle: function (data) {
            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({display:"none"});

            $span.hover(function(){
                $close.toggle();
            })

            $span.append([data.name,$close]);

            $(this).parent().before($span);

            partner_v.push(data.id);
            partner_name.push(data.name);

            $close.click(function(){
                $span.remove();
                removeValue(partner_v,data.id);
                removeValue(partner_name,data.name);
            });
        }
    });

    //前提计划选择
    //$("#addplan_premise").searchStaffPopup({
    //    url: "/Plan/GetSelectPlan",
    //    defText: "前提计划",
    //    selectHandle: function (data) {
    //        layer.close(selectUserTip);
    //        var $span = $("<span></span>").addClass("tag");
    //        var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

    //        $span.hover(function () {
    //            $close.toggle();
    //        });

    //        $span.append([data.name, $close]);
    //        $(this).parent().before($span);

    //        premise_v.push(data.id);
    //        premise_text.push(data.name);

    //        $close.click(function () {
    //            $span.remove();
    //            removeValue(premise_v, data.id);
    //            removeValue(premise_text, data.name);
    //        });
    //    }
    //});

    var tagsList = [];
    $.ajax({
        url: "/Shared/GetMostUsedTag",
        type: "post",
        dataType: "json",
        success:rsHandler(function(data){
            tagsList = $.map(data, function (n) {
                return {
                    label: n
                }
            })
        })
    });
    //addplan的标签设置
    $("#addplan_label").click(function(){
        var $this = $(this);
        $this.hide();
        var $con = $("<div style='display: inline-block;position: relative;'></div>");
        var $input = $('<input type="text" style="width: 100px;height: 26px;border: 1px solid #e3e3e3;vertical-align: top;padding-right: 25px"/>');
        var $em = $("<em style='position: absolute;right: 0px;'></em>").addClass("icon add-min hasborder");

        $con.append([$input,$em]).appendTo($this.parent());

        function addtag(){

            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({
                display:"none",
                "font-size": "12px"
            });

            $span.hover(function(){
                $close.toggle();
            })

            var iv = $input.val();

            $span.append([iv, $close]).attr({
                title:iv
            });
            $this.parent().before($span);

            tab_v.push(iv);

            $close.click(function(){
                $span.remove();
                removeValue(tab_v,iv);
            });

            $con.remove();
            $this.show();
        }

        $input.autocompleter({
            source: tagsList
        });
        $input.keydown(function(e){
            console.log(e.keyCode);
            if(e.keyCode == 13 && $(this).val()){
                addtag();
            }else if(e.keyCode == 27){
                $con.remove();
                $this.show();
            }
        }).blur(function(){
            if($(this).val()){
                addtag();
            }else{
                $con.remove();
                $this.show();
            }
        });

        $em.click(function(){
            if($input.val()){
                addtag();
            }
        });
    });

    //addplan执行方式加载
    $.ajax({
        type: "post",
        url: "/BuildNewPlan/GetExecutionList",
        dataType: "json",
        success: rsHandler(function (data) {
            $('#runmode option:not(:first)').remove();
            for (var i = 0, len = data.length; i < len ; i++) {
                $("<option></option>").val(data[i].id).html(data[i].text).appendTo($("#runmode"));
            }
            $('#runmode option:eq(0)').attr("selected","true");
        })
    });

    //计划预览列表
    function PlanCache($container){

        $container = $($container);

        var _this = this;

        var $ul = $("<ul></ul>");
        $container.html($ul);

        this.addLeaf = function($con,plan){
            console.log("==== 添加 ====");
            console.log(plan);
            var $storage = $con.siblings(".storage");
            var left = $storage.length == 0 ? 22 : ($storage.data("data-left") + 20);

            var $li = $("<li></li>").css({position:"relative"});
            var $div = $("<div class='storage' style='line-height: 20px'></div>");
            $div.data("data-plan",plan).data("data-left",left);
            var $divInfo = $("<div class='leaf-info'></div>").css({padding:"5px 0"});
            var $divShade = $("<div class='leaf-shade'></div>").css({position:"absolute",width:"100%",background:"#000",opacity:0.7,top:0,display:"none"});

            var $btnDelete = $("<span>删除</span>").css({color:"#fff",float:"right",padding:"5px 10px",cursor: "pointer"});
            var $btnInfo = $("<span>详情</span>").css({color:"#fff",float:"right",padding:"5px 10px",cursor: "pointer"});

            $btnInfo.click(function(e){
                e.stopPropagation();
                initPlan($div.data("data-plan"));
                _this.modifying = $div;
                $("#canCon_add").hide();
                $("#canCon_modify").show();
            });
            $btnDelete.click(function(e){
                e.stopPropagation();
                _this.removeLeaf($li);
            });

            $divShade.append([$btnDelete,$btnInfo]);
            $div.append([$divInfo,$divShade]).appendTo($li);

            $divInfo.append([
                "<span style='width: 55%;text-overflow: ellipsis;overflow: hidden;white-space: nowrap;vertical-align: bottom'><span style='margin-right: 5px;margin-left: " + left + "px'>" + plan.runModeText +"</span><span>" + plan.output + "</span></span>"
                ,"<span style='width: 15%;'>" + plan.responsibleUserName + "</span>"
                ,"<span style='width: 30%;'>" + plan.endTime + "</span>"
            ]);
            $div.hover(function(){
                $("div:eq(1)",this).show();
            },function(){
                $("div:eq(1)",this).hide();
            });
            $div.click(function(){
                var $clicked = $("div:eq(0)",$(this));
                if($clicked.hasClass("focus")){
                    $clicked.removeClass("focus");
                    _this.selected = undefined;
                }else{
                    $("div.focus",$("#addplan_list_table")).removeClass("focus");
                    $clicked.addClass("focus");
                    _this.selected = $div;
                }
            });

            $con.append($li);
        };
        this.addToRoot = function(plan){
            if(!$container.has($ul).length){
                $container.html($ul);
            }
            _this.addLeaf($ul,plan);
        };
        this.addToSelected = function(plan){
            if(_this.selected){
                var $next = _this.selected.next();

                if($next && $next.length && $next.is("ul")){
                    _this.addLeaf($next,plan);
                }else{
                    var left = parseInt($(".leaf-info>span>span:eq(0)",_this.selected).css("margin-left"));
                    var $unfold = $("<em class='circle mimusCircle'></em>").css({left:left - 22 + "px"});
                    var $ul = $("<ul></ul>");
                    _this.selected.after($ul);
                    _this.addLeaf($ul,plan);

                    $unfold.click(function(e){
                        $ul.slideToggle();
                        if($(this).hasClass("mimusCircle")){
                            $(this).removeClass("mimusCircle").addClass("plusCircle");
                        }else{
                            $(this).removeClass("plusCircle").addClass("mimusCircle");
                        }
                        e.stopPropagation();
                    });

                    $(".leaf-info>span:eq(0)",_this.selected).prepend($unfold);
                }
            }else{
                ncUnits.alert("请选择节点");
            }
        };
        this.modify = function(plan){
            if(_this.modifying){
                _this.modifying.data("data-plan",plan)
                var $info = $(".leaf-info",_this.modifying);
                $(">span>span:eq(0)",$info).html(plan.runModeText);
                $(">span>span:eq(1)",$info).html(plan.output);
                $(">span:eq(1)",$info).html(plan.responsibleUserName);
                $(">span:eq(2)",$info).html(plan.endTime);
            }else{
                ncUnits.alert("找不到该计划");
            }
        };
        this.removeLeaf = function($li){
            $li.remove();
            if(!$li.siblings().length){
                var $ul = $li.parent();

                var $storage = $ul.siblings(".storage");
                if($storage.length){
                    $("span em",$storage).remove();
                }

                $ul.remove();
            }
        };
        this.getData = function(){
            var plans = [];
            analysisTree($ul,plans);
            return plans;
        };
        this.getSelected = function(){
            return _this.selected;
        };
        function analysisTree(ul,plans){
            $(">li",ul).each(function(){
                var plan = $(this).children(".storage").data("data-plan");
                var cul = $(this).children("ul");
                if(cul.length){
                    plan.children = [];
                    analysisTree(cul,plan.children);
                }
                plans.push(plan);
            })
        }
    }

    var planCache = new PlanCache("#addplan_list_table");

    //function renderLeaf($jdom,data,tier,subscript){
    //    tier = tier || 0;
    //    tier ++;
    //    var $ul = $("<ul></ul>");
    //    for(var i= 0,len = data.length;i < len;i++){
    //        var plan = data[i];
    //        var $li = $("<li></li>").css({position:"relative"});
    //        var $div = $("<div style='line-height: 20px'></div>");
    //        var $divInfo = $("<div></div>").css({padding:"5px 0"});
    //        var $divShade = $("<div></div>").css({position:"absolute",width:"100%",background:"#000",opacity:0.7,top:0,display:"none"});
    //
    //        var $btnDelete = $("<span>删除</span>").css({color:"#fff",float:"right",padding:"5px 10px",cursor: "pointer"});
    //        var $btnInfo = $("<span>详情</span>").css({color:"#fff",float:"right",padding:"5px 10px",cursor: "pointer"});
    //
    //        $btnInfo.click(i,function(e){
    //            e.stopPropagation();
    //            console.log(data[e.data]);
    //        });
    //        $btnDelete.click(i,function(e){
    //            e.stopPropagation();
    //            data.splice(e.data,1,undefined);
    //            $li.remove();
    //        });
    //
    //        $divShade.append([$btnDelete,$btnInfo]);
    //        $div.append([$divInfo,$divShade]).appendTo($li);
    //
    //        var $unfold = "";
    //        if(plan.children && plan.children.length){
    //            $unfold = $("<em class='mimusCircle'></em>").css({left:(tier-1)*20});
    //            $unfold.click($li,function(e){
    //                $(">ul",e.data).slideToggle();
    //                $(this).toggleClass("plusCircle");
    //                e.stopPropagation();
    //            });
    //        }
    //        $divInfo.append([
    //            $("<span style='width: 50%;display: inline-block;'></span>").append([$unfold,"<span style='margin-right: 5px;margin-left: " + (tier*20 + 2) + "px'>" + plan.runModeText +"</span>","<span>" + plan.output + "</span>"])
    //            ,"<span style='width: 20%;display: inline-block;'>" + plan.responsibleUserName + "</span>"
    //            ,"<span style='width: 30%;display: inline-block;'>" + plan.endTime + "</span>"
    //        ]);
    //        $div.hover(function(){
    //            $("div:eq(1)",this).show();
    //        },function(){
    //            $("div:eq(1)",this).hide();
    //        });
    //        $div.click(function(){
    //            var $clicked = $("div:eq(0)",$(this));
    //            if($clicked.hasClass("focus")){
    //                $clicked.removeClass("focus");
    //            }else{
    //                $("div.focus",$("#addplan_list_table")).removeClass("focus");
    //                $clicked.addClass("focus");
    //            }
    //        });
    //        if(plan.children && plan.children.length){
    //            renderLeaf($li,plan.children,tier);
    //        }
    //        $ul.append($li);
    //    }
    //    $jdom.append($ul);
    //}

    /* 表单验证 开始 */

    function validate_addplan(){
        //var pass = true;
        console.log('end222Time_v', endTime_v)
        layer.closeTips();
        if(department_v == undefined){
            validate_reject('请选择部门', "#addplan_department_v");
            return false;
        }
        if(project_v == undefined){
            validate_reject('请选择项目', "#addplan_project_v");
            return false;
        }
        if(runMode_v == undefined || runMode_v == ""){
            validate_reject('请选择执行方式', "#runmode");
            return false;
        }
        if(output_v == undefined || output_v == ""){
            validate_reject('请填写事项输出结果', "#output");
            return false;
        }
        if(roundType_v == undefined || roundType_v == ""){
            validate_reject('请选择循环类型', "#roundtype");
            return false;
        }else if(roundType_v == 0){
            if(isTmp_v == undefined){
                validate_reject('请选择是否临时计划', "#isTmp");
                return false;
            }
        }else{
            if(roundTime_v == undefined){
                validate_reject('请输入循环时间', "#addplan_roundtime_v");
                return false;
            }
            if(workTime_v == undefined || workTime_v == ""){
                validate_reject('请输入单位完成工时', "#worktime");
                return false;
            }
        }
        if (endTime_v == undefined) {
            console.log('endTime_v', endTime_v)
            validate_reject('请输入完成时间', "#addplan_endtime_v");
            return false;
        }
        if (responsibleUser_v == undefined || responsibleUser_v == 0) {
            validate_reject('请选择责任人', "#addplan_responsibleUser");
            return false;
        }
        if (confirmUser_v == undefined||confirmUser_v==0) {
            validate_reject('请选择确认人', "#addplan_confirmUser");
            return false;
        }
        return true;
    }
    /* 表单验证 结束 */

    //添加
    $("#addplan_add").hover(function(){
        $(this).children().toggle();
    });
    $("#addplan_addmain").click(function(){
        output_v = $("#output").val().trim();
        workTime_v = $("#worktime").val();
        if(!validate_addplan()){
            return;
        }

        planCache.addToRoot({
            department:department_v
            ,project:project_v
            ,runMode:runMode_v
            ,output:output_v
            ,roundType:roundType_v
            ,roundTime:roundTime_v
            ,endTime:endTime_v
            ,responsibleUser:responsibleUser_v
            ,confirmUser:confirmUser_v
            ,isTmp:isTmp_v
            ,workTime:workTime_v
            ,partner: $.extend([],partner_v)
            ,premise:$.extend([],premise_v)
            ,tab:$.extend([],tab_v)


            , departmentText: department_text
            , projectText: project_text
            , runModeText: runMode_text
            , roundTypeText: roundType_text
            , responsibleUserName: responsibleUser_name
            , confirmUserName: confirmUser_name
            , partnerName: $.extend([], partner_name)
            , premiseText: $.extend([], premise_text)
            , meetingId: meetingId ? meetingId : null
        });
        //planCache();
        //console.log(JSON.stringify(plans));

        $("#addplan_list").show();
        layer.area(layIndex_addPlan,{width:960 ,height:630});
    });
    $("#addplan_addsub").click(function(){
        output_v = $("#output").val().trim();
        workTime_v = $("#worktime").val();

        if(!validate_addplan()){
            return;
        }

        if(endTime_v > planCache.getSelected().data("data-plan").endTime){
            validate_reject('完成时间超出了主计划的完成时间，请重设', "#addplan_endtime_v");
            return;
        }

        planCache.addToSelected({
            department:department_v
            ,project:project_v
            ,runMode:runMode_v
            ,output:output_v
            ,roundType:roundType_v
            ,roundTime:roundTime_v
            ,endTime:endTime_v
            ,responsibleUser:responsibleUser_v
            ,confirmUser:confirmUser_v
            ,isTmp:isTmp_v
            ,workTime:workTime_v
            ,partner: $.extend([],partner_v)
            ,premise:$.extend([],premise_v)
            ,tab:$.extend([],tab_v)

            , departmentText: department_text
            , projectText: project_text
            , runModeText: runMode_text
            , roundTypeText: roundType_text
            , responsibleUserName: responsibleUser_name
            , confirmUserName: confirmUser_name
            , partnerName: $.extend([], partner_name)
            , premiseText: $.extend([], premise_text)
            , meetingId: meetingId ? meetingId : null
        });

        //planCache();
        //console.log(JSON.stringify(plans));

        $("#addplan_list").show();
        layer.area(layIndex_addPlan,{width:960 ,height:630});
    });

    //提交
    $("#addplan_submit").off('click');
    $("#addplan_submit").click(function () {

        var submitData;
        if (planCache.getData()&&planCache.getData().length == 0) {
            output_v = $("#output").val().trim();
            workTime_v = $("#worktime").val();

            if (!validate_addplan()) {
                return;
            }
            

            submitData = [{
                department: department_v
                , project: project_v
                , runMode: runMode_v
                , output: output_v
                , roundType: roundType_v
                , roundTime: roundTime_v
                , endTime: endTime_v
                , responsibleUser: responsibleUser_v
                , confirmUser: confirmUser_v
                , isTmp: isTmp_v
                , workTime: workTime_v
                , partner: $.extend([], partner_v)
                , premise: $.extend([], premise_v)
                , keyword: $.extend([], tab_v)

                , departmentText: department_text
                , projectText: project_text
                , runModeText: runMode_text
                , roundTypeText: roundType_text
                , responsibleUserName: responsibleUser_name
                , confirmUserName: confirmUser_name
                , partnerName: $.extend([], partner_name)
                , premiseText: $.extend([], premise_text)
                ,meetingId:meetingId?meetingId:null
            }]
        } else {
            submitData = planCache.getData();
        }

        var loading_layer_index_1 = getLoadingPosition("#layer_addplan");

        $.ajax({
            type: "post",
            url: '/BuildNewPlan/SavePlan',
            dataType: "json",
            data:{
                param:JSON.stringify(submitData),
                submit:10
            },
            success: rsHandler(function (data) {
                ncUnits.alert("计划提交成功");
                layer.close(layIndex_addPlan);
                $("#addplan_list").hide();
                if (page_flag && page_flag == "CalendarProcess") {
                    loadingPlanList();
                } else if (page_flag == 'plan') {
                    fnScreCon();
                } else {
                    $("#addplan_submit").trigger('addPlan')
                }
            }),
            complete: rcHandler(function () {
                loading_layer_index_1.remove();         //关闭load层
            })
        });
    });
    //保存
    $("#addplan_save").click(function(){
        var submitData;
        if (planCache.getData().length == 0) {
            output_v = $("#output").val().trim();
            workTime_v = $("#worktime").val();

            if (!validate_addplan()) {
                return;
            }

            submitData = [{
                department: department_v
                , project: project_v
                , runMode: runMode_v
                , output: output_v
                , roundType: roundType_v
                , roundTime: roundTime_v
                , endTime: endTime_v
                , responsibleUser: responsibleUser_v
                , confirmUser: confirmUser_v
                , isTmp: isTmp_v
                , workTime: workTime_v
                , partner: $.extend([], partner_v)
                , premise: $.extend([], premise_v)

                , keyword: $.extend([], tab_v)

                , departmentText: department_text
                , projectText: project_text
                , runModeText: runMode_text
                , roundTypeText: roundType_text
                , responsibleUserName: responsibleUser_name
                , confirmUserName: confirmUser_name
                , partnerName: $.extend([], partner_name)
                , premiseText: $.extend([], premise_text)
                , meetingId: meetingId ? meetingId : null
            }]
        } else {
            submitData = planCache.getData();
        }

        var loading_layer_index_1 = getLoadingPosition("#layer_addplan");

        $.ajax({
            type: "post",
            url: '/BuildNewPlan/SavePlan',
            dataType: "json",
            data:{
                param:JSON.stringify(submitData),
                submit:0
            },
            success: rsHandler(function (data) {
                ncUnits.alert("计划保存成功");
                layer.close(layIndex_addPlan);
                $("#addplan_list").hide();
                if (page_flag && page_flag == "CalendarProcess") {
                    loadingPlanList();
                } else if (page_flag == 'plan') {
                    fnScreCon();
                } else {
                    $('#addplan_save').trigger('addPlan')
                }
                
            }),
            complete: rcHandler(function () {
                loading_layer_index_1.remove();         //关闭load层
            })
        });
    });
    //修改
    $("#addplan_modify").click(function(){
        output_v = $("#output").val().trim();
        workTime_v = $("#worktime").val();

        if (!validate_addplan()) {
            return;
        }

        planCache.modify({
            department: department_v
            , project: project_v
            , runMode: runMode_v
            , output: output_v
            , roundType: roundType_v
            , roundTime: roundTime_v
            , endTime: endTime_v
            , responsibleUser: responsibleUser_v
            , confirmUser: confirmUser_v
            , isTmp: isTmp_v
            , workTime: workTime_v
            , partner: $.extend([], partner_v)
            , premise: $.extend([], premise_v)
            , tab: $.extend([], tab_v)

            , departmentText: department_text
            , projectText: project_text
            , runModeText: runMode_text
            , roundTypeText: roundType_text
            , responsibleUserName: responsibleUser_name
            , confirmUserName: confirmUser_name
            , partnerName: $.extend([], partner_name)
            , premiseText: $.extend([], premise_text)
        });
        planCache.modifying = undefined;
        $("#canCon_modify").hide();
        $("#canCon_add").show();

        ncUnits.alert("修改成功");
    })
    /* 新建计划 结束 */



    function initPlan(o,mid) {
        o = o || {
            partner: []                 //协作人
            , premise: []                  //前提计划
            , tab: []                      //标签

            , roundType: 0

            , partnerName: []
            , premiseText: []
        }
        , meetingId = mid
        , department_v = o.department              //部门值
        , project_v = o.project              //项目值
        , runMode_v = o.runMode               //执行方式
        , output_v = o.output                  //事项输出结果
        , roundType_v = o.roundType        //循环类型
        , roundTime_v = o.roundTime         //循环时间
        , endTime_v = o.endTime                 //完成时间
        , responsibleUser_v = o.responsibleUser         //责任人
        , confirmUser_v = o.confirmUser            //确认人
        , isTmp_v = o.isTmp                  //是否临时计划
        , workTime_v = o.workTime               //单位完成工时
        , partner_v = o.partner                 //协作人
        , premise_v = o.premise                  //前提计划
        , tab_v = o.tab                      //标签


        , department_text = o.departmentText
        , project_text = o.projectText
        , runMode_text = o.runModeText
        , roundType_text = o.roundTypeText
        , responsibleUser_name = o.responsibleUserName
        , confirmUser_name = o.confirmUserName
        , partner_name = o.partnerName
        , premise_text = o.premiseText;

        $("#addplan_department_v").html(o.departmentText || "");
        $("#addplan_project_v").html(o.projectText || "");
        $("#runmode").val(o.runMode == undefined ? "" : o.runMode);
        $("#output").val(o.output || "");
        $("#roundtype").val(o.roundType == undefined ? "" : o.roundType);
        $("#addplan_roundtime_v").val(o.roundTime || "");
        $("#addplan_endtime_v").val(o.endTime || "");
        $("#addplan_responsibleUser").val(o.responsibleUserName || "");
        $("#addplan_confirmUser").val(o.confirmUserName || "");
        $("#isTmp").val(o.isTmp || "");
        $("#worktime").val(o.workTime || "");

        $("#roundtype").change();

        $("#addplan_partner").parent().siblings().remove();
        $("#addplan_premise").parent().siblings().remove();
        $("#addplan_label").parent().siblings().remove();

        for (var i = 0, len = o.partner.length; i < len; i++) {
            //(function(){
            var id = o.partner[i];
            var name = o.partnerName[i];

            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

            $span.hover(function () {
                $("span", this).toggle();
            });

            $span.append([name, $close]);

            $("#addplan_partner").parent().before($span);

            $close.click({
                id: id,
                name: name
            }, function (e) {
                $(this).parent("span").remove();
                removeValue(partner_v, e.data.id);
                removeValue(partner_name, e.data.name);
            });
            //})();
        }

        for (var i = 0, len = o.premise.length; i < len; i++) {
            //(function(){
            var id = o.premise[i];
            var name = o.premiseText[i];

            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

            $span.hover(function () {
                $close.toggle();
            });

            $span.append([name, $close]);
            $("#addplan_premise").parent().before($span);

            $close.click({
                id: id,
                name: name
            }, function (e) {
                $(this).parent("span").remove();
                removeValue(premise_v, e.data.id);
                removeValue(premise_text, e.data.name);
            });
            //})();
        }

        for (var i = 0, len = o.tab.length; i < len; i++) {
            //(function(){
            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

            $span.hover(function () {
                $close.toggle();
            });

            $span.append([o.tab[i], $close]);
            $("#addplan_label").parent().before($span);

            $close.click(o.tab[i], function () {
                $(this).parent("span").remove();
                removeValue(tab_v, e.data);
            });
            //})();
        }

        $("#layer_addplan em.arrowsBBTCom").removeClass("arrowsBBTCom").addClass("arrowsBBBCom");
        $("#layer_addplan .ztree").hide();
        if ($.fn.zTree.getZTreeObj("addplan_department")) {
            $.fn.zTree.getZTreeObj("addplan_department").cancelSelectedNode();
        }
        if ($.fn.zTree.getZTreeObj("addplan_project")) {
            $.fn.zTree.getZTreeObj("addplan_project").cancelSelectedNode();
        }
        
        loadDefault();
    }

    function closePlan() {
        layer.close(layIndex_addPlan);
        layer.closeTips();
        $("#addplan_list").hide();
    }

    function loadDefault() {
        var $departmentOptions = $("#addplan_department_v~.select-options");
        var $projectOptions = $("#addplan_project_v~.select-options");
        $departmentOptions.html("<div>--- 没有常用数据 ---</div>");
        $projectOptions.html("<div>--- 没有常用数据 ---</div>");
        $.ajax({
            type: "post",
            url: "/Plan/GetAddUserinfo",
            dataType: "json",
            success: rsHandler(function (data) {

                if (data.orgNameList && data.orgNameList.length) {
                    $departmentOptions.empty();
                    $.each(data.orgNameList, function (i, v) {
                        if (i == 0) {
                            $("#addplan_department_v").html(v.organizationName);
                            department_v = v.organizationId;
                            department_text = v.organizationName;
                        }
                        var $li = $("<li>" + v.organizationName + "</li>");
                        $li.click(function () {
                            $("#addplan_department_v").html(v.organizationName);
                            department_v = v.organizationId;
                            department_text = v.organizationName;
                        })
                        $departmentOptions.append($li);
                    })
                }

                if (data.projectList && data.projectList.length) {
                    $projectOptions.empty();
                    $.each(data.projectList, function (i, v) {
                        if (i == 0) {
                            $("#addplan_project_v").html(v.projectName);
                            project_v = v.projectId;
                            project_text = v.projectName;
                        }
                        var $li = $("<li>" + v.projectName + "</li>");
                        $li.click(function () {
                            $("#addplan_project_v").html(v.projectName);
                            project_v = v.projectId;
                            project_text = v.projectName;
                        })
                        $projectOptions.append($li);
                    })
                }
                if (data.UpUser) {
                    confirmUser_v = data.UpUser.userId;
                    confirmUser_name = data.UpUser.userName;
                    $("#addplan_confirmUser").val(confirmUser_name);
                }
                if (data.DownUser) {
                    responsibleUser_v = data.DownUser.userId;
                    responsibleUser_name = data.DownUser.userName;
                    $("#addplan_responsibleUser").val(responsibleUser_name);
                }
            })
        });
    }
    return {
        addPlan: function (mid) {
            var temp = null; 
            initPlan(temp,mid);
            layIndex_addPlan = $.layer({
                type: 1,
                shade: [0.5, '#000'],
                area: ['auto', 'auto'],
                //title: ['新建计划','background:#58b456;color:#fff;'],
                title: false,
                border: [0],
                page: { dom: '#layer_addplan' },
                move: ".drapdiv",
                closeBtn: false
            });

            $("#layer_addplan .closeWCom").click(closePlan)
        }
    }
}

