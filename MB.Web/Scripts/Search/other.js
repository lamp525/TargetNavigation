
var current_Info;
var page_flag = "CalendarProcess";
var batch_flag = false;
var planModule = (function () {

    var SCConData = {
        status: [],		// 状态
        stop: [],         //0：运行中  10:申请中止 90：已中止
        time: [],		// 时间 $type:时间条件类型,int值（1：本周，2：本月，0：自选时间(从$starttime至$endtime)）
        person: [],		// 人员
        department: [],		// 部门分类
        project: [],		// 项目分类
        whoPlan: 0,
        soontype: [],       //快速排序  1:今日未完成 2：超时计划 
        sorts: [{ type: 8, direct: 0 }]
    };
    var setting = {
        view: {
            showIcon: false,
            showLine: false,
            selectedMulti: false
        }
    };
    var listArgus = {
        calendarType: 1,            //日程类别： 1：我的日程 2：下属日程
        date: String,
        status: 1                   //计划状态：1：待审核计划  2：待确认计划（我的日程无该参数）
    };
    var batch_planId = [], batch_loopId = [];
 
    var events = {
        'click .planDetail': detailAction,//计划详情
        'click .suspend': stopAction,//终止计划
        'click .modification': modiAction,//计划修改
        'click .transimitPlan': transAction,//计划转办
        'click .decPlan': splitAction, //分解计划
        'click .submit': submitAction, //提交计划
        'click .audit': auditAction, //审核计划
        'click .confirmPlan': confirmAction, //计划确认
        'click .submitTo': submitToAction, //计划确认
        'click .revocation': revocationAction, //计划撤销
        'click .restart': restartAction //重新开始
    }
    function editEnable() {
        $('.planDetailMask').css('display', 'none');
    }

    function revocationAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx];
        var status = $(this).attr('operate');
        ncUnits.confirm({
            title: "撤销", html: "确定要撤销吗?", yes: function (id) {
                canceloperate(current_Info.planId, status)
                layer.close(id);
            }
        });
    }

    function restartAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx];
        ncUnits.confirm({
            title: "重新开始", html: "确定要重新开始吗?", yes: function (id) {
                $.post("/Plan/StopOrStartPlan", { "planId": current_Info.planId, "stop": 0 }, function (msg) {
                    if (msg == "ok") {
                        ncUnits.alert("重新开始成功！");
                        view.updateList();
                    }
                    else {
                        ncUnits.alert("重新开始失败！");
                    }
                    layer.close(id);
                });
            }
        });

    }
    function submitAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx]
        if (current_Info.isLoopPlan == 0) {
            $('#xxc_planId').val(current_Info.planId);
        } else {
            $('#xxc_planId').val(current_Info.loopId);
        }
        $("#plan_detail_modal").modal("show");
        submitconfirm($("#plan_detail_modal"));

    }

    function submitToAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx];
        ncUnits.confirm({
            title: "提交", html: "确定要提交吗?", yes: function (id) {
                if (current_Info.isLoopPlan == 1) {   //循环计划
                    submitoperate(current_Info.loopId, current_Info.initial, current_Info.isLoopPlan);
                } else {
                    submitoperate(current_Info.planId, current_Info.initial);
                }
                layer.close(id);
            }, no: function (id) { layer.close(id) }
        });
    }

    function confirmAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx]
        setValue()
        $("#plan_detail_modal").modal("show");
        DetailConfirm($("#plan_detail_modal"));


    }


    function auditAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx]
        setValue()
        $("#plan_detail_modal").modal("show");
        checkclick($("#plan_detail_modal"));
    }



    function setValue() {
        $('#xxc_status').val(current_Info.status);
        $('#xxc_stop').val(current_Info.stop);
        if (current_Info.isLoopPlan == 0) {
            $('#xxc_planId').val(current_Info.planId);
        } else {
            $('#xxc_planId').val(current_Info.loopId);
        }
    }


    function stopAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx]
        var msg = {
            success: '申请终止成功',
            err: '申请终止失败',
            txt: '确认要申请终止吗？'
        }
        confirmControl("/Plan/StopOrStartPlan", { "planId": (current_Info.planId == null ? current_Info.loopId : current_Info.planId), "stop": 10 }, msg);

    }

    function modiAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx];
        var msg = {
            success: '申请修改成功',
            err: '申请修改失败',
            txt: '确认要申请修改吗？'
        }
        confirmControl("/Plan/ChangePlanStatus", { "planId": (current_Info.planId == null ? current_Info.loopId : current_Info.planId), "status": 25, flag: 1 }, msg);
    }


    function confirmControl(url, updata,msg) {
        var url = url, updata = updata, msg = msg;
        ncUnits.confirm({
            title: '提示',
            html: msg.txt,
            yes: function (layer_confirm) {
                layer.close(layer_confirm);
                $.ajax({
                    type: "post",
                    url: url,
                    dataType: "json",
                    data:updata ,
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert(msg.success);
                            //loadingPlanList();
                            view.updateList();
                        } else {
                            ncUnits.alert(msg.err);
                        }
                    })
                })
            }
        })


    }


    function splitAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx]
        $("#planSplit_modal").attr('term', current_Info.planId);
        $("#planSplit_modal").modal("show");

    }


    var bindEvents = function () {

        var delegateEventSplitter = /^(\S+)\s*(.*)$/;
        var k, method, eventName, selector, match;
        var el = $('#idifySearch');
        for (k in events) {
            method = events[k];
            match = k.match(delegateEventSplitter);
            eventName = match[1];
            selector = match[2];
            method = events[k];
            el.on(eventName, selector, method)
            
        }

    }()


    function transAction() {
        var idx = $(this).parents('.planListAll').index();
        current_Info = searchData[idx];
        $("#layer_Transmitplan").attr("data_planId", (current_Info.planId == null ? vcurrent_Info.loopId : current_Info.planId)).attr("parentTime", current_Info.endTime);
        $("#layer_Transmitplan").modal("show");
        
    }


    
    function detailAction(){
        var idx = $(this).parents('.planListAll').index();
       
        current_Info = searchData[idx];
        console.log(current_Info)
        if (current_Info.isLoopPlan == 0) {
            $('#xxc_planId').val(current_Info.planId);
        } else {
            $('#xxc_planId').val(current_Info.loopId);
        }
        $("#plan_detail_modal").modal("show");
    
    
    }


    $("#plan_detail_modal").on("hide.bs.modal", function () {
        $(".listView").empty();
        $("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
        $("#detail_front_div").find('span:not(.detail_front_span)').remove();
        $("#detailAccessory").hide();
        $("#detail_operateinfo").html('').hide();
        $("#plan_detail_modal .modal-footer").hide();
        //$("#detail_looptype").empty();
        batch_flag = false;
        batch_planId = [];
        batch_loopId = [];
    });

    var planDetailFirst = true;
    $("#plan_detail_modal").on("shown.bs.modal", function () {
        var collUsers = [];         //协作人
        var frontplans = [];            //前提计划
        var tab_v = [];      //标签

        if (planDetailFirst) {
            organizationLoad($("#details_department"), $("#details_department_v"), 1);
            organizationLoad($("#details_project"), $("#details_project_v"), 2);

            //确认责任人
            personChosen($("#detai_responsibleUser"), $("#detail_confirmUser"));
            personChosen($("#detail_confirmUser"), $("#detai_responsibleUser"));
            //协作人选择
            $("#detail_partner").searchPopup({
                url: "/plan/GetOfferUsers",
                hasImage: true,
                selectHandle: function (data) {
                    collUsers.push({ id: data.id });
                    var $span = $("<span class='collplan tag' term='" + data.id + "'></span>");
                    var $close = $("<span class='close' term='" + data.id + "'>x</span>");

                    $span.append([" " + data.name, $close]);

                    $(this).parent().before($span);

                    $close.click(function () {
                        for (var i = 0; i < collUsers.length; i++) {
                            if (collUsers[i].id == data.id) {
                                collUsers.splice(i, 1);
                            }
                        }
                        $span.remove();
                    });
                }
            });

            //执行方式加载
            runModeLoading($("#detail_runmode"), null);

            //完成时间
            var detail_end = {
                elem: '#detail_endtime', //需显示日期的元素选择器
                event: 'click', //触发事件
                format: 'YYYY-MM-DD hh:mm', //日期格式
                istime: true, //是否开启时间选择
                isclear: true, //是否显示清空
                istoday: true, //是否显示今天
                issure: true, //是否显示确认
                festival: true, //是否显示节日
                choose: function (dates) { //选择好日期的回调
                    detail_start.max = dates;
                }
            }
            $("#detail_endtime").off('click');
            $("#detail_endtime").click(function () {
                laydate(detail_end);
            });

            //循环时间
            var detail_start = {
                elem: '#detail_startime',
                event: 'click',
                format: 'YYYY-MM-DD',
                isclear: true,
                istoday: true,
                issure: true,
                festival: true,
                choose: function (dates) {
                    detail_end.start = dates;
                    detail_end.min = dates;
                }
            }
            $("#detail_startime").off('click');
            $("#detail_startime").click(function () {
                laydate(detail_start);
            });

            //前提计划选择
            $("#detail_premise").searchPopup({
                url: "/plan/GetFrontPlans",
                selectHandle: function (data) {
                    frontplans.push({ planId: data.planId });
                    var $span = $("<span class='frontplan tag' term='" + data.planId + "'></span>");
                    var $close = $("<span class='close'>x</span>");

                    $span.append([" " + data.eventOutput, $close]);
                    $(this).parent().before($span);

                    $close.click(function () {
                        for (var i = 0; i < frontplans.length; i++) {
                            if (frontplans[i].planId == data.planId) {
                                frontplans.splice(i, 1);
                            }
                        }
                        $span.remove();
                    });
                }
            });

            //addplan的标签设置
            $("#detail_label").off('click');
            $("#detail_label").click(function () {
                var $this = $(this);
                $this.hide();
                var $con = $("<div style='display: inline-block;position: relative;'></div>");
                var $input = $('<input type="text" style="border: 1px solid #e3e3e3;width: 100px;height: 22px"/>');
                var $em = $("<em style='border: 1px solid #00B83F;background-color: #00B83F;  position: absolute;right: 0px;'></em>").addClass("icon add-min-grey");

                function addtag() {

                    var $span = $("<span class='tag'></span>");
                    var $close = $("<span class='close'>x</span>");

                    var iv = $input.val();

                    $span.append([" " + iv, $close]);
                    $this.parent().before($span);

                    tab_v.push(iv);

                    $close.click(function () {
                        $span.remove();
                        removeValue(tab_v, iv);
                    });

                    $con.remove();
                    $this.show();
                }

                $input.keypress(function (e) {
                    if (e.keyCode == 13 && $(this).val()) {
                        addtag();
                    }
                });
                $em.click(function () {
                    if ($input.val()) {
                        addtag();
                    }
                });
                $con.append([$input, $em]).appendTo($this.parent());
            });

            //循环类型
            $("#roundtype").change(function () {
                var roundType_v = $(this).val();
                var roundType_text = $("option:selected", this).text();
                if (roundType_v == undefined || roundType_v == "") {
                    $(this).css({ color: "#a0a0a0" });
                    $("#addplan_roundtime").hide();
                    $("#worktimeDiv").hide();
                } else {
                    $(this).css({ color: "#686868" });
                    if (roundType_v == 0) {
                        $("#addplan_roundtime").hide();
                        $("#worktimeDiv").hide();
                    } else {
                        $("#addplan_roundtime").show();
                        $("#worktimeDiv").show();
                    }
                }
            });


            //获取计划详情
            function detail_window() {
                /*获取计划详情 开始*/
                var planId = current_Info.planId 
                var status = current_Info.status;
                var stop = current_Info.stop;
                var isloop = 0;
                //var withfront = 0;
                var withfront = current_Info.isFronPlan;
                var initial = current_Info.initial;
                //var collPlan = 0;
                var collPlan = current_Info.IsCollPlan;
                console.log('ppp',planId)
                $.ajax({
                    type: "post",
                    url: "/plan/GetPlanInfoByPlanId",
                    dataType: "json",
                    data: { planId: planId, isloop: isloop, withfront: withfront, collPlan: collPlan },
                    success: rsHandler(function (data) {
                        $("#details_department_v").text(data.organizationName).attr('term', data.organizationId);
                        $("#details_project_v").text(data.projectName).attr('term', data.projectId);
                        $("#detail_runmode option[value=" + data.executionModeId + "]").attr("selected", true);
                        $("#detail_eventoutput").val(data.eventOutput);
                        if (!data.isLoopPlan) {                         //不是循环计划
                            $("#detail_looptype option:eq(1)").attr("selected", "true");
                            $("#xxx_startime").hide();
                            $("#xxx_initial").show();
                            $("#detail_initial option[value=" + data.initial + "]").attr("selected", "true");
                        }
                        else {                       //是循环计划
                            $("#detail_looptype option[value=" + data.loopType + "]").attr("selected", true);
                            $("#xxx_startime").show();
                            $("#detail_startime").val(data.startTimeNew);
                            $("#xxx_initial").hide();
                        }
                        $("#detail_endtime").val(data.endTimeNew);
                        $("#detail_responsibleUser").val(data.responsibleUserName).attr('term', data.responsibleUser);
                        $("#detail_confirmUser").val(data.confirmUserName).attr('term', data.confirmUser);

                        if (data.collPlanList.length > 0) {           //协作人
                            for (var i = 0; i < data.collPlanList.length; i++) {
                                var $span = $("<span class='collplan tag' term='" + data.collPlanList[i].id + "'></span>");
                                var $close = $("<span class='close' term='" + data.collPlanList[i].id + "'>x</span>");
                                $span.append([" " + data.collPlanList[i].name, $close]);

                                $("#detail_partner").parent().before($span);
                                collUsers.push({ id: data.collPlanList[i].id });
                                $close.click(function () {
                                    var collplanId = $(this).attr('term');
                                    for (var i = 0; i < collUsers.length; i++) {
                                        if (collUsers[i].id == collplanId) {
                                            collUsers.splice(i, 1);
                                        }
                                    }
                                    $(this).parent("span").remove();
                                });
                            }
                        }

                        if (data.withFront) {                   //前提计划
                            for (var i = 0; i < data.frontLists.length; i++) {
                                var $span = $("<span class='frontplan tag' term='" + data.frontLists[i].planId + "'></span>");
                                var $close = $("<span class='close'  term='" + data.frontLists[i].planId + "'>x</span>");
                                $span.append([" " + data.frontLists[i].eventOutput, $close]);
                                $("#detail_premise").parent().before($span);
                                frontplans.push({ planId: data.frontLists[i].planId });
                                $close.click(function () {
                                    var frontplanId = $(this).attr('term');

                                    for (var i = 0; i < frontplans.length; i++) {
                                        if (frontplans[i].planId == frontplanId) {
                                            frontplans.splice(i, 1);
                                        }
                                    }
                                    $(this).parent().remove();
                                    //                                    removeValue(premise_v, data.id);
                                    //                                    removeValue(premise_text, data.name);
                                });
                            }
                        }

                        //加载附件
                        if (data.planAttachmentList.length <= 0) {
                            $("#detailAccessory").hide()
                        }
                        else {
                            $("#detailAccessory").show();
                            var $allLoad = $("<li class='allDownload'>全部下载</li>");
                            $allLoad.click(function () {
                                downloadaddfile(planId);
                            });
                            $('#detailAccessory .accessoryDiv ul').empty().append($allLoad);
                            for (var i = 0; i < data.planAttachmentList.length; i++) {
                                var $spanDoc = $("<li class='liAccessory' savename='" + data.planAttachmentList[i].saveName + "' term='" + data.planAttachmentList[i].attachmentId + "'>" +
                                    "<span class='content'>" + data.planAttachmentList[i].attachmentName + "</span></li>");
                                $spanDoc.click(function () {
                                    downloadfile($(this));
                                });
                                $('#detailAccessory .accessoryDiv ul').append($spanDoc);
                            }
                        }
                    })
                });
                /*获取计划详情 结束*/

                //计划日志
                $.ajax({
                    type: "post",
                    url: "/plan/GetPlanOperates",
                    dataType: "json",
                    data: {
                        planId: planId, isloop: isloop
                    },
                    success: rsHandler(function (data) {
                        var $ul = $("<ul class='list-unstyled'></ul>");
                        var msgList = ['提交了该计划', "审核通过了该计划", "审核未通过该计划", "撤销了该计划的提交", "撤销了该计划的审核", "发表了评论", "下载了附件", "查看了你的计划"
                                        , "转办", "申请修改该计划", "申请中止该计划", "重新开始该计划", "删除该计划", "该计划确认通过", "该计划确认未通过", "该计划更新了进度", "该计划进行了分解计划"
                                        , "新建了计划", "新建了循环计划", "修改保存了该计划"];
                        for (var i = 0, len = data.length; i < len; i++) {
                            var info = data[i];
                            var $li = $("<li></li>");
                            var $span1 = $("<span style='width: 50%;max-width:420px' class='textOver' title=" + info.message + "></span>");
                            var $span2 = $("<span style='width: 50%;text-align: -webkit-right;float:right;'></span>");
                            $li.append([$span1, $span2]).appendTo($ul);
                            var str = "<span style='color:#58b456;'>" + info.user + "</span>";
                            var txt = info.type ? msgList[info.type - 1] : '异常操作';
                            var message = info.message ? (":" + info.message) : "";
                            str += txt + message;                                           
                            $span1.html(str);
                            $span2.html(info.time.replace('T', ' ').substr(0, 16));
                        }
                        $("#plan_detail_modal .listView").html($ul);
                    })
                });

          

                if (collPlan == 1) {  //协作计划
                    editDisabled();
                    if (status == 90 || stop == 90) {
                        $("#plan_detail_modal .modal-footer").hide();
                        return;
                    }
                    $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%").siblings().hide();
                    return;
                }

                if (listArgus.calendarType == 1) {  //我的日程                    
                    switch (true) {
                        case (status == 0 || status == 15) && stop == 0: //待提交
                            editEnable();
                            $("#plan_detail_modal .modal-footer").show();
                            $("#xxc_initial").val(initial);
                            $("#detail_looptype").attr('disabled', 'disabled');
                            if (isloop == "1") {
                                $("#detail_span1").text('提交').attr('operatetype', '提交').css("width", "50%");
                                $("#detail_span2").text('保存').attr('operatetype', '保存').css("width", "50%");
                                $("#detail_span3").hide();
                                return;
                            } 
                            $("#detail_span1").text('提交').attr('operatetype', '提交').css("width", "33.3%");
                            $("#detail_span2").text('保存').attr('operatetype', '保存').css("width", "33.3%");
                            $("#detail_span3").text('评论').attr('operatetype', '评论').css("width", "33.3%");
                            break;
                        case ((status == 10 || status == 25) && stop == 0) || stop == 10: //待审核
                            editEnable();
                            $("#plan_detail_modal .modal-footer").show();
                            $("#detail_span2").hide();
                            $("#detail_span3").hide();
                            if (isloop == "1") {
                                $("#detail_span1").hide();
                                return;
                            }
                            $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                            break;
                        case (status == 20 || status == 40) && stop == 0: //已审核
                            editEnable();
                            $("#plan_detail_modal .modal-footer").show();
                            $("#detail_span3").hide();
                            if (isloop == "1") {
                                $("#detail_span1").text('提交').attr('operatetype', '提交确认').css("width", "100%");
                                $("#detail_span2").hide();
                                return;
                            }
                            $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%");
                            $("#detail_span1").text('提交').attr('operatetype', '提交确认').css("width", "50%");
                            break;
                        case status == 30 && stop == 0: //待确认
                            editEnable();
                            $("#plan_detail_modal .modal-footer").show();
                            $("#detail_span2").hide();
                            $("#detail_span3").hide();
                            if (isloop == "1") {
                                $("#detail_span1").hide();
                                return;
                            } 
                            $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                            break
                        case (status == 90 && stop == 0) || stop == 90:
                            editEnable();
                            break;
                        
                                                    
                    }
                    return;
                }

                if (listArgus.calendarType == 2) { //下属日程
                    switch (true) {
                        case ((status == 10 || status == 25) && stop == 0) || stop == 10: //待审核
                            $("#detail_span3").hide();
                            editDisabled();
                            $("#plan_detail_modal .modal-footer").show();
                            if (isloop == "1") {
                                $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '100%');
                                $("#detail_span2").hide();
                                return;
                            }
                            $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '50%');
                            $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '50%');                                                  
                            break;
                        case (status == 30 && stop == 0) || (isloop == "1" && current_Info.status == 20): //待确认
                            $("#plan_detail_modal .modal-footer").show();
                            if (isloop == "1") {
                                $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "100%");
                                $("#detail_span2").hide();
                            } else {
                                $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%");
                                $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "50%");
                            }
                            $("#detail_span3").hide();
                            editDisabled();
                            break;


                    }
                    
                }


            }

            //详情按钮事件
            $('.xxc_operatebtn').off('click');
            $('.xxc_operatebtn').click(function () {
                var operateType = $(this).attr("operateType");
                var planId = current_Info.planId;
                var org = $("#details_department_v").attr('term');
                var pro = $("#details_project_v").attr("term");
                var runmode = $("#detail_runmode option:selected").val();
                var eventoutput = $("#detail_eventoutput").val();
                var resUser = $("#detail_responsibleUser").attr('term');
                var conUser = $("#detail_confirmUser").attr('term');
                var looptype = $("#detail_looptype option:selected").val();
                var endtime = $("#detail_endtime").val();
                var detail_initial = $("#detail_initial option:selected").val();
                var plan = {
                    planId: planId,
                    organizationId: org,
                    projectId: pro,
                    executionModeId: runmode,
                    eventOutput: eventoutput,
                    responsibleUser: resUser,
                    confirmUser: conUser,
                    loopType: looptype,
                    initial: initial,
                    endTime: endtime,
                    collPlanList: collUsers,
                    frontLists: frontplans
                };
                var plans = [];
                var planList = [plan];
                if (operateType == "提交") {
                    if (org == "" || pro == "" || runmode == "" || eventoutput == "" || resUser == "" || conUser == "" || looptype == "" || endtime == "" || initial == "") {
                        ncUnits.alert("存在空值，无法提交");
                        return;
                    }
                    if (resUser == conUser) {
                        ncUnits.alert("确认人不能与责任人相同，请重新选择");
                        return;
                    }
                    var initial = $("#xxc_initial").val();
                    plans.push(plan.collPlanList);

                    $.ajax({
                        type: "post",
                        url: "/plan/SaveUpdatePlan",
                        dataType: "json",
                        data: { plans: JSON.stringify(planList) },
                        success: rsHandler(function (data) {
                            if (data) {
                                $("#detailAccessory").hide();
                                $("#detail_partner").html('');
                                $("#detail_premise").html('');
                                $("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
                                $("#detail_front_div").find('span:not(.detail_front_span)').remove();
                                ncUnits.alert("计划已成功提交");
                                submitoperate(planId, detail_initial);
                                $("#plan_detail_modal").modal("hide");

                            } else {
                                ncUnits.alert("提交不成功");
                            }
                        })
                    });
                }
                else if (operateType == "保存") {
                    plans.push(plan);
                    $.ajax({
                        type: "post",
                        url: "/plan/SaveUpdatePlan",
                        dataType: "json",
                        data: { plans: JSON.stringify(plans) },
                        success: rsHandler(function (data) {
                            if (data) {
                                $("#detailAccessory").hide();
                                $("#detail_front_div").find('span:not(.detail_front_span)').remove();
                                ncUnits.alert("保存成功");
                                $("#plan_detail_modal").modal("hide");
                                //loadingPlanList();
                                view.updateList();
                            } else {
                                ncUnits.alert("保存失败");
                            }
                        })
                    });
                }
                else if (operateType == "评论") {
                    Discuss($("#plan_detail_modal"), planId);
                }
                else if (operateType == "审核") {
                    checkclick($("#plan_detail_modal"));
                }
                else if (operateType == "确认") {
                    DetailConfirm($("#plan_detail_modal"));
                }
                else if (operateType == "提交确认") {
                    submitconfirm($("#plan_detail_modal"));
                }
                else if (operateType == "撤销") {
                    $("#detailAccessory").hide();
                    canceloperate(planId, 10);
                    ncUnits.alert("计划已撤销");
                }
                collUsers = [];
                frontplans = [];
                plans = [];
            });
            planDetailFirst = false;
        }
        detail_window();
    });


    //确认
    function DetailConfirm(obj) {
        $("#detail_operateinfo").load("/plan/GetConfirmView", { height: parseInt(obj.height() - 160) }, function () {
            $('#detail_operateinfo').show();
        });
    }

    //提交确认
    function submitconfirm(obj) {
        $("#detail_operateinfo").load("/plan/GetSubmitConfirmView", { height: parseInt(obj.height() - 160) }, function () {
            $('#detail_operateinfo').show();
        });
    }


    //提交
    function submitoperate(planIdNew, initial) {
        console.log('inital',initial)
        $.ajax({
            url: "/Plan/SubmitPlan",
            type: "post",
            dataType: "json",
            data: { "planId": planIdNew, "initial": initial },
            success: rsHandler(function (data) {
                if (data) {
                    ncUnits.alert("计划已成功提交！");
                    //loadingPlanList();
                    view.updateList();
                } else {
                    ncUnits.alert("计划提交失败！");
                }
            })
        });
    }

    //设置计划详情界面不可编辑
    function editDisabled() {
        $('.planDetailMask').css('display', 'block');
    }
    //设置计划详情界面可编辑
    function editEnable() {
        $('.planDetailMask').css('display', 'none');
    }

    var selectUserTip;
    function personChosen($responsibleUser, $confirmUser) {
        $responsibleUser.searchPopup({
            url: "/Plan/GetOfferUsers",
            defText: "常用联系人",
            hasImage: true,
            selectHandle: function (data) {
                layer.close(selectUserTip);
                if (data.id == $confirmUser.attr("term")) {
                    selectUserTip = validate_reject('责任人与确认人不能为同一人', $(this));
                    $(this).val('').attr("term", '');
                } else {
                    $(this).attr("term", data.id);
                    $(this).val(data.name);
                }
            }
        });
    }

    //审核
    function checkclick(obj, check_flag) {
        if (check_flag) {    //批量审核
            $("#detail_operateinfo").load("/plan/GetCheckView", { height: parseInt(obj.height() - 205) }, function () {
                $('#detail_operateinfo').show();
                $.ajax({
                    type: "post",
                    url: "/CalendarProcess/GetPlanSimpleList",
                    dataType: "json",
                    data: { plans: JSON.stringify(batch_planId), loops: JSON.stringify(batch_loopId) },
                    success: rsHandler(function (data) {
                        if (data && data.length > 0) {
                            $('.calendar_batch').show();
                            $('.plan_selected').empty();
                            $.each(data, function (i, v) {
                                var $plan = $("<div class='check_name' planId='" + v.planId + "' isloop='" + v.isLoopPlan + "'><span>" + v.responseName + "</span><span class='check_eventoutput'>" + v.eventOutPut + "</span></div>");
                                if (v.planId == current_Info.planId) {
                                    $plan.addClass('greenColor');
                                }
                                $plan.click(function () {
                                    $(this).addClass('greenColor').siblings().removeClass('greenColor');
                                    //获取详情
                                    var planId = $(this).attr('planId');
                                    var isloop = $(this).attr('isloop');
                                    getplandetail(planId, isloop);
                                });
                                $('.plan_selected').prepend($plan);
                            });
                        }
                    })
                });
            });
        } else {     //单个审核
            console.log('asasswwwww')
            $("#detail_operateinfo").load("/plan/GetCheckView", { height: parseInt(obj.height() - 160) }, function () {
                $('#detail_operateinfo').show();

            });
        }


    }



    //转办
    /*------开始--------*/
    //责任人
    personChosen($("#transmitplan_responsibleUser"), $("#transmitplan_confirmUser"));
    //确认人
    personChosen($("#transmitplan_confirmUser"), $("#transmitplan_responsibleUser"));

    //确定
    $("#xxc_makesure").click(function () {
        var planIdNew = $("#layer_Transmitplan").attr("data_planId");
        var responsibleUser_t = $("#transmitplan_responsibleUser").attr("term");
        var confirmUser_t = $("#transmitplan_confirmUser").attr("term");
        if (!responsibleUser_t) {
            ncUnits.alert("请选择责任人");
            console.log(1)
            return;
        }
        else if (!confirmUser_t) {
            ncUnits.alert("请选择确认人");
            return;
        }
        $.ajax({
            type: "post",
            url: "/Plan/ChangeToDo",
            dataType: "json",
            data: { planId: parseInt(planIdNew), responseUser: parseInt(responsibleUser_t), confirmUser: parseInt(confirmUser_t) },
            success: rsHandler(function (data) {
                if (data) {
                    ncUnits.alert("计划转办成功！");
                    $("#layer_Transmitplan").modal("hide");
                    view.updateList();
                    //loadingPlanList();
                }
                else {
                    ncUnits.alert("计划转办失败！");
                }
            })
        });
    });
   
    //部门分类(flag==1) 项目分类(flag==2)的tree
    function organizationLoad($ztree, $chosenSpan, flag) {
        if (flag == 1) {          //部门加载
            var url = "/plan/GetOrganizationInfo";
        } else if (flag == 2) {       //项目分类加载
            var url = "/plan/GetProjectInfo";
        }
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            success: rsHandler(function (data) {
                $.fn.zTree.init($ztree, $.extend({
                    callback: {
                        onClick: function (e, id, node) {
                            $chosenSpan.html(getNodeNameLine(node)).attr('term', node.id);
                            $ztree.prev().find("em.glyphicon").trigger("click");
                        }
                    }
                }, setting), data);
            })
        });
    }

    //执行方式加载
    function runModeLoading($runModeSelect, planDetailExemode) {
        $.ajax({
            type: "post",
            url: "/plan/GetExecutionList",
            dataType: "json",
            success: rsHandler(function (data) {
                $runModeSelect.empty();
                for (var i = 0, len = data.length; i < len ; i++) {
                    $("<option></option>").val(data[i].id).html(data[i].text).appendTo($runModeSelect);
                }
                if (planDetailExemode != null) {
                    $runModeSelect.find("option").each(function () {
                        if (parseInt($(this).val()) == planDetailExemode) {
                            $(this).attr("selected", "true");
                        }
                    });
                }
            })
        });
    }


    function getNodeNameLine(node, nameLine) {
        if (node) {
            return getNodeNameLine(node.getParentNode(), nameLine ? node.name + " - " + nameLine : node.name);
        } else {
            return nameLine;
        }
    }
    //取消
    $("#layer_Transmitplan").on("hide.bs.modal", function () {
        $("#transmitplan_responsibleUser,#transmitplan_confirmUser").val('').attr("term", '');
        layer.close(selectUserTip);
    })

    //计划详情的评论
    function Discuss(obj, planId) {
        $("#detail_operateinfo").load("/plan/DiscussView", { height: parseInt(obj.height() - 160), planId: planId }, function () {
            $("#detail_operateinfo").show();
            $(".commentBox").show();
            $(".commentBox .discussCon").each(function () {
                if ($(this).height() != 16) {
                    $(this).parent('.discuss').height($(this).height() + 15);
                }
            });
            $('.commentDiv').each(function (index, element) {
                var height = $('.discuss', this).height();
                var replyHeight = $('.replyDiv', this).height();
                $('.discuss', this).height(height);
                $(this).height(height + 20 + replyHeight);
                if ($(this).index() == 0) {
                    $(this).css({ 'margin-top': '95px' });
                }
                if (height != 30) {
                    $('.botComent', this).css({ 'margin-left': '50px' });
                }
            });
        });
    }


    //计划详情的提交撤销
    function canceloperate(planId, status) {
        $.ajax({
            url: "/Plan/ChangePlanStatus",
            type: "post",
            dataType: "json",
            data: { "planId": planId, "status": status, flag: 0 },
            success: rsHandler(function (data) {
                if (data == "ok") {
                    ncUnits.alert("撤销成功！");
                    view.updateList();
                }
                else {
                    ncUnits.alert("撤销失败！");
                }
            })
        });
    }



    var splitModalFlag = true;
    $("#planSplit_modal").on("show.bs.modal", function () {
        var planInfos = [];
        var selfid = 0;            //添加计划的ID

        if (splitModalFlag) {
            organizationLoad($("#decplan_department"), $("#decplan_department_v"), 1);
            organizationLoad($("#decplan_project"), $("#decplan_project_v"), 2);

            //确认责任人
            personChosen($("#splitPlan_responsibleUser"), $("#splitPlan_confirmUser"));
            personChosen($("#splitPlan_confirmUser"), $("#splitPlan_responsibleUser"));

            //addplan执行方式加载
            runModeLoading($("#splitRunMode"), null);

            //完成时间
            $("#splitPlan_comTime").off('click');
            $("#splitPlan_comTime").click(function () {
                laydate({
                    elem: '#splitPlan_comTime',
                    event: 'click',
                    format: 'YYYY-MM-DD hh:mm',
                    //min: laydate.now(),
                    istime: 'true',
                    isclear: true,
                    istoday: true,
                    issure: true,
                    festival: true,
                    start: new Date().toLocaleDateString() + ' 17:30:00'
                });
            });

            //确定按钮事件
            $("#planSplitSure").off("click");
            $("#planSplitSure").click(function () {
                if (planInfos.length <= 0) {
                    ncUnits.confirm({
                        title: '提示',
                        html: '请先添加子计划！'
                    });
                    return;
                }
                $.ajax({
                    type: "post",
                    url: "/plan/Resolve",
                    dataType: "json",
                    data: { planList: JSON.stringify(planInfos) },
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert("分解成功！");
                            $("#planSplit_modal").modal("hide");
                            //loadingPlanList();
                            view.updateList();
                        } else {
                            ncUnits.alert("分解失败！");
                            return;
                        }
                    })
                });
            })
            splitModalFlag = false;
        }

        //添加子计划按钮
        $('.addA').off('click');
        $('.addA').click(function () {
            var parentPlanId, parentEndTime, imp, urg, dif, org, orgtext, pro, protext, execution, executionModel, eventoutput, resUser, resUserName, confUser, confUserName, date;
            var num = 0;
            function getPlanInfo(id) {
                //首先：获取输入
                parentPlanId = $("#planSplit_modal").attr("term");
                parentEndTime = $("#planSplit_modal").attr("parentTime");
                imp = $('.importantDegree').find('.liHit').length;
                urg = $('.urgenceDegree').find('.liHit').length;
                dif = $('.difficultDegree').find('.liHit').length;
                org = $('#decplan_department_v').attr('term');
                orgtext = $('#decplan_department_v').html();
                pro = $('#decplan_project_v').attr('term');
                protext = $('#decplan_project_v').html();
                execution = $('#splitRunMode').val();
                executionModel = $('#splitRunMode option:selected').text();
                eventoutput = $("#spilt_eventoutput").val();
                resUser = $("#splitPlan_responsibleUser").attr('term');
                resUserName = $("#splitPlan_responsibleUser").val();
                confUser = $("#splitPlan_confirmUser").attr('term');
                confUserName = $("#splitPlan_confirmUser").val();
                date = $("#splitPlan_comTime").val();

                if (imp <= 0 || urg <= 0 || dif <= 0 || org == "" || pro == "" || execution == "" || executionModel == "执行方式" || eventoutput == "" || resUser == "" || date == "") {
                    ncUnits.alert("存在空白项，请查看");
                    return false;
                }
                else if (Date.parse(date) > Date.parse(parentEndTime)) {
                    ncUnits.alert("子计划的完成时间必须小于父计划的完成时间");
                    return false;
                }
                //清空分解画面
                splitPlanEmpty();
                return {
                    selfId: (id != null) ? id : selfid,
                    parentPlan: parentPlanId,
                    importance: imp,
                    urgency: urg,
                    difficulty: dif,
                    organizationId: parseInt(org),
                    organizationName: orgtext,
                    projectName: protext,
                    projectId: parseInt(pro),
                    executionModeText: executionModel,
                    executionModeId: parseInt(execution),
                    eventOutput: eventoutput,
                    responsibleUser: parseInt(resUser),
                    confirmUser: parseInt(confUser),
                    responsibleUserName: resUserName,
                    confirmUserName: confUserName,
                    oldEndTime: date
                };
            }
            var planInfo = getPlanInfo(null);
            if (planInfo) {
                var html = $('<li term=' + planInfo.selfId + '>' +
                    '<span class="decplanState">' + planInfo.executionModeText + '</span>' +
                    '<span class="decplanTitle textOver">' + planInfo.eventOutput + '</span>' +
                    '<span class="decplanPerson">' + planInfo.responsibleUserName + '</span>' +
                    '<span class="decplanDate">' + planInfo.oldEndTime + '</span>' +
                    '<div class="operate_tr"><div class="operateDiv">' +
                    '<span class="operateBg"></span><div class="operateText">' +
                    '<ul class="list-inline"><li class="decplan_detail"><img src="../../Images/plan/detailsWhite.png" /><span>详情</span></li>' +
                    '<li class="decplan_delete"><img src="../../Images/plan/deleteWhite.png" /><span>删除</span></li></ul></div>' +
                    '</div></div></li>');
                //计划删除
                $(html).find('.decplan_delete').click(function () {
                    var _this = this;
                    ncUnits.confirm({
                        title: '提示',
                        html: '确认要删除？',
                        yes: function (layer_confirm) {
                            layer.close(layer_confirm);
                            var $li = $(_this).parents("li:eq(0)");
                            var id = $($li).attr("term");
                            for (var i = 0; i < planInfos.length; i++) {
                                if (planInfos[i].selfId == id) {
                                    planInfos.splice(i, 1);
                                    return true;
                                }
                            }
                            $li.remove();
                        }
                    });
                });

                //计划详情
                $(html).find('.decplan_detail').click(function () {
                    var $deleteSpan = $(this).next();
                    $deleteSpan.css('display', 'none');
                    var id = parseInt($(this).parents("li:eq(0)").attr("term"));
                    $('.importantDegree').find('li').removeClass('liHit');
                    $('.urgenceDegree').find('li').removeClass('liHit');
                    $('.difficultDegree').find('li').removeClass('liHit');
                    $('.importantDegree').find('li').each(function (index) { if (index < planInfos[id].importance) $(this).addClass('liHit'); });
                    $('.urgenceDegree').find('li').each(function (index) { if (index < planInfos[id].urgency) $(this).addClass('liHit'); });
                    $('.difficultDegree').find('li').each(function (index) { if (index < planInfos[id].difficulty) $(this).addClass('liHit'); });
                    $('#decplan_department_v').attr('term', planInfos[id].organizationId).text(planInfos[id].organizationName);
                    $('#decplan_project_v').attr('term', planInfos[id].projectId).text(planInfos[id].projectName);
                    $("#splitRunMode").val(planInfos[id].executionModeId);
                    $("#splitRunMode option[value=" + planInfos[id].executionModeId + "]").attr("selected", true);
                    $("#spilt_eventoutput").val(planInfos[id].eventOutput);
                    $("#splitPlan_responsibleUser").attr('term', planInfos[id].responsibleUser).val(planInfos[id].responsibleUserName);
                    $("#splitPlan_confirmUser").attr('term', planInfos[id].confirmUser).val(planInfos[id].confirmUserName);
                    $("#splitPlan_comTime").val(planInfos[id].oldEndTime);
                    $('.addA').css('display', 'none');
                    $('.addB').css('display', 'block');
                    // $(this).next().trigger('click');
                    $('.addB').off('click');
                    $('.addB').click(function () {
                        planInfos[id] = getPlanInfo(id);
                        $deleteSpan.css('display', 'block');
                        $('.addA').css('display', 'block');
                        $('.addB').css('display', 'none');
                        $(".decplanList > li ").each(function () {
                            if (parseInt($(this).attr("term")) == planInfos[id].selfId) {
                                $(this).find('.decplanState').html(planInfos[id].executionModeText);
                                $(this).find('.decplanTitle').html(planInfos[id].eventOutput);
                                $(this).find('.decplanPerson').html(planInfos[id].responsibleUserName);
                                $(this).find('.decplanDate').html(planInfos[id].oldEndTime);
                            }
                        });
                        splitPlanEmpty();
                    });
                });
                //拼接显示新添加的子计划
                $(".decplanList").append(html);

                planInfos.push(planInfo);
                selfid++;
            }
        });
    });

    //分解计划弹窗关闭事件
    $("#planSplit_modal").on("hide.bs.modal", function () {
        $("#planSplit_modal").attr("term", '');
        splitPlanEmpty();
        $("#decplanList").empty();
    });

    function splitPlanEmpty() {
        $(".needStartNoChosen li").removeClass("liHit");
        $('.addA').css('display', 'block');
        $('.addB').css('display', 'none');
        $("#decplan_department_v").text("").attr("term", "");
        $("#decplan_project_v").text("").attr("term", "");
        $("#planSplit_modal input").val("");
        $("#splitRunMode option:eq(0)").attr("selected", true);
        $("#spilt_eventoutput").val("");
        $("#splitPlan_responsibleUser").val("").attr("term", "");
        $("#splitPlan_confirmUser").val("").attr("term", "");
        $("#splitPlan_comTime").val("");
    }

    /*------结束--------*/

    //部门 项目分类的收缩展开
    $(".slideToggleCorn").off("click");
    $(".slideToggleCorn").click(function () {
        $(this).toggleClass("glyphicon-menu-down");
        $(this).toggleClass("glyphicon-menu-up");
        $(this).parent().next().slideToggle();
    });

    //五角星事件
    /* 评分五角星 开始 */
    $('ul.start li').hover(function () {
        var nums = $(this).index();
        var length = $(this).parent().children('li').length - 1;
        for (var i = 0; i <= length; i++) {
            if (i <= nums) {
                $(this).parent().children('li').eq(i).addClass('liHover');
            }
            else {
                $(this).parent().children('li').eq(i).removeClass('liHover');
            }
        }
    }, function () {
        $(this).parent().children('li').removeClass('liHover');
    });

    $('ul.start li').click(function () {
        var nums = $(this).index();
        var length = $(this).parent().children('li').length - 1;
        for (var i = 0; i <= length; i++) {
            if (i <= nums) {
                $(this).parent().children('li').eq(i).addClass('liHit');
            }
            else {
                $(this).parent().children('li').eq(i).removeClass('liHit');
            }
        }
    });
    /* 评分五角星 结束 */
    

}())


