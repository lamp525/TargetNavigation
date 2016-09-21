/**
 * Created by ZETA on 2015/7/6.
 * 等级评价（星型）
 * 参数:{
 *   number:$int         //星星个数
 *   color:$int          //星星颜色，默认字体颜色，1：红，2：黄，3：绿
 *   clicked:function(i)   //选取后调用方法,i为选取的星星个数
 * }
 */
$(function(){
    var colors = ["","red","yellow","green"];
    $.fn.extend({
        rating: function (o) {
            $(this).each(function(){
                var $this = $(this),
                    opt = {
                        number: 5,
                        color: 0,
                        clicked: function (i) {
                            console.log(i);
                        }
                    };
                $.extend(opt, o);

                if(!$this.hasClass("rating")){
                    $this.addClass("rating");
                }
                if(opt.color){
                    $this.addClass(colors[opt.color]);
                }

                var stars = "";
                for(var i = 0; i < opt.number; i++){
                    stars += "<span class='fa fa-star-o'></span>";
                }

                $this.append(stars);

                $this.children().each(function(i){
                    $(this).click(function(){
                        if($(this).hasClass("hit")){
                            $(this).removeClass("hit");
                            if(opt.clicked){
                                opt.clicked.call($this,0);
                            }
                        }else{
                            $(".hit",$this).removeClass("hit");
                            $(this).addClass("hit");
                            if(opt.clicked){
                                opt.clicked.call($this,opt.number - i);
                            }
                        }
                    })
                });
            });
        }
    });
});
