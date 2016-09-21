/**
 * Created by DELL on 2015/11/30.
 */
define([ 'planold', 'dialog', 'artTemplate', 'echarts', 'echarts/chart/line'], function (plan, dialog, template, echarts) {
    var plan = plan;

    var dialog = dialog;
    $(document).on('click', function (e) {
        $('.operate-list').remove();
        $('.downIndex').remove();
    })
    //日期显示
    var ShowDay = function () {
        this.day = {};//输入日期
        this.transDay = function (date) {
            var result = {
                year: date ? new Date(date).getFullYear() : new Date().getFullYear(),
                month: date ? new Date(date).getMonth() + 1 : new Date().getMonth() + 1,
                day: date ? new Date(date).getDate() : new Date().getDate(),
                week: date?new Date(date).getDay():new Date().getDay()
            }
            result.day = result.day < 10 ? '0' + result.day : result.day;
            result.month = result.month < 10 ? '0' + result.month : result.month;
            result.week = result.week == 0 ? 7 : result.week;
            return result
        };
        this.now = this.transDay();
        this.dayMinus = function () {
            var result = (+new Date(this.day.year + '-' + this.day.month + '-' + this.day.day)) - (+new Date(this.now.year + '-' + this.now.month + '-' + this.now.day));
            return result;

        }
        this.getDay = function (date) {
           
            this.day = this.transDay(date);  //传入日期格式化          
            var days = this.dayMinus() / 86400000; //相差天数           
            var result;
            switch (true) {
                case days>-3&&days<3:
                    result = { 0: '今天', 1: '明天', 2: '后天' ,'-1':'昨天','-2':'前天'}[days];
                    break;
                case days >= 3 && days<=90:
                    console.log('dd',days)
                    var plus = this.now.week+ days;
                    result = { '4': '周四', '5': '周五', '6': '周六', '7': '周日', '8': '下周一', '9': '下周二', '10': '下周三', '11': '下周四', '12': '下周五', '13': '下周六', '14': '下周日' }[plus] || date.substring(5, 10).replace('-', '月') + '日';
                    break
                case days<=-3 && days>=-90:
                    result = date.substring(5, 10).replace('-','月')+'日'
                    break;
                default:
                    result = date.substring(2, 7).replace('-', '年') + '月';

            }
            
            return result;


        }


    }

    //日程日历
    var Schedule = function () {
        var dateArray = [];
        var until = {
            getYear: function (that) {
                return parseInt(that.find('li').eq(1).attr('date-year'), 10);
            },
            getMonth: function (that) {
                return parseInt(that.find('li').eq(1).attr('date-month'), 10);
            },
            getWeek: function (year, month, day) {
                return new Date(year + '-' + month + '-' + day).getDay();
            },
            getDays: function (year, month) {
                var d = new Date(year, month, 0);
                return d.getDate();
            },
            setDate: function (temp, year, month, days, that, currentDays, selected) {
                that.find('li').eq(1).removeClass('gray-color');
                that.removeClass('choosed');
                var tYear = year;
                var tMonth = month;
                if (temp < 1 && days) {//上年
                    temp = days + temp;
                    tYear = tMonth == 1 ? tYear - 1 : tYear;
                    tMonth = tMonth == 1 ? 12 : tMonth - 1;
                    that.find('li').eq(1).addClass('gray-color');
                };
                if (temp > currentDays && currentDays) {//下年
                    temp = temp - currentDays;
                    tYear = tMonth == 12 ? tYear + 1 : tYear;
                    tMonth = tMonth == 12 ? 1 : tMonth + 1;
                    that.find('li').eq(1).addClass('gray-color');
                };
                if (selected && selected.year == tYear && selected.month == tMonth && selected.day == temp) {
                    that.addClass('choosed');
                }

                that.find('li').eq(1).html(temp).attr('date-year', tYear).attr('date-month', tMonth);
            }

        }
        this.selected = {
            year: new Date().getFullYear(),
            month: new Date().getMonth() + 1,
            day: new Date().getDate(),
            week: new Date().getDay() == 0 ? 7 : new Date().getDay()
        };//今天为默认选中日程
        //获取一周有日程的天数
        this.getCount = function () {
            $('.candleDate').find('.isCount').removeClass('count-red')
            var start = $('.candleDate').eq(0).find('.isCount');
            var end = $('.candleDate').eq(6).find('.isCount');
            var startDay = start.attr('date-year') + '-' + start.attr('date-month') + '-' + start.text();
            var endDay = end.attr('date-year') + '-' + end.attr('date-month') + '-' + end.text();
            console.log('start', startDay + '--' + endDay);
            $.post('/Calendar/GetWeekCalendarCount', { fromDate: startDay, toDate: endDay }, function (data) {
                var count = data.data;
                count = count.map(function (val) {
                    if (val.isZero == false) {
                        return val.date.substring(0, 10);
                    }
                })
                $('.candleDate').each(function () {
                    var ele = $(this).find('.isCount');
                    var month = Number(ele.attr('date-month'));
                    month = month < 10 ? '0' + month : month;
                    var day = Number(ele.text());
                    day = day < 10 ? '0' + day : day;
                    var date = ele.attr('date-year') + '-' + month + '-' + day;
                    if (count.indexOf(date) > -1) {
                        ele.addClass('count-red')
                    }
                })
            }, 'json')
        };
        this.dateInit = function () {
            var choose = this.selected;
            var year = choose.year, month = choose.month;
            var currentDays = until.getDays(year, month);//当前月天数
            var preDays = until.getDays(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1);//上个月天数
            $('.shedule-month .shedule-year').text(choose.year).next('span').text(choose.month);;
            var selected = this.selected
            $('.shedule-day .candleDate').each(function (n) {
                var t = choose.week - n - 1;
                var d = choose.day;
                var tYear = year;
                var tMonth = month;
                dateArray.push(d - t);
                var temp = dateArray[n];
                until.setDate(temp, year, month, preDays, $(this), currentDays, selected)
            })
            this.getCount();
        }
        this.dateChoose = function (day, ele) {
            this.selected.year = until.getYear(ele);
            this.selected.month = until.getMonth(ele);
            this.selected.day = parseInt(day, 10);
            this.selected.week = until.getWeek(this.selected.year, this.selected.month, this.selected.day)
            this.selected.week = this.selected.week == 0 ? 7 : this.selected.week;
            //return { year: this.selected.year, month: this.selected.month, day: this.selected.day }
            return this.selected.year + '-' + this.selected.month + '-' + this.selected.day;
        }

        this.preWeek = function () {
            var selected = this.selected;
            var year = parseInt($('.shedule-year').text(), 10), month = parseInt($('.shedule-month-day').text(), 10);
            var preDays = until.getDays(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1);//上个月天数
            if (Math.min.apply(Math, dateArray) <= 1) {
                year = month == 1 ? year - 1 : year;
                month = month == 1 ? 12 : month - 1;
                $('.shedule-year').text(year).next('.shedule-month-day').text(month);
                dateArray = dateArray.map(function (val) {
                    return preDays + val;
                })
            }
            dateArray = dateArray.map(function (val) {
                return val - 7
            });
            $('.shedule-day .candleDate').each(function (n) {
                var temp = dateArray[n];
                until.setDate(temp, year, month, preDays, $(this), '', selected)
            })
            this.getCount()
        }
        this.nextWeek = function () {
            var selected = this.selected;
            var year = parseInt($('.shedule-year').text(), 10), month = parseInt($('.shedule-month-day').text(), 10);
            var currentDays = until.getDays(year, month);//当前月天数
            if (Math.max.apply(Math, dateArray) >= currentDays) {
                year = month == 12 ? year + 1 : year;
                month = month == 12 ? 1 : month + 1;
                $('.shedule-year').text(year).next('.shedule-month-day').text(month);
                dateArray = dateArray.map(function (val) {
                    return val - currentDays
                })
            }
            dateArray = dateArray.map(function (val) {
                return val + 7
            })
            $('.shedule-day .candleDate').each(function (n) {
                var temp = dateArray[n];
                until.setDate(temp, year, month, '', $(this), currentDays, selected)
            })
            this.getCount();
        }
        this.getSelected = function () {
            return this.selected;
        }
    }

    //新建日程

    var Caldener = function (scope) {
        this.postData = {};
        var scope = scope;
        this.date;
        var start = {
            elem:'#start-times',
            event: 'click',
            format: 'YYYY-MM-DD hh:mm',
            isclear: true,
            istoday: true,
            issure: true,
            festival: true,
            istime: true,
            start: '2014-6-15 23:00:00',
            choose: function (date) {
                end.min = date
            },
            clear: function () {
                end.min = undefined;
            }
        };
        var end = {
            elem: '#end-times',
            event: 'click',
            format: 'YYYY-MM-DD hh:mm',
            isclear: true,
            istoday: true,
            issure: true,
            festival: true,
            istime: true,
            start: '2014-6-15 23:00:00',
            choose: function (date) {
                start.max = date;
            },
            clear: function () {
                start.max = undefined;
            }
        };

        this.initDate = function (event) {
            if ($('#start-times').attr('readonly') == 'readonly') {
                return;
            }
            var obj = new Date();
            var now = obj.getFullYear() + '-' + (obj.getMonth() + 1) + '-' + obj.getDate() + ' ' + obj.getHours() + ':' + obj.getMinutes() + ':00';
            //config.elem = event.name;
            //config.start = obj.getFullYear() + '-' + (obj.getMonth() + 1) + '-' + obj.getDate() + ' ' + obj.getHours() + ':' + obj.getMinutes() + ':00';
            if ($(this).attr('id') == 'start-times') {
                start.start = now;
                laydate(start);
            } else {
                end.start = now;
                laydate(end);
            }
           
        };
        this.checkInfo = function (check) {
            if (!check.startTime) {
                ncUnits.alert("开始时间不能为空！");
                return false;
            }
            if (!check.endTime) {
                ncUnits.alert("结束时间不能为空！");
                return false;
            }
            if (!check.comment) {
                ncUnits.alert("备注不能为空！");
                return false;
            }
            return true;
        };
        this.commitData = function (e) {
            var element = $(e.currentTarget);
            var alertData = {
                success: '新建日程成功',
                err: '新建日程失败'
            }
            if (element.text() == '编辑') {
                $('#scheduleMoal input').removeAttr('readonly');
                $('#scheduleMoal i').show();
                element.text('提交')
                return
            }
            var data = {
                startTime: $('#start-times').val(),
                endTime: $('#end-times').val(),
                place: $('#place').val(),
                comment: $('#comment').val()
            }
            var url = '/Calendar/AddCalendar'
            if (element.attr('pid')) {
                data.calendarId = element.attr('pid');
                url = '/Calendar/UpdCalendar'
                alertData = {
                    success: '修改日程成功',
                    err: '修改日程失败'
                }
            }
            if (!this.checkInfo(data)) {
                return;
            }
            var temp = this.date
            $.post(url, {data:JSON.stringify(data)}, function (result) {
                if (result.success == true) {
                    ncUnits.alert(alertData.success);
                    $('#scheduleMoal').modal('hide');
                    scope.http('/Calendar/GetDayCalendarInfo', {date:temp.year + '-' + temp.month + '-' + temp.day}, scope.init['initSchedule']);
                    return
                }
                ncUnits.alert(alertData.err);

            }, 'json')
                       
        };
        $('#scheduleMoal').off('click', '#start-times,#end-times');
        $('#scheduleMoal').on('click', '#start-times', this.initDate);
        $('#scheduleMoal').on('click', '#end-times', this.initDate);
        $('#scheduleMoal').on('click', '.commitData', this.commitData.bind(this));
        $('#scheduleMoal').on("hide.bs.modal", function () {
            start.max = undefined;
            end.min = undefined;
        }.bind(this));
    }
    Caldener.prototype = {
        show: function (date) {
            $('#scheduleMoal').modal('show');
            $('#scheduleMoal input').val('');
            this.date = date;
            $('#scheduleMoal input').removeAttr('readonly');
            $('#scheduleMoal i').show();
            $('.commitData').removeAttr('pid').text('提交');
            $('#scheduleMoal .modal-title').text('新建日程')
        },
        edit: function (date,data) {
            this.date = date;
            console.log(data)
            $('#scheduleMoal').modal('show');
            $('#start-times').val(data.startTime.replace('T', ' ').substring(0,16));
            $('#end-times').val(data.endTime.replace('T', ' ').substring(0,16))
            $('#place').val(data.place);
            $('#comment').val(data.comment);
            $('#scheduleMoal input').attr('readonly', true);
            $('#scheduleMoal i').hide();
            $('.commitData').text('编辑').attr('pid', data.calendarId);
            $('#scheduleMoal .modal-title').text('修改日程')
        },
        hide: function () {
            $('#scheduleMoal').modal('hide')
        }

    }

    function CommonHttp() {        
        var req;
        var showDay = new ShowDay();
        return function (url, post, func,ele) {
            console.time(url);
            var url = url, upData = post, func = func, data;
            var ele = ele ? ele : '';
            req =  $.ajax({
                type: "post",
                url: url,
                data: upData,
                dataType: "json",
                success: rsHandler(function (result) {
                    console.timeEnd(url);
                    switch (url) {
                        //个人文档列表
                        case '/Calendar/GetDayCalendarInfo':
                            $.each(result, function (n, val) {
                                //var startHour = parseInt(val.startTime.substring(11, 13),10);
                                //var startMin = parseInt(val.startTime.substring(14, 16), 10) < 10 ? '0' + parseInt(val.startTime.substring(14, 16), 10) : parseInt(val.startTime.substring(14, 16), 10);
                                //startHour = startHour < 12 ? '早晨'+startHour : '下午'+(startHour - 12?startHour - 12:'00');
                                //val.sTime = startHour + ':' + startMin;
                                //var endHour = parseInt(val.endTime.substring(11, 13), 10);
                                //var endMin = parseInt(val.endTime.substring(14, 16), 10) < 10 ? '0' + parseInt(val.endTime.substring(14, 16), 10) : parseInt(val.endTime.substring(14, 16), 10);
                                //endHour = endHour < 12 ? '早晨' + endHour : '下午' + (endHour - 12 ? endHour - 12 : '00');
                                //val.eTime = endHour + ':' + endMin;
                                val.sTime = val.startTime.substring(5, 16).replace('T', ' ');
                                val.eTime = val.endTime.substring(5, 16).replace('T', ' ');
                            })
                            data = {
                                list:result
                            }
                            break;
                            //部门工时列表
                        case '/UserIndex/GetPerformanceRankInfo':
                            data = {
                                list: result
                            }
                            break;
                            //折线图
                        case '/UserIndex/GetUserWorkTimeStatistics':
                            data = {
                                actualWorkTime: [],
                                effectiveWorkTime: [],
                                time:[]
                            }
                            $.each(result, function (n,val) {
                                data.actualWorkTime.push(val.actualWorkTime ? val.actualWorkTime : 0);
                                data.effectiveWorkTime.push(val.effectiveWorkTime ? val.effectiveWorkTime : 0);
                                data.time.push(val.workdate);
                            })
                          
                            break;
                        case '/UserIndex/GetNeedToDoPlanList':
                            console.log('tttt', upData.type)
                            var planCommit = [],planConfirm = []; //我的待提交，进行中
                            var planAudit = [], planSure = []; //下属待审核，待确认
                            plan(result, upData.type)
                            $.each(result, function (n, val) {
                                val.stars = [1, 2, 3, 4, 5];
                                val.endTime = showDay.getDay(val.endTime.substring(0, 10))
                                val.idx = n;
                                if (val.status == 0 || val.status == 15) {
                                    planCommit.push(val);
                                }else if ((val.status == 40 || val.status == 20) && upData.type == 0) {
                                    planConfirm.push(val);
                                }else if (((val.status == 10 || val.status == 25) && val.stop == 0) || val.stop == 10) {
                                    planAudit.push(val);
                                }else if (val.status == 30 && upData.type == 1) {
                                    planSure.push(val);
                                }
                                
                            })
                            data = {
                                planCommit: planCommit,
                                plamConfirm: planConfirm,
                                planAudit: planAudit,
                                planSure: planSure,
                            }
                           
                            break;
                        default:
                            data = result
                            break


                    }
                    func(data,ele);

                })
            })


        }
    }
    //echarts
    var Chart = function () {
        this.myChart;
        this.option = {
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data: ['自评工时', '有效工时'],
                y: 'bottom'
            },

            calculable: false,
            grid: { x: 20, y: 10, x2: 15, y2: 60 },
            xAxis: [
                {
                    type: 'category',
                    boundaryGap: false,
                    axisLine: {    // 轴线
                        show: false,
                        lineStyle: {
                            color: '#49A4FE',
                            width: 2
                        }
                    },
                    
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    axisLabel: {
                        formatter: '{value}'
                    },
                    splitNumber: 6,
                    max: 12,
                    min:0,
                    axisLine: {    // 轴线
                        show: false,
                        lineStyle: {
                            color: '#49A4FE',
                            width: 2
                        }
                    }
                }
            ],
            series: []
        };
        this.changeData = function (result) {
            this.myChart = echarts.init(document.getElementById('chart-pic'));
            this.option.series = [
                {
                    name: '自评工时',
                    type: 'line',
                    itemStyle: { normal: { areaStyle: { type: 'default' } } },
                    data: result.actualWorkTime
                },

                {
                    name: '有效工时',
                    itemStyle: { normal: { areaStyle: { type: 'default' } } },
                    type: 'line',
                    data: result.effectiveWorkTime
                }
            ];
            if (result.time.length < 7) {
                this.option.xAxis[0].data = result.time.map(function (val) {
                    return val + '周';
                })
            } else {
                this.option.xAxis[0].data = ['日', '一', '二', '三', '四', '五', '六'];
            }
           
            this.myChart.setOption(this.option);
            window.onresize = this.myChart.resize;
        }
        
    }

    //工时统计和绩效排行切换
    var SumTab = function () {
        this.sum = {
            url: '/UserIndex/GetUserWorkTimeStatistics'
            , param: 1
            , name: 'initSum'
        }
        this.timeMonth = function (m) {
            this.sum.param = parseInt(m, 10);
            return this.sum;
        }
        this.typeData = function (t, name) {
            this.sum.url = t;
            this.sum.name = name;
            return this.sum;
        }

    }


    //计划(1)，流程(3)，目标(2)
    var WorkAction = function () {
        this.type = 1;//默认为计划
        this.getType = function () {
            return this.type;

        }
        this.changeType = function (type) {
            this.type = type;
        }



    }

  


    //首页计划，目标，流程不变数据
    var Common = function () {
        this.events = {
            'click .month-tab,.week-tab': 'choose', //激励和未完成周月切换
            'click .month-sum-tab,.week-sum-tab': 'timetab', //工时和绩效周月切换
            'click .sum-grade,.sum-time': 'sumtab', //绩效和工时切换
            'click .candleDate': 'candleClick', //选择日程日期
            'click .move-day': 'weekDay',//日期切换
            'click .index-tabs': 'indexTabs',//目标流程计划切换
            'click .li-check': 'operate', //操作条显示
            'click .first-li,.last-li,.range-li,.showFiles': 'stopUp',
            'click .operate-list-li': 'disapear',
            'click .operate-list': 'reback',
            'change .checkAll,.checkAudit,.checkConfirm,.checkSure': 'checkall', //全选
            'change .checkSingle,.auditSingle,.confirmSingle,sureSingle': 'checksingle', //单独选择
            'click .myTarget,.subTarget': 'isMine',//我的（计划，目标）还是下属的（计划，目标）
            'click .showFiles': 'showDownload',//显示下载附件列表
            'click .single-down': 'singleDown',//单个下载
            'click .multi-down': 'multiDown',//多个下载
            'click .addSchedule': 'addSchedule', //添加日程
            'click .schedule-all': 'editSchedule',//修改日程
            'click .delSchedule': 'delSchedule',//删除日程
            'mouseup .change-progress': 'changeProgress', //修改进度
            'click .planDetail': 'planDetail', //计划详情
            'click .audit': 'planAudit' ,//计划审核
            'click .confirmPlan': 'planConfirm', //计划确认
            'click .submit': 'planSubmit',//计划提交
            'click .transimitPlan': 'planTrans', //计划转办
            'click .submitTo,.delPlans,.delPlan,.modification,.suspend': 'planOtherOperate' //计划其他操作(终止，修改，提交，删除)

        };
        //外部刷新自定义事件
        this.refreshEle = {
            'addPlan': '#xxc_checkpass,#addplan_save,#xxc_makesure,#addplan_submit,#planSplitSure,#submitUploadFile,#xxc_confirmpass,#confirmBtn,#xxc_checknopass,#xxc_confirmnopass'//新增计划刷新

        };
        this.refreshEvent = {
            'addPlan': 'listRefresh,statusRefresh'
        }
        this.el = $('#main-context');
        this.average = 8;//平均工时为8；
        this.nowTime = {
            year: new Date().getFullYear(),
            month: new Date().getMonth() + 1,
            day: new Date().getDate(),
            week: { 0: '星期天', 1: '星期一', 2:'星期二', 3: '星期三', 4: '星期四', 5: '星期五'}[new Date().getDay()] || '星期六'
        }
        var scheduleData = '';//日程数据
        var targetData = ''//计划，目标，流程数据
        this.http = new CommonHttp();
        this.post = {
            downLoad: { url: '/UserIndex/GetPlanAttachmentList'},//计划附件下载列表
            initPlan: { url: '/UserIndex/GetNeedToDoPlanList', isSubordinate: 0 },//计划列表
            initObject: { url: '/UserIndex/GetObjectiveList', isSubordinate: 0 },//目标列表
            initFlot: { url: '/UserIndex/GetUserFlowCompleteList', isSubordinate: 0 },//流程列表
            initPerson: { url: '/UserIndex/GetUserWorkTimeInfo', param: null },//工时信息
            initTodo: { url: '/UserIndex/GetUserPlanStatusInfo', param: 1 },//各个计划未完成状态数量
            initPer: { url: '/UserIndex/GetUserPlanCompleteInfo', param: 1 },//计划完成与未完成百分比
            initMotivate: { url: '/UserIndex/GetUserIncentiveInfo', param: 1 },//激励
            initSum: { url: '/UserIndex/GetUserWorkTimeStatistics', param: 1 },//工时统计
            grade: { url: '/UserIndex/GetPerformanceRankInfo', param: 1 },//绩效排行
            initSchedule: { url: '/Calendar/GetDayCalendarInfo', param: null, year: new Date().getFullYear().toString(), month: (new Date().getMonth() + 1).toString(), day: new Date().getDate().toString() } //日程
        };

        this.templateTo = function (data, selector, target) {
            var html = template(selector, data);
            var $id = document.getElementById(target);
            var move = { 'shedule-target': '.schedule-all', 'project-target': '.li-audit', 'grade-target': '.sum-person li' }[target]
            //if (target != 'project-target') {
                $(move).addClass('schedule-all-leave');
                var t = setTimeout(function () {
                    $('#' + target).html(html);
                    $(move).addClass('schedule-all-to');
                    clearTimeout(t)
                }, 200)
              
            //    return
            //}
            //$id.innerHTML = html;
        };
        this.getValue = function (selector) {
            var result = []
            $('input[name='+selector+']:checked').each(function (n, val) {
                result.push($(this).val())

            })
            return result;

        }
        this.setPlan = function (status) {
            var result;
            console.log('status',status)
            switch (status) {
                case '1': //我的计划待提交
                    var values = this.getValue('checkSingle');
                    if (values.length == 0) {
                        result = '<li class="submitTo operate-list-li" title="提交"></li><li class="delPlan del operate-list-li" title="删除"></li><li class="transimitPlan operate-list-li" title="转办"></li>';

                    } else {
                        result = '<li class="submitTo" title="提交"><li class="delPlans del operate-list-li" title="删除"></li></li>';
                       
                    }
                    break;
                case '2': //我的计划待确认
                    result = '</li><li class="transimitPlan operate-list-li" title="转办"></li><li class="modification operate-list-li" title="修改"></li><li class="suspend operate-list-li" title="终止"></li><li class="submit operate-list-li" title="提交"></li>';
                    break;
                case '3': //下属计划待审核
                    var values = this.getValue('auditSingle');
                    if (values.length == 0) {
                        result = '<li class="audit operate-list-li" title="审核"></li><li class="transimitPlan operate-list-li" title="转办"></li>';

                    } else {
                        result = '<li class="audit operate-list-li" title="审核"></li>'
                    }
                    break;
                case '4': //下属计划待确认
                    result = '<li class="confirmPlan operate-list-li" title="确认"></li><li class="transimitPlan operate-list-li" title="转办"></li>';
                    break;

            }
            return result

        };
        this.showOperate = function (selector) {
            var selector = selector.replace('.', '');
            var result = [],html;
            $('input[name=' + selector + ']:checked').each(function (n, val) {
                result.push($(this).val())

            })
            if (result.length == 0) {
                if (selector == 'checkSingle') {
                    html = '<li class="submitTo operate-list-li" title="提交"></li><li class="delPlan del operate-list-li" title="删除"></li><li class="transimitPlan operate-list-li" title="转办"></li>';
                } else {
                    html = '<li class="audit operate-list-li" title="审核"></li><li class="transimitPlan operate-list-li" title="转办"></li>';
                }
               
            } else {
                if (selector == 'auditSingle') {
                    html = '<li class="audit operate-list-li" title="审核"></li>';
                } else {
                    html = '<li class="submitTo operate-list-li" title="提交"></li><li class="delPlans del operate-list-li" title="删除"></li>';
                }
            }
            $('.operate-list ul').html(html)

        }
        this.file = new dialog.files(this.refreshView.bind(this)); //文件预览
        this.confirm = new dialog.confirm();//确认弹窗
        this.showDetail = new dialog.detail(this.refreshView.bind(this));//计划详情
        this.addPlan = new dialog.addplan(this.refreshView.bind(this));//新建计划或修改计划
        this.trans = new dialog.trans(this.refreshView.bind(this));//计划转办
        this.planOther = new dialog.other(this.refreshView.bind(this));//计划其他操作
        this.checkChild = function (parent, child) {
            if ($(parent).is(':checked')) {
                $(child).prop('checked', true);
                $(child).siblings().addClass('fa-check-square-o').removeClass('fa-square-o');
                $(parent).siblings().addClass('fa-check-square-o').removeClass('fa-square-o')

            } else {
                $(child).prop('checked', false);
                $(child).siblings().addClass('fa-square-o').removeClass('fa-check-square-o');
                $(parent).siblings().addClass('fa-square-o').removeClass('fa-check-square-o')
            }
            if (child == '.auditSingle' || child == '.checkSingle') {
                this.showOperate(child)
            }
            
        };
        this.checkParent = function (parent, child,ele) {
            var isChecked = true;
            if (ele.is(':checked')) {
                ele.siblings().addClass('fa-check-square-o').removeClass('fa-square-o');
            } else {
                ele.siblings().addClass('fa-square-o').removeClass('fa-check-square-o');
            }
            $(child).each(function () {
                if (!$(this).is(':checked')) {                 
                    isChecked = false;
                } 
            })
            if (isChecked) {
                $(parent).prop('checked', true);
                $(parent).siblings().addClass('fa-check-square-o').removeClass('fa-square-o')
            } else {
                $(parent).prop('checked', false);
                $(parent).siblings().addClass('fa-square-o').removeClass('fa-check-square-o');
            }
            
            if (child == '.auditSingle' || child == '.checkSingle') {
                this.showOperate(child)
            }
        };
        this.sumTab = new SumTab();
        this.shedule = new Schedule();
        this.chart = new Chart();
        this.workAction = new WorkAction();
        this.setPercent = function (per) {
            $('.undo-circle,.done-circle').show();
            if (per == 100) {
                $('.undo-circle').hide();
            } else if (per == 0) {
                $('.done-circle').hide();
            } 
            var per = per / 100;
            var done = 360 * per / 2+7;         
            var undo = -(360 * (1 - per) / 2) + 7;
            
            
            $('.done-circle').css({ 'transform': 'rotate(' + done + 'deg)' }).find('.inside-circle').text(Math.round(per * 100) + '%').css({ 'transform': 'rotate(' + (-done) + 'deg)' });
            $('.undo-circle').css({ 'transform': 'rotate(' + undo + 'deg)' }).find('.inside-circle').text(Math.round((1 - per) * 100) + '%').css({ 'transform': 'rotate(' + (-undo) + 'deg)' });
        }
        this.textData = function (content) {
            for (var i in content) {
                $(i).text(content[i])
            }
        }
        this.setActive = function (type,ele) {
            ele.addClass('workActive').siblings('li').removeClass('workActive');
            var left = $('.workActive').hasClass('myTarget')?0:70;
            $('.underLine').delay().animate({'left':left+'px'})
        }
        this.caldener = new Caldener(this);
    }

    Common.prototype = {
        init: {
            //工时模块数据初始化
            initPerson: function (data) {
                var dayLeave = (data.yesterdayWorkTime * 10) - (this.average * 10);
                var weekLeave = data.weekAvgWorkTIme - this.average;
                var content = {
                    '.person-week span': data.weekTotalWorkTime || 0,
                    '.person-yesterday span': data.yesterdayWorkTime || 0,
                    '.person-month span': data.monthTotalWorkTime || 0,
                    '.person-average span': data.weekAvgWorkTIme || 0,
                    '.person-day': dayLeave/10
                };
                if (dayLeave == 0) {
                    $('.person-day').hide();
                } else {
                    $('.person-day').show();
                }
                $('.icon-center').attr('src', data.imgUrl+'?'+Date.now());
                if (weekLeave < 0) {
                    $('.average-week').show();
                } else {
                    $('.average-week').hide();
                }
              
                this.textData(content)             
            }
            //未完成事项数据初始化
            , initTodo: function (data) {
                var content = {
                    '.c_status': data.checkingCount,
                    '.c_audit': data.checkedCount,
                    '.c_commit': data.submitingCount,
                    '.c_confirm': data.confirmingCount,
                  
                };
                this.textData(content);
              
                

            }
            , initPer: function (data) {
                $('.undo').text(data.notCompleteCount);
                $('.done').text(data.completeCount)
                var per = data.completeCount == 0 ? 0 : (data.completeCount) / (data.notCompleteCount + data.completeCount);
                per = per * 100;//已完成百分比
                this.setPercent(per)
                if (per <= 50) {
                    $('#rount').css('transform', "rotate(" + 3.6 * per + "deg)");
                    $('#rount2').hide();
                    return

                }
                $('#rount').css('transform', "rotate(180deg)");
                $('#rount2').show().css('transform', "rotate(" + 3.6 * (per - 50) + "deg)");

            }
            //激励模块数据初始化
            , initMotivate: function (data) {
                //$('.motivate-num').removeClass('gun');
                //var t = setTimeout(function () {
                    //$('.motivate-num').addClass('gun');
                    $('.motivate-num li').eq(0).find('span').text('+' + data.rewardAmount);
                    $('.motivate-num li').eq(1).find('span').text('-' + data.punishmentAmount);
                    
                //    clearTimeout(t)
                //}, 100)
            }
            //工时统计模块数据初始化
            , initSum: function (data) {
               
                this.chart.changeData(data);
            }
            //日程模块数据初始化
            , initSchedule: function (data) {
                scheduleData = data;
                this.templateTo(data, 'shedule-template', 'shedule-target');
            }
        },

        work: {
            //计划列表
            plan: function (data) {
                
                targetData = data;
                this.templateTo(data, 'project-template', 'project-target');
             
            },
            //目标列表
            object: function (data) {
                this.templateTo(data, 'project-template', 'project-target');
            },
            //流程列表
            flot: function (data) {

            }

        },
        //获取批量审核和提交的值
        getValues: function (selector,name) {
            var result = {
                normal: [],
                loop:[]
            }
            $('input[name=' + selector + ']:checked').each(function (n, val) {
                var initial = $(this).parents('.li-check').attr('initial');
                var isLoop = $(this).parents('.li-check').attr('isLoop');
                if (selector == 'checkSingle' && name == 'submitTo') {
                    if (isLoop == 1) {
                        result.loop.push($(this).val())
                    } else {
                        result.normal.push({ planId: $(this).val(), isInitial: initial == 1 ? false : true })
                    }
                    
                } else {
                    if (isLoop == 1) {
                        result.loop.push($(this).val())
                    } else {
                        result.normal.push($(this).val())
                    }
                    
                }
               

            })
            return result;
        },

        //计划详情
        planDetail: function (e) {
            var ele = $(e.currentTarget);
            //var pid = parseInt(ele.attr("pid"), 10);
            
            var isLoop = ele.parents('.li-check').attr('isLoop'); //0为一般计划 1为循坏计划
            var pid = Number(ele.parents('.li-check').attr("pid"));
            var flag =ele.attr('flag');
            //type 1我的计划 2下属计划
            //var type = $('.subTarget').hasClass('workActive') ? 2 : 1;
            if (flag == '4') {
                var type = isLoop == 1 ? 'circlePlan' : 'newPlan';                
                this.addPlan.edit(pid,type,isLoop)
                return
            }
            this.showDetail.show(pid,flag,isLoop);

        },
        //计划审核
        planAudit: function (e) {
            var ele = $(e.currentTarget);
            var values = this.getValues('auditSingle');
            var isLoop = ele.parents('.li-check').attr('isLoop'); //0为一般计划 1为循坏计划
            if ((values.normal && values.normal.length>0) || (values.loop && values.loop.length>0)) {
                //批量审核

                this.showDetail.multi(values)
                return;
            }
            var pid = ele.parents('.li-check').attr('pid');
          
            this.showDetail.show(pid, 1,isLoop);
        },
        //计划提交确认
        planConfirm: function (e) {
            var ele = $(e.currentTarget);
            var pid = ele.parents('.li-check').attr('pid');
            var isLoop = ele.parents('.li-check').attr('isLoop'); //0为一般计划 1为循坏计划
            this.showDetail.show(pid, 3,isLoop);
        },
        //计划确认
        planSubmit: function (e) {
            var ele = $(e.currentTarget);
            var pid = ele.parents('.li-check').attr('pid');
            var isLoop = ele.parents('.li-check').attr('isLoop'); //0为一般计划 1为循坏计划
            this.showDetail.show(pid, 2,isLoop);
        },
        //计划转办
        planTrans: function (e) {
            var ele = $(e.currentTarget);            
            var index = ele.parents('.li-check').index()-1;
            var tp = ele.parents('.li-check').attr('plan')
            var data = targetData[tp][index]
            //type 1我的计划 2下属计划
            //var type = $('.subTarget').hasClass('workActive') ? 2 : 1;
            //var userName = $('.person-show').text();
            //var relative = ele.parents('.operate-list').siblings('.list-audit-content').find('li').eq(3).text();
            //var event = ele.parents('.operate-list').siblings('.list-audit-content').find('li').eq(2).text();
            var msg = {
                confirmUser: data.confirmUserName,
                responseUser:data.responsibleUserName,
                event: data.eventOutput,
                cImg: data.confirmUserImage,
                rImg: data.responsibleUserImage,
                eTime: data.endTime,
                mode: data.executionMode

            }
            var pid = ele.parents('.li-check').attr('pid');
            this.trans.show(pid,msg)
        },
        //计划其他操作
        planOtherOperate: function (e) {
            var ele = $(e.currentTarget);
            var pid = ele.parents('.li-check').attr('pid');
            var initial = ele.parents('.li-check').attr('initial') == 1 ? false : true;
            var isLoop = ele.parents('.li-check').attr('isLoop')
            var name = ele.attr('class');
            var delegateEventSplitter = /^(\S+)\s*(.*)$/;
            name = name.match(delegateEventSplitter);
            var delta = name[1];
            var values = this.getValues('checkSingle', delta);
            pid = isLoop == 1?{loopId:pid,isLoop:isLoop}:{ planId: pid, isInitial: initial ,isLoop:isLoop}       
            if ((values.normal.length > 0 || values.loop.length>0 ) && (delta == 'submitTo' || delta == 'delPlans')) {
                delta = delta == 'submitTo' ? 'planSubmitMulti' : 'planDelMulti';
                pid = values;
               
            }
         
            var type = { 'submitTo': 'planSubmit', 'delPlan': 'planDel', 'modification': 'planEdit', 'suspend': 'planStop', 'planSubmitMulti': 'planSubmitMulti', 'planDelMulti': 'planDelMulti' }[delta];
            this.planOther.init(pid, type)
        },
        changeProgress: function (e) {
            var ele = $(e.currentTarget);
            var pid = parseInt(ele.attr("pid"), 10);
            var progress = parseInt(ele.val(),10);
            $.post('/PlanOperate/UpdateProgress', { planId: pid, progress: progress }, function (data) {
                if (data.success) {
                    ncUnits.alert("修改成功！");
                    ele.val(progress).siblings('small').text(progress + '%');
                    return
                }
                ncUnits.alert("修改失败！");

            },'json')
        },
        downloadList: function (data,ele) {            
            var list = '';
            $.each(data, function (n, val) {
                list += '<li class="down-main ellipsis"><i class="fa fa-eye preview"></i><i class="fa fa-download single-down" attachmentName=' + val.attachmentName + ' saveName=' + val.saveName + ' extension=' + val.extension + '></i>' + val.attachmentName + '</li>';
            })
            var main = "<div class='downList downIndex'><div class='list-head'><a href='javascript:;' class='multi-down'>全部下载</a></div><ul>"+list+"</ul></div>";
            ele.parents('.li-check').find('.first-li').append(main)

        },
        //单个下载
        singleDown:function(e){
            var ele = $(e.currentTarget);
            var attachmentName = ele.attr('attachmentName');
            var saveName = ele.attr('savename');
            $.post("/UserIndex/Download", { displayName: attachmentName, saveName: saveName,flag:0 }, function (data) {
                if (data == "success") {

                    window.location.href = "/UserIndex/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1";
                }
                return;
            });
        },
        //全部下载
        multiDown: function (e) {
            var ele = $(e.currentTarget);
            var planId = ele.parents('.downList').siblings('.show-download').attr('pid')
            $.post("/UserIndex/MultiDownload", { planId: planId, flag: 0 }, function (data) {
                if (data == "success") {
                    window.location.href = "/UserIndex/MultiDownload?planId=" + planId + "&flag=1";
                }
                return;
            });
        },
        showDownload: function (e) {
            e.stopPropagation();
            var ele = $(e.currentTarget);
            var pid = ele.attr("pid");
            var isLoop = ele.parents('.li-check').attr('isLoop')
            this.file.show(pid,ele,isLoop)
            //if (ele.siblings('.downIndex').length > 0) {
            //    $('.downIndex').remove();
            //    return;
            //}
            //$('.downIndex').remove();
            //this.http(this.post['downLoad'].url, {planId:pid},this.downloadList,ele)

        },
        stopUp: function (e) {
            e.stopPropagation();

        },
        //点击计划列表出现操作条
        operate: function (e) {            
            var ele = $(e.currentTarget);
            $('.li-audit').removeClass('schedule-all-to')
            e.stopPropagation();
            var type = this.workAction.getType();
            $('.operate-list').remove();
            $('.downList').remove();
            var offset = ele.offset();
            var relativeX = ((e.pageX - offset.left)-20)+'px';
            var relativeY = ((e.pageY - offset.top)-40)+'px';
            var status = ele.attr('indfy');
            if (type == 1) {
                var result = this.setPlan(status);
            }
            var html = '<div class="operate-list" style="top:'+relativeY+';left:'+relativeX+'"><ul>'+result+'</div>';
            ele.append(html);
                  

        },

        checkall: function (e) {
            var ele = $(e.currentTarget);
            var parent = '.'+ele.attr('class');
            var child = { '.checkAll': '.checkSingle', '.checkAudit': '.auditSingle', '.checkConfirm': 'confirmSingle', '.checkSure': 'sureSingle' }[parent] || '';
            this.checkChild(parent, child);

        },
        checksingle: function (e) {
            var ele = $(e.currentTarget);
            e.stopPropagation();
            var child = '.' + ele.attr('class');
            var parent = { '.checkSingle': '.checkAll', '.auditSingle': '.checkAudit', '.confirmSingle': '.checkConfirm', '.sureSingle': '.checkSure' }[child] || '';
            this.checkParent(parent, child, ele);
            

        },
        isMine: function(e){
            var ele = $(e.currentTarget);
            //if (ele.hasClass('workActive')) {
            //    return
            //}
           
            var isSubordinate = ele.hasClass('myTarget') ? 0 : 1;
            var type = this.workAction.getType()
            var url = type== 1 ? 'initPlan' : 'initObject';
            var func = type == 1 ? this.work.plan : this.work.object;
            this.setActive(type, ele);
            this.http(this.post[url].url, { 'type': isSubordinate }, func)
            
           
            
        },
        //日程日历切换时间
        weekDay: function (e) {
            var ele = $(e.currentTarget);
            if (ele.hasClass('fa-angle-left')) {
                this.shedule.preWeek();
                return;
            }
            this.shedule.nextWeek();
        },
        reback: function (e) {
            e.stopPropagation();
        },
        disapear:function(e){
            var t = setTimeout(function () {
                $('.operate-list').remove();
                clearTimeout(t)
            }, 10)
            //e.stopPropagation();
        },
        //日程选择具体日
        candleClick: function (e) {
            var ele = $(e.currentTarget);
            ele.addClass('choosed').siblings('.candleDate').removeClass('choosed');
            var day = ele.find('li').eq(1).text();
            var date = this.shedule.dateChoose(day, ele);
            this.http('/Calendar/GetDayCalendarInfo', {date:date}, this.init['initSchedule'])
        },
        //日程添加
        addSchedule: function () {
            var time = this.shedule.getSelected();
            delete time.week;
            this.caldener.show(time);
        },
        //修改日程
        editSchedule: function (e) {
            var time = this.shedule.getSelected();
            delete time.week;
            var ele = $(e.currentTarget);
            var idx = ele.index();
            var data = scheduleData.list[idx];
            this.caldener.edit(time,data);
        },
        //删除日程
        delSchedule: function (e) {
            e.stopPropagation();
            var ele = $(e.currentTarget);
            var idx = ele.parent('.schedule-all').index();
            console.log(this.confirm)
            this.confirm.alert({
                title: '是否删除?',
                backdrop: true,
                yes:function() {                    
                    $.post('/Calendar/DelCalendar', { calendarId: ele.parent('.schedule-all').attr('sid') }, function (data) {
                        if (data.success == true) {
                            ele.parent('.schedule-all').remove();
                            scheduleData.list.splice(idx, 1);
                            ncUnits.alert("删除成功！");
                            return
                        }
                        ncUnits.alert("删除失败！");

                    }, 'json')
                }
            });
            

        },
        //绩效排行
        grade: function (data) {
            
            this.templateTo(data, 'grade-template', 'grade-target');
        },
        //绩效和工时切换
        sumtab: function (e) {
            var ele = $(e.currentTarget);
            ele.addClass('tab-large').siblings('span').removeClass('tab-large')
            var type = ele.attr('name');
            var uri = this.post[type].url;
            var result = this.sumTab.typeData(uri, type);
            var func = type == 'initSum' ? this.init['initSum'] : this.grade;
            if (ele.hasClass('sum-grade')) {
                $('.person-tab').show().siblings('.chart-tab').hide();
            } else {
                $('.chart-tab').show().siblings('.person-tab').hide();
            }
            this.http(result.url, { statisticsType: result.param }, func)

        },
        //绩效和工时周月切换
        timetab: function (e) {
            var ele = $(e.currentTarget);
            var type = ele.attr('isMonth');
            ele.addClass('tabActive').siblings('a').removeClass('tabActive');
            var result = this.sumTab.timeMonth(type);
            var func = result.name == 'initSum' ? this.init['initSum'] : this.grade;
            this.http(result.url, { statisticsType: result.param }, func);
        },
        //激励和未完成周月切换
        choose: function (e) {
            var ele = $(e.currentTarget);
            var name = ele.attr('name');
            this.post[name].param = parseInt(ele.attr('isMonth'), 10);
            ele.addClass('tabActive').siblings('a').removeClass('tabActive');
            if (name == 'initTodo') {
                this.http(this.post[name].url, { statisticsType: this.post[name].param }, this.init[name]);
                this.http(this.post['initPer'].url, { statisticsType: this.post[name].param }, this.init['initPer'])
                return
            }
            this.http(this.post[name].url, { statisticsType: this.post[name].param }, this.init[name])
        },

        refreshView:function(){
            var isSubordinate = $('.myTarget').hasClass('workActive') ? 0 : 1;
            this.http(this.post['initPlan'].url, { 'type': isSubordinate }, this.work['plan']);
            if (isSubordinate == 1) return;
            var type = $('#wrapTo .week-tab').hasClass('tabActive') ? 1 : 2;
            this.http(this.post['initTodo'].url, { statisticsType: type }, this.init['initTodo']);
        },

        getscope: function () {
            var scope = this;
            return scope;
        },
        //刷新视图
        refresh: {
            //计划列表刷新
            listRefresh: function () {
                console.log('asasassasasasasasa')
                var isSubordinate = $('.myTarget').hasClass('workActive') ? 0 : 1;
                this.http(this.post['initPlan'].url, { 'type': isSubordinate }, this.work['plan']);
            },
            //未完成事项刷新
            statusRefresh: function () {
                if (!$('.myTarget').hasClass('workActive')) {
                    return;
                }
                var type = $('#wrapTo .week-tab').hasClass('tabActive') ? 1 : 2;
                this.http(this.post['initTodo'].url, { statisticsType: type }, this.init['initTodo']);
            },
            //我的激励刷新
            motivationRefresh: function () {

            },
            //个人工作信息刷新
            personalRefesh: function () {
                this.http(this.post['initPerson'].url, { param: null }, this.init['initPerson']);
            }
        },
        //首页计划，目标，流程切换
        indexTabs: function (e) {
            var ele = $(e.currentTarget);
            ele.parent('li').addClass('selected').siblings('li').removeClass('selected');
            var url, func;
            $('.underLine').animate({ 'left': '0' });
            $('.myTarget').addClass('workActive').siblings('li').removeClass('workActive');
            if (ele.hasClass('index-plan')) {
                url = '/UserIndex/GetNeedToDoPlanList';
                func = this.work.plan;
                this.http(url, { type: 0 }, func);
                $('.myTarget a').css('transform', "rotateY(0deg)").text('我的计划');
                $('.subTarget a').css('transform',"rotateX(0deg)").text('下属计划');
                this.workAction.changeType(1);
                return

            }
            if (ele.hasClass('index-object')) {
                url = '/UserIndex/GetObjectiveList';
                func = this.work.object;
                this.http(url, { type: 0 }, func);
                this.workAction.changeType(2);
                $('.myTarget a').css('transform', "rotateY(360deg)").text('我的目标');
                $('.subTarget a').css('transform', "rotateX(360deg)").text('下属目标');
                return

            }
            if (ele.hasClass('index-flot')) {
                url = '/UserIndex/GetUserFlowCompleteList';
                func = this.work.flot;
                this.http(url, { type: 0 }, func);
                this.workAction.changeType(3);
                return

            }
        },
        //首页初始化展示数据
        show: function () {
            //视图刷新绑定作用域
            for (var i in this.refresh) {
                this.refresh[i] = this.refresh[i].bind(this);
            }

            for (var i in this.work) {
                this.work[i] = this.work[i].bind(this);
            }
            for (var i in this.init) {
                this.init[i] = this.init[i].bind(this)
                if (i == 'initSchedule') {
                    this.http(this.post[i].url, { date:this.post[i].year+'-'+this.post[i].month+'-'+ this.post[i].day }, this.init[i])
                } else {
                    this.http(this.post[i].url, { statisticsType: this.post[i].param }, this.init[i])
                }

            }
            this.shedule.dateInit();
            this.http(this.post['initPlan'].url, { type: this.post['initPlan'].isSubordinate }, this.work.plan, 1);
            this.textData({
                '.li-year': this.nowTime.year + '.' + this.nowTime.month + '.' + this.nowTime.day,
                '.li-weekDay': this.nowTime.week
            })
            return this;
        },

        bind: function () {
            ////刷新绑定
            var re = this.refreshEvent;
            for (var m in this.refreshEle) {
                var eve = re[m].split(',');
                for (var i = 0; i < eve.length; i++) {
                    $(document).off(m,this.refreshEle[m], this.refresh[eve[i]])
                    $(document).on(m,this.refreshEle[m], this.refresh[eve[i]])
                }               
            }
            this.grade = this.grade.bind(this);
            var events = this.events;
            if (!events) {
                return;
            }
            //正则拆分事件字符串
            var delegateEventSplitter = /^(\S+)\s*(.*)$/;
            var k, method, eventName, selector, match;
            for (k in events) {
                method = events[k];
                match = k.match(delegateEventSplitter);
                eventName = match[1];
                selector = match[2];
                method = this[events[k]];
                //将view的this指向事件
                method = method.bind(this);
                this.el.off(eventName, selector)
                this.el.on(eventName, selector, method);
            }
        }
    }

    //var common = new Common();

    return Common



})