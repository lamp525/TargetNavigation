/**
 * Created by ZETA on 2015/6/4.
 */
$(function(){
    $.fn.extend({
        /**
         * 选择弹出框
         * @param o
         * {
         *   url : string,
         *   defText : string,
         *   hasImage : boolean,
         *   selectHandle : function
         * }
         */
        searchPopup: searchPopup,
        searchStaffPopup: searchPopup
    });
    function searchPopup(o){
        var $this = $(this),
            timer,
            opt = {
                url: "",
                hasImage: false,
                defText: "",
                selectHandle: function (data) {

                }
            };
        $.extend(opt, o);

        $(this).click(function (e) {
            e.stopPropagation();
            $(".optionCell-select").hide();
            if ($(this).next().hasClass("optionCell-select")) {
                $(this).next().show();
                var $input = $(this).next().find("input");
                $input.focus();
                $input.val("");
                $input.keyup();
            } else {
                var $con = $("<div style='position: absolute;left: 0;margin-top: -1px;border: solid 1px #ccc;z-index: 1;background-color: #fff;'></div>").addClass("optionCell-select");
                var $input = $("<input type='text' style='width: :100%;padding: 2px 5px'/>");

                var $span = $("<span></span>").css({"line-height":"normal","margin-left": "10px"});
                var $imitateSelect = $("<div style='max-height: 170px;overflow: auto;'></div>").addClass("imitateSelect-img");

                $con.append([$("<div style='margin: 10px'></div>").append($input), $span, $imitateSelect]);

                $(this).after($con);

                $con.click(function (e) {
                    e.stopPropagation();
                });
                $(document).click(function () {
                    $con.hide();
                });

                searchStaff();
                $input.focus();
                $input.keydown(function(){
                    timer = Date.now() + 500;
                });
                $input.keyup(function(){
                    setTimeout(function(){
                        if(Date.now() >= timer){
                            searchStaff();
                        }
                    },500);
                });

                //按返回数据更新选项
                function searchStaff() {
                    $imitateSelect.empty();
                    //var loading_layer_index = getLoadingPosition($imitateSelect);
                    var searchVal = $input.val();
                    $.ajax({
                        type: "post",
                        url: opt.url,
                        dataType: "json",
                        data: {
                            hasimg: opt.hasImage,
                            text: searchVal
                        },
                        success: rsHandler(function (data) {
                            if (searchVal == "" && opt.defText != "") {
                                $span.html(opt.defText).show();
                            }else{
                                $span.hide();
                            }
                            var $ul = $("<ul style='padding-left: 0;list-style: none'></ul>");
                            if (!data || data.length == 0) {
                                $imitateSelect.html("<div style='line-height: 40px;text-align: center;'> ------ 没有数据 ------ </div>");
                            } else {
                                for (var i = 0, len = data.length; i < len; i++) {
                                    var $li = $("<li style='padding: 2px 0 2px 10px;cursor: pointer'></li>");
                                    if (opt.hasImage) {
                                        var $img = $("<img class='img-thumbnail img-circle x32' style='margin-right: 5px'>");
                                        if (data[i].img) {
                                            $img.attr("src",data[i].img);
                                        }else{
                                            $img.attr("src","../../Images/common/portrait.png");
                                        }
                                        $li.append([$img, data[i].name]);
                                    } else {
                                        $li.append(data[i].name);
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
                                    $li.click(i, function (e) {
                                        opt.selectHandle.call($this, data[e.data]);
                                        $con.hide();
                                    });
                                    $ul.append($li);
                                }
                                $imitateSelect.html($ul);
                            }
                        }),
                        complete: function () {
                            //layer.close(loading_layer_index);
                        }
                    });
                }
            }
        })
    }
});