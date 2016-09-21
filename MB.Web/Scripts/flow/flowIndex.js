/**
 * Created by DELL on 2015/6/24.
 */

$(function () {
    var addflow_templateName = "", addflow_createUserName = "", addflow_createdepart = "", addflow_createTime = "", addflow_defaultTitle = 0;
    var firstLoad = false;
    //判断登录者是否是管理员
    var admin = $("#showAdmin").val();

    if (admin == "1") {
        $('.select_flowing').show();
        $('.select_hasSubmited').hide();
        $('.select_unChecked').hide();
        $('.select_processed').hide();
        $('.select_unRead').hide();
    }
    else {
        $('.select_flowing').hide();
        $('.select_hasSubmited').show();
        $('.select_unRead').show();
        $('.select_unChecked').show();
        $('.select_processed').show();
    }
    //获取列表参数（含筛选，排序）
    var argus = {
        type: null,//流程分类  
        status: [], //状态  1、待提交 2、已提交 3、待处理 4、已处理 5、已办结 6、待审核 7、待查阅 8、已审核 9、已查阅
        time: [], //创建时间  1、近一周  2、近一月  3、自定义
        person: [],    //创建人
        department: [],//创建部门
        sort: [{
            type: 0,
            direct: 0
        }]  //type:1、默认  2、紧急度   direct:1、升序  0、降序
    };
    var folder = null,
        treetype = 2,
        result = []

    //页面下拉列表重载hover事件
    $('[data-hover="dropdown"]').dropdownHover();
    /* 圆饼 开始 */

    var date = new Date()
        , year = date.getFullYear()
        , month = date.getMonth() + 1
        , $con = $(".panel-chart")
        , colors = com.ztnc.targetnavigation.unit.planStatusColor;

    //右侧个人信息
    loadPersonalInfo();

    function drawPlanProgress() {
        $("#chart3", $con).empty();

        var lodi = getLoadingPosition('.chart');//显示load层
        $.ajax({
            type: "post",
            url: "/FlowIndex/GetFlowProcessList",
            dataType: "json",
            data: {
                year: year,
                month: month,
                admin: admin
            },
            success: rsHandler(function (data) {
                //alert();
                var dountData = []
                    , count = 0
                    , $ul = $("<ul></ul>");

                for (var i = 0, len = data.length; i < len; i++) {
                    var color = colors[i];
                    var num = data[i].count;
                    if (num != 0) {
                        count += num;
                        dountData.push([num, color, data[i].id, data[i].text + "流程"]);
                    }
                    $ul.append('<li><span class="color" style="background-color:' + color + '"></span><span class="text">' + data[i].text + '</span></li>');
                    //$("#chart3 circle:eq(0)").css("cursor", "pointer");
                }
                $(".sign", $con).html($ul);
                Raphael("chart3", 330, 310).dountChart(165, 155, 55, 70, 110, dountData, function (data) {
                    //饼图click事件
                    $(".hashandle").slideDown();
                    //清空筛选条件
                    $(".selecteds").empty();
                    argus.status = [];
                    argus.time = [];
                    argus.person = [];
                    argus.department = [];
                    argus.sort = [{ type: 1, direct: 0 }];
                    $('.selection .selects a').removeClass('active');

                    //拼上时间
                    var day;
                    if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
                        day = 31;
                    }
                    else if (month == 4 || month == 6 || month == 9 || month == 11) {
                        day = 30;
                    }
                    else if (month = 2) {
                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && year % 100 == 0))//闰年
                        {
                            day = 29;
                        }
                        else {
                            day = 28;
                        }
                    }
                    var timeStart = year + '-' + month + '-01';
                    var timeEnd = year + '-' + month + '-' + day;
                    var timeQuan = timeStart + '至' + timeEnd;

                    if (timeQuan.length) {
                        addSelected({
                            flashFlag: 1,
                            text: timeQuan,
                            type: "time",
                            add: function () {
                                argus.time = [3, timeStart, timeEnd];
                            },
                            delete: function () {
                                argus.time = [];
                            }
                        });
                    }
                    $('.selection .status a').each(function () {
                        if ($(this).text() == data[3].substr(0, 3)) {
                            $(this).click();
                        }
                    });
                });

                $(".planNum span", $con).html(count);
                $(".month .text", $con).html(year + "年" + month + "月");
            }),
            complete: rcHandler(function () {             
                lodi.remove();         //关闭load层
            })
        });
    }

    drawPlanProgress();
    $(".arrowsBBLCom", $con).click(function () {
        var date = new Date()
            , thisyear = date.getFullYear()
            , thismonth = date.getMonth() + 1;
        if (year == thisyear && month == thismonth) {
            $(".arrowsBBRCom", $con).show();
        }
        if (month == 1) {
            year--;
            month = 12;
        } else {
            month--;
        }
        $(".month .text", $con).html(year + "/" + month);
        drawPlanProgress();
    });

    $(".arrowsBBRCom", $con).click(function () {
        var date = new Date()
            , thisyear = date.getFullYear()
            , thismonth = date.getMonth() + 1;
        if (month == 12 && year < date.getFullYear()) {
            year++;
            month = 1;
        } else {
            month++;
        }
        $(".month .text", $con).html(year + "/" + month);
        if (year >= thisyear && month >= thismonth) {
            $(this).hide();
        }
        drawPlanProgress();
    });
    /* 圆饼 结束 */

    //点击排序
    $('.orderby').click(function () {
        $(this).parents('.xxc_order').addClass('active').siblings().removeClass('active');
    })

    //类别筛选的显示隐藏
    //$('.dropdown').hover(function () {
    //    $(this).addClass('open');
    //}, function () {
    //    $(this).removeClass('open');
    //});

    //绑定流程类别
    $.ajax({
        type: "post",
        url: "/FlowIndex/GetTemplateCategoryList",
        dataType: "json",
        success: rsHandler(function (data) {
            if (data && data.length > 0) {
                $.each(data, function (e, i) {
                    var $category = $("<li><a href='#' term='" + i.categoryId + "' title='" + i.categoryName + "' >" + i.categoryName + "</a></li>"),
                        $line = $("<li class='divider short'></li>");
                    $('.xxc_category').append([$category, $line]);

                    $category.click(function () {
                        selectCategoryFlow($(this));
                    });
                });
                $('.xxc_category li:last').remove();
                $('.xxc_category').css('min-width', '200px');
                //全部注册事件
                $("#category_all").click(function () {
                    selectCategoryFlow($(this));
                });

            }
            //$("#xxc_selectedcate").text($('.xxc_category li:eq(0)').text());
        })
    });

    //流程类别搜索列表
    function selectCategoryFlow(obj) {
        $("#xxc_selectedcate").text(obj.text().length > 12 ? obj.text().substr(0, 12) : obj.text()).attr('title', obj.text());
        argus.type = obj.find('a').attr('term');
        loadFlows();
    }

    //筛选框组织部门树展开
    $(".directory-set").click(function () {
        if ($(this).children().hasClass("glyphicon-chevron-down")) {
            $(this).children(".department").removeClass("glyphicon-chevron-down");
            $(this).children(".department").addClass("glyphicon-chevron-up");
            $(".ztree").slideDown();
        } else {
            $(this).children(".department").addClass("glyphicon-chevron-down");
            $(this).children(".department").removeClass("glyphicon-chevron-up");
            $(".ztree").slideUp();
        }
    });
    /*筛选框出现隐藏*/
    $(".sift").click(function () {
        $(".hashandle").slideToggle();
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");

        }
        else $(this).addClass("active");
    });
    $(".drawer-handle").click(function () {
        $(".hashandle").slideUp();
        $(".sift").removeClass("active");

    });
    $(".moreCancel").click(function () {
        $(".hashandle").slideUp();
        $(".sift").removeClass("active");

    });
    /*获取到的筛选数据事件处理*/
    var addSelected = (function () {
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
            if (obj.nodeId) {
                $delete.attr('departId', obj.nodeId);
            }
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
                    loadFlows();
                }
            });

            obj.add();
            if (obj.flashFlag && obj.flashFlag == 1) {
                return;
            }
            if (firstLoad) {
                loadFlows();
            }
        };
    })();

    /*状态获取开始*/
    $('.status a').click(function () {
        var $this = $(this);
        if (!$this.hasClass('active')) {
            addSelected({
                text: $this.text(),
                add: function () {
                    $this.addClass("active");
                    argus.status.push(parseInt($this.attr('term')));
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.status = _.without(argus.status, parseInt($this.attr('term')));
                }
            })
        }
    });
    /*状态获取结束*/
    /*时间获取开始*/
    $("#near_month").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addSelected({
                text: "近一月",
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
    $("#near_week").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addSelected({
                text: "近一周",
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
    $("#user_defined").click(function () {
        var starttime = $("#start_time").val();
        var endtime = $("#end_time").val();
        var text = (starttime.length ? "" + starttime : "") + (endtime.length ? "至" + endtime : "");
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
        url: "/FlowIndex/GetOffUser",
        dataType: "json",
        success: rsHandler(function (data) {
            $.each(data.reverse(), function (i, v) {
                var $staff = $("<a href='#' class='option'>" + v.name + "</a>");
                $("#creaters").prepend($staff);
                $staff.click(function () {
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
                });
            });
        })
    });
    $("#commonUser").searchPopup({
        url: "/FlowIndex/GetOffUser",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            if (_.indexOf(argus.person, data.userId) == -1) {
                addSelected({
                    id: data.id,
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

    /*部门列表加载*/
    $.ajax({
        type: "post",
        url: "/Shared/GetOrganizationList",
        dataType: "json",
        data: { parent: null },
        success: rsHandler(function (data) {
            var folderTree;
            folderTree = $.fn.zTree.init($("#catalogue"), $.extend({
                callback: {
                    beforeClick: function (id, node) {
                        folderTree.checkNode(node, undefined, undefined, true);
                        return false;
                    },
                    onCheck: function (e, id, node) {
                        /*$(".selecteds").html(folderTree.getCheckedNodes().length);*/
                        if (node.checked) {
                            addSelected({
                                nodeId: node.id,
                                text: node.name,
                                add: function () {
                                    $(this).addClass("active");
                                    argus.department.push(node.id);
                                },
                                delete: function () {
                                    argus.department = _.without(argus.department, node.id);
                                    var ztree = $.fn.zTree.getZTreeObj("catalogue");
                                    var n = ztree.getNodeByParam("id", node.id);
                                    ztree.checkNode(n, undefined, undefined, true);

                                }
                            });
                            //$close.click(function () {
                            //    folderTree.checkNode(node, undefined, undefined, true);
                            //});
                            //node.mappingLi = $checked;
                            //folders.push(node.id);
                        } else {
                            $(".selecteds .selected").each(function () {
                                if ($(this).find(".glyphicon-remove").attr("departid") == node.id) {
                                    $(this).remove();
                                }
                            });
                            for (var i = 0; i < argus.department.length; i++) {
                                if (argus.department[i] == node.id) {
                                    argus.department.splice(i, 1);
                                    break;
                                }
                            }
                            loadFlows();
                        }
                    },
                    onNodeCreated: function (e, id, node) {
                        $(".selecteds li").each(function () {
                            var departId = $(this).attr('term');
                            if (parseInt(departId) == node.id) {
                                $(this).remove();
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
                    url: "/Shared/GetOrganizationList",
                    autoParam: ["id=parent"],
                    dataFilter: function (treeId, parentNode, responseData) {
                        return responseData.data;
                    }
                }
            }), data);
        })
    });
    /*清除筛选*/
    $("#removeFilter").click(function () {
        $(".selecteds .glyphicon-remove").trigger("click", true);
        loadFlows();
    });

    /* sortBar加载 */
    $(".sortbar").sortBar();

    /*排序条绑定事件 开始*/
    $(".sortbar").on("clicked.sortbar", function (e, order, sort) {
        argus.sort = [];
        var sortModel = {
            type: (parseInt(order) - 2),
            direct: sort
        }
        argus.sort.push(sortModel);
        loadFlows();
    });

    /*排序条绑定事件 结束*/

    /*流程列表加载开始*/
    // var formDetail;
    if (admin=="1") {
        loadFlows();
        firstLoad = true;
    }
    else if (admin=="2") {
        $('.select_unSubmit').click();
        firstLoad = true;
        $('.select_hasSubmited').click();
    }
    else{
        $('.select_unRead').click();
        firstLoad = true;
        $('.select_unChecked').click();
    }
    
    function loadFlows() {
        var list_lodi = getLoadingPosition('.row-flowChunk');  //加载局部视图的对象
        $.ajax({
            type: "post",
            url: "/FlowIndex/GetFlowIndexJson",
            dataType: "json",
            data: { admin: admin, data: JSON.stringify(argus) },
            complete: rcHandler(function () {             
                list_lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                //console.log(data);
                var $flowChunk = $(".row-flowChunk");
                $flowChunk.empty();
                $.each(data, function (n, value) {
                    var $col = $("<div class='col-xs-6'></div>");
                    var $cell = $("<div class='cell' term='" + value.operateStatus + "'></div>");
                    var $nest = $("<div class='nest'></div>");
                    var $smallImage = $("<div class='pull-left'><img class='img-thumbnail img-circle x64 ' src='" + value.img + "'></div>");
                    var $content = $("<div class='pull-right info-list'></div>");
                    var $templateName = $("<div class='templateName' term='" + value.templateId + "'><ul class='list-inline'><li>工作流:</li><li>" + value.templateName + "</li></ul></div>");
                    var $title = $("<div class='title' term='" + value.formId + "'><ul class='list-inline'><li>请求标题:</li><li class='flow_title' title='" + value.title + "'>" + value.title + "</li></ul></div>");

                    var operateUserNames = "";
                    if (value.operate && value.operate.length > 0) {
                        for (var i = 0; i < value.operate.length; i++) {

                            if (value.operate[i].mandataryUser != null) {
                                operateUserNames += value.operate[i].mandataryUserName;
                                if (value.operate.length > (i + 1)) {
                                    operateUserNames += "，";
                                }
                            }
                            else {
                                operateUserNames += value.operate[i].name;
                                if (value.operate.length > (i + 1)) {
                                    operateUserNames += "，";
                                }
                            }

                        }
                    }
                    var $noOperateUser = $("<div class='noOperate'><ul class='list-inline'><li style='vertical-align: top;'>当前未操作者:</li><li style='width: 200px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;' title='" + operateUserNames + "'>" + operateUserNames + "</li></ul></div>");
                    var $node = $("<div class='node' term='" + value.currentNode + "'><ul class='list-inline'><li>当前节点:</li><li>" + value.nodeName + "</li></ul></div>");
                    var startNum = "";
                    if (value.urgency && value.urgency > 0) {
                        for (var i = 0; i < value.urgency; i++) {
                            startNum += "<li class='liHit'></li>";
                        }
                    } else {
                        startNum = "<li></li><li></li><li></li><li></li><li></li>";
                    }
                    var $urgency = $("<span>紧急度：<span class='urgency urgencyR'><ul>" + startNum + "</ul></span></span>");
                    var $operate = $("<div class='operate' term='" + value.operateStatus + "'></div>");
                    var $btns = $("<div class='btns'><ul class='list-inline'></ul></div>");
                    var $details = $("<li term='" + value.isEntruct + "'><a class='detailsWhite' href='#' data-toggle='modal' data-target='#details-modal'  >详情</a></li>");
                    var $recall = $("<li><a class='recall' href='#'>撤回</a></li>");
                    var $cancel = $("<li><a class='recall' href='#'>撤消</a></li>");
                    var $delete = $("<li><a class='flow_delete' href='#'>删除</a></li>");
                    var $transpont = $("<li><a class='transmit' href='#'data-toggle='modal' data-target='#HR_modal'id='transmit'>转发</a></li>");
                    //var $return = $("<li><a class='return' href='#'>退回</a></li>");
                    var $submit = $("<li><a class='submit' href='#'>提交</a></li>");
                    var $suggestion = $("<li><a class='modify' href='#'data-toggle='modal' data-target='#suggestion_modal'id='suggestion'>意见</a></li>");
                    //var $agree = $("<li><a class='agree'href='#'>同意</a></li>");
                    var date = "", time = "";
                    if (value.createTime) {
                        date = value.createTime.toString().substring(0, value.createTime.toString().indexOf('T'));
                        time = value.createTime.toString().substring(value.createTime.toString().indexOf('T') + 1, value.createTime.toString().lastIndexOf(':'));
                    }
                    var $bottomUser = $("<div class='bottomUser'>" +
                    "<span>" + value.createUserName + "</span>" +
                    "<span>" + date + "</span><span>" + time + "</span>" +
                    "</div>");
                    $content.append([$templateName, $title, $noOperateUser, $node, $urgency]);
                    $nest.append([$smallImage, $content]);
                    if (admin == "1") {
                        if (value.operateStatus == 1) { //待提交
                            $btns.children('ul').append($details);
                        }
                        else if (value.operateStatus == 11) {  //流程中
                            $btns.children('ul').append($details, $delete, $recall);
                        }
                        else if (value.operateStatus == 5)//已查阅或者已办结显示按钮
                        {
                            $btns.children('ul').append([$details]);
                        }
                    }
                    else {
                        if (value.operateStatus == 1) { //待提交
                            $btns.children('ul').append([$details, $submit, $transpont, $delete]);
                        }
                        else if (value.operateStatus == 2) //已提交显示按钮
                        {
                            $btns.children('ul').append([$details, $transpont]);
                        }
                        else if (value.operateStatus == 13) {   //已提交，但是没有被操作过
                            $btns.children('ul').append([$details, $transpont,$cancel]);
                        }
                        else if (value.operateStatus == 6)//待审核显示按钮
                        {
                            $btns.children('ul').append([$details, $transpont]);
                        }
                        else if (value.operateStatus == 7)//待查阅显示按钮
                        {
                            $btns.children('ul').append([$details, $suggestion, $transpont]);
                        }
                        else if (value.operateStatus == 8)//已审核显示按钮
                        {
                            $btns.children('ul').append($details, $transpont);
                        }
                        else if (value.operateStatus == 9 || value.operateStatus == 5 || value.operateStatus == 12)//已查阅或者已办结或者委托显示按钮
                        {
                            $btns.children('ul').append([$details, $transpont]);
                        }
                        else if (value.operateStatus == 10) { //提交节点
                            $btns.children('ul').append([$details, $transpont]);
                        }
                    }
                    $btns.appendTo($operate);
                    $cell.append([$nest, $bottomUser, $operate]);
                    $col.append($cell).appendTo($flowChunk);

                    //提交
                    $submit.click(function () {
                        var templateId = $(this).parents('.cell').find('.templateName').attr('term');
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        var nodeId = $(this).parents('.cell').find('.node').attr('term');
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要提交吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/SubmitFlow",
                                    dataType: "json",
                                    data: { templateId: templateId, formId: formId, nodeId: nodeId,flag:0 },
                                    success: rsHandler(function (data) {
                                        if (data) {
                                            ncUnits.alert("流程提交成功!");
                                            loadFlows();
                                            drawPlanProgress();
                                        }
                                        else {
                                            ncUnits.alert("流程设置有误，请联系管理员!");
                                        }
                                    })
                                });
                            }
                        });
                    });

                    //没有被操作过的撤回
                    $cancel.click(function () {
                        var templateId = $(this).parents('.cell').find('.templateName').attr('term');
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        var nodeId = $(this).parents('.cell').find('.node').attr('term');

                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要撤回？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/CancelSubmit",
                                    dataType: "json",
                                    data: { templateId: templateId, formId: formId, nodeId: nodeId },
                                    success: rsHandler(function (data) {
                                        if (data) {
                                            ncUnits.alert("撤回操作成功!");
                                            loadFlows();
                                            drawPlanProgress();
                                        }
                                        else {
                                            ncUnits.alert("流程设置有误，请联系管理员!");
                                        }

                                    })
                                });
                            }
                        });
                    });

                    //撤回
                    $recall.click(function () {
                        var templateId = $(this).parents('.cell').find('.templateName').attr('term');
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        var nodeId = $(this).parents('.cell').find('.node').attr('term');
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要撤回？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/BackFirstNode",
                                    dataType: "json",
                                    data: { nodeId: nodeId, templateId: templateId, formId: formId },
                                    success: rsHandler(function (data) {
                                        if (data) {
                                            ncUnits.alert("撤回操作成功!");
                                            loadFlows();
                                            drawPlanProgress();
                                        }
                                        else {
                                            ncUnits.alert("流程设置有误，请联系管理员!");
                                        }

                                    })
                                });
                            }
                        });
                    });

                    //删除
                    $delete.click(function () {
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/deleteUserForm",
                                    dataType: "json",
                                    data: {
                                        formId: formId
                                    },
                                    success: rsHandler(function (data) {
                                        ncUnits.alert("删除成功!");
                                        loadFlows();
                                        drawPlanProgress();
                                    })
                                })
                            }
                        });

                    });

                    //详情
                    var newVal;
                    $details.click(function () {
                        var templateId = $(this).parents('.cell').find('.templateName').attr('term');
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        var nodeId = $(this).parents('.cell').find('.node').attr('term');
                        var operateStatus = $(this).parents('.operate').attr('term');
                        var isEntruct = $(this).attr('term');  //是否是委托流程

                        $("#details-modal .modal-content").load("/FlowIndex/LoadDetail", function () {

                            if (operateStatus==1) {
                                /* 评分五角星 开始 */
                                $('.stars ul li').hover(function () {
                                    var nums = $(this).index();
                                    var length = $(this).parent().children('li').length - 1;
                                    for (var i = 0; i <= length; i++) {
                                        if (i <= nums) {
                                            $(this).parent().children('li').eq(i).addClass('liHover');
                                        }
                                        else {
                                            $(this).parent().children('li').eq(i).addClass('liHoverNot');
                                        }
                                    }
                                }, function () {
                                    $(this).parent().children('li').removeClass('liHover').removeClass('liHoverNot');
                                });
                                $('.stars ul li').click(function () {
                                    var nums = $(this).index();
                                    var length = $(this).parent().children('li').length - 1;
                                    for (var i = 0; i <= length; i++) {
                                        if (i <= nums) {
                                            $(this).parent().children('li').eq(i).addClass('liHit');
                                        }
                                        else {
                                            $(this).parent().children('li').eq(i).removeClass('liHit');
                                        }
                                    }
                                });
                                /* 评分五角星 结束 */

                                //选择完成时间
                                $("#addflow_createtime").click(function () {
                                    laydate({
                                        elem: '#flowdetail_createtime_v',
                                        event: 'click',
                                        istime: true,
                                        format: 'YYYY-MM-DD hh:mm',
                                        isclear: true,
                                        istoday: true,
                                        issure: true,
                                        festival: true,
                                        start: new Date().toLocaleDateString() + ' 08:30:00',
                                        choose: function (dates) {
                                            endTime_v = dates;
                                            if (addflow_defaultTitle == 1) {
                                                addflow_createTime = dates.replace('-', '/').replace('-', '/');
                                                $(".detail_form_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                                            }
                                        },
                                        clear: function () {
                                            endTime_v = undefined;
                                        }
                                    });
                                });

                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/GetUserOrganizationList",
                                    dataType: "json",
                                    success: rsHandler(function (data) {
                                        if (data) {
                                            var orgId = data.orgList[0].id;
                                            $(".detail_department_ul").empty();
                                            if (data.orgList && data.orgList.length > 0) {
                                                $.each(data.orgList, function (e, i) {
                                                    var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_detail_departmentName'>" + i.name + "</a></li><li class='divider short'></li>");
                                                    $(".detail_department_ul").append($content);
                                                    $(".detail_department_ul .xxc_detail_departmentName").off('click');
                                                    $(".detail_department_ul .xxc_detail_departmentName").click(function () {
                                                        orgId = $(this).attr('term');
                                                        $("#detail_createdepart_span").text($(this).text()).attr('term', orgId);
                                                        if (addflow_defaultTitle == 1) {
                                                            addflow_createdepart = $("#detail_createdepart_span").text();
                                                            $(".detail_form_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                                                        }

                                                        //加载岗位
                                                        load_station(orgId);
                                                    });
                                                });
                                            }
                                            $("#detail_createdepart_span").text($('.detail_department_ul .xxc_detail_departmentName:eq(0)').text()).attr('term', $('.detail_department_ul .xxc_detail_departmentName:eq(0)').attr('term'));
                                            addflow_createdepart = $("#detail_createdepart_span").text();
                                            $(".detail_department_ul li:last").remove();
                                            //加载岗位
                                            //load_station(orgId);

                                            //加载表单
                                            loaddetail();
                                        }
                                    })
                                });

                                //加载岗位
                                function load_station(orgId, stationName) {
                                    $.ajax({
                                        type: "post",
                                        url: "/FlowIndex/GetUserStationList",
                                        dataType: "json",
                                        data: { orgId: orgId },
                                        success: rsHandler(function (data) {
                                            if (data && data.length > 0) {
                                                $(".detail_station_ul").empty();
                                                $.each(data, function (e, i) {
                                                    var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_detail_stationName'>" + i.name + "</a></li><li class='divider short'></li>");
                                                    $(".detail_station_ul").append($content);
                                                    $(".detail_station_ul .xxc_detail_stationName").off('click');
                                                    $(".detail_station_ul .xxc_detail_stationName").click(function () {
                                                        var orgId = $(this).attr('term');
                                                        $("#detail_createstation_span").text($(this).text()).attr('term', orgId);
                                                    });
                                                });
                                                $(".detail_station_ul li:last").remove();
                                                if (stationName) {
                                                    $('.detail_station_ul .xxc_detail_stationName').each(function () {
                                                        if ($(this).text() ==stationName) {
                                                            $("#detail_createstation_span").text($(this).text()).attr('term', $(this).attr('term'));
                                                        }
                                                    });
                                                } else {
                                                    $("#detail_createstation_span").text($('.detail_station_ul .xxc_detail_stationName:eq(0)').text()).attr('term', $('.detail_station_ul .xxc_detail_stationName:eq(0)').attr('term'));
                                                }
                                                
                                            }
                                        })
                                    });
                                }
                            } else {
                                loaddetail();
                            }

                            var modalExtend = $("#modal_detail_flowchart_con");
                            $("#modal_detail_flowchart_switch").click(function () {
                                if ($(this).hasClass("unfold")) {
                                    $(this).removeClass("unfold");
                                    modalExtend.animate({
                                        width: "0",
                                        right: "1px",
                                        "z-index": "0"
                                    }, 1000);
                                } else {
                                    $(this).addClass("unfold");
                                    modalExtend.animate({
                                        width: "400px",
                                        right: "-399px",
                                        "z-index": "-1"
                                    }, 1000, "", function () {
                                        renderFlowChart(formId);
                                    });
                                }
                            });
                            //加载表单列表
                            function loaddetail() {
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/GetTemplateInfoById",
                                    dataType: "json",
                                    data: {
                                        templateId: templateId,
                                        nodeId: nodeId,
                                        formId: formId,
                                        flag: 2,
                                        operateStatus: operateStatus
                                    },
                                    success: rsHandler(function (data) {
                                        //if (admin==1) {
                                        //    $('.flowDetailMask').show();
                                        //}
                                        //else if (operateStatus==2) {
                                        //    $('.flowDetailMask').show();
                                        //}
                                        //else {
                                        //    $('.flowDetailMask').hide();
                                        //}
                                        addflow_templateName = data.template.templateName;
                                        addflow_defaultTitle = data.template.defaultTitle;
                                        if (operateStatus == 1 && addflow_defaultTitle != 1) {
                                            $('.detail_form_title').removeAttr('readonly');
                                        } else {
                                            $('.detail_form_title').attr('readonly', 'true');
                                        }
                                        $('.detial_template_name').text(addflow_templateName);
                                        $('.detail_template_desc').text(data.template.description);
                                        $('#detail_modal_title').text(addflow_templateName);
                                        //渲染表单
                                        newVal = $("#detail_label-content").widgetFormResolver(data.controlInfo);
                                        var $suggest = $("<div class='flow_message_title'>建议/意见</div><div>" +
                                                         "<textarea class='flow_message_content' maxlength='250' id='flow_message'></textarea></div>");
                                        $('#xxc_detailsuggest').append($suggest).css("margin-top", "10px");
                                        //获取表单控件值
                                        $.ajax({
                                            type: "post",
                                            url: "/FlowIndex/GetFlowDetailListById",
                                            dataType: "json",
                                            data: { formId: formId, nodeId: nodeId },
                                            success: rsHandler(function (data) {
                                                if (data) {
                                                    $('.detail_form_title').val(data.title);
                                                    if (data.urgency > 0) {
                                                        $("#detail_urgencyStart li:lt(" + data.urgency + ")").addClass('liHit');
                                                    }
                                                    $('#detail_createuser_input').val(data.createUserName);
                                                    addflow_createUserName = data.createUserName;
                                                    addflow_createdepart = data.organizationName;
                                                    addflow_createTime = data.createTime.toString().replace('T', ' ').substr(0, 16);
                                                    if (operateStatus == 1) {
                                                        $('.detail_department_ul .xxc_detail_departmentName').each(function () {
                                                            if ($(this).text() == data.organizationName) {
                                                                $("#detail_createdepart_span").text($(this).text()).attr('term', $(this).attr('term'));
                                                                load_station($(this).attr('term'), data.stationName);
                                                            }
                                                        });

                                                    } else {
                                                        $('.detail_department_ul').remove();
                                                        $('.detail_station_ul').remove();
                                                        $('#detail_modal_content .caret').remove();
                                                        $('#detail_createdepart_span').parent().attr('readonly', 'true');
                                                        $('#detail_createstation_span').parent().attr('readonly', 'true');
                                                        $('#flowdetail_createtime_v').attr('readonly', 'true')
                                                        $('#detail_createdepart_span').text(addflow_createdepart).attr('title', addflow_createdepart);
                                                        $('#detail_createstation_span').text(data.stationName).attr('title', data.stationName);
                                                    }
                                                    $('#flowdetail_createtime_v').val(addflow_createTime);
                                                    if (data.controlValue && data.controlValue.length > 0) {
                                                        for (var i = 0; i < data.controlValue.length; i++) {
                                                            var obj;
                                                            if (data.controlValue[i].parentControl == "" || data.controlValue[i].parentControl == null || data.controlValue[i].parentControl == "main") {  //非明细列表中的控件

                                                                if (data.controlValue[i].controlType == 0) {   //标签
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    obj.find('.item-label').text(data.controlValue[i].description);
                                                                }
                                                                else if (data.controlValue[i].controlType == 1 || data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3 || data.controlValue[i].controlType == 11) { //输入框
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    obj.find('.item-content .form-control').val(data.controlValue[i].detailValue[0]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 4 || data.controlValue[i].controlType == 5) {  //单选框及复选框
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    for (var j = 0; j < data.controlValue[i].detailValue.length; j++) {
                                                                        if (data.controlValue[i].detailValue[j] == "false") {
                                                                            obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", false);
                                                                        }
                                                                        else {
                                                                            obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", true);
                                                                        }
                                                                    }
                                                                }
                                                                else if (data.controlValue[i].controlType == 6) {  //下拉列表
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    obj.find(".form-control option").each(function () {
                                                                        if ($(this).text() == data.controlValue[i].detailValue[0]) {
                                                                            $(this).attr('selected', 'selected');
                                                                        }
                                                                    });
                                                                }
                                                                else if (data.controlValue[i].controlType == 7 || data.controlValue[i].controlType == 8) {  //浏览框(除去文件浏览框)
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    obj.find("input[class='form-control']").val(data.controlValue[i].detailValue[0]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 12 || data.controlValue[i].controlType == 14) { //日期
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    obj.find(".item-content .form-control").val(data.controlValue[i].detailValue[0]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 13 || data.controlValue[i].controlType == 15) {  //日期区间
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    obj.find(".item-content .form-control:eq(0)").val(data.controlValue[i].detailValue[0]);
                                                                    obj.find(".item-content .form-control:eq(1)").val(data.controlValue[i].detailValue[1]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 9) {   //浏览框(文件浏览框)
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                                                                    $.each(data.controlValue[i].detailValue, function (x, v) {
                                                                        if (v != "") {
                                                                            var thisSaveName = data.controlValue[i].saveName[x];
                                                                            var $fileSpan = $('<span style="display:inline-block" class="file" term="' + thisSaveName + '" title=' + v + '><span class="text-overflow" style="display: inline-block;max-width: 200px;vertical-align: top;">' + v + '</span></span>'),
                                                                              $del = $("<a class='glyphicon glyphicon-remove clickable' style='visibility: hidden;margin-left: 5px'></a>");

                                                                            if (obj.find("input[type='file']").length <= 0) {
                                                                                obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                                                var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                                                upLoadValue.push(fileName);
                                                                            }
                                                                            else {
                                                                                var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                                                upLoadValue.push(fileName);
                                                                                $fileSpan.append($del).hover(function () {
                                                                                    $del.css('visibility', 'visible');
                                                                                }, function () {
                                                                                    $del.css('visibility', 'hidden');
                                                                                });
                                                                                obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                                                //删除
                                                                                $del.click(function () {
                                                                                    if (upLoadValue && upLoadValue.length > 0) {
                                                                                        $.each(upLoadValue, function (e, xxc_file) {
                                                                                            var fileName = xxc_file.split('*');
                                                                                            if (fileName && fileName.length > 2 && fileName[1] == thisSaveName) {
                                                                                                upLoadValue.splice(e, 1);
                                                                                            }
                                                                                        });
                                                                                    }
                                                                                    $(this).parent().parent().remove();
                                                                                });
                                                                            }
                                                                            //下载
                                                                            $fileSpan.find('span').click(function () {
                                                                                var displayName = $(this).text();
                                                                                $.post("/FlowIndex/Download", { displayName: displayName, saveName: thisSaveName, flag: 0 }, function (data) {
                                                                                    if (data == "success") {
                                                                                        //loadViewToMain("/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1");
                                                                                        window.location.href = "/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1";
                                                                                    }
                                                                                    else {
                                                                                        ncUnits.alert("文件不存在，无法下载!");
                                                                                    }
                                                                                    return;
                                                                                });
                                                                            });
                                                                        }
                                                                    })
                                                                }
                                                            }
                                                            else {     //明细列表中的控件
                                                                if (data.controlValue[i].controlType == 0) {   //标签
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    obj.find('.item-label').text(data.controlValue[i].description);
                                                                }
                                                                else if (data.controlValue[i].controlType == 1 || data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3 || data.controlValue[i].controlType == 11) { //输入框
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    obj.find('.item-content .form-control').val(data.controlValue[i].detailValue[0]);
                                                                    if (data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3) {
                                                                        obj.find('.item-content .form-control').blur();
                                                                    }
                                                                }
                                                                else if (data.controlValue[i].controlType == 4 || data.controlValue[i].controlType == 5) {  //单选框及复选框
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    for (var j = 0; j < data.controlValue[i].detailValue.length; j++) {
                                                                        if (data.controlValue[i].detailValue[j] == "false") {
                                                                            obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", false);
                                                                        }
                                                                        else {
                                                                            obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", true);
                                                                        }
                                                                    }
                                                                }
                                                                else if (data.controlValue[i].controlType == 6) {  //下拉列表
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    obj.find(".form-control option").each(function () {
                                                                        if ($(this).text() == data.controlValue[i].detailValue[0]) {
                                                                            $(this).attr('selected', 'selected');
                                                                        }
                                                                    });
                                                                }
                                                                else if (data.controlValue[i].controlType == 7 || data.controlValue[i].controlType == 8) {  //浏览框(非文件浏览框)
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    obj.find("input[class='form-control']").val(data.controlValue[i].detailValue[0]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 12 || data.controlValue[i].controlType == 14) { //日期
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    obj.find(".item-content .form-control").val(data.controlValue[i].detailValue[0]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 13 || data.controlValue[i].controlType == 15) {  //日期区间
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    obj.find(".item-content .form-control:eq(0)").val(data.controlValue[i].detailValue[0]);
                                                                    obj.find(".item-content .form-control:eq(1)").val(data.controlValue[i].detailValue[1]);
                                                                }
                                                                else if (data.controlValue[i].controlType == 9) {   //浏览框(文件浏览框)
                                                                    obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (data.controlValue[i].rowNumber - 1) + ")");
                                                                    $.each(data.controlValue[i].detailValue, function (x, v) {
                                                                        if (v != "") {
                                                                            var thisSaveName = data.controlValue[i].saveName[x];
                                                                            var $fileSpan = $('<span style="display:inline-block" class="file" term="' + thisSaveName + '" title=' + v + '><span class="text-overflow" style="display: inline-block;max-width: 200px;vertical-align: top;">' + v + '</span></span>'),
                                                                              $del = $("<a class='glyphicon glyphicon-remove clickable' style='visibility: hidden;margin-left: 5px'></a>");

                                                                            if (obj.find("input[type='file']").length <= 0) {
                                                                                obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                                                var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                                                upLoadValue.push(fileName);
                                                                            }
                                                                            else {
                                                                                var fileName = v + "*" + thisSaveName + "*9*" + data.controlValue[i].controlId;
                                                                                upLoadValue.push(fileName);
                                                                                $fileSpan.append($del).hover(function () {
                                                                                    $del.css('visibility', 'visible');
                                                                                }, function () {
                                                                                    $del.css('visibility', 'hidden');
                                                                                });
                                                                                obj.find('.item-content ul').append($("<li class='file-upload uploaded'></li>").append($fileSpan));
                                                                                //删除
                                                                                $del.click(function () {
                                                                                    if (upLoadValue && upLoadValue.length > 0) {
                                                                                        $.each(upLoadValue, function (e, xxc_file) {
                                                                                            var fileName = xxc_file.split('*');
                                                                                            if (fileName && fileName.length > 2 && fileName[1] == thisSaveName) {
                                                                                                upLoadValue.splice(e, 1);
                                                                                            }
                                                                                        });
                                                                                    }
                                                                                    $(this).parent().parent().remove();
                                                                                });
                                                                            }
                                                                            //下载
                                                                            $fileSpan.find('span').click(function () {
                                                                                var displayName = $(this).text();
                                                                                $.post("/FlowIndex/Download", { displayName: displayName, saveName: thisSaveName, flag: 0 }, function (data) {
                                                                                    if (data == "success") {
                                                                                        window.location.href = "/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1";
                                                                                        //loadViewToMain("/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1");
                                                                                    }
                                                                                    else {
                                                                                        ncUnits.alert("文件不存在，无法下载!");
                                                                                    }
                                                                                    return;
                                                                                });
                                                                            });
                                                                        }
                                                                    })
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                                var $flowdetail_cancel = $("<a id='flowDetail_modal_cancel' class='btn btn-transparency btn-lg user-defined' data-dismiss='modal'>取消</a>");
                                                var $flowdetail_save = $("<a class='btn btn-transparency btn-lg user-defined'>保存</a>");
                                                var $flowdetail_submit = $("<a class='btn btn-transparency btn-lg user-defined'>提交</a>");
                                                var $flowdetail_back = $("<a class='btn btn-transparency btn-lg user-defined'>退回</a>");
                                                var $flowdetail_agree = $("<a class='btn btn-transparency btn-lg user-defined'>同意</a>");
                                                var $flowdetail_submitMag = $("<a class='btn btn-transparency btn-lg user-defined'>提交</a>");
                                                if (admin != "1") {
                                                    if (operateStatus == 1) {  //待提交
                                                        $flowdetail_cancel.css("width", "33.2%");
                                                        $flowdetail_save.css("width", "33.4%");
                                                        $flowdetail_submit.css("width", "33.4%");
                                                        $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_save, $flowdetail_submit]);
                                                        $("#xxc_detailsuggest").hide();
                                                    }
                                                    else if (operateStatus == 6) {  //待审核
                                                        $flowdetail_cancel.css("width", "33.2%");
                                                        $flowdetail_back.css("width", "33.4%");
                                                        $flowdetail_agree.css("width", "33.4%");
                                                        $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_back, $flowdetail_agree]);
                                                        $("#xxc_detailsuggest").show();
                                                    }
                                                    else if (operateStatus == 10) {    //流程中的提交
                                                        $flowdetail_cancel.css("width", "33.2%");
                                                        $flowdetail_save.css("width", "33.4%");
                                                        $flowdetail_submitMag.css("width", "33.4%");
                                                        $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_save, $flowdetail_submitMag]);
                                                        $("#xxc_detailsuggest").show();
                                                    }
                                                    else {
                                                        $("#xxc_detailsuggest").hide();
                                                    }
                                                }

                                                //退回
                                                $flowdetail_back.click(function () {
                                                    var msg = $('#flow_message').val();
                                                    var reg = /<\w+>/;
                                                    if (msg.indexOf('null') >= 0 || msg.indexOf('NULL') >= 0 || msg.indexOf('&nbsp') >= 0 || reg.test(msg) || msg.indexOf('</') >= 0) {
                                                        ncUnits.alert("意见存在非法字符!");
                                                    }
                                                    $.ajax({
                                                        type: "post",
                                                        url: "/FlowIndex/TurnBack",
                                                        dataType: "json",
                                                        data: { nodeId: nodeId, templateId: templateId, formId: formId, suggest: msg, isEntruct: isEntruct },
                                                        success: rsHandler(function (data) {
                                                            $("#details-modal").modal("hide");
                                                            if (data) {
                                                                ncUnits.alert("退回操作成功!");
                                                                loadFlows();
                                                                drawPlanProgress();
                                                            } else {
                                                                ncUnits.alert("流程设置有误，请联系管理员!");
                                                            }
                                                        })
                                                    });
                                                });

                                                //同意
                                                $flowdetail_agree.click(function () {
                                                    var msg = $('#flow_message').val();
                                                    var reg = /<\w+>/;
                                                    if (msg.indexOf('null') >= 0 || msg.indexOf('NULL') >= 0 || msg.indexOf('&nbsp') >= 0 || reg.test(msg) || msg.indexOf('</') >= 0) {
                                                        ncUnits.alert("意见存在非法字符!");
                                                    }
                                                    $.ajax({
                                                        type: "post",
                                                        url: "/FlowIndex/AgreeFormFlow",
                                                        dataType: "json",
                                                        data: { templateId: templateId, formId: formId, nodeId: nodeId, isEtruct: isEntruct, suggest: msg },
                                                        success: rsHandler(function (data) {
                                                            $("#details-modal").modal("hide");
                                                            if (data) {
                                                                ncUnits.alert("流程审批通过!");
                                                                loadFlows();
                                                                drawPlanProgress();
                                                            }
                                                            else {
                                                                ncUnits.alert("流程设置有误，请联系管理员!");
                                                            }
                                                        })
                                                    });
                                                });

                                                //保存
                                                $flowdetail_save.click(function () {
                                                    var title = $('.detail_form_title').val();
                                                    var urg = $('#detail_urgencyStart .liHit').length;
                                                    var org = $('#detail_createdepart_span').attr('term');
                                                    var sta = $('#detail_createstation_span').attr('term');
                                                    var createtime = $('#flowdetail_createtime_v').val();
                                                   
                                                    var result = newVal.getJson();
                                                    if (result == false) {
                                                        ncUnits.alert("保存失败!");
                                                        return;
                                                    }
                                                    var formInfo = {
                                                        title: title,
                                                        urgency: urg,
                                                        organizationId: org,
                                                        stationId: sta,
                                                        createTime: createtime,
                                                        controlValue: result
                                                    };
                                                    var flag;
                                                    if (operateStatus==1) {
                                                        flag=1
                                                    } else {
                                                        flag = 2;
                                                    }
                                                    $.ajax({
                                                        type: "post",
                                                        url: "/FlowIndex/SaveControlValue",
                                                        dataType: "json",
                                                        data: { formId: formId, flag: flag, data: JSON.stringify(formInfo) },
                                                        success: rsHandler(function (data) {
                                                            if (flag) {
                                                                ncUnits.alert("保存成功!");
                                                                loadFlows();
                                                            } else {
                                                                ncUnits.alert("保存失败!");
                                                            }
                                                            $("#details-modal").modal("hide");
                                                        })
                                                    })
                                                });

                                                //提交
                                                $flowdetail_submit.click(function () {
                                                    var title = $('.detail_form_title').val();
                                                    var urg = $('#detail_urgencyStart .liHit').length;
                                                    var org = $('#detail_createdepart_span').attr('term');
                                                    var sta = $('#detail_createstation_span').attr('term');
                                                    var createtime = $('#flowdetail_createtime_v').val();

                                                    var result = newVal.getJson();
                                                    if (result == false) {
                                                        ncUnits.alert("提交失败!");
                                                        return;
                                                    }
                                                    var formInfo = {
                                                        title: title,
                                                        urgency: urg,
                                                        organizationId: org,
                                                        stationId: sta,
                                                        createTime: createtime,
                                                        controlValue: result
                                                    };
                                                    $.ajax({
                                                        type: "post",
                                                        url: "/FlowIndex/SubmitFlow",
                                                        dataType: "json",
                                                        data: { templateId: templateId, formId: formId, nodeId: nodeId,flag:1,data: JSON.stringify(formInfo) },
                                                        success: rsHandler(function (data) {
                                                            $("#details-modal").modal("hide");
                                                            if (data) {
                                                                ncUnits.alert("流程提交成功!");
                                                                loadFlows();
                                                                drawPlanProgress();
                                                            }
                                                            else {
                                                                ncUnits.alert("流程提交失败!");
                                                            }
                                                            
                                                            
                                                        })
                                                    });
                                                });

                                                //流程中的提交
                                                $flowdetail_submitMag.click(function () {
                                                    var result = newVal.getJson();
                                                    if (result == false) {
                                                        ncUnits.alert("提交失败!");
                                                        return;
                                                    }
                                                    var formInfo = {
                                                        controlValue: result
                                                    };
                                                    var msg = $('#flow_message').val();
                                                    var reg = /<\w+>/;
                                                    if (msg.indexOf('null') >= 0 || msg.indexOf('NULL') >= 0 || msg.indexOf('&nbsp') >= 0 || reg.test(msg) || msg.indexOf('</') >= 0) {
                                                        ncUnits.alert("意见存在非法字符!");
                                                    }
                                                    $.ajax({
                                                        type: "post",
                                                        url: "/FlowIndex/SubmitInFlow",
                                                        dataType: "json",
                                                        data: { templateId: templateId, formId: formId, nodeId: nodeId, suggest: msg, isEntruct: isEntruct, flag: 2, data: JSON.stringify(formInfo) },
                                                        success: rsHandler(function (data) {
                                                            $("#details-modal").modal("hide");
                                                            if (data) {
                                                                ncUnits.alert("提交成功!");
                                                                loadFlows();
                                                                drawPlanProgress();
                                                            }
                                                            else {
                                                                ncUnits.alert("流程设置有误，请联系管理员!");
                                                            }
                                                        })
                                                    })
                                                });

                                            })
                                        });
                                    })
                                });
                            }
                            


                        });
                    });

                    //转发
                    $transpont.click(function () {
                        var templateId = $(this).parents('.cell').find('.templateName').attr('term');
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        var nodeId = $(this).parents('.cell').find('.node').attr('term');

                        /*人力资源 开始*/
                        var personOrgId;
                        var personWithSub = false;
                        $("#transpont_modal .modal-content").load("/FlowIndex/LoadTranspont", function () {
                            $("#transpont_modal").modal("show");
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
                                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</spam></li>"),
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
                                var copyUsers = [];
                                $("#HR_modal_chosen li").each(function () {
                                    copyUsers.push($(this).attr('term'));
                                });
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/DuplicateForm",
                                    dataType: "json",
                                    data: { formId: formId, nodeId: nodeId, templateId: templateId, data: JSON.stringify(copyUsers) },
                                    success: rsHandler(function (data) {
                                        if (data) {
                                            ncUnits.alert("转发成功!");
                                            $("#HR_select").val('');
                                            $('#transpont_modal').modal('hide');
                                        } else {
                                            ncUnits.alert("已选人员包含当未操作人，请重新选择!");
                                        }

                                    })
                                });
                            });

                            //取消
                            $("#HR_cancel").click(function () {
                                $("#HR_select").val('');
                                $("#HR-haschildren").prop("checked", false);
                                $("#HR_modal_chosen_count").text(0);
                                $(".person_list input[type=checkbox]").prop("checked", false);
                                $("#HR_modal_chosen li").remove();
                            });
                            /*人力资源 结束*/
                        });
                    });

                    //意见
                    $suggestion.click(function () {
                        var formId = $(this).parents('.cell').find('.title').attr('term');
                        var nodeId = $(this).parents('.cell').find('.node').attr('term');

                        $("#suggestion_modal .modal-content").load("/FlowIndex/LoadSuggestion", function () {
                            $("a#suggestion_modal_submit").click(function () {
                                var suggestion = $.trim($("#idea_input").val());
                                if (suggestion == "") {
                                    ncUnits.alert("意见不能为空!");
                                    return;
                                }
                                else if (suggestion.length > 250) {
                                    ncUnits.alert("意见不能超过250字!");
                                    return;
                                }
                                var reg = /<\w+>/;
                                if (suggestion.indexOf('null') >= 0 || suggestion.indexOf('NULL') >= 0 || suggestion.indexOf('&nbsp') >= 0 || reg.test(suggestion) || suggestion.indexOf('</') >= 0) {
                                    ncUnits.alert("意见内容存在非法字符!");
                                    return;
                                }
                                $.ajax({
                                    type: "post",
                                    url: "/FlowIndex/AddContents",
                                    dataType: "json",
                                    data: {
                                        formId: formId,//表单ID
                                        nodeId: nodeId, //节点ID
                                        contents: suggestion//文件夹名称
                                    },
                                    success: rsHandler(function (data) {
                                        ncUnits.alert("意见发表成功!");
                                        $("#suggestion_modal").modal("hide");
                                        $("#idea_input").val("");
                                        loadFlows();
                                        drawPlanProgress();
                                    })
                                });
                            });
                        });
                    });

                });
                $addNewFile = $("<div class='col-xs-6'><div class='cell' style='height:143px'><div class='nest add'><div class='addPicture' data-toggle='modal' data-target='#addflow_modal' style='cursor:pointer'></div><div class='wordPosition'>新建流程</div></div></div></div>");
                $flowChunk.append($addNewFile);
                //$(".newadd .addPicture").click(function () {
                //    addNewFile();
                //});
                /* 评分五角星 开始 */
                //$(".nest .rating").rating({
                //    number: data.urgency, color: 1, clicked: function (i) {
                //        console.log(i)
                //    }
                //})
                /* 评分五角星 结束 */
                //选择卡片列表移上去出现绿条 开始
                $(".cell").hover(function () {
                    $(".operate", this).toggle();
                });
            })
        });
    };

    $('#details-modal').on("hidden.bs.modal", function () {
        layer.closeTips();
    });



    /*新建流程 开始*/
    var addflow_templateId;  //预览保存的模板ID
    var selectedtemplateId = null; //选择模板后保存的模板ID
    var saveData = null;      //保存控件信息
    var addflow_content = {
        templateId: null,
        organizationId: null,
        stationId: null,
        title: null,
        urgency: null,
        createTime: null,
        status: null,
        controlValue: []
    };
    $("#addflow_modal").on("shown.bs.modal", function () {
        var addflow_lodi = getLoadingPosition('.form-list');
        //绑定流程分类
        $.ajax({
            type: "post",
            url: "/FlowIndex/GetTemplateCategoryList",
            dataType: "json",
            complete: rcHandler(function () {             
                addflow_lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                if (data && data.length > 0) {
                    $(".category_content").empty();
                    var $formList = $('.form-list');
                    $formList.empty();
                    var showflag = true;
                    $.each(data, function (e, i) {
                        var $content = $(" <li><a  term=" + i.categoryId + " class='xxc_categoryName' style='overflow: hidden;text-overflow: ellipsis;white-space: nowrap' title='" + i.categoryName + "'>" + i.categoryName + "</a></li><li class='divider short'></li>");
                        $(".category_content").append($content);

                        /*加载表单 开始*/
                        $.ajax({
                            type: "post",
                            url: "/FlowIndex/GetTemplateList",
                            dataType: "json",
                            data: { categoryId: i.categoryId },
                            success: rsHandler(function (data) {
                                if (data && data.length > 0) {

                                    $.each(data, function (e, j) {
                                        var $categoryinfo = $('<div class="categoryinfo-div"></div>');
                                        var $categoryname = $('<div id="category' + j.categoryId + '" class="category-name col-xs-12" title="' + j.categoryName + '"><span style="overflow: hidden;text-overflow: ellipsis;' +
                                        'white-space: nowrap;">' + j.categoryName + '</span><div class="category-line"></div></div>');
                                        var $templatelistDiv = $('<div class="template-category"></div>');
                                        $formList.append($categoryinfo.append([$categoryname, $templatelistDiv]));
                                        if (j.templateList && j.templateList.length > 0) {
                                            var count = 1;
                                            $.each(j.templateList, function (s, k) {
                                                var $templateinfo = $('<div class="col-xs-4"><div class="cell addflow_cell" term=' + k.templateId + '><div class="pull-left img-circle templateId">'
                                                + '<span class="content-center">' + count + '</span></div><div class="info-list"><div class="title" style="overflow: hidden;text-overflow: ellipsis;white-space: nowrap" title="' + k.templateName + '">' + k.templateName + '</div>'
                                                + '<div class="des" title=' + k.description + ' style="word-wrap: break-word;overflow: hidden;text-overflow: ellipsis;white-space: nowrap" title="' + k.description + '">' + k.description + '</div></div>'
                                                + '<div class="form_preview"><span data-toggle="modal" data-target="#preview_modal" >预览</span></div><img src="../../Images/flow/addflow_select_hit.png" class="addflow_select" term="0"  >'
                                                + '</div></div>');
                                                $templatelistDiv.append($templateinfo);
                                                //加载事件

                                                //表单上移变色
                                                $(".addflow_cell").hover(function () {
                                                    if (!$(this).hasClass('nohover')) {
                                                        $(this).css("border", "1px solid #fbaa12");
                                                        $('.addflow_cell').each(function () {
                                                            if (!$(this).hasClass('nohover')) {
                                                                $(this).removeClass("gray-background").find(".addflow_select").hide();
                                                            }
                                                        })
                                                        //$('.addflow_cell').removeClass("gray-background").find(".addflow_select").hide();
                                                        $(this).find(".addflow_select").show();
                                                        $(this).addClass("gray-background");
                                                    }
                                                }, function () {
                                                    if (!$(this).hasClass('nohover')) {
                                                        $(this).css("border", "1px solid #ccc");
                                                        $('.addflow_cell').each(function () {
                                                            if (!$(this).hasClass('nohover')) {
                                                                $(this).find(".addflow_select").hide();
                                                                $(this).removeClass("gray-background");
                                                            }
                                                        })
                                                        
                                                    }
                                                });
                                                //点击勾选
                                                $(".addflow_select").off('click');
                                                $(".addflow_select").click(function () {
                                                    var term = $(this).attr("term");
                                                    $('.nohover').css("border", "1px solid #ccc").removeClass('gray-background').find('.addflow_select').attr('term', '0');
                                                    $('.nohover .addflow_select').attr("src", "../../Images/flow/addflow_select_hit.png").hide();
                                                    $('.addflow_cell').removeClass('nohover');
                                                    if (term == 0) {
                                                        $(this).parents(".addflow_cell").addClass('nohover');
                                                        addflow_templateId = $(this).parents('.addflow_cell').attr('term');
                                                        $(this).attr("src", "../../Images/flow/addflow_select.png").attr("term", "1");
                                                        $(this).parents('.addflow_cell').css("border", "1px solid #fbaa12");
                                                        //$(this).parents(".cell").addClass("gray-background").sibling().removeClass("gray-background").find(".addflow_select").hide();
                                                    }
                                                    else {
                                                        addflow_templateId = null;
                                                        $(this).attr("src", "../../Images/flow/addflow_select_hit.png").attr("term", "0");
                                                    }
                                                });
                                                //预览
                                                $('.form_preview span').off('click');
                                                $('.form_preview span').click(function () {
                                                    addflow_templateId = $(this).parents(".addflow_cell").attr('term');
                                                });
                                                count++;
                                            });
                                        }
                                    });
                                    var category_text = $('#xxc_selectedcate').attr('title');
                                    $(".xxc_categoryName").each(function () {
                                        if ($(this).attr('title') == category_text) {
                                            $(this).click();
                                        }
                                    });
                                }
                            })
                        });
                        /*加载表单 结束*/

                        $(".category_content .xxc_categoryName").off('click');
                        $(".category_content .xxc_categoryName").click(function () {
                            var cateId = $(this).attr('term');
                            $("#category_select").text($(this).text()).attr('term', cateId).attr('title', $(this).text());
                            var obj = $("#category" + cateId);
                            document.getElementById("category" + cateId).scrollIntoView();
                            //obj.scrollIntoView();
                        });

                    });
                    $("#category_select").text($('.category_content .xxc_categoryName:eq(0)').text()).attr('term', $('.category_content .xxc_categoryName:eq(0)').attr('term'));
                    $(".category_content li:last").remove();
                }
            })
        });



        //确定按钮点击事件
        $('#addflow_modal_submit').click(function () {
            if (addflow_templateId == null) {
                ncUnits.alert('请选择表单');
                return;
            }
            else {
                $(this).attr('data-toggle', 'modal');
                $(this).attr('data-target', '#preview_modal');
                $('.addflow_footer').show();
                $('#addflow-mask').hide();
            }
        });
    }).on("hidden.bs.modal", function () {
        $('.form-list').empty();
    });
    //显示预览画面
    $("#preview_modal").on("shown.bs.modal", function () {
        /* 评分五角星 开始 */
        $('.stars ul li').hover(function () {
            var nums = $(this).index();
            var length = $(this).parent().children('li').length - 1;
            for (var i = 0; i <= length; i++) {
                if (i <= nums) {
                    $(this).parent().children('li').eq(i).addClass('liHover');
                }
                else {
                    $(this).parent().children('li').eq(i).addClass('liHoverNot');
                }
            }
        }, function () {
            $(this).parent().children('li').removeClass('liHover').removeClass('liHoverNot');
        });
        $('.stars ul li').click(function () {
            var nums = $(this).index();
            var length = $(this).parent().children('li').length - 1;
            for (var i = 0; i <= length; i++) {
                if (i <= nums) {
                    $(this).parent().children('li').eq(i).addClass('liHit');
                }
                else {
                    $(this).parent().children('li').eq(i).removeClass('liHit');
                }
            }
        });
        /* 评分五角星 结束 */

        //部门岗位的鼠标进入离开事件
        //$("#department_select").parents('.addflow-text').mouseleave(function () {
        //    $(this).find('.department_ul').hide();
        //});
        //$("#department_select").parents('.addflow-text').mouseenter(function () {
        //    $(this).find('.department_ul').show();
        //});
        //$("#station_select").parents('.addflow-text').mouseleave(function () {
        //    $(this).find('.station_ul').hide();
        //});
        //$("#station_select").parents('.addflow-text').mouseenter(function () {
        //    $(this).find('.station_ul').show();
        //});


        //选择完成时间
        $("#addflow_createtime").click(function () {
            laydate({
                elem: '#addflow_createtime_v',
                event: 'click',
                istime: true,
                format: 'YYYY-MM-DD hh:mm',
                isclear: true,
                istoday: true,
                issure: true,
                festival: true,
                start: new Date().toLocaleDateString() + ' 08:30:00',
                choose: function (dates) {
                    endTime_v = dates;
                    if (addflow_defaultTitle==1) {
                        addflow_createTime = dates.replace('-', '/').replace('-', '/');
                        $("#preview_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                    }
                },
                clear: function () {
                    endTime_v = undefined;
                }
            });
        });

        $.ajax({
            type: "post",
            url: "/FlowIndex/GetUserOrganizationList",
            dataType: "json",
            success: rsHandler(function (data) {
                if (data) {
                    var orgId = data.orgList[0].id;
                    $("#yulan_createUser").val(data.userName);
                    addflow_createUserName = data.userName;
                    addflow_createTime = data.createTime.replace('T', ' ').substr(0, 16);
                    $("#addflow_createtime_v").val(addflow_createTime);
                    addflow_createTime = addflow_createTime.replace('-', '/').replace('-','/');
                    yulan_createTime = $("#addflow_createtime_v").val();
                    $(".department_ul").empty();
                    if (data.orgList && data.orgList.length > 0) {
                        $.each(data.orgList, function (e, i) {
                            var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_departmentName'>" + i.name + "</a></li><li class='divider short'></li>");
                            $(".department_ul").append($content);
                            $(".department_ul .xxc_departmentName").off('click');
                            $(".department_ul .xxc_departmentName").click(function () {
                                orgId = $(this).attr('term');
                                $("#department_select").text($(this).text()).attr('term', orgId);
                                if (addflow_defaultTitle==1) {
                                    addflow_createdepart = $("#department_select").text();
                                    $("#preview_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);
                                }
                                
                                //加载岗位
                                load_station(orgId);
                            });
                        });
                    }
                    $("#department_select").text($('.department_ul .xxc_departmentName:eq(0)').text()).attr('term', $('.department_ul .xxc_departmentName:eq(0)').attr('term'));
                    addflow_createdepart = $("#department_select").text();
                    $(".department_ul li:last").remove();
                    //加载岗位
                    load_station(orgId);
                    //加载表单
                    loadDataByTemplateId(addflow_templateId);
                }
            })
        });

        //加载岗位
        function load_station(orgId) {
            $.ajax({
                type: "post",
                url: "/FlowIndex/GetUserStationList",
                dataType: "json",
                data: { orgId: orgId },
                success: rsHandler(function (data) {
                    if (data && data.length > 0) {
                        $(".station_ul").empty();
                        $.each(data, function (e, i) {
                            var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_stationName'>" + i.name + "</a></li><li class='divider short'></li>");
                            $(".station_ul").append($content);
                            $(".station_ul .xxc_stationName").off('click');
                            $(".station_ul .xxc_stationName").click(function () {
                                var orgId = $(this).attr('term');
                                $("#station_select").text($(this).text()).attr('term', orgId);
                            });
                        });
                        $("#station_select").text($('.station_ul .xxc_stationName:eq(0)').text()).attr('term', $('.station_ul .xxc_stationName:eq(0)').attr('term'));
                        $(".station_ul li:last").remove();
                    }
                })
            });
        }



        //取消按钮点击事件
        $('#addflow_cancel').off('click')
        $('#addflow_cancel').click(function () {
            clearFlowContent();
            $('#preview_modal').modal('hide');
        });

        //添加新建流程信息
        $('.addflow_sure').off('click')
        $('.addflow_sure').click(function () {
            var flag = $(this).attr('term');
            addflow_content.templateId = addflow_templateId;
            addflow_content.title = $('.form-title').val();
            if (addflow_content.title=="") {
                validate_reject("标题不能为空", $('.form-title'));
                return;
            }
            else if (!justifyByLetter(addflow_content.title, "标题", $('.form-title'))) {
                return;
            }

            addflow_content.status = 1;
            addflow_content.urgency = $('#preview_modal .liHit').length;
            addflow_content.organizationId = $('#department_select').attr('term');
            addflow_content.stationId = $('#station_select').attr('term');
            addflow_content.createTime = $('#addflow_createtime_v').val();
            var result = saveData.getJson();
            if (result == false) return;
            addflow_content.controlValue = result;
            $.ajax({
                type: "post",
                url: "/FlowIndex/AddFlow",
                data: {
                    flag: flag,
                    data: JSON.stringify(addflow_content)
                },
                dataType: "json",
                success: rsHandler(function (data) {
                    //清空新建流程内容
                    clearFlowContent();
                    $('#preview_modal').modal('hide');
                    $("#addflow_modal").modal('hide');
                    if (data) {
                        ncUnits.alert("新建成功!");
                        loadFlows();
                        drawPlanProgress();
                    }
                    else {
                        ncUnits.alert("流程设置有误，请联系管理员!");
                    }


                })
            });
        });
    }).on("hide.bs.modal", function () {
        $("#Label-content").empty();
        //addflow_templateId=null;
        $('#addflow_modal_submit').removeAttr('data-toggle');
        $('#addflow_modal_submit').removeAttr('data-target');
        $('.stars li').removeClass('liHit');
        $('#preview_title').val('');
        $('#addflow_createtime_v').val('');
        $('.addflow_footer').hide();
        $('#addflow-mask').show();
        layer.closeTips();
    });


    //判断某变量是否具有非法字符
    function justifyByLetter(txt, name,obj) {
        var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            validate_reject(name, obj);
            return false;
        }
        return true;
    }

    // 按ID加载模板
    function loadDataByTemplateId(templateId) {
        if (templateId) {
            $.ajax({
                url: "/FlowIndex/GetTemplateInfoById",
                type: "post",
                dataType: "json",
                data: {
                    templateId: templateId,
                    nodeId: null,
                    formId: null,
                    flag: 1
                },
                success: rsHandler(function (data) {
                    addflow_templateName = data.template.templateName;
                    addflow_defaultTitle = data.template.defaultTitle;
                    if (addflow_defaultTitle == 1) {
                        $("#preview_title").attr("readonly", "true");
                        $("#preview_title").val(addflow_templateName + '-' + addflow_createdepart + '-' + addflow_createUserName + '-' + addflow_createTime);

                    } else {
                        $("#preview_title").removeAttr("readonly");

                    }
                    $('#preview_modal_title').text(data.template.templateName);
                    $('.template-name').text(data.template.templateName);
                    $('.template-desc').text(data.template.description);
                    saveData = $("#Label-content").widgetFormResolver(data.controlInfo);
                })
            })
        }
    }

    //清空新建完成后预览画面的内容
    function clearFlowContent() {
        $('.form-title').val('');
        $('.liHit').removeClass('liHit');
        $('#department_select').text('').removeAttr('term');
        $('.department_ul li').remove();
        $('#addflow_createtime_v').val('');
        addflow_templateName = "";
        addflow_createdepart = "";
        addflow_createTime = "";
        addflow_createUserName = "";
    }

    /*新建流程 结束*/

    function renderFlowChart(formId) {
        $.ajax({
            url: "/FlowChart/DisplayFormFlowChart",
            type: "post",
            dataType: "json",
            data: {
                formId: formId
            },
            success: rsHandler(function (data) {
                $("#modal_detail_flowchart").flowLine(data);
            })
        })
    }
})
