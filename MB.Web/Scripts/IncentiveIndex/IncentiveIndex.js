//@ sourceURL=IncentiveIndex.js
/**
 * Created by DELL on 15-9-16.
 */
$(function () {

    var userId = null; yearSelect = null, monthSelect = null, year = null;
    var changeTab =1;
    var rewardReason = ["计划完成超时", "计划审核超时", "计划确认超时", "流程审批超时", "目标完成超时", "目标审核超时", "目标确认超时"];
    var $parentView = $("#myMotivation > .row");
    var deductionMode = ["执行力金额扣罚","执行力工资比例扣罚"];

    loadingTime();
    //加载时间轴 年
    function loadingTime() {
        var lodi, $motivationTime = $parentView.find(".motivationTime");
        $motivationTime.empty();
        lodi = getLoadingPosition($motivationTime);     //显示load层
        $.ajax({
            type: "post",
            url: "/IncentiveIndex/GetRewardPunishNum",
            dataType: "json",
            data: { year: null, userId: userId },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $motivationTime.empty();
                for (var j = data.length - 1; j >= 0 ; j--) {
                    var v = data[j];
                    var number1 = v.number.toString().indexOf(".") > 0 ? v.number.toFixed(1) : v.number;
                    var $oneTime = $('<div class="oneTime"><label class="circleBig" term=' + v.year + '></label><span class="yearMonthSpan">' + v.year + '</span>' +
                       '<span class="changeMoneySpan" title=' + v.number + '>' + (number1 > 0 ? ("+" + number1) : (number1)) + '</span></div>');
                    $oneTime.children(".circleBig").click(function () {                    
                        if (!$(this).hasClass("circleChose")) {
                            $parentView.find("label.circleChose").removeClass("circleChose");
                            $parentView.find("span.timeValueChose").removeClass("timeValueChose");
                            $(this).toggleClass("circleChose");
                            $(this).next().toggleClass("timeValueChose");

                            if (changeTab == 2) {
                                $(this).closest(".personList").children(".list-inline").find("li:eq(2)").html('<span class=' + (v.number > 0 ? "greenColor" : "orangeColor") + '>' + (v.number > 0 ? ("+" + v.number.toFixed(1)) : v.number) + '</span>')
                            }
                            var number = parseInt($(this).next().next().attr("title"));
                            $parentView.find(".money_stamp").text("￥" + number.toFixed(1)).attr("title", number);
                            if (number > 0) {
                                $parentView.find(".money_stamp").removeClass("warnColor");
                                $parentView.find(".money_stamp").addClass("orangeColor");
                            } else {
                                $parentView.find(".money_stamp").removeClass("orangeColor");
                                $parentView.find(".money_stamp").addClass("warnColor");
                            }
                            yearSelect = parseInt($(this).attr("term"));
                            monthSelect = null;
                            loadingDetail(null, yearSelect);
                            DrawCharts(parseInt($parentView.find(".picSwitchover").attr("term")));
                        } else {
                            $parentView.find("label.circleChose").removeClass("circleChose");
                            $parentView.find("span.timeValueChose").removeClass("timeValueChose");
                            $(this).toggleClass("circleChose");
                            $(this).next().toggleClass("timeValueChose");
                        }
                        childTime($(this));
                    });
                    $motivationTime.append($oneTime);
                    if (j != 0) {
                        var $hr = $(' <hr class="verticalHr"/>');
                        $motivationTime.append($hr);
                    }
                    if (j == data.length - 1) {
                        yearSelect = v.year;
                        var valuesNode = $oneTime.children(".circleBig");
                        if (v.number > 0) {
                            $(valuesNode).next().next().toggleClass("orangeColor");
                        } else {
                            $(valuesNode).next().next().toggleClass("warnColor");
                        }
                        $parentView.find("label.circleChose").removeClass("circleChose");
                        $parentView.find("span.timeValueChose").removeClass("timeValueChose");
                        $(valuesNode).toggleClass("circleChose");
                        $(valuesNode).next().toggleClass("timeValueChose");

                        loadingDetail(null, yearSelect);
                        $parentView.find(".money_stamp").text("￥" + number1).attr("title", v.number);
                        if (v.number > 0) {
                            $parentView.find(".money_stamp").removeClass("warnColor");
                            $parentView.find(".money_stamp").addClass("orangeColor");
                        } else {
                            $parentView.find(".money_stamp").removeClass("orangeColor");
                            $parentView.find(".money_stamp").addClass("warnColor");
                        }
                        DrawCharts(parseInt($parentView.find(".picSwitchover").attr("term")));
                    }
                }
            })
        })
    }

    //加载子时间轴 月
    function childTime(value) {
        var yearMoney = parseInt($(value).next().next().text());
        if (yearMoney > 0) {
            $(value).next().next().toggleClass("orangeColor");
        } else {
            $(value).next().next().toggleClass("warnColor");
        }
        if (value.nextAll(".monthChild").length != 0) {
            value.nextAll(".monthChild").slideToggle();
        } else {
            var year = parseInt($(value).attr("term"));
            var lodi = getLoadingPosition($(value).closest(".motivationTime"));     //显示load层
            $.ajax({
                type: "post",
                url: "/IncentiveIndex/GetRewardPunishNum",
                dataType: "json",
                data: { year: year, userId: userId, },
                complete: rcHandler(function () {
                    lodi.remove();         //关闭load层
                }),
                success: rsHandler(function (data) {
                    var $child = $('<div class="monthChild"></div>');
                    $.each(data, function (i, v) {
                        var number2 = v.number.toString().indexOf(".") > 0 ? v.number.toFixed(1) : v.number;
                        var $monthDiv = $('<div class="oneTime"><label class="circleSmall" term=' + v.month + '></label><span class="yearMonthSpan">' + v.month + '月</span>' +
                            '<span class="changeMoneySpan ' + (v.number > 0 ? "orangeColor" : "warnColor") + '"  title=' + v.number + '>' + (number2 > 0 ? ("+" + number2) : (number2)) + '</span></div>');
                        $monthDiv.children(".circleSmall").click(function () {
                            if (!$(this).hasClass("circleChose")) {

                                if (changeTab == 2) {   
                                    $(this).closest(".personList").children(".list-inline").find("li:eq(2)").html('<span class=' + (v.number > 0 ? "greenColor" : "orangeColor") + '>' + (number2 > 0 ? ("+" + number2) : number2) + '</span>')
                                }
                                $parentView.find("label.circleChose").removeClass("circleChose");
                                $parentView.find("span.timeValueChose").removeClass("timeValueChose");
                                $(this).next().toggleClass("timeValueChose");
                                $(this).toggleClass("circleChose");
                                $parentView.find(".money_stamp").text("￥" + number2).attr("title", v.number);
                                if (v.number > 0) {
                                    $parentView.find(".money_stamp").removeClass("warnColor");
                                    $parentView.find(".money_stamp").addClass("orangeColor");
                                } else {
                                    $parentView.find(".money_stamp").removeClass("orangeColor");
                                    $parentView.find(".money_stamp").addClass("warnColor");
                                }
                                loadingDetail(v.month, year);
                                yearSelect = year;
                                monthSelect = v.month;
                                DrawCharts( parseInt( $parentView.find(".picSwitchover").attr("term")) );
                                
                            }
                            
                        });
                        $child.append($(' <hr class="verticalHr"/>'), $monthDiv);
                    });
                    $(value).parent().append($child);
                    $child.slideDown();
                })
            })
        }
    }

    //加载激励详细信息
    function loadingDetail(month, year) {
        var lodi, $myMotivationTable = $parentView.find(".motivationTable");
        $myMotivationTable.empty();
        lodi = getLoadingPosition($myMotivationTable);     //显示load层
        $.ajax({
            type: "post",
            url: "/IncentiveIndex/GetRewardPunishDetail",
            dataType: "json",
            data: { year: parseInt(year), month: parseInt(month), userId: userId },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $parentView.find(".detailTitleHead").text((month == null ? year + '年' : month + '月') + "奖励情况");
                $parentView.find(".yearAvgHour").text( (month == null ? '年':'月') + "平均有效工时:" + data.avgTime.toFixed(1));
                $parentView.find(".rewardValue").text(data.timeReward.toString().indexOf(".")>0?data.timeReward.toFixed(1):data.timeReward);
                var $tbody = $("<tbody></tbody>");
                $.each(data.detail, function (i, v) {
                    var deduNum = v.deductionNum.toString().indexOf(".") > 0 ? v.deductionNum.toFixed(1) : v.deductionNum;
                    if (v.deductionMode == 1) {
                        var text = "￥-" +deduNum;
                    } else {
                        var text = "-"+deduNum + "%";
                    }
                    var targetNum = v.targetName == null ? "" : v.targetName;
                    var $tr = $('<tr><td>' + (v.statisticeTime.substr(0, 10)) + '</td><td class="textOver" title=' + v.targetName + '>' + targetNum + '</td>' +
                      '<td>' + rewardReason[v.type - 1] + '</td><td>' + deductionMode[v.deductionMode-1]+ '</td><td class= "warnColor" >' + text + '</td></tr>');
                    $tbody.append($tr);
                });
                $myMotivationTable.empty().append($tbody)

            })
        });
    }

    //图表切换事件
    $(".picSwitchover").click(function () {
        //目标价值
        //  type:$int,             //1:计划执行力 2:流程执行力 3:功效价值 4:目标价值
        chartChange(this);
    });
    function chartChange(value) {
        var typeChart = parseInt($(value).attr("term"));
        typeChart++;
        if (typeChart > 4) {
            typeChart = 1;
        }
        $(value).attr("term", typeChart);
        DrawCharts(typeChart);
    }

    //绘制图表
    function DrawCharts(type) {
        var xData = [], count = [], completeCount = [], completeRate = [], timeoutCount = [], timeoutRate = [], legendData = [], titleText;
        var $circle = $parentView.find(".circleChose");
        var yearNum = parseInt($circle.attr("term")), monthNum, userIdNum;
        if (yearNum <= 12) {
            monthNum = yearNum;
            yearNum = parseInt( $circle.closest(".monthChild").prev().prev().text() );
        }else{
            monthNum = null;
        }
        if (changeTab == 1) {
            userId = null;
        } else {
            userId = parseInt( $(".trClick").attr("term") );
        }
        var lodi = getLoadingPosition( $parentView.find(".chartArea") );     //显示load层
        $.ajax({
            type: "post",
            url: "/IncentiveIndex/GetIncentiveData",
            dataType: "json",
            data: { type: type, year: yearNum, month: monthNum, userId: userId },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $.each(data, function (i, v) {
                    xData.push(v.name);
                    count.push(v.count);
                    completeCount.push(v.completeCount);
                    completeRate.push(v.completeRate);
                    timeoutCount.push(v.timeoutCount);
                    timeoutRate.push(v.timeoutRate);
                    //                        name:$int,                    //月份或日期
                    //                        count:$int,                   //总数        实际工时
                    //                        completeCount:$int,           //完成数     
                    //                        timeoutCount:$int,            //超时数      有效工时
                    //                        completeRate:$decimal,        //完成率      
                    //                        timeoutRate:$decimal          //超时率      效率系数
                });

                if (type == 4) {         //目标价值
                    legendData = ["完成数", "目标数", "完成率"];
                    titleText = "目标价值";
                } else if (type == 3) {           //功效价值
                    legendData = ["有效工时", "实际工时", "效率系数"];
                    titleText = "功效价值";
                } else if (type == 2) {      //流程执行力
                    legendData = ["超时数", "超时率"];
                    titleText = "流程执行力";
                } else if (type == 1) {        //计划执行力
                    legendData = ["超时数", "超时率"];
                    titleText = "计划执行力";
                }
                if (type == 1 || type == 2) {
                    var series = [
                        {
                            "name": legendData[0],
                            "type": "bar",
                            "data": timeoutCount,
                            itemStyle: { normal: { color: "#56b456" } },
                            barWidth: 15
                        }, {
                            "name": legendData[1],
                            "type": "line",
                            yAxisIndex: 1,
                            "data": timeoutRate,
                            itemStyle: { normal: { color: "#0166a7" } }
                        }
                    ];
                } else if(type == 3){
                    var series = [
                         {
                             "name": legendData[0],
                             "type": "bar",
                             "data": completeCount,
                             itemStyle: { normal: { color: "#56b456" } },
                             barWidth: 15
                         }, {
                             "name": legendData[1],
                             "type": "bar",
                             "data": timeoutCount,
                             xAxisIndex: 1,
                             itemStyle: { normal: { color: "#b6b6b6" } },
                             barWidth: 15
                         }, {
                             "name": legendData[2],
                             "type": "line",
                             yAxisIndex: 1,
                             "data": timeoutRate,
                             itemStyle: { normal: { color: "#0166a7" } }
                         }
                    ];
                } else {
                    var series = [
                     {
                         "name": legendData[0],
                         "type": "bar",
                         "data": completeCount,
                         itemStyle: { normal: { color: "#56b456" } },
                         barWidth: 15
                     }, {
                         "name": legendData[1],
                         "type": "bar",
                         "data": count,
                         xAxisIndex: 1,
                         itemStyle: { normal: { color: "#b6b6b6" } },
                         barWidth: 15
                     }, {
                         "name": legendData[2],
                         "type": "line",
                         yAxisIndex: 1,
                         "data": completeRate,
                         itemStyle: { normal: { color: "#0166a7" } }
                     }
                    ];
                }
                loadTap(xData, legendData, titleText, series);
            })
        })
    }

    function loadTap(xData, legendData, titleText, seriesData) {
        var myChart = echarts.init( $parentView.find(".chartArea:eq(0)")[0] );
        var option = {
            grid: {
                borderWidth: 0,
                width: 550,
                x: 40
            },
            title: {
                text: titleText,
                textStyle: {
                    fontSize: 18,
                    fontWeight: 'bolder',
                    color: '#333'
                },
                x: "center"
            },
            tooltip: {
                show: true
            },
            legend: {
                data: legendData,
                y: "bottom"
            },
            xAxis: [
                {
                    type: 'category',
                    data: xData,
                    splitLine: { show: false },
                    axisLine: false,
                    axisTick: false
                },
                {
                    type: 'category',
                    axisLine: { show: false },
                    axisTick: { show: false },
                    axisLabel: { show: false },
                    splitArea: { show: false },
                    splitLine: { show: false },
                    data: xData
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    name: 'objectNum',
                    axisLabel: {
                        textStyle: { color: "#56b456" }
                    },
                    splitLine: {
                        lineStyle: { color: ['#bbbbbb'], width: 1, type: 'dotted' }
                    },
                    position: 'left',
                    axisLine: false
                }, {
                    type: 'value',
                    axisLabel: {
                        formatter: '{value} %',
                        textStyle: { color: "#0166a7" }
                    },
                    name: 'objectPercent',
                    splitLine: { show: false },
                    position: 'right',
                    axisLine: false
                }
            ]
        };
        // 为echarts对象加载数据
        myChart.setOption(option);
        myChart.setSeries(seriesData);
    }


    //下属奖励
    var firstFlag = true;
    $("#subMotivationTab").click(function () { 
        $(".addPic").css("display", "inline-block");
        changeTab = 2;
        if (firstFlag) {
            var dateVar = new Date();
            yearSelect = dateVar.getFullYear();
            monthSelect = null;
            loadSubMotivation();
            firstFlag = false;
        }
    });

    //加载下属奖励
    function loadSubMotivation() {
        var lodi, $subMoneyTable = $("#subMoneyTable");
        $subMoneyTable.empty();
        lodi = getLoadingPosition($subMoneyTable);     //显示load层
        $.ajax({
            type: "post",
            url: "/IncentiveIndex/GetUnderReward",
            dataType: "json",
            data: { year: yearSelect, month: monthSelect },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $subMoneyTable.empty();
                var $ulView = $('<ul class="list-group" id="subPersonGroup"><li class="list-group-item personList">'+
                    '<ul class="list-inline"><li>人员</li><li>总额</li><li>应得奖励</li></ul></li></ul>');
                $.each(data, function (i, v) {
                    var $liView = addPersonOne(v);
                    $ulView.append($liView);
                });
                $subMoneyTable.append($ulView);
                if ($ulView.find(".personList:eq(1)").length!= 0 ) {
                    $ulView.find(".personList:eq(1)").children("ul.list-inline:eq(0)").trigger("click");
                }
            })
        })
    }

    function addPersonOne( v ) {
        var $liView = $('<li class="list-group-item personList" term=' + v.userId + '>' +
                                         '<ul class="list-inline">' +
                                            '<li>' + v.userName + '</li>' +
                                             '<li>' + v.actualNum + '</li>' +
                                             '<li><span class=' + (v.deservedNum > 0 ? "greenColor" : "orangeColor") + '>' + (v.deservedNum > 0 ? ("+" + v.deservedNum) : v.deservedNum) + '</span></li>' +
                                             '<li class="liDel"><span ></span></li>' +
                                         '</ul></li>');

        $liView.find(".liDel span").click(function () {
            var $close = $(this).closest(".personList");
            ncUnits.confirm({
                title: '提示',
                html: '你确认要删除这个用户吗？',
                yes: function (layer_confirm) {
                    $close.remove();
                    layer.close(layer_confirm);
                   
                }
            });
        });

        var $rowParent = $('<div class="row rewardView">' +
                           '<div  class="col-xs-9 motivationDetail">' +
                           '<div class="money_stamp textOver"></div>' +
                           '<ul class="list-group">' +
                                 '<li class="list-group-item"><h4 class="titleHead detailTitleHead" ></h4><h4>功效价值</h4></li>' +
                                 '<li class="list-group-item"><span class="col-xs-5 yearAvgHour" style="padding: 0px"></span>' +
                                       '<span>奖励：<span class="rewardValue"></span></span><h4 style="margin-top:22px"> 执行力</h4></li>' +
                                  '<li class="list-group-item"><table class="table motivationTable" ></table></li>' +
                           '</ul>' +
                           '<div style="height:400px;border:1px solid #ccc;padding:10px;overflow-x: auto; position: relative">' +
                               '<a  href="javaScript:void(0)" class="glyphicon glyphicon-menu-left picSwitchover" onclick="chartChange()" term="1"></a>' +
                               '<div class="chartArea" style="height:100%" ></div>' +
                           '</div>' +
                           '</div><div class="motivationTime col-xs-3" ></div></div>');
        $rowParent.find(".picSwitchover").click(function () {
            chartChange(this);
        })

        $liView.find("ul.list-inline:eq(0)").click(function () {
            $parentView = $rowParent;
            if (!$(this).parent().hasClass("trClick")) {
                $(".trClick .rewardView").slideUp();
                $(".trClick").removeClass("trClick");
                $(this).parent().addClass("trClick");
                if ( $(this).parent().find(".rewardView").length == 0 ) {
                    $liView.append($rowParent);
                    $rowParent.slideToggle();
                    userId = v.userId;
                    loadingTime();
                } else {
                    $rowParent.slideToggle();
                }
            } else {
                $rowParent.slideToggle();
            }
        });


        return $liView;
    }

    //我的奖励
    $("#myMotivationTab").click(function () {
        changeTab = 1;
        $(".addPic").css("display", "none");
        $("#subPersonList").slideUp();
        $parentView = $("#myMotivation > .row");
        userId = null;
    });

    var FirstClick = true;
    //添加人员
  
    $(".addPic").click(function () {
        $("#subPersonList").slideToggle(); 
        if (FirstClick) {
            var lodi = getLoadingPosition($("#personZtree"));     //显示load层;
            FirstClick = false;
            $.ajax({
                type: "post",
                url: "/IncentiveIndex/GetUnderReward",
                dataType: "json",
                data: {parent:null, year: yearSelect, month: monthSelect },
                complete: rcHandler(function () {
                    lodi.remove();         //关闭load层
                }),
                success: rsHandler(function (data) {
                    var index = 0;
                    $.each(data, function (i, v) {
                        data[i].id = v.userId;
                        data[i].name = v.userName;
                    })
                     var folderTree = $.fn.zTree.init($("#personZtree"), $.extend({
                        callback: {
                            onNodeCreated: function (e, id, node) {
                                node.actualNum = data[index].actualNum;
                                node.deservedNum = data[index++].deservedNum;
                                if ( $("li.personList[term=" + node.id + "]").length != 0 ) {
                                    var ztrees = $.fn.zTree.getZTreeObj(id);
                                    ztrees.checkNode(node, true, false);
                                }
                            },
                            onCheck: function (e, id, node) {        //选中事件
                                if (node.checked) {
                                    var vnode = {
                                        userId: node.id,
                                        userName: node.name,
                                        actualNum: node.actualNum,
                                        deservedNum: node.deservedNum
                                    }
                                    $("#subPersonGroup").append(addPersonOne(vnode));
                                }
                                else {
                                    $("li.personList[term=" + node.id + "]").remove();
                                }
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
                            chkStyle: "checkbox",
                            chkboxType: { "Y": "", "N": "" }
                        },
                        async: {
                            enable: true,
                            url: "/IncentiveIndex/GetUnderReward",
                            autoParam: ["id=parent"],
                            dataFilter: function (treeId, parentNode, responseData) {
                                $.each(responseData.data, function (i, v) {
                                    responseData.data[i].id = v.userId;
                                    responseData.data[i].name = v.userName;
                                })
                                index = 0;
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            })
        }
    });

    //个人信息
    loadPersonalInfo();

    //新建目标
    $("#newObjectiveBtn").click(function () {
        newOrDecomposition = 1;
        $("#objectiveNew_modal").modal('show');
        newInit();
    });

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
                    //$("#new-alarmTime").val(data.alarmTime == null ? "" : data.alarmTime.split("T", 1)[0]);
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
                    $(" #new-startTime,#new-endTime").off("click")
                    $(" #new-startTime,#new-endTime").click(function () {
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
                    if (data.objectiveFormula.length != null && data.objectiveFormula.length != 0) {
                        Returndata.objectiveFormula.length = 0;
                        for (var i = 0; i < data.objectiveFormula.length; i++) {
                            Returndata.objectiveFormula.push(jQuery.extend(true, {}, data.objectiveFormula[i]));
                        }

                    }

                })
            })


        }

    }
})

