var discusstext = "";
$(function () {
    $('.xubox_layer').css({ 'margin-left': '-481px', 'width': '962px' });
    //alert();
    if ($('.commentBox').css('display') == 'none') {
        $(this).css({ 'color': '#fff', 'background-color': '#fbac0f' });
    } else {
        $(this).css({ 'color': '#686868', 'background-color': '#ececec' });
    }
    //$('.commentBox').css({'height':parseInt($('#checksPopUp').css('height')) , 'overflow-y':'scroll'});
    //alert(parseInt($("#discussheight").val()));
    //$('.commentBox').css({ 'height': parseInt($("#discussheight").val()) });
    //alert($('.commentBox').height());
    $('.commentBox').toggle();
    fnComment();
    function fnComment() {
        commentDivLength = 0;
        fnCommentDiv();
        function fnCommentDiv() {
            commentDivLength = 0;
            $('.commentDiv').each(function (index, element) {
                var height = $('.discuss', this).height();
                var replyHeight = $('.replyDiv', this).height();
                $('.discuss', this).height(height);
                $(this).height(height + 20 + replyHeight);
                if ($(this).index() == 0) {
                    $(this).css({ 'margin-top': '95px' });
                }
                if (height != 30) {
                    //$('.botComent', this).css({ 'margin-left': '50px' });
                }
                //commentDivLength += $(this).height();
                //alert(commentDivLength);
            });
        }
        fncommentBox();
        function fncommentBox() {
            //alert();
            $('.botComent .accessory').unbind("click");
            $('.commentDiv .reply').unbind("click");
            $('.replyDiv textarea').unbind("keydown");
            $('.replyHandle span').unbind("click");
            $('.botComent .accessory').click(function () {
                if ($(this).find('.accessoryDiv').css('display') == 'none') {
                    $(this).parents('.commentList').find('.accessoryDiv').hide();
                    $(this).find('.accessoryDiv').show();
                    var width = parseInt($(this).find('.accessoryDiv').css('width'));
                    var height = parseInt($(this).find('.accessoryDiv').css('height'));
                    $(this).find('.labelDivBg').css({ 'width': width, 'height': height });
                } else {
                    $(this).find('.accessoryDiv').hide();
                }
            });
            $('.commentDiv .reply').click(function (index, element) {

                replyclick($(this));
            });
            $('.replyDiv textarea').keydown(function () {
                var row = parseInt($(this).val().length / 26) + 1;
                $(this).height(26 * row);
                $(this).parent('.replyDiv').height('auto');
                fnCommentDiv();
                if ($(this).parent().hasClass('commentUpDiv')) {
                    var marginTop = 70 + parseInt($(this).height());
                    //alert(marginTop);
                    $('.commentDiv:eq(0)').css({ 'margin-top': marginTop });
                }
            });
            //$('.commentUpDiv textarea').keydown(function () {
            //    var row = parseInt($(this).val().length / 26) + 1;
            //    $(this).height(26 * row);
            //    $(this).parent('.replyDiv').height('auto');
                
            //    //alert($(this).height());
            //    var marginTop = 70 + parseInt($(this).height());
            //    //alert(marginTop);
            //    $('.commentDiv:eq(0)').css({ 'margin-top': marginTop });
            //    //alert($('.commentDiv:eq(0)').css('margin-top'));
            //});
            $('.replyHandle span').click(function () {
                //alert($(this).length);
                //alert($(this).html());
                //if ($(this).index() == 0) {
                //    $(this).parents('.replyDiv').css({ 'height': '0px', 'min-height': '0px' }).hide();
                //    fnCommentDiv();
                //    var html = '<li class="commentDiv">' +
                //                   '<img class="portraitComment" src="../../Images/common/portrait.png" width="32" height="32" />' +
                //                   '<div class="discuss">' +
                //                       '<span style="color:#58b456">大刘：</span>' +
                //                       '<span class="discussCon">电视剧撒谎发生佛苟富贵复哈哈发顺丰</span>' +
                //                   '</div>' +
                //                   '<div class="botComent">' +
                //                       '<span class="time">12:25</span>' +
                //                       '<div class="accessory">' +
                //                           '<div class="accessoryDiv">' +
                //                               '<span class="accBg"></span>' +
                //                               '<span class="accText">双方就大煞风景撒谎发货</span>' +
                //                           '</div>' +
                //                       '</div>' +
                //                       '<span class="reply">回复</span>' +
                //                   '</div>' +
                //                   '<div class="replyDiv">' +
                //                       '<img class="portraitComment" src="../../Images/common/portrait.png" width="32" height="32" />' +
                //                       '<textarea maxlength="100">回复大刘：</textarea>' +
                //                       '<div class="replyHandle">' +
                //                           '<span style="margin-left:10px;">评论</span>' +
                //                           '<span>附件</span>' +
                //                       '</div>' +
                //                   '</div>' +
                //               '</li>';
                //    $(this).parents('.commentDiv').after(html);
                //    fncommentBox();
                //}
            });
        }
    }
    function replyclick(obj)
    {
        discusstext = obj.parents('.commentDiv').find('.replyDiv textarea').val();
        var replyDiv = obj.parent('.botComent').next('.replyDiv');
        if (replyDiv.css('display') == 'none') {
            replyDiv.css({ 'height': 'auto', 'min-height': '90px' });
        } else {
            replyDiv.css({ 'height': '0px', 'min-height': '0px' });
        }
        replyDiv.toggle();
        //fnCommentDiv();
        if (commentDivLength > ($('.commentBox').height() - 30)) {
            $('.commentBox').css({ 'overflow': 'hidden', 'overflow-y': 'scroll' });
        }
    }
    $("#xxc_suggestion").off('click');
    $("#xxc_suggestion").click(function () {
        var userId = $('#xxc_userDiscuss').attr('term');
        if ($(this).parents('.replyDiv').find('textarea').val() == '') {
            validate_reject('评论不能为空', $('.commentUpDiv textarea'));
        } else {
            var content = $(this).parents('.replyDiv').find('textarea').val();
            var planId = $("#xxc_planId").val();
            $.ajax({
                type: "post",
                url: "/plan/AddDiscuss",
                dataType: "json",
                data: { content: content, planId: planId },
                success: rsHandler(function (data) {
                    if (data) {
                        if (data.createUser==userId) {
                            var dishtml = " <li class='commentDiv' term='" + data.suggestionId + "'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                      + "<div class='discuss'><span term='" + data.createUser + "' style='color: #58b456'>" + data.createUserName + "</span><span>：</span>"
                      + "<span class='discussCon'>" + data.suggestion + "</span></div>"
                      + "<div class='botComent'><span class='time'>" + data.NewCreateTime + "</span><span class='reply'>回复</span><span class='delete'>删除</span> </div>"
                      + "<div class='replyDiv'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                          + "<textarea maxlength='100'>回复" + data.createUserName + "：</textarea><div class='replyHandle'>"
                              + "<span style='margin-left: 10px;'class='xxc_discuss'>评论</span><span style='display:none'>附件</span></div></div></li>";
                        }
                        else {
                            var dishtml = " <li class='commentDiv' term='" + data.suggestionId + "'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                      + "<div class='discuss'><span term='" + data.createUser + "' style='color: #58b456'>" + data.createUserName + "</span><span>：</span>"
                      + "<span class='discussCon'>" + data.suggestion + "</span></div>"
                      + "<div class='botComent'><span class='time'>" + data.NewCreateTime + "</span><span class='reply'>回复</span> </div>"
                      + "<div class='replyDiv'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                          + "<textarea maxlength='100'>回复" + data.createUserName + "：</textarea><div class='replyHandle'>"
                              + "<span style='margin-left: 10px;'class='xxc_discuss'>评论</span><span style='display:none'>附件</span></div></div></li>";
                        }
                       
                        $(".commentBox .commentList").append(dishtml);
                        //fnComment();
                        //alert('dnfbsd' + $('.commentUpDiv').height());

                        $('.commentDiv').each(function (index, element) {
                            //alert();
                            var height = $('.discuss', this).height();
                            var replyHeight = $('.replyDiv', this).height();
                            $('.discuss', this).height(height);
                            $(this).height(height + 20 + replyHeight);
                            if ($(this).index() == 0) {
                                //alert($('.commentUpDiv textarea').height());
                                //var marginTop = 70 + parseInt($('.commentUpDiv textarea').height());
                                $(this).css({ 'margin-top': '95px' });
                            }
                            //alert($(this).height());
                            //alert(height);
                            if (height != 30) {
                                $('.botComent', this).css({ 'margin-left': '50px' });
                            }
                            commentDivLength += $(this).height();
                            //alert(commentDivLength);
                        });
                        $(".reply").off('click');
                        $(".reply").click(function () {

                            $(".reply").click(replyclick($(this)));
                        });
                        $(".xxc_discuss").off('click');
                        $(".xxc_discuss").click(function () {
                            replydiscuss($(this));
                        });
                        $('.botComent .delete').off('click');
                        $('.botComent .delete').click(function () {
                            deletecomment($(this));
                        });
                        $(".commentDiv textarea").focus(function () {
                            texterafocus($(this));
                        });
                        $(".commentDiv textarea").blur(function () {
                            texterablur($(this));
                        });
                    }
                })
            });
            $(this).parents('.replyDiv').find('textarea').val('');
            $(this).parents('.replyDiv').find('textarea').width('304').height('26');
        }
    });

    $('.xxc_discuss').off('click')
    $('.xxc_discuss').click(function () {
        replydiscuss($(this));
    });

    /* 删除 开始 */
    $('.botComent .delete').click(function () {
        deletecomment($(this));
    });

    function deletecomment(obj)
    {
        var suggestionId = obj.parents('.commentDiv').attr('term');
        obj.addClass('deleteHit');
        ncUnits.confirm({
            title: '提示',
            html: '你确定要删除这条评论吗？',
            yes: function (layer_delete) {
                layer.close(layer_delete);
                $.post("/plan/DeleteComment", { suggestionId: suggestionId }, function (data) {
                    if (data=="ok") {
                        $('.deleteHit').parent().parent('.commentDiv').remove();
                        ncUnits.alert("已成功删除评论");
                        $('.commentList .commentDiv:eq(0)').css('margin-top', '96px');
                    }
                    else {
                        ncUnits.alert("删除评论失败");
                    }
                });
                
            },
            no: function (layer_delete) {
                layer.close(layer_delete);
                $('.deleteHit').removeClass('deleteHit');
            }
        });
    }
    /* 删除 结束 */
    

    $(".commentDiv").find("textarea").focus(function () {
        texterafocus($(this));
    }).blur(function () {
        texterablur($(this));
    });

    //输入框得到焦点清空
    function texterafocus(obj) {
        discusstext = obj.val();
        if (discusstext != "") {
            obj.val('');
        }
    }
    //输入框失去焦点还原
    function texterablur(obj) {
        if (obj.val() == "") {
            obj.val(discusstext);
        }
    }

    function replydiscuss(obj) {
        var userId = $('#xxc_userDiscuss').attr('term');
        if ($(obj).parents('.replyDiv').find('textarea').val() == "" || $(obj).parents('.replyDiv').find('textarea').val() == discusstext) {
            validate_reject('评论不能为空', $(obj).parents('.replyDiv').find('textarea'));
        } else {
            discusstext = "";
            var replyUser = parseInt(obj.parents('.commentDiv').find('.discuss span:eq(0)').attr('term'));
            var replyUserName = obj.parents('.commentDiv').find('.discuss span:eq(0)').text();
            var planId = $("#xxc_planId").val();
            var content = obj.parents('.replyDiv').find('textarea').val();
            $.ajax({
                type: "post",
                url: "/plan/AddReplySuggestion",
                dataType: "json",
                data: { planId: planId, replyUser: replyUser, replyUserName: replyUserName, content: content },
                success: rsHandler(function (data) {
                    if (data) {
                        if (data.createUser==userId) {
                            var dishtml = " <li class='commentDiv' term='" + data.suggestionId + "'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                       + "<div class='discuss'> <span term='" + data.createUser + "' style='color: #58b456'>" + data.createUserName + "</span><span>回复</span><span style='color: #58b456'>" + data.replyUserName + "：</span>"
                       + "<span class='discussCon'>" + data.suggestion + "</span></div>"
                       + "<div class='botComent'><span class='time'>" + data.NewCreateTime + "</span><span class='reply'>回复</span><span class='delete'>删除</span> </div>"
                       + "<div class='replyDiv'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                           + "<textarea maxlength='100'>回复" + data.createUserName + "：</textarea><div class='replyHandle'>"
                               + "<span style='margin-left: 10px;' class='xxc_discuss'>评论</span><span style='display:none'>附件</span></div></div></li>";
                        }
                        else {
                            var dishtml = " <li class='commentDiv' term='" + data.suggestionId + "'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                       + "<div class='discuss'> <span term='" + data.createUser + "' style='color: #58b456'>" + data.createUserName + "</span><span>回复</span><span style='color: #58b456'>" + data.replyUserName + "：</span>"
                       + "<span class='discussCon'>" + data.suggestion + "</span></div>"
                       + "<div class='botComent'><span class='time'>" + data.NewCreateTime + "</span><span class='reply'>回复</span></div>"
                       + "<div class='replyDiv'><img class='portraitComment' src='" + data.img + "' width='32' height='32' />"
                           + "<textarea maxlength='100'>回复" + data.createUserName + "：</textarea><div class='replyHandle'>"
                               + "<span style='margin-left: 10px;' class='xxc_discuss'>评论</span><span style='display:none'>附件</span></div></div></li>";
                        }
                        $(".commentBox .commentList").append(dishtml);
                        //alert($('.commentBox .replyDiv:eq(0)').siblings('.replyDiv').html());
                        //$('.commentBox .replyDiv:eq(0)').siblings('.replyDiv').remove();
                        //alert($('.commentBox .replyDiv').length);
                        $('.commentBox .replyDiv').each(function () {
                            if ($(this).index != 0) {
                                $(this).css({ 'height': '0px', 'min-height': '0px' });
                            }
                        });
                        $('.replyDiv textarea').keydown(function () {
                            var row = parseInt($(this).val().length / 26) + 1;
                            $(this).height(26 * row);
                            $(this).parent('.replyDiv').height('auto');
                            if ($(this).parent().hasClass('commentUpDiv')) {
                                var marginTop = 70 + parseInt($(this).height());
                                //alert(marginTop);
                                $('.commentDiv:eq(0)').css({ 'margin-top': marginTop });
                            }
                            //fnCommentDiv();
                        });
                        //$('.replyDiv textarea').keydown(function () {
                        //    var row = parseInt($(this).val().length / 26) + 1;
                        //    $(this).height(26 * row);
                        //    $(this).parent('.replyDiv').height('auto');
                        //    //fnCommentDiv();
                        //});
                        $('.commentDiv').each(function (index, element) {
                            //alert();
                            var height = $('.discuss', this).height();
                            var replyHeight = $('.replyDiv', this).height();
                            $('.discuss', this).height(height);
                            $(this).height(height + 20 + replyHeight);
                            if ($(this).index() == 0) {
                                //alert($('.commentUpDiv textarea').height());
                                //var marginTop = 70 + parseInt($('.commentUpDiv textarea').height());
                                $(this).css({ 'margin-top': '95px' });
                            }
                            //alert($(this).height());
                            //alert(height);
                            //alert()
                            if (height != 30) {
                                $('.botComent', this).css({ 'margin-left': '50px' });
                            }
                            commentDivLength += $(this).height();
                            //alert(commentDivLength);
                        });
                        $('.botComent', this).css({ 'margin-left': '50px' });
                        $(".reply").off('click');
                        $(".reply").click(function () {
                            $(".reply").click(replyclick($(this)));
                        });
                        $(".xxc_discuss").off('click');
                        $(".xxc_discuss").click(function () {
                            replydiscuss($(this));
                        });

                        $('.botComent .delete').off('click');
                        $('.botComent .delete').click(function () {
                            deletecomment($(this));
                        });
                        $(".commentDiv textarea").focus(function () {
                            texterafocus($(this));
                        });
                        $(".commentDiv textarea").blur(function () {
                            texterablur($(this));
                        });
                    }
                })
            });
        }
        $(obj).parents('.replyDiv').css("display", "none");
        $(obj).parents('.commentDiv').css("height", "50px");
        $(obj).parents('.replyDiv').find('textarea').val('');
    }
});