// 加载新闻轮播图片
    /**
        *ajax加载新闻图片
        *@param url   
    */
	function newsBannerRander(url){
        $.ajax({
            type: "post",
            // url: "../../test/data/index/index_news_bannerData.json",
            url: url,
            dataType: "json",
            data:{
                count:5
            },
            success: rsHandler(function (data) {
                //console.log(data);
                var gettpl = $("#tpl_banner").html();
                laytpl(gettpl).render(data, function(html){
                    $("#banner").html(html);
                });

                //事件
                banner();
            })
        });
    }

// 加载类别分类列表
    /**
        *ajax加载分类列表
        *@param type :new/inform/document, url1,url2,url3
    */
	var DirId = 0;
    function NavListLoader(type,url){
        $('.'+type+'sNavList').html('');
        $.ajax({
            type: "POST",
            url: url,
            data: {DriID:'0'},
            dataType: "json",
            success: rsHandler(function (data) {
                var html = '';
                $.each(data,function(index,value){
                    html += '<li dataId="' + value.directoryId + '">' + value.directoryName + '<em class="newNavListBg"></em></li>';
                });
                $('.'+type+'sNavList').append(html);

                /* 新闻分类大类点击 开始 */
                $('.' + type + 'sNavList li').each(function () {
                    var _this = this;
                    $(this).on('click',function(){
                        $(this).css('color','#58b557').find('em').css('display','block').parent().siblings()
                            .css('color','#373737').find('em').css('display','none');
                        $('.'+type+'sCategoryUp ul').html('');
                        /* 获取新闻分类种类(newsCategoryUp) 开始 */
                        var data = { DriID: parseInt($(_this).attr('dataId')) };
                        var str = '';
                        for (var i = 0; i < type.length; i++) {
                            if (i == 0) {
                                str+= type[i].toUpperCase();
                                console.log(str);
                             }
                            else {
                                str+= type[i];
                            }
                            
                        }
                        generalLoader('/News/GetNew', data, str);
                        CategoryUpLoader(_this,type,url);
                        /* 获取新闻分类种类 结束 */
                            
                    })
                 });
                /* 新闻分类大类点击 结束 */

            })
        });
    }; 
    //加载中间类别
    function CategoryUpLoader(that,type, url) {
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data:{
                DriID: $(that).attr('dataId')
            },
            success: rsHandler(function (data) {
                if (data != null) {
                    $('.' + type + 'sCategory').css('height', '31px').find('.' + type + 'sCategoryUp').css('display', 'block');
                    html = '';
                    $.each(data, function (index, value) {
                        html += '<li dataId="' + value.directoryId + '">' + value.directoryName + '</li>';
                    });
                    $('.' + type + 'sCategoryUp ul').append(html);
                    $('.' + type + 'sCategoryUp li').each(function () {
                        var _this = this;
                        $(this).on('click', function () {
                            $(this).css('color', '#58b557').siblings().css('color', '#373737');
                            $('.' + type + 'sCategoryDown ul').html('');
                            var data = { DriID: parseInt($(_this).attr('dataId')) };
                            var str = '';
                            for (var i = 0; i < type.length; i++) {
                                if (i == 0) {
                                    str += type[i].toUpperCase();
                                    console.log(str);
                                }
                                else {
                                    str += type[i];
                                }

                            }
                            generalLoader('/News/GetNew', data, str);
                            CategoryDownLoader(_this, type, url);
                            
                        });
                    });
                }
               

            })
        });
    }
    //加载下面类别
    function CategoryDownLoader(that,type,url){
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data:{
                DriID: $(that).attr('dataId')
            },
            success: rsHandler(function (data) {
                if (data != null) {
                    $('.' + type + 'sCategory').css('height', '61px').find('.' + type + 'sCategoryDown').css('display', 'block');
                    html = '';
                    $.each(data, function (index, value) {
                        html += '<li dataId="' + value.directoryId + '">' + value.directoryName + '</li>';
                    });
                    $('.' + type + 'sCategoryDown ul').append(html);
                    $('.' + type + 'sCategoryDown ul li').click(function () {
                        var data = { DriID: $(this).attr('term') };
                        var str = '';
                        for (var i = 0; i < type.length; i++) {
                            if (i == 0) {
                                str += type[i].toUpperCase();
                                console.log(str);
                            }
                            else {
                                str += type[i];
                            }

                        }
                        generalLoader('/News/GetNew', data, str);
                    });
                }
                                                                                   
            })
        });
    }

