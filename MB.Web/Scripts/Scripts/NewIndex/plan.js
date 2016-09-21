define(['layer', 'common'], function () {


    var planModule = function (searchData,mine) {
        var searchData = searchData;
        var downListFile;//附件列表
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
            calendarType: mine?2:1,            //日程类别： 1：我的日程 2：下属日程
            date: String,
            status: 1                   //计划状态：1：待审核计划  2：待确认计划（我的日程无该参数）
        };
        var batch_planId = [], batch_loopId = [];

        $(document).off('confirm', '.confirmFile')
        $(document).on('confirm', '.confirmFile', confirmAction)

        var events = {
            //'click .planDetail': detailAction,//计划详情
            //'click .suspend': stopAction,//终止计划
            //'click .modification': modiAction,//计划修改
            //'click .transimitPlan': transAction,//计划转办
            'click .decPlan': splitAction, //分解计划
            //'click .submit': submitAction, //提交计划
            //'click .audit': auditAction, //审核计划
            //'click .confirmPlan': confirmAction, //计划确认
            //'click .submitTo': submitToAction, //计划确认
            'click .revocation': revocationAction, //计划撤销
            'click .restart': restartAction, //重新开始
            //'click .del': delPlan, //计划删除
            'click .preview': preview//文件预览


        }
        var bindEvents = function () {
            var delegateEventSplitter = /^(\S+)\s*(.*)$/;
            var k, method, eventName, selector, match;
            var el = $('#main-context');
            for (k in events) {
                method = events[k];
                match = k.match(delegateEventSplitter);
                eventName = match[1];
                selector = match[2];
                method = events[k];
                el.off(eventName, selector);
                el.on(eventName, selector, method)

            }

        }()
        function editEnable() {
            $('.planDetailMask').css('display', 'none');
        }
        
        $(document).off('click', '.planSingle,.planMulti')
        $(document).on('click', '.planSingle,.planMulti', function () {
            if ($(this).hasClass('planSingle')) {
                var attachmentName = $(this).attr('attachmentName');
                var saveName = $(this).attr('savename');
                $.post("/UserIndex/Download", { displayName: attachmentName, saveName: saveName, flag: 0 }, function (data) {
                    if (data == "success") {

                        window.location.href = "/UserIndex/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1";
                    }
                    return;
                });

            } else {
                var planId = $(this).attr('pid')
                $.post("/UserIndex/MultiDownload", { planId: planId, flag: 0 }, function (data) {
                    if (data == "success") {
                        window.location.href = "/UserIndex/MultiDownload?planId=" + planId + "&flag=1";
                    }
                    return;
                });

            }

        })
        

        function preview() {
            var extension = $(this).siblings('.single-down').attr('extension');
            var savename = $(this).siblings('.single-down').attr('savename');
            $.ajax({
                type: "post",
                url: "/XXXViews/Shared/GetPreviewFile",
                dataType: "json",
                data: {
                    saveName: savename,
                    extension: extension
                },
                success: rsHandler(function (data) {
                    var oeTags = '<embed width="800" height="500" src=' + data + ' type="application/x-shockwave-flash"/>';
                    $.layer({
                        type: 1,
                        shade: [0.5, '#000'],
                        area: ['auto', 'auto'],
                        title: false,
                        border: [0],
                        page: {
                            html: oeTags
                        }
                    });
                })
            });
        }

        function delPlan() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx]
            if($(this).hasClass('delPlan')){
                var url = '/UserIndex/DeletePlan';
                console.log($(this).siblings('.list-audit-content').find('.checkSingle'))
                var param = { planId: parseInt(current_Info.planId,10)}
            }else{
                var url = '/plan/BatchDelete';
                var pid = [];
                $('input[name="checkSingle"]:checked').each(function (n,val) {
                    pid.push($(this).val());
                })
                var param = { planIdList: JSON.stringify(pid) }
            }
            ncUnits.confirm({
                title: "删除", html: "确定要删除计划吗?", yes: function (id) {
                    $.post(url, param, function (msg) {
                        if (url == '/UserIndex/DeletePlan') {
                            msg = JSON.parse(msg);
                        }
                        if (msg == "ok" || msg.data == true ) {
                            ncUnits.alert("删除成功！")
                            $('#confirmBtn').trigger('addPlan');
                            //view.updateList();
                        }
                        else {
                            ncUnits.alert("删除失败！");
                        }
                        layer.close(id);
                    });
                }
            });

        }

        function revocationAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            var status = current_Info.status;
            ncUnits.confirm({
                title: "撤销", html: "确定要撤销吗?", yes: function (id) {
                    canceloperate(current_Info.planId, status)
                    layer.close(id);
                }
            });
        }

        function restartAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            ncUnits.confirm({
                title: "重新开始", html: "确定要重新开始吗?", yes: function (id) {
                    $.post("/Plan/StopOrStartPlan", { "planId": current_Info.planId, "stop": 0 }, function (msg) {
                        if (msg == "ok") {
                            ncUnits.alert("重新开始成功！");
                            //view.updateList();
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
           
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            if (!current_Info.isLoop) {
                $('#xxc_planId').val(current_Info.planId);
            } else {
                $('#xxc_planId').val(current_Info.loopId);
            }

            $("#plan_detail_modal").modal("show");
            submitconfirm($("#plan_detail_modal"));
            

        }
        function getValues(selector) {

            var result = []
            $('input[name=' + selector + ']:checked').each(function (n, val) {
                result.push($(this).val())

            })
            return result;

        }

        function submitToAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            var values = getValues('checkSingle');
            ncUnits.confirm({
                title: "提交", html: "确定要提交吗?", yes: function (id) {
                    if (values.length > 0) {
                        var postData = { planIdList: JSON.stringify(values) };
                        submitoperate(postData, "/plan/BatchSubmitPlan");
                        return;
                    }
                    if (current_Info.isLoopPlan == 1) {   //循环计划
                        var postData = { "planId": current_Info.loopId, "initial": current_Info.initial };
                        //submitoperate(current_Info.loopId, current_Info.initial, current_Info.isLoopPlan);
                        submitoperate(postData, "/Plan/SubmitPlan");
                    } else {
                        var postData = { "planId": current_Info.planId, "initial": current_Info.initial };
                        submitoperate(postData, "/Plan/SubmitPlan");
                    }
                    layer.close(id);
                }, no: function (id) { layer.close(id) }
            });
        }

        function confirmAction(e,idx) {
            console.log('iqqaawa11x',idx)
            var idx = idx?idx:$(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            setValue()
            $("#plan_detail_modal").modal("show");
            DetailConfirm($("#plan_detail_modal"));


        }
        /* 批量审核 开始 */
        function fnCheckBox() {
            $('#detail_operateinfo').html('').attr('check',2);
            $("#bch_check").load("/plan/GetCheckView", { height: 240 }, function () {
                $(".checkBox .closeWCom").show();
                $('#bch_check').show();
                $('.checkBox').css({ 'margin-top': '200px', 'z-index': '9' });
                $("#xxc_checkpass").attr('flag', 2).attr('new','1');
                $('#xxc_checknopass').attr('flag', 2);
            });
            fnPopUpHeight($('.checksPopUp'));
        }


        function auditAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            var values = getValues('auditSingle');
            console.log(1111,values)
            if (values.length > 0) {
                fnCheckBox()

            } else {
                setValue()
                $('#detail_operateinfo').html('').attr('check', 1);
                $("#plan_detail_modal").modal("show");
                checkclick($("#plan_detail_modal"));
            }
         
        }



        function setValue() {
            $('#xxc_status').val(current_Info.status);
            $('#xxc_stop').val(current_Info.stop);
            console.log('ccc', current_Info)
            if (!current_Info.isLoop) {
                $('#xxc_planId').val(current_Info.planId);
            } else {
                $('#xxc_planId').val(current_Info.loopId);
            }
        }


        function stopAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx]
            var msg = {
                success: '申请终止成功',
                err: '申请终止失败',
                txt: '确认要申请终止吗？'
            }
            confirmControl("/Plan/StopOrStartPlan", { "planId": (current_Info.planId == null ? current_Info.loopId : current_Info.planId), "stop": 10 }, msg);

        }

        function modiAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            var msg = {
                success: '申请修改成功',
                err: '申请修改失败',
                txt: '确认要申请修改吗？'
            }
            confirmControl("/Plan/ChangePlanStatus", { "planId": (current_Info.planId == null ? current_Info.loopId : current_Info.planId), "status": 25, flag: 1 }, msg);
        }


        function confirmControl(url, updata, msg) {
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
                        data: updata,
                        success: rsHandler(function (data) {
                            if (data) {
                                ncUnits.alert(msg.success);
                                //loadingPlanList();
                                //view.updateList();
                                $('#confirmBtn').trigger('addPlan')
                            } else {
                                ncUnits.alert(msg.err);
                            }
                        })
                    })
                }
            })


        }


        function splitAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx]
            $("#planSplit_modal").attr('term', current_Info.planId);
            $("#planSplit_modal").modal("show");

        }




        function transAction() {
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            $("#layer_Transmitplan").attr("data_planId", (current_Info.planId == null ? vcurrent_Info.loopId : current_Info.planId)).attr("parentTime", current_Info.endTime);
            $("#layer_Transmitplan").modal("show");

        }



        function detailAction(e) {
            e.stopPropagation();
            //e.preventDefault()
            $('.operate-list').remove();
            var idx = $(this).parents('.li-check').attr('idx');
            current_Info = searchData[idx];
            console.log('current_Info',current_Info)
            if (!current_Info.isLoop) {
                $('#xxc_planId').val(current_Info.planId);
            } else {
                $('#xxc_planId').val(current_Info.loopId);
            }
            $("#plan_detail_modal").modal("show");
            editDisabled()


        }


        $("#plan_detail_modal").on("hide.bs.modal", function () {
            $(".listView").empty();
            $("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
            $("#detail_front_div").find('span:not(.detail_front_span)').remove();
            $("#detailAccessory").hide();
            $("#detail_operateinfo").html('').hide();
            $("#plan_detail_modal .modal-footer").hide();
            $('.downMain').remove();
            //$("#detail_looptype").empty();
            batch_flag = false;
            batch_planId = [];
            batch_loopId = [];
        });

        var planDetailFirst = true;
        $("#plan_detail_modal").off("shown.bs.modal");
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
                    console.log('这真他么的fuck')
                    /*获取计划详情 开始*/
                    var planId = current_Info.planId
                    var status = current_Info.status;
                    var stop = current_Info.stop?current_Info.stop:0;
                    var isloop = 0;
                    //var withfront = 0;
                    var withfront = current_Info.isFronPlan?current_Info.isFronPlan:0;
                    var initial = current_Info.initial? current_Info.initial:0;
                    //var collPlan = 0;
                    var collPlan = current_Info.IsCollPlan?current_Info.IsCollPlan:0;
                    $.ajax({
                        type: "post",
                        url: "/UserIndex/GetPlanInfoByPlanId",
                        dataType: "json",
                        data: { planId: planId, isloop: isloop, withfront: withfront, collPlan: collPlan },
                        success: rsHandler(function (data) {
                            downListFile = data;
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
                                $(document).off('click', '.accessory');
                                $(document).on('click', '.accessory', function () {
                                    if ($('.downMain').length > 0) {
                                        $('.downMain').remove();
                                        return;
                                    }
                                    var list = '';
                                    $.each(data.planAttachmentList, function (n, val) {
                                        list += '<li class="down-main ellipsis"><i class="fa fa-download single-down planSingle" attachmentName=' + val.attachmentName + ' saveName=' + val.saveName + ' extension=' + val.extension + '></i>' + val.attachmentName + '</li>';
                                    })
                                    var main = "<div class='downList downMain' style='right:-15px;'><div class='list-head'><a href='javascript:;' class='multi-down planMulti' pid=" + data.planId + ">全部下载</a></div><ul>" + list + "</ul></div>";
                                    $('#detailAccessory').append(main);
                                });
                             
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
                                $("#detail_span3").text('评论').attr('operatetype', '评论').css("width", "33.3%").show();
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
                                $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%").show();
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
                                $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%").show();
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
                                    ;
                                } else {
                                    $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '50%').show();
                                    $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '50%');
                                }
                               
                                break;
                            case (status == 30 && stop == 0) || (isloop == "1" && current_Info.status == 20): //待确认
                                $("#plan_detail_modal .modal-footer").show();
                                if (isloop == "1") {
                                    $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "100%");
                                    $("#detail_span2").hide();
                                } else {
                                    $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%").show();
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
                                    submitoperate({ "planId": planId, "initial": detail_initial }, "/Plan/SubmitPlan");
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
                                    //view.updateList();
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
            //获取计划详情
            function detail_window() {
                console.log('这真他么的日')
                /*获取计划详情 开始*/
                var planId = current_Info.planId
                var status = current_Info.status;
                var stop = current_Info.stop ? current_Info.stop : 0;
                var isloop = 0;
                //var withfront = 0;
                var withfront = current_Info.isFronPlan ? current_Info.isFronPlan : 0;
                var initial = current_Info.initial ? current_Info.initial : 0;
                //var collPlan = 0;
                var collPlan = current_Info.IsCollPlan ? current_Info.IsCollPlan : 0;
                $.ajax({
                    type: "post",
                    url: "/UserIndex/GetPlanInfoByPlanId",
                    dataType: "json",
                    data: { planId: planId, isloop: isloop, withfront: withfront, collPlan: collPlan },
                    success: rsHandler(function (data) {
                        downListFile = data;
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
                            $(document).off('click', '.accessory');
                            $(document).on('click', '.accessory', function () {
                                if ($('.downMain').length > 0) {
                                    $('.downMain').remove();
                                    return;
                                }
                                var list = '';
                                $.each(data.planAttachmentList, function (n, val) {
                                    list += '<li class="down-main ellipsis"><i class="fa fa-download single-down planSingle" attachmentName=' + val.attachmentName + ' saveName=' + val.saveName + ' extension=' + val.extension + '></i>' + val.attachmentName + '</li>';
                                })
                                var main = "<div class='downList downMain' style='right:-15px;'><div class='list-head'><a href='javascript:;' class='multi-down planMulti' pid=" + data.planId + ">全部下载</a></div><ul>" + list + "</ul></div>";
                                $('#detailAccessory').append(main);
                            });

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
                            $("#detail_span3").text('评论').attr('operatetype', '评论').css("width", "33.3%").show();
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
                            $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%").show();
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
                            $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%").show();
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
                                ;
                            } else {
                                $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '50%').show();
                                $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '50%');
                            }

                            break;
                        case (status == 30 && stop == 0) || (isloop == "1" && current_Info.status == 20): //待确认
                            $("#plan_detail_modal .modal-footer").show();
                            if (isloop == "1") {
                                $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "100%");
                                $("#detail_span2").hide();
                            } else {
                                $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%").show();
                                $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "50%");
                            }
                            $("#detail_span3").hide();
                            editDisabled();
                            break;


                    }

                }


            }
            detail_window();
        });


        //确认
        function DetailConfirm(obj) {
            console.log('啊啊啊啊啊啊啊啊')
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
        function submitoperate(postData,url) {
            $.ajax({
                url: url,
                type: "post",
                dataType: "json",
                data:postData,
                //data: { "planId": planIdNew, "initial": initial },
                success: rsHandler(function (data) {
                    $('#confirmBtn').trigger('addPlan')
                    if (data) {
                        ncUnits.alert("计划已成功提交！");
                        //loadingPlanList();
                        //view.updateList();
                       

                    } else{
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
        $("#xxc_makesure").off('click');
        $("#xxc_makesure").click(function () {
            var planIdNew = $("#layer_Transmitplan").attr("data_planId");
            var responsibleUser_t = $("#transmitplan_responsibleUser").attr("term");
            var confirmUser_t = $("#transmitplan_confirmUser").attr("term");
            if (!responsibleUser_t) {
                ncUnits.alert("请选择责任人");
           
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
                        //首页计划转办成功刷新列表
                        $('#xxc_makesure').trigger('addPlan')
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
                                //分解成功刷新首页计划列表
                                $('#planSplitSure').trigger('addPlan');
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



    }
    return planModule;



})