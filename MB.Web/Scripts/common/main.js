require.config({
    baseUrl:'../../',
    paths: {
        "text":'Libs/text/text',
        "echarts":'Libs/echarts/dist',
        "artTemplate": "Libs/artTemplate/template",
        "index": "Scripts/NewIndex/index",
        'director': 'Libs/director/director.min',
        'text': 'Libs/text/text',
        'router': 'Scripts/Router/router'
    }

})

require(['router'], function (router) {
    var events = {
        'click .newNav': navTab, //新导航切换
    };

    var bindEvents = function () {

        var delegateEventSplitter = /^(\S+)\s*(.*)$/;
        var k, method, eventName, selector, match;
        var el = $('.navbar');
        for (k in events) {
            method = events[k];
            match = k.match(delegateEventSplitter);
            eventName = match[1];
            selector = match[2];
            method = events[k];
            el.on(eventName, selector, method)

        }

    }()


    function navTab(e) {
        var ele = $(e.currentTarget);
        ele.addClass('active').siblings('.newNav').removeClass('active')
    }

})

