/**
 * 计划列表 （然而并没有什么卵用）
 * 依赖于jQuery
 * 参数:
 * opt={
 *   data:[{           //数据
 *      id:$int         //计划ID
 *      ,status: $int   //计划状态    0:待提交 1:待审核 2:已审核 3:待确认 4:已完成 5:已中止
 *      ,isPremise: $boolean          //是否前提计划
 *      ,isCirculation: $boolean      //是否循环计划
 *      ,hasAccessory: $boolean       //有否附件
 *      ,hasSubplan: $boolean         //有否子计划
 *      ,isCollaboration: $boolean    //是否协作计划
 *      ,isTemporary: $boolean        //是否临时计划
 *      ,executionMode:$string        //执行方式
 *      ,output:$string               //输出结果
 *      ,startTime:$string              //计划开始时间
 *      ,endTime:$string                //计划结束时间
 *      ,responsibleUser:$string      //责任人
 *      ,confirmUser:$string          //确认人
 *      ,importance:$int              //重要度
 *      ,urgency:$int                 //紧急度
 *      ,difficulty:$int              //难易度
 *      ,progress:$int                //完成情况
 *      ,finish:{                     //是否完成    未完成则无该属性
 *          quantity:$string          //完成数量
 *          ,quality:$string          //完成质量
 *          ,time:$string             //完成时间
 *          ,actualWorking:$string    //实际工时
 *          ,effectiveWorking:$string //有效工时
 *      }
 *   },{}...]
 * }
 */

$(function(){
    $.fn.planTag = function(opt){

        var _planList = [];

        var $container = $(this);

        if($container.hasClass("planList")){
            $container.addClass("planList");
        }

        $.each(opt.data,function(){
            var cb = new CreateTab(this);
            cb.init();
            cb.render($container);
            _planList.push(cb);
        });
        return {
            setSelectable:function(){
                $.each(_planList,function(){
                    this.setSelectable();
                });
            }
        };
    }
});

