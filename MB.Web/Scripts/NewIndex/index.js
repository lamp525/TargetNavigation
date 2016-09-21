/**
 * Created by DELL on 2015/11/30.
 */

//var IndexModule = (function(){
//        //$(document).on('click',function(){
//        //    $('.operate-list').remove();
//        //})
//        //$('.content-detail .li-check').mousedown(function(e){
//        //    if(e.which==3){
//        //        console.log('右键事件');
//        //        $('.operate-list').remove();
//        //        var offset = $(this).offset();
//        //        var relativeX = ((e.pageX - offset.left)-30)+'px';
//        //        var relativeY = ((e.pageY - offset.top)-33)+'px';
//        //        console.log("X: " + relativeX + "  Y: " + relativeY);
//        //        var html = '<div class="operate-list" style="top:'+relativeY+';left:'+relativeX+'"><ul><li></li><li></li><li></li></ul></div>';
//        //        //$('.operate-list').show().css({"top":relativeY,"left":relativeX});
//        //        $(this).append(html);
//        //    }
//        //
//        //
//        //})
//    //$('.content-detail .li-check').on('contextmenu',function(e){
//    //    e.stopPropagation();
//    //    return false;
//    //});
//    //
//    function CommonHttp() {
//        var req;
//        return function (url,post,func) {
//            var url = url,upData = post,func = func,data;
//            //console.log(post)
//            switch (url) {
//                //个人周工时
//                case '/UserIndex/GetUserWorkTimeInfo':
//                    data = {weekTotalWorkTime :46,monthTotalWorkTime:146,yesterdayWorkTime:36,weekAvgWorkTIme:56};
//                    break;
//                    //个人周月激励
//                case '/UserIndex/GetUserIncentiveInfo':
//                    data = post.isMonth == 0?{rewardAmount :'+1246',punishmentAmount:-120}: {rewardAmount :'+3246',punishmentAmount:-20};
//                    break;
//                    //个人文档列表
//                case '/UserIndex/GetCalendarJson':
//                    data = {
//                        list:[
//                            {startTime:"2015-12-3",endTime:"2015-12-10",place:'会议室A',comment:'会议会议',collaboration:[]},
//                            {startTime:"2015-12-3",endTime:"2015-12-10",place:'会议室A',comment:'会议会议',collaboration:[]},
//                            {startTime:"2015-12-3",endTime:"2015-12-10",place:'会议室A',comment:'说到底是',collaboration:[]},
//                            {startTime:"2015-12-3",endTime:"2015-12-10",place:'会议室A',comment:'是滴是滴',collaboration:[]},
//                            {startTime:"2015-12-3",endTime:"2015-12-10",place:'会议室A',comment:'爱啥啥',collaboration:[]}
//                        ]
//                    }
//                    break;
//                    //部门工时列表
//                case  '/UserIndex/GetUserPerformanceRankInfo':
//                    data = {
//                        list:[
//                            {userName:'张翰三',workTime:'25',smallImage:''},
//                            {userName:'王三',workTime:'25',smallImage:''},
//                            {userName:'李四',workTime:'25',smallImage:''},
//                            {userName:'搜索',workTime:'25',smallImage:''},
//                            {userName:'问你',workTime:'25',smallImage:''},
//                            {userName:'李丽',workTime:'25',smallImage:''},
//                            {userName:'王们',workTime:'25',smallImage:''},
//                            {userName:'张翰三',workTime:'25',smallImage:''},
//                            {userName:'张翰三',workTime:'25',smallImage:''},
//                            {userName:'张翰三',workTime:'25',smallImage:''},
//                        ]
//                    }
//                    break;
//                    //折线图
//                case '/UserIndex/GetUserWorkTimeStatistics':

