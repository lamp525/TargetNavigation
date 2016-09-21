define(function () {
    $(document).on('click', function () {
        $('.dropList').hide();
        $('#list-persons').html('');
        $('#select-person').val('');


    })
    //选人
    var Select = function (ele) {
     
        this.id;
        this.ids = [];
        var listNone = '<li class="choose-none" style="position:relative;padding:1px 9px;0;height:35px;margin-top:10px;text-align:center;">---暂无数据---</li>';
        var drop = '<div class="dropList" style="display:none;width:165px;min-height:50px;position:absolute;left:0px;border:1px solid #bfbfbf;">\
                        <div class="drop-input" style="margin-top:10px;"><input type="text" id="select-person"></div>\
                        <ul style="margin:0;padding:0 0 10px;max-height:90px;overflow-y:auto" id="list-persons"></ul>\
                    </div>';
        var html = '<div id="appendList" style="display:inline-block;height:30px;width:30px;position:relative;line-height:30px;text-align:center;border: 1px solid #D9D9D9;"><i class="fa fa-plus" id="add-attend" style="font-size:20px;color:#B8B8B8"></i>' + drop + '</div>';
        var el = $(ele);
        el.off('click', '#appendList,.dropList,.choose-list');
        el.on('click', '#appendList,.dropList,.choose-list', this.appendList.bind(this));
        el.off('input', '#select-person');
        el.on('input', '#select-person',this.selectPerson.bind(this));
        this.init = function (id) {
            if ($('#add-attend').length > 0) {
                return;
            }
            this.id = $('#' + id);
            this.id.append(html);
           

        };
        this.getIds = function () {
            return this.ids;
        }
        this.getData = function (searchText) {
            $.post("/BuildNewPlan/GetUserByUserId", { text: searchText, hasimg: true }, function (data) {
                var result = data.data;
                var list = '';
                if (result.length == 0) {
                    list = listNone;
                } else {
                    $.each(result, function (n, val) {
                        list += '<li pid=' + val.userId + ' class="choose-list" style="position:relative;padding:1px 9px;0;height:35px;margin-top:10px;"><img class="img-thumbnail img-circle" src=' + val.img + ' style="width:35px;height:35px;position:absolute;top:0;left:9px"><p style="text-align:left;margin: 3px 0 0 40px;">' + val.userName + '</p></li>';

                    });
                }

                $('#list-persons').html(list)

            }, 'json')
        }
    }
    Select.prototype = {
        chooseList: function (eles) {
            var pid = eles.attr('pid');
            var name = eles.find('p').text();
            $('#list-persons').html('');
            $('#select-person').val('');
            $('.dropList').hide();
            if (this.ids.indexOf(pid) == 0) {
                
                return
            }
            this.ids.push(pid);
            var nameList = '<div class="namelist" style="display:inline-block;line-height:30px;padding:0 5px;">' + name + '<i class="fa fa-remove" style="margin-left:10px;"></i></div>';
            $(nameList).insertBefore('#appendList');

        },
        appendList: function (e) {
            e.stopPropagation();
            var eles = $(e.currentTarget);
            if (eles.hasClass('choose-list')) {
                this.chooseList(eles);
                return;
            }
            if (eles.hasClass('dropList')) {               
                return;
            }
            $('.dropList').toggle();
            if (!$('.dropList').is(':hidden')) {
                this.getData('');
            }
        },
        selectPerson: function (e) {
            e.stopPropagation();
            var eles = $(e.currentTarget);
            var searchText = eles.val();
     
            this.getData(searchText);

        }

    }

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
                    console.log(data);
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

    return {
        select:Select
    }
})