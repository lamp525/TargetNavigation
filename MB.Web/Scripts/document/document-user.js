//@ sourceURL=document-manage.js
$(function () {
    //页面请求d对象参数（初始化1）
    var argus = {
        type: 2,
        name: undefined,
        folder: null,
        treetype: 2,
        time: [],
        person: [],
        sorts: {
            type: 1,
            direct: 0
        }
    };
    var documents = [],
        folders = [],
        userfolders = [];
    var ShareDocument = [];
    var file = [];
    var ShareFlag = 0;
    var NewDocFlag = 0;
    var UpdocPower = null;
    //组织树
    //var setting = {
    //    view: {
    //        showIcon: false,
    //        showLine: false,
    //        selectedMulti: false
    //    }
    //}; 
    var $con = $("#file_statistics")
        , $chart = $("#file_statistics_chart", $con)
        , $count = $(".chart-center", $con)
        , $label = $(".chart-label", $con)
        , colors = com.ztnc.targetnavigation.unit.planStatusColor;
    //右侧个人信息
    loadPersonalInfo();

    function drawPlanProgress() {
        $chart.empty();
        $count.empty();
        $label.empty();
        var lodi = getLoadingPosition($chart);//显示load层
        $.ajax({
            type: "post",
            url: "/UserDocument/GetDocumentStatisticsInfo",
            dataType: "json",
            success: rsHandler(function (data) {
                var dountData = []
                    , count = 0;

                for (var i = 0, len = data.length; i < len; i++) {

                    var color = colors[i];
                    if (data[i]["count"] > 0) {
                        var num = data[i]["count"];
                        count += num;
                        dountData.push([num, color, data[i]["id"], data[i]["text"]]);
                    }
                    $label.append('<li><span class="color-block" style="background-color:' + color + '"></span>' + data[i]["text"] + '</li>');

                }
                Raphael("file_statistics_chart", 270, 270).dountChart(135, 135, 55, 70, 110, dountData, function (data) {
                    //TODO 饼图click事件
                });
                $count.html('文档<span style="color:#58b456">' + count + '</span>项');
            }),
            complete: rcHandler(function () {
                lodi.remove();
            }),
        });
    }

    drawPlanProgress();

    /* 圆饼 结束 */

    //drawPlanProgress();
    //左侧树显示
    var rightFolderTree;
    loadrighttree();
    function loadrighttree() {
        $.ajax({
            type: "post",
            url: "/Shared/GetFolderList",
            dataType: "json",
            data: { type: argus.treetype, folder: null },
            success: rsHandler(function (data) {
                rightFolderTree = $.fn.zTree.init($("#lefttree"), {
                    callback: {
                        onClick: function (e, id, node) {
                            argus.folder = node.id;
                            $(".directory-set span").css("color", "#686868");
                            //$(".directory-set span").addClass("text-overflow");
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
                            } else {
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/getUserDocParentIds",
                                    dataType: "json",
                                    data: { documentId: node.id, type: argus.type },
                                    success: rsHandler(function (data) {
                                        if (data.length > 0) {
                                            $("#firstdirc_span span:not(:first)").remove();
                                            for (var i = 0; i < data.length; i++) {
                                                $dirc = $("<span>></span><span class='dirc_span' term='" + data[i].id + "'  style='width:auto;'>" + data[i].name + "</span>");
                                                $("#firstdirc_span").append($dirc);
                                            }
                                            $(".dirc_span:eq(" + $(".dirc_span").length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                            $(".dirc_span:eq(" + data.length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                            $(".dirc_span").off('click');
                                            $(".dirc_span").click(function () {
                                                dircclick($(this));
                                            });

                                        }
                                    })

                                });
                                loadDocuments();
                                isMove = true;
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
                        otherParam: ["type", argus.treetype],
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
    $("#TopTreeNode").click(function () {
        $("#firstdirc_span span:not(:first)").remove();
    })
    //导航条点击效果
    function dircclick(obj) {
        if (obj.text() == "目录设置") {
            argus.folder = null;
            $("#TopTreeNode").css("color", "#58b456");
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
    /*关闭移动弹窗执行的方法*/
    $("#move_modal").on("hide.bs.modal", function () {
        $('.moreBg').removeAttr('data-toggle');
        $('.moreBg').removeAttr('data-target');
        $('.moreBg').attr('href', '#');
    });

    /*文档加载*/
    loadDocuments();
    /*筛选开始*/
    /*筛选框出现隐藏*/
    $(".sift").click(function () {
        $(".hashandle").toggle();
        if ($(this).hasClass("active")) { $(this).removeClass("active"); }
        else $(this).addClass("active");

    });
    $(".drawer-handle").click(function () {
        $(".hashandle").hide();
        $(".sift").removeClass("active");
    });
    /*左边树隐藏和显示*/
    $(".leftTree-showbtn").click(function () {
        $(this).hide();
        $(this).next(".left-tree").show();
    });
    $(".leftTree-hidebtn").click(function () {
        $(this).parent(".left-tree").hide();
        $(this).parent('.alistcommen').prev('.leftTree-showbtn').show();
        $('.leftTree-showbtn').show();
    })
    /* 搜索框数据获取*/
    $("#filterbox_document_name").keyup(function (i) {
        if (i.keyCode == 13) {
            var name = $(this).val().trim();
            if ($(this).val().trim().length) {
                addSelected({
                    text: name,
                    type: "document",
                    add: function () {
                        argus.name = name;
                    },
                    delete: function () {
                        argus.name = undefined;
                    }
                });
            }
        }
    });
    $("#filterbox_document_name-submit").click(function () {
        var name = $("#filterbox_document_name").val().trim();
        if (name.length) {
            addSelected({
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

    /*时间获取开始*/
    $("#near_month").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addSelected({
                text: "近一月",
                type: "time",
                add: function () {
                    $this.addClass("active");
                    argus.time[0] = 1;
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.time = [];
                }
            });
        }
    });
    $("#near_week").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addSelected({
                text: "近一周",
                type: "time",
                add: function () {
                    $this.addClass("active");
                    argus.time[0] = 2;
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.time = [];
                }
            });
        }
    });
    $("#user_defined").click(function () {
        var starttime = $("#start_time").val();
        var endtime = $("#end_time").val();
        var text = (starttime.length ? "从" + starttime : "") + (endtime.length ? "到" + endtime : "");
        if (text.length) {
            addSelected({
                text: text,
                type: "time",
                add: function () {
                    argus.time = [3, starttime, endtime];
                },
                delete: function () {
                    argus.time = [];
                }
            });
        } else {
            ncUnits.alert("时间不能为空");
        }
    });
    var start = {
        elem: '#start_time',
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
            end.min = undefined;
        }
    },

        end = {
            elem: '#end_time',
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
                start.max = undefined;
            }
        };
    laydate(start);
    laydate(end);
    /*时间获取结束*/
    //员工加载
    $.ajax({
        type: "post",
        url: "/UserDocument/GetLastFiveCreateUser",
        dataType: "json",
        success: rsHandler(function (data) {
            $.each(data.reverse(), function (i, v) {
                var $staff = $("<a href='#' class='option'>" + v.name + "</a>");
                $("#creaters").prepend($staff);
                $staff.click(function () {
                    if (_.indexOf(argus.person, v.id) == -1) {
                        var $this = $(this);
                        if (!$this.hasClass("active")) {
                            addSelected({
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
                    }
                });
            });
        })
    });
    $("#commonUser").searchPopup({
        url: "/UserDocument/GetLastFiveCreateUser",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            if (_.indexOf(argus.person, data.id) == -1) {
                addSelected({
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
    /*员工数据加载结束*/
    /*清楚筛选*/
    $("#removeFilter").click(function () {
        $(".selecteds .glyphicon-remove").trigger("click", true);
        $("#removeFilter").css({ "color": " #fff", "background-color": "#e99900", "border": "1px solid #fbbf3d" });
        $("#filterbox_document_name").value = "";
        loadDocuments();
    });
    //if ($(".selecteds .glyphicon-remove").click()) {

    //    if (argus.text = "name") {
    //        $("#filterbox_document_name").value = "";
    //    }
    //}
    /*列表模式切换事件绑定 开始*/
    //$("#mode_card").click(function () {
    //    $(this).addClass("active");
    //    $("#mode_list").removeClass("active")
    //});
    //$("#mode_list").click(function () {
    //    $(this).addClass("active");
    //    $("#mode_card").removeClass("active")
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

    /*文档类型加载*/
    $("#docc").click(function () {
        if (isMove == true) {
            $(this).addClass('active');
            $(this).find('span').removeClass("personalNot").addClass("personalDocHit");
            if ($(".groupDoc").hasClass("active")) {
                $(".groupDoc").children('span', this).removeClass("groupDocHit").addClass("groupDocNot");
                $(".groupDoc").removeClass('active');
                $(".personalDoc").addClass('active');
                $(".personalDoc").children('span', this).removeClass("personalDocNot").addClass("personalDocHit");
            };
            $("#newFile_building", $(this).a).show();
            $("#newDoc_building", $(this).a).show();
            $("#MyDoc", $(this).a).show();
            $("#Myshare", $(this).a).hide();
            $("#ElseShareP", $(this).a).hide();
            argus.type = 2;
            argus.treetype = 2;
            argus.folder = null;
            loadrighttree();
            isMove = true;
            UpdocPower = null;
            $("#firstdirc_span span:not(:first)").remove();
            loadDocuments();
            $('.leftTree-showbtn').show();
        }
    })
    //var $noshare = $("<li><a href='#'>不共享</a></li>");
    $("#shareC").click(function () {
        if (isMove == true) {
            $(this).addClass('active');
            $(this).find('span').removeClass("pershareNot").addClass("pershareHit");
            if ($(".elseshare").hasClass("active")) {
                $(".elseshare").children('span', this).removeClass("elseshareHit").addClass("elseshareNot");
                $(".elseshare").removeClass('active');
            };
            $("#MyDoc").hide();
            $("#Myshare").show();
            $("#ElseShareP").hide();
            $("#newFile_building", $(this).a).hide();
            $("#newDoc_building", $(this).a).hide();
            //$("ul .dropdown-menu.batch-menu li.nomal").remove().add($noshare);
            $('.leftTree-showbtn').hide();
            argus.type = 3;
            argus.treetype = 2;
            argus.folder = null;
            loadrighttree();
            isMove = true;
            UpdocPower = null;
            loadDocuments();
            $("row-docChunk.newadd.col-xs-3").remove();
            $("#firstdirc_span span:not(:first)").remove();
            $addNewFile.hide();
        }
    });



    //请求后台标签数据
    tags(lab.autocomplete);

    $('.addLabel,.editLabel').on('click', function () {
        lab.add(this);
    })

    $("#myModal-newdoc").on('hide.bs.modal', function () {
        lab.empty();

    })

    $("#docBank").click(function () {
        $(".groupDoc a").children('span', this).removeClass("groupDocHit").addClass("groupDocNot");
    });
    $(".groupDoc").click(function () {
        if (isMove == true) {
            $("#newFile_building", $(this).a).hide();
            $("#newDoc_building", $(this).a).hide();
            $("#MyDoc", $(this).a).show();
            $("#Myshare", $(this).a).hide();
            $("#ElseShareP", $(this).a).hide();
            $(this).addClass('active');
            $(this).find('span').removeClass("groupDocNot").addClass("groupDocHit");
            if ($(".personalDoc").hasClass("active")) {
                $(".personalDoc").children('span', this).removeClass("personalDocHit").addClass("personalDocNot");
                $(".personalDoc").removeClass('active');
            }

            argus.type = 1;
            argus.treetype = 3;
            argus.folder = null;
            loadrighttree();
            isMove = true;
            UpdocPower = null;
            $("#firstdirc_span span:not(:first)").remove();
            loadDocuments();
            fileUpload();
        }
    });
    $(".personalDoc").click(function () {
        if (isMove == true) {
            $(this).addClass('active');
            $(this).find('span').removeClass("personalNot").addClass("personalDocHit");
            if ($(".groupDoc").hasClass("active")) {
                $(".groupDoc").children('span', this).removeClass("groupDocHit").addClass("groupDocNot");
                $(".groupDoc").removeClass('active');
            };
            $("#MyDoc", $(this).a).show();
            $("#Myshare", $(this).a).hide();
            $("#ElseShareP", $(this).a).hide();
            $("#newFile_building", $(this).a).show();
            $("#newDoc_building", $(this).a).show();
            argus.type = 2;
            argus.treetype = 2;
            argus.folder = null;
            loadrighttree();
            isMove = true;
            UpdocPower = null;
            $("#firstdirc_span span:not(:first)").remove();
            loadDocuments();
            fileUpload();
        }
    });
    $("#shareBank").click(function () {
        $(".elseshare a").children('span', this).removeClass("elseshareHit").addClass("elseshareNot");
    });
    $(".elseshare").click(function () {
        if (isMove == true) {
            $(this).addClass('active');
            $(this).find('span').removeClass("elseshareNot").addClass("elseshareHit");
            if ($(".pershare").hasClass("active")) {
                $(".pershare").children('span', this).removeClass("pershareHit").addClass("pershareNot");
                $(".pershare").removeClass('active');
            };
            $("#newFile_building", $(this).a).hide();
            $("#newDoc_building", $(this).a).hide();
            $("#MyDoc", $(this).a).hide();
            $("#Myshare", $(this).a).hide();
            $("#ElseShareP").show();
            argus.type = 4;
            argus.folder = null;
            loadrighttree();
            isMove = true;
            UpdocPower = null;
            $("#firstdirc_span span:not(:first)").remove();
            loadDocuments();
        }
    });
    $(".pershare").click(function () {
        if (isMove == true) {
            $(this).addClass('active');
            $(this).find('span').removeClass("pershareNot").addClass("pershareHit");
            if ($(".elseshare").hasClass("active")) {
                $(".elseshare").children('span', this).removeClass("elseshareHit").addClass("elseshareNot");
                $(".elseshare").removeClass('active');
            };
            $("#MyDoc", $(this).a).hide();
            $("#Myshare", $(this).a).show();
            $("#ElseShareP", $(this).a).hide();
            $("#newFile_building", $(this).a).hide();
            $("#newDoc_building", $(this).a).hide();
            argus.type = 3;
            argus.folder = null;
            loadrighttree();
            isMove = true;
            UpdocPower = null;
            $("#firstdirc_span span:not(:first)").remove();
            loadDocuments();
        }
    });
    /*批量事件 开始*/
    $('.batch-menu li').click(function () {
        if (isMove == true) {
            $("#newFile_building", $(this).a).hide();
            $("#newDoc_building", $(this).a).hide();
            isMove = false;
            $(".nest").hover(function () {
                $(".operate", this).hide();
            });

            if ($("#authority-a").parent().hasClass("active")) {
                return;
            }
            $('.moreBg').text($(this).text()).show();
            var txt = $(this).text();
            $('.moreCancel').show();
            $(".nest").each(function () {
                var power = $(this).attr('term')
                if (power == 4) {
                    $('.xxc_choose', this).addClass('choose').removeClass('prohibit').show();
                } else if ((power == 3 || power == 2) && (txt == "下载" || txt == "复制")) {
                    $('.xxc_choose', this).addClass('choose').removeClass('prohibit').show();
                }
            })
        }
    });

    $('.moreCancel').click(function () {
        //$(".nest",this).hover(function () {
        //    alert();
        //    $(".operate", this).toggle();
        //});
        $(this).hide();
        $('.moreBg').hide();
        $('.xxc_choose').removeClass('choose').removeClass('prohibit');
        $('.xxc_choose span').removeClass('spanHit');
        $('.chooseHit').removeClass('chooseHit');
        isMove = true;
        $("#newFile_building", $(this).a).show();
        $("#newDoc_building", $(this).a).show();
        loadDocuments();

    });
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        return isMove;
    })
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
                buildDepartmentTree();
                //选择文件夹
                $("#move_modal_search").selection({
                    url: "/Shared/GetLikeFolderList",
                    hasImage: false,
                    otherParam: { type: 2 },
                    selectHandle: function (data) {
                        var ztree = $.fn.zTree.getZTreeObj("move_modal_folder");
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
                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
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
            //$('.moreCancel').hide();
            //$('.moreBg').hide();
            //$('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            //$('.xxc_choose span').removeClass('spanHit');
        }
        else if (operate == "复制") {
            if (argus.treetype == 2) {
                $('#copy_modal').on('shown.bs.modal', function () {
                    //if ($(".groupDoc").hasClass("active")) { $(".docChooseCom").addClass("active") };
                    $(".docChoosePer").addClass("active");
                    $(".docChooseCom").removeClass("active");
                });
            }
            if (argus.treetype == 3) {
                $('#copy_modal').on('shown.bs.modal', function () {
                    //if ($(".groupDoc").hasClass("active")) { $(".docChooseCom").addClass("active") };
                    $(".docChooseCom").addClass("active");
                    $(".docChoosePer").removeClass("active");
                });
            }
            $(".docChooseCom").click(function () {
                $("#copy_select").val('');
                $("#copy_modal_chosen_count").text(0);
                $("#copy_modal_chosen li").remove();
                userfolders = [];

                argus.treetype = 3;
                $(".docChooseCom").addClass("active");
                $(".docChoosePer").removeClass("active");
                GetTreeByTreeIdFormCopy();
                x.reInit({
                    otherParam: {
                        type: 3
                    }
                })
            })
            $(".docChoosePer").click(function () {
                $("#copy_select").val('');
                $("#copy_modal_chosen_count").text(0);
                $("#copy_modal_chosen li").remove();
                folders = [];

                argus.treetype = 2;
                $(".docChoosePer").addClass("active");
                $(".docChooseCom").removeClass("active");
                GetTreeByTreeIdFormCopy();

                x.reInit({
                    otherParam: {
                        type: 2
                    }
                })
            })
            folders = [];
            userfolders = [];
            $('#copy_modal').modal('show');
            $.ajax({
                type: "post",
                url: "/Shared/GetFolderList",
                dataType: "json",
                data: { type: argus.treetype, folder: null },
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
                                    var $checked = $("<li term='" + node.id + "'><span>" + node.name + "</span></li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $("#copy_modal_chosen").append($checked.append($close));
                                    $close.click(function () {
                                        folderTree.checkNode(node, undefined, undefined, true);
                                        if (argus.treetype == 3) {
                                            for (var i = 0; i < folders.length; i++) {
                                                if (folders[i] == $(this).parent().attr("term")) {
                                                    folders.splice(i, 1);
                                                }
                                            }
                                        } else {
                                            for (var i = 0; i < userfolders.length; i++) {
                                                if (userfolders[i] == $(this).parent().attr("term")) {
                                                    userfolders.splice(i, 1);
                                                }
                                            }
                                        }
                                    });
                                    node.mappingLi = $checked;
                                    if (argus.treetype == 3) {
                                        folders.push(node.id);
                                    } else {
                                        userfolders.push(node.id);
                                    }
                                } else {
                                    node.mappingLi.remove();
                                    if (argus.treetype == 3) {
                                        folders = _.without(folders, node.id);
                                    } else {
                                        userfolders = _.without(userfolders, node.id);
                                    }
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
                            otherParam: ["type", argus.treetype],
                            dataFilter: function (treeId, parentNode, responseData) {
                                return responseData.data;
                            }
                        }
                    }), data);
                })
            });
            //选择文件夹
            var x = $("#copy_modal_search").selection({
                url: "/Shared/GetLikeFolderList",
                hasImage: false,
                otherParam: { type: argus.treetype },
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
                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#copy_modal_chosen").append($checked.append($close));
                        if (arg.treetype == 3) {
                            folders.push(data.id);
                        } else {
                            userfolders.push(data.id);
                        }
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
            //$('.moreCancel').hide();
            //$('.moreBg').hide();
            //$('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            //$('.xxc_choose span').removeClass('spanHit');
        }
        else if (operate == "删除") {
            ncUnits.confirm({
                title: '提示',
                html: '确认要删除？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    if (argus.type == 2) {
                        flag = 2;
                    } else {
                        flag = 1;
                    }
                    var dateD = { documentIds: documents, flag: flag };
                    $.ajax({
                        type: "post",
                        url: "/UserDocument/DeleteFlagDocument",
                        dataType: "json",
                        data: { data: JSON.stringify(dateD) },
                        success: rsHandler(function (data) {
                            if (data == true) {
                                loadDocuments();
                                loadrighttree();
                                ncUnits.alert("批量删除成功!");
                                $('.moreCancel').hide();
                                $('.moreBg').hide();
                                $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                                $('.xxc_choose span').removeClass('spanHit');
                                isMove = true;
                                $("#newFile_building", $(this).a).show();
                                $("#newDoc_building", $(this).a).show();
                            } else {
                                loadDocuments();
                                loadrighttree();
                                ncUnits.alert("批量删除失败!没有权限");
                                $('.moreCancel').hide();
                                $('.moreBg').hide();
                                $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                                $('.xxc_choose span').removeClass('spanHit');
                                isMove = true;
                                $("#newFile_building", $(this).a).show();
                                $("#newDoc_building", $(this).a).show();
                            }
                        })
                    });
                }
            });
            //$('.moreCancel').hide();
            //$('.moreBg').hide();
            //$('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            //$('.xxc_choose span').removeClass('spanHit');
        }
        else if (operate == "下载") {
            var type2 = 0;
            console.log(argus.type);
            if (argus.type != 1) {
                type2 = 1;
            }
            $.post("/UserDocument/MultiDownload", { data: JSON.stringify(documents), flag: 0, type: type2 }, function (data) {
                if (data == "success") {
                    //loadViewToMain("/UserDocument/MultiDownload?flag=1&type=" + type2);
                    window.location.href = "/UserDocument/MultiDownload?flag=1&type=" + type2 + "";
                    isMove = true;
                    documents = [];
                }
                else {
                    ncUnits.alert("下载失败!");
                    isMove = true;
                    loadDocuments();
                    loadrighttree();
                }
                documents = [];
                isMove = true;
                loadDocuments();
                loadrighttree();
            });
            $('.moreCancel').hide();
            $('.moreBg').hide();
            $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            $('.xxc_choose span').removeClass('spanHit');
        } else if (operate == "不共享") {

            var date1 = { documents: documents }
            ncUnits.confirm({
                title: '提示',
                html: '确认取消共享？',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);

                    $.ajax({
                        type: "post",
                        url: "/Shared/NoShareToOthers",
                        dataType: "json",
                        data: { data: JSON.stringify(date1) },
                        success: rsHandler(function (data) {
                            ncUnits.alert("取消共享成功");
                            isMove = true;
                            loadDocuments();
                        })
                    });
                }
            });
            //$('.moreCancel').hide();
            //$('.moreBg').hide();
            //$('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            //$('.xxc_choose span').removeClass('spanHit');
        } else if (operate == "共享") {
            $('#share_modal').modal('show');
            $("#share_modal_chosen").empty();
            $(".person_list ul").empty();
            $("#share_modal_chosen_count").text($("#share_modal_chosen li").length);
            $('#person-selectall').attr("checked", false);
            // $("#share_modal_chosen").empty(); 
            ShareDocument = documents;
            ShareFlag = 1;
            $.ajax({
                type: "post",
                url: "/Shared/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $.each(documents, function (e, j) {
                            if (j == data[i].id) {
                                data.splice(i, 1);
                            }
                        });
                    }
                    var folderTree = $.fn.zTree.init($("#share_modal_folder"), $.extend({
                        callback: {
                            onClick: function (e, id, node) {
                                $("#share-haschildren").prop("checked", false);
                                $("#share-selectall").prop("checked", false);
                                var checked = $("#share-haschildren").prop('checked');
                                stationWithSub = checked == true ? 1 : 0;
                                stationOrgId = node.id;
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetUserList",
                                    dataType: "json",
                                    data: { withSub: stationWithSub, organizationId: stationOrgId, withUser: false },
                                    success: rsHandler(function (data) {
                                        $(".person_list ul").remove();
                                        $("#person-haschildren").removeAttr("checked");
                                        $("#person-selectall").removeAttr("checked");
                                        if (data.length > 0) {
                                            $.each(data, function (i, v) {
                                                var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                                $(".person_list").append($stationHtml);
                                                $("#share_modal_chosen li").each(function () {
                                                    if ($(this).attr('term') == v.userId) {
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
            //人员搜索

        }


    });
    /*批量事件 结束*/
    $("#User_model_search").selection({
        url: "/Shared/GetUserListByNameFormUserDocument",
        hasImage: true,
        selectHandle: function (data) {
            $("#user_select").val(data.name);
            var flag = true;
            if ($("#share_modal_chosen li").length > 0) {
                $("#share_modal_chosen li").each(function () {
                    if ($(this).attr('term') == data.id) {
                        flag = false;
                    }
                });
            }
            if (flag == true) {
                var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
                           $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $("#share_modal_chosen").append($checked.append($close));
                $close.click(function () {
                    var nodeId = $(this).parent().attr("term");
                    $(this).parent().remove();
                    $("#share_modal_chosen_count").text($("#share_modal_chosen li").length);
                    $(".person_list ul").each(function () {
                        if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                            $(this).find("input[type='checkbox']").prop("checked", false);
                        }
                    });
                });
            }
            if ($(".person_list ul").length > 0) {
                $(".person_list ul").each(function () {
                    if ($(this).find("li:eq(1)").attr('term') == data.id.toString()) {
                        $(this).find("input[type='checkbox']").prop("checked", true);
                    }
                });
            }
            $("#share_modal_chosen_count").text($("#share_modal_chosen li").length);
        }
    });
    //文档列表list获取
    var mode = 0;
    var $addNewFile = $("");
    var $nest = $("");
    var isMove = true;
    var ComDoc = [];
    var partternFile = /(ppt|xls|doc|pptx|xlsx|docx|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    function loadDocuments() {
        var list_lodi = getLoadingPosition('.row-docChunk');
        if (argus.type == 1) {
            $("#shareto").hide();
            $("#firstdirc_span span:first").text("公司文档");
            $("#TopTreeNode span:first").text("公司文档");
        } else {
            $("#shareto").show();
            $("#firstdirc_span span:first").text("个人文档");
            $("#TopTreeNode span:first").text("个人文档");
        }
        $.ajax({
            type: "post",
            url: "/Userdocument/GetDocumentList",
            dataType: "json",
            data: { data: JSON.stringify(argus) },
            complete: rcHandler(function () {
                list_lodi.remove();
            }),
            success: rsHandler(function (data) {
                var $docChunk = $(".row-docChunk");
                var $docList = $(".row-docList");
                $docChunk.empty();
                $docList.empty();
                var date, time;
                //var i=0;
                $.each(data, function (n, value) {
                    var $col = $("<div class=col-xs-3 ></div>");
                    if (value.power != null) {
                        $nest = $("<div class=nest term=" + value.power + " ></div>");
                    } else if (UpdocPower != null) {
                        $nest = $("<div class=nest term=" + UpdocPower + " ></div>");
                    } else {
                        $nest = $("<div class=nest term=" + 4 + " ></div>");
                    }
                    var content = "";
                    var $fileName = $("<div class='title' title='" + value.displayName + "'>" + value.displayName + "</div>");
                    var $docList = $(".row-docList");
                    if (value.isFolder) {
                        $nest.addClass("filepng");
                        content = $("<dl class='dl-horizontal' title='" + value.description + "'><dd class='dl-horizontal' style='margin-left:0'>" + value.description + "</dd></dl>");
                        $('dd.dl-horizontal').dotdotdot();
                    }
                    else {
                        $nest.addClass("docpng");
                        date = value.createTime.toString().substring(0, value.createTime.toString().indexOf('T'));
                        time = value.createTime.toString().substring(value.createTime.toString().indexOf('T') + 1, value.createTime.toString().lastIndexOf(':'));
                        content = "<div class=content>" +
                          "<div><span class='content-label'>创建人：</span><span class='content-info'>" + value.createUserName + "</span></div>" +
                          "<div><span class='content-label'>创建时间：</span><span class='content-info'>" + date + "</span></div>" +
                          "</div>";
                    }
                    var $operate = $("<div class='operate'></div>");
                    var $btns = $("<div class='btns'></div>");
                    var $preview = $("<a href='#' class='preview'>预览</a>"),
                        $copy = $("<a href='#' data-toggle='modal' data-target='#copy_modal'>复制</a>"),
                        $move = $("<a href='' data-toggle='modal' data-target='#move_modal'id='operate_move'>移动</a>"),
                        $delete = $("<a href='#'>删除</a>"),
                        $download = $("<a href='#'>下载</a>");
                    $deleteShare = $("<a href='#'>不共享</a>")
                    $share = $("<a href='#' data-toggle='modal' data-target='#share_modal'>共享</a>");
                    var $choose = $("<div class='xxc_choose' term=" + value.documentId + "><span></span></div>");
                    if (argus.type == 3) {
                        $btns.append([$deleteShare, $share]).appendTo($operate);
                        $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);

                    } else if (argus.type == 4) {
                        $btns.append([$download, $copy]).appendTo($operate);
                        $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);

                    } else if (argus.type == 2) {
                        if (value.isFolder) {
                            $btns.append([$copy, $move, $delete, $download]).appendTo($operate);
                        } else {
                            if (partternFile.test(value.extension)) {
                                $btns.append($preview);
                            }
                            $btns.append([$copy, $move, $delete, $download, $share]).appendTo($operate);
                        }
                        $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                    } else if (argus.type == 1) {
                        if (!value.isFolder && partternFile.test(value.extension)) {
                            $btns.append($preview);
                        }
                        if (value.power == 4) {
                            $btns.append([$copy, $move, $delete, $download]).appendTo($operate);
                            $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                            ComDoc.push(value.documentId);
                        } else if (value.power == 2) {
                            $btns.append([$download, $copy]).appendTo($operate);
                            $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                        } else if (value.power == 3) {
                            $btns.append([$download, $copy]).appendTo($operate);
                            $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                        } else {
                            if (UpdocPower == 4) {
                                $btns.append([$copy, $move, $delete, $download]).appendTo($operate);
                                $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                            } else if (UpdocPower == 3) {
                                $btns.append([$download, $copy]).appendTo($operate);
                                $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                            } else if (UpdocPower == 2) {
                                $btns.append([$download, $copy]).appendTo($operate);
                                $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                            } else {
                                $btns.append([$copy, $move, $delete, $download]).appendTo($operate);
                                $nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                            }
                        }
                    }
                    // $btns.append([$copy, $move, $download, $share]).appendTo($operate);

                    //$nest.append(["<div class='grayTop' style='display:none'></div>", $fileName, content, $operate, $choose]);
                    $col.append($nest).appendTo($docChunk);

                    $preview.click(function () {
                        preview((argus.type == 1 ? 2 : (argus.type == 2 ? 4 : 0)), value.saveName, value.extension);
                    });

                    $copy.click(function () {

                        $("#copy_select").val('');
                        $("#copy_modal_chosen_count").text(0);
                        $("#copy_modal_chosen li").remove();
                        folders = [];
                        userfolders = [];

                        if (argus.treetype == 2) {
                            $('#copy_modal').on('shown.bs.modal', function () {
                                //if ($(".groupDoc").hasClass("active")) { $(".docChooseCom").addClass("active") };
                                $(".docChoosePer").addClass("active");
                                $(".docChooseCom").removeClass("active");
                            });
                        }
                        if (argus.treetype == 3) {
                            $('#copy_modal').on('shown.bs.modal', function () {
                                //if ($(".groupDoc").hasClass("active")) { $(".docChooseCom").addClass("active") };
                                $(".docChooseCom").addClass("active");
                                $(".docChoosePer").removeClass("active");
                            });
                        }
                        $(".docChooseCom").click(function () {
                            $("#copy_select").val('');
                            $("#copy_modal_chosen_count").text(0);
                            $("#copy_modal_chosen li").remove();
                            userfolders = [];

                            argus.treetype = 3;
                            $(".docChooseCom").addClass("active");
                            $(".docChoosePer").removeClass("active");
                            GetTreeByTreeIdFormCopy();

                            x.reInit({
                                otherParam: {
                                    type: 3
                                }
                            })
                        })
                        $(".docChoosePer").click(function () {
                            $("#copy_select").val('');
                            $("#copy_modal_chosen_count").text(0);
                            $("#copy_modal_chosen li").remove();
                            folders = [];

                            argus.treetype = 2;
                            $(".docChoosePer").addClass("active");
                            $(".docChooseCom").removeClass("active");
                            GetTreeByTreeIdFormCopy();

                            x.reInit({
                                otherParam: {
                                    type: 2
                                }
                            })
                        })
                        documents = [value.documentId];
                        folders = [];
                        userfolders = [];
                        $.ajax({
                            type: "post",
                            url: "/Shared/GetFolderList",
                            dataType: "json",
                            data: { type: argus.treetype, folder: null },
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

                                                    if (argus.treetype == 3) {
                                                        for (var i = 0; i < folders.length; i++) {
                                                            if (folders[i] == $(this).parent().attr("term")) {
                                                                folders.splice(i, 1);
                                                            }
                                                        }
                                                    } else {
                                                        for (var i = 0; i < userfolders.length; i++) {
                                                            if (userfolders[i] == $(this).parent().attr("term")) {
                                                                userfolders.splice(i, 1);
                                                            }
                                                        }
                                                    }
                                                });
                                                node.mappingLi = $checked;
                                                if (argus.treetype == 3) {
                                                    folders.push(node.id);
                                                } else {
                                                    userfolders.push(node.id);
                                                }
                                            } else {
                                                node.mappingLi.remove();
                                                if (argus.treetype == 3) {
                                                    folders = _.without(folders, node.id);
                                                } else {
                                                    userfolders = _.without(userfolders, node.id);
                                                }
                                            }
                                        },
                                        onNodeCreated: function (e, id, node) {
                                            var folderTree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                                            if (node.power == 2) {
                                                folderTree.setChkDisabled(node, true);
                                            }
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
                                        otherParam: ["type", argus.treetype],
                                        dataFilter: function (treeId, parentNode, responseData) {
                                            return responseData.data;
                                        }
                                    }
                                }), data);
                            })
                        });
                        //选择文件夹
                        var x = $("#copy_modal_search").selection({
                            url: "/Shared/GetLikeFolderList",
                            hasImage: false,
                            otherParam: { type: argus.treetype },
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
                                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
                                                   $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                        $("#copy_modal_chosen").append($checked.append($close));
                                        if (argus.treetype == 3) {
                                            folders.push(node.id);
                                        } else {
                                            userfolders.push(node.id);
                                        }
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
                        documents = [value.documentId];
                        $("#move_modal .modal-content").load("/Shared/GetMoveList", function () {
                            buildDepartmentTree();
                            //选择文件夹
                            $("#move_modal_search").selection({
                                url: "/Shared/GetLikeFolderList",
                                hasImage: false,
                                otherParam: { type: 2 },
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
                                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
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
                    $deleteShare.click(function () {
                        documents = [value.documentId];
                        var date1 = { documents: documents }
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认取消共享？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/NoShareToOthers",
                                    dataType: "json",
                                    data: { data: JSON.stringify(date1) },
                                    success: rsHandler(function (data) {
                                        ncUnits.alert("取消共享成功");
                                        loadDocuments();
                                        isMove = true;
                                    })
                                });
                            }
                        });
                    })
                    $delete.click(function () { 
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                documents = [value.documentId];
                                if (argus.type == 2) {
                                    flag = 2;
                                } else {
                                    flag = 1;
                                }
                                var dateD = { documentIds: documents, flag: flag };
                                $.ajax({
                                    type: "post",
                                    url: "/UserDocument/DeleteFlagDocument",
                                    dataType: "json",
                                    data: { data: JSON.stringify(dateD) },
                                    success: rsHandler(function (data) {
                                        if (data == true) {
                                            ncUnits.alert("删除成功");
                                            console.log(argus.type);
                                            loadDocuments();
                                            isMove = true;
                                            loadrighttree();
                                        } else {
                                            ncUnits.alert("没有权限删除");
                                            console.log(argus.type);
                                            loadDocuments();
                                            isMove = true;
                                            loadrighttree();
                                        }
                                    })
                                });
                            }
                        });
                    });
                    $download.click(function () {
                        documents = [value.documentId];
                        var type1 = 0;
                        if (argus.type != 1) {
                            type1 = 1;
                        }
                        $.post("/UserDocument/Download", { documentId: value.documentId, displayName: value.displayName, saveName: value.saveName, isFolder: value.isFolder, flag: 0, type: type1 }, function (data) {
                            if (data == "success") {
                                //loadViewToMain("/UserDocument/Download?documentId=" + value.documentId + "&displayName=" + escape(value.displayName) + "&saveName=" + value.saveName + "&isFolder=" + value.isFolder + "&flag=1&type=" + type1);
                                window.location.href = "/UserDocument/Download?documentId=" + value.documentId + "&displayName=" + escape(value.displayName) + "&saveName=" + value.saveName + "&isFolder=" + value.isFolder + "&flag=1&type=" + type1 + "";
                                documents = [];
                            }
                            else {
                                ncUnits.alert("下载失败");
                                documents = [];
                                loadDocuments();
                                isMove = true;
                            }
                        });
                    });
                    $share.click(function () {
                        documents = [value.documentId];
                        ShareDocument = documents;
                        $("#share_modal_chosen").empty();
                        $(".person_list ul").empty();
                        $("#share_modal_chosen_count").text($("#share_modal_chosen li").length);
                        $('#person-selectall').attr("checked", false);
                        // $("#share_modal_chosen").empty();
                        //获取已共享人员
                        $.ajax({
                            type: "post",
                            url: "/Shared/GetSharedUser",
                            dataType: "json",
                            data: { documentId: value.documentId },
                            success: rsHandler(function (data) {

                                $.each(data, function (i, v) {
                                    var $checked = $("<li term=" + v.userId + "><span>" + v.userName + "</spam></li>"),
                                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $close.click(function () {
                                        var nodeId = $(this).parent().attr("term");
                                        $(this).parent().remove();
                                        $("#share_modal_chosen_count").text($("#share_modal_chosen li").length);
                                        $(".person_list ul").each(function () {
                                            if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                                                $(this).find("input[type='checkbox']").prop("checked", false);
                                            }
                                        });
                                    });
                                    $("#share_modal_chosen").append($checked.append($close));

                                    $("#share_modal_chosen_count").text($("#share_modal_chosen li").length);
                                })
                            })
                        });
                        $.ajax({
                            type: "post",
                            url: "/Shared/GetOrganizationList",
                            dataType: "json",
                            data: { parent: null },
                            success: rsHandler(function (data) {
                                for (var i = 0; i < data.length; i++) {
                                    $.each(documents, function (e, j) {
                                        if (j == data[i].id) {
                                            data.splice(i, 1);
                                        }
                                    });
                                }
                                var folderTree = $.fn.zTree.init($("#share_modal_folder"), $.extend({
                                    callback: {
                                        onClick: function (e, id, node) {
                                            $("#share-haschildren").prop("checked", false);
                                            $("#share-selectall").prop("checked", false);
                                            var checked = $("#share-haschildren").prop('checked');
                                            stationWithSub = checked == true ? 1 : 0;
                                            stationOrgId = node.id;
                                            $.ajax({
                                                type: "post",
                                                url: "/Shared/GetUserList",
                                                dataType: "json",
                                                data: { withSub: stationWithSub, organizationId: stationOrgId, withUser: false },
                                                success: rsHandler(function (data) {
                                                    $(".person_list ul").remove();
                                                    $("#person-haschildren").removeAttr("checked");
                                                    $("#person-selectall").removeAttr("checked");
                                                    if (data.length > 0) {
                                                        $.each(data, function (i, v) {
                                                            var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                                            $(".person_list").append($stationHtml);
                                                            $("#share_modal_chosen li").each(function () {
                                                                if ($(this).attr('term') == v.userId) {
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

                    })
                    if (value.isFolder) {
                        $fileName.css("cursor", "pointer");
                        $fileName.click(function () {
                            if (isMove == true) {
                                UpdocPower = value.power;
                                if (value.power == 4 || UpdocPower == 4) {
                                    $("#newFile_building", $(this).a).show();
                                    $("#newDoc_building", $(this).a).show();
                                    argus.folder = value.documentId;
                                    $dirc = $("<span>></span><span class='dirc_span' term='" + value.documentId + "' style='width:auto;'>" + value.displayName + "</span>");
                                    $("#firstdirc_span").append($dirc);
                                    // $(".dic_first").removeClass("dirc_active");
                                    $(".dirc_span:eq(" + ($(".dirc_span").length - 1) + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $(".dirc_span:eq(" + data.length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $('.dirc_span').off('click');
                                    $('.dirc_span').click(function () {
                                        dircclick($(this));
                                    });
                                    loadDocuments();
                                    //更新右侧目录
                                    var n = rightFolderTree.getNodeByParam("id", value.documentId);
                                    rightFolderTree.expandNode(n);
                                    rightFolderTree.selectNode(n);
                                } else if (value.power == 3 || UpdocPower == 3) {
                                    $("#newFile_building", $(this).a).show();
                                    $("#newDoc_building", $(this).a).show();
                                    argus.folder = value.documentId;
                                    $dirc = $("<span>></span><span class='dirc_span' term='" + value.documentId + "' style='width: auto;'>" + value.displayName + "</span>");
                                    $("#firstdirc_span").append($dirc);
                                    $(".dirc_span:eq(" + ($(".dirc_span").length - 1) + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $(".dirc_span:eq(" + data.length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $('.dirc_span').off('click');
                                    $('.dirc_span').click(function () {
                                        dircclick($(this));
                                    });
                                    loadDocuments();
                                    //更新右侧目录
                                    var n = rightFolderTree.getNodeByParam("id", value.documentId);
                                    rightFolderTree.expandNode(n);
                                    rightFolderTree.selectNode(n);
                                } else if (value.power == 2 || argus.type == 3 || argus.type == 4 || UpdocPower == 2) {
                                    $("#newFile_building", $(this).a).hide();
                                    $("#newDoc_building", $(this).a).hide();
                                    argus.folder = value.documentId;
                                    $dirc = $("<span>></span><span class='dirc_span' term='" + value.documentId + "' style='width: auto;'>" + value.displayName + "</span>");
                                    $("#firstdirc_span").append($dirc);
                                    $(".dirc_span:eq(" + ($(".dirc_span").length - 1) + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $(".dirc_span:eq(" + data.length + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $('.dirc_span').off('click');
                                    $('.dirc_span').click(function () {
                                        dircclick($(this));
                                    });
                                    loadDocuments();
                                    //更新右侧目录
                                    var n = rightFolderTree.getNodeByParam("id", value.documentId);
                                    rightFolderTree.expandNode(n);
                                    rightFolderTree.selectNode(n);

                                }
                                else if (value.power == null && argus.type == 2) {
                                    $("#newFile_building", $(this).a).show();
                                    $("#newDoc_building", $(this).a).show();
                                    argus.folder = value.documentId;
                                    $dirc = $("<span>></span><span class='dirc_span' term='" + value.documentId + "' style='width: auto;'>" + value.displayName + "</span>");
                                    $("#firstdirc_span").append($dirc);
                                    $(".dirc_span:eq(" + ($(".dirc_span").length - 1) + ")").addClass("dirc_active").siblings().removeClass("dirc_active");
                                    $('.dirc_span').off('click');
                                    $('.dirc_span').click(function () {
                                        dircclick($(this));
                                    });
                                    loadDocuments();
                                    //更新右侧目录
                                    var n = rightFolderTree.getNodeByParam("id", value.documentId);
                                    rightFolderTree.expandNode(n);
                                    rightFolderTree.selectNode(n);
                                }
                            }
                        });

                    }

                    /*批量勾选 开始*/
                    documents = [];
                    $choose.click(function () {
                        $("#newFile_building", $(this).a).hide();
                        $("#newDoc_building", $(this).a).hide();
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
                            documents.push(value.documentId);

                        }
                    });
                    /*批量勾选 结束*/
                });
                if (mode == 0) {
                    $docList.children().removeClass("col-xs-12").addClass("col-xs-3");
                    $(".grayTop").css("display", "none");
                    $addNewFile = $("<div class='newadd col-xs-3'><div class='add'><div class='addPicture' data-toggle='modal' data-target='#myModal-newfile' style='cursor:pointer'></div><div class='wordPosition'>新建文件夹</div></div></div>");
                    $docChunk.append($addNewFile);
                }
                else {
                    $docChunk.children().removeClass("col-xs-3").addClass("col-xs-12");
                    $(".grayTop").show();
                }

                if (argus.type == 2 || UpdocPower == 4) {
                } else {
                    $addNewFile.hide();
                }
                $("#mode_list").off("click")
                $("#mode_list").click(function () {
                    $(this).addClass("active");
                    $("#mode_card").removeClass("active");
                    $(".operate").css("display", "block");
                    mode = 1;
                    loadDocuments();
                    isMove = true;
                    //$docChunk.children().removeClass("col-xs-3").addClass("col-xs-12").appendTo($docList);
                    //var grayTop=$("<div class='grayTop'></div>");
                    // $nest.prepend(grayTop);

                });

                $("#mode_card").off("click")
                $("#mode_card").click(function () {

                    $(this).addClass("active");
                    $("#mode_list").removeClass("active")

                    mode = 0;
                    loadDocuments();
                    isMove = true;
                    //$addNewFile.remove();

                    //$addNewFile = $("<div class='newadd col-xs-3'><div class='add'><div class='addPicture' data-toggle='modal' data-target='#myModal-newfile' style='cursor:pointer'></div><div class='wordPosition'>新建文件夹</div></div></div>");
                    //$docList.append($addNewFile);

                    ////$col.append($nest);
                    //$docList.children().removeClass("col-xs-12").addClass("col-xs-3").appendTo($docChunk);
                    //if($nest.children.hasClass("grayTop")) {$nest.removeClass("grayTop")};
                });

                /* 选择卡片列表移上去出现绿条 开始 */
                $(".nest").hover(function () {
                    $(".operate", this).toggle();
                });
                $('.moreCancel').hide();
                $('.moreBg').hide();
                $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                $('.xxc_choose span').removeClass('spanHit');
                /* 选择卡片列表移上去出现绿条 结束 */
            })
            /*      ,complete:function(){
             console.log("complete")
             }*/
        });

    }

    function GetTreeByTreeIdFormCopy() {
        $.ajax({
            type: "post",
            url: "/Shared/GetFolderList",
            dataType: "json",
            data: { type: argus.treetype, folder: null },
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
                                    if (argus.treetype == 3) {
                                        for (var i = 0; i < folders.length; i++) {
                                            if (folders[i] == $(this).parent().attr("term")) {
                                                folders.splice(i, 1);
                                            }
                                        }
                                    } else {
                                        for (var i = 0; i < userfolders.length; i++) {
                                            if (userfolders[i] == $(this).parent().attr("term")) {
                                                userfolders.splice(i, 1);
                                            }
                                        }
                                    }
                                });
                                node.mappingLi = $checked;
                                if (argus.treetype == 3) {
                                    folders.push(node.id);
                                } else {
                                    userfolders.push(node.id);
                                }
                            } else {
                                node.mappingLi.remove();
                                if (argus.treetype == 3) {
                                    folders = _.without(folders, node.id);
                                } else {
                                    userfolders = _.without(userfolders, node.id);
                                }
                            }
                        },
                        onNodeCreated: function (e, id, node) {
                            var folderTree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                            if (node.power == 2) {
                                folderTree.setChkDisabled(node, true);
                            }
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
                        otherParam: ["type", argus.treetype],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }




    /*获取到的筛选数据事件处理*/
    var addSelected = (function () {
        var _types = {};
        return function (obj) {
            obj = obj || {};
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
                    isMove = true;

                }
            });

            obj.add();
            isMove = true;
            loadDocuments();
        };
    })();



    /*复制事件 开始*/
    $("#copy_modal_submit").click(function () {
        var copymodel = { documentType: argus.type, documentId: documents, companyFolder: folders, userFolder: userfolders, withAuth: true };
        console.log(copymodel);
        if (copymodel.companyFolder.length > 0) {

            $.ajax({
                type: "post",
                url: "/Shared/Copy",
                dataType: "json",
                data: {
                    data: JSON.stringify(copymodel)
                },
                success: rsHandler(function (data) {
                    if (data == true) {
                        ncUnits.alert("复制成功");
                        loadDocuments();
                        isMove = true;
                        $("#copy_modal").modal("hide");
                        $("#copy_select").val('');
                        $("#copy_modal_chosen_count").text(0);
                        $("#copy_modal_chosen li").remove();
                        isMove = true;
                    }
                    else {
                        ncUnits.alert("复制失败");
                        loadDocuments();
                        isMove = true;
                        $("#copy_modal").modal("hide");
                        $("#copy_select").val('');
                        $("#copy_modal_chosen_count").text(0);
                        $("#copy_modal_chosen li").remove();
                        isMove = true;
                    }
                })
            });
        } else {
            ncUnits.alert("请选择复制路径");
        }
    })
    $("#copy_modal_cancel").click(function () {
        $("#copy_select").val('');
        $("#copy_modal_chosen_count").text(0);
        $("#copy_modal_chosen li").remove();
        isMove = true;
    });
    $("#copy_modal .close").click(function () {
        $("#copy_select").val('');
        $("#copy_modal_chosen_count").text(0);
        $("#copy_modal_chosen li").remove();
        isMove = true;
    });


    /*复制事件 结束*/

    /*共享事件 开始*/
    $("#share_modal_submit").click(function () {
        var userIdList = [];
        $("#share_modal_chosen li").each(function () {
            userIdList.push($(this).attr('term'));
        });
        if (userIdList.length > 0) {
            $.ajax({
                type: "post",
                url: "/Shared/ShareToOthers",
                dataType: "json",
                data: {
                    data: JSON.stringify({
                        documentId: ShareDocument
                        , userId: userIdList,
                        flag: ShareFlag
                    })
                },
                success: rsHandler(function (data) {
                    console.log(data);
                    if (data == true) {
                        ncUnits.alert("共享成功");
                        $("#share_modal").modal("hide");
                        loadDocuments();
                        ShareFlag = 0;
                        isMove = true;
                    } else {
                        ncUnits.alert("共享失败");
                        $("#share_modal").modal("hide");
                        loadDocuments();
                        ShareFlag = 0;
                        isMove = true;
                    }
                })
            });
        } else {
            ncUnits.alert("共享人不能为空");
        }

    })
    /*共享事件 结束*/
    var stationWithSub = 0;
    var stationOrgId;

    function buildDepartmentTree() {
        $.ajax({
            type: "post",
            url: "/Shared/GetFolderList",
            data: { type: argus.treetype, folder: null },
            dataType: "json",
            success: rsHandler(function (data) {
                for (var i = 0; i < data.length; i++) {
                    $.each(documents, function (e, j) {
                        if (j == data[i].id) {
                            data.splice(i, 1);
                        }
                    });
                }
                var folderTree = $.fn.zTree.init($("#move_modal_folder"), $.extend({
                    callback: {
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {
                            //  $("#move_modal_chosen_count").html(folderTree.getCheckedNodes().length);
                            $('#move_modal_chosen_count').text($("#move_modal_chosen li").length);
                            if (node.checked) {
                                var $checked = $("<li>" + node.name + "</li>"),
                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                $("#move_modal_chosen").find("li").remove();
                                $("#move_modal_chosen").append($checked.append($close));
                                $close.click(function () {
                                    folderTree.checkNode(node, undefined, undefined, true);
                                });
                                node.mappingLi = $checked;
                                folders = [];
                                folders.push(node.id);
                                $('#move_modal_chosen_count').text($("#move_modal_chosen li").length);
                                var moveModel = { documentType: argus.type, documentId: documents, folder: folders, withAuth: true }; 
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
                                                loadDocuments();
                                                isMove = true;
                                                loadrighttree();
                                            })
                                        });
                                }); 
                                $("#move_modal .close").click(function () {
                                    $("#move_select").val('');
                                    $("#move_modal_chosen_count").text(0);
                                    $("#move_modal_chosen li").remove();
                                    $('#move_modal_chosen_count').text($("#move_modal_chosen li").length);
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
                            if (node.power == 2) {
                                folderTree.setChkDisabled(node, true);
                            }

                            $("#move_modal_chosen li").each(function () {
                                var departId = $(this).attr('term');
                                $(this).remove();
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
                        radioType: "all",
                        chkboxType: { "Y": "", "N": "" }
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetFolderList",
                        autoParam: ["id=folder"],
                        otherParam: ["type", argus.treetype],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        }); 
    }

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
    var parttern = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    var completeTime = $('#time').val();
    var completeNum = $('#num').val();
    fileUpload();

    function fileUpload() {
      
        $('#fileupload').fileupload({
            url: '/UserDocument/InsertUserDocument',
            dataType: 'text',
            formData: { type: argus.type },
            //acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
            //acceptfiletypes: ".jpg",
            //maxFileSize: 5000,
            add: function (e, data) {
                layer.closeTips();
                var isSubmit = true;
                console.log(data);
                $.each(data.files, function (index, value) {
                    if (!parttern.test(value.name)) {
                        ncUnits.alert("你上传文件格式不对");
                        //ncUnits.confirm({
                        //    title: '提示',
                        //    html: '你上传文件格式不对。'
                        //});
                        isSubmit = false;
                        return;
                    } else if (value.size > 52428800) {
                        ncUnits.alert("你上传文件过大(最大50M)");

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

                        $('span', $fileSpan).on('click', function () {
                            var $this = $(this);
                            ncUnits.confirm({
                                title: '提示',
                                html: '是否确认删除附件？',
                                yes: function (layerID) {
                                    $.ajax({
                                        url: '/UserDocument/DeleteFile',
                                        type: "POST",
                                        data: { savename: file[index].saveName, type: argus.type },
                                    });
                                    $this.parent().parent().remove();
                                    $.each(file, function (i, v) {
                                        if (v.saveName == file[index].saveName) {
                                            file.splice(i, 1);
                                            return false;
                                        }
                                    })
                                    k--;
                                    layer.close(layerID);

                                }
                            });

                        });


                        //html = '<li><div class="up_progress up_progress' + i + '"></div></li>';
                        $('.files ul').append($("<li></li>").append(['<div class="up_progress up_progress' + i + '"></div>', $fileSpan]));
                        j = i;
                        $('.up_progress' + i++ + '').show();
                        //$('.files ul').css({ "border": "1px solid #ccc","margin-bottom": "10px" })
                        if ($('.files ul').height() >= $('.files').height()) { $('.files').css({ "margin-top": "-20px" }) };
                        if ($('.files ul').height() == 0) { $('.files').css({ "margin-top": "0" }) };
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
                    file.push(result.data);
                } else {
                    ncUnits.alert("上传文件内容不能为空");
                    $(".file").parent().parent().remove();

                }


                if (data.result.status == 0) {
                    ncUnits.alert("上传失败");
                    $this.parent().parent().remove();

                } else {
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
    /*新建文件事件开始*/
    //共享人选择
    var partner_v = [];
    var partner_name = [];
    var click = true;
    $("#addplan_partner").searchPopup({
        url: "/BuildNewPlan/GetIpUserByUserId",
        defText: "常用联系人",
        hasImage: true,
        selectHandle: function (data) {
            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span></span>").addClass("glyphicon glyphicon-remove").css({ display: "none" });
            if (partner_v.length == 0) {

                $span.hover(function () {
                    $close.toggle();
                });
                $span.append([data.name, $close]);

                $(this).parent().before($span);
                partner_v.push(data.id);
                partner_name.push(data.name);
            } else {
                if ($.inArray(data.id, partner_v) == -1) {
                    $span.hover(function () {
                        $close.toggle();
                    });
                    $span.append([data.name, $close]);
                    $(this).parent().before($span);
                    partner_v.push(data.id);
                    partner_name.push(data.name);
                } else {

                    ncUnits.alert("不能选择重复");
                }
            }
            $close.click(function () {
                $span.remove();
                partner_v = _.without(partner_v, data.id);
                partner_name = _.without(partner_name, data.name);

            });
        }
    });
    $("#newDoc_modal_submit").click(function () {
        var submitData;
        
        submitData = {
            type: argus.type,
            folder: argus.folder,
            file: file,
            SheraUser: partner_v
        };
        submitData.keyword = lab.get('#myModal-newdoc');
        console.log('submitData', submitData)
        if (file.length == 0) {
            ncUnits.alert("上传文件不能为空!");
            return;
        }
        if (click == true) {
            click = false;
            $.ajax({
                type: "post",
                url: "/Shared/AddFile",
                dataType: "json",
                data: {
                    param: JSON.stringify(submitData)
                },
                success: rsHandler(function (data) {
                    ncUnits.alert("新建成功");
                    $("#myModal-newdoc").modal("hide");
                    click = true;
                    loadDocuments();
                    isMove = true;
                    //    $(".files").empty();
                    //    $(".files").css({"margin-top":"0","border":"none"});
                    file.length = 0;
                })
            });
        }
        if ($(".files ul").empty()) { $(".files").css({ "margin-top": "0", "border-style": "none" }); }
    });
    //结束
    /*新建文件夹事件*/

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
        if (click == true) {
            click = false;
            $.ajax({
                type: "post",
                url: "/Shared/BuildNewUserFolder",
                dataType: "json",
                data: {
                    type: argus.type,
                    folder: argus.folder, //上级文件夹
                    folderName: Filename,//文件夹名称
                    description: descriptions //描述
                },
                success: rsHandler(function (data) {
                    ncUnits.alert("新建文件夹成功!");
                    $("#myModal-newfile").modal("hide");
                    click = true;
                    $("#input_filename").val("")
                    $("#input_filedes").val("");
                    loadDocuments();
                    isMove = true;
                    loadrighttree();


                })
            });
        }
    });
    $("#filterbox_document_name").keyup(function (e) {
        if (e.keyCode == 13) {
            var name = $(this).val().trim();
            if (name.length) {
                addSelected({
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
            addSelected({
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

    //包含下级
    $("#person-haschildren").click(function () {
        $(".person_list ul").remove();
        var checked = $(this).prop('checked');
        stationWithSub = checked == true ? 1 : 0;
        $.ajax({
            type: "post",
            url: "/Shared/GetUserList",
            dataType: "json",
            data: { withSub: stationWithSub, organizationId: stationOrgId, withUser: false },
            success: rsHandler(function (data) {
                if (data.length > 0) {
                    $.each(data, function (i, v) {
                        var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                        $(".person_list").append($stationHtml);
                        $("#share_modal_chosen li").each(function () {
                            if ($(this).attr('term') == v.userId) {
                                $stationHtml.find("input[type='checkbox']").attr('checked', true);
                            }
                        });
                    });
                    appendstation();
                }
            })
        });
    });
    $(".directory-set").click(function () {
        var n = rightFolderTree.getNodeByParam("id", argus.folder);
        rightFolderTree.cancelSelectedNode(n);
        $(this).find("span").css("color", "#58b456");
        argus.folder = null;
        loadDocuments();
        isMove = true;
    });
    //选择全部
    $('#person-selectall').click(function () {
        var allChecked = $(this).prop("checked");
        if (allChecked == true) {
            $(".person_list ul").each(function () {
                var term = $(this).find("li:eq(1)").attr("term");
                $("#share_modal_chosen li").each(function () {
                    if ($(this).attr('term') == term) {
                        $(this).remove();
                    }
                });
            });
            $(".person_list ul input[type='checkbox']").prop('checked', true);
            var length = $(".person_list input[type='checkbox']:checked").length

            for (var i = 0; i < length; i++) {
                var stationId = $(".person_list ul:eq(" + i + ")").find("li:eq(1)").attr('term');
                var stationName = $(".person_list ul:eq(" + i + ")").find("li:eq(1) span:eq(0)").text();
                var $checked = $("<li term=" + stationId + "><span>" + stationName + "</spam></li>"),
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $("#share_modal_chosen li").each(function () {
                    if ($(this).attr('term') == stationId) {
                        $(this).remove();
                        $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
                    }
                });
                $("#share_modal_chosen").append($checked.append($close));

                $close.click(function () {
                    var $thisId = $(this).parent().attr('term');
                    $(this).parent().remove();
                    $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
                    $(".person_list ul").each(function () {
                        if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                            $(this).find("input[type='checkbox']").prop("checked", false);
                        }
                        $('#person-selectall').attr("checked", false);
                    });
                    if ($("#share_modal_chosen li").length == 0 && allChecked == true) {
                        $('#person-selectall').attr("checked", false);
                    }
                });
            }
            $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
        }
        else {
            $(".person_list ul input[type='checkbox']").prop('checked', false);
            $(".person_list ul").each(function () {
                var term = $(this).find("li:eq(1)").attr("term");
                $("#share_modal_chosen li").each(function () {
                    if ($(this).attr('term') == term) {
                        $(this).remove();
                    }
                });
            });
            var length = $("#share_modal_chosen li").length
            $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
        }
    });


    function appendstation() {
        $(".person_list input[type='checkbox']").click(function () {
            var checked = $(this).prop('checked');
            var stationId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
            var stationName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
            if (checked == true) {
                $(this).attr('checked', true);
                $('#share_modal_chosen_count').text($("#share_modal_chosen li").length + 1);
                var $checked = $("<li term=" + stationId + "><span>" + stationName + "</spam></li>"),
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $("#share_modal_chosen li").each(function () {
                    if ($(this).attr('term') == stationId) {
                        $(this).remove();
                    }
                });

                $("#share_modal_chosen").append($checked.append($close));
                $close.click(function () {
                    var $thisId = $(this).parent().attr('term');
                    $(this).parent().remove();
                    $('#share_modal_chosen_count').text($("#station_modal_chosen li").length);
                    $(".person_list ul").each(function () {
                        if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                            $(this).find("input[type='checkbox']").prop("checked", false);
                        }
                    });
                });
            } else {
                $(this).attr('checked', false);
                $("#share_modal_chosen li").each(function () {
                    if ($(this).attr('term') == stationId) {
                        $(this).remove();
                        $('#share_modal_chosen_count').text($("#share_modal_chosen li").length);
                    }
                });

            }
        });
    }

});


//弹出附件下载失败的提示信息
function noFile() {
    ncUnits.alert("文件不存在，无法下载！");
}