//                    data = post.isMonth == 0?{actualWorkTime:[10,13,11,12,14,12,18],effectiveWorkTime:[9,10,16,18,13,11,20]}:{actualWorkTime:[14,12,20,15,10,4,22],effectiveWorkTime:[29.22,10,16,15,11,15]};
//                    break;
//                case '/UserIndex/GetUserPlanListInfo':
//                    data = {
//                        list: [
//                            { id: 1, userName: '张翰三', workTime: '25', smallImage: '' },
//                            { id: 2, userName: '王三', workTime: '25', smallImage: '' },
//                            { id: 3, userName: '李四', workTime: '25', smallImage: '' },
//                            { id: 4, userName: '搜索', workTime: '25', smallImage: '' },
//                            { id: 5, userName: '问你', workTime: '25', smallImage: '' },
//                            { id: 6, userName: '李丽', workTime: '25', smallImage: '' }
//                        //    { id: 7, userName: '王们', workTime: '25', smallImage: '' },
//                        //    { id: 8, userName: '张翰三', workTime: '25', smallImage: '' },
//                        //    { id: 9, userName: '张翰三', workTime: '25', smallImage: '' },
//                        //    { id: 10, userName: '张翰三', workTime: '25', smallImage: '' },
//                        ]
//                    }
//                    break;


//            }

//            func(data);
//            //req =  $.ajax({
//            //    type: "post",
//            //    url: url,
//            //    data: upData,
//            //    dataType: "json",
//            //    success: function (data) {
//            //        //console.log('sasa',data)
                    
//            //    }
//            //})


//        }
//    }
//    //echarts
//    var Chart = function(){
//        this.myChart = echarts.init(document.getElementById('chart-pic'));
//        this.option = {
//            tooltip : {
//                trigger: 'axis'
//            },
//            legend: {
//                data:['实际工时','评定工时'],
//                y:'bottom'
//            },

//            calculable : false,
//            grid: {x :20 , y:10 ,x2:10 , y2:60},
//            xAxis : [
//                {
//                    type : 'category',
//                    boundaryGap : false,
//                    axisLine : {    // 轴线
//                        show: true,
//                        lineStyle: {
//                            color: '#49A4FE',
//                            width: 2
//                        }
//                    },
//                    data : ['一','二','三','四','五','六','日']
//                }
//            ],
//            yAxis : [
//                {
//                    type : 'value',
//                    axisLabel : {
//                        formatter: '{value}'
//                    },
//                    splitNumber: 10,
//                    axisLine : {    // 轴线
//                        show: true,
//                        lineStyle: {
//                            color: '#49A4FE',
//                            width: 2
//                        }
//                    }
//                }
//            ],
//            series :[]
//        };
//        this.changeData = function(result){
//            this.option.series =  [
//                {
//                    name:'实际工时',
//                    type:'line',
//                    itemStyle: {normal: {areaStyle: {type: 'default'}}},
//                    data:result.actualWorkTime
//                },

//                {
//                    name:'评定工时',
//                    itemStyle: {normal: {areaStyle: {type: 'default'}}},
//                    type:'line',
//                    data:result.effectiveWorkTime
//                }
//            ];
//            this.myChart.setOption(this.option);

//        }
//        window.onresize = this.myChart.resize;
//    }


//    //工时统计和绩效排行切换
//    var SumTab = function(){
//        this.sum = {
//             url:'/UserIndex/GetUserWorkTimeStatistics'
//            ,param:0
//            ,name:'initSum'
//        }
//        this.timeMonth = function(m){
//            this.sum.param = parseInt(m,10);

//            return this.sum;
//        }
//        this.typeData = function(t,name){
//            this.sum.url = t;
//            this.sum.name = name;
//            return this.sum;
//        }

//    }

//    //日程日历
//    var Calendar = function(){
//        var dateArray = [];
//        var until = {
//            getYear:function(that){
//                return parseInt(that.find('li').eq(1).attr('date-year'),10);
//            },
//            getMonth:function(that){
//                return parseInt(that.find('li').eq(1).attr('date-month'),10);
//            },
//            getWeek:function(year,month,day){
//                return new Date(year+'-'+month+'-'+day).getDay();
//            },
//            getDays:function(year,month){
//                var d = new Date(year,month,0);
//                return d.getDate();
//            },
//            setDate:function(temp,year,month,days,that,currentDays,selected){
//                that.find('li').eq(1).removeClass('gray-color');
//                that.removeClass('choosed');
//                var tYear = year;
//                var tMonth = month;
//                if(temp<1 && days){//上年
//                    temp = days+temp;
//                    tYear = tMonth==1?tYear-1:tYear;
//                    tMonth = tMonth==1?12:tMonth-1;
//                    that.find('li').eq(1).addClass('gray-color');
//                };
//                if(temp>currentDays && currentDays){//下年
//                    temp = temp-currentDays;
//                    tYear = tMonth==12?tYear+1:tYear;
//                    tMonth = tMonth==12?1:tMonth+1;
//                    that.find('li').eq(1).addClass('gray-color');
//                };
//                if(selected && selected.year == tYear && selected.month == tMonth && selected.day == temp ){
//                    that.addClass('choosed');
//                }

