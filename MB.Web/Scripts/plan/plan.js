//@ sourceURL=plan.js
// 请求 筛选已选条件
var SCData = {
    status: [],		// 状态
    stop: [],
    time: [],		// 时间 $type:时间条件类型,int值（1：本周，2：本月，0：自选时间(从$starttime至$endtime)）
    person: [],		// 人员
    department: [],		// 部门分类
    project: [],		// 项目分类
    whoPlan: [],		// 谁的计划： 0-我的计划 1-下属计划
    sorts: [{
        type: 8,	// 排序类型（0:组织架构 1:项目分类 2：重要度 3：紧急度 4：责任人 5：确认人 6：计划提交时间 7：状态 8：默认）
        direct: 0	// 升/降序（0：降 1：升）
    }]
};
// 请求 筛选已选条件（经确定后生成的函数）
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
var list_lodi;  //加载局部视图的对象
var responsibleUser_v, responsibleUser_name;          //责任人
var confirmUser_v, confirmUser_name;             //确认人
var page_flag = "plan";    //页面标志
var pageIndex = 1;    //当前页码
var planIds = [];  //存储批量选择的计划
var planInfoChooseds = [];
//var soontype = [];  //快捷方式：1、今日未完成 2、超时计划

$(function () {
    //fnNav(3);
    //弹层列表（待隐藏）
    //var $popups = $(null);

    var operate = parseInt($('#xxc_operateId').val());
    //首次加载计划列表
    var operateName = "";
    if (operate == 2) {

        SCConData.status.push(0);
        SCConData.status.push(15);
        SCConData.stop.push(0);
        fnChoose("待提交", '0', '15', '0', "状态");
        operateName = "待提交";
    } else if (operate == 3) {
        SCConData.status.push(10);
        SCConData.status.push(25);
        SCConData.stop.push(10);
        fnChoose("待审核", '10', '25', '10', "状态");
        operateName = "待审核";
    }
    else if (operate == 4) {
        SCConData.status.push(20);
        SCConData.status.push(40);
        fnChoose("已审核", '20', '40', '-1', "状态");
        operateName = "已审核";
    }
    else if (operate == 5) {
        SCConData.status.push(30);
        fnChoose("待确认", '30', '-1', '-1', "状态");
        operateName = "待确认";
    }
    else if (operate == 6) {
        SCConData.status.push(90);
        fnChoose("已完成", '90', '-1', '-1', "状态");
        operateName = "已完成";
    }
    else if (operate == 7) {
        SCConData.stop.push(90);
        fnChoose("已中止", '-1', '-1', '90', "状态");
        operateName = "已中止";
    }
    else if (operate == 8) {
        SCConData.soontype.push(1);
        fnChooseShortcut("今日未完成", '1', "我的快捷方式");
        operateName = "今日未完成";
    }
    else if (operate == 9) {
        SCConData.soontype.push(2);
        fnChooseShortcut("超时计划", '2', "我的快捷方式");
        operateName = "超时计划";
    }
    else if (operate == 10) {
        fnChoose("近一月", "2", '-1', '-1', '时间');
        SCConData.time.push("2");
        pageIndex = 1;
        operateName = "近一月";
        $('.mySubPlan ul li').hide();
        SCConData.whoPlan = -1;
    }
    if (operate != 1) {
        $('.filterBox').show();
        $(".filterBox .state span").each(function () {
            if ($(this).text() == operateName) {
                $(this).css({ 'color': '#58b456' }).addClass('chosen');
            }
        });
        $(".specific .shortcut span").each(function () {
            if ($(this).text() == operateName) {
                $(this).css({ 'color': '#58b456' }).addClass('chosen');
            }
        });
        $(".specific .time span").each(function () {
            if ($(this).text() == operateName) {
                $(this).css({ 'color': '#58b456' }).addClass('chosen');
            }
        });
    }
    
    //右侧个人信息
    loadPersonalInfo();

    /* 圆饼 开始 */

    var date = new Date()
        , year = date.getFullYear()
        , month = date.getMonth() + 1
        , $con = $(".process")
        , colors = com.ztnc.targetnavigation.unit.planStatusColor;

    function drawPlanProgress() {
        $("#chart3", $con).empty();
        var lodi = getLoadingPosition('.chart');//显示load层
        $.ajax({
            type: "post",
            url: "/Plan/GetStatusCountByTime",
            dataType: "json",
            data: {
                year: year,
                month: month,
                operate: operate
            },
            success: rsHandler(function (data) {
                $("#chart3", $con).empty();
                //alert();
                var dountData = []
                    , count = 0
                    , $ul = $("<ul></ul>");

                for (var i = 0, len = data.length; i < len; i++) {
                    var color = colors[i];
                    var num = data[i].statusCount;
                    if (num != 0) {
                        count += num;
                        dountData.push([num, color, data[i].status, data[i].statusName+"计划"]);
                    }
                    $ul.append('<li><span class="color" style="background-color:' + color + '"></span><span class="text">' + data[i].statusName + '</span></li>');
                }
                $(".sign", $con).html($ul);
                Raphael("chart3", 330, 310).dountChart(165, 155, 55, 70, 110, dountData, function (data) {
                    emptysoonfind();
                    //拼上时间
                    var day;
                    if (month==1||month==3||month==5||month==7||month==8||month==10||month==12) {
                        day = 31;
                    }
                    else if (month==4||month==6||month==9||month==11) {
                        day = 30;
                    }
                    else if (month=2) {
                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && year % 100 == 0))//闰年
                        {
                            day = 29;
                        }
                        else {
                            day = 28;
                        }
                    }
                    var timeStart = year + '-' + month + '-01';
                    var timeEnd = year + '-' + month + '-' + day;
                    var timeQuan = timeStart + '至' + timeEnd;
                    SCConData.time = [];
                    $('.conditionDiv ul span').each(function () {
                        if ($(this).attr('classify') == "时间") {
                            $(this).parent().remove();
                            removetimeclass();
                        }
                        if ($(this).attr('classify') == "状态") {
                            $(this).parents('li').remove();
                        }
                    });
                    $('.time li:eq(3)').find('span:eq(0)').css({ 'color': '#58b456' }).addClass('chosen');
                    $('.time li:eq(3)').find('span:eq(2)').css({ 'color': '#58b456' }).addClass('chosen');
                    $('.time li:eq(3)').find('span:eq(3)').css({ 'color': '#58b456' }).addClass('chosen');
                    fnChoose(timeQuan, '0', '-1', '-1', '时间');
                    //SCConData.time.push([ [0,$(this).parent().find('.laydate-icon:eq(0)').html(),$(this).parent().find('.laydate-icon:eq(1)').html()] ]);
                    SCConData.time.push(0);
                    SCConData.time.push(timeStart);
                    SCConData.time.push(timeEnd);
                    //拼上状态
                    var stopflag;
                    var status1;
                    var status2;
                    SCConData.status = [];
                    $('.specific .state .chosen').removeClass('chosen').css({ 'color': '#686868' });
                    if (data[1] == '#57acdb') {
                        SCConData.status.push(0);
                        status1 = 0;
                        SCConData.status.push(15);
                        status2=15
                        SCConData.stop.push(0);
                        stopflag = 0;
                    }
                    else if (data[1] == '#e00e16') {
                        SCConData.status.push(10);
                        status1 = 10;
                        SCConData.status.push(25);
                        status2 = 25
                        SCConData.stop.push(10);
                        stopflag = 10;
                    }
                    else if (data[1] == '#be1d9a') {
                        SCConData.status.push(20);
                        status1 = 20;
                        SCConData.status.push(40);
                        status2 = 40;
                        SCConData.stop.push(0);
                        stopflag = 0;
                    }
                    else if (data[1] == '#49dca7') {
                        status1 = -1;
                        status2 = -1;
                        SCConData.stop.push(90);
                        stopflag = 90;
                    }
                    else {
                        SCConData.status.push(data[2]);
                        status1 = data[2];
                        status2 = -1;
                        SCConData.stop.push(0);
                        stopflag = 0;
                    }
                    fnChoose(data[3], status1, status2, stopflag, "状态");
                    operateName = data[3];
                    $('.filterBox').show();
                    $(".filterBox .state span").each(function () {
                        if ($(this).text() == operateName) {
                            $(this).css({ 'color': '#58b456' }).addClass('chosen');
                        }
                    });
                    pageIndex = 1;
                    $('.conL .bottom').hide();
                    $("#xxc_planList").html('');
                   fnScreCon();
                    //TODO 饼图click事件
                });
                $(".planNum span", $con).html(count);
                $(".month .text", $con).html(year + "年" + month + "月");
            }),
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            })
        });
    }

    drawPlanProgress();
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

    //var s1 = [['a',58], ['b',20], ['c',8], ['d',66], ['e',35],['f',20]];
    //var plot3 = $.jqplot('chart3', [s1], {
    //	seriesColors: [ "#57acdb", "#e00e16", "#be1d9a", "#fbab11", "#58b557","#49dca7"],  // 默认显示的分类颜色，
    //		//如果分类的数量超过这里的颜色数量，则从该队列中第一个位置开始重新取值赋给相应的分类
    //	seriesDefaults: {
    //		// make this a donut chart.
    //		renderer:$.jqplot.DonutRenderer,
    //		rendererOptions:{
    //			diameter: undefined, // 设置饼的直径
    //			padding: 20, // 饼距离其分类名称框或者图表边框的距离，变相该表饼的直径
    //			// 圆环的间隔.
    //			sliceMargin: 0,
    //			// 圆环转的角度
    //			startAngle: -90,
    //			showDataLabels: true,
    //			// By default, data labels show the percentage of the donut/pie.
    //			// You can show the data 'value' or data 'label' instead.
    //			dataLabels: 'value'
    //		},
    //		// 饼图阴影
    //		shadowAngle: 90
    //	},
    //	grid: {
    //		background: '#f9f9f9',
    //		borderWidth:0.0,
    //		shadow:false
    //	}
    //});
    //var planAmount = 0;
    //for ( var num in planProData.name ) {
    //	if ( num!='shuffle' ) {
    //		$('.process .sign ul li:eq('+num+') span:eq(1)').html(planProData.name[num]);
    //		planAmount+=parseInt(planProData.amount[num]);
    //	}
    //}
    //$('.planNum span:eq(0)').html(planAmount);
    /* 圆饼 结束 */
    var planCreate = addPlan();
    $('.addPlan').click(function () {
        planCreate.addPlan();
    });
    fnScreCon();

    // 获取 我的快捷内容
    //var mineData = {
    //	id:				[],
    //	name:			['小妞啊'],		// 名字
    //	post:			['前端开发'],		// 岗位
    //	portrait:		['url(../../Images/common/portrait.png) no-repeat'],		// 头像
    //	impoPlan:		['1'],		// 重要计划
    //	overTimePlan:	['2'],		// 超时计划
    //	process:		['3'],		// 我的流程
    //	incentive:		['4']		// 我的激励
    //};
    // 请求 饼图：按月份获取各状态的计划数量
    var monthData = {
        month: []	// 月份 1-12
    };
    // 获取 饼图：按月份获取各状态的计划数量
    //var planProData = {
    //	num: 	[],	// 状态号（0：待提交 1：待审核 2：已审核 3：待确认 4：已中止 5：已完成）
    //	amount:	['1','5','6','4','6','5'],	// 该状态下的计划数
    //	name:	['待提交','待审核','已审核','待确认','已中止','已完成']	// 状态名
    //};
    /* 数据交互 结束 */


    /* 我的计划+下属计划 开始 */
    $('.mySubPlan ul li').click(function () {
        if ($('span', this).hasClass('mySubPlanNot')) {
            $(this).parents('ul').find('span').removeClass('mySubPlanHit').addClass('mySubPlanNot');
            $('span', this).removeClass('mySubPlanNot').addClass('mySubPlanHit');
        }
    });
    /* 我的计划+下属计划 结束 */

    // 排序
    $('.headL span').click(function () {
        $('.headL span').removeClass('spanHit');
        //$(this).addClass('spanHit');
        if ($(this).index() < 5) {// 表示不包括自定义排序
            $('.headL span').removeClass('spanHit');
            $(this).addClass('spanHit');
        }
    });

    $(".showMode").click(function () {
        $(".showMode.hit").removeClass("hit");
        $(this).addClass("hit");

        if ($(this).hasClass("chunk")) {
            $(".planList").removeClass("listMode");
        } else {
            $(".planList").addClass("listMode");
        }
    })

    /* 评分五角星 开始 */
    $('.stars ul li').hover(function () {
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
    $('.stars ul li').click(function () {
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








    /* 筛选 开始 */
    //function fnAjaxPerClear() {
    //    // 筛选里的人员数据清空
    //    $('.personnel li').each(function (index, element) {
    //        if (($(this).index() >= 1) && ($(this).index() <= 5)) {
    //            $(this).find('span:eq(0)').attr({ 'term': '' });
    //            $(this).find('span:eq(0)').html('');
    //        }
    //    });
    //}
    ///* 筛选里的人员数据导入 开始*/
    //function fnAjaxPerData(){
    //    $.ajax({
    //        type: "post",
    //        url: "../../testSL/data/Plan/plan_perData.json",
    //        dataType: "json",
    //        data:{
    //            //count:5
    //        },
    //        success:rsHandler(function(data){
    //			$('.personnel li').each(function(index, element) {
    //                if ( ($(this).index()>=1) && ($(this).index()<=5) ) {
    //					$(this).find('span:eq(0)').attr({'term':data[$(this).index()-1][0]});
    //					$(this).find('span:eq(0)').html(data[$(this).index()-1][1]);
    //				}
    //            });
    //        })
    //    });
    //}
    ///* 筛选里的人员数据导入 结束*/



    /* 筛选出现、消失 开始 */
    $('.screen').click(function () {
        $('.headL .moreCancel').hide()
        $('.moreBg').hide();
        $('.xxc_choose').removeClass('choose').removeClass('prohibit');
        if ($('.filterBox').css('display') == 'none') {
            $(this).css({ 'color': '#58b456' });
            $('.filterBox').show();
            //fnAjaxPerData ();
        }
        else {
            $(this).css({ 'color': '#686868' });
            $('.filterBox').hide();
            //fnAjaxPerClear();
        }
    });
    /* 筛选出现、消失 结束 */

    /* 更多选项 开始 */
    function fnAjaxSectProClear() {
        // 筛选里的部门、项目数据清空
        $('.sectPro ul').remove();
    }
    /* 筛选里的部门数据导入 开始*/
    function fnAjaxSectionData(text) {
        var lodi = getLoadingPosition('.sectProMain');
        $.ajax({
            type: "post",
            url: "/Plan/GetOrganizationInfo",
            dataType: "json",
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                fnsectProAdd(data, 0, text);
            })
        });
    }
    /* 筛选里的部门数据导入 结束*/
    /* 筛选里的项目数据导入 开始*/
    function fnAjaxProjectData(text) {
        var lodi = getLoadingPosition('.sectProMain');
        $.ajax({
            type: "post",
            url: "/plan/GetProjectInfo",
            dataType: "json",
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                fnsectProAdd(data, 1, text);
            })
        });

    }
    /* 筛选里的项目数据导入 结束*/
    /* 筛选里的部门项目结构导入 开始*/
    function fnaddUl(thisData, nums) {
        for (var num in thisData) {
            if (num != 'shuffle') {
                if (num != 0) {
                    var spanTerm = $('.sectPro ul li input[type="checkbox"]').next();
                    var spanLength = spanTerm.length;
                    for (var i = 0; i < spanLength; i++) {
                        if (parseInt(spanTerm.eq(i).attr('term')) == thisData[num - 1].id) {
                            $('.nowSpan').removeClass('nowSpan');
                            spanTerm.eq(i).parent().parent().prev('span').addClass('nowSpan');
                            break;
                        }
                    }
                }
                $('.nowSpan').after(
				'<ul style="display:none;">' +
				'<li><span class="arrowSolidRCom"></span>' +
				'<input type="checkbox" />' +
				'<span class="nowSpan" term="' +
				thisData[num].id +
				'">' +
				thisData[num].name +
				'</span>' +
				'</li></ul>').removeClass('nowSpan');
                if (thisData[num].children) {
                    fnaddUl(thisData[num].children, nums);
                }
            }
        }
    }
    function fnsectProAdd(datas, num, text) {
        $('.nowSpan').removeClass('nowSpan');
        for (var nums in datas) {
            if (nums != 'shuffle') {
                $('.sectPro:eq(' + num + ')').append(
				'<ul class="firUl" style="display:none;">' +
				'<li><span class="arrowSolidRCom"></span>' +
				'<input type="checkbox" />' +
				'<span class="nowSpan" term="' +
				datas[nums].id +
				'">' +
				datas[nums].name +
				'</span>' +
				'</li></ul>');
                if (datas[nums].children) {
                    fnaddUl(datas[nums].children, nums);
                    $('.nowSpan').removeClass('nowSpan');
                }
            }
        }
        if (text == '部门项目加载结束') {
            //alert(1)
            // 重新加载一次部门和项目的箭头展开，不然操作不成功
            fnSPMArrowsBB();
            //重新加载一次部门和项目的勾选，不然操作不成功
            fnSectProC();
            //重新加载一次关闭X，不然操作不成功
            fnCloseWC();
        }
    }
    /* 筛选里的部门项目结构导入 结束*/
    $('.moreOpt div:eq(0)').click(function () {
        $(this).hide();
        $('.moreOpt div:eq(1)').show();
        $('.sectProMain').show();
        // 加载部门分类
        fnAjaxSectionData('部门项目加载中');
        // 加载项目分类
        fnAjaxProjectData('部门项目加载结束');
    });
    $('.moreOpt div:eq(1)').click(function () {
        $(this).hide();
        $('.moreOpt div:eq(0)').show();
        $('.sectProMain').hide();
        $('.sectProMain .SPMArrowsBB').removeClass('SPMArrowsTB');
        $('.sectProMain .arrowSolidRCom').removeClass('arrowSolidBCom');
        $('.sectProMain ul').hide();
        // 清空部门、项目分类
        fnAjaxSectProClear();
    });
    /* 更多选项 结束 */

    /* 筛选清空筛选条件 开始 */
    $('.empty').click(function () {
        if ($('.conditionDiv ul li').length<=0) {
            return;
        }
        emptydata();
        pageIndex = 1;
        $('.conL .bottom').hide();
        $("#xxc_planList").html('');
        fnScreCon();
    });
    function emptydata() {
        $('.conditionDiv ul li').remove();
        SCConData.status = [];
        SCConData.stop = [];
        SCConData.time = [];
        SCConData.person = [];
        SCConData.department = [];
        SCConData.project = [];
        SCConData.soontype = [];
        $('.chosen').css({ 'color': '#686868' }).removeClass('chosen');
        $('.sectPro ul li input[type="checkbox"]').prop("checked", false).removeAttr('disabled');
    }
    /* 筛选清空筛选条件 结束 */

    /* 筛选的确定 开始 */
    // 筛选内容的数据分组
    function fnScreConCom(clones, beClones) {
        clones = [];
        if (beClones.length > 0) {
            for (var i = 0; i < beClones.length; i++) {
                clones.push(beClones[i]);
            }
        }
        return clones;
    }


    $("#xxc_confirm").click(function () {
        //var count = 1;
        //if (stop.length>0) {
        //    for (var i=0; i < stop.length;i++)
        //    {
        //        if (stop[i] == 90) {
        //            count++;
        //            SCData.status.push(90);
        //        }
        //    } 
        //};
        //if (count == 1) {
        //    SCData.status.push(-1);
        //}
        //SCConData.status = fnScreConCom(SCConData.status, SCData.status);
        //SCConData.time = fnScreConCom(SCConData.time, SCData.time);
        //SCConData.person = fnScreConCom(SCConData.person, SCData.person);
        //SCConData.department = fnScreConCom(SCConData.department, SCData.department);
        //SCConData.project = fnScreConCom(SCConData.project, SCData.project);
        //fnScreCon();
    });
    /* 筛选的确定 结束 */

    //点击下属计划
    $(".conLMain .mySubPlan li").click(function () {
        SCConData.whoPlan = $(this).find("span").attr("term");
        if (SCConData.whoPlan == "0") {
            $("#xxc_examine").css('display', 'none');
            $("#xxc_examine").next('li').css('display', 'none');
            $('#xxc_del').css('display', 'block');
            $("#xxc_submit").css('display', 'block');
            $("#xxc_submit").next('li').css('display', 'block');
        }
        else {
            $("#xxc_examine").css('display', 'block');
            $("#xxc_examine").next('li').css('display', 'block');
            $("#xxc_submit").css('display', 'none');
            $("#xxc_submit").next('li').css('display', 'none');
            $('#xxc_del').css('display', 'none');
        }
        pageIndex = 1;
        $('.conL .bottom').hide();
        $("#xxc_planList").html('');
        fnScreCon();
    });

    ///*自定义排序点击事件开始*/
    //$("#xxc_sortsure").click(function () {
    //    fnScreCon();
    //});
    /*自定义排序点击事件结束*/

    /* 筛选的取消确定效果 开始 */
    $('.filterBox .handle').hover(function () {
        $(this).addClass('handleHover');
    }, function () {
        $(this).removeClass('handleHover');
    });
    $('.filterBox .selected .handle').click(function () {
        $(this).parent().find('.handle').removeClass('handleHit');
        $(this).addClass('handleHit');
    });
    /* 筛选的取消确定效果 结束 */

    /* 筛选人员的人员名称 开始 */
    //$("#detail_responsibleUser").val(data.responsibleUserName).attr('term', data.responsibleUser);
    //确认人选择
    $("#select_confirmUser").searchStaffPopup({
        url: "/Plan/GetOfferUsers",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            if (SCConData.person.length > 0) {
                for (var j = 0; j < SCConData.person.length; j++) {
                    if (SCConData.person[j] == data.id) {
                        return;
                    }
                }
            }
            fnChoose(data.name, data.id, '-1', '-1', '人员');
            SCConData.person.push(data.id);
            $('.filterBox .personnel li span').each(function () {
                for (var i = 0; i < SCConData.person.length; i++) {
                    if ($(this).attr('term') == SCConData.person[i]) {
                        fnSTP(this);
                    }
                }
            });
            pageindex = 1;
            $('.conL .bottom').hide();
            $("#xxc_planList").html('');
            fnScreCon();
            $(this).val(data.name);
        }
    });

    $('.filterBox .specific .perSec').focus(function () {
        if ($(this).val() == '人员名称' || $(this).val() == '') {
            $(this).val('');
            $(this).css({ 'color': '#686868' });
        } else {
            $('.perSecDiv').show();
        }
    }).blur(function () {
        if ($(this).val() == '') {
            $(this).val('人员名称');
            $(this).css({ 'color': '#686868' });
            $('.perSecDiv').css("display", "none");
        }

    });
    $('.filterBox .specific .perSec').keyup(function () {
        var filed = $(this).val();
        if (filed != "" && filed != "人员名称") {
            $.ajax({
                type: "post",
                url: "/plan/GetOfferUsers",
                dataType: "json",
                data: {
                    text: filed
                },
                success: rsHandler(function (data) {
                    $(".perSecMain .perSecDiv ul").html('');
                    for (var i = 0, len = data.length; i < len ; i++) {
                        if (SCConData.person.length > 0) {
                            for (var j = 0; j < SCConData.person.length; j++) {
                                if (SCConData.person[j] == data[i].id) {
                                    data.splice(i, 1);
                                }
                            }
                        }
                        if (data.length > 0) {
                            $(".perSecMain .perSecDiv").width('200');
                            $(".perSecMain .perSecDiv ul").append("<li><span term='" + data[i].id + "'>" + data[i].name + "</span></li>");
                            $('.perSecDiv').show();
                            $('.filterBox .personnel li span').click(function () {
                                if (!$(this).hasClass('chosen')) {
                                    fnSTP(this);
                                    fnChoose($(this).html(), $(this).attr('term'), '-1', '-1', '人员');
                                    SCConData.person.push(parseInt($(this).attr('term')));
                                    $('.filterBox .personnel li span').each(function () {
                                        for (var i = 0; i < SCConData.person.length; i++) {
                                            if ($(this).attr('term') == SCConData.person[i]) {
                                                fnSTP(this);
                                            }
                                        }

                                    });
                                    $('.perSecDiv').hide();
                                    pageIndex = 1;
                                    $('.conL .bottom').hide();
                                    $("#xxc_planList").html('');
                                    fnScreCon();
                                }
                            });
                        }
                        else {
                            $(".perSecMain .perSecDiv").css('width', '0');
                        }

                    }
                })
            });
        }
    });



    /* 部门，项目分类箭头 开始 */
    $('.sectProMain .SPMArrowsBB').click(function () {
        if ($(this).hasClass('SPMArrowsTB')) {
            $(this).removeClass('SPMArrowsTB');
            $(this).parent().parent().find('.arrowSolidRCom').removeClass('arrowSolidBCom');
            $(this).parent().parent().find('ul').hide();
        }
        else {
            $(this).addClass('SPMArrowsTB');
            $(this).parent().parent().find('.sectPro .firUl').show();
        }
    });
    function fnSPMArrowsBB() {
        $('.sectPro ul li .arrowSolidRCom').click(function () {
            if ($(this).hasClass('arrowSolidBCom')) {
                $(this).removeClass('arrowSolidBCom');
                $(this).siblings('ul').hide();
            }
            else {
                $(this).parent().parent().find('.arrowSolidRCom').removeClass('arrowSolidBCom');
                $(this).parent().parent().find('ul').hide();
                $(this).addClass('arrowSolidBCom');
                $(this).siblings('ul').show();
            }
        });
    }
    /* 部门，项目分类箭头 结束 */

    /* 下面部分选择 开始 */
    function fnChoose(text, terms, terms2, stop, classifys) {
        var length = $('.filterBox .conditionDiv ul li').length;
        console.log(length)
        if (length == 0) {
            $('.filterBox .conditionDiv ul').append(
				'<li style="margin-left:70px;"><span term="' +
				terms +
				'" term2="' + terms2 + '" stop="' + stop + '" classify="' +
				classifys +
				'">' +
				text +
				'</span><span class="closeW"></span></li>');
        }
        else {
            $('.filterBox .conditionDiv ul').append(
				'<li><span term="' +
				terms +
				'" term2="' + terms2 + '" stop="' + stop + '" classify="' +
				classifys +
				'">' +
				text +
				'</span><span class="closeW"></span></li>');
        }
        //重新加载一次关闭X，不然操作不成功
        fnCloseWC();
    }
    /* 下面部分选择 结束 *
    
    /* 筛选的状态、时间、人员、部门、项目 开始 */
    function fnSTP(thisthis) {
        if ($(thisthis).css('color') != '#58b456') {
            if ($(thisthis).parent().parent().hasClass('time')) {
                if ($(thisthis).hasClass('timeHover')) {
                    $(thisthis).css({ 'color': '#58b456' }).addClass('chosen');
                }
            }
            else {
                $(thisthis).css({ 'color': '#58b456' }).addClass('chosen');
            }
        }
    }

    /* SCConData.status.push 开始 */
    $('.filterBox .state li span').click(function () {
        if (!$(this).hasClass('chosen')) {
            emptysoonfind();
            fnSTP(this);
            fnChoose($(this).html(), $(this).attr('term'), $(this).attr('term2'), $(this).attr('stop'), '状态');
            if ($(this).attr('term')) {
                SCConData.status.push(parseInt($(this).attr('term')));
            }
            if ($(this).attr('term2')) {
                SCConData.status.push(parseInt($(this).attr('term2')));
            }
            SCConData.stop.push(parseInt($(this).attr('stop')));
            pageindex = 1;
            $('.conL .bottom').hide();
            $("#xxc_planList").html('');
            fnScreCon();
        }
    });
    /* SCConData.status.push 结束 */
    /* SCConData.time.push 开始 */
    /* 加载时间 开始 */
    var start = {
        elem: '#start',
        format: 'YYYY-MM-DD',
        //min: laydate.now(), //设定最小日期为当前日期
        //max: '2099-06-16', //最大日期
        istime: false,
        istoday: false,
        choose: function (datas) {
            end.min = datas; //开始日选好后，重置结束日的最小日期
            end.start = datas //将结束日的初始值设定为开始日
        },
        clear: function () {
            endTime_v = undefined;
            start.max = undefined;
        }
    };
    var end = {
        elem: '#end',
        format: 'YYYY-MM-DD',
        //min: laydate.now(),
        //max: '2099-06-16',
        istime: false,
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
    $('.filterBox .time li span').click(function () {
        emptysoonfind();
        if (!$(this).hasClass('chosen')) {
            if ($(this).html() == '近一周') {
                SCConData.time = [];
                $('.conditionDiv ul span').each(function () {
                    if ($(this).attr('classify') == "时间") {
                        $(this).parent().remove();
                        removetimeclass();
                    }

                });
                fnSTP(this);
                fnChoose($(this).html(), $(this).attr('term'), '-1', '-1', '时间');
                SCConData.time.push("1");
                pageIndex = 1;
                $('.conL .bottom').hide();
                $("#xxc_planList").html('');
                $('.conditionDiv ul li:eq(0)').css('margin-left', '70px');
                fnScreCon();
            }

            if ($(this).html() == "近一月") {
                SCConData.time = [];
                $('.conditionDiv ul span').each(function () {
                    if ($(this).attr('classify') == "时间") {
                        $(this).parent().remove();
                        removetimeclass();
                    }
                });

                fnSTP(this);
                fnChoose($(this).html(), $(this).attr('term'), '-1', '-1', '时间');
                SCConData.time.push("2");
                pageIndex = 1;
                $('.conL .bottom').hide();
                $("#xxc_planList").html('');
                $('.conditionDiv ul li:eq(0)').css('margin-left', '70px');
                fnScreCon();
            }
            if ($(this).html() == "确定") {
                var timeQuan = $(this).parent().find('.laydate-icon:eq(0)').html() +
				'至' +
				$(this).parent().find('.laydate-icon:eq(1)').html();

                if (timeQuan != '至') {
                    SCConData.time = [];
                    $('.conditionDiv ul span').each(function () {
                        if ($(this).attr('classify') == "时间") {
                            $(this).parent().remove();
                            removetimeclass();
                        }
                    });

                    $(this).parent().find('span:eq(0)').css({ 'color': '#58b456' }).addClass('chosen');
                    $(this).parent().find('span:eq(2)').css({ 'color': '#58b456' }).addClass('chosen');
                    $(this).parent().find('span:eq(3)').css({ 'color': '#58b456' }).addClass('chosen');
                    fnChoose(timeQuan, $(this).attr('term'), '-1', '-1', '时间');
                    //SCConData.time.push([ [0,$(this).parent().find('.laydate-icon:eq(0)').html(),$(this).parent().find('.laydate-icon:eq(1)').html()] ]);
                    SCConData.time.push(0);
                    SCConData.time.push($(this).parent().find('.laydate-icon:eq(0)').html());
                    SCConData.time.push($(this).parent().find('.laydate-icon:eq(1)').html());
                    pageIndex = 1;
                    $('.conL .bottom').hide();
                    $("#xxc_planList").html('');
                    $('.conditionDiv ul li:eq(0)').css('margin-left', '70px');
                    fnScreCon();
                }
                else {
                    ncUnits.alert("开始时间或者结束时间请选填至少一项");
                }
            }
        }
    });

    //清除时间筛选的样式
    function removetimeclass() {
        $('.specific .time .chosen').removeClass('chosen').css({ 'color': '#686868' });
        $('.specific .time .chosen').removeClass('chosen').css({ 'color': '#686868' });
        $('.specific .time .chosen').find('span:eq(0)').removeClass('chosen').css({ 'color': '#686868' });
        $('.specific .time .chosen').find('span:eq(2)').removeClass('chosen').css({ 'color': '#686868' });
        $('.specific .time .chosen').find('span:eq(3)').removeClass('chosen').css({ 'color': '#686868' });
    }
    /* SCConData.time.push 结束 */
    /* SCConData.person.push 开始 */
    $('.filterBox .personnel li span').click(function () {
        emptysoonfind();
        if (!$(this).hasClass('chosen')) {
            fnSTP(this);
            fnChoose($(this).html(), $(this).attr('term'), '-1', '-1', '人员');
            SCConData.person.push(parseInt($(this).attr('term')));
            $('.perSecDiv').hide();
            pageIndex = 1;
            $('.conL .bottom').hide();
            $("#xxc_planList").html('');
            fnScreCon();
        }
    });
    /* SCConData.person.push 结束 */

    /* 部门、项目选择 结束 */

    /* SCConData.department.push SCConData.project.push 开始 */
    function fnSectPro(thisthis, classify) {
        emptysoonfind();
        $(thisthis).parent().find('input[type="checkbox"]').prop("checked", true).attr({ 'disabled': 'disabled' });
        $(thisthis).parent().find('input[type="checkbox"]').next().css({ 'color': '#58b456' }).addClass('chosen');
        if (classify == '部门') {
            $(thisthis).parent().find('li input[type="checkbox"]').next().each(function (index, element) {
                SCConData.department.push(parseInt($(this).attr('term')));
            });
        }
        if (classify == '项目') {
            $(thisthis).parent().find('li input[type="checkbox"]').next().each(function (index, element) {
                SCConData.project.push(parseInt($(this).attr('term')));
            });
        }
        var i = 0;
        var liLength = $(thisthis).parents('li').length;
        var chooseText = '', text = '';
        $(thisthis).parents('li').each(function (index, element) {
            if (!$(this).hasClass('noInput')) {
                // $(this).find('input[type="checkbox"]:eq(0)').prop("checked", true).attr({ 'disabled': 'disabled' });
                //$(this).find('input[type="checkbox"]:eq(0)').next().css({ 'color': '#58b456' }).addClass('chosen');
                // $(thisthis).parent('li').find('input[type="checkbox"]').prop("checked", true).attr({ 'disabled': 'disabled' });
                //$(thisthis).parent('li').find('input[type="checkbox"]').next().css({ 'color': '#58b456' }).addClass('chosen');
                if (i == 0) {
                    chooseText = $(this).find('input[type="checkbox"]:eq(0)').next().html();
                }
                else {
                    chooseText = $(this).find('input[type="checkbox"]:eq(0)').next().html() + '-' + text;
                }
                i++;
                text = chooseText;
            }
        });
        fnChoose(chooseText, $(thisthis).parent('li').find('input[type="checkbox"]:eq(0)').next().attr('term'), '-1', '-1', classify);
    }
    function fnSectProC() {

        $('.filterBox .sectPro:eq(0) li input[type="checkbox"]').click(function () {
            if (!$(this).parent('li').find('input[type="checkbox"]:eq(0)').next().hasClass('chosen')) {
                fnSectPro(this, '部门');
                SCConData.department.push(parseInt($(this).parent('li').find('input[type="checkbox"]:eq(0)').next().attr('term')));
                pageIndex = 1;
                $('.conL .bottom').hide();
                $("#xxc_planList").html('');
                fnScreCon();
            }
        });
        $('.filterBox .sectPro:eq(1) li input[type="checkbox"]').click(function () {
            if (!$(this).parent('li').find('input[type="checkbox"]:eq(0)').next().hasClass('chosen')) {
                fnSectPro(this, '项目');
                SCConData.project.push(parseInt($(this).parent('li').find('input[type="checkbox"]:eq(0)').next().attr('term')));
                pageIndex = 1;
                $('.conL .bottom').hide();
                $("#xxc_planList").html('');
                fnScreCon();
            }
        });

    }
    /* SCConData.department.push SCConData.project.push 结束 */
    /* 筛选的状态、时间、人员、部门、项目 结束 */

    /* 已选条件的关闭X 开始 */
    function fnCloseWC() {
        $('.conditionDiv ul li .closeW').click(function () {
            var num = $(this).parent().index();
            $(this).parent().remove();
            if (num == 0) {
                $('.conditionDiv ul li:eq(0)').css({ 'margin-left': '70px' });
            }

            //for ( var datas in SCConData ) {
            /* SCConData.status.splice 开始 */
            if ($(this).parent().find('span:eq(0)').attr('classify') == '状态') {
                var staNum = $(this).parent().find('span:eq(0)').attr('term');
                var staNum2 = $(this).parent().find('span:eq(0)').attr('term2');
                var stop = $(this).parent().find('span:eq(0)').attr('stop');
                var stopLength = SCConData.stop.length;
                var staLength = SCConData.status.length;
                for (var i = 0; i < staLength; i++) {
                    if (SCConData.status[i] == staNum) {
                        SCConData.status.splice(i, 1);
                        break;
                    }
                }
                staLength = SCConData.status.length;
                for (var i = 0; i < staLength; i++) {
                    if (SCConData.status[i] == staNum2) {
                        SCConData.status.splice(i, 1);
                        break;
                    }
                }
                for (var i = 0; i < stopLength; i++) {
                    if (SCConData.stop[i] == stop) {
                        SCConData.stop.splice(i, 1);
                        break;
                    }
                }
                var stateLength = $('.state li').length;
                for (var i = 1; i <= stateLength; i++) {
                    if ($('.state li:eq(' + i + ') span:eq(0)').attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                        $('.state li:eq(' + i + ') span:eq(0)').css({ 'color': '#686868' }).removeClass('chosen');
                        break;
                    }
                }
            }

            /* SCConData.status.splice 结束 */
            /* SCConData.time.splice 开始 */
            if ($(this).parent().find('span:eq(0)').attr('classify') == '时间') {
                var timeLength = SCConData.time.length;
                SCConData.time.splice(0, parseInt(timeLength));
                for (var i = 0; i <= 1; i++) {
                    if ($('.time .timeHover:eq(' + i + ')').attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                        $('.time .timeHover:eq(' + i + ')').css({ 'color': '#686868' }).removeClass('chosen');
                        break;
                    }
                }
                if ($('.time .handle:eq(0)').attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                    $('.time .handle:eq(0)').parent().find('span:eq(0)').css({ 'color': '#686868' }).removeClass('chosen');
                    $('.time .handle:eq(0)').parent().find('span:eq(2)').css({ 'color': '#686868' }).removeClass('chosen');
                    $('.time .handle:eq(0)').parent().find('span:eq(3)').css({ 'color': '#686868' }).removeClass('chosen');
                }
            }
            /* SCConData.time.splice 结束 */
            /* SCConData.person.splice 开始 */
            if ($(this).parent().find('span:eq(0)').attr('classify') == '人员') {
                for (var perNum in SCConData.person) {
                    if (SCConData.person[perNum] == $(this).parent().find('span:eq(0)').attr('term')) {
                        SCConData.person.splice(perNum, 1);
                        break;
                    }
                }
                var perLength = $('.personnel li').length;
                for (var i = 1; i <= perLength; i++) {
                    if ($('.personnel li:eq(' + i + ') span:eq(0)').attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                        $('.personnel li:eq(' + i + ') span:eq(0)').css({ 'color': '#686868' }).removeClass('chosen');
                        break;
                    }
                }
                $(".perSecMain .perSecDiv ul li").each(function () {
                    if ($(this).find('span').attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                        $(this).find('span').css({ 'color': '#686868' }).removeClass('chosen');
                    }
                });
            }
            /* SCConData.person.splice 结束 */
            /* SCConData.department.splice 开始 */
            if ($(this).parent().find('span:eq(0)').attr('classify') == '部门') {
                var depLength = $('.sectPro:eq(0) ul li').length;
                for (var i = 0; i < depLength; i++) {
                    if ($('.sectPro:eq(0) ul li:eq(' + i + ')').find('input[type="checkbox"]:eq(0)').next().attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                        $('.sectPro:eq(0) ul li:eq(' + i + ')').find('input[type="checkbox"]').prop("checked", false).removeAttr('disabled');
                        $('.sectPro:eq(0) ul li:eq(' + i + ')').find('input[type="checkbox"]').next().css({ 'color': '#686868' }).removeClass('chosen');
                        $('.sectPro:eq(0) ul li:eq(' + i + ')').find('input[type="checkbox"]').next().each(function (index, element) {
                            // alert(SCConData.department);
                            for (var depNum in SCConData.department) {
                                if (SCConData.department[depNum] == $(this).attr('term')) {
                                    //alert(SCConData.department);
                                    SCConData.department.splice(depNum, 1);
                                }
                            }
                        });
                        $('.sectPro:eq(0) ul li:eq(' + i + ')').parents('li').each(function (index, element) {
                            if (!$(this).hasClass('noInput')) {
                                var deletes = 1;//0：子集中无勾选的项目，则该级可去勾选 1：子集中有勾选的项目，则该级不可去勾选
                                $('li', this).each(function (index, element) {
                                    //alert( $(this).find('input[type="checkbox"]:eq(0)').next().html() );
                                    if ($(this).find('input[type="checkbox"]:eq(0)').next().hasClass('chosen')) {
                                        deletes = 0;
                                    }
                                });
                                if (deletes == 1) {
                                    //alert( $(this).find('input[type="checkbox"]:eq(0)').next().html() );
                                    $(this).find('input[type="checkbox"]:eq(0)').prop("checked", false).removeAttr('disabled');
                                    $(this).find('input[type="checkbox"]:eq(0)').next().css({ 'color': '#686868' }).removeClass('chosen');
                                }
                                //alert();
                            }
                        });
                        break;
                    }
                }
            }
            /* SCConData.department.splice 结束 */
            /* SCConData.project.splice 开始 */
            if ($(this).parent().find('span:eq(0)').attr('classify') == '项目') {
                var proLength = $('.sectPro:eq(1) ul li').length;
                for (var i = 0; i < proLength; i++) {
                    if ($('.sectPro:eq(1) ul li:eq(' + i + ')').find('input[type="checkbox"]:eq(0)').next().attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                        $('.sectPro:eq(1) ul li:eq(' + i + ')').find('input[type="checkbox"]').prop("checked", false).removeAttr('disabled');
                        $('.sectPro:eq(1) ul li:eq(' + i + ')').find('input[type="checkbox"]').next().css({ 'color': '#686868' }).removeClass('chosen');
                        $('.sectPro:eq(1) ul li:eq(' + i + ')').find('input[type="checkbox"]').next().each(function (index, element) {
                            for (var proNum in SCConData.project) {
                                if (SCConData.project[proNum] == $(this).attr('term')) {
                                    //alert(SCConData.project);
                                    SCConData.project.splice(proNum, 1);
                                }
                            }
                        });
                        $('.sectPro:eq(1) ul li:eq(' + i + ')').parents('li').each(function (index, element) {
                            if (!$(this).hasClass('noInput')) {
                                var deletes = 1;//0：子集中无勾选的项目，则该级可去勾选 1：子集中有勾选的项目，则该级不可去勾选
                                $('li', this).each(function (index, element) {
                                    if ($(this).find('input[type="checkbox"]:eq(0)').next().hasClass('chosen')) {
                                        deletes = 0;
                                    }
                                });
                                if (deletes == 1) {
                                    $(this).find('input[type="checkbox"]:eq(0)').prop("checked", false).removeAttr('disabled');
                                    $(this).find('input[type="checkbox"]:eq(0)').next().css({ 'color': '#686868' }).removeClass('chosen');
                                }
                            }
                        });
                        break;
                    }
                }
            }
            /* SCConData.project.splice 结束 */
            //}
            pageIndex = 1;
            $('.conL .bottom').hide();
            $("#xxc_planList").html('');
            fnScreCon();
        });

    }
    fnCloseWC();
    /* 已选条件的关闭X 结束 */
    /* 筛选 结束 */


    /* 我的模块 开始 */


    //$.ajax({
    //    type: "post",
    //    url: "../../test/data/plan/plan_mine.json",
    //    dataType: "json",
    //    success:rsHandler(function(data){
    //        $('.myself .introduce .name').html(data.name);
    //        $('.myself .introduce .post').html(data.post);
    //        $('.myself .portraitBox .portrait').css({'background-image':'url(' + data.portrait + ')'});
    //        $('.planUl .newsM:eq(0)').html(data.impoPlan);
    //        $('.planUl .newsM:eq(1)').html(data.overTimePlan);
    //        $('.planUl .newsM:eq(2)').html(data.process);
    //        $('.planUl .newsM:eq(3)').html(data.incentive);
    //    })
    //});

    /* 我的模块 开始 */



    /* 底部内容 开始 */
    function fnBotRef() {
        $('.conL .bottom').show();
        $('.conL .bottom .refresh').show();
        $('.conL .bottom .botNone').hide();
    }
    function fnBotNone() {
        $('.conL .bottom').show();
        $('.conL .bottom .botNone').show();
        $('.conL .bottom .refresh').hide();
    }
    function fnBot() {
        $('.conL .bottom').hide();
    }
    /* 底部内容 开始 */





    /* 弹窗 开始 */
    /* 自定义排序 开始 */
    // var options=[];//存储所选的选项
    var popUpSort;
    var sortDelete = false;	//判断是否选中某一行
    $('.conLMain .headL .cusSort').on('click', function () {
        popUpSort = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.sort' },
            move: '.title',
            closeBtn: false
        });
        fnPopUpHeight($('.sort'));
        fnSortTabTxtHit();
    });
    $(".sortTabHeader").click(function () {
        $(".sortTabTxt").removeClass('sortTabTxtHit');
        $(".sortTabTxt select").css({ 'background-color': '#ececec' });
    });
    function fnSortTabTxtHit() {
        $(".sortTabTxt td").click(function () {
            if ($(this).index() == 0) {
                $(".sortTabTxt").removeClass('sortTabTxtHit');
                $(this).parent('.sortTabTxt').addClass('sortTabTxtHit');
                $(".sortTabTxt select").css({ 'background-color': '#ececec' });
                $(this).parent('.sortTabTxt').find('select').css({ 'background-color': '#58b456' });
            }
        });
    }
    $(".sort .closeWCom").click(function () {
        $('.sortTab table .sortTabTxt').html('');
        layer.close(popUpSort);

        // options=[];
    })
    $(".sort .canCon span:eq(0)").click(function () {
        layer.close(popUpSort);
    })
    //获取所选择的选择筛选条件

    $('.sort .sortBot span').eq(0).click(function () {//点击+
        if ($('.sortTabTxt').length == 7) {

            //Messenger().post({
            //    message: "筛选条件已满，无法添加新的筛选条件。",
            //    type: "error",
            //    showCloseButton: "true"
            //});
            ncUnits.alert("筛选条件已满，无法添加新的筛选条件。");
            return;
        }
        // options.push($('select.sortTitle:last').val());
        $('.sortTab table').append(
			'<tr class="sortTabTxt">' +
			'<td style="text-align:center;">主要关键字</td><td>' +
			'<select class="sortTitle"><option term="0">组织架构</option><option term="1">项目分类</option><option term="3">紧急度</option>' +
			'<option term="2">重要度</option><option term="4">责任人</option><option term="5">确认人</option>' +
			'<option term="6">计划完成时间</option><option term="7">状态</option></select></td>' +
			'<td><select><option term="0">降序</option><option term="1">升序</option></select></td></tr>'
		);
        // $('.sortTab table tr:last-child').find('.sortTitle option').click(function(){

        // });
        // $.each($('.sortTab table tr:last-child').find('.sortTitle option'),function(ind,value){
        // 	$.each(options,function(index,value){
        // 		if($('.sortTab table tr:last-child').find('.sortTitle option').eq(ind).val()==options[index]){
        // 			var t = $('.sortTab table tr:last-child').find('.sortTitle option').eq(ind).attr('term');
        // 			$('.sortTab table tr:last-child').find('.sortTitle option[term='+t+']').remove();
        // 		}
        // 	});
        // });


        fnSortTabTxtHit();
    });
    $('.sort .sortBot span').eq(1).click(function () {//点击-
        $('.sortTabTxt').each(function (index, element) {
            if ($(this).hasClass('sortTabTxtHit')) {
                sortDelete = true;
                $('.sortTab table .sortTabTxtHit').remove();
            }
        });
        if (sortDelete == false) {
            var length = $('.sortTab table tr').length - 1;
            if (length != 0) {
                $('.sortTab table tr:eq(' + length + ')').remove();
            }
        }
    });

    //自定义排序 点取消
    $('.sort .canCon span:eq(0)').click(function () {
        $('.sortTab table .sortTabTxt').html('');
        layer.close(popUpSort);
    });

    $('#xxc_sortsure').click(function () {//确认排序

        var selectVals = [];
        $('.sortTabTxt').each(function () {
            selectVals.push($(this).find('.sortTitle').val());
        });
        if (isRepeat(selectVals)) {
            ncUnits.alert("筛选条件有重复，请重新筛选。");
            return;
        }

        var USTextOne = '', USTextTwo = '';
        SCConData.sorts = [];
        for (var i = 0; i < $('.sortTabTxt').length; i++) {
            var selects = $('.sortTabTxt:eq(' + i + ') select');
            for (var j = 0; j < selects.length; j++) {
                for (var k = 0; k < selects.find('option').length; k++) {
                    if (selects.eq(j).find('option:eq(' + k + ')').val() == selects.eq(j).val()) {
                        if (j == 0) {
                            USTextOne = selects.eq(0).find('option:eq(' + k + ')').attr('term');
                        }
                        else {
                            USTextTwo = selects.eq(1).find('option:eq(' + k + ')').attr('term');
                        }
                        break;
                    }
                }
            }
            var sorts = { type: USTextOne, direct: USTextTwo };
            SCConData.sorts.push(sorts);
        }
        pageIndex = 1;
        $('.conL .bottom').hide();
        $("#xxc_planList").html('');
        fnScreCon();
        layer.close(popUpSort);//关闭窗口

    });
    function isRepeat(arr) { //判断筛选条件是否有重复

        var hash = {};

        for (var i in arr) {

            if (hash[arr[i]])

                return true;

            hash[arr[i]] = true;

        }

        return false;

    }
    /* 自定义排序 结束 */




    /* 个人资料设置 开始 */
    var popUpPerData;
    $('.conR .mine .introduce').on('click', function () {
        popUpPerData = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.perData' },
            closeBtn: false
        });
        fnPopUpHeight($('.perData'));
        fnSetDataAjax1();
    });
    $(".perData .closeWCom").click(function () {
        layer.close(popUpPerData);
    })
    // $(".perData .canCon").click(function(){
    // 	layer.close(popUpPerData);
    // })
    $('.conR .mine .portraitBox').click(function () {
        // var pn = $("#gotopagenum").val();//#gotopagenum是文本框的id属性  
        //location.href = "/UserHead/Index";//location.href实现客户端页面的跳转 
        loadViewToMain("/UserHead/Index");
    });
    //$('.modify').click(function () {
    //    // var pn = $("#gotopagenum").val();//#gotopagenum是文本框的id属性  
    //    location.href = "/UserHead/Index";//location.href实现客户端页面的跳转 
    //});
    //请求 个人资料设置
    var setData = {
        num: '30',			// 每页条数
        time: '30',		// 消息提示时间
        password: '2',		// 密码
    };
    // 个人资料获取数据

    function fnSetDataAjax1() {
        $.ajax({
            type: "post",
            url: "/User/GetPersonalSetting",
            dataType: "json",
            success: rsHandler(function (data) {
                //setData.password = data.password;
                $('#pageNum input:eq(0)').val(data.pageSize);
                $('#messTime input:eq(0)').val(data.refreshTime);
            })
        });
    }

    // 更新个人设定
    function fnSetDataAjax3() {
        var pagesize = $("#pageNum input:eq(0)").val();
        if (10 <= pagesize && pagesize <= 101) {
            var refreshTime = $("#messTime input:eq(0)").val();
            $.ajax({
                type: "post",
                url: "/User/UpPersonalSetting",
                dataType: "json",
                data: { pagesize: pagesize, refreshTime: refreshTime },
                success: rsHandler(function (data) {
                    //data.pageSize = setData.num;
                    //data.refreshTime = setData.time;
                    data.password = setData.password;
                    ncUnits.alert("更改个人设定成功");
                })
            });
        }
        else { ncUnits.alert("每页条数的范围必须在10~100"); }
    }

    // 更新密码
    function fnSetDataAjax2() {
        var oldpassword = $("#nowPass input:eq(0)").val();
        var password = $("#newPass input:eq(0)").val();
        $.ajax({
            type: "post",
            url: "/User/UpPwd",
            dataType: "json",
            data: { oldpwd: oldpassword, pwd: password },
            success: rsHandler(function (data) {
                console.log(data);
                //data.pageSize = setData.num;
                //data.refreshTime = setData.time;
                data.password = setData.password;
                ncUnits.alert("更改密码成功");
            })
        });
    }

    $('.perData .set .modify').click(function () {
        $(this).next('.cancel').show();
        // 编辑完成状态
        if ($(this).parents('.set').find('.list input:eq(0)').hasClass('inputHit')) {
            if ($(this).parents('.set').hasClass('password')) {
                // 判断当前密码是否为空
                if ($('#nowPass input:eq(0)').val() != '') {
                    // 判断新密码是否为空
                    if ($('#newPass input:eq(0)').val() != '') {
                        // 判断确认密码是否为空
                        if ($('#conPass input:eq(0)').val() != '') {

                            // 判断当前密码是否正确
                            //if (setData.password == $('#nowPass input:eq(0)').val()) {
                            // 判断新密码和确认密码是否一致
                            if ($('#newPass input:eq(0)').val() == $('#conPass input:eq(0)').val()) {
                                setData.password = $('#conPass input:eq(0)').val();
                                console.log(setData.password);
                                // 个人资料返回数据
                                $(this).next('.cancel').hide();
                                fnSetDataAjax2();
                                $('#nowPass input:eq(0)').val('********');
                                $('#newPass input:eq(0)').val('');
                                $('#conPass input:eq(0)').val('');
                                $(this).css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
                                $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
                            } else {
                                validate_reject('新密码和确认密码不一致', $('#conPass'));
                            }
                            //} else {
                            //    validate_reject('当前密码错误', $('#nowPass'));
                            //}

                        } else {
                            validate_reject('确认密码不能为空', $('#conPass'));
                        }
                    } else {
                        validate_reject('新密码不能为空', $('#newPass'));
                    }
                } else {
                    validate_reject('当前密码不能为空', $('#nowPass'));
                }
            } else {
                setData.num = $('#pageNum input:eq(0)').val();
                setData.time = $('#messTime input:eq(0)').val();
                // 个人资料返回数据
                fnSetDataAjax3();
                $(this).css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
                $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
            }
        }
            // 编辑状态
        else {
            if ($(this).parents('.set').hasClass('password')) {
                $(this).parents('.set').find('.list input:eq(0)').val('');
            }
            $(this).css({ 'background': 'url(../../Images/plan/chooseBlue.png) no-repeat' });
            $(this).parents('.set').find('.list input').addClass('inputHit').removeAttr('readonly');
        }
    });
    $('.perData .set .cancel').click(function () {
        $(this).hide();
        $('#nowPass input:eq(0)').val('********');
        $('#newPass input:eq(0)').val('');
        $('#conPass input:eq(0)').val('');
        $(this).prev().css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
        $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
    });
    /* 个人资料设置 结束 */





    //  $(".addPlan").click(addPlan);


    /* 新建计划 结束 */

    /* 我的快捷方式 开始 */
    $('.SCShortcut').click(function () {
        if ($(this).hasClass('chosen')) {
            return;
        }
        //清空筛选数据
        emptydata();
        SCConData.sorts = [{ type: 8, direct: 0 }];
        var term = $(this).attr('term');
        var obj1, obj2;
        if (term == '1') {
            obj1 = $('.shortcut .SCShortcut:eq(0)');
            obj2 = $('.planUl .SCShortcut:eq(0)');
        }
        else if (term == '2') {
            obj1 = $('.shortcut .SCShortcut:eq(1)');
            obj2 = $('.planUl .SCShortcut:eq(1)');
        }
        if (!obj1.hasClass('chosen') && !obj2.hasClass('chosen')) {
            //alert();
            SCConData.soontype.push(term);
            $('.filterBox').show();
            obj1.css({ 'color': '#58b456' }).addClass('chosen');
            obj2.addClass('chosen');
            fnChooseShortcut($(this).html(), $(this).attr('term'), '我的快捷方式');
            kjFindLoadDiv();
        }

    });
    /* 已选条件 开始 */
    function fnChooseShortcut(text, terms, classifys) {
        var length = $('.conditionDiv ul li').length;
        if (length == 0) {
            $('.conditionDiv ul').append(
                '<li style="margin-left:70px;"><span term="' +
                terms +
                '" classify="' +
                classifys +
                '">' +
                text +
                '</span><span class="closeW"></span></li>');
        }
        else {
            $('.conditionDiv ul').append(
                '<li><span term="' +
                terms +
                '" classify="' +
                classifys +
                '">' +
                text +
                '</span><span class="closeW"></span></li>');
        }
        //重新加载一次关闭X，不然操作不成功
        fnCloseShortcut();
    }

    function emptysoonfind() {
        $('.SCShortcut').css({ 'color': '#686868' }).removeClass('chosen');
        SCConData.soontype = [];
        $('.conditionDiv ul li').each(function () {
            var classify = $(this).find('span:eq(0)').attr('classify');
            if (classify == '我的快捷方式') {
                $(this).remove();
            }
        });
    }

    function kjFindLoadDiv() {

        pageIndex = 1;
        $('.conL .bottom').hide();
        $("#xxc_planList").html('');
        var list_lodi = getLoadingPosition('.planList');  //加载局部视图的对象
        $("#xxc_planList").load("/Plan/SelectOrSort", { params: JSON.stringify(SCConData), pageIndex: pageIndex }, function () {
            //回到默认排序
            $('.headL span').eq(0).addClass('spanHit').siblings().removeClass('spanHit');
            list_lodi.remove();
        });
    }
    /* 已选条件 结束 */
    /* 我的快捷的关闭 开始 */
    function fnCloseShortcut() {
        $('.conditionDiv ul li .closeW').click(function () {
            var num = $(this).parent().index();
            $(this).parent().remove();
            if ($(this).parent().find('span:eq(0)').attr('classify') == '我的快捷方式') {
                var kjNum = $(this).parent().find('span:eq(0)').attr('term');
                var kjLnegth = SCConData.soontype.length;
                for (var i = 0; i < kjLnegth; i++) {
                    if (SCConData.soontype[i] == kjNum) {
                        SCConData.soontype.splice(i, 1);
                        break;
                    }
                }
            }
            if (num == 0) {
                $('.conditionDiv ul li:eq(0)').css({ 'margin-left': '70px' });
            }
            var length = $('.SCShortcut').length;
            for (var i = 0; i < length; i++) {
                if ($('.shortcut .SCShortcut:eq(' + i + ')').attr('term') == $(this).parent().find('span:eq(0)').attr('term')) {
                    $('.shortcut .SCShortcut:eq(' + i + ')').css({ 'color': '#686868' }).removeClass('chosen');
                    $('.planUl .SCShortcut:eq(' + i + ')').css({ 'color': '#686868' }).removeClass('chosen');
                    break;
                }
            }
            kjFindLoadDiv();
        });
    }
    /* 我的快捷的关闭 结束 */
    /* 我的快捷方式 结束 */

    /* 弹窗 结束 */
    //排序点击事件:默认，重要度，紧急度和时间
    $("#xxc_order .xxc_changeorder").click(function () {
        var field = parseInt($(this).attr("term"));
        var directNew = parseInt($(this).attr("direct"));
        SCConData.sorts = [{ type: field, direct: directNew }];
        if (field != "8") {
            $('.headL .arrows .arrowSolidTCom').css({ 'background': 'url(../../Images/common/arrowSolidT.png) no-repeat' });
            $('.headL .arrows .arrowSolidBCom').css({ 'background': 'url(../../Images/common/arrowSolidB.png) no-repeat' });
            if (directNew == 1) {
                $(this).attr("direct", 0);
                $(this).find(".arrows .arrowSolidTCom").css({ 'background': 'url(../../Images/common/arrowSolidTHit.png) no-repeat' });
            } else {
                $(this).attr("direct", 1);
                $(this).find(".arrows .arrowSolidBCom").css({ 'background': 'url(../../Images/common/arrowSolidBHit.png) no-repeat' });
            }
        } else {
            $('.headL .arrows .arrowSolidTCom').css({ 'background': 'url(../../Images/common/arrowSolidT.png) no-repeat' });
            $('.headL .arrows .arrowSolidBCom').css({ 'background': 'url(../../Images/common/arrowSolidB.png) no-repeat' });
        }
        pageIndex = 1;
        $('.conL .bottom').hide();
        $("#xxc_planList").html('');
        fnScreCon();
    });
});

