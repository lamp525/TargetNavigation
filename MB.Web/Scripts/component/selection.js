/**
 * Created by ZETA on 2015/6/24.
 *
 * 选择弹出框
 * @param o
 * {
 *   url : string,
 *   hasImage : boolean,
 *   otherParam : object
 *   selectHandle : function
 * }
 * 数据格式：{name（必须）,affiliationName,img...}
 */
$(function(){
    $.fn.extend({
        selection: function (o) {
            var $this = $(this),
                timer,
                opt = {
                    url: "",
                    hasImage: false,
                    otherParam: {},
                    selectHandle: function (data) {
                        console.log(data);
                    }
                };
            $.extend(opt, o);

            var $ul = $("<ul style='position: absolute;display: none;width: 100%;max-height: 210px;padding-left: 0;margin-top: 11px;border: 1px solid #c3c3c3;background-color: #fff;overflow: auto;z-index: 3'></ul>")
            $this.wrap("<div class='pos-r .clearfix'></div>");
            $this.after($ul);

            if($this.hasClass("input-group")){
                var $input = $("input",$this),
                    $btn = $(".input-group-btn",$this);


                $input.keydown(function(){
                    timer = Date.now() + 500;
                });
                $input.keyup(function(){
                    setTimeout(function(){
                        if(Date.now() >= timer){
                            search();
                        }
                    },500);
                });

                $input.click(function(){
                    if($input.val()){
                        $ul.show();
                    }
                });
                $btn.click(search)
            }

            $this.parent().click(function (e) {
                e.stopPropagation();
            });
            $(document).click(function () {
                $ul.hide();
            });

            function search(){
                var val = $input.val();
                $ul.empty();
                if(val.length){
                    $ul.show();
                    $.ajax({
                        url: opt.url,
                        type: "post",
                        dataType: "json",
                        data: $.extend({
                            hasImage: opt.hasImage,
                            text: val
                        },opt.otherParam),
                        success: rsHandler(function (data) {
                            if (data.length == 0) {
                                var $li = $("<li  style='padding: 2px 0 2px 10px;cursor: pointer'>暂无数据</li>")
                                $ul.append($li);
                                return
                            }
                            $.each(data,function(i,v){
                                var $li = $("<li  style='padding: 2px 0 2px 10px;cursor: pointer'></li>"),
                                    affiliationName = v.affiliationName;
                                $ul.append($li);

                                if (opt.hasImage) {
                                    var $img = $("<img class='img-thumbnail img-circle x32' style='margin-right: 5px;'>");
                                    if (v.img) {
                                        $img.attr("src",v.img);
                                    }else{
                                        $img.attr("src","../../Images/common/portrait.png");
                                    }
                                    $li.append([$img, v.name + (affiliationName &&  affiliationName.length ? " — " + affiliationName : "")]);
                                } else {
                                    $li.append(v.name + (affiliationName &&  affiliationName.length ? " — " + affiliationName : ""));
                                }
                                $li.hover(function(){
                                    $(this).css({
                                        "background-color" : "#e0e0e0"
                                    });
                                },function(){
                                    $(this).css({
                                        "background-color" : "#fff"
                                    });
                                });
                                $li.click(function () {
                                    opt.selectHandle.call($this, v);
                                    $ul.hide();
                                });
                            })
                        })
                    });
                }else{
                    $ul.hide()
                }
            }
            return {
                reInit: function(o){
                    $.extend(opt, o);
                }
            }
        }
    });
});