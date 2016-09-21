define( ['director'], function (template) {
    var index = function () {
        $('.main-container').load("/Index/UserIndex", function () {
            require(['index'], function (index) {
                var common = new index();
                common.show().bind();
                $('.newNav').eq(0).addClass('active').siblings('.newNav').removeClass('active');
                $('.open-child li').eq(0).addClass('selected').siblings('li').removeClass('selected');

            })
        })
    }
    var object = function () {
        $('.main-container').load("/ObjectiveIndex/ObjectiveIndex", function () {
            $('.newNav').eq(1).addClass('active').siblings('.newNav').removeClass('active');
        })

    }
    var routes = {
        '/index': index,
        '/object':object


    }
    var router = Router(routes);
    router.init('/index');



})