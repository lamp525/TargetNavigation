//@ sourceURL=newsmanagement.js
/**
 * Created by DELL on 15-7-17.
 */
$(function () {
    //新闻列表请求所用参数
    var argus = {
        type: 0,
        directoryId: 0,
        currentPage: 1
    };
    var dirtype = 0;
    //批量操作所用参数
    var newids = [],
        dirsid = [],
        dirid = [],
        directory = [];
    var newurl, noticeurl, commUrl;
    var info;
    //上传图片
    var file = {
        imageId: 0,
        saveName: "",
        extension: "",
        width: 0,
        height: 0
    };
    var newsinfo = {
        newId: 0,
        directoryId: 0,
        title: "",
        contents: "",
        summary: "",
        titleImage: "",
        notice: false,
        viewNum: 1,
        isTop: false,
        publish: false,
        keyword: ""
    };
    var dirinfo = {
        directoryId: 0,
        directoryName: "",
        parentDirectory: 0,
        orderNum: 1
    };
    var imgUrl = "";


    //点击类型新闻/通知
    $(".neno li").click(function () {
        $("#dirname").text("");
        $("#dirname").attr("term", "");
        $("#bitneno").text($(this).text());
        $("#bitneno").attr("term", $(this).attr("term"));
        argus.type = $(this).attr("term");
        $(".nenodir").slideUp();
        
        loadtype();
        
    });

    var dirNameztree;
    $(".nenodir").slideUp();
    $(".dirlist").off("click");
    //点击加载分类名称（添加新闻 ）
    $(".dirlist").click(function () {
        if ($("#bitneno").text() != "") {
            $(".nenodir").slideToggle();
            loadtype();
        }
    });
    //加载类别
    function loadtype()
    {
        if (argus.type == 0) {
            //$(".nenodir>li").remove();
            $.ajax({
                type: "post",
                url: "/NewsManagement/GetNewsTypeList",
                dataType: "json",
                data: { currentPage: 1, dirId: 0 },
                success: rsHandler(function (data) {

                    //$.each(data, function (i,v) {
                    //    var $li = $("<li term='" + v.directoryId + "'><a href='#'>" + v.directoryName + "</a></li>");
                    //    $(".nenodir").append($li);
                    //});
                    //$(".nenodir li").click(function () {
                    //    $("#dirname").text($(this).text());
                    //    $("#dirname").attr("term", $(this).attr("term"));
                    //});
                    if (data.length > 0) {
                        dirNameztree = $.fn.zTree.init($("#ztree_dir"), {
                            callback: {
                                onClick: function (e, id, node) {
                                    if (node.name.length > 6) {

                                        $("#dirname").text(node.name.substring(0, 5) + ".");
                                        $("#dirname").attr("term", node.directoryId);
                                        $(".nenodir").slideUp();
                                    } else {
                                        $("#dirname").text(node.directoryName);
                                    }
                                    $("#dirname").attr("term", node.directoryId);
                                    $(".nenodir").slideUp();

                                },
                                onNodeCreated: function (e, id, node) {
                                    $("#" + node.tId).css("width", "auto");
                                    $("#" + node.tId + " a").css("width", "auto");
                                    if (node.name.length > 6) {
                                        $("#" + node.tId + "_span").text(node.name.substring(0, 6) + "...");
                                    }
                                }
                            },
                            view: {
                                showIcon: false,
                                showLine: false,
                                selectedMulti: false
                            },
                            async: {
                                enable: true,
                                url: "/NewsManagement/GetNewsTypeList",
                                autoParam: ["id"],
                                dataFilter: function (treeId, parentNode, responseData) {
                                    return responseData.data;
                                }
                            }
                        }, data);
                    }
                   
                })
            });
        }
        else if (argus.type == 1) {
            //$(".nenodir>li").remove();
            $.ajax({
                type: "post",
                url: "/NewsManagement/GetNoticeTypeList",
                dataType: "json",
                data: { currentPage: 1, dirId: 0 },
                success: rsHandler(function (data) {
                    //$.each(data, function (i, v) {
                    //    var $li = $("<li term='" + v.directoryId + "'><a href='#'>" + v.directoryName + "</a></li>");
                    //    $(".nenodir").append($li);
                    //});
                    //$(".nenodir li").click(function () {
                    //    $("#dirname").text($(this).text());
                    //    $("#dirname").attr("term", $(this).attr("term"));
                    //});
                    if (data.length > 0) {
                        dirNameztree = $.fn.zTree.init($("#ztree_dir"), {
                            callback: {
                                onClick: function (e, id, node) {
                                    if (node.name.length > 6) {

                                        $("#dirname").text(node.name.substring(0, 5) + ".");
                                        $("#dirname").attr("term", node.directoryId);
                                        $(".nenodir").slideUp();
                                    } else {
                                        $("#dirname").text(node.directoryName);
                                    }

                                    $("#dirname").attr("term", node.directoryId);
                                    $(".nenodir").slideUp();
                                },
                                onNodeCreated: function (e, id, node) {
                                    $("#" + node.tId).css("width", "auto");
                                    $("#" + node.tId + " a").css("width", "auto");
                                    if (node.name.length > 6) {
                                        $("#" + node.tId + "_span").text(node.name.substring(0, 6) + "...");
                                    }
                                }
                            },
                            view: {
                                showIcon: false,
                                showLine: false,
                                selectedMulti: false
                            },
                            async: {
                                enable: true,
                                url: "/NewsManagement/GetNoticeTypeList",
                                autoParam: ["id"],
                                dataFilter: function (treeId, parentNode, responseData) {
                                    return responseData.data;
                                }
                            }
                        }, data);
                    }

                })
            });
        }
    }

    //鼠标失焦事件
    $(".typeN").blur(function () {
        $(".nenodir").slideUp();
    });

    //弹出框
    $("#newFile_building").click(function () {
        $("#myModal-newdir").modal({
            remote: "/NewsManagement/GetAddDir"
        });
        dirinfo.directoryId = 0;
    });

    //添加新闻样式
    $("[name='edit'],.add").click(function () {
        $(".titleUl li").removeClass("active");
        $(".titleUl li:eq(2)").addClass("active");
        addview("none", "none", "block");

    });


    function keyword(word) {
        var word = word.replace(/(^\s+)|(\s+$)/g, "");
        word = word.replace(/\s+/g, ' ');
        word = word.split(' ');
        word.sort();
        var result = [word[0]];
        for (var i = 1; i < word.length; i++) {
            if (word[i] != result[result.length-1]) {
                result.push(word[i])
            }
        }
        return result;

    }

    //发布控件
    $(".authy-dropdown li a").click(function () {
        $("#power-span").find("span:first").text($(this).text());
    });
    $(".publish").click(function () {
        if ($(".enterTitle").attr("term") == "") {
            newsinfo.newId = 0;
        } else {
            newsinfo.newId = $(".enterTitle").attr("term");
        }

        newsinfo.titleImage = $("#titleimg").attr("term");
        newsinfo.directoryId = $("#dirname").attr("term");
        newsinfo.title = $(".enterTitle").val();
       
        $("#editor img").each(function () {
            if ($(this).width() > 700) {
                width = $(this).width() / 2;
                height = $(this).height() / 2;
                $(this).width(width);
                $(this).height(height);
            }

        });
        newsinfo.contents = $("#editor").wysiwyg().html();
        //newsinfo.contents = newsinfo.contents.replace(/</g, "|").replace(/>/g, "。");
        if (newsinfo.contents =="<br><br>")
        {
            ncUnits.alert("新闻内容不能为空");
            return false;
        }
        if (newsinfo.title == "") {
            ncUnits.alert("没有填写标题");
            return false;
        }
        if ($("#bitneno").text() == "" || $("#dirname").text() == "") {
            ncUnits.alert("没有选择类型")
            return false;
        }
        if ($("#bitneno").attr("term") == 0) {
            newsinfo.notice = false;
        }
        else {
            newsinfo.notice = true;
        }

        newsinfo.publish = false;
        if ($("#setTop").is(':checked')) {
            newsinfo.isTop = true;
        }
        else {
            newsinfo.isTop = false;
        }

        newsinfo.directoryName = $("#dirname").text();
        newsinfo.summary = $("#disc").val();
        newsinfo.keyword = keyword($("#kword").val());
        console.log('vv', newsinfo.keyword);
        newsinfo.viewnum = 1;
        ncUnits.confirm({
            title: '提示',
            html: '确认要发布？',
            yes: function (layer_confirm) {
                layer.close(layer_confirm);
                $.ajax({
                    type: "post",
                    url: "/NewsManagement/publish",
                    dataType: "json",
                    data: { data: JSON.stringify(newsinfo) },
                    success: rsHandler(function (data) {
                        if ($(".newname").text() == "添加新闻") {
                            ncUnits.alert("成功");
                        } else {
                            ncUnits.alert("修改成功");
                        }
                        clearContent();
                        addview("block", "none", "none");
                        $('#NewsMang').tab('show');
                        if (newsinfo.notice) {
                            loadNews("/NewsManagement/GetNoticeList");
                        } else {
                            loadNews("/NewsManagement/GetNewsList");
                        }
                    })
                });
            }
        });
    });
    //保存
    $(".save").click(function () {
        if ($(".enterTitle").attr("term") == "") {
            newsinfo.newId = 0;
        } else {
            newsinfo.newId = $(".enterTitle").attr("term");
        }

        newsinfo.titleImage = $("#titleimg").attr("term");
        newsinfo.directoryId = $("#dirname").attr("term");
        newsinfo.title = $(".enterTitle").val();

        $("#editor img").each(function () {
            if ($(this).width() > 700) {
                width = $(this).width() / 2;
                height = $(this).height() / 2;
                $(this).width(width);
                $(this).height(height);
            }

        });
        newsinfo.contents = $("#editor").wysiwyg().html();
        //newsinfo.contents = newsinfo.contents.replace(/</g, "|").replace(/>/g, "。");
        if (newsinfo.contents =="<br><br>") {
            ncUnits.alert("新闻内容不能为空");
            return false;
        }
        if (newsinfo.title == "") {
            ncUnits.alert("没有填写标题");
            return false;
        }
        if ($("#bitneno").text() == "" || $("#dirname").text() == "") {
            ncUnits.alert("没有选择类型");
            return false;
        }

        if ($("#bitneno").attr("term") == 0) {
            newsinfo.notice = false;
        }
        else {
            newsinfo.notice = true;
        }

        newsinfo.publish = false;
        if ($("#setTop").is(':checked')) {
            ncUnits.alert("未发布的新闻,不允许置顶");
            newsinfo.isTop = false;
            return false;
        }
        else {
            newsinfo.isTop = false;
        }

        //newsinfo.directoryName = $("#dirname").text();

        newsinfo.summary = $("#disc").val();
        newsinfo.keyword = $("#kword").val();
        newsinfo.viewnum = 1;
        ncUnits.confirm({
            title: '提示',
            html: '确认要保存？',
            yes: function (layer_confirm) {
                layer.close(layer_confirm);
                $.ajax({
                    type: "post",
                    url: "/NewsManagement/Save",
                    dataType: "json",
                    data: { data: JSON.stringify(newsinfo) },
                    success: rsHandler(function (data) {
                        if ($(".newname").text() == "添加新闻")
                        {
                            ncUnits.alert("成功");
                        } else {
                            ncUnits.alert("修改成功");
                        }
                      
                        clearContent();
                        $('#NewsMang').tab('show');
                        addview("block", "none", "none");
                        if (newsinfo.notice)
                        {
                            loadNews("/NewsManagement/GetNoticeList");
                        } else {
                            loadNews("/NewsManagement/GetNewsList");
                        }
                       

                    })
                });
            }
        });
    });

    //点击新闻/通知图标加载
    $(".newsclass .News").click(function () {
        argus.type = 0;
        argus.directoryId = 0;
        newurl = "/NewsManagement/GetNewsList";
        $(this).find("a").css("color", "#6AB674");
        $(".noticeicon").parent().css("color", "#686868");
        $(".prevP").addClass("prevPHit");
        $(".nextP").removeClass("nextPHit");
        loadpage(newurl, 1);
        $(".current").val(1);
    });
    $(".newsclass .Notice").click(function () {
        argus.type = 1;
        argus.directoryId = 0;
        noticeurl = "/NewsManagement/GetNoticeList";
        // loadNews(noticeurl);
        $(this).find("a").css("color", "#6AB674");
        //$(this).find(".noticeicon").addClass("noticeiconHit");
        //$(".newsicon").removeClass("newsiconHit");
        $(".newsicon").parent().css("color", "#686868");
        $(".prevP").addClass("prevPHit");
        $(".nextP").removeClass("nextPHit");
        loadpage(noticeurl, 1);
        $(".current").val(1);
    });
    //点击类别管理时触发
    $("#dirmanagement").click(function () {
        loaddirectorytree("/NewsManagement/GetNewsTypeList", null, 0);
    });
   
    //点击新闻/通知分类
    $(".Newsdir .newsicon").addClass("newsiconHit");
    $(".dirclass .Newsdir").click(function () {
        dirtype = 0;
        loaddirectorytree("/NewsManagement/GetNewsTypeList", null, 0);
        $(this).find("a").css("color", "#6AB674");

        $(".noticeicon").parent().css("color", "#686868");
    });
    $(".dirclass .Noticedir").click(function () {
        dirtype = 1;
        loaddirectorytree("/NewsManagement/GetNoticeTypeList", null, 1);
        $(this).find("a").css("color", "#6AB674");
        //$(this).find(".noticeicon").addClass("noticeiconHit");
        //$(".newsicon").removeClass("newsiconHit");
        $(".newsicon").parent().css("color", "#686868");

    });

    /*批量事件 开始*/
    $(".batch li").off('click');
    $('.batch li').click(function () {
        var operate = $.trim($(this).text());
        if (operate == "批量删除") {
            newids.splice(0, newids.length);
            $(".newslist :checked").each(function () {

                newids.push($(this).attr('term'));
                //$(this).parents("tr").remove();
            })
            if (newids.length == 0) {
                ncUnits.alert("未勾选");
                return;
            }
            ncUnits.confirm({
                title: '提示',
                html: '确认要删除？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/DeleteNews",
                        dataType: "json",
                        data: { data: JSON.stringify(newids) },
                        success: rsHandler(function (data) {
                            ncUnits.alert("删除成功");
                            loadNews(commUrl);

                        })
                    });
                }
            });

        }
        else if (operate == "批量发布") {
            newids.splice(0, newids.length);
            $(".newslist :checked").each(function () {
                newids.push($(this).attr('term'));
            })
            if (newids.length == 0) {
                ncUnits.alert("未勾选");
                return;
            }
            ncUnits.confirm({
                title: '提示',
                html: '确认要发布？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/PublishNews",
                        dataType: "json",
                        data: { data: JSON.stringify(newids) },
                        success: rsHandler(function (data) {
                            loadNews(commUrl);
                        })
                    });
                }
            });
        }
        else if (operate == "取消发布") {

            newids.splice(0, newids.length);
            $(".newslist :checked").each(function () {
                newids.push($(this).attr('term'));
            })
            if (newids.length == 0) {
                ncUnits.alert("未勾选");
                return;
            }
            ncUnits.confirm({
                title: '提示',
                html: '确认要取消发布？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/UnPublishNews",
                        dataType: "json",
                        data: { data: JSON.stringify(newids) },
                        success: rsHandler(function (data) {
                            loadNews(commUrl);
                        })
                    });
                }
            });
        }
        else if (operate == "批量置顶") {
            var publishFlag = true;
            newids.splice(0, newids.length);
            $(".newslist :checked").each(function () {
                newids.push($(this).attr('term'));
                if ($(this).parents("tr").find("a:last").text() == "发布") {
                    publishFlag = false;
                }
            })
            if (!publishFlag) {
                ncUnits.alert("新闻处于未发布状态,不允许置顶");
                return;
            }
            if (newids.length == 0) {
                ncUnits.alert("未勾选");
                return;
            }
            ncUnits.confirm({
                title: '提示',
                html: '确认要置顶？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/SetTopNews",
                        dataType: "json",
                        data: { data: JSON.stringify(newids) },
                        success: rsHandler(function (data) {
                            loadNews(commUrl);
                        })
                    });
                }
            });
        }
        else if (operate == "取消置顶") {

            newids.splice(0, newids.length);
            $(".newslist :checked").each(function () {
                newids.push($(this).attr('term'));
            })
            if (newids.length == 0) {
                ncUnits.alert("未勾选");
                return;
            }
            ncUnits.confirm({
                title: '提示',
                html: '确认要取消置顶？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/SetUnTopNews",
                        dataType: "json",
                        data: { data: JSON.stringify(newids) },
                        success: rsHandler(function (data) {
                            loadNews(commUrl);
                        })
                    });
                }
            });
        }
    });
    /*批量事件 结束*/








    //页面加载新闻列表
    loadNews("/NewsManagement/GetNewsList");
    //新闻/通知分类的收缩展开
    function clickdown(downclass, ztree,type,url) {
        $(downclass).click(function () {
            $(this).children("span:last").css("color", "#6AB674");
            if (type == 0)
            {
                $(".dset2").children("span:last").css("color", "#686868");
                var n = rightNewTree.getNodeByParam("id",argus.directoryId);
                rightNewTree.cancelSelectedNode(n);

                var r = rightNoticeTree.getNodeByParam("id", argus.directoryId);
                rightNoticeTree.cancelSelectedNode(r);
               
            } else
            {
                $(".dset1").children("span:last").css("color", "#686868");
                var r = rightNoticeTree.getNodeByParam("id", argus.directoryId);
                rightNoticeTree.cancelSelectedNode(r);

                var n = rightNewTree.getNodeByParam("id", argus.directoryId);
                rightNewTree.cancelSelectedNode(n);
            }
            addview("block", "none", "none");
            $('#NewsMang').tab('show');
            argus.type = type;
            argus.directoryId = 0;
            loadpage(url, 1);
            $(".current").val(1);
            if ($(this).children().hasClass("line-close")) {
                $(this).children(".arrowDown").removeClass("line-close");
                $(this).children(".arrowDown").addClass("line-down");
                $(ztree).slideDown();
            } else {
                $(this).children(".arrowDown").addClass("line-close");
                $(this).children(".arrowDown").removeClass("line-down");
                $(ztree).slideUp();
            }
        });
    }
    $(".ds1ztree,.ds2ztree").show("fast", function () {
        $(".dset1,.dset2").children(".arrowDown").removeClass("line-close");
        $(".dset1,.dset2").children(".arrowDown").addClass("line-down");
    });
    clickdown(".dset1", ".ds1ztree", 0, "/NewsManagement/GetNewsList");
    clickdown(".dset2", ".ds2ztree", 1, "/NewsManagement/GetNoticeList");





    //显示新建分类
    var $adir = $(".titleUl li:eq(1)");
    $(".titleUl li:eq(0)").click(function () {
        addview("block", "none", "none");
        loadNews(commUrl);
    });
    $adir.click(function () {
        addview("none", "block", "none");
    });
    $(".titleUl li:eq(2)").click(function () {
        addview("none", "none", "block");
    });

    //Function开始

    //显示foot
    function addview(n, no, b) {
        $(".titleUl .nav-right:eq(0)").css("display", n);
        $(".titleUl .nav-right:eq(1)").css("display", no);
        $(".btn-width").css("display", b);
        $(".nenodir").slideUp();
    }
    //文本编辑器
    function initToolbarBootstrapBindings() {
        var fonts = ['Serif', 'Sans', 'Arial', 'Arial Black', 'Courier',
                'Courier New', 'Comic Sans MS', 'Helvetica', 'Impact', 'Lucida Grande', 'Lucida Sans', 'Tahoma', 'Times',
                'Times New Roman', 'Verdana'],
            fontTarget = $('[title=Font]').siblings('.dropdown-menu');
        $.each(fonts, function (idx, fontName) {
            fontTarget.append($('<li><a data-edit="fontName ' + fontName + '" style="font-family:\'' + fontName + '\'">' + fontName + '</a></li>'));
        });
        $('a[title]').tooltip({ container: 'body' });
        $('.dropdown-menu input').click(function () { return false; })
            .change(function () { $(this).parent('.dropdown-menu').siblings('.dropdown-toggle').dropdown('toggle'); })
            .keydown('esc', function () { this.value = ''; $(this).change(); });

        $('[data-role=magic-overlay]').each(function () {
            var overlay = $(this), target = $(overlay.data('target'));
            overlay.css('opacity', 0).css('position', 'absolute').offset(target.offset()).width(target.outerWidth()).height(target.outerHeight());
        });
        if ("onwebkitspeechchange" in document.createElement("input")) {
            var editorOffset = $('#editor').offset();
            $('#voiceBtn').css('position', 'absolute').offset({ top: editorOffset.top, left: editorOffset.left + $('#editor').innerWidth() - 35 });
        } else {
            $('#voiceBtn').hide();
        }
    };
    function showErrorAlert(reason, detail) {
        var msg = '';
        if (reason === 'unsupported-file-type') { msg = "Unsupported format " + detail; }
        else {
            console.log("error uploading file", reason, detail);
        }
        $('<div class="alert"> <button type="button" class="close" data-dismiss="alert">&times;</button>' +
        '<strong>File upload error</strong> ' + msg + ' </div>').prependTo('#alerts');
    };
    initToolbarBootstrapBindings();
    $('#editor').wysiwyg({ fileUploadError: showErrorAlert });
    window.prettyPrint && prettyPrint();

    //bkLib.onDomLoaded(function() {
    //    new nicEditor({ fullPanel: true}).panelInstance('txtContent');
    //    //new nicEditor({buttonList : ['fontSize','bold','italic','underline','strikeThrough','subscript','superscript','html','image']}).panelInstance('txtContent');
    //});


    /*右侧目录加载 开始*/
    var rightNewTree, rightNoticeTree;
    loadrigthtreeNew();
    loadrigthtreeNotice();
    function loadrigthtreeNew() {
        $.ajax({
            type: "post",
            url: "/NewsManagement/GetNewsTypeList",
            dataType: "json",
            success: rsHandler(function (data) {
                rightNewTree = $.fn.zTree.init($("#newcatalogue"), {
                    callback: {
                        onClick: function (e, id, node) {
                            var r = rightNoticeTree.getNodeByParam("id", argus.directoryId);
                            rightNoticeTree.cancelSelectedNode(r);
                            $(".dset2").children("span:last").css("color", "#686868");
                            $(".dset1").children("span:last").css("color","#686868");
                            //$(".titleUl li").removeClass("active");
                            //$(".titleUl li:eq(0)").addClass("active");
                            addview("block", "none", "none");
                            argus.directoryId = node.directoryId;
                            $('#NewsMang').tab('show');
                           
                                $(".current").val(1);
                                argus.type = 0;
                                //loadNews("/NewsManagement/GetNewsList");
                                loadpage("/NewsManagement/GetNewsList", 1);
                        },
                        onNodeCreated: function (e, id, node) {
                            $("#" + node.tId).css("width", "auto");
                            $("#" + node.tId + "_a").css("width", "auto");
                        },
                        onExpand: function (e,id,node) {
                            if (node.isParent) {
                                $("#" + node.tId + "_ul").css("width", "auto");
                            }
                            $(".ds1ztree").css("overflow-x","auto");
                        }
                    },
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    async: {
                        enable: true,
                        url: "/NewsManagement/GetNewsTypeList",
                        autoParam: ["id"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }, data);
            })
        });
      
    }
    function loadrigthtreeNotice(){
        $.ajax({
            type: "post",
            url: "/NewsManagement/GetNoticeTypeList",
            dataType: "json",
            success: rsHandler(function (data) {
                rightNoticeTree = $.fn.zTree.init($("#noticecatalogue"), {
                    callback: {
                        onClick: function (e, id, node) {
                            var n = rightNewTree.getNodeByParam("id", argus.directoryId);
                            rightNewTree.cancelSelectedNode(n);

                           
                            $(".dset1").children("span:last").css("color", "#686868");
                            $(".dset2").children("span:last").css("color", "#686868");
                            //$(".titleUl li").removeClass("active");
                            //$(".titleUl li:eq(0)").addClass("active");
                            addview("block", "none", "none");
                            argus.directoryId = node.directoryId;
                            $('#NewsMang').tab('show');
                            $(".current").val(1)
                            argus.type = 1;
                            loadpage("/NewsManagement/GetNoticeList", 1);
                            
                        },
                        onNodeCreated: function (e, id, node) {
                            $("#" + node.tId).css("width", "auto");
                            $("#" + node.tId + "_a").css("width", "auto");
                        },
                        onExpand: function (e, id, node) {
                            if (node.isParent) {
                                $("#" + node.tId + "_ul").css("width", "auto");
                            }
                            $(".ds2ztree").css("overflow-x", "auto");
                        }
                    },
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    async: {
                        enable: true,
                        url: "/NewsManagement/GetNoticeTypeList",
                        autoParam: ["id"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }, data);
            })
        });
    }
    /*右侧目录加载 结束*/

    //var dragId;
    //    function ztreeBeforeDrag(id,node)
    //    {
    //        for (var i=0,l=node.length; i<l; i++) {
    //            dragId = node[i].parentTId;
    //            if (node[i].drag === false) {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //    //禁止将子节点拖拽为根节点
    //    function zTreeBeforeDrop(treeId, treeNodes, targetNode, moveType) {
    //        if(targetNode.parentTId == dragId&&(targetNode == null || (moveType != "inner" && !targetNode.parentTId))){
    //            return true;
    //        }
    //        else{
    //            return false;
    //        }
    //
    //    };
    //批量删除分类
    $(".del").click(function () {
        var node = directoryFolderTree.getCheckedNodes(true);
        if (node.length == 0) {
            ncUnits.alert("未勾选");
            return false;
        }
        ncUnits.confirm({
            title: '提示',
            html: '确认要删除？',
            yes: function (layer_confirm) {
                layer.close(layer_confirm);
                var nodeid = [];
               
                if (dirtype == 0) {
                    nodeid.splice(0, nodeid.length);
                    for (var i = 0; i < node.length; i++) {
                        if (node[i].isParent == true) {
                            ncUnits.alert("分类下存在子节点，不允许删除");
                            return false;
                        }
                        else if (node[i].isNew == true) {
                            ncUnits.alert("该分类下有新闻,不允许删除");
                            return false;
                        } else {
                            nodeid.push(node[i].directoryId);
                        }
                    }

                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/DeleteNewsType",
                        dataType: "json",
                        data: { data: JSON.stringify(nodeid) },
                        success: rsHandler(function (data) {

                            loaddirectorytree("/NewsManagement/GetNewsTypeList", null, 0);
                            ncUnits.alert("删除成功");
                            //更新右侧
                            loadrigthtreeNew();
                        })
                    });
                }
                else {
                    nodeid.splice(0, nodeid.length);
                    for (var i = 0; i < node.length; i++) {
                        if (node[i].isParent == true) {
                            ncUnits.alert("分类下存在子节点，不允许删除");
                            return false;
                        }
                        else if(node[i].isNew == true){
                            ncUnits.alert("该分类下有通知,不允许删除");
                            return false;
                        }else {
                            nodeid.push(node[i].directoryId);
                        }
                    }
                    $.ajax({
                        type: "post",
                        url: "/NewsManagement/DeleteNoticeType",
                        dataType: "json",
                        data: { data: JSON.stringify(nodeid) },
                        success: rsHandler(function (data) {
                            loaddirectorytree("/Newsmanagement/GetNoticeTypeList", null, 1);
                            ncUnits.alert("删除成功");
                            //更新右侧目录
                            loadrigthtreeNotice();
                        })
                    });
                }

            }
        });

    });
    //循环添加排序序号
    function scopeOrder1(childNode, pnode, begin, end, targetNode, treeNodes, sortData)
    {
        if (childNode.length != 0) {
            for (var i = 0; i < childNode.length; i++) {
                if (childNode[i].id == targetNode.id) {
                    begin = i;
                } else if (childNode[i].id == treeNodes[0].id) {
                    end = i;
                }

            }
            for (var i = begin + 1; i <= end - 1; i++) {
                sortData.push({ directoryId: childNode[i].directoryId, orderNum: parseInt(childNode[i].orderNum) + 1 });
                childNode[i].orderNum = parseInt(childNode[i].orderNum) + 1;
            }

        } else {
            for (var i = 0; i <pnode.length; i++) {
                if (pnode[i].id == targetNode.id) {
                    begin = i;
                } else if (pnode[i].id == treeNodes[0].id) {
                    end = i;
                }
            }
            for (var i = begin + 1; i <= end - 1; i++) {
                sortData.push({ directoryId: pnode[i].directoryId, orderNum: parseInt(pnode[i].orderNum) + 1 });
                pnode[i].orderNum = parseInt(pnode[i].orderNum) + 1;
            }
        }
    }
    function scopeOrder2(childNode, pnode, begin, end, targetNode, treeNodes, sortData) {
        if (childNode.length != 0) {
            for (var i = 0; i < childNode.length; i++) {
                if (childNode[i].id == treeNodes[0].id) {
                    begin = i;
                } else if (childNode[i].id == targetNode.id ) {
                    end = i;
                }

            }
            for (var i = begin + 1; i <= end - 1; i++) {
                sortData.push({ directoryId: childNode[i].directoryId, orderNum: parseInt(childNode[i].orderNum) - 1 });
                childNode[i].orderNum = parseInt(childNode[i].orderNum) - 1;
            }

        } else {
            for (var i = 0; i < pnode.length; i++) {
                if (pnode[i].id == treeNodes[0].id) {
                    begin = i;
                } else if (pnode[i].id == targetNode.id) {
                    end = i;
                }
            }
            for (var i = begin + 1; i <= end - 1; i++) {
                sortData.push({ directoryId: pnode[i].directoryId, orderNum: parseInt(pnode[i].orderNum) - 1 });
                pnode[i].orderNum = parseInt(pnode[i].orderNum) - 1;
            }
        }
    }
    var parentName = "";

    ////点击类别管理修改
    function zTreeBeforeRename(treeId, treeNode, newName, isCancel) {
        $("#myModal-newdir").modal({
            remote: "/NewsManagement/GetAddDir"
        });
        dirinfo.directoryId = treeNode.directoryId;
        dirinfo.directoryName = treeNode.directoryName;
        dirinfo.parentDirectory = treeNode.parentDirectory;
        dirinfo.orderNum = treeNode.orderNum;
        if (treeNode.parentTId != null) {
            parentName = treeNode.getParentNode().directoryName;
        } else {
            parentName = "";
        }

        isCancel = false;
        return isCancel;
    }
    //过滤节点
    function filter(node)
    {
        return node.level = 0;
    }
    //类别管理
    //parentId:参数
    //ztreeid:DIV标签id
    var directoryFolderTree;
    function loaddirectorytree(url, parentId, type) {
        var sortData = [], childNode = [], pnode = [];
        var begin = 0,end=0;
        dirtype = type;
        if (dirtype == 0) {
            $(".Newsdir .newsicon").addClass("newsiconHit");
            $(".Noticedir .noticeicon").removeClass("noticeiconHit");
            $(".Noticedir a").css("color", "#686868");
            $(".Newsdir a").css("color", "#6AB674");
        }
        else {
            $(".Newsdir .newsicon").removeClass("newsiconHit");
            $(".Noticedir .noticeicon").addClass("noticeiconHit");
            $(".Newsdir a").css("color", "#686868");
            $(".Noticedir a").css("color", "#6AB674");
        }
        $(".ckTwo").prop("checked", false);
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data: { id: parentId },
            success: rsHandler(function (data) {
                //分类路径
                $(".pages span:first").attr("term", url);
                pnode = data;
              
                directoryFolderTree = $.fn.zTree.init($("#catalogues"), {
                    callback: {
                        beforeEditName: zTreeBeforeRename,
                        onClick: function (e, id, node) {
                            //argus.directoryId = node.directoryId;
                            var n = directoryFolderTree.getNodeByParam("id", node.id);
                            directoryFolderTree.expandNode(n);
                            directoryFolderTree.selectNode(n);
                        },
                        onCheck: function (e, id, node) {
                            if (node.isParent == true) {
                                directoryFolderTree.expandNode(node, true, true, true);
                            }
                         
                            if (node.checked) {
                                dirsid.push(node.id);
                                directory.push(node);
                                //全选(类别)
                                var checkFlag = true;
                                var check = $("#catalogues .chk");
                                check.filter(function (i) {
                                    if ($(this).hasClass("checkbox_false_full")) {
                                        checkFlag = false;
                                    }
                                });
                                if (checkFlag) {
                                    $(".ckTwo").prop("checked", true);
                                }
                            }
                            else {
                               
                                if (node.parentTId != null) {
                                    node.getParentNode().checked = false;
                                }
                                //全选为false
                                $(".ckTwo").prop("checked", false);
                                for (var i = 0; i < dirsid.length; i++) {
                                    if (dirsid[i] == node.id) {
                                        dirsid.splice(i, 1);
                                    }
                                }
                                for (var i = 0; i < directory.length; i++) {
                                    if (directory[i].id == node.id) {
                                        directory.splice(i, 1);
                                    }
                                }
                            }

                           

                        },
                        beforeDrag: beforeDrag,
                        beforeDragOpen: beforeDragOpen,
                        //拖拽前
                        onDrag: onDrag,
                        //拖拽后
                        onDrop: onDrop,
                        //展开节点
                        onExpand: function (e, id, node) {
                            if (node.children.length != 0) {
                                for (var i = 0; i < node.children.length; i++) {
                                    $("#" + node.children[i].tId).css("width", "100%");
                                    $("#" + node.children[i].tId + " a").css("width", "100%");
                                }
                            }
                        },
                        beforeRemove: function (id, node) {
                            ncUnits.confirm({
                                title: '提示',
                                html: '确认要删除？',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    dirid.splice(0, newids.length);
                                    dirid.push(node.id);
                                    if (url == "/NewsManagement/GetNewsTypeList") {
                                        if (node.isParent == true) {
                                            ncUnits.alert("分类下存在子节点，不允许删除");

                                            return false;
                                        }
                                        if (node.isNew == true) {
                                            ncUnits.alert("该类别下有新闻,不允许删除");

                                            return false;
                                        }
                                        $.ajax({
                                            type: "post",
                                            url: "/NewsManagement/DeleteNewsType",
                                            dataType: "json",
                                            data: { data: JSON.stringify(dirid) },
                                            success: rsHandler(function (data) {
                                                ncUnits.alert("删除成功");
                                                directoryFolderTree.removeNode(node);
                                                //更新右侧目录
                                                loadrigthtreeNew();
                                               
                                            })
                                        });
                                    }
                                    else {
                                        if (node.isParent == true) {
                                            ncUnits.alert("分类下存在子节点，不允许删除");
                                            return false;
                                        }
                                        if (node.isNew == true) {
                                            ncUnits.alert("该类别下有通知,不允许删除");

                                            return false;
                                        }
                                        $.ajax({
                                            type: "post",
                                            url: "/NewsManagement/DeleteNoticeType",
                                            dataType: "json",
                                            data: { data: JSON.stringify(dirid) },
                                            success: rsHandler(function (data) {

                                                ncUnits.alert("删除成功");
                                                directoryFolderTree.removeNode(node);
                                                //更新右侧目录
                                               
                                                loadrigthtreeNotice();
                                            })
                                        });
                                    }

                                },
                                no: function (layer_confirm) {
                                    layer.close(layer_confirm);

                                }

                            });
                            return false;

                        },



                        //拖拽排序
                        beforeDrop: function (treeId, treeNodes, targetNode, moveType, isCopy) {
                            if (targetNode.parentTId != null) {
                                childNode = targetNode.getParentNode().children;
                            }
                          
                            className = (className === "dark" ? "" : "dark");
                            if (treeNodes[0].parentTId == null && moveType == "inner" || treeNodes[0].pId == null && targetNode.level != 0) {
                                return false;
                            }
                            //if (targetNode.isFirstNode == true && moveType == "prev")
                            //{
                            //    return false;
                            //}
                            //if (targetNode.isLastNode == true && moveType == "next") {
                            //    return false;
                            //}
                           
                            if (treeNodes[0].orderNum > targetNode.orderNum) {
                                sortData.splice(0, sortData.length);
                                if (moveType == "prev") {
                                    sortData.push({ directoryId: targetNode.directoryId, orderNum: parseInt(targetNode.orderNum) + 1 });
                                    sortData.push({ directoryId: treeNodes[0].directoryId, orderNum: targetNode.orderNum });
                                  
                                    scopeOrder1(childNode, pnode, begin, end, targetNode, treeNodes, sortData);
                                    
                                    treeNodes[0].orderNum = targetNode.orderNum;
                                    targetNode.orderNum = parseInt(targetNode.orderNum) + 1;

                                } else if (moveType == "next") {
                                    sortData.push({ directoryId: targetNode.directoryId, orderNum: targetNode.orderNum });
                                    sortData.push({ directoryId: treeNodes[0].directoryId, orderNum: parseInt(targetNode.orderNum) + 1 });

                                    scopeOrder1(childNode, pnode, begin, end, targetNode, treeNodes, sortData);
                                   
                                    targetNode.orderNum = targetNode.orderNum;
                                    treeNodes[0].orderNum = parseInt(targetNode.orderNum) + 1;

                                }

                            }
                            else {
                                sortData.splice(0, sortData.length);
                                if (moveType == "prev") {
                                    sortData.push({ directoryId: targetNode.directoryId, orderNum: targetNode.orderNum });
                                    sortData.push({ directoryId: treeNodes[0].directoryId, orderNum: parseInt(targetNode.orderNum) - 1 });

                                    scopeOrder2(childNode, pnode, begin, end, targetNode, treeNodes, sortData);

                                    targetNode.orderNum = targetNode.orderNum;
                                    treeNodes[0].orderNum = parseInt(targetNode.orderNum) - 1;

                                } else if (moveType == "next") {
                                    sortData.push({ directoryId: targetNode.directoryId, orderNum: parseInt(targetNode.orderNum) - 1 });
                                    sortData.push({ directoryId: treeNodes[0].directoryId, orderNum: targetNode.orderNum });

                                    scopeOrder2(childNode, pnode, begin, end, targetNode, treeNodes, sortData);

                                    treeNodes[0].orderNum = targetNode.orderNum;
                                    targetNode.orderNum = parseInt(targetNode.orderNum) - 1;


                                }
                            }

                            if (dirtype == 0) {
                                $.ajax({
                                    type: "post",
                                    url: "/NewsManagement/OrderNewsType",
                                    dataType: "json",
                                    data: { data: JSON.stringify(sortData) },
                                    success: rsHandler(function (data) {
                                        //loaddirectorytree("/NewsManagement/GetNewsTypeList",null,0);


                                        //更新右侧目录
                                        loadrigthtreeNew();
                                      
                                    })
                                });
                            } else {
                                $.ajax({
                                    type: "post",
                                    url: "/NewsManagement/OrderNoticeType",
                                    dataType: "json",
                                    data: { data: JSON.stringify(sortData) },
                                    success: rsHandler(function (data) {
                                        //loaddirectorytree("/NewsManagement/GetNoticeTypeList",null,1);

                                        //更新右侧目录
                                        loadrigthtreeNotice();
                                    })
                                });
                            }

                        }


                    },
                    check: {
                        chkStyle: "checkbox",
                        enable: true,
                        chkboxType: { "Y": "s", "N": "s" }
                    },
                    edit: {
                        drag: {
                            isMove: true,
                            autoExpandTrigger: true,
                            prev: dropPrev,
                            inner: dropInner,
                            next: dropNext
                        },
                        enable: true,
                        removeTitle: "remove",
                        renameTitle: "rename",
                        showRemoveBtn: true,
                        showRenameBtn: true
                    },
                    data: {
                        simpleData: {
                            enable: true,
                            pIdKey: "pId"
                        }
                    },
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: true

                    },
                    async: {
                        enable: true,
                        url: url,
                        autoParam: ["id"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            if (parentNode != null) {
                                parentNode.childOuter = false;
                            }
                            return responseData.data;
                        }
                    }
                }, data);
              
            })
        });
    }

    //点击节点将节点name显示在文本框上
    function zTreeOnClick(event, Id, node) {
        $(".btnck").text(node.directoryName);
        $(".btnck").attr("term", node.directoryId);
        $("#dropMenu").attr("aria-expanded", true).parent().addClass(".open");
        $(".dirUl").slideUp();

    };


    //弹窗分类目录
    //$('#myModal-newdir').on('show.bs.modal', function (e) {



    //});
    var folderTree
    //分类新增/修改
    $('#myModal-newdir').on('shown.bs.modal', function () {
        var $li;
        if (dirtype == 0) {
            $.ajax({
                type: "post",
                url: "/NewsManagement/GetNewsTypeList",
                dataType: "json",
                success: rsHandler(function (data) {

                    folderTree = $.fn.zTree.init($("#model_catalogue"), $.extend({
                        callback: {
                            beforeClick: function (id, node) {
                                return true;
                            },
                            onClick: zTreeOnClick,
                            onExpand: function (e, id, node) {
                                var n = folderTree.getNodeByParam("id", dirinfo.directoryId);
                                folderTree.removeNode(n);
                            },
                            onNodeCreated: function (e,id,node) {
                                if (node.name.length > 10) {
                                    $("#" + node.tId + "_span").text(node.name.substring(0, 10) + "...");
                                }
                            }
                        }
                    },
                  {
                      view: {
                          showIcon: false,
                          showLine: false
                      },
                      async: {
                          enable: true,
                          url: "/NewsManagement/GetNewsTypeList",
                          autoParam: ["id"],
                          dataFilter: function (treeId, parentNode, responseData) {

                              return responseData.data;
                          }
                      }
                  }), data);
                    update();
                    $li = $("<li style='height:19px;cursor:pointer;padding: 5px 0px 0px 15px;margin-bottom: 5px;'>无</li>");
                    $("#model_catalogue").prepend($li);
                    //默认没有上级内容
                    $li.click(function () {

                        $(".btnck").text("无");
                        $(".btnck").attr("term", "");
                        $(".dirUl").slideUp();
                    });
                })
            });
        }
        else {
            $.ajax({
                type: "post",
                url: "/NewsManagement/GetNoticeTypeList",
                dataType: "json",
                success: rsHandler(function (data) {
                  
                    folderTree = $.fn.zTree.init($("#model_catalogue"), $.extend({
                        callback: {
                            beforeClick: function (id, node) {
                                return true;
                            },
                            onClick: zTreeOnClick,
                            onExpand: function (e, id, node) {
                                var n = folderTree.getNodeByParam("id", dirinfo.directoryId);
                                folderTree.removeNode(n);
                            },
                            onNodeCreated: function (e, id, node) {
                                if (node.name.length > 10) {
                                    $("#" + node.tId + "_span").text(node.name.substring(0, 10) + "...");
                                }
                            }
                        }
                    },
                  {
                      view: {
                          showIcon: false,
                          showLine: false
                      },
                      async: {
                          enable: true,
                          url: "/NewsManagement/GetNoticeTypeList",
                          autoParam: ["id"],
                          dataFilter: function (treeId, parentNode, responseData) {
                              return responseData.data;
                          }
                      }
                  }), data);
                    update();
                    $li = $("<li style='height:19px;cursor:pointer;padding: 5px 0px 0px 15px;margin-bottom: 5px;'>无</li>");
                    $("#model_catalogue").prepend($li);
                    //默认没有上级内容
                    $("#model_catalogue li:eq(0)").click(function () {

                        $(".btnck").text("无");
                        $(".btnck").attr("term", "");
                        $(".dirUl").slideUp();
                    });
                })
            });
        }

        $("#dropMenu").off("click");
        $("#dropMenu").click(function () {
            $(".dirUl").slideToggle();
        });
        //鼠标失焦事件
        $(".dirUl").click(function (e) {
            e.stopPropagation();
        });

        $(document).click(function () {
            $(".dirUl").slideUp();
        })

        function update()
        {
            //修改赋值
            if (dirinfo.directoryId != 0) {
                $("#input_filename").val(dirinfo.directoryName);
                $(".btnck").attr("term", dirinfo.parentDirectory);
                $(".btnck").text(parentName);
                $("#newdir_modal_label").text("修改分类");
                var n = folderTree.getNodeByParam("id", dirinfo.directoryId);
                folderTree.removeNode(n);

            }
            else {
                $("#input_filename").val("");
                $(".btnck").attr("term", "");
                $(".btnck").text("");

            }
        }


        //确定按钮
        $("#dir_modal_submit").off("click");
        $("#dir_modal_submit").click(function () {
            dirinfo.directoryName = $("#input_filename").val();
            dirinfo.parentDirectory = $(".btnck").attr("term");

            if (dirinfo.directoryName == "") {
                ncUnits.alert("分类名称不能为空");
                return;
            }

            if (dirtype == 0) {
                $.ajax({
                    type: "post",
                    url: "/NewsManagement/SaveNewsType",
                    dataType: "json",
                    data: { data: JSON.stringify(dirinfo) },
                    success: rsHandler(function (data) {
                        if ($("#newdir_modal_label").text() == "新建分类")
                        {
                            ncUnits.alert("新建成功");
                        } else {
                            ncUnits.alert("修改成功");
                         }
                       
                        $("#myModal-newdir").modal("hide");

                        loaddirectorytree("/NewsManagement/GetNewsTypeList", null, 0);

                       //更新右侧目录
                        loadrigthtreeNew();
                      

                    })
                });

            }
            else {
                $.ajax({
                    type: "post",
                    url: "/NewsManagement/SaveNoticeType",
                    dataType: "json",
                    data: { data: JSON.stringify(dirinfo) },
                    success: rsHandler(function (data) {

                        if ($("#newdir_modal_label").text() == "新建分类") {
                            ncUnits.alert("新建成功");
                        } else {
                            ncUnits.alert("修改成功");
                        }

                        $("#myModal-newdir").modal("hide");

                        loaddirectorytree("/NewsManagement/GetNoticeTypeList", null, 1);
                        //更新右侧目录
                      
                        loadrigthtreeNotice();
                    })
                });
            }


        });
        $(".modal_close").click(function () {
            $(".dirUl").slideUp();
            $("#myModal-newdir").modal('hide');
        });
    });
    //模态框关闭事件
    $("#myModal-newdir").on("hide.bs.modal", function () {
        $("#input_filename").val("");
        $(".btnck").attr("term", "");
        $(".btnck").text("");
        $(".dirUl").slideUp();
        $("#newdir_modal_label").text("新建分类");
    });



    var page;
    //加载新闻列表
    function loadNews(url) {
        //加载图片
        var auth_lodi = getLoadingPosition('.listcommen');  //加载局部视图的对象
        if (argus.type == 0) {
            $("#bitneno").attr("term", "0");
            $("#bitneno").text("新闻");
            $(".News .newsicon").addClass("newsiconHit");
            $(".Notice .noticeicon").removeClass("noticeiconHit");
            $(".Notice a").css("color", "#686868");
            $(".News a").css("color", "#6AB674");
        }
        else if (argus.type == 1) {
            $(".News .newsicon").removeClass("newsiconHit");
            $(".Notice .noticeicon").addClass("noticeiconHit");
            $(".News a").css("color", "#686868");
            $(".Notice a").css("color", "#6AB674");
            $("#bitneno").attr("term", "1");
            $("#bitneno").text("通知");
        }
     
       
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data: {
                currentPage: argus.currentPage, dirId: argus.directoryId
            },
            complete: rcHandler(function () {
                auth_lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                if (data.length > 0) {
                    $(".pages").css("display", "block");
                    if (data[0].pageCount == 0) {
                        $("#pagecount").text("0");
                    }
                    else {
                        //分页列表总条数
                        $("#pagecount").text(data[0].pageCount);
                    }


                    //新闻/通知路径
                    //$(".pages span:last").attr("title", url);
                    commUrl = url;
                   
                    //每页条数
                    var num = $("#num").text();
                    //分页总页数
                    page = data[0].pageCount % num == 0 ? data[0].pageCount / num : parseInt(data[0].pageCount / num) + 1;

                    if (argus.currentPage == 1) {
                        $(".prevP").addClass("prevPHit");
                        if (page == 1) {
                            $(".prevP").addClass("prevPHit");
                            $(".nextP").addClass("nextPHit");
                        } else {
                            $(".nextP").removeClass("nextPHit");
                        }

                    } else if (argus.currentPage == page) {
                        $(".nextP").addClass("nextPHit");
                    }
                   else{
                        $(".prevP").removeClass("prevPHit");
                        $(".nextP").removeClass("nextPHit");
                    }
                    var $news = $(".listcommen");
                    var $table = $("<table class='table-condensed newslist' style='width:820px;'><tr><td><input class='ckOne' type='checkbox'term='0'></td>" +
                    "<td><span>栏目名称</span></td><td><span>所属分类</span></td><td><span>创建时间</span></td>" +
                    "<td><span>修改时间</span></td><td><span>操作选项</span></td></tr></table>");
                    var mode = 0;
                    $news.empty();
                    $.each(data, function (i, v) {
                        var $tr = $("<tr></tr>");
                        var $tdCk = $("<td style='width:4%;'><input type='checkbox' term=" + v.newId + "></td>");
                        var $td = $("<td style='width:45%;'></td>");
                        var $title;
                        if (v.title.length > 18) {
                            $title = $("<div style='margin-right:90px;'><a href='/NewsDel/NewsDel?id=" + v.newId + "'>" + v.title.substring(0, 18) + "..." + "</a></div>");
                        }
                        else {
                            $title = $("<div style='margin-right:90px;'><a href='/NewsDel/NewsDel?id=" + v.newId + "'>" + v.title + "</a></div>");
                        }

                        var $span = "";
                        if (v.isTop) {
                            $span = $("<span class='clickTop' style='cursor:pointer;'>置顶</span>");
                        }
                        else {
                            $span = $("<span class='gray'style='cursor:pointer;'>置顶</span>");
                        }
                        $title.append($span).appendTo($td);

                        var $directoryName;
                        if (v.directoryName.length > 4)
                        {
                            $directoryName = $("<td style='width:10%;'><span title='" + v.directoryName + "'>" + v.directoryName.substring(0, 5) + ".." + "</span></td>");
                        } else
                        {
                            $directoryName = $("<td style='width:10%;'><span title='" + v.directoryName + "'>" + v.directoryName + "</span></td>");
                        }
                       
                        var $createTime = $("<td style='width:10%;'><span>" + v.createTime.toString().substring(0, v.createTime.toString().indexOf('T')) + "</span></td>");
                        var $updateTime = $("<td style='width:10%;'><span>" + v.updateTime.toString().substring(0, v.createTime.toString().indexOf('T')) + "</span></td>");
                        var $operate = $("<td style='width:21%;'></td>");
                        var $opspan = $("<span class='sOne'></span>");
                        var $update = $("<a name='edit' href='#addnews' role='tab' data-toggle='tab' style='cursor: pointer;'>修改</a>");
                        $delete = $("<a href='#'style='cursor: pointer;'>删除</a>");
                        $publish = "";
                        if (v.publish) {
                            $publish = $("<a style='cursor: pointer;'>取消发布</a>");
                        }
                        else {
                            $publish = $("<a style='cursor: pointer;'>发布</a>");
                        }
                        $tr.append([$tdCk, $td, $directoryName, $createTime, $updateTime]);
                        $tr.append($operate);
                        $opspan.append([$update, $delete, $publish]).appendTo($operate);
                        $table.append($tr).appendTo($news);

                        //删除加载
                        $delete.click(function () {
                            ncUnits.confirm({
                                title: '提示',
                                html: '确认要删除？',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                    newids.splice(0, newids.length);
                                    newids.push(v.newId);
                                    $.ajax({
                                        type: "post",
                                        url: "/NewsManagement/DeleteNews",
                                        dataType: "json",
                                        data: { data: JSON.stringify(newids) },
                                        success: rsHandler(function (data) {
                                            //$(this).parents("tr").remove();
                                            loadNews(url);
                                           
                                        })
                                    });
                                }
                            });
                        });
                        //修改加载
                        $update.click(function () {
                            $.ajax({
                                type: "post",
                                url: "/NewsManagement/GetNewsInfo",
                                data: {newId:v.newId},
                                dataType: "json",
                                success: rsHandler(function (data) {
                                    info = data;
                                    $(".enterTitle").val(info.title);
                                    $(".enterTitle").attr("term", info.newId);
                                    if (info.contents == "null") {
                                        $("#editor").wysiwyg().html("");
                                    } else {
                                        $("#editor").wysiwyg().html(info.contents);
                                    }
                                  
                                    if (info.notice == true) {
                                        $("#bitneno").attr("term", "1");
                                        $("#bitneno").text("通知");
                                    }
                                    else {
                                        $("#bitneno").attr("term", "0");
                                        $("#bitneno").text("新闻");
                                    }
                                    $("#dirname").attr("term", info.directoryId);
                                    if (info.directoryName.length > 5) {
                                        $("#dirname").text(info.directoryName.substring(0,5)+".");
                                    } else {
                                        $("#dirname").text(info.directoryName);
                                    }
                                   
                                    if (info.isTop == true) {
                                        $("#setTop").prop("checked",true);
                                    }
                                    else {
                                        $("#setTop").prop("checked",false);

                                    }
                                    if (info.keyword ==null||info.keyword=="") {
                                        $("#kword").val("");
                                    } else {
                                        $("#kword").val(info.keyword.join(' '));
                                    }
                                  
                                    if (info.summary == ""||info.summary==null) {
                                        $("#disc").val("");
                                    } else {
                                        $("#disc").val(info.summary);
                                    }
                                 
                                    $("#titleimg").attr("term", info.titleImage);
                                    $(".newname").text("修改新闻");
                                })
                            });
                        });
                        //取消发布加载
                        $publish.click(function () {
                            newids.splice(0, newids.length);
                            newids.push(v.newId);
                            if ($(this).text() == "发布") {
                                $(this).text("取消发布");
                                $.ajax({
                                    type: "post",
                                    url: "/NewsManagement/PublishNews",
                                    dataType: "json",
                                    data: { data: JSON.stringify(newids) },
                                    success: rsHandler(function (data) {
                                        loadNews(url);
                                    })
                                });
                            } else if ($(this).text() == "取消发布") {
                                ncUnits.confirm({
                                    title: '提示',
                                    html: '确认要取消发布？',
                                    yes: function (layer_confirm) {
                                        layer.close(layer_confirm);
                                        $.ajax({
                                            type: "post",
                                            url: "/NewsManagement/UnPublishNews",
                                            dataType: "json",
                                            data: { data: JSON.stringify(newids) },
                                            success: rsHandler(function (data) {
                                                loadNews(url);
                                               
                                            })
                                        });
                                    }
                                });
                             
                            }

                        });
                        //取消置顶
                        $span.click(function () {
                            newids.splice(0, newids.length);
                            newids.push(v.newId);
                            if ($(this).hasClass("clickTop")) {
                               
                                ncUnits.confirm({
                                    title: '提示',
                                    html: '确认要取消置顶？',
                                    yes: function (layer_confirm) {
                                        layer.close(layer_confirm);
                                        $.ajax({
                                            type: "post",
                                            url: "/NewsManagement/SetUnTopNews",
                                            dataType: "json",
                                            data: { data: JSON.stringify(newids) },
                                            success: rsHandler(function (data) {
                                                $(this).removeClass("clickTop");
                                                $(this).addClass("gray");
                                                loadNews(url);
                                            })
                                        });
                                    }
                                });
                            } else {
                                if (!v.publish) {
                                    ncUnits.alert("新闻处于未发布状态,不允许置顶");
                                    return;
                                }
                                $(this).removeClass("gray");
                                $(this).addClass("clickTop");
                                $.ajax({
                                    type: "post",
                                    url: "/NewsManagement/SetTopNews",
                                    dataType: "json",
                                    data: { data: JSON.stringify(newids) },
                                    success: rsHandler(function (data) {
                                        loadNews(url);
                                    })
                                });
                            }

                        });

                    });
                    //全选
                    $(".ckOne").click(function () {

                        if ($(this).is(":checked")) {
                            $(".newslist input[type='checkbox']").prop("checked", true);
                        }
                        else {

                            $(".newslist input[type='checkbox']").prop("checked", false);
                        }
                    });

                    $(".newslist input[type='checkbox']").click(function () {
                       
                        $(this).each(function () {
                            if ($(this).is(':checked'))
                            {
                                var flag = true;
                                var checks = $(".newslist input[type='checkbox']");
                                checks.each(function (i, v) {
                                    if (i > 0 && !$(this).prop("checked")) {
                                        flag = false;
                                    }
                                });
                                if (flag) {
                                    $(".ckOne").prop("checked", true);
                                }
                            }
                            else
                            {
                                $(".ckOne").prop("checked", false);
                            }
                        });
                    });

                    //添加新闻样式
                    $("[name='edit'],.add").click(function () {
                        $(".titleUl li").removeClass("active");
                        $(".titleUl li:eq(2)").addClass("active");
                        addview("none", "none", "block");
                    });
                  
                }
                else {
                    //alert("该分类下没有新闻数据");
                    $(".listcommen table").remove();

                    $(".listcommen").text("该分类下没有数据");
                    $(".pages").css("display", "none");
                   
                }
            })

        });
    

    }
    //$(".current").keydown(function () {

    //    $(this).val($(this).val().replace(/\D/g, ''));


    //});
    //分页加载
    function loadpage(pageUrl, currentpage) {
     
                if (currentpage == 1) {
                    $(".prevP").addClass("prevPHit");
                    $(".nextP").removeClass("nextPHit");

                } else if (currentpage == page) {
                    $(".nextP").addClass("nextPHit");
                    $(".prevP").removeClass("prevPHit");
                } else {
                    $(".prevP").removeClass("prevPHit");
                    $(".nextP").removeClass("nextPHit");
                }
                argus.currentPage = currentpage;
                loadNews(pageUrl);
          
    }
    //点击上一页加载
    $(".prevP").click(function () {
        if (argus.currentPage == 1) {
            return false;
        }
        loadpage(commUrl, argus.currentPage - 1);
        $(".current").val(argus.currentPage);
    });
    //点击下一页加载
    $(".nextP").click(function () {
        if (argus.currentPage == page) {
            return false;
        }
        loadpage(commUrl, parseInt(argus.currentPage) + 1);
      
        $(".current").val(parseInt(argus.currentPage));
    });
    //手动输入页码加载
    $(".current").keyup(function (e) {
        $(this).val($(this).val().replace(/[^\d]/g, ''));
        if (e.keyCode == 13) {
            if ($(this).val() > page || $(this).val() < 1) {
                $(this).val(argus.currentPage);
                //loadpage(commUrl, 1);
                ncUnits.alert("抱歉，没有该页数");
                return false;
            }
            loadpage(commUrl, $(this).val());
        }

    });
    //只能填数字的验证
    $(".current").keydown(function () {
        //$(this).select();
    });
    $(".current").focus(function () {
        $(this).select();
    });
    $(".newname").text("添加新闻");
    //全选分类
    $(".ckTwo").click(function () {
        directoryFolderTree.expandAll(true);
        if ($(this).is(':checked')) {
            directoryFolderTree.checkAllNodes(true);
        }
        else {
            directoryFolderTree.checkAllNodes(false);
        }

    });
    //预览
    $("#auty-preview").click(function () {
        $("#Modal-preview").modal('show');
       // $("html").css("overflow","hidden");
        $(".newTitle").text($(".enterTitle").val());
        $(".numLook").text("查看次数:1");
        $(".newDetailDisplay").empty();
        $html=$("#editor").wysiwyg();
        $("#editor img").each(function () {
            if ($(this).width() > 700)
            {
                width=$(this).width() / 2;
                height = $(this).height() / 2;
                $(this).width(width);
                $(this).height(height);
            }
          
        });
        $(".newDetailDisplay").html($html.html());
        $(".newtype").text("类型:" + $("#bitneno").text() + "/" + $("#dirname").text());
        $(".chkey").text("中文关键字:" + $("#kword").val());
        $(".chdiscr").text("中文描述:" + $("#disc").val());

    });
    //点击关闭新闻预览
    $("#Modal-preview").on('show.bs.modal', function () {
        $(".closeview").click(function () {
            $("#Modal-preview").modal('hide');
        });
    });
   
    //模态框显示（上传图片）
    $("#titleimg").click(function () {
        $("#modal-titlephoto").modal('show');
        photoLoad();
       
    });
    $('#modal-titlephoto').on("shown.bs.modal", function () {
        //默认选中图片
        var src = $("#titleimg").attr("term");
        if (src != "") {
            $(".photochoose img").filter(
                function () {
                    if ($(this).attr("src") == src) {
                        var $choose = $(this).parent().find(".choose");
                        $choose.addClass("chooseHit");
                        $choose.find("span").addClass("spanHit");
                        return true
                    } 
                })
           
          
          
        }
    });
    //加载图片列表

    function photoLoad() {
        $.ajax({
            type: "post",
            url: "/NewsManagement/GetImageList",
            dataType: "json",
            success: rsHandler(function (data) {
                var $documents = $(".photochoose");
                $documents.empty();

                $.each(data, function (i, v) {

                    var $content = $("<div class='col-xs-3'></div>");
                    var $img = $("<img  class='img-rounded' src='" + v.imageUrl + "'> </div>");
                    var $operate = $("<div class='operate'></div>");
                    var $choose = $("<div class='xxc_choose choose' term='" + v.imageId + "' style='display:block;'><span></span></div>");
                    var $operateDiv = $("<div class='operateDiv'></div>");
                    var $operateBg = $("<span class='operateBg'></span>");
                    var $operateText = $("<div class='operateText'></div>");
                    var $ul = $("<ul></ul>");
                    var $check = $("<li class='checks'></li>");
                    var $delete = $("<li class='delete'></li>");
                    $ul.append($delete, $check);
                    $operateText.append($ul);
                    $operateDiv.append($operateBg, $operateText);
                    $operate.append($operateDiv);
                    $content.append($img, $operate, $choose);


                    $content.appendTo($documents);
                    /* 绿条hover效果开始 */
                    $content.hover(
                        function () {
                            $(this).find('.operate').css('display', 'block');
                            //当鼠标放上去的时候,程序处理
                        },
                        function () {
                            $(this).find('.operate').css('display', 'none');
                            //当鼠标离开的时候,程序处理
                        });
                    /* 绿条hover效果 结束 */
                    /* 点击绿条中的详情 预览图片  开始 */
                    $check.click(function () {
                        var $imgview = $("<ul><li><img  class='view' src='" + v.imageUrl + "'> </li><li><span class='closepic'></span></li></ul>");
                        $("#photoviewModal").empty();
                        $("#photoviewModal").append($imgview);
                        $("#photoviewModal").modal('show');
                    });
                    /* 点击绿条中的详情 预览图片  结束 */
                   
                    /* 删除 开始 */
                    $delete.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '你确认要删除这个图片吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                imgId = v.imageId;
                                //hai需要模块id   moduleId
                                $.ajax({
                                    type: "post",
                                    url: "/NewsManagement/DeleteImage",
                                    dataType: "json",
                                    data: { imgId: imgId },
                                    success: rsHandler(function (data) {
                                        photoLoad();//删除后，根据数据库重新加载页面
                                        ncUnits.alert("删除成功!");

                                    })
                                });
                            },
                            no: function (layer_confirm) {      //修改
                                layer.close(layer_confirm);
                            }
                        });
                    });
                    /* 删除 结束 */

                    //批量勾选 开始
                    $choose.click(function () {
                        if ($(this).hasClass('chooseHit')) {
                            $(".choose").removeClass("chooseHit");
                            $(".col-xs-3 span").removeClass("spanHit");
                            //$(this).removeClass('chooseHit');
                            //$('span', this).removeClass('spanHit');
                        }
                        else {
                            $(".chooseHit").removeClass("chooseHit");
                            $(".col-xs-3 span").removeClass("spanHit");
                            $(this).addClass('chooseHit');
                            $('span', this).addClass('spanHit');
                            imgUrl = $(this).parents(".col-xs-3").find("img").attr("src");
                        }
                    });
                    // 批量勾选 结束


                })

            })
        });
    };
    //点击关闭预览图片
    $("#photoviewModal").on("show.bs.modal", function () {
        $(".closepic").click(function () {
            $("#photoviewModal").modal('hide');

        });
    });
    //勾选图片确定
    $("#img_submit").click(function () {
        if ($(".chooseHit").hasClass("chooseHit")) {
            $("#titleimg").attr("term", imgUrl);
            $("#modal-titlephoto").modal('hide');
        }
        else {
            ncUnits.alert("未勾选图片");
        }

    });
    //勾选图片清除全部
    $("#clear-submit").click(function () {
        $(".chooseHit").removeClass("chooseHit");
        $(".col-xs-3 span").removeClass("spanHit");
    });

    /* 确认上传 开始 */
    var lay_affirm;
    $('.submit').on('click', function () {
        lay_affirm = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.affirmPopUp' },
            move: ".drapdiv",
            closeBtn: false
        });
        fnPopUpHeight($('.affirmPopUp'));
        $('.uploadBox').hide();


        $(".affirmPopUp .closeWCom").click(function () {
            layer.close(lay_affirm);
            $(".uploadBox").hide();
        })
    });

    $(".uploadBtn").click(function () {
        $('.xubox_layer').css({ 'margin-left': '-481px', 'width': '962px' });

        $(".uploadBox").show();
    });
    $('.finishTime input').blur(function () {
        var eachTime = $('#time').val();
        var numberOfPlan = $('#num').val();
        $('.allTime').html(eachTime * numberOfPlan);
    });
    function showDelete() {
        $('.uploadBox .file_upload .file').hover(function () {
            $(this).find('span').css('display', 'inline');
        }, function () {
            $(this).find('span').css('display', 'none');
        });

        //$('.file span').on('click', function () {
        //    var id = $(this).parent().attr('term');
        //    $.ajax({
        //        url: '/Plan/DeletePlanFileById',
        //        type:"POST",
        //        data: { id: id },
        //    });
        //    $(this).parent().parent().remove();
        //});
    }

    var html = '';
    var i = 1;
    var j;
    var k = 0;
    var parttern = /(\.|\/)(jpg|jpeg|tiff|bmp|png|gif)$/i;
    fileUpload();
    //上传图片
    function fileUpload() {
        $('#fileupload').fileupload({
            url: '/NewsManagement/InsertNewsImage',
            dataType: 'text',
            //acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
            //acceptfiletypes: ".jpg",
            //maxFileSize: 5000,
            add: function (e, data) {
                layer.closeTips();
                var isSubmit = true;

                $.each(data.files, function (index, value) {
                    if (!parttern.test(value.name)) {
                        ncUnits.alert("你上传文件格式不对");
                        //ncUnits.confirm({
                        //    title: '提示',
                        //    html: '你上传文件格式不对。'
                        //});
                        isSubmit = false;
                        return;
                    } else if (value.size > 209715200) {
                        ncUnits.alert("你上传文件过大(最大200M)");

                        //ncUnits.confirm({
                        //    title: '提示',
                        //    html: '你上传文件过大。'
                        //});
                        isSubmit = false;
                        return;
                    } else {
                        var $fileSpan = $('<span class="file" term="' + index + '"> ' + data.files[index].name + '<span>&nbsp&nbsp&nbsp&nbspx</span></span>');
                        $fileSpan.hover(function () {
                            $(this).find('span').css('display', 'inline');
                        }, function () {
                            $(this).find('span').css('display', 'none');
                        })
                        // ncUnits.alert("上传成功");
                        //file.saveName = data.files;
                        //file.extension = result.data.extension;
                        //    $.ajax({
                        //        type: "post",
                        //        url: "/NewsManagement/AddNewsImage",
                        //        dataType: "json",
                        //        data: { data: JSON.stringify(file) },
                        //        success: rsHandler(function (data) {
                        //            ncUnits.alert("上传成功");
                        //            photoLoad();
                        //        })
                        //    });
                    }


                });

                //showDelete();
                if (isSubmit) {
                    data.submit();
                }

            },
            complete: function (data, flag) {

                $('.up_progress').css('display', 'none');
                $('.files li').addClass('uploaded');

            },
            error: function (e, data) {

                ncUnits.alert('error');

            },
            done: function (e, data) {
                var result = JSON.parse(data.result);
                if (result.data.displayName != null) {
                    ncUnits.alert("上传成功");
                    photoLoad();

                } else {
                    ncUnits.alert("上传文件内容不能为空");
                    $(".file").parent().parent().remove();

                }


                if (data.result.status == 0) {
                    ncUnits.alert("上传失败");
                    $this.parent().parent().remove();

                } else {
                    ncUnits.alert("上传成功");
                    //$('.file').eq(k++).attr('term', $.parseJSON(data._response.result).data[0].attachmentId);
                }
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('.up_progress' + j + '').css('width', progress + '%');
            },
            always: function (e, data) {
            }
        });

    }

    //$('#fileupload').
    /* 确认上传 结束 */

    //取消按钮（添加新闻）
    $("#authy-cancel").click(function () {
        $('#NewsMang').tab('show');
        addview("block", "none", "none");
        if ($("#bitneno").attr("term") == 0) {
            loadNews("/NewsManagement/GetNewsList");
        } else {
            loadNews("/NewsManagement/GetNoticeList");
        }
        clearContent();
    });
    //清空标签内容(添加新闻)
    function clearContent()
    {
        //主题
        $(".enterTitle").val("");
        //新闻Id
        $(".enterTitle").attr("term", "");
        //内容
        $("#editor").wysiwyg().html(" ");
        //类型  新闻/通知
        $("#bitneno").attr("term", "");
        $("#bitneno").text("");
     //分类
        $("#dirname").attr("term","");
        $("#dirname").text("");
        //置顶
        $("#setTop").prop("checked",false);
        //关键字
        $("#kword").val("");
        //中文描述
        $("#disc").val("");
        //主题图片
        $("#titleimg").attr("term", "");
        $(".newname").text("添加新闻");
    }

});