//               that.find('li').eq(1).html(temp).attr('date-year',tYear).attr('date-month',tMonth);
//            }

//        }
//        this.selected = {
//            year:new Date().getFullYear(),
//            month:new Date().getMonth()+1,
//            day:new Date().getDate(),
//            week:new Date().getDay() == 0?7:new Date().getDay()
//        };//今天为默认选中日程
//        this.dateInit = function(){
//            var choose =  this.selected;
//            var year = choose.year,month = choose.month;
//            var currentDays = until.getDays(year,month);//当前月天数
//            var preDays = until.getDays(month==1?year-1:year,month==1?12:month-1);//上个月天数
//            $('.shedule-month .shedule-year').text(choose.year).next('span').text(choose.month);;
//            var selected = this.selected
//            $('.shedule-day .candleDate').each(function(n){
//                var t = choose.week-n-1;
//                var d = choose.day;
//                var tYear = year;
//                var tMonth = month;
//                dateArray.push(d-t);
//                var temp = dateArray[n];
//                until.setDate(temp,year,month,preDays,$(this),currentDays,selected)
//            })
//        }
//        this.dateChoose = function(day,ele){
//            this.selected.year = until.getYear(ele);
//            this.selected.month = until.getMonth(ele);
//            this.selected.day = parseInt(day,10);
//            this.selected.week = until.getWeek(this.selected.year,this.selected.month,this.selected.day)
//            this.selected.week = this.selected.week == 0?7:this.selected.week;
//            return {year: this.selected.year,month: this.selected.month,day: this.selected.day}
//        }
//        this.preWeek = function(){
//            var selected = this.selected;
//            var year = parseInt($('.shedule-year').text(),10),month=parseInt($('.shedule-month-day').text(),10);
//            var preDays = until.getDays(month==1?year-1:year,month==1?12:month-1);//上个月天数
//            if(Math.min.apply(Math,dateArray)<=1){
//                year = month==1?year-1:year;
//                month = month==1?12:month-1;
//                $('.shedule-year').text(year).next('.shedule-month-day').text(month);
//                dateArray = dateArray.map(function(val){
//                    return preDays+val;
//                })
//            }
//            dateArray = dateArray.map(function(val){
//                return val-7
//            });
//            $('.shedule-day .candleDate').each(function(n){
//                var temp =  dateArray[n];
//                until.setDate(temp,year,month,preDays,$(this),'',selected)
//            })
//        }
//        this.nextWeek = function(){
//            var selected = this.selected;
//            var year = parseInt($('.shedule-year').text(),10),month=parseInt($('.shedule-month-day').text(),10);
//            var currentDays = until.getDays(year,month);//当前月天数
//            if(Math.max.apply(Math,dateArray)>=currentDays){
//                year = month==12?year+1:year;
//                month = month==12?1:month+1;
//                $('.shedule-year').text(year).next('.shedule-month-day').text(month);
//                dateArray = dateArray.map(function(val){
//                    return val-currentDays
//                })
//            }
//            dateArray = dateArray.map(function(val){
//                    return val+7
//            })
//            $('.shedule-day .candleDate').each(function(n){
//                var temp = dateArray[n];
//                until.setDate(temp,year,month,'',$(this),currentDays,selected)
//            })
//        }
//    }

//    //计划，流程，目标
//    var WorkPlan = function(){



//    }



