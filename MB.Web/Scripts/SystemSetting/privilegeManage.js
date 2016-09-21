//@ sourceURL=privilegeManage.js
/**
 * Created by ZETA on 2015/11/12.
 */
$(function(){


    var authId=0;
    var $tableBody = $("#privilegeInfo tbody");

    loadPrivilegeInfo();

    /* 新增 开始*/
    var $addModal = $("#privilege_add_modal"),
        $addSubmit = $("#privilege_add_submit"),
        $addCancel = $("#privilege_add_cancel"),
        $addName = $("#privilege_add_name"),
        $addCheckAll = $("#privilege_add_checkall"),
        $addCheckPlan = $("#privilege_add_checkplan"),
        $addCheckObject = $("#privilege_add_checkobject"),
        $addCheckFlow = $("#privilege_add_checkflow"),
        $addCheckDocument = $("#privilege_add_checkdocument");

    var addPlans = [],
        addObjects = [],
        addFlows = [],
        addDocuments = [];
    $addModal.on("hidden.bs.modal",function(){
        $addName.val("");
        $(":checkbox",this).prop("checked",false);
        addPlans = [], addObjects = [], addFlows = [], addDocuments = [];
    });
    $addCheckAll.change(function(){
        $(".check-item-type :checkbox",$addModal).prop("checked",this.checked).change();
    });

    $(".check-items :checkbox",$addModal).change(function(){
        if(!this.checked){
            $addCheckAll.prop("checked",false);
            $(this).parents(".check-items").siblings(".check-item-type").find(":checkbox").prop("checked",false);
        }
    });
    $(".check-con",$addModal).each(function(){
        var $head = $(".check-item-type",this),
            $items = $(".check-items",this),
            $checkboxs = $(":checkbox",$items);

        $(":checkbox",$head).change(function(){
            $checkboxs.prop("checked",this.checked).change();
        });
        $(".caret",$head).click(function(){
            $(this).toggleClass("caret-left");
            $items.slideToggle();
        });
    });

    $(".check-items :checkbox",$addCheckPlan).change(function(){
        checkItem(this,addPlans);
    });
    $(".check-items :checkbox",$addCheckObject).change(function(){
        checkItem(this,addObjects);
    });
    $(".check-items :checkbox",$addCheckFlow).change(function(){
        checkItem(this,addFlows);
    });
    $(".check-items :checkbox",$addCheckDocument).change(function(){
        checkItem(this,addDocuments);
    });

    $addSubmit.click(function () {
        if ($addName.val() == "")
        {
            ncUnits.alert("未填权限名");
            return false;
        }
        console.log($addName.val());
        $.ajax({
            type: "post",
            url: "/AuthManagement/SaveAuth",
            dataType: "json",
            data: {
                data: JSON.stringify({
                    authName: $addName.val(),
                    planData: addPlans,
                    objectiveData: addObjects,
                    FlowData: addFlows,
                    userDocumentData: addDocuments
                })
            },
            success: rsHandler(function (data) {
                $addModal.modal("hide");
                loadPrivilegeInfo();
            })
        });
    });

    /* 新增 结束 */

    /* 修改 开始 */
    var $changeModal = $("#privilege_change_modal"),
        $changeSubmit = $("#privilege_change_submit"),
        $changeCancel = $("#privilege_change_cancel"),
        $changeName = $("#privilege_change_name"),
        $changeCheckAll = $("#privilege_change_checkall"),
        $changeCheckPlan = $("#privilege_change_checkplan"),
        $changeCheckObject = $("#privilege_change_checkobject"),
        $changeCheckFlow = $("#privilege_change_checkflow"),
        $changeCheckDocument = $("#privilege_change_checkdocument");

    var changePlans = [],
        changeObjects = [],
        changeFlows = [],
        changeDocuments = [];
    $changeModal.on("hidden.bs.modal",function(){
        $changeName.val("");
        $(":checkbox",this).prop("checked",false);
        changePlans = [], changeObjects = [], changeFlows = [], changeDocuments = [];
    });
    $changeCheckAll.change(function(){
        $(".check-item-type :checkbox",$changeModal).prop("checked",this.checked).change();
    });

    $(".check-items :checkbox",$changeModal).change(function(){
        if(!this.checked){
            $changeCheckAll.prop("checked",false);
            $(this).parents(".check-items").siblings(".check-item-type").find(":checkbox").prop("checked",false);
        }
    });
    $(".check-con",$changeModal).each(function(){
        var $head = $(".check-item-type",this),
            $items = $(".check-items",this),
            $checkboxs = $(":checkbox",$items);

        $(":checkbox",$head).change(function(){
            $checkboxs.prop("checked",this.checked).change();
        });
        $(".caret",$head).click(function(){
            $(this).toggleClass("caret-left");
            $items.slideToggle();
        });
    });

    $(".check-items :checkbox",$changeCheckPlan).change(function(){
        checkItem(this,changePlans);
    });
    $(".check-items :checkbox",$changeCheckObject).change(function(){
        checkItem(this,changeObjects);
    });
    $(".check-items :checkbox",$changeCheckFlow).change(function(){
        checkItem(this,changeFlows);
    });
    $(".check-items :checkbox",$changeCheckDocument).change(function(){
        checkItem(this,changeDocuments);
    });

    $changeSubmit.click(function () {
        if ($changeName.val() == "")
        {
            ncUnits.alert("未填写权限名");
            return false;
        }
        $.ajax({
            type: "post",
            url: "/AuthManagement/SaveAuth",
            dataType: "json",
            data: {
                data: JSON.stringify({
                    authId:authId,
                    authName: $changeName.val(),
                    planData: changePlans,
                    objectiveData: changeObjects,
                    FlowData: changeFlows,
                    userDocumentData: changeDocuments
                })
            },
            success: rsHandler(function (data) {
                $changeModal.modal("hide");
                loadPrivilegeInfo();
            })
        });
    });
    /* 修改 结束 */

    /* 权限转移 开始 */

    var $transferModal = $("#privilege_transfer_modal"),
        $transferSubmit = $("#privilege_transfer_submit"),
        $transferCancel = $("#privilege_transfer_cancel"),
        $transferFrom = $("#privilege_transfer_from"),
        $transferTo = $("#privilege_transfer_to"),
        $transferCheckAll = $("#privilege_transfer_checkall"),
        $transferCheckPlan = $("#privilege_transfer_checkplan"),
        $transferCheckObject = $("#privilege_transfer_checkobject"),
        $transferCheckFlow = $("#privilege_transfer_checkflow"),
        $transferCheckDocument = $("#privilege_transfer_checkdocument"),
        $transferCheckboxs = $("#privilege_transfer_checkboxs"),
        $transferPerson = $("#privilege_transfer_person");

    var transferPlans = [],
        transferObjects = [],
        transferFlows = [],
        transferDocuments = [];
    $transferFrom.searchPopup({
        url: "/User/GetXXUser",
        hasImage: true,
        selectHandle: function(data){
            $(this).val(data.name).attr("val",data.id);
        }
    });
    $transferTo.searchPopup({
        url: "/User/GetXXUser",
        hasImage: true,
        selectHandle: function(data){
            $(this).val(data.name).attr("val",data.id);
        }
    });
    $transferModal.on("hidden.bs.modal",function(){
        $transferFrom.val("").removeAttr("val");
        $transferTo.val("").removeAttr("val");
        $(":checkbox",this).prop("checked",false);
        transferPlans = [], transferObjects = [], transferFlows = [], transferDocuments = [];
    });
    $transferCheckAll.change(function(){
        $(".check-item-type :checkbox",$transferModal).prop("checked",this.checked).change();
    });

    $(".check-items :checkbox",$transferModal).change(function(){
        if(!this.checked){
            $transferCheckAll.prop("checked",false);
            $(this).parents(".check-items").siblings(".check-item-type").find(":checkbox").prop("checked",false);
        }
    });
    $(".check-con",$transferModal).each(function(){
        var $head = $(".check-item-type",this),
            $items = $(".check-items",this),
            $checkboxs = $(":checkbox",$items);

        $(":checkbox",$head).change(function(){
            $checkboxs.prop("checked",this.checked).change();
        });
        $(".caret",$head).click(function(){
            $(this).toggleClass("caret-left");
            $items.slideToggle();
        });
    });

    $(".check-items :checkbox",$transferCheckPlan).change(function(){
        checkItem(this,transferPlans);
    });
    $(".check-items :checkbox",$transferCheckObject).change(function(){
        checkItem(this,transferObjects);
    });
    $(".check-items :checkbox",$transferCheckFlow).change(function(){
        checkItem(this,transferFlows);
    });
    $(".check-items :checkbox",$transferCheckDocument).change(function(){
        checkItem(this,transferDocuments);
    });

    $transferSubmit.click(function () {
        if (typeof ($transferFrom.attr("val")) == "undefined")
        {
            ncUnits.alert("输出者不能为空");
            return false;
        }
        if (typeof ($transferTo.attr("val")) == "undefined") {
            ncUnits.alert("接受者不能为空");
            return false;
        }
        $.ajax({
            type: "post",
            url: "/AuthManagement/AuthShift",
            dataType: "json",
            data: {
                data: JSON.stringify({
                    turnUserId: $transferFrom.attr("val"),
                    acceptUserId: $transferTo.attr("val"),
                    planData: transferPlans,
                    objectiveData: transferObjects,
                    FlowData: transferFlows,
                    userDocumentData: transferDocuments
                })
            },
            success: rsHandler(function (data) {
                ncUnits.alert("权限转移成功");
                $transferModal.modal("hide");
            })
        });
    });

    //console.log("$transferFrom:",$transferFrom);
    //console.log("$transferTo:",$transferTo);
    //$transferFrom.click(function(){
    //    $transferCheckboxs.hide();
    //    $transferPerson.show();
    //});

    /* 权限转移 结束 */

    /* 加载数据 开始*/
    function loadPrivilegeInfo(){
        var $loading = getLoadingPosition("#privilegeInfo");
        $tableBody.empty();
        $.ajax({
            type: "post",
            url: "/AuthManagement/GetAuthList",
            dataType: "json",
            success: rsHandler(function (data) {
                $.each(data, function (i, v) {
                   var plans=v.planData,
                    objectives=v.objectiveData,
                    flows = v.FlowData,
                    documents = v.userDocumentData;
                    var $tr = $("<tr><td>" + v.authName + "</td><td>" + v.auth + "</td></tr>"),
                        $change = $("<a href='#privilege_change_modal' data-toggle='modal' data-target='#privilege_change_modal' style='margin-right:10px;'>修改</a>"),
                        $delete = $("<a href='#' term='"+v.authId+"'>删除</a>");
                    $tr.append($("<td></td>").append([$change,$delete]));
                    $tableBody.append($tr);

                    $change.click(function(){
                        authId=v.authId;
                        $changeName.val(v.authName);
                        //$.ajax({
                        //    type: "post",
                        //    url: "../../test/data/manage/privilegeInfo.json",
                        //    dataType: "json",
                        //    success: rsHandler(function (data) {
                                //var plans = data.planData,
                                //    objectives = data.objectiveData,
                                //    flows = data.flowData,
                                //    documents = data.userDocumentData;

                                if(plans && plans.length){
                                    $.each(plans,function(i,v){
                                        $changeCheckPlan.find(":checkbox[value=" + v + "]").prop("checked",true).change();
                                    })
                                }
                                if(objectives && objectives.length){
                                    $.each(objectives,function(i,v){
                                        $changeCheckObject.find(":checkbox[value=" + v + "]").prop("checked",true).change();
                                    })
                                }
                                if(flows && flows.length){
                                    $.each(flows,function(i,v){
                                        $changeCheckFlow.find(":checkbox[value=" + v + "]").prop("checked",true).change();
                                    })
                                }
                                if(documents && documents.length){
                                    $.each(documents,function(i,v){
                                        $changeCheckDocument.find(":checkbox[value=" + v + "]").prop("checked",true).change();
                                    })
                                }
                            //})
                        //})
                    });
                    $delete.click(function () {
                        var id=$(this).attr("term");
                        ncUnits.confirm({
                            title: '提示',
                            html: '确认要删除？',
                            yes: function (layer_confirm) {
                                layer.close(layer_confirm);
                                $.ajax({
                                    type: "post",
                                    url: "/AuthManagement/DeleteAuth",
                                    data: { id: id},
                                    dataType: "json",
                                    success: rsHandler(function (data) {
                                        $tr.remove();
                                    })
                                });
                            }
                        });
                       
                    })
                })
            }),
            complete:rcHandler(function(){
                $loading.remove();
            })
        });
    }
    /* 加载数据 结束*/

    function checkItem(dom,arr){
        if(dom.checked){
            if(arr.indexOf(dom.value) == -1){
                arr.push(dom.value);
            }
        }else{
            arr.splice(arr.indexOf(dom.value),1);
        }
    }
})