/*分页开始*/
/* 滚轮滚到底部触发事件 开始 */

$(window).scroll(function () {
    var scrollTop = $(this).scrollTop();
    var scrollHeight = $(document).height();
    var windowHeight = $(this).height();
    if (scrollTop + windowHeight >= scrollHeight) {
        if ($("#pageflag").val() == "true") {
            if ($('.chooseHit').length > 0) {
                $('.chooseHit').each(function () {
                    planIds.push($(this).attr('planId'));
                });
            }
            $('.refresh').show();
            $('.conL .bottom').show();
            $('#pagemessage').hide();
            pageIndex++;
            fnScreCon(1);
        }
        else {
            $('#pagemessage').text('已经到底啦，没有更多了~').show();
            $('.refresh').hide();
        }
    }
    /* 鼠标滚动时——planListTabTop一直在顶部——开始*/
    if (scrollTop >= 167) {
        $(".planListTabTop").show();
    }
    else {
        $(".planListTabTop").hide();
    }
    /* 鼠标滚动时——planListTabTop一直在顶部——结束*/

    /* 鼠标滚动时——topBot也在动——开始*/
    $('.topBot').css({ 'right': '10px', 'top': scrollTop + windowHeight - 40 });
    /* 鼠标滚动时——topBot也在动——结束*/
});
//                function funcScroll() {
//                    $('.loadingMore').css({ 'display': 'none' });
//                }
/* 滚轮滚到底部触发事件 结束 */
/*分页结束*/


