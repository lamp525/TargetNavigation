//$(function () {
    $('.xubox_layer').css({ 'margin-left': '-481px', 'width': '962px' });
    //$('.commentBox').css({ 'height': parseInt($("#discussheight").val()) });
    $(".uploadBtn").off('click');
    $(".uploadBtn").click(function () {
        $('.xubox_layer').css({ 'margin-left': '-481px', 'width': '962px' });

        $(".uploadBox").show();
    });
    
    $('.finishTime input').keyup(function () {
        var eachTime = $('#time').val();
        var numberOfPlan = $('#num').val();
        $('.allTime').html(eachTime * numberOfPlan);
    });
    $('#slider').slider({
        orientation: "horizontal",
        range: "min",
        max: 101,
        min: 0,
        animate: false,
        value: 90,
        slide: function () {
            var degree = $(this).slider('value');
            $('.degree').html(degree + '%');
        },
        change: function () {
            var degree = $(this).slider('value');
            $('.degree').html(degree + '%');
        },
        stop: function () {
            var val = $(this).slider('value');
            if (val > 90) {
                $(this).slider('value', 90);
                var degree = $(this).slider('value');
                $('.degree').html(degree + '%');
                ncUnits.alert('只可以拖到90%哦。');
            }
        }
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
    
    var pId = $("#xxc_planId").val();
    var html = '';
    var i = 1;
    var j;
    var k = 0;
    var parttern = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    var completeTime = $('#time').val();
    var completeNum = $('#num').val();
    var file_url = "/Plan/FileData";
    var fileUpLoad_url = "/Plan/PlanUplpadMultipleFiles";
    var filedelete_url = "/Plan/DeletePlanFileById";
    if (page_flag && page_flag == "CalendarProcess") {    //计划日程化页面
        if (current_Info && current_Info.isLoopPlan == 1) {   //是循环计划
            $("#time").val(current_Info.unitTime).attr("readonly", "true");
            var numberOfPlan = $('#num').val();
            $('.allTime').html(current_Info.unitTime * numberOfPlan);
            file_url = "/Plan/LoopFileData";
            fileUpLoad_url = "/Plan/LoopPlanUplpadMultipleFiles";
            filedelete_url = "/Plan/DeleteLoopPlanFileById";
        }
    }
    getplanfiles();
    function getplanfiles() {
        $.ajax({
            url: file_url,
            data: { planId: pId },
            dataType: 'JSON',///----------------------------------------------------------------
            type: 'post',
            success: rsHandler(function (data) {
                for (var i = 0, len = data.length; i < len; i++) {
                    k++;
                    var $fileSpan = $('<span class="file" term="' + data[i].attachmentId + '">' + data[i].attachmentName + '<span>&nbsp&nbsp&nbsp&nbspx</span></span>');
                    $('.files ul').append($("<li></li>").append($fileSpan));
                    $fileSpan.hover(function () {
                        $(this).find('span').css('display', 'inline');
                    }, function () {
                        $(this).find('span').css('display', 'none');
                    });
                    $('span', $fileSpan).on('click', function () {
                        var $this = $(this);
                        var id = $(this).parent().attr('term');
                        ncUnits.confirm({
                            title: '提示',
                            html: '是否确认删除附件？',
                            yes: function (layerID) {
                                $.ajax({
                                    url: '/Plan/DeletePlanFileById',
                                    type: "POST",
                                    data: { id: id },
                                });
                                $this.parent().parent().remove();
                                k--;
                                layer.close(layerID);


                            }

                        });



                    });
                }

            })
        });
    }
    
    $('#fileupload').fileupload({
        url: fileUpLoad_url,
        dataType: 'text',
        formData:{pid:pId},
        //acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        //acceptfiletypes: ".jpg",
        //maxFileSize: 5000,
        add: function (e, data) {
            layer.closeTips();
            var isSubmit = true;
            $.each(data.files, function (index, value) {
                
                if (!parttern.test(data.files[index].name)) {
                    ncUnits.alert("你上传文件格式不对");
                    //ncUnits.confirm({
                    //    title: '提示',
                    //    html: '你上传文件格式不对。'
                    //});
                    isSubmit = false;
                    return;
                } else if (data.files[index].size > 52428800) {
                    ncUnits.alert("你上传文件过大(最大50M)");

                    //ncUnits.confirm({
                    //    title: '提示',
                    //    html: '你上传文件过大。'
                    //});
                    isSubmit = false;
                    return;
                } else {
                    
                    var $fileSpan = $('<span class="file" term="">' + data.files[index].name + '<span>&nbsp&nbsp&nbsp&nbspx</span></span>');
                    
                    $fileSpan.hover(function () {
                            $(this).find('span').css('display', 'inline');
                        }, function () {
                            $(this).find('span').css('display', 'none');
                        });

                    $('span', $fileSpan).on('click', function () {
                        var $this = $(this);
                            var id = $(this).parent().attr('term');
                            ncUnits.confirm({
                                title: '提示',
                                html: '是否确认删除附件？',
                                yes: function (layerID) {
                                    $.ajax({
                                        url: filedelete_url,
                                        type: "POST",
                                        data: { id: id },
                                    });
                                    $this.parent().parent().remove();
                                    k--;
                                    layer.close(layerID);
                                }
                            });





                            //$.ajax({
                            //    url: '/Plan/DeletePlanFileById',
                            //    type: "POST",
                            //    data: { id: id },
                            //});
                            //$(this).parent().parent().remove();
                            //k--;
                        });
                    
                    //html = '<li><div class="up_progress up_progress' + i + '"></div></li>';
                    $('.files ul').append($("<li></li>").append(['<div class="up_progress up_progress' + i + '"></div>',$fileSpan]));
                    j = i;
                    $('.up_progress' + i++ + '').show();
                }
                
            });
            //showDelete();
            if (isSubmit) {
                data.submit();
            }
            
        },
        complete: function (e, data) {
            $('.up_progress').css('display', 'none');
            $('.files li').addClass('uploaded');
        },
        error: function (e, data) {
            
            ncUnits.alert('error');
            
        },
        done: function (e, data) {
            
            if ($.parseJSON(data.result).status == 0) {
                ncUnits.alert("上传失败");
            } else {
               $('.file').eq(k++ ).attr('term', $.parseJSON(data._response.result).data[0].attachmentId);
                
            }
        },
        progress: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('.up_progress' + j + '').css('width', progress + '%');
        },
        always: function (e, data) {
        }
    });

    //点击提交
    $('#submitUploadFile').click(function () {
        layer.closeTips();
        if (k == 0) {
            validate_reject("必须上传附件", ".files");
        } else if ($('#time').val() == '') {
            validate_reject("完成时间不能为空", "#time");
        } else if ($('#num').val() == '') {
            validate_reject("完成件数不能为空", "#num");
        } else {
            if (page_flag && page_flag == "CalendarProcess") {    //计划日程化页面
                if (current_Info && current_Info.isLoopPlan == 1) {   //是循环计划
                    $.ajax({
                        url: '/Plan/ConfirmingLoopPlan',
                        type: 'post',
                        dataType: 'JSON',
                        data: { loopId: pId, quantity: $('#num').val() },
                        success: function (data) {
                            ncUnits.alert("提交成功!");
                            $("#detailAccessory").hide();
                            $('#detailAccessory .accessoryDiv').hide().find('ul').html('');
                            $("#detail_operateinfo").html('');
                            $("#plan_detail_modal").modal("hide");
                            loadingPlanList();
                        }
                    });
                } else {  //普通计划
                    SubmitThisPlan();
                }
            } else {
                SubmitThisPlan()
            }
        }
        
    });

    function SubmitThisPlan()
    {
        $.ajax({
            url: '/Plan/Confirming',
            type: 'post',
            dataType: 'JSON',
            data: { planId: pId, time: $('#time').val(), quantity: $('#num').val() },
            success: function (data) {
                if (data) {
                    ncUnits.alert(data.message);
;
                    if (page_flag && page_flag == "CalendarProcess") {    //计划日程化页面
                        $("#plan_detail_modal").modal("hide");
                        loadingPlanList();
                    } else if (page_flag == 'plan') {     //计划页面
                        layer.close(planDetail);
                        fnScreCon();
                    } else {
                        $('#submitUploadFile').trigger('addPlan')
                        $("#plan_detail_modal").modal("hide");
                        //首页计划提交成功刷新列表
                        
                    }
                    ncUnits.alert("计划提交成功");
                    $("#detailAccessory").hide();
                    $("#detail_operateinfo").html('')
                } else {
                    ncUnits.alert("计划提交失败");
                }

            }

        });
    }

    //点击取消
    $('#cancelUploadFile').click(function () {
        layer.closeTips();
        $('.xubox_layer').css({ 'margin-left': '-280px', 'width': '560px' });
        $("#detail_operateinfo").html('');
    });
//});