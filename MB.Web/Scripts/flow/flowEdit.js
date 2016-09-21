//@ sourceURL=flowEdit.js

/**
 * Created by ZETA on 2015/7/6.
 */
$(function () {

    if ("undefined" == typeof systemOrFalse) {
        console.log("systemOrFalse未定义" );
    }
    //else {
  //      console.log("aaaaaaaaaaaaaaaaaaaaa systemOrFalse=" + systemOrFalse);
  //  }
    
    var systemFalse = systemOrFalse;
    systemOrFalse = undefined;
   
    
    /* 模板编辑 开始 */
    var $flowCon = $("#flowContainer"),
        $sliderCon = $("#sidebarContainer")
    //$("#nav_template_edit").click(function () {
    //    $flowCon.addClass("col-xs-12").removeClass("col-xs-9");
    //    $sliderCon.hide();
    //});
    //$("#nav_flow_edit").click(function () {
    //    $flowCon.removeClass("col-xs-12").addClass("col-xs-9");
    //    $sliderCon.show();
    //    $("#nodeSetTab").trigger("click");//触发点击事件
    //});
    /*圆饼 加载开始*/

    var date = new Date()
      , year = date.getFullYear()
      , month = date.getMonth() + 1
      , $con = $(".panel-chart")
      , colors = com.ztnc.targetnavigation.unit.planStatusColor;

    //右侧个人信息

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
                admin: 0
            },
            complete: rcHandler(function () {
                lodi.remove();
            }),
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
            })
        });
    }


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

    /*圆饼加载结束*/

    $("#nav_template_edit").click(function (e) {
        e.preventDefault();
        //切换判断
        var flag = false;
        if (flowTab == 1) {        //如果当前页面是节点设置画面
            flag = nodeModefyOrNot();
        } else if (flowTab == 2) {         //流程设置画面
            flag = modifyOrFalse();
        }
        if (flag == true) {
            ncUnits.confirm({
                title: '提示',
                html: '你的设置还没保存,确定要退出当前页面?',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    flowTab = 4;
                    $("#nav_template_edit").tab('show');
                    $("#flow_edit .nav-item").removeClass("green_color");
                    $flowCon.addClass("col-xs-12").removeClass("col-xs-9");
                    $sliderCon.hide();
                }
            });
        } else {
            flowTab = 4;
            $("#nav_template_edit").tab('show');
            $("#flow_edit .nav-item").removeClass("green_color");
            $flowCon.addClass("col-xs-12").removeClass("col-xs-9");
            $sliderCon.hide();
        }
    });


    $("#nav_flow_edit").click(function (e) {
        e.preventDefault();
        //切换判断
        var flag = false;
        if (flowTab == 1) {        //如果当前页面是节点设置画面
            flag = nodeModefyOrNot();
        } else if (flowTab == 2) {         //流程设置画面
            flag = modifyOrFalse();
        }
        if (flag == true) {
            ncUnits.confirm({
                title: '提示',
                html: '你的设置还没保存,确定要退出当前页面?',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    flowTab = 4;
                    $("#nav_flow_edit").tab('show');
                    $flowCon.removeClass("col-xs-12").addClass("col-xs-9");
                    $sliderCon.show();
                    loadPersonalInfo();
                    drawPlanProgress();
                    $("#nodeSetTab").trigger("click");//触发点击事件
                   
                }
            });
        } else {
            flowTab = 4;
            $("#nav_flow_edit").tab('show');
            $flowCon.removeClass("col-xs-12").addClass("col-xs-9");
            $sliderCon.show();
            loadPersonalInfo();
            drawPlanProgress();
            $("#nodeSetTab").trigger("click");//触发点击事件
          
            
        }
    });

    var $widget_list = $("#widget_list"),
        $widget_container = $("#widget_container"),
        $widget_edit = $("#widget_edit");

    $(".rating").rating({
        color: 1
    });

    var form = $widget_container.widgetFormEditor({
        dragItems: $(".widget-item", $widget_list),
        editor: $widget_edit
    });

    /* 表头设置 开始 */
    var $name = $("#form_name"),
        $desc = $("#form_description"),
        $title = $("#form_title"),

        $editName = $("#edit_form_name"),
        $editDesc = $("#edit_form_description"),
        $editTitleType = $("input[name='formTitle']");
    //$editTitle = $("#edit_form_title");

    $editName.on("input", function () {
        $name.text($(this).val());
    });
    $editDesc.on("input", function () {
        $desc.text($(this).val());
    });
    $editTitleType.change(function () {
        var v = $(this).val();
        switch (v) {
            case "1":
                //$title.val("");
                $title.attr("disabled", true);
                //$editTitle.hide();
                break;
            case "0":
                //$title.val($editTitle.val());
                $title.removeAttr("disabled");
                //$editTitle.show();
                break;
        }
    });
    //$editTitle.on("input",function(){
    //    $title.val($(this).val());
    //});

    /* 表头设置 结束 */


    //var templateId = "showmethemoney123";

    if (templateId) {
        loadDataByTemplateId(templateId);
    }

    /* 加载数据 结束 */

    /* 预览 开始 */
    $("#form_edit_preview").click(function () {
        if (validateTemplate()) {
            var templateName = $editName.val(),
                description = $editDesc.val(),
                defaultTitle = $editTitleType.filter(":checked").val() == "1";

            $("#preview_form_name").text(templateName);
            $("#preview_form_description").text(description);
            $("#preview_form_title").prop("disabled", defaultTitle);
            $("#preview_modal_content").widgetFormResolver(form.getJson(), true);

            $("#preview_modal").modal({
                backdrop: false,
                show: true
            });
        }
    });
    $("#preview_modal").on("hidden.bs.modal", function () {
        $("#preview_modal_content").empty();
        $("#preview_modal .rating .hit").removeClass("hit");
        $("#preview_form_title").val("");
        $("#preview_form_create_user").val("");
        $("#preview_form_create_department").val("");
        $("#preview_form_create_time").val("");
    });
    /* 预览 结束 */

    /* 取消 开始 */
    $("#form_edit_cancel").click(function () {
         systemOrFalse = systemFalse;
         loadViewToMain("/TemplateCategory/TemplateCategory");
    });
    /* 取消 结束 */

    /* 保存 开始 */
    $("#form_edit_save").click(function () {
        if (validateTemplate()) {
            var data = getFormData(0);
            $.ajax({
                url: "/TemplateEdit/SaveTemplate",
                type: "post",
                dataType: "json",
                data: {
                    data: JSON.stringify(data)
                },
                success: rsHandler(function (data) {
                    ncUnits.alert("保存成功!");
                    loadDataByTemplateId(templateId);
                })
            })
        }
    });
    /* 保存 结束 */

    /* 保存&应用 开始 */
    //$("#form_edit_saveUse,#preview_modal_submit").click(function () {
    //    if (validateTemplate()) {
    //        var data = getFormData(1);
    //        console.log(data);
    //        $.ajax({
    //            url:"../../test/data/success.json",
    //            type: "post",
    //            dataType: "json",
    //            data: {
    //                data: JSON.stringify(data)
    //            },
    //            success: rsHandler(function (data) {
    //                console.log("保存并应用成功");
    //            })
    //        })
    //    }
    //});
    /* 保存&应用 结束 */

    // 按ID加载模板
    function loadDataByTemplateId(templateId) {
        $.ajax({
            url: "/TemplateEdit/GetTemplateInfo",
            type: "post",
            dataType: "json",
            data: {
                id: templateId
            },
            success: rsHandler(function (data) {
                var template = data.template;

                $name.html(template.templateName);
                $editName.val(template.templateName);

                $desc.html(template.description);
                $editDesc.val(template.description);
                $editTitleType.filter("[value='" + template.defaultTitle + "']").click();

                form.loadData(data.controlInfo);
            })
        })
    }

    //获取当前模板数据
    function getFormData(status) {
        var templateName = $editName.val(),
            description = $editDesc.val(),
            defaultTitle = $editTitleType.filter(":checked").val(),
            controlInfo = form.getJson(),
            deleteControl = form.getDeleteControl(),
            deleteControlItem = form.getDeleteControlItem();
        return {
            template: {
                templateId: templateId,
                templateName: templateName,
                description: description,
                defaultTitle: defaultTitle,
                status: status
            },
            controlInfo: controlInfo,
            deleteControl: deleteControl,
            deleteControlItem: deleteControlItem
        }
    }

    //验证模板
    function validateTemplate() {
        layer.closeTips();
        if (!$editName.val().length) {
            validate_reject("模板名不能为空", $editName);
            return false;
        }
        return true;
    }
    /* 模板编辑 结束 */

    /* 流程编辑 开始 */

    var $addNodeSwitch = $("#addNode");
    //$("#flow_edit .nav-item").click(function () {
    //    $("#flow_edit .nav-item").removeClass("green_color");
    //    $(this).addClass("green_color");
    //});



    /* ---------------------------流程设置 开始----------------------------- */

    //设置右侧弹出框的位置
    $(".rightModal").css({
        "left": ($(".rightModal").parents('.modal-dialog').width() - 5)
    });

    var conditionArray = ["属于", "不属于", "等于", "大于", "小于"],
        conditionType = ["组织架构", "岗位", "人力资源", "控件"];

    var flowTab = 1;    //1:节点设置 2:流程设置  3:流程图
    var argus = [],    //设置、添加节点的所有信息
         delList = [],     //最终删除节点的Id
         delConditionList = [],      //最终删除的条件Id
         delCondition = [],   //删除流程条件Id
         addConditionList = [],     //新增的条件数组
         flow_info,        //
         flowSetIndex=-1,            //当前设置节点在argus中的下标，等于-1表示不曾被设置过
         flowSetButton;                 //点击的设置按钮

    var flowOrgCheckId = [], flowJobCheckId = [], JobOrg=-1, flowPerSonCheckId = [], personOrg=-1;    //选择条件Id

    var FlowInNodeList = [];

    $("#flowSetTab").click(function (e) {
        e.preventDefault();
        //切换判断
        var flag = false;
        if (flowTab == 1) {        //如果当前页面是节点设置画面
            flag = nodeModefyOrNot();
        } else if (flowTab == 2) {         //流程设置画面
            flag = modifyOrFalse();
        }
        if (flag == true) {
            ncUnits.confirm({
                title: '提示',
                html: '你的设置还没保存,确定要退出当前页面?',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $("#flowSetTab").tab('show');
                    $("#flow_edit .nav-item").removeClass("green_color");
                    $("#flowSetTab").addClass("green_color");
                    FlowArrayClean();
                    InOutNodeList();
              //     flowList();
                    flowTab = 2;
                    $addNodeSwitch.show();
                }
            });
        } else {
            $("#flowSetTab").tab('show');
            $("#flow_edit .nav-item").removeClass("green_color");
            $("#flowSetTab").addClass("green_color");
            FlowArrayClean();
            InOutNodeList();
           // flowList();
            flowTab = 2;
            $addNodeSwitch.show();
        }
    });


    //流程设置全局数组的清空
    function FlowArrayClean() {
        delList.length = 0;
        delConditionList.length = 0;
        argus.length = 0;
        addConditionList.length = 0;
        delCondition.length = 0;
    }


    //切换到别的页面时,判断是否修改  true代表已修改
    function modifyOrFalse() {
        if (argus.length != 0 || delList.length != 0 || delConditionList.length != 0 || delCondition.length != 0 || addConditionList.length != 0) {
            return true;
        } else {
            return false;
        }
    }


    //入口节点出口节点列表获取
    function InOutNodeList() {                 //$dropDownId 表示下拉框ul
        FlowInNodeList.length = 0;
        $("#flowTable").empty();
        var flowLoad = getLoadingPosition($("#flowTable"));     //显示load层
        //  FlowOutNodeList.length = 0;
        $.ajax({
            type: "post",
            url: "/FlowEdit/GetNodeList",
            dataType: "json",
            data: { id: templateId },
            complete: rcHandler(function () {
                flowLoad.remove();
                flowList();
            }),
            success: rsHandler(function (data) {
                $.each(data, function (i, v) {  
                    var inNodes = {
                        id: Number,
                        name: String,
                        type: Number
                    };
                    inNodes.id = v.nodeId
                    inNodes.name = v.nodeName;
                    inNodes.type = v.nodeType;
                    FlowInNodeList.push(inNodes);
                });
            })
        });
    }

    //dropdown下拉框事件
    function dropDownEvent(value) {
        var x = $(value).parents("ul").prev().find("span:eq(0)");
        x.text($(value).text());
        var term = $(value).attr("term");
        x.attr("term", term);
    }

    //节点对状态的控制 value指下拉框
    function statusControl(value) {
        var typeid = $(value).parent().prev().find(".dropdown-toggle span:eq(0)").attr("term").split(",");
        var type = typeid[0];
        if (type == 1 || type == 2) {
            $(value).find("ul li a[term='1']").trigger("click");
            $(value).find(".dropdown-toggle").attr("disabled", true)
        } else {
            $(value).find(".dropdown-toggle").attr("disabled", false);
        }
    }

    //修改节点流程信息
    function modifyNode(value, flag) {
        var setBtn = $(value).parents("tr").find("td:eq(2) button");
        var textArray = [];
        if (parseInt($(setBtn).attr("term")) == -1) {      //表示该节点流程信息尚未修改过
            var flowNodeSet = {
                nodeLinkMode: undefined,
                linkConditionList: undefined,
                linkFormulaList: undefined
            };

            var link = {
                linkId: Number,           //节点出口ID
                nodeEntryId: Number,        //入口节点
                nodeExitId: Number,         //出口节点
                templateId: Number,       //模板ID
                status: Number            //状态
            };



            link.linkId = $(value).parents("tr").find("td:eq(4) span").attr("term");
            textArray = $(value).parents("tr").find("td:eq(0) .dropdown-toggle span:eq(0)").attr("term").split(",");
            link.nodeEntryId = textArray[1];
            textArray = $(value).parents("tr").find("td:eq(3) .dropdown-toggle span:eq(0)").attr("term").split(",");
            link.nodeExitId = textArray[1];
            link.templateId = templateId;
            link.status = $(value).parents("tr").find("td:eq(1) .dropdown-toggle span:eq(0)").attr("term");

            flowNodeSet.nodeLinkMode = link;
            flowNodeSet.linkConditionList = [];
            flowNodeSet.linkFormulaList = [];
            argus.push(flowNodeSet);
            $(setBtn).attr("term", argus.length - 1);
        } else {               //表示该节点流程信息修改过
            var index = parseInt($.trim($(setBtn).attr("term")));
            if (flag == 1) {      //修改了入口节点
                textArray = $(value).parents("tr").find("td:eq(0) .dropdown-toggle span:eq(0)").attr("term").split(",");
                argus[index].nodeLinkMode.nodeEntryId = textArray[1];
            } else if (flag == 2) {         //修改了状态
                argus[index].nodeLinkMode.status = $(value).parents("tr").find("td:eq(1) .dropdown-toggle span:eq(0)").attr("term");

            } else if (flag == 3) {          //修改了出口节点
                textArray = $(value).parents("tr").find("td:eq(3) .dropdown-toggle span:eq(0)").attr("term").split(",");
                argus[index].nodeLinkMode.nodeExitId = textArray[1];
            }
        }
    }


    function statusDrop($dropmenu, choseStatus, changeFlag) {
        $.each($dropmenu, function (j, v1) {
            if (changeFlag) {
                var $exit = $(this).parents("td").find(".dropdown-toggle span:eq(0)");
                $exit.attr("term", "");
                $exit.text("");
            }
            var types = $(v1).attr("term").split(",")[0];
            if (types == "4" || types == "2") {
                if (choseStatus == 0) {         //   如果状态为不通过  如果是归档和提交节点,需要隐藏
                    $(v1).parent().hide();
                }
                else {
                    $(v1).parent().show();
                }
            }
            if (types == "1") {
                if (choseStatus == 0) {         //   如果状态为不通过  创建节点,显示
                    $(v1).parent().show();
                }
                else {
                    $(v1).parent().hide();
                }
            }
        });
    }


    //节点流程取得
    function flowList() {
        var $flowTable = $("#flowTable");

        $flowTable.empty();
        var flowLoad = getLoadingPosition($flowTable);     //显示load层
        var $thead = $("<thead class='colorGray'><tr><th>节点名称</th><th>状态</th><th>条件</th><th>出口节点</th><th></th></tr></thead>");
        var $tbody = $("<tbody></tbody>");

        $.ajax({
            type: "post",
            url: "/FlowEdit/GetNodeLinkList",
            dataType: "json",
            data: { id: templateId },
            success: rsHandler(function (data) {
                $flowTable.empty();
                $.each(data, function (i, v) {

                    //初始化出入口列表

                    var $tr = $("<tr></tr>");
                    //入口节点
                    var $td = $("<td style='width:25%'></td>");
                    var $dropDown = $("<div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                        "<span term=" + v.nodeEntryType + "," + v.nodeEntryId + ">" + v.nodeEntryName + "</span><span class='upPic'></span></span></div>");
                    var $ul = $("<ul class='dropdown-menu' role='menu' ></ul>");

                    $.each(FlowInNodeList, function (i, v) {
                        if (v.type != 4) {           //如果不是归档节点
                            var $li = $("<li><a href='#' term=" + v.type + "," + v.id + ">" + v.name + "</a></li>");
                            $li.appendTo($ul);
                        }
                    });
                    $ul.find("li a").off("click");
                    $ul.find("li a").click(function () {
                        dropDownEvent($(this));
                        statusControl($($dropDown).parent().next().find(".dropdown"));
                        modifyNode($(this), 1);
                    });
                    $dropDown.append($ul).appendTo($td);

                    //状态  通过 不通过 应该是 1 0
                    var $tdStatus = $("<td style='width:20%'><div class='dropdown'><button  class='dropdown-toggle form-control' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                        "<span term=" + v.status + ">" + (v.status == 1 ? "通过" : "不通过") + "</span><span class='upPic'></span></button>" +
                        "<ul class='dropdown-menu' role='menu' ><li><a href='#' term='1' >通过</a></li><li><a href='#' term='0'>不通过</a></li></ul></div></td>");

                    $tdStatus.find("a").off("click");
                    $tdStatus.find("a").click(function () {
                        // dropDownEvent($(this));
                        var changeFlag = false;
                        if ( $(this).attr("term") != $(this).parents("td").find(".dropdown-toggle span:eq(0)").attr("term")) {
                            changeFlag = true;
                        }
                        //当状态为不通过时, 控制输出口不可为归档 提交
                        var $dropmenu = $(this).parents("td").next().next().find(".dropdown-menu li a");
                        var choseStatus = parseInt($(this).attr("term"));
                        $.each($dropmenu, function (j, v1) {
                            if (changeFlag) {
                                var $exit = $(this).parents("td").find(".dropdown-toggle span:eq(0)");
                                $exit.attr("term", "");
                                $exit.text("");
                            }

                            var types = $(v1).attr("term").split(",")[0];
                            if (types == "4" || types == "2") {
                                if (choseStatus == 0) {         //   如果状态为不通过  如果是归档和提交节点,需要隐藏
                                    $(v1).parent().hide();
                                }
                                else {
                                    $(v1).parent().show();
                                }
                            }
                            if (types == "1") {
                                if (choseStatus == 0) {         //   如果状态为不通过  创建节点,显示
                                    $(v1).parent().show();
                                }
                                else {
                                    $(v1).parent().hide();
                                }
                            }
                        });
                        dropDownEvent($(this));
                        modifyNode($(this), 2);
                    });

                    if (v.nodeEntryType == 1 || v.nodeEntryType == 2) {
                        $($tdStatus).find("button").attr("disabled", true);
                        // $($tdStatus).find("button").attr("readonly", "readonly");
                    }

                    //设置
                    var $tdSet = $("<td style='width:10%'></td>");
                    var $btnSet = $("<button class='btn btn-default widthParent' data-toggle='modal' data-target='#flowSet_modal' style='text-align: left' term='-1' value=" + v.linkId + ">设置</button>");
                    $btnSet.click(function () {
                        //TODO
                        flow_info = v;
                        flowSetIndex = $(this).attr("term");        //获取flowSetIndex，以便查询该节点之前的设置信息
                        flowSetButton = $(this);

                    });
                    $tdSet.append($btnSet);

                    //出口节点
                    var $tdExit = $("<td style='width:25%'></td>");
                    var $dropDownExit = $("<div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                        "<span term=" + v.nodeExitType + "," + v.nodeExitId + ">" + v.nodeExitName + "</span><span class='upPic'></span></span></div>");

                    var $ulExit = $("<ul class='dropdown-menu' role='menu'  ></ul>");
                    $.each(FlowInNodeList, function (j, v1) {
                        var $li = $("<li><a href='#' term=" + v1.type + "," + v1.id + " >" + v1.name + "</a></li>");
                        $li.appendTo($ulExit);
                        //if (v1.type == 1 && v.status==0) {        
                        //    $li.hide();
                        //}

                        if (v1.type == "4" || v1.type == "2") {
                            if (v.status == 0) {         //   如果状态为不通过  如果是归档和提交节点,需要隐藏
                                $li.hide();
                            }
                            else {
                                $li.show();
                            }
                        }
                        if (v1.type == "1") {
                            if (v.status == 0) {         //   如果状态为不通过  创建节点,显示
                                $li.show();
                            }
                            else {
                                $li.hide();
                            }
                        }

                    });

                    $ulExit.find("a").off("click");
                    $ulExit.find("a").click(function () {
                        dropDownEvent($(this));
                        modifyNode($(this), 3);
                    });
                    $dropDownExit.append($ulExit).appendTo($tdExit);


                    //删除
                    var $tdDel = $("<td style='width:5%'></td>");
                    var $spanDel = $("<span class='delPic' term=" + v.linkId + "></span>");
                    $spanDel.click(function () {
                        var $this = $(this);
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $this.parents("tr").remove();
                                delList[delList.length] = v.linkId;

                                //如果该节点曾今被设置，则将argus中的设置信息删除
                                var index = $this.parents("tr").find("td:eq(2) button").attr("term");  //获取设置按钮的term
                                if (index != -1) {
                                    argus[index] = null;
                                }
                            }
                        });
                    });
                    $tdDel.append($spanDel);

                    $tr.append([$td, $tdStatus, $tdSet, $tdExit, $tdDel]);
                    $tbody.append($tr);
                });
                $flowTable.append([$thead, $tbody]);
            }),
            complete: rcHandler(function () {
                //添加节点 + 事件
                flowLoad.remove();         //关闭load层
                $("#addNode").off("click");
                $("#addNode").click(function () {
                    var flowNodeSet = {
                        nodeLinkMode: undefined,
                        linkConditionList: undefined,
                        linkFormulaList: undefined
                    };

                    var link = {
                        linkId: Number,           //节点出口ID
                        nodeEntryId: Number,        //入口节点
                        nodeExitId: Number,         //出口节点
                        templateId: Number,       //模板ID
                        status: Number            //状态
                    };

                    link.linkId = null;
                    link.nodeEntryId = null;
                    link.nodeExitId = null;
                    link.templateId = templateId;
                    link.status = 1;

                    flowNodeSet.nodeLinkMode = link;
                    flowNodeSet.linkConditionList = [];
                    flowNodeSet.linkFormulaList = [];
                    argus.push(flowNodeSet);

                    //界面增加一个tr元素
                    var $tr = $("<tr></tr>");
                    //入口节点
                    var $td = $("<td style='width:25%'></td>");
                    var $dropDown = $("<div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                        "<span > </span><span class='upPic'></span></span></div>");
                    var $ul = $("<ul class='dropdown-menu' role='menu'  ></ul>");
                    $.each(FlowInNodeList, function (i, v) {
                        if (v.type != 4) {
                            var $li = $("<li><a href='#' term=" + v.type + "," + v.id + ">" + v.name + "</a></li>");
                            $li.appendTo($ul);
                        }
                    });
                    $ul.find("li a").off("click");
                    $ul.find("li a").click(function () {
                        dropDownEvent($(this));
                        statusControl($($dropDown).parent().next().find(".dropdown"));
                        modifyNode($(this), 1);
                    });
                    $dropDown.append($ul).appendTo($td);

                    //状态
                    var $tdStatus = $("<td style='width:20%'><div class='dropdown'><button  class='dropdown-toggle form-control' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                        "<span term='1'>通过</span><span class='upPic'></span></button>" +
                        "<ul class='dropdown-menu' role='menu' ><li><a href='#' term='1' >通过</a></li><li><a href='#' term='0'>不通过</a></li></ul></div></td>");
                    $tdStatus.find("a").off("click");
                    $tdStatus.find("a").click(function () {
                     
                        var changeFlag = false;
                        if ($(this).attr("term") != $(this).parents("td").find(".dropdown-toggle span:eq(0)").attr("term")) {
                            changeFlag = true;
                        }
                        //当状态为不通过时, 控制输出口不可为归档 提交
                        var $dropmenu = $(this).parents("td").next().next().find(".dropdown-menu li a");
                        var choseStatus = parseInt($(this).attr("term"));
                        statusDrop($dropmenu, choseStatus, changeFlag);
                        //$.each($dropmenu, function (j, v1) {
                        //    var types = $(v1).attr("term").split(",")[0];
                        //    if (types == "4" || types == "2") {
                        //        if (choseStatus == 1) {         //   如果状态为不通过  如果是归档和提交节点,需要隐藏
                        //            $(v1).parent().hide();
                        //        }
                        //        else {
                        //            $(v1).parent().show();
                        //        }
                        //    }
                        //    if (types == "1") {
                        //        if (choseStatus == 1) {         //   如果状态为不通过  创建节点,显示
                        //            $(v1).parent().show();
                        //        }
                        //        else {
                        //            $(v1).parent().hide();
                        //        }
                        //    }
                        //});
                        dropDownEvent($(this));
                        modifyNode($(this), 2);
                    });

                    //设置
                    var $tdSet = $("<td style='width:10%'></td>");
                    var $btnSet = $("<button class='btn btn-default widthParent' term=" + (argus.length - 1) + " style='text-align: left' >设置</button>");
                    $btnSet.click(function () {
                        flowSetIndex = $(this).attr("term");
                        flowSetButton = $(this);
                        flow_info = argus[flowSetIndex].nodeLinkMode;
                        $("#flowSet_modal").modal('show');
                    });
                    $tdSet.append($btnSet);


                    //出口节点
                    var $tdExit = $("<td style='width:25%'></td>");
                    var $dropDownExit = $("<div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                        "<span> </span><span class='upPic'></span></span></div>");
                    var $ulExit = $("<ul class='dropdown-menu' role='menu'  ></ul>");
                    $.each(FlowInNodeList, function (j, v1) {
                        var $li = $("<li><a href='#' term=" + v1.type + "," + v1.id + " >" + v1.name + "</a></li>");
                        $li.appendTo($ulExit);
                        //if (v1.type == 1) {           //默认通过时 出口不得有创建节点
                        //    $li.hide();
                        //}
                    });
                    statusDrop($($ulExit).find("li a"), 1,false);

                    $ulExit.find("a").off("click");
                    $ulExit.find("a").click(function () {
                        dropDownEvent($(this));
                        modifyNode($(this), 3);
                    });
                    $dropDownExit.append($ulExit).appendTo($tdExit);


                    //删除
                    var $tdDel = $("<td style='width:5%'></td>");
                    var $spanDel = $("<span class='delPic' term=" + (argus.length - 1) + "></span>");
                    $spanDel.click(function () {
                        var $this = $(this);
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $this.parents("tr").remove();
                                argus[$this.attr("term")] = null;        //删除新增流程设置，将对应的argus元素置为null
                            }
                        });
                    });
                    $tdDel.append($spanDel);
                    $tr.append([$td, $tdStatus, $tdSet, $tdExit, $tdDel]);
                    $flowTable.find("tbody").append($tr);
                });

            })
        });
    }





    //流程设置模态框 打开
    $('#flowSet_modal').on('shown.bs.modal', function () {
        $("#rightCondition").show();
        $("#rightZtree").hide();
        addConditionList.length = 0;            //清空上次设置所添加的条件数组
        delCondition.length = 0;               //清空上次设置所删除数组

        $("#flowSet_modal_content .dropdown ul a").off("click");
        $("#flowSet_modal_content .dropdown ul a").click(function () {
            dropDownEvent(this);
        });

        //条件列表
        var $conditionTable = $("<table class='table table-hover table-condensed' id='conditionTable'><thead><tr><th style='width:40px'>编号</th><th style='width:130px'>字段</th><th style='width:130px'>条件</th><th style='width:40px'>操作</th></tr></thead></table>");
        var $tbody = $("<tbody></tbody>");
        $.ajax({
            type: "post",
            url: "/FlowEdit/GetLinkConditionRelatedInfo",
            dataType: "json",
            data: { linkId: flow_info.linkId, templateId: templateId },
            success: rsHandler(function (data) {
              //  console.log("哈哈哈哈");
                var controlList = data.controlInfoList;
                var conditionList = data.linkConditionList;
            //    console.log("controlList:" + controlList + "conditionList: " + conditionList);
                //模板字段列表
                var $flowField = $("#fieldOther");
                if (controlList != null) {
                    $.each(controlList, function (i, v) {
                        //2：数字输入框 3：金额小写 12：日期 14：日期时间 
                        var $row = $("<div  class='row'><label class='radio-inline col-xs-2' title=" + v.controlTitle + "><input type='radio' name='conditionType' class='otherRadio' value=" + v.controlType + "  term=" + v.controlId + "> " + v.controlTitle + " </label><div>");
                        var $dropdown = $("<div class='col-xs-3'><div class='dropdown'><span class='dropdown-toggle' data-toggle='dropdown'  data-delay='50' role='button' aria-expanded='false'>" +
                            "<span term='3' class='flowCondition' >等于</span><span class='upPic'></span></span>" +
                            "<ul class='dropdown-menu' role='menu'>" +
                            "<li><a href='#' term='3'>等于</a></li><li class='divider short'></li>" +
                            "<li><a href='#' term='4'>大于</a></li><li class='divider short'></li>" +
                            "<li><a href='#' term='5'>小于</a></li></ul></div></div>");
                        $dropdown.find("li a").click(function () { dropDownEvent(this); });
                        if (v.controlType == 2 || v.controlType == 3) {
                            var $input = $('<div class="col-xs-3"><input type="text"  class="form-control conditionResult" ></div>');
                            $input.find("input").focus(function () {
                                radioAutoCheck($(this));
                            });
                            $input.find("input").blur(function () {
           
                                if(  /^-?\d+(\.)$/.test( $(this).val() ) ){
                                    var values = $(this).val().substring(0, $(this).val().length - 1);
                                    $(this).val(values);
                                }
                            });
                            $input.find("input").bind("input", function () {
                                var reg = /^-?\d+(\.\d{1,2})$/;
                                var reg1 = /^-?\d+(\.)$/;
                                var reg2 = /^-?\d+$/;
                                var values = $(this).val().substring(0, $(this).val().length - 1);
                                var getValue = $(this).val();
                                if (reg2.test(getValue) == true) {
                                    return;
                                }
                                if (reg1.test(getValue) == true) {
                                    return;
                                }
                                if (reg.test(getValue) == false) {
                                    $(this).val(values);
                                    return
                                }
                            });


                        } else {
                            var $input = $("<div class='col-xs-3'><input type='button' class='btn btn-default conditionResult' ></div>");
                            var timeFlag, startTime;
                            if (v.controlType == 12) {
                                timeFlag = false;
                                startTime = new Date().toLocaleDateString();
                                formatTime = 'YYYY-MM-DD';
                            } else {
                                timeFlag = true;
                                startTime = new Date().toLocaleDateString() + ' 17:30:00';
                                formatTime = 'YYYY-MM-DD hh:mm';
                            }
                            var end = {
                                elem: $input.find(".conditionResult").get(0), //需显示日期的元素选择器
                                event: 'click', //触发事件
                                format: formatTime, //日期格式
                                istime: timeFlag, //是否开启时间选择
                                isclear: true, //是否显示清空
                                istoday: true, //是否显示今天
                                issure: true, //是否显示确认
                                festival: true, //是否显示节日
                                start: startTime,    //开始日期
                                choose: function (dates) { //选择好日期的回调
                                    endTime_v = dates;                         
                                },
                                clear: function () {
                                    endTime_v = undefined;
                                }
                            }

                            $input.click(function () {
                                radioAutoCheck($(this));
                                laydate(end);
                            });
                        }

                        $row.append([$dropdown, $input]).appendTo($flowField);
                    });
                }

                //条件列表
                if (conditionList != null) {
                    var cindex = 0;
                    $.each(conditionList, function (i, v) {

                        //检查该条件是否在删除数组中，若在删除数组中，则跳过不显示
                        if ($.inArray(v.conditionId, delConditionList) >= 0) {
                            return true;
                        }
                        cindex++;
                        //填写条件编号
                        var $indexBtn = $(" <button class='btn btn-transparency mathBtn' term=" + v.conditionId + ">" + cindex + "</button>");
                        $indexBtn.click(function () {
                            mathLetterBtn(this);
                        });
                        $("#conditionIndex").append($indexBtn);

                        //条件列表
                        var field, condition;
                        if (v.type >= 4) {        //如果是其它控件
                            field = v.controlTitle + "(" + conditionArray[v.condition - 1] + ")";
                            condition = v.value;
                        } else {
                            field = conditionType[v.type - 1] + "(" + conditionArray[v.condition - 1] + ")";
                            condition = v.targetName;
                        }
                        var $tr = $("<tr><td style='width:40px' term=" + v.conditionId + ">" + cindex + "</td><td  style='width:130px' title=" + field + ">" + field + "</td><td  style='width:130px' title=" + condition + ">" + condition + "</td></tr>");
                        var $deltd = $("<td style='width:40px'></td>")
                        var $del = $("<span class='delPic' term=" + cindex + "></span>");

                        //条件删除
                        $del.click(function () {
                            var $this = $(this);
                            var mathResult = $("#mathResult").text();
                            var deldelIndex = parseInt($.trim($(this).attr("term")));
                            if (mathResult.indexOf(deldelIndex.toString()) >= 0) {
                                ncUnits.alert("无法删除：条件已被使用！");
                            }
                            else {
                                ncUnits.confirm({
                                    title: '提示',
                                    html: '确认要删除吗？',
                                    yes: function (layer_confirm) {
                                        layer.close(layer_confirm);
                                        delCondition.push(v.conditionId);
                                        //修改下面条件的编号 公式编号 公式                                    
                                        $("#conditionIndex button").each(function () {            //移除编号
                                            if (parseInt($(this).text()) == deldelIndex) {
                                                $(this).nextAll().each(function () {
                                                    $(this).text(parseInt($(this).text()) - 1);
                                                });
                                                $(this).remove();
                                                return false;
                                            }
                                        });
                                        var trs = $this.parents("tr").nextAll();          //改变条件的序列号
                                        var mathOld = $.trim($("#mathResult").text());
                                        trs.each(function () {
                                            var indexOld = $(this).find("td:eq(0)").text();
                                            var indexNew = String(parseInt(indexOld) - 1);
                                            var re = new RegExp(indexOld, "g");
                                            mathOld = mathOld.replace(re, indexNew);
                                            $(this).find("td:eq(0)").text(indexNew);

                                            var x = parseInt($(this).find("td .delPic").attr("term")) - 1;
                                            $(this).find("td .delPic").attr("term", x);
                                            //修改之前临时添加条件的编号
                                            if (flowSetIndex != -1 && argus[flowSetIndex].linkConditionList.length != 0) {
                                                var conList = argus[flowSetIndex].linkConditionList;
                                                $.each(conList, function (j, v1) {
                                                    if (v1.serialNumber == indexOld) {
                                                        conList[j].serialNumber = indexNew;
                                                        return false;
                                                    }
                                                });
                                            }

                                            $.each(addConditionList, function (j, v1) {      //还需要修改addConditionList中的serialNumber
                                                if (v1.serialNumber == indexOld) {
                                                    addConditionList[j].serialNumber = indexNew;
                                                    return false;
                                                }
                                            });
                                        });
                                        $("#mathResult").text(mathOld);           //改变公式
                                        $this.parents("tr").remove();
                                    }
                                });
                            }
                        });

                        $deltd.append($del).appendTo($tr);
                        $tbody.append($tr);
                    });
                }
                $("#mathResult").text(data.linkFormulaInfo);
                mathJustify();          //判断哪些公式符号是不可用的
            }),
            complete: function () {
                //新增的条件列表
                if (flowSetIndex != -1) {      //表示该节点曾经被设置过
                    var conList = argus[flowSetIndex].linkConditionList;
                    $.each(conList, function (i, v) {

                        //添加条件编号
                        var $indexBtn = $(" <button class='btn btn-transparency mathBtn' term=" + v.conditionId + ">" + v.serialNumber + "</button>");
                        $indexBtn.click(function () {
                            mathLetterBtn(this);
                        });
                        $("#conditionIndex").append($indexBtn);

                        //添加条件
                        var $tr = $("<tr><td style='width:40px' term=" + v.conditionId + ">" + v.serialNumber + "</td><td  style='width:130px' title=" + v.controlTitle + ">" + v.controlTitle + "</td><td  style='width:130px' title=" + v.targetName + ">" + v.targetName + "</td></tr>");
                        var $deltd = $("<td style='width:40px'></td>");
                        var $del = $("<span class='delPic' term=" + i + " ></span>");

                        //条件删除
                        $del.click(function () {
                            var $this = $(this);
                            var mathResult = $("#mathResult").text();
                            if (mathResult.indexOf(v.serialNumber.toString()) >= 0) {
                                ncUnits.alert("无法删除：条件已被使用！");
                            }
                            else {
                                ncUnits.confirm({
                                    title: '提示',
                                    html: '确认要删除吗？',
                                    yes: function (layer_confirm) {
                                        layer.close(layer_confirm);
                                        var condelIndex = parseInt($this.attr("term"));
                                        for (k = condelIndex + 1; k < conList.length; k++) {
                                            conList[k].serialNumber = conList[k].serialNumber - 1;
                                        }
                                        conList.splice(condelIndex, 1);          //在之前新增的条件列表中删除此条件

                                        //修改下面条件的编号 公式编号 公式
                                        $("#conditionIndex button").each(function () {            //移除编号
                                            if (parseInt($(this).text()) == v.serialNumber) {
                                                $(this).nextAll().each(function (v1) {
                                                    $(this).text(parseInt($(this).text()) - 1);
                                                });
                                                $(this).remove();
                                                return false;
                                            }
                                        });
                                        var trs = $this.parents("tr").nextAll();          //改变条件的序列号
                                        var mathOld = $.trim($("#mathResult").text());
                                        trs.each(function () {
                                            var indexOld = $(this).find("td:eq(0)").text();
                                            var indexNew = String(parseInt(indexOld) - 1);
                                            var re = new RegExp(indexOld, "g");
                                            mathOld = mathOld.replace(re, indexNew);
                                            $(this).find("td:eq(0)").text(indexNew);
                                            var x = parseInt($(this).find("td .delPic").attr("term")) - 1;
                                            $(this).find("td .delPic").attr("term", x);
                                            $.each(addConditionList, function (j, v1) {      //还需要修改addConditionList中的serialNumber
                                                if (v1.serialNumber == indexOld) {
                                                    addConditionList[j].serialNumber = indexNew;
                                                    return false;
                                                }
                                            });
                                        });
                                        $("#mathResult").text(mathOld);           //改变公式

                                        $this.parents("tr").remove();
                                    }
                                });
                            }
                        });
                        $deltd.append($del).appendTo($tr);
                        $tbody.append($tr);

                    });
                }

                //公式
                if (flowSetIndex != -1) {         //如果公式被修改，还原原来的公式
                    var formulaTime = argus[flowSetIndex].linkFormulaList;
                    var txt = "";
                    $.each(formulaTime, function (i, v) {
                        if (v.displayText != null) {
                            txt = txt + v.displayText;
                        }
                        else if (v.operate == "|") {
                            txt = txt + "或";
                        }
                        else if (v.operate == "&") {
                            txt = txt + "且";
                        } else {
                            txt = txt + v.operate;
                        }
                    });
                    $("#mathResult").text(txt);
                    mathJustify();          //判断哪些公式符号是不可用的
                }
              //  console.log("complete");
            }
        });
        $tbody.appendTo($conditionTable);
        $("#rightContent").empty().append($conditionTable);
    });



    //选择申请人岗位+事件
    $("#flow_addJob").off("click");
    $("#flow_addJob").click(function () {
        rightChoose(1)
        radioAutoCheck($(this));
    });

    //选择申请人+事件
    $("#flow_addPerson").off("click");
    $("#flow_addPerson").click(function () {
        rightChoose(2);
        radioAutoCheck($(this));
    });

    //选择申请人部门+事件
    $("#flow_addDepart").off("click");
    $("#flow_addDepart").click(function () {
        rightChoose(3);
        radioAutoCheck($(this));
    });

    //单选框自动跳转
    function radioAutoCheck(value) {
        $(value).parents(".row").find("input").prop("checked", true);
    }

    var flowPersonWithSub = 0, flowJobWithSub=0, flowChosenOrg = null;
    function rightChoose(flag) {
        $("#rightCondition").hide();             //右侧条件隐藏
        $("#rightZtree").show();                  //右侧树显示
        $("#flowZtreecontent").empty();          //右侧树弹窗清空
        //  flowCheckId.length = 0;            //条件Id数组清空

        if (flag == 3) {                   //如果是部门选择
            $("#flowZtreeTitle").text("部门选择");
            var $flowOrgZtree = $("<div class='ztree flowZtreeOrg' id='partChooseTree'  style='height:100%'></div>");
            treeOrgLoadCheck();
            $("#flowZtreecontent").append($flowOrgZtree);
        } else {
            if (flag == 1) {
                $("#flowZtreeTitle").text("岗位选择");
            } else {
                $("#flowZtreeTitle").text("人员选择");
            }
            var $flowOrgZtree = $("<div class='ztree flowZtreeOrg'></div>");
            treeOrgLoad(flag);
            var $title = $("<hr class='hrAll'/><div class='checkbox' style='padding-left: 5px;margin-bottom: -5px;'>" +
                "<label class='pull-left'><input type='checkbox' class='flowContain'>包含下级</label><label style='margin-left: 220px;'><input type='checkbox' class='flowSelectAll'>全部选择</label></div><hr style='margin-bottom: 0px;'/>");
            $title.find(".flowSelectAll").click(function () {
                selectAllFunc(this, flag);
            });
            if (flag == 1) {
                if (flowJobWithSub == 1)
                    $($title).find(".flowContain").prop("checked",true);
            } else {
                if (flowPersonWithSub == 1)
                    $($title).find(".flowContain").prop("checked", true);
            }
            $title.find(".flowContain").click(function () {
                if (flag == 1) {
                    if ($(this).is(':checked')) {
                        flowJobWithSub = 1;
                    } else {
                        flowJobWithSub = 0;
                    }
                    if (JobOrg != -1) {
                        appendPersonJob(JobOrg, flag);
                    }
                }
                else {
                    if ($(this).is(':checked')) {
                        flowPersonWithSub = 1;
                    } else {
                        flowPersonWithSub = 0;
                    }
                    if (personOrg != -1) {
                        appendPersonJob(personOrg, flag);
                    }
                }
            });
            var $flowListUl = $("<div id='flowJobList'><ul class='list-unstyled' style='margin-top: -5px;'></ul></div>");
            $("#flowZtreecontent").append([$flowOrgZtree, $title, $flowListUl]);
        }
    }

    //增加条件
    $(".conditionAdd a").click(function () {
        //TODO  条件判断
        var $checked = $("#flowField input:checked");
        var targetName = "", field = "";            //条件结果

        //获取输入的条件
        var radioType;
        if ($($checked).hasClass("otherRadio")) {
            radioType = 4;
            targetName = $checked.parents(".row").find(".conditionResult").val();
        }
        else {
            radioType = $checked.val();
            if (radioType == 1){
                $("#flow_orgTarget li").each(function () {                
                    targetName = targetName + $(this).find("span:eq(0)").text() + ",";
                })
                targetName = targetName.substring(0, targetName.length - 1);
           //     targetName = $.trim($("#flow_orgTarget").text());
            }      
            else if (radioType == 2){
                $("#flow_jobTarget li").each(function(){
                    targetName = targetName + $(this).find("span:eq(0)").text()  + ",";
                })
                targetName = targetName.substring(0, targetName.length - 1);
                //   targetName = $.trim($("#flow_jobTarget").text());
            }
             
            else if (radioType == 3) {
                $("#flow_personTarget li").each(function () {
                    targetName = targetName + $(this).find("span:eq(0)").text()  + ",";
                })
                targetName = targetName.substring(0, targetName.length - 1);
             //   targetName = $.trim($("#flow_personTarget").text());
            }
        }


        //清空
        $("#flow_orgTarget").text("");
        $("#flow_jobTarget").text("");
        $("#flow_personTarget").text("");
        $("#fieldOther .conditionResult").val("");

        var condition = {
            serialNumber: Number,       //序列
            conditionId: Number,    //条件Id
            linkId: Number,         //节点出口ID
            type: Number,           //条件类型
            controlId: Number,      //控件ID
            condition: Number,      //条件
            value: String,       //结果
            targetId: undefined,     //目标ID
            targetName: String,       //目标name  为了保存临时数据，最终传递数据时可以去掉
            controlTitle: String          //控件的Name  为了保存临时数据，最终传递数据时可以去掉
        };

        if (radioType == 1) {            //组织架构
            if (targetName == "") {
                ncUnits.alert("请选择条件范围!");
                return;
            } else {
                condition.targetId = flowOrgCheckId.slice(0);
                condition.value = null;
                condition.controlId = null;
                field = conditionType[radioType - 1] + "(" + $checked.parents(".row").find(".flowCondition").text() + ")";
            }
        } else if (radioType == 2 || radioType == 3) {           //岗位    人员
            if (targetName == "") {
                ncUnits.alert("请选择条件范围!");
                return;
            } else {
                field = conditionType[radioType - 1] + "(" + $checked.parents(".row").find(".flowCondition").text() + ")";
                if (radioType == 2) {
                    condition.targetId = flowJobCheckId.slice(0);
                } else {
                    condition.targetId = flowPerSonCheckId.slice(0);
                }
                condition.value = null;
                condition.controlId = null;
            }
        } else {                      //其他控件
            if (targetName.length == 0) {
                ncUnits.alert("请选择条件范围!");
                return
            }
            //TODO 输入条件结果的判断
            condition.value = targetName;
            condition.targetId = null;
            condition.controlId = $checked.attr("term");
            field = $checked.parent().text() + "(" + $checked.parents(".row").find(".flowCondition").text() + ")";
        }
        condition.condition = $checked.parents(".row").find(".flowCondition").attr("term");
        condition.type = radioType;
        condition.linkId = flow_info.linkId;
        condition.conditionId = null                         //新增的条件Idcondition.type
        var cIndex = $("#conditionTable tr:last td:eq(0)").text();
        if (cIndex == "") {
            condition.serialNumber = 1;
        }
        else {
            condition.serialNumber = parseInt(cIndex) + 1;
        }
        condition.targetName = targetName;                 //额外添加的两个数据，数据传递时可去掉
        condition.controlTitle = field;
        addConditionList.push(condition);

        //添加一个条件tr
        var $addtr = $("<tr><td style='width:40px' term=" + condition.conditionId + ">" + condition.serialNumber + "</td><td style='width:130px' title=" + field + ">" + field + "</td><td style='width:130px' title=" + targetName + ">" + targetName + "</td></tr>");
        var $deltd = $("<td style='width:40px'></td>");
        var $del = $("<span class='delPic' term=" + condition.serialNumber + "></span>");
        //新增条件的删除
        $del.click(function () {
            var $this = $(this);
            var mathResult = $("#mathResult").text();
            var indexs = $this.attr("term");
            if (mathResult.indexOf(indexs.toString()) >= 0) {
                ncUnits.alert("无法删除：条件已被使用！");
            }
            else {
                ncUnits.confirm({
                    title: '提示',
                    html: '确认要删除吗？',
                    yes: function (layer_confirm) {
                        layer.close(layer_confirm);
                        addConditionList.splice($.inArray(condition, addConditionList), 1);
                        //修改下面条件的编号 公式编号 公式
                        $("#conditionIndex button").each(function () {            //移除编号
                            if (parseInt($(this).text()) == indexs) {
                                $(this).nextAll().each(function () {
                                    $(this).text(parseInt($(this).text()) - 1);
                                });
                                $(this).remove();
                                return false;
                            }
                        });
                        var trs = $this.parents("tr").nextAll();          //改变条件的序列号
                        var mathOld = $.trim($("#mathResult").text());
                        trs.each(function () {
                            var indexOld = $(this).find("td:eq(0)").text();
                            var indexNew = String(parseInt(indexOld) - 1);
                            var re = new RegExp(indexOld, "g");
                            mathOld = mathOld.replace(re, indexNew);
                            $(this).find("td:eq(0)").text(indexNew);
                            $(this).find("td .delPic").attr("term", indexNew);
                            $.each(addConditionList, function (i, v) {      //还需要修改addConditionList中的serialNumber
                                if (v.serialNumber == indexOld) {
                                    addConditionList[i].serialNumber = indexNew;
                                    return false;
                                }
                            });
                        });
                        $("#mathResult").text(mathOld);           //改变公式
                        $this.parents("tr").remove();

                    }
                });
            }
        });
        $deltd.append($del).appendTo($addtr);
        $("#conditionTable").append($addtr);

        //同时添加一个条件编号
        var $indexBtn = $("<button class='btn btn-transparency mathBtn' term=" + condition.conditionId + ">" + condition.serialNumber + "</button>");
        $indexBtn.click(function () {
            mathLetterBtn(this);
        });
        $("#conditionIndex").append($indexBtn);
        mathJustify();          //判断新增加按钮是否为disabled的

        $("#rightZtree").hide();           //右侧树隐藏
        $("#rightCondition").show();             //右侧条件显示
        $("#flowZtreecontent").empty();          //岗位人员列表清空

        //挑选的条件Id清空
        flowOrgCheckId.length = 0;
        flowJobCheckId.length = 0;
        flowPerSonCheckId.length = 0;
        JobOrg = -1;
        personOrg = -1;
    });

    //流程设置模态框 关闭  值清空
    $('#flowSet_modal').on('hidden.bs.modal', function () {
        //TODO
        $("#conditionIndex").empty();        //公式编号清空
        $("#mathResult").empty();             //公式结果清空
        $("#rightContent").empty();          //右侧条件列表弹窗清空
        $("#flowZtreecontent").empty();      //右侧树结构清空
        $("#fieldOther").empty();            //控件字段清
        $("#flow_personTarget").empty();
        $("#flow_orgTarget").empty();
        $("#flow_jobTarget").empty();

        $("#flowField .belong").attr("term", 1);
        $("#flowField .belong").text("属于");
        $("#fieldOther .flowCondition").attr("term", 3);
        $("#fieldOther .flowCondition").text("等于");

        flow_info = null;
        flowSetButton = null;
        flowSetIndex = -1;
        delCondition.length = 0;
        addConditionList.length = 0;

        $("#flowField").find(".row input:eq(0)").prop("checked", true);

        //挑选的条件Id清空
        flowOrgCheckId.length = 0;
        flowJobCheckId.length = 0;
        flowPerSonCheckId.length = 0;
        JobOrg = -1;
        personOrg = -1;
        flowPersonWithSub = 0;
        flowJobWithSub = 0;
        flowChosenOrg = null;
    });


    //弹出框组织架构菜单的加载  flag:1 表示根据架构选择岗位  flag:2 根据架构选择人员
    function treeOrgLoad(flag) {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($(".flowZtreeOrg"), $.extend({
                    callback: {
                        onNodeCreated: function (e, id, node) {
                            if (flag == 1 && node.id == JobOrg) {
                                var ztrees = $.fn.zTree.getZTreeObj(id);
                                ztrees.checkNode(node, true, false);
                                appendPersonJob(node.id, flag);
                            } else if (flag == 2 && node.id == personOrg) {
                                var ztrees = $.fn.zTree.getZTreeObj(id);
                                ztrees.checkNode(node, true, false);
                                appendPersonJob(node.id, flag);
                            }
                        },
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {        //选中事件
                            if (node.checked) {
                                appendPersonJob(node.id, flag);
                               // flowChosenOrg = node.id;
                                if (flag == 1)
                                    JobOrg = node.id;
                                else if (flag == 2)
                                    personOrg = node.id;
                            }
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },

                    check: {            //设置为单选按钮
                        enable: true,
                        chkStyle: "radio",
                        radioType: "all",
                        chkboxType: { "Y": "", "N": "" }
                    },

                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    //部门选择树
    function treeOrgLoadCheck() {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#partChooseTree"), $.extend({
                    callback: {
                        onNodeCreated: function (e, id, node) {
                            if ($.inArray(node.id, flowOrgCheckId) >= 0) {
                                var ztrees = $.fn.zTree.getZTreeObj(id);
                                ztrees.checkNode(node, true, false);
                            }
                        },
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {        //选中事件
                          //  var txt = $.trim($("#flow_orgTarget").text());
                            if (node.checked) {
                                flowOrgCheckId.push(node.id);
                                var $li = $("<li term=" + node.id + " ><span  title=" + node.name + ">" + node.name + "</span><span class='glyphicon glyphicon-remove'></span></li>");
                                $li.find("span:eq(1)").click(function () {
                                    var nodeId = $(this).parent().attr("term");                                   
                                    flowOrgCheckId.splice($.inArray(nodeId, flowOrgCheckId), 1);             
                                    var ztreess = $.fn.zTree.getZTreeObj("partChooseTree");
                                    var nodes = ztreess.getNodeByParam("id",nodeId, null);
                                    ztreess.checkNode(nodes, false, false);
                                    $(this).parent().remove();
                                });
                                $("#flow_orgTarget").append($li);
                                //if (txt == "")
                                //    txt = txt + node.name;
                                //else
                                //    txt = txt + "," + node.name;
                            }
                            else {
                                flowOrgCheckId.splice($.inArray(node.id, flowOrgCheckId), 1);
                                //var txta = txt.split(",");
                                //txta.splice($.inArray(node.name, txta), 1);
                                //txt = txta.join(",");
                                $("#flow_orgTarget li[term=" + node.id + "]").remove();
                            }
                      //      $("#flow_orgTarget").text(txt);
                      //     $("#flow_orgTarget").attr("title", txt);
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    check: {
                        autoCheckTrigger: false,
                        enable: true,
                        chkStyle: "checkbox",
                        chkboxType: { "Y": "", "N": "" } //Y：勾选，p：关联父节点，s：关联子节点，N：取消勾选
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    //人员列表 岗位列表   flag:1 表示根据架构选择岗位  flag:2 根据架构选择人员
    function appendPersonJob(id, flag) {
        var url, argusPersonJob;
        if (flag == 2) {      //人员列表
            url = "/Shared/GetUserList";
            argusPersonJob = { withSub: flowPersonWithSub, organizationId: id, withUser: true };
        } else {             //岗位列表
            url = "/Shared/GetStationList";
            argusPersonJob = { withSub: flowJobWithSub, organizationId: id };
        }
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data: argusPersonJob,
            success: rsHandler(function (data) {
                $("#flowJobList ul").empty();
                $.each(data, function (i, v) {
                    var $li = $("<li></li>");
                    var $label = $("<div class='checkbox'><label></label></div>");
                    var $checkBox, $span;
                    if (flag == 2) {       //人员列表
                        $checkBox = $("<input type='checkbox' class='flowPersonCheck' value=" + v.userId + ">");
                        if ($.inArray(v.userId, flowPerSonCheckId) >= 0) {             //如果已存在则默认被选中
                            $($checkBox).prop("checked", true);
                        }
                        $span = $("<span>" + v.userName + "</span>-<span>" + v.organizationName + "</span>");
                    } else {               //岗位列表
                        $checkBox = $("<input type='checkbox'  class='flowJobCheck' value=" + v.stationId + ">");
                        if ($.inArray(v.stationId, flowJobCheckId) >= 0) {             //如果已存在则默认被选中
                            $($checkBox).prop("checked", true);
                        }
                        $span = $("<span title=" + v.stationName + " >" + v.stationName + "</span>-<span>" + v.organizationName + "</span>");
                    }
                    $checkBox.click(function () {
                        if ($(this).prop("checked")) {
                            if ($(this).parents("#flowJobList").find("li input").length == $(this).parents("#flowJobList").find("li input:checked").length) {
                                $(".flowSelectAll").prop("checked", true);
                            }
                            //存储选择的岗位，人员     
                            if (flag == 2) {      //人员
                                flowPerSonCheckId.push(parseInt($(this).val()));
                                var $li = $("<li term=" + v.userId + " ><span  title=" + v.userName + ">" + v.userName + "</span><span class='glyphicon glyphicon-remove'></span></li>");
                                $li.find("span:eq(1)").click(function () {
                                    var useId = $(this).parent().attr("term");
                                    flowPerSonCheckId.splice( $.inArray( parseInt(useId), flowPerSonCheckId), 1);
                                    $(".flowSelectAll").prop("checked", false);
                                    $(this).parent().remove();
                                    $(".flowPersonCheck[value=" + useId + "]").prop("checked",false);
                                });
                                $("#flow_personTarget").append($li);

                            }
                            else {
                                flowJobCheckId.push(parseInt($(this).val()));                            
                                var $li = $("<li term=" + v.stationId + " ><span  title=" + v.stationName + ">" + v.stationName + "</span><span class='glyphicon glyphicon-remove'></span></li>");
                                $li.find("span:eq(1)").click(function () {
                                    var jobId = $(this).parent().attr("term");
                                    flowJobCheckId.splice( $.inArray( parseInt(jobId), flowJobCheckId), 1);
                                    $(".flowSelectAll").prop("checked", false);
                                    $(this).parent().remove();
                                    $(".flowJobCheck[value=" + jobId + "]").prop("checked", false);
                                });
                                $("#flow_jobTarget").append($li);
                            }
                        } else {
                            $(".flowSelectAll").prop("checked", false);
                            if (flag == 2) {      //人员
                                $("#flow_personTarget li[term=" + v.userId + "]").remove();
                                flowPerSonCheckId.splice($.inArray(parseInt($(this).val()), flowPerSonCheckId), 1);
                            }
                            else {
                                $("#flow_jobTarget li[term=" + v.stationId + "]").remove();                               
                                flowJobCheckId.splice($.inArray(parseInt($(this).val()), flowJobCheckId), 1);
                            }
                        }
                    });
                    $label.find("label").append([$checkBox, $span]);
                    $li.append($label);
                    $("#flowJobList ul").append($li);
                })
                if ($("#flowJobList").find("li input").length != 0 && $("#flowJobList").find("li input").length == $("#flowJobList").find("li input:checked").length) {
                    $(".flowSelectAll").prop("checked", true);
                } else {
                    $(".flowSelectAll").prop("checked", false);
                }
            })
        });
    }

    //全选
    function selectAllFunc(value, flag) {
        var flowCheckId;
        if (flag == 1) {
            var $ul = $("#flow_jobTarget");
            flowCheckId = flowJobCheckId;
        } else if (flag == 2) {
            var $ul = $("#flow_personTarget");
            flowCheckId = flowPerSonCheckId;
        }
        if ($(value).prop("checked")) {
            $("#flowJobList li input").prop("checked", true);
            $("#flowJobList li input").each(function () {
                if ($.inArray(parseInt($(this).val()), flowCheckId) < 0) {
                    flowCheckId.push(parseInt($(this).val()));                  
                    var $li = $("<li term=" + $(this).val() + " ><span  title=" + $.trim($(this).next().text())  + ">" + $.trim($(this).next().text()) + "</span><span class='glyphicon glyphicon-remove'></span></li>");
                    $li.find("span:eq(1)").click(function () {
                        var useId = $(this).parent().attr("term");
                        flowCheckId.splice($.inArray( parseInt(useId), flowCheckId), 1);
                        $(this).parent().remove();
                        $(".flowSelectAll").prop("checked", false);
                        if (flag == 1) {
                            $(".flowJobCheck[value=" + useId + "]").prop("checked", false);
                        }
                        else {
                            $(".flowPersonCheck[value=" + useId + "]").prop("checked", false);
                        }

                    });
                    $ul.append($li);
                }
            });
        }
        else {
            $("#flowJobList li input").prop("checked", false);
            $("#flowJobList li input").each(function () {
                var index = $.inArray(parseInt($(this).val()), flowCheckId);
                if (index >= 0) {
                    flowCheckId.splice(index, 1);
                   $($ul).find("li[term=" + $(this).val() + "]").remove();
                }
            })
        }
    }

    
    //公式符号
    $("#mathLetter .mathBtn").off("click");
    $("#mathLetter .mathBtn").click(function (e) {
        mathLetterBtn(this);
    });

    function mathLetterBtn(value) {
        var texts = $("#mathResult").text() + $(value).text();
        $("#mathResult").text(texts);
        mathJustify();
    }

    //退格
    $("#flowClearBack").click(function () {
        var texts = $("#mathResult").text();
        $("#mathResult").text(texts.substring(0, texts.length - 1));
        mathJustify();
    });

    //公式判断函数
    function mathJustify() {
        var math = $.trim($("#mathResult").text());
        if (math == "") {
            $("#mathLetter .mathBtn").attr("disabled", true);
            $("#mathLetter .mathBtn[term='3']").attr("disabled", false);
            $("#conditionIndex .mathBtn").attr("disabled", false);
            return;
        }
        var lastChar = math[math.length - 1];
        if (lastChar == "或" || lastChar == "且" || lastChar == "(") {
            $("#mathLetter .mathBtn").attr("disabled", true);
            $("#mathLetter .mathBtn[term='3']").attr("disabled", false);
            $("#conditionIndex button").each(function () {
                if (math.indexOf($(this).text()) < 0) {
                    $(this).attr("disabled", false);
                }
            });
        } else if (lastChar == ")") {                     //")"
            $("#mathLetter .mathBtn").attr("disabled", false);
            $("#conditionIndex button").attr("disabled", true);
            $("#mathLetter .mathBtn[term='3']").attr("disabled", true);
            var num1 = math.split("(").length - 1;
            var num2 = math.split(")").length - 1;
            if (num2 == num1) {
                $("#mathLetter .mathBtn[term='4']").attr("disabled", true);
            }
        } else {               //数字
            $("#conditionIndex button").attr("disabled", true);
            $("#mathLetter .mathBtn").attr("disabled", false);
            $("#mathLetter .mathBtn[term='3']").attr("disabled", true);
            var num1 = math.split("(").length - 1;
            var num2 = math.split(")").length - 1;
            if (num2 == num1) {
                $("#mathLetter .mathBtn[term='4']").attr("disabled", true);
            } else {
                $("#mathLetter .mathBtn[term='4']").attr("disabled", false);
            }
        }
        var flag = false;
        $("#conditionIndex button").each(function () {
            if (math.indexOf($(this).text()) < 0) {
                flag = true;
            }
        });
        if (flag == false && math != "") {
            $("#mathLetter .mathBtn[term='1']").attr("disabled", true);
            $("#mathLetter .mathBtn[term='2']").attr("disabled", true);
        }
    }

    //条件设置保存按钮事件
    $("#flowSet_save").click(function () {
        //判断公式是否正确
        var math = $.trim($("#mathResult").text());
        //if (math == "" && $("#conditionTable tr").length > 1 ) {
        //    ncUnits.alert("无法保存：必须设置公式!");
        //    return;
        //}
        var lastChar = math[math.length - 1];
        var num1 = math.split("(").length - 1;
        var num2 = math.split(")").length - 1;
        if (num1 != num2 || lastChar == "或" || lastChar == "且") {
            ncUnits.alert("无法保存：公式不正确！");
            return;
        }
        //var flag = true;
        //$("#conditionIndex .mathBtn").each(function () {
        //    var x = $.trim($(this).text());;
        //    if ( math.indexOf(x) < 0 ) {
        //        ncUnits.alert("公式必须包含所有条件！");
        //        flag = false;
        //    }
        //});
        //if( flag == false ){
        //    return;
        //}


        var flowNodeSet = {
            nodeLinkMode: undefined,
            linkConditionList: undefined,
            linkFormulaList: undefined
        };

        var link = {
            linkId: Number,           //节点出口ID
            nodeEntryId: Number,        //入口节点
            nodeExitId: Number,         //出口节点
            templateId: Number,       //模板ID
            status: Number            //状态
        };

        link.linkId = flow_info.linkId;
        link.nodeEntryId = flow_info.nodeEntryId;
        link.nodeExitId = flow_info.nodeExitId;
        link.status = flow_info.status;
        link.templateId = templateId;

        var formulaList = [];
        math = math.split("");
        var orderNum = 0;
        for (var i = 0; i < math.length; i++){
            v=math[i];
      //  $.each(math, function (i, v) {
            orderNum++;
            var formula = {
                serialNumber: Number,       //序列
                formulaId: Number,       //条件公式ID
                linkId: Number,          //节点出口ID
                conditionId: Number,    //流程条件ID
                displayText: String,    //表示名
                operate: String,        //操作符
                orderNum: Number        //排序
            };

            formula.serialNumber = null;          //TODO
            formula.formulaId = null;
            formula.linkId = flow_info.linkId;
            formula.conditionId = null;           //为操作符时 其为null
            formula.displayText = null;           //为操作符时 其为null
            formula.orderNum = orderNum;
            if (v == "或") {
                formula.operate = "|";
            } else if (v == "且") {
                formula.operate = "&";
            } else if (v == "(" || v == ")") {
                formula.operate = v;
            } else {
                var numnum = v;
                var jk =i+1;
                
                //console.log(math[jk]+" "+jk);
                for (jk = i + 1; parseInt(math) > 0 && parseInt(math[jk]) <= 9; jk++) {
                    numnum = numnum + math[jk];
                    i++;
                    //console.log("haah" + math[jk]);
                }
                //console.log("numnum"+numnum);
                formula.operate = null;
                formula.displayText = numnum;
                formula.serialNumber = parseInt(numnum);
                $("#conditionIndex .mathBtn").each(function () {
                    var x = $.trim($(this).text());
                    if (x == numnum) {
                        formula.conditionId = parseInt($(this).attr("term"));
                        return false;
                    }
                });
            }
            formulaList.push(formula);
        };

        if (flowSetIndex == -1) {   //如果之前没有被设置过
            flowNodeSet.nodeLinkMode = link;
            flowNodeSet.linkConditionList = addConditionList.slice(0);
            flowNodeSet.linkFormulaList = formulaList;
            argus.push(flowNodeSet);
            //改变该设置按钮的term值
            $(flowSetButton).attr("term", (argus.length - 1));
        } else {            //曾今被设置过
            argus[flowSetIndex].linkConditionList = argus[flowSetIndex].linkConditionList.concat(addConditionList);
            argus[flowSetIndex].linkFormulaList = formulaList;
        }
        delConditionList = delConditionList.concat(delCondition);
        $("#flowSet_modal").modal('hide');
    });

    //条件设置取消按钮
    $("#flowSet_cancel").click(function () {
        ncUnits.confirm({
            title: '提示',
            html: '确认要取消设置？',
            yes: function (layer_confirm) {
                layer.close(layer_confirm);
                $("#flowSet_modal").modal('hide');
            }
        });
    });



    //保存按钮
    $("#flowAllSave").off("click");
    $("#flowAllSave").click(function () {
        flowSetSaveAll(1);
    });

    //保存并返回按钮
    $("#flowAllSaveReturn").off("click");
    $("#flowAllSaveReturn").click(function () {
        flowSetSaveAll(2);
    });


    //流程设置的保存事件
    function flowSetSaveAll(flag) {

        //   保存前的控制       1：创建 2：提交 3:审批 4：归档
        var $tr = $("#flowTable tr");
        var $nodeEntry = [], $nodeExit = [], $status = [];
        $.each($tr, function (i, v) {
            if (i == 0) {
                return true;
            }
            var a1 = $(v).find("td:eq(0) .dropdown-toggle span:eq(0)");
            $nodeEntry.push(a1);
            a1 = $(v).find("td:eq(3) .dropdown-toggle span:eq(0)");
            $nodeExit.push(a1);
            a1 = $(v).find("td:eq(1) .dropdown-toggle span:eq(0)").attr("term");
            $status.push(a1);
        });

        var hasCreateFlag = false, commitFlag = true, checkFlag = false, statuFlag = false, noStatuFlag = false, timeFlag = true, circleFlag = true;
        var haveEndFlag = false, nullFlag = true, sameFlag = true, inOutSameFlag = false;
        var inArray = [],checkArray=[];    //入口的节点类型;
        $.each($nodeEntry, function (i, v) {
            if ($.trim($(v).text()) == $.trim($($nodeExit[i]).text())) {
                inOutSameFlag = true;
            }

            if ($.trim($(v).text()) == "") {     //入口不得为空
                nullFlag = false;
                return false;
            }
            var typeid = $(v).attr("term").split(",");
            var type = typeid[0];
            if (type == 1) {         //入口节点必须包含创建节点
                hasCreateFlag = true;
            }
            if (type == 2 && $status[i] != 1) {       //入口节点类型为"提交",必须为通过
                commitFlag = false;
            }
            if (type == 3) {               // 入口节点类型为"审批"时，必须同时存在通过和不通过
                if ($.inArray(typeid[1], checkArray) < 0) {
                    checkArray.push(typeid[1]);
                    checkFlag = true;
                    statuFlag = false;
                    var statusOld = $status[i];
                    $.each($nodeEntry, function (k, v1) {
                        if ($.trim(v1.attr("term")) != "") {
                            var id = v1.attr("term").split(",");
                            if (typeid[1] == id[1] && $status[k] != statusOld) {
                                statuFlag = true;
                            }
                        }
                    })
                    if (statuFlag == false) {
                        return false;
                    }
                }
            }
            //判断是否有重复的出入口对 
            $.each(inArray, function (j, v1) {
                if (typeid[1] == v1 && $($nodeExit[i]).text() == $($nodeExit[j]).text()) {
                    sameFlag = false;
                    return false;
                }
            });

            if (type == 3) {        
                var exitType = $nodeExit[i].attr("term").split(",");
                if (exitType[0] == 3) {
                    $.each($nodeExit, function (j1, k1) {
                        var exitexitId = k1.attr("term").split(",");
                        var ininId = $nodeEntry[j1].attr("term").split(",");
                        if (exitexitId[1] == typeid[1] && ininId[1] == exitType[1]) {
                            circleFlag = false;
                            return true;
                        }
                    });
                }
            }

            inArray.push(typeid[1]);

        });

        $.each($nodeExit, function (i, v) {
            if ($.trim($(v).attr("term")) == "") {          //出口不得为空
                nullFlag = false;
                return false;
            }
            var typeArray = $(v).attr("term").split(",");
            var type = typeArray[0];
            if (type == 4) {        // 出口节点中必须要有归档节点
                haveEndFlag = true;
            }
            if (type == 4 && $status[i] != 1) {       //出口节点类型为"归档"时，必须为通过
                commitFlag = false;
            }
            if (type != 4) {                    // 除"创建"和"归档"节点外，其他节点必须同时存在于入口和出口节点中
                if ($.inArray(typeArray[1], inArray) < 0)
                    timeFlag = false;
            }
        });

        if (nullFlag == false) {
            ncUnits.alert("失败：出入口节点不得为空");
            return;
        }
        if (inOutSameFlag) {
            ncUnits.alert("失败：出入口节点不能相同！");
            return;
        }
        if (hasCreateFlag == false) {
            ncUnits.alert("失败：入口节点必须包含创建节点");
            return;
        }
        if (commitFlag == false) {
            ncUnits.alert("失败： 入口节点类型为提交或出口节点类型为归档时，必须为通过");
            return;
        }
        if (checkFlag == true) {
            if (statuFlag == false) {
                ncUnits.alert("失败：入口节点类型为审批时，必须同时存在通过和不通过");
                return;
            }
        }
        if (haveEndFlag == false) {
            ncUnits.alert("失败：出口节点中必须要有归档节点");
            return;
        }
        if (timeFlag == false) {
            ncUnits.alert("失败：除创建和归档节点外，其他节点必须同时存在于入口和出口节点中");
            return;
        }
        if (sameFlag == false) {
            ncUnits.alert("失败：不可包含重复设置!");
            return;
        }
        if (circleFlag == false) {
            ncUnits.alert("失败：审批节点不可同时存在出入口相反的两条设置!");
            return;
        }



        //argus 参数的处理
        $.each(argus, function (i, v) {
            if (v == null) {          //如果为空，则移除
                argus.splice(i, 1);
                return true;
            }
            if (v.linkConditionList.length == 0) {
                return true;
            }
            $.each(v.linkConditionList, function (j, v1) {         //去掉条件列表中多余的参数
                var conditionNew = {
                    serialNumber: Number,       //序列
                    conditionId: Number,    //条件Id
                    linkId: Number,         //节点出口ID
                    type: Number,           //条件类型
                    controlId: Number,      //控件ID
                    condition: Number,      //条件
                    value: String,       //结果
                    targetId: undefined,    //目标ID
                    controlTitle: String          //控件的Name  为了保存临时数据，最终传递数据时可以去掉
                };
                conditionNew.serialNumber = v1.serialNumber;
                if (v1.conditionId == null)
                    conditionNew.conditionId = v1.conditionId;
                else
                conditionNew.conditionId = parseInt(v1.conditionId);
                conditionNew.linkId = v1.linkId;
                conditionNew.type = v1.type;
                conditionNew.controlId = v1.controlId;
                conditionNew.condition = v1.condition;
                conditionNew.value = v1.value;
                conditionNew.targetId = v1.targetId;
                conditionNew.controlTitle = v1.controlTitle;
                argus[i].linkConditionList[j] = conditionNew;
                 
              
            });
        });
        //保存

        var saveAgrus = {
            nodeLinkInfoList: undefined,
            deleteLinkId: undefined,
            deleteCondition: undefined
        }
        saveAgrus.nodeLinkInfoList = argus;
        saveAgrus.deleteLinkId = delList;
        saveAgrus.deleteCondition = delConditionList;
        $.ajax({
            url: "/FlowEdit/SaveNodeLink",
            type: "post",
            dataType: "json",
            data: {
                data: JSON.stringify(saveAgrus), templateId: templateId
            },
            success: rsHandler(function (data) {
              //  console.log("保存成功，传递的参数为：" + argus);
                ncUnits.alert("保存成功!");
                if (flag != 1) {              //跳转到表单管理界面
                    systemOrFalse = systemFalse;
                    FlowArrayClean();       //清空流程设置的全局数组
                    loadViewToMain("/TemplateCategory/TemplateCategory");         
                } else {
                    FlowArrayClean();
                    InOutNodeList();
                    //     flowList();
                    flowTab = 2;
                    $addNodeSwitch.show();
                }      
            }),
            complete: function () {
           //     console.log("保存完成");
                FlowArrayClean();       //清空流程设置的全局数组
            }
        });
    }

    /* --------------------------流程设置 结束 ------------------------------*/




    /* ---------------------------节点设置 开始--------------------------- */


    $("#nodeSetTab").click(function (e) {
        e.preventDefault();
        //切换判断
        var flag = false;
        if (flowTab == 1) {        //如果当前页面是节点设置画面
            flag = nodeModefyOrNot();
        } else if (flowTab == 2) {         //流程设置画面
            flag = modifyOrFalse();
        }
        if (flag == true) {
            ncUnits.confirm({
                title: '提示',
                html: '你的设置还没保存,确定要退出当前页面?',
                yes: function (layer_confirm) {
                    layer.close(layer_confirm);
                    $("#nodeSetTab").tab('show');
                    $("#flow_edit .nav-item").removeClass("green_color");
                    $("#nodeSetTab").addClass("green_color");
                    nodeList();
                    flowTab = 1;
                    $addNodeSwitch.show();
                }
            });
        } else {
            $("#nodeSetTab").tab('show');
            $("#flow_edit .nav-item").removeClass("green_color");
            $("#nodeSetTab").addClass("green_color");
            nodeList();
            flowTab = 1;
            $addNodeSwitch.show();
        }
    });
    var nodeRetrunData = {
        nodeInfo: [],
        deleteNode: [],        //删除节点ID
        deleteOperateId: []    //删除节点操作人ID
    };//全局变量
    var nodeAjaxRetrunData = {
        nodeInfo: [],
        deleteNode: [],        //删除节点ID
        deleteOperateId: []    //删除节点操作人ID
    };//全局变量
    var nodeId;//全局变量
    var nodeType;//全局变量
    var nodeData = {
        node: {
            nodeId: 0,           //节点ID
            templateId: 0,       //模板ID
            nodeName: "",     //节点名
            nodeType: 0          //节点类型
        },
        nodeField: [],
        nodeOperate: []
    }
    var targetIdArray1 = [];   //操作人目标ID  全局变量
    var targetNameArray1 = [];   //操作人目标Name  全局变量
    var targetorganizationId1 = null;//用于标记最后一次点击的组织架构id，在重新点击回到改组织架构之后，自动加载对应数据
    var targetWithSub1 = null;//用于标记最后一次点击的组织架构id时，是否点击的是包含下级
    var targetIdArray2 = [];   //操作人目标ID  全局变量
    var targetNameArray2 = [];   //操作人目标Name  全局变量
    var targetorganizationId2 = null;
    var targetWithSub2 = null;//用于标记最后一次点击的组织架构id时，是否点击的是包含下级
    var targetIdArray3 = [];   //操作人目标ID  全局变量
    var targetNameArray3 = [];   //操作人目标Name  全局变量
    var batchTargetIdArray1 = [];//批次目标ID  全局变量
    var batchTargetNameArray1 = [];//批次目标Name 全局变量
    var batchTargetorganizationId1 = null;
    var batchTargetWithSub1 = null;//用于标记最后一次点击的组织架构id时，是否点击的是包含下级
    var batchTargetIdArray2 = [];//批次目标ID  全局变量
    var batchTargetNameArray2 = [];//批次目标Name 全局变量
    var batchTargetorganizationId2 = null;
    var batchTargetWithSub2 = null;//用于标记最后一次点击的组织架构id时，是否点击的是包含下级
    var batchTargetIdArray3 = [];//批次目标ID  全局变量
    var batchTargetNameArray3 = [];//批次目标Name 全局变量

    var nodeOperateArray = [];//节点操作 全局变量
    var delOperateIdArray = [];  //删除节点操作人ID
    var nodeListLength = 0;//全局变量 记录数据库中读取的弄得list长度
    var batchOrNot = 0;//全局变量
    var ztreeFlag = 0;//全局变量
    var batchTypeFlag = new Array(null, null, null);//全局变量 记录batchType的点击状况
    //流程节点取得
    var nodeModefyFlag = 0;
    function nodeOperateModalClear() {
        batchTypeFlag[0] = null; batchTypeFlag[1] = null; batchTypeFlag[2] = null;
        $("#nodeOperateTableHead").css("width", "100%");
        nodeOperateArray.length = 0;
        targetIdArray1.length = 0;
        targetNameArray1.length = 0;
        targetIdArray2.length = 0;
        targetNameArray2.length = 0;
        targetIdArray3.length = 0;
        targetNameArray3.length = 0;
        batchTargetIdArray1.length = 0;
        batchTargetNameArray1.length = 0;
        targetIdArray2.length = 0;
        targetNameArray2.length = 0;
        targetIdArray3.length = 0;
        targetNameArray3.length = 0;
        targetorganizationId1 = null;
        targetWithSub1 = null;
        targetorganizationId2 = null;
        targetWithSub2 = null;
        batchTargetorganizationId1 = null;
        batchTargetWithSub1 = null;
        batchTargetorganizationId2 = null;
        batchTargetWithSub2 = null;
        var $tbody = $("#nodeOperateConditionTable tbody");
        $tbody.empty();
        $(".node-rightModal").css("display", "none");
        $(".Operate-nodeName").text("");
        $("#node-countersign").attr("term", "");
        $("#node-countersign").text("");
        $("#node-countersign").parent(".dropdown-toggle").removeClass("disabled");
        $("#node-countersign").parent(".dropdown-toggle").removeClass("noChoose");
        $(".node-batchCondition").parent(".dropdown-toggle").removeClass("disabled");
        $(".node-batchCondition").parent(".dropdown-toggle").removeClass("noChoose");

        $("#nodeOperateSet_modal .chooseName").each(function () {
            $(this).attr("title", "");
            $(this).text("");
        })
        $("#node-countersign").parent().siblings("ul").find("li").each(function () {
            $(this).css("display", "inline");
        })
        $(".NodeOperate-type").each(function () {
            $(this).prop("checked", false);
        })
        //无上级岗位
        $(".NodeOperate-type[term=1]").closest(".row").css("display", "block");
        $(".NodeOperate-type[term=4]").closest(".row").css("display","block");
        $(".NodeOperate-type[term=5]").closest(".row").css("display", "block");
        $(".NodeOperate-batchType").each(function () {
            $(this).prop("disabled", false);
        })
        $(".NodeOperate-batchType").each(function () {
            $(this).prop("checked", false);
        })

        $(".node-batchCondition").each(function () {
            $(this).attr("term", "1");
            $(this).text("属于");
        })


        $("#nodeOperateSet_modal .dropdown ul a").click(function () {
            nodeDropDownEvent(this);
        });
        $("#node_addProposerDepart").removeClass("noClick");
        $("#node_addProposerPost").removeClass("noClick");
        $("#node_addProposer").removeClass("noClick");
        //
        $(".NodeOperate-type:eq(0)").prop("disabled", false);
        $("#node_addOperatorDepart").removeClass("noClick");
        //
        $(".NodeOperate-type:eq(3)").prop("disabled", false);

    }
    function nodeDropDownEvent(value) {
        nodeModefyFlag = 1;
        var changeFlag = 0;
        $nodeId = $(value).closest("ul").attr("term");
        $NewNodeType = $(value).attr("term");
        $oldNodeType = $(value).closest(".dropdown").find(".nodeTypeSpan").attr("term");
        if ($NewNodeType != $oldNodeType && $(value).closest("ul").hasClass("changeAlert")) {         
            if ($nodeId.toString().substr(0, 1) != "z") {//数据库原有节点
                $.ajax({
                    type: "post",
                    url: " /NodeEdit/CheckDeleteFlag",
                    dataType: "json",
                    data: { nodeId: $nodeId },
                    success: rsHandler(function (deleteFlag) {
                        if (deleteFlag == true) {
                            //如果可以删除
                            for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
                                //判断是否存在数据需要清空
                                if (nodeRetrunData.nodeInfo[i].node.nodeId == $nodeId) {
                                    if (nodeRetrunData.nodeInfo[i].nodeOperate.length > 0) {
                                        ncUnits.confirm({
                                            title: '提示',
                                            html: '节点类型修改将清空操作者设置，确认继续？',
                                            yes: function (layer_confirm) {
                                                layer.close(layer_confirm);
                                                nodeRetrunData.nodeInfo[i].nodeOperate.length = 0;
                                                var x = $(value).parents("ul").prev().find("span:eq(0)");
                                                x.text($(value).text());
                                                var term = $(value).attr("term");
                                                x.attr("term", term);
                                            }
                                        });
                                    }
                                    
                                    break;
                                }
                            }

                        }
                        else {
                            ncUnits.alert("该节点在流程设置中已经进行相关设置，不能修改节点类型");
                        }

                    })
                })
            }
            else {//新添加节点
                for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
                    //判断是否新添加信息，并且提示
                    if (nodeRetrunData.nodeInfo[i].node.nodeId == $nodeId && nodeRetrunData.nodeInfo[i].nodeOperate.length > 0) {
                        //有新添加信息，提示
                        var jilu = i;
                        ncUnits.confirm({
                            title: '提示',
                            html: '节点类型修改将清空操作者设置，确认继续？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                nodeRetrunData.nodeInfo[jilu].nodeOperate.length = 0;
                                var x = $(value).parents("ul").prev().find("span:eq(0)");
                                x.text($(value).text());
                                var term = $(value).attr("term");
                                x.attr("term", term);
                            }
                        });
                        break;
                    }
                    else if (nodeRetrunData.nodeInfo[i].node.nodeId == $nodeId && nodeRetrunData.nodeInfo[i].nodeOperate.length <= 0) {
                        //无新添加信息 
                        var x = $(value).parents("ul").prev().find("span:eq(0)");
                        x.text($(value).text());
                        var term = $(value).attr("term");
                        x.attr("term", term);
                    }
                }
            }
        }
        else {
            var x = $(value).parents("ul").prev().find("span:eq(0)");
            x.text($(value).text());
            var term = $(value).attr("term");
            x.attr("term", term);
        }

    }
    function nodeTypeClassify() {
        console.log("nodeTypeClassify");
        if (nodeType == 1) {
            //节点类型为创建的场合，条件部分控件不能使用, 
            $(".NodeOperate-batchType").prop("disabled", true);
            $(".node-batchCondition").parent(".dropdown-toggle").addClass("disabled");
            $(".node-batchCondition").parent(".dropdown-toggle").addClass("noChoose");
            $("#node_addProposerDepart").addClass("noClick");
            $("#node_addProposerPost").addClass("noClick");
            $("#node_addProposer").addClass("noClick");
            //状态默认为无，不能使用   
            $("#node-countersign").attr("term", 0);
            $("#node-countersign").text("审批");
            $("#node-countersign").parent(".dropdown-toggle").addClass("disabled");
            $("#node-countersign").parent(".dropdown-toggle").addClass("noChoose");
            //无上级岗位
            $(".NodeOperate-type[term=4]").closest(".row").css("display", "none");
        }
        else if (nodeType == 2) {
            //节点类型为提交的场合 ,状态默认为提交，可选择提交或抄送
            $("#node-countersign").attr("term", 3);
            $("#node-countersign").text("提交");
            $("#node-countersign").closest(".dropdown").find("ul a").each(function () {
                if ($(this).attr("term") == 0 || $(this).attr("term") == 1 || $(this).attr("term") == 4) { $(this).parent("li").css("display", "none"); }
            })
            //所有人选项不可选
            $(".NodeOperate-type[term=5]").closest(".row").css("display", "none");
        }
        else if (nodeType == 3) {
            //节点类型为审批的场合 ,状态默认为审批，可选择审批、会签和抄送
            $("#node-countersign").attr("term", 0);
            $("#node-countersign").text("审批");
            $("#node-countersign").closest(".dropdown").find("ul a").each(function () {
                if ($(this).attr("term") == 3 || $(this).attr("term") == 4) { $(this).parent("li").css("display", "none"); }
            })
            //审批时默认为部门不可选
            $(".NodeOperate-type[term=1]").closest(".row").css("display", "none");
            $("#node_addOperatorDepart").addClass("noClick");
            //所有人选项不可选
            $(".NodeOperate-type[term=5]").closest(".row").css("display", "none");
        }
        else if (nodeType == 4) {
            //节点类型为归档的场合 ,状态默认为归档，可选择归档和抄送
            $("#node-countersign").attr("term", 4);
            $("#node-countersign").text("归档");
            $("#node-countersign").closest(".dropdown").find("ul a").each(function () {
                if ($(this).attr("term") == 0 || $(this).attr("term") == 1 || $(this).attr("term") == 3) { $(this).parent("li").css("display", "none"); }
            })
            //所有人选项不可选
            $(".NodeOperate-type[term=5]").closest(".row").css("display", "none");
        }
    }

    function nodeModefyOrNot() {
        //true xiugai
        if (nodeRetrunData.deleteNode.length != 0) {
           //console.log("nodeModefyOrNot：deleteNode true");
            return true;
        }
        if (nodeRetrunData.deleteOperateId.length != 0) {
          // console.log("nodeModefyOrNot：deleteOperateId true");
            return true;
        }
        if (nodeRetrunData.nodeInfo.length != nodeListLength) {
         //console.log("nodeModefyOrNot：nodeInfo true");
            return true;
        }
        if (nodeModefyFlag == 1) {
          //console.log("nodeModefyOrNot：nodeModefyFlag true");
            return true;
        }
        for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
            if (nodeRetrunData.nodeInfo[i].nodeField.length != 0) {
               //console.log("nodeModefyOrNot：nodeField true");
                return true;
            }
        }
        //console.log("nodeModefyOrNot：false");
        return false;
    }

    function nodeList() {
        nodeRetrunData.nodeInfo.length = 0;
        nodeRetrunData.deleteNode.length = 0;
        nodeRetrunData.deleteOperateId.length = 0;
        nodeModefyFlag = 0;
        nodeListLength = 0;
        var $nodeTable = $("#nodeTable");
        $nodeTable.empty();
        var nodeLoad = getLoadingPosition($nodeTable);     //显示load层
        var nodeTypeArray = new Array("", "创建", "提交", "审批", "归档");
        var $thead = $("<thead class='colorGray'><tr><th>节点名称</th><th>节点类型</th><th>字段权限</th><th>操作者</th><th></th></tr></thead>");
        var $tbody = $("<tbody></tbody>");
        $.ajax({
            type: "post",       
            url: "/NodeEdit/GetNodeInfoList",//GetNodeList
            dataType: "json",
            data: { templateId: templateId },
            success: rsHandler(function (data) {          
                $nodeTable.empty();
                if (data.length != 0) {
                    nodeListLength = data.length;
                    $.each(data, function (i, v) {                       
                        var nodeData = {
                            node: {
                                nodeId: 0,           //节点ID
                                templateId: 0,       //模板ID
                                nodeName: "",     //节点名
                                nodeType: 0  ,        //节点类型
                                operateEditFlag: false//节点操作人是否修改
                            },
                            nodeField: [],
                            nodeOperate: []
                        }
                        nodeData.node.nodeId = v.node.nodeId;
                        nodeData.node.nodeType = v.node.nodeType;
                        nodeData.node.nodeName = v.node.nodeName;
                        nodeData.node.templateId = templateId;
                        $.each(v.nodeOperate, function (flag, value) {
                            nodeData.nodeOperate.push(value);
                        })                       
                        nodeRetrunData.nodeInfo.push(nodeData);
                        if (v.node.nodeType == 4) {
                            var $tr = $("<tr class='nodeType4'></tr>");
                        }
                        else { var $tr = $("<tr ></tr>"); }
                        
                        //节点名称
                        var $tdName = $("<td style='width:20%'></td>");
                        var $nodeName = $("<input class='nodeName'term='" + v.node.nodeId + "' value='" + v.node.nodeName + "' maxlength='50'/>");
                        $nodeName.appendTo($tdName);

                        //节点类型
                        if (v.node.nodeType == 1 || v.node.nodeType == 4) {
                            var $tdType = $("<td style='width:10%'class='nodeTypeTd'><div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                              "<span class='nodeTypeSpan' term=" + v.node.nodeType + ">" + nodeTypeArray[v.node.nodeType] + "</span><span class='upPic'></span></span>" +
                              "</div></td>");

                        }
                        if (v.node.nodeType == 2 || v.node.nodeType == 3) {
                            var $tdType = $("<td style='width:10%'class='nodeTypeTd'><div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                               "<span class='nodeTypeSpan' term=" + v.node.nodeType + ">" + nodeTypeArray[v.node.nodeType] + "</span><span class='upPic'></span></span>" +
                               "<ul class='dropdown-menu changeAlert' role='menu' term='" + v.node.nodeId + "'><li><a href='javascript:void(0)' term='2' >提交</a></li><li><a href='javascript:void(0)' term='3'>审批</a></li></ul></div></td>");

                        }

                        //节点字段设置
                        var $tdFieldSet = $("<td style='width:10%'></td>");
                        var $FieldSet = $("<button class='btn btn-default widthParent' data-toggle='modal'   style='text-align: left' value='" + v.node.nodeId + "'>设置</button>");
                        $FieldSet.click(function () {
                            //点击字段设置，字段设置弹窗加载字段信息
                            nodeId = v.node.nodeId;
                            var loadFlag = 0;
                            $("#nodeFieldSet_modal").modal("show");
                            for (var j = 0; j < nodeRetrunData.nodeInfo.length; j++) {
                                if (nodeRetrunData.nodeInfo[j].node.nodeId == $(this).val()) {
                                    if (nodeRetrunData.nodeInfo[j].nodeField.length != 0) {
                                        nodeFieldChangedLoad(nodeRetrunData.nodeInfo[j].nodeField);
                                        loadFlag = 1;
                                    }
                                    break;
                                }
                            }
                            if (loadFlag == 0) { nodeFieldLoad(v.node.nodeId); }
                            
                        });
                        $tdFieldSet.append($FieldSet);

                        //操作者设置
                        var $tdOperateSet = $("<td style='width:10%'></td>");
                        var $OperateSet = $("<button class='btn btn-default widthParent' data-toggle='modal'  style='text-align: left' value=''>设置</button>");
                        $OperateSet.click(function () {
                            //TODO
                            nodeType = $(this).parent("td").siblings(".nodeTypeTd").find(".nodeTypeSpan").attr("term");

                            if (nodeType == "") {
                                ncUnits.alert("没有选择节点类型"); return;
                            }
                            else {
                                $("#nodeOperateSet_modal").modal('show');
                                nodeOperateModalClear();
                                $(".Operate-nodeName").text($(this).closest("tr").find(".nodeName").val());//v.nodeName
                                $(".Operate-nodeName").attr("title", $(this).closest("tr").find(".nodeName").val());
                                nodeId = v.node.nodeId;
                                for (var j = 0; j < nodeRetrunData.nodeInfo.length; j++) {
                                    if (nodeRetrunData.nodeInfo[j].node.nodeId == v.node.nodeId) {
                                        if (nodeRetrunData.nodeInfo[j].nodeOperate.length != 0) {
                                            nodeOperateAddConditionTableLoad(nodeRetrunData.nodeInfo[j].nodeOperate);
                                            break;
                                        }
                                    }
                                }
                            }
                            nodeTypeClassify();
                        });
                        $tdOperateSet.append($OperateSet);
                        //删除
                        var $tdDel = $("<td style='width:25%'></td>");
                        var $spanDel = $("<span class='delPic' term=" + v.node.nodeId + "></span>");

                        $spanDel.click(function () {
                            $this = $(this);
                            ncUnits.confirm({
                                title: '提示',
                                html: '确认要删除吗？',
                                yes: function (layer_confirm) {
                                    $.ajax({
                                        type: "post",
                                        url: " /NodeEdit/CheckDeleteFlag",
                                        dataType: "json",
                                        data: { nodeId: v.node.nodeId },
                                        success: rsHandler(function (deleteFlag) {
                                            if (deleteFlag == true) {

                                                nodeRetrunData.deleteNode[nodeRetrunData.deleteNode.length] = v.node.nodeId;
                                                for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
                                                    if (nodeRetrunData.nodeInfo[i].node.nodeId == $this.attr("term")) {
                                                        $($this).parents("tr").remove();
                                                        break;
                                                    }
                                                }
                                                nodeRetrunData.nodeInfo.splice(i, 1);
                                            }
                                            else {
                                                ncUnits.alert("该节点在流程设置中已经进行相关设置，不能删除");
                                            }

                                        })
                                    })
                                    layer.close(layer_confirm);

                                }
                            });
                        });
                        if (v.node.nodeType == 2 || v.node.nodeType == 3) {
                            $tdDel.append($spanDel);
                        }
                        $tr.append([$tdName, $tdType, $tdFieldSet, $tdOperateSet, $tdDel]);
                        $tbody.append($tr);
                    });
                }
                else {
                    for (var i = 1; i <= 2; i++) {
                        var $nodeData = {
                            node: {
                                nodeId: 0,           //节点ID
                                templateId: 0,       //模板ID
                                nodeName: "",     //节点名
                                nodeType: 0 ,         //节点类型
                                operateEditFlag: true//节点操作人是否修改
                            },
                            nodeField: [],
                            nodeOperate: []
                        }
                        //修改$nodeData.node.nodeId = 'z' + new Date().getTime();

                        $nodeData.node.nodeId = 'z' + i;
                        $nodeData.node.templateId = templateId;
                        nodeRetrunData.nodeInfo.push($nodeData);
                        if (i == 1) {
                            $nodeData.node.nodeType = 1;
                            var $tr = $("<tr ></tr>");
                            var $tdType = $("<td style='width:10%' class='nodeTypeTd'><div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                             "<span class='nodeTypeSpan'  term='1'>创建</span><span class='upPic'></span></span>" +
                             "</div></td>");
                        }
                        else if (i == 2) {
                            $nodeData.node.nodeType = 4;
                            var $tr = $("<tr class='nodeType4'></tr>");
                            var $tdType = $("<td style='width:10%' class='nodeTypeTd'><div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                             "<span class='nodeTypeSpan'  term='4'>归档</span><span class='upPic'></span></span>" +
                             "</div></td>");

                        }


                        //节点名称
                        var $tdName = $("<td style='width:20%'></td>");
                        var $nodeName = $(" <input class='nodeName'term='" + $nodeData.node.nodeId + "' value='' maxlength='50'/>");
                        $nodeName.appendTo($tdName);
                        //节点类型


                        //节点字段设置
                        var $tdFieldSet = $("<td style='width:10%'></td>");
                        var $FieldSet = $("<button class='btn btn-default widthParent' data-toggle='modal'  style='text-align: left' value='" + $nodeData.node.nodeId + "'>设置</button>");

                        $FieldSet.click(function () {
                            //点击字段设置，字段设置弹窗加载字段信息
                            nodeType = $(this).parent("td").siblings(".nodeTypeTd").find(".nodeTypeSpan").attr("term");                          
                            nodeId = $(this).val();
                            var loadFlag = 0;
                            for (var j = 0; j < nodeRetrunData.nodeInfo.length; j++) {
                                if (nodeRetrunData.nodeInfo[j].node.nodeId == $(this).val()) {
                                    if (nodeRetrunData.nodeInfo[j].nodeField.length != 0) {
                                        nodeFieldChangedLoad(nodeRetrunData.nodeInfo[j].nodeField);
                                        loadFlag = 1;
                                    }
                                    break;
                                }
                            }
                            if (loadFlag == 0) { nodeFieldLoad(null, nodeType); }
                            $("#nodeFieldSet_modal").modal("show");
                        });
                        $tdFieldSet.append($FieldSet);
                        //操作者设置
                        var $tdOperateSet = $("<td style='width:10%'></td>");
                        var $OperateSet = $("<button class='btn btn-default widthParent' data-toggle='modal'   style='text-align: left' value='" + $nodeData.node.nodeId + "'>设置</button>");
                        $OperateSet.click(function () {
                            //TODO
                            nodeType = $(this).parent("td").siblings(".nodeTypeTd").find(".nodeTypeSpan").attr("term");
                            if (nodeType == "") {
                                ncUnits.alert("没有选择节点类型"); return;
                            }
                            else {
                                $("#nodeOperateSet_modal").modal('show');
                                nodeOperateModalClear();
                                $(".Operate-nodeName").text($(this).closest("tr").find(".nodeName").val());
                                $(".Operate-nodeName").attr("title", $(this).closest("tr").find(".nodeName").val());
                                nodeId = $(this).val();
                                for (var j = 0; j < nodeRetrunData.nodeInfo.length; j++) {
                                    if (nodeRetrunData.nodeInfo[j].node.nodeId == $(this).val()) {
                                        if (nodeRetrunData.nodeInfo[j].nodeOperate.length != 0) {
                                            $("#nodeOperateConditionTable tbody").empty();//清空
                                            nodeOperateAddConditionTableLoad(nodeRetrunData.nodeInfo[j].nodeOperate);
                                            break;
                                        }
                                    }
                                }
                            }
                            nodeTypeClassify();

                        });
                        $tdOperateSet.append($OperateSet);
                        //删除
                        var $tdDel = $("<td style='width:25%'></td>");
                        $tr.append([$tdName, $tdType, $tdFieldSet, $tdOperateSet, $tdDel]);
                        $tbody.append($tr);
                    }

                }

                $nodeTable.append([$thead, $tbody]);
                //为每一个dropdown注册下拉事件
                $("#nodeTable .dropdown ul a").off("click");
                $("#nodeTable .dropdown ul a").click(function () {
                    nodeDropDownEvent(this);

                });
                $('#nodeTable .nodeName').bind('input propertychange', function () {
                    nodeModefyFlag = 1;
                });
            }),
            complete: rcHandler(function () {             
                nodeLoad.remove();         //关闭load层                
            })
            //rcHandler()
       
        });

        //重新注册添加事件
        $("#addNode").off("click");
        $("#addNode").click(function () {
            if ($("#con_set_node").hasClass("active")) {
                var $nodeData = {
                    node: {
                        nodeId: 0,           //节点ID
                        templateId: 0,       //模板ID
                        nodeName: "",     //节点名
                        nodeType: 0 ,         //节点类型
                        operateEditFlag: true//节点操作人是否修改
                    },
                    nodeField: [],
                    nodeOperate: []
                }
                $nodeData.node.nodeId = 'z' + new Date().getTime();
                $nodeData.node.templateId = templateId;
                nodeRetrunData.nodeInfo.push($nodeData);
                var $tr = $("<tr></tr>");
                //节点名称

                var $tdName = $("<td style='width:20%'></td>");
                var $nodeName = $(" <input class='nodeName'term='" + $nodeData.node.nodeId + "' value='' maxlength='50'/>");
                $nodeName.appendTo($tdName);
                //节点类型
                var $tdType = $("<td style='width:10%' class='nodeTypeTd'><div class='dropdown'><span  class='dropdown-toggle' data-toggle='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                    "<span class='nodeTypeSpan'  term=''></span><span class='upPic'></span></span>" +
                    "<ul class='dropdown-menu changeAlert' role='menu'term='" + $nodeData.node.nodeId + "' ><li><a href='javascript:void(0)' term='2' >提交</a></li><li><a href='javascript:void(0)' term='3'>审批</a></li></ul></div></td>");

                //节点字段设置
                var $tdFieldSet = $("<td style='width:10%'></td>");
                var $FieldSet = $("<button class='btn btn-default widthParent' data-toggle='modal'   style='text-align: left' value='" + $nodeData.node.nodeId + "'>设置</button>");

                $FieldSet.click(function () {
                    //点击字段设置，字段设置弹窗加载字段信息
                    nodeId = $(this).val();
                    var loadFlag = 0;
                    for (var j = 0; j < nodeRetrunData.nodeInfo.length; j++) {
                        if (nodeRetrunData.nodeInfo[j].node.nodeId == $(this).val()) {
                            if (nodeRetrunData.nodeInfo[j].nodeField.length != 0) {
                                nodeFieldChangedLoad(nodeRetrunData.nodeInfo[j].nodeField);
                                loadFlag = 1;
                            }
                            break;
                        }
                    }
                    if (loadFlag == 0) { nodeFieldLoad(null); }

                    $("#nodeFieldSet_modal").modal("show");
                });
                $tdFieldSet.append($FieldSet);

                //操作者设置
                var $tdOperateSet = $("<td style='width:10%'></td>");
                var $OperateSet = $("<button class='btn btn-default widthParent' data-toggle='modal'   style='text-align: left' value='" + $nodeData.node.nodeId + "'>设置</button>");
                $OperateSet.click(function () {
                    //TODO
                    nodeType = $(this).parent("td").siblings(".nodeTypeTd").find(".nodeTypeSpan").attr("term");

                    if (nodeType == "") {
                        ncUnits.alert("没有选择节点类型"); return;
                    }
                    else {
                        $("#nodeOperateSet_modal").modal('show');
                        nodeOperateModalClear();
                        $(".Operate-nodeName").text($(this).closest("tr").find(".nodeName").val());
                        $(".Operate-nodeName").attr("title",$(this).closest("tr").find(".nodeName").val());
                        nodeId = $(this).val();
                        for (var j = 0; j < nodeRetrunData.nodeInfo.length; j++) {
                            if (nodeRetrunData.nodeInfo[j].node.nodeId == $(this).val()) {
                                if (nodeRetrunData.nodeInfo[j].nodeOperate.length != 0) {
                                    $("#nodeOperateConditionTable tbody").empty();//清空
                                    nodeOperateAddConditionTableLoad(nodeRetrunData.nodeInfo[j].nodeOperate);
                                    break;
                                }
                            }
                        }
                    }
                    nodeTypeClassify();
                    

                });
                $tdOperateSet.append($OperateSet);

                //删除
                var $tdDel = $("<td style='width:25%'></td>");
                var $spanDel = $("<span class='delPic' term='" + $nodeData.node.nodeId + "'></span>");
                $spanDel.click(function () {
                    var $this = $(this);
                    ncUnits.confirm({
                        title: '提示',
                        html: '确认要删除吗？',
                        yes: function (layer_confirm) {
                            layer.close(layer_confirm);
                            for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
                                if (nodeRetrunData.nodeInfo[i].node.nodeId == $this.attr("term")) {
                                    $this.parents("tr").remove();
                                    break;
                                }
                            }
                            nodeRetrunData.nodeInfo.splice(i, 1);
                        }
                    });
                });
                $tdDel.append($spanDel);
                $tr.append([$tdName, $tdType, $tdFieldSet, $tdOperateSet, $tdDel]);
                $Flag = $("#nodeTable .nodeType4");

                $tr.insertBefore($Flag);
                $("#nodeTable .dropdown ul a").off("click");
                    
                $("#nodeTable .dropdown ul a").click(function () {
                    nodeDropDownEvent(this);
                });
                 
                $('#nodeTable .nodeName').bind('input propertychange', function () {
                    nodeModefyFlag = 1;
                });
            }
        });
    }



    function datasave(flag) {
        var nameFlag = 0;      
        $("#nodeTable tr .nodeName").each(function () {
            for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
                if (nodeRetrunData.nodeInfo[i].node.nodeId == $(this).attr("term")) {                  
                    var name = $.trim($(this).val());
                    if (name == "") {
                        ncUnits.alert("节点名称不能为空!");
                        nameFlag = 1;
                        return false;
                    }
                    else if (name.length > 50) {
                        ncUnits.alert("节点名称不能超过50字符!");
                        nameFlag = 1;
                        return false;
                    }
                    var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
                    if (name.indexOf('null') >= 0 || name.indexOf('NULL') >= 0 || name.indexOf('&nbsp') >= 0 || reg.test(name) || name.indexOf('</') >= 0) {
                        ncUnits.alert("节点名称存在非法字符!");
                        nameFlag = 1;
                        return false;
                    }
                    
                    if ($(this).parent("td").siblings(".nodeTypeTd").find(".nodeTypeSpan").attr("term") == "") {
                        ncUnits.alert("节点类型没有选择!");
                        nameFlag = 1;
                        return false;
                    }
                    nodeRetrunData.nodeInfo[i].node.nodeName = $(this).val();
                    nodeRetrunData.nodeInfo[i].node.nodeType = $(this).parent("td").siblings(".nodeTypeTd").find(".nodeTypeSpan").attr("term");

                }
            }

        })
        if (nameFlag == 1) { return; }
        for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
            for (var j = i+1; j < nodeRetrunData.nodeInfo.length; j++) {
                if (nodeRetrunData.nodeInfo[j].node.nodeName == nodeRetrunData.nodeInfo[i].node.nodeName) {
                    ncUnits.alert("节点名称不能重复!");       
                    return ;
                }
            }
        }
        //console.log("记录过名字的数据");
        //console.log(nodeRetrunData);
        
        for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {       
            if (nodeRetrunData.nodeInfo[i].nodeOperate.length <= 0) {
                ncUnits.alert("请给每一个节点都设置节点操作人");
              //  console.log("-----------------------------------------------------------------------------请给每一个节点都设置节点操作人");
                return;
            }
        }
        //克隆一个数组进行返回，以防止保存不正确
        nodeAjaxRetrunData.nodeInfo.length = 0;
        nodeAjaxRetrunData.deleteOperateId = 0;
        nodeAjaxRetrunData.deleteNode = 0;
        for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
            var $nodeData = {
                node: {
                   nodeId: 0,           //节点ID
                   templateId: 0,       //模板ID
                   nodeName: "",     //节点名
                   nodeType: 0,         //节点类型
                   operateEditFlag:false
               },
               nodeField: [],
               nodeOperate: []
            }
            $nodeData.node = $.extend(true, {}, nodeRetrunData.nodeInfo[i].node);
            for (var j = 0; j < nodeRetrunData.nodeInfo[i].nodeField.length; j++) {
                $nodeData.nodeField.push($.extend(true, {}, nodeRetrunData.nodeInfo[i].nodeField[j]));
            }
            for (var j = 0; j < nodeRetrunData.nodeInfo[i].nodeOperate.length; j++) {
                $nodeData.nodeOperate.push($.extend(true, {}, nodeRetrunData.nodeInfo[i].nodeOperate[j]));
            }           
            nodeAjaxRetrunData.nodeInfo.push($nodeData);
        }
        nodeAjaxRetrunData.deleteNode= nodeRetrunData.deleteNode.slice(0);
        nodeAjaxRetrunData.deleteOperateId=nodeRetrunData.deleteOperateId.slice(0);
        //console.log("删除id之前返回的数据nodeRetrunData");
        //console.log(nodeRetrunData);
       
        //console.log("删除id之前返回的数据nodeAjaxRetrunData");
        //console.log(nodeAjaxRetrunData);
        for (var i = 0; i < nodeAjaxRetrunData.nodeInfo.length; i++) {
            if (nodeAjaxRetrunData.nodeInfo[i].node.nodeId.toString().substr(0, 1) == "z") {
                nodeAjaxRetrunData.nodeInfo[i].node.nodeId = null;
            }
            for (var j = 0; j < nodeAjaxRetrunData.nodeInfo[i].nodeField.length; j++) {
                if (nodeAjaxRetrunData.nodeInfo[i].nodeField[j].nodeId.toString().substr(0, 1) == "z") {
                    nodeAjaxRetrunData.nodeInfo[i].nodeField[j].nodeId = null;
                }

            }
            for (var j = 0; j < nodeAjaxRetrunData.nodeInfo[i].nodeOperate.length; j++) {
                if (nodeAjaxRetrunData.nodeInfo[i].nodeOperate[j].nodeId.toString().substr(0, 1) == "z") {
                    nodeAjaxRetrunData.nodeInfo[i].nodeOperate[j].nodeId = null;
                }
                if (nodeAjaxRetrunData.nodeInfo[i].nodeOperate[j].operateId.toString().substr(0, 1) == "r") {
                    nodeAjaxRetrunData.nodeInfo[i].nodeOperate[j].operateId = null;
                }
            }

        }
        //console.log("最后返回的数据nodeRetrunData");
        //console.log(nodeRetrunData);
        //console.log("最后返回的数据nodeAjaxRetrunData");
        //console.log(nodeAjaxRetrunData);
        //返回数据
        $.ajax({
            type: "post",
            url: "/NodeEdit/SaveFlowNode ",
            dataType: "json",
            data: { data: JSON.stringify(nodeAjaxRetrunData), templateId: templateId },
            success: rsHandler(function (data) {
                //跳转到XX页面
                flowTab=2;
                ncUnits.alert("节点设置保存成功");
                if (flag == 1) {
                    nodeList();
                }
                else {              //跳转到表单管理界面
                    //TODO
                    systemOrFalse = systemFalse;
                  loadViewToMain("/TemplateCategory/TemplateCategory" );
                }
            })
        });
    }

    //数据保存
    $("#nodeAllSave").click(function () {
        datasave(1);
    })
    //数据保存且返回
    $("#nodeAllSaveReturn").click(function () {
        datasave(2);
    })
    //操作人设置保存
    $("#nodeOperateSet_save").click(function () {
        nodeModefyFlag = 1;//标记操作人设置已被修改
        var yesOrNot = 0;       
        //提交节点
        if (nodeType == 2 || nodeType == 3 || nodeType == 4) {
            for (var j = 0; j < nodeOperateArray.length; j++) {
                if (nodeOperateArray[j].countersign == 3 && nodeType == 2) {
                    yesOrNot = 1;
                    break;
                }
                else if (nodeType == 3 && (nodeOperateArray[j].countersign == 0 || nodeOperateArray[j].countersign == 1)) {
                    yesOrNot = 1;
                    break;
                }
                else if (nodeOperateArray[j].countersign == 4 && nodeType == 4) {
                    yesOrNot = 1;
                    break;
                }
            }
        }
                        
        if (nodeType == 2 && yesOrNot == 0) {
            ncUnits.alert("提交类型节点必须至少设置一条操作类型为提交的记录");
            return;
        }
        else if (nodeType == 3 && yesOrNot == 0) {
            ncUnits.alert("审批类型节点必须至少设置一条操作类型为会签或审批的记录");
            return;
        }
        else if (nodeType == 4 && yesOrNot == 0) {
            ncUnits.alert("归档类型节点必须至少设置一条操作类型为归档的记录");
            return;
        }

        var temp;
        for (var j = 0; j < nodeOperateArray.length; j++) {
            for (i = 0; i < nodeOperateArray.length-j-1; i++) {
                if (nodeOperateArray[i].orderNum > nodeOperateArray[i+1].orderNum) {
                    temp = nodeOperateArray[i];
                    nodeOperateArray[i] = nodeOperateArray[i + 1];
                    nodeOperateArray[i + 1] = temp;
                }
            }
        }
       
        for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
            if (nodeRetrunData.nodeInfo[i].node.nodeId == nodeId) {
                nodeRetrunData.nodeInfo[i].node.operateEditFlag = true;
                nodeRetrunData.nodeInfo[i].nodeOperate.length = 0;
                //把里面临时列表数组里面的数据添加进去
                for (var j = 0; j < nodeOperateArray.length; j++) {                 
                    nodeRetrunData.nodeInfo[i].nodeOperate.push(nodeOperateArray[j]);                    
                }
                break;
            }
        }
        nodeOperateArray.length = 0;
        delOperateIdArray.length = 0;
        $("#nodeOperateSet_modal").modal("hide");
    });
    //操作人设置取消
    $("#nodeOperateSet_cancle").click(function () {
        nodeOperateArray.length = 0;
        delOperateIdArray.length = 0;
        $("#nodeOperateSet_modal").modal("hide");
    })

    //节点字段设置保存
    $("#nodeFieldSet_save").click(function () {
        for (var i = 0; i < nodeRetrunData.nodeInfo.length; i++) {
            if (nodeRetrunData.nodeInfo[i].node.nodeId == nodeId) {
                break;
            }
        }
        nodeRetrunData.nodeInfo[i].nodeField.length = 0;
        $("#nodeFieldSet_modal_content .nodeFieldDiv .row").each(function () {
            var flagchecked = 0;
            var $nodeField = {    //节点字段
                nodeId: 0,         //节点ID
                controlId: 0,      //控件ID
                controlTitle: "",//控件title
                controlType:null,
                isDetail:null,//
                parentControl:null, 
                status: 0          //字段状态
            }

            $nodeField.nodeId = nodeId;
            if ($(this).attr("term") != "") {
                $nodeField.parentControl = $(this).attr("term");
            }
            else {
                $nodeField.parentControl = null;
            }
            if ($(this).parent(".nodeFieldDiv").hasClass("detail")) {
                $nodeField.isDetail = true;               
            }
            else { $nodeField.isDetail = false; }
            if ($(this).hasClass("parentControl-row")) {
                $nodeField.controlType = $(this).attr("value");
            }
            $nodeField.controlTitle = $(this).find(".controlTitle").text();
            if ($(this).find("input:eq(0)").is(":checked")) {
                $nodeField.controlId = $(this).find("input:eq(0)").attr("term");
                $nodeField.status = 0;
                nodeRetrunData.nodeInfo[i].nodeField.push($nodeField);
            }
            else if ($(this).find("input:eq(1)").is(":checked")) {
                $nodeField.controlId = $(this).find("input:eq(1)").attr("term");
                $nodeField.status = 1;
                nodeRetrunData.nodeInfo[i].nodeField.push($nodeField);
            }
            else if ($(this).find("input:eq(2)").is(":checked")) {
                $nodeField.controlId = $(this).find("input:eq(2)").attr("term");
                $nodeField.status = 2;
                nodeRetrunData.nodeInfo[i].nodeField.push($nodeField);
            }
           
        });
        $("#nodeFieldSet_modal").modal("hide");
    });


    function nodeFieldLoad(nodeId, nodeType) {
        var $nodeFieldSet = $("#nodeFieldSet_modal_content");
        $nodeFieldSet.empty();
        var nodeFieldSetLoad = getLoadingPosition($nodeFieldSet);     //显示load层
        var fieldStatusArray = new Array("隐藏", "只读", "编辑");
        var $Head = $("<div  class='row'><div class='col-xs-3'><label>字段</label></div><div class='col-xs-3'><label>隐藏</label></div><div class='col-xs-3'> <label>只读</label>  </div><div class='col-xs-3'>  <label>编辑</label>  </div> </div>  <hr class='hrAll' style='width:560px;'>");
        var $nodeFiledMain = $("<div  class='row'><div class='col-xs-12'> <label class='colorGray'>主表</label></div>  </div> <hr style='margin-top: 5px;'/>");
        var $nodeFiledDetail = $("<div  class='row'><div class='col-xs-12'> <label class='colorGray'>明细</label></div>  </div> <hr/>");
        var $miandiv = $("<div class='nodeFieldDiv'></div>");
        var $detaildiv = $("<div class='nodeFieldDiv detail'id='nodeFieldDiv-detail'></div>");
        $.ajax({
            type: "post",
            url: "/NodeEdit/GetNodeFieldList",
            dataType: "json",
            data: { templateId: templateId, nodeId: nodeId },
            success: rsHandler(function (data) {
                var dataCopy = data;
                $nodeFieldSet.append($Head, $nodeFiledMain);
                $.each(data, function (i, v) {
                    if (v.controlType == 16 || v.controlType == 18 || v.controlType == 19) {
                        var $nodeFiled = $("<div  class='row parentControl-row' value='" + v.controlType + "' term=''></div> ");
                    }
                    else if (v.isDetail || v.parentControl != "main") {
                        var $nodeFiled = $("<div  class='row'term='" + v.parentControl + "'></div> ");
                    }
                    else {
                        var $nodeFiled = $("<div  class='row' term=''></div> ");
                    }
                    var $nodeFiledName = $("<div class='col-xs-3'><label class='controlTitle' title='" + v.controlTitle + "' term='" + v.controlId + "'>" + v.controlTitle + "</label> </div>");
                    var $nodeFiledradio0 = $("<div class='col-xs-3'><input type='radio' name='" + v.controlId + "' class='" + v.controlId + "' term='" + v.controlId + "'></div>");
                    var $nodeFiledradio1 = $("<div class='col-xs-3'> <input type='radio' name='" + v.controlId + "' class='" + v.controlId + "' term='" + v.controlId + "'></div>");
                    var $nodeFiledradio2 = $("<div class='col-xs-3'> <input type='radio' name='" + v.controlId + "' class='" + v.controlId + "' term='" + v.controlId + "'></div> ");

                    $nodeFiled.append($nodeFiledName, $nodeFiledradio0, $nodeFiledradio1, $nodeFiledradio2);
                    if (v.isDetail) { $detaildiv.append($nodeFiled); }
                    else { $miandiv.append($nodeFiled); }
                });

                $nodeFieldSet.append($miandiv, $nodeFiledDetail, $detaildiv);
              
                $(".parentControl-row input").click(function () {
                    var parentControl = $(this).attr("term");
                    var checkedIndex=null;
                    if ($(this).closest(".parentControl-row").find("input:eq(0)").prop("checked")) {
                        checkedIndex=0;
                    }
                    else if ($(this).closest(".parentControl-row").find("input:eq(1)").prop("checked")) {
                        checkedIndex=1;
                    }
                    else if ($(this).closest(".parentControl-row").find("input:eq(2)").prop("checked")) {
                        checkedIndex=2;
                    };
                    //#nodeFieldDiv-detail
                    
                    $(".nodeFieldDiv .row[term='" + parentControl + "']").each(function () {
                         if (checkedIndex == 0) {
                             $(this).find("input:eq(0)").prop("disabled", false);
                             $(this).find("input:eq(1)").prop("disabled", true);
                             $(this).find("input:eq(2)").prop("disabled", true);
                             $(this).find("input:eq(0)").prop("checked", true);
                         }
                         else if (checkedIndex == 1) {
                             $(this).find("input:eq(0)").prop("disabled", false);
                             $(this).find("input:eq(1)").prop("disabled", false);
                             $(this).find("input:eq(2)").prop("disabled", true);
                             $(this).find("input:eq(1)").prop("checked", true);
                             
                         }
                         else if (checkedIndex == 2) {
                             $(this).find("input:eq(0)").prop("disabled", false);
                             $(this).find("input:eq(1)").prop("disabled", false);
                             $(this).find("input:eq(2)").prop("disabled", false);
                             $(this).find("input:eq(2)").prop("checked", true);
                         }

                     })
                })
                $.each(data, function (i, v) {
                    var ziduan = "." + v.controlId + ":eq(" + v.status + ")";
                    $(ziduan).prop("checked", true);
                    if (v.controlType == 16 || v.controlType == 18 || v.controlType == 19) {
                        $(ziduan).trigger("click");
                    }
                });
                //if (nodeId==null&&nodeType==1) {
                //    $.each(data, function (i, v) {
                //        var ziduan = "." + v.controlId + ":eq(2)";
                //        $(ziduan).prop("checked", true);
                //        if (v.controlType == 16) {
                //            $(ziduan).trigger("click");
                //        }
                //    });
                //}
                //else {
                //    $.each(data, function (i, v) {
                //        var ziduan = "." + v.controlId + ":eq(" + v.status + ")";
                //        $(ziduan).prop("checked", true);
                //        if (v.controlType == 16) {
                //            $(ziduan).trigger("click");
                //        }
                //    });
                //}
               
            }),
            complete: rcHandler(function () {             
                nodeFieldSetLoad.remove();         //关闭load层
            })
        });

    }

    function nodeFieldChangedLoad(data) {
        var $nodeFieldSet = $("#nodeFieldSet_modal_content");
        $nodeFieldSet.empty();
        var fieldStatusArray = new Array("隐藏", "只读", "编辑");
        var $Head = $("<div  class='row'><div class='col-xs-3'><label>字段</label></div><div class='col-xs-3'><label>隐藏</label></div><div class='col-xs-3'> <label>只读</label>  </div><div class='col-xs-3'>  <label>编辑</label>  </div> </div>  <hr class='hrAll'>");
        var $nodeFiledMain = $("<div  class='row'><div class='col-xs-12'> <label class='colorGray'>主表</label></div>  </div> <hr style='margin-top: 5px;'/>");
        var $nodeFiledDetail = $("<div  class='row'><div class='col-xs-12'> <label class='colorGray'>明细</label></div>  </div> <hr/>");
        var $miandiv = $("<div class='nodeFieldDiv'></div>");
        var $detaildiv = $("<div class='nodeFieldDiv detail'id='nodeFieldDiv-detail'></div>");
        $nodeFieldSet.append($Head, $nodeFiledMain);
        $.each(data, function (i, v) {
            if (v.controlType == 16 || v.controlType == 18 || v.controlType == 19) {
                var $nodeFiled = $("<div  class='row parentControl-row' value='" + v.controlType + "' term=''></div> ");
            }
            else if (v.isDetail || v.parentControl != null) {
                var $nodeFiled = $("<div  class='row'term='" + v.parentControl + "'></div> ");
            }
            else {
                var $nodeFiled = $("<div  class='row' term=''></div> ");
            }
            var $nodeFiledName = $("<div class='col-xs-3'><label class='controlTitle'  title='" + v.controlTitle + "'term='" + v.parentControl + "'>" + v.controlTitle + "</label> </div>");
            var $nodeFiledradio0 = $("<div class='col-xs-3'><input type='radio' name='" + v.controlId + "' class='" + v.controlId + "' term='" + v.controlId + "'></div>");
            var $nodeFiledradio1 = $("<div class='col-xs-3'> <input type='radio' name='" + v.controlId + "' class='" + v.controlId + "' term='" + v.controlId + "'></div>");
            var $nodeFiledradio2 = $("<div class='col-xs-3'> <input type='radio' name='" + v.controlId + "' class='" + v.controlId + "' term='" + v.controlId + "'></div> ");

            $nodeFiled.append($nodeFiledName, $nodeFiledradio0, $nodeFiledradio1, $nodeFiledradio2);
            if (v.isDetail) { $detaildiv.append($nodeFiled); }
            else { $miandiv.append($nodeFiled); }
        });

        $nodeFieldSet.append($miandiv, $nodeFiledDetail, $detaildiv);
      
        $(".parentControl-row input").click(function () {
            var parentControl = $(this).attr("term");
            var checkedIndex = null;
            if ($(this).closest(".parentControl-row").find("input:eq(0)").prop("checked")) {
                checkedIndex = 0;
            }
            else if ($(this).closest(".parentControl-row").find("input:eq(1)").prop("checked")) {
                checkedIndex = 1;
            }
            else if ($(this).closest(".parentControl-row").find("input:eq(2)").prop("checked")) {
                checkedIndex = 2;
            };
            $(".nodeFieldDiv .row[term='" + parentControl + "']").each(function () {
                if (checkedIndex == 0) {
                    $(this).find("input:eq(0)").prop("disabled", false);
                    $(this).find("input:eq(1)").prop("disabled", true);
                    $(this).find("input:eq(2)").prop("disabled", true);
                    $(this).find("input:eq(0)").prop("checked", true);
                }
                else if (checkedIndex == 1) {
                    $(this).find("input:eq(0)").prop("disabled", false);
                    $(this).find("input:eq(1)").prop("disabled", false);
                    $(this).find("input:eq(2)").prop("disabled", true);
                    $(this).find("input:eq(1)").prop("checked", true);

                }
                else if (checkedIndex == 2) {
                    $(this).find("input:eq(0)").prop("disabled", false);
                    $(this).find("input:eq(1)").prop("disabled", false);
                    $(this).find("input:eq(2)").prop("disabled", false);
                    $(this).find("input:eq(2)").prop("checked", true);
                }

            })
        })
        $.each(data, function (i, v) {
            var ziduan = "." + v.controlId + ":eq(" + v.status + ")";
            $(ziduan).prop("checked", true);
            if (v.controlType == 16 || v.controlType == 18 || v.controlType == 19) {
                $(ziduan).trigger("click");
            }
        });
    }



    var nodeOperateType = new Array("", "操作者部门", "操作者岗位", "操作者", "上级岗位", "所有人");
    var nodeOperateBatchType = new Array("", "申请人部门", "申请人岗位", "申请人");
    var countersignArray = new Array("审批", "会签", "抄送", "提交", "归档");
   
    function nodeOperateAddConditionTableLoad(data) {
        var $tbody = $("#nodeOperateConditionTable tbody");
        $.each(data, function (i, v) {
            nodeOperateArray.push(v);      
            var $tr = $("<tr term='" + v.operateId + "'></tr>");
            var $OrderNumTd = $("<td class='OrderNumTd' style='width:8%'>" + v.orderNum + "</td>");
            var $delTd = $("<td style='width:17%'></td>");
            var $delSpan = $("<span  class='delPic'></span>");
            var $upSpan = $("<span  class='order-upPic'></span>");
            var $downSpan = $("<span  class='order-downPic'></span>");
            var $td;
            
            if ((v.type == 4 || v.type == 5) && (v.batchType == 1 || v.batchType == 2 || v.batchType == 3)) {
                $td = $("<td style='width:30%' class='nodeOperateType'><span title='" + nodeOperateType[v.type] + "'>" + nodeOperateType[v.type] + "</span></td>"
            + "<td style='width:30%' class='nodeOperateBatchType'><span title='" + nodeOperateBatchType[v.batchType] + (v.batchCondition == 1 ? "属于" : "不属于") + v.batchTargetName.join(",") + "'>" + nodeOperateBatchType[v.batchType] + (v.batchCondition == 1 ? "属于" : "不属于") + v.batchTargetName.join(",") + "</span></td>"
            + "<td style='width:15%' class='nodeCountersign'><span>" + countersignArray[v.countersign] + "</span></td>")

            }
            else if ((v.type == 4 || v.type == 5) && v.batchType !== 1 && v.batchType !== 2 && v.batchType !== 3) {
                $td = $("<td style='width:30%' class='nodeOperateType'><span title='" + nodeOperateType[v.type] + "'>" + nodeOperateType[v.type] + "</span></td>"
          + "<td style='width:30%'class='nodeOperateBatchType'><span title='" + "无" + "'>" + "无" + "</span></td>"
          + "<td style='width:15%' class='nodeCountersign'><span>" + countersignArray[v.countersign] + "</span></td>")

            }
            else if (v.batchType !== 1 && v.batchType !== 2 && v.batchType !== 3) {
                $td = $("<td style='width:30%' class='nodeOperateType'><span title='" + nodeOperateType[v.type] + (v.condition == 1 ? "属于" : "不属于") + v.targetName.join(",") + "'>" + nodeOperateType[v.type] + (v.condition == 1 ? "属于" : "不属于") + v.targetName.join(",") + "</span></td>"
             + "<td style='width:30%'class='nodeOperateBatchType'><span title='" + "无" + "'>" + "无" + "</span></td>"
             + "<td style='width:15%' class='nodeCountersign'><span>" + countersignArray[v.countersign] + "</span></td>")

            }
            else {
                $td = $("<td style='width:30%' class='nodeOperateType'><span title='" + nodeOperateType[v.type] + (v.condition == 1 ? "属于" : "不属于") + v.targetName.join(",") + "'>" + nodeOperateType[v.type] + (v.condition == 1 ? "属于" : "不属于") + v.targetName.join(",") + "</span></td>"
              + "<td style='width:30%'class='nodeOperateBatchType'><span title='" + nodeOperateBatchType[v.batchType] + (v.batchCondition == 1 ? "属于" : "不属于") + v.batchTargetName.join(",") + "'>" + nodeOperateBatchType[v.batchType] + (v.batchCondition == 1 ? "属于" : "不属于") + v.batchTargetName.join(",") + "</span></td>"
              + "<td style='width:15%' class='nodeCountersign'><span>" + countersignArray[v.countersign] + "</span></td>");
            }
            $delTd.append([$upSpan, $downSpan, $delSpan]);
            $tr.append($OrderNumTd,$td, $delTd);
            $tbody.append($tr);
            if ($("#nodeOperateConditionTable").height() > 82) {
                $("#nodeOperateTableHead").css("width", "97%");
            }
             
            $upSpan.click(function () {
                //console.log("$upSpan was click");
                if ($(this).closest("tr").prev().attr("term") != undefined) {
                    var orderNum1 = $(this).parent("td").siblings(".OrderNumTd").text();
                    var operateId1 = $(this).closest("tr").attr("term");
                    var orderNum2 = $(this).closest("tr").prev().find(".OrderNumTd").text();
                    var operateId2 = $(this).closest("tr").prev().attr("term");

                    var nodeOperateType1 = $(this).closest("tr").find(".nodeOperateType").html();
                    var nodeOperateBatchType1 = $(this).closest("tr").find(".nodeOperateBatchType").html();
                    var nodeCountersign1 = $(this).closest("tr").find(".nodeCountersign").html();

                    var nodeOperateType2 = $(this).closest("tr").prev().find(".nodeOperateType").html();
                    var nodeOperateBatchType2 = $(this).closest("tr").prev().find(".nodeOperateBatchType").html();
                    var nodeCountersign2 = $(this).closest("tr").prev().find(".nodeCountersign").html();

                    var tr1 = $(this).closest("tr");
                    var tr2 = $(this).closest("tr").prev();
                    tr1.find(".nodeOperateType").html(nodeOperateType2);
                    tr2.find(".nodeOperateType").html(nodeOperateType1);

                    tr1.find(".nodeOperateBatchType").html(nodeOperateBatchType2);
                    tr2.find(".nodeOperateBatchType").html(nodeOperateBatchType1);

                    tr1.find(".nodeCountersign").html(nodeCountersign2);
                    tr2.find(".nodeCountersign").html(nodeCountersign1);
                    tr1.attr("term", operateId2);
                    tr2.attr("term", operateId1);
                    for (var j = 0; j < nodeOperateArray.length; j++) {
                        if (nodeOperateArray[j].orderNum == orderNum1 && nodeOperateArray[j].operateId == operateId1) {
                           // console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                            nodeOperateArray[j].orderNum--;
                         //   console.log("after--:" + nodeOperateArray[j].orderNum);

                        }
                        if (nodeOperateArray[j].orderNum == orderNum2 && nodeOperateArray[j].operateId == operateId2) {
                           // console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                            nodeOperateArray[j].orderNum++;
                           // console.log("after++:" + nodeOperateArray[j].orderNum);
                        }
                    }
                }

            });
            $downSpan.click(function () {
                //console.log("$downSpan was click");
                if ($(this).closest("tr").next().attr("term") != undefined) {
                    var orderNum1 = $(this).parent("td").siblings(".OrderNumTd").text();
                    var operateId1 = $(this).closest("tr").attr("term");
                    var orderNum2 = $(this).closest("tr").next().find(".OrderNumTd").text();
                    var operateId2 = $(this).closest("tr").next().attr("term");
                    //console.log("orderNum1" + orderNum1 + "operateId1" + operateId1);
                    //console.log("orderNum2" + orderNum2 + "operateId2" + operateId2);
                    var nodeOperateType1 = $(this).closest("tr").find(".nodeOperateType").html();
                    var nodeOperateBatchType1 = $(this).closest("tr").find(".nodeOperateBatchType").html();
                    var nodeCountersign1 = $(this).closest("tr").find(".nodeCountersign").html();

                    var nodeOperateType2 = $(this).closest("tr").next().find(".nodeOperateType").html();
                    var nodeOperateBatchType2 = $(this).closest("tr").next().find(".nodeOperateBatchType").html();
                    var nodeCountersign2 = $(this).closest("tr").next().find(".nodeCountersign").html();

                    var tr1 = $(this).closest("tr");
                    var tr2 = $(this).closest("tr").next();
                    tr1.find(".nodeOperateType").html(nodeOperateType2);
                    tr2.find(".nodeOperateType").html(nodeOperateType1);

                    tr1.find(".nodeOperateBatchType").html(nodeOperateBatchType2);
                    tr2.find(".nodeOperateBatchType").html(nodeOperateBatchType1);

                    tr1.find(".nodeCountersign").html(nodeCountersign2);
                    tr2.find(".nodeCountersign").html(nodeCountersign1);
                    tr1.attr("term", operateId2);
                    tr2.attr("term", operateId1);
                    for (var j = 0; j < nodeOperateArray.length; j++) {
                        if (nodeOperateArray[j].orderNum == orderNum1 && nodeOperateArray[j].operateId == operateId1) {
                          //  console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                            nodeOperateArray[j].orderNum++;
                          //  console.log("after++:" + nodeOperateArray[j].orderNum);
                        }
                        if (nodeOperateArray[j].orderNum == orderNum2 && nodeOperateArray[j].operateId == operateId2) {
                        //    console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                            nodeOperateArray[j].orderNum--;
                         //   console.log("after--:" + nodeOperateArray[j].orderNum);

                        }
                    }
                }


            });
            $delSpan.click(function () {
                var deleteOrderNum;
                for (var j = 0; j < nodeOperateArray.length; j++) {
                    if (nodeOperateArray[j].operateId == $(this).closest("tr").attr("term")) {
                        deleteOrderNum = nodeOperateArray[j].orderNum;
                       // console.log("deleteOrderNum" + deleteOrderNum)
                        break;
                    }
                }
                nodeOperateArray.splice(j, 1);
                $(this).parents("tr").remove();
                for (var j = 0; j < nodeOperateArray.length; j++) {
                    if (nodeOperateArray[j].orderNum > deleteOrderNum) {
                        //console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                        nodeOperateArray[j].orderNum--;

                        //console.log(" after nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);

                        $("#nodeOperateConditionTable tr[term='" + nodeOperateArray[j].operateId + "']").find(".OrderNumTd").text(nodeOperateArray[j].orderNum);
                    }
                }

            })
        });


    }



    //清空
    $("#nodeOperateSet-clear").click(function () {
        ncUnits.confirm({
            title: '提示',
            html: "确认要清空全部信息",
            yes: function (layer_confirm) {
                //将从数据库中获取的id全部放到临时删除列表中    
                for (var i = 0; i < nodeOperateArray.length; i++) {
                    delOperateIdArray[delOperateIdArray.length] = nodeOperateArray[i].operateId;
                }
                nodeOperateArray.length = 0;
                var $tbody = $("#nodeOperateConditionTable tbody");
                $tbody.empty();
                layer.close(layer_confirm);

            }
        });

    })
    //添加
    function OperateSetadd() {
        var operator = {
            operateId: 0,      //节点操作人ID
            nodeId: 0,         //节点ID
            type: 0,           //类型
            condition: 0,      //条件
            countersign: null,    //会签
            batchType: 0,      //批次条件
            batchCondition: 0, //条件
            targetId: [],     //操作人目标ID
            targetName: [],     //操作人目标ID
            batchTargetId: [], //批次目标ID
            batchTargetName: [], //批次目标ID
            orderNum: null
        }
        operator.operateId = 'r' + new Date().getTime();
        operator.condition = 1;//全部默认为1
        if ($("#node-countersign").attr("term") != "") { operator.countersign = $("#node-countersign").attr("term"); }

        var operatorName;
        $(".NodeOperate-type").each(function () {
            if ($(this).prop("checked")) {
                operator.type = $(this).attr("term");
                operatorName = $(this).parent(".col-xs-2").siblings(".chooseName").text();
            }
        })
        if (operator.type == 0) {
            ncUnits.alert("没有选择操作者类型"); return;
        }
        if (operator.type == 1 || operator.type == 2 || operator.type == 3) {
            if (operatorName == "") { ncUnits.alert("没有选择对应的操作者信息"); return; }
            else if (operatorName != "") {
                if (operator.type == 1) {
                    for (var i = 0; i < targetIdArray3.length; i++) {
                        operator.targetId.push(targetIdArray3[i]);
                        operator.targetName.push(targetNameArray3[i]);
                    }
                }
                else if (operator.type == 2) {
                    for (var i = 0; i < targetIdArray1.length; i++) {
                        operator.targetId.push(targetIdArray1[i]);
                        operator.targetName.push(targetNameArray1[i]);
                    }
                }
                else if (operator.type == 3) {
                    for (var i = 0; i < targetIdArray2.length; i++) {
                        operator.targetId.push(targetIdArray2[i]);
                        operator.targetName.push(targetNameArray2[i]);
                    }
                }

            }
        }

        var typeFlag = 1;
        var ProposerName;
        var batchFlag = 0;//标志在条件部分是否存在返回
        $(".NodeOperate-batchType").each(function () {
            if ($(this).prop("checked")) {
                operator.batchType = typeFlag;
                operator.batchCondition = $(this).parent(".col-xs-2").siblings(".col-xs-3").find(".node-batchCondition").attr("term");
                ProposerName = $(this).parent(".col-xs-2").siblings(".chooseName").text();
                if (operator.batchType == 0) { ncUnits.alert("没有选择批次条件类型"); batchFlag = 1; return false; }
                if (operator.batchCondition == 0) { ncUnits.alert("没有选择批次条件"); batchFlag = 1; return false; }
                if (ProposerName == "") { ncUnits.alert("没有选择对应的申请人信息"); batchFlag = 1; return false; }
                else if (ProposerName != "") {
                    if (operator.batchType == 1) {
                        for (var i = 0; i < batchTargetIdArray3.length; i++) {
                            operator.batchTargetId.push(batchTargetIdArray3[i]);
                            operator.batchTargetName.push(batchTargetNameArray3[i]);
                        }
                    }
                    else if (operator.batchType == 2) {
                        for (var i = 0; i < batchTargetIdArray1.length; i++) {
                            operator.batchTargetId.push(batchTargetIdArray1[i]);
                            operator.batchTargetName.push(batchTargetNameArray1[i]);
                        }
                    }
                    else if (operator.batchType == 3) {
                        for (var i = 0; i < batchTargetIdArray2.length; i++) {
                            operator.batchTargetId.push(batchTargetIdArray2[i]);
                            operator.batchTargetName.push(batchTargetNameArray2[i]);
                        }
                    }

                }
            }
            typeFlag++;
        })
        if (batchFlag == 1) { return; }
        var maxOrderNum = 0;
        for (var j = 0; j < nodeOperateArray.length; j++) {
            if (maxOrderNum < nodeOperateArray[j].orderNum) {
                maxOrderNum = nodeOperateArray[j].orderNum;

            }
        }
        operator.orderNum = maxOrderNum + 1;
       // console.log("maxOrderNum" + maxOrderNum);
        nodeOperateArray.push(operator);

        var $tbody = $("#nodeOperateConditionTable tbody");

        var $tr = $("<tr term='" + operator.operateId + "'></tr>");
        var $OrderNumTd = $("<td class='OrderNumTd' style='width:8%'>" + operator.orderNum + "</td>");
        var $delTd = $("<td style='width:17%'></td>");
        var $delSpan = $("<span  class='delPic'></span>");
        var $upSpan = $("<span  class='order-upPic'></span>");
        var $downSpan = $("<span  class='order-downPic'></span>");
        var $td;
        if ((operator.type == 4 || operator.type == 5) && operator.batchType != 0) {
            $td = $("<td style='width:30%'class='nodeOperateType'><span title='" + nodeOperateType[operator.type] + "'>" + nodeOperateType[operator.type] + "</span></td>"
        + "<td style='width:30%' class='nodeOperateBatchType'><span title=" + nodeOperateBatchType[operator.batchType] + (operator.batchCondition == 1 ? "属于" : "不属于") + operator.batchTargetName.join(",") + ">" + nodeOperateBatchType[operator.batchType] + (operator.batchCondition == 1 ? "属于" : "不属于") + operator.batchTargetName.join(",") + "</span></td>"
        + "<td style='width:15%' class='nodeCountersign'><span>" + $("#node-countersign").text() + "</span></td>");
        }
        else if ((operator.type == 4 || operator.type == 5) && operator.batchType == 0) {
            $td = $("<td style='width:30%' class='nodeOperateType'><span title='" + nodeOperateType[operator.type] + "'>" + nodeOperateType[operator.type] + "</span></td>"
        + "<td style='width:30%' class='nodeOperateBatchType'><span title=" + "无" + ">" + "无" + "</span></td>"
        + "<td style='width:15%' class='nodeCountersign'><span>" + $("#node-countersign").text() + "</span></td>");

        }

        else if (operator.batchType == 0) {
            $td = $("<td style='width:30%'class='nodeOperateType'><span title='" + nodeOperateType[operator.type] + (operator.condition == 1 ? "属于" : "不属于") + operator.targetName.join(",") + "'>" + nodeOperateType[operator.type] + (operator.condition == 1 ? "属于" : "不属于") + operator.targetName.join(",") + "</span></td>"
        + "<td style='width:30%'class='nodeOperateBatchType'><span title=" + "无" + ">" + "无" + "</span></td>"
        + "<td style='width:15%' class='nodeCountersign'><span>" + $("#node-countersign").text() + "</span></td>");

        }
        else {
            $td = $("<td style='width:30%'class='nodeOperateType'><span title='" + nodeOperateType[operator.type] + (operator.condition == 1 ? "属于" : "不属于") + operator.targetName.join(",") + "'>" + nodeOperateType[operator.type] + (operator.condition == 1 ? "属于" : "不属于") + operator.targetName.join(",") + "</span></td>"
             + "<td style='width:30%'class='nodeOperateBatchType'><span title=" + nodeOperateBatchType[operator.batchType] + (operator.batchCondition == 1 ? "属于" : "不属于") + operator.batchTargetName.join(",") + ">" + nodeOperateBatchType[operator.batchType] + (operator.batchCondition == 1 ? "属于" : "不属于") + operator.batchTargetName.join(",") + "</span></td>"
             + "<td style='width:15%' class='nodeCountersign'><span>" + $("#node-countersign").text() + "</span></td>");
        }
        $delTd.append([$upSpan, $downSpan, $delSpan]);
        $tr.append($OrderNumTd, $td, $delTd);
        $tbody.append($tr);
        if ($("#nodeOperateConditionTable").height() > 82) {
            $("#nodeOperateTableHead").css("width", "97%");
        }

        $upSpan.click(function () {
          //  console.log("$upSpan was click");
            if ($(this).closest("tr").prev().attr("term") != undefined) {
                var orderNum1 = $(this).parent("td").siblings(".OrderNumTd").text();
                var operateId1 = $(this).closest("tr").attr("term");
                var orderNum2 = $(this).closest("tr").prev().find(".OrderNumTd").text();
                var operateId2 = $(this).closest("tr").prev().attr("term");

                var nodeOperateType1 = $(this).closest("tr").find(".nodeOperateType").html();
                var nodeOperateBatchType1 = $(this).closest("tr").find(".nodeOperateBatchType").html();
                var nodeCountersign1 = $(this).closest("tr").find(".nodeCountersign").html();

                var nodeOperateType2 = $(this).closest("tr").prev().find(".nodeOperateType").html();
                var nodeOperateBatchType2 = $(this).closest("tr").prev().find(".nodeOperateBatchType").html();
                var nodeCountersign2 = $(this).closest("tr").prev().find(".nodeCountersign").html();

                var tr1 = $(this).closest("tr");
                var tr2 = $(this).closest("tr").prev();
                tr1.find(".nodeOperateType").html(nodeOperateType2);
                tr2.find(".nodeOperateType").html(nodeOperateType1);

                tr1.find(".nodeOperateBatchType").html(nodeOperateBatchType2);
                tr2.find(".nodeOperateBatchType").html(nodeOperateBatchType1);

                tr1.find(".nodeCountersign").html(nodeCountersign2);
                tr2.find(".nodeCountersign").html(nodeCountersign1);
                tr1.attr("term", operateId2);
                tr2.attr("term", operateId1);
                for (var j = 0; j < nodeOperateArray.length; j++) {
                    if (nodeOperateArray[j].orderNum == orderNum1 && nodeOperateArray[j].operateId == operateId1) {
                       // console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                        nodeOperateArray[j].orderNum--;
                       // console.log("after--:" + nodeOperateArray[j].orderNum);

                    }
                    if (nodeOperateArray[j].orderNum == orderNum2 && nodeOperateArray[j].operateId == operateId2) {
                     //   console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                        nodeOperateArray[j].orderNum++;
                     //   console.log("after++:" + nodeOperateArray[j].orderNum);
                    }
                }
            }

        });
        $downSpan.click(function () {
           // console.log("$downSpan was click");
            if ($(this).closest("tr").next().attr("term") != undefined) {
                var orderNum1 = $(this).parent("td").siblings(".OrderNumTd").text();
                var operateId1 = $(this).closest("tr").attr("term");
                var orderNum2 = $(this).closest("tr").next().find(".OrderNumTd").text();
                var operateId2 = $(this).closest("tr").next().attr("term");
                //console.log("orderNum1" + orderNum1 + "operateId1" + operateId1);
               // console.log("orderNum2" + orderNum2 + "operateId2" + operateId2);
                var nodeOperateType1 = $(this).closest("tr").find(".nodeOperateType").html();
                var nodeOperateBatchType1 = $(this).closest("tr").find(".nodeOperateBatchType").html();
                var nodeCountersign1 = $(this).closest("tr").find(".nodeCountersign").html();

                var nodeOperateType2 = $(this).closest("tr").next().find(".nodeOperateType").html();
                var nodeOperateBatchType2 = $(this).closest("tr").next().find(".nodeOperateBatchType").html();
                var nodeCountersign2 = $(this).closest("tr").next().find(".nodeCountersign").html();

                var tr1 = $(this).closest("tr");
                var tr2 = $(this).closest("tr").next();
                tr1.find(".nodeOperateType").html(nodeOperateType2);
                tr2.find(".nodeOperateType").html(nodeOperateType1);

                tr1.find(".nodeOperateBatchType").html(nodeOperateBatchType2);
                tr2.find(".nodeOperateBatchType").html(nodeOperateBatchType1);

                tr1.find(".nodeCountersign").html(nodeCountersign2);
                tr2.find(".nodeCountersign").html(nodeCountersign1);
                tr1.attr("term", operateId2);
                tr2.attr("term", operateId1);
                for (var j = 0; j < nodeOperateArray.length; j++) {
                    if (nodeOperateArray[j].orderNum == orderNum1 && nodeOperateArray[j].operateId == operateId1) {
                     //   console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                        nodeOperateArray[j].orderNum++;
                      //  console.log("after++:" + nodeOperateArray[j].orderNum);
                    }
                    if (nodeOperateArray[j].orderNum == orderNum2 && nodeOperateArray[j].operateId == operateId2) {
                     //   console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                        nodeOperateArray[j].orderNum--;
                      //  console.log("after--:" + nodeOperateArray[j].orderNum);

                    }
                }
            }


        });

        $delSpan.click(function () {
            var deleteOrderNum;
            for (var j = 0; j < nodeOperateArray.length; j++) {
                if (nodeOperateArray[j].operateId == $(this).closest("tr").attr("term")) {
                    deleteOrderNum = nodeOperateArray[j].orderNum;
                   // console.log("deleteOrderNum" + deleteOrderNum)
                    break;
                }
            }
            nodeOperateArray.splice(j, 1);
            $(this).parents("tr").remove();
            for (var j = 0; j < nodeOperateArray.length; j++) {
                if (nodeOperateArray[j].orderNum > deleteOrderNum) {
                  //  console.log("nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);
                    nodeOperateArray[j].orderNum--;

                   // console.log(" after nodeOperateArray[j].orderNum:" + nodeOperateArray[j].orderNum);

                    $("#nodeOperateConditionTable tr[term='" + nodeOperateArray[j].operateId + "']").find(".OrderNumTd").text(nodeOperateArray[j].orderNum);
                }
            }

        })
        //数据全部清零
        $(".node-rightModal").css("display", "none");
        targetIdArray1.length = 0;
        targetNameArray1.length = 0;
        targetIdArray2.length = 0;
        targetNameArray2.length = 0;
        targetIdArray3.length = 0;
        targetNameArray3.length = 0;

        batchTargetIdArray1.length = 0;
        batchTargetNameArray1.length = 0;
        batchTargetIdArray2.length = 0;
        batchTargetNameArray2.length = 0;
        batchTargetIdArray3.length = 0;
        batchTargetNameArray3.length = 0;
        targetorganizationId1 = null;
        targetorganizationId2 = null;
        batchTargetorganizationId1 = null;
        batchTargetorganizationId2 = null;
        $("#node_addOperatorDepart").siblings(".chooseName").text("");
        $("#node_addOperatorPost ").siblings(".chooseName").text("");
        $("#node_addOperator ").siblings(".chooseName").text("");
        $("#node_addProposerPost ").siblings(".chooseName").text("");
        $("#node_addProposerDepart ").siblings(".chooseName").text("");
        $("#node_addProposer ").siblings(".chooseName").text("");

        $(".NodeOperate-type").each(function () {
            $(this).prop("checked", false);
        })

        $(".NodeOperate-batchType").each(function () {
            $(this).prop("checked", false);
        })
    }
    $("#nodeOperateSet-add").click(function () {
       
        var type=0;
        if ($("#node-countersign").attr("term") == "") {
            ncUnits.alert("没有选择操作类型");
            return;
        }
        $(".NodeOperate-type").each(function () {
            if ($(this).prop("checked")) {
                type = $(this).attr("term");
            }
        })
        if (type == 0) { ncUnits.alert("没有选择操作人类型"); return; }
        if (type == 5) {
            if (nodeOperateArray.length > 0) {
                /////////////////////////////////////////////////////////////////

                ncUnits.confirm({
                    title: '提示',
                    html: '设置包含所有人的操作信息,继续添加将清空该节点原有的操作人设置信息，确认继续？',
                    yes: function (layer_confirm) {
                        nodeOperateArray.length = 0;
                        layer.close(layer_confirm);
                        $("#nodeOperateConditionTable tbody").empty();
                        ///////////////////
                        OperateSetadd();
                    }
                });
            }
            else { OperateSetadd(); }

        }
        else {
            var flag=0;
            for (var j = 0; j < nodeOperateArray.length; j++) {
                if (nodeOperateArray[j].type == 5) {
                    flag=1;
                    ncUnits.confirm({
                        title: '提示',
                        html: '您已设置包含所有人的操作信息,继续添加将清空该节点原有的操作人设置信息，确认继续？',
                        yes: function (layer_confirm) {
                            nodeOperateArray.length = 0;
                            layer.close(layer_confirm);
                            $("#nodeOperateConditionTable tbody").empty();
                            ///////////////////
                            OperateSetadd();
                        }
                    });
                    break;
                }
            }
            if (flag==0) {
                 OperateSetadd();
            }
        }

      

    })
    //当改变状态时，清除原有设置数据
    $("#node-countersign").closest(".dropdown").find("ul li").click(function () {
        if (nodeType == 3) {
            if ($(this).attr("id") == "countersign-no") {
                $(".NodeOperate-type[term=1]").closest(".row").css("display", "none");
                $("#node_addOperatorDepart").addClass("noClick");
                $(".NodeOperate-type[term=4]").closest(".row").css("display", "block");

            }
            if ($(this).attr("id") == "countersign-sign") {
                $(".NodeOperate-type[term=1]").closest(".row").css("display", "block");
                $("#node_addOperatorDepart").removeClass("noClick");
                //
                $(".NodeOperate-type[term=4]").closest(".row").css("display", "none");

            }
            if ($(this).attr("id") == "countersign-copy") {
                $(".NodeOperate-type[term=1]").closest(".row").css("display", "block");
                $("#node_addOperatorDepart").removeClass("noClick");
                //
                $(".NodeOperate-type[term=4]").closest(".row").css("display", "block");

            }
        }


        $("#nodeOperateSet_modal_content .chooseName").each(function () {
            $(this).attr("title", "");
            $(this).text("");
        })
        $(".NodeOperate-type").each(function () {
            $(this).prop("checked", false);
        })
       
        targetIdArray1.length = 0;
        targetNameArray1.length = 0;
        targetIdArray2.length = 0;
        targetNameArray2.length = 0;
        targetIdArray3.length = 0;
        targetNameArray3.length = 0;
        batchTargetIdArray1.length = 0;
        batchTargetNameArray1.length = 0;
        targetIdArray2.length = 0;
        targetNameArray2.length = 0;
        targetIdArray3.length = 0;
        targetNameArray3.length = 0;
        targetorganizationId1 = null;
        targetorganizationId2 = null;
        targetWithSub1 = null;
        targetWithSub2 = null;
        batchTargetorganizationId1 = null;
        batchTargetorganizationId2 = null;
        batchTargetWithSub1 = null;
        batchTargetWithSub2 = null;
        $(".node-rightModal").css("display", "none");
    })
    $(".NodeOperate-type").click(function () {
        $(".node-rightModal").css("display", "none");
    })

    $(".NodeOperate-batchType").click(function () {
        $(".node-rightModal").css("display", "none");       
        if ($(this).prop("checked") == true && batchTypeFlag[$(this).index(".NodeOperate-batchType")] == true) {
            $(this).prop("checked", false);
            batchTypeFlag[$(this).index(".NodeOperate-batchType")] = false;
            for (var i = 0; i <= 2; i++) {
                if (i == $(this).index(".NodeOperate-batchType")) {
                    continue;
                }
                else { batchTypeFlag[i] = $(".NodeOperate-batchType:eq("+i+")").prop("checked"); }
            }
        }
        else {
            batchTypeFlag[0] = $(".NodeOperate-batchType:eq(0)").prop("checked");
            batchTypeFlag[1] = $(".NodeOperate-batchType:eq(1)").prop("checked");
            batchTypeFlag[2] = $(".NodeOperate-batchType:eq(2)").prop("checked");
        }
         
    })

    //点击操作者岗位
    $("#node_addOperatorDepart").click(function () {
        if ($("#node-countersign").attr("term") == "") {
            ncUnits.alert("操作类型没有选择");
            return;
        }
        if ($(this).hasClass("noClick")) {
            return;
        }
        //默认选中左侧对应的radio
        $(this).siblings(".col-xs-2").find("input").prop("checked", true);
        $(".node-rightModal").css("display", "block");
        //$("#node_addOperatorDepart").siblings(".chooseName").text("");
        //$("#node_addOperatorPost ").siblings(".chooseName").text("");
        //$("#node_addOperator ").siblings(".chooseName").text("");
        //targetIdArray.length = 0;
        //targetNameArray.length = 0;
        NodeZtreeChoose(3, 1);
        batchOrNot = 1;
        ztreeFlag = 3;
    })
    $("#node_addOperatorPost").click(function () {
        if ($("#node-countersign").attr("term") == "") {
            ncUnits.alert("操作类型没有选择");
            return;
        }
        //默认选中左侧对应的radio
        $(this).siblings(".col-xs-2").find("input").prop("checked", true);
        $(".node-rightModal").css("display", "block");
        //$("#node_addOperatorDepart ").siblings(".chooseName").text("");
        //$("#node_addOperatorPost ").siblings(".chooseName").text("");
        //$("#node_addOperator ").siblings(".chooseName").text("");     
        //targetIdArray.length = 0;
        //targetNameArray.length = 0;
        NodeZtreeChoose(1, 1);
        batchOrNot = 1;
        ztreeFlag = 1;
    })
    $("#node_addOperator").click(function () {
        if ($("#node-countersign").attr("term") == "") {
            ncUnits.alert("操作类型没有选择");
            return;
        }
        //默认选中左侧对应的radio
        $(this).siblings(".col-xs-2").find("input").prop("checked", true);
        $(".node-rightModal").css("display", "block");
        //$("#node_addOperatorDepart ").siblings(".chooseName").text("");
        //$("#node_addOperatorPost ").siblings(".chooseName").text("");
        //$("#node_addOperator ").siblings(".chooseName").text("");    
        //targetIdArray.length = 0;
        //targetNameArray.length = 0;
        NodeZtreeChoose(2, 1);
        batchOrNot = 1;
        ztreeFlag = 2;
    })

    $("#node_addProposerDepart").click(function () {
        if (nodeType == 1) { return; }
        //默认选中左侧对应的radio
        $(this).siblings(".col-xs-2").find("input").prop("checked", true);
        $(".node-rightModal").css("display", "block");
        //$("#node_addProposerPost").siblings(".chooseName").text("");
        //$("#node_addProposerDepart").siblings(".chooseName").text("");
        //$("#node_addProposer").siblings(".chooseName").text("");
        //batchTargetIdArray.length = 0;
        //batchTargetNameArray.length = 0;
        NodeZtreeChoose(3, 2);
        batchOrNot = 2;
        ztreeFlag = 3;

    })
    $("#node_addProposerPost").click(function () {
        if (nodeType == 1) { return; }
        //默认选中左侧对应的radio
        $(this).siblings(".col-xs-2").find("input").prop("checked", true);
        $(".node-rightModal").css("display", "block");
        //$("#node_addProposerPost ").siblings(".chooseName").text("");
        //$("#node_addProposerDepart ").siblings(".chooseName").text("");
        //$("#node_addProposer ").siblings(".chooseName").text("");
        //batchTargetIdArray.length = 0;
        //batchTargetNameArray.length = 0;
        NodeZtreeChoose(1, 2);
        batchOrNot = 2;
        ztreeFlag = 1;
    })
    $("#node_addProposer").click(function () {
        if (nodeType == 1) { return; }
        //默认选中左侧对应的radio
        $(this).siblings(".col-xs-2").find("input").prop("checked", true);
        $(".node-rightModal").css("display", "block");
        //$("#node_addProposerPost").siblings(".chooseName").text("");
        //$("#node_addProposerDepart").siblings(".chooseName").text("");
        //$("#node_addProposer").siblings(".chooseName").text("");
        //batchTargetIdArray.length = 0;
        //batchTargetNameArray.length = 0;
        NodeZtreeChoose(2, 2);
        batchOrNot = 2;
        ztreeFlag = 2;
    })
    //全选
    function nodeSelectAllFunc(flag, flag2) {
        if (flag == 1 && flag2 == 1) {
            var $chooseName = $("#node_addOperatorPost").siblings(".chooseName")
        }
        else if (flag == 1 && flag2 == 2) {
            var $chooseName = $("#node_addProposerPost").siblings(".chooseName")
        }
        else if (flag == 2 && flag2 == 1) {
            var $chooseName = $("#node_addOperator").siblings(".chooseName")
        }
        else if (flag == 2 && flag2 == 2) {
            var $chooseName = $("#node_addProposer").siblings(".chooseName")
        }

        if ($(".nodeSelectAll").is(":checked")) {
            $("#nodeChooseList ul li input").each(function () {
                //如果不具有改选项的权限，不可以选择
                if ($(this).prop("disabled") == false) {
                    $(this).prop("checked", true);
                    var flagAdd = 0;
                    if (flag == 1 && flag2 == 1) {
                        var alertFlag=0
                        if (nodeType != 1 &&$(this).siblings("span:eq(0)").hasClass("zero")) {
                            $(this).prop("checked", false);
                            alertFlag=1;
                        }
                        else {
                            for (var i = 0; i < targetIdArray1.length; i++) {
                                if (targetIdArray1[i] == $(this).val()) {
                                    flagAdd = 1;
                                    break;
                                }
                            }
                            if (flagAdd != 1) {
                                targetIdArray1[targetIdArray1.length] = $(this).val();
                                targetNameArray1[targetNameArray1.length] = $(this).siblings("span:eq(0)").text() + "-" + $(this).siblings("span:eq(1)").text();

                                var chooseNameHtml = "";
                                for (var i = 0; i < targetIdArray1.length; i++) {
                                    chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList1' id='" + targetIdArray1[i] + "'></span></span>";
                                }
                                $chooseName.html(chooseNameHtml);
                            }


                        }
                        if (alertFlag == 1) {
                            ncUnits.alert("人数为0的岗位不能选择")
                        }
                       
                    }
                    else if (flag == 2 && flag2 == 1) {
                        for (var i = 0; i < targetIdArray2.length; i++) {
                            if (targetIdArray2[i] == $(this).val()) {
                                flagAdd = 1;
                                break;
                            }
                        }
                        if (flagAdd != 1) {
                            targetIdArray2[targetIdArray2.length] = $(this).val();
                            targetNameArray2[targetNameArray2.length] = $(this).siblings("span:eq(0)").text()+ "-"+ $(this).siblings("span:eq(1)").text();

                            var chooseNameHtml = "";
                            for (var i = 0; i < targetIdArray2.length; i++) {
                                chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList2' id='" + targetIdArray2[i] + "'></span></span>";
                            }
                            $chooseName.html(chooseNameHtml);
                        }
                    }
                    else if (flag == 1 && flag2 == 2) {
                        for (var i = 0; i < batchTargetIdArray1.length; i++) {
                            if (batchTargetIdArray1[i] == $(this).val()) {
                                flagAdd = 1;
                                break;
                            }
                        }
                        if (flagAdd != 1) {
                            batchTargetIdArray1[batchTargetIdArray1.length] = $(this).val();
                            batchTargetNameArray1[batchTargetNameArray1.length] = $(this).siblings("span:eq(0)").text() + "-" + $(this).siblings("span:eq(1)").text();
                            var chooseNameHtml = "";
                            for (var i = 0; i < batchTargetIdArray1.length; i++) {
                                chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchList1' id='" + batchTargetIdArray1[i] + "'></span></span>";
                            }
                            $chooseName.html(chooseNameHtml);
                        }

                    }
                    else if (flag == 2 && flag2 == 2) {
                        for (var i = 0; i < batchTargetIdArray2.length; i++) {
                            if (batchTargetIdArray2[i] == $(this).val()) {
                                flagAdd = 1;
                                break;
                            }
                        }
                        if (flagAdd != 1) {
                            batchTargetIdArray2[batchTargetIdArray2.length] = $(this).val();
                            batchTargetNameArray2[batchTargetNameArray2.length] = $(this).siblings("span:eq(0)").text()+"-"+ $(this).siblings("span:eq(1)").text();
                            var chooseNameHtml = "";
                            for (var i = 0; i < batchTargetIdArray2.length; i++) {
                                chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchList2' id='" + batchTargetIdArray2[i] + "'></span></span>";
                            }
                            $chooseName.html(chooseNameHtml);
                        }

                    }

                }

            })
        }
        else {
            $("#nodeChooseList ul li input").each(function () {
                if ($(this).prop("disabled") == false) {
                    $(this).prop("checked", false);
                    //移除数组中的对应元素
                    if (flag == 1 && flag2 == 1) {
                        for (var j = 0; j < targetIdArray1.length; j++) {
                            if ($(this).val() == targetIdArray1[j]) {
                                break;
                            }
                        }
                        targetIdArray1.splice(j, 1);
                        targetNameArray1.splice(j, 1);
                        var chooseNameHtml = "";
                        for (var i = 0; i < targetIdArray1.length; i++) {
                            chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList' id='" + targetIdArray1[i] + "'></span></span>";
                        }
                        $chooseName.html(chooseNameHtml);
                    }
                    else if (flag == 1 && flag2 == 2) {
                        for (var j = 0; j < batchTargetIdArray1.length; j++) {
                            if ($(this).val() == batchTargetIdArray1[j]) {
                                break;
                            }
                        }
                        batchTargetIdArray1.splice(j, 1);
                        batchTargetNameArray1.splice(j, 1);
                        var chooseNameHtml = "";
                        for (var i = 0; i < batchTargetIdArray1.length; i++) {
                            chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList' id='" + batchTargetIdArray1[i] + "'></span></span>";
                        }
                        $chooseName.html(chooseNameHtml);
                    }
                    else if (flag == 2 && flag2 == 1) {
                        for (var j = 0; j < targetIdArray2.length; j++) {
                            if ($(this).val() == targetIdArray2[j]) {
                                break;
                            }
                        }
                        targetIdArray2.splice(j, 1);
                        targetNameArray2.splice(j, 1);
                        var chooseNameHtml = "";
                        for (var i = 0; i < targetIdArray2.length; i++) {
                            chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList' id='" + targetIdArray2[i] + "'></span></span>";
                        }
                        $chooseName.html(chooseNameHtml);
                    }
                    else if (flag == 2 && flag2 == 2) {
                        for (var j = 0; j < batchTargetIdArray2.length; j++) {
                            if ($(this).val() == batchTargetIdArray2[j]) {
                                break;
                            }
                        }
                        batchTargetIdArray2.splice(j, 1);
                        batchTargetNameArray2.splice(j, 1);
                        var chooseNameHtml = "";
                        for (var i = 0; i < batchTargetIdArray2.length; i++) {
                            chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList' id='" + batchTargetIdArray2[i] + "'></span></span>";
                        }
                        $chooseName.html(chooseNameHtml);
                    }
                }


            })
        }
        $(".nodeZtreeList1").click(function () {
            var id = $(this).attr("id");
            $this = $(this);
            if (batchOrNot == 1 && ztreeFlag == 1) {
                $("#nodeChooseList li input").each(function () {
                    if ($(this).val() == id) {
                        $(this).prop("checked", false);
                    }
                })
            }
            for (var j = 0; j < targetIdArray1.length; j++) {
                if (targetIdArray1[j] == id) {
                    break;
                }
            }
            targetIdArray1.splice(j, 1);
            targetNameArray1.splice(j, 1);
            $this.parent("span").remove();
        })
        $(".nodeZtreeList2").click(function () {
            var id = $(this).attr("id");
            $this = $(this);
            if (batchOrNot == 1 && ztreeFlag == 2) {
                $("#nodeChooseList li input").each(function () {
                    if ($(this).val() == id) {
                        $(this).prop("checked", false);
                    }
                })
            }
            for (var j = 0; j < targetIdArray2.length; j++) {
                if (targetIdArray2[j] == id) {
                    break;
                }
            }
            targetIdArray2.splice(j, 1);
            targetNameArray2.splice(j, 1);
            $this.parent("span").remove();
        })
        $(".nodeZtreebatchList1").click(function () {
            var id = $(this).attr("id");
            $this = $(this);
            if (batchOrNot == 2 && ztreeFlag == 1) {
                $("#nodeChooseList li input").each(function () {
                    if ($(this).val() == id) {
                        $(this).prop("checked", false);
                    }
                })
            }
            for (var j = 0; j < batchTargetIdArray1.length; j++) {
                if (batchTargetIdArray1[j] == id) {
                    break;
                }
            }
            batchTargetIdArray1.splice(j, 1);
            batchTargetNameArray1.splice(j, 1);
            $this.parent("span").remove();
        })
        $(".nodeZtreebatchList2").click(function () {
            var id = $(this).attr("id");
            $this = $(this);
            if (batchOrNot == 2 && ztreeFlag == 2) {
                $("#nodeChooseList li input").each(function () {
                    if ($(this).val() == id) {
                        $(this).prop("checked", false);
                    }
                })
            }
            for (var j = 0; j < batchTargetIdArray2.length; j++) {
                if (batchTargetIdArray2[j] == id) {
                    break;
                }
            }
            batchTargetIdArray2.splice(j, 1);
            batchTargetNameArray2.splice(j, 1);
            $this.parent("span").remove();
        })
         
    };
    //ztree树种类选择
    function NodeZtreeChoose(flag, flag2) {
        $("#node-rightContent").empty();          //右侧树弹窗清空
        if (flag == 3) {                    //如果是部门选择
            $("#nodeZtreeTitle").text("部门选择");
            var $nodeOrgZtree = $("<div id='node-ZtreeContent' style='height:100%'><ul class='panel-body ztree creDepart' id='node-Ztree'></ul></div>");
            nodeTreeOrgLoadCheck(flag2);
            $("#node-rightContent").append($nodeOrgZtree);
        } else {
            if (flag == 1) {
                $("#nodeZtreeTitle").text("岗位选择");
            } else {
                $("#nodeZtreeTitle").text("人员选择");
            }
            var $nodeOrgZtree = $("<div id='node-ZtreeContent' style='height:300px'><ul class='panel-body ztree creDepart' id='node-Ztree'></ul></div>");
            nodeTreeOrgLoad(flag, flag2);

            if (flag2 == 1 && $("#node-countersign").attr("term") == 0 && nodeType == 3) {
                var $title = $("<hr class='hrAll' style='width: 412px;'/><div class='checkbox' style='padding-left: 10px;margin-bottom: -5px;'>" +
                    "<label><input type='checkbox' id='nodeWithSub'>包含下级</label><label style=' float:right; margin-right:20px;'><input type='checkbox' class='nodeSelectAll'  disabled>全部选择</label></div><hr style='margin-bottom: 0px;'/>");
            }
            else {
                var $title = $("<hr class='hrAll' style='width: 412px;'/><div class='checkbox' style='padding-left: 10px;margin-bottom: -5px;'>" +
                    "<label><input type='checkbox' id='nodeWithSub'>包含下级</label><label style=' float:right; margin-right:20px;'><input type='checkbox' class='nodeSelectAll'>全部选择</label></div><hr style='margin-bottom: 0px;'/>");
            }

            $title.find(".nodeSelectAll").click(function () {
                nodeSelectAllFunc(flag, flag2);
            });
            var $nodeList = $("<div id='nodeChooseList'><ul class='list-unstyled' style='margin-top: -5px;'></ul></div>");
            $("#node-rightContent").append([$nodeOrgZtree, $title, $nodeList]);
        }
    }

    //弹出框组织架构菜单的加载  flag:1 表示根据架构选择岗位  flag:2 根据架构选择人员
    function nodeTreeOrgLoad(flag, flag2) {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#node-Ztree"), $.extend({
                    callback: {
                        onNodeCreated: function (event, treeId, treeNode) {
                            var t = $.fn.zTree.getZTreeObj(treeId);
                            if (flag == 1 && flag2 == 1 && targetorganizationId1 != null) {
                                if (treeNode.id == targetorganizationId1) {
                                    t.checkNode(treeNode, true, false);
                                    if (targetWithSub1 == true) {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 1);
                                        $("#nodeWithSub").prop("checked", true);
                                    }
                                    else {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 0);
                                        $("#nodeWithSub").prop("checked", false);
                                    }
                                   
                                }
                            }
                            else if (flag == 1 && flag2 == 2 && batchTargetorganizationId1 != null) {
                                if (treeNode.id == batchTargetorganizationId1) {
                                    t.checkNode(treeNode, true, false);
                                    if (batchTargetWithSub1 == true) {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 1);
                                        $("#nodeWithSub").prop("checked", true);
                                    }
                                    else {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2,0);
                                        $("#nodeWithSub").prop("checked", false);
                                    }
                                   
                                }
                            }
                            else if (flag == 2 && flag2 == 1 && targetorganizationId2 != null) {
                                if (treeNode.id == targetorganizationId2) {
                                    t.checkNode(treeNode, true, false);
                                    if (targetWithSub2== true) {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 1);
                                        $("#nodeWithSub").prop("checked", true);
                                    }
                                    else {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 0);
                                        $("#nodeWithSub").prop("checked", false);
                                    }
                                }

                            }
                            else if (flag == 2 && flag2 == 2 && batchTargetorganizationId2 != null) {
                                if (treeNode.id == batchTargetorganizationId2) {
                                    t.checkNode(treeNode, true, false);
                                    if (batchTargetWithSub2 == true) {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 1);
                                        $("#nodeWithSub").prop("checked", true);
                                    }
                                    else {
                                        nodeAppendPersonJob(treeNode.id, flag, flag2, 0);
                                        $("#nodeWithSub").prop("checked", false);
                                    }
                                   
                                }
                            }
                        },
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {        //选中事件
                            if ($("#nodeWithSub").prop("checked")) {
                                nodeAppendPersonJob(node.id, flag, flag2, 1);
                            }
                            else { nodeAppendPersonJob(node.id, flag, flag2, 0); }
                            $("#nodeWithSub").off("click");
                            $("#nodeWithSub").click(function () {
                               
                                if ($(this).prop("checked") == true) {
                                    if (flag == 1 && flag2 == 1) { targetWithSub1 = true; }
                                    else if (flag == 1 && flag2 == 2) { batchTargetWithSub1 = true; }
                                    else if (flag == 2 && flag2 == 1) { targetWithSub2 = true; }
                                    else if (flag == 2 && flag2 == 2) { batchTargetWithSub2 = true; }
                                    nodeAppendPersonJob(node.id, flag, flag2, 1);
                                }
                                else {
                                    if (flag == 1 && flag2 == 1) { targetWithSub1 = false; }
                                    else if (flag == 1 && flag2 == 2) { batchTargetWithSub1 = false; }
                                    else if (flag == 2 && flag2 == 1) { targetWithSub2 = false; }
                                    else if (flag == 2 && flag2 == 2) { batchTargetWithSub2 = false; }
                                    nodeAppendPersonJob(node.id, flag, flag2, 0);
                                }
                               
                            })
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },

                    check: {            //设置为单选按钮
                        enable: true,
                        chkStyle: "radio",
                        radioType: "all",
                        chkboxType: { "Y": "", "N": "" }
                    },

                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    //部门选择树
    function nodeTreeOrgLoadCheck(flag2) {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#node-Ztree"), $.extend({
                    callback: {
                        onNodeCreated: function (event, treeId, treeNode) {
                            var t = $.fn.zTree.getZTreeObj(treeId);
                            if (flag2 == 1) {
                                for (var i = 0; i < targetIdArray3.length; i++) {
                                    if (targetIdArray3[i] == treeNode.id) {
                                        t.checkNode(treeNode, true, false);
                                    }
                                }

                            }
                            else if (flag2 == 2) {
                                for (var i = 0; i < batchTargetIdArray3.length; i++) {
                                    if (batchTargetIdArray3[i] == treeNode.id) {
                                        t.checkNode(treeNode, true, false);
                                    }
                                }
                            }
                        },
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        beforeCheck: function (id, node) {

                            if (node.checked == false && $("#node-countersign").attr("term") == 0 && flag2 == 1 && targetIdArray3.length >= 1 && nodeType == 3) {//在审批类型节点，且状态为无时单选
                                //   ncUnits.alert("操作类型为审批时只能单选");
                                return false;
                            }

                        },
                        onCheck: function (e, id, node) {
                            //选中事件
                            if (flag2 == 1) {
                                var $chooseName = $("#node_addOperatorDepart").siblings(".chooseName");
                            }
                            else if (flag2 == 2) {
                                var $chooseName = $("#node_addProposerDepart").siblings(".chooseName");
                            }
                            if (node.checked) {
                                if (flag2 == 1) {
                                    targetIdArray3[targetIdArray3.length] = node.id;
                                    targetNameArray3[targetNameArray3.length] = node.name;
                                    var chooseNameHtml = "";
                                    for(var i=0;i<targetIdArray3.length;i++){
                                        chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray3[i] + "<span class='glyphicon glyphicon-remove nodeZtreeCheck' id='" + targetIdArray3[i] + "'></span></span>";
                                    }                       
                                    $chooseName.html(chooseNameHtml);
                                }
                                else if (flag2 == 2) {
                                    batchTargetIdArray3[batchTargetIdArray3.length] = node.id;
                                    batchTargetNameArray3[batchTargetNameArray3.length] = node.name;
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < batchTargetIdArray3.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray3[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchCheck' id='" + batchTargetIdArray3[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                    
                                }

                            }
                            else {
                                //移除数组中的对应元素
                                if (flag2 == 1) {
                                    for (var j = 0; j < targetIdArray3.length; j++) {
                                        if (targetIdArray3[j] == node.id) {
                                            break;
                                        }
                                    }
                                    targetIdArray3.splice(j, 1);
                                    targetNameArray3.splice(j, 1);
                                    var chooseNameHtml="";
                                    for (var i = 0; i < targetIdArray3.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray3[i] + "<span class='glyphicon glyphicon-remove nodeZtreeCheck' id='" + targetIdArray3[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                }
                                else if (flag2 == 2) {
                                    for (var j = 0; j < batchTargetIdArray3.length; j++) {
                                        if (batchTargetIdArray3[j] == node.id) {
                                            break;
                                        }
                                    }
                                    batchTargetIdArray3.splice(j, 1);
                                    batchTargetNameArray3.splice(j, 1);
                                     
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < batchTargetIdArray3.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray3[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchCheck' id='" + batchTargetIdArray3[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                }


                            }
                            $(".nodeZtreeCheck").click(function () {                           
                                if (batchOrNot == 1 && ztreeFlag==3) {
                                    var tree = $.fn.zTree.getZTreeObj("node-Ztree");
                                    var checknode = tree.getNodeByParam("id", $(this).attr("id"), null);
                                    if (checknode != null) {
                                        tree.checkNode(checknode, false, false);                                                                 
                                    }
                                }
                                for (var j = 0; j < targetIdArray3.length; j++) {
                                    if (targetIdArray3[j] == $(this).attr("id")) {
                                        break;
                                    }
                                }
                                targetIdArray3.splice(j, 1);
                                targetNameArray3.splice(j, 1);
                                $(this).parent("span").remove();
                            })
                            $(".nodeZtreebatchCheck").click(function () {
                                if (batchOrNot == 2 && ztreeFlag == 3) {
                                    var tree = $.fn.zTree.getZTreeObj("node-Ztree");
                                    var checknode = tree.getNodeByParam("id", $(this).attr("id"), null);
                                    if (checknode != null) {
                                        tree.checkNode(checknode, false, false);                                                                     
                                    }
                                }               
                                for (var j = 0; j < batchTargetIdArray3.length; j++) {
                                        if (batchTargetIdArray3[j] == $(this).attr("id")) {
                                            break;
                                         }
                                }
                                batchTargetIdArray3.splice(j, 1);
                                batchTargetNameArray3.splice(j, 1);
                                $(this).parent("span").remove();
                            })
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false,
                        selectedMulti: false
                    },
                    check: {
                        autoCheckTrigger: false,
                        enable: true,
                        chkStyle: "checkbox",
                        chkboxType: { "Y": "", "N": "" } //Y：勾选，p：关联父节点，s：关联子节点，N：取消勾选
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }

    //人员列表 岗位列表   flag:1 表示根据架构选择岗位  flag:2 根据架构选择人员
    function nodeAppendPersonJob(id, flag, flag2, withSub) {
        var url, argus;
        if (flag == 2) {      //人员列表
            if (flag2 == 1) { targetorganizationId2 = id; }
            else if (flag2 == 2) { batchTargetorganizationId2 = id; }
            url = "/Shared/GetUserList";
            if (withSub == 1) {
                argus = { withSub: 1, organizationId: id, withUser: true };
            }
            else {
                argus = { withSub: 0, organizationId: id, withUser: true };
            }
          
        } else {             //岗位列表
            if (flag2 == 1) { targetorganizationId1 = id; }
            else if (flag2 == 2) { batchTargetorganizationId1 = id; }
            url = "/Shared/GetStationList";          
            if (withSub == 1 && flag2 == 1) {
                argus = { withSub: 1, organizationId: id ,withNum:true};
            }
            else if (withSub == 0 && flag2 == 1) {
                argus = { withSub: 0, organizationId: id, withNum: true };
            }
            else if (withSub == 1 && flag2 == 2) {
                argus = { withSub: 1, organizationId: id};
            }
            else {
               argus = { withSub: 0, organizationId: id };
            }

        }
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data: argus,
            success: rsHandler(function (data) {
                $("#nodeChooseList ul").empty();
                $.each(data, function (i, v) {
                    var $li = $("<li></li>");
                    var $label = $("<div class='checkbox'><label></label></div>");
                    var $checkBox, $span;
                    if (flag == 2) {       //人员列表
                        $checkBox = $("<input type='checkbox' class='nodePersonCheck' value=" + v.userId + ">");
                        $span = $("<span  title=" + v.userName + "-" + v.organizationName + ">" + v.userName + "</span>-<span>" + v.organizationName + "</span>");
                    } else {               //岗位列表
                        if (v.approval == 0 && (nodeType == 2 || nodeType == 3) && flag2 == 1 && $("#node-countersign").attr("term")!=2) {
                            $label = $("<div class='checkbox'><label class='disabled'></label></div>");
                            $checkBox = $("<input type='checkbox' disabled class='nodeJobCheck' value=" + v.stationId + ">");
                            if (v.userNum == 0) {
                                $span = $("<span  class='zero' title=" + v.stationName + "-" + v.organizationName + "(" + v.userNum + ")" + " >" + v.stationName + "</span>-<span>" + v.organizationName + "(" + v.userNum + ")" + "</span>");
                            }
                            else {
                                $span = $("<span  title=" + v.stationName + "-" + v.organizationName + "(" + v.userNum + ")" + " >" + v.stationName + "</span>-<span>" + v.organizationName + "(" + v.userNum + ")" + "</span>");
                            }

                        }
                        else if (v.approval != 0 && (nodeType == 2 || nodeType == 3) && flag2 == 1 && $("#node-countersign").attr("term") != 2) {
                            $checkBox = $("<input type='checkbox'  class='nodeJobCheck' value=" + v.stationId + ">");
                            if (v.userNum == 0) {
                                $span = $("<span class='zero' style='font-weight: bolder;' title=" + v.stationName + "-" + v.organizationName + "(" + v.userNum + ")" + " >" + v.stationName + "</span>-<span style='font-weight: bolder;'>" + v.organizationName + "(" + v.userNum + ")" + "</span>");
                            }
                            else {
                                $span = $("<span style='font-weight: bolder;' title=" + v.stationName + "-" + v.organizationName + "(" + v.userNum + ")" + " >" + v.stationName + "</span>-<span style='font-weight: bolder;'>" + v.organizationName + "(" + v.userNum + ")" + "</span>");
                            }
                          
                        }
                        else if (flag2 == 1) {
                            $checkBox = $("<input type='checkbox'  class='nodeJobCheck' value=" + v.stationId + ">");
                            if (v.userNum == 0) {
                                $span = $("<span  class='zero'  title=" + v.stationName + "-" + v.organizationName + "(" + v.userNum + ")" + " >" + v.stationName + "</span>-<span>" + v.organizationName + "(" + v.userNum + ")" + "</span>");

                            }
                            else {
                                $span = $("<span title=" + v.stationName + "-" + v.organizationName + "(" + v.userNum + ")" + " >" + v.stationName + "</span>-<span>" + v.organizationName + "(" + v.userNum + ")" + "</span>");

                            }
                          
                        }
                        else {
                            $checkBox = $("<input type='checkbox'  class='nodeJobCheck' value=" + v.stationId + ">");
                            $span = $("<span title=" + v.stationName + "-" + v.organizationName + " >" + v.stationName + "</span>-<span>" + v.organizationName + "</span>");

                        }
                    }
                    $checkBox.click(function () {

                        //复选框单击事件
                        if (flag == 1 && flag2 == 1) {
                            var $chooseName = $("#node_addOperatorPost").siblings(".chooseName")
                        }
                        else if (flag == 1 && flag2 == 2) {
                            var $chooseName = $("#node_addProposerPost").siblings(".chooseName")
                        }
                        else if (flag == 2 && flag2 == 1) {
                            var $chooseName = $("#node_addOperator").siblings(".chooseName")
                        }
                        else if (flag == 2 && flag2 == 2) {
                            var $chooseName = $("#node_addProposer").siblings(".chooseName")
                        }
                        if ($checkBox.is(":checked")) {
                            if ($("#node-countersign").attr("term") == 0 && flag2 == 1 && flag == 1 && targetIdArray1.length >= 1 && nodeType == 3) {
                                //在审批类型节点，且操作类型为审批时     操作者岗位    单选
                           
                                if ($checkBox.siblings("span:eq(0)").hasClass("zero")) {
                                    $checkBox.prop("checked", false);
                                    ncUnits.alert("该岗位人数为0，不能选择");
                                }
                                else {
                                    
                                    $("#nodeChooseList li input").each(function () {
                                       if ($(this).val() == targetIdArray1[0]) {
                                                $(this).prop("checked", false);
                                                
                                       }
                                    })
                                    targetIdArray1.length = 0;
                                    targetNameArray1.length = 0;
                                    $checkBox.prop("checked", true);
                                    targetIdArray1[targetIdArray1.length] = $checkBox.val();
                                    targetNameArray1[targetNameArray1.length] = $checkBox.siblings("span").text();
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < targetIdArray1.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList1' id='" + targetIdArray1[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                }
                                
                                
                            }

                            else if ($("#node-countersign").attr("term") == 0 && flag2 == 1 && flag == 2 && targetIdArray2.length >= 1 && nodeType == 3) {
                                //在审批类型节点，且操作类型为审批时  操作者   单选                                                            
                                    $("#nodeChooseList li input").each(function () {
                                        if ($(this).val() == targetIdArray2[0]) {
                                            $(this).prop("checked", false);

                                        }
                                    })
                                    targetIdArray2.length = 0;
                                    targetNameArray2.length = 0;
                                    $checkBox.prop("checked", true);
                                    targetIdArray2[targetIdArray2.length] = $checkBox.val();
                                    targetNameArray2[targetNameArray2.length] = $checkBox.siblings("span:eq(0)").text() + "-" + $checkBox.siblings("span:eq(1)").text();
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < targetIdArray2.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList2' id='" + targetIdArray2[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                
                              
                            }
                            else {
                                if (flag == 1 && flag2 == 1) {
                    
                                    if (nodeType != 1 && $checkBox.siblings("span:eq(0)").hasClass("zero")) {
                                        $checkBox.prop("checked", false);
                                        ncUnits.alert("该岗位没有人，请不要选择");
                                    }
                                    else {
                                        targetIdArray1[targetIdArray1.length] = $checkBox.val();
                                        targetNameArray1[targetNameArray1.length] = $checkBox.siblings("span:eq(0)").text() + "-" + $checkBox.siblings("span:eq(1)").text();
                                        var chooseNameHtml = "";
                                        for (var i = 0; i < targetIdArray1.length; i++) {
                                            chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList1' id='" + targetIdArray1[i] + "'></span></span>";
                                        }
                                        $chooseName.html(chooseNameHtml);
                                    }
                                                                 
                                }
                                else if (flag == 1 && flag2 == 2) {
                                   ///////////////////////
                                    batchTargetIdArray1[batchTargetIdArray1.length] = $checkBox.val();
                                    batchTargetNameArray1[batchTargetNameArray1.length] = $checkBox.siblings("span:eq(0)").text() + "-" + $checkBox.siblings("span:eq(1)").text();
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < batchTargetIdArray1.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchList1' id='" + batchTargetIdArray1[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                   // console.log("batchTargetNameArray1");
                                   // console.log(batchTargetNameArray1);
                                }
                                else if (flag == 2 && flag2 == 1) {
                                    targetIdArray2[targetIdArray2.length] = $checkBox.val();
                                    targetNameArray2[targetNameArray2.length] = $checkBox.siblings("span:eq(0)").text() + '-' + $checkBox.siblings("span:eq(1)").text();
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < targetIdArray2.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList2' id='" + targetIdArray2[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                }
                                else if (flag == 2 && flag2 == 2) {
                                   // console.log("batchTargetIdArray2");
                                  //  console.log(batchTargetIdArray2);
                                    batchTargetIdArray2[batchTargetIdArray2.length] = $checkBox.val();
                                    batchTargetNameArray2[batchTargetNameArray2.length] = $checkBox.siblings("span:eq(0)").text() + '-' + $checkBox.siblings("span:eq(1)").text();
                                    var chooseNameHtml = "";
                                    for (var i = 0; i < batchTargetIdArray2.length; i++) {
                                        chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchList2' id='" + batchTargetIdArray2[i] + "'></span></span>";
                                    }
                                    $chooseName.html(chooseNameHtml);
                                }
                            }

                        }
                        else {
                            if (flag == 1 && flag2 == 1) {
                                for (var j = 0; j < targetIdArray1.length; j++) {
                                    if (targetIdArray1[j] == $checkBox.val()) {
                                        break;
                                    }
                                }
                                targetIdArray1.splice(j, 1);
                                targetNameArray1.splice(j, 1);
                                var chooseNameHtml = "";
                                for (var i = 0; i < targetIdArray1.length; i++) {
                                    chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList1' id='" + targetIdArray1[i] + "'></span></span>";
                                }
                                $chooseName.html(chooseNameHtml);
                            }
                            else if (flag == 1 && flag2 == 2) {
                                for (var j = 0; j < batchTargetIdArray1.length; j++) {
                                    if (batchTargetIdArray1[j] == $checkBox.val()) {
                                        break;
                                    }
                                }
                                batchTargetIdArray1.splice(j, 1);
                                batchTargetNameArray1.splice(j, 1);
                                var chooseNameHtml = "";
                                for (var i = 0; i < batchTargetIdArray1.length; i++) {
                                    chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray1[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchList1' id='" + batchTargetIdArray1[i] + "'></span></span>";
                                }
                                $chooseName.html(chooseNameHtml);
                            }
                            else if (flag == 2 && flag2 == 1) {
                                for (var j = 0; j < targetIdArray2.length; j++) {
                                    if (targetIdArray2[j] == $checkBox.val()) {
                                        break;
                                    }
                                }
                                targetIdArray2.splice(j, 1);
                                targetNameArray2.splice(j, 1);
                                var chooseNameHtml = "";
                                for (var i = 0; i < targetIdArray2.length; i++) {
                                    chooseNameHtml = chooseNameHtml + "<span>" + targetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreeList2' id='" + targetIdArray2[i] + "'></span></span>";
                                }
                                $chooseName.html(chooseNameHtml);
                            }
                            else if (flag == 2 && flag2 == 2) {
                                for (var j = 0; j < batchTargetIdArray2.length; j++) {
                                    if (batchTargetIdArray2[j] == $checkBox.val()) {
                                        break;
                                    }
                                }
                                batchTargetIdArray2.splice(j, 1);
                                batchTargetNameArray2.splice(j, 1);
                                var chooseNameHtml = "";
                                for (var i = 0; i < batchTargetIdArray2.length; i++) {
                                    chooseNameHtml = chooseNameHtml + "<span>" + batchTargetNameArray2[i] + "<span class='glyphicon glyphicon-remove nodeZtreebatchList2' id='" + batchTargetIdArray2[i] + "'></span></span>";
                                }
                                $chooseName.html(chooseNameHtml);
                            }
                        }
                        $(".nodeZtreeList1").click(function () {
                            var id = $(this).attr("id");
                            $this = $(this);
                            if (batchOrNot == 1 && ztreeFlag == 1) {
                                $("#nodeChooseList li input").each(function () {
                                    if ($(this).val() == id) {
                                        $(this).prop("checked", false);                                                        
                                    }
                                })
                            }
                            for (var j = 0; j < targetIdArray1.length; j++) {
                                if (targetIdArray1[j] == id) {
                                    break;
                                }
                            }
                            targetIdArray1.splice(j, 1);
                            targetNameArray1.splice(j, 1);
                            $this.parent("span").remove();
                        })
                        $(".nodeZtreeList2").click(function () {
                            var id = $(this).attr("id");
                            $this = $(this);
                            if (batchOrNot == 1 && ztreeFlag == 2) {
                                $("#nodeChooseList li input").each(function () {
                                    if ($(this).val() == id) {
                                        $(this).prop("checked", false);
                                    }
                                })
                            }
                            for (var j = 0; j < targetIdArray2.length; j++) {
                                if (targetIdArray2[j] ==id) {
                                    break;
                                }
                            }
                            targetIdArray2.splice(j, 1);
                            targetNameArray2.splice(j, 1);
                            $this.parent("span").remove();
                        })
                        $(".nodeZtreebatchList1").click(function () {
                            var id = $(this).attr("id");
                            $this = $(this);
                            if (batchOrNot == 2 && ztreeFlag == 1) {
                                $("#nodeChooseList li input").each(function () {
                                    if ($(this).val() == id) {
                                        $(this).prop("checked", false);
                                    }
                                })
                            }
                            for (var j = 0; j < batchTargetIdArray1.length; j++) {
                                if (batchTargetIdArray1[j] == id) {
                                    break;
                                }
                            }
                            batchTargetIdArray1.splice(j, 1);
                            batchTargetNameArray1.splice(j, 1);
                            $this.parent("span").remove();
                        })
                        $(".nodeZtreebatchList2").click(function () {
                            var id = $(this).attr("id");
                            $this = $(this);
                            if (batchOrNot == 2 && ztreeFlag == 2) {
                                $("#nodeChooseList li input").each(function () {
                                    if ($(this).val() == id) {
                                        $(this).prop("checked", false);
                                    }
                                })
                            }
                            for (var j = 0; j < batchTargetIdArray2.length; j++) {
                                if (batchTargetIdArray2[j] == id) {
                                    break;
                                }
                            }
                            batchTargetIdArray2.splice(j, 1);
                            batchTargetNameArray2.splice(j, 1);
                            $this.parent("span").remove();
                        })
                    });
                    
                    $label.find("label").append([$checkBox, $span]);
                    $li.append($label);
                    $("#nodeChooseList  ul").append($li);
                })
                if (flag == 1 && flag2 == 1 && targetIdArray1.length != 0) {
                    for (var i = 0; i < targetIdArray1.length; i++) {
                        $("#nodeChooseList ul li input").each(function () {
                            if ($(this).val() == targetIdArray1[i]) {
                                $(this).prop("checked", "true");
                            }
                        })
                    }
                }
                else if (flag == 1 && flag2 == 2 && batchTargetIdArray1.length != 0) {
                    for (var i = 0; i < batchTargetIdArray1.length; i++) {
                        $("#nodeChooseList ul li input").each(function () {
                            if ($(this).val() == batchTargetIdArray1[i]) {
                                $(this).prop("checked", "true");
                            }
                        })
                    }

                }
                else if (flag == 2 && flag2 == 1 && targetIdArray2.length != 0) {
                    for (var i = 0; i < targetIdArray2.length; i++) {
                        $("#nodeChooseList ul li input").each(function () {
                            if ($(this).val() == targetIdArray2[i]) {
                                $(this).prop("checked", "true");
                            }
                        })
                    }
                }
                else if (flag == 2 && flag2 == 2 && batchTargetIdArray2.length != 0) {
                    for (var i = 0; i < batchTargetIdArray2.length; i++) {
                        $("#nodeChooseList ul li input").each(function () {
                            if ($(this).val() == batchTargetIdArray2[i]) {
                                $(this).prop("checked", "true");
                            }
                        })
                    }

                }



            })
        });
    }

    //设置右侧弹出框的位置
    $(".node-rightModal").css({
        "left": ($(".node-rightModal").parents('.modal-dialog').width() - 5)
    });
   // var jumpSign = GetQueryString("jumpSign");
    if (jumpSign == 1) {
        $("#nav_flow_edit").trigger("click");
    }
    /* ---------------------------节点设置 ---------------------------结束 */


    /* 流程图 开始 */
    $("#picSetTab").click(function (e) {
        e.preventDefault();
        //切换判断
        var flag = false;
        if (flowTab == 1) {        //如果当前页面是节点设置画面
            flag = nodeModefyOrNot();
        } else if (flowTab == 2) {         //流程设置画面
            flag = modifyOrFalse();
        }
        if (flag == true) {
            ncUnits.confirm({
                title: '提示',
                html: '你的设置还没保存,确定要退出当前页面?',
                yes: function (layer_confirm) {
                    $("#picSetTab").tab('show');
                    $("#flow_edit .nav-item").removeClass("green_color");
                    $("#picSetTab").addClass("green_color");
                    flowTab = 3;
                    layer.close(layer_confirm);
		            $addNodeSwitch.hide();
                    $.ajax({
                        url: "/FlowChart/DisplayTemplateFlowChart",
                        type: "post",
                        dataType: "json",
                        data: { id: templateId },
                        success: rsHandler(function (data) {
                            $("#con_flowchart").flowChart(data);
                        })
                    });
                }
            });
        } else {
            $("#picSetTab").tab('show');
            $("#flow_edit .nav-item").removeClass("green_color");
            $("#picSetTab").addClass("green_color");
            flowTab = 3;
            $addNodeSwitch.hide();
            $.ajax({
                url: "/FlowChart/DisplayTemplateFlowChart",
                type: "post",
                dataType: "json",
                data: { id: templateId },
                success: rsHandler(function (data) {
                    $("#con_flowchart").flowChart(data);
                })
            });
        }
    });
    /* 流程图 结束 */

    /* 流程编辑 结束 */
 
});