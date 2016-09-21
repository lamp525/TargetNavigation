//@ sourceURL=RosterIndex.js
/**
 * Created by DELL on 15-9-21.
 */
$(function () {
    console.log("xxxxxxxxxxxxxxx");
    ZTreeOrgLoad();
    var page_Now=1,orgId=0,userName=null;//默认第一次加载第一页
    var pageLength = null;
    var folderTree;
    //加载组织架构
    function ZTreeOrgLoad() {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null, organizationId: null },
            success: rsHandler(function (data) {
                console.log("ztree");
                 folderTree = $.fn.zTree.init($("#catalogue"), $.extend({
                    callback: {
                        onClick: function (e, id, node) {
                            $("#Roster_select").val("");
                            //点击事件
                            page_Now=1;
                            orgId = node.id;
                            userName = null;
                            RosterListLoad();
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
                        otherParam: ["organizationId", null],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    }
    var workStatusArray = ["离职", "在职", "退休"];
    var userTypeArray = ["实习", "试用","正式"];
    //搜索
    //$("#Roster_searchByName").selection({
    //    url: "/Shared/GetUserListByName",
    //    hasImage: true,
    //    selectHandle: function (data) {
    //        folderTree.cancelSelectedNode();
    //        $("#Roster_select").val(data.name);
    //        userName = data.name;
    //        orgId = 0;
    //        page_Now = 1;
    //        RosterListLoad();
    //    }
    //});
   
    $("#Roster_select_click").click(function () {
        folderTree.cancelSelectedNode();
        if ($("#Roster_select").val() == "") {
            userName = null;
            orgId = 0;
            page_Now = 1;
            RosterListLoad();
        } else {
            userName = $("#Roster_select").val();
            orgId = 0;
            page_Now = 1;
            RosterListLoad();
        }
    });

    //导出
    $("#information_Export").click(function () {
        if (userName == null) {
            window.location.href = "/Roster/ExportFile?orgId="+ orgId + "";
        }
        else { window.location.href = "/Roster/ExportFile?orgId=" + orgId + "&userName="+ userName + "";}
    });

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
    $transferTo.searchPopup({
        url: "/User/GetXXUser",
        hasImage: true,
        selectHandle: function (data) {
            $(this).val(data.name).attr("val", data.id);
        }
    });

    //$transferModal.on("shown.bs.modal", function () {
    //    $transferFrom.attr("val", transfer_userId);
    //    $transferFrom.val(transfer_name);
    //});

    $transferModal.on("hidden.bs.modal", function () {
        $transferFrom.val("").removeAttr("val");
        $transferTo.val("").removeAttr("val");
        $(":checkbox", this).prop("checked", false);
        transferPlans = [], transferObjects = [], transferFlows = [], transferDocuments = [];
    });
    $transferCheckAll.change(function () {
        $(".check-item-type :checkbox", $transferModal).prop("checked", this.checked).change();
    });

    $(".check-items :checkbox", $transferModal).change(function () {
        if (!this.checked) {
            $transferCheckAll.prop("checked", false);
            $(this).parents(".check-items").siblings(".check-item-type").find(":checkbox").prop("checked", false);
        }
    });
    $(".check-con", $transferModal).each(function () {
        var $head = $(".check-item-type", this),
            $items = $(".check-items", this),
            $checkboxs = $(":checkbox", $items);

        $(":checkbox", $head).change(function () {
            $checkboxs.prop("checked", this.checked).change();
        });
        $(".caret", $head).click(function () {
            $(this).toggleClass("caret-left");
            $items.slideToggle();
        });
    });

    $(".check-items :checkbox", $transferCheckPlan).change(function () {
        checkItem(this, transferPlans);
    });
    $(".check-items :checkbox", $transferCheckObject).change(function () {
        checkItem(this, transferObjects);
    });
    $(".check-items :checkbox", $transferCheckFlow).change(function () {
        checkItem(this, transferFlows);
    });
    $(".check-items :checkbox", $transferCheckDocument).change(function () {
        checkItem(this, transferDocuments);
    });

    $transferSubmit.click(function () {
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

    function checkItem(dom, arr) {
        if (dom.checked) {
            if (arr.indexOf(dom.value) == -1) {
                arr.push(dom.value);
            }
        } else {
            arr.splice(arr.indexOf(dom.value), 1);
        }
    }
    /* 权限转移 结束 */


    //默认加载全部信息
    RosterListLoad();
    var transfer_userId = 0, transfer_name = "";
    function RosterListLoad(){
        $("#RosterList").empty();
        $("#pageGo").val(page_Now);
           $.ajax({
               type: "post",
               url: "/Roster/GetRosterOrgList",
               dataType: "json",
               data: {currentPage:parseInt(page_Now),orgId:parseInt(orgId), userName: userName },
               success: rsHandler(function (data) {
                   if (data.length != 0) {
                      
                       $.each(data, function (i, v) {
                           var orgName = [], stationName = [];
                           $.each(v.station, function (flag, value) {
                               orgName.push(value.OrganizationName);
                               stationName.push(value.stationName);
                           });
                           pageLength = Math.ceil(v.pageCount / parseInt($("#lastPage").attr("term")));
                           console.log(pageLength);


                           if (page_Now == 1) {
                               if (pageLength != 1) {
                                   $("#previousPage").addClass("disabled");
                                   $("#firstPage").addClass("disabled");
                                   $("#nextPage").removeClass("disabled");
                                   $("#lastPage").removeClass("disabled");
                                   $("#nextPage").css("cursor", "pointer");
                                   $("#lastPage").css("cursor", "pointer");
                                   $("#previousPage").css("cursor", "default");
                                   $("#firstPage").css("cursor", "default");
                               } else {
                                   pageDisabled();
                               }
                           } else if (page_Now == pageLength) {
                                   $("#nextPage").addClass("disabled");
                                   $("#lastPage").addClass("disabled");
                                   $("#previousPage").removeClass("disabled");
                                   $("#firstPage").removeClass("disabled");
                                   $("#previousPage").css("cursor", "pointer");
                                   $("#firstPage").css("cursor", "pointer");
                                   $("#nextPage").css("cursor", "default");
                                   $("#lastPage").css("cursor", "default");
                              
                           } 

                           var m1 = "";
                           if (v.mobile1 != null) {
                               m1 = v.mobile1;
                           }
                           var m2 = "";
                           if (v.mobile2 != null) {
                               m2 = v.mobile2;
                           }
                           var $cell = $("<div class='cell'> </div>"),
                                 $span = $(" <span class='col-xs-2'>" + v.userNumber + "</span> <span class='col-xs-2'>" + v.userName + "</span>" +
                                 "<span class='col-xs-2 text-overflow' title='" + orgName.join(",") + "'>" + orgName.join(",") + "</span><span class='col-xs-2 text-overflow' title='" + orgName.join(",") + "'>" + stationName.join(",") + "</span>" +
                                "<span class='col-xs-2 text-overflow' title=" + m1 + "&nbsp&nbsp&nbsp" + m2 + ">" + m1 + "&nbsp&nbsp&nbsp&nbsp" + m2 + "</span>" + " <span class='col-xs-1'>" + workStatusArray[v.workStatus] + "</span>" + " <span class='col-xs-1'>" + userTypeArray[v.userType] + "</span>"),
                                 $operate = $("<div class='operate'>  </div>"),
                                 $ul = $("<ul></ul>"),
                                 $ChangeSpan = $("<li data-toggle='modal' data-target='#privilege_transfer_modal' term="+v.userId+" transfer_name="+v.userName+">权限转移</li>"),
                                 $modSpan = $("<li><a href='/Roster/RosterEdit?id=3&&RuserId=" + v.userId + "'>修改</a></li>"),
                                 //$StatusModSpan = $("<li>状态修改</li>"),
                                 $delSpan = $(" <li term=" + v.userId + ">删除</li>");
                           $ul.append($ChangeSpan, $modSpan, $delSpan);
                           $operate.append($ul);
                           $cell.append($span, $operate);
                           $cell.hover(function () {
                               $(".operate", this).css("display", "block");
                           }, function () {
                               $(".operate", this).css("display", "none");
                           });
                           $ChangeSpan.click(function () {
                               transfer_userId = $(this).attr("term");
                               transfer_name = $(this).attr("transfer_name");
                               $transferFrom.attr("val", transfer_userId);
                               $transferFrom.val(transfer_name);
                               
                           });

                           $delSpan.click(function () {
                               var userId = $(this).attr("term");
                               $this = $(this).closest(".cell");
                               ncUnits.confirm({
                                   title: '提示',
                                   html: '你确认要删除这个用户吗？',
                                   yes: function (layer_confirm) {
                                       layer.close(layer_confirm);
                                       $.ajax({
                                           type: "post",
                                           url: "/Roster/DeleteUser",
                                           dataType: "json",
                                           data: { userId: userId },
                                           success: rsHandler(function () {
                                               $this.remove();
                                               ncUnits.alert("删除成功!");
                                               if ($("#RosterList").html() == "")
                                               {
                                                   page_Now--;
                                                   RosterListLoad();
                                               }
                                            
                                              
                                           })
                                       })
                                   }

                               })

                           });
                           //$StatusModSpan.click(function () {
                           //    $("#effectTime").val(v.validDate.split("T")[0]);
                           //    $("#workStatusNow,#workStatus").text(workStatusArray[v.workStatus]);
                           //    $("#userName").text(v.userName);
                           //    $("#userName").attr("term", v.userId);
                           //    $("#workStatus").attr('term', v.workStatus);
                           //    $("#workStatusChange_modal").modal("show");
                           //});
                           $("#RosterList").append($cell);
                       })
                   }
                   else {
                       ncUnits.alert("没有此人信息");
                      
                       var $turn = $("<a>点击跳转至默认加载页</a>");
                       $turn.css({"cursor":"pointer","color":"#4DB747"});
                       $("#RosterList").append($turn);
                       $turn.click(function () {
                           $("#Roster_select").val("");
                           userName = null;
                           orgId = 0;
                           page_Now = 1;
                           RosterListLoad();
                       });
                     
                       pageDisabled();
                       
                   }

               })
           })
    }

    

    $("#firstPage").click(function () {
        if ($(this).hasClass("disabled") == false) {
            page_Now = 1;
            RosterListLoad();
            //$("#previousPage").addClass("disabled");
            //$("#nextPage").removeClass("disabled");
        }
    });
    //上一页
    $("#previousPage").click(function(){
        if($(this).hasClass("disabled")==false){
            page_Now--;
            RosterListLoad();
           
            $("#nextPage").removeClass("disabled");
            $("#lastPage").removeClass("disabled");
            $("#nextPage").css("cursor", "pointer");
            $("#lastPage").css("cursor", "pointer");
        }
    });
    //下一页
    $("#nextPage").click(function(){
        if($(this).hasClass("disabled")==false){
            page_Now++;
            RosterListLoad();
            $("#previousPage").removeClass("disabled");
            $("#firstPage").removeClass("disabled");
            $("#previousPage").css("cursor", "pointer");
            $("#firstPage").css("cursor", "pointer");
        }
    });
    $("#lastPage").click(function () {
        if ($(this).hasClass("disabled") == false) {
            page_Now = pageLength;
            RosterListLoad();
        }
       
        //$("#nextPage").addClass("disabled");
        //$("#previousPage").removeClass("disabled");
    });
    $("#pageGo").siblings("span").click(function(){
        var pagego=parseInt($("#pageGo").val());
        if (pagego > pageLength) {
            $("#pageGo").val(page_Now);
            ncUnits.alert("已超过最大页数")
        }
        else if(pagego==0){
            page_Now = 1;
            RosterListLoad();
        }
        else{
            page_Now=pagego;
            RosterListLoad();
            $("#previousPage").removeClass("disabled");
            $("#firstPage").removeClass("disabled");
            $("#nextPage").removeClass("disabled");
            $("#lastPage").removeClass("disabled");
            $("#nextPage").css("cursor", "pointer");
            $("#lastPage").css("cursor", "pointer");
            $("#previousPage").css("cursor", "pointer");
            $("#firstPage").css("cursor", "pointer");
        }

    });

    //鼠标禁用
    function pageDisabled()
    {
        $("#previousPage").addClass("disabled");
        $("#firstPage").addClass("disabled");
        $("#nextPage").addClass("disabled");
        $("#lastPage").addClass("disabled");
        $("#nextPage").css("cursor", "default");
        $("#lastPage").css("cursor", "default");
        $("#previousPage").css("cursor", "default");
        $("#firstPage").css("cursor", "default");
    }
/**---------------------------状态改变弹窗  开始-------------------------------------------**/
    

    $("#statusChange_submit").click(function(){
        argus={
            userId:0,            //用户ID
            workStatus:0,        //在职状态
            validDate:""     //有效时间
        }
        if ($("#userName").attr("term")=="") {
            ncUnits.alert("用户信息错误!");
            return;
        }
        else if ($("#effectTime").val()=="") {
            ncUnits.alert("未输入有效时间!");
            return;
        }
        else if ($("#workStatus").attr('term')=="") {
            ncUnits.alert("未选择状态!");
            return;
        }
        argus.userId= parseInt($("#userName").attr("term"));
        argus.validDate=$("#effectTime").val();
        argus.workStatus = parseInt($("#workStatus").attr('term'));
        $("#workStatusChange_modal").modal("hide");
        $.ajax({
            type:"post",
            url: "/Roster/UpdateWorkStatusById",
            dataType:"json",
            data: { userId: argus.userId, workStatus: argus.workStatus, validDate: argus.validDate },
            success: rsHandler(function () {
                RosterListLoad();//状态修改后重新加载
                ncUnits.alert("状态修改成功!");
            })
        })
    });

    $("#effectTime").siblings("span").click(function(){
      $(this).siblings("input").trigger("click");
   });
    //时间插件
    var start = {
        elem: '#effectTime',
        format: 'YYYY/MM/DD',
        max: '2099-06-16', //最大日期
        istime: false,
        istoday: false,
        choose: function(datas){
            end.min = datas; //开始日选好后，重置结束日的最小日期
            end.start = datas //将结束日的初始值设定为开始日
        },
        clear:function(){
            end.min = undefined;
        }
    };
    var end = {
        elem: '#effectTime',
        format: 'YYYY/MM/DD',
        max: '2099-06-16',
        istime: false,
        istoday: false,
        choose: function(datas){
            start.max = datas; //结束日选好后，重置开始日的最大日期
        },
        clear:function(){
            start.max = undefined;
        }
    };
    laydate(start);
    laydate(end);
    $("#workStatusChange_modal .dropdown ul li a").click(function(){
        dropDownEvent($(this));
    });
    function dropDownEvent(value){
        var x =  $(value).parents("ul").prev().find("span:eq(0)");
        x.text( $(value).text() );
        var term  = $(value).attr("term");
        x.attr("term",term);
    }

    /**---------------------------状态改变弹窗  结束-------------------------------------------**/
});