//    //首页计划，目标，流程不变数据
//    var Common = function(){
//        this.events = {
//            'click .month-tab,.week-tab':'choose', //激励和未完成周月切换
//            'click .month-sum-tab,.week-sum-tab':'timetab', //工时和绩效周月切换
//            'click .sum-grade,.sum-time':'sumtab', //绩效和工时切换
//            'click .candleDate':'candleClick', //选择日程日期
//            'click .move-day': 'weekDay',//日期切换
//            'click .index-tabs':'indexTabs'//目标流程计划切换
//        };
//        this.el = $('.main-container,.side-left');
//        this.http = new CommonHttp();
//        this.post = {
//            initPlan: { url: '/UserIndex/GetUserPlanListInfo',isSubordinate:0},//计划列表
//            initPerson:{url:'/UserIndex/GetUserWorkTimeInfo',param:null},//工时信息
//            initTodo:{url:'/UserIndex/GetUserPlanStatusInfo',param:0},//未完成
//            initMotivate:{url:'/UserIndex/GetUserIncentiveInfo',param:0},//激励
//            initSum:{url:'/UserIndex/GetUserWorkTimeStatistics',param:0},//工时统计
//            grade:{url:'/UserIndex/GetUserPerformanceRankInfo',param:0},//绩效排行
//            initSchedule: { url: '/UserIndex/GetCalendarJson', param: null, year: new Date().getFullYear().toString(), month: (new Date().getMonth() + 1).toString(), day: new Date().getDate().toString() } //日程
//        }
//        this.templateTo = function(data,selector,target){
//            var html = template(selector, data);
//            document.getElementById(target).innerHTML = html;
//        }
//        this.sumTab = new SumTab();
//        this.shedule = new Calendar();
//        this.chart = new Chart();

//    }
//    Common.prototype = {
//        init:{
//            //工时模块数据初始化
//            initPerson:function(data){
//                $('.person-week span').text(data.weekTotalWorkTime);
//                $('.person-yesterday span').text(data.yesterdayWorkTime);
//                $('.person-month span').text(data.monthTotalWorkTime);
//                $('.person-average span').text(data.weekAvgWorkTIme);
//            }
//            //未完成事项数据初始化
//            ,initTodo:function(data){

//            }
//            //激励模块数据初始化
//            ,initMotivate:function(data){
//                $('.motivate-num li').eq(0).find('span').text(data.rewardAmount);
//                $('.motivate-num li').eq(1).find('span').text(data.punishmentAmount);
//            }
//            //工时统计模块数据初始化
//            ,initSum:function(data){
//                $('.chart-tab').show().siblings('.person-tab').hide();
//                this.chart.changeData(data);
//            }
//            //日程模块数据初始化
//            ,initSchedule:function(data){
//                this.templateTo(data,'shedule-template','shedule-target');
//            }
//        },

//        work:{
//            //计划列表
//            plan: function (data) {
//                this.templateTo(data, 'project-template', 'project-target');
//            },
//            //目标列表
//            object: function (data) {

//            },
//            //流程列表
//            flot: function (data) {

//            }
        
//        },
//        //日程日历切换时间
//        weekDay:function(e){
//            var ele = $(e.currentTarget);
//            if(ele.hasClass('fa-angle-left')){
//                this.shedule.preWeek();
//                return;
//            }
//            this.shedule.nextWeek();
//        },
//        //日程选择具体日
//        candleClick:function(e){
//            var ele = $(e.currentTarget);
//            ele.addClass('choosed').siblings('.candleDate').removeClass('choosed');
//            var day = ele.find('li').eq(1).text();
//            var date = this.shedule.dateChoose(day,ele);
//            this.http('/UserIndex/GetCalendarJson',date,this.init['initSchedule'])
//        },
//        //绩效排行
//        grade:function(data) {
//            $('.person-tab').show().siblings('.chart-tab').hide();
//            this.templateTo(data,'grade-template','grade-target');
//        },
//        //绩效和工时切换
//        sumtab:function(e) {
//            var ele = $(e.currentTarget);
//            ele.addClass('tabActive tab-large').siblings('span').removeClass('tabActive tab-large')
//            var type = ele.attr('name');
//            var uri = this.post[type].url;
//            var result = this.sumTab.typeData(uri,type);
//            var func = type == 'initSum'?this.init['initSum']:this.grade;
//            this.http(result.url,{isMonth:result.param},func)

