define(['director'], function (template) {
    function showNav(idx) {
        $('.nav-li').eq(idx).addClass('nav-active current').siblings('.nav-li').removeClass('nav-active current');
        var width = $('.nav-li').eq(idx).outerWidth();
        var left = $('.nav-li').eq(idx).position().left;
        $('.focus').css({ 'left': left + 'px', 'width': width + 'px' });
        if (idx == 0) {
            $('.nav-active').siblings('.nav-li').removeClass('lineShow');
        } else {
            $('.nav-active').prev().addClass('lineShow').siblings('.nav-li').removeClass('lineShow');
        }

    }
    
    var index = function () {
        showNav(0)
        $('#main-context').load("/XXXViews/UserIndex/Index", function () {
            require(['index'], function (index) {
                var common = new index();
                common.show();
                planRefresh = common;
                $(document).one('ajaxStop', function () { common.bind() });
                $('.open-child li').eq(0).addClass('selected').siblings('li').removeClass('selected');
            })
        })
    }
    var object = function () {
        showNav(1)
        $('.main-container').load("/ObjectiveIndex/ObjectiveIndex", function () {
                        
        })
    }
    var version = function () {
        $('#main-context').load("/Version/GetVersionList", function () {
            require(['version'], function (version) {
                version.render();
            })
        })
    }
    var plan = function () {
        showNav(3)
        $('#main-context').load("/XXXViews/Plan/PlanIndex", function () {
            require(['plan'], function (Plan) {
                new Plan().render();
            })
        })
    }

    var routes = {
        '/index': index,
        //'/object':object
        '/version': version,
        '/plan': plan
    }
    var router = Router(routes);
    router.init("/index");
})