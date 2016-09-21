//@ sourceURL=document-manage.js
$(function () {

    /*初始化 开始*/
    //文档列表请求所用参数
    var argus = {
        docName: null,
        folder: null,
        time: [],
        person: [],
        sorts: {
            type: 1,
            direct: 1
        }
    };
    var documents = [],
        folders = [],
       xxc_documentId;
    bitchoperateing = false;
    /*初始化 结束*/
    //getAuthorityInfo(1)
    /*右侧目录加载 开始*/
    var rightFolderTree;
    loadrighttree();
    function loadrighttree() {
        $.ajax({
            type: "post",
            url: "/Shared/GetFolderList",
            dataType: "json",
            data: { type: 1, folder: null },
            success: rsHandler(function (data) {
                rightFolderTree = $.fn.zTree.init($("#catalogue"), {
                    callback: {
                        onClick: function (e, id, node) {
                            $("#authority-a").show();
                            argus.folder = node.id;
                            $(".directory-set span").css("color", "#686868");
                            //var setting = {
                            //    async: {
                            //        enable: true,
                            //        url: "../../test/data/plan/plan_getDepartmentOneLevel.json",
                            //        autoParam: [id]
                            //    }
                            //};
                            var n = rightFolderTree.getNodeByParam("id", node.id);
                            rightFolderTree.expandNode(n);
                            rightFolderTree.selectNode(n);
                            if ($("#authority-a").attr("aria-expanded") == "true") {
                                getAuthorityInfo(argus.folder);
                                $(".station_choose").remove();
                                $(".depart_choose").remove();
                                $(".HR_choose").remove();
                            } else {
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/getAllParentIds",
                                    dataType: "json",
                                    data: { documentId: node.id },
                                    success: rsHandler(function (data) {
                                        if (data.length > 0) {
                                            $(".dic_first~span").remove();
                                            for (var i = 0; i < data.length; i++) {
                                                $dirc = $("<span>></span><span class='dirc_span' term='" + data[i].id + "'>" + data[i].name + "</span>");
                                                $(".labelLast").append($dirc);
                                            }
                                            $(".dirc_span:eq(" + data.length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                            $(".dirc_span").off('click');
                                            $(".dirc_span").click(function () {
                                                dircclick($(this));
                                            });
                                        }
                                    })
                                    
                                });
                                loadDocuments();
                            }
                        },
                        onNodeCreated: function (event, treeId, treeNode) {
                            var ztree = $.fn.zTree.getZTreeObj("catalogue");
                            if (treeNode.id == 0) {
                                ztree.removeNode(treeNode);
                            }
                            //var node = rightFolderTree.getNodeByParam('id', 0);
                            //if (node) {
                            //    rightFolderTree.removeNode(node);
                            //}
                            if (treeNode.id == argus.folder) {
                                var n = rightFolderTree.getNodeByParam("id", argus.folder);
                                rightFolderTree.selectNode(n);
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
                        url: "/Shared/GetFolderList",
                        autoParam: ["id=folder"],
                        otherParam: ["type", "1"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }, data);
            })
        });
    }
    $(".dirc_span").off('click');
    $(".dirc_span").click(function () {
        dircclick($(this));
    });
    //导航条点击效果
    function dircclick(obj) {
        if (obj.text() == "目录设置") {
            var n = rightFolderTree.getNodeByParam("id", argus.folder);
            rightFolderTree.cancelSelectedNode(n);
            argus.folder = null;
        } else {
          
            argus.folder = obj.attr("term");
        }
        var index = $(".labelLast span").index(obj);
        obj.addClass("dirc_active").siblings().removeClass("dirc_active");
        $(".dirc_active~span").remove();
        loadDocuments();
        var n = rightFolderTree.getNodeByParam("id", argus.folder);
        rightFolderTree.expandNode(n);
        rightFolderTree.selectNode(n);
    }

    /*右侧目录加载 结束*/

    /*加载文档列表 开始*/
    loadDocuments();
    /*加载文档列表 结束*/
    $("#resource-a").on('shown.bs.tab', function () {
        $("#sure_auth").hide();
        $("#newFile_building").show();
        $("#mode_card").show();
        $("#mode_list").show();
        $("#gofilter").show();
        $(".nav-right .dropdown").show();
        $.ajax({
            type: "post",
            url: "/Shared/getAllParentIds",
            dataType: "json",
            data: { documentId: argus.folder },
            success: rsHandler(function (data) {
                if (data.length > 0) {
                    $(".dic_first~span").remove();
                    for (var i = 0; i < data.length; i++) {
                        $dirc = $("<span>></span><span class='dirc_span' term='" + data[i].id + "'>" + data[i].name + "</span>");
                        $(".labelLast").append($dirc);
                    }
                    $(".dirc_span:eq(" + data.length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                    $(".dirc_span").off('click');
                    $(".dirc_span").click(function () {
                        dircclick($(this));
                    });
                }
            })

        });
        loadDocuments();
    });
    //$("#resource-a").click(function () {

    //});
    /*筛选 开始*/
    $("#gofilter").click(function () {
        //取消当前批量操作 
        cancelBitchOperate();
        $("#filterbox").slideToggle();
        if ($(this).hasClass("active")) { $(this).removeClass("active"); }
        else $(this).addClass("active");
    });
    $("#filterbox_shrink").click(function () {
        $("#filterbox").slideUp();
        $("#gofilter").removeClass("active");
    });

    $("#filterbox_document_name").keyup(function (e) {
        if (e.keyCode == 13) {
            var name = $(this).val().trim();
            if (name.length) {
                addFilter({
                    text: name,
                    type: "document",
                    add: function () {
                        argus.docName = name;
                    },
                    delete: function () {
                        argus.docName = null;
                    }
                });
            }
        }
    });
    $("#filterbox_document_name_submit").click(function () {
        var name = $("#filterbox_document_name").val().trim();
        if (name.length) {
            addFilter({
                text: name,
                type: "document",
                add: function () {
                    argus.docName = name;
                },
                delete: function () {
                    argus.docName = null;
                }
            });
        }
    });

    //员工加载
    $.ajax({
        type: "post",
        url: "/DocumentManagement/GetLastFiveCreateUser",
        dataType: "json",
        success: rsHandler(function (data) {
            $.each(data.reverse(), function (i, v) {
                var $staff = $("<a href='#' class='option'>" + v.name + "</a>");
                $("#filterbox_staff").prepend($staff);
                $staff.click(function () {
                    var $this = $(this);
                    if (!$this.hasClass("active")) {
                        addFilter({
                            text: v.name,
                            add: function () {
                                $this.addClass("active");
                                argus.person.push(v.id);
                            },
                            delete: function () {
                                $this.removeClass("active");
                                argus.person = _.without(argus.person, v.id);
                            }
                        });
                    }
                });
            });
        })
    });
    $("#filterbox_select_staff").searchPopup({
        url: "/DocumentManagement/GetLastFiveCreateUser",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            if (_.indexOf(argus.person, data.userId) == -1) {
                addFilter({
                    id:data.id,
                    text: data.name,
                    add: function () {
                        argus.person.push(data.id);
                    },
                    delete: function () {
                        argus.person = _.without(argus.person, data.id);
                    }
                });
            }
        }
    });

    /*时间选择 开始*/
    $("#filterbox_week").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addFilter({
                text: "近一周",
                type: "time",
                add: function () {
                    $this.addClass("active");
                    argus.time = [];
                    argus.time[0] = 1;
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.time = [];
                }
            });
        }
    });
    $("#filterbox_month").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addFilter({
                text: "近一月",
                type: "time",
                add: function () {
                    $this.addClass("active");
                    argus.time = [];
                    argus.time[0] = 2;
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.time = [];
                }
            });
        }
    });
    $("#filterbox_time_submit").click(function () {
        var st = $("#filterbox_starttime").val(),
            et = $("#filterbox_endtime").val(),
            text = (st.length ? "从" + st : "") + (et.length ? "到" + et : "");
        if (text.length) {
            addFilter({
                text: text,
                type: "time",
                add: function () {
                    argus.time = [3, st, et];
                },
                delete: function () {
                    argus.time = [];
                }
            });
        }
    });
    var start = {
        elem: '#filterbox_starttime',
        event: 'click',
        format: 'YYYY-MM-DD',
        istoday: true,
        issure: true,
        festival: true,
        choose: function (dates) {
            end.start = dates;
            end.min = dates;
        },
        clear: function () {
            end.min = null;
        }
    },
        end = {
            elem: '#filterbox_endtime',
            event: 'click',
            format: 'YYYY-MM-DD',
            isclear: true,
            istoday: true,
            issure: true,
            festival: true,
            choose: function (dates) {
                start.max = dates;
            },
            clear: function () {
                start.max = null;
            }
        };
    laydate(start);
    laydate(end);
    /*时间选择 结束*/

    $("#filterbox_clear").click(function () {
        $("#filterbox .selecteds .glyphicon-remove").trigger("click", true);
        $("#filterbox_clear").css({ "color": " #fff", "background-color": "#e99900", "border": "1px solid #fbbf3d" });
        loadDocuments();
    });
    /*筛选 结束*/

    /*列表模式切换事件绑定 开始*/
    //$("#mode_card").click(function () {
    //    $(this).addClass("active");
    //    //$("#mode_list").removeClass("active")
    //});
    // $("#mode_list").click(function () {
    //$(this).addClass("active");
    //$("#mode_card").removeClass("active")
    //});
    /*列表模式切换事件绑定 结束*/

    /*排序条绑定事件 开始*/
    $(".sortbar").sortBar();
    $(".sortbar").on("clicked.sortbar", function (e, order, sort) {
        argus.sorts.type = order;
        argus.sorts.direct = sort;
        loadDocuments();
    });
    /*排序条绑定事件 结束*/

    /*复制事件 开始*/
    $("#copy_modal_submit").click(function () {
        if (folders.length > 0) {
            for (var i = 0; i < folders.length; i++) {
                if (folders[i] == 0) {
                    folders[i] = null;
                }
            }
        };
        var copymodel = { documentType: 1, documentId: documents, companyFolder: folders, userFolder: [], withAuth: false };
        $.ajax({
            type: "post",
            url: "/Shared/Copy",
            dataType: "json",
            data: {
                data: JSON.stringify(copymodel)
            },
            success: rsHandler(function (data) {
                if (data == true) {
                    loadrighttree();
                    loadDocuments();
                    $("#copy_modal").modal("hide");
                    $("#copy_select").val('');
                    $("#copy_modal_chosen_count").text(0);
                    $("#copy_modal_chosen li").remove();
                    ncUnits.alert("复制成功");

                }
                else {
                    $("#copy_modal").modal("hide");
                    $("#copy_select").val('');
                    $("#copy_modal_chosen_count").text(0);
                    $("#copy_modal_chosen li").remove();
                    ncUnits.alert("文件不存在，复制失败");
                }

            })
        });
    })
    $("#copy_modal_cancel").click(function () {
        $("#copy_select").val('');
        $("#copy_modal_chosen_count").text(0);
        $("#copy_modal_chosen li").remove();
    });
    $("#copy_modal .close").click(function () {
        $("#copy_select").val('');
        $("#copy_modal_chosen_count").text(0);
        $("#copy_modal_chosen li").remove();
    });

    /*复制事件 结束*/

    /*批量事件 开始*/
    $('.batch-menu li').click(function () {
        $(".cell").off("hover");
        $(".cell").hover(function () {
            $(".operate", this).hide();
        });
        if ($("#authority-a").parent().hasClass("active")) {
            return;
        }
        $('.isfolder').css('cursor', 'default');
        bitchoperateing = true;
        $(".btns").hide();
        $('.moreBg').text($(this).text()).show();
        $('.moreCancel').show();
        $('.xxc_choose').addClass('choose').removeClass('prohibit').show();
    });

    $('.moreCancel').click(function () {
        $(".cell").hover(function () {
            $(".operate", this).show();
        }, function () {
            $(".operate", this).hide();
        });
        $(".btns").show();
        $(this).hide();
        $('.isfolder').css('cursor', 'pointer');
        bitchoperateing = false;
        $('.moreBg').hide();
        $('.xxc_choose').removeClass('choose').removeClass('prohibit');
        $('.xxc_choose span').removeClass('spanHit');
        $('.chooseHit').removeClass('chooseHit');
    });
    $('.moreBg').click(function () {
        if (documents.length <= 0) {
            ncUnits.alert("请选择文档进行操作");
            return;
        }
        var operate = $.trim($(this).text());
        if (operate == "移动") {
            $(this).attr('data-toggle', 'modal');
            $(this).attr('data-target', '#move_modal');
            $("#move_modal .modal-content").load("/Shared/GetMoveList", function () {
                $(".cell").hover(function () {
                    $(".operate", this).show();
                }, function () {
                    $(".operate", this).hide();
                });
                bitchoperateing = false;
                $('.isfolder').css('cursor', 'pointer');
                $(".btns").show();
                $('.moreCancel').hide();
                $('.moreBg').hide();
                $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                $('.xxc_choose span').removeClass('spanHit');
                buildDepartmentTree();
                //选择文件夹
                $("#move_modal_search").selection({
                    url: "/Shared/GetLikeFolderList",
                    hasImage: false,
                    otherParam: { type: 1 },
                    selectHandle: function (data) {
                        var ztree = $.fn.zTree.getZTreeObj("move_modal_folder");
                        if (documents.indexOf(data.id) >= 0) {
                            return;
                        }
                        $("#move_select").val(data.name);
                        var n = ztree.getNodeByParam("id", data.id);
                        if (n) {
                            ztree.checkNode(n);
                        }
                        var flag = true;
                        if ($("#move_modal_chosen li").length > 0) {
                            $("#move_modal_chosen li").each(function () {
                                if ($(this).attr('term') == data.id) {
                                    flag = false;
                                }
                            });
                        }
                        if (flag == true) {
                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                       $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $("#move_modal_chosen").append($checked.append($close));
                            folders = [];
                            folders.push(data.id);
                            $close.click(function () {
                                var nodeId = $(this).parent().attr("term");
                                n = ztree.getNodeByParam("id", parseInt(nodeId));
                                if (n) {
                                    ztree.checkNode(n);
                                }
                                $(this).parent().remove();
                                $("#move_modal_chosen_count").text($("#move_modal_chosen li").length);
                            });
                        }
                        $("#move_modal_chosen_count").text($("#move_modal_chosen li").length);
                    }
                });
            });
        }
        else if (operate == "复制") {
            folders = [];
            $('#copy_modal').modal('show');
            $('#move_modal').modal('hide');
            $(".cell").hover(function () {
                $(".operate", this).show();
            }, function () {
                $(".operate", this).hide();
            });
            bitchoperateing = false;
            $(".btns").show();
            $('.isfolder').css('cursor', 'pointer');
            $('.moreCancel').hide();
            $('.moreBg').hide();
            $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            $('.xxc_choose span').removeClass('spanHit');
            $.ajax({
                type: "post",
                url: "/Shared/GetFolderList",
                dataType: "json",
                data: { type: 1, folder: null },
                success: rsHandler(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $.each(documents, function (e, j) {
                            if (j == data[i].id) {
                                data.splice(i, 1);
                            }
                        });
                    }
                    var folderTree = $.fn.zTree.init($("#copy_modal_folder"), $.extend({
                        callback: {
                            beforeClick: function (id, node) {
                                folderTree.checkNode(node, undefined, undefined, true);
                                return false;
                            },
                            onCheck: function (e, id, node) {
                                $("#copy_modal_chosen_count").html(folderTree.getCheckedNodes().length);
                                if (node.checked) {
                                    var $checked = $("<li>" + node.name + "</li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $("#copy_modal_chosen").append($checked.append($close));
                                    $close.click(function () {
                                        folderTree.checkNode(node, undefined, undefined, true);
                                    });
                                    node.mappingLi = $checked;
                                    folders.push(node.id);
                                } else {
                                    node.mappingLi.remove();
                                    folders = _.without(folders, node.id);
                                }
                            },
                            onNodeCreated: function (e, id, node) {
                                var folderTree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                                $("#copy_modal_chosen li").each(function () {
                                    var departId = $(this).attr('term');
                                    if (parseInt(departId) == node.id) {
                                        folderTree.checkNode(node);
                                    }
                                });
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false
                        },
                        check: {
                            enable: true,
                            chkStyle: "checkbox",
                            chkboxType: { "Y": "", "N": "" }
                        },
                        async: {
                            enable: true,
                            url: "/Shared/GetFolderList",
                            autoParam: ["id=folder"],
                            otherParam: ["type", "1"],
                            dataFilter: function (treeId, parentNode, responseData) {
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            });
            //选择文件夹
            $("#copy_modal_search").selection({
                url: "/Shared/GetLikeFolderList",
                hasImage: false,
                otherParam: { type: 1 },
                selectHandle: function (data) {
                    var ztree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                    $("#copy_select").val(data.name);
                    var n = ztree.getNodeByParam("id", data.id);
                    if (n) {
                        ztree.checkNode(n);
                    }
                    var flag = true;
                    if ($("#copy_modal_chosen li").length > 0) {
                        $("#copy_modal_chosen li").each(function () {
                            if ($(this).attr('term') == data.id) {
                                flag = false;
                            }
                        });
                    }
                    if (flag == true) {
                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#copy_modal_chosen").append($checked.append($close));
                        folders.push(data.id);
                        $close.click(function () {
                            var nodeId = $(this).parent().attr("term");
                            n = ztree.getNodeByParam("id", parseInt(nodeId));
                            if (n) {
                                ztree.checkNode(n);
                            }
                            $(this).parent().remove();
                            $("#copy_modal_chosen_count").text($("#copy_modal_chosen li").length);
                        });
                    }
                    $("#copy_modal_chosen_count").text($("#copy_modal_chosen li").length);
                }
            });
        }
        else if (operate == "删除") {
            ncUnits.confirm({
                title: '提示',
                html: '确认要删除？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $.ajax({
                        type: "post",
                        url: "/DocumentManagement/DeleteFlagDocument",
                        dataType: "json",
                        data: { data: JSON.stringify(documents) },
                        success: rsHandler(function (data) {
                            if (data == "ok") {
                                var ztree = $.fn.zTree.getZTreeObj("catalogue");
                                for (var i = 0; i < documents.length; i++) {
                                    var n = ztree.getNodeByParam("id", documents[i]);
                                    if (n) {
                                        ztree.removeNode(n);
                                    }
                                }
                                loadDocuments();
                                //loadrighttree();
                                documents = [];
                                ncUnits.alert("批量删除成功!");
                                cancelBitchOperate();
                            }
                        })
                    });
                }
            });
        }
        else if (operate = "下载") {
            $.post("/DocumentManagement/MultiDownload", { data: JSON.stringify(documents), flag: 0 }, function (data) {
                if (data == "success") {
                    //loadViewToMain("/DocumentManagement/MultiDownload?flag=1");
                    window.location.href = "/DocumentManagement/MultiDownload?flag=1";
                    documents = [];
                }
                else {
                    ncUnits.alert("下载失败!");
                }
                cancelBitchOperate();
                documents = [];
            });
        }


    });
    /*批量事件 结束*/

    /*Function 开始*/
    //加载文档列表
    var mode = 0;
    var partternFile = /(ppt|xls|doc|pptx|xlsx|docx|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    function loadDocuments() {
        var list_lodi = getLoadingPosition('.documents');  //加载局部视图的对象
        //取消当前批量操作 
        cancelBitchOperate();
        argus.folder = argus.folder == 0 ? null : argus.folder;
        if (argus.folder == null) {
            $("#authority-a").hide();
            $(".directory-set span").css("color", "#58b456")
        } else {
            $("#authority-a").show();
        }
        $.ajax({
            type: "post",
            url: "/DocumentManagement/GetCompanyDocumentList",
            dataType: "json",
            data: {
                data: JSON.stringify(argus)
            },
            complete:rcHandler(function(){
                list_lodi.remove();
            }),
            success: rsHandler(function (data) {
                if (data == "error") {
                    window.location.href = "/";
                }
                var $documents = $(".documents");
                var $docList = $(".row-docList");
                var $addNewFile = $("");
                $documents.empty();
                var date, time;
                $.each(data, function (i, v) {
                    date = ""; time = "";
                    var $col = $("<div class='col-xs-3'></div>");
                    var $cell = $("<div class='cell'></div>");
                    var desc = "";
                    var $fileName = $("<div class='title' title='" + v.displayName + "'>" + v.displayName + "</div>");
                    if (v.isFolder) {
                        $cell.addClass("folder");
                        desc = $("<dl class='dl-horizontal' title='" + v.description + "'><dd class='dl-horizontal' style='margin-left:0'>" + v.description + "</dd></dl>");
                        $('dd.dl-horizontal').dotdotdot();
                    } else {
                        $cell.addClass("docu");
                        date = v.createTime.toString().substring(0, v.createTime.toString().indexOf('T'));
                        time = v.createTime.toString().substring(v.createTime.toString().indexOf('T') + 1, v.createTime.toString().lastIndexOf(':'));
                        desc = $("<div class=content>" +
                        "<div><span class='content-label'>创建人：</span><span class='content-info'>" + v.createUserName + "</span></div>" +
                        "<div><span class='content-label'>创建时间：</span><span class='content-info'>" + date + "</span></div>" +
                        "</div>");
                    }
                    var $operate = $("<div class='operate'></div>");
                    var $btns = $("<div class='btns'></div>");
                    var $preview = $("<a href='#' class='preview'>预览</a>"),
                        $log = $("<a href='#' data-toggle='modal' data-target='#myModal-log'id='operate_log'>日志</a>"),
                        $copy = $("<a href='#' data-toggle='modal' data-target='#copy_modal'>复制</a>"),
                        $move = $("<a href='#' data-toggle='modal' data-target='#move_modal'id='operate_move'>移动</a>"),
                        $delete = $("<a href='#'>删除</a>"),
                        $download = $("<a href='#'>下载</a>");
                    var $choose = $("<div class='xxc_choose' term=" + v.documentId + "><span></span></div>");
                    if (!v.isFolder && partternFile.test(v.extension)) {
                        $btns.append($preview);
                    }
                    $btns.append([$log, $copy, $move, $delete, $download]).appendTo($operate);

                    //$btns.append([$log, $copy, $move, $download]).appendTo($operate);
                    $cell.append([$("<div class='grayTop'></div>"), $fileName, desc, $operate, $choose]);

                    $col.append($cell).appendTo($documents);
                    if (mode == 0) {
                        $documents.children().removeClass("col-xs-12").addClass("col-xs-3").removeClass("row-docList").addClass("card").addClass("document");
                        $(".grayTop").hide();
                        $(".dl-horizontal").show();
                        $(".content-label").show();
                    } else {
                        $documents.children().removeClass("col-xs-3").addClass("col-xs-12").removeClass("card").addClass("row-docList").removeClass("document");
                        $(".grayTop").show();
                        $(".dl-horizontal").hide();
                        $(".content-label").hide();
                    }
                    $cell.hover(function () {
                        $operate.toggle();
                    })
                    $("#mode_list").off("click")
                    $("#mode_list").click(function () {
                        $(".newadd").remove();
                        $("#mode_card").removeClass("active");
                        $("#mode_list").addClass("active");
                        mode = 1;
                        loadDocuments();
                        //$col.append($cell).appendTo($documents);
                        //$addNewFile = $("<div class='newadd col-xs-3'><div class='add'><div class='addPicture' data-toggle='modal' data-target='#myModal-newfile' style='cursor:pointer'></div><div class='wordPosition'>新建文件夹</div></div></div>");
                        //$documents.append($addNewFile);
                        //$documents.children().removeClass("col-xs-3").addClass("col-xs-12").removeClass("card").addClass("row-docList").removeClass("document");
                        $(".grayTop").show();
                        $(".dl-horizontal").hide();
                        $(".content-label").hide();
                    });
                    $("#mode_card").off("click");
                    $("#mode_card").click(function () {
                        $(".newadd").remove();
                        mode = 0;
                        $("#mode_card").addClass("active");
                        $("#mode_list").removeClass("active");
                        //$col.append($cell).appendTo($documents);
                        //$addNewFile = $("<div class='newadd col-xs-3'><div class='add'><div class='addPicture' data-toggle='modal' data-target='#myModal-newfile' style='cursor:pointer'></div><div class='wordPosition'>新建文件夹</div></div></div>");
                        //$documents.append($addNewFile);
                        loadDocuments();
                        $(".newadd .addPicture").click(function () {
                            addNewFile();
                        });
                        $documents.children().removeClass("col-xs-12").addClass("col-xs-3").removeClass("row-docList").addClass("card").addClass("document");
                        $(".grayTop").hide();
                        $(".dl-horizontal").show();
                        $(".content-label").show();
                    });
                    //$cell.append([$fileName, desc, $operate, $choose]);
                    // $col.append($cell).appendTo($documents);
                    //$(".dl-horizontal").each(function () {
                    //    var maxwidth = 28;
                    //    if ($(this).text().length > maxwidth) {
                    //        $(this).text($(this).text().substring(0, maxwidth));
                    //        $(this).html($(this).html() + '…');
                    //    }
                    //});
                    $preview.click(function () {
                        preview(2, v.saveName, v.extension);
                    });
                    $log.click(function () {
                        $("#myModal-log .modal-content").load("/DocumentManagement/GetLogList", { documentId: v.documentId }, function () {
                            $.ajax({
                                type: "post",
                                url: "/DocumentManagement/GetCompanyDocumenLogList",
                                dataType: "json",
                                data: { documentId: v.documentId },
                                success: rsHandler(function (data) {
                                    $('.chosebox table tr').remove();
                                    if (data.length > 0) {
                                        $.each(data, function (e, i) {
                                            var $logHeadHtml = $("<tr><td>" + i.createUserName + "</td></tr>");
                                            var $logMiddleHtml;
                                            if (i.type == 1) {
                                                $logMiddleHtml = $("<td>下载了该文档</td>");
                                            }
                                            else if (i.type == 2) {
                                                $logMiddleHtml = $("<td>复制了该文档</td>");
                                            }
                                            else if (i.type == 3) {
                                                $logMiddleHtml = $("<td>移动了该文档</td>");
                                            }
                                            else if (i.type == 4) {
                                                $logMiddleHtml = $("<td>新建了该文档</td>");
                                            }
                                            else if (i.type == 5) {
                                                $logMiddleHtml = $("<td>删除了该文档</td>");
                                            }
                                            else if (i.type == 6) {
                                                $logMiddleHtml = $("<td>对该文档进行了权限设置</td>");
                                            }
                                            var $logLastHtml = "<td>" + i.createDate + "</td><td>" + i.createHMS + "</td>";
                                            $('.chosebox table').append($logHeadHtml.append([$logMiddleHtml, $logLastHtml]));
                                        });
                                    }
                                })
                            });
                            //确定
                            $('#log_modal_submit').click(function () {
                                $("#myModal-log").modal("hide");
                            });
                        });
                    });

                    $copy.click(function () {
                        documents = [v.documentId];
                        folders = [];
                        $.ajax({
                            type: "post",
                            url: "/Shared/GetFolderList",
                            dataType: "json",
                            data: { type: 1, folder: null },
                            success: rsHandler(function (data) {
                                for (var i = 0; i < data.length; i++) {
                                    $.each(documents, function (e, j) {
                                        if (j == data[i].id) {
                                            data.splice(i, 1);
                                        }
                                    });
                                }
                                var folderTree = $.fn.zTree.init($("#copy_modal_folder"), $.extend({
                                    callback: {
                                        beforeClick: function (id, node) {
                                            //folderTree.checkNode(node, undefined, undefined, true);
                                            return false;
                                        },
                                        onCheck: function (e, id, node) {
                                            $("#copy_modal_chosen_count").html(folderTree.getCheckedNodes().length);
                                            if (node.checked) {
                                                var $checked = $("<li term='" + node.id + "'><span>" + node.name + "</span></li>"),
                                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                                $("#copy_modal_chosen").append($checked.append($close));
                                                $close.click(function () {
                                                    folderTree.checkNode(node, undefined, undefined, true);
                                                    for (var i = 0; i < folders.length; i++) {
                                                        if (folders[i] == $(this).parent().attr("term")) {
                                                            folders.splice(i, 1);
                                                        }
                                                    }
                                                });
                                                node.mappingLi = $checked;
                                                folders.push(node.id);
                                            } else {
                                                node.mappingLi.remove();
                                                folders = _.without(folders, node.id);
                                            }
                                        },
                                        onNodeCreated: function (e, id, node) {
                                            var folderTree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                                            $("#copy_modal_chosen li").each(function () {
                                                var departId = $(this).attr('term');
                                                //$(this).remove();
                                                if (parseInt(departId) == node.id) {
                                                    folderTree.checkNode(node, undefined, undefined, true);
                                                }
                                            });
                                        }
                                    }
                                }, {
                                    view: {
                                        showIcon: false,
                                        showLine: false
                                    },
                                    check: {
                                        enable: true,
                                        chkStyle: "checkbox",
                                        chkboxType: { "Y": "", "N": "" }
                                    },
                                    async: {
                                        enable: true,
                                        url: "/Shared/GetFolderList",
                                        autoParam: ["id=folder"],
                                        otherParam: ["type", "1"],
                                        dataFilter: function (treeId, parentNode, responseData) {
                                            return responseData.data;
                                        }
                                    }
                                }), data);
                            })
                        });

                        //选择文件夹
                        $("#copy_modal_search").selection({
                            url: "/Shared/GetLikeFolderList",
                            hasImage: false,
                            otherParam: { type: 1 },
                            selectHandle: function (data) {
                                var ztree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                                $("#copy_select").val(data.name);
                                var n = ztree.getNodeByParam("id", data.id);
                                if (!n.checked) {
                                    ztree.checkNode(n, undefined, undefined, true);
                                }
                                else {
                                    var flag = true;
                                    if ($("#copy_modal_chosen li").length > 0) {
                                        $("#copy_modal_chosen li").each(function () {
                                            if ($(this).attr('term') == data.id) {
                                                flag = false;
                                            }
                                        });
                                    }
                                    if (flag == true) {
                                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                        $("#copy_modal_chosen").append($checked.append($close));
                                        folders.push(data.id);
                                        $close.click(function () {
                                            var nodeId = $(this).parent().attr("term");
                                            n = ztree.getNodeByParam("id", parseInt(nodeId));
                                            if (!n.checked) {
                                                ztree.checkNode(n);
                                            }
                                            $(this).parent().remove();
                                            $("#copy_modal_chosen_count").text($("#copy_modal_chosen li").length);
                                        });
                                    }
                                    $("#copy_modal_chosen_count").text($("#copy_modal_chosen li").length);
                                }
                            }
                        });
                    });

                    $move.click(function () {
                        documents = [v.documentId];
                        $("#move_modal .modal-content").load("/Shared/GetMoveList", function () {
                            buildDepartmentTree();
                            //选择文件夹
                            $("#move_modal_search").selection({
                                url: "/Shared/GetLikeFolderList",
                                hasImage: false,
                                otherParam: { type: 1 },
                                selectHandle: function (data) {
                                    var ztree = $.fn.zTree.getZTreeObj("move_modal_folder");
                                    $("#move_select").val(data.name);
                                    var n = ztree.getNodeByParam("id", data.id);
                                    if (!n.checked) {
                                        ztree.checkNode(n, undefined, undefined, true);
                                    }
                                    else {
                                        var flag = true;
                                        if ($("#move_modal_chosen li").length > 0) {
                                            $("#move_modal_chosen li").each(function () {
                                                if ($(this).attr('term') == data.id) {
                                                    flag = false;
                                                }
                                            });
                                        }
                                        if (flag == true) {
                                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                                       $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                            $("#move_modal_chosen").append($checked.append($close));
                                            folders = [];
                                            folders.push(data.id);
                                            $close.click(function () {
                                                var nodeId = $(this).parent().attr("term");
                                                n = ztree.getNodeByParam("id", parseInt(nodeId));
                                                if (n) {
                                                    ztree.checkNode(n);
                                                }
                                                $(this).parent().remove();
                                                $("#move_modal_chosen_count").text($("#move_modal_chosen li").length);
                                            });
                                        }
                                        $("#move_modal_chosen_count").text($("#move_modal_chosen li").length);
                                    }
                                }
                            });
                        });
                    });

                    $delete.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                documents = [v.documentId];
                                $.ajax({
                                    type: "post",
                                    url: "/DocumentManagement/DeleteFlagDocument",
                                    dataType: "json",
                                    data: { data: JSON.stringify(documents) },
                                    success: rsHandler(function (data) {
                                        if (data == "ok") {
                                            var ztree = $.fn.zTree.getZTreeObj("catalogue");
                                            for (var i = 0; i < documents.length; i++) {
                                                var n = ztree.getNodeByParam("id", documents[i]);
                                               
                                                if (n) {
                                                    ztree.removeNode(n);
                                                }
                                            }
                                            loadDocuments();
                                            ncUnits.alert("删除成功!");
                                        }
                                    })
                                });
                            }
                        });
                    });

                    $download.click(function () {
                        documents = [v.documentId];
			$.ajax({
                            type: "post",
                            url: "/DocumentManagement/Download",
                            dataType: "json",
                            data: { documentId: v.documentId, displayName: v.displayName, saveName: v.saveName, isFolder: v.isFolder, flag: 0 },
                            success: rsHandler(function (data) {
                                if (data == "success") {
                                    window.location.href = "/DocumentManagement/Download?documentId=" + v.documentId + "&displayName=" + escape(v.displayName) + "&saveName=" + v.saveName + "&isFolder=" + v.isFolder + "&flag=1";
                                }
                            })
                        });
                        
                    });

                    if (v.isFolder) {
                        $fileName.css("cursor", "pointer").addClass('isfolder');
                        $fileName.click(function () {
                            if (bitchoperateing == false) {
                                argus.folder = v.documentId;
                                $dirc = $("<span>></span><span class='dirc_span' term='" + v.documentId + "'>" + v.displayName + "</span>");
                                $(".labelLast").append($dirc);
                                $(".dirc_span:last").addClass("dirc_active").siblings().removeClass("dirc_active");
                                $('.dirc_span').off('click');
                                $('.dirc_span').click(function () {
                                    dircclick($(this));
                                });
                                $(".directory-set span").css("color", "#686868");
                                loadDocuments();
                                //更新右侧目录
                                var n = rightFolderTree.getNodeByParam("id", v.documentId);
                                rightFolderTree.expandNode(n);
                                rightFolderTree.selectNode(n);
                            }

                        });
                    }

                    /*批量勾选 开始*/
                    documents = [];
                    $choose.click(function () {
                        if ($(this).hasClass('chooseHit')) {
                            $(this).removeClass('chooseHit');
                            $('span', this).removeClass('spanHit');
                            for (var i = 0; i < documents.length; i++) {
                                if ($(this).attr('term') == documents[i]) {
                                    documents.splice(i, 1);
                                }
                            }
                        }
                        else {
                            $(this).addClass('chooseHit');
                            $('span', this).addClass('spanHit');
                            documents.push(v.documentId);
                        }
                    });
                    /*批量勾选 结束*/
                });
                if (mode == 0) {
                    $addNewFile = $("<div class='newadd col-xs-3'><div class='add'><div class='addPicture' data-toggle='modal' data-target='#myModal-newfile' style='cursor:pointer'></div><div class='wordPosition'>新建文件夹</div></div></div>");
                    $documents.append($addNewFile);
                    $(".newadd .addPicture").click(function () {
                        addNewFile();
                    });
                }
            })
        });
    }

    //取消当前批量操作 
    function cancelBitchOperate() {
        $(".cell").hover(function () {
            $(".operate", this).show();
        }, function () {
            $(".operate", this).hide();
        });
        bitchoperateing = false;
        documents = [];
        $(".btns").show();
        $('.isfolder').css('cursor', 'pointer');
        $('.moreCancel').hide();
        $('.moreBg').hide();
        $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
        $('.xxc_choose span').removeClass('spanHit');
    }



    /**
     * 添加filter条件
     * @param obj
     * {
     *   text : string,
     *   type : string,
     *   add : function,
     *   delete : function
     * }
     */
    var addFilter = (function () {
        var _types = {};
        return function (obj) {
            obj = obj || {};
            if (obj.id) {
                var selectFlag = true;
                $.each(argus.person, function (e, i) {
                    if (obj.id == i) {
                        selectFlag = false;
                    }
                });
                if (!selectFlag) return;
            }
            var $condition = $("<span class='selected'></span>"),
                $delete = $("<span class='glyphicon glyphicon-remove'></span>");
            $(".selecteds").append($condition);
            $condition.append([obj.text + " ", $delete]);

            if (obj.type) {
                if (_types[obj.type]) {
                    _types[obj.type].children(".glyphicon-remove").trigger("click", true);
                }
                _types[obj.type] = $condition;
            }
            $delete.click(function (e, update) {
                if (obj.delete && typeof obj.delete == "function") {
                    obj.delete();
                }
                $condition.remove();
                if (obj.type) {
                    _types[obj.type] = undefined;
                }
                if (!update) {
                    loadDocuments();
                }
            });

            obj.add();
            loadDocuments();
        }
    })();

    /*权限设置 开始*/


    //添加权限设置请求所用参数
    var auths = {
        documentId: null,
        displayName: null,
        description: null,
        AuthorityList: []
    }
    var deleteDocuments = [];
    var targetId = [];
    var tagertName = [];
    var displayName;
    var description;
    var noNeedTarget = [];

    //点击权限设置
    $('#authority-a').on('shown.bs.tab', function () {
        //取消当前批量操作
        cancelBitchOperate();
        var authfile_flag = true;
        $("#getFile_name").blur(function () {
            authfile_flag = true;
            var Filename = $.trim($("#getFile_name").val());
            if (Filename == "") {
                ncUnits.alert("文件夹名称不能为空!");
                authfile_flag = false;
            }
            else if (Filename.length > 20) {
                ncUnits.alert("文件夹名称不能超过20字符!");
                authfile_flag = false;
            }
            var reg = /<\w+>/;
            if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                ncUnits.alert("文件夹名称存在非法字符!");
                authfile_flag = false;
            }
        });
        $("#getFile_des").blur(function () {
            authfile_flag = true;
            var descriptions = $("#getFile_des").val();
            if (descriptions.length > 100) {
                ncUnits.alert("文件夹描述不能超过100字符!");
                authfile_flag = false;
            } else if (descriptions.indexOf('null') >= 0 || descriptions.indexOf('NULL') >= 0 || descriptions.indexOf('&nbsp') >= 0 || reg.test(descriptions) || descriptions.indexOf('</') >= 0) {
                ncUnits.alert("文件夹描述存在非法字符!");
                authfile_flag = false;
            }
        });
        $("#sure_auth").show();
        getAuthorityInfo(argus.folder);
        $("#newFile_building").hide();
        $("#mode_card").hide();
        $("#mode_list").hide();
        $("#gofilter").hide();
        $(".nav-right .dropdown").hide();
        $("#filterbox").slideUp();
    });
    //$("#authority-a").click(function () {

    //});

    function getAuthorityInfo(id) {
        if (id == null) {
            return;
        }
        $(".permisList .table tbody").empty();
        var auth_lodi = getLoadingPosition('.tableuser');  //加载局部视图的对象
        $.ajax({
            type: "post",
            url: "/DocumentManagement/GetAuthorityList",
            dataType: "json",
            data: {
                documentId: id
            },
            complete:rcHandler(function(){
                auth_lodi.remove();
            }),
            success: rsHandler(function (data) {
                auths.documentId = data.documentId;
                deleteDocuments = [];
                displayName = data.displayName;
                description = data.description;
                var $authorityHtml;
                $("#getFile_name").val(data.displayName);
                $("#getFile_des").val(data.description);
                if (data.AuthorityList && data.AuthorityList.length > 0) {
                    for (var i = 0; i < data.AuthorityList.length; i++) {
                        if (data.AuthorityList[i].resultId) {
                            for (var j = 0; j < data.AuthorityList[i].resultId.length; j++) {
                                $authorityHtml = $(" <tr><td><span>" + data.AuthorityList[i].targetName[j] + "</span></td><td class='status'>" + data.AuthorityList[i].powerName + "</td><td><span term=" + data.AuthorityList[i].resultId[j] + " class='authy-delOld' style='cursor:pointer'>删除</span></td></tr>");

                                $(".permisList .table tbody").append($authorityHtml);

                                $("td:eq(0)").css("width", "500px");
                                $("td:eq(1)").css("width", "250px");
                                $("td:eq(2)").css("width", "120px");
                                $(".authy-delOld", $authorityHtml).off('click');
                                $(".authy-delOld", $authorityHtml).click(function () {
                                    var resultId = $(this).attr('term');
                                    var obj = $(this)
                                    ncUnits.confirm({
                                        title: '提示',
                                        html: '确认要删除？',
                                        yes: function (layer_auth) {
                                            layer.close(layer_auth);
                                            if (resultId) {
                                                obj.parents('tr').remove();
                                                deleteDocuments.push(resultId);
                                                $(".permisList .table td:eq(0)").css("width", "500px");
                                                $(".permisList .table td:eq(1)").css("width", "250px");
                                                $(".permisList .table td:eq(2)").css("width", "120px");
                                            }
                                        }
                                    });
                                });
                            }
                        }

                    }
                }
            })
        });
    }

    //下拉列表
    $('.authy-dropdown li').click(function () {
        var power = $(this).find('a').attr('term');
        var text = $(this).find('a').text();
        $('#power-span span:eq(0)').attr('term', power).text(text);
    })

    //单选框点击
    $('.permisSet input[type="radio"]').click(function () {
        var index = $('.permisSet input[type="radio"]').index($(this));
        $('.permisSet input[type="radio"]:not(:eq(' + index + '))').attr('checked', false);
    });

    //添加
    $('#authy-add').click(function () {
        if (auths.documentId == null) {
            ncUnits.alert('请选择文件夹');
            return;
        }
        if (($(".depart_choose").length < 0 && $(".station_choose").length < 0 && $(".HR_choose").length < 0)) {
            ncUnits.alert('权限设置不能为空');
            return;
        }
        if (targetId.length <= 0 || targetId.length < noNeedTarget.length) {
            ncUnits.alert("存在重复的权限，请重新设置!");
        }
        //初始化弹窗
        $("#department_modal_chosen_count").text(0);
        $("#department_modal_chosen li").remove();
        $("#person-haschildren").prop("checked", false);
        $("#HR_modal_chosen_count").text(0);
        $(".person_list input[type=checkbox]").prop("checked", false);
        $("#HR_modal_chosen li").remove();
        $("#station-haschildren").prop("checked", false);
        $("#station_modal_chosen_count").text(0);
        $(".station_list input[type=checkbox]").prop("checked", false);
        $("#station_modal_chosen li").remove();
        $(".depart_choose").remove();
        $(".station_choose").remove();
        $(".HR_choose").remove();
        var type = $('.permisSet input[type="radio"]').index($('.permisSet input[type="radio"]:checked')) + 1;
        var power = $('#power-span span:eq(0)').attr('term');
        var powerName = $('#power-span span:eq(0)').text();
        if (targetId.length > 0) {
            var $authorityAdd;
            var sameFlag;
            //auths.AuthorityList = [];
            var authyModel = { type: type, power: power, targetId: targetId };
            auths.AuthorityList.push(authyModel);
            var modelIndex = auths.AuthorityList.length - 1;
            for (var i = 0; i < targetId.length; i++) {
                sameFlag = false;
                $(".permisList .table tr").each(function () {
                    if ($(this).find("span:eq(0)").text() == tagertName[i]) {
                        sameFlag = true;
                    }
                });
                if (sameFlag == true) continue;
                $authorityAdd = $(" <tr><td><span>" + tagertName[i] + "</span></td><td class='status'>" + powerName + "</td><td><span term=" + targetId[i] + " index=" + modelIndex + " targrtIndex=" + i + "  class='authy-del' style='cursor:pointer'>删除</span></td></td></tr>");
                $(".permisList .table").append($authorityAdd);
                $(".authy-del", $authorityAdd).off('click');
                $(".authy-del", $authorityAdd).click(function () {
                    var obj = $(this);
                    ncUnits.confirm({
                        title: '提示',
                        html: '确认要删除？',
                        yes: function (layer_authAdd) {
                            var targrtId = obj.attr('term');
                            var index = obj.attr('index');
                            var targetIndex = obj.attr("targrtIndex");
                            if (index) {
                                auths.AuthorityList[index].targetId.splice(parseInt(targetIndex), 1);
                                obj.parents('tr').remove();
                                $(".permisList .table td:eq(0)").css("width", "500px");
                                $(".permisList .table td:eq(1)").css("width", "250px");
                                $(".permisList .table td:eq(2)").css("width", "120px");
                            }
                            layer.close(layer_authAdd);
                        }
                    });

                });
            }
            $(".permisList .table td:eq(0)").css("width", "500px");
            $(".permisList .table td:eq(1)").css("width", "250px");
            $(".permisList .table td:eq(2)").css("width", "120px");
        }
        targetId = []; tagertName = [];
    });

    //确定
    $('#authy-submit').click(function () {
        var new_displayName = $('#getFile_name').val();
        var new_description = $("#getFile_des").val();
        auths.displayName = new_displayName;
        auths.description = new_description;
        if (argus.folder == null) {
            ncUnits.alert('请选择文件夹');
            return;
        }
        if (new_displayName == "") {
            ncUnits.alert("文件夹名称不能为空!");
            return;
        }
        else if (new_displayName.length > 20) {
            ncUnits.alert("文件夹名称不能超过20字符!");
            return;
        }
        var reg = /<\w+>/;
        if (new_displayName.indexOf('null') >= 0 || new_displayName.indexOf('NULL') >= 0 || new_displayName.indexOf('&nbsp') >= 0 || reg.test(new_displayName) || new_displayName.indexOf('</') >= 0) {
            ncUnits.alert("文件夹名称存在非法字符!");
            return;
        }
        if (new_description.length > 100) {
            ncUnits.alert("文件夹描述不能超过100字符!");
            return;
        } else if (new_description.indexOf('null') >= 0 || new_description.indexOf('NULL') >= 0 || new_description.indexOf('&nbsp') >= 0 || reg.test(new_description) || new_description.indexOf('</') >= 0) {
            ncUnits.alert("文件夹描述存在非法字符!");
            return;
        }
        if (deleteDocuments.length > 0 || auths.AuthorityList.length > 0 || displayName != auths.displayName || description != auths.description) {
            $.ajax({
                type: "post",
                url: "/DocumentManagement/SetAuthority",
                dataType: "json",
                data: { deleteAuthorityIds: JSON.stringify(deleteDocuments), data: JSON.stringify(auths) },
                success: rsHandler(function (data) {
                    if (data == "ok") {
                        auths.AuthorityList = [];
                        targetId = [];
                        tagertName = [];
                        $('.permisSet input[type="radio"]').attr('checked', false);
                        $('#power-span span:eq(0)').attr('term', 4).text('完全控制');
                        loadrighttree();
                        $(".permisList .table tr").remove();
                        getAuthorityInfo(argus.folder);
                        ncUnits.alert('权限设置成功');
                    }
                })
            });
        }
        else {
            ncUnits.alert('当前无任何操作');
        }
    });

    //取消
    $('#authy-cancel').click(function () {
        deleteDocuments = [];
        auths.documentId = null;
        auths.displayName = null;
        auths.description = null;
        auths.AuthorityList = [];
        $(".depart_choose").remove();
        $(".station_choose").remove();
        $(".HR_choose").remove();
        $(".permisList table tr").remove();
        //var n = rightFolderTree.getNodeByParam("id", argus.folder);
        //rightFolderTree.cancelSelectedNode(n);
        //$(".directory-set").find("span").css("color", "#58b456");
        //argus.folder = null;
        $("#getFile_name").val('');
        $("#getFile_des").val('');
        $('.permisSet input[type="radio"]').attr('checked', false);
        $('#power-span span:eq(0)').attr('term', 4).text('完全控制');
        $("#resource-a").tab('show');
        loadDocuments();
    });

    //点击加号
    $('.permisSet .glyphicon-plus').click(function () {
        $(this).parent().find("input[type='radio']").click();
    });
    $('#department-add').hover(function () {
        $(this).children($(this)).css({ "color": "#58b456" });
    });

    /*弹窗 开始*/
    //组织架构
    var department_modal;
    $("#department-add").click(function () {
        $("#department_modal .modal-content").load("/Shared/GetDepartmentHtml", function () {
            //if ($(".depart_choose").length>0) {
            //    $(".depart_choose").each(function () {
            //        var term = $(this).parent().attr("term");
            //        var name = $(this).parent().find("span:eq(0)").text();
            //        var $checked = $("<li term=" + term + "><span>" + name + "</span></li>"),
            //        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
            //        $("#department_modal_chosen").append($checked.append($close));
            //    });
            //}
            $.ajax({
                type: "post",
                url: "/Shared/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    department_modal = $.fn.zTree.init($("#department_modal_folder"), $.extend({
                        callback: {
                            beforeClick: function (id, node) {
                                department_modal.checkNode(node, undefined, undefined, true);
                                return false;
                            },
                            onCheck: function (e, id, node) {
                                $("#department_modal_chosen_count").html(department_modal.getCheckedNodes().length);
                                if (node.checked) {
                                    var $checked = $("<li term=" + node.id + "><span>" + node.name + "</span></li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $("#department_modal_chosen").append($checked.append($close));
                                    $close.click(function () {
                                        department_modal.checkNode(node, undefined, undefined, true);
                                    });
                                    node.mappingLi = $checked;
                                    folders.push(node.id);
                                } else {
                                    node.mappingLi.remove();
                                    folders = _.without(folders, node.id);
                                }
                            },
                            onNodeCreated: function (e, id, node) {
                                if ($("#department_modal_chosen li").length > 0) {
                                    $("#department_modal_chosen li").each(function () {
                                        var departId = $(this).attr('term');
                                        if (parseInt(departId) == node.id) {
                                            $(this).remove();
                                            department_modal.checkNode(node, undefined, undefined, true);
                                        }
                                    });
                                }
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false
                        },
                        check: {
                            enable: true,
                            chkStyle: "checkbox",
                            chkboxType: { "Y": "", "N": "" }
                        },
                        async: {
                            enable: true,
                            url: "/Shared/GetOrganizationList",
                            autoParam: ["id=parent"],
                            dataFilter: function (treeId, parentNode, responseData) {
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            });
            //组织架构搜索
            $("#department_modal_search").selection({
                url: "/Shared/GetOrgListByName",
                hasImage: false,
                selectHandle: function (data) {
                    $("#department_select").val(data.name);
                    var n = department_modal.getNodeByParam("id", data.id);
                    if (n && !n.checked) {
                        department_modal.checkNode(n, undefined, undefined, true);
                    } else {
                        var flag = true;
                        if ($("#department_modal_chosen li").length > 0) {
                            $("#department_modal_chosen li").each(function () {
                                if ($(this).attr('term') == data.id) {
                                    flag = false;
                                }
                            });
                        }
                        
                        if (flag == true) {
                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                       $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $("#department_modal_chosen").append($checked.append($close));
                            $close.click(function () {
                                var nodeId = $(this).parent().attr("term");
                                n = department_modal.getNodeByParam("id", parseInt(nodeId));
                                if (n) {
                                    department_modal.checkNode(n);
                                }
                                $(this).parent().remove();
                                $("#department_modal_chosen_count").text($("#department_modal_chosen li").length);
                            });
                        }
                    }
                    $("#department_modal_chosen_count").text($("#department_modal_chosen li").length);
                }
            });
            //确定组织架构选择
            $("#department_modal_submit").click(function () {
                targetId = []; tagertName = [];
                noNeedTarget = [];
                $('#department_modal_chosen li').each(function () {
                    var name = $(this).find("span:eq(0)").text();
                    var flag = true;
                    $(".permisList .table tr").each(function () {
                        if ($(this).find("span:eq(0)").text().substring($(this).find("span:eq(0)").text().indexOf('>') + 1) == name) {
                            flag = false;
                        }
                    });
                    noNeedTarget.push($(this).attr('term'));
                    if (flag == true) targetId.push($(this).attr('term'));
                });
                $.ajax({
                    type: "post",
                    url: "/DocumentManagement/GetOrgInfoById",
                    dataType: "json",
                    data: { data: JSON.stringify(noNeedTarget) },
                    success: rsHandler(function (data) {
                        $(".depart_choose").remove();
                        $(".station_choose").remove();
                        $(".HR_choose").remove();
                        $.each(data, function (e, i) {
                            var newflag = true;
                            $(".permisList .table tr").each(function () {
                                if ($(this).find("span:eq(0)").text() == i) {
                                    newflag = false;
                                }
                            });
                            if (newflag == true) {
                                tagertName.push(i);
                            }
                            var $depart_choose = $("<li class='depart_choose' term='" + noNeedTarget[data.indexOf(i)] + "'><span>" + i + "</span></li>");
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $(".department-ul").append($depart_choose.append($close));
                            $close.click(function () {
                                $(this).parent().remove();
                                var id = $(this).parent().attr("term");
                                var name = $(this).parent().find('span:eq(0)').text();
                                for (var i = 0; i < targetId.length; i++) {
                                    if (targetId[i] == id) {
                                        targetId.splice(i, 1);
                                    }
                                }
                                for (var i = 0; i < noNeedTarget.length; i++) {
                                    if (noNeedTarget[i] == id) {
                                        noNeedTarget.splice(i, 1);
                                    }
                                }
                                for (var i = 0; i < tagertName.length; i++) {
                                    if (tagertName[i] == name) {
                                        tagertName.splice(i, 1);
                                    }
                                }
                            });
                            // $("#department-add").addClass("text-overflow");
                        });
                    })
                });
                $('#department_modal').modal('hide');
            });
            //取消组织架构选择
            $('#department_modal_cancel').click(function () {
                $("#department_modal_chosen_count").text(0);
                $("#department_modal_chosen").remove();
            });
        });
    });

    /*岗位 开始*/
    var stationWithSub = 0;
    var stationOrgId;
    $("#station-add").click(function () {
        $("#station_modal .modal-content").load("/Shared/GetStationHtml", function () {
            $.ajax({
                type: "post",
                url: "/Shared/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    var folderTree = $.fn.zTree.init($("#station_modal_folder"), $.extend({
                        callback: {
                            onClick: function (e, id, node) {
                                $("#station-haschildren").prop("checked", false);
                                $("#station-selectall").prop("checked", false);
                                var checked = $("#station-haschildren").prop('checked');
                                stationWithSub = checked == true ? 1 : 0;
                                stationOrgId = node.id;
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetStationList",
                                    dataType: "json",
                                    data: { withSub: stationWithSub, organizationId: stationOrgId },
                                    success: rsHandler(function (data) {
                                        $(".station_list ul").remove();
                                        if (data.length > 0) {
                                            $.each(data, function (i, v) {
                                                var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.stationId + "><span>" + v.stationName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                                $(".station_list").append($stationHtml);
                                                $("#station_modal_chosen li").each(function () {
                                                    if ($(this).attr('term') == v.stationId) {
                                                        $stationHtml.find("input[type='checkbox']").attr('checked', true);
                                                    }
                                                });
                                            });
                                            appendstation();
                                        }
                                    })
                                });
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false,
                            selectedMulti: false
                        },
                        async: {
                            enable: true,
                            url: "/Shared/GetOrganizationList",
                            autoParam: ["id=parent"],
                            dataFilter: function (treeId, parentNode, responseData) {
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            });

            //岗位搜索
            $("#station_modal_search").selection({
                url: "/Shared/GetStationListByName",
                hasImage: false,
                selectHandle: function (data) {
                    $("#station_select").val(data.name);
                    var flag = true;
                    if ($("#station_modal_chosen li").length > 0) {
                        $("#station_modal_chosen li").each(function () {
                            if ($(this).attr('term') == data.id) {
                                flag = false;
                            }
                        });
                    } if (flag == true) {
                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span>-" + data.affiliationName + "</li>"),
                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#station_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var nodeId = $(this).parent().attr("term");
                            $(this).parent().remove();
                            $("#station_modal_chosen_count").text($("#station_modal_chosen li").length);
                            $(".station_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#station-selectall").prop("checked", false);
                        });
                    }
                    if ($(".station_list ul").length > 0) {
                        $(".station_list ul").each(function () {
                            if ($(this).find("li:eq(1)").attr('term') == data.id) {
                                $(this).find("input[type='checkbox']").prop("checked", true);
                            }
                        });
                    }
                    $("#station_modal_chosen_count").text($("#station_modal_chosen li").length);
                }
            });


            //加载点击复选框的事件
            function appendstation() {
                $(".station_list input[type='checkbox']").click(function () {
                    var checked = $(this).prop('checked');
                    var stationId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
                    var stationName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
                    var orgName = $(this).parents(".list-inline").find("li:eq(1) span:eq(1)").text();
                    if (checked == true) {
                        $(this).attr('checked', true);
                        $('#station_modal_chosen_count').text($("#station_modal_chosen li").length + 1);
                        var $checked = $("<li term=" + stationId + "><span>" + stationName + "</span> -" + orgName + " </li>"),
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#share_modal_chosen li").each(function () {
                            if ($(this).attr('term') == stationId) {
                                $(this).remove();
                                $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
                            }
                        });
                        $("#station_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var $thisId = $(this).parent().attr('term');
                            $(this).parent().remove();
                            $('#station_modal_chosen_count').text($("#station_modal_chosen li").length);
                            $(".station_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#station-selectall").prop("checked", false);
                        });
                    } else {
                        $(this).attr('checked', false);
                        $("#station_modal_chosen li").each(function () {
                            if ($(this).attr('term') == stationId) {
                                $(this).remove();
                                if ($("#station_modal_chosen li").length <= 0) {
                                    $("#station-selectall").prop("checked", false);
                                }
                                $('#station_modal_chosen_count').text($("#station_modal_chosen li").length);
                            }
                        });

                    }
                });
            }

            //包含下级
            $("#station-haschildren").click(function () {
                if (stationOrgId == null) {
                    ncUnits.alert("请选择部门!");
                    return;
                }
                $(".station_list ul").remove();
                var checked = $(this).prop('checked');
                stationWithSub = checked == true ? 1 : 0;
                $.ajax({
                    type: "post",
                    url: "/Shared/GetStationList",
                    dataType: "json",
                    data: { withSub: stationWithSub, organizationId: stationOrgId },
                    success: rsHandler(function (data) {
                        if (data.length > 0) {
                            $.each(data, function (i, v) {
                                var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.stationId + "><span>" + v.stationName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                $(".station_list").append($stationHtml);
                                $("#station_modal_chosen li").each(function () {
                                    if ($(this).attr('term') == v.stationId) {
                                        $stationHtml.find("input[type='checkbox']").attr('checked', true);
                                    }
                                });
                            });
                            appendstation();
                        }
                    })
                });
            });
            //选择全部
            $('#station-selectall').click(function () {
                var allChecked = $(this).prop("checked");
                if (allChecked == true) {
                    $(".station_list ul").each(function () {
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("#station_modal_chosen li").each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    $(".station_list ul input[type='checkbox']").prop('checked', true);
                    var length = $(".station_list input[type='checkbox']:checked").length

                    for (var i = 0; i < length; i++) {
                        var stationId = $(".station_list ul:eq(" + i + ")").find("li:eq(1)").attr('term');
                        var stationName = $(".station_list ul:eq(" + i + ")").find("li:eq(1) span:eq(0)").text();
                        var orgName = $(".station_list ul:eq(" + i + ")").find("li:eq(1) span:eq(1)").text();
                        var $checked = $("<li term=" + stationId + "><span>" + stationName + "</span>-" + orgName + "</li>"),
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#station_modal_chosen").append($checked.append($close));
                        $close.click(function () {

                            var $thisId = $(this).parent().attr('term');
                            $(this).parent().remove();
                            $('#station_modal_chosen_count').text($("#station_modal_chosen li").length);
                            $(".station_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#station-selectall").prop("checked", false);
                        });
                    }
                    $('#station_modal_chosen_count').text($("#station_modal_chosen li").length);
                }
                else {
                    $(".station_list ul input[type='checkbox']").prop('checked', false);
                    $(".station_list ul").each(function () {
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("#station_modal_chosen li").each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    var length = $("#station_modal_chosen li").length
                    $('#station_modal_chosen_count').text(length);
                }
            });

            //确定
            $("#station_modal_submit").click(function () {
                targetId = []; tagertName = [];
                noNeedTarget = [];
                $("#station_modal_chosen li").each(function () {
                    var name = $(this).find("span:eq(0)").text();
                    var flag = true;
                    $(".permisList .table tr").each(function () {
                        if ($(this).find("span:eq(0)").text().substring($(this).find("span:eq(0)").text().indexOf('>') + 1) == name) {
                            flag = false;
                        }
                    });
                    noNeedTarget.push($(this).attr('term'));
                    if (flag == true) targetId.push($(this).attr('term'));
                });
                $.ajax({
                    type: "post",
                    url: "/DocumentManagement/GetStationInfoById",
                    dataType: "json",
                    data: { data: JSON.stringify(noNeedTarget) },
                    success: rsHandler(function (data) {
                        $(".depart_choose").remove();
                        $(".station_choose").remove();
                        $(".HR_choose").remove();
                        $.each(data, function (e, i) {
                            var newflag = true;
                            $(".permisList .table tr").each(function () {
                                if ($(this).find("span:eq(0)").text() == i) {
                                    newflag = false;
                                }
                            });
                            if (newflag == true) {
                                tagertName.push(i);
                            }
                            var $station_choose = $("<li class='station_choose' term='" + noNeedTarget[data.indexOf(i)] + "'><span>" + i + "</span></li>");
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $(".station-ul").append($station_choose.append($close));
                            $close.click(function () {
                                $(this).parent().remove();
                                var id = $(this).parent().attr("term");
                                var name = $(this).parent().find('span:eq(0)').text();
                                for (var i = 0; i < targetId.length; i++) {
                                    if (targetId[i] == id) {
                                        targetId.splice(i, 1);
                                    }
                                }
                                for (var i = 0; i < noNeedTarget.length; i++) {
                                    if (noNeedTarget[i] == id) {
                                        noNeedTarget.splice(i, 1);
                                    }
                                }
                                for (var i = 0; i < tagertName.length; i++) {
                                    if (tagertName[i] == name) {
                                        tagertName.splice(i, 1);
                                    }
                                }
                            });
                        });
                    })
                });
                $("#station_select").val('');
                $('#station_modal').modal('hide');
            });

            //取消
            $("#station_cancel").click(function () {
                $("#station_select").val('');
                $("#station-haschildren").prop("checked", false);
                $("#station_modal_chosen_count").text(0);
                $(".station_list input[type=checkbox]").prop("checked", false);
                $("#station_modal_chosen li").remove();
            });
        });
    });
    /*岗位 结束*/

    /*人力资源 开始*/
    var personOrgId;
    var personWithSub = false;
    $("#HR-add").click(function () {
        $("#HR_modal .modal-content").load("/Shared/GetPersonHtml", function () {
            $.ajax({
                type: "post",
                url: "/Shared/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    var folderTree = $.fn.zTree.init($("#HR_modal_folder"), $.extend({
                        callback: {
                            onClick: function (e, id, node) {
                                $("#HR-haschildren").prop("checked", false);
                                $("#person-selectall").prop("checked", false);
                                var checked = $("#HR-haschildren").prop('checked');
                                personWithSub = checked == true ? 1 : 0;
                                personOrgId = node.id;
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetUserList",
                                    dataType: "json",
                                    data: { withSub: personWithSub, organizationId: personOrgId },
                                    success: rsHandler(function (data) {
                                        $(".person_list ul").remove();
                                        if (data.length > 0) {
                                            $.each(data, function (i, v) {
                                                var $personHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                                $(".person_list").append($personHtml);
                                                $("#HR_modal_chosen li").each(function () {
                                                    if ($(this).attr('term') == v.userId) {
                                                        $personHtml.find("input[type='checkbox']").prop('checked', true);
                                                    }
                                                });
                                            });
                                            appendperson();
                                        }
                                    })
                                });
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false,
                            selectedMulti: false
                        },
                        async: {
                            enable: true,
                            url: "/Shared/GetOrganizationList",
                            autoParam: ["id=parent"],
                            dataFilter: function (treeId, parentNode, responseData) {
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            });

            //人员搜索
            $("#HR_modal_search").selection({
                url: "/Shared/GetUserListByName",
                hasImage: true,
                selectHandle: function (data) {
                    $("#HR_select").val(data.name);
                    var flag = true;
                    if ($("#HR_modal_chosen li").length > 0) {
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == data.id) {
                                flag = false;
                            }
                        });
                    }
                    if (flag == true) {
                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#HR_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var nodeId = $(this).parent().attr("term");
                            $(this).parent().remove();
                            $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
                            $(".person_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#person-selectall").prop("checked", false);
                        });
                    }
                    if ($(".person_list ul").length > 0) {
                        $(".person_list ul").each(function () {
                            if ($(this).find("li:eq(1)").attr('term') == data.id.toString()) {
                                $(this).find("input[type='checkbox']").prop("checked", true);
                            }
                        });
                    }
                    $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
                }
            });


            //人员复选框点击事件
            function appendperson() {
                $(".person_list input[type='checkbox']").click(function () {
                    var checked = $(this).prop('checked');
                    var personId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
                    var personName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
                    var showflag = true;
                    if (checked == true) {
                        $(this).attr('checked', true);
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr("term") == personId) {
                                showflag = false;
                            }
                        });
                        if (showflag) {
                        $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length + 1);
                        var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#HR_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var $thisId = $(this).parent().attr('term');
                            $(this).parent().remove();
                            $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                            $(".person_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#person-selectall").prop("checked", false);
                        });
                        }
                    } else {
                        $(this).prop('checked', false);
                        $(this).parents(".person_list").find("li").each(function () {
                            if ($(this).attr('term') && $(this).attr('term') == personId) {
                                $(this).parents(".list-inline").find("li:eq(0) input").prop('checked', false);
                            }
                        });
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == personId) {
                                $(this).remove();
                                if ($("#HR_modal_chosen li").length <= 0) {
                                    $("#person-selectall").prop("checked", false);
                                }
                                $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                            }
                        });
                    }
                });
            }

            //包含下级
            $("#HR-haschildren").click(function () {
                if (personOrgId == null) {
                    ncUnits.alert("请选择部门!");
                    return;
                }
                $(".person_list ul").remove();
                var checked = $(this).prop('checked');
                personWithSub = checked == true ? 1 : 0;
                $.ajax({
                    type: "post",
                    url: "/Shared/GetUserList",
                    dataType: "json",
                    data: { withSub: personWithSub, organizationId: personOrgId },
                    success: rsHandler(function (data) {
                        if (data.length > 0) {
                            $.each(data, function (i, v) {
                                var $personHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                $(".person_list").append($personHtml);
                                $("#HR_modal_chosen li").each(function () {
                                    if ($(this).attr('term') == v.userId) {
                                        $personHtml.find("input[type='checkbox']").attr('checked', true);
                                    }
                                });
                            });
                            appendperson();
                        }
                    })
                });
            });
            //选择全部
            $('#person-selectall').click(function () {
                var personAll = $(this).prop("checked");
                var showflag = true;
                if (personAll == true) {
                    $(".person_list ul").each(function () {
                        showflag = true;
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    $(".person_list ul input[type='checkbox']").prop('checked', true);

                    var length = $(".person_list input[type='checkbox']:checked").length
                    $('#HR_modal_chosen_count').text(length);
                    for (var i = 0; i < length; i++) {
                        showflag = true;
                        var personId = $(".person_list ul:eq(" + i + ")").find("li:eq(1)").attr('term');
                        var personName = $(".person_list ul:eq(" + i + ")").find("li:eq(1) span:eq(0)").text();
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == personId) {
                                showflag = false;
                            }
                        });
                        if (showflag) {
                        var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#share_modal_chosen li").each(function () {
                            if ($(this).attr('term') == stationId) {
                                $(this).remove();
                                $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
                            }
                        });
                        $("#HR_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var $thisId = $(this).parent().attr('term');
                            $(this).parent().remove();
                            $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);
                            $(".person_list ul").each(function () {
                                if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                    $(this).find("input[type='checkbox']").prop("checked", false);
                                }
                            });
                            $("#person-selectall").prop("checked", false);
                        });
                    }
                       
                    }
                    $('#HR_modal_chosen_count').text($("#HR_modal_chosen li").length);

                }
                else {
                    $(".person_list ul").each(function () {
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("#HR_modal_chosen li").each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    $(".person_list ul input[type='checkbox']").prop('checked', false);
                    var length = $("#HR_modal_chosen li").length
                    $('#HR_modal_chosen_count').text(length);
                }
            });

            //确定
            $("#HR_modal_submit").click(function () {
                targetId = []; tagertName = [];
                noNeedTarget = [];
                $(".depart_choose").remove();
                $(".station_choose").remove();
                $(".HR_choose").remove();
                $("#HR_modal_chosen li").each(function () {
                    var name = $(this).find("span:eq(0)").text();
                    var flag = true;
                    $(".permisList .table tr").each(function () {
                        if ($(this).find("span:eq(0)").text() == name) {
                            flag = false;
                        }
                    });
                    if (flag == true) {
                        targetId.push($(this).attr('term'));
                        tagertName.push($(this).find('span:eq(0)').text());
                    }
                    noNeedTarget.push($(this).attr('term'));
                    var $HR_choose = $("<li class='HR_choose' term='" + $(this).attr('term') + "'><span>" + name + "</span></li>");
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                    $(".person-ul").append($HR_choose.append($close));
                    $close.click(function () {
                        $(this).parent().remove();
                        var id = $(this).parent().attr("term");
                        var name = $(this).parent().find('span:eq(0)').text();
                        for (var i = 0; i < targetId.length; i++) {
                            if (targetId[i] == id) {
                                targetId.splice(i, 1);
                            }
                        }
                        for (var i = 0; i < noNeedTarget.length; i++) {
                            if (noNeedTarget[i] == id) {
                                noNeedTarget.splice(i, 1);
                            }
                        }
                        for (var i = 0; i < tagertName.length; i++) {
                            if (tagertName[i] == name) {
                                tagertName.splice(i, 1);
                            }
                        }
                    });
                });
                $("#HR_select").val('');
                $('#HR_modal').modal('hide');
            });

            //取消
            $("#HR_cancel").click(function () {
                $("#HR_select").val('');
                $("#person-haschildren").prop("checked", false);
                $("#HR_modal_chosen_count").text(0);
                $(".person_list input[type=checkbox]").prop("checked", false);
                $("#HR_modal_chosen li").remove();
            });
        });
    });
    /*人力资源 结束*/

    /*弹窗 结束*/

    /*权限设置 结束*/

    //绑定组织树形结构
    function buildDepartmentTree() {
        $.ajax({
            type: "post",
            url: "/Shared/GetFolderList",
            data: { type: 1, folder: null },
            dataType: "json",
            success: rsHandler(function (data) {
                //for (var i = 0; i < data.length; i++) {
                //    $.each(documents, function (e, j) {
                //        if (j == data[i].id) {
                //            data.splice(i, 1);
                //        }
                //    });
                //}
                var folderTree = $.fn.zTree.init($("#move_modal_folder"), $.extend({
                    callback: {
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {
                            //if (node.id==argus.folder) {
                            //    folderTree.checkNode(node, false);
                            //    var oldNodeId = $("#move_modal_chosen li:eq(0)").attr('term');
                            //    var oldNode = folderTree.getNodeByParam('id', oldNodeId);
                            //    $("#move_modal_chosen li").remove();
                            //    folderTree.checkNode(oldNode, undefined, undefined, true);
                            //    return;
                            //}
                            $("#move_modal_chosen_count").html(folderTree.getCheckedNodes().length);
                            if (node.checked) {
                                var $checked = $("<li term='" + node.id + "'>" + node.name + "</li>"),
                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                $("#move_modal_chosen").find("li").remove();
                                $("#move_modal_chosen").append($checked.append($close));
                                $close.click(function () {
                                    folderTree.checkNode(node, undefined, undefined, true);
                                });
                                node.mappingLi = $checked;
                                folders = [];
                                node.id = node.id == 0 ? null : node.id;
                                folders.push(node.id);
                                var moveModel = { documentType: 1, documentId: documents, folder: folders };
                                $('#move_modal_submit').click(function () {
                                    $.ajax({
                                        type: "post",
                                        url: "/Shared/Move",
                                        dataType: "json",
                                        data: {
                                            data: JSON.stringify(moveModel)
                                        },
                                        success: rsHandler(function (data) {
                                            $("#move_select").val('');
                                            $("#move_modal_chosen_count").text(0);
                                            $("#move_modal_chosen li").remove();
                                            $("#move_modal").modal("hide");
                                            ncUnits.alert("移动成功!");
                                            $('.moreBg').removeAttr('data-toggle');
                                            $('.moreBg').removeAttr('data-target');
                                            loadDocuments();
                                            loadrighttree();
                                        })
                                    });
                                });
                                $("#move_modal_cancel").click(function () {
                                    $("#move_select").val('');
                                    $("#move_modal_chosen_count").text(0);
                                    $("#move_modal_chosen li").remove();
                                    $('.moreBg').removeAttr('data-toggle');
                                    $('.moreBg').removeAttr('data-target');
                                });
                                $("#move_modal .close").click(function () {
                                    $("#move_select").val('');
                                    $("#move_modal_chosen_count").text(0);
                                    $("#move_modal_chosen li").remove();
                                });
                                $('.moreBg').removeAttr('data-toggle');
                                $('.moreBg').removeAttr('data-target');
                            } else {
                                node.mappingLi.remove();
                                folders = _.without(folders, node.id);
                            }
                        },
                        onNodeCreated: function (e, id, node) {
                            var folderTree = $.fn.zTree.getZTreeObj("move_modal_folder");
                            argus.folder = argus.folder == null ? 0 : argus.folder;
                            if (node.id == argus.folder) {
                                folderTree.setChkDisabled(node, true);

                            }
                            for (var i = 0; i < documents.length; i++) {
                                var n = folderTree.getNodeByParam("id", documents[i]);
                                folderTree.removeNode(n);
                            }
                            $("#move_modal_chosen li").each(function () {
                                var departId = $(this).attr('term');
                                if (departId == node.id) {
                                    $(this).remove();
                                }
                                if (parseInt(departId) == node.id) {
                                    folderTree.checkNode(node, undefined, undefined, true);
                                }
                            });
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false
                    },
                    check: {
                        enable: true,
                        chkStyle: "radio",
                        radioType: "all"

                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetFolderList",
                        autoParam: ["id=folder"],
                        otherParam: ["type", "1"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }
    //移动弹窗隐藏后执行的方法
    $("#move_modal").on('hidden.bs.modal', function () {
        documents = [];
        $('.moreBg').removeAttr('data-toggle');
        $('.moreBg').removeAttr('data-target');
    });
    /*Function 结束*/

    /*新建文件夹事件*/
    $("#newFile_building").click(function () {
        addNewFile();
    })
    var click = true;
    function addNewFile() {
        //取消当前批量操作 
        cancelBitchOperate();
        $("#myModal-newfile .modal-content").load("/Shared/GetNewFolder", function () {
            var build_flag = true;
            $("#input_filename").blur(function () {
                build_flag = true;
                var Filename = $.trim($("#input_filename").val());
                if (Filename == "") {
                    ncUnits.alert("文件夹名称不能为空!");
                }
                else if (Filename.length > 20) {
                    ncUnits.alert("文件夹名称不能超过20字符!");
                }
                var reg = /<\w+>/;
                if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                    ncUnits.alert("文件夹名称存在非法字符!");
                }
            });
            //$("#input_filedes").blur(function () {
            //    build_flag = true;
            //    var descriptions = $("#input_filedes").val();
            //    if (descriptions.length > 100) {
            //        ncUnits.alert("文件夹描述不能超过100字符!");
            //    } else if (descriptions.indexOf('null') >= 0 || descriptions.indexOf('NULL') >= 0 || descriptions.indexOf('&nbsp') >= 0 || reg.test(descriptions) || descriptions.indexOf('</') >= 0) {
            //        ncUnits.alert("文件夹描述存在非法字符!");
            //    }
            //});
            $("#newfile_modal_submit").click(function () {
                    var intID = argus.folder;
                    var Filename = $.trim($("#input_filename").val());
                    if (Filename == "") {
                        ncUnits.alert("文件夹名称不能为空!");
                        return;
                    }
                    else if (Filename.length > 20) {
                        ncUnits.alert("文件夹名称不能超过20字符!");
                        return;
                    }
                    var reg = /<\w+>/;
                    if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                        ncUnits.alert("文件夹名称存在非法字符!");
                        return;
                    }
                    var descriptions = $("#input_filedes").val();
                    if (descriptions.length > 100) {
                        ncUnits.alert("文件夹描述不能超过100字符!");
                        return;
                    } else if (descriptions.indexOf('null') >= 0 || descriptions.indexOf('NULL') >= 0 || descriptions.indexOf('&nbsp') >= 0 || reg.test(descriptions) || descriptions.indexOf('</') >= 0) {
                        ncUnits.alert("文件夹描述存在非法字符!");
                        return;
                    }
                    if (click == true) {
                        click = false;
                        $.ajax({
                            type: "post",
                            url: "/Shared/BuildNewUserFolder",
                            dataType: "json",
                            data: {
                                type: 1,
                                folder: intID, //上级文件夹
                                folderName: Filename,//文件夹名称
                                description: descriptions //描述
                            },
                            success: rsHandler(function (data) {
                                if (data == "ok") {
                                    ncUnits.alert("新建文件夹成功!");
                                    $("#myModal-newfile").modal("hide");
                                    click = true;
                                    loadDocuments();
                                    loadrighttree();
                                } else {
                                    ncUnits.alert("新建文件夹失败!");
                                    click = true;
                                }
                            })
                        });
                    }               
            })
        });
    }

    /*点击共享在弹窗中加载数据*/
    $('#share_modal').on('loaded.bs.modal', function () {
        $("#operate_share").click(function () {
            documents = [value.documentId];
            folders = [];
            $.ajax({
                type: "post",
                url: "../../test/data/plan/plan_user.json",
                dataType: "json",
                success: rsHandler(function (data) {
                    var folderTree = $.fn.zTree.init($("#share_modal_folder"), $.extend({
                        callback: {
                            beforeClick: function (id, node) {
                                folderTree.checkNode(node, undefined, undefined, true);
                                return false;
                            },
                            onCheck: function (e, id, node) {
                                $("#share_modal_chosen_count").html(folderTree.getCheckedNodes().length);
                                if (node.checked) {
                                    var $checked = $("<li>" + node.name + "</li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $("#share_modal_chosen").append($checked.append($close));
                                    $close.click(function () {
                                        folderTree.checkNode(node, undefined, undefined, true);
                                    });
                                    node.mappingLi = $checked;
                                    folders.push(node.id);
                                } else {
                                    node.mappingLi.remove();
                                    folders = _.without(folders, node.id);
                                }
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false
                        },
                        check: {
                            enable: true,
                            chkStyle: "checkbox",
                            chkboxType: { "Y": "", "N": "" }
                        }
                    }), data);
                })
            });
        });
    });
    //点击目录设置
    $(".directory-set").click(function () {
        if ($("#authority-a").attr("aria-expanded") != "true") {
            var n = rightFolderTree.getNodeByParam("id", argus.folder);
            rightFolderTree.cancelSelectedNode(n);
            $(this).find("span").css("color", "#58b456");
            argus.folder = null;
            $(".dic_first").click();
        }
    });

    /*关闭移动弹窗执行的方法*/
    //$("#move_modal").on("hide.bs.modal", function () {
    //    $('.moreBg').removeAttr('data-toggle');
    //    $('.moreBg').removeAttr('data-target');
    //});
    //限制字符个数
    
});

//弹出附件下载失败的提示信息
function noFile() {
    ncUnits.alert("文件不存在，无法下载！");
}