//加载计划列表并注册一系列点击操作
function fnScreCon(loadFlag) {
    if (loadFlag != 1) {
        var list_lodi = getLoadingPosition('.planList');  //加载局部视图的对象
    }
    $("#xxc_planList").load("/Plan/SelectOrSort", { params: JSON.stringify(SCConData), pageIndex: pageIndex }, function (data) {
        if (goToLoginPage(data)) {
            if (loadFlag == 1) {
                if (cardoperate) {
                    cardListShow();
                    if (planIds.length > 0) {
                        for (var i = 0; i < planIds.length; i++) {
                            $('.PLChunk').each(function () {
                                if ($(this).attr('term') == planIds[i]) {
                                    $(this).find('.xxc_choose').addClass('chooseHit').removeClass('prohibit');
                                    $(this).find('.xxc_choose span').addClass('spanHit');
                                }
                            });
                        }

                    }
                }

            }
            else {
                $('.headL .moreCancel').hide();
                $('.moreBg').hide();
                $('.xxc_choose').removeClass('choose').removeClass('prohibit');
                cardoperate = "";
            }
            list_lodi.remove();
            //lodi = undefined;
        }
        
    });
}

//load请求验证登录和权限
function goToLoginPage(data)
{
    if (data == '{"success":true,"login":false,"data":"","message":""}') {
        location.href = "/"; 
    } else {
        return true;
    }
}

