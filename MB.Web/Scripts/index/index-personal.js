var page_flag = "plan";    //页面标志
$(function () {
    //右侧个人信息
    loadPersonalInfo();

    var planCreater = addPlan();

    var plan_lodi = getLoadingPosition('#myPlanlist .planUl');
    $("#myPlanlist .planUl").load("/User/GetUserList", function () {
        $("#myPlanlist .addCom").click(function () {
            planCreater.addPlan();
        });
        plan_lodi.remove()
    });

   
    //进入流程
    $("#gotoflow").click(function () {
        loadViewToMain("/FlowIndex/FlowIndex/2");
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
                    if (addflow_defaultTitle == 1) {
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
                    addflow_createTime = addflow_createTime.replace('-', '/').replace('-', '/');
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
                                if (addflow_defaultTitle == 1) {
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
            if (addflow_content.title == "") {
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
    function justifyByLetter(txt, name, obj) {
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

   
    /*加载流程模块 开始*/
    loadFlows();
    var flow_lodi = getLoadingPosition('.flow .planUl');//显示load层
    function loadFlows() {
        $.ajax({
            type: "post",
            url: "/FlowIndex/GetUserUnCompleteList",
            dataType: "json",
            complete: rcHandler(function () {
                flow_lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {
                if (data && data.length > 0) {
                    $('.flow .planUl').empty();
                    $.each(data, function (i, v) {
                        var $liflow = $("<li style='cursor: pointer;' data-toggle='modal' data-target='#details-modal' formid='" + v.formId + "' nodeId='" + v.nodeId + "'  templateId='" + v.templateId + "' status='" + v.status + "'><a href='#'><span class='text flowtitle' title='" + v.title + "'>" + v.title + "t</span></a><span class='index_span_text'>" + v.createTime.replace('T', ' ').substr(0, 16) + "</span></li>");
                        var $liLine = $("<li class='line'></li>");
                        $('.flow .planUl').append([$liflow, $liLine]);
                        //详情
                        var newVal;
                        $liflow.click(function () {
                            var templateId = $(this).attr('templateId');
                            var formId = $(this).attr('formId');
                            var nodeId = $(this).attr('nodeId');
                            var operateStatus = $(this).attr('status');
                            $("#details-modal .modal-content").load("/FlowIndex/LoadDetail", function () {
                                if (operateStatus == 1) {
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
                                                        var $content = $(" <li><a href='#'  term=" + i.id + " class='xxc_detail_departmentName'>" + i.name + "</a></li><li class='divider short'></li>");
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
                                                            if ($(this).text() == stationName) {
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
                                                    if (operateStatus == 1) {  //待提交
                                                        $flowdetail_cancel.css("width", "29.65%");
                                                        $flowdetail_save.css("width", "29.65%");
                                                        $flowdetail_submit.css("width", "29.65%");
                                                        $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_save, $flowdetail_submit]);
                                                        $("#xxc_detailsuggest").hide();
                                                    }
                                                    else if (operateStatus == 6) {  //待审核
                                                        $flowdetail_cancel.css("width", "29.65%");
                                                        $flowdetail_back.css("width", "29.65%");
                                                        $flowdetail_agree.css("width", "29.65%");
                                                        $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_back, $flowdetail_agree]);
                                                        $("#xxc_detailsuggest").show();
                                                    }
                                                    else if (operateStatus == 10) {    //流程中的提交
                                                        $flowdetail_cancel.css("width", "29.65%");
                                                        $flowdetail_save.css("width", "29.65%");
                                                        $flowdetail_submitMag.css("width", "29.65%");
                                                        $('.flowdetail_operate').append([$flowdetail_cancel, $flowdetail_save, $flowdetail_submitMag]);
                                                        $("#xxc_detailsuggest").show();
                                                    }
                                                    else {
                                                        $("#xxc_detailsuggest").hide();
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
                                                        if (operateStatus == 1) {
                                                            flag = 1
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
                                                            data: { templateId: templateId, formId: formId, nodeId: nodeId, flag: 1, data: JSON.stringify(formInfo) },
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
                    });
                }
            })
        });
    }

    /*加载流程模块 结束*/

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


    $.datepicker.setDefaults({
        closeText: "确认", // Display text for close link
        prevText: "上个月", // Display text for previous month link
        nextText: "下个月", // Display text for next month link
        currentText: "本月", // Display text for current month link
        monthNames: ["一月","二月","三月","四月","五月","六月","七月","八月","九月","十月","十一月","十二月"], // Names of months for drop-down and formatting
        monthNamesShort: ["一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二"], // For formatting
        dayNames: ["周日", "周一", "周二", "周三", "周四", "周五", "周六"], // For formatting
        dayNamesShort: ["日", "一", "二", "三", "四", "五", "六"], // For formatting
        dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"], // Column headings for days starting at Sunday
        weekHeader: "周", // Column header for week of the year
        dateFormat: "mm-dd-yy", // See format options on parseDate
        firstDay: 1, // The first day of the week, Sun = 0, Mon = 1, ...
        yearSuffix: "年" // Additional text to append to the year in the month headers
    });
    //切换周计划与下属计划
    $('.TTwo').click(function () {
        $(this).css({ 'color': '#58b557', 'font-size': '14px' });
        $('.TOne').css({ 'color': '#bbb', 'font-size': '12px' });

    });
    $('.TOne').click(function () {
        $(this).css({ 'color': '#58b557', 'font-size': '14px' });
        $('.TTwo').css({ 'color': '#bbb', 'font-size': '12px' });
    });
    /* 折线图 开始 */
    // Graph Data ##############################################
    var statistics_type = 0,
    statistics_mode = 0,
    date = new Date(),
    thisyear = year = date.getFullYear(),
    thismonth = month = date.getMonth() + 1,
    $cur = $("#curTime"),
    $last = $(".LMonth.last"),
    $next = $(".LMonth.next");
    var calendarId;
    var partners = [];

    function chartRender() {
        var lodi = getLoadingPosition('#personalWorktime');
        $.ajax({
            type: "post",
            url: "/UserIndexWorkTime/getJson",
            dataType: "json",
            data: {
                mode: statistics_mode,
                year: year,
                month: month
            },
            complete: rcHandler(function () {
                lodi.remove();         //关闭load层
            }),
            success: rsHandler(function (data) {

                var graphData = [{
                    // Visits
                    data: [],
                    color: '#fbac0f',
                    points: { radius: 4, fillColor: '#fbac0f' }
                }, {
                    // Visits
                    data: [],
                    color: '#58b456',
                    points: { radius: 4, fillColor: '#58b456' }
                }
                ];
                for (var i = 0, len = data.length; i < len; i++) {
                    graphData[0].data.push([data[i].workdate, data[i].totaleffective]);
                    graphData[1].data.push([data[i].workdate, data[i].totalwork]);
                }

                $.plot($('#graph-lines'), graphData, {
                    series: {
                        points: {
                            show: true
                            //radius: 5//第一条线的圈大小
                        },
                        lines: {
                            show: true//点间连接线
                        }
                        //shadowSize: 5
                    },
                    grid: {
                        color: '#000',
                        borderColor: 'transparent',
                        //borderWidth: 20,
                        hoverable: true
                        , labelMargin: 10
                    },
                    // 横坐标
                    xaxis: {
                        ticks: statistics_mode == 0 ? [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] : [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31]
                        //,tickFormatter: function (axis) {
                        //    return axis.toString() + (statistics_mode == 0 ? "月" : "日");
                        //}
                        , labelWidth: 28
                        , tickColor: 'transparent'
                    },
                    // 纵坐标
                    yaxis: {
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
                    //,
                    //// 纵坐标
                    //yaxis: {
                    //    tickSize: 87//纵坐标每一格差值
                    //}
                });
            })
        });
    }


    $("#statistics_title ul li a").click(function () {
        statistics_type = $(this).attr("value");
        $("#statistics_title .TOne").html($(this).html());
        chartRender();
    });


    /* 年月功效 开始 */

    //$(document).ready(function () {
    $cur.html(year + "年");
    $last.html(year - 1);
    $next.html(year + 1);
    chartRender();
    // })

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
            $cur.html(year + "年");
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
            $cur.html(year + "年" + month + "月");
            $last.html(month == 1 ? 12 : month - 1);
            $next.html(month == 12 ? 1 : month + 1);
        }
    }
    /* 时间写入 结束 */
    /* 折线图 结束 */



    /* 年月工效切换 开始 */
    $('.conLTwo .year').addClass('YMHit');
    $('.conLTwo .year').click(function () {
        $('.conLTwo .month').removeClass('YMHit');
        $(this).addClass('YMHit');
    });
    $('.conLTwo .month').click(function () {
        $('.conLTwo .year').removeClass('YMHit');
        $(this).addClass('YMHit');
    });

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
            //  previousPoint = item.dataIndex;
            //$('#tooltip').remove();
            var data = item.series.data,
                index = item.dataIndex,
                content = [];

            showTooltip(item.pageX, item.pageY, "工时：" + data[index][1]);
            //}
        } else {
            $('#tooltip').stop();
            $('#tooltip').fadeOut(200);
            //previousPoint = null;
        }
    });
    /* 年月工效切换 结束 */

    /*日程安排 开始*/
    var date = {}, day, year, month;
    var html = ''; //将要添加的HTML
    var flag = false;//判断是否对日历进行了选择
    var lay_Agenda; //日程弹窗
    var dayIndex = 0;
    $('.calendar').datepicker({
        prevText: '前一月',
        nextText: '后一月',
        yearSuffix: '年',
        dateFormat: "yy-mm-dd",
        showMonthAfterYear: true,
        firstDay: 0,
        beforeShowDay: function (date) {
            var i = dayIndex++;
            $.ajax({
                url: '/User/getcalendarJson',
                type: 'POST',
                dataType: 'json',
                data: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()
                },
                success: rsHandler(function (data) {
                    if (data.length) {
                        $(".calendar tbody td:eq(" + i + ")").addClass("ui-datepicker-info");
                    }
                })
            })
            return [true, ""];
        },
        // changeYear:true,
        // changeMonth:true,
        showOtherMonths: true,
        selectOtherMonths: true,
        altField: '#showCalendarDate',
        altFormat: 'yy' + '年' + 'mm' + '月' + 'dd' + '日',
        // yearRange:'1900:',
        onSelect: function (dateText, instance) {
            dayIndex = 0;
            flag = true;
            year = dateText.substr(0, 4);
            month = dateText.substr(5, 2);
            day = dateText.substr(8, 2);
            // if($('.wrapThings p').css('display')=='block'){
            //     $('.calendarDetails').datepicker('setDate', dateText);
            //     $('.wrapThings p').trigger('click');
            // }
            date = { 'year': year, 'month': month, 'day': day };
            if (html != '') {
                $('.wrapThings').html('');
                html = '';
                $('.calThing .arrowRight').css('display', 'none');
                $('.calThing .arrowLeft').css('display', 'none');
            }
            $.ajax({
                url: '/User/getcalendarJson',
                type: 'POST',
                dataType: 'json',
                data: date,
                success: rsHandler(function (data) {
                    // var data = [];
                    // var json = $.parseJSON(text);
                    // var json = $.parseJSON(response);
                    html = '';
                    $.each(data, function (index, value) {
                        //alert(value.calendarId);
                        //alert(value.place);
                        if (value.place == null) {
                            value.place = '';
                        }
                        if (value.comment == null) {
                            value.comment = '';
                        }
                        html += '<p><span class="calendarId" style="display:none;">' + value.calendarId + '</span>时间：' +
                                '<span class="strTime"><span class="valStartTime">' + value.FstartTime + '</span> - ' +
                                '<span class="valEndTime">' + value.FendTime + '</span></span><br />地址：' +
                                '<span class="valAddress">' + value.place + '</span><br />备忘：' +
                                '<span class="valMemo">' + value.comment + '</span><br />标签：' +
                                '<span class="valTags">' + value.tag + '</span>' +
                                '<span class="valPartners" style="display:none;">' + value.partner + '</span></p>';
                    });

                    $('.wrapThings').append(html);
                    $('.wrapThings').find('p:first').css('display', 'block');
                    var thingCarousel = {}; //定义一个事件轮播对象，存放事件的数目count、事件轮播当前播放的索引以及轮播的定时器        
                    thingCarousel.n = 0; //当前轮播索引
                    thingCarousel.timer = null;//轮播定时器
                    thingCarousel.count = $('.wrapThings p').length; //count是p的总数
                    if (thingCarousel.count > 1) { //如果事件数量大于1，就进行轮播
                        // $('.calThing .arrowRight').css('display','block');
                        $('.calThing .arrowRight').css('display', 'block');
                        // 进行自动轮播
                        thingCarousel.timer = setInterval(showNext, 4000);
                        //鼠标悬浮停止轮播
                        $('.calThing').hover(function () {
                            clearInterval(thingCarousel.timer);
                        }, function () {
                            thingCarousel.timer = setInterval(showNext, 4000);
                        });
                        //下一个点击轮播
                        $('.calThing .arrowRight').off("click")
                        $('.calThing .arrowLeft').off("click")
                        $('.calThing .arrowRight').click(function () {
                            showNext();
                        });
                        //上一个点击轮播
                        $('.calThing .arrowLeft').click(function () {
                            showPrev();
                        });
                        //轮播下一个函数
                        function showNext() {
                            if (thingCarousel.n >= (thingCarousel.count - 1)) {
                                thingCarousel.n = 0;
                                $('.calThing .arrowLeft').css('display', 'none');
                                $('.calThing .arrowRight').css('display', 'block');
                            } else {
                                thingCarousel.n++;
                                if (thingCarousel.n == (thingCarousel.count - 1))
                                    $('.calThing .arrowRight').css('display', 'none');
                                $('.calThing .arrowLeft').css('display', 'block');
                            }
                            $('.wrapThings p').filter(':visible').hide().parent().children().eq(thingCarousel.n).show();
                        }
                        //轮播上一个函数
                        function showPrev() {
                            if (thingCarousel.n <= 0) {
                                thingCarousel.n = (thingCarousel.count - 1);
                                $('.calThing .arrowLeft').css('display', 'block');
                                $('.calThing .arrowRight').css('display', 'none');
                            } else {
                                thingCarousel.n--;
                                if (thingCarousel.n == 0)
                                    $('.calThing .arrowLeft').css('display', 'none');
                                $('.calThing .arrowRight').css('display', 'block');
                            }
                            $('.wrapThings p').filter(':visible').fadeOut().parent().children().eq(thingCarousel.n).fadeIn();
                        }
                    }

                    /* 日程详情弹窗 开始 */
                    var tags = []//保存标签至编辑界面
                    partners = [];//保存参与者至编辑界面
                    //var calendarId;
                    $('.wrapThings p').click(function () {
                        lay_Agenda = $.layer({
                            type: 1,
                            shade: [0.5, '#000'],
                            area: ['auto', 'auto'],
                            title: false,
                            border: [0],
                            page: { dom: '.layer_Agenda' },
                            move: ".drapdiv",
                            closeBtn: false
                        });
                        $('.layer_Agenda .canCon span:eq(0)').click(function () {
                            layer.close(lay_Agenda);
                            tags = [];
                            partners = [];
                        });
                        fnPopUpHeight($('.layer_Agenda'));
                        $('.showInfo').css('display', 'block');
                        $('.addAgendaWidget').css('display', 'none');
                        $('#confirmAgenda').html('编辑')
                        calendarId = $(this).find('.calendarId').text();
                        //calendarId = $(this).find('.calendarId').text();
                        //partners.push(data.id);
                        //alert(calendarId);
                        // console.log(calendarId);
                        // calendarId = 21;
                        $.ajax({
                            url: '/User/getCalendarJsonByCid',
                            dataType: 'json',
                            type: 'post',
                            data: { 'calendarId': calendarId },
                            success: function (data) {  
                                $('.layer_Agenda .showTime').html(data.data[0].FstartTime + ' - ' + data.data[0].FendTime);
                                $('.layer_Agenda .showAddr').html(data.data[0].place);
                                $('.layer_Agenda .showMemo').html(data.data[0].comment);
                                var re = new RegExp(',', 'g');
                                var strTags = data.data[0].tag.toString().replace(re, '&nbsp&nbsp&nbsp&nbsp');
                                $('.layer_Agenda .showTag').html(strTags);
                                var strPartners = data.data[0].partnerName.toString().replace(re, '；');
                                $('.layer_Agenda .showPartner').html(strPartners);

                            }
                        });
                        $('#confirmAgenda').click(function () {
                            if ($('#confirmAgenda').html() == '编辑') {
                                $('.showInfo').css('display', 'none');
                                $('.addAgendaWidget').css('display', 'block');
                                $('#confirmAgenda').html('确定');
                                $.ajax({
                                    url: '/User/getCalendarJsonByCid',
                                    dataType: 'json',
                                    type: 'POST',
                                    data: { 'calendarId': calendarId },
                                    success: function (data) {
                                        $('.layer_Agenda .startTime').val(data.data[0].FstartTime);
                                        $('.layer_Agenda .endTime').val(data.data[0].FendTime);
                                        $('.layer_Agenda .inputAddr').val(data.data[0].place);
                                        $('.layer_Agenda .inputMemoArea').val(data.data[0].comment);
                                        tags = data.data[0].tag;
                                        partners = data.data[0].partnerName;
                                        partner_v = data.data[0].partner;
                                        $.each(tags, function (index) {
                                            $('#addAgenda_label').before('<span class="tag tagsSpan">' + tags[index] + '<span class="close" style="display: none;">X</span></span>');
                                        });
                                        tags = [];
                                        $.each(partners, function (index) {
                                            console.log(partners);
                                            $('#addAgenda_partner').before('<span class="tag">' + partners[index] + '<span class="close" style="display: none;">X</span></span>');
                                        });
                                        partners = [];
                                        $('.tag').hover(function () {
                                            $(this).find('.close').toggle();
                                        });
                                        $('.close').click(function () {
                                            $(this).parent().remove();
                                        })
                                    }
                                });
                            }

                        });
                        $(".layer_Agenda .closeWCom").click(function () {
                            layer.close(lay_Agenda);
                        });
                    });
                })
            });
        },
        onChangeMonthYear: function (year, month, ins) {
            dayIndex = 0;
            $('.calenYear').val(year);
            var myMonth = parseInt(month) < 10 ? '0' + month : month;
            $('.calenMonth').val(myMonth);
        }
    });


    //加载月份事务
    function loadMonthThing(monthJson) {
        $('.calendarDetails tr a').click(function () {
            event.stopPropagation();
        });
        $(".calendarDetails td").unbind("click");
        $.each(monthJson, function (index, value) {
            $('.calendarDetails tr a').each(function () {
                if ($(this).html() == value.day) {
                    $(this).css({ 'background-color': '#EB2D2C', 'color': '#fff' });
                    $(this).parent().css('position', 'relative');
                    var html = '';
                    $.each(value.schedule, function (index, value) {
                        html += '<div class="dateThings">' +
                                        '<small>开始时间：' + value.startTime + '</small>' +
                                        '<small>结束时间:' + value.endTime + '</small>' +
                                        '<small>地点:' + value.place + '</small>' +
                                        '<small>内容:' + value.comment + '</small>' +
                                        '<small>标签:' + value.tag + '</small>' +
                                    '</div>';
                    });
                    $(this).parent().append(html);
                    $(this).parent().find('div:first').css('display', 'block');
                    var thingCarousel = {}; //定义一个事件轮播对象，存放事件的数目count、事件轮播当前播放的索引以及轮播的定时器        
                    thingCarousel.n = 1; //当前轮播索引
                    thingCarousel.timer = null;//轮播定时器
                    thingCarousel.count = $(this).parent().find('div').length; //count是p的总数
                    if (thingCarousel.count > 1) { //如果事件数量大于1，就进行轮播
                        $(this).parent().append('<span class="arrowRight"></span>');
                        $(this).parent().append('<span class="arrowLeft"></span>');
                        $('.calendarDetails tr .arrowRight').css('display', 'block');
                        $('.calendarDetails tr .arrowLeft').css('display', 'block');
                        //设置点击轮播
                        $('.calendarDetails tr .arrowRight').click(function () {
                            showDayThingN();
                            event.stopPropagation();
                        });
                        $('.calendarDetails tr .arrowLeft').click(function () {
                            showDayThingP();
                            event.stopPropagation();
                        });
                        // 轮播下一个函数
                        function showDayThingN() {
                            if (thingCarousel.n >= (thingCarousel.count)) {
                                thingCarousel.n = 1;
                            } else {
                                thingCarousel.n++;
                            }
                            $('.calendarDetails tr span').parent().find('div').filter(':visible').hide().parent().children().eq(thingCarousel.n).show();
                            // $('.calendarDetails tr span').parent().find('div').filter(':visible').hide();
                        }
                        // 轮播上一个函数
                        function showDayThingP() {
                            if (thingCarousel.n <= 1) {
                                thingCarousel.n = thingCarousel.count;
                            } else {
                                thingCarousel.n--;
                            }
                            $('.calendarDetails tr span').parent().find('div').filter(':visible').hide().parent().children().eq(thingCarousel.n).show();
                            // $('.calendarDetails tr span').parent().find('div').filter(':visible').hide();
                        }
                    }
                }
            });
        });
    }

    /*日程安排 结束*/

    /*设置日历输入框 开始*/
    if (!flag) {
        var currentDay = new Date();
        day = currentDay.getDate();
    }
    year = $('#showCalendarDate').val().substr(0, 4);
    month = $('#showCalendarDate').val().substr(5, 2);
    $('.calenYear').val(year).attr('placeholder', $('.calenYear').val());
    $('.calenMonth').val(month).attr('placeholder', $('.calenMonth').val());

    $('.calenYear').blur(function () {
        dayIndex = 0;
        var strP = /^\d+$/;
        if ($(this).val() == '') { $(this).val(year); }
        if (parseInt($(this).val()) > 2020 || parseInt($(this).val()) < 2005 || !strP.test($(this).val())) {
            // ncUnits.confirm('你输入的年份有错误。年份范围是1900年至下一年。');
            ncUnits.confirm({
                title: '提示',
                html: '你输入的年份有错误。年份范围是1900年至下一年。请重新输入。'
            });
            $(this).val(year);
        }
        $('.calendar').datepicker('setDate', $(this).val() + '-' + $('.calenMonth').val() + '-' + '10');
        $('.ui-state-active').css({ 'background': '#f9f9f9', 'color': '#222' });
    });
    $('.calenMonth').blur(function () {
        dayIndex = 0;
        var strP = /^\d+$/;
        if ($(this).val() == '') { $(this).val(month); }
        if (parseInt($(this).val()) > 12 || parseInt($(this).val()) <= 0 || !strP.test($(this).val())) {
            // alert('你输入的月份有错误。');
            ncUnits.confirm({
                title: '提示',
                html: '你输入的月份有错误。请重新输入。'
            });
            $(this).val(month);
        }
        $('.calendar').datepicker('setDate', $('.calenYear').val() + '-' + $(this).val() + '-' + '10');
        $('.ui-state-active').css({ 'background': '#f9f9f9', 'color': '#222' });
    });
    /*设置日历输入框 结束*/
    /*弹窗 开始*/
    //新建计划
    //新增计划

    /* 个人资料设置 开始 */
    var popUpPerData;
    $('.conR .mine .introduce').on('click', function () {
        popUpPerData = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.perData' },
            closeBtn: false
        });
        fnPopUpHeight($('.perData'));
        fnSetDataAjax1();
    });
    $(".perData .closeWCom").click(function () {
        layer.close(popUpPerData);
    })
    // $(".perData .canCon").click(function(){
    // 	layer.close(popUpPerData);
    // })
    $('.conR .mine .portraitBox').click(function () {
        // var pn = $("#gotopagenum").val();//#gotopagenum是文本框的id属性  
        //location.href = "/UserHead/Index";//location.href实现客户端页面的跳转 
        loadViewToMain("/UserHead/Index");
    });
    //$('.modify').click(function () {
    //    // var pn = $("#gotopagenum").val();//#gotopagenum是文本框的id属性  
    //    location.href = "/UserHead/Index";//location.href实现客户端页面的跳转 
    //});
    //请求 个人资料设置
    var setData = {
        num: '30',			// 每页条数
        time: '30',		// 消息提示时间
        password: '2',		// 密码
    };
    // 个人资料获取数据

    function fnSetDataAjax1() {
        $.ajax({
            type: "post",
            url: "/User/GetPersonalSetting",
            dataType: "json",
            success: rsHandler(function (data) {
                //setData.password = data.password;
                $('#pageNum input:eq(0)').val(data.pageSize);
                $('#messTime input:eq(0)').val(data.refreshTime);
            })
        });
    }

    // 更新个人设定
    function fnSetDataAjax3() {
        var pagesize = $("#pageNum input:eq(0)").val();
        if (10 <= pagesize && pagesize <= 101) {
            var refreshTime = $("#messTime input:eq(0)").val();
            $.ajax({
                type: "post",
                url: "/User/UpPersonalSetting",
                dataType: "json",
                data: { pagesize: pagesize, refreshTime: refreshTime },
                success: rsHandler(function (data) {
                    //data.pageSize = setData.num;
                    //data.refreshTime = setData.time;
                    data.password = setData.password;
                    ncUnits.alert("更改个人设定成功");
                })
            });
        }
        else { ncUnits.alert("每页条数的范围必须在10~100"); }
    }

    // 更新密码
    function fnSetDataAjax2() {
        var oldpassword = $("#nowPass input:eq(0)").val();
        var password = $("#newPass input:eq(0)").val();
        $.ajax({
            type: "post",
            url: "/User/UpPwd",
            dataType: "json",
            data: { oldpwd: oldpassword, pwd: password },
            success: rsHandler(function (data) {
                //data.pageSize = setData.num;
                //data.refreshTime = setData.time;
                data.password = setData.password;
                ncUnits.alert("更改密码成功");
            })
        });
    }

    $('.perData .set .modify').click(function () {
        $(this).next('.cancel').show();
        // 编辑完成状态
        if ($(this).parents('.set').find('.list input:eq(0)').hasClass('inputHit')) {
            if ($(this).parents('.set').hasClass('password')) {
                // 判断当前密码是否为空
                if ($('#nowPass input:eq(0)').val() != '') {
                    // 判断新密码是否为空
                    if ($('#newPass input:eq(0)').val() != '') {
                        // 判断确认密码是否为空
                        if ($('#conPass input:eq(0)').val() != '') {

                            // 判断当前密码是否正确
                            //if (setData.password == $('#nowPass input:eq(0)').val()) {
                            // 判断新密码和确认密码是否一致
                            if ($('#newPass input:eq(0)').val() == $('#conPass input:eq(0)').val()) {
                                setData.password = $('#conPass input:eq(0)').val();
                                console.log(setData.password);
                                // 个人资料返回数据
                                $(this).next('.cancel').hide();
                                fnSetDataAjax2();
                                $('#nowPass input:eq(0)').val('********');
                                $('#newPass input:eq(0)').val('');
                                $('#conPass input:eq(0)').val('');
                                $(this).css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
                                $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
                            } else {
                                validate_reject('新密码和确认密码不一致', $('#conPass'));
                            }
                            //} else {
                            //    validate_reject('当前密码错误', $('#nowPass'));
                            //}

                        } else {
                            validate_reject('确认密码不能为空', $('#conPass'));
                        }
                    } else {
                        validate_reject('新密码不能为空', $('#newPass'));
                    }
                } else {
                    validate_reject('当前密码不能为空', $('#nowPass'));
                }
            } else {
                setData.num = $('#pageNum input:eq(0)').val();
                setData.time = $('#messTime input:eq(0)').val();
                // 个人资料返回数据
                fnSetDataAjax3();
                $(this).css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
                $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
            }
        }
            // 编辑状态
        else {
            if ($(this).parents('.set').hasClass('password')) {
                $(this).parents('.set').find('.list input:eq(0)').val('');
            }
            $(this).css({ 'background': 'url(../../Images/plan/chooseBlue.png) no-repeat' });
            $(this).parents('.set').find('.list input').addClass('inputHit').removeAttr('readonly');
        }
    });
    $('.perData .set .cancel').click(function () {
        $(this).hide();
        $('#nowPass input:eq(0)').val('********');
        $('#newPass input:eq(0)').val('');
        $('#conPass input:eq(0)').val('');
        $(this).prev().css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
        $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
    });
    /* 个人资料设置 结束 */

    $(".addPlan").click(function () {
        planCreater.addPlan();
    })

    /*添加日程 开始*/
    var dateOfNow = year + '-' + month + '-' + day + '';

    $('.addAgenda').on('click', function () {
        //alert($('.startTime').length);
        //$('.startTime').html('');
        //alert();
        //$('.endTime').html('');
        //$('.inputAddr').html('');
        //$('.inputMemoArea').html('');
        dateOfNow = date.year + '-' + date.month + '-' + date.day + ''; 
        lay_Agenda = $.layer({
            type: 1,
            shade: [0.5, '#000'],
            area: ['auto', 'auto'],
            title: false,
            border: [0],
            page: { dom: '.layer_Agenda' },
            move: ".drapdiv",
            closeBtn: false
        });
        calendarId = "";
        $('.startTime').val('');
        $('.endTime').val('');
        $('.inputAddr').val('');
        $('.inputMemoArea').val('');
        $('.showTime').html('');
        $('.showAddr').html('');
        $('.showMemo').html('');
        $('.showPartner').html('');
        $('#confirmAgenda').html('确定');
        $('.agendaInput').show();
        $('.timeList').show();
        $('.inputMemoArea').show();
        $('.addAgendaWidget').show();

        fnPopUpHeight($('.layer_Agenda'));

        $(".layer_Agenda .closeWCom").click(function () {
            layer.close(lay_Agenda);
        });
        $('.layer_Agenda .canCon span:eq(0)').click(function () {
            layer.close(lay_Agenda);
        });
    });
    //addAgenda的选择时间
    $('.startTime').click(function () {
        showTimeList('.startTimeList');
        // event.stopPropagation();
        stopPropagation();
    });
    $('.endTime').click(function () {
        showTimeList('.endTimeList');
        // event.stopPropagation();
        stopPropagation();
    });
    $(document).click(function () {
        $('.startTimeList').css('display', 'none');
        $('.endTimeList').css('display', 'none');
    });
    function stopPropagation(e) { //阻止冒泡 
        e = e || window.event;
        if (e.stopPropagation) { //W3C阻止冒泡方法  
            e.stopPropagation();
        } else {
            e.cancelBubble = true; //IE阻止冒泡方法  
        }
    }
    function showTimeList(parent) {//显示选择时间框
        $(parent + ' .hour').hover(function () {
            $(this).children().css('display', 'block');
        }, function () {
            $(this).children().css('display', 'none');
        });
        // alert($(parent+' .mins li').html());
        $(parent + ' .mins li').click(function () {
            
            var input = parent.substr(1, parent.length - 5);
            $('.' + input).val(dateOfNow + ' ' + $(this).html());
            $(parent).css('display', 'none');
        });
        
        dateOfNow = year + '-' + month + '-' + day + '';
        $(parent).css('display', 'block');
    }
    //addAgenda的标签设置
    var tab_v = [],
        partner_v = [];
    $("#addAgenda_label").click(function () {
        var $this = $(this);
        $this.hide();
        var $con = $("<div style='display: inline-block;position: relative;'></div>");
        var $input = $('<input type="text" style="border: 1px solid #e3e3e3;width: 100px;height: 22px"/>');
        var $em = $("<em style='border: 1px solid #00B83F;background-color: #00B83F;  position: absolute;right: 0px;'></em>").addClass("icon add-min-grey");

        function addtag() {

            var $span = $("<span></span>").addClass("tag tagsSpan");
            var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

            $span.hover(function () {
                $close.toggle();
            })

            var iv = $input.val();

            $span.append([iv, $close]);
            $this.before($span);

            tab_v.push(iv);

            $close.click(function () {
                $span.remove();
                removeValue(tab_v, iv);
                console.log(tab_v);
            });

            $con.remove();
            $this.show();
        }

        $input.keypress(function (e) {
            if (e.keyCode == 13 && $(this).val()) {
                addtag();
            }
        });
        $em.click(function () {
            if ($input.val()) {
                addtag();
            }
        });

        $con.append([$input, $em]).appendTo($this.parent());
    });
    
    //addAgenda参与者选择
    partners = [];//保存选择的参与者
    //var arrPartners = [];
    //fnSSP();
    //function fnSSP() {

    $("#addAgenda_partner").searchStaffPopup({
        url: "/BuildNewPlan/GetUserByUserId",
        defText: "常用联系人",
        hasImage: true,
        selectHandle: function (data) {
            var $span = $("<span></span>").addClass("tag");
            var $close = $("<span>X</span>").addClass("close").css({ display: "none" });

            $span.hover(function () {
                $close.toggle();
            })

            $span.append([data.name, $close]);

            $(this).parent().before($span);

            partner_v.push(data.id);
            partner_name.push(data.name);

            $close.click(function () {
                $span.remove();
                removeValue(partner_v, data.id);
                removeValue(partner_name, data.name);
            });
        }
    });
    //}

    //添加确认提交
    $('#confirmAgenda').click(function () {
        //alert();
        if ($(this).html() == '确定') {
            //alert('calendarId'+calendarId);
            var tags = [];
            $('.tagsSpan').each(function () {
                var text = $(this).text()
                var tag = text.substr(0, text.length - 1);
                tags.push(tag);
            });

            //alert(partners);
            //scalendarId = $(this).find('.calendarId').text();
            //partners.push(data.id);
            console.log(partner_v);
            //console.log(arrPartners);
            var agendaData = {
                'year': year,
                'month': month,
                'day': day,
                'startTime': $('.startTime').val(),
                'endTime': $('.endTime').val(),
                'place': $('.inputAddr').val(),
                'comment': $('.inputMemoArea').val(), 
                'partner': partner_v,
                'calendarId': calendarId
            };
            tags = [];
            console.log(partner_v);
            $.ajax({
                type: "post",
                url: '/User/AddCalendarById',
                dataType: "json",
                data: { agendaData: JSON.stringify(agendaData)}
            });
            //console.log(agendaData);
            layer.close(lay_Agenda);
        }

    });
    /*添加日程 结束*/
    /*弹窗 结束*/
    Date.prototype.format = function (format) {
        var o = {
            "M+": this.getMonth() + 1, //month 
            "d+": this.getDate(), //day 
            "h+": this.getHours(), //hour 
            "m+": this.getMinutes(), //minute 
            "s+": this.getSeconds(), //second 
            "q+": Math.floor((this.getMonth() + 3) / 3), //quarter 
            "S": this.getMilliseconds() //millisecond 
        }

        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    }
});

