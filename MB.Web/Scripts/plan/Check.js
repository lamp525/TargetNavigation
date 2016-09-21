//$(function () {
    var planIdNew = $("#xxc_planId").val();
    var status = $("#xxc_status").val();
    var stop = $("#xxc_stop").val();
    $(".degreeDiv").find("ul li").removeClass("liHit");
    if ((status == '25' && stop == '0') || stop == '10') {
        $.ajax({
            url: "/plan/GetPlanCheckInfo",
            type: "post",
            dataType: "json",
            data: { planId: planIdNew },
            success: rsHandler(function (data) {
                var importance = parseInt(data.importance);
                var urgency = parseInt(data.urgency);
                var difficulty = parseInt(data.difficulty);
                for (var i = 0; i < importance; i++) {
                    $("#xxc_import").find("ul li:eq(" + i + ")").addClass('liHit');
                }
                for (var i = 0; i < urgency; i++) {
                    $("#xxc_urgency").find("ul li:eq(" + i + ")").addClass('liHit');
                }
                for (var i = 0; i < difficulty; i++) {
                    $("#xxc_difficulty").find("ul li:eq(" + i + ")").addClass('liHit');
                }
            })
        });
    }
    if (page_flag && page_flag == "CalendarProcess" && batch_flag && batch_flag == true) {   //日程化页面
        $("#xxc_checkpass").attr("flag", "3");
        $("#xxc_checknopass").attr("flag", "3");
    } else if (page_flag && page_flag == "plan") {     //计划页面
        layer.area(planDetail, { width: 962 });
    }
    
    //$('.checkBox').show().removeClass('checkMulti').addClass('checkSingle').css({ 'left': '560px', 'height': parseInt($('#checkheight').val()) });
    //var leftCheck = page_flag ? '560px' : '560px';
    var leftCheck = $('#detail_operateinfo').attr('check')==2? '350px' : '560px';
    var heightCheck = $('#detail_operateinfo').attr('check')==2?'450px':'100%'
    $('.checkBox').show().removeClass('checkMulti').addClass('checkSingle').css({ 'left': leftCheck, 'height': heightCheck });
    $('.view textarea').keydown(function () {
        var row = parseInt($(this).val().length / 26) + 1;
        $(this).height(26 * row);
        $(this).parent('.replyDiv').height('auto');
    });

    /* 评分五角星 开始 */
    $('.checkBox .stars ul li').hover(function () {
        var nums = $(this).index();
        var status = $("#xxc_status").val();
        var stop = $("#xxc_stop").val();
        if ((status == 25 && stop == 0) || stop == 10) {
            return;
        }
        var length = $(this).parent().children('li').length - 1;
        for (var i = 0; i <= length ; i++) {
            if (i <= nums) {
                $(this).parent().children('li').eq(i).addClass('liHover');
            }
            else {
                $(this).parent().children('li').eq(i).addClass('liHoverNot');
            }
        }
    }, function () {
        $(this).parent().children('li').removeClass('liHover').removeClass('liHoverNot');
    });
    $('.checkBox .stars ul li').click(function () {
        var nums = $(this).index();
        var status = $("#xxc_status").val();
        var stop = $("#xxc_stop").val();
        if ((status == 25 && stop == 0) || stop == 10) {
            return;
        }
        var length = $(this).parent().children('li').length - 1;
        for (var i = 0; i <= length ; i++) {
            if (i <= nums) {
                $(this).parent().children('li').eq(i).addClass('liHit');
            }
            else {
                $(this).parent().children('li').eq(i).removeClass('liHit');
            }
        }
    });
    /* 评分五角星 结束 */
    $(".checkBox .closeWCom").off('click');
    $(".checkBox .closeWCom").click(function () {
        $("#bch_check").hide().html('');
    });

    //自动调整按钮的高度
    $("#xxc_checkinfo").keydown(function () {
        var addHeight = parseInt($("#xxc_checkinfo").css("height")) - 26;
        $(".checkOperate").css("margin-top", 30 + addHeight);
    });

    //审核不通过
    $("#xxc_checknopass").off("click");
    $("#xxc_checknopass").click(function () {
        var checkinfo = $("#xxc_checkinfo").val();
        var flag = $(this).attr('flag');
        if (flag == '1') {
            $.ajax({
                type: "post",
                url: "/plan/ExaminePlan",
                dataType: "json",
                data: { planId: planIdNew, importance: 0, urgency: 0, difficulty: 0, isTrue: false, message: checkinfo },
                success: rsHandler(function (data) {
                    if (data == "ok") {
                        
                        if (page_flag && page_flag == "CalendarProcess") {   //日程化页面
                            $("#plan_detail_modal").modal("hide");
                            loadingPlanList();
                        } else if (page_flag == 'plan') {                            
                            //计划页面
                            layer.close(planDetail);
                            fnScreCon();
                        } else {
                            //首页计划转办成功刷新列表
                            $('#xxc_checknopass').trigger('addPlan');
                            $("#plan_detail_modal").modal("hide");
                        }
                        $("#detailAccessory").hide();
                        $("#detail_operateinfo").html('');
                        $("#xxc_planId").val('');
                        $("#detail_partner").html('');
                        $("#detail_premise").html('');
                        ncUnits.alert("审核成功！");

                    }
                    else {
                        ncUnits.alert("审核出错！");
                    }
                })
            });
        }
        else if (flag == '2') {
            if ($('#xxc_checkpass').attr('new')) {
                var planIds = planIds ? planIds : [];
                $('input[name="auditSingle"]:checked').each(function (n, val) {
                    planIds.push($(this).val())

                })
            }
            $.ajax({
                type: "post",
                url: "/plan/batchExaminePlan",
                dataType: "json",
                data: { planIdList: JSON.stringify(planIds), importance: 0, urgency: 0, difficulty: 0, isTrue: false, message: checkinfo },
                success: rsHandler(function (data) {
                    if (data == "ok") {
                        if (!page_flag) {
                            //首页计划转办成功刷新列表
                            $('#xxc_checknopass').trigger('addPlan');
                        }
                        $('.checkBox').hide();
                        $("#xxc_checkpass").attr('flag', '1');
                        planIds = [];
                        fnScreCon();
                        $("#detail_partner").html('');
                        $("#detail_premise").html('');
                        ncUnits.alert("审核成功！");
                        
                    }
                    else {
                        ncUnits.alert("批量审核出错！");
                    }
                })
            });
        }
        else if (flag=='3') {      //计划日程化批量操作
            var plancheckmodel = {
                planId: batch_planId,
                loopInfo:[],
                message: checkinfo,
                type:false
            }
            if (batch_loopId && batch_loopId.length>0) {
                $.each(batch_loopId, function (i,v) {
                    var loop = { loopId: v };
                    plancheckmodel.loopInfo.push(loop);
                });
            }
            $.ajax({
                type: "post",
                url: "/CalendarProcess/PlanBatchAudit",
                dataType: "json",
                data: { data: JSON.stringify(plancheckmodel) },
                success: rsHandler(function (data) {
                    if (data) {
                        $('.checkBox').hide();
                        $("#xxc_checkpass").attr('flag', '1');
                        $("#detail_partner").html('');
                        $("#detail_premise").html('');
                        $("#plan_detail_modal").modal("hide");
                        loadingPlanList();
                    }
                    else {
                        ncUnits.alert("批量审核出错！");
                    }
                })
            });
        }
    });

    //审核通过
    $("#xxc_checkpass").off("click");
    $("#xxc_checkpass").click(function () {
        // 通过的时候,非空验证
        if ($(this).index() == 0) {
            var boolR = false, boolY = false, boolG = false;
            $('.degreeDiv .degree').each(function (index, element) {
                var num = $(this).index();
                if ($('li', this).hasClass('liHit')) {
                    if (num == 0) {
                        boolR = true;
                    }
                    if (num == 1) {
                        boolY = true;
                    }
                    if (num == 2) {
                        boolG = true;
                    }
                }
            });
            if ((boolR == false) || (boolY == false) || (boolG == false)) {
                ncUnits.alert('有空白项');
                return;
            }
        }
        //} else {
        //    if ($(this).parents('.checkBox').hasClass('checkSingle')) {
        //        layer.close(popUpChecks);
        //    }
        //    if ($(this).parents('.checkBox').hasClass('checkMulti')) {
        //        layer.close(popUpCheckBox);
        //    }
        //}
        var flag = $(this).attr('flag');

        var imp = $("#xxc_import").find("ul .liHit").length;
        var urg = $("#xxc_urgency").find("ul .liHit").length;
        var dif = $("#xxc_difficulty").find("ul .liHit").length;
        var checkinfo = $("#xxc_checkinfo").val();
        if (flag == 1) {
            $.ajax({
                type: "post",
                url: "/plan/ExaminePlan",
                dataType: "json",
                data: { planId: planIdNew, importance: imp, urgency: urg, difficulty: dif, isTrue: true, message: checkinfo },
                success: rsHandler(function (data) {
                    
                    if (data == "ok") {
                      
                        if (page_flag && page_flag == "CalendarProcess") {   //日程化页面
                            $("#plan_detail_modal").modal("hide");
                            loadingPlanList();
                        } else if (page_flag == 'plan') {     //计划页面
                            layer.close(planDetail);
                            fnScreCon();
                        } else {
                            //首页计划转办成功刷新列表
                            $('#xxc_checkpass').trigger('addPlan');
                            $("#plan_detail_modal").modal("hide");
                           
                        }
                        $("#detailAccessory").hide();
                        $("#detail_operateinfo").html('');
                        $("#xxc_planId").val('');
                        ncUnits.alert("审核成功！");

                    }
                    else {
                        ncUnits.alert("审核出错！");
                    }
                })
            });
        } else if (flag == 2) {
            if (!planIds) {
              var planIds = [];
                $('input[name="auditSingle"]:checked').each(function (n, val) {
                    planIds.push($(this).val())

                })
            }
            $.ajax({
                type: "post",
                url: "/plan/batchExaminePlan",
                dataType: "json",
                data: { planIdList: JSON.stringify(planIds), importance: imp, urgency: urg, difficulty: dif, isTrue: true, message: checkinfo },
                success: rsHandler(function (data) {
                    if (data == "ok") {
                        if (!page_flag) {
                           
                            $('#xxc_checkpass').trigger('addPlan');
                        } else {
                            
                                                       
                             fnScreCon()
                        }
                        ncUnits.alert("批量审核成功！");
                        planIds = [];
                        $('.checkBox').hide();
                        $("#xxc_checkpass").attr('flag', '1');
                    } else {
                        $('.checkBox').hide();
                        ncUnits.alert("批量审核出错！");
                    }
                })
            });
        }
        else if (flag == '3') {      //计划日程化批量操作
            var plancheckmodel = {
                planId: batch_planId,
                loopInfo: [],
                importance: imp,
                urgency: urg,
                difficulty:dif,
                message: checkinfo,
                type: true
            }
            if (batch_loopId && batch_loopId.length > 0) {
                $.each(batch_loopId, function (i, v) {
                    var loop = { loopId: v };
                    plancheckmodel.loopInfo.push(loop);
                });
            }
            $.ajax({
                type: "post",
                url: "/CalendarProcess/PlanBatchAudit",
                dataType: "json",
                data: { data: JSON.stringify(plancheckmodel) },
                success: rsHandler(function (data) {
                    if (data) {
                        $('.checkBox').hide();
                        $("#xxc_checkpass").attr('flag', '1');
                        $("#plan_detail_modal").modal("hide");
                        loadingPlanList();
                    }
                    else {
                        ncUnits.alert("批量审核出错！");
                    }
                })
            });
        }

    });
//});