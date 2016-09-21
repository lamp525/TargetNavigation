//@ sourceURL=flowClassify-form.js
/**
 * Created by DELL on 2015/7/7.
 */
$(function () {
   
    var systemOr = 1;
    var $forms = $(".form-list");
    $('#power-span span:eq(0)').text("");
    $("#input_filename").val("")
    $("#input_filedes").val("");
    //$(".addFlowClassify").click(function () {
    //    $("#classifyModify_modal").modal('show');
    //    //清空输入框
    //    $("#classifyModify_modal .classifyDescrib").val("");
    //    $("#classifyModify_modal .classifyName").val("");
    //    $("#syterm_form").off("click");
    //})
    $("a#formManage").parent('li').removeClass('active');
    //$("#syterm_form").off("click");
    //$("#userdefined_form").off("click");
    $("a.glyphicon.glyphicon-plus.addFormClassify").removeClass("addFormClassify").addClass("addFlowClassify");
    //$("a#flowType").click(function () {
    //    //$("#syterm_form").off("click");
    //    //$("#userdefined_form").off("click");
    //    $("span.glyphicon.glyphicon-menu-down.form2").removeClass('active');
    //    $("a.glyphicon.glyphicon-plus.addFormClassify").removeClass("addFormClassify").addClass("addFlowClassify");
    //    $(".glyphicon-plus").show();
    //    $(".addFormClassify").off("click");
    //    $(".addFlowClassify").click(function () {
    //        $("#formBuild_modal").modal('hide');
    //        $("#classifyModify_modal").modal('show');
    //    })

    //})


    $(".des").dotdotdot();

    //获取列表参数（含筛选，排序）
    var argus = {
        type: true,  //表单类型 1、系统表单 0、自定义表单
        hovertype: true,  //表单类型 1、系统表单 0、自定义表单
        clickType: true,
        categoryId: null  //表单分类ID
    };
    var testflow_templateId;
    var addflow_templateId;
    var templates = [];
    var categorys = [];
    var thicategory = [];
    var thisclickCaregory = null;
    var thisCaregoryName = null;
    var isMore = true;
    if ($(".formManage").children().hasClass('active') || ($(".formManage").click())) {

        function loadForms() {
            var orderNum = 0;//修改
            var list_lodi = getLoadingPosition($("#Load-position") );//.form-list
            $.ajax({
                type: "post",
                url: "/Template/GetTemplateList",
                dataType: "json",
                data: { cateoryId: argus.categoryId, system: argus.type },
                complete: rcHandler(function () {
                    list_lodi.remove();
                }),
                success: rsHandler(function (data) {
                    console.log("请求参数" + "ID" + argus.categoryId + "表单类型" + argus.type);
                    console.log(data);
                    console.log("cs 1+data.length" + data.length);
                    var $formList = $(".form-list");
                    $formList.empty();
                    //   var categoryId=argus.categoryId;
                    $.each(data, function (n, value) {
                       
                        if (argus.type == value.system) {
                            ++orderNum;//修改
                            var $col = $("<div class='col-xs-3' ></div>");
                            var $cell = $("<div class=cell term=" + value.templateId + " ></div>");
                            var $smallImage = $("<div class='pull-left img-circle templateId'>" + "<span class='content-center'>" + orderNum + "</span></div>");
                            var $content = $("<div class='info-list'></div>");
                            var $title = $("<div class='title'>" + value.templateName + "</div>");
                            var $des = $("<div class='des' title='" + value.description + "'>" + value.description + "</div>");
                            var $operate = $("<div class='operate'></div>");
                            var $btns = $("<div class='btns'><ul class='list-inline'></ul></div>");
                            var $formTest = $("<li><a class='form-test' href='#'>测试</a></li>");//测试
                            var $formProcess = $("<li><a class='form-process' href='/TemplateEdit/TemplateEdit?templateId=" + value.templateId + "&jumpSign=1&systemOrFalse=" + systemOr + "'>流程</a></li>");//切换到流程设置画面
                            var $systermCopy = $("<li><a class='systermCopy'  data-toggle='modal' data-target='#formCopy_modal'>复制</a></li>");
                            var $formBatch = $("<li class='dropdown'>" +
                            "<a href='#' class='dropdown-toggle' data-toggle='dropdown' data-hover='dropdown' data-delay='50' role='button' aria-expanded='false'>" +
                            "<span class='operateMore'>更多</span><span class='glyphicon glyphicon-menu-down'></span>" +
                            "</a>" +
                            "<ul class='dropdown-menu batch-menu more' role='menu'>" +
                            "</ul>" +
                            "</li>");
                            var $preview = $("<li><a style='padding-left: 20px;'>预览</a></li><li class='divider short'></li>"),
                                $edit = $("<li><a   href='/TemplateEdit/TemplateEdit?templateId=" + value.templateId + "&jumpSign=0&systemOrFalse=" + systemOr + "' style='padding-left: 20px;'>编辑</a></li><li class='divider short'></li>"),
                                $copy = $("<li><a  data-toggle='modal' data-target='#formCopy_modal' href='#' style='padding-left: 20px;'>复制</a></li><li class='divider short'></li>"),
                                $delete = $("<li><a href='#' style='padding-left: 20px;'>删除</a></li><li class='divider short'></li>"),
                                $move = $("<li><a data-toggle='modal' data-target='#formMove_modal' href='#' id='operate_move' style='padding-left: 20px;'>移动</a></li>");
                            var $choose = $("<div class='xxc_choose' termm=" + value.templateId + "  trrm=" + value.categoryId + "><span></span></div>");
                          
                            $edit.find("a").click(function () {
                                console.log("herhehehehheh"+  $(this).attr("href") );
                            })

                            $formBatch.find('ul').append([$preview, $edit, $copy, $delete, $move]);
                            $content.append([$title, $des]);
                            if (value.status == 1) {
                                var $use = $("<div class='Use AlreadyUse' term=" + value.testFlag + "><a></a></div>");
                            }
                            else {
                                //console.log(' value.testFlag：' + value.testFlag);
                                var $use = $("<div class='Use stopUse' term=" + value.testFlag + " ><a></a></div>");
                            }
                            if (argus.type == true) {
                                $btns.find("ul").append([$formProcess, $systermCopy])
                                $cell.append([$smallImage, $content, $operate, $choose]);
                            } else {
                                if (value.IsUse == true) {
                                    $btns.find("ul").append([$formTest,$formProcess, $formBatch]);
                                    $cell.append([$smallImage, $content, $operate, $use]);
                                } else {
                                    $btns.find("ul").append([$formTest,$formProcess, $formBatch]);
                                    $cell.append([$smallImage, $content, $operate, $choose, $use]);
                                }
                                
                            };
                           
                            $btns.appendTo($operate);
                            $col.append($cell).appendTo($formList);
                            $des.dotdotdot();
                            $use.click(function () {
                                if ($(this).hasClass("stopUse")) {
                                    //console.log(' value.testFlag' + value.testFlag);
                                    if ( $(this).attr( "term" )== "false" ) {
                                        ncUnits.alert("该表单尚未通过测试，无法启用！");
                                    }
                                    else {
                                        $this = $(this);
                                        ncUnits.confirm({
                                            title: '提示',
                                            html: '确定要启用该表单吗？',
                                            yes: function (layer_confirm) {
                                                var templateId = $this.closest(".cell").attr("term");
                                                $.ajax({
                                                    type: "post",
                                                    url: "/Template/Stopandstart",
                                                    dataType: "json",
                                                    data: { templateId: templateId, flag: 1 },
                                                    success: rsHandler(function () {
                                                        ncUnits.alert("该表单已启用");
                                                        $this.removeClass("stopUse");
                                                        $this.addClass("AlreadyUse");
                                                    })
                                                })
                                                layer.close(layer_confirm);

                                            }
                                        });
                                    }
                                    
                                }
                                else {
                                    $this = $(this);
                                    ncUnits.confirm({
                                        title: '提示',
                                        html: '确定要停用该表单吗？',
                                        yes: function (layer_confirm) {
                                            var templateId = $this.closest(".cell").attr("term");
                                            $.ajax({
                                                type: "post",
                                                url: "/Template/Stopandstart",
                                                dataType: "json",
                                                data: { templateId: templateId, flag: 2 },
                                                success: rsHandler(function () {
                                                    $this.removeClass("AlreadyUse");
                                                    $this.addClass("stopUse");
                                                    $this.css("display","block")
                                                })
                                            })
                                            layer.close(layer_confirm);

                                        }
                                    });
                                }
                                
                                 
                            })
                            $systermCopy.click(function (data) {
                                categorys = [];
                                var thiscategory = [value.categoryId]
                                $("#copy_select").val('');
                                $("#copy_modal_chosen_count").text(0);
                                $("#copy_modal_chosen li").remove();
                                $("#formCopy_modal .modal-content").load("/Template/GetCoyp", function () {
                                    templates.push(value.templateId);
                                    $.ajax({
                                        type: "post",
                                        url: "/Template/GetTemplateCaregoryList",
                                        dataType: "json",
                                        data: { system: false },
                                        success: rsHandler(function (data) {
                                            for (var i = 0; i < data.length; i++) {
                                                $.each(thiscategory, function (e, j) {
                                                    if (j == data[i].id) {
                                                        //data.splice(i, 1);
                                                    }
                                                });
                                            }
                                            var categoryTree = $.fn.zTree.init($("#copy_modal_folder"), $.extend({
                                                callback: {
                                                    beforeClick: function (id, node) {
                                                        //folderTree.checkNode(node, undefined, undefined, true);
                                                        return false;
                                                    },
                                                    onCheck: function (e, id, node) {
                                                        $("#copy_modal_chosen_count").html(categoryTree.getCheckedNodes().length);
                                                        if (node.checked) {
                                                            var $checked = $("<li term='" + node.id + "'><span title='" + data.name + "'>" + node.name + "</span></li>"),
                                                                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                                            $("#copy_modal_chosen").append($checked.append($close));
                                                            $close.click(function () {
                                                                categoryTree.checkNode(node, undefined, undefined, true);
                                                                for (var i = 0; i < categorys.length; i++) {
                                                                    if (categorys[i] == $(this).parent().attr("term")) {
                                                                        categorys.splice(i, 1);
                                                                    }
                                                                }
                                                            });
                                                            node.mappingLi = $checked;
                                                            categorys.push(node.id);
                                                        } else {
                                                            node.mappingLi.remove();
                                                            categorys = _.without(categorys, node.id);
                                                        }
                                                    },
                                                    onNodeCreated: function (e, id, node) {
                                                        var categoryTree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                                                        $("#copy_modal_chosen li").each(function () {
                                                            var departId = $(this).attr('term');
                                                            //$(this).remove();
                                                            if (parseInt(departId) == node.id) {
                                                                categoryTree.checkNode(node, undefined, undefined, true);
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
                                                }
                                            }), data);
                                        })
                                    });
                                    //选择文件夹
                                    $("#copy_modal_search").selection({
                                        url: "/template/selectCaregoryList",
                                        hasImage: false,
                                        otherParam: { type: 2 },
                                        selectHandle: function (data) {
                                            for (var i = 0; i < data.length; i++) {
                                                $.each(categorys, function (e, j) {
                                                    if (j == data[i].id) {
                                                        data.splice(i, 1);
                                                    }
                                                });
                                            }
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
                                                    var $checked = $("<li term=" + data.id + "><span title='" + data.name + " '>" + data.name + "</span></li>"),
                                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                                    $("#copy_modal_chosen").append($checked.append($close));
                                                    categorys.push(data.id);
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
                                    /*复制事件 开始*/
                                    $("#copy_modal_submit").click(function () {
                                        var copymodel = { templateId: templates, caregoryid: categorys };
                                        if (categorys.length > 0) {
                                            if (isCopy == true) {
                                                isCopy = false;
                                                $.ajax({
                                                    type: "post",
                                                    url: "/Template/CopyTemplate ",
                                                    dataType: "json",
                                                    data: {
                                                        data: JSON.stringify(copymodel)
                                                    },
                                                    success: rsHandler(function (data) {
                                                        if (data == true) {
                                                            ncUnits.alert("复制成功");
                                                            loadForms();
                                                            isCopy = true;
                                                            $("#formCopy_modal").modal("hide");
                                                            $("#copy_select").val('');
                                                            $("#copy_modal_chosen_count").text(0);
                                                            $("#copy_modal_chosen li").remove();

                                                            isCopy = true;
                                                        }
                                                        else {
                                                            ncUnits.alert("复制失败");
                                                            loadForms();
                                                            isCopy = true;
                                                            $("#formCopy_modal").modal("hide");
                                                            $("#copy_select").val('');
                                                            $("#copy_modal_chosen_count").text(0);
                                                            $("#copy_modal_chosen li").remove();
                                                            isCopy = true;
                                                        }
                                                    })
                                                });
                                            }
                                        } else {
                                            ncUnits.alert("请选择分类");
                                        }

                                    })
                                    $("#copy_modal_cancel").click(function () {
                                        $("#copy_select").val('');
                                        categorys = [];
                                        $("#copy_modal_chosen_count").text(0);
                                        $("#copy_modal_chosen li").remove();
                                        isCopy = true;
                                    });
                                    $("#copy_modal .close").click(function () {
                                        $("#copy_select").val('');
                                        categorys = [];
                                        $("#copy_modal_chosen_count").text(0);
                                        $("#copy_modal_chosen li").remove();
                                        isCopy = true;
                                    });
                                    /*复制事件 结束*/

                                });
                                //bitchoperateing = false;
                            });
                          
                            $copy.click(function (data) {
                                categorys = [];
                               
                                var thiscategory = [value.categoryId]
                                console.log(categorys);
                                $("#copy_select").val('');
                                $("#copy_modal_chosen_count").text(0);
                                $("#copy_modal_chosen li").remove();
                                $("#formCopy_modal .modal-content").load("/Template/GetCoyp", function () {
                                    templates = [];
                                    templates.push(value.templateId);
                                    $.ajax({
                                        type: "post",
                                        url: "/Template/GetTemplateCaregoryList",
                                        dataType: "json",
                                        data: { system: argus.type },
                                        success: rsHandler(function (data) {
                                            for (var i = 0; i < data.length; i++) {
                                                $.each(thiscategory, function (e, j) {
                                                    if (j == data[i].id) {
                                                        //data.splice(i, 1);
                                                    }
                                                });
                                            }
                                            //categorys.push(value.categoryId);
                                            var categoryTree = $.fn.zTree.init($("#copy_modal_folder"), $.extend({
                                                callback: {
                                                    beforeClick: function (id, node) {
                                                        //folderTree.checkNode(node, undefined, undefined, true);
                                                        return false;
                                                    },
                                                    onCheck: function (e, id, node) {
                                                        $("#copy_modal_chosen_count").html(categoryTree.getCheckedNodes().length);
                                                        if (node.checked) {
                                                            var $checked = $("<li term='" + node.id + "'><span>" + node.name + "</span></li>"),
                                                                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                                            $("#copy_modal_chosen").append($checked.append($close));
                                                            $close.click(function () {
                                                                categoryTree.checkNode(node, undefined, undefined, true);
                                                                for (var i = 0; i < categorys.length; i++) {
                                                                    if (categorys[i] == $(this).parent().attr("term")) {
                                                                        categorys.splice(i, 1);
                                                                    }
                                                                }
                                                            });
                                                            node.mappingLi = $checked;
                                                            categorys.push(node.id);
                                                        } else {
                                                            node.mappingLi.remove();
                                                            categorys = _.without(categorys, node.id);
                                                        }
                                                    },
                                                    onNodeCreated: function (e, id, node) {
                                                        var categoryTree = $.fn.zTree.getZTreeObj("copy_modal_folder");
                                                        $("#copy_modal_chosen li").each(function () {
                                                            var departId = $(this).attr('term');
                                                            //$(this).remove();
                                                            if (parseInt(departId) == node.id) {
                                                                categoryTree.checkNode(node, undefined, undefined, true);
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
                                                }
                                            }), data);
                                        })
                                    });
                                    //选择文件夹 
                                    $("#copy_modal_search").selection({

                                        url: "/template/selectCaregoryList",
                                        hasImage: false,
                                        otherParam: { type: false },
                                        selectHandle: function (data) {
                                            categorys.push(value.categoryId);
                                            for (var i = 0; i < data.length; i++) {
                                                $.each(categorys, function (e, j) {
                                                    if (j == data[i].id) {
                                                        data.splice(i, 1);
                                                    }
                                                });
                                                console.log(categorys)
                                            }
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
                                                    categorys.push(data.id);
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
                                    /*复制事件 开始*/
                                    $("#copy_modal_submit").click(function () {
                                        if (categorys.length > 0) {
                                            for (var i = 0; i < categorys.length; i++) {
                                                if (categorys[i] == 0) {
                                                    categorys[i] = null;
                                                }
                                            }

                                        };
                                        console.log(categorys);
                                        var copymodel = { templateId: templates, caregoryid: categorys };
                                        if (categorys.length > 0) {
                                            $.ajax({
                                                type: "post",
                                                url: "/Template/CopyTemplate ",
                                                dataType: "json",
                                                data: {
                                                    data: JSON.stringify(copymodel)
                                                },
                                                success: rsHandler(function (data) {
                                                    if (data == true) {
                                                        ncUnits.alert("复制成功");
                                                        loadForms();
                                                        isCopy = true;
                                                        $("#formCopy_modal").modal("hide");
                                                        $("#copy_select").val('');
                                                        $("#copy_modal_chosen_count").text(0);
                                                        $("#copy_modal_chosen li").remove();

                                                        isCopy = true;
                                                    }
                                                    else {
                                                        ncUnits.alert("复制失败");
                                                        loadForms();
                                                        isCopy = true;
                                                        $("#formCopy_modal").modal("hide");
                                                        $("#copy_select").val('');
                                                        $("#copy_modal_chosen_count").text(0);
                                                        $("#copy_modal_chosen li").remove();
                                                        isCopy = true;
                                                    }
                                                })
                                            });
                                        } else {
                                            ncUnits.alert("请选择分类");
                                        }
                                    })
                                    $("#copy_modal_cancel").click(function () {
                                        $("#copy_select").val('');
                                        categorys = [];
                                        $("#copy_modal_chosen_count").text(0);
                                        $("#copy_modal_chosen li").remove();
                                        isCopy = true;
                                    });
                                    $("#copy_modal .close").click(function () {
                                        $("#copy_select").val('');
                                        categorys = [];
                                        $("#copy_modal_chosen_count").text(0);
                                        $("#copy_modal_chosen li").remove();
                                        isCopy = true;
                                    });
                                    /*复制事件 结束*/
                                });
                                //bitchoperateing = false;
                            });
                            $move.click(function () {
                                categorys = [];
                                chooseCargoryId = [value.categoryId]
                                $("#move_modal_chosen_count").text(0);
                                $("#move_modal_chosen li").remove();
                                $("#move_select").val('');
                                if (templates.length > 0) {
                                    for (var i = 0; i < templates.length; i++) {
                                        if (templates[i] == 0) {
                                            templates[i] = null;
                                        }
                                    }
                                };
                                $("#formMove_modal .modal-content").load("/Template/GetMove", function () {
                                    templates.push(value.templateId);
                                    //categorys.push(data.id);
                                    buildDepartmentTree();
                                    //选择文件夹
                                    $("#move_modal_search").selection({
                                        url: "/Template/selectCaregoryList",
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
                                                    categorys.push(data.id);
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
                                bitchoperateing = false;
                            });
                            /* 删除 开始 */
                            $delete.click(function () {
                                ncUnits.confirm({
                                    title: '提示',
                                    html: '确认要删除？',
                                    yes: function (layer_confirm) {
                                        layer.close(layer_confirm);
                                        templates = [];
                                        templates.push(value.templateId);
                                        var Delete = { templateId: templates }
                                        $.ajax({
                                            type: "post",
                                            url: "/Template/DeleteTemp",
                                            dataType: "json",
                                            data: { data: JSON.stringify(Delete) },
                                            success: rsHandler(function (data) {
                                                if (data == 3) {
                                                    ncUnits.alert("删除成功");
                                                    loadForms();
                                                    isCopy = true;
                                                    //loadrighttree();
                                                } else if (data == 2) {
                                                    ncUnits.alert("不能删除，表单正在使用");
                                                    loadForms();
                                                    isCopy = true;
                                                    //loadrighttree();
                                                } else {
                                                    ncUnits.alert("删除失败");
                                                    loadForms();
                                                    isCopy = true;
                                                }
                                            })
                                        });
                                    }
                                });
                                bitchoperateing = false;
                            });
                            /* 删除 结束 */
                            /*批量勾选 开始*/
                            templates = [];
                           
                            $choose.click(function () {
                                if ($(this).hasClass('chooseHit')) {
                                    $(this).removeClass('chooseHit');
                                    $('span', this).removeClass('spanHit');
                                    for (var i = 0; i < templates.length; i++) {
                                        if ($(this).attr('termm') == templates[i]) {
                                            templates.splice(i, 1);
                                        }
                                    }
                                }
                                else {
                                    $(this).addClass('chooseHit');
                                    $('span', this).addClass('spanHit');
                                    templates.push(value.templateId);
                                }
                            });
                            /*批量勾选 结束*/
                            //预览开始
                            $preview.off("click");
                            $preview.click(function () {
                                addflow_templateId = value.templateId; 
                                $("#preview_modal").modal("show");
                           
                            })

                            //测试开始
                            $formTest.click(function () {
                                testflow_templateId = value.templateId;
                                $testActiveNode = $(this);
                                $("#flow_test_modal").modal("show");
                            });


                        }
                    });
                    var $addNewFile = $("<div class='form-newadd col-xs-3' id='form-newbulding' style='cursor:pointer'>" +
                    "<div class='form-add'><div class='form-addPicture'></div>" +
                    "<div class='form-wordPosition'>新建表单</div></div>" +
                    "</div>");
                    console.log("新建表单");
                    $formList.append($addNewFile);

                    $addNewFile.off("click");
                    $addNewFile.click(function () {
                        $("#formBuild_modal").modal('show');
                        //$("li.dropdown.addborder").off("click")
                        $('#power-span span:eq(0)').text("");
                        $("#input_filename").val("")
                        $("#input_filedes").val("");
                        cancelBitchOperate();
                        isMore = true;
                        createForm();
                    });

                    //选择卡片列表移上去出现绿条 开始
                    $(".cell").hover(function () {
                        $(".operate", this).css("display", "block");
                        $(".AlreadyUse", this).css("display", "block");
                        // $(".operate", this).toggle();
                        // $(".AlreadyUse", this).toggle();
                        //$(".cell",this).css("background-color","#dff0de").toggle();
                    },function () {
                        $(".operate", this).css("display","none");
                        $(".AlreadyUse", this).css("display", "none");
                        //$(".cell",this).css("background-color","#dff0de").toggle();
                    }
                    );
                    if (argus.type == false) {
                        $(".form-list.sortable .cell*").css({ "background-color": "#f5f5f5", "border": "1px solid #ddd" });
                        $(".form-add").css({ "background-color": "#f5f5f5", "border": "1px solid #ddd" });
                        //$formList.hide($addNewFile);
                    }
                    else {
                        $(".form-newadd").remove();
                        console.log("移除新建表单");
                    }
                    /*   $(".cell").onmouseover(function(){
                     $(".cell",this).css("background-color","#dff0de");
                     })
                     $(".cell").onmouseout(function(){
                     $(".cell",this).css("background-color","#f6fbf5");
                     })*/
                })
            });
            /*批量显示权限*/
        }
 
        var mode = 0;
        function createForm() {
            /**注释** 
            $("#input_filename").blur(function () {
                authfile_flag = true;
                var Filename = $.trim($("#input_filename").val());
                //修改
                if (Filename == "") {                
                    ncUnits.alert("表单名称不能为空!");
                    authfile_flag = false;
                }
                else if (Filename.length > 20) {
                    ncUnits.alert("表单名称不能超过20字符!");
                    authfile_flag = false;
                }
                var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
                if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                    ncUnits.alert("表单名称存在非法字符!");
                    authfile_flag = false;
                }
            });
            $("#input_filedes").blur(function () {
                authfile_flag = true;

                var descriptions = $("#input_filedes").val();
                if (descriptions.length > 100) {
                    ncUnits.alert("表单描述不能超过100字符!");
                    authfile_flag = false;
                }
                var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
                if (descriptions.indexOf('null') >= 0 || descriptions.indexOf('NULL') >= 0 || descriptions.indexOf('&nbsp') >= 0 || reg.test(descriptions) || descriptions.indexOf('</') >= 0) {
                    ncUnits.alert("表单描述存在非法字符!");
                    authfile_flag = false;
                }
            });
            /**注释**/
            //下拉列表

            $('#power-span span:eq(0)').text(" ");
            $("li.dropdown.addborder").click(function () {
                //if (mode == 0) {
                $.ajax({
                    type: "post",
                    url: "/Template/GetTemplateCaregoryList",
                    data: { system: argus.hovertype },
                    dataType: "json",
                    success: rsHandler(function (data) {
                        $("ul.dropdown-menu.batch-menu.formClassify.newbuilde").find("li").remove();
                        for (var i = 0; i < data.length; i++) {

                            var $formclassify = $("<li term='" + data[i].categoryId + "'><a maxlength='6' title=' " + data[i].categoryName + " '>" + data[i].categoryName + "</a></li>");
                            var $divider = $("<li class='divider short'></li>");
                            $($formclassify, $divider).appendTo($("ul.dropdown-menu.batch-menu.formClassify.newbuilde"));

                            argus.categoryId = "";
                            $(".userdefinedForm li").click(function () {
                                argus.categoryId = $(this).attr('term');
                                //argus.clickType = false;
                                //loadForms();
                            })

                            $('.newbuilde li').off("click");
                            $('.newbuilde li').click(function () {
                                var power = $(this).attr('term');
                                var text = $(this).find('a').text();
                                $('#power-span span:eq(0)').attr('term', power).text(text);
                            })
                        }
                    })
                });
                //}
                //++mode;
            })
            if (thisclickCaregory != null) {
                $('#power-span span:eq(0)').attr('term', thisclickCaregory).text(thisCaregoryName);
            }
            var isClick = true;
            authfile_flag = false;
            $("#newfile_modal_submit").off("click");
            $("#newfile_modal_submit").click(function () {
                templates.push(argus.templateId);
                categorys.push($('#power-span span:eq(0)').attr('term'));
                var Filename = $.trim($("#input_filename").val());
                if (Filename == "") {
                    ncUnits.alert("表单名称不能为空!");
                    return;
                }
                else if (Filename.length > 20) {
                    ncUnits.alert("表单名称不能超过20字符!");
                    return;
                }
                var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
                if (Filename.indexOf('null') >= 0 || Filename.indexOf('NULL') >= 0 || Filename.indexOf('&nbsp') >= 0 || reg.test(Filename) || Filename.indexOf('</') >= 0) {
                    ncUnits.alert("表单名称存在非法字符!");
                    return;
                }
                var descriptions = $("#input_filedes").val();
                var categoryChoose = $('#power-span span:eq(0)').text();
                if (categoryChoose == " ") {
                    ncUnits.alert("表单分类不能为空!");
                    return;
                }
                var add = {
                    system: argus.type,
                    categoryId: categorys[0], //分类ID
                    templateName: Filename,//模板名称
                    description: descriptions
                }
                if (isClick == true) {
                    isClick = false;
                    $.ajax({
                        type: "post",
                        url: "/Template/AddNewTemplate",
                        dataType: "json",
                        data: {
                            data: JSON.stringify(add)
                        }, //描述

                        success: rsHandler(function (data) {
                            if (data != null) {
                                ncUnits.alert("新建表单成功!");
                                loadViewToMain("/TemplateEdit/TemplateEdit?templateId="+ data +"&jumpSign=0&systemOrFalse=" +systemOr);
                                //window.location.href = "/TemplateEdit/TemplateEdit?templateId=" + data + "";
                                isClick = true;
                                //$("#formBuild_modal").modal("hide");
                                //$('#power-span span:eq(0)').text("");
                                //$("#input_filename").val("")
                                //$("#input_filedes").val("");
                                //loadForms();
                                //mode = 0;
                                //isCopy = true;
                            } else {
                                ncUnits.alert("新建表单失败!");
                                isClick = true;
                            }
                        })
                    });
                }
                loadForms();
            });


        }
        //显示预览画面
  
        /*新建表单开始*/
        //if ($("#form-newbulding").click() || $(".addFormClassify").click()) {
        //    $("li.dropdown.addborder").off("click")
        //    $('#power-span span:eq(0)').text("");
        //    $("#input_filename").val("")
        //    $("#input_filedes").val("");

        //    $("#formBuild_modal").on('shown.bs.modal', function () {

        //        cancelBitchOperate();//取消当前批量操作
        //        createForm();

        //        $(".des").dotdotdot();
        //        $('#power-span span:eq(0)').text(" ");
        //        //if (folders.length > 0) {
        //        //    for (var i = 0; i < folders.length; i++) {
        //        //        if (folders[i] == 0) {
        //        //            folders[i] = null;
        //        //        }
        //        //    }
        //        //};
        //    });
        //}        

        $(".des").dotdotdot();
        $('#power-span span:eq(0)').text(" ");

        //$("#form-newbulding").click(function () {
        //    $("li.dropdown.addborder").off("click")
        //    $('#power-span span:eq(0)').text("");
        //    $("#input_filename").val("")
        //    $("#input_filedes").val("");

        //    $("#formBuild_modal").on('shown.bs.modal', function () {
        //        $(".formClassify .newbuilde li").remove();
        //        cancelBitchOperate();//取消当前批量操作
        //        createForm();

        //        $(".des").dotdotdot();
        //        $('#power-span span:eq(0)').text(" ");
        //    });

        //})
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
                    start: new Date().toLocaleDateString() + ' 17:30:00',
                    choose: function (dates) {
                        endTime_v = dates;
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
                    if (data && data.length > 0) {
                        $("#yulan_createUser").val(data[0].userName);
                        $(".department_ul").empty();
                        $.each(data, function (e, i) {
                            var $content = $(" <li><a href='#' term=" + i.id + " class='xxc_departmentName'>" + i.name + "</a></li><li class='divider short'></li>");
                            $(".department_ul").append($content);
                            $(".department_ul .xxc_departmentName").off('click');
                            $(".department_ul .xxc_departmentName").click(function () {
                                var orgId = $(this).attr('term');
                                $("#department_select").text($(this).text()).attr('term', orgId);
                            });
                            $("#department_select").text($('.department_ul .xxc_departmentName:eq(0)').text()).attr('term', $('.department_ul .xxc_departmentName:eq(0)').attr('term'));
                        });
                        $(".department_ul li:last").remove();
                    }
                })
            });

            loadDataByTemplateId(addflow_templateId);

            //取消按钮点击事件
            $('#addflow_cancel').click(function () {
                clearFlowContent();
                $('#preview_modal').modal('hide');
            });

            //添加新建流程信息
            $('.addflow_sure').click(function () {
                var flag = $(this).attr('term');
                addflow_content.templateId = addflow_templateId;
                addflow_content.title = $('.form-title').val();
                addflow_content.status = 1;
                addflow_content.urgency = $('#preview_modal .liHit').length;
                addflow_content.organizationId = $('#department_select').attr('term');
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
                        loadFlows();
                    })
                });
            });
        }).on("hidden.bs.modal", function () {
            $("#Label-content").empty();
            //addflow_templateId=null;
            $('#addflow_modal_submit').removeAttr('data-toggle');
            $('#addflow_modal_submit').removeAttr('data-target');
            $('.stars li').removeClass('liHit');
            $('#addflow_createtime_v').val('');
            $('.addflow_footer').hide();
            $('#addflow-mask').show();
        });

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
                        flag: 1
                    },
                    success: rsHandler(function (data) {
                        if (data.template.defaultTitle == 1) {
                            $("#preview_title").attr("readonly", "true");
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
        }

        $(".addFormClassify").click(function () {
            $("li.dropdown.addborder").off("click")
            $('#power-span span:eq(0)').text("");
            $("#input_filename").val("")
            $("#input_filedes").val("");
            $("#formBuild_modal").on('shown.bs.modal', function () {

                cancelBitchOperate();//取消当前批量操作
                isMore = true;
                createForm();

                $(".des").dotdotdot();
                $('#power-span span:eq(0)').text(" ");

            });
        })
        /*新建表单结束*/

  

        //分类排序参数列表
        /*系统表单列表获取和分类开始*/
        $("#formManage").click(function () {
         
            $.ajax({
                type: "post",
                url: "/Template/GetTemplateCaregoryList",
                data: { system: true },
                dataType: "json",
                success: rsHandler(function (data) {
                    //console.log(data);
                    $("#ddlsystemList li").remove();
                    for (var i = 0; i < data.length; i++) {
                        //console.log(data[i].id);
                        var $formclassify = $("<li term='" + data[i].categoryId + "'><a maxlength='6' title=' " + data[i].categoryName + " '>" + data[i].categoryName + "</a></li>");
                        var $divider = $("<li class='divider short'></li>");
                        $($formclassify, $divider).appendTo($("#ddlsystemList"));
                        //$("ul.dropdown-menu.batch-menu.formClassify.userdefinedForm li").off('click');
                        $("#ddlsystemList li").off("click");
                        argus.categoryId = "";
                        $("#ddlsystemList li").click(function () {
                            systemOr = 1;
                            argus.type = true,
                            argus.categoryId = $(this).attr('term');
                            $("#ddlsystemList .active").removeClass('active');
                            $("#ddlUserList .active").removeClass('active');
                            $(this).addClass('active');
                            $(".form_userdefined.active").removeClass('active');
                            $(".form_userdefined").css({ "background": "url(../../Images/flow/自定义表单.png) no-repeat" });
                            $("span.glyphicon.glyphicon-menu-down.form2").removeClass('active');

                            $(".form_systerm").addClass('active');
                            $(".form_systerm.active").css({ "background": "url(../../Images/flow/系统表单hit.png) no-repeat" });
                            $("span.glyphicon.glyphicon-menu-down.form1").addClass('active');
                            $(".glyphicon-plus").hide();
                            $(".pull-right.dropdown").hide();
                            thisclickCaregory = $(this).attr('term');
                            thisCaregoryName = $(this).find('a').text();
                            loadForms();
                          
                            argus.clickType = true;
                        })
                        //$('#ddlsystemList li').off("click");
                        //$('#ddlsystemList li').click(function () {
                        //    var power = $(this).attr('term');
                        //    var text = $(this).find('a').text();
                        //    $('#power-span span:eq(0)').attr('term', power).text(text);
                        //})
                    }
                })
            });

            $.ajax({
                type: "post",
                url: "/Template/GetTemplateCaregoryList",
                data: { system: false },
                dataType: "json",
                success: rsHandler(function (data) { 
                    $("#ddlUserList li").remove();
                    //console.log(data);
                    for (var i = 0; i < data.length; i++) {
                        //console.log(data[i].id);
                        var $formclassify = $("<li term='" + data[i].categoryId + "'><a maxlength='6' title=' " + data[i].categoryName + " '>" + data[i].categoryName + "</a></li>");
                        var $divider = $("<li class='divider short'></li>");
                        $($formclassify, $divider).appendTo($("#ddlUserList"));
                        //$("ul.dropdown-menu.batch-menu.formClassify.userdefinedForm li").off('click');
                        $("#ddlUserList li").off("click");
                        argus.categoryId = "";
                        $("#ddlUserList li").click(function () {
                            systemOr = 0;
                            argus.type = false,
                            argus.categoryId = $(this).attr('term');
                            $("#ddlUserList .active").removeClass('active');
                            $("#ddlsystemList .active").removeClass('active');
                            $(this).addClass('active');
                            $(".form_systerm.active").removeClass('active');
                            $(".form_systerm").css({ "background": "url(../../Images/flow/系统表单.png) no-repeat" });
                            $("span.glyphicon.glyphicon-menu-down.form1").removeClass('active');
                            $(".form_userdefined").addClass('active');
                            $(".form_userdefined.active").css({ "background": "url(../../Images/flow/自定义表单hit.png) no-repeat" });
                            $(".glyphicon-plus").show();
                            $(".pull-right.dropdown").show();
                            thisclickCaregory = $(this).attr('term');
                            thisCaregoryName = $(this).find('a').text();
                            loadForms();
                            argus.clickType = false;
                        })
                        $('.newbuilde li').off("click");
                        $('.newbuilde li').click(function () {
                            var power = $(this).attr('term');
                            var text = $(this).find('a').text();
                            $('#power-span span:eq(0)').attr('term', power).text(text);
                        })
                    }
                })
            });
            $('.moreCancel').hide();
            $('.moreBg').hide();
            $(".glyphicon-plus").hide();
            $(".form_systerm").addClass('active');
            $(".form_systerm").css({ "background": "url(../../Images/flow/系统表单hit.png) no-repeat" });
            $("span.glyphicon.glyphicon-menu-down.form1").addClass('active');
            $(".form_userdefined.active").removeClass('active');
            $(".form_userdefined").css({ "background": "url(../../Images/flow/自定义表单.png) no-repeat" });
            argus.type = true;
            argus.clickType = true;
            $(".pull-right.dropdown").hide();
            //loadForms();

            //$("ul.dropdown-menu.batch-menu.formClassify").clear();
 
            $("#userdefined_form").off("click");
            $("#userdefined_form").click(function () {
                systemOr = 0;
                $("#ddlsystemList .active").removeClass('active');
                $("#ddlUserList .active").removeClass('active');
                $(".glyphicon-plus").show();
                $(".form_systerm.active").removeClass('active');
                $(".form_systerm").css({ "background": "url(../../Images/flow/系统表单.png) no-repeat" });
                $("span.glyphicon.glyphicon-menu-down.form1").removeClass('active');

                $(".form_userdefined").addClass('active');
                $(".form_userdefined.active").css({ "background": "url(../../Images/flow/自定义表单hit.png) no-repeat" });
                $("span.glyphicon.glyphicon-menu-down.form2").addClass('active');
                argus.type = false;
                argus.clickType = false;
                thisclickCaregory = null;
                $(".pull-right.dropdown").show();
                $('.newbuilde li').off("click");
                argus.categoryId = "";
                // getFormclassify();
                $('.moreCancel').hide();
                $('.moreBg').hide();
                loadForms();

            });
 
            $("#syterm_form").off("click");
            $("#syterm_form").click(function () {
                systemOr = 1;
                $("#ddlsystemList .active").removeClass('active');
                $("#ddlUserList .active").removeClass('active');
                $(".glyphicon-plus").hide();
                $('.moreBg').text($(this).text()).hide();
                $('.moreCancel').hide();
                $(".form_userdefined.active").removeClass('active');
                $(".form_userdefined").css({ "background": "url(../../Images/flow/自定义表单.png) no-repeat" });
                $("span.glyphicon.glyphicon-menu-down.form2").removeClass('active');

                $(".form_systerm").addClass('active');
                $(".form_systerm.active").css({ "background": "url(../../Images/flow/系统表单hit.png) no-repeat" });
                $("span.glyphicon.glyphicon-menu-down.form1").addClass('active');
                argus.type = true;
                argus.clickType = true;
                $(".pull-right.dropdown").hide();
                argus.categoryId = "";
                $('.moreCancel').hide();
                $('.moreBg').hide();
                // getFormclassify();
                loadForms();
            })
        })
        /*系统表单列表获取分类结束*/
        /*系统表单列表获取和分类开始*/
        /* if(argus.type==2){
             $('.userdefinedForm li a').click( function () {
                 categorys=argus.categoryId.push( parseInt($(this).attr('term')) );
                 for(i=0;i<categorys.length;i++){ loadForms();}
             });
         }*/
        /*系统表单列表获取分类结束*/
        /*表单列表加载开始*/
        var isCopy = true;//是否能按 
        function buildDepartmentTree() {
            // categorys.push(argus.categoryId);
            //categorys=$(".xxc_choose").attr('term');
            $.ajax({
                type: "post",
                url: "/Template/GetTemplateCaregoryList",
                data: { system: argus.type },
                dataType: "json",
                success: rsHandler(function (data) {
                    //for (var i = 0; i < data.length; i++) {
                    //    $.each(chooseCargoryId, function (e, j) {
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
                                $("#move_modal_chosen_count").text(folderTree.getCheckedNodes().length);
                                if (node.checked) {
                                    var $checked = $("<li term='" + node.id + "'><span>" + node.name + "</span></li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $("#move_modal_chosen").find("li").remove();
                                    $("#move_modal_chosen").append($checked.append($close));
                                    $close.click(function () {
                                        folderTree.checkNode(node, undefined, undefined, true);
                                    });
                                    node.mappingLi = $checked;
                                    categorys.push(node.id);
                                    $('#move_modal_chosen_count').text($("#move_modal_chosen li").length);
                                    //var moveModel = {templateIds: templates, caregoryid: categorys };
                                    bitchoperateing = true;

                                } else {
                                    node.mappingLi.remove();
                                    categorys = _.without(categorys, node.id);
                                    //$("#"+node.id).remove();
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
                        }
                    }), data);
                })
            });
            //categorys = [];
            $("#move_modal_submit").click(function () {
                //categorys.push(node.id); 
                //console.log(categorys);
                var movemodel = { templateIds: templates, caregoryid: categorys };
                console.log(movemodel);
                if (categorys.length > 0) {
                    if (isCopy == true) {
                        isCopy = false;
                        $.ajax({
                            type: "post",
                            url: "/Template/MoveTemplate",
                            dataType: "json",
                            data: {
                                data: JSON.stringify(movemodel)
                            },
                            success: rsHandler(function (data) {
                                if (data == true) {
                                    ncUnits.alert("移动成功");
                                    loadForms();
                                    isCopy = true;
                                    $("#formMove_modal").modal("hide");
                                    //node.mappingLi.remove();
                                    categorys = [];
                                    //categorys = _.without(categorys, node.id);
                                    $("#move_select").val('');
                                    $("#move_modal_chosen_count").text(0);
                                    $("#move_modal_chosen li").remove();
                                    isMore = true;
                                }
                                else {
                                    ncUnits.alert("移动失败");
                                    loadForms();
                                    isCopy = true;
                                    $("#formMove_modal").modal("hide");
                                    //node.mappingLi.remove();
                                    categorys = [];
                                    //categorys = _.without(categorys, node.id);
                                    $("#move_select").val('');
                                    $("#move_modal_chosen_count").text(0);
                                    $("#move_modal_chosen li").remove();
                                    isMore = true;
                                }
                            })
                        });
                    }
                } else {
                    ncUnits.alert("请选择分类");
                }
                bitchoperateing = false;
            })
            $("#move_modal_cancel").click(function () {
                categorys = [];
                $("#formMove_modal").modal("hide");
                $("#move_select").val('');
                $("#move_modal_chosen_count").text(0);
                $("#move_modal_chosen li").remove();
                isCopy = true;
                bitchoperateing = false;
            });
            $("#move_modal .close").click(function () {
                categorys = [];
                $("#move_select").val('');
                $("#formMove_modal").modal("hide");
                $("#move_modal_chosen_count").text(0);
                $("#move_modal_chosen li").remove();
                isCopy = true;
                bitchoperateing = false;
            });
            $('.moreBg').removeAttr('data-toggle');
            $('.moreBg').removeAttr('data-target');
            bitchoperateing = false;
        }
        /*批量事件 开始*/
        var chooseCargoryId = [];
        $('.batch-menu.lot a').click(function () {
            templates = [];
            categorys = []; 
            isMore = false;
            $(".cell").off("hover");
            $(".cell").hover(function () {
                $(".operate", this).hide();
            });
            if ($("#authority-a").parent().hasClass("active")) {
                return;
            }
            //$('.isfolder').css('cursor', 'default');
            bitchoperateing = true;
            $(".btns").hide();
            $('.moreBg').text($(this).text()).show();
            $('.moreCancel').show();
            $('.xxc_choose').addClass('choose').removeClass('prohibit').show();
            if ($(".xxc_choose").hasClass('chooseHit')) {
                $(".xxc_choose").removeClass('chooseHit');
                $('span', '.xxc_choose').removeClass('spanHit');
            }
         
        });

        $('.moreCancel').click(function () {
            $(this).hide();
            $('.moreBg').hide();
            $('.xxc_choose').removeClass('choose').removeClass('prohibit');
            $('.xxc_choose span').removeClass('spanHit');
            $('.chooseHit').removeClass('chooseHit');
            bitchoperateing = false;
            isCopy = true;
            isMore = true;
            loadForms();
        });

        $('.moreBg').click(function () {

            gouxuan = true;
            if (templates.length <= 0) {
                ncUnits.alert("请选择表单进行操作");
                return;
            }
         
            var operate = $.trim($(this).text());
            if (operate == "批量移动") {
             
                //templates.push($(".xxc_choose").attr('termm')); 
                chooseCargoryId.push($(".xxc_choose").attr('trrm'));
           
                $(this).attr('data-toggle', 'modal');
                $(this).attr('data-target', '#formMove_modal');
                $("#formMove_modal .modal-content").load("/Template/GetMove", function () {
                    buildDepartmentTree();
                    //选择文件夹
                    $("#move_modal_search").selection({
                        url: "/Template/selectCaregoryList",
                        hasImage: false,
                        otherParam: { templateId: templates, categoryId: categorys },
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
                                    templates = [];
                                    templates.push(data.id);
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
                    cancelBitchOperate();
                    isMore = true;
                });
                gouxuan = false;
            }
            if (operate == "批量删除") {
                //templates=$(".xxc_choose").attr('term');
                ncUnits.confirm({
                    title: '提示',
                    html: '确认要删除？',
                    yes: function (layer_confirm) {
                        layer.close(layer_confirm); 
                        var dateD = { templateId: templates };
                        $.ajax({
                            type: "post",
                            url: "/Template/DeleteTemp",
                            dataType: "json",
                            data: { data: JSON.stringify(dateD) },
                            success: rsHandler(function (data) { 
                                if (data == 3) {
                                    loadForms();
                                    ncUnits.alert("批量删除成功!");
                                    $('.moreCancel').hide();
                                    $('.moreBg').hide();
                                    $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                                    $('.xxc_choose span').removeClass('spanHit');
                                    cancelBitchOperate();
                                    isMore = true
                                    isCopy = true;
                                } else if (data == 2) {
                                    ncUnits.alert("不能删除，表单正在使用");
                                    loadForms();
                                    cancelBitchOperate(); 
                                    $('.moreCancel').hide();
                                    $('.moreBg').hide();
                                    $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                                    $('.xxc_choose span').removeClass('spanHit');
                                    isMore = true
                                    isCopy = true;
                                } else {
                                    loadForms();
                                    cancelBitchOperate();
                                    ncUnits.alert("批量删除失败!");
                                    $('.moreCancel').hide();
                                    $('.moreBg').hide();
                                    $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
                                    $('.xxc_choose span').removeClass('spanHit');
                                    isMore = true
                                    isCopy = true;
                                }






                            })
                        });
                    }
                });
                gouxuan = false;
            }

        });

        //取消当前批量操作
        function cancelBitchOperate() {
            isMore = true;
            $(".cell").hover(function () {
                $(".operate", this).show();
            }, function () {
                $(".operate", this).hide();
            });
            bitchoperateing = false;
            categorys = [];
            $(".btns").show();
            $('.isfolder').css('cursor', 'pointer');
            $('.moreCancel').hide();
            $('.moreBg').hide();
            $('.xxc_choose').removeClass('choose').removeClass('prohibit').removeClass('chooseHit');
            $('.xxc_choose span').removeClass('spanHit');
        }

        $("#ddlUser").hover(function () {
            $(this).addClass('open');
            argus.hovertype = false;
            $(".form_userdefined").addClass('active');
            $(".form_userdefined").css({ "background": "url(../../Images/flow/自定义表单hit.png) no-repeat" });
            $("span.glyphicon.glyphicon-menu-down.form2").addClass('active');
        }, //mouseenter  
               function () {
                   if (argus.clickType == true) {
                       $(".form_userdefined.active").removeClass('active');
                       $(".form_userdefined").css({ "background": "url(../../Images/flow/自定义表单.png) no-repeat" });
                       $("span.glyphicon.glyphicon-menu-down.form2").removeClass('active');
                   }
               });//mouseleave  

        $("#ddlSystem").hover(function () {
            $(this).addClass('open');
            argus.hovertype = true;
            $(".form_systerm").addClass('active');
            $(".form_systerm").css({ "background": "url(../../Images/flow/系统表单hit.png) no-repeat" });
            $("span.glyphicon.glyphicon-menu-down.form1").addClass('active');
        }, //mouseenter  
                function () {
                    if (argus.clickType == false) {
                        $(".form_systerm.active").removeClass('active');
                        $(".form_systerm").css({ "background": "url(../../Images/flow/系统表单.png) no-repeat" });
                        $("span.glyphicon.glyphicon-menu-down.form1").removeClass('active');
                    }
                });//mouseleave   
    }
    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
 
   
    $("a#formManage").click(function () {
        //$("#syterm_form").off("click");
        //$("#userdefined_form").off("click");
        argus.categoryId = null;
        argus.type = true;
        $(".addFlowClassify").off("click");
        $("a.glyphicon.glyphicon-plus.addFlowClassify").removeClass("addFlowClassify").addClass("addFormClassify");
        if ("undefined" == typeof systemOr ) {
            console.log("systemOfFalse未定义");
            loadForms();
        }
        else if (systemOr == "1") {
            $("#syterm_form").trigger("click");
        } else if ( systemOr  == "0") {
            $("#userdefined_form").trigger("click");
            $("#ddlUser").removeClass("open");
        } 

        $(".addFormClassify").click(function () {
            //$("li.dropdown.addborder").off("click")
            $("#formBuild_modal").modal('show');
            $("#classifyModify_modal").modal('hide');
            cancelBitchOperate();//取消当前批量操作
            isMore = true;
            $('#power-span span:eq(0)').text("");
            $("#input_filename").val("")
            $("#input_filedes").val("");

            createForm();

        })
    })
    //  var jumpFlag = GetQueryString("jumpFlag");
    if ("undefined" != typeof systemOrFalse) {
        if (systemOrFalse == "1" || systemOrFalse == "0") {
            systemOr = systemOrFalse;
            systemOrFalse = undefined;
            $("#formManage").trigger("click");
            //if (GetQueryString("systemOrFalse") == "1") {
            //    $("#syterm_form").trigger("click");
            //} else {
            //    $("#userdefined_form").trigger("click");
            //    $("#ddlUser").removeClass("open");
            //}
        }
    } else {
        console.log("systemOrFalse未定义阿拉了拉");
    }


    //-------------------------------测试---------------------------------------//

    //  var testAllNodeArray = [];            //临时保存的测试数据
    var testTemplate;                  //获取的模板对象 用于获取值 赋值
    var $testActiveNode;               //存储当前点击的测试按钮
    var testNodeArray;
    
    //下拉框事件
    function dropDown(value){
        var span = $(value).parents(".dropdown").find(".dropdown-toggle span:eq(0)");
        $(span).attr("term", $(value).attr("term")).attr("title", $(value).attr("title") );
        $(span).text( $(value).text() );
    }

    // 按ID加载模板
    function testloadDataByTemplateId(templateId, nodeId, nodeType) {
        if (templateId) {
            $.ajax({
                url: "/FlowIndex/GetTemplateInfoById",
                type: "post",
                dataType: "json",
                data: {
                    templateId: templateId,
                    nodeId: nodeId,
                    flag: (nodeType==1? 1:2)
                },
                success: rsHandler(function (data) {	
                    //数据清空
                    $("#test_label-content").empty();
                    $('.stars li').removeClass('liHit');
                    //     $('#testCreateTime').val('');		
                    $('#test_createPerson').val('').attr('term', '');
                    $('#test_createOrg').val('').attr('term', '');
                    $('#test_createPosition span:eq(0)').attr("term",'').text('').attr('title','');
                    $('#test_createPosition ul').empty();
                    $("#flowTestName").val('');

                    if (data.template.defaultTitle == 1) {
                        $("#flowTestName").attr("readonly", "true");
                    } else {
                        $("#flowTestName").removeAttr("readonly");
                    }
                    $('#flow_test_title').text(data.template.templateName);
                    $('#test_main_title').text(data.template.templateName);
                    $('#test_des_title').text(data.template.description);
                    testTemplate = $("#test_label-content").widgetFormResolver(data.controlInfo);

                    //---如果流程测试有数据保存在本地，则显示本地保存数据
                    if ( testNodeArray != null ) {
                        var  v = testNodeArray;
                        $('#flowTestName').val( v.title);
                        $.each($('#testUrgence li'), function (j, v1) {
                            if (j < v.urgency) {
                                $(v1).addClass("liHit");
                            }
                        })
                        $("#test_createPerson").attr("term", v.userId).val(v.userName);
                        $("#test_createOrg").attr("term", v.organizationId).val(v.organizationName);
                        if ($("#test_createPosition").hasClass("OnlyOneSta")) {           //如果岗位确定
                            $("#test_createPosition span:eq(0)").attr("term", v.stationId).text(v.stationName).attr("title", v.stationName);
                            $('#test_createPosition ul').empty().append($('<li><a href="#" class="textOverFlow" term=' + v.stationId + ' title=' + v.stationName + ' >' + v.stationName + '</a></li>'));
                        } else {
                            load_station(v.organizationId, v.stationId, v.userId);          //加载岗位下拉框
                        }

                        setJson(v);               //TODO                                                
                    }					
                })
            })
        }
    }

    //加载节点
    function testLoadNode(templateId) {
        $("#test_node_list ul").empty();
        $("#test_node_list span:eq(0)").text('').attr('term', '');
        $.ajax({
            url: "/FlowTest/GetValidNodeInfoList",
            type: "post",
            dataType: "json",
            data: {
                templateId: templateId,
            },
            success: rsHandler(function (data) {
                if (data && data.length > 0) {
             
                    $.each(data, function (i, v) {
                        if (v.nodeType == 1) {            //默认最初选中创建节点
                            $("#test_node_list span:eq(0)").text(v.nodeName).attr("term", v.nodeId);
                            $("#flow_test_commit").css("width", "50%");
                            $("#flow_test_save").show();
                            $("#flow_test_commit").show();
                            $("#flow_test_modal .modal-footer:hidden").show();
                            testloadDataByTemplateId(templateId, v.nodeId, v.nodeType)             //加载模板
                        }
                        var $li = $('<li><a href="#" term=' + v.nodeId + ' dataType=' + v.nodeType + ' >' + v.nodeName + '</a></li>');
                        $li.find("a").click(function () {
                            dropDown(this);
                            testloadDataByTemplateId(templateId, v.nodeId, v.nodeType)             //加载模板
                            var type = parseInt($(this).attr("dataType"));
                            $("#flow_test_modal .modal-footer").show();
                            $("#flow_test_modal .modal-footer .btn").hide();
                            if (type == 1) {     //创建节点
                                $("#flow_test_commit").css("width", "50%");
                                $("#flow_test_save,#flow_test_commit").show();
                            } else if (type == 2) {
                                $("#flow_test_commit").show();
                                $("#flow_test_commit").css("width", "100%");
                            } else if (type == 3) {
                                $("#flow_test_back,#flow_test_agree").show();
                            } else {
                                $("#flow_test_modal .modal-footer").hide();
                            }
                            if (type == 1) {             //创建节点
                                $("#flowTestName,#testRow input,#testRow button").attr("disabled", false);
                                if ( $("#flow_test_modal .starsR").hasClass("disabledStart") ) {
                                    fiveStart();
                                }
                            } else {
                                $("#flowTestName,#testRow input,#testRow button").attr("disabled", true);
                                $("#flow_test_modal .starsR").addClass("disabledStart");
                                $('#flow_test_modal .stars ul li').off("hover");
                                $('#flow_test_modal .stars ul li').off("click");
                            }
                        })
                        $("#test_node_list ul").append($li);
                    })
                    if ( testNodeArray==null ) {               //按钮不可用
                        $("#test_node_list button").attr("disabled", true);
                    }
                } else {
                    testloadDataByTemplateId(templateId, null,null);
                    $("#flow_test_modal .modal-footer").hide();
                }
               
            }),
            complete: function () {
                console.log("----测试完成哈哈哈哈");
            }
        });
    }

    //模板赋值
    function setJson(data) {
        console.log("data.controlValue" + data.controlValue + "data.controlValue.length" + data.controlValue.length);
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
                        obj.find('.item-content .form-control').val(data.controlValue[i].rowNumberList[0].detailValue[0]);
                    }
                    else if (data.controlValue[i].controlType == 4 || data.controlValue[i].controlType == 5) {  //单选框及复选框
                        obj = $("[itemid=" + data.controlValue[i].controlId + "]");
						
                        $.each(data.controlValue[i].rowNumberList[0].detailValue, function(j,v1){
                            if ( v1  == "false") {
                                obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", false);
                            }
                            else {
                                obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j + ")").prop("checked", true);
                            }
                        });					
                    }
                    else if (data.controlValue[i].controlType == 6) {  //下拉列表
                        obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                        obj.find(".form-control option").each(function () {
                            if ($(this).text() == data.controlValue[i].rowNumberList[0].detailValue[0]) {
                                $(this).attr('selected', 'selected');
                            }
                        });
                    }
                    else if (data.controlValue[i].controlType == 7 || data.controlValue[i].controlType == 8) {  //浏览框(除去文件浏览框)
                        obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                        obj.find("input[class='form-control']").val(data.controlValue[i].rowNumberList[0].detailValue[0]);
                    }
                    else if (data.controlValue[i].controlType == 12 || data.controlValue[i].controlType == 14) { //日期
                        obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                        obj.find(".item-content .form-control").val(data.controlValue[i].rowNumberList[0].detailValue[0]);
                    }
                    else if (data.controlValue[i].controlType == 13 || data.controlValue[i].controlType == 15) {  //日期区间
                        obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                        obj.find(".item-content .form-control:eq(0)").val(data.controlValue[i].rowNumberList[0].detailValue[0]);
                        obj.find(".item-content .form-control:eq(1)").val(data.controlValue[i].rowNumberList[0].detailValue[1]);
                    }
                    else if (data.controlValue[i].controlType == 9) {   //浏览框(文件浏览框)
                        obj = $("[itemid=" + data.controlValue[i].controlId + "]");
                        $.each(data.controlValue[i].rowNumberList[0].detailValue, function (x, v) {
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
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + ( v1.rowNumber - 1) + ")");
                            obj.find('.item-label').text(data.controlValue[i].description);	
                        });

                    }
                    else if (data.controlValue[i].controlType == 1 || data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3 || data.controlValue[i].controlType == 11) { //输入框
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (v1.rowNumber - 1) + ")");
                            obj.find('.item-content .form-control').val( v1.detailValue[0] );
                            if (data.controlValue[i].controlType == 2 || data.controlValue[i].controlType == 3) {
                                obj.find('.item-content .form-control').blur();
                            }
                        });
					    
                    }
                    else if (data.controlValue[i].controlType == 4 || data.controlValue[i].controlType == 5) {  //单选框及复选框
                        $.each(data.controlValue[i].rowNumberList,function(j1,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (v1.rowNumber - 1) + ")");
                            $.each(v1.detailValue,function(j2,v2){
                                if ( v2== "false") {
                                    obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j2 + ")").prop("checked", false);
                                }
                                else {
                                    obj.find("[name=" + data.controlValue[i].controlId + "]:eq(" + j2 + ")").prop("checked", true);
                                }
                            });
                        });
                    }
                    else if (data.controlValue[i].controlType == 6) {  //下拉列表
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (v1.rowNumber - 1) + ")");
                            obj.find(".form-control option").each(function () {
                                if ($(this).text() ==v1.detailValue[0]) {
                                    $(this).attr('selected', 'selected');
                                }
                            });
                        });
                    }
                    else if (data.controlValue[i].controlType == 7 || data.controlValue[i].controlType == 8) {  //浏览框(非文件浏览框)
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (v1.rowNumber - 1) + ")");
                            obj.find("input[class='form-control']").val(v1.detailValue[0]);
                        });
                    }
                    else if (data.controlValue[i].controlType == 12 || data.controlValue[i].controlType == 14) { //日期
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + ( v1.rowNumber - 1) + ")");
                            obj.find(".item-content .form-control").val( v1.detailValue[0]);
                        });
                    }
                    else if (data.controlValue[i].controlType == 13 || data.controlValue[i].controlType == 15) {  //日期区间
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (v1.rowNumber - 1) + ")");
                            obj.find(".item-content .form-control:eq(0)").val(v1.detailValue[0]);
                            obj.find(".item-content .form-control:eq(1)").val(v1.detailValue[1]);
                        });
                    }
                    else if (data.controlValue[i].controlType == 9) {   //浏览框(文件浏览框)
                        $.each(data.controlValue[i].rowNumberList,function(j,v1){
                            obj = $("[itemid=" + data.controlValue[i].controlId + "]:eq(" + (v1.rowNumber - 1) + ")");
                            $.each( v1.detailValue, function (x, v) {
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
                                                //window.location.href = "/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1";
                                                loadViewToMain("/FlowIndex/Download?displayName=" + escape(displayName) + "&saveName=" + thisSaveName + "&flag=1");
                                            }
                                            else {
                                                ncUnits.alert("文件不存在，无法下载!");
                                            }
                                            return;
                                        });
                                    });
                                }
                            })
                        });                   
                    }
                }
            }
        }
    }


    //加载流程图
    function testLoadPic(templateId) {
        $("#test_flowPic").empty();
        $.ajax({
            url: "/FlowTest/DisplayFlowChart",
            type: "post",
            dataType: "json",
            data: { templateId: templateId },
            success: rsHandler(function (data) {
                $("#linkTestRatio").text("节点出口测试率 : "+ data.linkTestRatio +"%");
                $("#test_flowPic").flowChart(data);
            })
        });
    }


    function fiveStart() {
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

    }
    //测试流程图打开
    $("#flow_test_modal").on('show.bs.modal', function () {

        //定义临时数组，存放临时保存的数据
        testNodeArray = null;
        $("#test_rightModal").css({
            "left": ($("#test_rightModal").closest(".modal-dialog").width() - 5)
        });

        fiveStart();
        testLoadNode(testflow_templateId);
        testLoadPic(testflow_templateId);
	
		
        //保存
        $("#flow_test_save").off("click");
        $("#flow_test_save").click(function () {
            testSave();
        })

        //同意
        $("#flow_test_agree").off("click");
        $("#flow_test_agree").click(function () {
            testAgreeBackCommit("同意");
        })

        //退回
        $("#flow_test_back").off("click");
        $("#flow_test_back").click(function () {
            testAgreeBackCommit("退回");
        })

        //提交		     
        $("#flow_test_commit").off("click");
        $("#flow_test_commit").click(function () {
            testAgreeBackCommit("提交");
        })

		
    });


    //判断某变量是否具有非法字符
    function justifyByLetter(txt, name) {
        var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            ncUnits.alert(name);
            return false;
        }
        return true;
    }
    
    //保存
    function testSave() {
        $(".xubox_layer").remove();
        var title = $('#flowTestName').val();
        var urg = $('#testUrgence .liHit').length;
        var personId = parseInt($("#test_createPerson").attr("term"));
        var personName = $("#test_createPerson").val();
        var orgId = parseInt($("#test_createOrg").attr('term'));
        var orgName = $("#test_createOrg").val();
        var staId = parseInt($('#test_createPosition span:eq(0)').attr('term'));
        var staName = $('#test_createPosition span:eq(0)').text();

        var result = testTemplate.getOtherJson();
        if (result == false) {
            ncUnits.alert("保存本地失败!");
            return false;
        }
        var nodeId = parseInt($("#test_node_list span:eq(0)").attr("term"));

        if (testNodeArray != null) {        //曾经被保存过                                 
            testNodeArray.title = title;
            testNodeArray.urgency = urg;
            testNodeArray.organizationId = orgId;
            testNodeArray.organizationName = orgName,
            testNodeArray.stationName = staName
            testNodeArray.stationId = staId;
            testNodeArray.userId = personId;
            testNodeArray.userName = personName,
            testNodeArray.controlValue = result;
        } else {                               //没有被保存过
            testNodeArray = {
                nodeId: nodeId,
                title: title,
                urgency: urg,
                organizationId: orgId,
                organizationName: orgName,
                stationId: staId,
                stationName: staName,
                userId: personId,
                userName: personName,
                controlValue: result
            };
        }
        ncUnits.alert("保存本地成功!");
        return true;
    }

    //同意 退回 提交
    function testAgreeBackCommit(flag) {
        $(".xubox_layer").remove();
        if (flag == "提交") {
            if (testSave() == false) {
                return;
            }
        }

        var url;
        if (flag == "同意") {
            url = "/FlowTest/AgreeFlowForm";
        } else if (flag == "退回") {
            url = "/FlowTest/SendBackFlowForm";
        } else if (flag == "提交") {
            if ($("#flow_test_save").is(":visible") == true) {           //创建节点的提交
                url = "/FlowTest/SubmitNewForm";
            } else {
                url = "/FlowTest/SubmitFlowForm";
            }
        }

        var title = $('#flowTestName').val();
        var urg = $('#testUrgence .liHit').length;
        var personId = parseInt($("#test_createPerson").attr("term"));
        var orgId = parseInt($("#test_createOrg").attr('term'));  
        var staId = parseInt($('#test_createPosition span:eq(0)').attr('term'));
        var currentNode = parseInt($("#test_node_list span:eq(0)").attr("term"));
        var result = testTemplate.getJson();
        if (result == false) {
            ncUnits.alert(flag+"出错!");
            return;
        }

        //输入检查
        if (title == null) {
            ncUnits.alert("标题不得为空!");
            $("#flowTestName").focus();
            return;
        } else if (justifyByLetter(title, "标题") ==false ) {
            return;
        }

        if ( isNaN( personId ) ) {
            ncUnits.alert("创建人不得为空!");
            return;
        }
        if ( isNaN(orgId) ) {
            ncUnits.alert("创建部门不得为空!");
            return;
        }
        if (isNaN(staId ) ) {
            ncUnits.alert("创建岗位不得为空!");
            return;
        }


        var dataval = {
            templateId: testflow_templateId,
            organizationId: orgId,
            stationId: staId,
            title: title,
            urgency: urg,
            status:null,
            currentNode: currentNode,
            createUser:personId,
            //        createTime: createtime,
            controlValue: result
        };
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data: { data : JSON.stringify(dataval) },
            success: rsHandler(function (data) {
                if (data) {
                    if (flag == "提交") {               //提交成功下拉框可选
                        $("#test_node_list button").attr("disabled", false);
                    }
                    ncUnits.alert( flag+"成功!");
                    testLoadPic(testflow_templateId);
                }
                else {
                    ncUnits.alert(flag + "失败!");
                }
            })
        });

    }


    //测试流程图关闭
    $("#flow_test_modal").on("hidden.bs.modal", function () {
        $(".xubox_layer").remove();
        $("#test_label-content").empty();
        $('.stars li').removeClass('liHit');
        //  $('#testCreateTime').val('');		
        $('#test_createPerson').val('').attr('term', '');
        $('#test_createOrg').val('').attr('term', '');
        $('#test_createPosition span:eq(0)').attr("term",'').text('').attr('title','');
        $('#test_createPosition ul').empty();
		
        $('#test_node_list span:eq(1)').text('').attr("term",'');
        $("#test_node_list ul").empty();
		
        $("#flow_test_save").hide();
        $("#flow_test_commit").hide();
        $("#flow_test_back").hide();
        $("#flow_test_agree").hide();
        $("#flow_test_modal .modal-footer").hide();
        $("#flowTestName,#testRow input,#testRow button").attr("disabled", false);
        testNodeArray = null;
    });


    //人员选择模态框
    $("#test_createPerson").click(function () {
        $("#HR_modal").modal('show');
    })

    var personWithSub, personOrgId = null, personOrgName;

    $("#HR_modal").on("show.bs.modal", function () {

        $('#person-selectall').parent().parent().hide();

        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#HR_modal_folder"), $.extend({
                    callback: {
                        onClick: function (e, id, node) {
                            var checked = $("#HR-haschildren").prop('checked');
                            personWithSub = checked == true ? 1 : 0;
                            personOrgId = node.id;
                            personOrgName = node.name;
                            addUserList();
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
        //包含下级
        $("#HR-haschildren").off("click");
        $("#HR-haschildren").click(function () {
            if ($(this).is(":checked")) {
                personWithSub = 1;
            } else {
                personWithSub = 0;
            }
            if (personOrgId != null) {
                addUserList();
            }
        });
    });
	
    $("#HR_modal").on("hide.bs.modal", function () {
        $("#HR-haschildren").prop("checked", false);
        $(".person_list").empty();
        $("#HR_modal_chosen").empty();
        $('#HR_modal_chosen_count').text(0);
        personWithSub = 0;
        personOrgId = null;

    });
    //添加人员列表
    function addUserList(){
        $.ajax({
            type: "post",
            url: "/Shared/GetUserList",
            dataType: "json",
            data: { withSub: personWithSub, organizationId: personOrgId },
            success: rsHandler(function (data) {
                $(".person_list").empty();
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

    //人员复选框点击事件
    function appendperson() {
        $(".person_list input[type='checkbox']").click(function () {
            var checked = $(this).prop('checked');
            var personId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
            var personName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
            var showflag = true;
            if (checked == true) {        
                $(".person_list ul input:checked").prop("checked", false);
                $(this).prop('checked', true);
                $('#HR_modal_chosen_count').text(1);
                var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $close.click(function () {
                    $(".person_list ul input:checked").prop("checked", false);
                    $(this).parent().remove();
                    $('#HR_modal_chosen_count').text(0);
                });
                $("#HR_modal_chosen").empty().append($checked.append($close));
            } else {
                $(this).prop('checked', false);
                $("#HR_modal_chosen").empty();
                $('#HR_modal_chosen_count').text(0);             
            }
        });
    }


    //人员搜索
    $("#HR_modal_search").selection({
        url: "/Shared/SelectUserInfoList",
        hasImage: true,
        selectHandle: function (data) {
            $("#HR_select").val(data.userName);
            var flag = true;
            if ($("#HR_modal_chosen li").length > 0) {
                $("#HR_modal_chosen li").each(function () {
                    if ($(this).attr('term') == data.userId) {
                        flag = false;
                    }
                });
            }
            if (flag == true) {
                personOrgId = data.organizationId;
                personOrgName = data.organizationName;
                var $checked = $("<li term=" + data.userId + " dataStaName=" + data.stationName + " dataStaId=" + data.stationId + "><span>" + data.userName + "</span></li>"),
                           $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $("#HR_modal_chosen").empty().append($checked.append($close));
                $close.click(function () {
                    var nodeId = $(this).parent().attr("term");
                    $(this).parent().remove();
                    $("#HR_modal_chosen_count").text(0);
                    $(".person_list ul").each(function () {
                        if ($(this).find("li:eq(1)").attr('term') == nodeId) {
                            $(this).find("input[type='checkbox']").prop("checked", false);
                        }
                    });
                });
            }
            if ($(".person_list ul").length > 0) {
                $(".person_list ul").each(function () {
                    if ($(this).find("li:eq(1)").attr('term') == data.userId.toString()) {
                        $(this).find("input[type='checkbox']").prop("checked", true);
                    }
                });
            }
            $("#HR_modal_chosen_count").text($("#HR_modal_chosen li").length);
        }
    });

    //选择人员确定按钮
    $("#HR_modal_submit").click(function () {
        if ($('#HR_modal_chosen_count').text() == "1" ) {
            var $li  =$("#HR_modal_chosen li");
            var personId = parseInt( $li.attr("term") );
            $("#test_createPerson").val($("#HR_modal_chosen li>span").text()).attr("term", personId);
            $("#test_createOrg").attr("term", personOrgId).val(personOrgName);
            //获取岗位
            if ( $li.attr("dataStaId") != undefined ) {
                $('#test_createPosition span:eq(0)').attr("term", $li.attr("dataStaId")).text($li.attr("dataStaName")).attr('title', $li.attr("dataStaName"));
                $('#test_createPosition ul').empty().append($('<li><a href="#" class="textOverFlow" term=' + $li.attr("dataStaId") + ' title=' + $li.attr("dataStaName") + ' >' + $li.attr("dataStaName") + '</a></li>'));
                $('#test_createPosition').addClass("OnlyOneSta");
            } else {
                $('#test_createPosition.OnlyOneSta').removeClass("OnlyOneSta");
                load_station(personOrgId, null, personId);
            }
        }
        $("#HR_modal").modal('hide');
    });

    //加载岗位
    function load_station(orgId, staId, userId) {
        $('#test_createPosition span:eq(0)').attr("term", '').text('').attr('title', '');
        $('#test_createPosition ul').empty();
        $.ajax({
            type: "post",
            url: "/FlowTest/GetCreateUserStationList",
            dataType: "json",
            data: { userId: userId, orgId: orgId },
            success: rsHandler(function (data) {
                if (data && data.length > 0) {
                    var $departDrop = $('#test_createPosition ul');
                    $departDrop.empty();
                    $.each(data, function (i, v) {
                        console.log("v" + v);
                        if ((staId != null && v.id == staId) || (staId == null && i == 0)) {      //无默认岗位时，默认加载第一个岗位
                            $("#test_createPosition span:eq(0)").attr("term", v.id).text(v.name).attr("title", v.name);
                        }
                        var $li = $('<li><a href="#" class="textOverFlow" term=' + v.id + ' title=' + v.name + ' >' + v.name + '</a></li>');
                        $li.find("a").click(function () {
                            dropDown(this);
                        })
                        $departDrop.append($li);
                    });
                }
            })
        });
    }
  
    //结束测试
    $("#endTestBtn").click( function (){
            $.ajax({
            type: "post",
            url: "/FlowTest/StopFlowTest",
            dataType: "json",
            data: {templateId:testflow_templateId },
            success: rsHandler(function (data) {
                if (data) {
                    $testActiveNode.closest(".cell").find("div.Use").attr("term", true);
                } else {
                    $testActiveNode.closest(".cell").find("div.Use").attr("term", false);
                }
                $("#flow_test_modal").modal('hide');
            })
        });
    });

})