function CreateTab(opt){
    this.jqdom = null;
    this.data = opt;

    //tag初始化
    this.init=function(){
        this.jqdom = $("<div></div>").addClass("PLChunk");
    };

    //渲染
    this.render=function($container){

        /* dom添加 -- 开始 */

        //标签
        var $label = $("<div></div>").addClass("label");
        var $labelOne = $("<span></span>").addClass("labelOne");
        var $labelTwo = $("<span></span>").addClass("labelTwo");
        if(this.data.isPremise || this.data.isCirculation || this.data.hasAccessory || this.data.hasSubplan || this.data.isCollaboration || this.data.isTemporary){
            $label.append($labelOne);

            if(this.data.isPremise){
                $labelTwo.append('<img src="../Images/plan/recyclePlan.png" title="前提计划">');
            }
            if(this.data.isCirculation){
                $labelTwo.append('<img src="../Images/plan/recyclePlan.png" title="循环计划">');
            }
            if(this.data.hasAccessory){
                $labelTwo.append('<img src="../Images/plan/recyclePlan.png" title="附件">');
            }
            if(this.data.hasSubplan){
                $labelTwo.append('<img src="../Images/plan/recyclePlan.png" title="子计划">');
            }
            if(this.data.isCollaboration){
                $labelTwo.append('<img src="../Images/plan/recyclePlan.png" title="协作计划">');
            }
            if(this.data.isTemporary){
                $labelTwo.append('<img src="../Images/plan/recyclePlan.png" title="临时计划">');
            }
            $label.append([$labelOne,$labelTwo]);
        }
        var $main = $("<div></div>").addClass("main");

        //环形进度条
        var $knobwrapper = $("<div></div>").addClass("knobwrapper");
        var $knob = $("<input/>").addClass("knob");

        $knob.val(this.data.progress);
        $knobwrapper.append($knob);

        //plan信息
        var $runMode = $("<div></div>").addClass("runMode");
        var $runModeUl = $("<ul></ul>");
        $runModeUl.append($("<li>执行方式：" + this.data.executionMode + "</li>"));
        $runModeUl.append($("<li>输出结果：" + this.data.output + "</li>"));
        $runModeUl.append($("<li>计划完成时间：" + this.data.endTime + "</li>"));
        $runModeUl.append($("<li>确认人：" + this.data.confirmUser + "</li>"));

        //重要度、紧急度、难易度
        //var $importance = $("<div></div>").addClass("stars starsR");
        //var $urgency = $("<div></div>").addClass("stars starsY");
        //var $difficulty = $("<div></div>").addClass("stars starsG");

        var tmp = [
            ["重要度","starsR",this.data.importance]
            ,["紧急度","starsY",this.data.urgency]
            ,["难易度","starsG",this.data.difficulty]
        ];
        $.each(tmp,function(){
            var c = this[2];

            var $starsdiv = $("<div></div>").addClass("stars " + this[1]);

            $("<ul></ul>").append(function(){
                var result = "";
                if(c){
                    for(var i = 0;i < c; i++){
                        result += '<li class="liHit"></li>';
                    }
                }else{
                    for(var i = 0;i < 5; i++){
                        result += '<li></li>';
                    }
                }

                return result;
            }).appendTo($starsdiv);

            $('<li></li>').append('<span style="float:left;">' + this[0] + '：</span>').append($starsdiv).appendTo($runModeUl);
        });

        $runMode.append($runModeUl);

        $main.append([$knobwrapper,$runMode]);

        //完成情况统计
        var finish = this.data.finish
        if(finish){
            var $accomplishment = $("<div></div>").addClass("accomplishment");
            var $arrowsBBLCom = $("<span></span>").addClass("arrowsBBLCom");
            $accomplishment.append($arrowsBBLCom);
            $accomplishment.append("<ul><li>完成数量：" + finish.quantity + "</li>" +
            "<li>完成质量：" + finish.quality + "</li>" +
            "<li>完成时间：" + finish.time + "</li>" +
            "<li>实际工时：" + finish.actualWorking + "时</li>" +
            "<li>有效工时：" + finish.effectiveWorking + "时</li></ul>");

            $main.append($accomplishment);
        }

        //选择框
        var $choose = $("<div></div>").addClass("choose");
        var $chooseSpan = $("<span></span>");
        $choose.append($chooseSpan);

        //负责人
        var $dutyPerson = $("<div></div>").addClass("dutyPerson").html(this.data.responsibleUser + " " + this.data.startTime);

        //操作条
        var $operate = $("<div></div>").addClass("operate");
        var $operateDiv = $("<div></div>").addClass("operateDiv");
        var $operateBg = $("<span></span>").addClass("operateBg");
        var $operateText = $("<div></div>").addClass("operateText");
        $operateDiv.append([$operateBg,$operateText]);

        var $submit = $('<li><img src="../Images/plan/chunk.png" /><span>提交</span></li>');
        var $ok = $('<li><img src="../Images/plan/chunk.png" /><span>确认</span></li>');
        var $pause = $('<li><img src="../Images/plan/chunk.png" /><span>中止</span></li>');
        var $modify = $('<li><img src="../Images/plan/chunk.png" /><span>修改</span></li>');
        var $transfer = $('<li><img src="../Images/plan/chunk.png" /><span>转办</span></li>');
        var $disintegrate = $('<li><img src="../Images/plan/chunk.png" /><span>分解</span></li>');
        var $cancel = $('<li><img src="../Images/plan/chunk.png" /><span>撤销</span></li>');
        var $restart = $('<li><img src="../Images/plan/chunk.png" /><span>重新开始</span></li>');
        var $pigeonhole = $('<li><img src="../Images/plan/chunk.png" /><span>归档</span></li>');
        $("<ul></ul>").append([$submit,$ok,$pause]).appendTo($operateText);
        $operate.append($operateDiv);

        this.jqdom.append([$label,$main,$choose,$dutyPerson,$operate]);

        $container.append(this.jqdom);

        /* dom添加 -- 结束 */

        /* 事件绑定 -- 开始 */

        //选择框勾选、取消
        $choose.click(function(){
            if ( $(this).hasClass('chooseHit') ) {
                $(this).removeClass('chooseHit');
                $chooseSpan.removeClass('spanHit');
            } else {
                $(this).addClass('chooseHit');
                $chooseSpan.addClass('spanHit');
            }
        });

        //选择卡片列表移上去出现绿条
        this.jqdom.hover( function () {
            $operate.show();
        } , function () {
            $operate.hide();
        });

        /* 事件绑定 -- 开始 */

        /* 外部组件渲染 -- 开始 */

        //环形进度条
        $knob.knob({
            width:80,
            max: 100,
            min: 0,
            thickness:.3,
            fgColor: com.ztnc.targetnavigation.unit.planStatusColor[this.data.status],
            bgColor: '#e4e4e4',
            inputColor: '#686868',
            displayPrevious:true
        });

        /* 外部组件渲染 -- 结束 */

        /* 外部可调用方法 -- 开始 */

        //给出进入可选择模式的方法
        this.setSelectable = function(){
            $choose.show();
        };

        /* 外部可调用方法 -- 结束 */
    };

    //绑定事件
    this.bindEvent = function(){

    };

    //销毁
    this.destroy = function(){

    };
}