//@ sourceURL=widgetFormResolver.js
/**
 * Created by ZETA on 2015/6/18.
 */
var upLoadValue = [];
$(function(){
    var isPreview = false;

    $.fn.extend({
        widgetFormResolver: function (data, bool) {
            upLoadValue = [];
            var that = $(this).addClass("widget-form-resolver").empty();
            isPreview = bool;

            $.each(format(data), function (i, widget) {
                console.log(widget);
                if (widget.control == undefined) {
                    return;
                } 
                var item = createItem(widget.control.controlType);
                item.init(widget);
                that.append(item.render());
            });

            that.delegate("[lmid]", "blur", function () {
                var $that = $(this),
                    lmid = $that.attr("lmid"),
                    $account = $that.parents(".account-container"),
                    $accountTr = $that.parents(".account-container tbody tr");

                if ($accountTr.length > 0) {
                    var $up = $accountTr.find("[linkedLmid='" + lmid + "']");
                    if ($up.length > 0) {
                        $up.val(changeMoneyToChinese(rmoney($that.val())));
                    }
                }
                that.find("[linkedLmid='" + lmid + "']").not($("[linkedLmid='" + lmid + "']", $account)).val(changeMoneyToChinese(rmoney($that.val())));
            });


            //$(".account-container tbody tr").each(function () {
            //    var $that = $(this);
            //    $that.find("[linkedLmid]").each(function () {
            //        var $up = $(this),
            //            lmid = $up.attr("linkedLmid"),
            //            $low = $that.find("[lmid='" + lmid + "']");
            //        if (!$low.length) {
            //            $low = that.find("[lmid='" + lmid + "']");
            //        }
            //        $low.blur(function () {
            //            $up.val(changeMoneyToChinese(rmoney($(this).val())));
            //        })
            //    })
            //});
            //that.find("[linkedLmid]").not(".account-container [linkedLmid]").each(function () {
            //    var $up = $(this),
            //        $low = that.find("[lmid='" + $(this).attr("linkedLmid") + "']");
            //    $low.blur(function () {
            //        $up.val(changeMoneyToChinese(rmoney($(this).val())));
            //    })
            //});

            return {
                getJson: function () {
                    layer.closeTips();
                    var returnValueFlag = true;
                    var controlvalue = [];
                    that.children().each(function () {
                        if (!returnValueFlag) {
                            return false;
                        }
                        if ($(this).find('tr') && $(this).find('tr').length > 0) {
                            var parentId = $(this).attr('itemid');
                            var rowIndex = 0;
                            $(this).find("tbody tr").each(function () {
                                if (!returnValueFlag) {
                                    return false;
                                }
                                rowIndex++;
                                $(this).find("td:not('.account-operate')").each(function () {
                                    rowList = [];
                                    var value = $(this).find(".item-widget").data("item").getVal(true);
                                    if (value.constructor != window.Array && value == "-1") {
                                        returnValueFlag = false;
                                        return false;
                                    }
                                    if (value.constructor == window.Array) {
                                        rowList = value;
                                    } else {
                                        rowList.push(value);
                                    }
                                    var controlModel = {
                                        parentControl: parentId,
                                        controlId: $(this).find(".item-widget").attr('itemid'),
                                        rowNumberList: [{
                                            rowNumber: rowIndex,
                                            detailValue: rowList
                                        }]
                                    };
                                    if (controlvalue.length > 0) {
                                        var addflag = true;
                                        $.each(controlvalue, function (e, i) {
                                            if (controlModel.controlId == i.controlId) {
                                                i.rowNumberList.push(controlModel.rowNumberList[0]);
                                                addflag = false;
                                            }
                                        });
                                        if (addflag) {
                                            controlvalue.push(controlModel);
                                        }
                                    } else{
                                        controlvalue.push(controlModel);
                                    }
                                });
                            });
                        } else if($(this).find('.item-widget')&&$(this).find('.item-widget').length>0){
                            var parentId=$(this).attr('itemid');
                            $(this).find('.item-widget').each(function(){
                                var rowList=[];
                                var value=$(this).data("item").getVal(true);

                                if (value.constructor != window.Array && value == "-1") {
                                    returnValueFlag = false;
                                    return false;
                                }
                                if (value.constructor == window.Array) {
                                    rowList = value;
                                } else {
                                    rowList.push(value);
                                }
                                var controlModel = {
                                    parentControl: parentId,
                                    controlId: $(this).attr('itemid'),
                                    rowNumberList: [{
                                        rowNumber: 1,
                                        detailValue: rowList
                                    }]
                                };
                                controlvalue.push(controlModel);
                            });
                        } else{
                            var rowList=[];
                            var value=$(this).data("item").getVal(true);
                            if(value.constructor != window.Array&&value=="-1" ){
                                returnValueFlag=false;
                                return false;
                            }

                            if (value.constructor == window.Array) {

                                rowList = value;
                            } else {
                                rowList.push(value);
                            }
                            var controlModel = {
                                parentControl: null,
                                controlId: $(this).attr('itemid'),
                                rowNumberList: [{
                                    rowNumber: 1,
                                    detailValue: rowList
                                }]
                            };
                            controlvalue.push(controlModel);
                        }

                    })
                    return returnValueFlag ? controlvalue : false;
                },
                getOtherJson: function () {
                    var returnValueFlag = true;
                    var controlvalue = [];
                    var index = 0;
                    that.children().each(function () {

                        if ($(this).find('tr') && $(this).find('tr').length > 0) {
                            var parentId = $(this).attr('itemid');
                            var rowIndex = 0;
                            $(this).find("tbody tr").each(function () {
                                rowIndex++;
                                $(this).find("td:not('.account-operate')").each(function () {
                                    rowList = [];
                                    var value = $(this).find(".item-widget").data("item").getVal(true);
                                    if (value.constructor != window.Array && value == "-1") {
                                        returnValueFlag = false;
                                        return;
                                    }
                                    if (value.constructor == window.Array) {
                                        rowList = value;
                                    } else {
                                        rowList.push(value);
                                    }
                                    
                                    var description, controlType, saveName;
                                    //寻找子节点
                                    var controlNodeId = $(this).find(".item-widget").attr('itemid');
                                    $.each(data, function (i, v) {
                                        if (v.control.controlId == controlNodeId) {
                                           description = v.control.description;
                                            controlType = v.control.controlType;
                                            saveName = v.control.saveName;
                                        }
                                    });


                                    var controlModel = {
                                        parentControl: parentId,
                                        controlType: controlType,
                                        controlId: controlNodeId,
                                        description: description,
                                        saveName: saveName,
                                        rowNumberList: [{
                                            rowNumber: rowIndex,
                                            detailValue: rowList
                                        }]
                                    };
                              
                                    if (controlvalue.length > 0) {
                                        var addflag = true;
                                        $.each(controlvalue, function (e, i) {
                                            if (controlModel.controlId == i.controlId) {
                                                i.rowNumberList.push(controlModel.rowNumberList[0]);
                                                addflag = false;
                                            }
                                        });
                                        if (addflag) {
                                            controlvalue.push(controlModel);
                                        }
                                    } else {
                                        controlvalue.push(controlModel);
                                    }
                                });
                            });
                        } else if ($(this).find('.item-widget') && $(this).find('.item-widget').length > 0) {
                            var parentId = $(this).attr('itemid');
                            $(this).find('.item-widget').each(function () {
                                var rowList = [];
                                var value = $(this).data("item").getVal(true);

                                if (value.constructor != window.Array && value == "-1") {
                                    returnValueFlag = false;
                                    return;
                                }
                                if (value.constructor == window.Array) {
                                    rowList = value;
                                } else {
                                    rowList.push(value);
                                }

                                var description, controlType, saveName;
                                //寻找子节点
                                var controlNodeId = $(this).attr('itemid');
                                $.each(data, function (i, v) {
                                    if (v.control.controlId == controlNodeId) {
                                        description = v.control.description;
                                        controlType = v.control.controlType;
                                        saveName = v.control.saveName;
                                    }
                                });


                                var controlModel = {
                                    parentControl: parentId,
                                    controlType: controlType,
                                    controlId: controlNodeId,
                                    description: description,
                                    saveName: saveName,
                                    rowNumberList: [{
                                        rowNumber: 1,
                                        detailValue: rowList
                                    }]
                                };
                                controlvalue.push(controlModel);
                            });
                        } else {
                            var rowList = [];
                            var value = $(this).data("item").getVal(true);
                            if (value.constructor != window.Array && value == "-1") {
                                returnValueFlag = false;
                                return;
                            }

                            if (value.constructor == window.Array) {

                                rowList = value;
                            } else {
                                rowList.push(value);
                            }
                            var description = data[index].control.description;
                            var controlType = data[index].control.controlType;
                            var saveName = data[index].control.saveName;

                            var controlModel = {
                                parentControl: null,
                                controlType: controlType,
                                controlId: $(this).attr('itemid'),
                                description: description,
                                saveName: saveName,
                                rowNumberList: [{
                                    rowNumber: 1,
                                    detailValue: rowList
                                }]
                            };
                            controlvalue.push(controlModel);
                        }
                        index++;
            

                    })
                    return returnValueFlag ? controlvalue : false;
                }
            }
        }
    });

    var Item = function () { }
    Item.prototype._wrap = function () {
        return $("<div class='item-widget' itemid='" + this.controlInfo.control.controlId + "'></div>").data("item", this);
    }
    Item.prototype.init = function (opt) {
        this.controlInfo = opt;
    }
    Item.prototype.getTitle = function () {
        var $title = $("<div class='item-title'>" + (this.controlInfo.control.title || "") + "</div>");
        if (this.controlInfo.control.require == 1) {
            $title.addClass("sign-requ");
        }
        return $title;
    }
    Item.prototype.bindFormula = function () {
        console.log("abstract function");
    }
    Item.prototype.bindStatistics = function () {
        console.log("abstract function");
    }
    Item.prototype.getContent = function () {
        console.log("abstract function");
    }
    Item.prototype.render = function () {
        console.log("abstract function");
    }
    Item.prototype.getVal = function () {
        console.log("abstract function");
    }
    Item.prototype.setVal = function () {
        console.log("abstract function");
    }


    var Label = function () { }
    Label.prototype = new Item();
    Label.prototype.getContent = function () {
        var control = this.controlInfo.control;
        return $("<div class='item-label' style='background-color: " + control.color + "'>" + control.description + "</div>")
    }
    Label.prototype.render = function () {
        var $con = this._wrap();
        return $con.append(this.getContent());
    }
    Label.prototype.getVal = function () {
        return "";
    }

    var Input = function () {
        this.$valBox = undefined;
    }
    Input.prototype = new Item();
    Input.prototype._getValBox = function () {
        return $("<input type='text' class='form-control'>");
    }
    Input.prototype.getContent = function () {
        this.$valBox = this._getValBox();
        return $("<div class='item-content'></div>").append(["<div class='item-description'>" + this.controlInfo.control.description + "</div>", this.$valBox])
    }
    Input.prototype.render = function () {
        var $con = this._wrap(),
            control = this.controlInfo.control;
        $con.append([this.getTitle(), this.getContent()]);
        if (this.controlInfo.control.status && this.controlInfo.control.status == 1) {
            this.$valBox.attr('ReadOnly', 'true');
        }
        this.$valBox.addClass(getSizeClass(control.size));
        return $con;
    }
    Input.prototype.getVal = function () {
        var control = this.controlInfo.control;
        var value = this.$valBox.val();
        if (control.require && control.require == 1 && value == "") {
            validate_reject("输入不能为空", this.$valBox);
            return "-1";
        }
        return value;
    }

    var NumberInput = function () { }
    NumberInput.prototype = new Input();
    NumberInput.prototype.bindFormula = function (fn) {
        this.$valBox.blur(fn);
    }
    NumberInput.prototype.bindStatistics = function (fn) {
        this.$valBox.blur(fn);
    }
    NumberInput.prototype._getValBox = function () {
        return $("<input type='text' class='form-control' maxlength='18'>").on("input", function () {
            this.value = this.value.replace(/[^0-9\.]/g, '');
        })
    }
    NumberInput.prototype.getVal = function (flag) {
        var control = this.controlInfo.control;
        var value = this.$valBox.val();
        if (flag) {
            if (control.require && control.require == 1 && value.length <= 0) {
                validate_reject("输入不能为空", this.$valBox);
                return "-1";
            }
        }
        return value.length ? value : 0;
    }
    NumberInput.prototype.setVal = function (v) {
        this.$valBox.val(v).blur();
    }

    var TextArea = function () { }
    TextArea.prototype = new Input();
    TextArea.prototype._getValBox = function () {
        return $("<textarea class='form-control' rows='3' maxlength='500'>");
    }

    var DateInput = function () { }
    DateInput.prototype = new Input();
    DateInput.prototype._getValBox = function () {
        var status = this.controlInfo.control.status,
            $input = $("<input type='text' class='form-control form-time' placeholder='年-月-日'>").click(function () {
                if (status && status != 1) {
                    laydate({
                        elem: $input[0],
                        issure: false
                    });
                }
            })
        return $input;
    }

    var DateIntervalInput = function () { }
    DateIntervalInput.prototype = new Input();
    DateIntervalInput.prototype._getValBox = function () {
        var $start = $("<input type='text' class='form-control form-time' placeholder='年-月-日'>"),
            $end = $("<input type='text' class='form-control form-time' placeholder='年-月-日'>"),
            status = this.controlInfo.control.status;
        if (status && status != 1) {
            var start = {
                elem: $start[0],
                issure: false,
                choose: function (dates) {
                    end.start = dates;
                    end.min = dates;
                },
                clear: function () {
                    end.min = undefined;
                }
            },
                end = {
                    elem: $end[0],
                    issure: false,
                    choose: function (dates) {
                        start.max = dates;
                    },
                    clear: function () {
                        start.max = undefined;
                    }
                };
            $start.click(function () {
                laydate(start);
            });
            $end.click(function () {
                laydate(end);
            });
        } else {
            $start.attr("readonly", "true");
            $end.attr("readonly", "true");
        }
        return $("<div class='doubleDate'></div>").append([$start, "<span class='space-line'> - </span>", $end]);
    }
    DateIntervalInput.prototype.getVal = function () {
        var start = this.$valBox.find('.form-time:eq(0)').val();
        var end = this.$valBox.find('.form-time:eq(1)').val();
        var control = this.controlInfo.control;
        if (control.require && control.require == 1 && start == "") {
            validate_reject("开始日期不能为空", this.$valBox.find('.form-time:eq(0)'));
            return "-1";
        }
        else if (control.require && control.require == 1 && end == "") {
            validate_reject("结束日期不能为空", this.$valBox.find('.form-time:eq(1)'));
            return "-1";
        }
        return [start, end];
    }

    var DateTimeInput = function () { }
    DateTimeInput.prototype = new Input();
    DateTimeInput.prototype._getValBox = function () {
        var status = this.controlInfo.control.status,
            $input = $("<input type='text' class='form-control form-time' placeholder='年-月-日 时:分'>").click(function () {
                if (status && status != 1) {
                    laydate({
                        elem: $input[0],
                        format: 'YYYY-MM-DD hh:mm',
                        istime: true
                    });
                }
            });
        return $input;
    }
    DateTimeInput.prototype.getVal = function () {
        var control = this.controlInfo.control;
        var value = this.$valBox.val();
        if (control.require && control.require == 1 && value == "") {
            validate_reject("日期不能为空", this.$valBox);
            return "-1";
        }
        return value;
    }

    var DateTimeIntervalInput = function () { }
    DateTimeIntervalInput.prototype = new Input();
    DateTimeIntervalInput.prototype._getValBox = function () {
        var $start = $("<input type='text' class='form-control form-time' placeholder='年-月-日 时:分'>"),
            $end = $("<input type='text' class='form-control form-time' placeholder='年-月-日 时:分'>"),
            status = this.controlInfo.control.status;
        if (status && status != 1) {
            var start = {
                elem: $start[0],
                format: 'YYYY-MM-DD hh:mm',
                istime: true,
                choose: function (dates) {
                    end.start = dates;
                    end.min = dates;
                },
                clear: function () {
                    end.min = undefined;
                }
            },
                end = {
                    elem: $end[0],
                    format: 'YYYY-MM-DD hh:mm',
                    istime: true,
                    choose: function (dates) {
                        start.max = dates;
                    },
                    clear: function () {
                        start.max = undefined;
                    }
                };
            $start.click(function () {
                laydate(start);
            });
            $end.click(function () {
                laydate(end);
            });
        } else {
            $start.attr('readonly', 'true');
            $end.attr('readonly', 'true');
        }

        return $("<div class='doubleDate'></div>").append([$start, "<span class='space-line'> - </span>", $end]);
    }
    DateTimeIntervalInput.prototype.getVal = function () {
        var start = this.$valBox.find('.form-time:eq(0)').val();
        var end = this.$valBox.find('.form-time:eq(1)').val();
        var control = this.controlInfo.control;
        if (control.require && control.require == 1 && start == "") {
            validate_reject("开始日期不能为空", this.$valBox.find('.form-time:eq(0)'));
            return "-1";
        } else if (control.require && control.require == 1 && end == "") {
            validate_reject("结束日期不能为空", this.$valBox.find('.form-time:eq(1)'));
            return "-1";
        }
        return [start, end];
    }

    var HR = function () {
        this.$modal = undefined;
    }
    HR.prototype = new Input();
    HR.prototype.modal = function () {
        var that = this,
            control = this.controlInfo.control,
            mutliSelect = control.mutliSelect == "1";
        if (!$("#widgetForm_hr_modal").length && control.status && control.status != 1) {
            $("body").append("<div class='modal fade widget-form-modal' id='widgetForm_hr_modal' tabindex='-1' role='dialog' aria-hidden=true data-backdrop='false' data-keyboard='false'><div class='modal-dialog'><div class='modal-content'></div></div></div>");
            $("#widgetForm_hr_modal").on("hidden.bs.modal", function () {
                $(".modal-content", this).children().detach();
            })
        }

        var $modalCon = $("#widgetForm_hr_modal");

        if (!this.$modal) {
            this.$modal = $("<div></div>");

            var $selectAll = $("<input type='checkbox'>"),
                $hasChildren = $("<input type='checkbox'>"),
                $personList = $("<div class='personList over-y'></div>"),
                $chosen = $("<ul class='chosens'></ul>"),
                $chosenCount = $("<span class='badge badge-success'></span>"),
                $select = $("<input type='text' class='form-control' placeholder='选择人员' aria-describedby='basic-addon2'>"),
                $selection = $("<div class='input-group'></div>").append([$select, "<span class='input-group-btn'> <a class='btn btn-default' type='a'><span class='glyphicon glyphicon-search'></span></a> </span>"]),
                $treebox = $("<div class='ztree over-y' id='" + control.controlId + "'></div>"),
                $submit = $("<a class='btn btn-transparency'>确定</a>");

            this.$modal.append(["<div class='modal-header'><button type='button' class='close' data-dismiss='modal' aria-label='Close'><span class='glyphicon glyphicon-remove' aria-hidden='true'></span>" +
            "</button><h4 class='modal-title'>人力资源</h4></div>", $selection,
            $("<div class='fifty-fifty clearfix'></div>").append([$("<div class='chosebox fifty-fifty-vertical'></div>").append([$treebox, $("<div></div>")
                .append([$("<div class='operateBar clearfix'></div>").append([$("<label class='clickable pull-left'></label>")
                    .append([$hasChildren, "包含下级"]), mutliSelect ? $("<label class='clickable pull-right'></label>")
                    .append([$selectAll, "全部选择"]) : ""]), $personList])]), $("<div class='chosenbox'></div>").append([$("<div class='chosen-count'></div>")
                .append(["已选:", $chosenCount.html(0)]), $chosen])]),
            $("<div class='modal-footer fifty-fifty'></div>").append(["<a class='btn btn-transparency' data-dismiss='modal'>取消</a>", $submit])]);

            /*人力资源 开始*/
            var personOrgId;
            var personWithSub = false;
            $.ajax({
                type: "post",
                url: "/Shared/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    $.fn.zTree.init($treebox, $.extend({
                        callback: {
                            onClick: function (e, id, node) {
                                $selectAll.prop("checked", false);
                                var checked = $hasChildren.prop('checked');
                                personWithSub = checked == true ? 1 : 0;
                                personOrgId = node.id;
                                $("ul", $personList).remove();
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetUserList",
                                    dataType: "json",
                                    data: { withSub: personWithSub, organizationId: personOrgId },
                                    success: rsHandler(function (data) {
                                        if (data.length > 0) {
                                            $.each(data, function (i, v) {
                                                addPerson(v);
                                            });
                                            bindCheckPerson();
                                        }
                                    })
                                });
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

            //人员搜索
            $selection.selection({
                url: "/Shared/GetUserListByName",
                hasImage: true,
                selectHandle: function (data) {
                    $select.val(data.name);
                    var flag = true;
                    if ($("li", $chosen).length > 0) {
                        $("li", $chosen).each(function () {
                            if ($(this).attr('term') == data.id) {
                                flag = false;
                            }
                        });
                    }
                    if (flag == true) {
                        addChosenPerson(data.id, data.name);
                    }
                    if ($("ul", $personList).length > 0) {
                        $("ul", $personList).each(function () {
                            if ($(this).find("li:eq(1)").attr('term') == data.id.toString()) {
                                $(this).find("input").prop("checked", true);
                            }
                        });
                    }
                    $chosenCount.text($("li", $chosen).length);
                }
            });

            //包含下级
            $hasChildren.click(function () {
                $("ul", $personList).remove();
                var checked = $(this).prop('checked');
                personWithSub = checked == true ? 1 : 0;
                $.ajax({
                    type: "post",
                    url: "/Shared/GetUserList",
                    dataType: "json",
                    data: { withSub: personWithSub, organizationId: personOrgId },
                    success: rsHandler(function (data) {
                        if (data.length > 0) {
                            $.each(data, function (i, v) {
                                addPerson(v);
                            });
                            bindCheckPerson();
                        }
                    })
                });
            });
            //选择全部
            $selectAll.click(function () {
                var personAll = $(this).prop("checked");
                var showflag = true;
                if (personAll == true) {
                    $("ul", $personList).each(function () {
                        showflag = true;
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("li", $chosen).each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    $("ul input", $personList).prop('checked', true);

                    var length = $("input:checked", $personList).length
                    $chosenCount.text(length);
                    for (var i = 0; i < length; i++) {
                        showflag = true;
                        var personId = $("ul:eq(" + i + ")", $personList).find("li:eq(1)").attr('term');
                        var personName = $("ul:eq(" + i + ")", $personList).find("li:eq(1) span:eq(0)").text();
                        $("li", $chosen).each(function () {
                            if ($(this).attr('term') == personId) {
                                showflag = false;
                            }
                        });
                        if (showflag) {
                            var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $("#share_modal_chosen li").each(function () {
                                if ($(this).attr('term') == stationId) {
                                    $(this).remove();
                                }
                            });
                            $chosen.append($checked.append($close));
                            $close.click(function () {
                                var $thisId = $(this).parent().attr('term');
                                $(this).parent().remove();
                                $chosenCount.text($("li", $chosen).length);
                                $("ul", $personList).each(function () {
                                    if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                        $(this).find("input").prop("checked", false);
                                    }
                                });
                                $selectAll.prop("checked", false);
                            });
                        }
                    }
                    $chosenCount.text($("li", $chosen).length);
                }
                else {
                    $("ul", $personList).each(function () {
                        var term = $(this).find("li:eq(1)").attr("term");
                        $("li", $chosen).each(function () {
                            if ($(this).attr('term') == term) {
                                $(this).remove();
                            }
                        });
                    });
                    $("ul input", $personList).prop('checked', false);
                    var length = $("li", $chosen).length;
                    $chosenCount.text(length);
                }
            });

            //添加人员
            function addPerson(v) {
                var $personHtml = $("<ul class='list-inline'><li><input type='" + (mutliSelect ? "checkbox" : "radio") + "'></li><li term=" + v.userId + "><span>" + v.userName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                $personList.append($personHtml);
                $("li", $chosen).each(function () {
                    if ($(this).attr('term') == v.userId) {
                        $personHtml.find("input").prop('checked', true);
                    }
                });
            }

            //人员复选框点击事件
            function bindCheckPerson() {
                $("input", $personList).click(function () {
                    var checked = $(this).prop('checked');
                    var personId = $(this).parents(".list-inline").find("li:eq(1)").attr('term');
                    var personName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
                    var showflag = true;
                    if (checked == true) {
                        $("li", $chosen).each(function () {
                            if ($(this).attr("term") == personId) {
                                showflag = false;
                            }
                        });
                        if (showflag) {
                            addChosenPerson(personId, personName);
                        }
                    } else {
                        $personList.find("li").each(function () {
                            if ($(this).attr('term') && $(this).attr('term') == personId) {
                                $(this).parents(".list-inline").find("li:eq(0) input").prop('checked', false);
                            }
                        });
                        $("li", $chosen).each(function () {
                            if ($(this).attr('term') == personId) {
                                $(this).remove();
                                if ($("li", $chosen).length <= 0) {
                                    $selectAll.prop("checked", false);
                                }
                                $chosenCount.text($("li", $chosen).length);
                            }
                        });
                    }
                });
            }

            //添加被选人
            function addChosenPerson(personId, personName) {
                if (!mutliSelect) {
                    $(".close", $chosen).click();
                }
                $chosenCount.text($("li", $chosen).length + 1);
                var $checked = $("<li term=" + personId + "><span>" + personName + "</span></li>"),
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $chosen.append($checked.append($close));
                $close.click(function () {
                    var $thisId = $(this).parent().attr('term');
                    $(this).parent().remove();
                    $chosenCount.text($("li", $chosen).length);
                    $("ul", $personList).each(function () {
                        if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                            $(this).find("input").prop("checked", false);
                        }
                    });
                    $selectAll.prop("checked", false);
                });
            }
            /*人力资源 结束*/

            //确定
            $submit.click(function () {
                var data = [];
                $("li", $chosen).each(function () {
                    data.push({ name: $(">span", this).html(), id: $(this).attr("term") })
                });
                that.setVal(data);
                $modalCon.modal('hide');
            });
        }
        $(".modal-content", $modalCon).html(this.$modal);
        $modalCon.modal("show");
    }
    HR.prototype._getValBox = function () {
        var that = this,
            $group = $("<div class='input-group'></div>"),
            $input = $("<input type='text' class='form-control'>"),
            $btn = $("<a class='btn btn-default'><span class='fa fa-user'></span></a>");

        $input.focus(function () {
            $(this).blur();
            that.modal();
        });
        $btn.click(function () {
            that.modal();
        });

        return $group.append([$input, $("<span class='input-group-btn'></span>").append($btn)]);
    }
    HR.prototype.setVal = function (listData) {
        this.data = listData;
        var tmpdata = "";
        $.each(listData, function (i, data) {
            if (i == 0) {
                tmpdata += data.name;
            } else {
                tmpdata += ("," + data.name);
            }
        })
        this.$valBox.find("input").val(tmpdata);
    }
    HR.prototype.getVal = function () {
        var control = this.controlInfo.control;
        var value = this.$valBox.find("input").val();
        if (control.require && control.require == 1 && value == "") {
            validate_reject("人员选择不能为空", this.$valBox.find("input"));
            return "-1";
        }
        return value;
    }

    var Department = function () { }
    Department.prototype = new Input();
    Department.prototype.modal = function () {
        var that = this,
            control = this.controlInfo.control,
            mutliSelect = control.mutliSelect == "1";

        if (!$("#widgetForm_department_modal").length && control.status && control.status != 1) {
            $("body").append("<div class='modal fade widget-form-modal' id='widgetForm_department_modal' tabindex='-1' role='dialog' aria-hidden='true' data-backdrop='false' data-keyboard='false'> <div class='modal-dialog'> <div class='modal-content'></div></div></div>");
            $("#widgetForm_department_modal").on("hidden.bs.modal", function () {
                $(".modal-content", this).children().detach();
            })
        }

        var $modalCon = $("#widgetForm_department_modal");

        if (!this.$modal) {
            this.$modal = $("<div></div>");

            var $treebox = $("<div class='ztree' id='" + control.controlId + "'></div>"),
                $chosenCount = $("<span class='badge badge-success'></span>"),
                $chosen = $("<ul class='chosens'></ul>"),
                $submit = $("<a class='btn btn-transparency user-defined'>确定</a>"),
                $select = $("<input type='text' class='form-control' placeholder='选择组织架构' aria-describedby='basic-addon2'>"),
                $selection = $("<div class='input-group'></div>").append([$select, "<span class='input-group-btn'> <a class='btn btn-default' type='a'><span class='glyphicon glyphicon-search'></span></a> </span>"]);

            this.$modal.append(["<div class='modal-header'><button type='button' class='close' data-dismiss='modal' aria-label='Close'><span class='glyphicon glyphicon-remove' aria-hidden='true'></span>" +
            "</button><h4 class='modal-title'>组织架构</h4></div>", $selection,
            $("<div class='fifty-fifty clearfix'></div>").append([$("<div class='chosebox'></div>").append($treebox), $("<div class='chosenbox'></div>").append([$("<div class='chosen-count'></div>").append(["已选:", $chosenCount.html(0)]), $chosen])]),
            $("<div class='modal-footer fifty-fifty'></div>").append(["<a class='btn btn-transparency' data-dismiss='modal'>取消</a>", $submit])]);

            var department_modal;
            $.ajax({
                type: "post",
                url: "/Shared/GetOrganizationList",
                dataType: "json",
                data: { parent: null },
                success: rsHandler(function (data) {
                    department_modal = $.fn.zTree.init($treebox, $.extend({
                        callback: {
                            beforeClick: function (id, node) {
                                department_modal.checkNode(node, undefined, undefined, true);
                                return false;
                            },
                            onCheck: function (e, id, node) {

                                if (node.checked) {
                                    if (!mutliSelect) {
                                        $chosen.empty();
                                    }
                                    var $checked = $("<li term=" + node.id + "><span>" + node.name + "</span></li>"),
                                        $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                    $chosen.append($checked.append($close));
                                    $close.click(function () {
                                        department_modal.checkNode(node, undefined, undefined, true);
                                    });
                                    node.mappingLi = $checked;
                                } else {
                                    node.mappingLi.remove();
                                }
                                $chosenCount.html(department_modal.getCheckedNodes().length);
                            },
                            onNodeCreated: function (e, id, node) {
                                if ($("li", $chosen).length > 0) {
                                    $("li", $chosen).each(function () {
                                        var departId = $(this).attr('term');
                                        if (parseInt(departId) == node.id) {
                                            $(this).remove();
                                            department_modal.checkNode(node, undefined, undefined, true);
                                        }
                                    });
                                }
                            }
                        }
                    }, {
                        view: {
                            showIcon: false,
                            showLine: false
                        },
                        check: {
                            enable: true,
                            chkStyle: mutliSelect ? "checkbox" : "radio",
                            chkboxType: { "Y": "", "N": "" },
                            radioType: "all"
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
            //组织架构搜索
            $selection.selection({
                url: "/Shared/GetOrgListByName",
                hasImage: false,
                selectHandle: function (data) {
                    $select.val(data.name);
                    var n = department_modal.getNodeByParam("id", data.id);
                    if (n && !n.checked) {
                        department_modal.checkNode(n, undefined, undefined, true);
                    } else {
                        var flag = true;
                        if ($("li", $chosen).length > 0) {
                            $("li", $chosen).each(function () {
                                if ($(this).attr('term') == data.id) {
                                    flag = false;
                                }
                            });
                        }

                        if (flag == true) {
                            if (!mutliSelect) {
                                $(".close", $chosen).click();
                            }
                            var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                                $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                            $chosen.append($checked.append($close));
                            $close.click(function () {
                                var nodeId = $(this).parent().attr("term");
                                n = department_modal.getNodeByParam("id", parseInt(nodeId));
                                if (n) {
                                    department_modal.checkNode(n);
                                }
                                $(this).parent().remove();
                                $chosenCount.text($("li", $chosen).length);
                            });
                        }
                    }
                    $chosenCount.text($("li", $chosen).length);
                }
            });
            //确定组织架构选择
            $submit.click(function () {
                var data = [];
                $('li', $chosen).each(function () {
                    data.push({ name: $(">span", this).html(), id: $(this).attr("term") })
                });
                that.setVal(data);
                $modalCon.modal('hide');
            });
        }

        $(".modal-content", $modalCon).html(this.$modal);
        $modalCon.modal("show");
    }
    Department.prototype._getValBox = function () {
        var that = this,
            $group = $("<div class='input-group'></div>"),
            $input = $("<input type='text' class='form-control'>"),
            $btn = $("<a class='btn btn-default'><span class='fa fa-sitemap'></span></a>");

        $input.focus(function () {
            $(this).blur();
            that.modal();
        });
        $btn.click(function () {
            that.modal();
        });

        return $group.append([$input, $("<span class='input-group-btn'></span>").append($btn)]);
    }
    Department.prototype.setVal = function (listData) {
        this.data = listData;
        var tmpdata = "";
        $.each(listData, function (i, data) {
            if (i == 0) {
                tmpdata += data.name;
            } else {
                tmpdata += ("," + data.name);
            }
        })
        this.$valBox.find("input").val(tmpdata);
    }
    Department.prototype.getVal = function () {
        var control = this.controlInfo.control;
        var value = this.$valBox.find("input").val();
        if (control.require && control.require == 1 && value == "") {
            validate_reject("组织架构选择不能为空", this.$valBox.find("input"));
            return "-1";
        }
        return value;
    }

    var FileInput = function () { }
    FileInput.prototype = new Input();
    FileInput.prototype._getValBox = function () {
        var $group = $("<div class='input-group fileuploader'></div>"),
            $input = $("<input type='file' class='form-control'" + (this.controlInfo.control.mutliSelect == "1" ? " multiple" : "") + ">"),
            $faker = $("<input type='text' class='form-control' placeholder='请选择上传文件'>"),
            $btn = $("<a class='btn btn-default'><span class='fa fa-folder-open'></span></a>"),
            that = this,
            control = this.controlInfo.control;
        if (control.status && control.status == 1) {
            $group.attr("disabled", "true");
            $input.attr("disabled", "true");
        }

        var parttern = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i,
            i = 1, k = 0, j;
        $input.fileupload({
            url: isPreview ? '/Shared/ReturnSuccessJson' : '/FlowIndex/UploadDocument',
            dataType: 'text',
            add: function (e, data) {
                k = $('.file').length > 0 ? $('.file').length :0 ;
                layer.closeTips();
                var isSubmit = true;
                $.each(data.files, function (index, value) {
                    if (!parttern.test(value.name)) {
                        ncUnits.alert("你上传文件格式不对");
                        isSubmit = false;
                        return false;
                    } else if (value.size > 52428800) {
                        ncUnits.alert("你上传文件过大(最大50M)");
                        isSubmit = false;
                        return false;
                    } else {
                        var $fileSpan = $('<span class="file" style="display: inline-block;"  term="" title=' + value.name + '><span class="text-overflow" style="display: inline-block;max-width: 200px;vertical-align: top;">' + value.name + '</span></span>'),
                            $del = $("<a class='glyphicon glyphicon-remove clickable' style='visibility: hidden;margin-left: 5px'></a>");

                        $fileSpan.append($del).hover(function () {
                            $del.css('visibility', 'visible');
                        }, function () {
                            $del.css('visibility', 'hidden');
                        });
                        $del.click(function () {
                            var id = $(this).parent().attr('term');
                            if (upLoadValue && upLoadValue.length > 0) {
                                $.each(upLoadValue, function (e, xxc_file) {
                                    var fileName = xxc_file.split('*');
                                    if (fileName && fileName.length > 2 && fileName[1] == id) {
                                        upLoadValue.splice(e, 1);
                                    }
                                });
                            }
                            $(this).parent().parent().remove();
                            k--;
                        });
                        $fileSpan.find('span').click(function () {
                            var displayName = $(this).text();
                            var thisSaveName = $(this).parent().attr('term');
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
                        that.$files.append($("<li class='file-upload'></li>").append([$fileSpan, "<div class='up_progress up_progress" + i + "'></div>"]));
                        j = i++;
                    }
                });
                if (isSubmit) {
                    data.submit();
                }
            },
            complete: function () {
                $('.up_progress', that.$files).css('display', 'none');
                $('li', that.$files).addClass('uploaded');
            },
            error: function () {
                ncUnits.alert('出现错误');
            },
            done: function (e, data) {
                var fileName = $.parseJSON(data.result).displayName + "*" + $.parseJSON(data.result).saveName+"*9*"+control.controlId;
                upLoadValue.push(fileName);
                if ($.parseJSON(data.result).status == 0) {
                    ncUnits.alert("上传失败");
                } else {
                    $('.file', that.$files).eq(k++).attr('term', isPreview ? "" : $.parseJSON(data._response.result).saveName);
                    
                }
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('.up_progress' + j, that.$files).css('width', progress + '%');
            }
        });
        if (control.status && control.status == 1) {
            return $group;
        }
        else {
            return $group.append([$input, $faker, $("<span class='input-group-btn'></span>").append($btn)]);
        }
        
    }
    FileInput.prototype.getContent = function () {
        this.$valBox = this._getValBox();
        this.$files = $("<ul class='list-inline' style='line-height: 20px;margin-left: 0'></ul>");
        return $("<div class='item-content'></div>").append(["<div class='item-description'>" + this.controlInfo.control.description + "</div>",this.$valBox,this.$files])
    }
    FileInput.prototype.getVal = function () {
        var control = this.controlInfo.control;
        if (control.require && control.require == 1 && upLoadValue.length <=0) {
            validate_reject("请上传文件", this.$valBox);
            return "-1";
        }

        return upLoadValue;
    }

    var Radio = function () {
        this.$valBox = undefined;
    }
    Radio.prototype = new Item();
    Radio.prototype._getValBox = function () {
        var control = this.controlInfo.control,
            items = this.controlInfo.item,
            $con = $("<div></div>");

        $.each(items, function (o, item) {
            if (control.vertical == 1) {
                if (control.status && control.status == 1) {
                    $con.append("<div class='radio'><label><input type='radio' disabled='true' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label></div>");
                }
                else {
                    $con.append("<div class='radio'><label><input type='radio' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label></div>");
                }
            } else {
                if (control.status && control.status == 1) {
                    $con.append("<label class='radio-inline'><input type='radio' disabled='true' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label>");
                }
                else {
                    $con.append("<label class='radio-inline'><input type='radio' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label>");
                }
            }
        });
        return $con;
    }
    Radio.prototype.getContent = function () {
        this.$valBox = $("<div class='item-content'></div>").append(["<div class='item-description'>" + this.controlInfo.control.description + "</div>", this._getValBox()]);
        return this.$valBox;
    }
    Radio.prototype.render = function () {
        var $con = this._wrap(),
            control = this.controlInfo.control;
        $con.append([this.getTitle(), this.getContent()]);
        return $con;
    }

    Radio.prototype.getVal = function () {
        var value = [];
        var count = 0;
        var control = this.controlInfo.control;
        this.$valBox.find('input[type="radio"]').each(function () {
            if ($(this).prop('checked') == false) count++;
            value.push($(this).prop('checked').toString());
        });

        if (control.require && control.require == 1 && count >= this.$valBox.find('input[type="radio"]').length) {
            validate_reject("选项不能为空", this.$valBox.find('input[type="radio"]').eq(0));
            return "-1";
        }
        return value;
    };

    var Checkbox = function () {
        this.$valBox = undefined;
    }
    Checkbox.prototype = new Radio();
    Checkbox.prototype._getValBox = function () {
        var control = this.controlInfo.control,
            items = this.controlInfo.item,
            $con = $("<div></div>");

        $.each(items, function (o, item) {
            if (control.vertical == 1) {
                if (control.status && control.status == 1) {
                    $con.append("<div class='checkbox'><label><input type='checkbox' disabled='true' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label></div>");
                }
                else {
                    $con.append("<div class='checkbox'><label><input type='checkbox' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label></div>");
                }
            } else {
                if (control.status && control.status == 1) {
                    $con.append("<label class='checkbox-inline'><input type='checkbox' disabled='true' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label>");
                }
                else {
                    $con.append("<label class='checkbox-inline'><input type='checkbox' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "") + "><span>" + item.itemText + "</span></label>");
                }
            }
        });
        return $con;
    }
    Checkbox.prototype.getVal = function () {
        var value = [];
        var count = 0;
        var control = this.controlInfo.control;
        this.$valBox.find('input[type="checkbox"]').each(function () {
            if ($(this).prop('checked') == false) count++;
            value.push($(this).prop('checked').toString());
        });
        if (control.require && control.require == 1 && count >= this.$valBox.find('input[type="checkbox"]').length) {
            validate_reject("选项不能为空", this.$valBox.find('input[type="checkbox"]').eq(0));
            return "-1";
        }
        return value;
    }

    var LowMoney = function () {
        this.$valBox = undefined;
    }
    LowMoney.prototype = new Item();
    LowMoney.prototype.bindFormula = function (fn) {
        this.$valBox.blur(fn);
    }
    LowMoney.prototype.bindStatistics = function (fn) {
        this.$valBox.blur(fn);
    }
    LowMoney.prototype._getValBox = function () {
        return $("<input type='text' class='form-control' maxlength='15' lmid='" + this.controlInfo.control.controlId + "'>").on("input", function () {
            this.value = this.value.replace(/[^0-9\.]/g, '');
        }).on("blur", function () {
            this.value = fmoney(this.value);
        });
    }
    LowMoney.prototype.getContent = function () {
        this.$valBox = this._getValBox();
        return $("<div class='item-content'></div>").append(["<div class='item-description'>" + this.controlInfo.control.description + "</div>", this.$valBox])
    }
    LowMoney.prototype.render = function () {
        var $con = this._wrap(),
            control = this.controlInfo.control;
        $con.append([this.getTitle(), this.getContent()]);
        if (control.status && control.status == 1) {
            this.$valBox.attr('ReadOnly', 'true');
        }
        this.$valBox.addClass(getSizeClass(control.size));
        return $con;
    }
    LowMoney.prototype.getVal = function (flag) {
        var value = this.$valBox.val();
        var control = this.controlInfo.control;
        if (flag) {
            if (control.require && control.require == 1 && value.length <= 0) {
                validate_reject("输入不能为空", this.$valBox);
                return "-1";
            }
        }
        return value.length ? value.replace(/,/g, "") : 0;
    }
    LowMoney.prototype.setVal = function (v) {
        this.$valBox.val(fmoney(v)).blur();
    }

    var UpMoney = function () { }
    UpMoney.prototype = new LowMoney();
    UpMoney.prototype._getValBox = function () {
        return $("<input type='text' class='form-control' linkedLmid='" + this.controlInfo.control.linked + "' disabled>");
    }

    var Select = function () {
        this.$valBox = undefined;
    }
    Select.prototype = new Item();
    Select.prototype._getValBox = function () {
        var control = this.controlInfo.control,
            items = this.controlInfo.item,
            $con = this.$valBox = $("<select data-width='" + (control.size == 3 ? "100%" : control.size == 2 ? "80%" : "60%") + "' class='form-control " + getSizeClass(control.size) + "'></select>");
        if (control.status && control.status == 1) {
            $con.attr("disabled", "disabled");
        }
        $.each(items, function (o, item) {
            $con.append("<option>" + item.itemText + "</option>")
        });
        return $con;
    }
    Select.prototype.getContent = function () {
        return $("<div class='item-content'></div>").append(["<div class='item-description'>" + this.controlInfo.control.description + "</div>", this._getValBox()]);
    }
    Select.prototype.render = function () {
        var $con = this._wrap(),
            control = this.controlInfo.control;
        $con.append([this.getTitle(), this.getContent()]);

        //this.$valBox.selectpicker();
        return $con;
    }
    Select.prototype.getVal = function () {
        var control = this.controlInfo.control;
        var selectValue = this.$valBox.val();
        if (control.require && control.require == 1 && selectValue == "") {
            validate_reject("下拉框选择不能为空", this.$valBox.find(".form-control"));
            return "-1";
        }
        return selectValue;
    }

    var Line = function () { }
    Line.prototype = new Item();
    Line.prototype.getContent = function () {
        var control = this.controlInfo.control;
        return $("<div class='line " + getLineStyle(control.lineType) + "' style='border-color: " + control.color + "'></div>")
    }
    Line.prototype.render = function () {
        var $con = this._wrap();
        return $con.append(this.getContent());
    }
    Line.prototype.getVal = function () {
        return "";
    }

    var ItemLayout = function () { }
    ItemLayout.prototype._wrap = function () {
        return $("<div class='item-layout' itemid='" + this.controlInfo.control.controlId + "'></div>").data("item", this);
    }
    ItemLayout.prototype.init = function (opt) {
        this.controlInfo = opt;
    }
    ItemLayout.prototype.render = function () {
        console.log("abstract function");
    }

    var Col2 = function () { }
    Col2.prototype = new ItemLayout();
    Col2.prototype.colCount = 2;
    Col2.prototype.render = function () {
        var items = this.controlInfo.children || []
            , $con = this._wrap()
            , $table = $("<div class='dis-table'></div>");

        for (var i = 0; i < this.colCount; i++) {
            var item = items[i],
                $cell = $("<div class='dis-cell'></div>");
            $table.append($cell);
            if (item) {
                var widget = createItem(item.control.controlType);
                widget.init(item);
                $cell.append(widget.render());
            }
        }
        return $con.append($table);
    }
    Col2.prototype.getVal = function () {
        return "";
    }

    var Col3 = function () { }
    Col3.prototype = new Col2();
    Col3.prototype.colCount = 3;

    var Account = function () {
        this.$table = undefined;
    }
    Account.prototype = new ItemLayout();
    Account.prototype._formatFormula = function (listJson) {
        var map = {};
        $.each(listJson, function (i, v) {
            var id = v.formulaId,
                index = v.orderNum - 1;
            if (!map[id]) {
                map[id] = [];
            }
            map[id][index] = v;
        })
        return _.values(map);
    }
    Account.prototype.addRow = function () {
        var items = this.controlInfo.children
            , formulas = this.controlInfo.formula
            , $tbody = $("tbody", this.$table)
            , $tfoot = $("tfoot", this.$table)
            , AccountControl = this.controlInfo.control;
        var $tr = $("<tr></tr>");
        $tbody.append($tr);

        if (items && items.length) {
            $.each(items, function (i, item) {
                var control = item.control,
                    type = control.controlType,
                    id = control.controlId;
                var widget = createItem(type);
                widget.init(item);
                $tr.append($("<td></td>").append(widget.render()))

                if (control.columnStatistics && (type == 2 || type == 3)) {
                    widget.bindStatistics(function () {
                        var count = 0;
                        $("[itemid='" + id + "']", $tbody).each(function () {
                            count += parseFloat($(this).data("item").getVal());
                        });
                        $("[itemid='" + id + "']", $tfoot).html(count);
                    });
                }
            })
        }
        if (AccountControl.status&&AccountControl.status!=1) {
            $("<td class='account-operate'></td>").append($("<a href='javascript:void(0)'><span class='glyphicon glyphicon-trash'></span></a>").click(function () {
                $tr.remove();
            })).appendTo($tr);
        }
        if (formulas && formulas.length) {
            formulas = this._formatFormula(formulas);
            $.each(formulas, function (i, formulaItems) {
                var y, xs = [], formulaStr = "";
                $.each(formulaItems, function (i, formulaItem) {
                    if (i == 0) {
                        if (formulaItem.controlId) {
                            y = formulaItem.controlId;
                        }
                    } else {
                        if (formulaItem.operate != "=") {
                            if (formulaItem.controlId) {
                                xs.push(formulaItem.controlId);
                            }
                            formulaStr += (formulaItem.operate || formulaItem.displayText || formulaItem.controlId);
                        }
                    }
                })

                if (y) {
                    var itemY = $tr.find("[itemid='" + y + "']").data("item");
                    $.each(xs, function (i, x) {
                        var itemX = $tr.find("[itemid='" + x + "']").data("item");
                        itemX.bindFormula(function () {
                            var tmpFormulaStr = formulaStr;
                            $.each(xs, function (i, _x) {
                                var item_X = $tr.find("[itemid='" + _x + "']").data("item");
                                tmpFormulaStr = tmpFormulaStr.replace(_x, item_X.getVal());
                            })
                            itemY.setVal(eval(tmpFormulaStr));
                        });
                    });
                }
            })
        }
    }
    Account.prototype.render = function () {
        var that = this
            , control = this.controlInfo.control
            , items = this.controlInfo.children;
        this.$table = $("<table class='account-table'><thead><tr></tr></thead><tbody></tbody><tfoot><tr></tr></tfoot></table>");

        var $con = this._wrap()
            , $title = $("<div class='account-label'>" + control.title + "</div>")
            , $description = $("<div class='account-description'>" + control.description + "</div>")
            , $cellHead = $("thead tr", this.$table)
            , $cellBody = $("tbody", this.$table)
            , $cellFoot = $("tfoot tr", this.$table);

        $con.addClass("onlyCell");
        if (items && items.length) {
            $.each(items, function (i, item) {
                if (item) {
                    if (i > 0) {
                        $con.removeClass("onlyCell");
                    }
                    var control = item.control,
                        type = control.controlType;
                    $cellHead.append("<td><div class='item-title" + (control.require == "1" ? " sign-requ" : "") + "'>" + (control.title || "") + "</div></td>");
                    if(control.columnStatistics && (type == 2 || type == 3)){
                        $cellFoot.append("<td>合计: <span class='summation' itemid='" + control.controlId + "'></span></td>");
                    } else {
                        $cellFoot.append($("<td></td>"));
                    }
                }
            });
        }
        if (control.status && control.status!=1) {
            $cellHead.append($("<td class='account-operate'></td>").append($("<a href='javascript:void(0)'><span class='glyphicon glyphicon-plus'></span></a>").click(function () {
                that.addRow();
            })));
            $cellFoot.append($("<td></td>"));
        }
        
        

        for (var i = 0; i < control.defaultRowNum; i++) {
            this.addRow();
        }

        return $con.append([$title, $description, $("<div class='account-container'></div>").append(this.$table)]);
    }
    Account.prototype.getVal = function () {
        return "";
    }

    var createItem = (function () {
        var map = [];
        map[0] = Label;
        map[1] = Input;
        map[2] = NumberInput;
        map[3] = LowMoney;
        map[4] = Radio;
        map[5] = Checkbox;
        map[6] = Select;
        map[7] = HR;
        map[8] = Department;
        map[9] = FileInput;
        map[10] = Line;
        map[11] = TextArea;
        map[12] = DateInput;
        map[13] = DateIntervalInput;
        map[14] = DateTimeInput;
        map[15] = DateTimeIntervalInput;
        map[16] = Account;
        map[17] = UpMoney;
        map[18] = Col2;
        map[19] = Col3;
        return function (typeIndex) {
            return new map[typeIndex]();
        }
    })();



    function format(listJson) {
        var tree = {};
        $.each(listJson, function (i, v) {
            var p = v.control.parentControl;
            if (p && p != "main") {
                var node = tree[p],
                    index = v.control.columnIndex - 1;
                if (!node) {
                    tree[p] = node = {
                        children: []
                    }
                }
                if (!node.children) {
                    node.children = [];
                }
                node.children[index] = v;
            } else {
                var id = v.control.controlId;
                tree[id] = $.extend(tree[id], v);
            }
        });

        return _.values(tree);
    }

    function fmoney(s, n) {
        if (typeof s == "number" || s.length) {
            n = n > 0 && n <= 20 ? n : 2;
            s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
            var l = s.split(".")[0].split("").reverse(), r = s.split(".")[1];
            var t = "";
            for (var i = 0; i < l.length; i++) {
                t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
            }
            return t.split("").reverse().join("") + "." + r;
        } else {
            return "0.00"
        }
    }

    function rmoney(s) {
        return parseFloat(s.replace(/[^\d\.-]/g, ""));
    }

    function changeMoneyToChinese(money) {
        var cnNums = new Array("零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"); //汉字的数字
        var cnIntRadice = new Array("", "拾", "佰", "仟"); //基本单位
        var cnIntUnits = new Array("", "万", "亿", "兆"); //对应整数部分扩展单位
        var cnDecUnits = new Array("角", "分", "毫", "厘"); //对应小数部分单位
        var cnInteger = "整"; //整数金额时后面跟的字符
        var cnIntLast = "元"; //整型完以后的单位
        var maxNum = 999999999999999.9999; //最大处理的数字

        var IntegerNum; //金额整数部分
        var DecimalNum; //金额小数部分
        var ChineseStr = ""; //输出的中文金额字符串
        var parts; //分离金额后用的数组，预定义

        if (money == "") {
            return "";
        }

        money = parseFloat(money);
        if (money >= maxNum) {
            alert('超出最大处理数字');
            return "";
        }
        if (money == 0) {
            ChineseStr = cnNums[0] + cnIntLast + cnInteger;
            return ChineseStr;
        }
        money = money.toString(); //转换为字符串
        if (money.indexOf(".") == -1) {
            IntegerNum = money;
            DecimalNum = '';
        } else {
            parts = money.split(".");
            IntegerNum = parts[0];
            DecimalNum = parts[1].substr(0, 4);
        }
        if (parseInt(IntegerNum, 10) > 0) {//获取整型部分转换
            var zeroCount = 0;
            var IntLen = IntegerNum.length;
            for (var i = 0; i < IntLen; i++) {
                var n = IntegerNum.substr(i, 1),
                    p = IntLen - i - 1,
                    q = p / 4,
                    m = p % 4;
                if (n == "0") {
                    zeroCount++;
                } else {
                    if (zeroCount > 0) {
                        ChineseStr += cnNums[0];
                    }
                    zeroCount = 0; //归零
                    ChineseStr += cnNums[parseInt(n)] + cnIntRadice[m];
                }
                if (m == 0 && zeroCount < 4) {
                    ChineseStr += cnIntUnits[q];
                }
            }
            ChineseStr += cnIntLast;
            //整型部分处理完毕
        }
        if (DecimalNum != '') {//小数部分
            var decLen = DecimalNum.length;
            for (var i = 0; i < decLen; i++) {
                n = DecimalNum.substr(i, 1);
                if (n != '0') {
                    ChineseStr += cnNums[Number(n)] + cnDecUnits[i];
                }
            }
        }
        if (ChineseStr == '') {
            ChineseStr += cnNums[0] + cnIntLast + cnInteger;
        }
        else if (DecimalNum == '') {
            ChineseStr += cnInteger;
        }
        return ChineseStr;
    }

    var getSizeClass = (function () {
        var map = [];
        map[1] = "small";
        map[2] = "medium";
        map[3] = "large";
        return function (num) {
            return map[num];
        }
    })()

    var getLineStyle = (function () {
        var map = [];
        map[1] = "line-solid";
        map[2] = "line-dashed";
        map[3] = "line-double";
        return function (num) {
            return map[num];
        }
    })()
})