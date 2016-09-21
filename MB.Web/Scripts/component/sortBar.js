/**
 * Created by ZETA on 2015/9/8.
 */
$(function () {
    $.fn.extend({
        sortBar: function (o) {
            $(this).each(function () {
                var $this = $(this);

                $this.children().each(function (i) {
                    var $$this = $(this);

                    $(this).click(function () {
                        $(this).addClass("active").siblings(".active").removeClass("active");
                    });

                    $(".sort-icon", $$this).children().each(function (j) {
                        j = j == 0 ? 1 : 0;
                        $(this).data("sort", j);
                        //icon点击触发
                        $(this).click(function () {
                            $(".sort-icon .active", $this).removeClass("active");
                            $(this).addClass("active");
                            clicked(i + 1, j);
                        });
                    });

                    //orderby点击触发
                    $(".orderby", $$this).click(function () {
                        var $activing = $(".sort-icon [class^='icon']:not('.active'):first", $$this);
                        $("[class^='icon'].active", $this).removeClass("active");
                        $activing.addClass("active");
                        clicked(i + 1, $activing.data("sort"));
                    })
                });
                function clicked(sort, order) {
                    $this.trigger("clicked.sortbar", [sort, order])
                }
            });
        }
    });
});