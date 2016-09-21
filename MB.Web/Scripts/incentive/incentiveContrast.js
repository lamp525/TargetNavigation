//@ sourceURL=incentiveContrast.js
/**
 * Created by DELL on 15-8-14.
 */
$(function(){
    var contrastType=null,type=1;
    var  targetId = [];
    var tagetName = [];
    $("#year-year,#startTime-month,#endTime-month,#startTime-date,#endTime-date").focus(function(){//
        var  id="#"+$(this).attr("id");
        if(id=="#year-year"){
            //format="YYYY";
            $("#time-btn-group button[term='1']").trigger("click");
        }
         else if(id=="#endTime-month"||id=='#startTime-month'){
             //format="YYYY-MM";
            $("#time-btn-group button[term='2']").trigger("click");
         }
        else if(id=="#startTime-date"||id=="#endTime-date"){
            //format="YYYY-MM-DD";
            $("#time-btn-group button[term='3']").trigger("click");
        }
        
    })
    //时间插件
    
    laydate({
        elem: "#year-year",
        format: "YYYY",
        max: '2099-06-16', //最大日id期
        istime: false,
        istoday: false
    })

    var monthStart = {
        elem: "#startTime-month",
        format: "YYYY-MM",
        max: '2099-06-16', //最大日id期
        istime: false,
        istoday: false,
        choose: function (datas) {
            monthEnd.min = datas; //开始日选好后，重置结束日的最小日期
            monthEnd.start = datas //将结束日的初始值设定为开始日
        },
        clear: function () {
            monthEnd.min = undefined;
        }
    };
    var monthEnd = {
        elem: "#endTime-month",
        format: "YYYY-MM",
        max: '2099-06-16',
        istime: false,
        istoday: false,
        choose: function (datas) {
            monthStart.max = datas; //结束日选好后，重置开始日的最大日期
        },
        clear: function () {
            monthStart.max = undefined;
        }
    };
    laydate(monthStart);
    laydate(monthEnd);

    var $contrastWeek = $("#contrastWeek"),
        $contrastWeekMonth = $("#contrastWeekMonth"),
        $contrastWeekYear = $("#contrastWeekYear");
    $contrastWeekYear.click(function () {
        WdatePicker({
            dateFmt: 'yyyy',
            ychanged: function () {
                $contrastWeek.empty();

                var year = this.value,
                    month = $contrastWeekMonth.val();

                var weekCount = getWeekCount(year, month - 1);

                for (var i = 1; i <= weekCount; i++) {
                    $contrastWeek.append("<option>" + i + "</option>")
                }
            }
        });
    }).val(new Date().getFullYear());
    //    on("input", function () {
    //    var year = this.value,
    //        month = $contrastWeekMonth.val();

    //    var weekCount = getWeekCount(year, month - 1);

    //    for (var i = 1; i <= weekCount; i++) {
    //        $contrastWeek.append("<option>" + i + "</option>")
    //    }
    //    console.log(weekCount);
    //}).val(new Date().getFullYear());
    
    $contrastWeekMonth.change(function () {
        $contrastWeek.empty();

        var month = this.value,
            year = $contrastWeekYear.val();

        var weekCount = getWeekCount(year, month - 1);

        for (var i = 1; i <= weekCount; i++) {
            $contrastWeek.append("<option>" + i + "</option>")
        }
    }).change();

    //var dateStart = {
    //    elem: "#startTime-date",
    //    format: "YYYY-MM-DD",
    //    max: '2099-06-16', //最大日id期
    //    istime: false,
    //    istoday: false,
    //    choose: function (datas) {
    //        dateEnd.min = datas; //开始日选好后，重置结束日的最小日期
    //        dateEnd.start = datas //将结束日的初始值设定为开始日
    //    },
    //    clear: function () {
    //        dateEnd.min = undefined;
    //    }
    //};
    //var dateEnd = {
    //    elem: "#endTime-date",
    //    format: "YYYY-MM-DD",
    //    max: '2099-06-16',
    //    istime: false,
    //    istoday: false,
    //    choose: function (datas) {
    //        dateStart.max = datas; //结束日选好后，重置开始日的最大日期
    //    },
    //    clear: function () {
    //        dateStart.max = undefined;
    //    }
    //};
    //laydate(dateStart);
    //laydate(dateEnd);

     $("#year-year,#startTime-month,#endTime-month,#startTime-date,#endTime-date").siblings("a").click(function(){
         $(this).siblings("input").click();
     });
    $("#contrastCondition .btn-group button").click(function(){
        $(this).addClass("green_color");
        $(this).siblings("button").removeClass("green_color");
    })
    $(".sub-nav .nav-item").click(function(){
        $(this).addClass("green_color");
        $(this).siblings(".nav-item").removeClass("green_color");
        type=$(this).attr("term");
        TableAndChart();
    })
    //折线图

    $("#time-btn-group button").click(function(){
        contrastType=$(this).attr("term");
        if($(this).attr("term")==1){
            $(".contrastCondition-year").css("display","inline-block");
            $(".contrastCondition-month,.contrastCondition-week").css("display","none");
        }
        else if($(this).attr("term")==2){
            $(".contrastCondition-month").css("display","inline-block");
            $(".contrastCondition-year,.contrastCondition-week").css("display","none");
        }
        else if($(this).attr("term")==3){
            $(".contrastCondition-week").css("display","inline-block");
            $(".contrastCondition-month,.contrastCondition-year").css("display","none");
        }
    });


    $("#contrastCondition-sure").click(function(){
        TableAndChart();
    });
    function TableAndChart(){
        var argus={
            type:null,             //1:计划执行力 2:流程执行力 3:功效价值 4:目标价值
            contrastType:null,      //1：年 2:月 3:日
            startTime:"",     //年/月/开始时间
            endTime: "",       //结束时间
            week:[],            //年月周
            userId:[],         //人员ID
            orgId:[]           //部门ID
        }
        argus.contrastType=contrastType;
        argus.type=type;
        if(argus.contrastType==1){
            if($("#year-year").val()==""){
                ncUnits.alert("请输入年份");
                return;
            }
            argus.startTime=$("#year-year").val();
        }
        else if(argus.contrastType==2){
            if ($("#startTime-month").val() == "" || $("#endTime-month").val() == "") {
             
                ncUnits.alert("请输入开始时间和结束时间");
                return;
            }
          
            argus.startTime=$("#startTime-month").val();
            argus.endTime=$("#endTime-month").val();
        }
        else if (argus.contrastType == 3) { 
            //if ($("#startTime-date").val() == "" || $("#endTime-date").val() == "") {
            //    ncUnits.alert("请输入开始时间和结束时间");
            //    return;
            //}
            //if ($("#endTime-date").val() > $("#startTime-date").val()) {
            //    ncUnits.alert("开始时间不能小于结束时间");
            //    return;
            //}
            //argus.startTime=$("#startTime-date").val();
            //argus.endTime=$("#endTime-date").val();
            argus.week = [$("#contrastWeekYear").val(), $("#contrastWeekMonth").val(), $("#contrastWeek").val()]
        }
        else {
            ncUnits.alert("请选择并输入时间");
            return;
        }

        if(targetId.length==0){
            ncUnits.alert("请选择部门或岗位");
            return;
        }
        else if($(".department-ul").find(".depart_choose")){
            argus.userId = targetId.slice(0);
        }
        else if($(".HR_choose").find(".depart_choose")){
          
            argus.orgId = targetId.slice(0);
        }
        DrawCharts(argus);
    }
    function loadTable(data){
        $("#chartTab").trigger("click");
        $("#table-main  tbody").empty();
        $.each(data,function(i,v){
          var $tr=$("<tr></tr>");
          var $td = $(" <td style='width:15%'>" + v.name + "</td><td style='width:15%'>" + v.planCount + "</td><td style='width:15%'>" + v.timeoutCount + "</td>" +
              "<td style='width:15%'>" + v.timeoutRate + "</td><td style='width:15%'>" + v.completeCount + "</td><td style='width:15%'>" + v.comcompleteRate + "</td><td style='width:5%'></td>");
          $tr.append($td);
          $("#table-main  tbody").append($tr);
      })
    }
    //绘制图表
    function DrawCharts(argus){
        var xData=[], count=[],completeCount=[],completeRate=[], timeoutCount =[],timeoutRate=[],legendData =[],titleText;
        $.ajax({
            type: "post",
            url: "/IncentiveContarst/GetRewardPunishNum",
            dataType: "json",
            data:{data:JSON.stringify(argus)  },
            success: rsHandler(function (data) {
                $.each(data,function(i,v){
                    xData.push(v.name);
                    count.push(v.planCount);
                    completeCount.push(v.completeCount);
                    completeRate.push(v.completeRate);
                    timeoutCount.push(v.timeoutCount);
                    timeoutRate.push(v.timeoutRate);
//                        name:$int,                    //月份或日期
//                        count:$int,                   //总数        实际工时
//                        completeCount:$int,           //完成数      有效工时
//                        timeoutCount:$int,            //超时数
//                        completeRate:$decimal,        //完成率      效率系数
//                        timeoutRate:$decimal          //超时率
                });
                loadTable(data);
                if(argus.contrastType==1){//年
                    titleText = argus.startTime+"年";
                }
                else   if(argus.contrastType==2){//月
                    titleText = argus.startTime.split("-")[0]+"年"+argus.startTime.split("-")[1]+"月-"+argus.endTime.split("-")[0]+"年"+argus.endTime.split("-")[1]+"月";
                }
                else   if(argus.contrastType==3){//日
                    titleText = argus.startTime+"年"
                }

                if( argus.type==4 ){         //目标价值
                    legendData = ["完成数","目标数","完成率"];
                    titleText = titleText+"目标价值";
                }else if( argus.type==3  ){           //功效价值
                    legendData = ["有效工时","实际工时","效率系数"];
                    titleText = titleText+"功效价值";
                }else if( argus.type==2  ){      //流程执行力
                    legendData = ["超时数","超时率"];
                    titleText = titleText+"流程执行力";
                }else if( argus.type==1  ){        //计划执行力
                    legendData = ["超时数","超时率"];
                    titleText = titleText+"计划执行力";
                }
                if( argus.type==1 || argus.type==2){
                    var series = [
                        {
                            "name":legendData[0],
                            "type":"bar",
                            "data":timeoutCount,
                            itemStyle: {normal: {color:"#56b456"}},
                            barWidth:15
                        },{
                            "name":legendData[1],
                            "type":"line",
                            yAxisIndex:1,
                            "data":timeoutRate,
                            itemStyle: {normal: {color:"#0166a7"}}
                        }
                    ];
                }else {
                    var series = [
                        {
                            "name":legendData[0],
                            "type":"bar",
                            "data":completeCount,
                            itemStyle: {normal: {color:"#56b456"}},
                            barWidth:15
                        },  {
                            "name":legendData[1],
                            "type":"bar",
                            "data":count,
                            xAxisIndex:1,
                            itemStyle: {normal: {color:"#b6b6b6"}},
                            barWidth:15
                        },{
                            "name":legendData[2],
                            "type":"line",
                            yAxisIndex:1,
                            "data":completeRate,
                            itemStyle: {normal: {color:"#0166a7"}}
                        }
                    ];
                }
                loadTap(xData,legendData,titleText,series);
            })
        })
    }

    function loadTap(xData,legendData,titleText, seriesData){
        var myChart = echarts.init(document.getElementById('main'));
        var option = {
            grid:{
                borderWidth:0,
                width:550,
                x:40
            },
            title:{
                text:titleText,
                textStyle:{
                    fontSize: 18,
                    fontWeight: 'bolder',
                    color: '#333'
                },
                x:"center"
            },
            tooltip: {
                show: true
            },
            legend: {
                data:legendData,
                y:"bottom"
            },
            xAxis : [
                {
                    type : 'category',
                    data : xData,
                    splitLine: {show: false},
                    axisLine: false,
                    axisTick:false
                },
                {
                    type : 'category',
                    axisLine: {show:false},
                    axisTick: {show:false},
                    axisLabel: {show:false},
                    splitArea: {show:false},
                    splitLine: {show:false},
                    data : xData
                }
            ],
            yAxis : [
                {
                    type : 'value',
                    name:'objectNum',
                    axisLabel:{
                        textStyle:{ color:"#56b456"}
                    },
                    splitLine:{
                        lineStyle:{ color: ['#bbbbbb'], width: 1,type: 'dotted'}
                    },
                    position:'left',
                    axisLine: false
                },{
                    type : 'value',
                    axisLabel:{
                        formatter:'{value} %',
                        textStyle:{ color:"#0166a7" }
                    },
                    name:'objectPercent',
                    splitLine:{show:false},
                    position:'right',
                    axisLine: false
                }
            ]
        };
        // 为echarts对象加载数据
        myChart.setOption(option);
        myChart.setSeries(seriesData);
    }



    //
    /*弹窗 开始*/
    //组织架构

    var department_modal;
    $("#department-add").click(function () {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null },
            success: rsHandler(function (data) {
                department_modal = $.fn.zTree.init($("#department_modal_folder"), $.extend({
                    callback: {
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
                                });
                                node.mappingLi = $checked;

                            } else {
                                node.mappingLi.remove();
                            }
                        },
                        onNodeCreated: function (e, id, node) {
                            if ($("#department_modal_chosen li").length > 0) {
                                $("#department_modal_chosen li").each(function () {
                                    var departId = $(this).attr('term');
                                    if (parseInt(departId) == node.id) {
                                        $(this).remove();
                                        department_modal.checkNode(node, undefined, undefined, true);
                                    }
                                });
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
        //确定组织架构选择
        $("#department_modal_submit").click(function () {
            targetId.length=0,tagetName.length=0;
            $('#department_modal_chosen li').each(function () {
                var name = $(this).find("span:eq(0)").text();
                 targetId.push($(this).attr('term'));
                 tagetName.push(name);
            });
            $(".depart_choose").remove();
            $(".HR_choose").remove();
            for(var i=0;i<targetId.length;i++){
                var $depart_choose = $("<li class='depart_choose' term='" + targetId[i] + "'><span>" + tagetName[i] + "</span></li>");
                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $(".department-ul").append($depart_choose.append($close));
                $close.click(function () {
                    $(this).parent().remove();
                    var id = $(this).parent().attr("term");
                    for (var j = 0; j < targetId.length;j++) {
                        if (targetId[j] == id) {
                            targetId.splice(j, 1);
                            tagetName.splice(j, 1);
                        }
                    }
                });

            }

            $('#department_modal').modal('hide');
        });
        //取消组织架构选择
        $('#department_modal_cancel').click(function () {
            $("#department_modal_chosen_count").text(0);
            $("#department_modal_chosen").remove();
        });
      //  $("#department_modal .modal-content").load("/Shared/GetDepartmentHtml", function () {                });
    });
    /*人力资源 开始*/
    var personOrgId;
    var personWithSub = false;
    $("#HR-add").click(function () {
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
                    var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
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
            targetId.length=0,tagetName.length=0;
            $(".depart_choose").remove();
            $(".HR_choose").remove();
            $("#HR_modal_chosen li").each(function () {
                var name = $(this).find("span:eq(0)").text();
                targetId.push($(this).attr('term'));
                tagetName.push($(this).find('span:eq(0)').text());
                var $HR_choose = $("<li class='HR_choose' term='" + $(this).attr('term') + "'><span>" + name + "</span></li>");
                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $(".department-ul").append($HR_choose.append($close));
                $close.click(function () {
                    $(this).parent().remove();
                    var id = $(this).parent().attr("term");
                    var name = $(this).parent().find('span:eq(0)').text();
                    for (var i = 0; i < targetId.length; i++) {
                        if (targetId[i] == id) {
                            targetId.splice(i, 1);
                        }
                    }
                    for (var i = 0; i < tagetName.length; i++) {
                        if (tagetName[i] == name) {
                            tagetName.splice(i, 1);
                        }
                    }
                });
            });
            $("#HR_select").val('');
            $('#HR_modal').modal('hide');
        });
        //取消
        $("#HR_cancel").click(function () {
            $("#HR_select").val('');
            $("#person-haschildren").prop("checked", false);
            $("#HR_modal_chosen_count").text(0);
            $(".person_list input[type=checkbox]").prop("checked", false);
            $("#HR_modal_chosen li").remove();
        });
    //    $("#HR_modal .modal-content").load("/Shared/GetPersonHtml", function () { });
    });
    /*人力资源 结束*/

    /*弹窗  $("#chart_Table h4 .nav-item").click(function(){
     ncUnits.alert("click");
     if($(this).attr("id")=="TableTab"){
     $("#con_chartTab").css("display","none");
     $("#con_TableTab").css("display","block");
     }
     else if(($(this).attr("id")=="chartTab")){
     $("#con_chartTab").css("display","block");
     $("#con_TableTab").css("display","none");

     }
     })结束*/

});