//        },
//        //绩效和工时周月切换
//        timetab:function(e){
//            var ele = $(e.currentTarget);
//            var type = ele.attr('isMonth');
//            ele.addClass('tabActive').siblings('a').removeClass('tabActive')
//            var result = this.sumTab.timeMonth(type);
//            var func = result.name == 'initSum'?this.init['initSum']:this.grade;
//            this.http(result.url,{isMonth:result.param},func);
//        },
//        //激励和未完成周月切换
//        choose:function(e){
//            var ele = $(e.currentTarget);
//            var name = ele.attr('name');
//            this.post[name].param = parseInt(ele.attr('isMonth'),10);
//            ele.addClass('tabActive').siblings('a').removeClass('tabActive')
//            this.http(this.post[name].url,{isMonth:this.post[name].param},this.init[name])
//        },
//        indexTabs:function (e) {
//            var ele = $(e.currentTarget);
//            ele.parent('li').addClass('selected').siblings('li').removeClass('selected');
//            var url, func;
//            if (ele.hasClass('index-plan')) {
//                url = '/UserIndex/GetUserPlanListInfo';
//                func = this.work.plan;
//                this.http(url, {isSubordinate:0},func)
//                return
                
//            }
//            if (ele.hasClass('index-object')) {
//                url = '/UserIndex/GetUserObjectListInfo';
//                func = this.work.object;
//                this.http(url, { isSubordinate: 0 }, func)
//                return

//            }
//            if (ele.hasClass('index-flot')) {
//                url = '/UserIndex/GetUserFlotListInfo';
//                func = this.work.flot;
//                this.http(url, { isSubordinate: 0 }, func)
//                return

//            }
//        },
//        //首页初始化展示数据
//        show: function () {
//            for (var i in this.work) {
//                this.work[i] = this.work[i].bind(this);
//            }
//            for(var i in this.init){
//                this.init[i] = this.init[i].bind(this)
//                if( i == 'initSchedule'){
//                    this.http(this.post[i].url,{year:this.post[i].year,month:this.post[i].month,day:this.post[i].day},this.init[i])
//                }else{
//                    this.http(this.post[i].url,{isMonth:this.post[i].param},this.init[i])
//                }

//            }
//            this.shedule.dateInit();
//            this.http(this.post['initPlan'].url, {isSubordinate: this.post['initPlan'].isSubordinate},this.work.plan)
//            return this;
//        },
//        bind:function(){
//            this.grade = this.grade.bind(this);
           
//            var events = this.events;
//            if (!events) {
//                return;
//            }
//            //正则拆分事件字符串
//            var delegateEventSplitter = /^(\S+)\s*(.*)$/;
//            var k, method, eventName, selector, match;
//            for (k in events) {
//                method = events[k];
//                match = k.match(delegateEventSplitter);
//                eventName = match[1];
//                selector = match[2];
//                method = this[events[k]];
//                //将view的this指向事件
//                method = method.bind(this);
//                this.el.off(eventName, selector)
//                this.el.on(eventName, selector, method);
//            }
//        }
//    }
//    var common = new Common();
//    common.show().bind();




//}())


