//@ sourceURL=index-company.js
/**
 * Created by DELL on 15-9-16.
 */
$(function () {

    function newsBannerRander() {
        $.ajax({
            type: "post",
            //url: "../../test/data/index/index_company_bannerData.json",
            url: "/Company/AjaxImageNewsData",
            dataType: "json",
            data: {
                count: 5
            },
            success: rsHandler(function (data) {
                console.log(data);
                var gettpl = $("#tpl_banner").html();
                laytpl(gettpl).render(data, function (html) {
                    $("#banner").html(html);
                });

                //事件
                banner();
            })
        });
    }


    /* 新闻图片轮播 - 结束*/

    /* 新闻列表加载 - 开始*/
    //html写入
    //data = [[$id,$text,$time,$aUrl],[]...]
    //var newsData = [
    //    [1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //    ,[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //    ,[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //];


    //function newsListRander() {
    //    $.ajax({
    //        type: "post",
    //        url: "../../test/data/index/index_company_newsData.json",
    //        dataType: "json",
    //        data: {
    //            count: 8
    //        },
    //        success: rsHandler(function (data) {
    //            var gettpl = $("#tpl_link_list").html();
    //            laytpl(gettpl).render(data, function (html) {
    //                $("#news_list").html(html);
    //            });
    //        })
    //    });
    //};

    /* 新闻列表加载 - 结束*/

    $(".newsContainer .refresh").click(function () {
        newsBannerRander();
        //newsListRander();
    });
    newsBannerRander();
    //newsListRander();

    /* 通知列表加载 - 开始*/
    //html写入
    //data = [[$text,$time,$aUrl],[]...]
    //var messageData = [
    //    [1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //    ,[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //    ,[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //];

    //function messageRander() {
    //    $.ajax({
    //        type: "post",
    //        url: "../../test/data/index/index_company_messageData.json",
    //        dataType: "json",
    //        data: {
    //            count: 9
    //        },
    //        success: rsHandler(function (data) {
    //            var gettpl = $("#tpl_link_list").html();
    //            laytpl(gettpl).render(data, function (html) {
    //                $("#message_list").html(html);
    //            });
    //        })
    //    });
    //}

    //var gettpl = $("#tpl_link_list").html();

    /* 通知列表加载 - 结束*/

    //$(".message .refresh").click(messageRander);
    //messageRander();

    /* 文档列表加载 - 开始*/
    //html写入
    //data = [[$text,$time,$aUrl],[]...]
    //var docuData = [
    //    [1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //    ,[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //    ,[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"],[1,"孙子","2011-11-11","#"]
    //];

    function docuRander() {
        var lodi = getLoadingPosition('.docuInfor');
        $.ajax({
            type: "post",
            //url: "../../test/data/index/index_company_docuData.json",
            url: "/Company/AjaxDocumentData",
            dataType: "json",
            data: {
                count: 9
            },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) { 
                var gettpl = $("#tpl_docu_list").html();
                laytpl(gettpl).render(data, function (html) {
                    $("#docu_list").html(html);
                });
                $('.docuInfor ul li').hover(
                    function () {
                        $('.liHover', this).toggle();
                    }
                );
            })
        });
    }

    /* 文档列表加载 - 结束*/

    $(".docuInfor .refresh").click(docuRander);
    docuRander();

    /* 文档中心列表jq 开始
        $('.docuInfor ul li').hover(
            function () {
                    $('.liHover',this).animate({'top':'0px','height':'30px'},500);
            },
            function () {
                $('.liHover',this).animate({'top':'30px','height':'0px'},500);
            }
        ); */
    /* 文档中心列表jq 结束 */
    /* 文档中心列表jq 开始
        $('.docuInfor ul li').hover(
            function () {
                if ( $('.liHover input',this).val()=='ture' ) {
                    $(this).addClass('liHit');
                    $('.liHit .liHover').animate({'top':'0px','height':'30px'},500);
                }
            },
            function () {
                if ( $('.liHover input',this).val()=='ture' ) {
                    $('.liHit .liHover input').val('false');
                    //$(this).addClass('liHit');
                    $('.liHit .liHover').animate({'top':'30px','height':'0px'},500);
                    setTimeout ( function () {
                        //alert( $(this).html() );
                        $('.liHit .liHover input').val('ture');
                        $('.liHit').removeClass('liHit');
                    } , 1000);
                }
            }
        ); */
    /* 文档中心列表jq 结束 */
    /* 文档中心列表jq 开始 */

    /* 文档中心列表jq 结束 */
    /* 文档中心列表jq 开始
        $('.docuInfor ul li').hover(
            function () {
                //alert(0);
                var length = $('.liHover').length;
                //alert(2);
                var one = true;
                //alert(1);
                for ( var i=0;i<length;i++ ) {
                    if ( $('.docuInfor ul li .liHover input').val()=='false' ) {
                        one = false;
                    }
                }
                //alert(one);
                if ( one==true ) {
                    //alert(one);
                    $(this).addClass('liHit');
                    $('.liHit .liHover input').val('false');
                    setTimeout ( function () {
                        $('.liHit .liHover').slideDown(500);
                        $('.liHit .liHover input').val('true');
                    } , 200);
                }
            },
            function () {
                $('.liHit .liHover').slideUp(500);
                $('.liHit').removeClass('liHit');
                //alert(1);
            }
        ); */
    /* 文档中心列表jq 结束 */



    // Graph Data ##############################################
    //var graphData = [{
    //		// Visits
    //		data: [ [1, 168, 1], [2, 160, 2], [3, 170, 3], [4, 160, 5], [5, 165, 7], [6, 130, 90], [7, 160, 1], [8, 190, 1], [9, 130, 1], [10, 140, 1], [11, 160, 1], [12, 150, 1] ],
    //		color: '#eb2d2c',
    //		points: { radius: 4, fillColor: '#eb2d2c' }
    //	}, {
    //		// Returning Visits
    //		data: [ [1, 130], [2, 130], [3, 160], [4, 150], [5, 160], [6, 50], [7, 60], [8, 55], [9, null], [10, null], [11, null], [12, null] ],
    //		color: '#ef7b27',
    //		points: { radius: 4, fillColor: '#ef7b27' }
    //	}, {
    //		// Returning Visits
    //		data: [ [1, 110], [2, 160], [3, 130], [4, 140], [5, 135], [6, 40], [7, 70], [8,150], [9, 80], [10, 70], [11, 60], [12, 40] ],
    //		color: '#fbac0f',
    //		points: { radius: 4, fillColor: '#fbac0f' }
    //	}
    //];

    // Lines Graph #############################################
    //$.plot($('#graph-lines'), graphData, {
    //	series: {
    //		points: {
    //			show: true
    //			//radius: 5//第一条线的圈大小
    //		},
    //		lines: {
    //			show: true//点间连接线
    //		},
    //		shadowSize: 0
    //	},
    //	grid: {
    //		color: '#58b456',
    //		borderColor: 'transparent',
    //		borderWidth: 20,
    //		hoverable: true
    //	},
    //	// 横坐标
    //	xaxis: {
    //		ticks:[1,2,3,4,5,6,7,8,9,10,11,12],
    //		tickFormatter: function(axis) {
    //           return axis.toString()+"月";
    //       },
    //		tickColor: 'transparent',
    //		tickDecimals: 0//小数点
    //	},
    //	// 纵坐标
    //	yaxis: {
    //		tickSize: 50//纵坐标每一格差值
    //	}
    //});

    // Bars Graph ##############################################
    //$.plot($('#graph-bars'), graphData, {
    //	series: {
    //		bars: {
    //			show: true,
    //			barWidth: .9,
    //			align: 'center'
    //		},
    //		shadowSize: 0
    //	},
    //	grid: {
    //		color: '#646464',
    //		borderColor: 'transparent',
    //		borderWidth: 20,
    //		hoverable: true
    //	},
    //	xaxis: {
    //		ticks:[1],
    //		tickColor: 'transparent',
    //		tickDecimals: 0
    //	},
    //	yaxis: {
    //		tickSize: 1000
    //	}
    //});

    // Graph Toggle ############################################
    //$('#graph-bars').hide();

    //$('#lines').on('click', function (e) {
    //	$('#bars').removeClass('active');
    //	$('#graph-bars').fadeOut();
    //	$(this).addClass('active');
    //	$('#graph-lines').fadeIn();
    //	e.preventDefault();
    //});
    //
    //$('#bars').on('click', function (e) {
    //	$('#lines').removeClass('active');
    //	$('#graph-lines').fadeOut();
    //	$(this).addClass('active');
    //	$('#graph-bars').fadeIn().removeClass('hidden');
    //	e.preventDefault();
    //});


    /* 折线图 结束 */
    var statistics_type = 0,
        statistics_mode = 0,
        date = new Date(),
        thisyear = year = date.getFullYear(),
        thismonth = month = date.getMonth() + 1,
        $cur = $("#curTime"),
        $last = $(".LMonth.last"),
        $next = $(".LMonth.next");

    $("#statistics_title ul li a").click(function () {
        statistics_type = $(this).attr("value");
        $("#statistics_title .TOne").html($(this).html());
        chartRender();
    });


    /* 年月功效 开始 */

    $cur.html(year+"年");
    $last.html(year - 1);
    $next.html(year + 1);
    chartRender();


    $(".statistics_check_mode").click(function () {
        $(this).siblings(".YMHit").removeClass("YMHit");
        $(this).addClass("YMHit");
        statistics_mode = $(this).attr("value");
        timeRender();
        chartRender();
    });

    //last
    $last.click(function () {
        if (statistics_mode == 0) {
            year--;
        } else {
            if (month == 1) {
                month = 12;
                year--;
            } else {
                month--;
            }
        }
        timeRender();
        chartRender();
    });
    //next
    $next.click(function () {
        if ($(this).hasClass("disabled")) {
            return;
        }
        if (statistics_mode == 0) {
            year++;
        } else {
            if (month == 12) {
                month = 1;
                year++;
            } else {
                month++;
            }
        }
        timeRender();
        chartRender();
    });
    //$('.effect .year').click( function () {
    //	$('.effect .month').removeClass('YMHit');
    //	$(this).addClass('YMHit');
    //   statistics_mode = 0;
    //});
    //$('.effect .month').click( function () {
    //	$('.effect .year').removeClass('YMHit');
    //	$(this).addClass('YMHit');
    //   statistics_mode = 1
    //});

    /*
	$('.TOP .year').addClass('YMHit');
	$('.TOP .year').click( function () {
		$('.TOP .month').removeClass('YMHit');
		$(this).addClass('YMHit');
	});
	$('.TOP .month').click( function () {
		$('.TOP .year').removeClass('YMHit');
		$(this).addClass('YMHit');
	});
	*/
    /* 年月功效切换 结束 */

    $(".effect .refresh").click(function () {
        
        chartRender();
        
        timeRender();
        
    })

    /* 折线图 开始 */
    function chartRender() {
        console.log("绘图");
        //var charurl;
        //if (statistics_type == 0) {
        //    charurl = "../../test/data/index/index_company_docuData.json";
        //} else if (statistics_type == 1) {
        //    charurl = "../../test/data/index/index_company_docuData.json";
        //} else {
        //    charurl = "../../test/data/index/index_company_docuData.json";
        //}
        var lodi2 = getLoadingPosition('#worktime');
        $.ajax({
            type: "post",
            url: "/worktimeIndex/getJson",
            dataType: "json",
            data: {
                mode: statistics_mode,
                year: year,
                month: month
            },
            complete: rcHandler(function () {
                lodi2.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                //    top3:[]
                //}

                //for(var j = 0;j<3;j++){
                //    var z = [];
                //    var len = (statistics_mode == 0 ? 12 : 30);
                //    for(var i = 1; i <= len; i++){
                //        z.push([i,Math.floor(Math.random()*200),Math.floor(Math.random()*200)]);
                //    }
                //    data.top3.push(z);
                //}

                /* TOP10加载 开始 */
                var gettpl = $("#tpl_top10_list").html();
                laytpl(gettpl).render(data.top10, function (html) {
                    $("#top10_list").html(html);
                });

                $('.docuInfor ul li').hover(function () {
                    $('.liHover', this).show();
                }, function () {
                    $('.liHover', this).hide();
                }
                );
                /* TOP10加载 开始 */

                /* 折线图绘制 开始 */

                var colors = ['#eb2d2c', '#ef7b27', '#fbac0f'];

                var graphData = [];
                if (data.top3 != null) {
                    for (var i = 0, len = Math.min(3, data.top3.length) ; i < len; i++) {
                        var topi = []
                            , d = data.top3[i];
                        for (var j = 0, slen = d.length; j < slen; j++) {
                            topi.push([d[j].workdate, d[j].totaleffective, d[j].totalwork]);
                        }
                        graphData.push({
                            data: topi,
                            color: colors[i],
                            points: { radius: 4, fillColor: colors[i] }
                        });
                    }
                }


                //var graphData = [{
                //    // Visits
                //    data: data.top3[0],
                //    color: '#eb2d2c',
                //    points: { radius: 4, fillColor: '#eb2d2c' }
                //}, {
                //    // Returning Visits
                //    data: data.top3[1],
                //    color: '#ef7b27',
                //    points: { radius: 4, fillColor: '#ef7b27' }
                //}, {
                //    // Returning Visits
                //    data: data.top3[2],
                //    color: '#fbac0f',
                //    points: { radius: 4, fillColor: '#fbac0f' }
                //}
                //];
                $.plot($('#graph-lines'), graphData, {
                    series: {
                        points: {
                            show: true
                            //radius: 5//第一条线的圈大小
                        },
                        lines: {
                            show: true//点间连接线
                        }
                        //,shadowSize: 3
                    },
                    grid: {
                        color: '#000',
                        borderColor: 'transparent',
                        //borderWidth: 15,
                        hoverable: true,
                        labelMargin: 10
                        //minBorderMargin:10
                    },
                    // 横坐标
                    xaxis: {
                        ticks: statistics_mode == 0 ? [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] : [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31]
                        //tickFormatter: function (axis) {
                        //    return axis.toString() + (statistics_mode == 0 ? "月" : "日");
                        //}
                        , labelWidth: 28
                        , tickColor: 'transparent'
                        //,tickDecimals: 0//小数点
                        //,tickLength:100
                    }
                    // 纵坐标
                    , yaxis: {
                        tickDecimals: 0,
                        labelWidth: 30,
                        ticks: statistics_mode == 0 ? [0, 100, 200, 300, 400, 401] : [0, 5, 10, 15, 20, 20.01],
                        min: 0,
                        transform: function (v) {
                            var val;
                            if (statistics_mode == 0) {
                                if (v <= 400) {
                                    val = v;
                                } else {
                                    val = 500 - 40000 / v;
                                }
                            } else {
                                if (v <= 20) {
                                    val = v;
                                } else {
                                    val = 25 - 100 / v;
                                }
                            }
                            return val;
                        },
                        inverseTransform: function (v) {
                            var val;
                            if (statistics_mode == 0) {
                                if (v <= 400) {
                                    val = v;
                                } else {
                                    val = 40000 / (500 - v);
                                }
                            } else {
                                if (v <= 20) {
                                    val = v;
                                } else {
                                    val = 100 / (25 - v);
                                }
                            }
                            return val;
                        }
                    }
                });
                $("#lineLabel .linesOne").html(data.top10[0].orgname);
                $("#lineLabel .linesTwo").html(data.top10[1].orgname);
                $("#lineLabel .linesThree").html(data.top10[2].orgname);
                
            })
        });
    }
    /* 折线图绘制 结束 */

    /* 时间写入 开始 */
    function timeRender() {
        if (statistics_mode == 0) {
            if (year >= thisyear) {
                year = thisyear;
                $next.addClass("disabled");
            } else {
                if ($next.hasClass("disabled")) {
                    $next.removeClass("disabled");
                }
            }
            $cur.html(year+"年");
            $last.html(year - 1);
            $next.html(year + 1);
        } else {
            if (year >= thisyear && month >= thismonth) {
                year = thisyear;
                month = thismonth;
                $next.addClass("disabled");
            } else {
                if ($next.hasClass("disabled")) {
                    $next.removeClass("disabled");
                }
            }
            $cur.html(year + "年" + month+"月");
            $last.html(month == 1 ? 12 : month - 1);
            $next.html(month == 12 ? 1 : month + 1);
        }
    }
    /* 时间写入 结束 */

    //tooltip
    $('<div id="tooltip" style="display: none"></div>').appendTo('body');

    function showTooltip(x, y, contents) {
        $('#tooltip').stop();
        $('#tooltip').css({
            top: y - 16,
            left: x + 20
        }).html(contents).fadeIn(200);
    }

    //var previousPoint = null;

    $('#graph-lines').bind('plothover', function (event, pos, item) {
        if (item) {
            //if (previousPoint != item.dataIndex) {
            //	previousPoint = item.dataIndex;
            //$('#tooltip').remove();
            var data = item.series.data,
                index = item.dataIndex,
                content = [];
            if (statistics_type == 0) {
                content = ["评定工时", "实际工时"];
            } else if (statistics_type == 1) {
                content = ["总件数", "超时件数"];
            } else {
                content = ["总目标", "完成目标"];
            }
            showTooltip(item.pageX, item.pageY, content[0] + "：" + data[index][1] + " h/人<br/>" + content[1] + "：" + data[index][2]+" h/人");
            //}
        } else {
            $('#tooltip').stop();
            $('#tooltip').fadeOut(200);
            //previousPoint = null;
        }
    });
});