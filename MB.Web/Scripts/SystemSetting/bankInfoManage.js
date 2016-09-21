/**
 * Created by ZETA on 2015/11/12.
 */
$(function(){



    var $tableBody = $("#bankInfo tbody");

    loadBankInfo();

    /* 新增 开始*/
    var $addModal = $("#bankcard_add_modal"),
        $addSubmit = $("#bankcard_add_submit"),
        $addCancel = $("#bankcard_add_cancel"),
        $addNumber = $("#bankcard_add_number"),
        $addOpening = $("#bankcard_add_opening"),
        $addType = $("#bankcard_add_type");

    $addModal.on("hidden.bs.modal",function(){
        $addNumber.val("");
        $addOpening.val("");
        $addType.val("");
    });
    $addSubmit.click(function () {
        if ($addNumber.val().length<6) {
            ncUnits.alert("卡号必须六位");
            return;
        }
        if ($addNumber.val().length > 6) {
            ncUnits.alert("卡号只能六位");
            return;
        }
            $.ajax({
                type: "post",
                url: "/bankInfoManage/AddNewBankCard",
                dataType: "json",
                data: {
                    cardId: $addNumber.val(),
                    bankId: $addOpening.val()
                },
                success: rsHandler(function (data) {
                    if (data == 2) {
                        ncUnits.alert("卡号已存在");
                    } else {
                        ncUnits.alert("添加成功");
                        $addModal.modal("hide");
                        loadBankInfo();
                    }
                })
            }); 
    });
    /* 新增 结束 */

    /* 修改 开始 */
    var upDateCardId = "";
    var $changeModal = $("#bankcard_change_modal"),
        $changeSubmit = $("#bankcard_change_submit"),
        $changeCancel = $("#bankcard_change_cancel"),
        $changeNumber = $("#bankcard_change_number"),
        $changeOpening = $("#bankcard_change_opening"),
        $changeType = $("#bankcard_change_type");

    $changeSubmit.click(function () {
        if ($changeNumber.val().length > 6) {
            ncUnits.alert("卡号只能六位");
            return;
        }
        if ($changeNumber.val().length < 6) {
            ncUnits.alert("卡号必须六位");
            return;
        }
        $.ajax({
            type: "post",
            url: "/bankInfoManage/updateBankCard",
            dataType: "json",
            data: {
                cardId: $changeNumber.val(),
                bankId: $changeOpening.val(),
                updateCardId:upDateCardId
            },
            success: rsHandler(function (data) {
                if (data == 2) {
                    ncUnits.alert("卡号已存在");
                } else {
                    ncUnits.alert("修改成功！");
                    $changeModal.modal("hide");
                    loadBankInfo();
                }
            })
        });
    });

    /* 修改 结束 */

    /* 加载数据 开始*/
    function loadBankInfo(){
        var $loading = getLoadingPosition("#bankInfo");
        $tableBody.empty();
        $.ajax({
            type: "post",
            url: "/bankInfoManage/GetBankCardList",
            dataType: "json",
            success: rsHandler(function (data) {
                $.each(data,function(i,v){
                    var $tr = $("<tr><td>" + v.cardId + "</td><td>" + v.bankName + "</td> </tr>"),
                        $change = $("<a href='#bankcard_change_modal' data-toggle='modal' data-target='#bankcard_change_modal'>修改      </a>"),
                        $delete = $("<a href='#'>删除</a>");
                    $tr.append($("<td></td>").append([$change,$delete]));
                    $tableBody.append($tr); 
                    $change.click(function () {
                        $("#bankcard_change_number").val = v.cardId; 
                        $("#bankcard_change_opening option[value=" + v.bankId + "]").attr("selected", true);
                        $changeNumber.val(v.cardId);
                        $changeOpening.val(v.bankId);
                        upDateCardId = v.cardId;
                    });
                    $delete.click(function () {
                        ncUnits.confirm({
                            title: '提示',
                            html: '你确认要删除这条银行信息吗？',
                            yes: function (layer_confirm) {
                                var deleteId = v.cardId;
                                $.ajax({
                                    type: "post",
                                    url: "/bankInfoManage/deleteBankCard",
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
    $.ajax({
        type: "post",
        url: "/bankInfoManage/GetBank",
        dataType: "json",
        success: rsHandler(function (data) {
            $.each(data, function (i, v) {
                $("#bankcard_add_opening").append("<option value=" + v.bankId + ">" + v.bankName + "</option>");
                $("#bankcard_change_opening").append("<option value=" + v.bankId + ">" + v.bankName + "</option>");
            })

        })

    })
    /* 加载数据 结束*/
    $("#bankcard_add_number,#bankcard_change_number").bind("input", function () {
        var $value = $(this);
        var reg = /^\d+(\d{0,6})?/,
            vals = $value.val().match(reg); 
        $value.val(vals ? vals[0] : "");
        if ($(this).val().length > 6) {
            ncUnits.alert("长度不能超出6位");
            $value.val() = ""; 
        }
    });
})