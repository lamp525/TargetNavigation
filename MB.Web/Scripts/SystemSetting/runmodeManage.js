/**
 * Created by ZETA on 2015/11/12.
 */
$(function(){



    var $tableBody = $("#runmodeInfo tbody");

    loadRunmodeInfo();

    /* 新增 开始*/
    var $addModal = $("#runmode_add_modal"),
        $addSubmit = $("#runmode_add_submit"),
        $addCancel = $("#runmode_add_cancel"),
        $addRunmode = $("#runmode_add");

    $addModal.on("hidden.bs.modal",function(){
        $addRunmode.val("");
    });
 
    $addSubmit.click(function () {
        if ($addRunmode.val() == "") {
            ncUnits.alert("方式不能为空");
            return;
        }
        if ($changeRunmode.val().length > 10) {
            ncUnits.alert("执行方式长度不能超过10！");
            return;
        }
            $.ajax({
                type: "post",
                url: "/ExecutionMode/AddNewExecutionMode",
                dataType: "json",
                data: {
                    executionMode: $addRunmode.val()
                },
                success: rsHandler(function (data) {
                   
                    if (data == 2) {
                        ncUnits.alert("不可重复添加！");
                    }
                    else {
                        ncUnits.alert("添加成功！");
                        $addModal.modal("hide");
                        loadRunmodeInfo();
                    }
                })
            }); 
    });
    /* 新增 结束 */

    /* 修改 开始 */

    var $changeModal = $("#runmode_change_modal"),
        $changeSubmit = $("#runmode_change_submit"),
        $changeCancel = $("#runmode_change_cancel"),
        $changeRunmode = $("#runmode_change");

    var updateId=null;

    $changeSubmit.click(function () {
        if ($changeRunmode.val().length > 10) {
            ncUnits.alert("执行方式长度不能超过10！");
        } else {
            $.ajax({
                type: "post",
                url: "/ExecutionMode/UpdateExecutionMode",
                dataType: "json",
                data: {

                    model: $changeRunmode.val(),
                    id: updateId
                },
                success: rsHandler(function (data) {
                    if (data == 2) {
                        ncUnits.alert("不可重复添加相同执行方式！");
                    } else {
                        ncUnits.alert("修改成功！");
                        $changeModal.modal("hide");
                        loadRunmodeInfo();
                    }
                })
            });
        }
    });

    /* 修改 结束 */
    
    /* 加载数据 开始*/
    function loadRunmodeInfo(){
        var $loading = getLoadingPosition("#runmodeInfo");
        $tableBody.empty();
        $.ajax({
            type: "post",
            url: "/ExecutionMode/GetRunModel",
            dataType: "json",
            success: rsHandler(function (data) {
                $.each(data,function(i,v){
                    var $tr = $("<tr><td>" + v.executionMode + "</td></tr>"),
                        $change = $("<a href='#runmode_change_modal' data-toggle='modal' data-target='#runmode_change_modal'>修改     </a> "),
                        $delete = $("<a href='#'>删除</a>");
                    $tr.append($("<td></td>").append([$change,$delete]));
                    $tableBody.append($tr);

                    $change.click(function () {
                        console.log(v.executionId)
                        $changeRunmode.val(v.executionMode);
                        $("#runmode_change").val = v.executionMode;
                        updateId = v.executionId;
                        console.log(updateId);
                    });
                    $delete.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '你确认要删除执行方式吗？',
                            yes: function (layer_confirm) {
                                var deleteId = v.executionId;
                                $.ajax({
                                    type: "post",
                                    url: "/ExecutionMode/DeleteExecutionMode",
                                    dataType: "json",
                                    data: { id: deleteId },
                                    success: rsHandler(function (data) {
                                        $tr.remove();
                                        layer.close(layer_confirm);
                                        ncUnits.alert("删除成功");
                                    })
                                });
                            },
                            no: function (layer_confirm) {
                                layer.close(layer_confirm);
                            }
                        })
                    
                    })
                })
            }),
            complete:rcHandler(function(){
                $loading.remove();
            })
        });
    }

  
 
    /* 加载数据 结束*/
})