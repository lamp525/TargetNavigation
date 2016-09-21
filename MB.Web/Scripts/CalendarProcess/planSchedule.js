//@ sourceURL=planSchedule.js
/**
 * Created by DELL on 15-8-14.
 */
var listArgus = {               //计划列表加载变量
    calendarType: 1,            //日程类别： 1：我的日程 2：下属日程
    date: String,
    status: 1                   //计划状态：1：待审核计划  2：待确认计划（我的日程无该参数）
};

var page_flag = "CalendarProcess";    //页面标志，与计划页面区分开来
var current_Info;
var batch_planId = [], batch_loopId = [];
var batch_flag = false;

$(function () {
    //右侧个人信息
    loadPersonalInfo();
    //// 右侧圆饼
    //var date = new Date()
    //    , year = date.getFullYear()
    //    , month = date.getMonth() + 1
    //    , $con = $("#object_statistics");
    //var  colors = ["#be1d9a","#fbab11","#64d8ae","#57acdb","#58b557","#e02215"];   //1：待提交 2：待审核 3：审核通过 4：待确认 5：已完成 6、超时 int
    var monthArray = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    //
    //
    //function drawPlanProgress(){
    //    var lodi;//load弹出层
    //    $("#object_chart").empty();
    //    $(".chart-center",$con).empty();
    //    $.ajax({
    //        type: "post",
    //        url: "../../test/data/Objective/GetObjectiveStatusList.json",
    //        dataType: "json",
    //        data:{
    //            year:year,
    //            month:month
    //        },
    //        beforeSend:function(){
    //            lodi=getLoadingPosition('.chart-container');//显示load层
    //        },
    //        success:rsHandler(function(data){
    //            console.log("success");
    //            var dountData = [] ,count = 0 ,$ul =  $(".chart-label",$con);
    //            $ul.empty();
    //            for (var i = 0, len = data.length; i < len; i++) {
    //                var color = colors[i];
    //                var num = data[i].count;
    //                if (num != 0) {
    //                    count += num;
    //                    dountData.push([num, color, data[i].id, data[i].text + "目标"]);
    //                }
    //                $ul.append('<li><span class="color" style="background-color:' + color + '"></span><span class="text">' + data[i].text + '</span></li>');
    //
    //            }
    //            Raphael("object_chart", 330, 310).dountChart(165, 155, 55,70,110,dountData,function(data){
    //                //TODO 饼图click事件
    //                //拼上时间
    //                var day;
    //                if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
    //                    day = 31;
    //                }
    //                else if (month == 4 || month == 6 || month == 9 || month == 11) {
    //                    day = 30;
    //                }
    //                else if (month = 2) {
    //                    if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && year % 100 == 0))//闰年
    //                    {
    //                        day = 29;
    //                    }
    //                    else {
    //                        day = 28;
    //                    }
    //                }
    //                var timeStart = year + '/' + month + '/01';
    //                var timeEnd = year + '/' + month + '/' + day;
    //            });
    //            $(".chart-center",$con).html('目标<span style="color:#58b456">' + count + '</span>项');
    //            $(".month .text",$con).html(year + "年" + month + "月");
    //        }),
    //        complete:function(){
    //            lodi.remove();//关闭load层
    //        }
    //    });
    //}
    //
    //drawPlanProgress();
    //$(".arrowsBBLCom",$con).click(function(){
    //    var date = new Date()
    //        ,thisyear = date.getFullYear()
    //        ,thismonth = date.getMonth() + 1;
    //    if(year == thisyear && month == thismonth){
    //        $(".arrowsBBRCom",$con).show();
    //    }
    //    if(month == 1){
    //        year -- ;
    //        month = 12;
    //    }else{
    //        month --;
    //    }
    //    $(".month .text",$con).html(year + "/" + month);
    //    drawPlanProgress();
    //});
    //
    //$(".arrowsBBRCom",$con).click(function(){
    //    var date = new Date()
    //        ,thisyear = date.getFullYear()
    //        ,thismonth = date.getMonth() + 1;
    //    if(month == 12 && year < date.getFullYear()){
    //        year ++;
    //        month = 1;
    //    }else{
    //        month ++;
    //    }
    //    $(".month .text",$con).html(year + "/" + month);
    //    if(year >= thisyear && month >= thismonth){
    //        $(this).hide();
    //    }
    //    drawPlanProgress();
    //});




    var checkArray = ["金额", "时间", "数字"],     //考核类型 1：金额 2：时间 3：数字
         loginId = 1;    //登录人
    var detailFlag = 1;                  //detailFlag=1 目标详情画面, detailFlag=2 目标审核画面, detailFlag=3 目标确认画面



    ////菜单月选择 日选择
    //$("#dayMonthChosen li").click(function () {
    //    if (!$(this).hasClass("timeChosen")) {
           
    //    }
    //});
    //$("#dayMonthChosen li:eq(0)").trigger("click");

    //刷新时间轴
    function timeLine() {
        var mydate = new Date();
        var year = mydate.getFullYear(),
            month = mydate.getMonth() + 1,
            day = mydate.getDate();
        if ((year % 400 == 0) || (year % 4 == 0) && (year % 100 != 0)) {
            monthArray[1] = 29;
        } else {
            monthArray[0] = 28;
        }
        listArgus.date = year + "-" + month + "-" + day;
        var num = monthArray[month - 1],
            currentTime = month,
            s = "月";
        loadNewTimes(year, month, day, num);
        $("#timeLine h4:eq(0)").text(currentTime + s).attr("term", currentTime);
        $("#prevPic").prev().text(currentTime - 1 + s).attr("term", currentTime - 1);
        $("#nextPic").prev().text(currentTime + 1 + s).attr("term", currentTime + 1);

    }
    

    //刷新时间轴 
    function loadNewTimes(now_year, now_month, now_day, num) {
        var mydate = new Date();
        var year = mydate.getFullYear(),
            month = mydate.getMonth() + 1,
            day = mydate.getDate();
        if (now_year == year && now_month == month) {
            now_day = day;
        }
        var $ul = $("#timeBtnContent ul");
        $ul.empty();
        for (var i = 1; i <= num; i++) {
            var $li = $('<li>' + i + '<span class="markCircle" term="' + i + '"></span></li>');
            $li.click(function () {
                if (!$(this).hasClass("timeChosen")) {
                    $("#timeBtnContent .timeChosen").removeClass("timeChosen");
                    $(this).addClass("timeChosen");
                    //if( listArgus.dateType == 1 ){
                    //    month = i;
                    //    day = null;
                    //}else{
                    day = $(this).find(".markCircle").attr('term');
                    // $("#timeBtnContent ul").scrollLeft = parseInt(day) * 58;
                    //}
                    if (month == "12") {
                        $("#nextPic").hide();
                    } else if (month == "1") {
                        $("#prevPic").hide();
                    }
                    listArgus.date = listArgus.date.split('-')[0] + "-" + listArgus.date.split('-')[1] + "-" + day;
                    loadingPlanList();
                    loadingFlowList();
                    loadingFObjectList();
                    if (listArgus.calendarType == 1) {
                        $(".headTitle li").hide();
                        $(".myPlanScheduleLi").show();
                    } else {
                        $(".headTitle li").show();
                        $(".myPlanScheduleLi").hide();
                    }
                }
            });
            if (now_year == year && now_month == month && i == now_day) {
                $li.trigger("click");
            }
            $ul.append($li);
        }
    }

    // 滚动条定位当前日期位置
    function change_sroll() {
        if ($("#timeBtnContent .timeChosen").length <= 0) {
            $("#timeBtnContent ul").scrollLeft(0);
        } else {
            var positionA = $("#timeBtnContent .timeChosen").position().left;
            var positionB = $("#timeBtnContent ul").scrollLeft();
            $("#timeBtnContent ul").scrollLeft(positionA + positionB);  
        }

    }

    //加载时间轴
    timeLine();

    setTimeout(change_sroll, 300);
    

    $("#prevPic,#nextPic").click(function () {
        var mydate = new Date();
        var year = mydate.getFullYear();
        var s, currentTime;
        if ($("#timeLine h4:eq(0)").text().indexOf("年") > 0) {
            s = "年";
        } else {
            s = "月";
        }
        if ($(this).hasClass("glyphicon-menu-right")) {
            $("#prevPic").show();
            $("#prevPic").prev().show();
            $("#timeLine h4").each(function () {
                var nowMonth = parseInt($(this).attr("term"));
                if (nowMonth == "12") {
                    $("#nextPic").hide();
                    $("#nextPic").prev().hide();
                }
                currentTime = nowMonth + 1;
                $(this).text(currentTime + s).attr("term", currentTime);
            })
        } else {
            $("#nextPic").show();
            $("#nextPic").prev().show();
            $("#timeLine h4").each(function () {
                var nowMonth = parseInt($(this).attr("term"));
                if (nowMonth == "1") {
                    $("#prevPic").hide();
                    $("#prevPic").prev().hide();
                }
                currentTime = nowMonth - 1;
                $(this).text(currentTime + s).attr("term", currentTime);
            })
        }
        var currentMonth = currentTime > 12 ? 12 : (currentTime - 1);
        listArgus.date = listArgus.date.split('-')[0] + "-" + currentMonth + "-" + listArgus.date.split('-')[2]
        //刷新时间轴
        loadNewTimes(year, currentMonth, 1, monthArray[currentMonth - 1]);
        change_sroll();
    })

    //我的日程  下属日程
    $("#myScheduleTab,#subScheduleTab").click(function () {
        if (!$(this).parent().hasClass("active")) {
            $("#planScheduleContainer .active").removeClass("active");
            $(this).parent().addClass("active");
            listArgus.calendarType = parseInt($(this).attr("term"));
            if (listArgus.calendarType == 1) {
                listArgus.status = null;
                $(".headTitle li").hide();
                $(".myPlanScheduleLi").show();
            } else {
                listArgus.status = 1;
                $("#planList ul li a").removeClass("greenColor");
                $("#planList ul li:eq(1) a").addClass("greenColor");
                $("#objectList ul li a").removeClass("greenColor");
                $("#objectList ul li:eq(0) a").addClass("greenColor");
                $(".headTitle li").show();
                $(".myPlanScheduleLi").hide();
            }
            loadingPlanList();
            loadingFlowList();
            loadingFObjectList();
        }
    });


    //加载流程列表
    function loadingFlowList() {
        var $planContent = $("#contentFlow");
        var lodi;
        $.ajax({
            type: "post",
            url: "/CalendarProcess/GetFlowList",
            dataType: "json",
            data: { calendarType: listArgus.calendarType, date: listArgus.date },
            beforeSend: function () {
                $planContent.empty();
                lodi = getLoadingPosition($planContent);     //显示load层
            },
            success: rsHandler(function (data) {
                $planContent.empty();
                if (data && data.length > 0) {
                    $.each(data, function (i, v) {
                        var $container = $('<div class="row containerListOne relativePos hoverClass" term=' + v.formId + '></div>');
                        var $firstDiv = $('<div class="imageCol">' +
                            '<img class="personImage" />' +
                            '<p>' + v.createUserName + '</p></div>');
                        $firstDiv.find(".personImage").attr("src", v.img);

                        //if (listArgus.calendarType == 2) {
                        //    var $OK = $('<span class="choseCheck"><a href="javaScript:void(0)" class="glyphicon glyphicon-ok"></a></span>');
                        //    $OK.find("a").click(function () {
                        //        $(this).toggleClass("orangeColor");
                        //        if ($(this).hasClass("orangeColor")) {
                        //            $container.removeClass("hoverClass");
                        //        } else {
                        //            $container.addClass("hoverClass");
                        //        }
                        //    });
                        //    $firstDiv.append($OK);
                        //}
                        var operateUserNames = "";
                        if (v.operate && v.operate.length > 0) {
                            for (var i = 0; i < v.operate.length; i++) {

                                if (v.operate[i].mandataryUser != null) {
                                    operateUserNames += v.operate[i].mandataryUserName;
                                    if (v.operate.length > (i + 1)) {
                                        operateUserNames += "，";
                                    }
                                }
                                else {
                                    operateUserNames += v.operate[i].name;
                                    if (v.operate.length > (i + 1)) {
                                        operateUserNames += "，";
                                    }
                                }

                            }
                        }
                        var $secondDiv = $('<div class="col-xs-4" ><ul class="list-unstyled listOnePlan">' +
                            '<li>工作流 : <span class="textOver" title=' + v.templateName + '>' + v.templateName + '</span></li>' +
                            '<li>请求标题 : <span  class="textOver" title=' + v.title + '>' + v.title + '</span></li>' +
                            '<li>当前未操作者 : <span style="width: 200px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" title="' + operateUserNames + '">' + operateUserNames + '</span></li>' +
                            '<li>当前节点 : <span  class="textOver" title=' + v.node + '>' + v.nodeName + '</span></li></ul></div>');

                        var $thirdDiv = $('<div class="col-xs-3"><ul class="list-unstyled listTwoPlan">' +
                            '<li><span class="pull-left">紧急度 :</span></li></ul></div>');
                        $thirdDiv.find("li:eq(0)").append(startDraw(v.urgency, "startImportant"));
                        var $forthDiv = $(' <div class="col-xs-3 pull-right relativePos text-right"  ><span class="createTimeSpan">创建于 : ' + v.createTime.replace('T', ' ').substr(0, 16) + '</span></div>');

                        $container.append([$firstDiv, $secondDiv, $thirdDiv, $forthDiv]);
                        var $operation = $('<ul class="list-inline operate"></ul>');
                        var $liCommit = $("<li class='liCommit'>提交</li>"),
                            $liForward = $("<li class='liForward'>转发</li>"),
                            $liDetail = $("<li class='liDetail'>详情</li>"),
                            $liDel = $("<li class='liDel'>删除</li>");
                        //删除
                        $liDel.click(function () {
                            var formId = v.formId;
                            ncUnits.confirm({
                                title: '提示',
                                html: '确认要删除？',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    $.ajax({
                                        type: "post",
                                        url: "/FlowIndex/deleteUserForm",
                                        dataType: "json",
                                        data: { formId: formId },
                                        success: rsHandler(function (data) {
                                            ncUnits.alert("删除成功!");
                                            loadingFlowList();
                                        })
                                    })
                                }
                            });
                        });

                        //详情
                        var newVal;
                        $liDetail.click(function () {
                            current_Info = v;
                            $("#details-modal").modal("show");
                        });

                        //转发
                        $liForward.click(function () {
                            current_Info = v;
                            HrModalTab = 2;


                            /*人力资源 开始*/
                            var personOrgId;
                            var personWithSub = false;
                            $("#transpont_modal .modal-content").load("/FlowIndex/LoadTranspont", function () {
                                $("#transpont_modal").modal("show");
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetOrganizationList",
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
                                                url: "/Shared/GetOrganizationList",
                                                autoParam: ["id=parent"],
                                                dataFilter: function (treeId, parentNode, responseData) {
                                                    return responseData.data;
                                                }
                                            }
                                        }), data);
                                    })
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
                                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
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
                                    var copyUsers = [];
                                    $("#HR_modal_chosen li").each(function () {
                                        copyUsers.push($(this).attr('term'));
                                    });
                                    $.ajax({
                                        type: "post",
                                        url: "/FlowIndex/DuplicateForm",
                                        dataType: "json",
                                        data: { formId: current_Info.formId, nodeId: current_Info.currentNode, templateId: current_Info.templateId, data: JSON.stringify(copyUsers) },
                                        success: rsHandler(function (data) {
                                            if (data) {
                                                ncUnits.alert("转发成功!");
                                                $("#HR_select").val('');
                                                $('#transpont_modal').modal('hide');
                                            } else {
                                                ncUnits.alert("已选人员包含当未操作人，请重新选择!");
                                            }

                                        })
                                    });
                                });

                                //取消
                                $("#HR_cancel").click(function () {
                                    $("#HR_select").val('');
                                    $("#HR-haschildren").prop("checked", false);
                                    $("#HR_modal_chosen_count").text(0);
                                    $(".person_list input[type=checkbox]").prop("checked", false);
                                    $("#HR_modal_chosen li").remove();
                                });
                                /*人力资源 结束*/
                            });
                        });

                        //提交
                        $liCommit.click(function () {
                            var templateId = v.templateId;
                            var formId = v.formId;
                            var nodeId = v.currentNode;
                            ncUnits.confirm({
                                title: '提示',
                                html: '确认要提交吗？',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    $.ajax({
                                        type: "post",
                                        url: "/FlowIndex/SubmitFlow",
                                        dataType: "json",
                                        data: { templateId: templateId, formId: formId, nodeId: nodeId, flag: 0 },
                                        success: rsHandler(function (data) {
                                            if (data) {
                                                ncUnits.alert("流程提交成功!");
                                                loadingFlowList();
                                            }
                                            else {
                                                ncUnits.alert("流程设置有误，请联系管理员!");
                                            }
                                        })
                                    });
                                }
                            });
                        });

                        if (listArgus.calendarType == 1) {   //我的日程
                            if (v.status == 1) {            // 1、待提交的流程：删除、转发、提交和详情。
                                $operation.append([$liDel, $liForward, $liCommit, $liDetail]);
                            } else if (v.status == 2) {       // 2、待审核的流程：转发和详情。
                                $operation.append([$liForward, $liDetail]);
                            }

                        } else {      //下属日程
                            if (v.operateStatus == 6)//待审核显示按钮
                            {
                                $operation.append([$liForward, $liDetail]);
                            }
                            else if (v.operateStatus == 10) { //提交节点
                                $operation.append([$liForward, $liDetail]);
                            }
                        }
                        $planContent.append($container.append($operation));
                    })
                }

            }),
            complete: function () {
                lodi.remove(); //关闭load层
            }
        })
    }

    function renderFlowChart(formId) {
        $.ajax({
            url: "/FlowChart/DisplayFormFlowChart",
            type: "post",
            dataType: "json",
            data: {
                formId: formId
            },
            success: rsHandler(function (data) {
                $("#modal_detail_flowchart").flowLine(data);
            })
        })
    }

    //加载目标列表
    function loadingFObjectList() {
        var $planContent = $("#contentObject");
        var lodi;
        $.ajax({
            type: "post",
            url: "/CalendarProcess/GetObjectiveList",
            dataType: "json",
            data: { data: JSON.stringify(listArgus) },
            beforeSend: function () {
                $planContent.empty();
                lodi = getLoadingPosition($planContent);     //显示load层
            },
            success: rsHandler(function (data) {
                $planContent.empty();
                loginId = data.userId;
                if (data.ObjectiveList && data.ObjectiveList.length > 0) {
                    $.each(data.ObjectiveList, function (i, v) {
                        var $container = $('<div class="row containerListOne relativePos hoverClass" term=' + v.objectiveId + '><div class="objectHeadTitle">' + v.objectiveTypeName + '</div></div>');
                        var $firstDiv = $('<div class="imageCol">' +
                            '<img class="personImage" >' +
                            '<p>' + v.createUser + '</p></div>');
                        $firstDiv.find(".personImage").attr("src", v.bigImage);
                        //if (listArgus.calendarType == 2) {
                        //    var $OK = $('<span class="choseCheck"><a href="javaScript:void(0)" class="glyphicon glyphicon-ok"></a></span>');
                        //    $OK.find("a").click(function () {
                        //        $(this).toggleClass("orangeColor");
                        //        if ($(this).hasClass("orangeColor")) {
                        //            $container.removeClass("hoverClass");
                        //        } else {
                        //            $container.addClass("hoverClass");
                        //        }
                        //    });
                        //    $firstDiv.append($OK);
                        //}
                        var authHtml="";
                        if (listArgus.calendarType == 1 && v.authorizedUser != null && loginId != v.authorizedUser) {
                            authHtml = "<span> | 被授权人：" + v.authorizedUserName;
                        }
                        var timeValue = v.startTime.replace('T', ' ').substr(0, 10) + "至" + v.endTime.replace('T', ' ').substr(0, 10);
                        var $secondDiv = $('<div class="col-xs-4" ><ul class="list-unstyled listOnePlan">' +
                            '<li>目标名称 : <span class="textOver" title=' + v.objectiveName + '>' + v.objectiveName + '</span></li>' +
                            '<li>起止时间 : <span>' + timeValue + '</span></li>' +
                            '<li>奖励基数 : <span>' + (v.bonus == null ? '' : v.bonus) + '</span></li>' +
                            '<li>' + (listArgus.calendarType == 1 ? "确认人" : "责任人") + ' : <span>' + (listArgus.calendarType == 1 ? v.confirmUserName : v.responsibleUserName) + '</span>'+authHtml+'</li></ul></div>');

                        

                        var $thirdDiv = $('<div class="col-xs-3"><ul class="list-unstyled listTwoPlan">' +
                            '<li>理想值 : ' + v.expectedValue + '</li><li>实际值 : ' + v.expectedValue + '</li><li>警戒时间 : ' + (v.alarmTime == null ? '' : v.alarmTime.split('T')[0]) + '</li><li>考核类型 : ' + checkArray[v.checkType - 1] + '</li></ul></div>');

                        var $forthDiv = $('<div class="col-xs-3 pull-right text-right relativePos"  >' +
                            '<h4 class="weight">' + (v.weight == null ? "" : ("权重：" + v.weight + "%")) + '</h4>' +
                            '<span class="createTimeSpan">创建于 : ' + v.createTime.split('T')[0] + '</span></div>');
                        $container.append([$firstDiv, $secondDiv, $thirdDiv, $forthDiv]);
                        var $operation = $('<ul class="list-inline operate"></ul>');
                        var $liSubmit = $("<li class='liCommit'>提交</li>"),
                            $liUnfold = $("<li class='liOpen'>展开</li>"),
                            $liDetail = $("<li class='liDetail'>详情</li>"),
                            $liPower = $("<li class='liPower' > 授权</li>"),
                            $liDel = $("<li class='liDel'>删除</li>"),
                            $liModify = $("<li class='liModify'>修改</li>"),
                            $liPowerCancel = $("<li class='liPowerCancel'>取消授权</li>"),
                            $liCommit = $("<li class='liCommit'>提交确认</li>");

                        //删除
                        $liDel.click(function () {
                            current_Info = v;
                            revokeDelCancel("删除");
                        });
                        //展开
                        $liUnfold.click(function () {
                            current_Info = v;
                            $unfoldNode = $(this);
                            $("#objective_open_id").val(v.objectiveId);
                            $("#object_unfold_modal").modal("show");
                        });
                        $liSubmit.click(function () {
                            newOrDecomposition = 3;
                            $("#objectiveNew_modal").modal('show');
                            newInit(v.objectiveId, (loginId == v.authorizedUser ? 1 : 2));
                        });

                        //详情
                        $liDetail.click(function () {
                            current_Info = v;
                            $("#object_detail_modal").modal("show");
                        });
                        //授权
                        $liPower.click(function () {
                            current_Info = v;
                            HrModalTab = 1;
                            $("#Object_HR_modal").modal("show");
                        });

                        //取消授权
                        $liPowerCancel.click(function () {
                            current_Info = v;
                            revokeDelCancel("取消授权");
                        });

                        //提交确认
                        $liCommit.click(function () {
                            detailFlag = 4;
                            $activeNode = $(this);
                            current_Info = v;
                            $("#object_detail_modal").modal("show");
                        });

                        //修改
                        $liModify.click(function () {
                            if (loginId == v.responsibleUser && v.status == 3) {
                                $("#objectModifyRole").hide();
                            } else {
                                $("#objectModifyRole:hidden").show();
                            }
                            current_Info = v;
                            $activeNode = $(this);
                            $("#object_modify_modal").modal("show");
                        });
                        if (listArgus.calendarType == 1) {
                            if (v.status == 1) {                                                        //   1-1、待提交：提交、删除、展开、详情。
                                if (loginId == v.responsibleUser && v.authorizedUser == null) {        //  1、登陆用户为目标责任人且目标未授权给他人的场合： 授权
                                    $operation.append([$liPower, $liSubmit, $liDel, $liUnfold, $liDetail]);
                                } else if (loginId == v.authorizedUser) {               // 2、登陆用户为目标被授权人的的场合：
                                    $operation.append([$liSubmit, $liDel, $liUnfold, $liDetail]);
                                }
                                else if (loginId == v.responsibleUser && v.authorizedUser != null) {
                                    $operation.append([$liPowerCancel, $liUnfold, $liDetail]);
                                }
                            } else if (v.status == 3 || v.status == 6) {             //  1-2、进行中：修改、提交确认、展开、详情。
                                $operation.append([$liModify, $liCommit, $liUnfold, $liDetail]);
                            }
                        } else {       //下属日程
                            var $liSure = $("<li class='liSure'>确认</li>"),
                                $liCheck = $("<li class='liChecks' >审核</li>");

                            //确认
                            $liSure.click(function () {
                                detailFlag = 3;
                                current_Info = v;
                                $activeNode = $(this);
                                $("#object_detail_modal").modal("show");
                            });

                            //审核
                            $liCheck.click(function () {
                                detailFlag = 2;
                                current_Info = v;
                                $activeNode = $(this);
                                $("#object_detail_modal").modal("show");
                            });
                            if (listArgus.status == 1 && v.status == 2) {   //待审核
                                $operation.append([$liModify, $liCheck, $liUnfold, $liDetail]);
                            } else if (listArgus.status == 2 && v.status == 4) {  //待确认
                                $operation.append([$liSure, $liUnfold, $liDetail]);
                            }
                        }
                        $planContent.append($container.append($operation));
                    })
                }

            }),
            complete: function () {
                lodi.remove();         //关闭load层
            }
        })
    }

    //撤销事件 删除事件  取消授权事件 parentChild=1表示是第一层目标事件，否则表示子目标事件
    function revokeDelCancel(flag) {
        var objectId = current_Info.objectiveId;
        var isHasChild = current_Info.isHasChild;
        var url;
        var confirmText = '确定要' + flag + '吗?';
        if (flag == "撤销") {
            url = "/ObjectiveIndex/RevokeObjective";
        } else if (flag == "删除") {
            url = "/ObjectiveIndex/DeleteObjective";
            if (isHasChild) {
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
                            loadingFObjectList();
                        }
                        else
                            ncUnits.alert(flag + "失败!");
                    })
                });
            }
        });
    }

    $("#object_modify_modal").on('shown.bs.modal', function () {
        objectiveModify(current_Info.objectiveId);
        documentUpload($("#fileUpload"), $("#object_modify_modal .progress"), $("#modifyObjectDocument"), $("#modifyObjectLog"));
        if (loadFormaluFlag && $("#objectModifyRole").is(':visible')) {
            $("#modifyFormularView .modal-body").empty();
            FormaluView(FormulaList, maxValue, minValue, formulaType, $("#modifyFormularView .modal-body"));
        }
    })

    $("#objectModifySave,#objectModifyCommit").off("click");
    $("#objectModifySave,#objectModifyCommit").click(function () {
        modifySave(parseInt($(this).attr("term")));
        //unfoldObjectCall(null, hrHorizontal);                    //重新加目标展开页面
        if (unflodFlag == 1) {
            unflodFlag = 2;
        }
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
    $("#objectModifyTab").click(function () {
        if ($(this).hasClass("disabledColor")) {
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

    //计划 批量审核确认 全清
    $("#planList .headTitle li:not(.myPlanScheduleLi) a").click(function () {
        var term = parseInt($(this).attr("term"));
        if (term == 1) {             //待审核计划 
            if (!$(this).hasClass("greenColor")) {
                $(this).closest("ul").find(".greenColor").removeClass("greenColor");
                $(this).addClass("greenColor");
                listArgus.status = term;
                loadingPlanList();
                $(this).closest("ul").find("li:eq(3) a").text("批量审核").show();
                $(this).closest("ul").find("li:eq(4)").show();
            }
        } else if (term == 2) {
            if (!$(this).hasClass("greenColor")) {
                $(this).closest("ul").find(".greenColor").removeClass("greenColor");
                $(this).addClass("greenColor");
                listArgus.status = term;
                loadingPlanList();
                $(this).closest("ul").find("li:eq(3) a").hide();
                $(this).closest("ul").find("li:eq(4)").hide();
            }
        } else if (term == 3) {             //批量操作
            $("#planList .orangeColor").each(function () {
                if ($(this).attr("data_planId") == '0') {
                    batch_loopId.push(parseInt($(this).attr("data_loopId")));
                } else {
                    batch_planId.push(parseInt($(this).attr("data_planId")));
                }
            });
            if (batch_planId.length == 0 && batch_loopId.length == 0) {
                ncUnits.alert("请选择操作的计划");
            } else {
                if (batch_planId.length > 0) {
                    $.ajax({
                        type: "post",
                        url: "/CalendarProcess/GetPlanDetail",
                        dataType: "json",
                        data: { planId: batch_planId[0], isloop: false },
                        success: rsHandler(function (data) {
                            current_Info = data;
                            $('#xxc_planId').val(data.planId);
                        })
                    });
                } else {
                    $.ajax({
                        type: "post",
                        url: "/CalendarProcess/GetPlanDetail",
                        dataType: "json",
                        data: { planId: batch_loopId[0], isloop: true },
                        success: rsHandler(function (data) {
                            current_Info = data;
                            $('#xxc_planId').val(data.loopId);
                        })
                    });
                }
                batch_flag = true;
                $("#plan_detail_modal").modal("show");
                checkclick("true");
            }
        } else if (term == 4) {              //全清
            $("#planList .orangeColor").removeClass("orangeColor").addClass("whiteColor");
        }
    });

    //目标 批量审核确认 全清
    $("#objectList .headTitle li:not(.myPlanScheduleLi) a").click(function () {
        var term = parseInt($(this).attr("term"));
        if (term == 1) {             //待审核目标
            if (!$(this).hasClass("greenColor")) {
                $(this).closest("ul").find(".greenColor").removeClass("greenColor");
                $(this).addClass("greenColor");
                listArgus.status = term;
                loadingFObjectList();
                //$(this).closest("ul").find("li:eq(3) a").text("批量审核").show();
                $(this).closest("ul").find("li:eq(4)").show();
            }
        } else if (term == 2) {
            if (!$(this).hasClass("greenColor")) {
                $(this).closest("ul").find(".greenColor").removeClass("greenColor");
                $(this).addClass("greenColor");
                listArgus.status = term;
                loadingFObjectList();
                //$(this).closest("ul").find("li:eq(3) a").hide();
                $(this).closest("ul").find("li:eq(4)").hide();
            }
        }
        //else if (term == 3) {             //批量操作
        //    $("#planList .orangeColor").each(function () {
        //        if ($(this).attr("data_planId") == '0') {
        //            batch_loopId.push(parseInt($(this).attr("data_loopId")));
        //        } else {
        //            batch_planId.push(parseInt($(this).attr("data_planId")));
        //        }
        //    });
        //    if (batch_planId.length == 0 && batch_loopId.length == 0) {
        //        ncUnits.alert("请选择操作的计划");
        //    } else {
        //        if (batch_planId.length > 0) {
        //            $.ajax({
        //                type: "post",
        //                url: "/CalendarProcess/GetPlanDetail",
        //                dataType: "json",
        //                data: { planId: batch_planId[0], isloop: false },
        //                success: rsHandler(function (data) {
        //                    current_Info = data;
        //                    $('#xxc_planId').val(data.planId);
        //                })
        //            });
        //        } else {
        //            $.ajax({
        //                type: "post",
        //                url: "/CalendarProcess/GetPlanDetail",
        //                dataType: "json",
        //                data: { planId: batch_loopId[0], isloop: true },
        //                success: rsHandler(function (data) {
        //                    current_Info = data;
        //                    $('#xxc_planId').val(data.loopId);
        //                })
        //            });
        //        }
        //        batch_flag = true;
        //        $("#plan_detail_modal").modal("show");
        //        checkclick($("#plan_detail_modal"), "true");
        //    }
        //} else if (term == 4) {              //全清
        //    $("#objectList .orangeColor").removeClass("orangeColor").addClass("whiteColor");
        //}
    });

    //--------------------弹窗---------------------------//

    //----流程------//
    //新建流程
    var addflow_templateId;  //预览保存的模板ID
    var selectedtemplateId = null; //选择模板后保存的模板ID
    var saveData = null;      //保存控件信息
    var addflow_content = {
        templateId: null,
        organizationId: null,
        stationId: null,
        title: null,
        urgency: null,
        createTime: null,
        status: null,
        controlValue: []
    };
    $("#addflow_modal").on("shown.bs.modal", function () {
        var addflow_lodi = getLoadingPosition('.form-list');
        //绑定流程分类
        $.ajax({
            type: "post",
            url: "/FlowIndex/GetTemplateCategoryList",
            dataType: "json",
            success: rsHandler(function (data) {
                if (data && data.length > 0) {
                    $(".category_content").empty();
                    var $formList = $('.form-list');
                    $formList.empty();
                    var showflag = true;
                    $.each(data, function (e, i) {
                        var $content = $(" <li><a  term=" + i.categoryId + " class='xxc_categoryName' style='overflow: hidden;text-overflow: ellipsis;white-space: nowrap' title='" + i.categoryName + "'>" + i.categoryName + "</a></li><li class='divider short'></li>");
                        $(".category_content").append($content);

                        /*加载表单 开始*/
                        $.ajax({
                            type: "post",
                            url: "/FlowIndex/GetTemplateList",
                            dataType: "json",
                            data: { categoryId: i.categoryId },
                            success: rsHandler(function (data) {
                                if (data && data.length > 0) {

                                    $.each(data, function (e, j) {
                                        var $categoryinfo = $('<div class="categoryinfo-div"></div>');
                                        var $categoryname = $('<div id="category' + j.categoryId + '" class="category-name col-xs-12" title="' + j.categoryName + '"><span style="overflow: hidden;text-overflow: ellipsis;' +
                                        'white-space: nowrap;">' + j.categoryName + '</span><div class="category-line"></div></div>');
                                        var $templatelistDiv = $('<div class="template-category"></div>');
                                        $formList.append($categoryinfo.append([$categoryname, $templatelistDiv]));
                                        if (j.templateList && j.templateList.length > 0) {
                                            var count = 1;
                                            $.each(j.templateList, function (s, k) {
                                                var $templateinfo = $('<div class="col-xs-4"><div class="cell addflow_cell" term=' + k.templateId + '><div class="pull-left img-circle templateId">'
                                                + '<span class="content-center">' + count + '</span></div><div class="info-list"><div class="title" style="overflow: hidden;text-overflow: ellipsis;white-space: nowrap" title="' + k.templateName + '">' + k.templateName + '</div>'
                                                + '<div class="des" title=' + k.description + ' style="word-wrap: break-word;overflow: hidden;text-overflow: ellipsis;white-space: nowrap" title="' + k.description + '">' + k.description + '</div></div>'
                                                + '<div class="form_preview"><span data-toggle="modal" data-target="#preview_modal" >预览</span></div><img src="../../Images/flow/addflow_select_hit.png" class="addflow_select" term="0"  >'
                                                + '</div></div>');
                                                $templatelistDiv.append($templateinfo);
                                                //加载事件

                                                //表单上移变色
                                                $(".addflow_cell").hover(function () {
                                                    if (showflag == true) {
                                                        $(this).css("border", "1px solid #fbaa12");
                                                        $('.addflow_cell').removeClass("gray-background").find(".addflow_select").hide();
                                                        $(this).find(".addflow_select").show();
                                                        $(this).addClass("gray-background");
                                                    }
                                                }, function () {
                                                    if (showflag == true) {
                                                        $(".addflow_select").hide();
                                                        $(this).css("border", "1px solid #ccc");
                                                        $('.addflow_cell').removeClass("gray-background");
                                                    }
                                                });
                                                //点击勾选
                                                $(".addflow_select").off('click');
                                                $(".addflow_select").click(function () {

                                                    var term = $(this).attr("term");
                                                    if (term == 0) {
                                                        addflow_templateId = $(this).parents('.addflow_cell').attr('term');
                                                        showflag = false;
                                                        $(this).attr("src", "../../Images/flow/addflow_select.png").attr("term", "1");
                                                        $(this).parents('.addflow_cell').css("border", "1px solid #fbaa12");
                                                        //$(this).parents(".cell").addClass("gray-background").sibling().removeClass("gray-background").find(".addflow_select").hide();
                                                    }
                                                    else {
                                                        addflow_templateId = null;
                                                        showflag = true;
                                                        $(this).attr("src", "../../Images/flow/addflow_select_hit.png").attr("term", "0");
                                                    }
                                                });
                                                //预览
                                                $('.form_preview span').off('click');
                                                $('.form_preview span').click(function () {
                                                    addflow_templateId = $(this).parents(".addflow_cell").attr('term');
                                                });
                                                count++;
                                            });
                                        }
                                    });
                                    var category_text = $('#xxc_selectedcate').attr('title');
                                    $(".xxc_categoryName").each(function () {
                                        if ($(this).attr('title') == category_text) {
                                            $(this).click();
                                        }
                                    });
                                    addflow_lodi.remove();
                                }
                            })
                        });
                        /*加载表单 结束*/

                        $(".category_content .xxc_categoryName").off('click');
                        $(".category_content .xxc_categoryName").click(function () {
                            var cateId = $(this).attr('term');
                            $("#category_select").text($(this).text()).attr('term', cateId).attr('title', $(this).text());
                            var obj = $("#category" + cateId);
                            document.getElementById("category" + cateId).scrollIntoView();
                            //obj.scrollIntoView();
                        });

                    });
                    $("#category_select").text($('.category_content .xxc_categoryName:eq(0)').text()).attr('term', $('.category_content .xxc_categoryName:eq(0)').attr('term'));
                    $(".category_content li:last").remove();
                }
            })
        });



        //确定按钮点击事件
        $('#addflow_modal_submit').click(function () {
            if (addflow_templateId == null) {
                ncUnits.alert('请选择表单');
                return;
            }
            else {
                $(this).attr('data-toggle', 'modal');
                $(this).attr('data-target', '#preview_modal');
                $('.addflow_footer').show();
                $('#addflow-mask').hide();
            }
        });
    }).on("hidden.bs.modal", function () {
        $('.form-list').empty();
    });
    //显示预览画面
    $("#preview_modal").on("shown.bs.modal", function () {
        /* 评分五角星 开始 */
        $('.stars ul li').hover(function () {
            var nums = $(this).index();
            var length = $(this).parent().children('li').length - 1;
            for (var i = 0; i <= length; i++) {
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
        $('.stars ul li').click(function () {
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

        //部门岗位的鼠标进入离开事件
        //$("#department_select").parents('.addflow-text').mouseleave(function () {
        //    $(this).find('.department_ul').hide();
        //});
        //$("#department_select").parents('.addflow-text').mouseenter(function () {
        //    $(this).find('.department_ul').show();
        //});
        //$("#station_select").parents('.addflow-text').mouseleave(function () {
        //    $(this).find('.station_ul').hide();
        //});
        //$("#station_select").parents('.addflow-text').mouseenter(function () {
        //    $(this).find('.station_ul').show();
        //});


        //选择完成时间
        $("#addflow_createtime").click(function () {
            laydate({
                elem: '#addflow_createtime_v',
                event: 'click',
                istime: true,
                format: 'YYYY-MM-DD hh:mm',
                isclear: true,
                istoday: true,
                issure: true,
                festival: true,
                start: new Date().toLocaleDateString() + ' 08:30:00',
                choose: function (dates) {
                    endTime_v = dates;
                    if (addflow_defaultTitle == 1) {
                        addflow_createTime = dates.replace('-', '/').replace('-', '/');
                        $("#preview_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                    }
                },
                clear: function () {
                    endTime_v = undefined;
                }
            });
        });

        $.ajax({
            type: "post",
            url: "/FlowIndex/GetUserOrganizationList",
            dataType: "json",
            success: rsHandler(function (data) {
                if (data) {
                    var orgId = data.orgList[0].id;
                    $("#yulan_createUser").val(data.userName);
                    addflow_createUserName = data.userName;
                    addflow_createTime = data.createTime.replace('T', ' ').substr(0, 16);
                    $("#addflow_createtime_v").val(addflow_createTime);
                    addflow_createTime = addflow_createTime.replace('-', '/').replace('-', '/');
                    yulan_createTime = $("#addflow_createtime_v").val();
                    $(".department_ul").empty();
                    if (data.orgList && data.orgList.length > 0) {
                        $.each(data.orgList, function (e, i) {
                            var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_departmentName'>" + i.name + "</a></li><li class='divider short'></li>");
                            $(".department_ul").append($content);
                            $(".department_ul .xxc_departmentName").off('click');
                            $(".department_ul .xxc_departmentName").click(function () {
                                orgId = $(this).attr('term');
                                $("#department_select").text($(this).text()).attr('term', orgId);
                                if (addflow_defaultTitle == 1) {
                                    addflow_createdepart = $("#department_select").text();
                                    $("#preview_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                                }

                                //加载岗位
                                load_addflow_station(orgId);
                            });
                        });
                    }
                    $("#department_select").text($('.department_ul .xxc_departmentName:eq(0)').text()).attr('term', $('.department_ul .xxc_departmentName:eq(0)').attr('term'));
                    addflow_createdepart = $("#department_select").text();
                    $(".department_ul li:last").remove();
                    //加载岗位
                    load_addflow_station(orgId);
                    //加载表单
                    loadDataByTemplateId(addflow_templateId);
                }
            })
        });

        //加载岗位
        function load_addflow_station(orgId) {
            $.ajax({
                type: "post",
                url: "/FlowIndex/GetUserStationList",
                dataType: "json",
                data: { orgId: orgId },
                success: rsHandler(function (data) {
                    if (data && data.length > 0) {
                        $(".station_ul").empty();
                        $.each(data, function (e, i) {
                            var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_stationName'>" + i.name + "</a></li><li class='divider short'></li>");
                            $(".station_ul").append($content);
                            $(".station_ul .xxc_stationName").off('click');
                            $(".station_ul .xxc_stationName").click(function () {
                                var orgId = $(this).attr('term');
                                $("#station_select").text($(this).text()).attr('term', orgId);
                            });
                        });
                        $("#station_select").text($('.station_ul .xxc_stationName:eq(0)').text()).attr('term', $('.station_ul .xxc_stationName:eq(0)').attr('term'));
                        $(".station_ul li:last").remove();
                    }
                })
            });
        }

        // 按ID加载模板
        function loadDataByTemplateId(templateId) {
            if (templateId) {
                $.ajax({
                    url: "/FlowIndex/GetTemplateInfoById",
                    type: "post",
                    dataType: "json",
                    data: {
                        templateId: templateId,
                        nodeId: null,
                        formId: null,
                        flag: 1
                    },
                    success: rsHandler(function (data) {
                        addflow_templateName = data.template.templateName;
                        addflow_defaultTitle = data.template.defaultTitle;
                        if (addflow_defaultTitle == 1) {
                            $("#preview_title").attr("readonly", "true");
                            $("#preview_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);

                        } else {
                            $("#preview_title").removeAttr("readonly");

                        }
                        $('#preview_modal_title').text(data.template.templateName);
                        $('.template-name').text(data.template.templateName);
                        $('.template-desc').text(data.template.description);
                        saveData = $("#Label-content").widgetFormResolver(data.controlInfo);
                    })
                })
            }
        }



        //取消按钮点击事件
        $('#addflow_cancel').off('click')
        $('#addflow_cancel').click(function () {
            clearFlowContent();
            $('#preview_modal').modal('hide');
        });

        //添加新建流程信息
        $('.addflow_sure').off('click')
        $('.addflow_sure').click(function () {
            var flag = $(this).attr('term');
            addflow_content.templateId = addflow_templateId;
            addflow_content.title = $('.form-title').val();
            if (addflow_content.title == "") {
                validate_reject("标题不能为空", $('.form-title'));
                return;
            }
            else if (!justifyByLetter(addflow_content.title, "标题", $('.form-title'))) {
                return;
            }

            addflow_content.status = 1;
            addflow_content.urgency = $('#preview_modal .liHit').length;
            addflow_content.organizationId = $('#department_select').attr('term');
            addflow_content.stationId = $('#station_select').attr('term');
            addflow_content.createTime = $('#addflow_createtime_v').val();
            var result = saveData.getJson();
            if (result == false) return;
            addflow_content.controlValue = result;
            $.ajax({
                type: "post",
                url: "/FlowIndex/AddFlow",
                data: {
                    flag: flag,
                    data: JSON.stringify(addflow_content)
                },
                dataType: "json",
                success: rsHandler(function (data) {
                    //清空新建流程内容
                    clearFlowContent();
                    $('#preview_modal').modal('hide');
                    $("#addflow_modal").modal('hide');
                    if (data) {
                        ncUnits.alert("新建成功!");
                        loadingFlowList();
                    }
                    else {
                        ncUnits.alert("流程设置有误，请联系管理员!");
                    }


                })
            });
        });
    }).on("hide.bs.modal", function () {
        $("#Label-content").empty();
        //addflow_templateId=null;
        $('#addflow_modal_submit').removeAttr('data-toggle');
        $('#addflow_modal_submit').removeAttr('data-target');
        $('.stars li').removeClass('liHit');
        $('#preview_title').val('');
        $('#addflow_createtime_v').val('');
        $('.addflow_footer').hide();
        $('#addflow-mask').show();
        layer.closeTips();
    });

    //流程详情
    var modalExtend = $("#modal_detail_flowchart_con");
    $("#details-modal").on('show.bs.modal', function () {
        var templateId = current_Info.templateId;
        var formId = current_Info.formId;
        var nodeId = current_Info.currentNode;
        var operateStatus = current_Info.operateStatus;
        var isEntruct = current_Info.isEntruct;  //是否是委托流程
        if (operateStatus == 1) {
            /* 评分五角星 开始 */
            $('.stars ul li').hover(function () {
                var nums = $(this).index();
                var length = $(this).parent().children('li').length - 1;
                for (var i = 0; i <= length; i++) {
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
            $('.stars ul li').click(function () {
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

            //选择完成时间
            $("#flowdetail_createtime").click(function () {
                laydate({
                    elem: '#flowdetail_createtime_v',
                    event: 'click',
                    istime: true,
                    format: 'YYYY-MM-DD hh:mm',
                    isclear: true,
                    istoday: true,
                    issure: true,
                    festival: true,
                    start: new Date().toLocaleDateString() + ' 08:30:00',
                    choose: function (dates) {
                        endTime_v = dates;
                        if (addflow_defaultTitle == 1) {
                            addflow_createTime = dates.replace('-', '/').replace('-', '/');
                            $(".detail_form_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                        }
                    },
                    clear: function () {
                        endTime_v = undefined;
                    }
                });
            });

            $.ajax({
                type: "post",
                url: "/FlowIndex/GetUserOrganizationList",
                dataType: "json",
                success: rsHandler(function (data) {
                    if (data) {
                        var orgId = data.orgList[0].id;
                        $(".detail_department_ul").empty();
                        if (data.orgList && data.orgList.length > 0) {
                            $.each(data.orgList, function (e, i) {
                                var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_detail_departmentName'>" + i.name + "</a></li><li class='divider short'></li>");
                                $(".detail_department_ul").append($content);
                                $(".detail_department_ul .xxc_detail_departmentName").off('click');
                                $(".detail_department_ul .xxc_detail_departmentName").click(function () {
                                    orgId = $(this).attr('term');
                                    $("#detail_createdepart_span").text($(this).text()).attr('term', orgId);
                                    if (addflow_defaultTitle == 1) {
                                        addflow_createdepart = $("#detail_createdepart_span").text();
                                        $(".detail_form_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                                    }

                                    //加载岗位
                                    load_station(orgId);
                                });
                            });
                        }
                        $("#detail_createdepart_span").text($('.detail_department_ul .xxc_detail_departmentName:eq(0)').text()).attr('term', $('.detail_department_ul .xxc_detail_departmentName:eq(0)').attr('term'));
                        addflow_createdepart = $("#detail_createdepart_span").text();
                        $(".detail_department_ul li:last").remove();
                        //加载岗位
                        //load_station(orgId);

                        //加载表单
                        loaddetail();
                    }
                })
            });

            //加载岗位
            function load_station(orgId, stationName) {
                $.ajax({
                    type: "post",
                    url: "/FlowIndex/GetUserStationList",
                    dataType: "json",
                    data: { orgId: orgId },
                    success: rsHandler(function (data) {
                        if (data && data.length > 0) {
                            $(".detail_station_ul").empty();
                            $.each(data, function (e, i) {
                                var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_detail_stationName'>" + i.name + "</a></li><li class='divider short'></li>");
                                $(".detail_station_ul").append($content);
                                $(".detail_station_ul .xxc_detail_stationName").off('click');
                                $(".detail_station_ul .xxc_detail_stationName").click(function () {
                                    var orgId = $(this).attr('term');
                                    $("#detail_createstation_span").text($(this).text()).attr('term', orgId);
                                });
                            });
                            $(".detail_station_ul li:last").remove();
                            if (stationName) {
                                $('.detail_station_ul .xxc_detail_stationName').each(function () {
                                    if ($(this).text() == stationName) {
                                        $("#detail_createstation_span").text($(this).text()).attr('term', $(this).attr('term'));
                                    }
                                });
                            } else {
                                $("#detail_createstation_span").text($('.detail_station_ul .xxc_detail_stationName:eq(0)').text()).attr('term', $('.detail_station_ul .xxc_detail_stationName:eq(0)').attr('term'));
                            }

                        }
                    })
                });
            }
        } else {
            loaddetail();
        }

        
        $("#modal_detail_flowchart_switch").off("click");
        $("#modal_detail_flowchart_switch").click(function () {
            if ($(this).hasClass("unfold")) {
                $(this).removeClass("unfold");
                modalExtend.animate({
                    width: "0",
                    right: "1px",
                    "z-index": "0"
                }, 1000);
            } else {
                $(this).addClass("unfold");
                modalExtend.animate({
                    width: "400px",
                    right: "-399px",
                    "z-index": "-1"
                }, 1000, "", function () {
                    renderFlowChart(formId);
                });
            }
        });
        //加载表单列表
        function loaddetail() {
            $.ajax({
                type: "post",
                url: "/FlowIndex/GetTemplateInfoById",
                dataType: "json",
                data: {
                    templateId: templateId,
                    nodeId: nodeId,
                    formId: formId,
                    flag: 2,
                    operateStatus: operateStatus
                },
                success: rsHandler(function (data) {
                    $('#xxc_detailsuggest').empty();
                    addflow_templateName = data.template.templateName;
                    addflow_defaultTitle = data.template.defaultTitle;
                    if (operateStatus == 1 && addflow_defaultTitle != 1) {
                        $('.detail_form_title').removeAttr('readonly');
                    } else {
                        $('.detail_form_title').attr('readonly', 'true');
                    }
                    $('.detial_template_name').text(addflow_templateName);
                    $('.detail_template_desc').text(data.template.description);
                    $('#detail_modal_title').text(addflow_templateName);
                    //渲染表单
                    newVal = $("#detail_label-content").widgetFormResolver(data.controlInfo);
                    var $suggest = $("<div class='flow_message_title'>建议/意见</div><div>" +
                        "<textarea class='flow_message_content' maxlength='250' id='flow_message'></textarea></div>");
                    $('#xxc_detailsuggest').append($suggest).css("margin-top", "10px");
                    //获取表单控件值
                    $.ajax({
                        type: "post",
                        url: "/FlowIndex/GetFlowDetailListById",
                        dataType: "json",
                        data: { formId: formId, nodeId: nodeId },
                        success: rsHandler(function (data) {
                            if (data) {
                                $('.detail_form_title').val(data.title);
                                if (data.urgency > 0) {
                                    $("#detail_urgencyStart li:lt(" + data.urgency + ")").addClass('liHit');
                                }
                                $('#detail_createuser_input').val(data.createUserName);
                                addflow_createUserName = data.createUserName;
                                addflow_createdepart = data.organizationName;
                                addflow_createTime = data.createTime.toString().replace('T', ' ').substr(0, 16);
                                if (operateStatus == 1) {
                                    $('.detail_department_ul .xxc_detail_departmentName').each(function () {
                                        if ($(this).text() == data.organizationName) {
                                            $("#detail_createdepart_span").text($(this).text()).attr('term', $(this).attr('term'));
                                            load_station($(this).attr('term'), data.stationName);
                                        }
                                    });

                                } else {
                                    $('.detail_department_ul').remove();
                                    $('.detail_station_ul').remove();
                                    $('#detail_modal_content .caret').remove();
                                    $('#detail_createdepart_span').parent().attr('readonly', 'true');
                                    $('#detail_createstation_span').parent().attr('readonly', 'true');
                                    $('#flowdetail_createtime_v').attr('readonly', 'true')
                                    $('#detail_createdepart_span').text(addflow_createdepart).attr('title', addflow_createdepart);
                                    $('#detail_createstation_span').text(data.stationName).attr('title', data.stationName);
                                }
                                $('#flowdetail_createtime_v').val(addflow_createTime);
                                if (data.controlValue && data.controlValue.length > 0) {
                                    for (var i = 0; i < data.controlValue.length; i++) {
                                        var obj;
                                        if (data.controlValue[i].parentControl == "" || data.controlValue[i].parentControl == null || data.controlValue[i].parentControl == "main") {  //非明细列表中的控件

                                            if (data.controlValue[i].controlType == 0) {   //标签
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                obj.find('.item-label').text(data.controlValue[i].description);
                                            }
                                            else if (data.controlValue[i].controlType == 1 || data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3 || data.controlValue[i].controlType == 11) { //输入框
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                obj.find('.item-content .form-control').val(data.controlValue[i].detailValue[0]);
                                            }
                                            else if (data.controlValue[i].controlType == 4 || data.controlValue[i].controlType == 5) {  //单选框及复选框
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                for (var j = 0; j < data.controlValue[i].detailValue.length; j++) {
                                                    if (data.controlValue[i].detailValue[j] == "false") {
                                                        obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", false);
                                                    }
                                                    else {
                                                        obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", true);
                                                    }
                                                }
                                            }
                                            else if (data.controlValue[i].controlType == 6) {  //下拉列表
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                obj.find(".form-control option").each(function () {
                                                    if ($(this).text() == data.controlValue[i].detailValue[0]) {
                                                        $(this).attr('selected', 'selected');
                                                    }
                                                });
                                            }
                                            else if (data.controlValue[i].controlType == 7 || data.controlValue[i].controlType == 8) {  //浏览框(除去文件浏览框)
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                obj.find("input[class='form-control']").val(data.controlValue[i].detailValue[0]);
                                            }
                                            else if (data.controlValue[i].controlType == 12 || data.controlValue[i].controlType == 14) { //日期
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                obj.find(".item-content .form-control").val(data.controlValue[i].detailValue[0]);
                                            }
                                            else if (data.controlValue[i].controlType == 13 || data.controlValue[i].controlType == 15) {  //日期区间
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                obj.find(".item-content .form-control:eq(0)").val(data.controlValue[i].detailValue[0]);
                                                obj.find(".item-content .form-control:eq(1)").val(data.controlValue[i].detailValue[1]);
                                            }
                                            else if (data.controlValue[i].controlType == 9) {   //浏览框(文件浏览框)
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                $.each(data.controlValue[i].detailValue, function (x, v) {
                                                    if (v != "") {
                                                        var thisSaveName = data.controlValue[i].saveName[x];
                                                        var $fileSpan = $('<span style="display:inline-block" class="file" term="' + thisSaveName + '" title=' + v + '><span class="text-overflow" style="display: inline-block;max-width: 200px;vertical-align: top;">' + v + '</span></span>'),
                                                            $del = $("<a class='glyphicon glyphicon-remove clickable' style='visibility: hidden;margin-left: 5px'></a>");

                                                        if (obj.find("input[type='file']").length <= 0) {
                                                            obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                            var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                            upLoadValue.push(fileName);
                                                        }
                                                        else {
                                                            var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                            upLoadValue.push(fileName);
                                                            $fileSpan.append($del).hover(function () {
                                                                $del.css('visibility', 'visible');
                                                            }, function () {
                                                                $del.css('visibility', 'hidden');
                                                            });
                                                            obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                            //删除
                                                            $del.click(function () {
                                                                if (upLoadValue && upLoadValue.length > 0) {
                                                                    $.each(upLoadValue, function (e, xxc_file) {
                                                                        var fileName = xxc_file.split('*');
                                                                        if (fileName && fileName.length > 2 && fileName[1] == thisSaveName) {
                                                                            upLoadValue.splice(e, 1);
                                                                        }
                                                                    });
                                                                }
                                                                $(this).parent().parent().remove();
                                                            });
                                                        }
                                                        //下载
                                                        $fileSpan.find('span').click(function () {
                                                            var displayName = $(this).text();
                                                            $.post("/FlowIndex/Download", { displayName: displayName, saveName: thisSaveName, flag: 0 }, function (data) {
                                                                if (data == "success") {
                                                                    //loadViewToMain("/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1");
                                                                    window.location.href = "/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1";
                                                                }
                                                                else {
                                                                    ncUnits.alert("文件不存在，无法下载!");
                                                                }
                                                                return;
                                                            });
                                                        });
                                                    }
                                                })
                                            }
                                        }
                                        else {     //明细列表中的控件
                                            if (data.controlValue[i].controlType == 0) {   //标签
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                obj.find('.item-label').text(data.controlValue[i].description);
                                            }
                                            else if (data.controlValue[i].controlType == 1 || data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3 || data.controlValue[i].controlType == 11) { //输入框
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                obj.find('.item-content .form-control').val(data.controlValue[i].detailValue[0]);
                                                if (data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3) {
                                                    obj.find('.item-content .form-control').blur();
                                                }
                                            }
                                            else if (data.controlValue[i].controlType == 4 || data.controlValue[i].controlType == 5) {  //单选框及复选框
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                for (var j = 0; j < data.controlValue[i].detailValue.length; j++) {
                                                    if (data.controlValue[i].detailValue[j] == "false") {
                                                        obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", false);
                                                    }
                                                    else {
                                                        obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", true);
                                                    }
                                                }
                                            }
                                            else if (data.controlValue[i].controlType == 6) {  //下拉列表
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                obj.find(".form-control option").each(function () {
                                                    if ($(this).text() == data.controlValue[i].detailValue[0]) {
                                                        $(this).attr('selected', 'selected');
                                                    }
                                                });
                                            }
                                            else if (data.controlValue[i].controlType == 7 || data.controlValue[i].controlType == 8) {  //浏览框(非文件浏览框)
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                obj.find("input[class='form-control']").val(data.controlValue[i].detailValue[0]);
                                            }
                                            else if (data.controlValue[i].controlType == 12 || data.controlValue[i].controlType == 14) { //日期
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                obj.find(".item-content .form-control").val(data.controlValue[i].detailValue[0]);
                                            }
                                            else if (data.controlValue[i].controlType == 13 || data.controlValue[i].controlType == 15) {  //日期区间
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                obj.find(".item-content .form-control:eq(0)").val(data.controlValue[i].detailValue[0]);
                                                obj.find(".item-content .form-control:eq(1)").val(data.controlValue[i].detailValue[1]);
                                            }
                                            else if (data.controlValue[i].controlType == 9) {   //浏览框(文件浏览框)
                                                obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                $.each(data.controlValue[i].detailValue, function (x, v) {
                                                    if (v != "") {
                                                        var thisSaveName = data.controlValue[i].saveName[x];
                                                        var $fileSpan = $('<span style="display:inline-block" class="file" term="' + thisSaveName + '" title=' + v + '><span class="text-overflow" style="display: inline-block;max-width: 200px;vertical-align: top;">' + v + '</span></span>'),
                                                            $del = $("<a class='glyphicon glyphicon-remove clickable' style='visibility: hidden;margin-left: 5px'></a>");

                                                        if (obj.find("input[type='file']").length <= 0) {
                                                            obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                            var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                            upLoadValue.push(fileName);
                                                        }
                                                        else {
                                                            var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                            upLoadValue.push(fileName);
                                                            $fileSpan.append($del).hover(function () {
                                                                $del.css('visibility', 'visible');
                                                            }, function () {
                                                                $del.css('visibility', 'hidden');
                                                            });
                                                            obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                            //删除
                                                            $del.click(function () {
                                                                if (upLoadValue && upLoadValue.length > 0) {
                                                                    $.each(upLoadValue, function (e, xxc_file) {
                                                                        var fileName = xxc_file.split('*');
                                                                        if (fileName && fileName.length > 2 && fileName[1] == thisSaveName) {
                                                                            upLoadValue.splice(e, 1);
                                                                        }
                                                                    });
                                                                }
                                                                $(this).parent().parent().remove();
                                                            });
                                                        }
                                                        //下载
                                                        $fileSpan.find('span').click(function () {
                                                            var displayName = $(this).text();
                                                            $.post("/FlowIndex/Download", { displayName: displayName, saveName: thisSaveName, flag: 0 }, function (data) {
                                                                if (data == "success") {
                                                                    window.location.href = "/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1";
                                                                    //loadViewToMain("/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1");
                                                                }
                                                                else {
                                                                    ncUnits.alert("文件不存在，无法下载!");
                                                                }
                                                                return;
                                                            });
                                                        });
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }

                            }
                            var $flowdetail_cancel = $("<a id='flowDetail_modal_cancel' class='btn btn-transparency btn-lg user-defined' data-dismiss='modal'>取消</a>");
                            var $flowdetail_save = $("<a class='btn btn-transparency btn-lg user-defined'>保存</a>");
                            var $flowdetail_submit = $("<a class='btn btn-transparency btn-lg user-defined'>提交</a>");
                            var $flowdetail_back = $("<a class='btn btn-transparency btn-lg user-defined'>退回</a>");
                            var $flowdetail_agree = $("<a class='btn btn-transparency btn-lg user-defined'>同意</a>");
                            var $flowdetail_submitMag = $("<a class='btn btn-transparency btn-lg user-defined'>提交</a>");
                            if (operateStatus == 1) {  //待提交
                                $flowdetail_cancel.css("width", "33.2%");
                                $flowdetail_save.css("width", "33.4%");
                                $flowdetail_submit.css("width", "33.4%");
                                $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_save, $flowdetail_submit]);
                                $("#xxc_detailsuggest").hide();
                            }
                            else if (operateStatus == 6) {  //待审核
                                $flowdetail_cancel.css("width", "33.2%");
                                $flowdetail_back.css("width", "33.4%");
                                $flowdetail_agree.css("width", "33.4%");
                                $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_back, $flowdetail_agree]);
                                $("#xxc_detailsuggest").show();
                            }
                            else if (operateStatus == 10) {    //流程中的提交
                                $flowdetail_cancel.css("width", "33.2%");
                                $flowdetail_save.css("width", "33.4%");
                                $flowdetail_submitMag.css("width", "33.4%");
                                $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_save, $flowdetail_submitMag]);
                                $("#xxc_detailsuggest").show();
                            }
                            else {
                                $("#xxc_detailsuggest").hide();
                            }

                            //退回
                            $flowdetail_back.click(function () {
                                var msg = $('#flow_message').val();
                                var reg = /<\w+>/;
                                if (msg.indexOf('null') >= 0 || msg.indexOf('NULL') >= 0 || msg.indexOf('&nbsp') >= 0 || reg.test(msg) || msg.indexOf('</') >= 0) {
                                    ncUnits.alert("意见存在非法字符!");
                                }
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/TurnBack",
                                    dataType: "json",
                                    data: { nodeId: nodeId, templateId: templateId, formId: formId, suggest: msg, isEntruct: isEntruct },
                                    success: rsHandler(function (data) {
                                        $("#details-modal").modal("hide");
                                        if (data) {
                                            ncUnits.alert("退回操作成功!");
                                            loadingFlowList();
                                        } else {
                                            ncUnits.alert("流程设置有误，请联系管理员!");
                                        }
                                    })
                                });
                            });

                            //同意
                            $flowdetail_agree.click(function () {
                                var msg = $('#flow_message').val();
                                var reg = /<\w+>/;
                                if (msg.indexOf('null') >= 0 || msg.indexOf('NULL') >= 0 || msg.indexOf('&nbsp') >= 0 || reg.test(msg) || msg.indexOf('</') >= 0) {
                                    ncUnits.alert("意见存在非法字符!");
                                }
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/AgreeFormFlow",
                                    dataType: "json",
                                    data: { templateId: templateId, formId: formId, nodeId: nodeId, isEtruct: isEntruct, suggest: msg },
                                    success: rsHandler(function (data) {
                                        $("#details-modal").modal("hide");
                                        if (data) {
                                            ncUnits.alert("流程审批通过!");
                                            loadingFlowList();
                                        }
                                        else {
                                            ncUnits.alert("流程设置有误，请联系管理员!");
                                        }
                                    })
                                });
                            });

                            //保存
                            $flowdetail_save.click(function () {
                                var title = $('.detail_form_title').val();
                                var urg = $('#detail_urgencyStart .liHit').length;
                                var org = $('#detail_createdepart_span').attr('term');
                                var sta = $('#detail_createstation_span').attr('term');
                                var createtime = $('#flowdetail_createtime_v').val();

                                var result = newVal.getJson();
                                if (result == false) {
                                    ncUnits.alert("保存失败!");
                                    return;
                                }
                                var formInfo = {
                                    title: title,
                                    urgency: urg,
                                    organizationId: org,
                                    stationId: sta,
                                    createTime: createtime,
                                    controlValue: result
                                };
                                var flag;
                                if (operateStatus == 1) {
                                    flag = 1
                                } else {
                                    flag = 2;
                                }
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/SaveControlValue",
                                    dataType: "json",
                                    data: { formId: formId, flag: flag, data: JSON.stringify(formInfo) },
                                    success: rsHandler(function (data) {
                                        if (flag) {
                                            ncUnits.alert("保存成功!");
                                            loadingFlowList();
                                        } else {
                                            ncUnits.alert("保存失败!");
                                        }
                                        $("#details-modal").modal("hide");
                                    })
                                })
                            });

                            //提交
                            $flowdetail_submit.click(function () {
                                var title = $('.detail_form_title').val();
                                var urg = $('#detail_urgencyStart .liHit').length;
                                var org = $('#detail_createdepart_span').attr('term');
                                var sta = $('#detail_createstation_span').attr('term');
                                var createtime = $('#flowdetail_createtime_v').val();

                                var result = newVal.getJson();
                                if (result == false) {
                                    ncUnits.alert("提交失败!");
                                    return;
                                }
                                var formInfo = {
                                    title: title,
                                    urgency: urg,
                                    organizationId: org,
                                    stationId: sta,
                                    createTime: createtime,
                                    controlValue: result
                                };
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/SubmitFlow",
                                    dataType: "json",
                                    data: { templateId: templateId, formId: formId, nodeId: nodeId, flag: 1, data: JSON.stringify(formInfo) },
                                    success: rsHandler(function (data) {
                                        $("#details-modal").modal("hide");
                                        if (data) {
                                            ncUnits.alert("流程提交成功!");
                                            loadingFlowList();
                                        }
                                        else {
                                            ncUnits.alert("流程提交失败!");
                                        }


                                    })
                                });
                            });

                            //流程中的提交
                            $flowdetail_submitMag.click(function () {
                                var result = newVal.getJson();
                                if (result == false) {
                                    ncUnits.alert("提交失败!");
                                    return;
                                }
                                var formInfo = {
                                    controlValue: result
                                };
                                var msg = $('#flow_message').val();
                                var reg = /<\w+>/;
                                if (msg.indexOf('null') >= 0 || msg.indexOf('NULL') >= 0 || msg.indexOf('&nbsp') >= 0 || reg.test(msg) || msg.indexOf('</') >= 0) {
                                    ncUnits.alert("意见存在非法字符!");
                                }
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/SubmitInFlow",
                                    dataType: "json",
                                    data: { templateId: templateId, formId: formId, nodeId: nodeId, suggest: msg, isEntruct: isEntruct, flag: 2, data: JSON.stringify(formInfo) },
                                    success: rsHandler(function (data) {
                                        $("#details-modal").modal("hide");
                                        if (data) {
                                            ncUnits.alert("提交成功!");
                                            loadingFlowList();
                                        }
                                        else {
                                            ncUnits.alert("流程设置有误，请联系管理员!");
                                        }
                                    })
                                })
                            });

                        })
                    });
                })
            });
        }

    }).on('hide.bs.modal', function () {
        $("#details-modal .modal-footer").empty();
        $("#modal_detail_flowchart_switch").removeClass("unfold");
        modalExtend.animate({
            width: "0",
            right: "1px",
            "z-index": "0"
        }, 1000);
    });

    //判断某变量是否具有非法字符
    function justifyByLetter(txt, name, obj) {
        var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            validate_reject(name, obj);
            return false;
        }
        return true;
    }

    //清空新建完成后预览画面的内容
    function clearFlowContent() {
        $('.form-title').val('');
        $('.liHit').removeClass('liHit');
        $('#department_select').text('').removeAttr('term');
        $('.department_ul li').remove();
        $('#addflow_createtime_v').val('');
        addflow_templateName = "";
        addflow_createdepart = "";
        addflow_createTime = "";
        addflow_createUserName = "";
    }

    //----计划------//

    //计划新建
    var planCreate = addPlan();
    $('#planNewTab').click(function () {
        planCreate.addPlan();
    });



    //计划详情
    /*------开始--------*/
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
                                if (plan.loopType == "0") {
                                    submitoperate(planId, detail_initial);
                                } else {
                                    submitoperate(planId, detail_initial, 1);   //循环计划提交操作
                                }

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
                                loadingPlanList();
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
                    checkclick();
                }
                else if (operateType == "确认") {
                    DetailConfirm();
                }
                else if (operateType == "提交确认") {
                    submitconfirm();
                }
                else if (operateType == "撤销") {
                    $("#detailAccessory").hide();
                    $('#detailAccessory .accessoryDiv').hide().find('ul').html('');
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
    //获取计划详情
    function detail_window() {
        /*获取计划详情 开始*/
        var planId = current_Info.isLoopPlan == 0 ? current_Info.planId : current_Info.loopId;
        var status = current_Info.status;
        var stop = current_Info.stop;
        var isloop = current_Info.isLoopPlan;
        var withfront = current_Info.isFronPlan;
        var initial = current_Info.initial;
        var collPlan = current_Info.IsCollPlan;
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
                    $("#detailAccessory .accessory").off("click");
                    $("#detailAccessory .accessory").click(function () {
                        if ($(this).next(".accessoryDiv").css("display") == "none") {
                            $(this).next(".accessoryDiv").show();
                        } else {
                            $(this).next(".accessoryDiv").hide();
                        }
                        $('#detailAccessory .accessoryDiv ul').html('');
                        $('#detailAccessory .accessoryDiv ul').append("<li class='allDownload'>全部下载</li>");
                        for (var i = 0; i < data.planAttachmentList.length; i++) {
                            $('#detailAccessory .accessoryDiv ul').append("<li class='liAccessory' savename='" + data.planAttachmentList[i].saveName + "' term='" + data.planAttachmentList[i].attachmentId + "'><span class='content'>" + data.planAttachmentList[i].attachmentName + "</span></li>");
                        }

                        //单个下载
                        $('.liAccessory').off('click');
                        $('.liAccessory').click(function () {
                            downloadfile($(this));
                        });

                        //全部下载
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
                planId: planId, isloop: isloop
            },
            success: rsHandler(function (data) {
                var $ul = $("<ul class='list-unstyled'></ul>");
                for (var i = 0, len = data.length; i < len; i++) {
                    var info = data[i];
                    var $li = $("<li></li>");
                    var $span1 = $("<span style='width: 50%;max-width:420px' class='textOver' title=" + info.message + "></span>");
                    var $span2 = $("<span style='width: 50%;text-align: -webkit-right;float:right;'></span>");
                    $li.append([$span1, $span2]).appendTo($ul);

                    var str = "<span style='color:#58b456;'>" + info.user + "</span>";
                    if (isloop == 1) {   //循环计划
                        if (info.type == 1) {
                            str += "提交了循环计划结果";
                        } else if (info.type == 2) {
                            str += ("审核通过了该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 3) {
                            str += ("审核未通过该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 4) {
                            str += ("撤销了该计划的提交" + (info.message ? (":" + info.message) : ""));
                        } else if (info.type == 5) {
                            str += ("撤销了该计划的审核" + (info.message ? (":" + info.message) : ""));
                        } else if (info.type == 6) {
                            str += ("确认通过了该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 7) {
                            str += ("确认未通过该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 8) {
                            str += "中止了该计划";
                        } else if (info.type == 9) {
                            str += "申请修改了该计划";
                        } else if (info.type == 10) {
                            str += "保存提交了该计划";
                        } else if (info.type == 11) {
                            str += "下载了附件";
                        } else if (info.type == 12) {
                            str += "新建了该循环计划";
                        } else {
                            str = "异常操作"
                        }
                    }
                    else {
                        if (info.type == 1) {
                            str += "提交了该计划";
                        } else if (info.type == 2) {
                            str += ("审核通过了该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 3) {
                            str += ("审核未通过该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 4) {
                            str += "撤销了该计划的提交";
                        } else if (info.type == 5) {
                            str += "撤销了该计划的审核";
                        } else if (info.type == 6) {
                            str += ("发表了评论" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 7) {
                            str += "下载了附件";
                        } else if (info.type == 8) {
                            str += ("查看了你的计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        } else if (info.type == 9) {
                            str += "转办了该计划";
                        } else if (info.type == 10) {
                            str += "申请修改该计划";
                        } else if (info.type == 11) {
                            str += "申请中止该计划";
                        } else if (info.type == 12) {
                            str += "重新开始该计划";
                        }
                        else if (info.type == 13) {
                            str += "删除该计划";
                        }
                        else if (info.type == 14) {
                            str += ("确认通过了该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        }
                        else if (info.type == 15) {
                            str += ("确认未通过该计划" + ((info.message && info.message != "") ? (":" + info.message) : ""));
                        }
                        else if (info.type == 16) {
                            str += "更新了该计划的进度";
                        }
                        else if (info.type == 17) {
                            str += "对该计划进行了分解";
                        }
                        else if (info.type == 18) {
                            str += "新建了该计划";
                        }
                        else if (info.type == 20) {
                            str += "修改保存了该计划";
                        } else {
                            str = "异常操作"
                        }
                    }

                    $span1.html(str);
                    $span2.html(info.time.replace('T', ' ').substr(0, 16));
                }
                $("#plan_detail_modal .listView").html($ul);
            })
        });
        if (batch_flag) {
            $("#plan_detail_modal .modal-footer").hide();
        } else {
            if (collPlan == 1) {
                if (status == 90 || stop == 90) {
                    $("#plan_detail_modal .modal-footer").hide();
                }
                else {
                    $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                    $("#detail_span2").hide();
                    $("#detail_span3").hide();
                }
                editDisabled();
            }

            else if (listArgus.calendarType == 1) {   //我的日程
                if ((status == 0 || status == 15) && stop == 0) {//待提交
                    $("#plan_detail_modal .modal-footer").show();
                    $("#xxc_initial").val(initial);
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
                    $("#plan_detail_modal .modal-footer").show();
                    if (isloop == "1") {
                        $("#detail_span1").hide();
                    } else {
                        $("#detail_span1").text('评论').attr('operatetype', '评论').css("width", "100%");
                    }
                    $("#detail_span2").hide();
                    $("#detail_span3").hide();
                    editDisabled();
                } else if ((status == 20 || status == 40) && stop == 0) {//已审核
                    $("#plan_detail_modal .modal-footer").css("display", "block");
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
                    $("#plan_detail_modal .modal-footer").show();
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
                    editDisabled();
                }
                else if (stop == 90) {
                    editDisabled();
                }
            }
            else if (listArgus.calendarType == 2) {   //下属日程
                if (((status == 10 || status == 25) && stop == 0) || stop == 10) {//待审核
                    $("#plan_detail_modal .modal-footer").show();
                    if (isloop == "1") {
                        $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '100%');
                        $("#detail_span2").hide();
                    } else {
                        $("#detail_span2").text('评论').attr('operatetype', '评论').css('width', '50%');
                        $("#detail_span1").text('审核').attr('operatetype', '审核').css('width', '50%');
                    }
                    $("#detail_span3").hide();
                    editDisabled();
                }
                else if ((status == 30 && stop == 0) || (isloop == "1" && current_Info.status == 20)) {//待确认
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
                }
            }
        }
    }
    //计划详情的评论
    function Discuss(obj, planId) {
        $("#detail_operateinfo").load("/plan/DiscussView", { height: 640, planId: planId }, function () {
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
                    fnScreCon();
                }
                else {
                    ncUnits.alert("撤销失败！");
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

    $("#plan_detail_modal").on("hide.bs.modal", function () {
        $(".listView").empty();
        $("#detail_partner_div").find('span:not(.detail_partner_span)').remove();
        $("#detail_front_div").find('span:not(.detail_front_span)').remove();
        $("#detailAccessory").hide();
        $("#detail_operateinfo").html('').hide();
        $("#plan_detail_modal .modal-footer").hide();
        batch_flag = false;
        batch_planId = [];
        batch_loopId = [];
    });
    /*------结束--------*/


    //转办
    /*------开始--------*/
    //责任人
    personChosen($("#transmitplan_responsibleUser"), $("#transmitplan_confirmUser"));
    //确认人
    personChosen($("#transmitplan_confirmUser"), $("#transmitplan_responsibleUser"));
    //取消
    $("#layer_Transmitplan").on("hide.bs.modal", function () {
        $("#transmitplan_responsibleUser,#transmitplan_confirmUser").val('').attr("term", '');
        layer.close(selectUserTip);
    })
    //确定
    $("#xxc_makesure").click(function () {
        var planIdNew = $("#layer_Transmitplan").attr("data_planId");
        var responsibleUser_t = $("#transmitplan_responsibleUser").attr("term");
        var confirmUser_t = $("#transmitplan_confirmUser").attr("term");
        if (responsibleUser_t == "") {
            ncUnits.alert("请选择责任人");
            return;
        }
        else if (confirmUser_t == "") {
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
                    loadingPlanList();
                }
                else {
                    ncUnits.alert("计划转办失败！");
                }
            })
        });
    });
    /*------结束--------*/


    //计划通用函数
    /*------开始--------*/

    //单个文件下载
    function downloadfile(obj) {
        var attachmentName = obj.find('span').text();
        var saveName = obj.attr('savename');
        $.post("/plan/Download", { displayName: attachmentName, saveName: saveName, flag: 0 }, function (data) {
            if (data == "success") {
                window.location.href = "/plan/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1";
            }
            return;
        });
    }

    //全部下载
    function downloadaddfile(planId) {
        $.post("/plan/MultiDownload", { planId: planId, flag: 0 }, function (data) {
            if (data == "success") {
                //loadViewToMain("/plan/MultiDownload?planId=" + planId + "&flag=1");
                window.location.href = "/plan/MultiDownload?planId=" + planId + "&flag=1";
            }
            return;
        });
    }

    //人员选择
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

    var setting = {
        view: {
            showIcon: false,
            showLine: false,
            selectedMulti: false
        }
    };

    function getNodeNameLine(node, nameLine) {
        if (node) {
            return getNodeNameLine(node.getParentNode(), nameLine ? node.name + " - " + nameLine : node.name);
        } else {
            return nameLine;
        }
    }

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

    /*------结束--------*/


    //分解
    /*------开始--------*/

    var splitModalFlag = true;
    var planInfos = [];
    $("#planSplit_modal").on("show.bs.modal", function () {
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
                            loadingPlanList();
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
                            console.log($li.html());
                            var id = $($li).attr("term");
                            for (var i = 0; i < planInfos.length; i++) {
                                if (planInfos[i].selfId == id) {
                                    planInfos.splice(i, 1);
                                    break;
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
                    $('.addB').css('display', 'initial');
                    // $(this).next().trigger('click');
                    $('.addB').off('click');
                    $('.addB').click(function () {
                        planInfos[id] = getPlanInfo(id);
                        $deleteSpan.css('display', 'initial');
                        $('.addA').css('display', 'initial');
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
        $("#planSplit_modal .decplanList").empty();
        planInfos = [];
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





    //----目标------//

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

    $(" #new-startTime, #new-endTime ").click(function () {
        var id = "#" + $(this).attr("id");
        timeThree(id, null, null, null, null);
    });

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
        $("#new-startTime,#new-endTime").val("");

        $("#new-responsibleUser,#new-confirmUser,#new-description").val("");
        $("#new-responsibleUser,#new-confirmUser").attr("title", "");
        $("#new-responsibleUser,#new-confirmUser").attr("term", "");
        $("#new-responsibleUser,#new-confirmUser").prop("disabled", false);
        $("#new-responsibleUser,#new-confirmUser").siblings("a").removeClass("disabled");
        $("#formula_modal_monitor").text("");

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
            endTime = $("#new-endTime").val();
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

    /*人力资源 开始*/
    var personWithSub = null, personOrgId = null, HrModalTab = 1;
    $("#Object_HR_modal").on('show.bs.modal', function () {
        $('#person-selectall').parent().hide();
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#Objective_HR_modal_folder"), $.extend({
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
                                            $("#Objective_HR_modal_chosen li").each(function () {
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
    $("#Objective_HR_modal_search").selection({
        url: "/Shared/GetUserListByName",
        hasImage: true,
        selectHandle: function (data) {
            $("#Objective_HR_search").val(data.name);
            var flag = true;
            if ($("#Objective_HR_modal_chosen li").length > 0) {
                $("#Objective_HR_modal_chosen li").each(function () {
                    if ($(this).attr('term') == data.id) {
                        flag = false;
                    }
                });
            }
            if (flag == true) {
                $("#Objective_HR_modal_chosen").empty();
                var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                           $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $("#Objective_HR_modal_chosen").append($checked.append($close));
                $close.click(function () {
                    var nodeId = $(this).parent().attr("term");
                    $(this).parent().remove();
                    $("#Objective_HR_modal_chosen_count").text($("#Objective_HR_modal_chosen li").length);
                    $(".person_list ul").each(function () {
                        if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                            $(this).find("input[type='checkbox']").prop("checked", false);
                        }
                    });
                });
            }
            if ($(".person_list ul").length > 0) {
                $(".person_list ul").each(function () {
                    if ($(this).find("li:eq(1)").attr('term') == data.id.toString()) {
                        $(this).find("input[type='checkbox']").prop("checked", true);
                    }
                });
            }
            $("#Objective_HR_modal_chosen_count").text($("#Objective_HR_modal_chosen li").length);
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
                $(".person_list ul input:checked").prop("checked", false);
                $(this).prop('checked', true);
                $('#Objective_HR_modal_chosen_count').text(1);
                var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $close.click(function () {
                    $(".person_list ul input:checked").prop("checked", false);
                    $(this).parent().remove();
                    $('#Objective_HR_modal_chosen_count').text(0);
                });
                $("#Objective_HR_modal_chosen").empty().append($checked.append($close));
            } else {
                $(this).prop('checked', false);
                $("#Objective_HR_modal_chosen").empty();
                $('#Objective_HR_modal_chosen_count').text(0);
            }
        })
    }

    //包含下级
    $("#Objective_person_haschildren").click(function () {
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
                        $("#Objective_HR_modal_chosen li").each(function () {
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
                $("#Objective_HR_modal_chosen li").each(function () {
                    if ($(this).attr('term') == term) {
                        $(this).remove();
                    }
                });
            });
            $(".person_list ul input[type='checkbox']").prop('checked', true);

            var length = $(".person_list input[type='checkbox']:checked").length
            $('#Objective_HR_modal_chosen_count').text(length);
            for (var i = 0; i < length; i++) {
                showflag = true;
                var personId = $(".person_list ul:eq(" + i + ")").find("li:eq(1)").attr('term');
                var personName = $(".person_list ul:eq(" + i + ")").find("li:eq(1) span:eq(0)").text();
                $("#Objective_HR_modal_chosen li").each(function () {
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
                    $("#Objective_HR_modal_chosen").append($checked.append($close));
                    $close.click(function () {
                        var $thisId = $(this).parent().attr('term');
                        $(this).parent().remove();
                        $('#Objective_HR_modal_chosen_count').text($("#Objective_HR_modal_chosen li").length);
                        $(".person_list ul").each(function () {
                            if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                $(this).find("input[type='checkbox']").prop("checked", false);
                            }
                        });
                        $("#person-selectall").prop("checked", false);
                    });
                }

            }
            $('#Objective_HR_modal_chosen_count').text($("#Objective_HR_modal_chosen li").length);

        }
        else {
            $(".person_list ul").each(function () {
                var term = $(this).find("li:eq(1)").attr("term");
                $("#Objective_HR_modal_chosen li").each(function () {
                    if ($(this).attr('term') == term) {
                        $(this).remove();
                    }
                });
            });
            $(".person_list ul input[type='checkbox']").prop('checked', false);
            var length = $("#Objective_HR_modal_chosen li").length
            $('#Objective_HR_modal_chosen_count').text(length);
        }
    });

    //确定
    $("#Objective_HR_modal_submit").click(function () {
        if ($('#Objective_HR_modal_chosen_count').text() == 0) {
            ncUnits.alert("请选择授权人!");
        } else {
            var personId = parseInt($("#Objective_HR_modal_chosen li").attr("term"));
            if (personId == current_Info.responsibleUser) {
                ncUnits.alert("责任人与授权人不得相同!");
            } else {
                $.ajax({
                    type: "post",
                    url: "/ObjectiveIndex/AuthorizeObjective",
                    dataType: "json",
                    data: { objectiveId: current_Info.objectiveId, authorizedUser: personId },
                    success: rsHandler(function (data) {
                        ncUnits.alert("授权成功!");
                        $('#Object_HR_modal').modal('hide');
                        loadingFObjectList();
                    })
                })
            }
        }
    });

    //人员资源模态框关闭事件
    $("#HR_modal").on('hide.bs.modal', function () {
        $("#HR_modal_search").val('');
        $("#Objective_person_haschildren").prop("checked", false);
        $("#person-selectall").prop("checked", false);
        $("#Objective_HR_modal_chosen_count").text(0);
        $(".person_list").empty();
        $("#Objective_HR_modal_chosen li").remove();
    });
    /*人力资源 结束*/


    ///*目标删除 开始*/
    ////删除事件  parentChild=1表示是第一层目标事件，否则表示子目标事件
    //function delObject(value, parentChild) {
    //    var objectId = parseInt($(value).closest(".containerListOne").attr("term"));
    //    var confirmText = '确定要删除吗?';
    //    if (parentChild == 3 && parseInt($(value).attr("child")) > 0) {
    //        confirmText = "该目标有子目标,确定要删除吗?"
    //    }
    //    ncUnits.confirm({
    //        title: '提示',
    //        html: confirmText,
    //        yes: function (layer_delete) {
    //            layer.close(layer_delete);
    //            $.ajax({
    //                type: "post",
    //                url: "../../test/data/success.json",
    //                dataType: "json",
    //                data: { objectiveId: objectId },
    //                success: rsHandler(function (data) {
    //                    if (data) {
    //                        ncUnits.alert("删除成功!");
    //                        if (parentChild == 1) {                     //目标
    //                            loadingFObjectList();
    //                        } else if (parentChild == 2) {           //展开中子目标的删除
    //                            //   unfoldObjectCall(null,hrHorizontal);                    //重新加目标展开页面
    //                        }
    //                    }
    //                    else
    //                        ncUnits.alert('删除失败');
    //                })
    //            });
    //        }
    //    });
    //}
    ///*目标删除 结束*/



    /*目标详情 开始*/

    /*目标详情 结束*/


    /*目标提交 开始*/

    /*目标提交 结束*/


    /*目标修改 开始*/

    /*目标修改 结束*/


    /*目标展开 开始*/
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
            data: { objectiveId: parseInt($("#objective_open_id").val()) },
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
                    current_Info = data;
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
            var parent = $($unfoldNode).closest(".objectChild");
            if (parent.length != 0) {            //表示刷新授权目标所在层
                loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                $(parent).remove();
            } else {             //表示刷新最顶层
                loadingFObjectList();
            }
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
            if (current_Info.authorizedUser != null && current_Info.authorizedUser != loginId) {
                $newDiv.find(".modifyDocumentAdd").hide();
            } else {
                if (v.status < 4 && (loginId == current_Info.responsibleUser || loginId == current_Info.confirmUser || loginId == current_Info.authorizedUser)) {
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
                current_Info = v;
                $("#object_detail_modal").modal("show");
            });
            //修改事件
            $liModify.click(function () {
                current_Info = v;
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
            $("#objectiveNew_modal").css("z-index", "1060").modal('show');
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
    /*目标展开 结束*/


    /*目标新建 开始*/
    var FormulaArray = new Array();//全局变量存储公式
    var formulaEditOrAdd = null;//标记当前为公式编辑还是添加状态
    var formulaNum = 0;//公式编号
    var formulaSetInput = [[], []];//存放当前编辑的公式
    var newOrDecomposition;//全局变量，区分是新建还是分解新建：1 新建：2 分解 3：保存后再次点击详情再次打开
    var parentObjectiveStartTime, parentObjectiveEndTime;//父目标开始时间，父目标结束时间
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
        alarmTime: null,              //警戒时间
        responsibleOrg: null,              //责任部门
        responsibleUser: null,             //责任人
        confirmUser: null,                 //确认人
        formula: null,                     //公式 0：无公式 1：默认公式 2：自定义
        maxValue: null,
        minValue: null,
        objectiveFormula: []             //目标公式为自定义的场合
    };
    //加载目标列表参数
    var objectListArgus = {
        soonSelect: Number,   //快捷查询，忽略其他筛选条件：1：待提交 2：待审核 3：审核通过 4：待确认 5：已完成 6、超时 int
        status: [],            //目标状态 1：待提交 2：待审核 3：审核通过（进行中、已超时） 4：待确认 5：已完成 int[]
        department: [],  //目标对象组织架构, int[]
        person: [],     //目标对象人员 int[]
        startTime: [],      //目标预计开始时间：1、近一周  2、近一月  3、自定义 string[]
        objectiveType: 1       //1、我的目标  2、下属目标 int
    },
        personModal = 1,
        object_info = {};

    //全局数组
    var statusArray = ["待提交", "待审核", "审核通过", "待确认", "已完成"],     //1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成
        checkTypeArray = ["金额", "时间", "数字"];               ////考核类型 1：金额 2：时间 3：数字

    var operateResult = ["创建", "删除", "授权", "提交", "撤销", "审核通过", "审核不通过", "修改", "分解目标", "更新进度", "确认通过", "确认不通过", "下载文档", "查看", "上传文档", "删除文档", "提交确认", "取消授权"];
    var messageResult = [6, 7, 8, 11, 12];
    var $activeNode, $unfoldNode;                //存储授权节点，便于刷新该目标层
    $("#newObjectiveBtn").click(function () {
        newOrDecomposition = 1;
        $("#objectiveNew_modal").modal('show');
        newInit();
    });

    $("#object_detail_modal").on('show.bs.modal', function () {
        objectDetail(current_Info.objectiveId);
        if (detailFlag == 2) {       //如果是审核
            if (loadFormaluFlag) {
                $("#FormularViewTab .modal-body").empty();
                FormaluView(FormulaList, maxValue, minValue, formulaType, $("#FormularViewTab .modal-body"));
            }
        }
    });

    //详情框关闭
    $("#object_detail_modal").on('hide.bs.modal', function () {
        detailFlag = 1;
        $("#FormularViewTab").removeClass("active");
        $("#detailViewTab").addClass("active");
        $("#object_detail_modal .rightModal").hide();
        FormulaList = null;
        $("#object_detail_modal .modal-body").empty();
        FormulaArray.length = 0;
        loadFormaluFlag = true;
    });

    //目标新建
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
                        loadingFObjectList();
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
                        loadingFObjectList();
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
    /*目标新建 结束*/

    var fieldArray = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "警戒时间", "权重", "奖励基数", "数字"];
    var FormulaList, formulaType, maxValue, minValue;
    //目标详情内容
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
                        loadDocument(this, 2, objectiveId, v.displayName, v.saveName, v.extension);
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
                    if (data.result != 0) {
                        ncUnits.alert("审核成功！");
                        $("#object_detail_modal").modal('hide');
                        loadingFObjectList();
                        //statusCount();
                        //var parent = $($activeNode).closest(".objectChild");
                        //if (parent.length != 0) {            //表示刷新授权目标所在层
                        //    loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
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
                            loadingFObjectList();
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

    //目标详情
    $("#objectDetailLabel").click(function () {
        $("#FormularViewTab").removeClass("active");
        $("#detailViewTab").addClass("active");
        objectDetail(current_Info.objectiveId);
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
                data: { objectiveId: current_Info.objectiveId, actualValue: actualValue },
                success: rsHandler(function (data) {
                    if (data) {
                        ncUnits.alert("提交成功！");
                        $("#object_detail_modal").modal('hide');
                        //statusCount();
                        //var parent = $($activeNode).closest(".objectChild");
                        //if (parent.length != 0) {            //表示刷新授权目标所在层
                        //    loadChildObjectList($(parent).prev(), parseInt($(parent).attr("term")));
                        //    $(parent).remove();
                        //} else {             //表示刷新最顶层
                        //    loadObjectList();
                        //}
                        loadingFObjectList();
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

    function controlInput($value) {
        var reg = /^\d+(\.\d{0,2})?/,
            vals = $value.val().match(reg);

        $value.val(vals ? vals[0] : "");
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
                var x = true;
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

    var formulaSetCheck1 = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "警戒时间", "权重", "奖励基数", "数字"];
    var formulaSetCheck2 = ["实际值", "指标值", "理想值", "开始时间", "结束时间", "警戒时间", "权重", "奖励基数", "数字", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];

    $(".areabox a").click(function () {
        if ($(this).attr("id") != "formula_modal_time") {
            $("#formula_modal_monitor").text($("#formula_modal_monitor").text() + $(this).attr("value"));
            var x = false;
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

        var formulaSet = [[], []];//将输入数据进行处理
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
                    var x = false;
                    formulaSet[1].push(x);
                }
            }
            else if (number != null && rep.test(formulaSetInput[0][i]) == false) {
                formulaSet[0].push(number);
                var x = false;
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
                valueType: ""
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


    //--------------------公共函数
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

    //目标修改
    function objectiveModify(objectId) {
        $("#object_modify_modal input").val("");
        $("#object_modify_modal table").empty();
        $.ajax({
            type: "post",
            //url: "/ObjectiveIndex/GetObjectInfo",              //目标修改URL
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
            //if ($("#modifyAlertTime").val() == "") {
            //    ncUnits.alert("警戒时间不能为空!");
            //    return;
            //}
            //argus.alarmTime = $("#modifyAlertTime").val();
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
                        if ($activeNode.closest("ul").hasClass("unfoldChildOpera")) {
                            unfoldObjectCall(null, hrHorizontal);
                        } else {
                            loadingFObjectList();
                        }
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



});

//加载计划列表
function loadingPlanList() {
    var $planContent = $("#contentPlan");
    var lodi;
    $.ajax({
        type: "post",
        url: "/CalendarProcess/GetPlanList",
        dataType: "json",
        data: { data: JSON.stringify(listArgus) },
        beforeSend: function () {
            $planContent.empty();
            lodi = getLoadingPosition($planContent);     //显示load层
        },
        success: rsHandler(function (data) {
            $planContent.empty();
            if (data && data.length > 0) {
                $.each(data, function (i, v) {
                    var $container = $('<div class="row containerListOne relativePos hoverClass"  data_loopId=' + v.loopId + ' data_planId=' + v.planId + '></div>');
                    var $firstDiv = $('<div class="imageCol">' +
                        '<img class="personImage"/>' +
                        '<p>' + v.createUserName + '</p></div>');
                    $firstDiv.find(".personImage").attr("src", v.bigImage);
                    if (listArgus.calendarType == 2&&listArgus.status==1) {
                        if (v.isLoopPlan == '0' || listArgus.status == 2) {
                            var $OK = $('<span class="choseCheck"><a href="#" class="glyphicon glyphicon-ok" data_planId="' + v.planId + '" data_loopId="' + v.loopId + '"></a></span>');
                            $OK.find("a").click(function () {
                                $(this).toggleClass("orangeColor");
                                if ($(this).hasClass("orangeColor")) {
                                    $container.removeClass("hoverClass");
                                } else {
                                    $container.addClass("hoverClass");
                                }
                            });
                            $firstDiv.append($OK);
                        }
                    }
                    var $secondDiv = $('<div class="col-xs-4" ><ul class="list-unstyled listOnePlan">' +
                        '<li>执行方式 : <span>' + v.executionMode + '</span></li>' +
                        '<li>输出结果 : <span  class="textOver" title=' + v.eventOutput + '>' + v.eventOutput + '</span></li>' +
                        '<li>计划完成时间 : <span>' + v.endTime.replace('T', ' ').substr(0, 16) + '</span></li>' +
                        '<li>' + (listArgus.calendarType == 1 ? "确认人" : "责任人") + ' : <span>' + (listArgus.calendarType == 1 ? v.confirmUserName : v.responsibleUserName) + '</span></li></ul></div>');
                    var $thirdDiv = $('<div class="col-xs-3"><ul class="list-unstyled listTwoPlan">' +
                        '<li><span class="pull-left">重要度 :</span></li>' +
                        '<li><span class="pull-left">难易度 :</span></li>' +
                        '<li><span class="pull-left">紧急度 :</span></li></ul></div>');
                    $thirdDiv.children().children("li:eq(0)").append(startPlanDraw(v.importance, "startImportant"));
                    $thirdDiv.children().children("li:eq(1)").append(startPlanDraw(v.difficulty, "startDifficult"));
                    $thirdDiv.children().children("li:eq(2)").append(startPlanDraw(v.urgency, "startUrgent"));

                    var $forthDiv = $(' <div class="col-xs-3 pull-right relativePos text-right"  ><span class="createTimeSpan">创建于 : ' + v.createTime.replace('T', ' ').substr(0, 16) + '</span></div>');
                    if (v.planType == 0) {
                        $forthDiv.append($('<span class="circleMark glyphicon glyphicon-time"></span>'));
                    } else if (v.planType == 2) {
                        $forthDiv.append($('<span class="circleMark circulation"></span>'));
                    }
                    $container.append([$firstDiv, $secondDiv, $thirdDiv, $forthDiv]);
                    var $operation = $('<ul class="list-inline operate"></ul>');
                    var $liCommit = $("<li class='liCommit'>提交</li>"),
                        $liCommitFile = $("<li class='liCommit'>提交</li>"),
                        $giveOther = $("<li class='liForward'>转办</li>"),
                        $liDetail = $("<li class='liDetail'>详情</li>"),
                        $liSplit = $("<li class='liSplit'>分解</li>"),
                        $liModify = $("<li class='liModify'>修改</li>"),
                        $liStop = $("<li class='liStop'>中止</li>"),
                        $liCheck = $("<li class='liCheck'>审核</li>"),
                        $liConfirm = $("<li class='liConfirm'>确认</li>");

                    //提交
                    $liCommit.click(function () {
                        ncUnits.confirm({
                            title: "提交", html: "确定要提交吗?", yes: function (id) {
                                if (v.isLoopPlan == 1) {   //循环计划
                                    submitoperate(v.loopId, v.initial, v.isLoopPlan);
                                } else {
                                    submitoperate(v.planId, v.initial);
                                }
                                layer.close(id);
                            }, no: function (id) { layer.close(id) }
                        });

                    });
                    //转办
                    $giveOther.click(function () {
                        $("#layer_Transmitplan").attr("data_planId", (v.planId == null ? v.loopId : v.planId)).attr("parentTime", v.endTime);
                        $("#layer_Transmitplan").modal("show");
                    });
                    //详情
                    $liDetail.click(function () {
                        current_Info = v;
                        if (v.isLoopPlan == 0) {
                            $('#xxc_planId').val(v.planId);
                        } else {
                            $('#xxc_planId').val(v.loopId);
                        }
                        $("#plan_detail_modal").modal("show");
                    });
                    //分解
                    $liSplit.click(function () {
                        $("#planSplit_modal").attr('term', v.planId);
                        $("#planSplit_modal").modal("show");

                    });
                    //修改
                    $liModify.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要申请修改吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                if (v.isLoopPlan == 1) {   //循环计划
                                    $.ajax({
                                        type: "post",
                                        url: "/Plan/UpdateLoopPlan",
                                        dataType: "json",
                                        data: { "loopId": v.loopId },
                                        success: rsHandler(function (data) {
                                            if (data) {
                                                ncUnits.alert("申请修改成功！");
                                                loadingPlanList();
                                            } else {
                                                ncUnits.alert("申请修改失败！");
                                            }
                                        })
                                    })
                                } else {
                                    $.ajax({
                                        type: "post",
                                        url: "/Plan/ChangePlanStatus",
                                        dataType: "json",
                                        data: { "planId": v.planId, "status": 25, flag: 1 },
                                        success: rsHandler(function (data) {
                                            if (data == "ok") {
                                                ncUnits.alert("申请修改成功！");
                                                loadingPlanList();
                                            } else {
                                                ncUnits.alert("申请修改失败！");
                                            }
                                        })
                                    })
                                }
                            }
                        })
                    });
                    $liStop.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要中止计划吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                if (v.isLoopPlan == 1) {   //循环计划
                                    $.ajax({
                                        type: "post",
                                        url: "/Plan/StopLoopPlan",
                                        dataType: "json",
                                        data: { "loopId": v.loopId },
                                        success: rsHandler(function (data) {
                                            if (data) {
                                                ncUnits.alert("循环计划中止成功！");
                                                loadingPlanList();
                                            } else {
                                                ncUnits.alert("循环计划中止失败！");
                                            }
                                        })
                                    })
                                } else {
                                    $.ajax({
                                        type: "post",
                                        url: "/Plan/StopOrStartPlan",
                                        dataType: "json",
                                        data: { "planId": v.planId, "stop": 10 },
                                        success: rsHandler(function (data) {
                                            if (data == "ok") {
                                                ncUnits.alert("申请中止成功！");
                                                loadingPlanList();
                                            } else {
                                                ncUnits.alert("申请中止失败！");
                                            }
                                        })
                                    })
                                }

                            }
                        })
                    });
                    //已审核提交操作
                    $liCommitFile.click(function () {
                        current_Info = v;
                        if (v.isLoopPlan == 0) {
                            $('#xxc_planId').val(v.planId);
                        } else {
                            $('#xxc_planId').val(v.loopId);
                        }
                        $("#plan_detail_modal").modal("show");
                        submitconfirm();
                    });

                    //审核
                    $liCheck.click(function () {
                        current_Info = v;
                        if (v.isLoopPlan == 0) {
                            $('#xxc_planId').val(v.planId);
                        } else {
                            $('#xxc_planId').val(v.loopId);
                        }
                        $("#plan_detail_modal").modal("show");
                        checkclick();
                    });

                    //确认
                    $liConfirm.click(function () {
                        current_Info = v;
                        if (v.isLoopPlan == 0) {
                            $('#xxc_planId').val(v.planId);
                        } else {
                            $('#xxc_planId').val(v.loopId);
                        }
                        $("#plan_detail_modal").modal("show");
                        DetailConfirm();
                    });

                    if (listArgus.calendarType == 1) {
                        if (v.isLoopPlan == 1) {      //循环计划
                            if (v.status == 0) {    //待提交状态
                                $operation.append([$liCommit, $liDetail]);
                            } else if (v.status == 20) {   //审核通过
                                $operation.append([$liModify, $liStop, $liCommitFile, $liDetail]);
                            }
                        } else {
                            if (v.IsCollPlan == 1) {  //协作计划
                                $operation.append($liDetail);
                            } else {
                                if ((v.status == 0 || v.status == 15) && v.stop == 0) {            //1、待提交的计划：转办、提交和详情。
                                    $operation.append([$giveOther, $liCommit, $liDetail]);
                                } else if ((v.status == 20 || v.status == 40) && v.stop == 0) {       //  2、已审核的计划：分解、转办、修改、中止、提交、详情。
                                    $operation.append([$liSplit, $giveOther, $liModify, $liStop, $liCommitFile, $liDetail]);
                                }
                            }
                        }

                    } else if (listArgus.calendarType == 2) {      //下属日程
                        if (v.isLoopPlan == 1) {   //循环计划
                            if (v.status == 10 || v.status == 25) {    //待审核
                                $operation.append([$liCheck, $liDetail]);
                            } else if (v.status == 20) {   //待确认
                                $operation.append([$liConfirm, $liDetail]);
                            }
                        } else {
                            if (((v.status == 10 || v.status == 25) && v.stop == 0) || v.stop == 10) {    //待审核
                                $operation.append([$giveOther, $liCheck, $liDetail]);
                            } else if ((v.status == 30 && v.stop == 0) || (v.isLoopPlan == 1 && v.status == 20)) {   //待确认
                                $operation.append([$giveOther, $liConfirm, $liDetail]);
                            }
                        }
                    }
                    $planContent.append($container.append($operation));

                })
            }

        }),
        complete: function () {
            lodi.remove();       //关闭load层
        }
    })
}

//绘制五角星进度条
function startDraw(num, className) {
    var $ul = $('<ul class="list-inline start"></ul>');
    if (!num || num == 0) {
        for (var i = 0 ; i < 5; i++) {
            var $li = $("<li></li>");
            $ul.addClass("needStartNoChosen").append($li);
        }
    }
    else {
        for (var i = 0 ; i < num; i++) {
            var $li = $("<li class=" + className + "></li>");
            $ul.append($li);
        }
    }
    return $ul;
}

//计划模块绘制五角星进度条
function startPlanDraw(num, className) {
    var $ul = $('<ul class="list-inline start"></ul>');
    if (!num || num == 0) {
        for (var i = 0 ; i < 5; i++) {
            var $li = $("<li class='planNoColorStart'></li>");
            $ul.append($li);
        }
    }
    else {
        for (var i = 0 ; i < num; i++) {
            var $li = $("<li class=" + className + "></li>");
            $ul.append($li);
        }
        if (num<5) {
            for (var j = 0; j < (5 - num) ;j++){
                var $li = $("<li class='planNoColorStart'></li>");
                $ul.append($li);
            }
        }
    }
    return $ul;
}

//提交确认
function submitconfirm() {
    $("#detail_operateinfo").load("/plan/GetSubmitConfirmView", { height: 640 }, function () {
        $('#detail_operateinfo').show();
    });
}

//审核
function checkclick(check_flag) {
    if (check_flag) {    //批量审核
        $("#detail_operateinfo").load("/plan/GetCheckView", { height: 595 }, function () {
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
        $("#detail_operateinfo").load("/plan/GetCheckView", { height: 640 }, function () {
            $('#detail_operateinfo').show();
        });
    }


}

//提交
function submitoperate(planIdNew, initial, isloop) {
    if (isloop) {   //提交循环计划
        $.ajax({
            url: "/Plan/SubmitLoopPlan",
            type: "post",
            dataType: "json",
            data: { "loopId": planIdNew },
            success: rsHandler(function (data) {
                if (data) {
                    ncUnits.alert("计划已成功提交！");
                    loadingPlanList();
                } else {
                    ncUnits.alert("计划提交失败！");
                }
            })
        });
    } else {
        $.ajax({
            url: "/Plan/SubmitPlan",
            type: "post",
            dataType: "json",
            data: { "planId": planIdNew, "initial": initial ? 1 : initial },
            success: rsHandler(function (data) {
                if (data) {
                    ncUnits.alert("计划已成功提交！");
                    loadingPlanList();
                } else {
                    ncUnits.alert("计划提交失败！");
                }
            })
        });
    }

}

//确认
function DetailConfirm() {
    $("#detail_operateinfo").load("/plan/GetConfirmView", { height: 640 }, function () {
        $('#detail_operateinfo').show();
    });
}

function getplandetail(planId, isloop) {
    $.ajax({
        type: "post",
        url: "/CalendarProcess/GetPlanDetail",
        dataType: "json",
        data: { planId: planId, isloop: isloop },
        success: rsHandler(function (data) {
            if (data) {
                $("#details_department_v").text(data.organizationName);
                $("#details_project_v").text(data.projectName);
                $("#detail_runmode option[value=" + data.executionModeId + "]").attr("selected", true);
                $("#detail_eventoutput").val(data.eventOutput);
                $("#detail_endtime").val(data.endTime.replace('T', ' ').substr(0, 16));
                $("#detail_responsibleUser").val(data.responsibleUserName);
                $("#detail_confirmUser").val(data.confirmUserName);
                $("#detail_initial option[value=" + data.initial + "]").attr("selected", "true");
            }
        })
    });
    //计划日志
    $.ajax({
        type: "post",
        url: "/plan/GetPlanOperates",
        dataType: "json",
        data: {
            planId: planId, isloop: isloop == true ? 1 : 0
        },
        success: rsHandler(function (data) {
            var $ul = $("<ul class='list-unstyled'></ul>");
            for (var i = 0, len = data.length; i < len; i++) {
                var info = data[i];
                var $li = $("<li></li>");
                var $span1 = $("<span style='width: 50%;max-width:420px' class='textOver' title=" + info.message + "></span>");
                var $span2 = $("<span style='width: 50%;text-align: -webkit-right;float:right;'></span>");
                $li.append([$span1, $span2]).appendTo($ul);

                var str = "<span style='color:#58b456;'>" + info.user + "</span>";
                if (info.type == 1) {
                    str += "提交了该计划";
                } else if (info.type == 2) {
                    str += ("审核通过了该计划" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 3) {
                    str += ("审核未通过该计划" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 4) {
                    str += ("撤销了该计划的提交" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 5) {
                    str += ("撤销了该计划的审核" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 6) {
                    str += ("发表了评论" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 7) {
                    str += ("下载了附件" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 8) {
                    str += ("查看了你的计划" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 9) {
                    str += ("转办" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 10) {
                    str += ("申请修改该计划" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 11) {
                    str += ("申请中止该计划" + (info.message ? (":" + info.message) : ""));
                } else if (info.type == 12) {
                    str += ("重新开始该计划" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 13) {
                    str += ("删除该计划" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 14) {
                    str += ("该计划确认通过" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 15) {
                    str += ("该计划确认未通过" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 16) {
                    str += ("该计划更新了进度" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 17) {
                    str += ("该计划进行了分解计划" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 18) {
                    str += ("新建了计划" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 19) {
                    str += ("新建了循环计划" + (info.message ? (":" + info.message) : ""));
                }
                else if (info.type == 20) {
                    str += ("修改保存了该计划" + (info.message ? (":" + info.message) : ""));
                } else {
                    str = "异常操作"
                }
                $span1.html(str);
                $span2.html(info.time.replace('T', ' ').substr(0, 16));
            }
            $("#plan_detail_modal .listView").html($ul);
        })
    });
}