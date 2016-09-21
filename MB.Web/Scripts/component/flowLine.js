/**
 * Created by ZETA on 2015/8/19.
 */
$(function(){
    $.fn.extend({
        flowLine: function(data){
            this.empty();

            var $ul = $("<ul class='list-unstyled chart-flowline'></ul>").appendTo(this);

            $("<li class='node-last' style='height: 40px'></li>").append(["<div class='v-line'></div>", "<img class='x32 img-circle chart-flowline-portrait' src='" + data.smallImage + "'>"]).appendTo($ul);

            $.each(data.operateInfo,function(i,v){
                $ul.find(".node-last").removeClass("node-last");
                var $li = $("<li class='chart-flowline-node node-last'></li>"),
                    $info = $("<div class='chart-flowline-info'>" + v.operateInfo + "</div>");
                if(v.operateTime){
                    switch (v.result){
                        case 1:
                            $info.addClass("node-submit");
                            break;
                        case 2:
                            $info.addClass("node-pass");
                            break;
                        case 3:
                            $info.addClass("node-back");
                            break;
                        case 4:
                            $info.addClass("node-repeal");
                            break;
                        case 5:
                            $info.addClass("node-load");
                            break;
                        case 6:
                            $info.addClass("node-check");
                            break;
                    }
                    $li.append(["<div class='chart-flowline-time'>" + v.operateTime + "</div>","<div class='v-line'></div>",$info]).appendTo($ul);
                    if(v.contents && v.contents.length){
                        var $liExtend = $("<li class='chart-flowline-node-extend'></li>").append(["<div class='v-line'></div>","<div class='chart-flowline-opinion'>" + v.contents + "</div>"]).appendTo($ul);
                        $("<a class='glyphicon glyphicon-comment chart-flowline-opinion-switch'></a>").click(function(){
                            $liExtend.slideToggle();
                        }).appendTo($info);
                    }
                }else{
                    $info.addClass("node-waiting");
                    $li.append(["<div class='v-line'></div>",$info]).appendTo($ul);
                }
            });
        }
    });
});