var targetModule = (function () {

    timeThree("#expectTime", null, null, null, null);
    timeThree("#targetTime", null, null, null, null);
    var events = {
        'click .liDetail': detailAction,  //目标详情
        'click #objectRoleLabel': roleAction,
        'click #objectDetailLabel': labelAction,
        'click .liOpen': openAction,  //目标展开
        'click .liModify': modifyAction, //修改事件
        'click .slideBtn': slideAction,
        'click .commonBtn': showRight,
        'click .liSure': sureAction,//确认事件
        'click .liCheck': checkAction,//审核事件
        'click .liPower': powerAction,//授权事件
        'click #HR_modal_submit': hrSubmit, //授权提交
        'click .liCommit': commitAction, //提交事件
        'click .liConfirm': confirmAction,//提交确认事件
        'click .liPowerCancel,.liDel,.liRevocation': cancelAction, //取消授权,删除事件，撤销事件
        'click #objectModifySave,#objectModifyCommit': saveAction, //修改保存目标
        'click .slideDown':toggleAction  //加载子目标
    };
    var object_unfoldVar = {
        "objectiveId": 0,
        "responsibleUser": null,
        "confirmUser": null,
        "authorizedUser": null
    };
    var fieldArray = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "警戒时间", "权重", "奖励基数", "数字"];
    var FormulaArray = new Array();//全局变量存储公式
    var formulaEditOrAdd = null;//标记当前为公式编辑还是添加状态
    var formulaNum = 0;//公式编号
    var formulaSetInput = [[], []];//存放当前编辑的公式
    var newOrDecomposition;//全局变量，区分是新建还是分解新建：1 新建：2 分解 3：保存后再次点击详情再次打开
    var parentObjectiveStartTime, parentObjectiveEndTime;//父目标开始时间，父目标结束时间
    var personModal = 1, object_info = {};             //personModal=1 筛选条件的人员选择, personModal=2 授权人员选择
    var $activeNode, $unfoldNode;//存储授权节点，便于刷新该目标层
    var loadFormaluFlag = true;
    var objectiveId, loginId = userId,modifyOldPOrg = -1;
    var unflodFlag = 0;
    var detailFlag = 1;//detailFlag=1 目标详情画面, detailFlag=2 目标审核画面, detailFlag=3 目标确认画面,
    var messageResult = [6, 7, 8, 11, 12];
    var checkTypeArray = ["金额", "时间", "数字"];
    var statusArray = ["待提交", "待审核", "审核通过", "待确认", "已完成"];   //1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成
    var operateResult = ["创建", "删除", "授权", "提交", "撤销", "审核通过", "审核不通过", "修改", "分解目标", "更新进度", "确认通过", "确认不通过", "下载文档", "查看", "上传文档", "删除文档", "提交确认", "取消授权"];
    var FormulaList, formulaType, maxValue, minValue;
    var Returndata = {
        parentObjective: null,
        objectiveName: "",            //目标名称
        objectiveType: null,               //目标对象 1：组织架构 2：人员
        bonus: null,                   //奖励基数
        weight: null,                  //权重
        checkType: null,                   //考核类型 1：金额 2：时间 3：数字
        objectiveValue: "",           //指标值
        expectedValue: "",            //理想值
        description: "",              //备注项
        startTime: null,               //开始时间
        endTime: null,                //截止时间
        alarmTime: null,              //警戒时间
        responsibleOrg: null,              //责任部门
        responsibleUser: null,             //责任人
        confirmUser: null,                 //确认人
        formula: null,                     //公式 0：无公式 1：默认公式 2：自定义
        maxValue: null,
        minValue: null,
        objectiveFormula: []             //目标公式为自定义的场合
    };
    var split_info = {
        objectiveId: Number,
        objectiveType: Number,
        responsibleUser: Number,
        confirmUser: Number,
        confirmUserName: String,
        responsibleUserName: String,
        startTime: String,
        endTime: String,
        responsibleOrg: Number,
    };


    var bindEvents = function () {
        var delegateEventSplitter = /^(\S+)\s*(.*)$/;
        var k, method, eventName, selector, match;
        var el = $('#idifySearch');
        for (k in events) {
            method = events[k];
            match = k.match(delegateEventSplitter);
            eventName = match[1];
            selector = match[2];
            method = events[k];
            el.off(eventName, selector);
            el.on(eventName, selector, method);

        }

    }();


    function toggleAction() {
        $(this).toggleClass("glyphicon-triangle-top");
        $(this).toggleClass("glyphicon-triangle-bottom");
        if ($(this).attr('flag') == '1') {
            $(this).parents('.parentObject').toggleClass("backParent");
            
        }
        if ($(this).parents('.wrapAll').eq(0).children().hasClass('objectChild')) {
            $(this).parents('.wrapAll').eq(0).children('.objectChild').eq(0).slideToggle();
            return;

        }
        var tpl = document.getElementById('childTarget').innerHTML;
        var tHtml = _.template(tpl);
        $(this).parents('.wrapAll').eq(0).append(tHtml({ list: searchData[0] }));
        
        
    }



    function sureAction() {
        detailFlag = 3;
        var idx = $(this).parents('.wrapAll').index();
        object_info = searchData[idx];
        $activeNode = $(this);
        objectiveId = $(this).attr('term');
        $("#object_detail_modal").modal("show");
        objectDetail();
    }

    function checkAction() {
        detailFlag = 2;
        var idx = $(this).parents('.wrapAll').index();
        object_info = searchData[idx];
        $activeNode = $(this);
        objectiveId = $(this).attr('term');
        $("#object_detail_modal").modal("show");
        objectDetail();
    }

    function powerAction() {
        personModal = 2;
        var idx = $(this).parents('.wrapAll').index();
        object_info = searchData[idx];
        //object_info = v;
        $activeNode = $(this);
        objectiveId = $(this).attr('term');
        $("#HR_modal").modal("show");

    }
    function openAction() {
        var idx = $(this).parents('.wrapAll').index();
        object_info = searchData[idx];
        object_unfoldVar.objectiveId = object_info.objectiveId;
        object_unfoldVar.responsibleUser = object_info.responsibleUser;
        object_unfoldVar.confirmUser = object_info.confirmUser;
        object_unfoldVar.authorizedUser = object_info.authorizedUser;
        $('#object_unfold_modal').modal('show');
        objectiveId = $(this).attr('term')
        unfoldObjectCall(null, hrHorizontal);
        $('.modifyDocumentAdd').off('click')
        $('.modifyDocumentAdd').on('click',function () {
            addChildObject(this);

        })
    }

    function cancelAction() {
        objectiveId = $(this).attr('term');
        
        var flag = parseInt($(this).attr('flag'), 10);
        if (flag == 3 && $(this).hasClass('.liDel')) {
            unflodFlag = 2;
        }
        var txt = $(this).text();
        revokeDelCancel($(this), txt, flag);

    }



    function confirmAction() {
        detailFlag = 4;
        $activeNode = $(this);
        var idx = $(this).parents('.wrapAll').index();
        object_info = searchData[idx];
        $("#object_detail_modal").modal("show");
        objectiveId = $(this).attr('term');
        objectDetail()

    }

    function commitAction() {
        newOrDecomposition = 3;
        $("#objectiveNew_modal").modal('show');
        objectiveId = $(this).attr('term')
        //newInit(objectiveId, (loginId == v.authorizedUser ? 1 : 2));
        newInit(objectiveId, (loginId == 1))

    }

    function detailAction() {
        objectiveId = $(this).attr('term');
        detailFlag = 1;
        $("#object_detail_modal").modal("show");
        objectDetail();

    }


    function modifyAction() {
      
        if (!$(this).attr('active')) {
            $activeNode = $(this);
            var idx = $(this).parents('.wrapAll').index();
            var v = searchData[idx];
            if (loginId == v.responsibleUser && v.status == 3) {
                $("#objectModifyRole").hide();
            } else {
                $("#objectModifyRole:hidden").show();

            }
        }
        objectiveId = $(this).attr('term');
        $("#object_modify_modal").modal("show");
       
        //objectiveModify();
        //if (loadFormaluFlag && $("#objectModifyRole").is(':visible')) {
        //    $("#modifyFormularView .modal-body").empty();
        //    FormaluView(FormulaList, maxValue, minValue, formulaType, $("#modifyFormularView .modal-body"));
        //}
        
    }
    $("#object_modify_modal").on('shown.bs.modal', function () {
        if (loadFormaluFlag && $("#objectModifyRole").is(':visible')) {
            $("#modifyFormularView .modal-body").empty();
            FormaluView(FormulaList, maxValue, minValue, formulaType, $("#modifyFormularView .modal-body"));
        }
    })

    $("#object_modify_modal").on('show.bs.modal', function () {
        objectiveModify($($activeNode).closest("ul").attr("term"));
        documentUpload($("#fileUpload"), $("#object_modify_modal .progress"), $("#modifyObjectDocument"), $("#modifyObjectLog"));
    });

    function showRight() {
        personChoose(2);

    }

    function saveAction() {
        modifySave(parseInt($(this).attr("term")));
        unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
        if (unflodFlag == 1) {
            unflodFlag = 2;
        }
    }

    //撤销事件 删除事件  取消授权事件 parentChild=1表示是第一层目标事件，否则表示子目标事件
    function revokeDelCancel(value, flag, parentChild) {
        //var objectId = parseInt($(value).attr("term"));
        var url;
        console.log('flag', parentChild)
        var confirmText = '确定要' + flag + '吗?';
        if (flag == "撤销") {
            url = "/ObjectiveIndex/RevokeObjective";
        } else if (flag == "删除") {
            url = "/ObjectiveIndex/DeleteObjective";
            if (parentChild == 3 && parseInt($(value).attr("child")) > 0) {
                confirmText = "该目标有子目标,确定要删除吗?"
            }
        } else {
            url = "/ObjectiveIndex/CancelAuthorizeObjective";
        }
        ncUnits.confirm({
            title: '提示',
            html: confirmText,
            yes: function (layer_delete) {
                layer.close(layer_delete);
                $.ajax({
                    type: "post",
                    url: url,
                    dataType: "json",
                    data: { objectiveId: objectiveId },
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert(flag + "成功!");
                            view.updateList();
                            //if (parentChild == 1) {                     //第一层目标
                            //    //statusCount();
                            //    //loadObjectList();
                            //    //drawPlanProgress();       //刷新饼图
                            //} else if (parentChild == 2) {                 //第二层目标
                            //    var objectId = parseInt($(value).closest(".objectChild").attr("term"));
                            //    var parent = $(value).closest(".objectChild").prev();
                            //    $(value).closest(".objectChild").remove();
                            //    //statusCount();        //刷新状态统计
                            //    loadChildObjectList(parent, objectId);
                            //    drawPlanProgress();
                            //} else if (parentChild == 3) {           //展开中子目标的删除
                            //    unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
                            //}
                        }
                        else
                            ncUnits.alert(flag + "失败!");
                    })
                });
            }
        });
    }


    //添加子节点
    function addChildObject(values) {
        var $this = $(values);
        var $parent = $this.closest(".oneChild").children(".unfoldChild");
        if ($parent != null && $parent.is(":hidden")) {
            $this.closest(".oneChild").find(".slideBtn:eq(0)").toggleClass("glyphicon-triangle-top");
            $this.closest(".oneChild").find(".slideBtn:eq(0)").toggleClass("glyphicon-triangle-bottom");
            $parent.slideToggle("fast", function () {
                hrHorizontal($this.closest(".oneChild").parents(".unfoldChild"));
                modifyDocumentAdd($this);
            });
            hrHorizontal($this.closest(".oneChild").parents(".unfoldChild"));
        } else {
            modifyDocumentAdd($this);
        }
    }

    $(" #new-startTime, #new-endTime").click(function () {
        var id = "#" + $(this).attr("id");
        timeThree(id, null, null, null, null);
    });

    //添加子节点
    function modifyDocumentAdd(value) {
        var $newDiv = $('<div class="oneChild"><hr class="hrVertical"/><div  class="oneBlock">' +
            '<div style="height: 21px;"><span class="pull-right glyphicon glyphicon-remove" ></span></div>' +
            '<div style="display: block;line-height: 56px;"><a href="#">点击进行编辑</a></div></div></div>');

        $newDiv.find("a").click(function () { //分解
            var $parents = $(this).closest(".unfoldChild").prev();
            split_info.objectiveId = parseInt($parents.find(".liDetail:eq(0)").attr("term"));
            var $objectObj = $parents.find(".unfoldObjObject:eq(0)");
            split_info.objectiveType = parseInt($objectObj.attr("term"));
            split_info.responsibleUser = parseInt($objectObj.attr("response"));
            split_info.responsibleUserName = $objectObj.attr("responseName");
            split_info.confirmUser = parseInt($objectObj.attr("confirms"));
            split_info.confirmUserName = $objectObj.attr("confirmsName");
            var timeArrayss = $parents.find(".unfoldSETime:eq(0)").text().split("~");
            split_info.startTime = timeArrayss[0];
            split_info.endTime = timeArrayss[1];
            split_info.responsibleOrg = $objectObj.attr("resOrgId");

            newOrDecomposition = 2;
            $("#objectiveNew_modal").modal('show');
            newInit();
        });

        var childAll = $(value).closest(".oneBlock").next();              //获取该节点的子节点
        if (!$(childAll).hasClass("unfoldChild")) {              //如果没有子节点
            var $child = $('<div class="unfoldChild"><hr class="hrVertical"/><hr class="hrHorizontal"/></div>');
            $child.append($newDiv);
            $(value).closest(".oneChild").append($child);
            $(value).closest(".oneBlock").find(".slideBtn:eq(0)").show();
        } else {                          //如果有子节点层  添加子节点
            childAll.append($newDiv);
            hrHorizontal(null);
        }
        $newDiv.find(".glyphicon-remove").click(function () {
            var num = $(this).closest(".unfoldChild").children(".oneChild").length;
            if (num == 1) {
                $(this).closest(".unfoldChild").prev().find(".slideBtn:eq(0)").hide();
                $(this).closest(".unfoldChild").remove();
            } else if (num == 2) {
                $(this).closest(".unfoldChild").children(".hrHorizontal").hide();
                $(this).closest(".oneChild").remove();
            } else {
                $(this).closest(".oneChild").remove();
            }
            hrHorizontal(null);
        });
    }

    function objectiveModify() {
        $("#object_modify_modal input").val("");
        $("#object_modify_modal table").empty();
        var objectId = objectiveId;
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectInfo",              //目标修改URL
            dataType: "json",
            data: { objectiveId: objectId },
            success: rsHandler(function (data) {
                //公式信息
                formulaType = data.formula;
                FormulaList = data.objectiveFormula;
                maxValue = data.maxValue;
                minValue = data.minValue;
                var name = data.objectiveType == 1 ? data.responsibleOrgName : data.responsibleUserName;
                var term = data.objectiveType == 1 ? data.responsibleOrg : data.responsibleUser;
                $("#objectName").val(data.objectiveName == null ? "" : data.objectiveName);
                $("#objectName").attr("term", objectId);
                $("#objectName").attr("parentId", data.parentObjective);
                $("#objectName").attr("change", data.displayChangeFlag);
                $("#objectName").attr("status", data.status);
                $("#modifyObjectInput span:eq(0)").text(name).attr("title", name).attr("chooseId", term).attr("term", data.objectiveType).attr("data_orgName", data.responsibleOrgName);
                $("#objectBonus").val(data.bonus == null ? "" : data.bonus);
                $("#modifyWeight").val(data.weight == null ? "" : (data.weight + "%"));
                $("#modifyCheckType span:eq(0)").text(checkTypeArray[data.checkType - 1]);
                $("#modifyCheckType span:eq(0)").attr("term", data.checkType);

                $(".modifyObjectValue > *").each(function () {
                    $(this).hide();
                });
                if (data.checkType == 1) {        //金额
                    $("#targetMoney").parent().show();
                    $("#targetMoney").val(data.objectiveValue);
                    $("#expectMoney").parent().show();
                    $("#expectMoney").val(data.expectedValue);
                } else if (data.checkType == 2) {     //时间
                    $("#targetTime").parent().show();
                    $("#targetTime").val(data.objectiveValue);
                    $("#expectTime").parent().show();
                    $("#expectTime").val(data.expectedValue);
                } else {                //数字
                    $("#targetNumber").show();
                    $("#targetNumber").val(data.objectiveValue);
                    $("#expectNumber").show();
                    $("#expectNumber").val(data.expectedValue);
                }

                $("#modifyRemark").val(data.description);
                $("#modifyStartTime").val(data.startTime.replace('T', ' ').substr(0, 10));
                $("#modifyEndTime").val(data.endTime.replace('T', ' ').substr(0, 10));
                $("#modifyAlertTime").val(data.alarmTime == null ? '' : data.alarmTime.replace('T', ' ').substr(0, 10));
                $("#responsePerson").val(data.responsibleUserName);
                $("#responsePerson").attr("term", data.responsibleUser);
                $("#confirmPerson").val(data.confirmUserName);
                $("#confirmPerson").attr("term", data.confirmUser);

                //时间函数
                $(" #modifyStartTime,#modifyEndTime,#modifyAlertTime").off("click")
                $(" #modifyStartTime,#modifyEndTime,#modifyAlertTime").click(function () {
                    var id = "#" + $(this).attr("id");
                    timeThree(id, data.minStartTime, data.maxStartTime, data.minEndTime, data.maxEndTime);
                });

                $("#object_modify_modal .dateCorn").off("click");
                $("#object_modify_modal .dateCorn").click(function () {
                    var id = "#" + $(this).parent().prev().attr("id");
                    timeThree(id, data.minStartTime, data.maxStartTime, data.minEndTime, data.maxEndTime);
                });

                if (data.status == 3) {               //进行时
                    $("#modifyObjectInput button").attr("disabled", true);
                    $("#modifyCheckType button").attr("disabled", true);
                    $("#responsePerson").attr("disabled", true);
                    $("#confirmPerson").attr("disabled", true);
                }
                if (data.objectiveType == 2) {
                    $("#responsePerson").attr("disabled", true);
                }
                //相关文档
                $.each(data.documentInfo, function (i, v) {
                    var $tr = $("<tr term=" + v.attachmentId + " data_displayName =" + v.displayName + "><td class='col-xs-9'><span class='textOverFlow maxWidthTwo ' title=" + v.displayName + ">" + v.displayName + "</span></td>" +
                        "<td class='text-right'><a href='#' class='documentLoad'>下载</a></td><td class='text-right'  style='width: 38px;'><a href='#' class='documentPre'>预览</a></td></tr>");
                    var $del = $("<td class='text-right'  style='width: 38px;'><a href='#' >删除</a></td>");
                    $($del).click(function () {        //文档删除
                        documentDelete(this, objectId);
                    });
                    $($tr).append($del);
                    $tr.find(".documentLoad").click(function () {       //文档下载
                        loadDocument(this, 1, objectId, v.displayName, v.saveName);
                    });
                    $tr.find(".documentPre").click(function () {       //文档预览
                        //loadDocument(this, 2, objectId, v.displayName, v.saveName);
                        preview(7, v.saveName, v.extension);
                    });
                    $("#modifyObjectDocument").append($tr);
                });

                //操作日志
                $.each(data.operateLog, function (i, v) {
                    var message = "《" + data.objectiveName + "》";
                    if (operateResult[v.result - 1] == "上传文档" || operateResult[v.result - 1] == "下载文档" || operateResult[v.result - 1] == "删除文档") {
                        message = "";
                    }
                    var $tr = $("<tr term=" + v.operateId + " ><td class='col-xs-9' style='position:relative'><span style='color:#58b653'>" + v.reviewUserName + "</span>" +
                        "<span>" + operateResult[v.result - 1] + "</span><span style='color:#58b653;max-width:357px' class='textOverFlow' title=" + message + ">" + message + "</span><a  href='#' class='glyphicon glyphicon-comment'></a>" +
                        "<div class='operateMessage textOverFlow' title=" + v.message + ">意见 : " + v.message + "</div></td>" +
                        "<td class='text-right'><span>" + v.reviewTime.replace('T', ' ').substr(0, 19) + "</span></td></tr>");
                    if ($.inArray(v.result, messageResult) >= 0) {
                        $tr.find(".glyphicon-comment").show();
                        $tr.find(".glyphicon-comment").off("click");
                        $tr.find(".glyphicon-comment").click(function () {
                            if ($tr.find(".operateMessage:hidden").length != 0) {
                                $tr.animate({ height: '40px' }, "5000");
                            } else {
                                $tr.animate({ height: '26px' }, "5000");
                            }
                            $tr.find(".operateMessage").slideToggle("5000");
                        });
                    }

                    $("#modifyObjectLog").append($tr);
                });
            })
        });
    }

    function slideAction() {
        $(this).toggleClass("glyphicon-triangle-top");
        $(this).toggleClass("glyphicon-triangle-bottom");
        $(this).closest(".oneChild").find(".unfoldChild:eq(0)").slideToggle("fast", function () {
            hrHorizontal($(this).closest(".oneChild").parents(".unfoldChild"));
        });
        hrHorizontal($(this).closest(".oneChild").parents(".unfoldChild"));
    }



    //目标展开
    function unfoldObjectFunc(children, $parent) {
        if (children.length == 0) {
            return;
        }
        $.each(children, function (i, v) {
            var times = (v.startTime == null ? '' : v.startTime.substring(0, 10)) + '~' + (v.endTime == null ? '' : v.endTime.substring(0, 10));
           
            var titleVar;
            if (v.objectiveType == 1) {
                titleVar = v.objectiveTypeName;
            } else {
                titleVar = v.objectiveTypeName.split(" - ").join("-");
            }
            var $newDiv = $('<div class="oneChild"><hr class="hrVertical"/><div  class="oneBlock" ><div class="labels"><span class="labelOne"></span><div class="labelTwo"><span>' + statusArray[v.status - 1] + '</span></div>' +
                '<span class="labelThree"></span><span class="modifyDocumentAdd pull-right noMarginTop" aria-hidden="true" term="0"></span></div>' +
                '<ul class="list-unstyled"><li><span class="unfoldObjName textOverFlow" title=' + v.objectiveName + '>' + v.objectiveName + '</span><span class="unfoldWeight">' + (v.weight == null ? "" : (v.weight + "%")) + '</span></li>' +
                '<li>目标对象 : <span class="unfoldObjObject textOverFlow" response=' + v.responsibleUser + '  responseName=' + v.responsibleUserName + ' confirms=' + v.confirmUser + '  confirmsName=' + v.confirmUserName + ' term=' + v.objectiveType + '   title=' + titleVar + '  resOrgId=' + v.responsibleOrg + ' >' + v.objectiveTypeName + '</span></li>' +
                '<li>起止时间 : <span class="unfoldSETime"> ' + times + '</span><span class="pull-right slideBtn glyphicon glyphicon-triangle-top"></span></li></ul></div></div>');
            if (v.childObjectiveList.length == 0) {
                $newDiv.find(".slideBtn:eq(0)").hide();
            }
            if (object_unfoldVar.authorizedUser != null && object_unfoldVar.authorizedUser != loginId) {
                $newDiv.find(".modifyDocumentAdd").hide();
            } else {
                if (v.status < 4 && (loginId == object_unfoldVar.responsibleUser || loginId == object_unfoldVar.confirmUser || loginId == object_unfoldVar.authorizedUser)) {
                    
                    $newDiv.find(".modifyDocumentAdd").click(function () {
                        addChildObject(this);
                    });
                } else {
                    $newDiv.find(".modifyDocumentAdd").hide();
                }
            }
            //操作条
            var $operateDiv = $('<div class="operateDiv unfoldOperate"><ul class="list-inline unfoldChildOpera" term=' + v.objectiveId + '></ul></div>');

            var $liDel = $("<li class='liDel' term=" + v.objectiveId + " child=" + v.childObjectiveList.length + " flag=3>删除</li>"),
                 $liModify = $("<li class='liModify' term=" + v.objectiveId + " active='1'>修改</li>"),
                 $liDetail = $("<li class='liDetail' term=" + v.objectiveId + ">详情</li>");

            //删除
            //$liDel.click(function () {
            //    revokeDelCancel(this, "删除", 3);
            //    unflodFlag = 2;
            //});
            //详情事件
            //$liDetail.click(function () {
            //    detailFlag = 1;
            //    object_info = v;
            //    $("#object_detail_modal").modal("show");
            //});
            //修改事件
            //$liModify.click(function () {
            //    $activeNode = $(this);
            //    $("#object_modify_modal").modal("show");
            //});

            if (loginId == v.responsibleUser || loginId == v.authorizedUser) {
                if (v.status == 1) {      //可以删除
                    $operateDiv.find("ul").append($liDel);
                } else if (v.status == 3 || v.status == 6) { //可修改
                    $operateDiv.find("ul").append($liModify);
                }
            } else if (loginId == v.confirmUser) {                    //可修改
                if (v.status == 2 || v.status == 3 || v.status == 6) {
                    $operateDiv.find("ul").append($liModify);
                }
            }
            $operateDiv.find("ul").append($liDetail);

            $newDiv.find(".oneBlock").append($operateDiv);
            var childAll = $($parent).next();              //获取该节点的子节点
            if (!$(childAll).hasClass("unfoldChild")) {              //如果没有子节点
                var $child = $('<div class="unfoldChild"><hr class="hrVertical"/><hr class="hrHorizontal"/></div>');
                $child.append($newDiv);
                $($parent).closest(".oneChild").append($child);
            } else {                          //如果有子节点层  添加子节点
                childAll.append($newDiv);
            }

            //展开收缩
            if (v.childObjectiveList.length != 0) {
                $newDiv.find(".slideBtn:eq(0)").show();
            }
            $newDiv.find(".slideBtn").click(function () {
                slideBtnClickFunc(this);
            });
            unfoldObjectFunc(v.childObjectiveList, $newDiv.find(".oneBlock"));
        })
    }
    //目标展开,回调函数 页面加载完 改变线的宽度以及位置
    function unfoldObjectCall(index, callback) {
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/ExpandObjective",
            dataType: "json",
            data: { objectiveId: object_unfoldVar.objectiveId },
            success: rsHandler(function (data) {
                $("#object_unfold_modal .unfoldChild:eq(1)").remove();
                if (data.childObjectiveList.length != 0) {
                    $(".oneBlock .labelTwo span").text("已分解");
                } else {
                    $(".oneBlock .labelTwo span").text(statusArray[data.status - 1]);
                }
                //加号的判断 + TODO
                if (data.authorizedUser != null && data.authorizedUser != loginId) {
                    $(".modifyDocumentAdd").hide();
                } else {
                    if (data.status < 4 && (loginId == data.responsibleUser || loginId == data.confirmUser || loginId == data.authorizedUser)) {
                        $(".modifyDocumentAdd").show();
                       
                    } else {                              //如果有授权给别人的情况
                        $(".modifyDocumentAdd").hide();
                    }
                }
                $(".oneBlock .liDetail").attr("term", data.objectiveId);
                $(".oneBlock .liDetail").off('click');
                $(".oneBlock .liDetail").click(function () {
                    detailFlag = 1;
                    object_info = data;
                    $("#object_detail_modal").modal("show");
                });
                $(".oneBlock .unfoldObjName").text(data.objectiveName).attr("title", data.objectiveName);

                var titleVar;
                if (data.objectiveType == 1) {
                    titleVar = data.objectiveTypeName;
                } else {
                    titleVar = data.objectiveTypeName.split(" - ").join("-");
                }
                var $objectObjSpan = $('<span class="unfoldObjObject textOverFlow" response=' + data.responsibleUser + '  responseName="' + data.responsibleUserName + '" confirms=' + data.confirmUser + '  confirmsName=' + data.confirmUserName + ' term=' + data.objectiveType + '   title=' + titleVar + '   resOrgId=' + data.responsibleOrg + ' >目标对象 : ' + data.objectiveTypeName + '</span></li>');
                $(".oneBlock > ul li:eq(1)").empty().append($objectObjSpan);

                $(".oneBlock .unfoldSETime").text((data.startTime).substring(0, 10) + "~" + (data.endTime).substring(0, 10));
                $(".oneBlock .unfoldWeight").text((data.weight == null ? "" : (data.weight + "%")));

                if (data.childObjectiveList.length == 0) {
                    $(".oneBlock .slideBtn:eq(0) ").hide();
                } else {
                    $(".oneBlock .slideBtn:eq(0) ").show();
                }
                var firstBlock = $(".oneBlock");
                unfoldObjectFunc(data.childObjectiveList, firstBlock);
                callback(index);
            })
        });

    }

    function hrHorizontal(values) {
        //显示水平线  水平线的样式控制
        if (values != null) {
            var hrArray = $(values).children(".hrHorizontal");
        } else {
            var hrArray = $(".hrHorizontal");
        }
        hrArray.each(function () {
            if ($(this).siblings(".oneChild").length == 1) {
                return true;
            }
            $(this).show();
            var $first = $(this).siblings(".oneChild:first");
            var $end = $(this).siblings(".oneChild:last");
            var EndLeft = $end.position().left;
            var FirstLeft = $first.position().left;
            var FirstWidth = $first.width();
            var EndWidth = $end.width();
            var widthNum = EndLeft - FirstLeft - FirstWidth / 2 + EndWidth / 2;
            var leftNum = FirstLeft + FirstWidth / 2;
            $(this).css("width", widthNum - 1);
            $(this).css("left", leftNum);
        });
    }

    function roleAction() {
        $("#detailViewTab").removeClass("active");
        $("#FormularViewTab").addClass("active");
        $("#objectDetailLabel").addClass("disabledColor");
        $("#objectRoleLabel").removeClass("disabledColor");
        if (detailFlag != 2) {       //如果是审核
            objectFormula();
        }

    }

    function labelAction() {
        $("#FormularViewTab").removeClass("active");
        $("#detailViewTab").addClass("active");
        objectDetail();
    }

    function objectFormula() {
        var $modal_body = $("#FormularViewTab .modal-body");
        $($modal_body).empty();
        var formulaNo = $("<div class='radio row' style='margin-bottom: 50px;'><label><input type='radio' name='formula'  value='0' >不使用</label></div>");
        var formulaDefault = $("<div id='formulaDefault'><div class='row'>" +
            "<label class='radio-inline col-xs-2'><input type='radio' name='formula' value='1'> 默认公式</label>" +
            "<div class='col-xs-2' style='margin-top: -14px;margin-right: -15px;'><span>实际值-指标值</span><hr class='formulaHr'/><span>理想值-指标值</span></div><span>*100%</span></div><hr/>" +
            "<div class='row'><div class='col-xs-offset-2 col-xs-4'><span>最大奖励 = 奖励系数*</span><input type='number' class='form-control' disabled></div>" +
            "<div class='col-xs-offset-1 col-xs-4'><span>最大惩罚 = 惩罚系数*</span><input type='number' class='form-control' disabled></div></div></div>");
        var formulaDefine = $("<div class='radio' style='margin-top: 50px;'><label><input type='radio' name='formula' value='3'>自定义公式</label></div><hr/>");
        var $div = $("<div id='formulaDefine'></div>");
        if (formulaType == 0) {                  //无公式
            //$(formulaDefault).find("input").attr("disabled", true);
            //$(formulaDefault).find("span").addClass("disabledColor");
            //$(formulaDefine).find("input").attr("disabled", true);
            $(formulaDefault).find(".radio-inline").addClass("disabled");
            $(formulaDefault).find("span").addClass("disabledColor");
            $(formulaDefine).addClass("disabled");
            $(formulaNo).find("input[type='radio']").prop("checked", true);
        } else if (formulaType == 1) {              //默认公式
            //$(formulaNo).find("input").attr("disabled", true);
            //$(formulaDefine).find("input").attr("disabled", true);
            $(formulaNo).addClass("disabled");
            $(formulaDefine).addClass("disabled");
            $(formulaDefault).find("input[type='radio']").prop("checked", true);
            $(formulaDefault).find("input[type='number']:eq(0)").val(maxValue);
            $(formulaDefault).find("input[type='number']:eq(1)").val(minValue);
        } else if (formulaType == 2) {                          //自定义公式
            //$(formulaNo).find("input").attr("disabled", true);
            //$(formulaDefault).find("input").attr("disabled", true);
            $(formulaNo).addClass("disabled");
            $(formulaDefault).find(".radio-inline").addClass("disabled");
            $(formulaDefault).find("span").addClass("disabledColor");
            $(formulaDefine).find("input[type='radio']").prop("checked", true);
            var formulaFlag, formulaText = "";
            var $table = $("<table class='table table-hover'></table>");
            var formulaIndexNum = 1;
            for (i = 0; i < FormulaList.length; i++) {
                v = FormulaList[i];
                if (i == 0) {
                    formulaFlag = v.formulaNum;
                    $($table).append("<tr term=" + formulaFlag + "><td class='formulaIndex'>" + formulaIndexNum + ")</td><td class='formulaTd'></td><td class='formulaDropdown'></td><td class='formulaValue'></td></tr>");
                }
                var $tr = $($table).find("tr:last");
                if (v.formulaNum == formulaFlag) {       //一条公式
                    if (i >= FormulaList.length - 2 || FormulaList[i + 2].formulaNum != formulaFlag) {
                        if (i == FormulaList.length - 2) {
                            $($tr).find("td:eq(1)").html(formulaText);
                        }
                        if (v.field == 8 || v.field == 9) {          //如果是奖励基数 固定奖励
                            var $dropdown = $("<div class='dropdown'><button class='btn btn-default dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false' disabled>" +
                                "<span>" + (v.field == 8 ? '奖励基数' : '固定奖励') + "</span><span class='pull-right upPic'></span></button>" +
                                "<ul class='dropdown-menu' role='menu'><li><a herf='#'>奖励基数</a></li><li><a herf='#'>固定奖励</a></li></ul></div>");
                            $($tr).find("td:eq(2)").append($dropdown);
                        } else if (FormulaList[i - 1].field == 8 && v.numValue != null) {
                            var $input = $("<span>奖励基数*</span><input type='number'  class='form-control' value=" + v.numValue + " disabled>");
                            $($tr).find("td:eq(3)").append($input);
                        } else if (FormulaList[i - 1].field == 9 && v.numValue != null) {
                            var $input = $("<input type='number'  style='width:97%' class='form-control' value=" + v.numValue + " disabled >");
                            $($tr).find("td:eq(3)").append($input);
                        }
                    } else if (v.field != null) {
                        formulaText = formulaText + " " + fieldArray[v.field - 1];
                    } else if (v.operate != null) {
                        if (v.operate == "|")
                            formulaText = formulaText + "<span style='margin:8px'>或</span>";
                        else if (v.operate == "&")
                            formulaText = formulaText + "<span  style='margin:8px' >且</span>";
                        else
                            formulaText = formulaText + " " + v.operate;
                    } else if (v.numValue != null) {
                        formulaText = formulaText + " " + v.numValue;
                    }
                } else {
                    formulaIndexNum++;
                    $($tr).find("td:eq(1)").html(formulaText);
                    formulaText = "";
                    formulaFlag = v.formulaNum;
                    $($table).append("<tr term=" + formulaFlag + "><td class='formulaIndex'>" + formulaIndexNum + ")</td><td class='formulaTd'></td><td class='formulaDropdown'></td><td class='formulaValue'></td></tr>");
                    i--;
                }
            }
            $($div).append($table);
        } else {
            $(formulaNo).addClass("disabled");
            $(formulaDefault).find(".radio-inline").addClass("disabled");
            $(formulaDefault).find("span").addClass("disabledColor");
            $(formulaDefine).addClass("disabled");
        }
        $modal_body.append([formulaNo, formulaDefault, formulaDefine, $div]);
    }

    function objectDetail() {
        console.log('gungungun')
        $("#objectRoleLabel").addClass("disabledColor");
        $("#objectDetailLabel").removeClass("disabledColor");
        var $modal_body = $("#detailViewTab .modal-body");
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectInfo",
            dataType: "json",
            data: { objectiveId: objectiveId },
            success: rsHandler(function (data) {
                $($modal_body).empty();
                
                //目标内容
                var ChangeInfo = data.ChangeInfo;
                formulaType = data.formula;
                FormulaList = data.objectiveFormula;
                maxValue = data.maxValue;
                minValue = data.minValue;
                actualValue = data.actualValue;
                actualEndTime = data.actualEndTime;
                var $content = $("<h4 style='margin-top: 0px;'>目标内容</h4><hr/>");
                var $contentOne = $("<div class='row borderTop' id='detailContent'><ul class='list-unstyled col-xs-6'>" +
                    "<li><label>目标名称:</label><span class='textOverFlow maxWidthOne' title=" + data.objectiveName + ">" + data.objectiveName + "</span></li>" +
                    "<li><label>奖励基数:</label><span>" + (data.bonus == null ? '' : data.bonus) + "</span></li>" +
                    "<li><label>考核类型:</label><span>" + checkTypeArray[data.checkType - 1] + "</span></li>" +
                    "<li><label>理想值:</label><span>" + (data.expectedValue == null ? '' : data.expectedValue) + "</span></li></ul>" +
                    "<ul class='list-unstyled col-xs-6'>" +
                    "<li><label>目标对象:</label><span class='textOverFlow maxWidthOne' title=" + data.objectiveName + ">" + (data.objectiveType == 1 ? data.responsibleOrgName : data.responsibleUserName) + "</span></li>" +
                    "<li><label>权重:</label><span>" + (data.weight == null ? "" : (data.weight + "%")) + "</span></li>" +
                    "<li><label>指标值:</label><span>" + (data.objectiveValue == null ? '' : data.objectiveValue) + "</span></li>" +
                    "<li><label>备注项目:</label><span class='textOverFlow maxWidthOne' title=" + (data.description == undefined ? '' : data.description) + ">" + (data.description == undefined ? '' : data.description) + "</span></li></ul></div>");
                var $contentTwo = $(" <ul class='list-inline  row borderTop'>" +
                    "<li class='col-xs-4'><label>开始时间：</label><span>" + data.startTime.replace('T', ' ').substr(0, 10) + "</span></li>" +
                    "<li  class='col-xs-4'><label>结束时间：</label><span>" + data.endTime.replace('T', ' ').substr(0, 10) + "</span></li>" +
                    "<li  class='col-xs-4'><label>警戒时间：</label><span>" + (data.alarmTime == null ? '' : data.alarmTime.replace('T', ' ').substr(0, 10)) + "</span></li></ul>");
                var $contentThree = $(" <ul class='list-inline row borderTop'>" +
                    "<li  class='col-xs-4'><label>责任人：</label><span>" + data.responsibleUserName + "</span></li>" +
                    "<li  class='col-xs-4'><label>确认人：</label><span>" + data.confirmUserName + "</span></li></ul>");
                if (data.displayChangeFlag == 1 && ChangeInfo) {     //红字显示变更
                    if (ChangeInfo.objectiveNameUpdate == 1) {
                        $($contentOne).find("li:eq(0)").addClass("changeColor");
                    }
                    if (ChangeInfo.weightUpdate == 1) {
                        $($contentOne).find("li:eq(5)").addClass("changeColor");
                    }
                    if (ChangeInfo.objectiveValueUpdate == 1) {
                        $($contentOne).find("li:eq(6)").addClass("changeColor");
                    }
                    if (ChangeInfo.expectedValueUpdate == 1) {
                        $($contentOne).find("li:eq(3)").addClass("changeColor");
                    }
                    if (ChangeInfo.bonusUpdate == 1) {
                        $($contentOne).find("li:eq(1)").addClass("changeColor");
                    }
                    if (ChangeInfo.startTimeUpdate == 1) {
                        $($contentTwo).find("li:eq(0)").addClass("changeColor");
                    }
                    if (ChangeInfo.endTimeUpdate == 1) {
                        $($contentTwo).find("li:eq(1)").addClass("changeColor");
                    }
                    if (ChangeInfo.alarmTimeUpdate == 1) {
                        $($contentTwo).find("li:eq(2)").addClass("changeColor");
                    }
                }
                //分解目标
                var subObject = $("<h4> 分解目标</h4><hr/>");
                var subObjectTable = $("<table class='table' id='splitObject'></table>");
                $.each(data.childrenObjective, function (i, v) {
                    var objectiveTitle = v.objectiveType == 1 ? v.responsibleOrgName : v.responsibleUserName;
                    var $tr = $("<tr><td><label>目标名称：</label><span class='textOverFlow' title=" + v.objectiveName + ">" + v.objectiveName + "</span></td>" +
                        "<td><label>目标对象：</label><span class='textOverFlow' title=" + objectiveTitle + ">" + objectiveTitle + "</span></td>" +
                        "<td><label>权重：</label><span>" + (v.weight == null ? "" : (v.weight + "%")) + "</span></td><td><label>责任人：</label><span>" + v.responsibleUserName + "</span></td>" +
                        "<td><a href='#'  class='subObjectLook' term=" + v.objectiveId + ">查看</a></td></tr>");
                    $($tr).find(".subObjectLook").off("click");
                    $($tr).find(".subObjectLook").click(function () {
                        objectDetail(parseInt($(this).attr("term")));
                    });
                    subObjectTable.append($tr);
                });
                //相关文档

                var $document = $("<h4 class='col-xs-4 noPaddingLeft' >相关文档</h4>");
                var $documentTable = $("<table class='table' id='objectDocument'></table>");
                $.each(data.documentInfo, function (i, v) {

                    var $tr = $("<tr term=" + v.attachmentId + "><td class='col-xs-10'><span class='textOverFlow maxWidthTwo ' title=" + v.displayName + ">" + v.displayName + "</span></td>" +
                        "<td class='text-right'><a href='#' class='documentLoad'>下载</a></td><td class='text-right'  style='width: 38px;'><a href='#' class='documentPre'>预览</a></td></tr>");
                    $tr.find(".documentLoad").click(function () {       //文档下载
                        loadDocument(this, 1, objectiveId, v.displayName, v.saveName);
                    });
                    $tr.find(".documentPre").click(function () {       //文档预览
                        preview(7, v.saveName, v.extension);
                    });

                    if (detailFlag == 4) {      //如果是提交操作
                        console.log('detail',detailFlag)
                        var $td = $(" <td class='text-right'><a href='#' class='documentLoad'>删除</a></td>");
                        $td.find("a").click(function () {
                            documentDelete(this, objectiveId);
                        });
                        $tr.append($td);
                    }
                    $documentTable.append($tr);
                });

                //操作日志
                var operateLog = $("<h4>操作日志</h4><hr />");
                var operateLogTable = $("<table class='table table-hover'></table>");
                $.each(data.operateLog, function (i, v) {
                    var message = "《" + data.objectiveName + "》";
                    if (operateResult[v.result - 1] == "上传文档" || operateResult[v.result - 1] == "下载文档" || operateResult[v.result - 1] == "删除文档") {
                        message = "";
                    }
                    var $tr = $("<tr term=" + v.operateId + " >" +
                        "<td class='col-xs-9' style='position:relative' ><span style='color:#58b653'>" + v.reviewUserName + "</span>" +
                        "<span>" + operateResult[v.result - 1] + "</span><span style='color:#58b653;max-width:357px' class='textOverFlow' title=" + message + ">" + message + "</span>" +
                        "<a  href='#' class='glyphicon glyphicon-comment'></a>" +
                        "<div class='operateMessage textOverFlow' title=" + v.message + ">意见 : " + v.message + "</div></td>" +
                        "<td class='text-right'><span>" + v.reviewTime.replace('T', ' ').substr(0, 19) + "</span></td></tr>");
                    if ($.inArray(v.result, messageResult) >= 0) {
                        $tr.find(".glyphicon-comment").show();
                        $tr.find(".glyphicon-comment").off("click");
                        $tr.find(".glyphicon-comment").click(function () {
                            if ($tr.find(".operateMessage:hidden").length != 0) {
                                $tr.animate({ height: '40px' }, "5000");
                            } else {
                                $tr.animate({ height: '26px' }, "5000");
                            }
                            $tr.find(".operateMessage").slideToggle("5000");
                        });
                    }
                    operateLogTable.append($tr);
                });

                if (detailFlag == 4) {      //如果是提交操作
                    var $add = $('<span class="modifyDocumentAdd  pull-right"><input type="file"  class="fileUploadBtn" name="files[]" multiple ></span>');
                    var $progressView = $('<div class="progress"><div class="bar" style="width:0%;"></div></div>');
                    documentUpload($add.find("input"), $progressView, $documentTable, operateLogTable);       //文档的上传事件
                } else {
                    var $add = "";
                    var $progressView = "";
                }
                $($modal_body).append([$content, $contentOne, $contentTwo, $contentThree, subObject, subObjectTable, $document, $add, $progressView, $("<hr/>"), $documentTable, operateLog, operateLogTable]);

                if (detailFlag == 2) {            //目标审核
                    checksFunc(data);
                } else if (detailFlag == 3) {                         //目标确认
                    objectSureFunc(data);
                } else if (detailFlag == 4) {               //目标的提交
                    //  $("#object_detail_modal .modal-footer").hide();
                    objectCommit(data.checkType);
                }
            })
        });
    }



    //目标确认
    function objectSureFunc(valueData) {
        $(".rightModal").css({
            "left": ($(".rightModal").parents('.modal-content').width() - 5)
        });
        $("#object_detail_modal .rightModal").show();
        $("#rightTitle").text("确认");
        var $value = $("<p>实际值: " + valueData.actualValue + "&nbsp;&nbsp;&nbsp;" + (valueData.checkType == 1 ? '元' : '') + "</p>");
        var $text = $("<textarea class='form-control message'  rows='4' maxlength='250'></textarea>");
        var $passBtn = [];
        $passBtn[0] = $("<a class='btn btn-transparency btn_xs pull-right'  term='11'>通过</a>");
        $passBtn[1] = $("<a class='btn btn-transparency btn_xs pull-right' term='12'>不通过</a>");
        //var sureArgus = {
        //    objectiveId: Number,       //目标ID
        //    status: Number,            //状态
        //    message: String,        //操作意见
        //    result: Number             //操作类型 11：确认通过 12：确认不通过
        //};
        $.each($passBtn, function (i, v) {
            $(v).bind("click", function () {
                var message = $.trim($text.val());
                //if (justifyByLetter(message, "意见") == false) {
                //    return;
                //}
                var result = parseInt($(this).attr("term"));
                var objectiveId = valueData.objectiveId;
                //var status = valueData.status;
                $.ajax({
                    type: "post",
                    url: "/ObjectiveIndex/ConfirmObjective",
                    dataType: "json",
                    data: { objectiveId: objectiveId, message: message, result: result },
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert("确认完成！");
                            $("#object_detail_modal").modal('hide');
                            view.updateList();
                            //statusCount();
                            //var parent = $($activeNode).closest(".objectChild");
                            //if (parent.length != 0) {            //表示刷新授权目标所在层
                            //    loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                            //    $(parent).remove();
                            //} else {             //表示刷新最顶层
                            //    loadObjectList();
                            //}
                            //drawPlanProgress();
                        } else {
                            ncUnits.alert("确认失败!");
                        }
                    })
                })
            });
        });
        $("#rightContent").empty().append([$value, $text, $passBtn[0], $passBtn[1]]);
    }


    function timeThree(id, minStartTime, maxStartTime, minEndTime, maxEndTime) {
        var startTime = null;
        var endTime = null;
        if (id == "#modifyStartTime") {              //开始
            startTime = minStartTime;
            endTime = $("#modifyEndTime").val() > $("#modifyAlertTime").val() ? $("#modifyAlertTime").val() : $("#modifyEndTime").val();
            if (endTime != "" && maxStartTime != null && endTime > maxStartTime) {
                endTime = maxStartTime;
            }
        } else if (id == "#modifyEndTime") {
            startTime = $("#modifyStartTime").val();
            if (startTime != "" && minEndTime != null && startTime < minEndTime) {
                startTime = minEndTime;
            }
            endTime = maxEndTime;
        } else if (id == "#modifyAlertTime") {
            startTime = $("#modifyStartTime").val();
            endTime = $("#modifyEndTime").val();
        } else if (id == "#new-startTime") {
            //if ($("#new-alarmTime").val() == "") {
            //    endTime = $("#new-endTime").val();
            //}
            //else if ($("#new-endTime").val() == "") {
            //    endTime = $("#new-alarmTime").val();
            //}
            //else {
            //    //取较小值
            //    endTime = $("#new-endTime").val() > $("#new-alarmTime").val() ? $("#new-alarmTime").val() : $("#new-endTime").val();
            //}

            startTime = minStartTime;
            endTime = $("#new-endTime").val() > $("#new-alarmTime").val() ? $("#new-alarmTime").val() : $("#new-endTime").val();
            if (endTime != "" && maxStartTime != null && endTime > maxStartTime) {
                endTime = maxStartTime;
            }
            if (parentObjectiveStartTime != null) {
                startTime = parentObjectiveStartTime;
                if (endTime == "") {
                    endTime = parentObjectiveEndTime;
                }
            }

        } else if (id == "#new-endTime") {
            //取较大值
            //startTime = $("#new-startTime").val() < $("#new-alarmTime").val() ? $("#new-alarmTime").val() : $("#new-startTime").val();


            startTime = $("#new-startTime").val();
            if (startTime != "" && minEndTime != null && startTime < minEndTime) {
                startTime = minEndTime;
            }
            endTime = maxEndTime;
            if (parentObjectiveEndTime != null) {
                endTime = parentObjectiveEndTime;
                if (startTime == "") {
                    startTime = parentObjectiveStartTime;
                }

            }
        }
        else if (id == "#new-alarmTime") {
            startTime = $("#new-startTime").val();
            endTime = $("#new-endTime").val();
            if (parentObjectiveEndTime != null) {
                if (startTime == "") {
                    startTime = parentObjectiveStartTime;
                }
                if (endTime == "") {
                    endTime = parentObjectiveEndTime;
                }
            }
        }
        if (startTime == "") {
            startTime = null;
        }
        if (endTime == "") {
            endTime == null;
        }
        timeChosen(id, startTime, endTime);
    }


    function timeChosen(value, startTime, endTime) {
        var time = {
            elem: value,
            format: 'YYYY-MM-DD',
            max: '2099-06-16', //最大日期
            istime: false,
            istoday: false
        };
        if (startTime != null && startTime != "") {
            time.start = startTime;
            time.min = startTime;
        }
        if (endTime != null && endTime != "") {
            time.max = endTime;
        }
        laydate(time);
    }

    function personChoose(flagKind) {
        $(".rightModal").css({
            "left": ($(".rightModal").parents('.modal-content').width() - 5)
        });
        $("#object_modify_modal .rightModal").show();
        $("#chooseTitle").text("人员选择");
        var $ztrees = $("<div class='ztree' id='personZtree'></div>");
        var $title = $("<hr class='noMarginBottom'/><div class='checkbox' style='padding-left: 5px;margin-bottom: -5px;'>" +
            "<label><input type='checkbox' class='modifyInclude'>包含下级</label></div><hr style='margin-bottom: 0px;'/>");
        if (flagKind == 1 && perWithSub == 1) {
            $($title).find(".modifyInclude").prop("checked", true);
        }
        $title.find(".modifyInclude").click(function () {
            if (flagKind == 1) {
                if ($(this).is(":checked")) {
                    perWithSub = 1;
                } else {
                    perWithSub = 0;
                }
            }
            if (modifyOldPOrg != -1) {
                appendPersonRight(modifyOldPOrg, flagKind);
            }
        });
        var $personList = $("<ul class='list-unstyled' id='modifyPersonList'></ul>");
        $("#chooseContent").empty().append([$ztrees, $title, $personList]);
        treeOrgLoadPerson(flagKind);
    }

    //人员 选择树
    function treeOrgLoadPerson(flagKind) {
        var $span = $("#modifyObjectInput span:eq(1)");
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#personZtree"), $.extend({
                    callback: {
                        onNodeCreated: function (e, id, node) {
                            if (flagKind == 1) {
                                if (modifyOldPOrg != -1 && node.id == modifyOldPOrg) {
                                    var ztrees = $.fn.zTree.getZTreeObj(id);
                                    ztrees.selectNode(node);
                                    appendPersonRight(node.id, flagKind);
                                }
                            }
                        },
                        onClick: function (e, id, node) {        //选中事件
                            appendPersonRight(node.id, flagKind);
                            if (flagKind == 1) {
                                modifyOldPOrg = node.id;
                                modifyOldPOrgName = node.name;
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
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    function appendPersonRight(organizationId, flag) {
        if ($(".modifyInclude").is(":checked")) {
            var withSub = 1;
        } else {
            var withSub = 0;
        }
        if (flag == 1) {
            var $modifySpan = $("#modifyObjectInput span:eq(0)");
        } else if (flag == 2) {
            var $modifySpan = $("#responsePerson");
        }
        else if (flag == 3) {
            var $modifySpan = $("#confirmPerson");
        }
        $.ajax({
            type: "post",
            url: "/Shared/GetUserList",
            dataType: "json",
            data: { withSub: withSub, organizationId: organizationId },
            success: rsHandler(function (data) {
                $("#modifyPersonList").empty();
                $.each(data, function (i, v) {
                    var $li = $("<li></li>");
                    var $label = $("<div class='radio'><label></label></div>");
                    var $radio = $("<input type='radio' class='flowPersonCheck' name='person' value=" + v.userId + ">");
                    $radio.click(function () {
                        $(this).prop("checked", true);
                        if (flag == 1) {
                            $modifySpan.attr("chooseId", v.userId);
                            $modifySpan.text(v.userName).attr("title", v.userName).attr("data_orgname", modifyOldPOrgName);
                            $modifySpan.attr("term", 2);
                            modifyOldPerson = v.userId;
                            $("#responsePerson").val(v.userName);
                            $("#responsePerson").attr("term", v.userId);
                            $("#responsePerson").attr("disabled", true);
                            $("#responsePerson").next().attr("disabled", true);
                        } else {
                            $modifySpan.attr("term", v.userId);
                            $modifySpan.val(v.userName);
                        }
                    });
                    if (flag == 1 && v.userId == modifyOldPerson) {
                        $radio.trigger("click");
                    }
                    var $span = $("<span>" + v.userName + "</span>-<span>" + v.organizationName + "</span>");
                    $label.find("label").append([$radio, $span]);
                    $li.append($label);
                    $("#modifyPersonList").append($li);
                })
            })
        })
    }

    //修改保存
    function modifySave(flag) {
        //保存判断

        //责任人 确认人判断
        if ($("#responsePerson").attr("term") == $("#confirmPerson").attr("term")) {
            ncUnits.alert("责任人与确认人不得相同!");
            return;
        }

        //目标名称输入判断
        var objectName = $.trim($("#objectName").val());
        if (objectName == "") {
            ncUnits.alert("目标名称不得为空!");
            return;
        }
        if (justifyByLetter(objectName, "目标名称") == false) {
            return
        }

        //备注项目
        var modifyRemark = $.trim($("#modifyRemark").val());
        if (justifyByLetter(modifyRemark, "备注项目") == false) {
            return;
        }

        var argus = {
            objectiveId: Number,                 //目标ID
            parentObjective: Number,             //父目标ID
            displayChangeFlag: Boolean,          //显示变更Flag 0：显示  1：显示
            objectiveName: String,            //目标名称
            objectiveType: Number,               //目标对象 1：组织架构 2：人员
            bonus: Number,                   //奖励基数
            weight: Number,                  //权重
            checkType: Number,                   //考核类型 1：金额 2：时间 3：数字
            objectiveValue: String,           //指标值
            expectedValue: String,            //理想值
            description: String,              //备注项
            startTime: Date,               //开始时间
            endTime: Date,                //截止时间
            alarmTime: Date,              //警戒时间
            responsibleOrg: String,              //责任部门     
            responsibleUser: Number,             //责任人
            confirmUser: Number,                 //确认人
            status: Number,                      //状态 1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成
            message: String,               //意见
            objectiveFormulaInfo: null           //公式

        };
        var objFormulaInfo = {
            maxValue: null,                      //最大奖励数
            minValue: null,                    //最大扣除数
            formula: null,                 //公式 0：无公式 1：默认公式 2：自定义
            objectiveFormula: null         //目标公式信息
        }

        if ($("#objectModifyRole").is(":hidden")) {
            objFormulaInfo = null;
            modifyAfterConfirm();
            return;
        }

        objFormulaInfo.formula = parseInt($("#modifyFormularView input[name='formula']:checked").attr("term"));
        if (flag == 2) {         //提交必须设置公式
            if (objFormulaInfo.formula == null || isNaN(objFormulaInfo.formula) || (objFormulaInfo.formula == 2 && FormulaArray.length == 0)) {
                ncUnits.alert("提交必须设置公式！");
                return;
            }
        }
        if (objFormulaInfo.formula == 1) {
            var confirmText = null;
            if ($("#maxValue").val() == "" && flag == 2) {
                if (rightFormula() == true) {
                    confirmText = "最大奖励系数不得为空!是否恢复之前设置?";
                } else {
                    ncUnits.alert("最大奖励系数不得为空！");
                    return;
                }
            } else if ($("#minValue").val() == "" && flag == 2) {
                if (rightFormula() == true) {
                    confirmText = "最大惩罚系数不得为空!是否恢复之前设置?";
                } else {
                    ncUnits.alert("最大惩罚系数不得为空！");
                    return;
                }
            }
            if (confirmText != null) {
                ncUnits.confirm({
                    title: '提示',
                    html: confirmText,
                    yes: function (layer_delete) {                 //恢复原来设置
                        layer.close(layer_delete);
                        objFormulaInfo.formula = formulaType;
                        objFormulaInfo.objectiveFormula = FormulaList;
                        objFormulaInfo.maxValue = maxValue;
                        objFormulaInfo.minValue = minValue;
                        modifyAfterConfirm();
                    },
                    no: function (layer_delete) {                 //前去保存
                        layer.close(layer_delete);
                        $("#objectModifyRole").trigger("click");
                    }
                })
            } else {                   //否则保存设置公式
                objFormulaInfo.maxValue = $("#maxValue").val();
                objFormulaInfo.minValue = $("#minValue").val();
                modifyAfterConfirm();
            }

        } else if (objFormulaInfo.formula == 2) {
            if (objectiveRuleSureFlag == false) {
                if (rightFormula() == true) {
                    ncUnits.confirm({
                        title: '提示',
                        html: '你所设置的公式还没保存,是否去保存?',
                        yes: function (layer_delete) {                 //保存
                            layer.close(layer_delete);
                            $("#objectModifyRole").trigger("click");
                        },
                        no: function (layer_delete) {                    //恢复为最初公式
                            layer.close(layer_delete);
                            objFormulaInfo.formula = formulaType;
                            objFormulaInfo.objectiveFormula = FormulaList;
                            objFormulaInfo.maxValue = maxValue;
                            objFormulaInfo.minValue = minValue;
                            modifyAfterConfirm();
                        }
                    })
                } else {
                    ncUnits.alert("公式还没有保存！");
                    $("#objectModifyRole").trigger("click");
                    return;
                }
            } else {
                for (var j = 0 ; j < FormulaArray.length; j++) {
                    if (FormulaArray[j].formulaId == "z") {
                        FormulaArray[j].formulaId = null;
                    }
                }
                objFormulaInfo.objectiveFormula = FormulaArray;
                modifyAfterConfirm();
            }
        } else if (objFormulaInfo.formula == 0) {
            modifyAfterConfirm();
        } else {
            objFormulaInfo.formula = formulaType;
            objFormulaInfo.objectiveFormula = FormulaList;
            objFormulaInfo.maxValue = maxValue;
            objFormulaInfo.minValue = minValue;
            modifyAfterConfirm();
        }


        function modifyAfterConfirm() {
            argus.objectiveFormulaInfo = objFormulaInfo;
            var objectSpan = $("#modifyObjectInput span:eq(0)");
            var objectName = $("#objectName");
            argus.objectiveId = parseInt(objectName.attr("term"));
            argus.parentObjective = parseInt(objectName.attr("parentId"));
            argus.displayChangeFlag = parseInt(objectName.attr("change"));
            argus.objectiveName = $.trim(objectName.val());
            argus.objectiveType = parseInt($(objectSpan).attr("term"));
            if ($.trim($("#objectBonus").val()) == "") {
                ncUnits.alert("奖励基数不能为空!");
                return;
            }
            argus.bonus = parseInt($("#objectBonus").val());
            if ($("#modifyWeight").val() == "") {
                ncUnits.alert("目标权重值不能为空!");
                return;
            } else if (parseInt($("#modifyWeight").val()) > 100) {
                ncUnits.alert("目标权重值不能超过100!");
                return;
            }
            argus.weight = parseInt($("#modifyWeight").val());
            argus.checkType = parseInt($("#modifyCheckType span:eq(0)").attr("term"));
            if ($.trim($("#targetInput input:visible").val()) == "") {
                ncUnits.alert("指标值不能为空!");
                return;
            }
            argus.objectiveValue = $.trim($("#targetInput input:visible").val());
            if ($.trim($("#expectInput input:visible").val()) == "") {
                ncUnits.alert("理想值不能为空!");
                return;
            }
            argus.expectedValue = $.trim($("#expectInput input:visible").val());
            argus.description = $.trim($("#modifyRemark").val());
            if ($("#modifyStartTime").val() == "") {
                ncUnits.alert("开始时间不能为空!");
                return;
            }
            argus.startTime = $("#modifyStartTime").val();
            if ($("#modifyEndTime").val() == "") {
                ncUnits.alert("结束时间不能为空!");
                return;
            }
            argus.endTime = $("#modifyEndTime").val();
            if ($("#modifyAlertTime").val() == "") {
                ncUnits.alert("警戒时间不能为空!");
                return;
            }
            argus.alarmTime = $("#modifyAlertTime").val();
            if (argus.objectiveType == 1) {
                argus.responsibleOrg = $(objectSpan).attr("chooseId");
            } else {
                argus.responsibleOrg = $(objectSpan).attr("data_orgName");
            }
            if ($("#responsePerson").val() == "") {
                ncUnits.alert("责任人不能为空!");
                return;
            }
            argus.responsibleUser = parseInt($("#responsePerson").attr("term"));
            if ($("#confirmPerson").val() == "") {
                ncUnits.alert("确认人不能为空!");
                return;
            }
            argus.confirmUser = parseInt($("#confirmPerson").attr("term"));
            argus.status = parseInt(objectName.attr("status"));
            argus.message = $.trim($("#modifyMessage").val());
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/EditObjective",
                dataType: "json",
                data: { data: JSON.stringify(argus), flag: flag },
                success: rsHandler(function (data) {
                    if (data == 0) {
                        ncUnits.alert("修改成功!");
                        $("#object_modify_modal").modal("hide");
                        view.updateList();
                        //if (!$activeNode.closest("ul").hasClass("unfoldChildOpera")) {
                        //    statusCount();
                        //    var parent = $($activeNode).closest(".objectChild");
                        //    if (parent.length != 0) {            //表示刷新授权目标所在层
                        //        loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                        //        $(parent).remove();
                        //    } else {             //表示刷新最顶层
                        //        loadObjectList();
                        //    }
                        //    drawPlanProgress();
                        //} else {
                        //    object_unfoldVar.objectiveId = argus.parentObjective;
                        //    unfoldObjectCall(null, hrHorizontal);
                        //}
                    } else if (data == -1) {
                        ncUnits.alert("修改失败!");
                    }
                    else {
                        ncUnits.alert("第" + data + "个目标公式设置有误!");
                    }
                })
            })
        }
    }

    //判断某变量是否具有非法字符
    function justifyByLetter(txt, name) {
        var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            ncUnits.alert(name);
            return false;
        }
        return true;
    }

    function FormulaLoad(objectiveFormula) {
        FormulaArray.length = 0;
        formulaEditOrAdd = null;
        formulaSetInput[0].length = 0;
        formulaSetInput[1].length = 0;
        var orderNumArray = new Array();//存放每组公式的最大序号
        formulaNum = 0;
        var numValueFlag = null, numValue = null;
        for (var i = 0; i < objectiveFormula.length; i++) {
            if (formulaNum < objectiveFormula[i].formulaNum) {
                formulaNum = objectiveFormula[i].formulaNum;
            }
        }
        for (var i = 0; i <= formulaNum; i++) {
            orderNumArray[i] = 0;
            for (var j = 0; j < objectiveFormula.length; j++) {
                if (objectiveFormula[j].formulaNum == i && orderNumArray[i] < objectiveFormula[j].orderNum) {
                    orderNumArray[i] = objectiveFormula[j].orderNum;
                }
            }
        }
        for (var i = 1; i <= formulaNum; i++) {
            numValueFlag = null; numValue = null;
            for (var j = 0; j < objectiveFormula.length; j++) {
                if (objectiveFormula[j].formulaNum == i) {

                    if (objectiveFormula[j].orderNum == orderNumArray[i] || objectiveFormula[j].orderNum == orderNumArray[i] - 1) {
                        objectiveFormula[j].formulaId = "z";
                        if (objectiveFormula[j].numValue != null) {
                            numValue = objectiveFormula[j].numValue;
                        }
                        else if (objectiveFormula[j].field != null) {
                            numValueFlag = objectiveFormula[j].field;
                        }
                    }
                    else {
                        formulaSetInput[1].push(objectiveFormula[j].valueType);
                        if (objectiveFormula[j].field != null) {
                            formulaSetInput[0].push(formulaSetCheck1[parseInt(objectiveFormula[j].field) - 1]);
                        }
                        if (objectiveFormula[j].operate != null) {
                            if (objectiveFormula[j].operate == "&") {
                                formulaSetInput[0].push("且");
                            }
                            else if (objectiveFormula[j].operate == "|") {
                                formulaSetInput[0].push("或");
                            }
                            else {
                                formulaSetInput[0].push(objectiveFormula[j].operate);
                            }

                        }
                        if (objectiveFormula[j].numValue != null) {
                            formulaSetInput[0].push(objectiveFormula[j].numValue);
                        }
                    }
                    FormulaArray.push(jQuery.extend(true, {}, objectiveFormula[j]));

                }
            }
            var $tr = $("<tr term='" + i + "'></tr>");
            var $formulaContentTd = $("<td style='width:35%;'> <span class='formula-content'title='" + formulaSetInput[0].join("") + "'>" + formulaSetInput[0].join("") + "</span></td>");
            var $dropdownTd = $("<td style='width:17%;'><div class='dropdown'><span class='dropdown-toggle' data-toggle='dropdown'  data-delay='50' role='button' aria-expanded='false'>" +
                "<span term='" + numValueFlag + "' class='new-numValueFlag'>" + (numValueFlag == 8 ? "奖励基数" : "固定奖励") + "</span> <span class='upPic'></span> </span>" +
                "<ul class='dropdown-menu' role='menu'><li><a href='#' term='8'>奖励基数</a></li><li class='divider short'></li><li><a href='#' term='9'>固定奖励</a></li> </ul> </div></td>");

            if (numValueFlag == 8) {
                var $Td = $("<td style='width:28%;'class='choose'><div class='input-group group1'> <span>奖励基数*</span><input type='text' class='new-numValue form-control' value='" + numValue + "' maxlength='10'> </div>" +
                "<div class='input-group group2' style='display:none;'> <input type='text'class='new-numValue  form-control' style='width: 95%;' maxlength='10'> </div></td>");
            }
            else if (numValueFlag == 9) {
                var $Td = $("<td style='width:28%;'class='choose'><div class='input-group group1'style='display:none;'> <span>奖励基数*</span><input type='text' class='new-numValue form-control'  maxlength='10'> </div>" +
                "<div class='input-group group2'> <input type='text'class='new-numValue  form-control' style='width: 95%;' value='" + numValue + "' maxlength='10'> </div></td>");

            }

            var $operateTd = $("<td style='width:19%;'></td>"),
                $delete = $("<a role='button' class='btn btn-default new-formula-delete'>删除</a>");
            $edit = $("<a role='button' class='btn btn-default new-formula-edit'>编辑</a>");
            $delete.click(function () {
                //删除所有该编号的公式数据
                $this = $(this);
                var formulaNumNow = $(this).parents("tr").attr("term");
                ncUnits.confirm({
                    title: '提示',
                    html: '确定要删除该公式吗？',
                    yes: function (layer_confirm) {
                        layer.close(layer_confirm);
                        for (var i = 0; i < FormulaArray.length; i++) {
                            if (FormulaArray[i].formulaNum == formulaNumNow) {
                                FormulaArray.splice(i, 1);
                                i = i - 1;
                            }
                        }
                        for (var i = 0; i < FormulaArray.length; i++) {
                            if (FormulaArray[i].formulaNum > formulaNumNow) {
                                FormulaArray[i].formulaNum = FormulaArray[i].formulaNum - 1;  //修改编号
                            }
                        }
                        $this.parents("tr").remove();
                        $("#formula-set-content tbody tr").each(function () {
                            if ($(this).attr("term") > formulaNumNow) {
                                $(this).attr("term", parseInt($(this).attr("term")) - 1);
                            }
                        });
                    }
                });
            })
            $edit.click(function () {
                formulaEditOrAdd = $(this).parents("tr").attr("term");
                formulaSetInput[0].length = 0;
                formulaSetInput[1].length = 0;
                $("#formula_modal_monitor").text("");
                $("#formula_modal").modal("show");
                for (var j = 0; j < FormulaArray.length; j++) {
                    if (FormulaArray[j].formulaNum == $(this).parents("tr").attr("term") && objectiveFormula[j].formulaId != "z") {
                        formulaSetInput[1].push(objectiveFormula[j].valueType);
                        if (FormulaArray[j].field != null) {
                            formulaSetInput[0].push(formulaSetCheck1[parseInt(FormulaArray[j].field) - 1]);
                        }
                        if (FormulaArray[j].operate != null) {
                            if (FormulaArray[j].operate == "&") {
                                formulaSetInput[0].push("且");
                            }
                            else if (FormulaArray[j].operate == "|") {
                                formulaSetInput[0].push("或");
                            }
                            else {
                                formulaSetInput[0].push(FormulaArray[j].operate);
                            }

                        }
                        if (FormulaArray[j].numValue != null) {
                            formulaSetInput[0].push(FormulaArray[j].numValue)
                        }
                    }
                }
                $("#formula_modal_monitor").text(formulaSetInput[0].join(""));
            });

            $operateTd.append($delete, $edit);
            $tr.append($formulaContentTd, $dropdownTd, $Td, $operateTd);
            $("#formula-set-content tbody").append($tr);
            $(".new-numValue").bind("input", function () {
                controlInput($(this));
            });
            $("#formula-set-content  .dropdown ul a").click(function () {
                if ($(this).attr("term") == 8) {
                    $(this).closest("td").siblings(".choose").find(".group1").css("display", "block");
                    $(this).closest("td").siblings(".choose").find(".group2").css("display", "none");
                }
                else {
                    $(this).closest("td").siblings(".choose").find(".group1").css("display", "none");
                    $(this).closest("td").siblings(".choose").find(".group2").css("display", "block");
                }
                var x = $(this).parents("ul").prev().find("span:eq(0)");
                x.text($(this).text());
                var term = $(this).attr("term");
                x.attr("term", term);
            });
            formulaSetInput[0].length = 0;
            formulaSetInput[1].length = 0;
        }


    }

    function FormaluView(objectiveFormula, formulaMaxArgu, formulaMinArgu, formulaTypeArgu, parentView) {
        parentView.load("/ObjectiveIndex/GetObjectiveFormulaHtml", function () {
            if (objectiveFormula != null) {
                FormulaLoad(objectiveFormula);
            }
            loadFormaluFlag = false;

            $("#maxValue ,#minValue").bind("input", function () {
                controlInput($(this));
            });
            $("#objectiveRule-Sure").off("click");
            $("#objectiveRule-Sure").click(function () {
                if ($(".formula:eq(0)").prop("checked")) {
                }
                else if ($(".formula:eq(1)").prop("checked")) {
                    if ($("#maxValue").val() == "") {
                        ncUnits.alert("最大奖励公式输入框不能为空!");
                        return;
                    }
                    if ($("#minValue").val() == "") {
                        ncUnits.alert("最大惩罚公式输入框不能为空!");
                        return;
                    }
                }
                else if ($(".formula:eq(2)").prop("checked")) {
                    if ($("#formula-set-content tbody>tr").length == 0) {
                        ncUnits.alert("没有设置自定义公式!");
                        return;
                    }
                    var formulaFlag = 0;
                    $("#formula-set-content tbody tr").each(function () {
                        if ($(this).find(".new-numValueFlag").attr("term") == 8 && $(this).find(".group1 .new-numValue").val() == "") {
                            formulaFlag = 1;
                            ncUnits.alert("奖励基数值不能为空");
                            return false;
                        }
                        if ($(this).find(".new-numValueFlag").attr("term") == 9 && $(this).find(".group2 .new-numValue").val() == "") {
                            formulaFlag = 1;
                            ncUnits.alert("固定奖励值不能为空");
                            return false;
                        }
                    });
                    if (formulaFlag == 1) {
                        return;
                    }
                    for (var i = 0; i < FormulaArray.length; i++) {
                        if (FormulaArray[i].formulaId == "z") {
                            FormulaArray.splice(i, 1);
                            i = i - 1;
                        }
                    }
                    $("#formula-set-content tbody tr").each(function () {
                        var $objectiveFormula = {
                            formulaId: 'z',        //公式ID
                            formulaNum: null,       //公式编号
                            orderNum: null,         //排序
                            field: null,            //字段
                            operate: "",       //操作符
                            numValue: ""       //具体值
                        }
                        $objectiveFormula.formulaNum = $(this).attr("term");
                        $objectiveFormula.field = $(this).find(".new-numValueFlag").attr("term");
                        $objectiveFormula.operate = null;
                        $objectiveFormula.numValue = null;
                        var orderNumNow = 0;
                        for (var i = 0; i < FormulaArray.length; i++) {
                            if (orderNumNow < FormulaArray.length && $(this).attr("term") == FormulaArray[i].formulaNum) {
                                orderNumNow = FormulaArray[i].orderNum;
                            }
                        }
                        orderNumNow++;
                        $objectiveFormula.orderNum = orderNumNow;
                        FormulaArray.push($objectiveFormula);

                        var $objectiveFormula = {
                            formulaId: 'z',        //公式ID
                            formulaNum: null,       //公式编号
                            orderNum: null,         //排序
                            field: null,            //字段
                            operate: "",       //操作符
                            numValue: ""       //具体值
                        }
                        $objectiveFormula.formulaNum = $(this).attr("term");
                        $objectiveFormula.field = null;
                        $objectiveFormula.operate = null;
                        if ($(this).find(".new-numValueFlag").attr("term") == "8") {
                            $objectiveFormula.numValue = $(this).find(".group1 .new-numValue").val();
                        }
                        else if ($(this).find(".new-numValueFlag").attr("term") == "9") {
                            $objectiveFormula.numValue = $(this).find(".group2 .new-numValue").val();
                        }

                        orderNumNow++;
                        $objectiveFormula.orderNum = orderNumNow;
                        FormulaArray.push($objectiveFormula);

                    })
                    objectiveRuleSureFlag = true;
                    if (FormulaArray.length == 0) {
                        ncUnits.alert("自定义公式不能为空!");
                        return;
                    }

                }
                else {
                    ncUnits.alert("没有选择公式!");
                    return;
                }
                ncUnits.alert("目标规则已保存");
            });

            $("#objectiveRule-setFormula").off("click");
            $("#objectiveRule-setFormula").click(function () {
                $("#formula_modal").modal("show");
                formulaEditOrAdd = null;
                formulaSetInput[0].length = 0;
                formulaSetInput[1].length = 0;
                $(".formula:eq(2)").trigger("click");
                $("#formula_modal_monitor").text("");
            });


            $(".formula.new_Formular").off("click");
            $(".formula.new_Formular").click(function () {
                if ($(this).attr("term") == 0) {
                    $("#default-formula,#maxValue-div,#minValue-div").addClass("disabledColor");
                    $("#maxValue,#minValue").prop("disabled", true);
                    $("#objectiveRule-setFormula").addClass("disabled");
                }
                else if ($(this).attr("term") == 1) {
                    $("#default-formula,#maxValue-div,#minValue-div").removeClass("disabledColor");
                    $("#maxValue,#minValue").prop("disabled", false);
                    $("#objectiveRule-setFormula").addClass("disabled");
                }
                else if ($(this).attr("term") == 2) {
                    $("#default-formula,#maxValue-div,#minValue-div").addClass("disabledColor");
                    $("#maxValue,#minValue").prop("disabled", true);
                    $("#objectiveRule-setFormula").removeClass("disabled");
                }
            })
            if (formulaTypeArgu == 1) {
                $("#maxValue").val(formulaMaxArgu);
                $("#minValue").val(formulaMinArgu);
            }
            if (formulaTypeArgu != null) {
                $(".formula.new_Formular[term=" + formulaTypeArgu + "]").trigger("click");
            }


            $("#maxValue,#minValue").focus(function () {
                $(".formula:eq(1)").trigger("click");
            });
        });
    }

    $("#object_detail_modal").on('hide.bs.modal', function () {
        $("#FormularViewTab").removeClass("active");
        $("#detailViewTab").addClass("active");
        $("#object_detail_modal .rightModal").hide();
        FormulaList = null;
        $("#object_detail_modal .modal-body").empty();
        FormulaArray.length = 0;
        loadFormaluFlag = true;
    });
    $("#object_detail_modal").on('shown.bs.modal', function () {
        if (detailFlag == 2) {       //如果是审核
            if (loadFormaluFlag) {
                $("#FormularViewTab .modal-body").empty();
                FormaluView(FormulaList, maxValue, minValue, formulaType, $("#FormularViewTab .modal-body"));
            }
        }
    })
    //判断数据库中公式设置是否正确
    function rightFormula() {
        if (formulaType == 1 && (maxValue == null || minValue == null)) {
            return false;
        } else if (formulaType == 2 && (FormulaList == null || FormulaList.length == 0)) {
            return false;
        } else if (formulaType == null) {
            return false;
        }
        return true;

    }
    //输入控制
    $("#modifyWeight,#expectMoney,#targetMoney,#objectBonus,#new-objectiveValue-money,#new-expectedValue-money ,#new-weight,#new-bonus").bind("input", function () {
        controlInput($(this));
    })
    function controlInput($value) {
        //var reg = /^-?\d+(\.\d{0,1}\d{0,1})?$/;
        //var values = $value.val().substring(0, $value.val().length - 1);
        //var getValue = $($value).val();
        //if (reg.test(getValue) == false) {
        //    $($value).val(values);
        //    return
        //}

        var reg = /^\d+(\.\d{0,2})?/,
            vals = $value.val().match(reg);

        $value.val(vals ? vals[0] : "");
    }

    //文档下载  预览 flag=1下载, flag=2预览
    function loadDocument(value, flag, objectiveId, displayName, saveName, extension) {
        if (flag == 1) {       //下载
            $.post("/ObjectiveIndex/Download", { displayName: displayName, saveName: saveName, flag: 0, objectiveId: objectiveId }, function (data) {
                if (data == "success") {
                    window.location.href = "/ObjectiveIndex/Download?displayName=" + escape(displayName) + "&saveName=" + saveName + "&flag=1&objectiveId=" + objectiveId + "";
                    refreshOpera(objectiveId, $(value).closest("table").nextAll("table"));
                }
                else {
                    ncUnits.alert("文件不存在，无法下载!");
                }
                return;
            });
        } else if (flag == 2) {               //预览
            preview(7, saveName, extension);
        }
    }

    //文档的删除
    function documentDelete(value, objectId) {
        var attachmentId = parseInt($(value).closest("tr").attr("term"));
        ncUnits.confirm({
            title: '提示',
            html: '确认要删除吗？',
            yes: function (layer_confirm) {
                layer.close(layer_confirm);
                $.ajax({
                    type: "post",
                    url: "/ObjectiveIndex/DeleteDocument",
                    dataType: "json",
                    data: { objectiveId: objectId, attachmentId: attachmentId },
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert("删除成功");
                            refreshOpera(objectId, $(value).closest("table").nextAll("table"));
                            $(value).closest("tr").remove();
                        } else {
                            ncUnits.alert("删除失败");
                        }
                    })
                })
            }
        });
    }

    //刷新操作日志
    function refreshOpera(objectId, $table) {
        $table.empty();
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectiveNewLogs",              //目标修改URL
            dataType: "json",
            data: { objectiveId: objectId },
            success: rsHandler(function (data) {
                //操作日志
                $.each(data, function (i, v) {
                    message = v.objectiveName
                    if (operateResult[v.result - 1] == "上传文档" || operateResult[v.result - 1] == "下载文档" || operateResult[v.result - 1] == "删除文档") {
                        message = "";
                    }
                    var $tr = $("<tr term=" + v.operateId + " ><td class='col-xs-9' style='position:relative'><span style='color:#58b653'>" + v.reviewUserName + "</span>" +
                        "<span>" + operateResult[v.result - 1] + "</span><span style='color:#58b653;max-width:357px' class='textOverFlow' title=" + message + ">" + message + "</span><a  href='#' class='glyphicon glyphicon-comment'></a>" +
                        "<div class='operateMessage textOverFlow' title=" + v.message + ">意见 : " + v.message + "</div></td>" +
                        "<td class='text-right'><span>" + v.reviewTime + "</span></td></tr>");
                    if ($.inArray(v.result, messageResult) >= 0) {
                        $tr.find(".glyphicon-comment").show();
                        $tr.find(".glyphicon-comment").off("click");
                        $tr.find(".glyphicon-comment").click(function () {
                            if ($tr.find(".operateMessage:hidden").length != 0) {
                                $tr.animate({ height: '40px' }, "5000");
                            } else {
                                $tr.animate({ height: '26px' }, "5000");
                            }
                            $tr.find(".operateMessage").slideToggle("5000");
                        });
                    }

                    $table.append($tr);
                });
            })
        });
    }


    function newInit(objectiveId, Flag) {
        operateFlag = Flag;
        FormulaArray.length = 0;
        formulaEditOrAdd = null;
        formulaNum = 0;
        formulaSetInput[0].length = 0;
        formulaSetInput[1].length = 0;
        Returndata.objectiveFormula.length = 0;
        parentObjectiveStartTime = null;
        parentObjectiveEndTime = null;
        $("#ModalObjectiveTab").trigger("click");

        $("#ModalObjectiveRuleTab").css("display", "none");
        console.log('ssssssss', split_info);
        //数据初始化
        if (newOrDecomposition == 1 || newOrDecomposition == 2) {
            if (loadFormaluFlag) {
                FormaluView(null, null, null, null, $("#modal_con_objectiveRule .modal-body"));
            }
            if (newOrDecomposition == 2) {
                parentObjectiveStartTime = split_info.startTime;
                parentObjectiveEndTime = split_info.endTime;
                $("#new-objectiveType").attr("term", split_info.objectiveType);
                $("#new-objectiveType").attr("value", split_info.responsibleOrg);
                if (split_info.objectiveType == 1) {

                    //1、父目标的目标类型为组织架构的场合（objectiveType=1）：确认人文本框默认显示父目标的责任人，且该文本框和人员图标不可用。
                    $("#new-confirmUser").attr("term", split_info.responsibleUser);
                    $("#new-confirmUser").val(split_info.responsibleUserName);
                    $("#new-confirmUser").attr("title", split_info.responsibleUserName);
                    $("#new-confirmUser").prop("disabled", true);
                    $("#new-confirmUser").siblings("a").addClass("disabled");
                }
                else if (split_info.objectiveType == 2) {

                    //目标对象  分解：父目标的目标类型为人员的场合（objectiveType=2）：该下拉框默认显示父目标的责任人，且该下拉框不可用。                   
                    $("#new-objectiveType").text(split_info.responsibleUserName);
                    $("#new-objectiveType").parent().addClass("disabled");
                    //责任人 目标对象为人员的场合： 默认显示目标对象选定的人员，且该文本框和人员图标不可用。
                    $("#new-responsibleUser").attr("term", split_info.responsibleUser);
                    $("#new-responsibleUser").val(split_info.responsibleUserName);
                    $("#new-responsibleUser").attr("title", split_info.responsibleUserName);
                    $("#new-responsibleUser").prop("disabled", true);
                    $("#new-responsibleUser").siblings("a").addClass("disabled");
                    //确认人 2、父目标的目标类型为人员的场合（objectiveType=2）：确认人文本框默认显示父目标的确认人，且该文本框和人员图标不可用。
                    $("#new-confirmUser").attr("term", split_info.confirmUser);
                    $("#new-confirmUser").val(split_info.confirmUserName);
                    $("#new-confirmUser").attr("title", split_info.confirmUserName);
                    $("#new-confirmUser").prop("disabled", true);
                    $("#new-confirmUser").siblings("a").addClass("disabled");
                }
                if ($("#new-confirmUser").attr("term") == loginId) {
                    $("#ModalObjectiveRuleTab").css("display", "inline-block");
                }
            }
        }
        else {
            console.log('mmmmmmmmmm')
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/GetSimpleObjectInfo",
                dataType: "json",
                data: { objectiveId: objectiveId },
                success: rsHandler(function (data) {
                    if (loadFormaluFlag) {
                        FormaluView(data.objectiveFormula, data.maxValue, data.minValue, data.formula, $("#modal_con_objectiveRule .modal-body"));
                    }

                    Returndata.objectiveId = data.objectiveId;
                    Returndata.parentObjective = null;
                    Returndata.formula = data.formula;
                    $("#new-objectiveName").val(data.objectiveName);
                    $("#new-objectiveType").attr("term", data.objectiveType);
                    $("#new-objectiveType").attr("value", data.responsibleOrg);
                    if (data.objectiveType == 1) {
                        $("#new-objectiveType").text(data.responsibleOrgName);
                    }
                    else if (data.objectiveType == 2) {
                        $("#new-objectiveType").text(data.responsibleUserName);
                        $("#new-responsibleUser").prop("disabled", true);
                        $("#new-responsibleUser").siblings("a").addClass("disabled");
                    }
                    $("#new-objectiveType").parent().removeClass("disabled");
                    $("#new-bonus").val(data.bonus);
                    $("#new-weight").val(data.weight);
                    //默认金额
                    $("#new-checkType-dropdown ul li a:eq(" + (data.checkType - 1) + ")").trigger("click");
                    if (data.checkType == 1) {
                        $("#new-expectedValue-money").val(data.expectedValue);
                        $("#new-objectiveValue-money").val(data.objectiveValue);
                    }
                    else if (data.checkType == 2) {
                        $("#new-expectedValue-date").val(data.expectedValue);
                        $("#new-objectiveValue-date").val(data.objectiveValue);
                    }
                    else {
                        $("#new-expectedValue-number").val(data.expectedValue);
                        $("#new-objectiveValue-number").val(data.objectiveValue);
                    }
                    $("#new-description").val(data.description);
                    $("#new-startTime").val(data.startTime.split("T", 1)[0]);
                    $("#new-endTime").val(data.endTime.split("T", 1)[0]);
                    $("#new-alarmTime").val(data.alarmTime == null ? "" : data.alarmTime.split("T", 1)[0]);
                    $("#new-responsibleUser").val(data.responsibleUserName);
                    $("#new-responsibleUser").attr("title", data.responsibleUserName);
                    $("#new-responsibleUser").attr("term", data.responsibleUser);

                    $("#new-confirmUser").val(data.confirmUserName);
                    $("#new-confirmUser").attr("title", data.confirmUserName);
                    $("#new-confirmUser").attr("term", data.confirmUser);
                    if (data.confirmUser == loginId) {
                        $("#ModalObjectiveRuleTab").css("display", "inline-block");
                        // $("#ModalObjectiveRuleTab").trigger("click");
                    }
                    //时间函数
                    $(" #new-startTime,#new-endTime,#new-alarmTime").off("click")
                    $(" #new-startTime,#new-endTime,#new-alarmTime").click(function () {
                        var id = "#" + $(this).attr("id");
                        timeThree(id, data.minStartTime, data.maxStartTime, data.minEndTime, data.maxEndTime);
                    });

                    $("#objectiveNew_modal .new-date").off("click");
                    $("#objectiveNew_modal .new-date").click(function () {
                        var id = "#" + $(this).prev().attr("id");
                        timeThree(id, data.minStartTime, data.maxStartTime, data.minEndTime, data.maxEndTime);
                    });

                    $("#formula_modal_monitor").text("");
                    $("#objectiveNew_modal .modal-right-content").css("display", "none");
                    $("#formula-set-content tbody").empty();
                    if (data.parentObjective != undefined && data.parentObjective != null) {
                        //对于分解并保存之后之后的点击详情
                        Returndata.parentObjective = data.parentObjective;
                        parentObjectiveStartTime = data.parentObjectivestartTime;
                        parentObjectiveEndTime = data.parentObjectiveendTime;
                        if (data.objectiveType == 1) {
                            //1、父目标的目标类型为组织架构的场合（objectiveType=1）：确认人文本框默认显示父目标的责任人，且该文本框和人员图标不可用。                            
                            $("#new-confirmUser").prop("disabled", true);
                            $("#new-confirmUser").siblings("a").addClass("disabled");
                        }
                        else if (data.objectiveType == 2) {
                            //目标对象  分解：父目标的目标类型为人员的场合（objectiveType=2）：该下拉框默认显示父目标的责任人，且该下拉框不可用。                            
                            $("#new-objectiveType").parent().addClass("disabled");
                            //责任人 目标对象为人员的场合： 默认显示目标对象选定的人员，且该文本框和人员图标不可用。                          
                            $("#new-responsibleUser").prop("disabled", true);
                            $("#new-responsibleUser").siblings("a").addClass("disabled");
                            //确认人 2、父目标的目标类型为人员的场合（objectiveType=2）：确认人文本框默认显示父目标的确认人，且该文本框和人员图标不可用。                         
                            $("#new-confirmUser").prop("disabled", true);
                            $("#new-confirmUser").siblings("a").addClass("disabled");
                        }
                    }
                    if (data.objectiveFormula != null && data.objectiveFormula.length != 0) {
                        Returndata.objectiveFormula.length = 0;
                        for (var i = 0; i < data.objectiveFormula.length; i++) {
                            Returndata.objectiveFormula.push(jQuery.extend(true, {}, data.objectiveFormula[i]));
                        }

                    }

                })
            })


        }

    }

    //新建模态框关闭     值清空
    $("#objectiveNew_modal").on('hide.bs.modal', function () {
        loadFormaluFlag = true;
        $("#modal_con_objectiveRule .modal-body").empty();
        FormulaArray.length = 0;
        $("#objectiveNew_modal .modal-right-content").css("display", "none");
        $("#new-objectiveName").val("");
        $("#new-objectiveType").attr("term", "");
        $("#new-objectiveType").attr("value", "");
        $("#new-objectiveType").text("");
        $("#new-objectiveType").parent().removeClass("disabled");
        $("#new-bonus,#new-weight").val("");
        //默认金额
        $("#new-checkType").attr("term", "1");
        $("#new-checkType").text("金额");
        $("#new-objectiveValue-money-group,#new-expectedValue-money-group").css("display", "block");
        $("#new-objectiveValue-date-group,#new-expectedValue-date-group").css("display", "none");
        $("#new-objectiveValue-number,#new-expectedValue-number").css("display", "none");

        $("#new-expectedValue-money,#new-objectiveValue-money").val("");
        $("#new-expectedValue-date,#new-objectiveValue-date").val("");
        $("#new-expectedValue-number,#new-objectiveValue-number").val("");
        $("#new-startTime,#new-endTime,#new-alarmTime").val("");

        $("#new-responsibleUser,#new-confirmUser,#new-description").val("");
        $("#new-responsibleUser,#new-confirmUser").attr("title", "");
        $("#new-responsibleUser,#new-confirmUser").attr("term", "");
        $("#new-responsibleUser,#new-confirmUser").prop("disabled", false);
        $("#new-responsibleUser,#new-confirmUser").siblings("a").removeClass("disabled");
        $("#formula_modal_monitor").text("");

    });
    //目标对象 点击事件
    $("#new-objectiveType-dropdown").find("ul li a").click(function () {
        if ($(this).attr("term") == 1) {
            $(".modal-right-content").css("display", "block");
            $("#newModal-rightTitle").text("架构");
            var $ztree = $("<ul class='panel-body ztree creDepart' id='node-Ztree' style='max-height: 500px;;overflow:auto'></ul>");
            $("#node-rightContent").empty().append($ztree);
            ZTreeOrgLoad();
        }
        else {
            $(".modal-right-content").css("display", "block");
            $("#newModal-rightTitle").text("人员选择")
            var $ztree = $("<ul class='panel-body ztree creDepart' id='node-Ztree' style='height: 280px;overflow:auto'></ul>");
            var $title = $("<hr class='hrAll' style='width: 412px;'/><div class='checkbox' style='padding-left: 10px;margin-bottom: -5px;'>" +
                "<label><input type='checkbox' id='newModal-withSub'>包含下级</label></div><hr style='margin-bottom: 0px;'/>");
            var $nodeList = $("<div id='newModal-ChooseList'><ul class='list-unstyled' style='margin-top: -5px;'></ul></div>");
            $("#node-rightContent").empty().append([$ztree, $title, $nodeList]);
            ZTreeLoad(1);
        }
    });

    //架构ztree加载
    function ZTreeOrgLoad() {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#node-Ztree"), $.extend({
                    callback: {
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {
                            //选中事件           
                            $("#new-responsibleUser").prop("disabled", false);
                            $("#new-responsibleUser").siblings("a").prop("disabled", false);
                            $("#new-objectiveType").text(node.name);
                            $("#new-objectiveType").attr("term", "1");
                            $("#new-objectiveType").attr("value", node.id);
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    check: {
                        autoCheckTrigger: false,
                        enable: true,
                        chkStyle: "radio",
                        radioType: "all",
                        chkboxType: { "Y": "", "N": "" } //Y：勾选，p：关联父节点，s：关联子节点，N：取消勾选
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    //人员复选框点击事件
    function appendperson() {
        $(".person_list input[type='checkbox']").click(function () {
            var checked = $(this).prop('checked');
            var personId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
            var personName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
            var showflag = true;
            if (checked == true) {
                if (personModal == 2) {
                    $(".person_list ul input:checked").prop("checked", false);
                    $(this).prop('checked', true);
                    $('#HR_modal_chosen_count').text(1);
                    var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                    $close.click(function () {
                        $(".person_list ul input:checked").prop("checked", false);
                        $(this).parent().remove();
                        $('#HR_modal_chosen_count').text(0);
                    });
                    $("#HR_modal_chosen").empty().append($checked.append($close));
                } else {
                    $(this).prop('checked', true);
                    $("#HR_modal_chosen li").each(function () {
                        if ($(this).attr("term") == personId) {
                            showflag = false;
                            return true;
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
                }
            } else {
                $(this).prop('checked', false);
                if (personModal == 2) {
                    $("#HR_modal_chosen").empty();
                    $('#HR_modal_chosen_count').text(0);
                } else {
                    $(this).parents(".person_list").find("li").each(function () {
                        if ($(this).attr('term') && $(this).attr('term') == personId) {
                            $(this).parents(".list-inline").find("li:eq(0) input").prop('checked', false);
                        }
                    });
                    $("#HR_modal_chosen li").each(function () {
                        if ($(this).attr('term') == personId) {
                            $(this).remove();
                            if ($("#HR_modal_chosen li").length <= 0) {
                                $("#person-selectall").prop("checked", false);
                            }
                            $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                        }
                    });
                }
            }
        });
    }

    // 人员选择ztree加载
    function ZTreeLoad(flag) { //flag=1标记为目标对象的人员选择 flag=2标记为责任人的人员选择 flag=3标记为确认人的人员选择
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#node-Ztree"), $.extend({
                    callback: {
                        onClick: function (e, id, node) {
                            if ($("#newModal-withSub").prop("checked")) {
                                AppendPersonJob(node.id, flag, 1);
                            }
                            else {
                                AppendPersonJob(node.id, flag, 0);
                            }
                            $("#newModal-withSub").off("click");
                            $("#newModal-withSub").click(function () {
                                if ($(this).prop("checked")) {
                                    AppendPersonJob(node.id, flag, 1);
                                }
                                else {
                                    AppendPersonJob(node.id, flag, 0);
                                }
                            })

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
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    function AppendPersonJob(id, flag, withsub) {
        var url, argus;
        url = "/Shared/GetUserList";

        if (withsub == 0) { argus = { withSub: 0, organizationId: id, withUser: true }; }
        else { argus = { withSub: 1, organizationId: id }; }
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data: argus,
            success: rsHandler(function (data) {
                $("#newModal-ChooseList ul").empty();
                $.each(data, function (i, v) {
                    var $li = $("<li></li>");
                    var $label = $("<div class='checkbox'><label></label></div>");
                    var $radio, $span;
                    $radio = $("<input type='radio'  name='listRadio' value=" + v.userId + ">");
                    $span = $("<span title=" + v.userName + "-" + v.organizationName + " >" + v.userName + "-" + v.organizationName + "</span>");
                    $radio.click(function () {
                        //flag=1标记为目标对象的人员选择 flag=2标记为责任人的人员选择 flag=3标记为确认人的人员选择
                        if (flag == 1) {
                            $("#new-objectiveType").text($(this).siblings("span").text());
                            $("#new-objectiveType").attr("term", "2");
                            $("#new-objectiveType").attr("value", v.organizationName);//记录组织架构名字
                            $("#new-responsibleUser").val($(this).siblings("span").text());
                            $("#new-responsibleUser").attr("title", $(this).siblings("span").text());
                            $("#new-responsibleUser").attr("term", $(this).val());
                            $("#new-responsibleUser").prop("disabled", true);
                            $("#new-responsibleUser").siblings("a").addClass("disabled");
                            //目标对象为人员的场合：责任人 显示目标对象选定的人员， 且该文本框和人员图标不可用。
                        }
                        else if (flag == 2) {
                            $("#new-responsibleUser").val($(this).siblings("span").text());
                            $("#new-responsibleUser").attr("title", $(this).siblings("span").text());
                            $("#new-responsibleUser").attr("term", $(this).val());

                        }
                        else {
                            $("#new-confirmUser").val($(this).siblings("span").text());
                            $("#new-confirmUser").attr("title", $(this).siblings("span").text());
                            $("#new-confirmUser").attr("term", $(this).val());
                            if ($(this).val() == loginId) {
                                $("#ModalObjectiveRuleTab").css("display", "inline-block");
                            }
                            else {
                                $("#ModalObjectiveRuleTab").css("display", "none");
                            }
                        }
                    });

                    $label.find("label").append([$radio, $span]);
                    $li.append($label);
                    $("#newModal-ChooseList  ul").append($li);
                })

            })
        });
    }
    //上传文档
    var parttern = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    var fileNum = 0, completeNum = 0;
    function documentUpload(fileUpload, progressHtml, documentTable, operaTable) {
        var objectiveId = parseInt($($activeNode).closest("ul").attr("term"));
        $(fileUpload).fileupload({
            url: "/ObjectiveIndex/UploadDocument",
            dataType: 'text',
            formData: { objectiveId: objectiveId },
            add: function (e, data) {
                layer.closeTips();
                var isSubmit = true;
                $.each(data.files, function (index, value) {
                    if (!parttern.test(value.name)) {
                        ncUnits.alert("你上传文件格式不对");
                        isSubmit = false;
                        return;
                    } else if (value.size > 52428800) {
                        ncUnits.alert("你上传文件过大(最大50M)");
                        isSubmit = false;
                        return;
                    } else {
                        //显示进度条
                        fileNum++;
                        $(progressHtml).show();
                    }
                })
                if (isSubmit) {
                    data.submit();
                }
            },
            complete: function (e, data) {
                completeNum++;
                if (completeNum == fileNum) {
                    $(progressHtml).hide();
                    $(progressHtml).find('.bar').css('width', '0');
                    refreshDoc(objectiveId, documentTable);
                    refreshOpera(objectiveId, operaTable);
                    fileNum = 0;
                    completeNum = 0;
                }
            },
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $(progressHtml).find('.bar').css('width', progress + '%');
            },
            error: function (e, data) {
                ncUnits.alert('上传出错');
            },
            done: function (e, data) {
                //   $("#object_modify_modal .progress").hide();
                if ($.parseJSON(data.result).status == 0) {
                    ncUnits.alert("上传失败");
                } else {
                    ncUnits.alert("上传成功");
                }
            }
        });
    }


    //目标提交
    function objectCommit(checkType) {
        $(".rightModal").css({
            "left": ($(".rightModal").parents('.modal-content').width() - 5)
        });
        $("#object_detail_modal .rightModal").show();
        $("#rightTitle").text("完成情况");
        var $Actual = $('<div class="form-inline" id="completeRight"><label class="control-label" >实际值:</label></div>');
        if (checkType == 1) {
            var $input = $('<div class="input-group"><input type="text" class="form-control"  maxlength="20" >' +
                '<span class="input-group-addon">' +
                '<span class="glyphicon glyphicon-yen" ></span></span></div>');
            $input.find("input").bind('input', function () {
                controlInput($(this));
            });

        } else if (checkType == 2) {
            var $input = $('<div class="input-group"><input type="button" class="form-control" id="completeDate"  placeholder="日期"><span class="input-group-addon">' +
                '<span class="dateCorn" ></span></span></div>');
        } else {
            var $input = $('<input type="text" class="form-control"  maxlength="20" oninput="value=value.replace(/[^\d]/g,"")">');
            $input.find("input").bind('input', function () {
                controlInput($(this));
            });
        }
        $Actual.append($input);
        var $cancelBtn = $("<a class='btn btn-transparency btn_xs pull-right'>取消</a>");
        var $commitBtn = $("<a class='btn btn-transparency btn_xs pull-right'>提交</a>");

        //提交
        $commitBtn.click(function () {
            var actualValue = $.trim($Actual.find("input").val());
            if (actualValue == "") {
                ncUnits.alert("实际值不得为空!");
                return;
            }
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/SubmitObjectiveExecuteResult",
                dataType: "json",
                data: { objectiveId:objectiveId, actualValue: actualValue },
                success: rsHandler(function (data) {
                    if (data) {
                        ncUnits.alert("提交成功！");
                        $("#object_detail_modal").modal('hide');
                        view.updateList();
                        //statusCount();
                        //var parent = $($activeNode).closest(".objectChild");
                        //if (parent.length != 0) {            //表示刷新授权目标所在层
                        //    loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                        //    $(parent).remove();
                        //} else {             //表示刷新最顶层
                        //    loadObjectList();
                        //}
                        //drawPlanProgress();
                    } else {
                        ncUnits.alert("提交失败!");
                    }
                })
            });
        });

        //取消
        $cancelBtn.click(function () {
            $("#object_detail_modal").modal('hide');
        });

        $("#rightContent").empty().append([$Actual, $commitBtn, $cancelBtn]);
        if (checkType == 2) {
            timeChosen("#completeDate", null, null);
            $input.find("span").click(function () {
                timeChosen("#completeDate", null, null);
            });
        }
    }
    //刷新文档
    function refreshDoc(objectId, $table) {
        $table.empty();
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectiveNewDocuments",              //文档获取
            dataType: "json",
            data: { objectiveId: objectId },
            success: rsHandler(function (data) {
                //相关文档
                $table.empty();
                $.each(data, function (i, v) {
                    var $tr = $("<tr term=" + v.attachmentId + "><td class='col-xs-9'><span class='textOverFlow maxWidthTwo ' title=" + v.displayName + ">" + v.displayName + "</span></td>" +
                        "<td class='text-right'><a href='#' class='documentLoad'>下载</a></td><td class='text-right'  style='width: 38px;'><a href='#' class='documentPre'>预览</a></td></tr>");
                    var $del = $("<td class='text-right'  style='width: 38px;'><a href='#' >删除</a></td>");
                    $($del).click(function () {        //文档删除
                        documentDelete(this, objectId);
                    });
                    $($tr).append($del);
                    $tr.find(".documentLoad").click(function () {       //文档下载
                        loadDocument(this, 1, objectId, v.displayName, v.saveName);

                    });
                    $tr.find(".documentPre").click(function () {       //文档预览
                        preview(7, v.saveName, v.extension);
                    });
                    $table.append($tr);
                });
            })
        });

    }

    //------------------人力资源
    var personOrgId;
    var personWithSub = false;
    $("#HR_modal").on('show.bs.modal', function () {
        if (personModal == 1) {                      //筛选条件的人员选择
            $('.adopt .pull-right').show();
            var $chosePerson = $(".selectedCondition li span[classify='人员']");
            $('#HR_modal_chosen_count').text($chosePerson.length);
            $chosePerson.each(function () {
                var $checked = $("<li term=" + $(this).attr("term") + "><span>" + $(this).text() + "</span></li>"),
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
            });
        } else {
            $('.adopt .pull-right').hide();
        }

        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#HR_modal_folder"), $.extend({
                    callback: {
                        onClick: function (e, id, node) {
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
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    });

    function hrSubmit() {
        if (personModal == 1) {
            circleClean();
            $(".selectedCondition li span[classify='人员']").parent().remove();
            objectListArgus.length = 0;
            $("#HR_modal_chosen li").each(function () {
                objectListArgus.person.push(parseInt($(this).attr('term')));
                addSelects($(this).find("span:eq(0)").text(), $(this).attr("term"), "人员");
            });
            $('#HR_modal').modal('hide');
            loadObjectList();
        }
        else {
            if ($('#HR_modal_chosen_count').text() == 0) {
                ncUnits.alert("请选择授权人!");
            } else {
                var personId = parseInt($("#HR_modal_chosen li").attr("term"));
                if (personId == object_info.responsibleUser) {
                    ncUnits.alert("责任人与授权人不得相同!");
                } else {
                    $.ajax({
                        type: "post",
                        url: "/ObjectiveIndex/AuthorizeObjective",
                        dataType: "json",
                        data: { objectiveId:objectiveId, authorizedUser: personId },
                        success: rsHandler(function (data) {
                            ncUnits.alert("授权成功!");
                            $('#HR_modal').modal('hide');
                            view.updateList();
                            //statusCount();
                            //var parent = $($activeNode).closest(".objectChild");
                            //if (parent.length != 0) {            //表示刷新授权目标所在层
                            //    //loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                            //    $(parent).remove();
                            //} else {             //表示刷新最顶层
                            //    //loadObjectList();
                            //}
                            //drawPlanProgress();       //刷新饼图
                        })
                    })
                }
            }
        }


    }


    //架构选择
    $("#modifyObjectInput a[term='1']").click(function () {
        $(".rightModal").css({
            "left": ($(".rightModal").parents('.modal-content').width() - 5)
        });
        $("#object_modify_modal .rightModal").show();
        $("#chooseTitle").text("部门选择");
        var $ztrees = $("<div class='ztree' id='orgChooseZtree'></div>");
        $("#chooseContent").empty().append($ztrees);
        treeOrgLoadCheck($ztrees);
    });
    //部门选择树
    function treeOrgLoadCheck(valueZtree) {
        var $spanModify = $("#modifyObjectInput span:eq(0)");
        var type = parseInt($spanModify.attr("term"));
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init(valueZtree, $.extend({
                    callback: {
                        onNodeCreated: function (e, id, node) {
                            var flag = false;
                            if (modifyOldOrg != -1) {
                                if (node.id == modifyOldOrg) {
                                    flag = true;
                                }
                            } else if (type == 1 && node.id == parseInt($spanModify.attr("chooseId"))) {
                                flag = true;
                            }
                            if (flag) {
                                var ztrees = $.fn.zTree.getZTreeObj(id);
                                ztrees.checkNode(node, true, false);
                                $spanModify.attr('chooseId', node.id);
                                $spanModify.text(node.name).attr("title", node.name);
                                $spanModify.attr("term", 1);
                                $("#responsePerson").attr("disabled", false);
                                $("#responsePerson").next().attr("disabled", false);
                                modifyOldOrg = node.id;
                            }
                        },
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {        //选中事件
                            $spanModify.attr('chooseId', node.id);
                            $spanModify.text(node.name).attr("title", node.name);
                            $spanModify.attr("term", 1);
                            modifyOldOrg = node.id;
                            $("#responsePerson").attr("disabled", false);
                            $("#responsePerson").next().attr("disabled", false);
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    check: {
                        enable: true,
                        chkStyle: "radio",
                        radioType: "all"
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    //人员选择
    $("#modifyObjectInput a[term='2']").click(function () {
        personChoose(1);
    });

    //目标修改弹框关闭 值清空
    $("#object_modify_modal").on('hide.bs.modal', function () {
        $("#object_modify_modal input").val("");
        $("#object_modify_modal table").empty();
        $("#object_modify_modal .rightModal").hide();
        $("#modifyMessage").val("");
        modifyOldPerson = -1;
        modifyOldOrg = -1;
        modifyOldPOrg = -1;
        modifyOldPOrgName = null;
        perWithSub = 0;
        $("#objectModifyTab").trigger("click");
        $("#modifyFormularView .modal-body").empty();
        FormulaList = null;
        FormulaArray.length = 0;
        loadFormaluFlag = true;
    });

    $("#new-objectiveValue-date,#new-expectedValue-date,#new-startTime,#new-endTime,#new-alarmTime").siblings("a").click(function () {
        $(this).siblings("input").trigger("click");
    });

    $("#new-responsibleUser,#new-confirmUser").siblings("a").click(function () {
        $(this).siblings("input").trigger("focus");
    });


    //确认人
    $("#new-confirmUser").focus(function () {
        // if (newOrDecomposition == 1) { //新建 }
        $(".modal-right-content").css("display", "block");
        $("#newModal-rightTitle").text("人员选择");
        var $ztree = $("<ul class='panel-body ztree creDepart' id='node-Ztree' style='height: 280px;overflow:auto;'></ul>");
        var $title = $("<hr class='hrAll' style='width: 412px;'/><div class='checkbox' style='padding-left: 10px;margin-bottom: -5px;'>" +
            "<label><input type='checkbox' id='newModal-withSub'>包含下级</label></div><hr style='margin-bottom: 0px;'/>");
        var $nodeList = $("<div id='newModal-ChooseList'><ul class='list-unstyled' style='margin-top: -5px;'></ul></div>");
        $("#node-rightContent").empty().append([$ztree, $title, $nodeList]);
        ZTreeLoad(3);

        //flag=1标记为目标对象的人员选择 flag=2标记为责任人的人员选择 flag=3标记为确认人的人员选择


    });

    //责任人
    $("#new-responsibleUser").focus(function () {
        if ($("#new-objectiveType").attr("term") == "") {
            ncUnits.alert("请先选择目标对象");
            return;
        }
        if ($("#new-objectiveType").attr("term") == "1") {
            // 1、目标对象为组织架构的场合：点击文本框或人员图标，弹出人员选择画面，选定人员后，将该人员显示到责任人文本框。
            $(".modal-right-content").css("display", "block");
            $("#newModal-rightTitle").text("人员选择");
            var $ztree = $("<ul class='panel-body ztree creDepart' id='node-Ztree' style='height: 280px;overflow:auto'></ul>");
            var $title = $("<hr class='hrAll' style='width: 412px;'/><div class='checkbox' style='padding-left: 10px;margin-bottom: -5px;'>" +
                "<label><input type='checkbox' id='newModal-withSub'>包含下级</label></div><hr style='margin-bottom: 0px;'/>");
            var $nodeList = $("<div id='newModal-ChooseList'><ul class='list-unstyled' style='margin-top: -5px;'></ul></div>");
            $("#node-rightContent").empty().append([$ztree, $title, $nodeList]);
            ZTreeLoad(2); //flag=1标记为目标对象的人员选择 flag=2标记为责任人的人员选择 flag=3标记为确认人的人员选择
        }
        //2、目标对象为人员的场合： 显示目标对象选定的人员， 且该文本框和人员图标不可用。
    });


    //人员资源模态框关闭事件
    $("#HR_modal").on('hide.bs.modal', function () {
        $("#HR_modal_search").val('');
        $("#person-haschildren").prop("checked", false);
        $("#person-selectall").prop("checked", false);
        $("#HR_modal_chosen_count").text(0);
        $(".person_list").empty();
        $("#HR_modal_chosen li").remove();
    });
    /*人力资源 结束*/
    //人员搜索
    $("#HR_modal_search").selection({
        url: "/Shared/GetUserListByName",
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
                if (personModal == 1) {
                    $("#HR_modal_chosen").append($checked.append($close));
                } else {
                    $("#HR_modal_chosen").empty().append($checked.append($close));
                }
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

    //目标审核+
    function checksFunc(valueData) {
        var objectiveId = valueData.objectiveId;
        var objectiveFormula = {
            "formula": null,
            "maxValue": null,
            "minValue": null,
            "objectiveFormula": null
        }

        $(".rightModal").css({
            "left": ($(".rightModal").parents('.modal-content').width() - 5)
        });
        $("#object_detail_modal .rightModal").show();

        $("#rightTitle").text("意见");
        var $text = $("<textarea class='form-control message'  rows='4' maxlength='250'></textarea>");
        var $passBtn = [];
        $passBtn[0] = $("<a class='btn btn-transparency btn_xs pull-right' id='passBtn' term='6'>通过</a>");
        $passBtn[1] = $("<a class='btn btn-transparency btn_xs pull-right' id='noPassBtn'  term='7'>不通过</a>");

        $.each($passBtn, function (i, v) {
            $(v).click(function () {
                var message = $.trim($($text).val());
                var result = parseInt($(this).attr("term"));
                objectiveFormula.formula = parseInt($("#FormularViewTab input[name='formula']:checked").attr("term"));
                if (result == 6) {
                    if (objectiveFormula.formula == null || isNaN(objectiveFormula.formula) || (objectiveFormula.formula == 2 && FormulaArray.length == 0)) {
                        ncUnits.alert("审核通过必须设置公式！");
                        return;
                    }
                }
                if (objectiveFormula.formula == 1) {
                    var confirmText = null;
                    if ($("#maxValue").val() == "") {
                        if (rightFormula() == true) {
                            confirmText = "最大奖励系数不得为空!是否恢复之前公式?";
                        } else {
                            ncUnits.alert("最大奖励系数不得为空！");
                            return;
                        }
                    } else if ($("#minValue").val() == "") {
                        if (rightFormula() == true) {
                            confirmText = "最大惩罚系数不得为空!是否恢复之前公式?";
                        } else {
                            ncUnits.alert("最大惩罚系数不得为空！");
                            return;
                        }
                    }
                    if (confirmText != null) {
                        ncUnits.confirm({
                            title: '提示',
                            html: confirmText,
                            yes: function (layer_delete) {                 //恢复原来设置
                                layer.close(layer_delete);
                                objectiveFormula.formula = valueData.formula;
                                objectiveFormula.objectiveFormula = valueData.objectiveFormula;
                                objectiveFormula.maxValue = valueData.maxValue;
                                objectiveFormula.minValue = valueData.minValue;
                                checkAfterConfirm(result, message);
                            },
                            no: function (layer_delete) {                 //保存
                                layer.close(layer_delete);
                                $("#objectRoleLabel").trigger("click");
                            }
                        })
                    } else {
                        objectiveFormula.maxValue = $("#maxValue").val();
                        objectiveFormula.minValue = $("#minValue").val();
                        checkAfterConfirm(result, message);
                    }

                } else if (objectiveFormula.formula == 2) {
                    if (objectiveRuleSureFlag == false) {
                        if (rightFormula() == true) {                  //之前设置公式正确 才可恢复
                            ncUnits.confirm({
                                title: '提示',
                                html: '你所设置的公式还没保存,是否恢复之前公式?',
                                yes: function (layer_delete) {                  //恢复为最初公式
                                    layer.close(layer_delete);
                                    objectiveFormula.formula = valueData.formula;
                                    objectiveFormula.objectiveFormula = valueData.objectiveFormula;
                                    objectiveFormula.maxValue = valueData.maxValue;
                                    objectiveFormula.minValue = valueData.minValue;
                                    checkAfterConfirm(result, message);
                                },
                                no: function (layer_delete) {                     //保存   
                                    layer.close(layer_delete);
                                    $("#objectRoleLabel").trigger("click");
                                }
                            })
                        } else {
                            ncUnits.alert("公式还没有保存!");
                            return;
                        }
                    } else {
                        for (var j = 0 ; j < FormulaArray.length; j++) {
                            if (FormulaArray[j].formulaId == "z") {
                                FormulaArray[j].formulaId = null;
                            }
                        }
                        objectiveFormula.objectiveFormula = FormulaArray;
                        checkAfterConfirm(result, message);
                    }
                } else if (objectiveFormula.formula == 0) {
                    checkAfterConfirm(result, message);
                } else {
                    objectiveFormula.formula = valueData.formula;
                    objectiveFormula.objectiveFormula = valueData.objectiveFormula;
                    objectiveFormula.maxValue = valueData.maxValue;
                    objectiveFormula.minValue = valueData.minValue;
                    checkAfterConfirm(result, message);
                }
            })
        });

        function checkAfterConfirm(result, message) {
            var dataArgus = {
                "objectiveId": objectiveId,
                "message": message,
                "result": result,
                "objectiveFormulaInfo": objectiveFormula
            }
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/ApproveObjective",
                dataType: "json",
                data: { data: JSON.stringify(dataArgus) },
                success: rsHandler(function (data) {
                    if (data.result == 0) {
                        ncUnits.alert("审核成功！");
                        $("#object_detail_modal").modal('hide');
                        view.updateList();
                        //statusCount();
                        //var parent = $($activeNode).closest(".objectChild");
                        //if (parent.length != 0) {            //表示刷新授权目标所在层
                        //    //loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                        //    $(parent).remove();
                        //} else {             //表示刷新最顶层
                        //    loadObjectList();
                        //}
                        //drawPlanProgress();
                    } else if (data.result == -1) {
                        ncUnits.alert("审核失败！");
                    }
                    else {
                        ncUnits.alert("第" + data.result + "个目标公式设置有误！");
                    }
                })
            })
        }
        $("#rightContent").empty().append([$text, $passBtn[0], $passBtn[1]]);
    }


    $("#newModal-Save").click(function () {
        newModalDataSave(2);
    });
    $("#newModal-Submit").click(function () {
        newModalDataSave(1);//提交时
    });
    //新建或分解数据保存
    function newModalDataSave(flag) {
        //目标对象正确性检验      
        if ($("#new-objectiveType").attr("term") == "") {
            ncUnits.alert("目标对象不能为空!");
            return;
        }
        //时间正确性检验
        if ($("#new-startTime").val() == "") {
            ncUnits.alert("开始时间不能为空!");
            return;
        }
        if ($("#new-endTime").val() == "") {
            ncUnits.alert("截止时间不能为空!");
            return;
        }
        if ($("#new-responsibleUser").attr("term") == "") {
            ncUnits.alert("责任人不能为空!");
            return;
        }
        if ($("#new-confirmUser").attr("term") == "") {
            ncUnits.alert("确认人不能为空!");
            return;
        }
        if ($("#new-responsibleUser").attr("term") == $("#new-confirmUser").attr("term")) {
            ncUnits.alert("责任人和确认人不能相同");
            return;
        }
        if ($("#new-weight").val() != "" && parseInt($("#new-weight").val()) > 100) {
            ncUnits.alert("目标权重值不能超过100!");
            return;
        }
        //目标分解状况下的时间判断
        if (parentObjectiveStartTime != null && parentObjectiveStartTime != "" && $("#new-startTime").val() < parentObjectiveStartTime) {
            ncUnits.alert("分解子目标的开始时间不应早于父目标");
            return;
        }
        if (parentObjectiveEndTime != null && parentObjectiveEndTime != "" && $("#new-endTime").val() > parentObjectiveEndTime) {
            ncUnits.alert("分解子目标的结束时间不应晚于父目标");
            return;
        }
        //目标名称正确性检验
        var name = $.trim($("#new-objectiveName").val());
        var reg = /[`~!@#$%^&*()_+<>?:"{},\/;'[\]]/im;
        if (name != "") {
            if (name.indexOf('null') >= 0 || name.indexOf('NULL') >= 0 || name.indexOf('&nbsp') >= 0 || reg.test(name) || name.indexOf('</') >= 0) {
                ncUnits.alert("目标名称存在非法字符!");
                return;
            }
        }
        //备注项目非法字符验证
        var name = $.trim($("#new-description").val());
        if (name != "") {
            if (name.indexOf('null') >= 0 || name.indexOf('NULL') >= 0 || name.indexOf('&nbsp') >= 0 || reg.test(name) || name.indexOf('</') >= 0) {
                ncUnits.alert("备注项目存在非法字符!");
                return;
            }
        }

        if (flag == 1) {//提交时
            //目标名称正确性检验
            var name = $.trim($("#new-objectiveName").val());
            if (name == "") {
                ncUnits.alert("目标名称不能为空!");
                return;
            }
            //目标基数正确性检验
            if ($("#new-bonus").val() == "") {
                ncUnits.alert("奖励基数不能为空!");
                return;
            }
            //目标权重正确性检验
            if ($("#new-weight").val() == "") {
                ncUnits.alert("目标权重值不能为空!");
                return;
            }
            else if (parseInt($("#new-weight").val()) > 100) {
                ncUnits.alert("目标权重值不能超过100!");
                return;
            }
            //指标值和理想值正确性检验
            if (($("#new-checkType").attr("term") == 1 && $("#new-expectedValue-money").val() == "") || ($("#new-checkType").attr("term") == 2 && $("#new-expectedValue-date").val() == "") || ($("#new-checkType").attr("term") == 3 && $("#new-expectedValue-number").val() == "")) {
                ncUnits.alert("理想值不能为空!");
                return;
            }
            if (($("#new-checkType").attr("term") == 1 && $("#new-objectiveValue-money").val() == "") || ($("#new-checkType").attr("term") == 2 && $("#new-objectiveValue-date").val() == "") || ($("#new-checkType").attr("term") == 3 && $("#new-objectiveValue-number").val() == "")) {
                ncUnits.alert("指标值不能为空!");
                return;
            }
            //时间正确性检验           
            //if ($("#new-alarmTime").val() == "") {
            //    ncUnits.alert("警戒时间不能为空!");
            //    return;
            //}


            //公式正确性检验
            if (loginId == $("#new-confirmUser").attr("term")) {
                if ($(".formula:eq(0)").prop("checked")) {
                    Returndata.formula = 0;
                }
                else if ($(".formula:eq(1)").prop("checked")) {
                    if ($("#maxValue").val() == "") {
                        ncUnits.alert("最大奖励公式输入框不能为空!");
                        return;
                    }
                    if ($("#minValue").val() == "") {
                        ncUnits.alert("最大惩罚公式输入框不能为空!");
                        return;
                    }

                }
                else if ($(".formula:eq(2)").prop("checked")) {
                    if (FormulaArray.length == 0) {
                        ncUnits.alert("自定义公式不能为空!");
                        return;
                    }
                }
                else {
                    ncUnits.alert("公式没有选择!");
                    return;
                }
            }
            else {
                // Returndata.formula = null;
                Returndata.objectiveFormula.length = 0;
            }


        }
        Returndata.objectiveName = $("#new-objectiveName").val();
        Returndata.objectiveType = $("#new-objectiveType").attr("term");
        Returndata.bonus = $("#new-bonus").val();
        Returndata.weight = $("#new-weight").val().split("%", 1)[0];

        Returndata.checkType = $("#new-checkType").attr("term");
        if (Returndata.checkType == 1) {
            Returndata.expectedValue = $("#new-expectedValue-money").val();
            Returndata.objectiveValue = $("#new-objectiveValue-money").val();
        }
        else if (Returndata.checkType == 2) {
            Returndata.expectedValue = $("#new-expectedValue-date").val();
            Returndata.objectiveValue = $("#new-objectiveValue-date").val();
        }
        else if (Returndata.checkType == 3) {
            Returndata.expectedValue = $("#new-expectedValue-number").val();
            Returndata.objectiveValue = $("#new-objectiveValue-number").val();
        }
        Returndata.description = $("#new-description").val();
        Returndata.startTime = $("#new-startTime").val();
        Returndata.endTime = $("#new-endTime").val();
        //Returndata.alarmTime = $("#new-alarmTime").val();

        Returndata.responsibleOrg = $("#new-objectiveType").val();

        Returndata.responsibleUser = $("#new-responsibleUser").attr("term");
        Returndata.confirmUser = $("#new-confirmUser").attr("term");
        var zRegister = [];
        if ($(".formula:eq(0)").prop("checked")) {
            Returndata.formula = 0;
        }
        else if ($(".formula:eq(1)").prop("checked")) {
            Returndata.formula = 1;
            Returndata.maxValue = $("#maxValue").val();
            Returndata.minValue = $("#minValue").val();
        }
        else if ($(".formula:eq(2)").prop("checked")) {
            if (objectiveRuleSureFlag == true) {
                Returndata.formula = 2;
                Returndata.objectiveFormula.length = 0;
                for (var i = 0; i < FormulaArray.length; i++) {
                    if (FormulaArray[i].formulaId == "z") {
                        FormulaArray[i].formulaId = null;
                        zRegister.push(i);
                    }
                    Returndata.objectiveFormula.push(jQuery.extend(true, {}, FormulaArray[i]));
                }
            }
            else {

                ncUnits.alert("公式没有保存!");
                return;
            }
        }

        if (Returndata.confirmUser != loginId && Returndata.formula != null) {
            Returndata.formula = null;
            Returndata.objectiveFormula.length = 0;
        }
        if (newOrDecomposition == 1) {
            delete Returndata.parentObjective;
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/NewObjective",
                dataType: "json",
                data: { flag: flag, data: JSON.stringify(Returndata) },
                success: rsHandler(function (data) {
                    if (data == 0) {
                        ncUnits.alert("新建目标成功");
                        $("#objectiveNew_modal").modal("hide");
                        statusCount();
                        loadObjectList();
                        drawPlanProgress();
                    } else if (data == -1) {
                        ncUnits.alert("新建目标失败");
                        $("#objectiveNew_modal").modal("hide");
                    } else {
                        ncUnits.alert("第" + data + "个目标公式设置有误!");
                        for (var i = 0; i < zRegister.length; i++) {
                            FormulaArray[zRegister[i]].formulaId = "z";
                        }
                    }

                })
            });
        }
        else if (newOrDecomposition == 2) {
            Returndata.parentObjective = split_info.objectiveId;
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/SplitObjective",
                dataType: "json",
                data: { flag: flag, data: JSON.stringify(Returndata) },
                success: rsHandler(function (data) {
                    if (data == 0) {
                        ncUnits.alert("分解目标成功");
                        $("#objectiveNew_modal").modal("hide");
                        unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
                        if (unflodFlag == 1) {
                            unflodFlag = 2;
                        }
                    }
                    else if (data == -1) {
                        ncUnits.alert("分解目标失败");
                        $("#objectiveNew_modal").modal("hide");
                    }
                    else {
                        ncUnits.alert("第" + data + "个目标公式设置有误!");
                    }

                }),
                error: function () {
                    ncUnits.alert("分解目标失败");
                }
            });

        }
        else if (newOrDecomposition == 3) {
            if (Returndata.parentObjective == null) {
                delete Returndata.parentObjective;
            }

            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/UpdateObjective",
                dataType: "json",
                data: { flag: flag, operateFlag: operateFlag, data: JSON.stringify(Returndata) },
                success: rsHandler(function (data) {
                    if (data == 0) {
                        ncUnits.alert("操作成功");
                        $("#objectiveNew_modal").modal("hide");
                        statusCount();
                        loadObjectList();
                        drawPlanProgress();
                    }
                    else if (data == -1) {
                        ncUnits.alert("操作失败");
                        $("#objectiveNew_modal").modal("hide");
                    }
                    else {
                        ncUnits.alert("第" + data + "个目标公式设置有误!");
                    }
                })
            });
        }
        /*

         else if (newOrDecomposition == 4) {
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/UpdateObjective",
            dataType: "json",
            data: { flag: flag, data: JSON.stringify(Returndata) },
            success: rsHandler(function () {
                ncUnits.alert("操作成功");
                loadObjectList();
                statusCount();
                drawPlanProgress();
            })
        });
    }
        */

    }

}())
