//@ sourceURL=objectiveHome.js
/**
 * Created by DELL on 15-8-14.
 */
$(function () {
    // $("#punckListTab").css("color", "#58b456");

    // 右侧圆饼
    var date = new Date()
        , year = date.getFullYear()
        , month = date.getMonth() + 1
        , $con = $("#object_statistics");
    var colors = ["#be1d9a", "#fbab11", "#64d8ae", "#57acdb", "#58b557", "#e02215"];   //1：待提交 2：待审核 3：审核通过 4：待确认 5：已完成 6、超时 int

    //右侧个人信息
    loadPersonalInfo();
    function drawPlanProgress() {
        $("#object_chart").empty();
        $(".chart-center", $con).empty();
        var lodi = getLoadingPosition('.chart-container');//显示load层
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectiveProcessList",
            dataType: "json",
            data: {
                year: year,
                month: month
            },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $("#object_chart").empty();
                $(".chart-center", $con).empty();
                var dountData = [], count = 0, $ul = $(".chart-label", $con);
                $ul.empty();
                for (var i = 0, len = data.length; i < len; i++) {
                    var color = colors[i];
                    var num = data[i].count;
                    if (num != 0) {
                        count += num;
                        dountData.push([num, color, data[i].id, data[i].text + "目标"]);
                    }
                    $ul.append('<li><span class="color" style="background-color:' + color + '"></span><span class="text">' + data[i].text + '</span></li>');

                }
                Raphael("object_chart", 330, 310).dountChart(165, 155, 55, 70, 110, dountData, function (data) {
                    if (!$("#punckListTab").hasClass("TabActiveColor")) {
                        $('#punckListTab').trigger("click");
                    }
                    //TODO 饼图click事件
                    //拼上时间
                    var day;
                    if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
                        day = 31;
                    }
                    else if (month == 4 || month == 6 || month == 9 || month == 11) {
                        day = 30;
                    }
                    else if (month = 2) {
                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && year % 100 == 0))//闰年
                        {
                            day = 29;
                        }
                        else {
                            day = 28;
                        }
                    }
                    var timeStart = year + '/' + month + '/01';
                    var timeEnd = year + '/' + month + '/' + day;
                    $("#startTime,#endTime").val('');
                    $("#filterBox:hidden").slideDown();
                    $(".chooseCondition:eq(1):not('.chosen')").addClass("chosen");
                    $(".selectedCondition").empty();
                    $("#specific .chosen").removeClass("chosen");
                    $("#orgPersonSelect").text("全部");
                    $("#orgPersonSelect").attr("term", 1);
                    $("#objectKind a.chosen").removeClass("chosen");
                    addSelects((timeStart + "-" + timeEnd), 3, "时间1");
                    addSelects(data[3], data[2], "状态1");
                    objectListArgus.startTime = ["3", timeStart, timeEnd];
                    objectListArgus.soonSelect = null;
                    objectListArgus.department.length = 0;
                    objectListArgus.person.length = 0;
                    objectListArgus.status.length = 0;
                    objectListArgus.status.push(data[2]);
                    loadObjectList();

                });
                $(".chart-center", $con).html('目标<span style="color:#58b456">' + count + '</span>项');
                $(".month .text", $con).html(year + "年" + month + "月");
            })
        });
    }

    //drawPlanProgress();
    $(".arrowsBBLCom", $con).click(function () {
        var date = new Date()
            , thisyear = date.getFullYear()
            , thismonth = date.getMonth() + 1;
        if (year == thisyear && month == thismonth) {
            $(".arrowsBBRCom", $con).show();
        }
        if (month == 1) {
            year--;
            month = 12;
        } else {
            month--;
        }
        $(".month .text", $con).html(year + "/" + month);
        drawPlanProgress();
    });

    $(".arrowsBBRCom", $con).click(function () {
        var date = new Date()
            , thisyear = date.getFullYear()
            , thismonth = date.getMonth() + 1;
        if (month == 12 && year < date.getFullYear()) {
            year++;
            month = 1;
        } else {
            month++;
        }
        $(".month .text", $con).html(year + "/" + month);
        if (year >= thisyear && month >= thismonth) {
            $(this).hide();
        }
        drawPlanProgress();
    });

    //环形进度条
    function roundProcess(value, status) {
        var text = $(value).data("text");
        $(value).knob({
            width: 80,
            height: 80,
            min: 0,
            thickness: .3,
            fgColor: colors[status],
            bgColor: '#e0e0e0',
            inputColor: '#888',
            release: function (v) {
                if (text) {
                    return false;
                }
                if (parseInt(v) > 90 || parseInt(v) < 0) {
                    v = 90;
                }
                var objectId = $(value).attr("term");
                $.ajax({
                    type: "post",
                    url: "/ObjectiveIndex/UpdateObjectiveProcess",
                    dataType: "json",
                    data: { objectiveId: objectId, newProcess: v },
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert("已更新进度！");
                        }
                        else {
                            ncUnits.alert("更新进度失败");
                        }
                    })
                });
            },
            format: function (v) {
                if (text) {
                    return text;
                } else {
                    //if (v > 90) {
                    //    return  "90%";
                    //} 
                    return v + "%";
                }
            }
        });
    }


    //加载目标列表参数
    var objectListArgus = {
        soonSelect: Number,   //快捷查询，忽略其他筛选条件：1：待提交 2：待审核 3：审核通过 4：待确认 5：已完成 6、超时 int
        status: [],            //目标状态 1：待提交 2：待审核 3：审核通过（进行中、已超时） 4：待确认 5：已完成 int[]
        department: [],  //目标对象组织架构, int[]
        person: [],     //目标对象人员 int[]
        startTime: [],      //目标预计开始时间：1、近一周  2、近一月  3、自定义 string[]
        objectiveType: Number       //1、我的目标  2、下属目标 int
    },
     personModal = 1, object_info, loginId;             //personModal=1 筛选条件的人员选择, personModal=2 授权人员选择
    objectListArgus.objectiveType = 1;         //默认为我的目标

    //全局数组
    var statusArray = ["待提交", "待审核", "审核通过", "待确认", "已完成"],     //1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成
        checkTypeArray = ["金额", "时间", "数字"];               ////考核类型 1：金额 2：时间 3：数字

    var operateResult = ["创建", "删除", "授权", "提交", "撤销", "审核通过", "审核不通过", "修改", "分解目标", "更新进度", "确认通过", "确认不通过", "下载文档", "查看", "上传文档", "删除文档", "提交确认", "取消授权"];
    var messageResult = [6, 7, 8, 11, 12];
    var $activeNode, $unfoldNode;                //存储授权节点，便于刷新该目标层
    var detailFlag = 1;                  //detailFlag=1 目标详情画面, detailFlag=2 目标审核画面, detailFlag=3 目标确认画面,

    //时间插件
    var start = {
        elem: '#startTime',
        format: 'YYYY-MM-DD',
        max: '2099-06-16', //最大日期
        istime: false,
        istoday: false,
        choose: function (datas) {
            end.min = datas; //开始日选好后，重置结束日的最小日期
            end.start = datas //将结束日的初始值设定为开始日
        },
        clear: function () {
            end.min = undefined;
        }
    };
    var end = {
        elem: '#endTime',
        format: 'YYYY-MM-DD',
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
    laydate(start);
    laydate(end);


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

    //列表点击事件
    $('#punckListTab').click(function () {
        if (!$(this).hasClass("TabActiveColor")) {
            $(this).addClass("TabActiveColor");
            if (objectListArgus.objectiveType == 1) {
                $("#myObject").addClass('active');
                $("#subObject").removeClass("active");
            } else {
                $("#myObject").removeClass('active');
                $("#subObject").addClass("active");
            }
            statusCount();
            loadObjectList();
            drawPlanProgress();
           
        }
    });


    function timeThree(id, minStartTime, maxStartTime, minEndTime, maxEndTime) {
        var startTime = null;
        var endTime = null;
        if (id == "#modifyStartTime") {              //开始
            startTime = minStartTime;
            endTime = $("#modifyEndTime").val();
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
            //endTime = $("#new-endTime").val() > $("#new-alarmTime").val() ? $("#new-alarmTime").val() : $("#new-endTime").val();
            endTime = $("#new-endTime").val()
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
        //else if (id == "#new-alarmTime") {
        //    startTime = $("#new-startTime").val();
        //    endTime = $("#new-endTime").val();
        //    if (parentObjectiveEndTime != null) {
        //        if (startTime == "") {
        //            startTime = parentObjectiveStartTime;
        //        }
        //        if (endTime == "") {
        //            endTime = parentObjectiveEndTime;
        //        }
        //    }
        //}
        if (startTime == "") {
            startTime = null;
        }
        if (endTime == "") {
            endTime == null;
        }
        timeChosen(id, startTime, endTime);
    }

    $("#timeSelect .form-inline .dateCorn:eq(0)").click(function () {
        laydate(start);
    });

    $("#timeSelect .form-inline .dateCorn:eq(1)").click(function () {
        laydate(end);
    });



    //看完我收起来,筛选
    $(".chooseCondition").click(function () {
        $("#filterBox").slideToggle();
        $(".chooseCondition:eq(1)").toggleClass("chosen");
    });


    //清空环形中的筛选条件
    function circleClean() {
        if ($(".selectedCondition>li.circleCondition").length != 0) {
            $(".selectedCondition").empty();
            objectListArgus.startTime.length = 0;
            objectListArgus.status.length = 0;
        }
        objectListArgus.soonSelect = null;
        $("#objectKind li a").removeClass("chosen");
    }

    //时间筛选条件
    $("#timeSelect li a").click(function () {
        if (!$(this).hasClass("chosen")) {
            circleClean();
            var text;
            if ($(this).attr("term") == 3) {
                var start = $("#startTime").val();
                var end = $("#endTime").val();
                if (start == "" && end == "") {
                    return;
                }
                text = start + "-" + end;
                addSelects(text, $(this).attr("term"), "时间");
                objectListArgus.startTime[0] = "3";
                objectListArgus.startTime[1] = start;
                objectListArgus.startTime[2] = end;
            } else {
                text = $(this).text();
                objectListArgus.startTime.length = 0;
                objectListArgus.startTime[0] = $(this).attr("term");
                addSelects(text, $(this).attr("term"), "时间");
            }
            loadObjectList();

            if ($(this).attr("term") == 3) {
                $(this).parent().prev().find("label:eq(0)").addClass("chosen");
                $(this).parent().prev().find("label:eq(1)").addClass("chosen");
            } else {
                $(this).addClass("chosen");
            }
        }
    });

    //目标对象
    $("#object_target .dropdown-menu a").click(function () {
        dropDown(this);
        if ($(this).attr("term") == 0) {         //全部
            circleClean();
            objectListArgus.department.length = 0;
            objectListArgus.person.length = 0;
            $(".selectedCondition li span[classify='组织']").parent().remove();
            $(".selectedCondition li span[classify='人员']").parent().remove();
            loadObjectList();
        }
        else if ($(this).attr("term") == 2) {
            personModal = 1;
            $("#HR_modal").modal("show");
        }
    });

    //下拉框事件
    function dropDown(value) {
        var span = $(value).parents(".dropdown").find(".dropdown-toggle span:eq(0)");
        $(span).attr("term", $(value).attr("term"));
        $(span).text($(value).text());
    }

    //状态筛选条件
    $("#statusCondition li a").click(function () {
        if (!$(this).hasClass("chosen")) {
            circleClean();
            addSelects($(this).text(), $(this).attr("term"), "状态");
            $(this).addClass("chosen");
            //$("#objectKind li a").removeClass("chosen");
            objectListArgus.status.push(parseInt($(this).attr("term")));
            //objectListArgus.soonSelect = null;
            loadObjectList();
        }
    });

    //添加条件
    function addSelects(text, term, classify) {
        if (classify == "时间") {
            var timeSelect = $(".selectedCondition li span[classify='时间']");
            if (timeSelect.length != 0) {
                $(timeSelect).next().trigger("click");
            }
        }
        if (classify == "状态1" || classify == "时间1") {
            classify = classify.substring(0, classify.length - 1);
            var $span = $("<li class='circleCondition'><span term=" + term + " classify=" + classify + ">" + text + "</span><span class='closeBtn'></span></li>");
        } else {
            var $span = $("<li><span term=" + term + " classify=" + classify + ">" + text + "</span><span class='closeBtn'></span></li>");
        }
        $span.find(".closeBtn").click(function () {
            closeBtn(this);
        });
        $(".selectedCondition").append($span);
    }

    //选择条件的X事件
    function closeBtn(value) {
        var classify = $(value).prev().attr("classify"), term = parseInt($(value).prev().attr("term"));
        if (classify == "时间") {
            if (objectListArgus.startTime.length > 2) {
                $("#startTime,#endTime").val('');
            }
            objectListArgus.startTime.length = 0;
            $("#timeSelect li .chosen").removeClass("chosen");
        } else if (classify == "组织") {
            objectListArgus.department.splice($.inArray(term, objectListArgus.department), 1);
        } else if (classify == "人员") {
            objectListArgus.person.splice($.inArray(term, objectListArgus.person), 1);
        } else if (classify == "状态") {
            objectListArgus.status.splice($.inArray(term, objectListArgus.status), 1);
            $("#statusCondition .chosen[term=" + term + "]").removeClass("chosen");
        }
        $(value).parent().remove();
        loadObjectList();
    }

    //条件清空事件
    $("#cleanCondition").click(function () {
        $(".selectedCondition").empty();
        $("#startTime,#endTime").val('');
        objectListArgus.startTime.length = 0;
        objectListArgus.department.length = 0;
        objectListArgus.person.length = 0;
        objectListArgus.status.length = 0;
        $("#specific .chosen").removeClass("chosen");
        $("#orgPersonSelect").text("全部");
        $("#orgPersonSelect").attr("term", 1);
        loadObjectList();
    });




    //我的目标列表开始
    $("#myObjectTab").click(function () {
        $(this).tab('show');
        objectListArgus.objectiveType = 1;
        objectListArgus.soonSelect = null;
        $("#objectKind li a").removeClass("chosen");
        statusCount();
        loadObjectList();
        drawPlanProgress();
    });

    //下属目标
    $("#subObjectTab").click(function () {
        $(this).tab('show');
        objectListArgus.objectiveType = 2;
        objectListArgus.soonSelect = null;
        $("#objectKind li a").removeClass("chosen");
        statusCount();
        loadObjectList();
        drawPlanProgress();
    });

    //统计函数
    function statusCount() {
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectiveStatusList",
            dataType: "json",
            data: { flag: objectListArgus.objectiveType },
            success: rsHandler(function (data) {
                var texts = $("#objectKind li a.chosen").text();
                $("#objectKind ul").empty();
                $.each(data, function (i, v) {
                    var $li = $("<li term=" + v.id + "><a  href='#'>" + v.text + " : </a><span style='color:" + colors[v.id - 1] + "'>" + v.count + "</span></li>");
                    if (texts == (v.text + " : ")) {
                        $li.find("a").addClass("chosen");
                    }
                    $($li).click(function () {
                        $("#objectKind li a").removeClass("chosen");
                        $(this).find("a").addClass("chosen");
                        objectListArgus.soonSelect = parseInt($(this).attr("term"));
                        $("#statusCondition a.chosen").removeClass("chosen");
                        $(".selectedCondition li span[classify='状态']").parent().remove();
                        objectListArgus.startTime.length = 0;
                        objectListArgus.department.length = 0;
                        objectListArgus.person.length = 0;
                        objectListArgus.status.length = 0;
                        loadObjectList();
                    });
                    $("#objectKind ul").append($li);
                })
            })
        });
    }



    //加载目标列表
    statusCount();
    loadObjectList();
    drawPlanProgress();       //刷新饼图

    function loadObjectList() {
        if (objectListArgus.objectiveType == 1) {
            var $myObject = $("#myObject");
        } else {
            var $myObject = $("#subObject");
        }
        $myObject.empty();
        var lodi = getLoadingPosition($myObject);     //显示load层
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetObjectiveList",
            dataType: "json",
            data: { data: JSON.stringify(objectListArgus) },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                $myObject.empty();
                loginId = data.userId;
                if (data.ObjectiveList == null) {            //如果没有数据
                    var noObject = $("<div id='noObjectView'>还没有任何目标，点击&nbsp;&nbsp;<span class='add_big'></span>&nbsp;&nbsp;添加</div>");
                    noObject.find(".add_big").click(function () {
                        newOrDecomposition = 1;
                        $("#objectiveNew_modal").modal('show');
                        newInit();
                    })
                    $myObject.append(noObject);
                    return;
                }
                $.each(data.ObjectiveList, function (i, v) {
                    var $parent = $("<div></div>");
                    var row = PLChunk(v, 1);
                    $($parent).append(row).appendTo($myObject);
                });
            })
        });
    }

    var object_unfoldVar = {
        "objectiveId": 0,
        "responsibleUser": null,
        "confirmUser": null,
        "authorizedUser": null
    };
    //绘制目标的每一行 v指一条目标的信息， loginId：登录用户ID， flag=1表示是第一层目标加载
    function PLChunk(v, flag) {
        var $row = $("<div class='PLChunk'><div class='objectLabel'><span class='orgLabel' term=" + v.objectiveType + ">" + v.objectiveTypeName + "</span><span class='glyphicon glyphicon-triangle-bottom pull-right'></span></div></div>");

        //子目标列表
        if (v.isHasChild) {                 //如果有子目标列表
            $($row).find("span:eq(1)").click(function () {
                if ($(this).hasClass("glyphicon-triangle-bottom")) {
                    var $next = $(this).closest(".PLChunk").next();
                    if ($($next).hasClass("objectChild")) {
                        $($next).slideToggle();
                    } else {
                        loadChildObjectList($(this).closest(".PLChunk"), v.objectiveId);
                    }
                } else {
                    var $next = $(this).closest(".PLChunk").next();
                    if ($($next).hasClass("objectChild")) {
                        $($next).slideToggle();
                    }
                }
                if (flag == 1) {
                    $(this).closest(".PLChunk").parent().toggleClass("parentObject");
                }
                $(this).toggleClass("glyphicon-triangle-bottom");
                $(this).toggleClass("glyphicon-triangle-top");
            });

        } else {
            $($row).find("span:eq(1)").hide();
        }

        if (v.status == 5) {          //进行中和已完成的目标按百分比显示对应颜色的外圆圈，圆圈内显示目标进度百分比。
            var $main = $("<div class='row main'><div class='knobwrapper'><input class='knob'   data-skin='tron' data-displayInput='true'  term=" + v.objectiveId + " value=" + (v.progress == null ? 0 : v.progress) + " data-angleArc=360></div></div>");
        } else if (v.status == 3) {            //已审核
            var $main = $("<div class='row main'><div class='knobwrapper'><input class='knob'   data-max=90  data-skin='tron' data-displayInput='true'  term=" + v.objectiveId + " value=" + (v.progress == null ? 0 : v.progress) + " data-angleArc=324></div></div>");
        } else {                        //其他状态的目标显示对应颜色的外圆圈，圆圈内显示目标状态文字。
            var $main = $("<div class='row main'><div class='knobwrapper'><input class='knob'  data-angleArc=324   data-skin='tron' data-displayInput='true' value=100   term=" + v.objectiveId + "  data-text=" + statusArray[v.status - 1] + " ></div></div>");
        }
        if (v.status == 3 && objectListArgus.objectiveType == 1) {                //只有进行中状态的目标进度条可以更改进度
            $($main).find(".knob").attr("data-readonly", false);
        } else {
            $($main).find(".knob").attr("data-readonly", true);
        }

        roundProcess($($main).find(".knob"), v.status - 1);
        var $objectRunOne = $("<div class='runMode objectRunOne'></div>");
        var term, text;
        if (objectListArgus.objectiveType == 1) {            //我的目标
            term = v.confirmUser;
            text = "确认人： " + v.confirmUserName;
            if (v.authorizedUser != null && loginId != v.authorizedUser) {                              //如果有授权给别人的情况
                text = text + " | 被授权人: " + v.authorizedUserName;
            }
        } else {
            term = v.responsibleUser;
            text = "责任人： " + v.responsibleUserName;
        }

        var timeValue = v.startTime.replace('T', ' ').substr(0, 10) + "至" + v.endTime.replace('T', ' ').substr(0, 10);
        var $ul = $("<ul  class='list-unstyled'><li term=" + v.objectiveId + ">目标名称：<span class='textOverFlow' style='max-width: 76%' title=" + v.objectiveName + ">" + v.objectiveName + "</span></li>" +
            "<li>起止时间：<span  class='textOverFlow' style='max-width: 76%' title=" + timeValue + ">" + timeValue + "</span></li>" +
            "<li>奖励基数：" + (v.bonus == null ? '' : v.bonus) + "</li><li term=" + term + ">" + text + "</li></ul>");


        //var $ul  =$("<ul  class='list-unstyled'><li term="+ v.objectiveId +">目标名称：<span class='textOverFlow' title="+ v.objectiveName +">"+ v.objectiveName +"</span></li><li>起止时间："+ v.statTime +" - "+ v.statTime +"</li>" +
        //    "<li>奖励基数："+ v.bonus +"</li><li term="+ term +">"+ text +"</li></ul>");
        $objectRunOne.append($ul);

        var $objectRunTwo = $("<div class='col-xs-3 runMode objectRunTwo'><ul  class='list-unstyled'><li>理想值：" + v.expectedValue + "</li>" +
            "<li>指标值：" + v.objectiveValue + "</li><li term=" + v.checkType + ">考核类型：" + checkTypeArray[v.checkType-1] + "</li></ul></div>");

        var $weight = $("<div class='objectWeight pull-right'>" + (v.weight == null ? "" : ("权重：" + v.weight + "%")) + "</div>");

        //操作条  1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成
        var $opera = $(" <div class='operateDiv'><ul class='list-inline pull-right' term=" + v.objectiveId + "></ul></div>");
        var $liPower = $("<li class='liPower' > 授权</li>"),
            $liCommit = $("<li class='liCommit'>提交确认</li>"),
            $liDel = $("<li class='liDel' term=" + v.objectiveId + ">删除</li>"),
            $liRevocation = $("<li class='liRevocation' term=" + v.objectiveId + " >撤销</li>"),
            $liModify = $("<li class='liModify'>修改</li>"),
            $liOpen = $("<li class='liOpen'>展开</li>"),
            $liSubmit = $("<li class='liCommit' term=" + v.objectiveId + ">提交</li>"),
            $liDetail = $("<li class='liDetail' term=" + v.objectiveId + ">详情</li>"),
            $liPowerCancel = $("<li class='liPowerCancel' term=" + v.objectiveId + ">取消授权</li>");

        //撤销
        $liRevocation.click(function () {
            revokeDelCancel($(this), "撤销", flag);
        });

        //删除
        $liDel.click(function () {
            revokeDelCancel($(this), "删除", flag);
        });

        //取消授权
        $liPowerCancel.click(function () {
            revokeDelCancel($(this), "取消授权", flag);
        });

        //授权事件
        $liPower.click(function () {
            personModal = 2;
            object_info = v;
            $activeNode = $(this);
            $("#HR_modal").modal("show");
        });

        //提交
        $liSubmit.click(function () {
            newOrDecomposition = 3;
            $("#objectiveNew_modal").modal('show');
            newInit(v.objectiveId, (loginId == v.authorizedUser ? 1 : 2));
        });

        //详情事件
        $liDetail.click(function () {
            detailFlag = 1;
            object_info = v;
            $("#object_detail_modal").modal("show");
        });

        //修改事件
        $liModify.click(function () {
            if (loginId == v.responsibleUser && v.status == 3) {
                $("#objectModifyRole").hide();
            } else {
                $("#objectModifyRole:hidden").show();
            }
            $activeNode = $(this);
            $("#object_modify_modal").modal("show");
        });

        //提交确认事件
        $liCommit.click(function () {
            detailFlag = 4;
            $activeNode = $(this);
            object_info = v;
            $("#object_detail_modal").modal("show");
        });

        //展开事件
        $liOpen.click(function () {
            object_unfoldVar.objectiveId = v.objectiveId;
            object_unfoldVar.responsibleUser = v.responsibleUser;
            object_unfoldVar.confirmUser = v.confirmUser;
            object_unfoldVar.authorizedUser = v.authorizedUser;

            //  object_info = v;
            $unfoldNode = $(this);
            $("#object_unfold_modal").modal("show");
        });
        //操作条
        if (loginId == v.responsibleUser || loginId == v.authorizedUser) {        //我的目标     1、登陆用户为目标责任人  或者登陆用户为目标被授权人
            if (v.authorizedUser == null || loginId == v.authorizedUser) {                 //1、登陆用户为目标责任人且目标未授权给他人的场合： 3、登陆用户为目标被授权人的的场合：
                if (v.status == 1) {                                            //待提交：授权、提交、删除、展开、详情。
                    if (v.authorizedUser == null)
                        $($opera).find("ul").append([$liPower, $liSubmit, $liDel, $liOpen, $liDetail]);
                    else
                        $($opera).find("ul").append([$liSubmit, $liDel, $liOpen, $liDetail]);
                } else if (v.status == 2 || v.status == 4) {                    // 待审核：撤销、展开、详情。 1-4、待确认：撤销、展开、详情。
                    $($opera).find("ul").append([$liRevocation, $liOpen, $liDetail]);
                } else if (v.status == 3 || v.status == 6) {                   //进行中（已超时）：修改、提交、展开、详情
                    $($opera).find("ul").append([$liModify, $liCommit, $liOpen, $liDetail]);
                } else {                                                            //已完成
                    $($opera).find("ul").append([$liOpen, $liDetail]);
                }
            } else {                                                              //a： 取消授权、展开、详情。
                $($opera).find("ul").append([$liPowerCancel, $liOpen, $liDetail]);
            }
        } else if (loginId == v.confirmUser) {                        //下属目标
            var $liSure = $("<li class='liSure'>确认</li>"),
                $liCheck = $("<li class='liChecks' >审核</li>");

            //确认事件
            $liSure.off("click");
            $liSure.click(function () {
                detailFlag = 3;
                object_info = v;
                $activeNode = $(this);
                $("#object_detail_modal").modal("show");
            });


            //审核事件
            $liCheck.off("click");
            $liCheck.click(function () {
                detailFlag = 2;
                object_info = v;
                $activeNode = $(this);
                $("#object_detail_modal").modal("show");
            });

            if (v.status == 2) {            // 待审核：修改、审核、展开、详情。
                $($opera).find("ul").append([$liModify, $liCheck, $liOpen, $liDetail]);
            } else if (v.status == 3 || v.status == 6) {                //进行中（已超时）：修改、分解、展开、详情。
                $($opera).find("ul").append([$liModify, $liOpen, $liDetail]);
            } else if (v.status == 4) {        //待确认：确认、展开、详情。
                $($opera).find("ul").append([$liSure, $liOpen, $liDetail]);
            } else {          //已完成：展开、详情。
                $($opera).find("ul").append([$liOpen, $liDetail]);
            }
        } else {                   //其余情况
            $($opera).find("ul").append($liDetail);
        }
        $main.append([$objectRunOne, $objectRunTwo, $weight, $opera]);
        $row.append($main);
        return $row;
    }

    //加载子目标列表
    function loadChildObjectList(vlaue, objectId) {
        var $childList = $("<div class='objectChild' term=" + objectId + "></div>");
        if ($(vlaue).width() < 700) {
            alert($(vlaue).height());
            $childList.css("margin-left", "0px");
        }
        $.ajax({
            type: "post",
            url: "/ObjectiveIndex/GetChildrenObjectiveList",
            dataType: "json",
            data: { objectiveId: objectId },
            success: rsHandler(function (data) {
                $.each(data.ObjectiveList, function (i, v) {
                    $($childList).append(PLChunk(v, 2));
                })
                $(vlaue).after($childList);
                $($childList).animate({
                    height: 'toggle'
                });
            })
        });
    }


    //撤销事件 删除事件  取消授权事件 parentChild=1表示是第一层目标事件，否则表示子目标事件
    function revokeDelCancel(value, flag, parentChild) {
        var objectId = parseInt($(value).attr("term"));
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
                    data: { objectiveId: objectId },
                    success: rsHandler(function (data) {
                        if (data) {
                            ncUnits.alert(flag + "成功!");
                            if (parentChild == 1) {                     //第一层目标
                                statusCount();
                                loadObjectList();
                                drawPlanProgress();       //刷新饼图
                            } else if (parentChild == 2) {                 //第二层目标
                                var objectId = parseInt($(value).closest(".objectChild").attr("term"));
                                var parent = $(value).closest(".objectChild").prev();
                                $(value).closest(".objectChild").remove();
                                statusCount();        //刷新状态统计
                                loadChildObjectList(parent, objectId);
                                drawPlanProgress();
                            } else if (parentChild == 3) {           //展开中子目标的删除
                                unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
                            }
                        }
                        else
                            ncUnits.alert(flag + "失败!");
                    })
                });
            }
        });
    }


    /*弹窗 开始*/
    //------------------组织架构
    var department_modal;
    $('#department_modal').on('show.bs.modal', function () {
        $("#department_modal_chosen_count").html(objectListArgus.department.length);
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            success: rsHandler(function (data) {
                department_modal = $.fn.zTree.init($("#department_modal_folder"), $.extend({
                    callback: {
                        onNodeCreated: function (e, id, node) {
                            if ($.inArray(node.id, objectListArgus.department) >= 0) {
                                department_modal.checkNode(node, true, false);
                                var $checked = $("<li term=" + node.id + "><span>" + node.name + "</span></li>"),
                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                $("#department_modal_chosen").append($checked.append($close));
                                $close.click(function () {
                                    department_modal.checkNode(node, undefined, undefined, true);
                                    $(this).parent().remove();
                                });
                                node.mappingLi = $checked;
                            }
                        },
                        beforeClick: function (id, node) {
                            department_modal.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {
                            $("#department_modal_chosen_count").html(department_modal.getCheckedNodes().length);
                            if (node.checked) {
                                var $checked = $("<li term=" + node.id + "><span>" + node.name + "</span></li>"),
                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                $("#department_modal_chosen").append($checked.append($close));
                                $close.click(function () {
                                    department_modal.checkNode(node, undefined, undefined, true);
                                    $(this).parent().remove();
                                });
                                node.mappingLi = $checked;
                            } else {
                                $(node.mappingLi).remove();
                            }
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false
                    },
                    check: {
                        enable: true,
                        chkStyle: "checkbox",
                        chkboxType: { "Y": "", "N": "" }
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

    //组织架构弹窗关闭事件
    $('#department_modal').on("hide.bs.modal", function () {
        $("#department_modal_chosen_count").text(0);
        $("#department_modal_chosen").empty();
    });

    $("#department_modal_submit").click(function () {
        circleClean();
        objectListArgus.department.length = 0;
        $(".selectedCondition li span[classify='组织']").parent().remove();
        $('#department_modal_chosen li').each(function () {
            objectListArgus.department.push(parseInt($(this).attr('term')));
            addSelects($(this).find('span:eq(0)').text(), $(this).attr('term'), "组织");
            $('#department_modal').modal('hide')
        });
        loadObjectList();
        //statusCount();
        //drawPlanProgress();       //刷新饼图
    });

    //组织架构搜索
    $("#department_modal_search").selection({
        url: "/Shared/GetOrgListByName",
        hasImage: false,
        selectHandle: function (data) {
            $("#department_select").val(data.name);
            var n = department_modal.getNodeByParam("id", data.id);
            if (n && !n.checked) {
                department_modal.checkNode(n, undefined, undefined, true);
            } else {
                var flag = true;
                if ($("#department_modal_chosen li").length > 0) {
                    $("#department_modal_chosen li").each(function () {
                        if ($(this).attr('term') == data.id) {
                            flag = false;
                        }
                    });
                }

                if (flag == true) {
                    var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                               $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                    $("#department_modal_chosen").append($checked.append($close));
                    $close.click(function () {
                        var nodeId = $(this).parent().attr("term");
                        n = department_modal.getNodeByParam("id", parseInt(nodeId));
                        if (n) {
                            department_modal.checkNode(n);
                        }
                        $(this).parent().remove();
                        $("#department_modal_chosen_count").text($("#department_modal_chosen li").length);
                    });
                }
            }
            $("#department_modal_chosen_count").text($("#department_modal_chosen li").length);
        }
    });

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

    //包含下级
    $("#HR-haschildren").click(function () {
        if (personOrgId==null) {
            ncUnits.alert("请选择部门!");
            return;
        }
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
        else {
            $(".person_list ul").each(function () {
                var term = $(this).find("li:eq(1)").attr("term");
                $("#HR_modal_chosen li").each(function () {
                    if ($(this).attr('term') == term) {
                        $(this).remove();
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
        if (personModal == 1) {
            circleClean();
            $(".selectedCondition li span[classify='人员']").parent().remove();
            objectListArgus.length = 0;
            objectListArgus.person = [];
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
                        data: { objectiveId: object_info.objectiveId, authorizedUser: personId },
                        success: rsHandler(function (data) {
                            ncUnits.alert("授权成功!");
                            $('#HR_modal').modal('hide');
                            statusCount();
                            var parent = $($activeNode).closest(".objectChild");
                            if (parent.length != 0) {            //表示刷新授权目标所在层
                                loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                                $(parent).remove();
                            } else {             //表示刷新最顶层
                                loadObjectList();
                            }
                            drawPlanProgress();       //刷新饼图
                        })
                    })
                }
            }
        }
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


    //------------------目标详情

    var fieldArray = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "权重", "奖励基数", "数字"];
    var FormulaList, formulaType, maxValue, minValue;
    $("#object_detail_modal").on('show.bs.modal', function () {
        objectDetail(object_info.objectiveId);

    });

    $("#object_detail_modal").on('shown.bs.modal', function () {
        if (detailFlag == 2) {       //如果是审核
            if (loadFormaluFlag) {
                $("#FormularViewTab .modal-body").empty();
                FormaluView(FormulaList, maxValue, minValue, formulaType, $("#FormularViewTab .modal-body"));
            }
        }
    })



    $("#object_detail_modal").on('hide.bs.modal', function () {
        $("#FormularViewTab").removeClass("active");
        $("#detailViewTab").addClass("active");
        $("#object_detail_modal .rightModal").hide();
        FormulaList = null;
        $("#object_detail_modal .modal-body").empty();
        FormulaArray.length = 0;
        loadFormaluFlag = true;
    });

    function objectDetail(objectiveId) {
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
                    "<li  class='col-xs-4'><label>结束时间：</label><span>" + data.endTime.replace('T', ' ').substr(0, 10) + "</span></li></ul>");
                var $contentThree = $(" <ul class='list-inline row borderTop'>" +
                    "<li  class='col-xs-4'><label>责任人：</label><span>" + data.responsibleUserName + "</span></li>" +
                    "<li  class='col-xs-4'><label>确认人：</label><span>" + data.confirmUserName + "</span></li>");
         
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
                    //if (ChangeInfo.alarmTimeUpdate == 1) {
                    //    $($contentTwo).find("li:eq(2)").addClass("changeColor");
                    //}
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
                        $("#object_detail_modal .rightModal").hide();
                        detailFlag = 1;
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
                        "<div class='operateMessage textOverFlow' title=" + v.message  + ">意见 : " + v.message + "</div></td>" +
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
                if (!data.keyword) {
                    $contentThree.append("<li  class='col-xs-12'><label>标签：</label></li></ul>")

                } else {
                    $contentThree.append("<li  class='col-xs-12'><label>标签：</label><span>" + data.keyword.join(',') + "</span></li></ul>")
                }
                if (detailFlag == 2) {            //目标审核
                    checksFunc(data);
                } else if (detailFlag == 3) {                         //目标确认
                    objectSureFunc(data);
                } else if (detailFlag == 4) {               //目标的提交
                    //  $("#object_detail_modal .modal-footer").hide();
                    objectCommit(data.checkType, data.objectiveValue);
                }
            })
        });
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
            preview(7,saveName,extension);
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
            for (i = 0; i < FormulaList.length;i++){
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
                            var $input = $("<input type='number'  style='width:97%' class='form-control' value="+ v.numValue + " disabled >");
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

    //目标详情
    $("#objectDetailLabel").click(function () {
        $("#FormularViewTab").removeClass("active");
        $("#detailViewTab").addClass("active");
        objectDetail(object_info.objectiveId);
    });

    //目标规则
    $("#objectRoleLabel").click(function () {
        $("#detailViewTab").removeClass("active");
        $("#FormularViewTab").addClass("active");
        $("#objectDetailLabel").addClass("disabledColor");
        $("#objectRoleLabel").removeClass("disabledColor");
        if (detailFlag != 2) {       //如果是审核
            objectFormula();
        } 
    });

    //设置右侧弹出框的位置
    $(".rightModal").css({
        "left": ($(".rightModal").parents('.modal-content').width() - 5)
    });

    //判断数据库中公式设置是否正确
    function rightFormula() {
        if (formulaType == 1 && (maxValue == null || minValue == null)) {
            return false;
        } else if( formulaType == 2 && ( FormulaList==null || FormulaList.length==0 ) ) {
            return false;
        } else if (formulaType==null) {
            return false;
        }
        return true;

    }
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
                    if (objectiveFormula.formula == null || isNaN(objectiveFormula.formula)  || (objectiveFormula.formula == 2 && FormulaArray.length == 0)) {
                        ncUnits.alert("审核通过必须设置公式！");
                        return;
                    }
                }
                if (objectiveFormula.formula == 1) {
                    var confirmText = null;
                    if ($("#maxValue").val() == "") {
                        if ( rightFormula() == true) {
                            confirmText = "最大奖励系数不得为空!是否恢复之前公式?";
                        } else {
                            ncUnits.alert("最大奖励系数不得为空！");
                            return;
                        }
                    } else if ($("#minValue").val() == "") {
                        if ( rightFormula()==true ) {
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
                        if (rightFormula() == true ) {                  //之前设置公式正确 才可恢复
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
                        }else {
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
                    if (data.result==0) {
                        ncUnits.alert("审核成功！");
                        $("#object_detail_modal").modal('hide');
                        statusCount();
                        var parent = $($activeNode).closest(".objectChild");
                        if (parent.length != 0) {            //表示刷新授权目标所在层
                            loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                            $(parent).remove();
                        } else {             //表示刷新最顶层
                            loadObjectList();
                        }
                        drawPlanProgress();
                    } else if(data.result==-1){
                        ncUnits.alert("审核失败！");
                    }
                    else {
                        ncUnits.alert("第"+data.result+"个目标公式设置有误！");
                    }
                })
            })
        }
        $("#rightContent").empty().append([$text, $passBtn[0], $passBtn[1]]);
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
                            statusCount();
                            var parent = $($activeNode).closest(".objectChild");
                            if (parent.length != 0) {            //表示刷新授权目标所在层
                                loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                                $(parent).remove();
                            } else {             //表示刷新最顶层
                                loadObjectList();
                            }
                            drawPlanProgress();
                        } else {
                            ncUnits.alert("确认失败!");
                        }
                    })
                })
            });
        });
        $("#rightContent").empty().append([$value, $text, $passBtn[0], $passBtn[1]]);
    }

    //目标提交
    function objectCommit(checkType, objectiveValue) {
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
            var $input = $('<input type="text" class="form-control"  maxlength="20">');
            $input.bind('input', function () {
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
            } else if (actualValue == objectiveValue)
            {
                ncUnits.alert("实际值与指标值不能相等");
                return;
            }
            $.ajax({
                type: "post",
                url: "/ObjectiveIndex/SubmitObjectiveExecuteResult",
                dataType: "json",
                data: { objectiveId: object_info.objectiveId, actualValue: actualValue },
                success: rsHandler(function (data) {
                    if (data) {
                        ncUnits.alert("提交成功！");
                        $("#object_detail_modal").modal('hide');
                        statusCount();
                        var parent = $($activeNode).closest(".objectChild");
                        if (parent.length != 0) {            //表示刷新授权目标所在层
                            loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                            $(parent).remove();
                        } else {             //表示刷新最顶层
                            loadObjectList();
                        }
                        drawPlanProgress();
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

    //----------------------目标修改

    var modifyOldOrg = -1, modifyOldPerson = -1, modifyOldPOrg = -1, perWithSub = 0, modifyOldPOrgName = null;

  
    $("#object_modify_modal").on('shown.bs.modal', function () {
        if (loadFormaluFlag && $("#objectModifyRole").is(':visible') ) {
            $("#modifyFormularView .modal-body").empty();
            FormaluView(FormulaList, maxValue, minValue, formulaType, $("#modifyFormularView .modal-body"));
        }
    })
    $("#objectModifyTab").click(function () {
        if ( $(this).hasClass("disabledColor")) {
            $(this).removeClass("disabledColor");
            $("#objectModifyRole").addClass("disabledColor");
        } 
    });
    $("#objectModifyRole").click(function () {
        if ($(this).hasClass("disabledColor")) {
            $(this).removeClass("disabledColor");
            $("#objectModifyTab").addClass("disabledColor");
        }
    });
    $("#object_modify_modal").on('show.bs.modal', function () {
        objectiveModify($($activeNode).closest("ul").attr("term"));
        documentUpload($("#fileUpload"), $("#object_modify_modal .progress"), $("#modifyObjectDocument"), $("#modifyObjectLog"));
    });

    //目标修改弹框关闭 值清空
    $("#object_modify_modal").on('hide.bs.modal', function () {
        lab.empty();
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

    $("#modifyCheckType li a").each(function () {
        $(this).click(function () {
            var type = parseInt($(this).attr("term"));
            $(".modifyObjectValue > *").each(function () {
                $(this).hide();
            });
            if (type == 1) {         //金额
                $("#targetMoney:hidden").parent().show();
                $("#expectMoney:hidden").parent().show();
            } else if (type == 2) {       //时间
                $("#targetTime:hidden").parent().show();
                $("#expectTime:hidden").parent().show();
            } else {           //数字
                $("#targetNumber:hidden").show();
                $("#expectNumber:hidden").show();
            }
            dropDown(this);
        });
    });


    timeThree("#expectTime", null, null, null, null);
    timeThree("#targetTime", null, null, null, null);



    //$(" #modifyStartTime,#modifyEndTime,#modifyAlertTime").click(function () {
    //    var id = "#" + $(this).attr("id");
    //    timeThree(id);
    //});

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


    $("#modifyWeight ,#new-weight").blur(function () {
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

    $("#modifyWeight,#new-weight").focus(function () {
        var value = $(this).val();
        if (value.indexOf("%") > 0) {
            value = value.substring(0, value.length - 1);
        }
        $(this).val(value);
    });

    //目标修改
    function objectiveModify(objectId) {
        $("#object_modify_modal input").val("");
        $("#object_modify_modal table").empty();
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
                lab.template(data.keyword, '.label-modify');
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
                //$("#modifyAlertTime").val(data.alarmTime == null ? '' : data.alarmTime.replace('T', ' ').substr(0, 10));
                $("#responsePerson").val(data.responsibleUserName);
                $("#responsePerson").attr("term", data.responsibleUser);
                $("#confirmPerson").val(data.confirmUserName);
                $("#confirmPerson").attr("term", data.confirmUser);

                //时间函数
                $(" #modifyStartTime,#modifyEndTime").off("click")
                $(" #modifyStartTime,#modifyEndTime").click(function () {
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
                    $("#responsePerson").next().attr("disabled", true);
                    $("#confirmPerson").attr("disabled", true);
                    $("#confirmPerson").next().attr("disabled", true);
                }
                if (data.objectiveType == 2) {
                    $("#responsePerson").attr("disabled", true);
                    $("#responsePerson").next().attr("disabled", true);
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

    //责任人
    $("#responsePerson").click(function () {
        personChoose(2);
    });

    $("#responsePerson").next().click(function () {
        personChoose(2);
    });

    //确认人
    $("#confirmPerson").click(function () {
        personChoose(3);
    });
    $("#confirmPerson").next().click(function () {
        personChoose(3);
    });

    //判断某变量是否具有非法字符
    function justifyByLetter(txt, name) {
        var reg = /[`~!@#$%^&*()_+<>?:"{},\/;'[\]]/im;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            ncUnits.alert(name);
            return false;
        }
        return true;
    }

    $("#objectModifySave,#objectModifyCommit").off("click");
    $("#objectModifySave,#objectModifyCommit").click(function () {
        modifySave(parseInt($(this).attr("term")));
        unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
        if (unflodFlag == 1) {
            unflodFlag = 2;
        }
    });

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
            //alarmTime: Date,              //警戒时间
            responsibleOrg: String,              //责任部门     
            responsibleUser: Number,             //责任人
            confirmUser: Number,                 //确认人
            status: Number,                      //状态 1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成
            message: String,               //意见
            objectiveFormulaInfo:null           //公式

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
        if( flag== 2){         //提交必须设置公式
            if (objFormulaInfo.formula == null || isNaN(objFormulaInfo.formula) || (objFormulaInfo.formula == 2 && FormulaArray.length == 0)) {
                ncUnits.alert("提交必须设置公式！");
                return;
            }
        }
        if (objFormulaInfo.formula == 1) {
            var confirmText = null;
            if ($("#maxValue").val() == "" && flag == 2) {
                if (  rightFormula() == true ) {
                    confirmText = "最大奖励系数不得为空!是否恢复之前设置?";
                } else {
                    ncUnits.alert("最大奖励系数不得为空！");
                    return;
                }
            } else if ($("#minValue").val() == "" && flag == 2) {
                if (  rightFormula() == true ) {
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
                        objFormulaInfo.objectiveFormula =FormulaList;
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
            //获取标签
            argus.keyword = lab.get('#object_modify_modal');
            argus.objectiveFormulaInfo = objFormulaInfo;
            var objectSpan = $("#modifyObjectInput span:eq(0)");
            var objectName = $("#objectName");
            argus.objectiveId = parseInt(objectName.attr("term"));
            argus.parentObjective = parseInt(objectName.attr("parentId"));
            argus.displayChangeFlag = parseInt(objectName.attr("change"));
            argus.objectiveName = $.trim(objectName.val());
            argus.objectiveType = parseInt($(objectSpan).attr("term"));
            if ($.trim($("#objectBonus").val())=="") {
                ncUnits.alert("奖励基数不能为空!");
                return;
            }
            argus.bonus = parseInt($("#objectBonus").val());
            if ($("#modifyWeight").val() == "") {
                ncUnits.alert("目标权重值不能为空!");
                return;
            } else if (parseInt($("#modifyWeight").val())>100) {
                ncUnits.alert("目标权重值不能超过100!");
                return;
            }
            argus.weight = parseInt($("#modifyWeight").val());
            argus.checkType = parseInt($("#modifyCheckType span:eq(0)").attr("term"));
            if ($.trim($("#targetInput input:visible").val())== "") {
                ncUnits.alert("指标值不能为空!");
                return;
            }
            argus.objectiveValue = $.trim($("#targetInput input:visible").val());
            if ($.trim($("#expectInput input:visible").val())=="") {
                ncUnits.alert("理想值不能为空!");
                return;
            }
            argus.expectedValue = $.trim($("#expectInput input:visible").val());
            argus.description = $.trim($("#modifyRemark").val());
            if ($("#modifyStartTime").val()=="") {
                ncUnits.alert("开始时间不能为空!");
                return;
            }
            argus.startTime = $("#modifyStartTime").val();
            if ($("#modifyEndTime").val()=="") {
                ncUnits.alert("结束时间不能为空!");
                return;
            }
            argus.endTime = $("#modifyEndTime").val();
            //if ($("#modifyAlertTime").val()=="") {
            //    ncUnits.alert("警戒时间不能为空!");
            //    return;
            //}
            //argus.alarmTime = $("#modifyAlertTime").val();
            if (argus.objectiveType == 1) {
                argus.responsibleOrg = $(objectSpan).attr("chooseId");
            } else {
                argus.responsibleOrg = $(objectSpan).attr("data_orgName");
            }
            if ($("#responsePerson").val()=="") {
                ncUnits.alert("责任人不能为空!");
                return;
            }
            argus.responsibleUser = parseInt($("#responsePerson").attr("term"));
            if ($("#confirmPerson").val()=="") {
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
                    if (data==0) {
                        ncUnits.alert("修改成功!");
                        $("#object_modify_modal").modal("hide");
                        if (!$activeNode.closest("ul").hasClass("unfoldChildOpera")) {
                            statusCount();
                            var parent = $($activeNode).closest(".objectChild");
                            if (parent.length != 0) {            //表示刷新授权目标所在层
                                loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                                $(parent).remove();
                            } else {             //表示刷新最顶层
                                loadObjectList();
                            }
                            drawPlanProgress();
                        } else {
                            object_unfoldVar.objectiveId = argus.parentObjective;
                            unfoldObjectCall(null, hrHorizontal);
                        }
                    } else if(data==-1) {
                        ncUnits.alert("修改失败!");
                    }
                    else {
                        ncUnits.alert("第"+data+"个目标公式设置有误!");
                    }
                })
            })
        }
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
                        "<div class='operateMessage textOverFlow' title=" + v.message  + ">意见 : " + v.message + "</div></td>" +
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



    //----------------------目标展开
    var unflodFlag = 0;
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

    //输入控制
    $("#modifyWeight,#expectMoney,#targetMoney,#objectBonus,#new-objectiveValue-money,#new-expectedValue-money ,#new-weight,#new-bonus").bind("input", function () {
        controlInput($(this));
    });

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


    $("#object_unfold_modal").on('show.bs.modal', function () {
        unflodFlag = 1;
        unfoldObjectCall(null, hrHorizontal);
    });

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

    //模态框关闭时 刷新目标列表
    $("#object_unfold_modal").on('hide.bs.modal', function () {
        $("#object_unfold_modal .unfoldChild:eq(1)").remove();
        if (unflodFlag == 2) {
            statusCount();
            var parent = $($unfoldNode).closest(".objectChild");
            if (parent.length != 0) {            //表示刷新授权目标所在层
                loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                $(parent).remove();
            } else {             //表示刷新最顶层
                loadObjectList();
            }
            drawPlanProgress();
            unflodFlag = 0;
        }
    });

    //目标展开
    function unfoldObjectFunc(children, $parent) {
        if (children.length == 0) {
            return;
        }
        $.each(children, function (i, v) {
            var times = (v.startTime == null ? '' : v.startTime.substring(0, 10)) + '~' + (v.endTime == null ? '' : v.endTime.substring(0, 10));
            console.log('v', v)
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

            var $liDel = $("<li class='liDel' term=" + v.objectiveId + " child=" + v.childObjectiveList.length + ">删除</li>"),
                 $liModify = $("<li class='liModify'>修改</li>"),
                 $liDetail = $("<li class='liDetail' term=" + v.objectiveId + ">详情</li>");

            //删除
            $liDel.click(function () {
                revokeDelCancel(this, "删除", 3);
                unflodFlag = 2;
            });
            //详情事件
            $liDetail.click(function () {
                detailFlag = 1;
                object_info = v;
                $("#object_detail_modal").modal("show");
            });
            //修改事件
            $liModify.click(function () {
                $activeNode = $(this);
                $("#object_modify_modal").modal("show");
            });

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

    $(".oneBlock .modifyDocumentAdd").click(function () {
        addChildObject(this);
    });

    $(".oneBlock .slideBtn ").click(function () {
        slideBtnClickFunc(this);
    });

    //展开收起子节点
    function slideBtnClickFunc(values) {
        $(values).toggleClass("glyphicon-triangle-top");
        $(values).toggleClass("glyphicon-triangle-bottom");
        $(values).closest(".oneChild").find(".unfoldChild:eq(0)").slideToggle("fast", function () {
            hrHorizontal($(values).closest(".oneChild").parents(".unfoldChild"));
        });
        hrHorizontal($(values).closest(".oneChild").parents(".unfoldChild"));
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

    //----------展开结束--------------//

    //新建目标
    $("#newObjectiveBtn").click(function () {
        newOrDecomposition = 1;
        $("#objectiveNew_modal").modal('show');
        newInit();
    });



    //----------------------------------------------------娇娇---------------------------------------------------//
    var operateFlag = null;//区分两种提交
    var loadFormaluFlag = true;
    var objectiveRuleSureFlag = true;
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
        //alarmTime: null,              //警戒时间
        responsibleOrg: null,              //责任部门
        responsibleUser: null,             //责任人
        confirmUser: null,                 //确认人
        formula: null,                     //公式 0：无公式 1：默认公式 2：自定义
        maxValue: null,
        minValue: null,
        objectiveFormula: []             //目标公式为自定义的场合
    };

    timeThree(".new-objectiveValue-date", null, null, null, null);
    timeThree(".new-expectedValue-date", null, null, null, null);

    $(" #new-startTime, #new-endTime").click(function () {
        var id = "#" + $(this).attr("id");
        timeThree(id, null, null, null, null);
    });



    var FormulaArray = new Array();//全局变量存储公式
    var formulaEditOrAdd = null;//标记当前为公式编辑还是添加状态
    var formulaNum = 0;//公式编号
    var formulaSetInput = [[],[]];//存放当前编辑的公式
    var newOrDecomposition;//全局变量，区分是新建还是分解新建：1 新建：2 分解 3：保存后再次点击详情再次打开
    var parentObjectiveStartTime, parentObjectiveEndTime;//父目标开始时间，父目标结束时间

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

   
    //请求后台标签数据
    tags(lab.autocomplete)
    $('.addLabel,.editLabel').on('click', function () {
        lab.add(this);
    })

    //新建模态框关闭     值清空
    $("#objectiveNew_modal").on('hide.bs.modal', function () {
        lab.empty();
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
        $("#new-startTime,#new-endTime").val("");

        $("#new-responsibleUser,#new-confirmUser,#new-description").val("");
        $("#new-responsibleUser,#new-confirmUser").attr("title","");
        $("#new-responsibleUser,#new-confirmUser").attr("term", "");
        $("#new-responsibleUser,#new-confirmUser").prop("disabled", false);
        $("#new-responsibleUser,#new-confirmUser").siblings("a").removeClass("disabled");
        $("#formula_modal_monitor").text("");

    });
    /*$(".modal-right-content").css({
        "left": ($("#objectiveNew_modal").find('.modal-left-content').width() - 5)
    });*/

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
                            $("#new-responsibleUser").attr("title",$(this).siblings("span").text());
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
                            $("#new-confirmUser").attr("title",$(this).siblings("span").text());
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
    //新建或分解数据保存
    function newModalDataSave(flag) {
        //获取标签
        Returndata.keyword = lab.get('#objectiveNew_modal');
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
        if ($("#new-weight").val()!=""&&parseInt($("#new-weight").val()) > 100) {
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
            if ($("#new-weight").val()== "") {
                ncUnits.alert("目标权重值不能为空!");
                return;
            }
            else if (parseInt($("#new-weight").val())>100) {
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
        if (Returndata.expectedValue == Returndata.objectiveValue)
        {
            ncUnits.alert("理想值与指标值不能相等");
            return false;
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
                    if (data==0) {
                        ncUnits.alert("新建目标成功");
                        $("#objectiveNew_modal").modal("hide");
                        statusCount();
                        loadObjectList();
                        drawPlanProgress();
                    } else if (data==-1) {
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
                    if (data==0) {
                        ncUnits.alert("分解目标成功");
                        $("#objectiveNew_modal").modal("hide");
                        unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
                        if (unflodFlag == 1) {
                            unflodFlag = 2;
                        }
                    }
                    else if (data==-1) {
                        ncUnits.alert("分解目标失败");
                        $("#objectiveNew_modal").modal("hide");
                    }
                    else {
                        ncUnits.alert("第"+data+"个目标公式设置有误!");
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
                    if (data==0) {
                        ncUnits.alert("操作成功");
                        $("#objectiveNew_modal").modal("hide");
                        statusCount();
                        loadObjectList();
                        drawPlanProgress();
                    }
                    else if (data==-1) {
                        ncUnits.alert("操作失败");
                        $("#objectiveNew_modal").modal("hide");
                    }
                    else {
                        ncUnits.alert("第"+data+"个目标公式设置有误!");
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

    $("#new-objectiveValue-date,#new-expectedValue-date,#new-startTime,#new-endTime").siblings("a").click(function () {
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
                var x=true;
                formulaSetInput[0].push(datas);
                formulaSetInput[1].push(x);
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

    var formulaSetCheck1 = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "权重", "奖励基数", "数字"];
    var formulaSetCheck2 = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "权重", "奖励基数", "数字", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];

    $(".areabox a").click(function () {
        if ($(this).attr("id") != "formula_modal_time") {
            $("#formula_modal_monitor").text($("#formula_modal_monitor").text() + $(this).attr("value"));
            var x=false;
            formulaSetInput[0].push($(this).attr("value"));
            formulaSetInput[1].push(x);
        }
        
    });
    $("#formula_modal_backspace").click(function () {
        formulaSetInput[0].pop();
        formulaSetInput[1].pop();
        $("#formula_modal_monitor").text(formulaSetInput[0].join(""));
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
        if (formulaSetInput[0].length == 0) {
            ncUnits.alert("您还没有输入公式");
            return;
        }
        if (rp1.test(formulaSetInput[0][0]) || (formulaCheck(formulaSetInput[0][0], formulaSetCheck) == true)) {
            //公式开头是运算符
            ncUnits.alert("公式错误");
            return;
        }
        if (rp1.test(formulaSetInput[0][formulaSetInput[0].length - 1]) || (formulaCheck(formulaSetInput[0][formulaSetInput[0].length - 1], formulaSetCheck) == true)) {
            //公式结尾是运算符
            ncUnits.alert("公式错误");
            return;
        }
        for (var i = 0; i < formulaSetInput[0].length; i++) {
            if (formulaSetInput[0][i] == ")") {
                if (leftBracketFlag == 0) {
                    //公式中先出现)
                    ncUnits.alert("公式括号不匹配");
                    return;
                }
                rightBracketFlag++;
            }
            if (formulaSetInput[0][i] == "(") {
                leftBracketFlag++;
            }
            if ((i + 1) < formulaSetInput[0].length && formulaCheck(formulaSetInput[0][i], formulaSetCheck) == true && formulaCheck(formulaSetInput[0][i + 1], formulaSetCheck) == true) {
                //公式中两个操作符连续出现
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput[0].length && formulaSetInput[0][i] == "(" && (formulaCheck(formulaSetInput[0][i + 1], formulaSetCheck) == true || rp1.test(formulaSetInput[0][i + 1]))) {
                //公式中（后面连续出现操作符
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput[0].length && formulaSetInput[0][i] == ")" && formulaSetInput[0][i + 1] == (".")) {
                //公式中)后面出现.
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput[0].length && (((formulaSetInput[0][i] == "且" || formulaSetInput[0][i] == "或") && formulaSetInput[0][i + 1] == (formulaSetInput[0][i] == "且" || formulaSetInput[0][i] == "或")))) {
                //公式中且与或连续出现
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput[0].length && ((formulaSetInput[0][i] == ")" && formulaSetInput[0][i + 1] == "(") || (formulaSetInput[0][i] == "(" && formulaSetInput[0][i + 1] == ")"))) {
                //公式中左右括号连续出现
                ncUnits.alert("公式错误");
                return;
            }
            if ((i + 1) < formulaSetInput[0].length && (formulaCheck(formulaSetInput[0][i], formulaSetCheck1) == true && formulaCheck(formulaSetInput[0][i + 1], formulaSetCheck2) == true || formulaCheck(formulaSetInput[0][i], formulaSetCheck2) == true && formulaCheck(formulaSetInput[0][i + 1], formulaSetCheck1) == true)) {
                //公式中两个操作数连续出现(不包括两个数字连续出现)
                ncUnits.alert("公式错误");
                return;
            }

            var rep = /[0-9]/;
            if ((i - 1) >= 0 && formulaSetInput[0][i] == "." && rep.test(formulaSetInput[0][i - 1]) == false) {
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

        var formulaSet = [[],[]];//将输入数据进行处理
        for (var i = 0; i < formulaSetInput[0].length; i++) {
            if (rep.test(formulaSetInput[0][i])) {
                if (number == null) {
                    number = formulaSetInput[0][i];
                }
                else {
                    number = number + formulaSetInput[0][i];
                }
                if (i + 1 == formulaSetInput[0].length) {
                    formulaSet[0].push(number);
                    var  x=false;
                    formulaSet[1].push(x);
                }
            }
            else if (number != null && rep.test(formulaSetInput[0][i]) == false) {
                formulaSet[0].push(number);
                var  x=false;
                formulaSet[1].push(x);
                formulaSet[0].push(formulaSetInput[0][i]);
                formulaSet[1].push(formulaSetInput[1][i]);
                number = null;
            }
            else if (rep.test(formulaSetInput[0][i]) == false) {
                formulaSet[0].push(formulaSetInput[0][i]);
                formulaSet[1].push(formulaSetInput[1][i]);
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
            var $formulaContentTd = $("<td style='width:35%;'> <span class='formula-content'title='" + formulaSetInput[0].join("") + "'>" + formulaSetInput[0].join("") + "</span></td>");
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
                $this = $(this);
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
                                FormulaArray[i].formulaNum = FormulaArray[i].formulaNum - 1;
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
                for (var i = 0; i < FormulaArray.length; i++) {
                    if (FormulaArray[i].formulaNum == $(this).parents("tr").attr("term") && FormulaArray[i].formulaId != "z") {
                        formulaSetInput[1].push(FormulaArray[i].valueType);
                        if (FormulaArray[i].field != null) {
                            formulaSetInput[0].push(formulaSetCheck1[parseInt(FormulaArray[i].field) - 1]);
                        }
                        if (FormulaArray[i].operate != null) {
                            if (FormulaArray[i].operate == "&") {
                                formulaSetInput[0].push("且")
                            }
                            else if (FormulaArray[i].operate == "|") {
                                formulaSetInput[0].push("或")
                            }
                            else {
                                formulaSetInput[0].push(FormulaArray[i].operate)
                            }

                        }
                        if (FormulaArray[i].numValue != null) {
                            formulaSetInput[0].push(FormulaArray[i].numValue)
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
        }
        else {
            //如果是修改公式，则改变对应的公式
            $("#formula-set-content tbody tr").each(function () {
                if ($(this).attr("term") == formulaEditOrAdd) {
                    $(this).find(".formula-content").text(formulaSetInput[0].join(""));
                    $(this).find(".formula-content").attr("title", formulaSetInput[0].join(""));
                }
            })
        }

        for (var i = 0; i < formulaSet[1].length; i++) {
            var $objectiveFormula = {
                formulaId: null,        //公式ID
                formulaNum: null,       //公式编号
                orderNum: null,         //排序
                field: null,            //字段
                operate: "",       //操作符
                numValue: "",     //具体值
                valueType:""
            }
            if (formulaEditOrAdd != null) {
                $objectiveFormula.formulaNum = formulaEditOrAdd;
            }
            else { $objectiveFormula.formulaNum = formulaNum; }
            $objectiveFormula.orderNum = i + 1;
            $objectiveFormula.valueType = formulaSet[1][i];
            if (formulaCheck(formulaSet[0][i], ["(", ")", "+", "-", "*", ">", "/", "≥", ">", "≤", "且", "或"])) {
                $objectiveFormula.field = null;
                $objectiveFormula.numValue = null;
                if (formulaSet[0][i] == "且") {
                    $objectiveFormula.operate = "&";
                }
                else if (formulaSet[0][i] == "或") {
                    $objectiveFormula.operate = "|";
                }
                else {
                    $objectiveFormula.operate = formulaSet[0][i];
                }
               
            }
            else if (formulaCheck(formulaSet[0][i], formulaSetCheck1)) {

                for (var j = 0; j < formulaSetCheck1.length; j++) {
                    if (formulaSetCheck1[j] == formulaSet[0][i]) {
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
                $objectiveFormula.numValue = formulaSet[0][i];
               
            }
            FormulaArray.push($objectiveFormula);
        }
        formulaSetInput[0].length = 0;
        formulaSetInput[1].length = 0;
    });

    $("#formula_modal_clear").click(function () {
        formulaSetInput[0].length = 0;
        formulaSetInput[1].length = 0;
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
                    $("#new-confirmUser").attr("title",split_info.responsibleUserName);
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
                    $("#new-responsibleUser").attr("title",split_info.responsibleUserName);
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
                    lab.template(data.keyword, '.label-add');
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
                    $("#new-responsibleUser").attr("title",data.responsibleUserName);
                    $("#new-responsibleUser").attr("term", data.responsibleUser);

                    $("#new-confirmUser").val(data.confirmUserName);
                    $("#new-confirmUser").attr("title",data.confirmUserName);
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
                    if (data.objectiveFormula.length !=null&&data.objectiveFormula.length != 0) {
                        Returndata.objectiveFormula.length = 0;
                        for (var i = 0; i < data.objectiveFormula.length; i++) {
                            Returndata.objectiveFormula.push(jQuery.extend(true, {}, data.objectiveFormula[i]));
                        }

                    }

                })
            })


        }

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
    function dropDownEvent(value) {
        var x = $(value).parents("ul").prev().find("span:eq(0)");
        x.text($(value).text());
        var term = $(value).attr("term");
        x.attr("term", term);
    }


    /*弹窗 结束*/


    //---------------------------------甘特图-----------------------------------------//


});

