//@ sourceURL=Confirm.js
//$(function () {
    $('.xubox_layer').css({ 'margin-left': '-481px', 'width': '962px', 'background-color': '#f9f9f9' });
    var planIdNew = $("#xxc_planId").val();
    var confirmInfo_url = "/plan/GetPlanConfirmInfo";
    $.ajax({
        url: "/plan/GetPlanConfirmInfo",
        type: "post",
        dataType: "json",
        data: { planId: planIdNew },
        success: rsHandler(function (data) {
            console.log(112121221122121)
            if (data != null) {
                $('.realEachTime').text(data.time);
                $('.realNum').text(data.quantity);
                $('.realAllTime').text(parseInt(data.time) * parseInt(data.quantity));
                $('.evaluateTimeV').text((parseInt(data.time) * parseInt(data.quantity) * parseFloat($('.numDegree').val()) * parseFloat($('.quaDegree').val()) * parseFloat($('.timeDegree').val())).toFixed(1));
            }
        })
    });
    
    $("#confirmBox").show().css("height", "" + parseInt($("#confirmheight").val()) + "");
    $('#num_slider').slider({
        orientation: "horizontal",
        range: "min",
        max: (parseInt($("#completeQuantity").val()) * 100 + 1),
        min: 0,
        value: 100,
        slide: function () {
            refreshValue('.numDegree', '#num_slider');
        },
        change: function () {
            refreshValue('.numDegree', '#num_slider');
        }
    });
    $('#qua_slider').slider({
        orientation: "horizontal",
        range: "min",
        max: (parseInt($("#completeQuality").val())*100 + 1),
        min: 0,
        value: 100,
        slide: function () {
            refreshValue('.quaDegree', '#qua_slider');
        },
        change: function () {
            refreshValue('.quaDegree', '#qua_slider');
        }
    });
    $('#time_slider').slider({
        orientation: "horizontal",
        range: "min",
        max: (parseInt($("#completeTime").val()) * 100 + 1),
        min: 0,
        value: 100,
        slide: function () {
            refreshValue('.timeDegree', '#time_slider');
        },
        change: function () {
            refreshValue('.timeDegree', '#time_slider');
        }
    });
    //刷新拖动值
    function refreshValue(refreshArea, silder) {
        var degree = $(silder).slider('value');
        $(refreshArea).val((degree / 100).toFixed(1));
        calculateTime();
    }


    // 手动设置系数
    $('.numDegree').blur(function () {
        var nvalu = $(this).val();
        $('#num_slider').slider('value', nvalu * 100);
        calculateTime();
    });
    $('.quaDegree').blur(function () {
        var qvalu = $(this).val();
        $('#qua_slider').slider('value', qvalu * 100);
        calculateTime();
    });
    $('.timeDegree').blur(function () {
        var tvalu = $(this).val();
        $('#time_slider').slider('value', tvalu * 100);
        calculateTime();
    });

    // 计算评定完成时间
    calculateTime();
    function calculateTime() {
        var evaluateTimeVal = $('.realAllTime').html() * $('.numDegree').val() * $('.quaDegree').val() * $('.timeDegree').val();
        $('.evaluateTimeV').html((evaluateTimeVal).toFixed(1));
    }

    //确认通过
    $("#xxc_confirmpass").off('click');
    $("#xxc_confirmpass").click(function () {
        var planIdNew = $("#xxc_planId").val();
        var numDegree = $(".numDegree").val();
        var quaDegree = $(".quaDegree").val();
        var timeDegree = $(".timeDegree").val();
        var confirmMsg = $("#xxc_confirmMsg").val();
        if (numDegree == "" || quaDegree == "" || timeDegree == "") {
            ncUnits.alert("存在空值");
            return;
        }
        $.ajax({
            type: "post",
            url: "/plan/ConfirmPlan",
            dataType: "json",
            data: { planId: planIdNew, completeQuantity: numDegree, completeQuality: quaDegree, completeTime: timeDegree, isTrue: true, msg: confirmMsg },
            success: rsHandler(function (data) {
                if (data) {
                   
                    if (page_flag && page_flag == "CalendarProcess") {   //日程化页面
                        $("#plan_detail_modal").modal("hide");
                        loadingPlanList();
                    } else if (page_flag == 'plan') {  //计划页面
                        if (planDetail) {
                            layer.close(planDetail);
                        }

                        fnScreCon();
                    } else {
                        //首页计划确认成功刷新列表
                        $('#xxc_confirmpass').trigger('addPlan')
                        $("#plan_detail_modal").modal("hide");
                       
                    }
                    ncUnits.alert("计划确认成功！");
                    $("#detailAccessory").hide();
                    $('#detailAccessory .accessoryDiv').hide().find('ul').html('');
                    $("#detail_operateinfo").html('');
                    $(".evaluateBox").hide();
                    
                } else {
                    ncUnits.alert("计划确认出错！");
                }
            })
        });
    });
    //确认未通过
    $("#xxc_confirmnopass").off('click');
    $("#xxc_confirmnopass").click(function () {
        var planIdNew = $("#xxc_planId").val();
        var confirmMsg = $("#xxc_confirmMsg").val();
        $.ajax({
            type: "post",
            url: "/plan/ConfirmPlan",
            dataType: "json",
            data: { planId: planIdNew, completeQuantity: '0', completeQuality: '0', completeTime: '0', isTrue: false, msg: confirmMsg },
            success: rsHandler(function (data) {
                if (data) {
                    
                    if (page_flag && page_flag == "CalendarProcess") {   //日程化页面
                        $("#plan_detail_modal").modal("hide");
                        loadingPlanList();
                    } else if (page_flag == 'plan') {  //计划页面
                        if (planDetail) {
                            layer.close(planDetail);
                        }

                        fnScreCon();
                    } else {
                        //首页计划确认成功刷新列表
                        $('#xxc_confirmnopass').trigger('addPlan')
                        $("#plan_detail_modal").modal("hide");
                    }
                    $("#detailAccessory").hide();
                    $('#detailAccessory .accessoryDiv').hide().find('ul').html('');
                    $("#detail_operateinfo").html('');
                    $(".evaluateBox").hide();
                    ncUnits.alert("计划确认未通过成功！");

                } else {
                    ncUnits.alert("计划确认出错！");
                }
            })
        });
    });
//});