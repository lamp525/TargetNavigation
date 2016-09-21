//@ sourceURL=planList.js
var layerIndex_checks;
//详情框对象
var planDetail;
var planIdTemp;
var planDetailExemode;
//批量操作的标志
var cardoperate;

//协作人
var collUsers = [];

//前提计划
var frontplans = [];


var labels = [];
// 环形进度条
$(function () {
    $('.refresh').hide();
   // $('botNone').css('margin-top','20px');
    $('.conL .bottom').show();
    $('#num_currentPage').text($('#currentPageCount').val());
    $('#num_pageCount').text($('#pageCount').val());
    if (SCConData.whoPlan == "1")
    {
        $('.knob').each(function () {
            if ($(this).attr('data-readonly') == "false") {
                $(this).attr('data-readonly', 'true').addClass("xxc_konb");
            }
        });
    }
    else {
        $(".xxc_knob").attr('data-readonly', 'false').removeClass("xxc_konb");
    }
    
    $(".knob").each(function () {
        var text = $(this).data("text");
        $(this).knob({
            width:80,
            height:80,
            min: 0,
            thickness: .3,
            //fgColor: '#2B99E6',
            bgColor: '#e0e0e0',
            inputColor: '#888',
            format: function (v) {
                if (text) {
                    return text;
                } else {
                    return v + "%";
                }
            },
            release: function (v) {
                if (text) {
                    return false;
                }
                if (parseInt(v) > 90 || parseInt(v) < 0) {
                    v = 90;
                }
                var planIdNew = this.$.attr("term");
                $.ajax({
                    type: "post",
                    url: "/plan/UpdateProcess",
                    dataType:"json",
                    data: { planId: planIdNew, newProcess: v },
                    success: rsHandler(function (data) {
                        ncUnits.alert("已更新进度！");
                    })
                });
                //$.post("/plan/UpdateProcess", { planId: planIdNew, newProcess: v }, function () {
                //    ncUnits.alert("已更新进度！");
                //});
            }
        });
    });
    
    /* 选择卡片列表移上去出现绿条 开始 */
    $('.PLChunk').hover(function () {
        $('.operate', this).show();
    }, function () {
        $('.operate', this).hide();
    });
    function fnsusPlan() {
        $('.susPlan').show();
    }
    /* 选择卡片列表移上去出现绿条 结束 */

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
    $('checkBox .stars ul li').click(function () {
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
    
    /* 选择卡片列表 开始 */
    $('.conL .headR .arrowsBBBDiv ul li').off('click')
    $('.conL .headR .arrowsBBBDiv ul li').click(function () {
        cardoperate = $('a', this).html();
        cardListShow();
    });

   
    $('.headL .moreCancel').off('click');
    $('.headL .moreCancel').click(function () {
        cardoperate = "";
        $(this).hide();
        $('.moreBg').hide();
        $('.xxc_choose').removeClass('choose').removeClass('prohibit');
        $('.xxc_choose span').removeClass('spanHit');
        $('.chooseHit').removeClass('chooseHit');
        fnOperate();
    });

    $('.PLChunk .xxc_choose').click(function () {
        if ($(this).hasClass('chooseHit')) {
            $(this).removeClass('chooseHit');
            $('span', this).removeClass('spanHit');
        }
        else {
            $(this).addClass('chooseHit');
            $('span', this).addClass('spanHit');
        }
    });
    $(".moreBg").off('click');
    $(".moreBg").click(function () {
        fnOperate();
        
        var opera = $(this).attr('term');//1、导出 2、归类 3、提交  4、审批  5、删除 
        planIds = [];
        if ($('.chooseHit').length > 0) {
            $('.chooseHit').each(function () {
                planIds.push($(this).attr('planId'));
                var eventout = $(this).parents('.PLChunk').find('.runMode .runModeInfo li:eq(1)');
                var planInfoChoosed = { planId: $(this).attr('planId'), eventout: eventout };
                planInfoChooseds.push(planInfoChoosed);
            });
        }
        if (planIds.length<=0) {
            ncUnits.alert("请选择计划！");
            return;
        }
        if ($(this).html() == '归类') {
            planInfoChooseds = [];
            if ($('.chooseHit').length > 0) {
                $('.chooseHit').each(function () {
                    var eventout = $(this).parents('.PLChunk').find('.runMode .runModeInfo li:eq(1) span').text();
                    var planInfoChoosed = { planId: $(this).attr('planId'), eventoutput: eventout };
                    planInfoChooseds.push(planInfoChoosed);
                });
            }
            layerSortPlan();

        }
        if (opera == '1') {
            $.post("/plan/NpoiExcelByPlanId", { planIdList: JSON.stringify(planIds), flag: 0 }, function (data) {
                if (data == "success") {
                    //loadViewToMain("/plan/NpoiExcelByPlanId?flag=1");
                    window.location.href = "/plan/NpoiExcelByPlanId?flag=1";
                }
                planIds = [];
            });
        }
        if (opera == '3') {
            $.post("/plan/BatchSubmitPlan", { planIdList: JSON.stringify(planIds) }, function () {
                planIds = [];
                fnScreCon();
            });
        }
        if (opera == '4') {
            fnCheckBox();
        }
        if (opera == '5') {
            $.post("/plan/BatchDelete", { planIdList: JSON.stringify(planIds) }, function () {
                planIds = [];
                fnScreCon();
            });
        }
        cardoperate = "";
        $('.headL .moreCancel').hide();
        $('.moreBg').hide();
        $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
        $('.xxc_choose span').removeClass('spanHit');
    });




    /* 归类到计划 开始 */
    //layerSortPlan ();
    var popUpSortPlan;
    function layerSortPlan() {
        //alert();
        popUpSortPlan = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '#sortPlan' },
            move: ".drapdiv",
            closeBtn: false
        });
        $('#sortPlan .selected').css({ 'height': $('#sortPlan .selected').css('height') });
        $('#sortPlan .selected').css({ 'float': 'none' });
        // 加载已选计划
        $('#sortPlan .conditionDiv ul li').remove();
        for (var num in planInfoChooseds) {
            var length = $('#sortPlan .conditionDiv ul li').length;
            if (length == 0) {
                $('#sortPlan .conditionDiv ul').append(
					'<li style="margin-left:70px;"><span term="' +
					planInfoChooseds[num].planId +
					'">' +
					planInfoChooseds[num].eventoutput +
					'</span><span class="closeW"></span></li>');
            }
            else {
                $('#sortPlan .conditionDiv ul').append(
					'<li><span term="' +
					planInfoChooseds[num].planId +
					'">' +
					planInfoChooseds[num].eventoutput +
					'</span><span class="closeW"></span></li>');
            }
            fnSCLIHover();
            fnSCloseW();
        }
    }
    $("#sortPlan .closeWCom").click(function () {
        clearSortPlan();
        layer.close(popUpSortPlan);
    });
    $("#sortPlan .canCon span:eq(0)").click(function () {
        clearSortPlan();
        layer.close(popUpSortPlan);
    });
    function clearSortPlan() {
        $("#sortPlan .childPlanDiv").parent().find('.childPlanDiv').hide();
        $('#sortPlan .output').empty();
        $('#sortPlan .dutyPer').empty();
        $('#sortPlan .finTime').empty();
        $("#STInput").val('');
        planInfoChooseds = [];
    }
    function fnSCLIHover() {
        $("#sortPlan .conditionDiv li").hover(function () {
            $('span:eq(0)', this).css({ 'color': '#58b456' });
            $('.closeW', this).css({ 'background': 'url(../../Images/common/closeWHit.png) no-repeat' });
        }, function () {
            $('span:eq(0)', this).css({ 'color': '#686868' });
            $('.closeW', this).css({ 'background': 'url(../../Images/common/closeW.png) no-repeat' });
        });
    }
    $("#sortPlan .search").click(function () {
        $('#sortPlan .output').empty();
        $('#sortPlan .dutyPer').empty();
        $('#sortPlan .finTime').empty();
        $.ajax({
            type: "post",
            url: "/Plan/GetSelectPlan",
            dataType: "json",
            data: {
                //count:5
                key: $.trim($("#STInput").val())
            },
            success: rsHandler(function (data) {
                $("#sortPlan .childPlanDiv").parent().find('.childPlanDiv').show();
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        $.each(planInfoChooseds, function (e, chooseid) {
                            if (chooseid.planId == data[i].planId) {
                                data.splice(i, 1);
                            }
                        });
                    }
                }
                if (data.length > 0) {
                    var date = "";
                    var time = "";
                    for (var num in data) {
                        
                        if (data[num].endTime != null) {
                            date = data[num].endTime.toString().substring(0, data[num].endTime.toString().indexOf('T'));
                            time = data[num].endTime.toString().substring(data[num].endTime.toString().indexOf('T') + 1, data[num].endTime.toString().lastIndexOf(':'));
                        }
                        else {
                            date = "";
                            time = "";
                        }
                        $('#sortPlan .output').append(
                            '<li><div class="level">' +
                            '<input class="CPRadio" term="' + data[num].planId + '" name="CPRadio" type="radio" />' +
                            '<div class="levelDiv"><span class="levelBg"></span><ul class="levelList">' +
                            '<li>紧急：<span>' + data[num].urgency + '</span></li>' +
                            '<li>重要：<span>' + data[num].importance + '</span></li>' +
                            '</ul></div></div>' +
                            '<span class="outputText">' + data[num].eventOutput + '</span>' +
                            '<div class="CPDetail"><span class="CPDetailBg"></span>' +
                            '<div class="CPDetailText">' +
                            '<span>' + data[num].organizationName + ' </span>' +
                            '<span>' + data[num].projectName + ' </span>' +
                            '<span>' + data[num].progress + '</span></div></div></li>');
                        $('#sortPlan .dutyPer').append(
                            '<li><span>' + data[num].responsibleUserName + '</span></li>');
                        $('#sortPlan .finTime').append(
                            '<li><div><span>' + date+ '</span> <span>'+time+'</span></div></li>');
                    }
                    // 重要度、紧急度
                    fnLevel();
                    // 输出结果上移显示详情
                    fnSCOutputTextHover();
                }
                
            })
        });
       
    });
    $("#sortPlan .arrowsBBB").click(function () {
        if ($(this).parent().find('.childPlanDiv').css('display') != 'none') {
        } else {
            $(this).parent().find('.arrowsBBBDiv').hide();
        }
        $(this).parent().find('.childPlanDiv').toggle();
    });
    function fnSCloseW() {
        $('#sortPlan .closeW').unbind("click");
        $('#sortPlan .closeW').click(function () {
            for (var num in planInfoChooseds) {
                if (planInfoChooseds[num].planId == $(this).parent().find('span:eq(0)').attr('term')) {
                    //alert($(this).parent().find('span:eq(0)').attr('term'));
                    planInfoChooseds.splice(num,1);
                }
            }
            //console.log(planInfoChooseds);
            var num = $(this).parent().index();
            $(this).parent().remove();
            if (num == 0) {
                $('#sortPlan .conditionDiv ul li:eq(0)').css({ 'margin-left': '70px' });
            }
            // var length = $('.SCShortcut').length;
            // for ( var i=0;i<length;i++ ) {
            // 	if ( $('.perSecMain li:eq('+i+')').attr('term')==$(this).parent().find('span:eq(0)').attr('term') ) {
            // 		$('.perSecMain li:eq('+i+')').css({'color':'#686868'}).removeClass('chosen');
            // 		break;
            // 	}
            // }
        });
    }
    $('#sortPlan #STInput').click(function () {
        if ($(this).val() == '事项输出结果') {
            //$(this).val('');
        }
    });
    //$('#sortPlan #STInput').change(function () {
       
    //});
    // 输出结果上移显示详情
    function fnSCOutputTextHover() {
        $('.childPlanDiv .outputText').hover(function () {
            $(this).next('.CPDetail').show();
            var width = parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(0)').css('width')) +
                parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(1)').css('width')) +
                parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(2)').css('width')) +
                10;
            $(this).next('.CPDetail').css({ 'width': width });
            $(this).next('.CPDetail').find('.CPDetailBg').css({ 'width': width });
        }, function () {
            $(this).next('.CPDetail').hide();
        });
    }
    //function fnSCRadioC() {
    $('#sortPlan .canCon span:eq(1)').click(function () {
        var planid = [], parentid = '';
        //planid
        for (var num in planInfoChooseds) {
            planid.push(parseInt(planInfoChooseds[num].planId));
        }
        $('#sortPlan .output .CPRadio').each(function () {
            if ($(this).prop("checked")) {
                parentid = $(this).attr('term');
                //alert();
                //$(thisthis).parent().find('input[type="checkbox"]').prop("checked", true).a
            }// else {
            //    console.log('1111');
            //}
        });
        if (parentid=='') {
            ncUnits.alert("请选择目标计划！");
            return;
        }
        //parentid = parseInt($('#sortPlan .output .CPRadio').attr('term'));
        $.ajax({
            type: "post",
            url: "/Plan/PlantoParentPlan",
            dataType: "json",
            data: {
                //count:5
                planid:JSON.stringify(planInfoChooseds),
                ParentId: parentid
            },
            success: rsHandler(function (data) {
                ncUnits.alert("归类成功！");
                clearSortPlan();
                layer.close(popUpSortPlan);
                fnScreCon();
            })
        });
    });
    //}
    /* 归类到计划 结束 */



    /* 选择卡片列表移上去出现绿条 开始 */
    fnOperate();
    function fnOperate() {
        $('.PLChunk').hover(function () {
            $('.operate', this).show();
        }, function () {
            $('.operate', this).hide();
        });
        function fnsusPlan() {
            $('.susPlan').show();
        }
    }
    /* 选择卡片列表移上去出现绿条 结束 */
    
    /* 批量审核 开始 */
    function fnCheckBox() {
        $('#detail_operateinfo').html('');
        $("#bch_check").load("/plan/GetCheckView", { height: 240 }, function () {
            $(".checkBox .closeWCom").show();
            $('#bch_check').show();
            $('.checkBox').css({ 'margin-top': '200px','z-index':'9' });
            $("#xxc_checkpass").attr('flag', 2);
            $('#xxc_checknopass').attr('flag', 2);
            });
        fnPopUpHeight($('.checksPopUp'));
    }
    /* 批量审核 结束 */
    /* 选择卡片列表 结束 */

    /*转办计划 开始*/
    transimitPlan_responsibleUser_v = undefined;
    transimitPlan_confirmUser_v = undefined;
    layerIndex_transimitPlan = undefined;
    $('.xxc_transimitPlan').click(function () {
        $("#xxc_planId").val($(this).attr('term'));
        layerIndex_transimitPlan = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            //title: ['新建计划','background:#58b456;color:#fff;'],
            title: false,
            border: [0],
            page: { dom: '.layer_Transmitplan' },
            move: ".drapdiv",
            closeBtn: false
        });
        transimitPlan_responsibleUser_v = 0;
        transimitPlan_confirmUser_v = 0
    });

    //确认责任人
    $("#transmitplan_responsibleUser").searchStaffPopup({
        url: "/Plan/GetOfferUsers",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            transimitPlan_responsibleUser_v = data.id;
            responsibleUser_name = data.name;
            $(this).val(responsibleUser_name);
        }
    });
    //确认人选择
    $("#transmitplan_confirmUser").searchStaffPopup({
        url: "/Plan/GetOfferUsers",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            transimitPlan_confirmUser_v = data.id;
            confirmUser_name = data.name;
            $(this).val(confirmUser_name);
        }
    });
    //取消
    $("#addplan_cancel").off("click");
    $("#addplan_cancel").click(function () {
        //$(".xubox_main").css("display", "none");
        layer.close(layerIndex_transimitPlan);
        $("#transmitplan_responsibleUser").val('');
        $("#transmitplan_confirmUser").val('');
        $("#xxc_planId").val('');
        $('.optionCell-select').remove();
    });
    $('.layer_Transmitplan .closeWCom').click(function () {
        layer.close(layerIndex_transimitPlan);
        $("#transmitplan_responsibleUser").val('');
        $("#transmitplan_confirmUser").val('');
        $("#xxc_planId").val('');
        $('.optionCell-select').remove();
    });
    //确定
    $("#xxc_makesure").off("click");
    $("#xxc_makesure").click(function () {
        var planIdNew = $("#xxc_planId").val();
        var responsibleUser_t = $("#transmitplan_responsibleUser").val();
        var confirmUser_t = $("#transmitplan_confirmUser").val();
        if (responsibleUser_t == "") {
            ncUnits.alert("请选择责任人");
            return;
        }
        else if (confirmUser_t == "") {
            ncUnits.alert("请选择确认人");
            return;
        }
        else if (responsibleUser_t == confirmUser_t) {
            ncUnits.alert("责任人和确认人不能相同");
            return;
        }
        $.ajax({
            type: "post",
            url: "/Plan/ChangeToDo",
            dataType: "json",
            data: { planId: parseInt(planIdNew), responseUser: parseInt(transimitPlan_responsibleUser_v), confirmUser: parseInt(transimitPlan_confirmUser_v) },
            success: rsHandler(function (data) {
                if (data) {
                    layer.close(layerIndex_transimitPlan);
                    $("#xxc_planId").val('');
                    $("#transmitplan_responsibleUser").val('');
                    $("#transmitplan_confirmUser").val('');
                    fnScreCon();
                    $('.optionCell-select').remove();
                    ncUnits.alert("计划转办成功！");
                }
                else {
                    ncUnits.alert("计划转办失败！");
                }
            })
        });
    });

    /*转办计划 结束*/

    /* 审核 开始 */
    //审核窗口的对象

    $('.checks').click(function () {
        //$("#xxc_planId").val($(this).attr('term'));
        //$("#xxc_status").val($(this).attr('status'));
        //$("#xxc_stop").val($(this).attr('stop'));
        //layerIndex_checks = $.layer({
        //    type: 1,
        //    shade: [0],
        //    area: ['auto', 'auto'],
        //    title: false,
        //    border: [0],
        //    page: { dom: '.checksPopUp' }
        //});
        //fnPopUpHeight($('.checksPopUp'));
        //$('.checkBox').hide();
        $('#bch_check').html('');
        $("#detail_operateinfo").html('');
        detail_window($(this));
        checkclick($('#layer_details'));
    });
    $('.checkBtn').off("click");
    $('.checkBtn').click(function () {
        checkclick($('#layer_details'));
    });

    function checkclick(obj) {
        $("#detail_operateinfo").load("/plan/GetCheckView", { height: parseInt(obj.css('height'))},function(){
            $('#detail_operateinfo').show();
        } );

    }

    /* 审核 结束 */

    //修改操作
    $(".operateText .xxc_update").click(function () {
        var planIdNew = $(this).attr('term');
        ncUnits.confirm({
            title: "修改", html: "确定要申请修改吗?", yes: function (id) {
                $.ajax({
                    type: "post",
                    url: "/Plan/ChangePlanStatus",
                    dataType: "json",
                    data: { "planId": planIdNew, "status": 25, flag: 1 },
                    success: rsHandler(function (data) {
                        if (data == "ok") {
                            ncUnits.alert("申请修改成功！");
                            fnScreCon();
                        }
                        else {
                            ncUnits.alert("申请修改失败！");
                        }
                        layer.close(id);
                    })
                });
            }
        });

    });

    //撤销操作
    $(".operateText .xxc_revoke").click(function () {
        var planIdNew = $(this).attr('term');
        var status = $(this).attr('operate');
        ncUnits.confirm({
            title: "撤销", html: "确定要撤销吗?", yes: function (id) {
                canceloperate(planIdNew, status)
                layer.close(id);
            }
        });

    });

    function canceloperate(planId,status)
    {
        $.ajax({
            type: "post",
            url: "/Plan/ChangePlanStatus",
            dataType: "json",
            data: { "planId": planId, "status": status, flag: 0 },
            success: rsHandler(function (data) {
                if (data == "ok") {
                    ncUnits.alert("撤销成功！");
                    fnScreCon();
                }
                else {
                    ncUnits.alert("撤销失败！");
                }
            })
        });
    }

    //中止操作
    $(".operateText .suspend").click(function () {
        var planIdNew = $(this).attr('term');
        ncUnits.confirm({
            title: "中止", html: "确定要中止吗?", yes: function (id) {
                $.ajax({
                    type: "post",
                    url: "/Plan/StopOrStartPlan",
                    dataType: "json",
                    data: { "planId": planIdNew, "stop": 10 },
                    success: rsHandler(function (data) {
                        if (data == "ok") {
                            ncUnits.alert("申请中止成功！");
                            fnScreCon();
                        }
                        else {
                            ncUnits.alert("申请中止失败！");
                        }
                        layer.close(id);
                    })
                });
            }
        });
    });

    //重新开始操作
    $(".operateText .restart").click(function () {
        var planIdNew = $(this).attr('term');
        ncUnits.confirm({
            title: "重新开始", html: "确定要重新开始吗?", yes: function (id) {
                $.post("/Plan/StopOrStartPlan", { "planId": planIdNew, "stop": 0 }, function (msg) {
                    if (msg == "ok") {
                        ncUnits.alert("重新开始成功！");
                        fnScreCon();
                    }
                    else {
                        ncUnits.alert("重新开始失败！");
                    }
                    layer.close(id);
                });
            }
        });
    });

    //提交操作
    $(".operateText .xxc_submit").click(function () {
        var planIdNew = $(this).attr('term');
        var initial = $(this).attr('initial');
        obj = $(this);
        ncUnits.confirm({
            title: "提交", html: "确定要提交吗?", yes: function (id) {
                if (checkinput(obj)) {
                    submitoperate(planIdNew, initial);
                }
                else {
                    ncUnits.alert('存在空白项无法提交');
                }
                
                layer.close(id);
            }, no: function (id) { layer.close(id) }
        });

    });

    //提交非空验证
    function checkinput(obj)
    {
        var organizationName = obj.parents('.PLChunk').find('.labelLast span:eq(0)').text();
        var projectName = obj.parents('.PLChunk').find('.labelLast span:eq(1)').text();
        var executionMode = obj.parents('.PLChunk').find('.runMode li:eq(0) span').text();
        var eventOutput = obj.parents('.PLChunk').find('.runMode li:eq(1) span').text();
        var endTimeNew = obj.parents('.PLChunk').find('.runMode li:eq(2) span').text();
        var confirmUserName = obj.parents('.PLChunk').find('.runMode li:eq(3) span').text();
        var responsibleUserName = obj.parents('.PLChunk').find('.dutyPerson span').text();
        if (organizationName == "" || projectName == "" || executionMode == "" || eventOutput == "" || endTimeNew == "" || confirmUserName == "" || responsibleUserName == "") {
            return false;
        }
        else {
            return true;
        }
    }

    //提交
    function submitoperate(planIdNew, initial) {
        $.ajax({
            url: "/Plan/SubmitPlan",
            type: "post",
            dataType: "json",
            data: { "planId": planIdNew, "initial": initial },
            success: rsHandler(function (data) {
                if (data) {
                    ncUnits.alert("计划已成功提交！");
                    fnScreCon();
                } else {
                    ncUnits.alert("计划提交失败！");
                }
            })
        });
    }
    //点击加号新建计划
    //$(".addPlan").off('click');
    //$(".addPlan").click(addPlan);

    /* 前提计划 开始 */
    $('.headR .more').hover(function () {
        $(this).find('.arrowsBBB').addClass('arrowsBBBHit');
        $(this).find('.arrowsBBBDiv').show();
    }, function () {
        $(this).find('.arrowsBBB').removeClass('arrowsBBBHit');
        $(this).find('.arrowsBBBDiv').hide();
    });

    //$('.headR .more').removeAttr('hover');
    /* 前提计划 结束 */

    /* 前提计划 开始 */
    $('.premisePlan').off('click');
    $('.premisePlan').click(function () {
        var obj = $(this);
        var planIdNew = $(this).attr("term");
        $.ajax({
            type: "post",
            url: "/plan/GetFrontPlans",
            dataType:"json",
            data: { planId: planIdNew },
            success: rsHandler(function (data) {
                if (data != null) {
                    $(obj).parents('.plLabel').find('.labelDiv ul ').html('');
                    for (var i = 0; i < data.length; i++) {
                        $(obj).parents('.plLabel').find('.labelDiv ul ').append("<li term='" + data[i].planId + "'><span class='content'>" + data[i].eventOutput + "</span><span class='schedule'>" + data[i].process + "</span>%</li>");
                        $(obj).parent().parent().find('.labelDivBg').css({ 'width': parseInt($(obj).parent().parent().find('.labelDiv').css('width')), 'height': parseInt($(obj).parent().parent().find('.labelDiv').css('height')) });
                    }
                }
            })
        });
        //$.getJSON("/plan/GetFrontPlans", { planId: planIdNew }, function (data) {
        //    if (data != null) {
        //        $(obj).parents('.label').find('.labelDiv ul ').html('');
        //        for (var i = 0; i < data.length; i++) {
        //            $(obj).parents('.label').find('.labelDiv ul ').append("<li term='" + data[i].planId + "'><span class='content'>" + data[i].eventOutput + "</span><span class='schedule'>" + data[i].process + "</span>%</li>");
        //            $(obj).parent().parent().find('.labelDivBg').css({ 'width': parseInt($(obj).parent().parent().find('.labelDiv').css('width')), 'height': parseInt($(obj).parent().parent().find('.labelDiv').css('height')) });
        //        }
        //    }
        //});
        if ($(this).parent().parent().find('.labelDiv').css('display') == 'none') {
            $(this).parent().parent().find('.labelDiv').show();
        } else {
            $(this).parent().parent().find('.labelDiv').css('display', "none");
        }
    });
    /* 前提计划 结束 */



    /*附件开始*/
    var partternFile = /(ppt|xls|doc|pptx|xlsx|docx|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    $(".accessory").off('click');
    $("#xxc_planList .accessory").click(function () {
        $(this).parents('.plLabel').find('.labelDiv ul ').html('');
        var obj = $(this);
        var planIdNew = $(this).attr("term");
        $.ajax({
            type: "post",
            url: "/plan/GetPlanAttachmentList",
            dataType: "json",
            data: { planId: planIdNew },
            success: rsHandler(function (data) {
                if (data != "") {
                    var $ld = $(obj).parents('.plLabel').find('.labelDiv ul ');
                    $(obj).parents('.plLabel').find('.labelDiv ul ').html('');
                    $(obj).parents('.plLabel').find('.labelDiv ul ').append("<li class='allDownload'>全部下载</li>");
                    for (var i = 0; i < data.length; i++) {
                        $(obj).parents('.plLabel').find('.labelDiv ul ').append("<li savename='" + data[i].saveName + "' extension='" + data[i].extension + "' term='" + data[i].attachmentId + "'>" + (partternFile.test(data[i].attachmentName) ? "<span class='glyphicon glyphicon-eye-open accessoryOpe preview'></span>" : "") + "<span class='glyphicon glyphicon-download-alt accessoryOpe downfile'></span><span class='content'>" + data[i].attachmentName + "</span></li>");
                    }
                    $(obj).parents('.plLabel').find('.labelDiv li').addClass('liAccessory');
                    $(obj).parents('.plLabel').find('.labelDiv li.allDownload').removeClass('liAccessory');
                    $(obj).parents('.plLabel').find('.labelDiv .schedule').hide();
                    $('.downfile,.content', $ld).off('click');
                    $('.downfile,.content', $ld).click(function () {
                        downloadfile($(this).parent());
                    });
                    $('.preview', $ld).off('click');
                    $('.preview', $ld).click(function () {
                        preview(3, $(this).parent().attr("savename"), $(this).parent().attr("extension"));
                    });

                    $('.allDownload', $ld).off('click');
                    $('.allDownload', $ld).click(function () {
                        downloadaddfile(planIdNew);
                    });
                }
                else {
                    $(obj).parents('.plLabel').find('.labelDiv ul ').append("<li ><span class='content'>无附件</span></li>");
                }
                $(obj).parent().parent().find('.labelDivBg').css({ 'width': parseInt($(obj).parent().parent().find('.labelDiv').css('width')), 'height': parseInt($(obj).parent().parent().find('.labelDiv').css('height')) });
                $(obj).parents('.plLabel').find('.labelDiv .allDownload').show();
            })
        });
        if ($(this).parent().parent().find('.labelDiv').css('display') == 'none') {
            $(this).parent().parent().find('.labelDiv').show();
        } else {
            $(this).parent().parent().find('.labelDiv').css('display', "none");
        }
    });

    //单个文件下载
    function downloadfile(obj)
    {
        var attachmentName = obj.find('span').text();
        var saveName = obj.attr('savename');
        $.post("/plan/Download", { displayName: attachmentName, saveName: saveName,flag:0 }, function (data) {
            if (data == "success") {
                //loadViewToMain("/plan/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1");
                window.location.href = "/plan/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1";
            }
            return;
        });
        
    }

    //全部下载
    function downloadaddfile(planId)
    {
        $.post("/plan/MultiDownload", { planId:planId,flag:0 }, function (data) {
            if (data == "success") {
                //loadViewToMain("/plan/MultiDownload?planId=" + planId + "&flag=1");
                window.location.href = "/plan/MultiDownload?planId=" + planId+"&flag=1";
            }
            return;
        });
    }
    

    ///* 附件 开始 */
    //$('.accessory').click(function () {
    //    $(this).parents('.label').find('.labelDiv .allDownload').show();
    //    $(this).parents('.label').find('.labelDiv li').addClass('liAccessory');
    //    $(this).parents('.label').find('.labelDiv li.allDownload').removeClass('liAccessory');
    //    $(this).parents('.label').find('.labelDiv .schedule').hide();
    //    fnLabelDiv(this);
    //});
    ///* 附件 结束 */


    /* 子计划 开始 */
    $('.childrenPlan').click(function () {
        var planIdNew = $(this).attr("term");
        var date = "";
        var time = "";
        var obj = $(this).parents(".plLabel").find(".childPlanDiv");
        $.ajax({
            type: "post",
            url: "/plan/GetChildPlanList",
            dataType:"json",
            data: { planId: planIdNew },
            success: rsHandler(function (data) {
                if (data != "") {
                    $(obj).html('');
                    $(obj).append("<ul class='output'><li class='CPHead'>输出结果</li></ul><ul class='dutyPer'><li class='CPHead'>责任人</li></ul><ul class='finTime'><li class='CPHead'>完成时间</li></ul>");
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].withSub) {
                            $(obj).find(".output").append("<li><span num='" + ($(obj).length + i) + "' term='" + data[i].childPlanId + "' class='addCircle'></span></li>");
                        }
                        else {
                            $(obj).find(".output").append("<li><span term='" + data[i].childPlanId + "' class='cutCircle'></span></li>");
                        }

                        if (data[i].endTime != null) {
                            date = data[i].endTime.toString().substring(0, data[i].endTime.toString().indexOf('T'));
                            time = data[i].endTime.toString().substring(data[i].endTime.toString().indexOf('T') + 1, data[i].endTime.toString().lastIndexOf(':'));
                        }
                        else {
                            date = "";
                            time = "";
                        }
                        var childHtml;
                        if (data[i].urgency == 5 || data[i].importance == 5) {
                            childHtml = "<div class='level' style='display:block'><div class='levelDiv'>"
                            + "<span class='levelBg'></span><ul class='levelList'><li>紧急度：<span>" + data[i].urgency + "</span></li><li>重要度：<span>" + data[i].importance + "</span></li>"
                            + "</ul></div> </div><span class='outputText'>" + data[i].eventOutput + "</span><div class='CPDetail'><span class='CPDetailBg'></span>"
                                         + "<div class='CPDetailText'><span>部门分类:" + data[i].organizationName + "</span>  <span>项目分类:" + data[i].projectName + "</span>  <span>进程:" + data[i].process + "</span>"
                                            + "</div></div>";
                        }
                        else {
                            childHtml = "<div class='level' style='display:none'><div class='levelDiv'>"
                            + "<span class='levelBg' ></span><ul class='levelList'><li>紧急度：<span>" + data[i].urgency + "</span></li><li>重要度：<span>" + data[i].importance + "</span></li>"
                            + "</ul></div> </div><span class='outputText'>" + data[i].eventOutput + "</span><div class='CPDetail'><span class='CPDetailBg'></span>"
                                         + "<div class='CPDetailText'><span>部门分类:" + data[i].organizationName + "</span>  <span>项目分类:" + data[i].projectName + "</span>  <span>进程:" + data[i].process + "</span>"
                                            + "</div></div>";
                        }

                        $(obj).find(".output").children("li:eq(" + (i + 1) + ")").append(childHtml);
                        $(obj).find(".dutyPer").append("<li><span>" + data[i].responsibleUserName + "</span></li>");
                        $(obj).find(".finTime").append("<li><div><span>" + date + "</span> <span>" + time + "</span></div></li>");

                    }

                    $('.childPlanDiv .level').off("hover");
                    $('.childPlanDiv .level').hover(leveHover1, leveHover2);
                    $('.childPlanDiv .outputText').off("hover");
                    $('.childPlanDiv .outputText').hover(outTextOver, outTextLeave);
                    $(obj).find(".addCircle").off("click");
                    $(obj).find(".addCircle").click(addCircleclickEvent);
                    $(obj).find(".cutCircle").off("click");
                    $(obj).find(".cutCircle").click(cutCircleClickEvent);
                    if ($(obj).css('display') == 'none') {
                        $(obj).show();
                        $('.outputText').each(function (index, element) {
                            var width = parseInt($(this).parent().parent().css('width')) - 24 - 14 - 1;
                            $(this).css({ 'width': width });
                        });
                    } else {
                        $(obj).hide();
                        $(obj).find('ul ul').hide();
                    }
                }
            })
            });

    });

    var leveHover1 = function () {
        $('.levelDiv', this).show();
        $('.levelDiv .levelList', this).show();
    }
    var leveHover2 = function () {
        $('.levelDiv', this).hide();
    }

    var outTextOver = function () {
        $(this).next('.CPDetail').show();
        var width = parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(0)').css('width')) +
            parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(1)').css('width')) +
            parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(2)').css('width')) +
            30;
        //console.log(parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(0)').css('width')));
        //console.log(parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(1)').css('width')));
        //console.log(parseInt($(this).next('.CPDetail').find('.CPDetailText span:eq(2)').css('width')));
        $(this).next('.CPDetail').css({ 'width': width });
        $(this).next('.CPDetail').find('.CPDetailBg').css({ 'width': width });
    }
    var outTextLeave = function () {
        $(this).next('.CPDetail').hide();
    }
    var addCircleclickEvent = function () {

        var planIdNew = $(this).attr("term");
        var num = parseInt($(this).attr("num"));
        var date = "";
        var time = "";
        var obj = $(this);
        $.ajax({
            type: "post",
            url: "/plan/GetChildPlanList",
            dataType: "json",
            data: { planId: planIdNew },
            success: rsHandler(function (data) {
                if (data != "") {
                    $('.AC').removeClass('AC');
                    $('.addCircle').removeClass('AC').addClass("BC");
                    obj.addClass('AC').removeClass('addCircle').addClass('cutCircle');
                    $('.cutCircle').addClass("BC");
                    var num1 = -1, sign = -1, signLi = -1;
                    $('.BC').each(function (index, element) {
                        num1++;
                        if ($(this).hasClass('AC')) {
                            sign = num1;
                            signLi = sign + 1;
                        }
                    });
                    for (var i = 0; i < data.length; i++) {
                        signLi = signLi + i;
                        if (data[i].withSub) {
                            $(obj).parent().append("<ul><li><span num='" + i + "' term='" + data[i].childPlanId + "' class='addCircle'></span></li></ul>");
                        } else {
                            $(obj).parent().append("<ul><li><span num='" + i + "' term='" + data[i].childPlanId + "' class='cutCircle'></span></li></ul>");
                            $(obj).addClass("cutCircle").removeClass("addCircle");
                        }
                        if (data[i].endTime != null) {
                            date = data[i].endTime.toString().substring(0, data[i].endTime.toString().indexOf('T'));
                            time = data[i].endTime.toString().substring(data[i].endTime.toString().indexOf('T') + 1, data[i].endTime.toString().lastIndexOf(':'));
                        }
                        else {
                            date = "";
                            time = "";
                        }
                        var childHtml;
                        if (data[i].urgency == 5 || data[i].importance == 5) {
                            childHtml = "<div class='level' style='display:block'><div class='levelDiv'><span class='levelBg'></span>"
                                                   + "<ul class='levelList'><li>紧急度：<span>" + data[i].urgency + "</span></li><li>重要度：<span>" + data[i].importance + "</span></li></ul></div>"
                                       + "</div><span class='outputText'>" + data[i].eventOutput + "</span> <div class='CPDetail'><span class='CPDetailBg'></span>"
                                        + "<div class='CPDetailText'><span>部门分类:" + data[i].organizationName + "</span>  <span>项目分类:" + data[i].projectName + "</span>  <span>进度:" + data[i].process + "</span>"
                                           + "</div></div>";
                        }
                        else {
                            childHtml = "<div class='level' style='display:none'><div class='levelDiv'><span class='levelBg'></span>"
                                                   + "<ul class='levelList'><li>紧急度：<span>" + data[i].urgency + "</span></li><li>重要度：<span>" + data[i].importance + "</span></li></ul></div>"
                                       + "</div><span class='outputText'>" + data[i].eventOutput + "</span> <div class='CPDetail'><span class='CPDetailBg'></span>"
                                        + "<div class='CPDetailText'><span>部门分类:" + data[i].organizationName + "</span>  <span>项目分类:" + data[i].projectName + "</span>  <span>进度:" + data[i].process + "</span>"
                                           + "</div></div>";
                        }
                        $(obj).parent().find("ul:eq(" + (i + 1) + ") li").append(childHtml);
                        //alert(signLi)
                        //alert($(obj).parents(".childPlanDiv").find(".dutyPer li:eq(" + signLi + ")").html())
                        $(obj).parents(".childPlanDiv").find(".dutyPer li:eq(" + signLi + ")").append("<ul><li><span>" + data[i].responsibleUserName + "</span></li></ul>");
                        $(obj).parents(".childPlanDiv").find(".finTime li:eq(" + signLi + ")").append(" <ul><li><div><span>" + date + "</span> <span>" + time + "</span></div></li></ul>");

                    }

                    $('.childPlanDiv .level').off("hover");
                    $('.childPlanDiv .level').hover(leveHover1, leveHover2);
                    $('.childPlanDiv .outputText').off("hover");
                    $('.childPlanDiv .outputText').hover(outTextOver, outTextLeave);
                    $(obj).parent().find("ul:not(':first') .addCircle").off("click");
                    $(obj).parent().find("ul:not(':first') .addCircle").click(addCircleclickEvent);
                    //$('.addCircle').removeClass('AC');
                    $(obj).addClass('AC').removeClass("addCircle").addClass("cutCircle");
                    //$(obj).next().show();
                    $(obj).siblings('ul').show();
                    $(obj).siblings('ul').find('.outputText').each(function (index, element) {
                        var width = parseInt($(this).parent().parent().css('width')) - 24 - 14 - 1;
                        $(this).css({ 'width': width });
                    });
                    $(obj).parents('.childPlanDiv').find('.dutyPer ul:eq(' + sign + ')').show();
                    $(obj).parents('.childPlanDiv').find('.dutyPer ul:eq(' + sign + ')').siblings('ul').show();
                    $(obj).parents('.childPlanDiv').find('.finTime ul:eq(' + sign + ')').show();
                    $(obj).parents('.childPlanDiv').find('.finTime ul:eq(' + sign + ')').siblings('ul').show();
                    $(obj).off("click");
                    $(obj).click(function () {
                        cutCircleClickEvent($(this), parseInt($(obj).attr("num")))
                    });
                }
            })
        });
    }

    var cutCircleClickEvent = function (obj, num1) {
        num1++;
        $('.CC').removeClass('CC');
        $('.cutCircle').removeClass('CC').addClass('BC');
        obj.addClass('CC').removeClass("cutCircle").addClass("addCircle");
        $('.addCircle').addClass('BC');
        obj.prev().show();
        var num = -1, sign = -1, sighLi = -1;
        $('.BC').each(function (index, element) {
            num++;
            if ($(this).hasClass('CC')) {
                sign = num;
                signLi = sign + 1;
            }
        });
        obj.siblings('ul').hide();
        obj.siblings('ul').find('.cutCircle').addClass("addCircle").removeClass("cutCircle");
        obj.siblings('ul').find('ul').hide();
        obj.parents('.childPlanDiv').find('.dutyPer li:eq(' + signLi + ')').find('span:eq(0)').siblings('ul').remove();
        //obj.parents('.childPlanDiv').find('.finTime li:eq(' + signLi + ')').remove();
        obj.parents('.childPlanDiv').find('.finTime li:eq(' + signLi + ')').find('div:eq(0)').siblings('ul').remove();
        //$('.CC').parent().find('ul').remove();
        obj.parent().find("ul:not(':first')").remove();
        //obj.parents(".childPlanDiv").find(".dutyPer li:eq(" + num1 + ")").remove();
        //obj.parents(".childPlanDiv").find(".finTime li:eq(" + num1 + ")").remove();

        obj.off("click");
        obj.click(addCircleclickEvent);

    }
    /* 子计划 结束 */

    /* 提交确认 开始 */
    $('.xxc_submitconfirm').off('click');
    $('.xxc_submitconfirm').on('click', function () {
        $("#detail_operateinfo").html('');
        detail_window($(this));
        submitconfirm($("#layer_details"));
    });
    function submitconfirm(obj){
        $("#detail_operateinfo").load("/plan/GetSubmitConfirmView", { height: parseInt(obj.css("height")) }, function () {
            $('#detail_operateinfo').show();
        });
    }
   
    /* 提交确认 结束 */


    /* 确认 开始 */
    $('.affirm').off('click');
    $('.affirm').on('click', function () {
        $("#detail_operateinfo").html('');
        detail_window($(this));
        DetailConfirm($("#layer_details"));
    });

    function DetailConfirm(obj) {
        $("#detail_operateinfo").load("/plan/GetConfirmView", { height: parseInt(obj.css("height"))},function(){
            $('#detail_operateinfo').show();
        });
    }
    $('.evaluateBtn').off('click');
    $(".evaluateBtn").click(function () {
        DetailConfirm($("#layer_details"));
    });
    /* 确认 结束 */

    /* 计划详情 开始 */
    

    //设置计划详情界面不可编辑
    function editDisabled() {
        $('.planDetailMask').css('display','block');
    }
    //设置计划详情界面可编辑
    function editEnable() {
        $('.planDetailMask').css('display', 'none');
    }

    $(".planDetail").click(function () {
        $('.xxc_runmode').html();
        $('#detail_operateinfo').hide();
        detail_window($(this));
    });
    
    function detail_window(obj) {
        $(".listView").html('');
        $("#xxc_planId").val(obj.attr('term'));
        /*获取计划详情 开始*/
        var planId = obj.attr('term');
        var status = obj.attr('status');
        var stop = obj.attr('stop');
        var isloop = obj.attr('isloop');
        var withfront = obj.attr('withfront');
        var collPlan = obj.attr("collPlan");
        $("#xxc_status").val(obj.attr('status'));
        $("#xxc_stop").val(obj.attr('stop'));
        $("#xxc_isloop").val(obj.attr('isloop'));
        $.ajax({
            type: "post",
            url:"/plan/GetPlanInfoByPlanId",
            dataType:"json",
            data: { planId: planId, isloop: isloop, withfront: withfront, collPlan: collPlan },
            success: rsHandler(function (data) {
                $("#details_department_v").text(data.organizationName).attr('term', data.organizationId);
                $("#details_project_v").text(data.projectName).attr('term', data.projectId);
                $("#detail_runmode option").each(function () {
                    if (parseInt($(this).val()) == data.executionModeId) {
                        $(this).attr("selected", "true");
                    }
                });
                $("#detail_eventoutput").val(data.eventOutput);
                if (!data.isLoopPlan) {
                    $("#detail_looptype option:eq(1)").attr("selected", "true");
                    $("#xxx_startime").hide();
                    $("#xxx_initial").show();
                    $("#detail_initial option").each(function () {
                        if (parseInt($(this).val()) == data.initial) {
                            $(this).attr("selected", "true").attr("term", data.initial);
                        }
                    });
                }
                else {
                    $("#detail_looptype option").each(function () {
                        if (parseInt($(this).val()) == data.loopType) {
                            $(this).attr("selected", "true");
                        }
                    });
                    $("#xxx_startime").show();
                    $("#detail_startime").val(data.startTimeNew);
                    $("#xxx_initial").hide();
                }
                $("#detail_endtime").val(data.endTimeNew);
                $("#detail_responsibleUser").val(data.responsibleUserName).attr('term', data.responsibleUser);
                $("#detail_confirmUser").val(data.confirmUserName).attr('term', data.confirmUser);
                //$("#detail_initial option").each(function () {
                //    if (parseInt($(this).val()) == data.initial) {
                //        $(this).attr("selected", "true").attr("term", data.initial);
                //    }
                //});
                if (data.collPlanList.length>0) {
                    for (var i = 0; i < data.collPlanList.length; i++) {
                    
                        var $span = $("<span class='collplan' term='" + data.collPlanList[i].id + "'></span>").addClass("tag");
                        var $close = $("<span term='" + data.collPlanList[i].id + "'>X</span>").addClass("close").css({ display: "none" });
                        $span.hover(function () {
                            $(".close",this).toggle();
                        })

                        $span.append([data.collPlanList[i].name, $close]);

                        $("#detail_partner").parent().before($span);
                        collUsers.push({ id: data.collPlanList[i].id });
                        $close.click(function () {
                            var collplanId = $(this).attr('term');//-------------------------------------
                            $.ajax({
                                url:"/Plan/DeletePlanCooperation",
                                type: "post",
                                data: { planId: planId, userId: collplanId },
                                dataType: "JSON",
                                success: rsHandler(function (data) {

                                })
                            });
                            for (var i = 0; i < collUsers.length; i++) {
                                if (collUsers[i].id == collplanId) {
                                    collUsers.splice(i, 1);
                                }
                            }
                            $(this).parent("span").remove();
                            //console.log(collplanId);
                            //console.log(planId);

                            //$span.remove();
                        });
                    }
                }
                if (data.withFront) {
                    for (var i = 0; i < data.frontLists.length; i++) {
                        var $span = $("<span class='frontplan' term='" + data.frontLists[i].planId + "'></span>").addClass("tag");
                        var $close = $("<span term='" + data.frontLists[i].planId + "'>X</span>").addClass("close").css({ display: "none" });

                        $span.hover(function () {
                            $close.toggle();
                        });

                        $span.append([data.frontLists.eventOutput, $close]);
                        $("#detail_premise").parent().before($span);
                        frontplans.push({ planId: data.frontLists[i].planId });
                        $close.click(function () {
                            var frontplanId = $(this).attr('term');
                        
                            for (var i = 0; i < frontplans.length; i++) {
                                if (frontplans[i].planId == frontplanId) {
                                    frontplans.splice(i, 1);
                                }
                            }
                            $span.remove();
                            removeValue(premise_v, data.id);
                            removeValue(premise_text, data.name);
                        });
                    }
                }
                labels = [];
                $('#detail_label_div').find('.tag').remove();
                if (data.keyword) {
                    for (var i = 0; i < data.keyword.length; i++) {
                        var $span = $("<span class='tag'></span>");
                        var $close = $("<span term='" + data.keyword[i] + "'>X</span>").addClass("close").css({ display: "none" });

                 
                        $span.append([data.keyword[i], $close]);
                        $span.hover(function () {
                            //$close.toggle();
                            $(this).find('span').toggle();
                      
                        });

                        $("#detail_label").parent().before($span);
                        $close.click(function () {
                            $span.remove();
                        });

                        labels.push(data.keyword[i]);
                        $close.click(function () {
                            var label = $(this).attr('term');

                            for (var i = 0; i < labels.length; i++) {
                                if (labels[i] == label) {
                                    labels.splice(i, 1);
                                }
                            }
                            $span.remove();
                        });
                    }
                }

                //加载附件
                if (data.planAttachmentList.length <= 0) {
                    $("#detailAccessory").hide()
                }
                else {
                    $("#detailAccessory").show();
                    $("#detailAccessory .accessory").off("click");
                    $("#detailAccessory .accessory").click(function () {
                        if ($(this).next(".accessoryDiv").css("display")=="none") {
                            $(this).next(".accessoryDiv").show();
                        } else {
                            $(this).next(".accessoryDiv").hide();
                        }
                        $('#detailAccessory .accessoryDiv ul').html('');
                        $('#detailAccessory .accessoryDiv ul').append("<li class='allDownload'>全部下载</li>");
                        for (var i = 0; i < data.planAttachmentList.length; i++) {
                            $('#detailAccessory .accessoryDiv ul').append("<li class='liAccessory' savename='" + data.planAttachmentList[i].saveName + "' term='" + data.planAttachmentList[i].attachmentId + "'><span class='content'>" + data.planAttachmentList[i].attachmentName + "</span></li>");
                        }
                        $('.liAccessory').off('click');
                        $('.liAccessory').click(function () {
                            downloadfile($(this));
                        });

                        $('.allDownload').off('click');
                        $('.allDownload').click(function () {
                            downloadaddfile(planId);
                        });
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
                planId: planId,isloop:isloop
            },
            success: rsHandler(function (data) {
                var $ul = $("<ul></ul>");
                for (var i = 0, len = data.length; i < len; i++) {
                    var info = data[i];
                    var $li = $("<li></li>");
                    var $span1 = $("<span style='width: 50%;word-break: break-all;'></span>");
                    var $span2 = $("<span style='width: 50%;text-align: -webkit-right;float:right;'></span>");
                    $li.append([$span1, $span2]).appendTo($ul);

                    var str = "<span style='color:#58b456;'>" + info.user + "</span>";
                    if (info.type == 1) {
                        str += "提交了该计划";
                    } else if (info.type == 2) {
                        str += ("审核通过了该计划" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 3) {
                        str += ("审核未通过该计划" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 4) {
                        str += ("撤销了该计划的提交" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 5) {
                        str += ("撤销了该计划的审核" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 6) {
                        str += ("发表了评论" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 7) {
                        str += ("下载了附件" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 8) {
                        str += ("查看了你的计划" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 9) {
                        str += ("转办" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 10) {
                        str += ("申请修改该计划" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 11) {
                        str += ("申请中止该计划" + (info.message.length ? (":" + info.message) : ""));
                    } else if (info.type == 12) {
                        str += ("重新开始该计划" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 13) {
                        str += ("删除该计划" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 14) {
                        str += ("该计划确认通过" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 15) {
                        str += ("该计划确认未通过" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 16) {
                        str += ("该计划更新了进度" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 17) {
                        str += ("该计划进行了分解计划" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 18) {
                        str += ("新建了计划" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 19) {
                        str += ("新建了循环计划" + (info.message.length ? (":" + info.message) : ""));
                    }
                    else if (info.type == 20) {
                        str += ("修改保存了该计划" + (info.message.length ? (":" + info.message) : ""));
                    } else {
                        str = "异常操作"
                    }
                    $span1.html(str);
                    $span2.html(info.timeNew);
                }
                $("#layer_details .listView").html($ul);
            })
        });
        $("#detail_canCon").show();
        $("#detail_span1").show()
        $("#detail_span2").show();
        $("#detail_span3").show();
        if (collPlan == 1) {
            if (status==90||stop==90) {
                $("#detail_canCon").hide();
            }
            else {
                $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                $("#detail_span2").hide();
                $("#detail_span3").hide();
            }
            editDisabled();
        }
        else {
            if (SCConData.whoPlan == 0) {//责任人
                if ((status == 0 || status == 15) && stop == 0) {//待提交
                    $("#xxc_initial").val(obj.attr('initial'));
                    $("#detail_looptype").attr('disabled', 'disabled');
                    if (isloop == "1") {
                        $("#detail_span1").text('提交').attr('operatetype', '提交').css("width", "50%");
                        $("#detail_span2").text('保存').attr('operatetype', '保存').css("width", "50%");
                        $("#detail_span3").hide();
                    } else {
                        $("#detail_span1").text('提交').attr('operatetype', '提交').css("width", "33.3%");
                        $("#detail_span2").text('保存').attr('operatetype', '保存').css("width", "33.3%");
                        $("#detail_span3").text('评论').attr('operatetype', '评论').css("width", "33.3%");
                    }
                    editEnable();
                } else if (((status == 10 || status == 25) && stop == 0) || stop == 10) {//待审核
                    if (isloop == "1") {
                        $("#detail_span1").hide();
                    } else {
                        $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                    }
                    $("#detail_span2").hide();
                    $("#detail_span3").hide();
                    editDisabled();
                } else if ((status == 20 || status == 40) && stop == 0) {//已审核
                    if (isloop == "1") {
                        $("#detail_span1").text('提交').attr('operatetype', '提交确认').css("width", "100%");
                        $("#detail_span2").hide();
                    } else {
                        $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%");
                        $("#detail_span1").text('提交').attr('operatetype', '提交确认').css("width", "50%");
                    }
                    $("#detail_span3").hide();
                    editDisabled();
                } else if (status == 30 && stop == 0) {//待确认
                    if (isloop == "1") {
                        $("#detail_span1").hide();
                    } else {
                        $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                    }
                    $("#detail_span2").hide();
                    $("#detail_span3").hide();
                    editDisabled();
                }
                else if (status == 90 && stop == 0) {//已完成
                    $("#detail_canCon").hide();
                    editDisabled();
                }
                else if (stop == 90) {
                    $("#detail_canCon").hide();
                    editDisabled();
                }
            }
            else if (SCConData.whoPlan == 1) {//确认人
                if ((status == 0 || status == 15) && stop == 0) {//待提交
                    if (isloop == "1") {
                        $("#detail_span1").hide();
                    } else {
                        $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                    }
                    $("#detail_span2").hide();
                    $("#detail_span3").hide();
                    editDisabled();
                } else if (((status == 10 || status == 25) && stop == 0) || stop == 10) {//待审核
                    if (isloop == "1") {
                        $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '100%');
                        $("#detail_span2").hide();
                    } else {
                        $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '50%');
                        $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '50%');
                    }
                    $("#detail_span3").hide();
                    editDisabled();
                } else if ((status == 20 || status == 40) && stop == 0) {//已审核

                    if (isloop == "1") {
                        $("#detail_span1").hide();
                        $("#detail_span2").hide();
                    } else {

                        var initial = obj.attr('initial');

                        if (initial == "0") {
                            $("#detail_span1").hide();
                            $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '100%');
                        }
                        else {
                            $("#detail_span1").text('撤销').attr('operatetype', '撤销').css('width', '50%');
                            $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '50%');
                        }

                    }
                    $("#detail_span3").hide();
                    editDisabled();
                } else if (status == 30 && stop == 0) {//待确认
                    if (isloop == "1") {
                        $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "100%");
                        $("#detail_span2").hide();
                    } else {
                        $("#detail_span2").text('评论').attr('operatetype', '评论').css("width", "50%");
                        $("#detail_span1").text('确认').attr('operatetype', '确认').css("width", "50%");
                    }
                    $("#detail_span3").hide();
                    editDisabled();
                }
                else if (status == 90 && stop == 0) {//已完成
                    $("#detail_canCon").hide();
                    editDisabled();
                }
                else if (stop == 90) {
                    $("#detail_canCon").hide();
                    editDisabled();
                }
            }
        }
        
        planDetail = $.layer({
            type: 1,
            shade: [0.5,'#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '#layer_details' },
            move:'.title',
            closeBtn: true
        });

        //$(".planDetail .closeWCom").click(function () {
        //    $("#detail_partner").html('');
        //    $("#detail_premise").html('');
        //    layer.close(planDetail);
        //    $("#detail_operateinfo").html('');
        //})
    }
    //$("#detailAccessory .accessory").click(function () {
    //    $(this).next(".accessoryDiv").toggle();
    //})
    
    var setting = {
        view: {
            showIcon: false,
            showLine: false,
            selectedMulti: false
        }
    };

    //关闭
    $('#layer_details .closeWCom').off('click');
    $('#layer_details .closeWCom').click(function () {
        $("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
        $("#detail_front_div").find('span:not(.detail_front_span)').remove();
        $("#detail_label_div").find('span:not(.detail_label_span)').remove();
        $("#detailAccessory").hide();
        layer.close(planDetail);
        $("#detail_operateinfo").html('');
        $('#detailAccessory').hide();
        $('#detailAccessory .accessoryDiv').hide().find('ul').html('');
    });
    
    /*详情中的按钮操作 开始*/
    $('.xxc_operatebtn').off('click');
    $('.xxc_operatebtn').click(function () {
        var operatetype = $(this).attr("operatetype");
        var planId = $("#xxc_planId").val();
        if (planId) {
            planIdTemp = planId;
        }
        var org = $("#details_department_v").attr('term');
        var pro = $("#details_project_v").attr("term");
        var runmode = $("#detail_runmode option:selected").val();
        planDetailExemode = runmode;
        var eventoutput = $("#detail_eventoutput").val();
        var resUser = $("#detail_responsibleUser").attr('term');
        var conUser = $("#detail_confirmUser").attr('term');
        var looptype = $("#detail_looptype option:selected").val();
        var endtime = $("#detail_endtime").val();
        var initial = $("#detail_initial option:selected").val();
        var plan = { planId: planIdTemp, organizationId: org, projectId: pro, executionModeId: runmode, eventOutput: eventoutput, responsibleUser: resUser, confirmUser: conUser, loopType: looptype, initial: initial, endTime: endtime, collPlanList: collUsers, frontLists: frontplans, keyword: labels };
        var plans = [];
        var planList =[{ planId: planIdTemp, organizationId: org, projectId: pro, executionModeId: runmode, eventOutput: eventoutput, responsibleUser: resUser, confirmUser: conUser, loopType: looptype, initial: initial, endTime: endtime, collPlanList: collUsers, frontLists: frontplans ,keyword:labels}];
        if (operatetype == "提交") {
            if (org == "" || pro == "" || runmode == "" || eventoutput == "" || resUser == "" || conUser == "" || looptype == "" || endtime == "" || initial == "") {
                
                ncUnits.alert("存在空值，无法提交");
                return;
            }
            if (resUser == conUser) {
                ncUnits.alert("确认人不能与责任人相同，请重新选择");
                return;
            }
            var initial= $("#xxc_initial").val();
            plans.push(plan.collPlanList);
           
            $.ajax({
                url: "/plan/SaveUpdatePlan",
                type: "post",
                dataType: "json",
                data: {
                    plans: JSON.stringify(planList) 
                },
                success: rsHandler(function (data) {
                    if (data == "ok") {
                        $("#detailAccessory").hide();
                        layer.close(planDetail);
                        $("#detail_partner").html('');
                        $("#detail_premise").html('');
                        submitoperate(planId, initial);
                        $("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
                        $("#detail_front_div").find('span:not(.detail_front_span)').remove();
                        ncUnits.alert("计划已成功提交");
                    }
                    else {
                        ncUnits.alert("提交不成功");
                    }
                })
            });
            
            
        }
        else if (operatetype == "保存") {
            plans.push(plan);
            $.ajax({
                url: "/plan/SaveUpdatePlan",
                type: "post",
                dataType: "json",
                data: {
                    plans: JSON.stringify(plans)
                },
                success: rsHandler(function (data) {
                    if (data == "ok") {
                        $("#detailAccessory").hide();
                        layer.close(planDetail);
                        fnScreCon();
                        //$("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
                        $("#detail_front_div").find('span:not(.detail_front_span)').remove();
                        ncUnits.alert("保存成功");

                    }
                    else {
                        ncUnits.alert("保存失败");
                    }
                })
            });
        }
        else if (operatetype == "评论") {
            Discuss($("#layer_details"),planId);
        }
        else if (operatetype == "审核") {
            checkclick($("#layer_details"));
            
        }
        else if (operatetype == "确认") {
            DetailConfirm($("#layer_details"));
        }
        else if (operatetype == "提交确认") {
            submitconfirm($("#layer_details"));
        }
        else if (operatetype == "撤销") {
            $('#detailAccessory').hide();
            $('#detailAccessory .accessoryDiv').hide().find('ul').html('');
            layer.close(planDetail);
            canceloperate(planId, 10);
            ncUnits.alert("计划已撤销");
        }
        collUsers = [];
        frontplans = [];
        plans = [];
        
    })

    function Discuss(obj,planId)
    {
        $("#detail_operateinfo").load("/plan/DiscussView", { height: parseInt(obj.css('height')),planId:planId }, function () {
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
                commentDivLength += $(this).height();
            });
        });
    }
    /*详情中的按钮操作 结束*/

    function getNodeNameLine(node, nameLine) {
        if (node) {
            return getNodeNameLine(node.getParentNode(), nameLine ? node.name + " - " + nameLine : node.name);
        } else {
            return nameLine;
        }
    }
    
    //部门分类的tree
    $.ajax({
        type: "post",
        url: "/plan/GetOrganizationInfo",
        dataType: "json",
        success: rsHandler(function (data) {
            $.fn.zTree.init($("#details_department"), $.extend({
                callback: {
                    onClick: function (e, id, node) {
                        var nodeName = getNodeNameLine(node);
                        $("#details_department_v").html(nodeName);
                        $("#details_department_v").attr("term", node.id);
                        department_v = node.id;
                        department_text = nodeName;
                    }
                }
            }, setting), data);
            //$.fn.zTree.init($("#addsubplan_department"), $.extend({
            //    callback:{
            //        onClick:function(e,id,node){
            //            $("#addsubplan_department_v").html(getNodeNameLine(node));
            //            sub_department_v = node.id;
            //        }
            //    }
            //},setting),data);
        })
    });
    //项目分类的tree
    $.ajax({
        type: "post",
        url: "/plan/GetProjectInfo",
        dataType: "json",
        success: rsHandler(function (data) {
            $.fn.zTree.init($("#details_project"), $.extend({
                callback: {
                    onClick: function (e, id, node) {
                        var nodeName = getNodeNameLine(node);
                        $("#details_project_v").html(nodeName);
                        $("#details_project_v").attr("term", node.id);
                        project_v = node.id;
                        project_text = nodeName;
                    }
                }
            }, setting), data);
            //$.fn.zTree.init($("#addsubplan_project"),$.extend({
            //    callback:{
            //        onClick:function(e,id,node){
            //            $("#addsubplan_project_v").html(getNodeNameLine(node));
            //            sub_project_v = node.id;
            //        }
            //    }
            //},setting),data);
        })
    });
    
    //addplan部门分类的收缩展开
    $("#detail_department_icon").off('click');
    $("#detail_department_icon").click(function () {
        if ($(this).hasClass("arrowsBBBCom")) {
            $(this).removeClass("arrowsBBBCom");
            $(this).addClass("arrowsBBTCom");
            $("#details_department").slideDown();
        } else {
            $(this).addClass("arrowsBBBCom");
            $(this).removeClass("arrowsBBTCom");
            $("#details_department").slideUp();
        }
    });

    //addplan项目分类的收缩展开
    $("#detail_project_icon").off('click');
    $("#detail_project_icon").click(function () {
        if ($(this).hasClass("arrowsBBBCom")) {
            $(this).removeClass("arrowsBBBCom");
            $(this).addClass("arrowsBBTCom");
            $("#details_project").slideDown();
        } else {
            $(this).addClass("arrowsBBBCom");
            $(this).removeClass("arrowsBBTCom");
            $("#details_project").slideUp();
        }
    });

    //laydate.skin("huanglv");

    //addplan完成时间选项的弹层
    var end = {
        elem: '#detail_endtime', //需显示日期的元素选择器
        event: 'click', //触发事件
        format: 'YYYY-MM-DD hh:mm', //日期格式
        istime: true, //是否开启时间选择
        isclear: true, //是否显示清空
        istoday: true, //是否显示今天
        issure: true, //是否显示确认
        festival: true, //是否显示节日
        //min: '1900-01-01 00:00:00', //最小日期
        //max: '2099-12-31 23:59:59', //最大日期
        //start: '2014-6-15 23:00:00',    //开始日期
        //fixed: false, //是否固定在可视区域
        //zIndex: 99999999, //css z-index
        choose: function (dates) { //选择好日期的回调
            endTime_v = dates;
            start.max = dates;
        }
    }

    $("#detail_endtime").off('click');
    $("#detail_endtime").click(function () {
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
        choose: function (dates) {
            roundTime_v = dates;
            end.start = dates;
            end.min = dates;
        }
    }
    //$("#detail_roundtime").off('click');
    //$("#detail_roundtime").click(function () {
    //    laydate(start);
    //});


    $("#detail_runmode").change(function () {
        runMode_v = $(this).val();
        runMode_text = $("option:selected", $(this)).text();
    });

    $("#isTmp").change(function () {
        isTmp_v = $(this).val();
    });
    //循环类型
    $("#roundtype").change(function () {
        roundType_v = $(this).val();
        roundType_text = $("option:selected", this).text();
        if (roundType_v == undefined || roundType_v == "") {
            $(this).css({ color: "#a0a0a0" });
            $("#addplan_roundtime").hide();
            $("#worktimeDiv").hide();
        } else {
            $(this).css({ color: "#686868" });
            if (roundType_v == 0) {
                $("#addplan_roundtime").hide();
                $("#worktimeDiv").hide();
                //$("#layer_addplan .title .sub").show();
            } else {
                $("#addplan_roundtime").show();
                $("#worktimeDiv").show();
                //$("#layer_addplan .title .sub").hide();
            }
        }
    });
    
    //责任人选择
    $("#detai_responsibleUser").searchStaffPopup({
        url: "/plan/GetOfferUsers",
        hasImage: true,
        selectHandle: function (data) {
            responsibleUser_v = data.id;
            responsibleUser_name = data.name;
            $(this).val(responsibleUser_name);
            $(this).attr("term", data.id);
        }
    });
    
    //确认人选择
    $("#detail_confirmUser").searchStaffPopup({
        url: "/plan/GetOfferUsers",
        hasImage: true,
        selectHandle: function (data) {
            confirmUser_v = data.id;
            confirmUser_name = data.name;
            $(this).val(data.name);
            $(this).attr("term", data.id);
        }
    });

    //协作人选择
    $("#detail_partner").searchStaffPopup({
        url: "/plan/GetOfferUsers",
        hasImage: true,
        selectHandle: function (data) {
            
            collUsers.push({ id: data.id });
            var $span = $("<span class='collplan' term='" + data.id + "'></span>").addClass("tag");
            var $close = $("<span term='" + data.id + "'>X</span>").addClass("close").css({ display: "none" });
            $span.hover(function () {
                $close.toggle();
            })

            $span.append([data.name, $close]);

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

    //前提计划选择
    $("#detail_premise").searchStaffPopup({
        url: "/plan/GetFrontPlans",
        selectHandle: function (data) {
            frontplans.push({ planId: data.planId });
            var $span = $("<span class='frontplan' term='" + data.planId + "'></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

            $span.hover(function () {
                $close.toggle();
            });

            $span.append([data.eventOutput, $close]);
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
    
    //详情设置
    var tagsList = [];
    $.ajax({
        url: "/Shared/GetMostUsedTag",
        type: "post",
        dataType: "json",
        success: rsHandler(function (data) {
            tagsList = $.map(data, function (n) {
                return {
                    label: n
                }
            })
        })
    });
    $("#detail_label").off('click');
    $("#detail_label").click(function () {
        var $this = $(this);
        $this.hide();
        var $con = $("<div style='display: inline-block;position: relative;'></div>");
        var $input = $('<input type="text" style="border: 1px solid #e3e3e3;width: 100px;height: 26px;vertical-align: top;"/>');
        var $em = $("<em style='border: 1px solid #00B83F;background-color: #00B83F;  position: absolute;right: 0px;'></em>").addClass("icon add-min-grey");

        $con.append([$input, $em]).appendTo($this.parent());

        function addtag() {

            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({
                display: "none",
                "font-size": "12px"
            });

            $span.hover(function () {
                $close.toggle();
            })

            var iv = $input.val();

            $span.append([iv, $close]);
            $this.parent().before($span);

            //tab_v.push(iv);

            $close.click(function () {
                $span.remove();
                removeValue(tab_v, iv);
            });

            $con.remove();
            $this.show();
        }

        $input.autocompleter({
            source: tagsList
        });
        $input.keydown(function (e) {
            console.log(e.keyCode);
            if (e.keyCode == 13 && $(this).val()) {
                addtag();
            } else if (e.keyCode == 27) {
                $con.remove();
                $this.show();
            }
        }).blur(function () {
            if ($(this).val()) {
                addtag();
            } else {
                $con.remove();
                $this.show();
            }
        });

        $em.click(function () {
            if ($input.val()) {
                addtag();
            }
        });

    });
    
    //addplan执行方式加载
    //$.ajax({
    //    type: "post",
    //    url: "/plan/GetExecutionList",
    //    dataType: "json",
    //    success: rsHandler(function (data) {
    //        for (var i = 0, len = data.length; i < len ; i++) {
    //            $("<option term='" + data.id + "'></option>").val(data[i].id).html(data[i].text).appendTo($("#detail_runmode"));
    //        }
    //    })
    //});

    /* 计划详情 结束 */

    /* 评论 开始 */
    
    /* 评论 结束 */
    
    /*分解计划 开始*/
    var decPlan;
    var department_v              //部门值
    , project_v               //项目值
    , runMode_v                //执行方式
    , output_v                   //事项输出结果
    , endTime_v                  //完成时间
    , responsibleUser_v          //责任人
    , confirmUser_v             //确认人
    , partner_v = [];                 //协作人
    var selfid;//添加计划的ID
    var planInfos = [];

    function getNodeNameLine(node, nameLine) {
        if (node) {
            return getNodeNameLine(node.getParentNode(), nameLine ? node.name + " - " + nameLine : node.name);
        } else {
            return nameLine;
        }
    }
    $('.decPlan').off('click');
    $('.decPlan').click(function () {
        $("#xxc_planId").val($(this).attr('term'));
        $("#xxc_endTime").val($(this).attr('endTime'));
        selfid = 0;
        decPlan = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            //title: ['新建计划','background:#58b456;color:#fff;'],
            title: false,
            border: [0],
            page: { dom: '.layer_decplan' },
            move: ".title",
            closeBtn: false
        });
        $(".layer_decplan .closeWCom").click(function () {
            getOldData();
        })
    });
    //部门分类的tree
    $.ajax({
        type: "post",
        url: "/plan/GetOrganizationInfo",
        dataType: "json",
        success: rsHandler(function (data) {
            $.fn.zTree.init($("#decplan_department"), $.extend({
                callback: {
                    onClick: function (e, id, node) {
                        $("#decplan_department_v").html(getNodeNameLine(node));
                        $("#decplan_department_v").attr('term', node.id);
                        department_v = node.id;
                        $("#decplan_department_icon").click();
                    }
                }
            }, setting), data);
        })
    });
    var setting = {
        view: {
            showIcon: false,
            showLine: false,
            selectedMulti: false
        }
    };
    //项目分类的tree
    $.ajax({
        type: "post",
        url: "/plan/GetProjectInfo",
        dataType: "json",
        success: rsHandler(function (data) {
            $.fn.zTree.init($("#decplan_project"), $.extend({
                callback: {
                    onClick: function (e, id, node) {
                        $("#decplan_project_v").html(getNodeNameLine(node));
                        project_v = node.id;
                        $("#decplan_project_v").attr('term', project_v);
                        $("#decplan_project_icon").click();
                    }
                }
            }, setting), data);
        })
    });

    //decplan部门分类的收缩展开
    $("#decplan_department_icon").off('click');
    $("#decplan_department_icon").click(function () {
        if ($(this).hasClass("arrowsBBBCom")) {
            $(this).removeClass("arrowsBBBCom");
            $(this).addClass("arrowsBBTCom");
            $("#decplan_department").slideDown();
        } else {
            $(this).addClass("arrowsBBBCom");
            $(this).removeClass("arrowsBBTCom");
            $("#decplan_department").slideUp();
        }
    });

    //decplan项目分类的收缩展开
    $("#decplan_project_icon").off('click');
    $("#decplan_project_icon").click(function () {
        if ($(this).hasClass("arrowsBBBCom")) {
            $(this).removeClass("arrowsBBBCom");
            $(this).addClass("arrowsBBTCom");
            $("#decplan_project").slideDown();
        } else {
            $(this).addClass("arrowsBBBCom");
            $(this).removeClass("arrowsBBTCom");
            $("#decplan_project").slideUp();
        }
    });

    $("#decplan_runmode").change(function () {
        runMode_v = $(this).val();
    });
    //addplan执行方式加载
    //var aaa = [{id:1,text:'孙子'},{}...]
    $.ajax({
        type: "post",
        url: "/plan/GetExecutionList",
        dataType: "json",
        success: rsHandler(function (data) {
            $('.xxc_runmode').html('');
            for (var i = 0, len = data.length; i < len ; i++) {
                $("<option></option>").val(data[i].id).html(data[i].text).appendTo($(".xxc_runmode"));
            }
            if (planDetailExemode) {
                $("#detail_runmode option").each(function () {
                    if (parseInt($(this).val()) == planDetailExemode) {
                        $(this).attr("selected", "true");
                    }
                });
            }
        })
    });
    //确认责任人
    $("#decplan_responsibleUser").searchStaffPopup({
        url: "/plan/GetOfferUsers",
        hasImage: true,
        selectHandle: function (data) {
            $(this).val(data.name);
            $(this).attr('term', data.id);
            responsibleUser_v = data.id;
        }
    });
    //确认人选择
    $("#decplan_confirmUser").searchStaffPopup({
        url: "/plan/GetOfferUsers",
        hasImage: true,
        selectHandle: function (data) {
            confirmUser_v = data.id;
            confirmUser_name = data.name;
            $(this).attr('term', data.id);
            $(this).val(data.name);
        }
    });
    $("#decplan_endtime").off('click');
    $("#decplan_endtime").click(function () {
        laydate({
            elem: '#decplan_endtime_v',
            event: 'click',
            format: 'YYYY-MM-DD hh:mm',
            //min: laydate.now(),
            istime:'true',
            isclear: true,
            istoday: true,
            issure: true,
            festival: true,
            start: new Date().toLocaleDateString() + ' 17:30:00',
            choose: function (dates) {
                endTime_v = dates;
            }
        });
    });


    //点击加号添加计划数据
    var html;//添加计划需要添加的html
    $('.addA').off('click');
    $('.addA').click(function () {
        var parentPlanId, imp, urg, dif, org, orgtext, pro, protext, execution, executionModel, eventoutput, resUser, resUserName, confUser, confUserName, date
        var num = 0;
        function getPlanInfo(id) {
            //首先：获取输入
            parentPlanId = $("#xxc_planId").val();
            imp = $('.fenjie_import').find('.liHit').length;
            urg = $('.fenjie_urg').find('.liHit').length;
            dif = $('.fenjie_dif').find('.liHit').length;
            org = $('#decplan_department_v').attr('term');
            orgtext = $('#decplan_department_v').html();
            pro = $('#decplan_project_v').attr('term');
            protext = $('#decplan_project_v').html();
            execution = $('#decplan_runmode').val();
            executionModel = $('#decplan_runmode option:selected').text();
            eventoutput = $("#fenjie_eventoutput").val();
            resUser = $("#decplan_responsibleUser").attr('term');
            resUserName = $("#decplan_responsibleUser").val();
            confUser = $("#decplan_confirmUser").attr('term');
            confUserName = $("#decplan_confirmUser").val();
            date = $("#decplan_endtime_v").val();
            var parentdate = $("#xxc_endTime").val();
            if (imp <= 0 || urg <= 0 || dif <= 0 || org == "" || pro == "" || execution == "" || executionModel=="执行方式" || eventoutput == "" || resUser == "" || date == "") {
               
                ncUnits.alert("存在空白项，请查看");
                selfid--;
                return false;
            }
            else if (Date.parse(date) > Date.parse(parentdate)) {
                ncUnits.alert("子计划的完成时间必须小于父计划的完成时间");
                selfid--;
                return false;
            }
            //清空分解画面
            emptyPlan();
            return {
                selfId: (id) ? id : selfid,
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
        var planInfo = getPlanInfo();
        if (planInfo) {
            var html = '<li><span class="decplanSelfId" style="display:none;">' + planInfo.selfId + '</span>' +
                  '<span class="decplanState">' + planInfo.executionModeText + '</span>' +
                  '<span class="decplanTitle">' + planInfo.eventOutput + '</span>' +
                  '<span class="decplanPerson">' + planInfo.responsibleUserName + '</span>' +
                  '<span class="decplanDate">' + planInfo.oldEndTime + '</span>' +
                  '<div class="operate_tr"><div class="operateDiv">' +
                  '<span class="operateBg"></span><div class="operateText">' +
                  '<ul><li class="decplan_detail"><img src="../../Images/plan/detailsWhite.png" /><span>详情</span></li>' +
                  '<li class="decplan_delete"><img src="../../Images/plan/deleteWhite.png" /><span>删除</span></li></ul></div>' +
                  '</div></div></li>';
            //拼接显示新添加的子计划
            $(".decplanDone .decplanList").append(html);
            $('.operate_tr li').off('hover');
            $('.decplanList li').hover(function () {
                $(this).find('.operate_tr').css('display', 'block');
            }, function () {
                $(this).find('.operate_tr').css('display', 'none');
            });
            planInfos.push(planInfo);
        }
        //计划删除
        $('.decplan_delete').off("click");
        $('.decplan_delete').click(function () {
            var _this = this;
            ncUnits.confirm({
                title: '提示',
                html: '确认要删除？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    var $li = $(_this).parent().parent().parent().parent().parent();
                    var id = $li.find('.decplanSelfId').html();
                    for (var i = 0; i < planInfos.length; i++) {
                        if (planInfos[i].selfId==id) {
                            planInfos.splice(i, 1);
                        }
                    }
                    
                    $li.remove();
                    console.log(planInfos);
                }
            });
        });

        //计划详情
        $('.decplan_detail').off('click');
        $('.decplan_detail').click(function () {
            var $deleteSpan = $(this).next()
            $deleteSpan.css('display', 'none');
            var id = $(this).parent().parent().parent().parent().parent().find('.decplanSelfId').html();
            $('.decplanImportant').find('li').removeClass('liHit');
            $('.decplanEmergency').find('li').removeClass('liHit');
            $('.decplanDifficulty').find('li').removeClass('liHit');
            $('.decplanImportant').find('li').each(function (index) { if (index < planInfos[id].importance) $(this).addClass('liHit'); });
            $('.decplanEmergency').find('li').each(function (index) { if (index < planInfos[id].urgency) $(this).addClass('liHit'); });
            $('.decplanDifficulty').find('li').each(function (index) { if (index < planInfos[id].difficulty) $(this).addClass('liHit'); });
            $('#decplan_department_v').attr('term', planInfos[id].organizationId);
            $('#decplan_department_v').html(planInfos[id].organizationName);
            $('#decplan_project_v').attr('term', planInfos[id].projectId);
            $('#decplan_project_v').html(planInfos[id].projectName);
            $('#decplan_runmode').val(planInfos[id].executionModeId);
            $('#decplan_runmode option:selected').text(planInfos[id].executionModel);
            $("#fenjie_eventoutput").val(planInfos[id].eventOutput);
            $("#decplan_responsibleUser").attr('term', planInfos[id].responsibleUser);
            $("#decplan_responsibleUser").val(planInfos[id].responsibleUserName);
            $("#decplan_confirmUser").attr('term', planInfos[id].confirmUser);
            $("#decplan_confirmUser").val(planInfos[id].confirmUserName);
            $("#decplan_endtime_v").val(planInfos[id].oldEndTime);
            $('.addA').css('display', 'none');
            $('.addB').css('display', 'block');
            // $(this).next().trigger('click');
            $('.addB').off('click');
            $('.addB').click(function () {
                planInfos[id] = getPlanInfo(id);
                $deleteSpan.css('display', 'block');
                $('.addA').css('display', 'block');
                $('.addB').css('display', 'none');
                $(".decplanDone .decplanList").find('.decplanSelfId').each(function () {
                    if ($(this).html() == planInfos[id].selfId) {
                        $(this).parent().find('.decplanState').html(planInfos[id].executionModeText);
                        $(this).parent().find('.decplanTitle').html(planInfos[id].eventOutput);
                        $(this).parent().find('.decplanPerson').html(planInfos[id].responsibleUserName);
                        $(this).parent().find('.decplanDate').html(planInfos[id].oldEndTime);
                    }
                });
                emptyPlan();
            });
        });
        selfid++;
    });
    //取消
    $("#fenjie_cancel").off('click');
    $("#fenjie_cancel").click(function () {
        getOldData();
    });

    //恢复最初的界面
    function getOldData() {
        html = '';
        planInfos = [];
        $("#xxc_planId").val('');
        $('.addA').css('display', 'block');
        $('.addB').css('display', 'none');
        emptyPlan();
        layer.close(decPlan);
        $(".decplanDone .decplanList").html(html);
    }

    //确定
    $("#fenjie_sure").off('click');
    $("#fenjie_sure").click(function () {
        if (planInfos.length<=0) {
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
                    planInfos = [];
                    emptyPlan();
                    $("#xxc_planId").val('');
                    $("#decplanDone .con_decplanDone table tbody").html('');
                    $('.addA').css('display', 'block');
                    $('.addB').css('display', 'none');
                    layer.close(decPlan);
                    ncUnits.alert("分解成功！");
                    fnScreCon();
                    $(".decplanDone .decplanList").html('');
                }
                else {
                    ncUnits.alert("分解失败！");
                }
            })
        });
    });

    function getdecplandata()
    {
        var parentPlanId, imp, urg, dif, org, orgtext, pro, protext, execution, executionModel, eventoutput, resUser, resUserName, confUser, confUserName, date;
        //首先：获取输入
        parentPlanId = $("#xxc_planId").val();
        imp = $('.fenjie_import').find('.liHit').length;
        urg = $('.fenjie_urg').find('.liHit').length;
        dif = $('.fenjie_dif').find('.liHit').length;
        org = $('#decplan_department_v').attr('term');
        orgtext = $('#decplan_department_v').html();
        pro = $('#decplan_project_v').attr('term');
        protext = $('#decplan_project_v').html();
        execution = $('#decplan_runmode').val();
        executionModel = $('#decplan_runmode option:selected').text();
        eventoutput = $("#fenjie_eventoutput").val();
        resUser = $("#decplan_responsibleUser").attr('term');
        resUserName = $("#decplan_responsibleUser").val();
        confUser = $("#decplan_confirmUser").attr('term');
        confUserName = $("#decplan_confirmUser").val();
        date = $("#decplan_endtime_v").val();
        if (imp > 0 || urg > 0 || dif > 0 || org != "" || pro != "" || execution != "" || executionModel != "执行方式" || eventoutput != "" || resUser != "" || confUser!="" || date != "") {
            ncUnits.confirm({
                title: '提示',
                html: '计划未成功添加，确定要取消吗？',
                yes: function (id) {
                getOldData();
                layer.close(id);
            },
            });
            
        }
    }

    //清空分解信息
    function emptyPlan() {
        $('.fenjie_import').find('.liHit').removeClass('liHit');
        $('.fenjie_urg').find('.liHit').removeClass('liHit');
        $('.fenjie_dif').find('.liHit').removeClass('liHit');
        $('#decplan_department_v').text('');
        $('#decplan_department_v').attr('term', '0');
        $('#decplan_project_v').text('');
        $('#decplan_project_v').attr('term', '0');
        $('#decplan_department').find('.curSelectedNode').removeClass('curSelectedNode');
        $('#decplan_project').find('.curSelectedNode').removeClass('curSelectedNode');
        $('#decplan_runmode').val('');
        $("#fenjie_eventoutput").val('');
        $("#decplan_responsibleUser").val('');
        $("#decplan_confirmUser").val('');
        $("#decplan_endtime_v").val('');

        if ($("#decplan_department_icon").hasClass("arrowsBBTCom")) {
            $("#decplan_department_icon").click();
        }
        if ($("#decplan_project_icon").hasClass("arrowsBBTCom")) {
            $("#decplan_project_icon").click();
        }
        //$("#decplan_department_icon").trigger('click');
        //$("#decplan_project_icon").trigger('click');
        //$("#decplan_department").slideUp();
        //$("#decplan_project").slideUp();
    }
    /*分解计划 结束*/
    



    /* 计划块上面的三角切换块 开始 */
    $('.main .arrowsBBLCom').click(function () {
        $(this).hide();
        $(this).next('.accomplishment').show();
    });
    $('.main .arrowsBBRCom').click(function () {
        $(this).parent('.accomplishment').hide();
        $(this).parent('.accomplishment').prev('.arrowsBBLCom').show();
    });
    /* 计划块上面的三角切换块 结束 */

    
});

function fnBindPlanInfo() {
    var planIdNew = $("#xxc_planId").val();
    $("#checksPopUp").load("/Plan/getPlanInfoById", { planID: planIdNew }, { params: JSON.stringify(SCConData) });
}


//弹出附件下载失败的提示信息
function noFile() {
    ncUnits.alert("下载失败！");
}