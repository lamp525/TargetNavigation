require.config({
    baseUrl: '../../../',
    paths: {
        //"jquery": 'Libs/jQuery/jquery-1.11.2',
        "text": 'Libs/text/text',
        "echarts": 'Libs/echarts/dist',
        "artTemplate": "Libs/artTemplate/template",
        
        "jqueryUi": "Libs/jQuery-ui/jquery-ui",
        'imgareaselect': "Scripts/imgareaselect/jquery.imgareaselect.pack",
        'message': 'Libs/messenger/js/messenger',
        
        //"popup": "Scripts/component/searchPopup",
        'director': 'Libs/director/director.min',
        'text': 'Libs/text/text',
        'bootstrap': 'Libs/bootstrap/js/bootstrap.min',
        
        'bootstrap-hover': 'Libs/bootstrap-hover-dropdown/bootstrap-hover-dropdown',
        'layer': 'Libs/layer/layer',
        'ztree': 'Libs/zTree/js/jquery.ztree.all-3.5.min',
        'pagewalkthrough': 'Libs/jquery-pagewalkthrough/jquery.pagewalkthrough',
        'router': 'Scripts/Scripts/Router/router',
        'fileupload': 'Libs/jqFileUpload/js/jquery.fileupload',
        'widget': 'Libs/jqFileUpload/js/vendor/jquery.ui.widget',


        "planold": "Scripts/Scripts/NewIndex/plan",
        "common": "Scripts/Scripts/Common/common",
        "dialog": "Scripts/Scripts/Dialog/dialog",
        "index": "Scripts/Scripts/NewIndex/index",
        "version": "Scripts/Scripts/versions/versions",
        "plan": "Scripts/Scripts/Plan/plan",
        "units": "Scripts/common/units",
        "inits": "Scripts/common/init",
        "addPlan": "Scripts/plan/addPlan"
    },
     urlArgs: "v=" + (new Date()).getTime() 

})
define(['dialog', 'router', 'bootstrap', 'pagewalkthrough','common'], function (dialog) {
    var planCreate;//计划添加
    var icon = new dialog.icon(); //头像添加
    var pwd = new dialog.pwd(); //头像添加
    var planNew = new dialog.addplan();//新新建计划
    var http = function (url, func) {
        var url = url, func = func;
        var req;
        req = $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            success: rsHandler(function (data) {
                func(data)

            })
        })
    }
    var events = {
        'click .nav-li': navTab, //新导航切换
        'click .old-anchor>a,.oldTo': oldNavTab, //旧版本链接
        'click #help_guide': guide, //引导
        'click .addPlan': addPlans,//新建计划
        'click .person-quit': quit,//退出
        'click #change_position': changePosition,//岗位切换显示
        'mouseleave .main-info':dismiss,
        'click .choose-position': updatePosition,//岗位切换
        'click #edit_pwd': editPwd,//密码修改
        'click .person-icon': editIcon, //修改头像
        'click .color-swatch': changeTheme, //主题切换
        'click .addNew': newCircle, //新循环计划
    };

    var initUrl = {
        'personInfo': '/XXXViews/User/GetHeadUserInfo'
    }
    var initShow = {
        'personInfo': function (data) {
            //var position = data.userOrgStation[0].userOrgStationName.split('-');
            console.log('main', data)

            $('.person-name span,.person-show').text(data.userName);
            $('.person-position span').text(data.stationName);
            $('.person-department span').text(data.orgName);
            //$('.position-present').text(data.userOrgStation[0].userOrgStationName);
            //data.userOrgStation.push({ userOrgStationName: '产品经理-目标导航事业部' }, { userOrgStationName: '开发-目标导航事业部' })
            $('.position-present').text(data.stationName+'-'+data.orgName)
            if (data.headImage) {
                $('.imgPerson').attr('src', data.headImage + '?' + Date.now())
            }
            
        }

    }
    var init = function () {
        for (var i in initUrl) {
            http(initUrl[i], initShow[i])
        }

    }()
    $('.nav-li').off('hover');
    $('.nav-li').hover(function () {
        $(this).addClass('nav-active').siblings().removeClass('nav-active');
        navPosition($(this));
    }, function () {
        var ele = $('.current');
        ele.addClass('nav-active').siblings().removeClass('nav-active');
        navPosition(ele);
    })
    function navPosition(ele,type) {
        var focus = $('.focus');
        var width = ele.outerWidth();
        var left = ele.position().left;
        var focusLeft = focus.position().left;
        if (focusLeft == left) return
        if ($('.nav-active').index() == 0) {
            $('.nav-active').siblings('.nav-li').removeClass('lineShow');
        } else {
            $('.nav-active').prev().addClass('lineShow').siblings('.nav-li').removeClass('lineShow');
        }
        
        var set = focusLeft > left ? -20 : 20;
        focus.stop().animate({ 'width': width + 'px', 'left': (left + set) + 'px' }, 150, function () {
            focus.css('left', left + 'px');
         
            
        })

    }

    var bindEvents = function () {

        var delegateEventSplitter = /^(\S+)\s*(.*)$/;
        var k, method, eventName, selector, match;
        var el = $(document);
        for (k in events) {
          
            method = events[k];
            match = k.match(delegateEventSplitter);
            eventName = match[1];
            selector = match[2];
            method = events[k];
            el.off(eventName, selector)
            el.on(eventName, selector, method)

        }

    }();


    //function newPlan(e) {
    //    planNew.show('newPlan');
    //}
    function newCircle(e) {
        planNew.show('circlePlan');

    }

    function editIcon() {
        icon.show();
    }

    function editPwd() {
        pwd.show();
    }

    function quit() {
        window.location.href = "/";
    }

    function dismiss() {
        $('.position-change').hide();
    }

    function navTab(e) {
        var ele = $(e.currentTarget);
        ele.addClass('nav-active current').siblings('.nav-li').removeClass('nav-active current')
    }

    function oldNavTab(e) {
        var href = $(this).attr("href");
        sessionStorage.setItem("currentUrl", href);

        location.href = "/Shared/HomeIndex";
        return false
    }

    function changePosition(e) {
        $('.position-change').toggle();
        if (!$('.position-change').is(':hidden')) {
           
            $.post('/XXXViews/User/GetUserStationInfo', {}, function (data) {
                var li = '';
                $.each(data.data, function (n, val) {
                    li += '<li class="choose-position" oid=' + val.orgId + ' sid=' + val.stationId + '>' + val.stationName + '-' + val.orgName + '</li>';
                })
                $('.position-drop ul').html(li);

            }, 'json')

        }

    }
    function updatePosition() {
        var name = $(this).text();
        var position = name.split('-');
        var post = {
            stationId: $(this).attr('sid'),
            orgId: $(this).attr('oid'),
            stationName: position[0],
            orgName: position[1]
        }
        $('.position-present').text(name);
        $('.person-position span').text(position[0]);
        $('.person-department span').text(position[1]);
        $.post('/XXXViews/User/ChangeStation', { data: JSON.stringify(post) }, function () {

        })

    }

    function addPlans(e) {       
        //if(!planCreate){
        //    planCreate = addPlan();
        //}   
        //planCreate.addPlan(); 
        planNew.show('newPlan')
    }


    function changeTheme(e){
        var $ele = $(e.currentTarget),
            $class = $("#change_theme"),
            theme = $ele.attr("theme");

        $ele.addClass("selected").siblings(".selected").removeClass("selected");
        $class.attr("href", "../../Styles/Styles/themes/" + theme + ".css");

        localStorage.setItem("ncTheme",theme);
    }

    //引导
    function guide() {
        $("body").pagewalkthrough({
            name: 'guider',
            steps: [{
                wrapper: '#sn-personal',
                popup: {
                    content: '个人主页中待办事项的分类在此处选择,试一下吧！',
                    type: 'tooltip',
                    position: 'right'
                }
            }, {
                wrapper: '#statistics',
                popup: {
                    content: '工时统计,日/周/月以及平均工时一览！',
                    type: 'tooltip',
                    position: 'bottom'
                }
            }, {
                wrapper: '#wrapTo .todo-status',
                popup: {
                    content: '计划状态,点击可直接跳转计划页面哦！',
                    type: 'tooltip',
                    position: 'bottom'
                }
            }, {
                wrapper: '.motivation .more-motivate',
                popup: {
                    content: '激励入口,点击进入激励模块，方便快捷！',
                    type: 'tooltip',
                    position: 'left'
                }
            }, {
                wrapper: '.sum-top',
                popup: {
                    content: '工时统计、部门绩效排行榜点击切换！',
                    type: 'tooltip',
                    position: 'left'
                }
            }, {
                wrapper: '#project-target .x1',
                popup: {
                    content: '要处理的待办事项太多怎么办？不用担心，有批量功能帮你搞定！',
                    type: 'tooltip',
                    position: 'right'
                }
            }, {
                wrapper: '.li-check',
                popup: {
                    content: '想要审核、提交任务？点击即可快速找到你想要的操作！',
                    type: 'tooltip',
                    position: 'top'
                }
            }, {
                wrapper: '#planlist_checked .range-li',
                popup: {
                    content: '计划进度显示，已审核计划进程，可手动更改状态！',
                    type: 'tooltip',
                    position: 'left'
                }
            }, {
                wrapper: '.myschedule',
                popup: {
                    content: '你的日程我的牵绊,今日日程一键查看！',
                    type: 'tooltip',
                    position: 'left'
                }
            }],
            onClose: function () {
                localStorage.setItem("guider", true);
            }
        });
        $("body").pagewalkthrough('show');
    }

    var guided = localStorage.getItem("guider");

    if (!guided) {
        guide();
    }

    var ncTheme = localStorage.getItem("ncTheme");
    console.log('saasass', ncTheme)
    if (ncTheme) {
        $(".color-swatch[theme='" + ncTheme + "']").click();
    } else {
        $(".color-swatch:last").click();
    }
    
})