//显示批量的操作
function cardListShow() {
    if (cardoperate == '全部导出') {
        $('.moreBg').hide();
        $('.moreCancel').hide();
        $('.PLChunk .choose').hide();
        if ($('.PLChunk').length <= 1) {
            ncUnits.alert("没有计划，无需导出！");
            return;
        }
        //loadViewToMain("/plan/NpoiExcel");
        window.location.href = "/plan/NpoiExcel";
        planIds = [];
        //$('.PLChunk .prohibit').hide();
        return;
    } else {
        var status;
        var stop;
        $('.moreBg').show();
        $(".moreCancel").show();
        $('.chooseHit span').removeClass('spanHit');
        $('.chooseHit').removeClass('chooseHit');
        if (cardoperate == '导出') {
            if ($('.PLChunk').length <= 1) {
                $('.moreBg').hide();
                $(".moreCancel").hide();
                ncUnits.alert("没有计划，无需导出！");
                return;
            }
            //$('.moreBg').css({ 'background': 'url(/Images/plan/export.png) no-repeat' });
            $('.xxc_choose').addClass('choose').removeClass('prohibit');
            $('.moreBg').attr('term', '1');
        }
        if (cardoperate == '归类') {
            //$('.moreBg').css({ 'background': 'url(/Images/plan/classify.png) no-repeat' });
            $('.xxc_choose').addClass('choose').removeClass('prohibit');
            $('.moreBg').attr('term', '2');
        }
        if (cardoperate == '提交') {
            //$('.moreBg').css({ 'background': 'url(/Images/plan/submiteB.png) no-repeat' });
            $('.moreBg').attr('term', '3');
            $(".PLChunk").each(function () {
                status = $(this).attr('status');
                stop = $(this).attr("stop");
                if ((status == "0" || status == "15") && stop == "0") {
                    $(this).find('.xxc_choose').addClass("choose").removeClass("prohibit");
                }
                else {
                    $('.prohibit').show();
                    $(this).find('.xxc_choose').addClass("prohibit").removeClass("choose");
                }
            });
        }
        if (cardoperate == '审批') {
            //$('.moreBg').css({ 'background': 'url(/Images/plan/examine.png) no-repeat' });
            $('.moreBg').attr('term', '4');
            $(".PLChunk").each(function () {
                status = $(this).attr('status');
                stop = $(this).attr("stop");
                if (status == "10" && stop == "0") {
                    $(this).find('.xxc_choose').addClass("choose").removeClass("prohibit");
                }
                else {
                    $('.prohibit').show();
                    $(this).find('.xxc_choose').addClass("prohibit").removeClass("choose");
                }
            });
        }
        if (cardoperate == '删除') {
            //$('.moreBg').css({ 'background': 'url(/Images/plan/delete.png) no-repeat' });
            $('.moreBg').attr('term', '5');
            $(".PLChunk").each(function () {
                status = $(this).attr('status');
                stop = $(this).attr("stop");
                initial = $(this).attr("initial");
                if ((status == "0" || status == "15" || (status == "20" && initial=="0")) && stop == "0") {
                    $(this).find('.xxc_choose').addClass("choose").removeClass("prohibit");
                }
                else {
                    $('.prohibit').show();
                    $(this).find('.xxc_choose').addClass("prohibit").removeClass("choose");
                }
            });
        }
        $('.moreBg').html(cardoperate);
        if ($('.PLChunk .choose').css('display') == 'none') {
            $('.PLChunk .choose').css({ 'display': 'block' });
        }
        
        $('.PLChunk').unbind('mouseenter').unbind('mouseleave');
    }
}