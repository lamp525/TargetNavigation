var enddate;
var startdate;
$(function () {

    var d = new Date()
        , endTime = laydate.now(-d.getDate(), "YYYY-MM-DD")
        , startTime = laydate.now((d.setMonth(d.getMonth() - 1, 1) - Date.now()) / 86400000, "YYYY-MM-DD")

        , sortby = 0
        , sortDirect = 1
        , staffSortby = 0
        , staffSortDirect = 1
        , statisticsId
        , departmentId

        , layer_index_staff;

    $("#start").val(startTime);
    $("#end").val(endTime);
    $("#start").blur(function () {
        setTimeout(function () {
            startTime = $("#start").val();
            console.log(startTime);
        }, 100);
    });
    $("#end").blur(function () {
        setTimeout(function () {
            endTime = $("#end").val();
            console.log(endTime);
        }, 100);
    });
    $("#checkST").click(function () {
        renderDepartmentCompare(1);
    });
    $("#selectST").change(function () {
        statisticsId = $(this).val();
        renderDepartmentCompare(1);
    });
    $.ajax({
        type: "post",
        url: "/PlanStatistics/GetStatisticsList",    //TODO 有无统计
        dataType: "json",
        success: rsHandler(function (data) {
            if (data && data.length) {
                $(".mainCon .contain").show();
                var $select = $("#selectST");
                for (var i = 0, len = data.length; i < len; i++) {
                    $select.append("<option " + (i ? "" : "selected") + " value='" + data[i].statisticsId + "'>" + data[i].statisticsName + "</option>")
                }
                $("#selectST").change();
            } else {
                $(".mainCon .addSta").show();
            }
        })
    });

    /* main高度 开始 */
    // main最小高度=浏览器时下窗口可视区域高度 -100
    //$(window).resize(function () {
    $('.main').css({ 'min-height': $(window).height() - 100 });
    //}).resize();
    function fnMainHeight() {
        var num = (parseInt($('.chartDiv').length) - 1) / 4;
        $('.chartAll').css({ 'height': (num + 1) * 305 + 10 });
        if (parseInt($('.main').css('height')) > ($(window).height() - 100)) {
            $('.main').css({ 'margin-bottom': '30px' });
        }
    }
    fnMainHeight();
    /* main高度 结束 */


    /* 自定义统计表 开始 */
    $('.addBtn').on('click', function () {

        var layer_index = $.layer({
            type: 1,
            shade: [0],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.statisTab' },
            closeBtn: false
        });
        fnPopUpHeight($('.statisTab'));
        // 加载部门分类
        var setting = {
            view: {
                showIcon: false,
                showLine: false,
                selectedMulti: false
            }
            , callback: {
                onClick: function (e, id, node) {
                    console.log(1);
                }
                , beforeCheck: function (id, node) {
                    if (node.checked) {
                        var zTree = $.fn.zTree.getZTreeObj("departmentTree");
                        zTree.setChkDisabled(node, false, undefined, true);
                    }
                }
                , onCheck: function (e, id, node) {
                    if (node.checked) {
                        var zTree = $.fn.zTree.getZTreeObj("departmentTree");
                        zTree.setChkDisabled(node, true, undefined, true);
                        zTree.setChkDisabled(node, false);
                    }
                }
            }
            , check: {
                enable: true
                , chkboxType: { "Y": "s", "N": "s" }
            }
        };

        $.ajax({
            type: "post",
            url: "/PlanStatistics/GetOrganizationInfo",
            dataType: "json",
            success: rsHandler(function (data) {
                $.fn.zTree.init($("#departmentTree"), setting, data);
            })
        });

        $(".statisTab .closeWCom").click(function () {
            $("#st_name").val('');
            layer.close(layer_index);
            layer.closeTips();
        })

        $('.statisTab .canCon span').off("click");
        $('.statisTab .canCon span:eq(0)').click(function () {
            $("#st_name").val('');
            layer.close(layer_index);
        });
        $('.statisTab .canCon span:eq(1)').click(function () {
            if ($("#st_name").val() == "") {
                validate_reject('统计名不能为空', $("#st_name"));
                return;
            }
            $('.mainCon .addSta').hide();
            $('.mainCon .contain').show();
            var zTree = $.fn.zTree.getZTreeObj("departmentTree")
                , nodes = zTree.getCheckedNodes()
                , ids = []
                , name = $("#st_name").val();
            for (var i = 0, len = nodes.length; i < len ; i++) {
                ids.push(nodes[i].id);
            }
            $.ajax({
                type: "post",
                url: "/PlanStatistics/AddStatisticsInfo",     //TODO 新建统计
                dataType: "json",
                data: {
                    param: JSON.stringify({ statisticsName: name, organizationId: ids })
                },
                success: rsHandler(function (data) {
                    $("#selectST").append("<option selected value='" + data.statisticsId + "'>" + name + "</option>")
                    statisticsId = data.statisticsId;
                    renderDepartmentCompare(1);
                    ncUnits.alert("统计添加成功");
                    $("#st_name").val('');
                    layer.close(layer_index);
                })
            });
        });
    });


    $(".departmentCompare .arrows>span").click(function () {
        $(".departmentCompare .arrows>span.hit").removeClass("hit");
        $(this).addClass("hit");

        if ($(this).hasClass("arrowSolidTCom")) {
            sortDirect = 0;
        } else {
            sortDirect = 1;
        }

        if ($(this).parent().parent().hasClass("planNum")) {
            sortby = 0;
        } else {
            sortby = 1;
        }

        renderDepartmentCompare(1);
    });

    $(".staffCompare .arrows>span").click(function () {
        $(".staffCompare .arrows>span.hit").removeClass("hit");
        $(this).addClass("hit");

        if ($(this).hasClass("arrowSolidTCom")) {
            staffSortDirect = 0;
        } else {
            staffSortDirect = 1;
        }

        if ($(this).parent().parent().hasClass("planNum")) {
            staffSortby = 0;
        } else if ($(this).parent().parent().hasClass("accomplishNum")) {
            staffSortby = 1;
        } else {
            staffSortby = 2;
        }

        renderStaffCompare();
    });

    //获取统计ID渲染内容
    function renderDepartmentCompare(obj) {
        if (obj == 1) {
            enddate = $("#end").val();
            startdate = $("#start").val();
        }
        if (layer_index_staff) {
            layer.close(layer_index_staff);
        }

        $.ajax({
            type: "post",
            url: "/PlanStatistics/GetStatusByOrg",       //TODO 获取统计内容
            dataType: "json",
            data: {
                statisticsId: statisticsId
                , startTime: startdate
                , endTime: enddate
                , sortby: sortby
                , sortDirect: sortDirect
            },
            success: rsHandler(function (data) {
                var $con = $("#dountDiv");
                $con.empty();
                for (var i = 0, len = data.length; i < len; i++) {

                    var department = data[i]
                        , count = 0
                        , dountData = [];

                    for (var j = 0, plen = department.plans.length; j < plen; j++) {
                        var plan = department.plans[j];
                        if (plan.count!=0) {
                            count += plan.count;
                            dountData.push([plan.count, plan.color, plan.status, plan.statusName]);
                        }
                    }

                    var $chart = $("<div class='canvas'></div>");
                    var $name = $("<span class='chartText'>" + department.name + "</span>");

                    $name.click(department.id, function (e) {
                        $(".chartText.focus").removeClass("focus");
                        var offset = $(this).position();
                        departmentId = e.data;
                        renderStaffCompare(offset);
                        $(this).addClass("focus");
                    });

                    $("<div class='chartDiv'></div>").append([$("<div class='chartSingle'></div>").append($("<div class='chart'></div>").append([$chart, "<div class='planNum interior'>计划<span style='color:#58b456'>" + count + "</span>项</div>"])), $name]).appendTo($con);
                    Raphael($chart.get(0), 250, 250).dountChart(125, 125, 50, 65, 100, dountData);
                    fnMainHeight();
                }
            })
        });
    }

    function renderStaffCompare(offset) {
        enddate = $("#end").val();
        startdate = $("#start").val();
        if (layer_index_staff) {
            layer.close(layer_index_staff);
        }
        layer_index_staff = $.layer($.extend({
            type: 1,
            shade: [0],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.chartDetail' },
            fix: false,
            close:function(){
            $(".chartText.focus").removeClass("focus");}
        }, (offset ? { offset: [offset.top - $(document).scrollTop() + 30 + "px", "50px"] } : {})));

        console.log(layer_index_staff);
        $.ajax({
            type: "post",
            url: "/PlanStatistics/GetPlanStatistics",       //TODO 获取部门内成员数据
            dataType: "json",
            data: {
                organizationId: departmentId
                , startTime: startdate
                , endTime: enddate
                , sortby: staffSortby
                , sort: staffSortDirect
            },
            success: rsHandler(function (data) {
                var con = $(".CDAll");
                con.empty();
                for (var k = 0, ulen = data.length; k < ulen; k++) {
                    var staff = data[k]
                        , str = '<div class="CDDiv">' +
                            '<div class="portraitBox">' +
                            '<div class="portrait">';
                    var middel;
                    if (staff.image) {
                        middel = '<img style="height: 64px;width: 64px;margin: 3px;border-radius: 50%" src="/HeadImage/' + staff.image + '" />';
                    }
                    else {
                        middel = '<img style="height: 64px;width: 64px;margin: 3px;border-radius: 50%" src="../../Images/common/portrait.png" />';
                    }
                    str += middel;
                            
                    str+=   '</div>' +
                            '<span class="name">' + staff.name + '</span>' +
                            '</div>' +
                            '<div class="CDText">' +
                            '<ul class="CDTextNum">' +
                            '<li>事项总数：<span>' + staff.eventTotalCount + '</span>件</li>' +
                            '<li>完成件数：<span>' + staff.completeCount + '</span>件</li>' +
                            '<li>待确认件数：<span>' + staff.unConfirm + '</span>件</li>' +
                            '<li>未完成件数：<span>' + staff.unCompleteCount + '</span>件</li>' +
                            '<li>审核件数：<span>' + staff.submitCount + '</span>件</li>' +
                            '<li>待提交件数：<span>' + staff.unCommittedCount + '</span>件</li>' +
                            '<li>已中止件数：<span>' + staff.stopCount + '</span>件</li>' +
                            '</ul>' +
                            '<div class="CDTextR">' +
                            '<div class="accomRate" style="color:' + (parseInt(staff.completeRate) < 90 ? '#58b456' : '#eb2d2c') + '">' +
                            '完成率<span>' + staff.completeRate + '</span>%' +
                            '</div>' +
                            '<ul class="CDTextRate">' +
                            '<li>确认率：<span>' + staff.confirmRate + '</span>%</li>' +
                            '<li>未完成率：<span>' + staff.unCompleteRate + '</span>%</li>' +
                            '<li>审核率：<span>' + staff.submitRate + '</span>%</li>' +
                            '<li>提交率：<span>' + staff.committedRate + '</span>%</li>' +
                            '<li>中止率：<span>' + staff.stopRate + '</span>%</li>' +
                            '</ul>' +
                            '</div>' +
                            '</div>' +
                            '</div>';

                    con.append(str);
                }
                $(".chartDetail .CDHeaL span").html(data.length);
                fnChartDetailHeight();
            })
        })
    }

    /* 自定义统计表 结束 */

    /* 加载时间 开始 */
    var start = {
        elem: '#start',
        format: 'YYYY-MM-DD',
        start: startTime,
        //min: laydate.now(), //设定最小日期为当前日期
        //max: '2099-06-16 23:59:59', //最大日期
        //istime: true,
        istoday: false,
        choose: function (datas) {
            console.log("end.start:" + end.start);
            end.min = datas; //开始日选好后，重置结束日的最小日期
            end.start = datas;//将结束日的初始值设定为开始日
        },
        clear: function () {
            endTime_v = undefined;
            start.max = undefined;
        }
    };

    var end = {
        elem: '#end',
        format: 'YYYY-MM-DD',
        start: endTime,
        //min: laydate.now(),
        //max: '2099-06-16 23:59:59',
        //istime: true,
        istoday: false,
        choose: function (datas) {

            start.max = datas; //结束日选好后，重置开始日的最大日期
        },
        clear: function () {
            endTime_v = undefined;
            start.max = undefined;
        }
    };



    laydate(start);
    laydate(end);
    /* 加载时间 结束 */

    /* 统计图详情 开始 */
    //$('.chartText').on('click', function(){
    //    $.layer({
    //        type: 1,
    //        shade: [0],
    //        offset:[$(this).offset().top + "px",$(this).offset().left + "px"],
    //        area: ['auto', 'auto'],
    //        title: false,
    //        border: [0],
    //        page: {dom : '.chartDetail'}
    //    });
    //    fnPopUpHeight ($('.chartDetail'));
    //});
    function fnChartDetailHeight() {
        var num = Math.ceil($('.CDDiv').length / 3);
        $('.CDAll').css({ 'height': num * 170 });
    }

    /* 统计图详情 结束 */


});