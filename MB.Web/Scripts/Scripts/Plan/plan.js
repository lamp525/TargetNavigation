//@ sourceURL=plan.js
/**
 * Created by ZETA on 2016/1/26.
 */
define(["bootstrap-hover"], function () {
    var Plan = function(){
        var _this = this;
        this.$con = $('#modal_plan');
        this.isMine = true;
        this.isLoop = false;
        this.filter = {
            userId:0,
            page: 0,
            group: 0,
            groupValue: 0,
            sortInfo: [{
                sortType: 1,
                orderType : 1
            }],
            filterInfo: {
                fast: 1,
                status: [],
                fromTime: "",
                toTime: "",
                userIds: ""
            },
            lastTime: ""
        };

        //快速建计划
        this.shortcutPlan = (function(){
            var $con = $("#shortcut_plan"),
                $date = $con.find(".shortcut-time"),
                $output = $con.find("input:text"),
                today = laydate.now();

            $date.html(today).attr("time",today).click(function(){
                laydate({
                    isclear: false,
                    choose:function(dates){
                        $date.attr("time",dates);
                    }
                })
            });

            $con.find(".shortcut-btn").click(addPlan);

            function addPlan(){
                var url = "/XXXViews/Plan/QuickAddPlan",
               data={ endTime :$date.attr("time"),
                   eventOutput:$output.val()};
                post(url,function(){
                    ncUnits.alert("添加成功");
                    $output.val("");
                },{
                    data:JSON.stringify(data)
                });
            }

            return {
                addplan: addPlan
            }
        })();

        //左侧导航条
        this.nav = (function(){
            var $con = $(".side-nav"),
                $myNav = $("#nav_myplan"),
                $subNav = $("#nav_subplan"),
                $mySubmiting = $("#nav_myplan_submiting"),
                $myChecking = $("#nav_myplan_checking"),
                $myChecked = $("#nav_myplan_checked"),
                $myConfirming = $("#nav_myplan_confirming"),
                $subSubmiting = $("#nav_subplan_submiting"),
                $subChecking = $("#nav_subplan_checking"),
                $subChecked = $("#nav_subplan_checked"),
                $subConfirming = $("#nav_subplan_confirming");

            //导航条点击事件
            $con.find("li").click(function(e){
                e.stopPropagation();
                var $this = $(this);
                if($this.hasClass("has-children")){
                    $this.toggleClass("active").children(".open-child").slideToggle();
                }else{
                    $con.find(".selected").removeClass("selected");
                    $this.addClass("selected");

                    _this.filter.filterInfo.status[0] = $this.attr("planStatus");
                    if($this.parents("#nav_myplan").length){
                        _this.isMine = true;
                    }else if($this.parents("#nav_subplan").length){
                        _this.isMine = false;
                    }

                    _this.refreshPlanList();
                }
            });

            refreshPlanCount();

            //计划数量刷新
            function refreshPlanCount(){
                //我的计划
                post("/XXXViews/Plan/GetMyPlanStatusInfo", function (data) {
                    var submitingCount = data.planSubmitingCount + data.loopSubmitingCount,
                        checkingCount = data.planCheckingCount + data.loopCheckingCount,
                        checkedCount = data.planCheckedCount + data.loopCheckedCount,
                        confirmingCount = data.planConfirmingCount + data.loopConfirmingCount;

                    $mySubmiting.find(".label").html(submitingCount);
                    $myChecking.find(".label").html(checkingCount);
                    $myChecked.find(".label").html(checkedCount);
                    $myConfirming.find(".label").html(confirmingCount);
                });
                //下属计划
                post("/XXXViews/Plan/GetSubordinatePlanStatusInfo", function (data) {
                    $subSubmiting.find(".label").html(data.submitingCount);
                    $subChecking.find(".label").html(data.checkingCount);
                    $subChecked.find(".label").html(data.checkedCount);
                    $subConfirming.find(".label").html(data.confirmingCount);
                });
            }

            return {
                refreshPlanCount: refreshPlanCount
            }
        })();

        //排序条
        this.sortbar = (function(){
            _this.$con.find(".sortbar").on("clicked.sortbar",function(e,sort,order){
                console.log($(this));
                _this.filter.sortInfo[0].sortType = sort;
                _this.filter.sortInfo[0].orderType = order ? 1 : 2;

                _this.refreshPlanList();
            }).sortBar();

            return {}
        })();

        this.events = {
            'click .side-stretchController': this.stretch,
            'click .main-container .nav-tabs li': this.changePlanType,
            'click #groupMode>li': this.group
        };
    };
    Plan.prototype = {
        //右侧伸缩
        stretch: function(e){
            $(".side-right").toggleClass("unfolded");
        },
        //一般计划\循环计划切换
        changePlanType: function(e){
            var $this = $(e.currentTarget);
            this.isLoop = ($this.attr("isLoop") == "true");
            this.refreshPlanList();
        },
        //分组
        group: function(e){
            var $this = $(e.currentTarget);
            this.filter.group = $this.attr("group");
            this.refreshPlanList();
        },
        //刷新计划列表
        refreshPlanList: function(){
            if (this.isLoop) {
                var planUrl = this.isMine ? "/XXXViews/Plan/GetMyLoopPlanList" : "/XXXViews/Plan/GetSubordinateLoopPlanList",
                    groupUrl = this.isMine ? "/XXXViews/Plan/GetMyLoopPlanGourpInfo" : "/XXXViews/Plan/GetSubordinateLoopPlanGourpInfo",
                    $con = $("#loopPlanBox");
            } else {
                var planUrl = this.isMine ? "/XXXViews/Plan/GetMyPlanList" : "/XXXViews/Plan/GetSubordinatePlanList",
                    groupUrl = this.isMine ? "/XXXViews/Plan/GetMyPlanGourpInfo" : "/XXXViews/Plan/GetSubordinatePlanGourpInfo",
                    $con = $("#ordinaryPlanBox");
            }
            if(this.filter.group){
                post(groupUrl,function(data){
                    tpl("tpl_groupplanbox",data,$con);
                    $.each(data,function(i,v){
                        post(planUrl, function (data) {
                            tpl("tpl_planbox",data,$("[groupId='" + v.groupId + "'] .group-content"));
                        }, $.extend({
                            groupValue: v.groupId
                        }, { data:JSON.stringify(this.filter) }));
                    });
                }, { data: JSON.stringify(this.filter) });
            }else{
                post(planUrl, function (data) {
                    tpl("tpl_planbox",data,$con);
                }, { data: JSON.stringify(this.filter) });
            }
        },
        _bind: function () {
            var events = this.events;
            if (!events) {
                return;
            }
            //正则拆分事件字符串
            var delegateEventSplitter = /^(\S+)\s*(.*)$/;
            var k, method, eventName, selector, match;
            for (k in events) {
                match = k.match(delegateEventSplitter);
                eventName = match[1];
                selector = match[2];
                method = events[k].bind(this);
                this.$con.on(eventName, selector, method);
            }
        },
        render:function(){
            this._bind();
        }
    };
    function post(url,callback,param) {
        $.ajax({
            url: url,
            type: "post",
            dataType: "json",
            data: param || {},
            success: rsHandler(callback)
        })
    }
    function tpl(tplid,data,$container){
        $container.html(template(tplid, data));
    }

    return Plan;
});