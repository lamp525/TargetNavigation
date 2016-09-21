/**
 * Created by ZETA on 2015/6/18.
 */
$(function(){
    $.fn.extend({
        widgetFormEditor: function(o){

            var opt = $.extend({
                dragItems: undefined,
                editor: undefined
            },o);
            var $widget_container = $(this);
            var $dragItems = opt.dragItems;
            var $editor = opt.editor;
            var activedWidget = undefined;
            var receiveItem = 0;
            var colIndex = 0;
            var deleteControl = [];
            var deleteControlItem = [];

            /* 控件原型 开始 */
            var Widget = function(){
                this.id = undefined;
                this.name = undefined;
                this.$editorContainer = $editor;
                this.$view = undefined;
                this.$editor = undefined;
                this.$viewContent = undefined;
                this.$label = undefined;

                this.type = undefined;
                this.parent = undefined;
                this.controlInfo = undefined;
                this.actived = false;

                this.$link = undefined;

                this.formulaCount = 0;
            }
            Widget.prototype._wrap = function(){
                var that = this;
                var $tableRow = $("<div class='table-row'></div>"),
                    $operateBar = $("<div class='field-operate'></div>"),
                    $delete = $("<a href='javascript:void(0)' class='glyphicon glyphicon-remove'></a>"),
                    $mask = $("<div class='widget-mask move-handle'></div>");
                this.$viewContent = $("<div class='field-divide'></div>");
                this.$view.append([$tableRow,$mask]);
                $tableRow.append([this.$viewContent,$operateBar]);
                $operateBar.append($delete);

                $delete.click(function(){
                    that._remove();
                });

                this.$view.mouseover(function(){
                    $operateBar.css({
                        visibility : "visible"
                    });
                });
                this.$view.mouseout(function(){
                    $operateBar.css({
                        visibility : ""
                    });
                });
            }
            Widget.prototype.changeLabel = function(v){
                v = toTXT(v);
                $(".label-text",this.$label).html(v);
                if(v.length){
                    this.$label.removeClass("empty");
                }else{
                    this.$label.addClass("empty");
                }
                this.controlInfo.control.title = v;

                if(this.$link){
                    $(":text",this.$link).val(v);
                }
            }
            Widget.prototype._remove = function(quick){
                var that = this,
                    success = true;
                if(this.formulaCount){
                    ncUnits.alert("不可删除,有公式使用了该控件!");
                    success = false
                }else if(this.controlInfo.control.used){
                    ncUnits.alert("不可删除,该控件已被使用!");
                    success = false
                }else{
                    if(quick){
                        remove();
                    }else{
                        ncUnits.confirm({
                            title:"删除",
                            html:"确认删除该控件?",
                            yes:function(layid){
                                remove();
                                layer.close(layid);
                            }
                        })
                    }

                    function remove(){
                        var control = that.controlInfo.control;
                        that.$view.remove();
                        that.$editor.remove();
                        if(control.loaded){
                            deleteControl.push(control.controlId);
                        }
                        var $link = that.$link;
                        if($link){
                            $link.find(":text").val("");
                            $link.find(":checkbox").attr("disabled",true).prop("checked",false);
                            that.parent.formatCols();
                        }
                        that.afterRemove();
                    }
                }
                return success;
            }
            Widget.prototype.activate = function(){
                if(!this.actived){
                    if(activedWidget){
                        activedWidget.deactivate();
                    }
                    this.$view.addClass("active");
                    this.$editorContainer.html(this.$editor);
                    activedWidget = this;
                    this.actived = true;
                }

                if(this.afterActivate){
                    this.afterActivate();
                }
            }
            Widget.prototype.deactivate = function(){
                this.$view.removeClass("active");
                this.$editor.detach();
                this.actived = false;
            }
            Widget.prototype._bindEvent = function(){
                var that = this;
                this.$view.click(function(e){
                    e.stopPropagation();
                    that.activate();
                });
            }
            Widget.prototype.getData = function(){
                var temp = {};
                $.extend(temp,this.controlInfo);
                delete temp.children;
                return temp;
            }
            Widget.prototype.init = function(){
                console.log("abstract function");
            }
            Widget.prototype.render = function(){
                console.log("abstract function");
            }
            Widget.prototype.afterActivate = function(){
                console.log("abstract function");
            }
            Widget.prototype.afterRemove = function(){
                console.log("abstract function");
            }
            /* 控件原型 结束 */

            var Label = function(){}
            Label.prototype = new Widget();
            Label.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            description:"空白的标签",
                            color:"#fff",
                            controlType:0
                        }
                    }
                    $.extend(this.controlInfo,def)
                }
            }
            Label.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control;

                var $label = $("<div class='widget-label' style='background-color: " + control.color + "'>" + control.description + "</div>");
                this.$viewContent.append($label);

                this.$editor = $("<div class='widget-editor'></div>");
                var $textarea = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>");
                var $colorpicker = $("<div></div>");
                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>描述</label>",$textarea])
                    ,$("<div class='form-group'></div>").append(["<label>背景色</label>",$colorpicker])
                ]);

                $textarea.on("input",function(){
                    var v = toTXT($(this).val());
                    $label.html(v);
                    control.description = v;
                });

                $colorpicker.colorpicker({
                    color:control.color,
                    history:false,
                    strings:"主题色,标准色,网格式,主题式"
                }).on("change.color", function(e, c){
                    $label.css('background-color', c);
                    control.color = c;
                });

                this._bindEvent();
            }

            var Input = function(){}
            Input.prototype = new Widget();
            Input.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control: {
                            title: "输入框",
                            description: "",
                            controlType: 1,
                            size: 2,
                            require: 0
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Input.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control;

                var $inputCon = $("<div class='widget-input'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $input = $("<input type='text' class='form-control'>"),
                    $textarea = $("<textarea class='form-control' rows='3' maxlength='500'></textarea>"),
                    $number = $("<input type='number' class='form-control' maxlength='18'>"),
                    $cur = $input;
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>");
                this.$viewContent.append($inputCon);
                $inputCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$cur])]));

                this.$editor = $("<div class='widget-editor'></div>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),
                    $typeEditor = $("<div class='radio'><label><input type='radio' name='inputType' value='1'> 单行文本框 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='inputType' value='11'> 多行文本框 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='inputType' value='2'> 数字 </label></div>"),
                    $sizeEditor = $("<label class='radio-inline'><input type='radio' name='inputSize' value='3'> 大尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='inputSize' value='2'> 标准尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='inputSize' value='1'> 小尺寸 </label>"),
                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='inputRequired' value='1'> 是 </label>" +
                        "<label class='radio-inline'><input type='radio' name='inputRequired' value='0'> 否 </label>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>组件类型</label>",$typeEditor])
                    ,$("<div class='form-group'></div>").append(["<label>控件大小</label>",$("<div></div>").append($sizeEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                ]);

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                })

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $("input",$typeEditor).change(function(){
                    var val = $(this).val(),
                        $link = that.$link;
                    if(that.formulaCount){
                        ncUnits.alert("不可修改:有公式使用了该控件");
                        $("input[value='2']",$typeEditor).prop("checked",true);
                    }else{
                        switch(val){
                            case "1":
                                $cur.replaceWith($input);
                                $cur = $input;
                                if($link){
                                    $link.find(":checkbox").attr("disabled",true).prop("checked",false);
                                }
                                control.columnStatistics = 0;
                                break;
                            case "11":
                                $cur.replaceWith($textarea);
                                $cur = $textarea;
                                if($link){
                                    $link.find(":checkbox").attr("disabled",true).prop("checked",false);
                                }
                                control.columnStatistics = 0;
                                break;
                            case "2":
                                $cur.replaceWith($number);
                                $cur = $number;
                                if($link){
                                    $link.find(":checkbox").attr("disabled",false);
                                }
                                break;
                            default:
                        }
                        control.controlType = val;
                    }
                });

                $("input",$sizeEditor).change(function(){
                    var val = $(this).val(),
                        tmp = $input.add($textarea).add($number);
                    tmp.removeClass("small medium large");
                    switch(val){
                        case "3":
                            tmp.addClass("large");
                            break;
                        case "2":
                            tmp.addClass("medium");
                            break;
                        case "1":
                            tmp.addClass("small");
                            break;
                        default:
                    }
                    control.size = val;
                });

                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                this._bindEvent();

                $("input[value='" + control.controlType + "']",$typeEditor).click().change();
                $("input[value='" + control.size + "']",$sizeEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
            }

            var DateInput = function(){}
            DateInput.prototype = new Widget();
            DateInput.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title: "日期",
                            description:"",
                            controlType:12,
                            size:2,
                            require:0
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            DateInput.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control;

                var $dateCon = $("<div class='widget-date'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $date = $("<input type='text' class='form-control form-time' placeholder='年-月-日'>"),
                    $date2 = $("<input type='text' style='display: none' class='form-control form-time' placeholder='年-月-日'>"),
                    $spaceLine = $("<span class='space-line'> - </span>"),
                    $datebox = $("<div></div>");
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>");
                this.$viewContent.append($dateCon);
                $dateCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$datebox.append([$date,$spaceLine,$date2])])]));

                this.$editor = $("<div class='widget-editor'></div>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),
                    $typeEditor = $("<div class='radio'><label><input type='radio' name='dateType' value='12'> 日期 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='dateType' value='14'> 日期时间 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='dateType' value='13'> 日期区间 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='dateType' value='15'> 日期时间区间 </label></div>"),
                    $sizeEditor = $("<label class='radio-inline'><input type='radio' name='dateSize' value='3'> 大尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='dateSize' value='2'> 标准尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='dateSize' value='1'> 小尺寸 </label>"),
                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='dateRequired' value='1'> 是 </label>" +
                        "<label class='radio-inline'><input type='radio' name='dateRequired' value='0'> 否 </label>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>组件类型</label>",$typeEditor])
                    ,$("<div class='form-group'></div>").append(["<label>控件大小</label>",$("<div></div>").append($sizeEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                ]);

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $("input",$typeEditor).change(function(){
                    var val = $(this).val(),
                        tmp = $date2.add($spaceLine);
                    switch(val){
                        case "12":
                            tmp.hide();
                            $datebox.removeClass("doubleDate");
                            $date.attr("placeholder","年-月-日");
                            break;
                        case "13":
                            tmp.show();
                            $spaceLine.css("display","");
                            $datebox.addClass("doubleDate");
                            $date.attr("placeholder","年-月-日");
                            $date2.attr("placeholder","年-月-日");
                            break;
                        case "14":
                            tmp.hide();
                            $datebox.removeClass("doubleDate");
                            $date.attr("placeholder","年-月-日 时:分");
                            break;
                        case "15":
                            tmp.show();
                            $spaceLine.css("display","");
                            $datebox.addClass("doubleDate");
                            $date.attr("placeholder","年-月-日 时:分");
                            $date2.attr("placeholder","年-月-日 时:分");
                            break;
                        default:

                    }
                    control.controlType = val;
                });

                $("input",$sizeEditor).change(function(){
                    var val = $(this).val(),
                        tmp = $date.add($date2);
                    tmp.removeClass("small medium large");
                    switch(val){
                        case "3":
                            tmp.addClass("large");
                            break;
                        case "2":
                            tmp.addClass("medium");
                            break;
                        case "1":
                            tmp.addClass("small");
                            break;
                        default:

                    }
                    control.size = val;
                });

                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                this._bindEvent();

                $("input[value='" + control.controlType + "']",$typeEditor).click().change();
                $("input[value='" + control.size + "']",$sizeEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
            }

            var Check = function(){
                this.$optionEditorCon = undefined;
            }
            Check.prototype = new Widget();
            Check.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title: "复选框",
                            description:"",
                            controlType:5,
                            vertical: 0,
                            require:0
                        },
                        item:[{
                            itemText: "选项1"
                        },{
                            itemText: "选项2"
                        },{
                            itemText: "选项3"
                        }]
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Check.prototype.getData = function(){

                var items = this.controlInfo.item = [];

                $("[storer]",this.$optionEditorCon).each(function(i){
                    var item = $(this).data("item") || {};
                    item.itemText = $(this).val();
                    item.checkOn = $(this).siblings(":checkbox").is(":checked") ? 1 : 0;
                    item.orderNum = i + 1;
                    items.push(item);
                });

                var temp = {};
                $.extend(temp,this.controlInfo);
                delete temp.children;
                return temp;
            }
            Check.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control,
                    items = this.controlInfo.item;

                var $checkCon = $("<div class='widget-check'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $checksCon = $("<div></div>");
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>");
                this.$viewContent.append($checkCon);
                $checkCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$checksCon])]));

                this.$editor = $("<div class='widget-editor'></div>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $layoutEditor = $("<label class='radio-inline'><input type='radio' name='checkLayout' value='0'> 横 </label>" +
                        "<label class='radio-inline'><input type='radio' name='checkLayout' value='1'> 列 </label>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),

                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='checkRequired' value='1'> 是 </label>" +
                        "<label class='radio-inline'><input type='radio' name='checkRequired' value='0'> 否 </label>");

                this.$optionEditorCon = $("<div class='optionsBox'></div>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>选项布局</label>",$("<div></div>").append($layoutEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>选项设置</label>",this.$optionEditorCon])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                ]);

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                });

                $("input",$layoutEditor).change(function(){
                    var val = $(this).val(),
                        $checks = $checksCon.find("label");
                    switch(val){
                        case "0":
                            if(!$checks.hasClass("checkbox-inline")){
                                $checks.addClass("checkbox-inline");
                                $checks.unwrap();
                            }
                            break;
                        case "1":
                            if($checks.hasClass("checkbox-inline")){
                                $checks.removeClass("checkbox-inline");
                                $checks.wrap("<div class='checkbox'></div>");
                            }
                            break;
                        default:
                    }
                    control.vertical = val;
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });


                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                $.each(items,function(i,item){
                    var $check = $("<label class='checkbox-inline'><input type='checkbox'" + (item.checkOn ? " checked" : "" ) + "><span>" + item.itemText + "</span></label>"),
                        $option = $("<div style='margin-bottom: 10px'><input type='checkbox'" + (item.checkOn ? " checked" : "" ) + "><input type='text' value='" + item.itemText +
                        "' maxlength=10 storer><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span><span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span></div>");
                    $checksCon.append($check);
                    that.$optionEditorCon.append($option);
                    link($check,$option,item);
                    $("[storer]",$option).data("item",item);
                });

                function link($view,$edit,item){
                    var $labelView = $("span",$view),
                        $checkEdit = $(":checkbox",$edit),
                        $labelEdit = $(":text",$edit),
                        $addEdit = $(".glyphicon-plus",$edit),
                        $removeEdit = $(".glyphicon-minus",$edit),
                        $upEdit = $(".glyphicon-arrow-up",$edit),
                        $downEdit = $(".glyphicon-arrow-down",$edit);

                    $checkEdit.click(function(){
                        if(this.checked){
                            $(":checkbox",$view).prop("checked",true);
                        }else{
                            $(":checkbox",$view).prop("checked",false);
                        }
                    });

                    $labelEdit.on("input",function(){
                        $labelView.html(toTXT($(this).val()));
                    });
                    $labelEdit.change(function(){
                        var v = $(this).val();
                        if(v.length){
                            this.defaultValue = v;
                        }else{
                            this.value = this.defaultValue;
                            $(this).trigger("input");
                            ncUnits.alert("选项名不能为空！");
                        }
                    })

                    $addEdit.click(function(){
                        var $newView = $("<label class='checkbox-inline'><input type='checkbox'><span>选项</span></label>"),
                            $newEdit = $("<div style='margin-bottom: 10px'><input type='checkbox'><input type='text' value='选项' maxlength=10 storer><span class='glyphicon glyphicon-plus'></span>" +
                            "<span class='glyphicon glyphicon-minus'></span><span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span></div>")

                        if(control.vertical == 1){
                            $view.parent().after($newView);
                            $newView.removeClass("checkbox-inline");
                            $newView.wrap("<div class='checkbox'></div>");
                        }else{
                            $view.after($newView);
                        }
                        $edit.after($newEdit);
                        link($newView,$newEdit);
                    });

                    $removeEdit.click(function(){
                        if($edit.siblings().length){
                            $view.remove();
                            $edit.remove();
                            if(item.itemId){
                                deleteControlItem.push(item.itemId);
                            }
                        }else{
                            ncUnits.alert("不能删除最后一项!")
                        }
                    });

                    $upEdit.click(function(){
                        var $viewPrev = $view.prev(),
                            $editPrev = $edit.prev();

                        if($viewPrev.length){
                            $viewPrev.before($view)
                        }
                        if($editPrev.length){
                            $editPrev.before($edit)
                        }
                    });

                    $downEdit.click(function(){
                        var $viewNext = $view.next(),
                            $editNext = $edit.next();

                        if($viewNext.length){
                            $viewNext.after($view)
                        }
                        if($editNext.length){
                            $editNext.after($edit)
                        }
                    })
                }

                this._bindEvent();

                $("input[value='" + control.vertical + "']",$layoutEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
            }

            var Radio = function(){
                this.$optionEditorCon = undefined;
            }
            Radio.prototype = new Widget();
            Radio.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title: "单选框",
                            description:"",
                            controlType:4,
                            vertical: 0,
                            require:0
                        },
                        item : [{
                            itemText: "选项1"
                        },{
                            itemText: "选项2"
                        },{
                            itemText: "选项3"
                        }]
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Radio.prototype.getData = function(){

                var items = this.controlInfo.item = [];

                $("[storer]",this.$optionEditorCon).each(function(i){
                    var item = $(this).data("item") || {};
                    item.itemText = $(this).val();
                    item.checkOn = $(this).siblings(":radio").is(":checked") ? 1 : 0;
                    item.orderNum = i + 1;
                    items.push(item);
                });

                var temp = {};
                $.extend(temp,this.controlInfo);
                delete temp.children;
                return temp;
            }
            Radio.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control,
                    items = this.controlInfo.item;

                var $radioCon = $("<div class='widget-radio'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $radiosCon = $("<div></div>");
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>");
                this.$viewContent.append($radioCon);
                $radioCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$radiosCon])]));

                this.$editor = $("<div class='widget-editor'></div>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $layoutEditor = $("<label class='radio-inline'><input type='radio' name='checkLayout' value='0' checked> 横 </label>" +
                        "<label class='radio-inline'><input type='radio' name='checkLayout' value='1'> 列 </label>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),
                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='checkRequired' value='1'> 是 </label>" +
                    "<label class='radio-inline'><input type='radio' name='checkRequired' value='0'> 否 </label>");

                this.$optionEditorCon = $("<div class='optionsBox'></div>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>选项布局</label>",$("<div></div>").append($layoutEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>选项设置</label>",this.$optionEditorCon])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                ]);

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                });

                $("input",$layoutEditor).change(function(){
                    var val = $(this).val(),
                        $radios = $radiosCon.find("label");
                    switch(val){
                        case "0":
                            if(!$radios.hasClass("radio-inline")){
                                $radios.addClass("radio-inline");
                                $radios.unwrap();
                            }
                            break;
                        case "1":
                            if($radios.hasClass("radio-inline")){
                                $radios.removeClass("radio-inline");
                                $radios.wrap("<div class='radio'></div>");
                            }
                            break;
                        default:
                    }
                    control.vertical = val;
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                $.each(items,function(i,item){
                    var $check = $("<label class='radio-inline'><input type='radio' name='" + control.controlId + "t'" + (item.checkOn ? " checked" : "" ) + "><span>" + item.itemText + "</span></label>"),
                        $option = $("<div style='margin-bottom: 10px'><input type='radio' name='" + control.controlId + "'" + (item.checkOn ? " checked" : "" ) + "><input type='text' value='" + item.itemText +
                        "' maxlength=10 storer><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span><span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span></div>");
                    $radiosCon.append($check);
                    that.$optionEditorCon.append($option);
                    link($check,$option,item);
                    $("[storer]",$option).data("item",item);
                });

                function link($view,$edit,item){
                    var $labelView = $("span",$view),
                        $checkEdit = $(":radio",$edit),
                        $labelEdit = $(":text",$edit),
                        $addEdit = $(".glyphicon-plus",$edit),
                        $removeEdit = $(".glyphicon-minus",$edit),
                        $upEdit = $(".glyphicon-arrow-up",$edit),
                        $downEdit = $(".glyphicon-arrow-down",$edit);

                    $checkEdit.click(function(){
                        if(this.checked){
                            $(":radio",$view).prop("checked",true);
                        }else{
                            $(":radio",$view).prop("checked",false);
                        }
                    });

                    $labelEdit.on("input",function(){
                        $labelView.html(toTXT($(this).val()));
                    });
                    $labelEdit.change(function(){
                        var v = $(this).val();
                        if(v.length){
                            this.defaultValue = v;
                        }else{
                            this.value = this.defaultValue;
                            $(this).trigger("input");
                            ncUnits.alert("选项名不能为空！");
                        }
                    })

                    $addEdit.click(function(){
                        var $newView = $("<label class='radio-inline'><input type='radio' name='" + control.controlId + "t'><span>选项</span></label>"),
                            $newEdit = $("<div style='margin-bottom: 10px'><input type='radio' name='" + control.controlId + "'><input type='text' value='选项' maxlength=10 storer><span class='glyphicon glyphicon-plus'></span>" +
                            "<span class='glyphicon glyphicon-minus'></span><span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span></div>")

                        $edit.after($newEdit);
                        link($newView,$newEdit);
                        if(control.vertical == 1){
                            $view.parent().after($newView);
                            $newView.removeClass("radio-inline");
                            $newView.wrap("<div class='radio'></div>");
                        }else{
                            $view.after($newView);
                        }
                    });

                    $removeEdit.click(function(){
                        if($edit.siblings().length){
                            $view.remove();
                            $edit.remove();
                            if(item.itemId){
                                deleteControlItem.push(item.itemId);
                            }
                        }else{
                            ncUnits.alert("不能删除最后一项!")
                        }
                    });

                    $upEdit.click(function(){
                        var $viewPrev = $view.prev(),
                            $editPrev = $edit.prev();

                        if($viewPrev.length){
                            $viewPrev.before($view)
                        }
                        if($editPrev.length){
                            $editPrev.before($edit)
                        }
                    });

                    $downEdit.click(function(){
                        var $viewNext = $view.next(),
                            $editNext = $edit.next();

                        if($viewNext.length){
                            $viewNext.after($view)
                        }
                        if($editNext.length){
                            $editNext.after($edit)
                        }
                    })
                }

                this._bindEvent();

                $("input[value='" + control.vertical + "']",$layoutEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
            }

            var Browse = function(){}
            Browse.prototype = new Widget();
            Browse.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title: "浏览框",
                            description:"",
                            controlType:7,
                            size: 2,
                            require:0,
                            mutliSelect:0
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Browse.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control;

                var $browseCon = $("<div class='widget-browse'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $browse = $("<div class='input-group'></div>"),
                    $input = $("<input type='text' class='form-control'>"),
                    $person = $("<a class='btn btn-default'><span class='fa fa-user'></span></a>"),
                    $organization = $("<a class='btn btn-default' style='display: none'><span class='fa fa-sitemap'></span></a>"),
                    $file = $("<a class='btn btn-default' style='display: none'><span class='fa fa-folder-open'></span></a>"),
                    $cur = $person;
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>"),
                this.$viewContent.append($browseCon);
                $browseCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$browse])]));
                $browse.append([$input,$("<span class='input-group-btn'></span>").append([$person,$organization,$file])]);

                this.$editor = $("<div class='widget-editor'></div>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'></textarea>"),
                    $typeEditor = $("<div class='radio'><label><input type='radio' name='browseType' value='7' checked> 人力资源 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='browseType' value='8'> 组织架构 </label></div>" +
                        "<div class='radio'><label><input type='radio' name='browseType' value='9'> 文件 </label></div>"),
                    $sizeEditor = $("<label class='radio-inline'><input type='radio' name='browseSize' value='3'> 大尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='browseSize' value='2'> 标准尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='browseSize' value='1'> 小尺寸 </label>"),
                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='browseRequired' value='1'> 是 </label>" +
                        "<label class='radio-inline'><input type='radio' name='browseRequired' value='0'> 否 </label>"),
                    $multipleEditor = $("<label class='radio-inline'><input type='radio' name='browseMultiple' value='0'> 单选 </label>" +
                        "<label class='radio-inline'><input type='radio' name='browseMultiple' value='1'> 多选 </label>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>组件类型</label>",$typeEditor])
                    ,$("<div class='form-group'></div>").append(["<label>控件大小</label>",$("<div></div>").append($sizeEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>多选</label>",$("<div></div>").append($multipleEditor)])
                ]);

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $("input",$typeEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "7":
                            $cur.hide();
                            $cur = $person;
                            $cur.show();
                            break;
                        case "8":
                            $cur.hide();
                            $cur = $organization;
                            $cur.show();
                            break;
                        case "9":
                            $cur.hide();
                            $cur = $file;
                            $cur.show();
                            break;
                        default:
                    }
                    control.controlType = val;
                });

                $("input",$sizeEditor).change(function(){
                    var val = $(this).val();
                    $browse.removeClass("small medium large");
                    switch(val){
                        case "3":
                            $browse.addClass("large");
                            break;
                        case "2":
                            $browse.addClass("medium");
                            break;
                        case "1":
                            $browse.addClass("small");
                            break;
                        default:
                    }
                    control.size = val;
                });

                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                $("input",$multipleEditor).change(function(){
                    control.mutliSelect = $(this).val();
                });

                this._bindEvent();

                $("input[value='" + control.controlType + "']",$typeEditor).click().change();
                $("input[value='" + control.size + "']",$sizeEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
                $("input[value='" + control.mutliSelect + "']",$multipleEditor).click().change();
            }

            var Select = function(){
                this.$optionEditorCon = undefined;
            }
            Select.prototype = new Widget();
            Select.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title: "下拉框",
                            description:"",
                            controlType:6,
                            size: 2,
                            require:0
                        },
                        item : [{
                            itemText: "选项1"
                        },{
                            itemText: "选项2"
                        },{
                            itemText: "选项3"
                        }]
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Select.prototype.getData = function(){
                var items = this.controlInfo.item = [];
                $("[storer]",this.$optionEditorCon).each(function(i){
                    var item = $(this).data("item") || {};
                    item.itemText = $(this).val();
                    item.orderNum = i + 1;
                    items.push(item);
                });
                var temp = {};
                $.extend(temp,this.controlInfo);
                delete temp.children;
                return temp;
            }
            Select.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control,
                    items = this.controlInfo.item;

                var $selectCon = $("<div class='widget-select'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $select = $("<select class='form-control'></select>");
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>");
                this.$viewContent.append($selectCon);
                $selectCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$("<div></div>").append($select)])]));

                this.$editor = $("<div class='widget-editor'></div>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),
                    $sizeEditor = $("<label class='radio-inline'><input type='radio' name='selectSize' value='3'> 大尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='selectSize' value='2'> 标准尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='selectSize' value='1'> 小尺寸 </label>"),
                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='selectRequired' value='1'> 是 </label>" +
                    "<label class='radio-inline'><input type='radio' name='selectRequired' value='0'> 否 </label>");

                this.$optionEditorCon = $("<div class='optionsBox'></div>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>列名设置</label>",this.$optionEditorCon])
                    ,$("<div class='form-group'></div>").append(["<label>控件大小</label>",$("<div></div>").append($sizeEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                ]);

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $("input",$sizeEditor).change(function(){
                    var val = $(this).val();
                    $select.removeClass("small medium large");
                    switch(val){
                        case "3":
                            $select.addClass("large");
                            break;
                        case "2":
                            $select.addClass("medium");
                            break;
                        case "1":
                            $select.addClass("small");
                            break;
                        default:
                    }
                    control.size = val
                });

                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                $.each(items,function(i,item){
                    var $option = $("<option>" + item.itemText + "</option>"),
                        $items = $("<div style='margin-bottom: 10px'><input type='text' value='" + item.itemText + "' maxlength=10 storer><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span>" +
                        "<span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span></div>");
                    $select.append($option);
                    that.$optionEditorCon.append($items);
                    link($option,$items,item);
                    $("[storer]",$items).data("item",item);
                });

                function link($view,$edit,item){
                    var $checkEdit = $(":checkbox",$edit),
                        $labelEdit = $(":text",$edit),
                        $addEdit = $(".glyphicon-plus",$edit),
                        $removeEdit = $(".glyphicon-minus",$edit),
                        $upEdit = $(".glyphicon-arrow-up",$edit),
                        $downEdit = $(".glyphicon-arrow-down",$edit);

                    $checkEdit.click(function(){
                        if(this.checked){
                            $view.show();
                        }else{
                            $view.hide();
                        }
                    });

                    $labelEdit.on("input",function(){
                        $view.html(toTXT($(this).val()));
                    });
                    $labelEdit.change(function(){
                        var v = $(this).val();
                        if(v.length){
                            this.defaultValue = v;
                        }else{
                            this.value = this.defaultValue;
                            $(this).trigger("input");
                            ncUnits.alert("选项名不能为空！");
                        }
                    })

                    $addEdit.click(function(){
                        var $newView = $("<option>选项</option>"),
                            $newEdit = $("<div style='margin-bottom: 10px'><input type='text' value='选项' maxlength=10 storer><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span><span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span></div>")

                        $view.after($newView);
                        $edit.after($newEdit);
                        link($newView,$newEdit);
                    });

                    $removeEdit.click(function(){
                        if($edit.siblings().length){
                            $view.remove();
                            $edit.remove();
                            if(item.itemId){
                                deleteControlItem.push(item.itemId);
                            }
                        }else{
                            ncUnits.alert("不能删除最后一项!");
                        }
                    });

                    $upEdit.click(function(){
                        var $viewPrev = $view.prev(),
                            $editPrev = $edit.prev();

                        if($viewPrev.length){
                            $viewPrev.before($view)
                        }
                        if($editPrev.length){
                            $editPrev.before($edit)
                        }
                    });

                    $downEdit.click(function(){
                        var $viewNext = $view.next(),
                            $editNext = $edit.next();

                        if($viewNext.length){
                            $viewNext.after($view)
                        }
                        if($editNext.length){
                            $editNext.after($edit)
                        }
                    })
                }

                this._bindEvent();

                $("input[value='" + control.size + "']",$sizeEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
            }

            var Money = function(){
                this.lowCases = undefined;
            }
            Money.prototype = new Widget();
            Money.prototype._money_upper_arr = [];
            Money.prototype._money_low_arr = [];
            Money.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title: "金额",
                            description:"",
                            controlType:3,
                            size: 2,
                            require:0,
                            linked:""
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Money.prototype.refreshLowCases = function(){
                var that = this;
                $.each(that._money_upper_arr,function(i,up){
                    var $lc = up.lowCases;
                    $lc.empty()
                    $.each(that._money_low_arr,function(i,low){
                        $lc.append("<option value='" + low.controlInfo.control.controlId + "'" + (low.controlInfo.control.controlId == that.controlInfo.control.linked ? " selected" : "") + ">" + low.controlInfo.control.title + "</option>");
                    })
                    $lc.selectpicker("refresh");
                });
            }
            Money.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control;

                var $moneyCon = $("<div class='widget-money'></div>"),
                    $description = $("<div class='control-description'>" + control.description + "</div>"),
                    $money = $("<input type='text' class='form-control'>");
                this.$label = $("<span class='control-label'><div class='label-text'>" + control.title + "</div></span>"),
                this.$viewContent.append($moneyCon);
                $moneyCon.append($("<div class='form-group clearfix'></div>").append([this.$label,$("<div class='control-content'></div>").append([$description,$money])]));

                this.$editor = $("<div class='widget-editor'></div>");
                this.lowCases = $("<select data-width='100px'></select>");
                var $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),
                    $typeEditor = $("<label class='radio-inline'><input type='radio' name='moneyType' value='3' checked> 金额小写 </label>" +
                        "<label class='radio-inline'><input type='radio' name='moneyType' value='17'> 金额大写 </label>"),
                    $lowcaseBox = $("<span style='display: none;margin-left: 5px'></span>"),
                    $sizeEditor = $("<label class='radio-inline'><input type='radio' name='moneySize' value='3'> 大尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='moneySize' value='2'> 标准尺寸 </label>" +
                        "<label class='radio-inline'><input type='radio' name='moneySize' value='1'> 小尺寸 </label>"),
                    $requiredEditor = $("<label class='radio-inline'><input type='radio' name='moneyRequired' value='1'> 是 </label>" +
                        "<label class='radio-inline'><input type='radio' name='moneyRequired' value='0'> 否 </label>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>类型选择</label>",$("<div></div>").append([$typeEditor,$lowcaseBox.append(this.lowCases)])])
                    ,$("<div class='form-group'></div>").append(["<label>控件大小</label>",$("<div></div>").append($sizeEditor)])
                    ,$("<div class='form-group'></div>").append(["<label>必填</label>",$("<div></div>").append($requiredEditor)])
                ]);

                this.lowCases.selectpicker();
                this.lowCases.change(function(){
                    control.linked = $(this).val();
                })

                $labelEditor.on("input",function(){
                    var v = $(this).val();
                    that.changeLabel(v);
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $("input",$typeEditor).change(function(){
                    if(that.formulaCount){
                        ncUnits.alert("不可修改:有公式使用了该控件");
                        $("input[value='3']",$typeEditor).prop("checked",true);
                    }else{
                        var val = $(this).val(),
                            $link = that.$link;
                        switch(val){
                            case "3":
                                //that.textTransform = 0;
                                that._money_low_arr.push(that);
                                var i = that._money_upper_arr.indexOf(that);
                                if(i >= 0){
                                    that._money_upper_arr.splice(i,1);
                                }
                                $lowcaseBox.hide();
                                that.refreshLowCases();
                                if($link){
                                    $link.find(":checkbox").attr("disabled",false);
                                }
                                break;
                            case "17":
                                //that.textTransform = 1;
                                that._money_upper_arr.push(that);
                                var i = that._money_low_arr.indexOf(that);
                                if(i >= 0){
                                    that._money_low_arr.splice(i,1);
                                }
                                $lowcaseBox.show();
                                that.refreshLowCases();
                                control.linked = that.lowCases.val();
                                if($link) {
                                    $link.find(":checkbox").attr("disabled", true).prop("checked", false);
                                }
                                break;
                            default:
                        }

                        control.controlType = val;
                    }
                });

                $("input",$sizeEditor).change(function(){
                    var val = $(this).val();
                    $money.removeClass("small medium large");
                    switch(val){
                        case "3":
                            $money.addClass("large");
                            break;
                        case "2":
                            $money.addClass("medium");
                            break;
                        case "1":
                            $money.addClass("small");
                            break;
                        default:
                    }
                    control.size = val;
                });

                $("input",$requiredEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "0":
                            that.$label.removeClass("sign-requ");
                            break;
                        case "1":
                            that.$label.addClass("sign-requ");
                            break;
                        default:
                    }
                    control.require = val;
                });

                this._bindEvent();

                $("input[value='" + control.controlType + "']",$typeEditor).click().change();
                $("input[value='" + control.size + "']",$sizeEditor).click().change();
                $("input[value='" + control.require + "']",$requiredEditor).click().change();
                if(control.linked.length){
                    this.lowCases.selectpicker('val', control.linked);
                }
            }
            Money.prototype.afterActivate = function(){
                this.refreshLowCases();
            }
            Money.prototype.afterRemove = function(){
                this.refreshLowCases();
                var i = this._money_low_arr.indexOf(this);
                if(i >= 0){
                    this._money_low_arr.splice(i,1);
                }
            }

            var Line = function(){}
            Line.prototype = new Widget();
            Line.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            controlType:10,
                            lineType: 1,
                            color:"#ccc"
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Line.prototype.render = function(){
                this._wrap();
                var that = this,
                    control = this.controlInfo.control;

                var $line = $("<div class='widget-line line' style='border-color: " + control.color + "'></div>");
                this.$viewContent.append($line);

                this.$editor = $("<div class='widget-editor'></div>");
                var $typeEditor = $("<div class='radio'><label><input type='radio' name='inputType' value='1'><div class='line line-solid'></div></label></div>" +
                    "<div class='radio'><label><input type='radio' name='inputType' value='3'><div class='line line-double'></div></label></div>" +
                    "<div class='radio'><label><input type='radio' name='inputType' value='2'><div class='line line-dashed'></div></label></div>"),
                    $colorpicker = $("<div></div>");
                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>分割线类型</label>",$typeEditor])
                    ,$("<div class='form-group'></div>").append(["<label>分割线颜色</label>",$colorpicker])
                ]);

                $("input",$typeEditor).change(function(){
                    var val = $(this).val();
                    switch(val){
                        case "1":
                            $line.removeClass("line-double line-dashed").addClass("line-solid");
                            break;
                        case "3":
                            $line.removeClass("line-solid line-dashed").addClass("line-double");
                            break;
                        case "2":
                            $line.removeClass("line-double line-solid").addClass("line-dashed");
                            break;
                        default:
                    }
                    control.lineType = val;
                });

                $colorpicker.colorpicker({
                    color:control.color,
                    history:false,
                    strings:"主题色,标准色,网格式,主题式"
                }).on("change.color", function(e, c){
                    $line.css('border-color', c);
                    control.color = c;
                });

                this._bindEvent();

                $("input[value='" + control.lineType + "']",$typeEditor).click().change();
            }



            /* 布局原型 开始 */
            var Layout = function(){
                this.id = undefined;
                this.$editorContainer = $editor;
                this.$view = undefined;
                this.$editor = undefined;
                this.$viewContent = undefined;
                this.$operateBar = undefined;

                this.type = undefined;
                this.parent = undefined;
                this.actived = false;
                this.controlInfo = undefined;
            }
            Layout.prototype.layoutHandle = function(){
                var that = this;
                $(".layout-cell",this.$view).each(function(i){
                    $(this).sortable({
                        placeholder: "widget_placeholder"
                        ,connectWith: $(".layout-cell:empty",$widget_container).selector + "," + $widget_container.selector
                        ,handle: ".move-handle"
                        ,tolerance: "pointer"
                        ,update: function(e,ui){
                            var $item = ui.item;
                            if($item.is(".layout")){
                                if($item.data("rendered")){
                                    ui.sender.sortable("cancel");
                                }else{
                                    $item.remove();
                                }
                            }else{
                                if(!$item.data("rendered")){
                                    $item.removeAttr("style");
                                    renderWidget($item);
                                }
                                var widget = $item.data("widget");
                                if(receiveItem){
                                    widget.parent = receiveItem
                                    widget.controlInfo.control.parentControl = (receiveItem == "main" ? receiveItem : receiveItem.controlInfo.control.controlId);
                                }
                                widget.controlInfo.control.columnIndex = colIndex;
                            }
                        }
                        ,receive:function(){
                            receiveItem = that;
                            colIndex = i + 1;
                        }
                        ,stop:function(){
                            receiveItem = 0;
                            colIndex = 0;
                        }
                        ,over:function(e,ui){
                            var $item = ui.item;
                            if($item.is(".layout")){
                                ui.placeholder.hide();
                            }
                        }
                        ,out:function(e,ui){
                            ui.placeholder.show();
                        }
                    }).disableSelection();
                })
            }
            Layout.prototype._wrap = function(){
                var that = this;
                var $tableRow = $("<div class='table-row'></div>"),
                    $delete = $("<a href='javascript:void(0)' class='glyphicon glyphicon-trash'></a>"),
                    $move = $("<a href='javascript:void(0)' class='glyphicon glyphicon-move move-handle'></a>");

                this.$operateBar = $("<div class='layout-operate'></div>");
                this.$viewContent = $("<div class='layout-divide'></div>");

                this.$view.append($tableRow);
                $tableRow.append([this.$viewContent,this.$operateBar]);
                this.$operateBar.append([$delete,$move]);

                $delete.click(function(){
                    that._remove();
                })

                this.$view.mouseover(function(){
                    that.$operateBar.css({
                        visibility : "visible"
                    });
                });
                this.$view.mouseout(function(){
                    that.$operateBar.css({
                        visibility : ""
                    });
                });
            }
            Layout.prototype._remove = function(){
                var that = this;
                if(this.controlInfo.control.used){
                    ncUnits.alert("不可删除,该控件已被使用!");
                }else{
                    ncUnits.confirm({
                        title:"删除",
                        html:"确认删除该控件?",
                        yes:function(layid){
                            var control = that.controlInfo.control;
                            that.$view.remove();
                            if(that.$editor){
                                that.$editor.remove();
                            }
                            if(control.loaded){
                                deleteControl.push(control.controlId);
                            }
                            that.afterRemove();

                            layer.close(layid);
                        }
                    })
                }
            }
            Layout.prototype.activate = function(){
                if(!this.actived){
                    if(activedWidget){
                        activedWidget.deactivate();
                    }
                    this.$view.addClass("active");
                    this.$editorContainer.html(this.$editor);
                    activedWidget = this;
                    this.actived = true;
                }
            }
            Layout.prototype.deactivate = function(){
                this.$view.removeClass("active");
                if(this.$editor){
                    this.$editor.detach();
                }
                this.actived = false;
            }
            Layout.prototype._bindEvent = function(){
                var that = this;
                if(this.$editBtn){
                    this.$editBtn.click(function(e){
                        e.stopPropagation();
                        that.activate();
                    });
                }
                this.layoutHandle();
            }
            Layout.prototype.getData = function(){
                var temp = {};
                $.extend(temp,this.controlInfo);
                delete temp.children;
                return temp;
            }
            Layout.prototype.init = function(){
                console.log("abstract function");
            }
            Layout.prototype.render = function(){
                console.log("abstract function");
            }
            Layout.prototype.afterRemove = function(){
                console.log("abstract function");
            }
            /* 布局原型 结束 */

            var Col2 = function(){}
            Col2.prototype = new Layout();
            Col2.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            controlType:18
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Col2.prototype.render = function(){
                var that = this,
                    children = this.controlInfo.children;
                this._wrap();

                this.$viewContent.append("<div class='dis-table'><div class='col-xs-6 layout-cell'></div><div class='col-xs-6 layout-cell'></div></div>");

                if(children){
                    $.each(children,function(i,child){
                        var colIndex = child.control.columnIndex ? child.control.columnIndex - 1 : i,
                            $w = $("<div class='widget-item component'></div>");
                        that.$viewContent.find(".layout-cell:eq(" + colIndex + ")").append($w);
                        renderWidget($w,child);
                    });
                }
                this._bindEvent();
            }

            var Col3 = function(){}
            Col3.prototype = new Layout();
            Col3.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            controlType:19
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
            }
            Col3.prototype.render = function(){
                var that = this,
                    children = this.controlInfo.children;
                this._wrap();

                this.$viewContent.append("<div class='dis-table'><div class='col-xs-4 layout-cell'></div><div class='col-xs-4 layout-cell'></div><div class='col-xs-4 layout-cell'></div></div>");

                if(children){
                    $.each(children,function(i,child){
                        var colIndex = child.control.columnIndex ? child.control.columnIndex - 1 : i,
                            $w = $("<div class='widget-item component'></div>");
                        that.$viewContent.find(".layout-cell:eq(" + colIndex + ")").append($w);
                        renderWidget($w,child);
                    });
                }
                this._bindEvent();
            }


            var Account = function(){
                this.cols = [];
                this.$formulas = undefined;
            }
            Account.prototype = new Layout();
            Account.prototype.init = function(o){
                this.controlInfo = {};
                if(o){
                    $.extend(this.controlInfo,o);
                }else{
                    var def = {
                        control:{
                            title:"",
                            description:"",
                            controlType:16,
                            defaultRowNum:1
                        }
                    }
                    $.extend(this.controlInfo,def);
                }
                if(!this.controlInfo.children){
                    this.controlInfo.children = [];
                }
            }
            Account.prototype.getData = function(){
                var that = this,
                    wId = this.controlInfo.control.controlId;
                this.formatCols();
                var formulas = this.controlInfo.formula = [];
                this.$formulas.find(".formula-text").each(function(){
                    var formulaId = $(this).data("id");
                    $(this).children().each(function(i){
                        var widget = $(this).data("widget"),
                            number = $(this).data("number"),
                            operational = $(this).data("operate");
                        if(widget){
                            formulas.push({
                                formulaId:formulaId,
                                orderNum:i + 1,
                                detailControl:wId,
                                controlId: widget.controlInfo.control.controlId
                            })
                        }else if(number){
                            formulas.push({
                                formulaId:formulaId,
                                orderNum:i + 1,
                                detailControl:wId,
                                displayText:number
                            })
                        }else if(operational){
                            formulas.push({
                                formulaId:formulaId,
                                orderNum:i + 1,
                                detailControl:wId,
                                operate:operational
                            })
                        }
                    });
                })
                var temp = {};
                $.extend(temp,this.controlInfo);
                delete temp.children;
                return temp;
            }
            Account.prototype.formatCols = function(){
                var cols = this.cols;
                cols.length = 0;
                $(".layout-cell .widget-item",this.$view).each(function(i){
                    var widget = $(this).data("widget");
                    widget.controlInfo.control.columnIndex = i + 1;
                    cols.push(widget);
                });
            }
            Account.prototype.formatFormula = function(listJson){
                var map = {};
                $.each(listJson,function(i,v){
                    var id = v.formulaId,
                        index = v.orderNum - 1;
                    if(!map[id]){
                        map[id] = [];
                    }
                    map[id][index] = v;
                })
                return map;
            }
            Account.prototype.layoutHandle = function(){
                var that = this;
                $(".layout-cell",this.$view).sortable({
                    placeholder: "widget_placeholder"
                    ,connectWith: $(".layout-cell:empty",$widget_container).selector + "," + $widget_container.selector
                    ,handle: ".move-handle"
                    ,tolerance: "pointer"
                    ,receive:function(){
                        receiveItem = that;
                    }
                    ,remove:function(){
                        var $linkItem = $(this).data("link"),
                            widget = $linkItem.data("link");
                        $(":text",$linkItem).val("");
                        $linkItem.removeData("link");
                        $(":checkbox",$linkItem).removeProp("checked").attr("disabled",true);
                        widget.$link = undefined;
                    }
                    ,update:function(e,ui){
                        var $item = ui.item;
                        if($item.is(".layout")){
                            if($item.data("rendered")){
                                ui.sender.sortable("cancel");
                            }else{
                                $item.remove();
                            }
                        }else{
                            if (!$item.data("rendered")) {
                                $item.removeAttr("style");
                                renderWidget($item);
                            }
                            var widget = $item.data("widget"),
                                $linkItem = $(this).data("link"),
                                control = widget.controlInfo.control,
                                type = control.controlType;
                            if(receiveItem){
                                widget.parent = receiveItem;
                                control.parentControl = (receiveItem == "main" ? receiveItem : receiveItem.controlInfo.control.controlId);
                            }
                            if(type == 2 || type == 3){
                                $(":checkbox", $linkItem).attr("disabled", false);
                            }
                            widget.$link = $linkItem;
                            $linkItem.data("link", widget);
                            $(":text", $linkItem).val(widget.controlInfo.control.title);
                            $(":checkbox", $linkItem).prop("checked", !!widget.controlInfo.control.columnStatistics);
                            that.formatCols();
                        }
                    }
                    ,stop: function(e,ui){
                        receiveItem = 0;
                        var widget = ui.item.data("widget");
                        if(widget && widget.formulaCount && widget.parent != that){
                            $(this).sortable("cancel");

                            var $linkItem = $(this).data("link"),
                                control = widget.controlInfo.control,
                                type = control.controlType;
                            widget.parent = that;
                            control.parentControl = that.controlInfo.control.controlId;
                            if(type == 2 || type == 3){
                                $(":checkbox", $linkItem).attr("disabled", false);
                            }
                            widget.$link = $linkItem;
                            $linkItem.data("link", widget);
                            $(":text", $linkItem).val(widget.controlInfo.control.title);
                            $(":checkbox", $linkItem).prop("checked", !!widget.controlInfo.control.columnStatistics);
                            that.formatCols();

                            ncUnits.alert("不可移动,有公式使用了该控件!");
                        }
                    }
                    ,over:function(e,ui){
                        var $item = ui.item;
                        if($item.is(".layout")){
                            ui.placeholder.hide();
                        }
                    }
                    ,out:function(e,ui){
                        ui.placeholder.show();
                    }
                }).disableSelection();
            }
            Account.prototype.render = function(){
                this._wrap();
                this.$view.addClass("onlyCell");
                var that = this,
                    control = this.controlInfo.control,
                    children = this.controlInfo.children,
                    formula = this.controlInfo.formula;

                var $add = $("<a href='javascript:void(0)' class='glyphicon glyphicon-plus'></a>"),
                    $accountLabel = $("<div class='account-label'>" + control.title + "</div>"),
                    $description = $("<div class='account-description'>" + control.description + "</div>"),
                    $table = $("<table class='account-table'><tbody><tr></tr></tbody></table>"),
                    $labelEditor = $("<input type='text' class='form-control' maxlength='50' value='" + control.title + "'>"),
                    $descriptionEditor = $("<textarea class='form-control' rows='3' maxlength='100'>" + control.description + "</textarea>"),
                    $rowNum = $("<input type='text' class='short' value='" + control.defaultRowNum + "'>"),
                    $summationBtn = $("<a class='btn btn-transparency pull-right' data-toggle='modal' data-target='#formula_modal' data-backdrop=false style='padding: 2px 5px'>设置</a>");

                this.$formulas = $("<ul class='list-unstyled formulaList'></ul>");
                this.$editBtn = $("<a href='javascript:void(0)' class='glyphicon glyphicon-pencil'></a>");
                this.$editor = $("<div class='widget-editor'></div>");
                this.$optionEditorCon = $("<div class='optionsBox'></div>");

                this.$editor.append([
                    $("<div class='form-group'></div>").append(["<label>标题</label>",$labelEditor])
                    ,$("<div class='form-group'></div>").append(["<label>描述</label>",$descriptionEditor])
                    ,$("<div class='form-group'></div>").append(["<label>列名设置</label><label class='pull-right'>是否合计</label>",this.$optionEditorCon])
                    ,$("<div class='form-group'></div>").append(["<label>默认行数</label>",$("<div></div>").append($rowNum)])
                    ,$("<div class='form-group'></div>").append(["<label>公式</label>",$summationBtn,"<hr>",this.$formulas])
                ]);

                this.$operateBar.prepend([this.$editBtn,$add]);
                this.$viewContent.append([$accountLabel,$description,$("<div class='account-container'></div>").append($table)]);

                $labelEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $accountLabel.html(val);
                    control.title = val;
                });

                $descriptionEditor.on("input",function(){
                    var val = toTXT($(this).val());
                    $description.html(val);

                    control.description = val;
                });

                $rowNum.on("input",function(){
                    control.defaultRowNum = $(this).val();
                });

                $summationBtn.click(function(){
                    var $keyarea = $("#formula_modal_cols");
                    $keyarea.empty();
                    $.each(that.cols,function(i,w){
                        if(w.controlInfo){
                            var type = w.controlInfo.control.controlType;
                            if(type == 2 || type == 3){
                                var control = w.controlInfo.control;
                                var $key = $("<a class='btn btn-default' title='" + control.title + "'>" + control.title + "</a>");
                                $keyarea.append($key);
                                $key.data("widget",w);
                            }
                        }
                    });

                    $("#formula_modal_submit").data("appendto",that.$formulas);
                });

                $add.click(function(){
                    var $tr = $("tr",$table),
                        $newView = $("<td class='layout-cell'></td>"),
                        $newEdit = $("<div style='margin-bottom: 10px'><input type='text' storer disabled><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span><span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span><input type='checkbox' class='pull-right' style='margin-right: 15px' disabled></div>")

                    $tr.append($newView);
                    that.$optionEditorCon.append($newEdit);
                    link($newView,$newEdit);
                    that.layoutHandle();
                    $newView.data("link",$newEdit);
                    if($tr.children().length > 1){
                        that.$view.removeClass("onlyCell");
                    }
                });

                $.each((children && children.length) ? children : [{},{}],function(i,item){
                    if(i > 0){
                        that.$view.removeClass("onlyCell");
                    }
                    var $option = $("<td class='layout-cell'></td>"),
                        $item = $("<div style='margin-bottom: 10px'><input type='text' value='' storer disabled><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span>" +
                        "<span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span><input type='checkbox' class='pull-right' style='margin-right: 15px' disabled></div>");
                    $("tr",$table).append($option);
                    that.$optionEditorCon.append($item);
                    link($option,$item);
                    $("[storer]",$item).data("item",item);
                    $option.data("link",$item);
                    if(item.control){
                        var $w = $("<div class='widget-item component'></div>");
                        $option.append($w);
                        renderWidget($w,item);

                        var widget = $w.data("widget"),
                            type = item.control.controlType;
                        widget.parent = that;
                        widget.$link = $item;
                        $item.data("link",widget);
                        $(":text",$item).val(widget.controlInfo.control.title);
                        $(":checkbox",$item).attr("disabled", type != 2 && type != 3).attr("checked",!!widget.controlInfo.control.columnStatistics);
                    }
                });
                this.formatCols();

                if(formula){
                    $.each(this.formatFormula(formula),function(k,formulaItems){
                        var $formula = $("<li></li>"),
                            $formulaText = $("<span class='formula-text'></span>").data("id",k),
                            $del = $("<span class='glyphicon glyphicon-trash clickable'></span>");
                        $formula.append([$formulaText,$("<span class='formula-operate'></span>").append($del)]).appendTo(that.$formulas);
                        $.each(formulaItems,function(i,formulaItem){
                            if(formulaItem.controlId){
                                var widget = $("[widgetid='" + formulaItem.controlId + "']",$widget_container).data("widget");
                                var $key = $("<span>" + widget.controlInfo.control.title + "</span>").data("widget",widget);
                                $formulaText.append($key);
                            }else if(formulaItem.displayText){
                                var text = formulaItem.displayText;
                                var $key = $("<span>" + text + "</span>").data("number",text);
                                $formulaText.append($key);
                            }else if(formulaItem.operate){
                                var text = formulaItem.operate;
                                var $key = $("<span>" + text + "</span>").data("operate",text);
                                $formulaText.append($key);
                            }
                        });
                        $del.click(function(){
                            $formula.remove();
                        })
                    })
                }

                function link($view,$edit){
                    var $labelEdit = $(":text",$edit),
                        $addEdit = $(".glyphicon-plus",$edit),
                        $removeEdit = $(".glyphicon-minus",$edit),
                        $upEdit = $(".glyphicon-arrow-up",$edit),
                        $downEdit = $(".glyphicon-arrow-down",$edit),
                        $checkEdit = $(":checkbox",$edit);

                    $checkEdit.click(function(){
                        var widget = $edit.data("link");
                        if(widget){
                            if(this.checked){
                                widget.controlInfo.control.columnStatistics = 1;
                            }else{
                                widget.controlInfo.control.columnStatistics = 0;
                            }
                        }
                    });

                    //$labelEdit.on("input",function(){
                    //    $(".col-title",$view).html($(this).val());
                    //});

                    $addEdit.click(function(){
                        var $newView = $("<td class='layout-cell'></td>"),
                            $newEdit = $("<div style='margin-bottom: 10px'><input type='text' storer disabled><span class='glyphicon glyphicon-plus'></span><span class='glyphicon glyphicon-minus'></span>" +
                            "<span class='glyphicon glyphicon-arrow-up'></span><span class='glyphicon glyphicon-arrow-down'></span><input type='checkbox' class='pull-right' style='margin-right: 15px' disabled></div>")

                        $view.after($newView);
                        $edit.after($newEdit);
                        that.layoutHandle();
                        link($newView,$newEdit);
                        $newView.data("link",$newEdit);
                        if($("tr",$table).children().length > 1){
                            that.$view.removeClass("onlyCell");
                        }
                    });

                    $removeEdit.click(function(){
                        var widget = $view.children(".component").data("widget");
                        if(!widget || widget._remove(true)){
                            $view.remove();
                            $edit.remove();
                            if($("tr",$table).children().length < 2){
                                that.$view.addClass("onlyCell");
                            }
                            that.formatCols();
                        }
                    });

                    $upEdit.click(function(){
                        var $viewPrev = $view.prev(),
                            $editPrev = $edit.prev();

                        if($viewPrev.length){
                            $viewPrev.before($view)
                        }
                        if($editPrev.length){
                            $editPrev.before($edit)
                        }
                    });

                    $downEdit.click(function(){
                        var $viewNext = $view.next(),
                            $editNext = $edit.next();

                        if($viewNext.length){
                            $viewNext.after($view)
                        }
                        if($editNext.length){
                            $editNext.after($edit)
                        }
                    });
                }

                this._bindEvent();
            }

            function renderWidget($widget,data){
                $widget.empty();
                $widget.each(function(){
                    var widget = createWidget(data ? getType(data.control.controlType) : $(this).attr("wtype"));
                    widget.parent = receiveItem;
                    widget.$view = $(this);
                    $(this).data("widget",widget);
                    widget.init(data);
                    var id = widget.controlInfo.control.controlId = widget.controlInfo.control.controlId || getId();
                    widget.controlInfo.control.parentControl = receiveItem ? ( typeof receiveItem == "string" ? receiveItem : receiveItem.controlInfo.control.controlId) : widget.controlInfo.control.parentControl;
                    widget.controlInfo.control.columnIndex = colIndex ? colIndex : widget.controlInfo.control.columnIndex;
                    widget.render();
                    $(this).data("rendered",true);
                    $(this).attr("widgetid",id);
                    widget.activate();
                });
            }

            var createWidget = (function(){
                var map = {
                    "col2" : Col2,
                    "col3" : Col3,
                    "account" : Account,
                    "label" : Label,
                    "input" : Input,
                    "date" : DateInput,
                    "check" : Check,
                    "radio" : Radio,
                    "browse" : Browse,
                    "select" : Select,
                    "money" : Money,
                    "line" : Line
                }
                return function(type){
                    var widget = new map[type]();
                    return widget;
                }
            })();

            var getType = (function(){
                var map = [];
                map[0] = "label";
                map[1] = map[2] = map[11] = "input";
                map[3] = map[17] = "money";
                map[4] = "radio";
                map[5] = "check";
                map[6] = "select";
                map[7] = map[8] = map[9] = "browse";
                map[10] = "line";
                map[12] = map[13] = map[14] = map[15] = "date";
                map[16] = "account";
                map[18] = "col2";
                map[19] = "col3";
                return function(type){
                    return map[type];
                }
            })()

            $widget_container.sortable({
                placeholder: "widget_placeholder"
                ,connectWith: $(".layout-cell:empty",$widget_container).selector
                ,handle: ".move-handle"
                ,tolerance: "pointer"
                ,update: function(e,ui){
                    var $item = ui.item;
                    if(!$item.data("rendered")){
                        $item.removeAttr("style");
                        renderWidget($item);
                    }
                    var widget = $item.data("widget");
                    if(receiveItem){
                        widget.parent = receiveItem;
                        widget.controlInfo.control.parentControl = (receiveItem == "main" ? receiveItem : receiveItem.controlInfo.control.controlId);
                    }
                }
                ,receive: function(){
                    receiveItem = "main";
                }
                ,stop: function(){
                    receiveItem = 0;
                }
            }).disableSelection();
            $dragItems.draggable({
                helper: "clone"
                ,appendTo: document.body
                ,cursor: "move"
                ,cursorAt: { top: 15, left: 60 }
                ,refreshPositions:true
                ,connectToSortable: $(".layout-cell:empty",$widget_container).selector + "," + $widget_container.selector
                ,start: function(e,ui){
                    ui.helper.width(ui.helper.width());
                }
            }).disableSelection();

            initFormula();

            function initEditor(){
                Money.prototype._money_upper_arr = [];
                Money.prototype._money_low_arr = [];
            }
            return {
                getJson:function(){
                    var widgets = [];
                    var index = 1;
                    $(".widget-item",$widget_container).each(function(){
                        var data = $(this).data("widget").getData();
                        if(data.control.parentControl == "main"){
                            data.control.rowIndex = index++;
                        }
                        data.control.status = 2;
                        widgets.push(data);
                    });

                    return widgets;
                },
                getDeleteControl:function(){
                    return deleteControl
                },
                getDeleteControlItem:function(){
                    return deleteControlItem;
                },
                loadData:function(datas){
                    $widget_container.empty();
                    initEditor();
                    $.each(format(datas),function(i,data){
                        var type = data.control.controlType,
                            $widget;
                        if(type == 16 || type == 18 || type == 19){
                            $widget = $("<div class='widget-item layout'></div>")
                        }else{
                            $widget = $("<div class='widget-item component'></div>")
                        }
                        $widget_container.append($widget);
                        renderWidget($widget,data);
                    })
                }
            }
        }
    });

    function format(listJson){
        var tree = {};
        $.each(listJson,function(i,v){
            var p = v.control.parentControl;
            if( p && p != "main"){
                if(tree[p]){
                    if(tree[p].children){
                        tree[p].children.push(v);
                    }else{
                        tree[p].children = [v];
                    }
                }else{
                    tree[p] = {
                        children:[v]
                    }
                }
            }else{
                var id = v.control.controlId;
                tree[id] = $.extend(tree[id],v);
            }
        });

        return _.values(tree);
    }

    var getId = (function(){
        var labels = ["greedisgood","showmethemoney"];
        return function(type){
            //type : 0/undefined 控件    1 公式
            type = type || 0;
            return labels[type] + Date.now();
        }
    })();

    var initFormula = function(){
        if(!$("#formula_modal").length){
            var html = "<div class='modal fade calculator' id=formula_modal tabindex=-1 role=dialog aria-labelledby=formula_modal_label aria-hidden=true>" +
                "<div class=modal-dialog><div class=modal-content><div class=modal-header><button type=button class=close data-dismiss=modal aria-label=Close>" +
                "<span class='glyphicon glyphicon-remove' aria-hidden=true></span></button><h4 class=modal-title id=formula_modal_label>公式设置</h4></div>" +
                "<div class='monitor well well-sm'><div class='text-right monitor-inner' id='formula_modal_monitor'></div></div><div class='operate clearfix'><a class='btn btn-transparency pull-right' id=formula_modal_backspace>退格</a>" +
                "<a class='btn btn-transparency pull-right' id=formula_modal_clear>清除全部</a></div><hr><div class='fifty-fifty clearfix area-key'><div class=area>" +
                "<div class='areabox clearfix' id=formula_modal_cols></div></div><div class='area area-number'><fieldset class='areabox clearfix' disabled><a class='btn btn-default btn-operational' value='('>(</a>" +
                "<a class='btn btn-default btn-operational' value=')'>)</a><a class='btn btn-default btn-operational' value='+'>+</a><a class='btn btn-default btn-operational' value='-'>-</a><a class='btn btn-default btn-number' value='1'>1</a>" +
                "<a class='btn btn-default btn-number' value='2'>2</a><a class='btn btn-default btn-number' value='3'>3</a><a class='btn btn-default btn-operational' value='*'>*</a><a class='btn btn-default btn-number' value='4'>4</a><a class='btn btn-default btn-number' value='5'>5</a>" +
                "<a class='btn btn-default btn-number' value='6'>6</a><a class='btn btn-default btn-operational' value='/'>/</a><a class='btn btn-default btn-number' value='7'>7</a><a class='btn btn-default btn-number' value='8'>8</a><a class='btn btn-default btn-number' value='9'>9</a>" +
                "<a class='btn btn-default btn-high btn-operational pull-right hide' value='='>=</a><a class='btn btn-default btn-long btn-number' value='0'>0</a><a class='btn btn-default btn-number' value='.'>·</a></fieldset></div></div>" +
                "<div class='modal-footer fifty-fifty'><a class='btn btn-transparency' data-dismiss=modal>取消</a><a class='btn btn-transparency' id=formula_modal_submit>确定</a>" +
                "</div></div></div></div>";
            $("body").append(html);

            var $formula = $("#formula_modal"),
                $backspace = $("#formula_modal_backspace"),
                $clear = $("#formula_modal_clear"),
                $monitor = $("#formula_modal_monitor"),
                $cols = $("#formula_modal_cols"),
                $numbox = $(".area-number .areabox",$formula),
                $submit = $("#formula_modal_submit");

            $monitor.on("DOMNodeInserted DOMNodeRemoved",function(){
                $(this).children(":last").get(0).scrollIntoView();
            })

            $cols.delegate(".btn","click",function(){
                if(!$(this).attr("disabled")){
                    var widget = $(this).data("widget");
                    var $key = $("<span>" + widget.controlInfo.control.title + "</span>");
                    $key.data("widget",widget);

                    if($monitor.children().length == 0){
                        $numbox.attr("disabled",false);
                        $(this).attr("disabled",true);
                        $monitor.append([$key,$("<span>=</span>").data("operate","=")]);
                    }else{
                        $monitor.append($key);
                    }
                }
            });

            $(".area-number .btn-number",$formula).click(function(){
                var text = $(this).attr("value");
                var $key = $("<span>" + text + "</span>");
                $key.data("number",text);
                $monitor.append($key);
            });
            $(".btn-operational",$formula).click(function(){
                var text = $(this).attr("value");
                var $key = $("<span>" + text + "</span>");
                $key.data("operate",text);
                $monitor.append($key);
            });

            $backspace.click(function(){
                var $last = $monitor.children(":last");
                if($last.html() == "="){
                    $monitor.empty();
                }else{
                    $last.remove();
                }
                if($monitor.children().length == 0){
                    init();
                }
            });
            $clear.click(function(){
                init();
            });
            $formula.on('hidden.bs.modal', function () {
                init();
            });

            $submit.click(function(){
                var $formula = $("<li></li>"),
                    $formulaText = $("<span class='formula-text'></span>"),
                    $del = $("<a href='javascript:void(0)' class='glyphicon glyphicon-trash clickable'></a>"),
                    $appendto = $(this).data("appendto");

                var start = false,
                    check = true,
                    formulaStr = "",
                    yId = "",
                    widgetCache = [];
                $monitor.children().each(function(i){
                    var widget = $(this).data("widget"),
                        number = $(this).data("number"),
                        operate = $(this).data("operate");
                    if(i == 0 && widget){
                        yId = widget.controlInfo.control.controlId;
                    }else if(start){
                        formulaStr += (widget ? "x" : (number || operate));
                    }else{
                        if(operate == "="){
                            start = true;
                        }
                    }

                    if(widget){
                        widgetCache.push(widget);
                    }
                });
                if(formulaStr == ""){
                    check = false;
                    ncUnits.alert("公式不能为空!");
                }else{
                    try{
                        var regular1 = /^[^+\-\*\/]/,
                            regular2 = /[+\-\*\/][+\-\*\/]/;
                        if(!(regular1.test(formulaStr) && !regular2.test(formulaStr) && $.isNumeric(eval("var x = 1.1;" + formulaStr)))){
                            ncUnits.alert("公式不正确!");
                            check = false;
                        }
                    }catch(e){
                        ncUnits.alert("公式不正确!");
                        check = false;
                    }
                }
                if(check){
                    $appendto.children().each(function(){
                        if($(".formula-text>:eq(0)",this).data("widget").controlInfo.control.controlId == yId){
                            check = false;
                            ncUnits.alert("控件对应公式过多!");
                            return false;
                        }
                    })
                }

                //if(check){
                //    var formulaHtmlStr = getFormulaFromEle($monitor);
                //    $appendto.children().each(function(){
                //        if(formulaHtmlStr == getFormulaFromEle($(".formula-text",this))){
                //            check = false;
                //            ncUnits.alert("公式重复!");
                //            return false;
                //        }
                //    })
                //}

                if(check){
                    $monitor.children().clone(true).appendTo($formulaText);
                    $formula.append([$formulaText,$("<span class='formula-operate'></span>").append($del)]).appendTo($appendto);

                    $del.click(function(){
                        $formula.remove();
                        $.each(widgetCache,function(i,w){
                            w.formulaCount --;
                        })
                    });
                    ncUnits.alert("公式添加成功!");
                    init();
                    $formulaText.data("id",getId(1));

                    $.each(widgetCache,function(i,w){
                        w.formulaCount ++;
                    })
                }
            });

            function init(){
                $monitor.empty();
                $cols.find(".btn[disabled]").attr("disabled",false);
                $numbox.prop("disabled",true);
            }

            function getFormulaFromEle($con){
                var formulaStr = "";
                $con.children().each(function(){
                    var widget = $(this).data("widget"),
                        number = $(this).data("number"),
                        operate = $(this).data("operate");

                    if(widget){
                        formulaStr += widget.controlInfo.control.controlId;
                    }else{
                        formulaStr += (number || operate);
                    }
                });
                return formulaStr;
            }
        }
    }
})