//记载列表
    function ListLoader(Categorys){
        Categorys.each(function(){
            $(this).on('click',function(){
                $.ajax({
                    type: "post",
                    url: url3,
                    dataType: "json",
                    data:{
                        parentId:$(this).attr('dataId')
                    },
                });
            });
        });
    }
//加载普通新闻、通知、文档列表
    function generalLoader(url, data, type) {
        $('.generalNewsListArea ul').html('');
        console.log(data);
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data:data,
            success: rsHandler(function (data) {
                if (data != null) {
                    var index;
                    var html = '';
                    var htmlBlocks = [];
                    var dataCount = data.length;
                    var numOfBlock = dataCount % 10 == 0 ? dataCount / 10 : parseInt(dataCount / 10) + 1;
                    for (var i = 0; i < numOfBlock; i++) {
                        html += '<li class="generalNewsList"><ul></ul></li>'
                    }
                    $('.generalNewsListArea ul').prepend(html);
                    html = '';
                    for (var i = 0; i < numOfBlock; i++) {
                        htmlBlocks[i] = '';
                        for (var j = 0; j < 10; j++) {
                            index = i * 10 + j;
                            if (index < dataCount) {
                                htmlBlocks[i] += '<li><span class="summaryOf' + type + '"><a href=/NewsDel/NewsDel?id=' + data[index].newId + '>' + data[index].title + '</a></span>' +
                                           '<span class="dateOf' + type + '">' + data[index].FCreatTime + '</span></li>';
                            }
                        }
                        $('.generalNewsList ul').eq(i).append(htmlBlocks[i]);
                    }
                }
            })
        });
    }

    /* 图片切换jq 开始 */
    var MyMar33;
    function banner() {
        clearInterval(MyMar33);
        var bn_id = 0;
        var bn_id2 = 1;
        var speed33 = 5000;
        var qhjg = 1;

        $("#banner .d1").hide();
        $("#banner .d1").eq(0).fadeIn("slow");
        if ($("#banner .d1").length > 1) {
            $("#banner_id li").eq(0).addClass("nuw");
            function Marquee33() {
                bn_id2 = bn_id + 1;
                if (bn_id2 > $("#banner .d1").length - 1) {
                    bn_id2 = 0;
                }
                $("#banner .d1").eq(bn_id).css("z-index", "2");
                $("#banner .d1").eq(bn_id2).css("z-index", "1");
                $("#banner .d1").eq(bn_id2).show();
                $("#banner .d1").eq(bn_id).fadeOut("slow");
                $("#banner_id li").removeClass("nuw");
                $("#banner_id li").eq(bn_id2).addClass("nuw");
                bn_id = bn_id2;
            };

            MyMar33 = setInterval(Marquee33, speed33);

            $("#banner_id li").click(function () {
                var bn_id3 = $("#banner_id li").index(this);
                if (bn_id3 != bn_id && qhjg == 1) {
                    qhjg = 0;
                    $("#banner .d1").eq(bn_id).css("z-index", "2");
                    $("#banner .d1").eq(bn_id3).css("z-index", "1");
                    $("#banner .d1").eq(bn_id3).show();
                    $("#banner .d1").eq(bn_id).fadeOut("slow", function () { qhjg = 1; });
                    $("#banner_id li").removeClass("nuw");
                    $("#banner_id li").eq(bn_id3).addClass("nuw");
                    bn_id = bn_id3;
                }
            })
            $("#banner_id").hover(
                function () {
                    clearInterval(MyMar33);
                }
                ,
                function () {
                    MyMar33 = setInterval(Marquee33, speed33);
                }
            )
        }
        else {
            $("#banner_id").hide();
        }
    }
    /* 图片切换jq 结束 */