define(['artTemplate', 'echarts', 'echarts/chart/line'], function (template, echarts) {
    function CommonHttp() {
        
        var req;
        return function (url, post, func) {
            var url = url, upData = post, func = func, data;
          
            req =  $.ajax({
                type: "post",
                url: url,
                data: upData,
                dataType: "json",
                success: function (result) {
                    switch (url) {
                        //个人周工时
                        case '/UserIndex/GetUserWorkTimeInfo':
                            data = result.data;
                            break;
                            //个人周月激励
                        case '/UserIndex/GetUserIncentiveInfo':
                            data = post.isMonth == 0 ? { rewardAmount: '+1246', punishmentAmount: -120 } : { rewardAmount: '+3246', punishmentAmount: -20 };
                            break;
                            //个人文档列表
                        case '/UserIndex/GetCalendarJson':
                            data = {
                                list: [
                                    { startTime: "2015-12-3", endTime: "2015-12-10", place: '会议室A', comment: '会议会议', collaboration: [] },
                                    { startTime: "2015-12-3", endTime: "2015-12-10", place: '会议室A', comment: '会议会议', collaboration: [] },
                                    { startTime: "2015-12-3", endTime: "2015-12-10", place: '会议室A', comment: '说到底是', collaboration: [] },
                                    { startTime: "2015-12-3", endTime: "2015-12-10", place: '会议室A', comment: '是滴是滴', collaboration: [] },
                                    { startTime: "2015-12-3", endTime: "2015-12-10", place: '会议室A', comment: '爱啥啥', collaboration: [] }
                                ]
                            }
                            break;
                            //部门工时列表
                        case '/UserIndex/GetUserPerformanceRankInfo':
                            data = {
                                list: [
                                    { userName: '张翰三', workTime: '25', smallImage: '' },
                                    { userName: '王三', workTime: '25', smallImage: '' },
                                    { userName: '李四', workTime: '25', smallImage: '' },
                                    { userName: '搜索', workTime: '25', smallImage: '' },
                                    { userName: '问你', workTime: '25', smallImage: '' },
                                    { userName: '李丽', workTime: '25', smallImage: '' },
                                    { userName: '王们', workTime: '25', smallImage: '' },
                                    { userName: '张翰三', workTime: '25', smallImage: '' },
                                    { userName: '张翰三', workTime: '25', smallImage: '' },
                                    { userName: '张翰三', workTime: '25', smallImage: '' },
                                ]
                            }
                            break;
                            //折线图
                        case '/UserIndex/GetUserWorkTimeStatistics':

                            data = post.isMonth == 0 ? { actualWorkTime: [10, 13, 11, 12, 14, 12, 18], effectiveWorkTime: [9, 10, 16, 18, 13, 11, 20] } : { actualWorkTime: [14, 12, 20, 15, 10, 4, 22], effectiveWorkTime: [29.22, 10, 16, 15, 11, 15] };
                            break;
                        case '/UserIndex/GetUserPlanListInfo':
                            data = {
                                list: [
                                    { id: 1, userName: '张翰三', workTime: '25', smallImage: '' },
                                    { id: 2, userName: '王三', workTime: '25', smallImage: '' },
                                    { id: 3, userName: '李四', workTime: '25', smallImage: '' },
                                    { id: 4, userName: '搜索', workTime: '25', smallImage: '' },
                                    { id: 5, userName: '问你', workTime: '25', smallImage: '' },
                                    { id: 6, userName: '李丽', workTime: '25', smallImage: '' },
                                    { id: 7, userName: '王们', workTime: '25', smallImage: '' },
                                    { id: 4, userName: '搜索', workTime: '25', smallImage: '' },
                                    { id: 5, userName: '问你', workTime: '25', smallImage: '' },
                                    { id: 6, userName: '李丽', workTime: '25', smallImage: '' },
                                    { id: 7, userName: '王们', workTime: '25', smallImage: '' },
                                    { id: 4, userName: '搜索', workTime: '25', smallImage: '' },
                                    { id: 5, userName: '问你', workTime: '25', smallImage: '' },
                                    { id: 6, userName: '李丽', workTime: '25', smallImage: '' },
                                    { id: 7, userName: '王们', workTime: '25', smallImage: '' }

                                ]
                            }
                            break;
                        case '/UserIndex/GetUserPlanStatusInfo':
                            data = result.data;
                            break;



                    }
                    func(data);

                }
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
            grid: { x: 20, y: 10, x2: 10, y2: 60 },
            xAxis: [
                {
                    type: 'category',
                    boundaryGap: false,
                    axisLine: {    // 轴线
                        show: true,
                        lineStyle: {
                            color: '#49A4FE',
                            width: 2
                        }
                    },
                    data: ['一', '二', '三', '四', '五', '六', '日']
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    axisLabel: {
                        formatter: '{value}'
                    },
                    splitNumber: 10,
                    axisLine: {    // 轴线
                        show: true,
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
            this.myChart.setOption(this.option);
            window.onresize = this.myChart.resize;
        }
        
    }


    //工时统计和绩效排行切换
    var SumTab = function () {
        this.sum = {
            url: '/UserIndex/GetUserWorkTimeStatistics'
            , param: 0
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

    //日程日历
    var Calendar = function () {
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
        }
        this.dateChoose = function (day, ele) {
            this.selected.year = until.getYear(ele);
            this.selected.month = until.getMonth(ele);
            this.selected.day = parseInt(day, 10);
            this.selected.week = until.getWeek(this.selected.year, this.selected.month, this.selected.day)
            this.selected.week = this.selected.week == 0 ? 7 : this.selected.week;
            return { year: this.selected.year, month: this.selected.month, day: this.selected.day }
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
            'change .checkAll': 'checkall', //全选
            'change .checkSingle':'checksingle' //单独选择
        };
        this.el = $('.main-container,.side-left');
        this.http = new CommonHttp();
        this.post = {
            initPlan: { url: '/UserIndex/GetUserPlanListInfo', isSubordinate: 0 },//计划列表
            initPerson: { url: '/UserIndex/GetUserWorkTimeInfo', param: null },//工时信息
            initTodo: { url: '/UserIndex/GetUserPlanStatusInfo', param: 0 },//未完成
            initMotivate: { url: '/UserIndex/GetUserIncentiveInfo', param: 0 },//激励
            initSum: { url: '/UserIndex/GetUserWorkTimeStatistics', param: 0 },//工时统计
            grade: { url: '/UserIndex/GetUserPerformanceRankInfo', param: 0 },//绩效排行
            initSchedule: { url: '/UserIndex/GetCalendarJson', param: null, year: new Date().getFullYear().toString(), month: (new Date().getMonth() + 1).toString(), day: new Date().getDate().toString() } //日程
        };
        this.templateTo = function (data, selector, target) {
            var html = template(selector, data);
            document.getElementById(target).innerHTML = html;
        };
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
                return
            }
            $(parent).prop('checked', false);
            $(parent).siblings().addClass('fa-square-o').removeClass('fa-check-square-o')
        };
        this.sumTab = new SumTab();
        this.shedule = new Calendar();
        this.chart = new Chart();
        this.workAction = new WorkAction();

    }
    Common.prototype = {
        init: {
            //工时模块数据初始化
            initPerson: function (data) {
                $('.person-week span').text(data.weekTotalWorkTime);
                $('.person-yesterday span').text(data.yesterdayWorkTime);
                $('.person-month span').text(data.monthTotalWorkTime);
                $('.person-average span').text(data.weekAvgWorkTIme);
            }
            //未完成事项数据初始化
            , initTodo: function (data) {
                console.log('222')
                $('.c_status').text(data.checkingStatus);
                $('.c_audit').text(data.checkedCount);
                $('.c_commit').text(data.submitStatus);
                $('.c_confirm').text(data.confirmingCount);
                var per = (data.allCount - data.notCompleteCount) / data.allCount;
                per = per * 100;
                per = 30;
                if (per <= 50) {
                    $('#rount').css('transform',"rotate(" + 3.6 * per + "deg)");
                    $('#rount2').hide();
                    return

                }
                $('#rount').css('transform', "rotate(180deg)");
                $('#rount2').show().css('transform',"rotate(" + 3.6 * (per - 50) + "deg)");

            }
            //激励模块数据初始化
            , initMotivate: function (data) {
                $('.motivate-num li').eq(0).find('span').text(data.rewardAmount);
                $('.motivate-num li').eq(1).find('span').text(data.punishmentAmount);
            }
            //工时统计模块数据初始化
            , initSum: function (data) {
                $('.chart-tab').show().siblings('.person-tab').hide();
                this.chart.changeData(data);
            }
            //日程模块数据初始化
            , initSchedule: function (data) {
                this.templateTo(data, 'shedule-template', 'shedule-target');
            }
        },

        work: {
            //计划列表
            plan: function (data) {
                this.templateTo(data, 'project-template', 'project-target');
            },
            //目标列表
            object: function (data) {

            },
            //流程列表
            flot: function (data) {

            }

        },

        operate: function () {
            var type = this.workAction.getType();
            

        },

        checkall: function (e) {
            var ele = $(e.currentTarget);
            var parent = '.'+ele.attr('class');
            var child = { '.checkAll': '.checkSingle' }[parent] || '';
            this.checkChild(parent, child);

        },
        checksingle: function (e) {
            var ele = $(e.currentTarget);
            var child = '.' + ele.attr('class');
            var parent = { '.checkSingle': '.checkAll' }[child] || '';
            this.checkParent(parent, child,ele);

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
        //日程选择具体日
        candleClick: function (e) {
            var ele = $(e.currentTarget);
            ele.addClass('choosed').siblings('.candleDate').removeClass('choosed');
            var day = ele.find('li').eq(1).text();
            var date = this.shedule.dateChoose(day, ele);
            this.http('/UserIndex/GetCalendarJson', date, this.init['initSchedule'])
        },
        //绩效排行
        grade: function (data) {
            $('.person-tab').show().siblings('.chart-tab').hide();
            this.templateTo(data, 'grade-template', 'grade-target');
        },
        //绩效和工时切换
        sumtab: function (e) {
            var ele = $(e.currentTarget);
            ele.addClass('tabActive tab-large').siblings('span').removeClass('tabActive tab-large')
            var type = ele.attr('name');
            var uri = this.post[type].url;
            var result = this.sumTab.typeData(uri, type);
            var func = type == 'initSum' ? this.init['initSum'] : this.grade;
            this.http(result.url, { isMonth: result.param }, func)

        },
        //绩效和工时周月切换
        timetab: function (e) {
            var ele = $(e.currentTarget);
            var type = ele.attr('isMonth');
            ele.addClass('tabActive').siblings('a').removeClass('tabActive');
            var result = this.sumTab.timeMonth(type);
            var func = result.name == 'initSum' ? this.init['initSum'] : this.grade;
            this.http(result.url, { isMonth: result.param }, func);
        },
        //激励和未完成周月切换
        choose: function (e) {
            var ele = $(e.currentTarget);
            var name = ele.attr('name');
            this.post[name].param = parseInt(ele.attr('isMonth'), 10);
            ele.addClass('tabActive').siblings('a').removeClass('tabActive');
            this.http(this.post[name].url, { isMonth: this.post[name].param }, this.init[name])
        },
        indexTabs: function (e) {
            var ele = $(e.currentTarget);
            ele.parent('li').addClass('selected').siblings('li').removeClass('selected');
            var url, func;
            if (ele.hasClass('index-plan')) {
                url = '/UserIndex/GetUserPlanListInfo';
                func = this.work.plan;
                this.http(url, { isSubordinate: 0 }, func);
                this.workAction.changeType(1);
                return

            }
            if (ele.hasClass('index-object')) {
                url = '/UserIndex/GetObjectiveList';
                func = this.work.object;
                this.http(url, { isSubordinate: 0 }, func);
                this.workAction.changeType(2);
                return

            }
            if (ele.hasClass('index-flot')) {
                url = '/UserIndex/GetUserFlowCompleteList';
                func = this.work.flot;
                this.http(url, { isSubordinate: 0 }, func);
                this.workAction.changeType(3);
                return

            }
        },
        //首页初始化展示数据
        show: function () {
            for (var i in this.work) {
                this.work[i] = this.work[i].bind(this);
            }
            for (var i in this.init) {
                this.init[i] = this.init[i].bind(this)
                if (i == 'initSchedule') {
                    this.http(this.post[i].url, { year: this.post[i].year, month: this.post[i].month, day: this.post[i].day }, this.init[i])
                } else {
                    this.http(this.post[i].url, { isMonth: this.post[i].param }, this.init[i])
                }

            }
            this.shedule.dateInit();
            this.http(this.post['initPlan'].url, { isSubordinate: this.post['initPlan'].isSubordinate }, this.work.plan);
            return this;
        },
        relaese: function () {



        },
        bind: function () {
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