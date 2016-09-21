//@ sourceURL=incentiveFormula.js
/**
 * Created by DELL on 15-8-14.
 */
 
$(function () { 
    loadPersonalInfo();  
    var ruleArray = [];
    var deleteIdArray = [];
    var TimeList = [];
    var HasList = [];
    //保存
    $("#planExtension-save").click(function () {
        var Returndata = {
            rule: [],
            deleteId: []              //删除规则ID
        }
        
        //if ($("#planExtension_load>.row").length == 0) {
        //    ncUnits.alert("请定义规则");
        //    return;
        //}
        //else {
            for (var i = 0; i < ruleArray.length; i++) {
                Returndata.rule.push($.extend(true, {}, ruleArray[i]))
                Returndata.rule[i].ruleId = null;
            }
            Returndata.deleteId = deleteIdArray.slice(0);
       // }
        $.ajax({
            type: "post",
            url: "/DelayRule/UpdateAndDelete",
            dataType: "json",
            data: { data: JSON.stringify(Returndata) },
            success: rsHandler(function () {
                ncUnits.alert("保存成功");
                TimeList = [];
                planExtensionLoad();
            })
        });
    });
    $("#AT-save").click(function () {
        var Returndata = {
            rule: [],
            deleteId: []                       //删除规则ID
        }
        if ($("#AT_load>.row").length == 0) {
            ncUnits.alert("请添加公式");
            return;
        }
        else {
            for (var i = 0; i < ruleArray.length; i++) {
                Returndata.rule.push($.extend(true, {}, ruleArray[i]))
                Returndata.rule[i].ruleId = null;
            }
            Returndata.deleteId = deleteIdArray.slice(0);
        }
        $.ajax({
            type: "post",
            url: "/DelayRule/UpdateAndDelete",
            dataType: "json",
            data: { data: JSON.stringify(Returndata) },
            success: rsHandler(function () {
                ncUnits.alert("保存成功");
                TimeList = [];
                ApprovalTimeoutLoad();
            })
        });
    });
    $("#EH-save").click(function () {
        var EffectiveHoursData = {
            value: {
                standardTime: null,      //标准工时
                incentiveType: null,       //激励方式
                maxValueType: null,        //最高奖励类型
                maxValue: null,        //最高奖励
                maxAverage: null,      //奖励平均
                minValueType: null,        //最高扣除类型
                minValue: null,        //最高扣除
                minAverage: null       //扣除平均
            },
            custom: [],
            deleteId: []                     //删除自定义公式ID
        }
        if ($("#standardTime").val() == "") {
            ncUnits.alert("请输入标准工时");
            return;
        }
        EffectiveHoursData.value.standardTime = $("#standardTime").val();
        var flag = false;
        $(".incentiveType").each(function () {
            if ($(this).prop("checked")==true) {
                EffectiveHoursData.value.incentiveType = $(this).attr("term");
                if (EffectiveHoursData.value.incentiveType == 1) {
                    if ($("#maxValue").val() == "") {
                        ncUnits.alert("请输入最高奖励额度");
                        flag = true;
                        return false;
                    }
                    if ($("#maxAverage").val() == "") {
                        ncUnits.alert("请输入平均奖励额度");
                        flag = true;
                        return false;
                    }
                    if ($("#minValue").val() == "") {
                        ncUnits.alert("请输入最大扣除额度");
                        flag = true;
                        return false;
                    }
                    if ($("#minAverage").val() == "") {
                        ncUnits.alert("请输入平均扣除额度");
                        flag = true;
                        return false;
                    }
                  
                }
                else if (EffectiveHoursData.value.incentiveType==2) {
                    //if ($("#EffectiveHours_load>.row").length == 0) {
                    //    ncUnits.alert("请自定义激励方式");
                    //    flag = true;
                    //    return false;
                    //}
                    //else {
                        for (var i = 0; i < ruleArray.length; i++) {
                            EffectiveHoursData.custom.push($.extend(true, {}, ruleArray[i]))
                            EffectiveHoursData.custom[i].customId = null;
                       // }
                    }
                }
            }
  
        });
        EffectiveHoursData.value.maxValueType = $("#maxValueType").attr("term");
        EffectiveHoursData.value.minValueType = $("#minValueType").attr("term");
        EffectiveHoursData.value.maxValue = $("#maxValue").val();
        EffectiveHoursData.value.maxAverage = $("#maxAverage").val();
        EffectiveHoursData.value.minValue = $("#minValue").val();
        EffectiveHoursData.value.minAverage = $("#minAverage").val();
        if (flag == true) {
            return;
        }
        if (EffectiveHoursData.value.incentiveType == null) {
            ncUnits.alert("请选择激励方式");
            return;
        }

        EffectiveHoursData.deleteId = deleteIdArray.slice(0);
        $.ajax({
            type: "post",
            url: "/DelayRule/ValueUpdateAndDelete",
            dataType: "json",   
            data: { data: JSON.stringify(EffectiveHoursData) },
            success: rsHandler(function () {
                EffectiveHoursLoad();
                HasList = [];
                ncUnits.alert("保存成功");
            })
        });
 
    });
    //点击切换
    $("#planExtensionTab").click(function () {
        planExtensionLoad();
        $(this).addClass("green_color");
        $("#ApprovalTimeoutTab").removeClass("green_color");
        $("#EffectiveHoursTab").removeClass("green_color");
    });
    $("#ApprovalTimeoutTab").click(function () {
        ApprovalTimeoutLoad();
        $(this).addClass("green_color");
        $("#planExtensionTab").removeClass("green_color");
        $("#EffectiveHoursTab").removeClass("green_color");
    });
    $("#EffectiveHoursTab").click(function () {
        EffectiveHoursLoad();
        $(this).addClass("green_color");
        $("#ApprovalTimeoutTab").removeClass("green_color");
        $("#planExtensionTab").removeClass("green_color");
    })


    planExtensionLoad();
    //radio点击事件
    $(".incentiveType").click(function () {
        if ($(this).attr("id") == "incentiveType-1") {
            //按比例不可用
            $("#EH-maxValue-row,#maxValue,#maxAverage,#EH-minValue-row,#minValue,#minAverage").addClass("disabledColor");
            $("#EH-maxValue-row .dropdown-toggle,#EH-minValue-row .dropdown-toggle").addClass("disabled");
            $("#maxValue,#maxAverage,#minValue,#minAverage").prop("disabled", true);
            //自定义部分不可用
            $("#EH-Add-row,#EH-customStartTime,#EH-customEndTime,#EH-deductionNumLeft,#EH-deductionNumRight").addClass("disabledColor");
            $("#EH-Add-row .dropdown-toggle").addClass("disabled");
            $("#EH-Add-row input[type='text']").prop("disabled", true);
            $("#EffectiveHours-add").addClass("disabled");
            $("#EffectiveHours_load").addClass("disabled");

        }
        else if ($(this).attr("id") == "incentiveType-2") {
            //按比例可用
            $("#EH-maxValue-row,#maxValue,#maxAverage,#EH-minValue-row,#minValue,#minAverage").removeClass("disabledColor");
            $("#EH-maxValue-row .dropdown-toggle,#EH-minValue-row .dropdown-toggle").removeClass("disabled");
            $("#maxValue,#maxAverage,#minValue,#minAverage").prop("disabled", false);
            //自定义部分不可用
            $("#EH-Add-row,#EH-customStartTime,#EH-customEndTime,#EH-deductionNumLeft,#EH-deductionNumRight").addClass("disabledColor");
            $("#EH-Add-row .dropdown-toggle").addClass("disabled");
            $("#EH-Add-row input[type='text']").prop("disabled", true);
            $("#EffectiveHours-add").addClass("disabled");
            $("#EffectiveHours_load").addClass("disabled");
        }
        else if ($(this).attr("id") == "incentiveType-3") {
            //按比例不可用
            $("#EH-maxValue-row,#maxValue,#maxAverage,#EH-minValue-row,#minValue,#minAverage").addClass("disabledColor");
            $("#EH-maxValue-row .dropdown-toggle,#EH-minValue-row .dropdown-toggle").addClass("disabled");
            $("#maxValue,#maxAverage,#minValue,#minAverage").prop("disabled", true);
            //自定义部分可用
            $("#EH-Add-row,#EH-customStartTime,#EH-customEndTime,#EH-deductionNumLeft,#EH-deductionNumRight").removeClass("disabledColor");
            $("#EH-Add-row .dropdown-toggle").removeClass("disabled");
            $("#EH-Add-row input[type='text']").prop("disabled", false);
            $("#EffectiveHours-add").removeClass("disabled");
            $("#EffectiveHours_load").removeClass("disabled");
        }

    })

    //+ - 及小数的输入控制
    $("#EH-deductionNumLeft").bind("input keyDown", function () {
   
        //var values = $(this).val().substring(0, $(this).val().length - 1);
        //var getValue = $(this).val();
        //if (reg.test(getValue) == false) {
        //    $(this).val(values);
        //    return;
        //}

        var $value = $(this);
        var reg = /^([+-])?$/,
            vals = $value.val().match(reg);

        $value.val(vals ? vals[0] : "");
    });
    $("#AT_delayStartTime,#AT_delayEndTime,#planExtension_delayStartTime,#planExtension_delayEndTime,#EH-customStartTime,#EH-customEndTime").bind("input", function () {
        //var reg = /^\d+(\.\d{0,1})?$/;
        //var values = $(this).val().substring(0, $(this).val().length - 1);
        //var getValue = $(this).val();
        //if (reg.test(getValue) == false) {
        //    $(this).val(values);
        //    return
        //}       
        //if (getValue.indexOf(".") == -1 && getValue.length >= 5) {
        //    $(this).val(values);
        //    return
        //}
        //else if (getValue.indexOf(".") != -1 && getValue.length >= 6) {
        //    $(this).val(values);
        //    return
        //}
        var $value = $(this);
        var reg = /^\d+(\.\d{0,1})?/,
            vals = $value.val().match(reg);

        $value.val(vals ? vals[0] : "");
    });
    $("#AT_delayStartTime,#AT_delayEndTime,#planExtension_delayStartTime,#planExtension_delayEndTime,#EH-customStartTime,#EH-customEndTime,#planExtension_deductionNum,#AT_deductionNum,#maxValue,#minValue,#maxAverage,#minAverage,#EH-deductionNumRight").bind("blur", function () {
        var getValue = $(this).val();
        var values = $(this).val().substring(0, $(this).val().length - 1);
        if (getValue.substring($(this).val().length - 1, $(this).val().length) == ".") {
            $(this).val(values);
        }
    })
    ///^-?[0-9]\d*$/
    $("#planExtension_deductionNum,#AT_deductionNum,#maxValue,#minValue,#maxAverage,#minAverage,#EH-deductionNumRight").bind("input", function () {
        //var reg = /^\d+(\.\d{0,1}\d{0,1}^[+-])?$/;
        //var values = $(this).val().substring(0, $(this).val().length - 1);
        //var getValue = $(this).val();
        //if (reg.test(getValue) == false) {
        //    $(this).val(values);
        //    return
        //}
        //if (getValue.indexOf(".") == -1 && getValue.length >= 9) {
        //    $(this).val(values);
        //    return
        //}
        //else if (getValue.indexOf(".") != -1 && getValue.length >= 10) {
        //    $(this).val(values);
        //    return
        //}


        var $value = $(this);
        var reg = /^\d+(\.\d{0,1})?/,
            vals = $value.val().match(reg);

        $value.val(vals ? vals[0] : "");
    });
    //有效工时加载
    function EffectiveHoursLoad() {
        $("#EH-customStartTime,#EH-customEndTime,#EH-deductionNumLeft,#EH-deductionNumRight").val("");
        ruleArray.length = 0;
        deleteIdArray.length = 0;
        $("#EffectiveHours_load").empty();
        var $EffectiveHours_load = getLoadingPosition("#con_EffectiveHours");     //显示load层
        $.ajax({
            type: "post",
            url: "/DelayRule/GetValueIncentive",
            dataType: "json",
            data: { type: 1 },
            complete: rcHandler(function () {
                $EffectiveHours_load.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                if (data != null) {
                    $(".incentiveType:eq(" + data.incentiveType + ")").trigger("click");
                    $("#standardTime").val(data.standardTime);
                    $("#maxValueType").text(data.maxValueType = 1 ? "金额" : "比例");
                    $("#maxValue").val(data.maxValue);
                    $("#maxAverage").val(data.maxAverage);
                    $("#minValueType").text(data.minValueType = 1 ? "金额" : "比例");
                    $("#minValue").val(data.minValue);
                    $("#minAverage").val(data.minAverage);
                }
            })
        })
      //  var $planExtension_load = getLoadingPosition("#EffectiveHours_load");     //显示load层
        $.ajax({
            type: "post",
            url: "/DelayRule/GetValueCustList",
            dataType: "json",
            complete: rcHandler(function () {
               // $planExtension_load.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {//data.custom
                HasList = data;
                $.each(data, function (i, v) {
                    var $row = $("<div class='row' style='margin-bottom:10px'></div>");
                    var $cell = $("<div  class='cell'></div>");
                    var $content = $(" <div class='col-xs-1'></div><div class='col-xs-2' ><span class='span-control'>" + (v.customType == "1" ? "不足" : "超出") + "</span></div>" +
                        "<div class='col-xs-3'><span class='span-control'>工时:</span><span class='span-control'>" + v.customStartTime + "-" + v.customEndTime + "</span></div> "
                        + "<div class='col-xs-2'> <span class='span-control'>" + (v.deductionMode == 1 ? "金额" : "比例") + "</span></div> "
                        + "<div class='col-xs-2'> <span  class='span-control'>" + v.deductionNum + (v.deductionMode == 1 ? "元" : "%") + "</span></div> "
                        + "<div class='col-xs-2'> </div>");
                    var $operate = $("<div class='operate'> </div>");
                    var $delete = $("<a class='operate-delete'term='" + v.customId + "'>删除</a>");
                    $delete.click(function () {
                        if ($("#EffectiveHours_load").hasClass("disabled") == false) {
                            $this = $(this);
                            ncUnits.confirm({
                                title: '提示',
                                html: '你确认要删除这条公式吗？',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    $this.closest(".row").remove();
                                    deleteIdArray[deleteIdArray.length] = $this.attr("term");
                                },
                                no: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                }
                            })
                        }
                 
                    })
                    $operate.append($delete);
                    $cell.append([$content, $operate]);
                    $row.hover(function () {
                        $(this).find(".operate").css("display", "block");
                    }, function () {
                        $(this).find(".operate").css("display", "none");
                    });
                    $row.append($cell);
                    $("#EffectiveHours_load").append($row);
                })
            })
        })
    }
    //审批超时加载
    function ApprovalTimeoutLoad() {
        $("#AT_delayStartTime,#AT_delayEndTime,#AT_deductionNum").val("");
        ruleArray.length = 0;
        deleteIdArray.length = 0;
        $("#AT_load").empty();
        var $AT_load = getLoadingPosition("#AT_load");     //显示load层
        $.ajax({
            type: "post",
            url: "/DelayRule/GetDelay",
            dataType: "json",
            data: { type: 2 },
            complete: rcHandler(function () {
                $AT_load.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                TimeList = data;
                $.each(data, function (i, v) {
                    var $row = $("<div class='row' style='margin-bottom:10px'></div>");
                    var $cell = $("<div  class='cell'></div>");
                    var $content = $(" <div class='col-xs-1'></div><div class='col-xs-2' ><span class='span-control'>审批超时</span></div>" +
                        "<div class='col-xs-3'><span class='span-control'>" + v.delayStartTime + "-" + v.delayEndTime + "</span></div> "
                        + "<div class='col-xs-2'> <span class='span-control'>" + (v.deductionMode == 1 ? "金额" : "比例") + "</span></div> "
                        + "<div class='col-xs-2'> <span  class='span-control'>" + v.deductionNum + (v.deductionMode == 1 ? "元" : "%") + "</span></div> "
                        + "<div class='col-xs-2'> </div>");
                    var $operate = $("<div class='operate'> </div>");
                    var $delete = $("<a class='operate-delete'term='" + v.ruleId + "'>删除</a>");
                    $delete.click(function () {
                        $this = $(this);
                        ncUnits.confirm({
                            title: '提示',
                            html: '你确认要删除这条公式吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $this.closest(".row").remove();
                                deleteIdArray[deleteIdArray.length] = $this.attr("term");
                            },
                            no: function (layer_confirm) {
                                layer.close(layer_confirm);
                            }
                        })                    
                    })
                    $operate.append($delete);
                    $cell.append([$content, $operate]);
                    $row.hover(function () {
                        $(this).find(".operate").css("display", "block");
                    }, function () {
                        $(this).find(".operate").css("display", "none");
                    });
                    $row.append($cell);
                    $("#AT_load").append($row);
                })
            })
        })
    }
    //计划超时加载
    function planExtensionLoad() {
        $("#planExtension_delayStartTime,#planExtension_delayEndTime,#planExtension_deductionNum").val("");
        ruleArray.length = 0;
        deleteIdArray.length = 0;
        $("#planExtension_load").empty();
        var $planExtension_load = getLoadingPosition("#planExtension_load");     //显示load层
        $.ajax({
            type: "post",
            url: "/DelayRule/GetDelay",
            dataType: "json",
            data: { type: 1 },
            complete: rcHandler(function () {
                $planExtension_load.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                TimeList=data;
                $.each(data, function (i, v) {
                    var $row = $("<div class='row' style='margin-bottom:10px'></div>");
                    var $cell = $("<div  class='cell'></div>");
                    var $content = $(" <div class='col-xs-1'></div><div class='col-xs-2' ><span class='span-control'>计划延期</span></div>" +
                         "<div class='col-xs-3'><span class='span-control'>" + v.delayStartTime + "-" + v.delayEndTime + "</span></div> "
                       + "<div class='col-xs-2'> <span class='span-control'>" + (v.deductionMode == 1 ? "金额" : "比例") + "</span></div> "
                       + "<div class='col-xs-2'> <span  class='span-control'>" + v.deductionNum + (v.deductionMode == 1 ? "元" : "%") + "</span></div> "
                       + "<div class='col-xs-2'> </div>");
                    var $operate = $("<div class='operate'> </div>");
                    var $delete = $("<a class='operate-delete'term='" + v.ruleId + "'>删除</a>");
                    $delete.click(function () {
                        $this = $(this);
                        ncUnits.confirm({
                            title: '提示',
                            html: '你确认要删除这条公式吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $this.closest(".row").remove();
                                deleteIdArray[deleteIdArray.length] = $this.attr("term");
                            },
                            no: function (layer_confirm) {
                                layer.close(layer_confirm);
                            }
                        })
                      
                    })
                    $operate.append($delete);
                    $cell.append([$content, $operate]);
                    $row.hover(function () {
                        $(this).find(".operate").css("display", "block");
                    }, function () {
                        $(this).find(".operate").css("display", "none");
                    });
                    $row.append($cell);
                    $("#planExtension_load").append($row);
                })
            })
        })
    }
    $("#EffectiveHours-add").click(function () {
        if ($("#EH-customStartTime").val() == "") {
            ncUnits.alert("请输入工时")
            return;
        }
        if ($("#EH-customEndTime").val() == "") {
            ncUnits.alert("请输入工时")
            return;
        }
     
        if ( $("#EH-customStartTime").val() > $("#EH-customEndTime").val()) {
         
            ncUnits.alert("开始时间不得小于结束时间")
            return;
        }
        if ($("#EH-deductionNumRight").val() == "") {
            ncUnits.alert("请输入具体金额")
            return;
        }
        if ($("#EH-deductionNumLeft").val() == "") {
            ncUnits.alert("请输入金额的正负号")
            return;
        }
        var customInfo = {
            customId: null,            //规则ID
            customType: null,          //规则类型
            customStartTime: null,  //规定开始时间
            customEndTime: null,   //规定结束时间
            deductionMode: null,       //方式
            deductionNum: null     //金额
        }
        var ishas = false;
        customInfo.customId = 'r' + new Date().getTime();
        customInfo.customType = $("#EH-customType").attr("term");
        customInfo.customStartTime = $("#EH-customStartTime").val();
        customInfo.customEndTime = $("#EH-customEndTime").val();
        customInfo.deductionMode = $("#EH-deductionMode").attr("term");
        customInfo.deductionNum = $("#EH-deductionNumLeft").val() + $("#EH-deductionNumRight").val();
      
 
        $.each(HasList, function (i, v) {
            
            if ((parseInt(v.customStartTime) >= parseInt(customInfo.customStartTime) && v.customStartTime <= parseInt(customInfo.customEndTime)) || (v.customEndTime <= parseInt(customInfo.customStartTime) && v.customEndTime >= parseInt(customInfo.customEndTime))) {
                if (v.customType == customInfo.customType) {
                    ishas = true;
                }
            }
        });
        if (ishas == false) {
            ruleArray.push(customInfo);
            HasList.push(customInfo);
            var $row = $("<div class='row' style='margin-bottom:10px'></div>");
            var $cell = $("<div  class='cell'></div>");
            var $content = $(" <div class='col-xs-1'></div><div class='col-xs-2' ><span class='span-control'>" + (customInfo.customType == "1" ? "不足" : "超出") + "</span></div>" +
                "<div class='col-xs-3'><span class='span-control'>工时:</span><span class='span-control'>" + customInfo.customStartTime + "-" + customInfo.customEndTime + "</span></div> "
                + "<div class='col-xs-2'> <span class='span-control'>" + (customInfo.deductionMode == 1 ? "金额" : "比例") + "</span></div> "
                + "<div class='col-xs-2'> <span  class='span-control'>" + customInfo.deductionNum + (customInfo.deductionMode == 1 ? "元" : "%") + "</span></div> "
                + "<div class='col-xs-2'> </div>");
            var $operate = $("<div class='operate'> </div>");
            var $delete = $("<a class='operate-delete'term='" + customInfo.ruleId + "'>删除</a>");
            $delete.click(function () {
                if ($("#EffectiveHours_load").hasClass("disabled") == false) {
                    ncUnits.confirm({
                        title: '提示',
                        html: '你确认要删除这条公式吗？',
                        yes: function (layer_confirm) {
                            $this = $(this);
                            layer.close(layer_confirm);
                            for (var i = 0; i < ruleArray.length; i++) {
                                if (ruleArray[i].ruleId == $this.attr("term")) {
                                    ruleArray.splice(i, 1);
                                }
                            }
                            $this.closest(".row").remove();
                        },
                        no: function (layer_confirm) {
                            layer.close(layer_confirm);
                        }
                    });
                }

            });
            $row.hover(function () {
                $(this).find(".operate").css("display", "block");
            }, function () {
                $(this).find(".operate").css("display", "none");
            });
            $operate.append($delete);
            $cell.append([$content, $operate]);
            $row.append($cell);
            $("#EffectiveHours_load").append($row);

        } else {
            ncUnits.alert("时间范围已存在");
        }
     
        });
    $("#AT_Add").click(function () {
        if ($("#AT_delayStartTime").val() == "") {
            ncUnits.alert("请输入开始时间")
            return;
        }
        if ($("#AT_delayEndTime").val() == "") {
            ncUnits.alert("请输入结束时间")
            return;
        }
        if ($("#AT_delayStartTime").val() > $("#AT_delayEndTime").val()) {
            ncUnits.alert("开始时间不能早于结束时间")
            return;
        }
        if ($("#AT_deductionNum").val() == "") {
            ncUnits.alert("请输入数量")
            return;
        }
       
        var ruleInfo = {
            ruleId: null,                  //规则ID
            ruleType: null,                //规则类型
            delayStartTime: null,      //规定开始时间
            delayEndTime: null,        //规定结束时间
            deductionMode: null,           //方式
            deductionNum: null         //金额
        }
        var flagg = false;
        ruleInfo.ruleId = 'r' + new Date().getTime();
        ruleInfo.delayStartTime = $("#AT_delayStartTime").val();
        ruleInfo.ruleType = 2;
        ruleInfo.delayEndTime = $("#AT_delayEndTime").val();
        ruleInfo.deductionMode = $("#AT_deductionMode").attr("term");
        ruleInfo.deductionNum = $("#AT_deductionNum").val(); 
        $.each(TimeList, function (i, v) { 
            if ((v.delayStartTime >=parseInt(ruleInfo.delayStartTime) && v.delayStartTime <= parseInt(ruleInfo.delayEndTime)) || (v.delayEndTime <= parseInt(ruleInfo.delayStartTime) && v.delayEndTime >= parseInt(ruleInfo.delayEndTime))) {
                flagg = true;
            }
        });
        if (flagg == false) {
            TimeList.push(ruleInfo);
            ruleArray.push(ruleInfo);
            var $row = $("<div class='row' style='margin-bottom:10px'></div>");
            var $cell = $("<div  class='cell'></div>");
            var $content = $(" <div class='col-xs-1'></div><div class='col-xs-2' ><span class='span-control'>审批超时</span></div>" +
                "<div class='col-xs-3'><span class='span-control'>" + ruleInfo.delayStartTime + "-" + ruleInfo.delayEndTime + "</span></div> "
                + "<div class='col-xs-2'> <span class='span-control'>" + (ruleInfo.deductionMode == 1 ? "金额" : "比例") + "</span></div> "
                + "<div class='col-xs-2'> <span  class='span-control'>" + ruleInfo.deductionNum + (ruleInfo.deductionMode == 1 ? "元" : "%") + "</span></div> "
                + "<div class='col-xs-2'> </div>");
            var $operate = $("<div class='operate'> </div>");
            var $delete = $("<a class='operate-delete'term='" + ruleInfo.ruleId + "'>删除</a>");
            $delete.click(function () {
                $this = $(this);
                ncUnits.confirm({
                    title: '提示',
                    html: '你确认要删除这条公式吗？',
                    yes: function (layer_confirm) {
                        layer.close(layer_confirm);
                        for (var i = 0; i < ruleArray.length; i++) {
                            if (ruleArray[i].ruleId == $this.attr("term")) {
                                ruleArray.splice(i, 1);
                            }
                        }
                        $this.closest(".row").remove();
                    },
                    no: function (layer_confirm) {
                        layer.close(layer_confirm);
                    }
                })

            });
            $row.hover(function () {
                $(this).find(".operate").css("display", "block");
            }, function () {
                $(this).find(".operate").css("display", "none");
            });
            $operate.append($delete);
            $cell.append([$content, $operate]);
            $row.append($cell);
            $("#AT_load").append($row);
        } else {
            ncUnits.alert("时间范围已存在");
        }
    });
    $("#planExtension_Add").click(function () {
        if ($("#planExtension_delayStartTime").val() == "") {
            ncUnits.alert("请输入开始时间")
            return;
        }
        if ($("#planExtension_delayEndTime").val() == "") {
            ncUnits.alert("请输入结束时间")
            return;
        }
        if ($("#planExtension_delayStartTime").val() > $("#planExtension_delayEndTime").val()) {
            ncUnits.alert("开始时间不能早于结束时间")
            return;
        }
        if ($("#planExtension_deductionNum").val() == "") {
            ncUnits.alert("请输入数量")
            return;
        }
        var ishasFlag = false;
        var ruleInfo = {
            ruleId: null,                  //规则ID
            ruleType: null,                //规则类型
            delayStartTime: null,      //规定开始时间
            delayEndTime: null,        //规定结束时间
            deductionMode: null,           //方式
            deductionNum: null         //金额
        }
        ruleInfo.ruleId = 'r' + new Date().getTime();
        ruleInfo.ruleType = 1;
        ruleInfo.delayStartTime = $("#planExtension_delayStartTime").val();
        ruleInfo.delayEndTime = $("#planExtension_delayEndTime").val();
        ruleInfo.deductionMode = $("#planExtension_deductionMode").attr("term");
        ruleInfo.deductionNum = $("#planExtension_deductionNum").val();

        $.each(TimeList, function (i, v) {
            if ((v.delayStartTime >= parseInt(ruleInfo.delayStartTime) && v.delayStartTime <= parseInt(ruleInfo.delayEndTime)) || (v.delayEndTime <= parseInt(ruleInfo.delayStartTime) && v.delayEndTime >= parseInt(ruleInfo.delayEndTime))) {
                ishasFlag = true;
            }
        });
        if (ishasFlag == false) {
            TimeList.push(ruleInfo);
            ruleArray.push(ruleInfo);
            var $row = $("<div class='row' style='margin-bottom:10px'></div>");
            var $cell = $("<div  class='cell'></div>");
            var $content = $(" <div class='col-xs-1'></div><div class='col-xs-2' ><span class='span-control'>计划延期</span></div>" +
                "<div class='col-xs-3'><span class='span-control'>" + ruleInfo.delayStartTime + "-" + ruleInfo.delayEndTime + "</span></div> "
                + "<div class='col-xs-2'> <span class='span-control'>" + (ruleInfo.deductionMode == 1 ? "金额" : "工资") + "</span></div> "
                + "<div class='col-xs-2'> <span  class='span-control'>" + ruleInfo.deductionNum + (ruleInfo.deductionMode == 1 ? "元" : "%") + "</span></div> "
                + "<div class='col-xs-2'> </div>");
            var $operate = $("<div class='operate'> </div>");
            var $delete = $("<a class='operate-delete'term='" + ruleInfo.ruleId + "'>删除</a>");
            $delete.click(function () {
                $this = $(this);
                ncUnits.confirm({
                    title: '提示',
                    html: '你确认要删除这条公式吗？',
                    yes: function (layer_confirm) {
                        layer.close(layer_confirm);
                        for (var i = 0; i < ruleArray.length; i++) {
                            if (ruleArray[i].ruleId == $this.attr("term")) {
                                ruleArray.splice(i, 1);
                            }
                        }
                        $this.closest(".row").remove();

                    },
                    no: function (layer_confirm) {
                        layer.close(layer_confirm);
                    }
                })

            });
            $row.hover(function () {
                $(this).find(".operate").css("display", "block");
            }, function () {
                $(this).find(".operate").css("display", "none");
            });
            $operate.append($delete);
            $cell.append([$content, $operate]);
            $row.append($cell);
            $("#planExtension_load").append($row);
        } else {
            ncUnits.alert("时间范围已存在")
        }
    });
    //下拉框控制
    $("#planExtension_deductionMode,#AT_deductionMode,#EH-deductionMode,#maxValueType,#minValueType").closest(".dropdown").find("ul a").click(function () {
        var x = $(this).parents("ul").prev().find("span:eq(0)");
        x.text($(this).text());
        var term = $(this).attr("term");
        x.attr("term", term);
        if ($(this).parents("ul").prev().find("span:eq(0)").attr("id") == "planExtension_deductionMode") {
            if (term == 1) {
                $("#planExtension_NumUnit").text("元")
            }
            else {
                $("#planExtension_NumUnit").text("%")
            }
        }
        else if ($(this).parents("ul").prev().find("span:eq(0)").attr("id") == "AT_deductionMode") {
            if (term == 1) {
                $("#AT_NumUnit").text("元")
            }
            else {
                $("#AT_NumUnit").text("%")
            }
        }
        else if ($(this).parents("ul").prev().find("span:eq(0)").attr("id") == "EH-deductionMode") {
            if (term == 1) {
                $("#EH-deductionUnit").text("元")
            }
            else {
                $("#EH-deductionUnit").text("%")
            }
        }
        else if ($(this).parents("ul").prev().find("span:eq(0)").attr("id") == "maxValueType") {
            if (term == 1) {
                $("#maxValue-Unit").text("元");
                $("#maxAverage-Unit").text("元");
            }
            else {
                $("#maxValue-Unit").text("%");
                $("#maxAverage-Unit").text("%");
            }
        }
        else if ($(this).parents("ul").prev().find("span:eq(0)").attr("id") == "minValueType") {
            if (term == 1) {
                $("#minValue-Unit").text("元");
                $("#minAverage-Unit").text("元");
            }
            else {
                $("#minValue-Unit").text("%");
                $("#minAverage-Unit").text("%");
            }
        }


    });

    $("#EH-customType").closest(".dropdown").find("ul a").click(function () {
        dropDownEvent($(this));
    });
    function dropDownEvent(value) {
        var x = $(value).parents("ul").prev().find("span:eq(0)");
        x.text($(value).text());
        var term = $(value).attr("term");
        x.attr("term", term);
    }


    /*---------------------------------------目标新建 ------------------------*/

    var operateFlag = null;//区分两种提交
    var loadFormaluFlag = true;
    var objectiveRuleSureFlag = true;
   
    var  loginId=null;
    timeThree(".new-objectiveValue-date", null, null, null, null);
    timeThree(".new-expectedValue-date", null, null, null, null);

    $("#new-startTime, #new-endTime ,#new-alarmTime").click(function () {
        var id = "#" + $(this).attr("id");
        timeThree(id, null, null, null, null);
    });


    $("#new-weight").blur(function () {
        var value = $(this).val();
        var reg = /^-?\d+\.$/;
        if (reg.test(value)) {
            value = value.substr(0, value.length - 1);
        }
        if (value.indexOf("%") < 0 && value.length > 0) {
            value = value + "%";
        }
        $(this).val(value);
    });

    $("#new-weight").focus(function () {
        var value = $(this).val();
        if (value.indexOf("%") > 0) {
            value = value.substring(0, value.length - 1);
        }
        $(this).val(value);
    });
    $("#expectMoney,#targetMoney,#objectBonus,#new-objectiveValue-money,#new-expectedValue-money ,#new-weight,#new-bonus").bind("input keydown", function () {
        controlInput($(this));
    });
    var FormulaArray = new Array();//全局变量存储公式
    var formulaEditOrAdd = null;//标记当前为公式编辑还是添加状态
    var formulaNum = 0;//公式编号
    var formulaSetInput = new Array();//存放当前编辑的公式
    
    $("#new-checkType-dropdown ul li a").click(function () {
        if ($(this).attr("term") == 1) {
            $("#new-objectiveValue-money-group").css("display", "block");
            $("#new-expectedValue-money-group").css("display", "block");
            $("#new-objectiveValue-date-group").css("display", "none");
            $("#new-expectedValue-date-group").css("display", "none");
            $("#new-objectiveValue-number").css("display", "none");
            $("#new-expectedValue-number").css("display", "none");
        }
        else if ($(this).attr("term") == 2) {
            $("#new-objectiveValue-date-group").css("display", "block");
            $("#new-expectedValue-date-group").css("display", "block");
            $("#new-objectiveValue-money-group").css("display", "none");
            $("#new-expectedValue-money-group").css("display", "none");
            $("#new-objectiveValue-number").css("display", "none");
            $("#new-expectedValue-number").css("display", "none");
        }
        else if ($(this).attr("term") == 3) {
            $("#new-objectiveValue-number").css("display", "block");
            $("#new-expectedValue-number").css("display", "block");
            $("#new-objectiveValue-date-group").css("display", "none");
            $("#new-expectedValue-date-group").css("display", "none");
            $("#new-objectiveValue-money-group").css("display", "none");
            $("#new-expectedValue-money-group").css("display", "none");
        }
        var x = $(this).parents("ul").prev().find("span:eq(0)");
        x.text($(this).text());
        var term = $(this).attr("term");
        x.attr("term", term);
    });



    //根据考核类型控制指标值和理想值
    $("#new-objectiveValue,#new-expectedValue").click(function () {
        if ($("#new-checkType").attr("term") == "") {
            ncUnits.alert("请先选择考核类型");
        }
    });


    //点击切换 目标和目标规则
    $(".nav-item").click(function () {
        $(this).addClass("modal-titleClick");
        $(this).siblings(".nav-item").removeClass("modal-titleClick");
    });


    //点击目标规则，隐藏弹出框左侧部分
    $("#ModalObjectiveRuleTab").click(function () {
        $(".modal-right-content").css("display", "none");
    });
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

    //确认人
    $("#new-confirmUser").focus(function () {
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
    //ztree下列表加载
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
    /*右侧弹出框事件 结束*/


    $("#newModal-Save").click(function () {
        newModalDataSave(2);
    });
    $("#newModal-Submit").click(function () {
        newModalDataSave(1);//提交时
    });
    //新建目标数据保存
    function newModalDataSave(flag) {
        var objectiveData = {
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
            ncUnits.alert("结束时间不能为空!");
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
        
        if (flag == 1) {//提交时
            //目标名称正确性检验
            var name = $.trim($("#new-objectiveName").val());
            if (name == "") {
                ncUnits.alert("目标名称不能为空!");
                return;
            }
            var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
            if (name.indexOf('null') >= 0 || name.indexOf('NULL') >= 0 || name.indexOf('&nbsp') >= 0 || reg.test(name) || name.indexOf('</') >= 0) {
                ncUnits.alert("目标名称存在非法字符!");
                return;
            }

            //目标基数正确性检验
            if ($("#new-bonus").attr("term") == "") {
                ncUnits.alert("目标基数不能为空!");
                return;
            }
            //目标权重正确性检验
            if ($("#new-weight").attr("term") == "") {
                ncUnits.alert("目标对象不能为空!");
                return;
            }
            //指标值和理想值正确性检验
            if (($("#new-checkType").attr("term") == 1 && $("#new-expectedValue-money").val() == "") || ($("#new-checkType").attr("term") == 2 && $("#new-expectedValue-date").val() == "") || ($("#new-checkType").attr("term") == 3 && $("#new-expectedValue-number").val() == "")) {
                ncUnits.alert("指标值不能为空!");
                return;
            }
            if (($("#new-checkType").attr("term") == 1 && $("#new-objectiveValue-money").val() == "") || ($("#new-checkType").attr("term") == 2 && $("#new-objectiveValue-date").val() == "") || ($("#new-checkType").attr("term") == 3 && $("#new-objectiveValue-number").val() == "")) {
                ncUnits.alert("理想值不能为空!");
                return;
            }
            //时间正确性检验           
            if ($("#new-alarmTime").val() == "") {
                ncUnits.alert("警戒时间不能为空!");
                return;
            }

            var name = $.trim($("#new-description").val());
            if (name.indexOf('null') >= 0 || name.indexOf('NULL') >= 0 || name.indexOf('&nbsp') >= 0 || reg.test(name) || name.indexOf('</') >= 0) {
                ncUnits.alert("备注项目存在非法字符!");
                return;
            }

            //公式正确性检验
            if (loginId == $("#new-confirmUser").attr("term")) {
                if ($(".formula:eq(0)").prop("checked")) {
                    objectiveData.formula = 0;
                }
                else if ($(".formula:eq(1)").prop("checked")) {
                    if ($("#maxValue").val("") == "") {
                        ncUnits.alert("最大奖励公式输入框不能为空!");
                        return;
                    }
                    if ($("#minValue").val("") == "") {
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
                objectiveData.formula = null;
                objectiveData.objectiveFormula.length = 0;
            }
        }
        objectiveData.objectiveName = $("#new-objectiveName").val();
        objectiveData.objectiveType = $("#new-objectiveType").attr("term");
        objectiveData.bonus = $("#new-bonus").val();
        objectiveData.weight = $("#new-weight").val().split("%", 1)[0];

        objectiveData.checkType = $("#new-checkType").attr("term");
        if (objectiveData.checkType == 1) {
            objectiveData.expectedValue = $("#new-expectedValue-money").val();
            objectiveData.objectiveValue = $("#new-objectiveValue-money").val();
        }
        else if (objectiveData.checkType == 2) {
            objectiveData.expectedValue = $("#new-expectedValue-date").val();
            objectiveData.objectiveValue = $("#new-objectiveValue-date").val();
        }
        else if (objectiveData.checkType == 3) {
            objectiveData.expectedValue = $("#new-expectedValue-number").val();
            objectiveData.objectiveValue = $("#new-objectiveValue-number").val();
        }
        objectiveData.description = $("#new-description").val();
        objectiveData.startTime = $("#new-startTime").val();
        objectiveData.endTime = $("#new-endTime").val();
        objectiveData.alarmTime = $("#new-alarmTime").val();

        objectiveData.responsibleOrg = $("#new-objectiveType").val();

        objectiveData.responsibleUser = $("#new-responsibleUser").attr("term");
        objectiveData.confirmUser = $("#new-confirmUser").attr("term");
        if ($(".formula:eq(0)").prop("checked")) {
            objectiveData.formula = 0;
        }
        else if ($(".formula:eq(1)").prop("checked")) {
            objectiveData.formula = 1;
            objectiveData.maxValue = $("#maxValue").val();
            objectiveData.minValue = $("#minValue").val();
        }
        else if ($(".formula:eq(2)").prop("checked")) {
            if (objectiveRuleSureFlag == true) {
                objectiveData.formula = 2;
                objectiveData.objectiveFormula.length = 0;
                for (var i = 0; i < FormulaArray.length; i++) {
                    if (FormulaArray[i].formulaId == "z") {
                        FormulaArray[i].formulaId = null;
                    }
                    objectiveData.objectiveFormula.push(jQuery.extend(true, {}, FormulaArray[i]));
                }
            }
            else {
                ncUnits.alert("公式没有保存!");
                return;
            }
        }
        if (objectiveData.confirmUser != loginId && objectiveData.formula != null) {
            objectiveData.formula = null;
            objectiveData.objectiveFormula.length = 0;
        }      
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/NewObjective",
            dataType: "json",
            data: { flag: flag, data: JSON.stringify(objectiveData) },
            success: rsHandler(function () {
                ncUnits.alert("新建目标成功");
                $("#objectiveNew_modal").modal("hide");
                statusCount();
                loadObjectList();
                drawPlanProgress();
            })
        });
    }

    $("#new-objectiveValue-date,#new-expectedValue-date,#new-startTime,#new-endTime,#new-alarmTime").siblings("a").click(function () {
        $(this).siblings("input").trigger("click");
    });

    $("#new-responsibleUser,#new-confirmUser").siblings("a").click(function () {
        $(this).siblings("input").trigger("focus");
    });




    /*------------------------------------目标规则   开始----------------------------------------------------------------*/

    /*公式设置 开始*/

    $("#formula_modal_time").click(function () {
        var start = {
            elem: '#formula_modal_time_hidden',
            format: 'YYYYMMDD',
            max: '2099-06-16', //最大日id期
            istime: false,
            istoday: false,
            choose: function (datas) {
                end.min = datas; //开始日选好后，重置结束日的最小日期
                end.start = datas //将结束日的初始值设定为开始日
                $("#formula_modal_monitor").text($("#formula_modal_monitor").text() + datas);
                formulaSetInput.push(datas);
            },
            clear: function () {
                end.min = undefined;
            }
        };
        var end = {
            elem: '#formula_modal_time_hidden',
            format: 'YYYYMMDD',
            max: '2099-06-16',
            istime: false,
            istoday: false,
            choose: function (datas) {
                start.max = datas; //结束日选好后，重置开始日的最大日期
            },
            clear: function () {
                start.max = undefined;
            }
        };
        laydate.reset();
        laydate(start);
        laydate(end);
    })



    var formulaSetCheck = ["+", "-", "*", ">", "/", "≥", "<", "≤"];
    var formulaSetCheck3 = ["(", ")", "+", "-", "*", ">", "/", "≥", "<", "≤"];
    var rp1 = /["." "且" "或"]/;

    var formulaSetCheck1 = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "警戒时间", "权重", "奖励基数", "数字"];
    var formulaSetCheck2 = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "警戒时间", "权重", "奖励基数", "数字", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];

    $(".areabox a").click(function () {
        if ($(this).attr("id") == "formula_modal_time") {

        }
        else {
            $("#formula_modal_monitor").text($("#formula_modal_monitor").text() + $(this).attr("value"));
            formulaSetInput.push($(this).attr("value"));
        }

    });
    $("#formula_modal_backspace").click(function () {
        formulaSetInput.pop($(this).attr("value"));
        $("#formula_modal_monitor").text(formulaSetInput.join(""));
    });
    function formulaCheck(data, array) {
        for (var i = 0; i < array.length; i++) {
            if (array[i] == data) {
                return true;
            }
        }
    }
    $("#formula_modal_submit").click(function () {
        var leftBracketFlag = 0;
        var rightBracketFlag = 0;
        if (formulaSetInput.length == 0) {
            ncUnits.alert("您还没有输入公式");
            return;
        }
        if (rp1.test(formulaSetInput[0]) || (formulaCheck(formulaSetInput[0], formulaSetCheck) == true)) {
            //公式开头是运算符
            ncUnits.alert("公式错误");
            return;
        }
        if (rp1.test(formulaSetInput[formulaSetInput.length - 1]) || (formulaCheck(formulaSetInput[formulaSetInput.length - 1], formulaSetCheck) == true)) {
            //公式结尾是运算符
            ncUnits.alert("公式错误");
            return;
        }
        for (var i = 0; i < formulaSetInput.length; i++) {
            if (formulaSetInput[i] == ")") {
                if (leftBracketFlag == 0) {
                    //公式中先出现)
                    ncUnits.alert("公式括号不匹配");
                    return;
                }
                rightBracketFlag++;
            }
            if (formulaSetInput[i] == "(") {
                leftBracketFlag++;
            }
            if ((i + 1) < formulaSetInput.length && formulaCheck(formulaSetInput[i], formulaSetCheck) == true && formulaCheck(formulaSetInput[i + 1], formulaSetCheck3) == true) {
                //公式中两个操作符连续出现
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput.length && formulaSetInput[i] == "(" && (formulaCheck(formulaSetInput[i + 1], formulaSetCheck) == true || rp1.test(formulaSetInput[i + 1]))) {
                //公式中（后面连续出现操作符
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput.length && formulaSetInput[i] == ")" && formulaSetInput[i + 1] == (".")) {
                //公式中)后面出现.
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput.length && (((formulaSetInput[i] == "且" || formulaSetInput[i] == "或") && formulaSetInput[i + 1] == (formulaSetInput[i] == "且" || formulaSetInput[i] == "或")))) {
                //公式中且与或连续出现
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput.length && ((formulaSetInput[i] == ")" && formulaSetInput[i + 1] == "(") || (formulaSetInput[i] == "(" && formulaSetInput[i + 1] == ")"))) {
                //公式中左右括号连续出现
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput.length && (formulaCheck(formulaSetInput[i], formulaSetCheck1) == true && formulaCheck(formulaSetInput[i + 1], formulaSetCheck2) == true || formulaCheck(formulaSetInput[i], formulaSetCheck2) == true && formulaCheck(formulaSetInput[i + 1], formulaSetCheck1) == true)) {
                //公式中两个操作数连续出现(不包括两个数字连续出现)
                ncUnits.alert("公式错误");
                return;
            }

            var rep = /[0-9]/;
            if ((i - 1) >= 0 && formulaSetInput[i] == "." && rep.test(formulaSetInput[i - 1]) == false) {
                //公式中小数点出啊先在非数字的操作数后面
                ncUnits.alert("公式错误");
                return;
            }
        }
        if (leftBracketFlag != rightBracketFlag) {
            ncUnits.alert("公式括号不匹配");
            return;
        }
        objectiveRuleSureFlag = false;
        $("#formula_modal").modal('hide');
        var number = null;
        var rep = /^\d$/;

        var formulaSet = new Array();//将输入数据进行处理
        for (var i = 0; i < formulaSetInput.length; i++) {
            if (rep.test(formulaSetInput[i])) {
                if (number == null) {
                    number = formulaSetInput[i];

                }
                else {
                    number = number + formulaSetInput[i];
                }
                if (i + 1 == formulaSetInput.length) {
                    formulaSet.push(number);
                }
            }
            else if (number != null && rep.test(formulaSetInput[i]) == false) {
                formulaSet.push(number);
                formulaSet.push(formulaSetInput[i]);
                number = null;
            }
            else if (rep.test(formulaSetInput[i]) == false) {
                formulaSet.push(formulaSetInput[i]);
            }
        }

        if (formulaEditOrAdd != null) {
            for (var i = 0; i < FormulaArray.length; i++) {
                if (FormulaArray[i].formulaNum == formulaEditOrAdd) {
                    FormulaArray.splice(i, 1);
                    i = i - 1;
                }
            }
            //如果是修改公式，先删除数据中的当前公式
        }
        else {
            formulaNum = 0;
            for (var i = 0; i < FormulaArray.length; i++) {
                if (FormulaArray[i].formulaNum > formulaNum) {
                    formulaNum = FormulaArray[i].formulaNum;
                }
            }
            formulaNum++;
        }

        if (formulaEditOrAdd == null) {//如果是新建一个公式，则增添一行记录
            var $tr = $("<tr term='" + formulaNum + "'></tr>");
            var $formulaContentTd = $("<td style='width:35%;'> <span class='formula-content'title='" + formulaSetInput.join("") + "'>" + formulaSetInput.join("") + "</span></td>");
            var $dropdownTd = $("<td style='width:16%;'><div class='dropdown'><span class='dropdown-toggle' data-toggle='dropdown'  data-delay='50' role='button' aria-expanded='false'>" +
                "<span term='8' class='new-numValueFlag'>奖励基数</span> <span class='upPic'></span> </span>" +
                "<ul class='dropdown-menu' role='menu'><li><a href='#' term='8'>奖励基数</a></li><li class='divider short'></li><li><a href='#' term='9'>固定奖励</a></li> </ul> </div></td>");

            var $Td = $("<td style='width:28%;'class='choose'><div class='input-group group1'> <span>奖励基数*</span><input type='text' class='new-numValue form-control' maxlength='10'> </div>" +
                "<div class='input-group group2' style='display:none;'> <input type='text'class='new-numValue  form-control' style='width: 95%;' maxlength='10'> </div></td>");

            var $operateTd = $("<td style='width:19%;'></td>"),
                $delete = $("<a role='button' class='btn btn-default new-formula-delete'>删除</a>");
            $edit = $("<a role='button' class='btn btn-default new-formula-edit'>编辑</a>");
            $delete.click(function () {
                var formulaNumNow = $(this).parents("tr").attr("term");
                for (var i = 0; i < FormulaArray.length; i++) {
                    if (FormulaArray[i].formulaNum == formulaNumNow) {
                        FormulaArray.splice(i, 1);
                        i = i - 1;
                    }
                }
                for (var i = 0; i < FormulaArray.length; i++) {
                    if (FormulaArray[i].formulaNum > formulaNumNow) {
                        FormulaArray[i].formulaNum = FormulaArray[i].formulaNum - 1;
                    }
                }
                $(this).parents("tr").remove();

                $("#formula-set-content tbody tr").each(function () {
                    if ($(this).attr("term") > formulaNumNow) {
                        $(this).attr("term", parseInt($(this).attr("term")) - 1);
                    }
                });
            })
            $edit.click(function () {
                formulaEditOrAdd = $(this).parents("tr").attr("term");
                formulaSetInput.length = 0;
                $("#formula_modal_monitor").text("");
                $("#formula_modal").modal("show");
                for (var i = 0; i < FormulaArray.length; i++) {
                    if (FormulaArray[i].formulaNum == $(this).parents("tr").attr("term") && FormulaArray[i].formulaId != "z") {
                        if (FormulaArray[i].field != null) {
                            formulaSetInput.push(formulaSetCheck1[parseInt(FormulaArray[i].field) - 1]);
                        }
                        if (FormulaArray[i].operate != null) {
                            if (FormulaArray[i].operate == "&") {
                                formulaSetInput.push("且")
                            }
                            else if (FormulaArray[i].operate == "|") {
                                formulaSetInput.push("或")
                            }
                            else {
                                formulaSetInput.push(FormulaArray[i].operate)
                            }

                        }
                        if (FormulaArray[i].numValue != null) {
                            formulaSetInput.push(FormulaArray[i].numValue)
                        }
                    }
                }
                $("#formula_modal_monitor").text(formulaSetInput.join(""));
            });

            $operateTd.append($delete, $edit);
            $tr.append($formulaContentTd, $dropdownTd, $Td, $operateTd);
            $("#formula-set-content tbody").append($tr);
            $(".new-numValue").bind("input keydown", function () {
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
        }
        else {
            //如果是修改公式，则改变对应的公式
            $("#formula-set-content tbody tr").each(function () {
                if ($(this).attr("term") == formulaEditOrAdd) {
                    $(this).find(".formula-content").text(formulaSetInput.join(""));
                    $(this).find(".formula-content").attr("title", formulaSetInput.join(""));
                }
            })
        }

        for (var i = 0; i < formulaSet.length; i++) {
            var $objectiveFormula = {
                formulaId: null,        //公式ID
                formulaNum: null,       //公式编号
                orderNum: null,         //排序
                field: null,            //字段
                operate: "",       //操作符
                numValue: ""       //具体值
            }
            if (formulaEditOrAdd != null) {
                $objectiveFormula.formulaNum = formulaEditOrAdd;
            }
            else { $objectiveFormula.formulaNum = formulaNum; }
            $objectiveFormula.orderNum = i + 1;
            if (formulaCheck(formulaSet[i], ["(", ")", "+", "-", "*", ">", "/", "≥", ">", "≤", "且", "或"])) {
                $objectiveFormula.field = null;

                if (formulaSet[i] == "且") {
                    $objectiveFormula.operate = "&";
                }
                else if (formulaSet[i] == "或") {
                    $objectiveFormula.operate = "|";
                }
                else {
                    $objectiveFormula.operate = formulaSet[i];
                }
                $objectiveFormula.numValue = null;
            }
            else if (formulaCheck(formulaSet[i], formulaSetCheck1)) {

                for (var j = 0; j < formulaSetCheck1.length; j++) {
                    if (formulaSetCheck1[j] == formulaSet[i]) {
                        $objectiveFormula.field = j + 1;
                        $objectiveFormula.operate = null;
                        $objectiveFormula.numValue = null;
                        break;
                    }
                }

            }
            else {
                $objectiveFormula.field = null;
                $objectiveFormula.operate = null;
                $objectiveFormula.numValue = formulaSet[i];
            }
            FormulaArray.push($objectiveFormula);
        }
        formulaSetInput.length = 0;

    });

    $("#formula_modal_clear").click(function () {
        formulaSetInput.length = 0;
        $("#formula_modal_monitor").text("");
    });

    /*公式设置 结束*/

    /*------------------------------------目标规则   结束----------------------------------------------------------------*/
    function FormaluView(objectiveFormula, formulaMaxArgu, formulaMinArgu, formulaTypeArgu, parentView) {
        parentView.load("/ObjectiveIndex/GetObjectiveFormulaHtml", function () {
            if (objectiveFormula != null) {
                FormulaLoad(objectiveFormula);
            }
            loadFormaluFlag = false;

            $("#maxValue ,#minValue").bind("input keydown", function () {
                controlInput($(this));
            });
            $("#objectiveRule-Sure").off("click");
            $("#objectiveRule-Sure").click(function () {
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
                ncUnits.alert("自定义公式已保存");
            });

            $("#objectiveRule-setFormula").off("click");
            $("#objectiveRule-setFormula").click(function () {
                $("#formula_modal").modal("show");
                formulaEditOrAdd = null;
                formulaSetInput.length = 0;
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
    $("#objectiveNew_modal").on('show.bs.modal', function (){
        FormulaArray.length = 0;
        formulaEditOrAdd = null;
        formulaNum = 0;
        formulaSetInput.length = 0;    
        $("#ModalObjectiveTab").trigger("click");
        $("#ModalObjectiveRuleTab").css("display", "none");
        if (loadFormaluFlag) {
            FormaluView(null, null, null, null, $("#modal_con_objectiveRule .modal-body"));
        }
        //获取当前登录账号id
        $.ajax({
            type: "post",
            url: "/DelayRule/GetLoginUser",
            dataType: "json",
            data: {},
            success: rsHandler(function (result) {
                loginId = result;
            })
        });    
    })
   
    function controlInput($value) {
        var reg = /^-?\d+(\.\d{0,1}\d{0,1})?$/;
        var values = $($value).val().substring(0, $($value).val().length - 1);
        var getValue = $($value).val();
        if (reg.test(getValue) == false) {
            $($value).val(values);
            return
        }
    }
    function timeThree(id, minStartTime, maxStartTime, minEndTime, maxEndTime) {
        var startTime = null;
        var endTime = null;
         if (id == "#new-startTime") {
            if ($("#new-alarmTime").val() == "") {
                endTime = $("#new-endTime").val();
            }
            else if ($("#new-endTime").val() == "") {
                endTime = $("#new-alarmTime").val();
            }
            else {
                //取较小值
                endTime = $("#new-endTime").val() > $("#new-alarmTime").val() ? $("#new-alarmTime").val() : $("#new-endTime").val();
            }
             

        } else if (id == "#new-endTime") {
            //取较大值
            startTime = $("#new-startTime").val() < $("#new-alarmTime").val() ? $("#new-alarmTime").val() : $("#new-startTime").val();             
        }
        else if (id == "#new-alarmTime") {
            startTime = $("#new-startTime").val();
            endTime = $("#new-endTime").val();          
        }
        if (startTime == "") {
            startTime = null;
        }
        if (endTime == "") {
            endTime == null;
        }
        timeChosen(id, startTime, endTime);
    }

    //单个时间函数
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

    /*新建目标 结束*/

});

