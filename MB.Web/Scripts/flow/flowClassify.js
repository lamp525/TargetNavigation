/**
 * Created by DELL on 2015/6/24.
 */
$(function () {
    var admin;

    var folder = null,
        treetype = 2,
        result = []
    var click = true;

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
    /*加载流程分类列表   开始*/
    //loadDocuments();
    $(".addFlowClassify").click(function () {
        $("#classifyModify_modal").modal('show');
        //清空输入框
        argus.categoryId = null;//修改
        $("#classifyModify_modal .classifyDescrib").val("");
        $("#classifyModify_modal .classifyName").val("");
        $("#syterm_form").off("click");
    })
    $("a#flowType").click(function () {
        //$("#syterm_form").off("click");
        //$("#userdefined_form").off("click");
        $("span.glyphicon.glyphicon-menu-down.form2").removeClass('active');
        $("a.glyphicon.glyphicon-plus.addFormClassify").removeClass("addFormClassify").addClass("addFlowClassify");
        loadDocuments();
        $(".glyphicon-plus").show();
        $(".addFormClassify").off("click");
        var flag1;
        $(".addFlowClassify").click(function () {
            flag1 = 1;
            $("#formBuild_modal").modal('hide').on("hidden.bs.modal", function () {
                if (flag1 == 1) {
                    $("#classifyModify_modal").modal('show');
                    flag1 = 0;
                }
            }); 
            argus.categoryId = null;//修改
            $("#classifyModify_modal .classifyDescrib").val("");
            $("#classifyModify_modal .classifyName").val("");
            $("#syterm_form").off("click");//修改

        })

    })
    var argus = {
        categoryId: null,       //分类ID
        categoryName: [],  //分类名
        comment: []        //描述 ,
    };//编辑分类参数列表
    var argus2 = {
        categoryId: null,  //分类名
        orderNum: []       //排序
    };//分类排序参数列表

    function loadDocuments() {
        var add_lodi = getLoadingPosition(".documents");
        $(".panel-footer-userdefined").hide();
        $.ajax({
            type: "post",
            url: "/TemplateCategory/GetTemplateList",
            dataType: "json",
            complete: rcHandler(function () {
                add_lodi.remove();
            }),
            success: rsHandler(function (data) {
                var $documents = $(".documents");
                var $docList = $(".row-docList");
                var mode = 0;
                var orderNum = 0;//修改
                $documents.empty();
                $.each(data, function (i, v) {
                    ++orderNum;//修改
                    var $content = $("<div class='col-xs-3 classifyupdate'id='" + v.categoryId + "'></div>");
                    var $col = $("<div class='susclass'></div>");
                    var $number = $("<div class='shuzikuang'>" + orderNum + "</div>");
                    var $operate = $("<div class='operate'></div>");
                    var $operateDiv = $("<div class='operateDiv'></div>");
                    var $operateBg = $("<span class='operateBg'></span>");
                    var $operateText = $("<div class='operateText'></div>");
                    var $modification = $("<button type='button' class='btn modification' data-toggle='modal' data-target='#classifyModify_modal'><span>修改</span></button>");
                    var $delete = $("<button class='btn delete' data-toggle='modal' data-target='#' ><span>删除</span></button>");
                    var $detail = $("<button  class='btn detail' data-toggle='modal'data-target='#classifyDetail_modal'><span>详情</span></button>");
                    if (v.system == 1) {
                        $col.addClass("systemsusclass");
                        $operateText.append($detail);
                    }
                    else {
                        $operateText.append($modification, $delete, $detail);
                    }
                    $operateDiv.append($operateBg, $operateText);
                    $operate.append($operateDiv);
                    var $wordPosition = $("<div class='wordPosition'><label>" + v.categoryName + "</label><br/><label class='label2 wrapper'  data-toggle='tooltip'  title='" + v.comment + "'>" + v.comment + "</label></div>");
                    $col.append($number, $wordPosition, $operate);
                    $content.append($col);
                    $content.appendTo($documents);

                    //剩下应该是其他一些方法
                    /*修改----- 描述超长折叠 开始 */
                    $(".wrapper").dotdotdot();
                    /* 修改-----描述超长折叠 结束 */

                    /* 绿条hover效果开始 */
                    $col.hover(
                        function () {
                            $(this).find('.operate').css('display', 'block');
                            //当鼠标放上去的时候,程序处理
                        },
                        function () {
                            $(this).find('.operate').css('display', 'none');
                            //当鼠标离开的时候,程序处理
                        });
                    /* 绿条hover效果 结束 */
                    /* 点击绿条中的详情 获取分类详情  开始 */
                    $detail.click(function () {
                        clearClassifymodal();//清空原有input框中的内容
                        categoryId = v.categoryId;
                        $.ajax({
                            type: "post",
                            url: "/TemplateCategory/GetTemplateInfoById",
                            dataType: "json",
                            data: { tempcaregoryId: categoryId },
                            success: rsHandler(function (data) {
                                // if (data == "ok") {
                                // loadDocuments(); 
                                argus.categoryId = data.categoryId;
                                $("#classifyDetail_modal .classifyName").val(data.categoryName);
                                $("#classifyDetail_modal .classifyDescrib").val(data.comment);
                                $("#classifyModify_modal .classifyName").val(data.categoryName);
                                $("#classifyModify_modal .classifyDescrib").val(data.comment);
                                //$("#classifyDetail_modal  .btnDetailSure").off("click");
                                if (data.system == true) {
                                    $("#classifyDetail_modal .modal-footer").css('display', 'none');
                                }
                                else {
                                    $("#classifyDetail_modal .modal-footer").css('display', 'block');
                                }
                                //   }
                            })
                        });
                    });
                    /* 点击绿条中的详情 获取分类详情  结束 */
                    /* 点击绿条中的修改 获取分类详情  开始 */
                    $modification.click(function () {

                        clearClassifymodal();//清空原有input框中的内容
                        categoryId = v.categoryId;
                        $.ajax({
                            type: "post",
                            url: "/TemplateCategory/GetTemplateInfoById",
                            dataType: "json",
                            data: { tempcaregoryId: categoryId },
                            success: rsHandler(function (data) {
                                // loadDocuments(); 
                                argus.categoryId = data.categoryId;
                                $("#classifyModify_modal .classifyName").val(data.categoryName);
                                $("#classifyModify_modal .classifyDescrib").val(data.comment);

                            })
                        });
                    });
                    /* 点击绿条中的修改 获取分类详情  开始 */

                    /* 删除 开始 */
                    $delete.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '你确认要删除这个分类吗？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                categoryId = v.categoryId;
                                $.ajax({
                                    type: "post",
                                    url: "/TemplateCategory/DeleteCaregoryById",
                                    dataType: "json",
                                    data: { CaregoryId: categoryId },
                                    success: rsHandler(function (data) {
                                        if (data == 2) {
                                            loadDocuments();//删除后，根据数据库重新加载页面
                                            ncUnits.alert("删除成功!");
                                        } else if (data == 3) {
                                            ncUnits.alert("无法删除，已使用分类!");
                                        }

                                    })
                                });
                            },
                            no: function (layer_confirm) {
                                layer.close(layer_confirm);
                            }
                        });
                    });
                    /* 删除 结束 */

                });
                //    var oldIndex;
                //  var newIndex; 
                /* 清空输入框内容开始  */
                function clearClassifymodal() {
                    $("#classifyDetail_modal .classifyName").val("");
                    $("#classifyDetail_modal .classifyDescrib").val("");
                    //清空输入框
                    $("#classifyModify_modal .classifyDescrib").val("");
                    $("#classifyModify_modal .classifyName").val("");
                };
                /* 清空输入框内容结束  */
                /* 添加新建分类的div 开始*/
                var $newadd = $("<div class='newadd col-xs-3 newaddClassifycol'></div>");
                var $add = $("<div class='add'></div>");
                var $addPicture = $("<button class='addPicture' id='addFlowClassify'>新建分类</button>");
                $add.append($addPicture);
                $newadd.append($add);
                $newadd.appendTo($documents);
                /* 添加新建分类的div 结束*/
                $("#addFlowClassify").click(function () {
                    $("#classifyModify_modal").modal('show');
                    //清空输入框
                    $("#classifyModify_modal .classifyDescrib").val("");
                    $("#classifyModify_modal .classifyName").val("");
                    //利用存储过程获取categoryId,故此处设置为“”
                    argus.categoryId = null;
                    //     argus.orderNum=$(".CategoryList .col-xs-3.newaddClassifycol").index()+1;

                });
                /*点击流程分类左边的+号新建分类 结束*/
            })
        });
    }

    if ($(".flowType").children().hasClass('active') || ($(".flowType").click())) {


        loadDocuments();
        
        /* 拖动排序开始 */
        $(".documents").sortable({
            cursor: "move",
            items: ".classifyupdate",
            opacity: 0.6,
            revert: true,
            placeholder: "ui-state-highlight",

            update: function (event, ui) {
                var arrayObj = new Array();
                $(".documents >div").each(function () {
                    if ($(this).attr("id") != undefined) {    //将全部分类的id和orderNum信息保存在数组arrayObj中
                        var argus2 = {
                            categoryId: [],  //分类名
                            orderNum: []       //排序
                        };//分类排序参数列表
                        argus2.categoryId = $(this).attr("id");
                        argus2.orderNum = $(this).index() + 1;
                        arrayObj.push(argus2);
                    }
                });
                $.ajax({
                    type: "post",
                    url: "/TemplateCategory/UpateOldNum",
                    dataType: "json",
                    data: { data: JSON.stringify(arrayObj) },
                    success: rsHandler(function () {
                        ncUnits.alert("排序成功");
                        loadDocuments();///添加新分类成功后 根据数据库重新加载页面
                    })
                });
            }
        });
        /* 拖动排序结束  */
        /*加载流程分类列表  结束*/
        /* 点击分类编辑弹出框中的确定 更新数据  开始 */
        $("#classifyModify_modal .btnModifySure").click(function () {
            var Filename = $.trim($("#classifyModify_modal .classifyName").val());
            if (Filename == "") {
                ncUnits.alert("流程分类名不能为空!");
                return;
            }
            else if (Filename.length > 20) {
                ncUnits.alert("流程分类名不能超过20字符!");
                return;
            }
            var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
            if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                ncUnits.alert("流程分类名存在非法字符!");
                return;
            }
            var Classifydescrib = $.trim($("#classifyModify_modal .classifyDescrib").val());
            if (Classifydescrib.length > 250) {
                ncUnits.alert("描述不能超过250字符!");
                return;
            }
            if (Classifydescrib.indexOf('null') >= 0 || Classifydescrib.indexOf('NULL') >= 0 || Classifydescrib.indexOf('&nbsp') >= 0 || reg.test(Classifydescrib) || Classifydescrib.indexOf('</') >= 0) {
                ncUnits.alert("描述中存在非法字符!");
                return;
            }
            argus.categoryName = $("#classifyModify_modal .classifyName").val();
            argus.comment = $("#classifyModify_modal .classifyDescrib").val();
            if (click == true) {
                click = false;
                $.ajax({
                    type: "post",
                    url: "/TemplateCategory/AddNewCaregory",
                    dataType: "json",
                    data: { data: JSON.stringify(argus) },
                    success: rsHandler(function () {
                        if (argus.categoryId == null) {
                            $('#classifyModify_modal').modal('hide');
                            click = true;
                            ncUnits.alert("新建成功!");
                            loadDocuments();
                        } else {
                            $('#classifyModify_modal').modal('hide');
                            click = true;
                            ncUnits.alert("更新成功!");
                            loadDocuments();
                        }
                    })
                });
            }


        });


        var flag;
        $("#classifyDetail_modal .btnDetailSure").click(function () {
            flag = 1;
            $("#classifyDetail_modal").modal('hide').on("hidden.bs.modal", function () {
                if (flag == 1) {
                    $("#classifyModify_modal").modal('show');
                    flag = 0;
                }

            });

        });
    }





 





})
$(function () { $("[data-toggle='tooltip']").tooltip(); });