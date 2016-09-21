$(function() {

    /*初始化 开始*/
    //文档列表请求所用参数
    var argus = {
        status:[],
        time:[],
        person:[]
    };
    /*初始化 结束*/

   
    /*加载文档列表 开始*/
    loadEntrustList();
    /*加载文档列表 结束*/

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
                admin : 0
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
                lodi.remove();
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

    /*筛选 开始*/
    $("#go_filter").click(function () {
        $("#filterbox").slideToggle();
        if ($(this).hasClass("active")) { $(this).removeClass("active"); }
        else $(this).addClass("active");
    });
    $("#filterbox_shrink").click(function () {
        $("#filterbox").slideUp();
        $("#go_filter").removeClass("active");
    });

    /*状态选择 开始*/
    $("#filterbox_processing").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addFilter({
                text: "进行中",
                add: function () {
                    $this.addClass("active");
                    argus.status.push(0);
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.status = _.without(argus.status, 0);
                }
            });
        }
    });

    $("#filterbox_complete").click(function () {
        var $this = $(this);
        if (!$this.hasClass("active")) {
            addFilter({
                text: "已完结",
                add: function () {
                    $this.addClass("active");
                    argus.status.push(1);
                },
                delete: function () {
                    $this.removeClass("active");
                    argus.status = _.without(argus.status, 1);
                }
            });
        }
    });
    /*状态选择 结束*/

    /*人员选择 开始*/
    $.ajax({
        type: "post",
        url: "/BuildNewPlan/GetIpUserByUserId",
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
        url: "/BuildNewPlan/GetIpUserByUserId",
        hasImage: true,
        defText: "常用联系人",
        selectHandle: function (data) {
            if (_.indexOf(argus.person, data.userId) == -1) {
                addFilter({
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
    /*人员结束 开始*/

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

    setTime("filterbox_starttime", "filterbox_endtime");
    setTime("input_start_time", "input_end_time");
    /*时间选择 结束*/

    $("#filterbox_clear").click(function () {
        $("#filterbox .selecteds .glyphicon-remove").trigger("click", true);
        loadEntrustList();
    });
    /*筛选 结束*/

    var entrustId;
    /*加载委托列表 开始*/
    function loadEntrustList() {
        var list_lodi = getLoadingPosition('.entrust');
        $.ajax({
            type: "post",
            url: "/FlowEntrust/GetFlowEntrustList",
            dataType: "json",
            data: {
                data: JSON.stringify(argus)
            },
            complete: rcHandler(function () {
                list_lodi.remove();
            }),
            success: rsHandler(function (data) {
                if (data == "error") {
                    window.location.href = "/";
                }

                var $entrustList = $("#entrust_list");
                var $addNewEntrust = $("");

                $entrustList.empty();
                var date, time;
                $.each(data, function (i, v) {
                    var $col = $("<div class='col-xs-4 card'></div>");
                    var $cell = $("<div class='cell'></div>");
                    var $content = $("<div class='content' style=' height: 100px'></div>");

                    var $mandataryUser = $("<div><span class='content-label'>被委托人：</span><span class='content-info'>" + v.mandataryUserName + "</span></div>");
                    var $number = $("<div style='margin-top: 5px;'><span class='content-label'>数量：</span><span class='content-info'>" + v.number + "</span></div>");
                    var $validTime = $("<div style='margin-top: 5px;'><span class='content-label'>有效时间：</span><span class='content-info'>" + v.startTime.toString().substring(0, v.startTime.toString().indexOf('T')) + "</span>&nbsp;&nbsp;&nbsp;&nbsp;- &nbsp;&nbsp;" +
                    "<span class='content-info'>" + v.endTime.toString().substring(0, v.endTime.toString().indexOf('T')) + "</span></div>");
                    $content.append($mandataryUser, $number, $validTime);                    
                    
                    var $footer = $("<div class='horizontal' style=' height: 20px'><span class='content-label'> " + v.entrustuserName + "&nbsp;&nbsp;&nbsp;" + v.createTime.toString().substring(0, v.createTime.toString().indexOf('T')) + "&nbsp;" + v.createTime.toString().substring(v.createTime.toString().indexOf('T') + 1, v.createTime.toString().lastIndexOf(':')) + "</span></div>");

                    var $operate = $("<div class='operate'></div>");
                    var $btns = $("<div class='btns'><ul class='list-inline'></ul></div>");
                    var $detail = $("<a href='#' data-toggle='modal' data-target='#detail_modal' id='operate_detail' class='operate_detail'>详情</a>"),
                        $callback = $("<a href='#' class='callback'>收回</a>");

                    if (v.isComplate == true) {
                        $btns.append($detail).appendTo($operate);
                    } else {
                        $btns.append([$detail, $callback]).appendTo($operate);
                    }

                    $cell.append([$("<div class='grayTop'></div>"), $content, $operate, $footer]);

                    $col.append($cell).appendTo($entrustList);

                    $cell.hover(function () {
                        $operate.toggle();
                    });

                    $detail.click(function () {
                        entrustId = v.entrustId;
                        $.ajax({
                            type: "post",
                            url: "/FlowEntrust/GetFlowEbyId",
                            dataType: "json",
                            data: { id: entrustId },
                            success: rsHandler(function (data) {
                                // 显示委托详情
                                $("#mandatary_user").html(data.mandataryUserName);
                                $("#entrust_user").html(data.entrustuserName);
                                $("#entrust_number").html(data.number);
                                $("#start_time").html(data.startTime.toString().substring(0, data.startTime.toString().indexOf('T')));
                                $("#end_time").html(data.endTime.toString().substring(0, data.endTime.toString().indexOf('T')));

                                $("#detail_modal_list tr").remove();
                                // 显示委托结果
                                var $resultHtml;
                                $.each(data.entrusList, function (i, result) {
                                    $resultHtml = $(" <tr><td><span>《" + result.templateName + "》</span></td></tr>");
                                    $("#detail_modal_list").append($resultHtml);
                                });
                            })
                        });

                        //确定
                        $('#detail_modal_submit').click(function () {
                            $("#detail_modal").modal("hide");
                        });
                    });

                    $callback.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要收回？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                entrustId = v.entrustId;
                                $.ajax({
                                    type: "post",
                                    url: "/FlowEntrust/UpdateFlowE",
                                    dataType: "json",
                                    data: { id: entrustId },
                                    success: rsHandler(function (data) {
                                        if (data == true) {
                                            loadEntrustList();
                                            ncUnits.alert("收回成功!");
                                        }
                                    })
                                });
                            }
                        });
                    });
                });

                $addNewEntrust = $("<div class='newadd col-xs-4'><div class='add'><div class='addPicture' data-toggle='modal' data-target='#new_modal' style='cursor:pointer'></div><div class='wordPosition'>新建委托</div></div></div>");
                $entrustList.append($addNewEntrust);
                $(".newadd .addPicture").click(function () {
                    addNewEntrust();
                });
            })
        });
    }
    /*加载委托列表 结束*/

    /*新建委托 开始*/
    $("#add_entrust").click(function () {
        addNewEntrust();
    });

    var templates = [];
    var entrustUser = $("#input_userId").val();
    var mandataryUser;
    var categoryId;

    function addNewEntrust() {
        // 画面初期化
        templates = [];
        $("#input_mandatary").val("");
        $("#input_entrust").val($("#input_userName").val());
        $("#flow_count").html(0);
        $("#input_start_time").val("");
        $("#input_end_time").val("");
        $("#add_modal_list tr").remove();

        var add_lodi = getLoadingPosition("#add_modal_list");

        // 加载模板分类树形
        $.ajax({
            type: "post",
            url: "/FlowEntrust/GetCaregoryList",
            dataType: "json",
            complete: rcHandler(function () {
                add_lodi.remove();
            }),
            success: rsHandler(function (data) {
                var categoryTree = $.fn.zTree.init($("#flow_list"), $.extend({
                    callback: {
                        onCheck: function (e, id, node) {
                            // 勾选模板分类（第一层节点）
                            if (node.isParent == true) {
                                if (node.isParent == true) {
                                    categoryTree.expandNode(node, true, true, true);
                                }
                            }else {
                                if (node.checked) {
                                    var $checked = $(" <tr id='" + node.id +"'><td><span>《" + node.name + "》</span></td></tr>");
                                    $("#add_modal_list").append($checked);
                                    templates.push(node.id);
                                } else {
                                    templates = _.without(templates, node.id);
                                    $("#"+ node.id).remove();
                                }
                                $("#flow_count").html(templates.length);
                            }
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false
                    },
                    check: {
                        autoCheckTrigger: true,
                        enable: true,
                        chkStyle: "checkbox",
                        chkboxType: {"Y": "ps", "N": "ps"}
                    },
                    async: {
                        enable: true,
                        url: "/FlowEntrust/GetTemListById",
                        autoParam: ["id=parent"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            if(parentNode.checked){
                                $.each(responseData.data, function(i, n){
                                    categoryTree.checkNode(n, true, true, true);
                                })
                            }
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });

        $("#input_mandatary").searchPopup({
            url: "/BuildNewPlan/GetIpUserByUserId",
            hasImage: true,
            defText: "常用联系人",
            selectHandle: function (data) {
                $("#input_mandatary").val(data.name);
                mandataryUser = data.id;
            }
        });

        $("#input_entrust").searchPopup({
            url: "/BuildNewPlan/GetIpUserByUserId",
            hasImage: true,
            defText: "常用联系人",
            selectHandle: function (data) {
                $("#input_entrust").val(data.name);
                entrustUser = data.id;
            }
        });

        // 确定按钮按下
        $("#new_modal_submit").off("click");
        $("#new_modal_submit").click(function () {
            var add_model = {
                entrustUser: null,
                mandataryUser: null,
                number: null,
                startTime: null,
                endTime: null,
                templateId: []
            };

            if(mandataryUser == null){
                ncUnits.alert("被委托人不能为空!");
                return;
            }

            if(entrustUser == null){
                ncUnits.alert("委托人不能为空!");
                return;
            }

            if (mandataryUser == entrustUser)
            {
                ncUnits.alert("委托人和被委托人不能为同一人!");
                return;
            }

            var startTime = $.trim($("#input_start_time").val());
            if(startTime == ""){
                ncUnits.alert("开始时间不能为空!");
                return;
            }

            var endTime = $.trim($("#input_end_time").val());
            if(endTime == ""){
                ncUnits.alert("结束时间不能为空!");
                return;
            }

            if(startTime > endTime){
                ncUnits.alert("开始时间不能大于结束时间!");
                return;
            }

            if ($("#add_modal_list tr").length == 0) {
                ncUnits.alert("请选择要委托的流程!");
                return;
            }

            add_model.entrustUser = entrustUser;
            add_model.mandataryUser = mandataryUser;
            add_model.startTime = $.trim($("#input_start_time").val()) + " 00:00:00";
            add_model.endTime = $.trim($("#input_end_time").val()) + " 23:59:59";
            add_model.number = $("#flow_count").html();
            add_model.templateId = templates;
            console.log(add_model);
            var isClick = true;
            if (isClick == true) {
                isClick = false;
                $.ajax({
                    type: "post",
                    url: "/FlowEntrust/AddFlowE",
                    dataType: "json",
                    data: { data: JSON.stringify(add_model) },
                    success: rsHandler(function (data) {
                        if (data == true) {
                            ncUnits.alert("新建流程委托成功!");
                            $("#new_modal").modal("hide");
                            loadEntrustList();
                            isClick = true
                        } else {
                            ncUnits.alert("新建流程委托失败!");
                            isClick = true
                        }

                    })
                });
            }
        });
    }

    /*新建委托 结束*/

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
                    loadEntrustList();
                }
            });

            obj.add();
            loadEntrustList();
        }
    })();

    function setTime(startId, endId){
        var start = {
                elem: '#' + startId,
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
                elem: '#' + endId,
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
    }
});