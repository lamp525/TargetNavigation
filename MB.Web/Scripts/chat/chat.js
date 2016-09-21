/**
 * Created by DELL on 2015/8/5.
 */
$(function () {

    //获取群组个人数量
    function getTotal() {
        $.ajax({
            type: "post",
            url: "/IMMessage/GetTypeCount",
            dataType: "json",
            success: rsHandler(function (data) {
                $('.person').eq(0).find('.last').html(data.contactCount);
                $('.person').eq(1).find('.last').html(data.groupCount);
                $('.person').eq(2).find('.last').html(data.userCount);
                $('.person').eq(3).find('.last').html(data.conversationCount);
            })
        });

    }
    getTotal();

    //最小化chat消息提示

    function msgHint() {
        if (messageList.length > 0) {
            $('.msg span').addClass('listenNew');
        } else {
            $('.msg span').removeClass('listenNew');
        }

    }




    // 连接IM服务器开始

    var user = {
        userId: null,
        userName: null,
        headImage: null
    };

    var sendData = {
        type: null,
        groupId: null,
        sendUser: user,
        receiveUser: [user],
        messageId: null,
        message: null,
        fileName:null,
        sendTime: null
    };
    var messageList = [];
    var ws;
    var url = $("#imHost").html() + $("#userId").html();
    if ("WebSocket" in window) {
        ws = new WebSocket(url);
    }
    else if ("MozWebSocket" in window) {
        ws = new MozWebSocket(url);
    }
    else
        ncUnits.alert("浏览器版本过低，请升级您的浏览器。\r\n浏览器要求：IE10+/Chrome14+/FireFox7+/Opera11+");

    // 注册各类回调
    ws.onopen = function () {
        //ncUnits.alert("连接服务器成功");
        $('.banCheck').addClass('checkGreen');
    }

    ws.onclose = function () {
        //ncUnits.alert("与服务器断开连接");
        $('.banCheck').removeClass('checkGreen');
    }
    ws.onerror = function () {
        ncUnits.alert("数据传输发生错误");
    }

    function getMessage()
    {
        //服务连接成功后，查询登录用户有无未读消息
        $.ajax({
            type: "post",
            url: "/IMMessage/GetMessage",
            dataType: "json",
            success: rsHandler(function (data) {
            })
        });
    }

    ws.onmessage = function (receiveMsg) {
        console.log("接收到消息了");

        if (receiveData.type != 1 && receiveData.type != 2) {
            getMessage();

            if (toTalk == null || toTalk == "") {
                messageList.push(receiveData);
                newMsg(hasMessage);
                latestNew();
                msgHint();
                noteCount();
                $.ajax({
                    type: "post",
                    url: "/IMMessage/ReceiveMessage",
                    dataType: "json",
                    data: { id: receiveData.messageId },
                    success: rsHandler(function (data) {
                    })
                });

                return;
            } else if (toTalk.type == 2 && toTalk.groupId != receiveData.groupId) {
                getMessage();
                messageList.push(receiveData);
                newMsg(hasMessage);
                latestNew();
                msgHint();
                noteCount();
                $.ajax({
                    type: "post",
                    url: "/IMMessage/ReceiveMessage",
                    dataType: "json",
                    data: { id: receiveData.messageId },
                    success: rsHandler(function (data) {
                    })
                });

                return;
            } else if (toTalk.type != 2 && toTalk.contactId != receiveData.sendUser.userId) {
                getMessage();
                messageList.push(receiveData);
                newMsg(hasMessage);
                latestNew();
                msgHint();
                noteCount();
                $.ajax({
                    type: "post",
                    url: "/IMMessage/ReceiveMessage",
                    dataType: "json",
                    data: { id: receiveData.messageId },
                    success: rsHandler(function (data) {
                    })
                });

                return;
            }
        }
        
        //$('#temp').html('');
        var html = "";

        switch (receiveData.type) {
            case 1:             // 上线消息
                // 改变用户头像
                var idUser = '#' + receiveData.sendUser.userId;
                $(idUser).removeClass('grayFilter');
                console.log('上线了',receiveData)
                break;
            case 2:             // 下线消息
                // 改变用户头像
                var idUser ='#'+receiveData.sendUser.userId;
                $(idUser).addClass('grayFilter');
                console.log('下线了', receiveData)
                break;
            case 3:             // 普通文字消息
                //receiveData.sendUser.headImage = receiveData.sendUser.headImage ? '/' + $("#filePath").html() + '/' + receiveData.sendUser.headImage : '/Images/common/portrait.png';
                html = '<li><div class="imgTalk leftChat"><img src=' + receiveData.sendUser.headImage + ' /></div><div class="mainTalk leftChat"><div class="nameTop textLeft"><span>' + receiveData.sendUser.userName + '</span></div><div class="colorRight speakMsg ">' + receiveData.message + '<div class="leftArrow"><em></em><span></span></div></div></div></li>';
                break;
            case 4:            // 图片
                //receiveData.sendUser.headImage = receiveData.sendUser.headImage ? '/' + $("#filePath").html() + '/' + receiveData.sendUser.headImage : '/Images/common/portrait.png';
                html = '<li><div class="imgTalk leftChat"><img src=' + receiveData.sendUser.headImage + ' /></div><div class="mainTalk leftChat"><div class="nameTop textLeft">' + receiveData.sendUser.userName + '</div><div><img src=' + '/' + $("#filePath").html() + '/' + receiveData.fileName + ' class="talkPic" /></div></div></li>';
                break;
            case 5:            // 文件
                //receiveData.sendUser.headImage = receiveData.sendUser.headImage ? '/' + $("#filePath").html() + '/' + receiveData.sendUser.headImage : '/Images/common/portrait.png';
                html = '\
                        <li>\
                        <div class="imgTalk leftChat">\
                        <img src=' + receiveData.sendUser.headImage + ' />\
                        </div>\
                        <div class="fileTalk leftChat">\
                        <div class="nameTop textLeft">' + receiveData.sendUser.userName + '</div>\
                        <div class="wrapFile">\
                        <div class="fileName">\
                        <span class="fa fa-file-code-o code"></span>\
                        <span class="fileIntro">' + receiveData.message + '</span>\
                        </div>\
                        <div class="fileDown"><a href="#" class="fileDownload" fileName=' + receiveData.fileName + ' message=' + receiveData.message + '><span class="fa fa-download"><span></a></div>\
                        </div>\
                        </div>\
                        </li>\
                        '
        }

        $('.contentTo .list').append(html);

        $(".fileDownload").click(function () {
            var fileName = $(this).attr('fileName');
            var message = $(this).attr('message');
            console.log('mess',message)
            $.post("/IMMessage/DownloadFile", { displayName: message, saveName: fileName, flag: 0 }, function (data) {
                if (data == "success") {
                    //loadViewToMain("/IMMessage/DownloadFile?displayName=" + escape(receiveData.message) + "&saveName=" + receiveData.fileName + "&flag=1");
                    window.location.href = "/IMMessage/DownloadFile?displayName=" + escape(message) + "&saveName=" + fileName + "&flag=1";
                }
                else {
                    ncUnits.alert("下载失败");
                }
            });
        });
        $('.contentTo').scrollTop($('.contentTo')[0].scrollHeight);

        $.ajax({
            type: "post",
            url: "/IMMessage/ReceiveMessage",
            dataType: "json",
            data: { id: receiveData.messageId },
            success: rsHandler(function (data) {
            })
        });
    }

    // 结束



    //div可拖动改变大小
    if ($.cookie('oldWidth')) {
        $('#chat').css('width', $.cookie('oldWidth') + 'px');
        $('#searchInput').css('width', ($.cookie('oldWidth') - 58) + 'px')
    }
    showResize();
   
    $(window).resize(function () {
        throttle(showResize, window);
    });

    function throttle(method,context) {
        clearTimeout(method.tId);
        method.tId = setTimeout(function () {
            method.call(context);
        }, 1000)
    }

    function showResize() {
        $("#chat").removeClass('ui-resizable');
        var windowwidth = $(window).width();
        var dWidth = $('#chat').width()+2;
        var leftpos = windowwidth - dWidth;
        //if ($.browser.msie) {
        //    // 此浏览器为 IE
            
        //} 


        $('#chat').css('left', leftpos + 'px');
        $("#chat").resizable({
            handles: 'w,e',
            maxWidth: 650,
            minWidth: 155,
            start: function (event, ui) {

            },
            resize: function (event, ui) {
                var dWidth = $('#chat').width();
                $('#searchInput').css('width',(dWidth-58)+'px')
                //$('.wraGroup #rganzieTree').css('width', dWidth);
            },
            stop: function (event, ui) {
                var w = $('#chat').width();
                $.cookie('oldWidth', w, { expires: 360 });
            }
        });
    }

    //div可拖动改变大小结束

    //文件上传
    var img = '';
    var partternFile = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|7z|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    var patternImg = /(\.|\/)(jpg|jpeg|bmp|png|gif)$/i;
    var j = 1;
    function fileUpload(upId) {
        var type = 0;
        switch (upId) {
            case '#fileupload':
                type = 1;
                break;
            case '#fileuploadImg':
                type = 2;
                break;
            case '#fileuploadFile':
                type = 3;
                break;
        }
        var jqXHR = null;
         $(upId).fileupload({
            url: '/IMMessage/UploadFile',
            dataType: 'text',
            formData: { type: type },
            send:function(e,data){
            },
            add: function (e, data) {
                var isSubmit = true;
               
                //var photoarry = [];
                $.each(data.files, function (index, value) {
                    if (value.size > 209715200) {
                        ncUnits.alert("上传文件过大(最大200M)");

                        isSubmit = false;
                        return;
                    } else {
                        switch (upId) {
                            case '#fileuploadGroup':
                                if (!patternImg.test(value.name)) {
                                    ncUnits.alert("上传文件格式不对");
                                    isSubmit = false;
                                    return;
                                }
                                var reader = new FileReader();
                                reader.onload = function (e) {
                                    $('#showImg').attr('src', e.target.result);
                                    $('#showImg').removeClass('hide')
                                }
                                reader.readAsDataURL(data.files[0]);
                                break;
                            case '#fileuploadImg':
                                if (!patternImg.test(value.name)) {
                                    ncUnits.alert("上传文件格式不对");
                                    isSubmit = false;
                                    return;
                                }
                                var reader = new FileReader();
                                reader.onload = function (e) {
                                    var html = '<li><div class="imgTalk rightChat"><img src=' + $(".img-circle").attr("src") + ' /></div><div class="mainTalk rightChat"><div class="nameTop textRight">' + $(".personal_info_name").html() + '</div><div><img src=' + e.target.result + ' class="talkPic" /></div></div></li>';
                                    $('.contentTo .list').append(html);
                                    $('.contentTo').scrollTop($('.contentTo')[0].scrollHeight);
                                }
                                reader.readAsDataURL(data.files[0]);
                                break;
                            case '#fileuploadFile':
                                if (!partternFile.test(value.name)) {
                                    ncUnits.alert("上传文件格式不对");
                                    isSubmit = false;
                                    return;
                                }
     
                                  var  html = '\
                                      <li>\
                                       <div class="imgTalk rightChat">\
                                       <img src=' + $(".img-circle").attr("src") + ' />\
                                       </div>\
                                        <div class="fileTalk rightChat">\
                                        <div class="nameTop textRight">' + $(".personal_info_name").html() + '</div>\
                                        <div class="wrapFile">\
                                        <div class="fileName">\
                                         <span class="fa fa-file-code-o code"></span>\
                                         <span class="fileIntro"><p>' + data.files[0].name + '</p></span>\
                                        </div>\
                                         <div class="fileDown downProgress'+j+'"><a href="javascript:;"><span class="cancelUpload" style="font-size:12px;position:relative;top:-3px;">取消<span></a></div>\
                                        </div>\
                                        </div>\
                                    </li>\
                                    ';
                                    $('.contentTo .list').append(html);
                                    var progress = '<div class="progress progress' + j + '"></div>';
                                    var percent = '<span class="percent percent' + j + '"></span>'
                                   
                               
                                    $('.downProgress'+j+'').append(progress);
                                    $('.downProgress' + j + '').append(percent);
                                    var $p = $('.downProgress' + j + '').siblings('.fileName').children('.fileIntro').find('p').eq(0);
                                    var $d = $('.downProgress' + j + '').siblings('.fileName').children('.fileIntro');                                 
                                    if ($p.outerHeight() > $d.height()) {
                                        $d.addClass('fileAfter').attr('data-content', '...').attr('title', data.files[0].name);
                                        
                                    }
                                    j++;
                                    console.log('t11t', $('.contentTo')[0].scrollHeight);
                                   
                                  
                                    $('.contentTo').scrollTop($('.contentTo')[0].scrollHeight);
                                                   
                           
                                break;
                        }
                    }
                });
                console.log('sub',isSubmit);
                if (isSubmit) {
                    jqXHR = data.submit();
                }
            },
            complete: function (data, flag) {
                $('#showImg').attr('name', data.responseText);
   
            },
            error: function (e, data) {
                ncUnits.alert('error');
            },
            done: function (e, data) {
                var result = JSON.parse(data.result);
                $('.cancelUpload').remove();
                if (result.data.displayName != null) {
                    ncUnits.alert("上传成功");
                    
                    if (upId == '#fileuploadImg') {
                        //var sendMsg = {
                        //    talkImage:result.data.saveName + "." + result.data.extension   //聊天图片
                        //};
                        //....发送聊天
                        say(4, result.data.displayName, result.data.saveName + "." + result.data.extension);

                    }
                    if (upId == '#fileuploadFile') {
                        //var sendMsg = {
                        //    talkFile: result.data.saveName + "." + result.data.extension   //聊天文件

                        //};     
                        //....发送聊天
                        say(5, result.data.displayName, result.data.saveName + "." + result.data.extension);
                    }
                   

                } else {
                    ncUnits.alert("上传文件内容不能为空");
                }
                if (data.result.status == 0) {
                    ncUnits.alert("上传失败");

                } else {
                    ncUnits.alert("上传成功");
                 
                }
          
            },
            progress: function (e, data) {
                if (upId == '#fileuploadFile') {
                    //var progress = parseInt(data.loaded / data.total * 100, 10);
                    //var width = (progress / 100) * 130 + 'px';
                    //$('.progress').css('width', width);
                    var s = j - 1;
                    var progress = parseInt(data.loaded / data.total * 100, 10);
                    $('.progress' + s + '').siblings('.percent' + s + '').html(progress + '%');
                    $('.progress' + s + '').css('width', progress * 0.6 + '%');
                }
            },
            always: function (e, data) {
            }
        })
        $(document).on('click', '.cancelUpload', function () {
            jqXHR.abort();
            $(this).parent().siblings().remove();
            $(this).html('已取消');
            
        })


    }

  
    //文件上传结束

    //右侧聊天会话点击
    var pos = '';
    var toTalk = '';
    var choose = '';
    var talk = {
        'group': '',
        'content': ''
    };
    var hasMessage = []; //当前窗口打开列表信息

    loadPersonalInfo();

    function deletePerson(data) {
        var msgData = data;
  
        $('.doc').mousemove(function () {
            $(this).find('.downLoad').removeClass('hide');
            $(this).find('.trash').removeClass('hide');
            $(this).find('.quit').removeClass('hide');
            $(this).find('.delPerson').removeClass('hide');
        })
        $('.doc').mouseleave(function () {
            $(this).find('.downLoad').addClass('hide');
            $(this).find('.trash').addClass('hide');
            $(this).find('.quit').addClass('hide');
            $(this).find('.delPerson').addClass('hide');
        })

        //删除临时群组
        $('.trash').click(function (event) {
            event.stopPropagation();
            var tid = $(this).parents('.second').attr('id');
            $(this).parents('.second').remove();
            //$.get('',{},function(){

            // })

        });

        //删除联系人
        $('.delPerson').click(function (event) {
            event.stopPropagation();
            var pid = $(this).parents('.second').attr('id');
            var ele = $(this).parents('.second');
            var hasIndex = $(this).parents('.second').index();
            ncUnits.confirm({
                title: '提示',
                html: '是否确认删除联系人？',
                yes: function (layerID) {
                    layer.close(layerID);

                    $.ajax({
                        type: "post",
                        url: "/IMMessage/DeleteImContact",
                        dataType: "json",
                        data: { contactsId: pid },
                        success: rsHandler(function (data) {
                            if (data == "ok") {
                                ncUnits.alert("删除成功!");
                                ele.remove();
                                var personNum = parseInt($('.person').eq(0).find('.last').html(), 10);
                                $('.person').eq(0).find('.last').html(personNum - 1);
                                hasMessage.splice(hasIndex, 1);
                                if (personNum - 1 == 0) {
                                    noPerson(0)
                                }
                            } else {
                                ncUnits.alert("删除失败!");
                            }
                        })
                    });
                }
            });
        });

        //删除群组
        $('.quit').click(function (event) {
            event.stopPropagation();
            var tid = $(this).parents('.second').attr('id');
            var ele = $(this).parents('.second');
            var create = $(this).attr('create');
            var title;
            if (create == $('#userId').html()) {
                title = '是否确认解散该群组？';
            } else {
                title = '是否确认退出群组？'
            }
            ncUnits.confirm({
                title: '提示',
                html:title ,
                yes: function (layerID) {
                    layer.close(layerID);

                    $.ajax({
                        type: "post",
                        url: "/IMMessage/QuitGroup",
                        dataType: "json",
                        data: { groupId: tid },
                        success: rsHandler(function (data) {
                            if (data == "ok") {
                                ncUnits.alert("退出成功!");
                                ele.remove();
                                var groupNum = parseInt($('.person').eq(1).find('.last').html(), 10);
                                $('.person').eq(1).find('.last').html(groupNum - 1);
                                if (groupNum - 1 == 0) {
                                    noPerson(1)
                                }
                            } else {
                                ncUnits.alert("退出失败!");
                            }
                        })
                    });
                }
            });
        })
        $('.third').click(function (event) {
            event.stopPropagation();
        })

        $('.second').click(function (event) {

            event.stopPropagation();
            var number = $(this).attr('number')
            var sindex = $(this).parents('.person').index();
            var tindex = $(this).index();
            toTalk = msgData[tindex];
            if (sindex == 3) {
                if (toTalk.type == 1) {
                    toTalk.contactId = toTalk.id;
                } else {
                    toTalk.groupId = toTalk.id;
                }
            }

            hideSearch();
            if (sindex == 0 || (sindex == 3 && number == 1)) {
                $('.top .showGroup').addClass('hide');
            } else {
                $('.top .showGroup').removeClass('hide');

            }
            chatDlg();
        })
    }

    function hideSearch() {
        if (!$('.searchPerson').hasClass('hide')) {
            $('.searchPerson').addClass('hide');
        }
        $('.selectPerson ul').html('');

    }
    //messageList = [{}]

    function chatDlg() {
        $('#temp').html('');
        $('.contentTo .list').html('');
        console.log('wswswqweweew')
       
        var html = '';
        //$('#chat').hide();
        var name = toTalk.contactName || toTalk.groupName || toTalk.name;
        var headImage = toTalk.headImage ? toTalk.headImage : "/Images/common/portrait.png";
        $('.wrapSpeak').removeClass('hide');
        //$('.msg').removeClass('hide');
  
        $('#temp').focus();
        $('.speak .top span').eq(0).html(name);
        $('.speak .top img').eq(0).attr('src', headImage);
        //窗口未打开时接收消息
        var idSelect = toTalk.contactId || toTalk.groupId || toTalk.id;
        //去闪烁
        var selectId = '#' + idSelect;
        if ($(selectId).find('.memPhoto').hasClass('listenNew')) {
            $(selectId).find('.memPhoto').removeClass('listenNew');
            console.log('wwwwww')
        }
        var listMsg = [];
        var userId = $('#userId').html();
        //$.each(messageList, function (n, val) {
        //    var tempId = val.groupId == null?val.sendUser.userId:val.groupId;
        //    if (idSelect == tempId) {
        //        listMsg.push(val);
        //        messageList.splice(n, 1);
        //    }
        //});

        for (k in messageList) {
                var tempId = messageList[k].groupId || messageList[k].sendUser.userId;
                if (idSelect == tempId) {
                    listMsg.push(messageList[k]);
                    delete messageList[k];
                }

        }
        var pushMessage = [];
        $.each(messageList, function (i, val) {
            if (typeof(val) != 'undefined') {
                pushMessage.push(val)
            }
        })
        messageList = pushMessage;
        if (messageList.length == 0) {
            $('.newGif').addClass('hide');
        } else {
            $.each(messageList, function (n, val) {
                console.log('bvbv', val)
                if (val.type != -1) {
                    $('.newGif').removeClass('hide');
                    return;
                }
                if (!$('.newGif').hasClass('hide')) {
                    $('.newGif').addClass('hide');
                }

            })
        }
        msgHint();
        $.each(listMsg, function (n, val) {
            var style = val.id == userId ? "rightChat" : "leftChat";
            var topStyle = val.id == userId ? "textRight" : "textLeft";
            
            var str = '/' + $('#headImagePath').html() + '/';
            if (val.sendUser.headImage.indexOf(str) == -1) {
                val.sendUser.headImage = str + val.sendUser.headImage
            }
            //var colorStyle = val.id != userId ? "colorRight" : "colorLeft";
            switch (val.type) {
                case 3:
                    html += '<li><div class="imgTalk ' + style + '"><img src=' + val.sendUser.headImage + ' /></div><div class="mainTalk ' + style + '"><div class="nameTop ' + topStyle + '">' + val.sendUser.userName + '</div><div class="colorRight speakMsg"><span>' + val.message + '</span><div class="leftArrow"><em></em><span></span></div></div></div></li>';
                    break;
                case 4:
                    html += '<li><div class="imgTalk ' + style + '"><img src=' + val.sendUser.headImage + ' /></div><div class="mainTalk ' + style + '"><div class="nameTop ' + topStyle + '">' + val.sendUser.userName + '</div><div><img src=' + '/' + $("#filePath").html() + '/' + val.fileName + ' class="talkPic" /></div></div></li>';
                    break;
                case 5:
                    html += '\
                          <li>\
                           <div class="imgTalk ' + style + '"><img src=' + val.sendUser.headImage + ' />\
                           </div>\
                            <div class="fileTalk ' + style + '">\
                            <div class="nameTop ' + topStyle + '">' + val.sendUser.userName + '</div>\
                            <div class="wrapFile">\
                            <div class="fileName">\
                             <span class="fa fa-file-code-o code"></span>\
                             <span class="fileIntro">'+val.message+'</span>\
                            </div>\
                             <div class="fileDown"><a href="javascript:;" class="fileDownload"  fileName=' + val.fileName + ' message=' + val.message + '><span class="fa fa-download"><span></a></div>\
                            </div>\
                            </div>\
                        </li>\
                        ';
                    break
            }
           
        })


        $('.contentTo .list').append(html);
        $(".fileDownload").click(function () {
            var fileName = $(this).attr('fileName');
            var message = $(this).attr('message');
            $.post("/IMMessage/DownloadFile", { displayName: message, saveName: fileName, flag: 0 }, function (data) {
                if (data == "success") {
                    //loadViewToMain("/IMMessage/DownloadFile?displayName=" + escape(message) + "&saveName=" + fileName + "&flag=1");
                    window.location.href = "/IMMessage/DownloadFile?displayName=" + escape(message) + "&saveName=" + fileName + "&flag=1";
                }
                else {
                    ncUnits.alert("下载失败");
                }
            });
        });
        $('.contentTo').scrollTop($('.contentTo')[0].scrollHeight);

        sendData.receiveUser = [];

        // 群组聊天
        if (toTalk.type == 2) {

            sendData.groupId = toTalk.groupId;

            $.ajax({
                type: "post",
                url: "/IMMessage/GetImGroupUser",
                dataType: "json",
                data: { groupId: toTalk.groupId },
                success: rsHandler(function (data) {
                    $.each(data, function (n, val) {
                        var receive = {
                            userId: val.contactId,
                            userName: val.contactName,
                            headImage: val.headImage
                        };

                        sendData.receiveUser.push(receive);
                    });
                })
            });

        } else {
            sendData.groupId = null;
            var receive = {
                userId: toTalk.contactId,
                userName: toTalk.contactName,
                headImage: toTalk.headImage
            };

            sendData.receiveUser.push(receive);
        }

    }


    //搜素
    //$('.search #searchInput').click(function (e) {
    //    e.stopPropagation();
    //    if (!$('.searchPerson').hasClass('hide')) {
    //        $('.searchPerson').addClass('hide');
    //        $('.search #choose').val('');
    //    } else {
    //        $('.searchPerson').removeClass('hide');
    //    }
    //    if (!$('.newAdd').hasClass('hide')) {
    //        $('.newAdd').addClass('hide')
    //    }
    //});

    $('.search #searchInput').focus(function () {
        if ($('.searchPerson').hasClass('hide')) {
            $('.searchPerson').removeClass('hide');
        }
    });

    $('.search #searchInput').blur(function () {
        if (!$('.searchPerson').hasClass('hide') && !$('#searchInput').val()) {
            $('.searchPerson').addClass('hide');
        }
    })


    $('.search #searchInput').on('input', function (e) {
        e.stopPropagation();
        var searchText = $(this).val();
        //$('#searchInput').val(searchText);
        $('.searchPerson ul').html('');
        $.post("/Shared/GetUserListByName", { text: searchText }, function (data) {
            var result = data.data;
            var html = '';
            $.each(result, function (n, val) {
                html += '<li><a href="javascript:;"><img src="' + val.img + '" alt=""/><span>' + val.name + '</span></a></li>'
            })
            $('.searchPerson ul').html(html);
            $('.searchPerson ul li a').click(function (e) {
                var index = $(this).parent('li').index();
                toTalk = {
                    contactId: data.data[index].id,
                    contactName: data.data[index].name,
                    headImage: data.data[index].img
                }
                $('.top .showGroup').addClass('hide');
                $('.search input[type="text"]').val('');
                $('.searchPerson').addClass('hide')
                $('.searchPerson ul').html('');
                chatDlg();
            })
        }, 'json')
    });
    //搜素结束


    $('.smallSize .banSmall').click(function () {
        $('#chat').hide();
        $('.msg').removeClass('hide');
        $('.person ul').remove();
        msgHint();
    })


    $('.pClick').click(function (event) {
        $(this).addClass('green');
        $(this).siblings('.person').removeClass('green');
        $(this).find('.iconHide').removeClass('hide');
        $(this).find('.iconPicture').addClass('hide');
        $(this).siblings('.person').find('.iconHide').addClass('hide');
        $(this).siblings('.person').find('.iconPicture').removeClass('hide');
        $('.person .wrapGroup').removeClass('green');
        $(this).siblings('.person').children('.noPerson').remove();
        if ($(this).find('li').length>0) {
             $(this).children('ul').remove();
             return false;
        };
        if (!$('.searchPerson').hasClass('hide')) {
            $('.searchPerson').addClass('hide');
        }
       

        //var temp = $(this).index();
        pos = $(this).index();
        loadList(this, pos);
    
    });
    


    $('.person .wrapGroup').click(function () {
        if (!$('.searchPerson').hasClass('hide')) {
            $('.searchPerson').addClass('hide');
        }
        $(this).addClass('green');
        $(this).find('.iconHide').removeClass('hide');
        $(this).find('.iconPicture').addClass('hide');
        $(this).parent('.person').siblings('.person').removeClass('green');
        $(this).parent('.person').siblings('.person').find('.iconPicture').removeClass('hide');
        $(this).parent('.person').siblings('.person').find('.iconHide').addClass('hide');
        $(this).parent('.person').siblings('.person').children('.noPerson').remove();
        if ($(this).parent('.person').find('li').length > 0) {
            $(this).parent('.person').children('ul').remove();
            return false;
        };
        pos = $(this).parent('.person').index();

        loadList($('.person').eq(2), pos);

    })

    function noPerson(idx) {
        var idx = idx;
        if ($('.person').eq(idx).find('div').hasClass('noPerson')) {
            $('.person').eq(idx).find('.noPerson').remove();
            $('.person').eq(idx).find('ul').remove();
            return;
        }
        var html = '\
                <div class="noPerson">\
                </div>\
        ';
        var action = '<div class="addPerson"><img src="../../Images/common/addHit.png" alt=""/></div>'
        var ul = '<ul></ul>';
        $('.person').eq(idx).append(html);
        if (idx == 3) {
            $('.person').eq(idx).find('.noPerson').html('暂无会话记录');
        } else if (idx == 4) {
            $('.person').eq(idx).find('.noPerson').html('暂无文档文件');
        } else {
            $('.person').eq(idx).find('.noPerson').append(action);
        }
        $('.person').eq(idx).siblings('.person').find('.noPerson').remove();
        $('.person').eq(idx).append(ul);
        $('.person').eq(idx).siblings('.person').find('ul').remove();
        $('.addPerson').click(function () {
            event.stopPropagation();
            addNum(pos)
        })
    }

    //发起聊天
    $('.msg').click(function () {
        
        closeTalk();
        cleanGroup();
        cleanHistory();
        var obj = $('.person').eq(3);
        loadList(obj,3);

    })

    $('.say').click(function () {
        var temp = $('#temp').html(); //文字聊天内容
        if (!temp) {
            ncUnits.alert("不能发送空消息！");
            return false;
        }
        say(3, temp, null);
        $('#temp').focus();
    });
    $('.closeTalk').click(function () {
        $('#temp').html('');
        closeTalk();
        removeHistory();
        cleanGroup();
        toTalk = '';
        //var obj = $('.person').eq(3);
        //loadList(obj, 3);
    })


    //div可编辑光标定位








    //div可编辑光标结束


    document.getElementById('temp').onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        if (ev.shiftKey && ev.keyCode == 13) {
            console.log('换行');
            $('#temp').append('\n');
            return
        }
        if (ev.keyCode == 13) {
            ev.preventDefault();
            $('.say').click();
        }
        
    }

    function say(type, message, fileName) {
        
        sendData.type = type;
        sendData.sendUser.userId = $(".personal_info_id").html();
        sendData.sendUser.userName = $(".personal_info_name").html();
        sendData.sendUser.headImage = $(".personal_info_portrait").attr("src");
        sendData.message = message;
        sendData.fileName = fileName;
        sendData.sendTime = new Date();
        ws.send(JSON.stringify(sendData));
        
        if (type == 3) {
            $('#temp').html('');
            console.log('2234')
            var html = '<li><div class="rightChat">' + message + '</div></li>';
            var html = '<li><div class="imgTalk rightChat"><img src=' + sendData.sendUser.headImage + ' /></div><div class="mainTalk rightChat"><div class="nameTop textRight">' + sendData.sendUser.userName + '</div><div class="colorLeft speakMsg"><p class="pTalk">' + message + '</p><div class="rightArrow"><em></em><span></span></div></div></div></li>';
            $('.contentTo .list').append(html);
            $('.contentTo').scrollTop($('.contentTo')[0].scrollHeight);
        }

    }


    function closeTalk() {
        $('.msg').addClass('hide');
        $('.wrapSpeak').addClass('hide');
        $('#chat').show();
        $('.contentTo ul').html('');
    }

    //聊天发送图片,文件

    $('.left-top div').eq(1).on('click', function () {
        var upId = '#fileuploadImg'
        fileUpload(upId)

    })
    $('.left-top div').eq(2).on('click', function () {
        var upId = '#fileuploadFile'
        fileUpload(upId)

    })





    //聊天发送图片结束

    //发起聊天结束


    //新建群组
    //增加群成员
    $('.addTo').click(function () {
        var talkMsg = toTalk;
        addNum(1, talkMsg)
    })

    //增加群成员结束


    //ztree开始

    var setting = {
        //check: {
        //    enable: true,
        //    chkStyle: "radio"
        //},
        data: {
            simpleData: {
                enable: true
            }
        },
        view: {
            showIcon: false,
            showLine: false,
            selectedMulti: false
        },
        callback: {
            onClick: onCheck
        },
        async: {
            enable: true,
            url: "/Shared/GetOrganizationList",
            autoParam: ["id=parent"],
            dataFilter: function (treeId, parentNode, responseData) {
                return responseData.data;
            }
        }
    };

    var code;

    //function setCheck() {
    //    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
    //        py = $("#py").attr("checked") ? "p" : "",
    //        sy = $("#sy").attr("checked") ? "s" : "",
    //        pn = $("#pn").attr("checked") ? "p" : "",
    //        sn = $("#sn").attr("checked") ? "s" : "",
    //        type = {"Y": py + sy, "N": pn + sn};
    //    zTree.setting.check.chkboxType = type;
    //    showCode('setting.check.chkboxType = { "Y" : "' + type.Y + '", "N" : "' + type.N + '" };');
    //}

    function showCode(str) {
        if (!code) code = $("#code");
        code.empty();
        code.append("<li>" + str + "</li>");
    }

    function initTree() {
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            success: function (data) {
                $.fn.zTree.init($("#treeDemo"), setting, data.data);
                //setCheck();
                //$("#py").bind("change", setCheck);
                //$("#sy").bind("change", setCheck);
                //$("#pn").bind("change", setCheck);
                //$("#sn").bind("change", setCheck);
            }
        });
    }


    //ztree结束

    //选择成员开始
    $('.showAdd').click(function () {
        hideSearch();
        if ($('.newAdd').hasClass('hide')) {
            $('.newAdd').removeClass('hide');
            return;
        }
        $('.newAdd').addClass('hide');

    })

    $('.newAdd .addNum').click(function () {
        var idx = $(this).index();
        addNum(idx)

    });

    var personWithSub = false;

    function addNum(idx, msg) {
        $('.allNumber').remove();
        $('.newAdd').addClass('hide');
        var talkMsg = msg;
        var title = '';
        vName = [];
        var index = idx;
        switch (index) {
            case 0:
                title = '添加';
                break;
            case 1:
                title = '新建群组';
                break;
            case 2:
                title = '新建临群组';
                break;
            default :
                return;

        }

        var addNumber = '\
           <div class="allNumber" id="dragNum">\
           <div class="numberHead">\
           <span>' + title + '</span>\
           <span class="glyphicon glyphicon-remove closeDlg"></span>\
           </div>\
           <div class="numberBody">\
           <div class="treeWrap">\
           <ul id="treeDemo" class="ztree">\
           </ul>\
            <div class="selectPerson">\
            <div class="pTop">\
            <div class="contain">\
            <input type="checkbox" class="checkLow"/>\
            <span>包含下级</span>\
            </div>\
            <div class="allSelect">\
            <input type="checkbox" class="checkAll"/>\
            <span>选择全部\
            </div>\
            </span>\
            </div>\
            <ul></ul>\
           </div>\
           </div>\
           <div class="numberSelect">\
           <div class="selectTop">已选:\
           <span>\
           </span>\
           </div>\
           <ul>\
           </ul>\
           </div>\
           </div>\
           <div class="numberFoot">\
           <a  href="javascript:;" class="cancel">取消\
           </a>\
           <a href="javascript:;" class="getPerson">确定\
           </a>\
           </div>\
            </div>\
            ';
        var isGroup = '\
        <div class="searchGroup">\
           <span  class="groupPre">群组名</span>\
           <input type="text" class="groupName" maxlength=20 />\
           <div class=upIcion>\
           <a href="javascript:;" class="groupIcon">上传头像</a>\
           <input id="fileuploadGroup" type="file" name="files[]" multiple/>\
           </div>\
            <div class="imgShow">\
            <img id="showImg" src="" name="" class="hide" >\
             </div>\
           <span class="wordLimit"></span>\
           </div>\
        ';
        var isPerson = '\
        <div class="searchGroup" style="height: 30px;line-height: 30px;">\
           <span>选择人员</span>\
           </div>\
        ';
        var groupSearch = '<div class="groupSearch"><span>选择人员</span><span></span></div>'
        if ($('.allNumber').length > 0) {
            return;
        }
        $('body').append(addNumber);
        if (index == 1 || index == 2) {
            $('.numberBody').prepend(isGroup);
            $('.treeWrap').prepend(groupSearch);
            $('.allNumber').addClass('groupHeight');
            $('.selectPerson').addClass('sHeight')
            $('.selectPerson ul').addClass('uHeight');
            $('.numberBody').addClass('bodyone');
            $('.numberSelect').addClass('selone');
        } else {
            $('.numberBody').prepend(isPerson);
            $('.allNumber').addClass('personHeight');
            $('.selectPerson').addClass('ssHeight')
            $('.selectPerson ul').addClass('uuHeight');
            $('.numberBody').addClass('bodytwo');
            $('.numberSelect').addClass('seltwo');
        }
        initTree();
        if (talkMsg) {
            $('.searchGroup .groupName').val(talkMsg.groupName).attr('readonly', 'readonly').css({'background':'#fff','border':'none','position':'absolute','left':'65px'});
            $('.upIcion').remove();
            $('.numberHead span').eq(0).html('新增群成员');
            $('.groupPre').remove();
            $('.imgShow').css({'position':'absolute','left':'10px','top':'2px','margin-left':'0px'})
            $('#showImg').removeClass('hide').attr('src', talkMsg.headImage);
        }
        //$("#dragNum").draggable();
        var upId = '#fileuploadGroup'
        fileUpload(upId);
        $('.groupName').on('input', function (e) {
            var len = $(this).val().length;
            var leave = 20 - len;
            var word = leave == 20 ? '' : '还可以输入' + leave + '字';
            $('.wordLimit').html(word);
        })


        $('.numberFoot .getPerson').bind('click', function () {
            //添加组成员
            var create = {};
            if (talkMsg) {
                var memberAdd = {};
                memberAdd.groupId = talkMsg.groupId;
                memberAdd.groupUserId = [];
                $.each(vName, function (n, val) {
                    memberAdd.groupUserId.push(parseInt(val.id));
                });
                if (memberAdd.groupUserId.length == 0) {
                    ncUnits.alert("请选择成员!");
                    return;
                }

                $.ajax({
                    type: "post",
                    url: "/IMMessage/AddImGroupUser",
                    dataType: "json",
                    data: { data: JSON.stringify(memberAdd) },
                    success: rsHandler(function (data) {
                        if (data == "ok") {
                            ncUnits.alert("添加成功!");
                            loadMember(talkMsg);
                        } else {
                            ncUnits.alert("添加失败!");
                        }
                    })
                });
            } else {
                //新增组和联系人
                create.groupName = $('.searchGroup .groupName').val();
     
                var url = '';
                create.groupUserId = [];
                $.each(vName, function (n, val) {
                    create.groupUserId.push(parseInt(val.id));
                });
                create.groupUserId = JSON.stringify(create.groupUserId);

                if (index == 2) {
                   
                } else if (index == 1) {
                    var headImage = $.parseJSON($('#showImg').attr('name'));
                    if (create.groupName == "") {
                        ncUnits.alert("请添加群组名!");
                        return;
                    }
                    create.headImage =headImage?headImage.data.saveName + "." + headImage.data.extension:'';
                    create.type = 2;
                    create.groupUserId = [];
                    $.each(vName, function (n, val) {
                        create.groupUserId.push(parseInt(val.id));
                    });

                    if (create.groupUserId.length == 0) {
                        ncUnits.alert("请选择群成员!");
                        retutn;
                    }
                    $.ajax({
                        type: "post",
                        url: "/IMMessage/AddImGroup",
                        dataType: "json",
                        data: {data: JSON.stringify(create)},
                        success: rsHandler(function (data) {
                            if (data == "ok") {
                                ncUnits.alert("添加成功!");
                                getTotal()
                                if ($('.person').eq(index).find('ul').length == 0 && $('.noPerson').length == 0) {
                                    return;
                                }
                                loadList($('.person').eq(index), index);
                            } else {
                                ncUnits.alert("添加失败!");
                            }
                        })
                    });
                } else {
                    var contactsId = [];
                    $.each(vName, function (n, val) {
                        contactsId.push(parseInt(val.id));
                    });
                    if (contactsId.length == 0) {
                        ncUnits.alert("请选择联系人!");
                        retutn;
                    }

                    $.ajax({
                        type: "post",
                        url: "/IMMessage/AddImContact",
                        dataType: "json",
                        data: { contactsId: JSON.stringify(contactsId) },
                        success: rsHandler(function (data) {

                            if (data == "ok") {
                                getTotal()
                                ncUnits.alert("添加成功!");
                                if ($('.person').eq(index).find('ul').length == 0 && $('.noPerson').length == 0) {
                                    return;
                                }
                                loadList($('.person').eq(index), index);
                            } else {
                                ncUnits.alert("添加失败!");
                            }
                        })
                    });
                }
            }
            //deletePerson();
            $('.allNumber').remove();
        });
        $('.numberFoot .cancel').bind('click', function () {
            $('.allNumber').remove();
            vName = [];
        })
        $('.closeDlg').bind('click', function () {
            $('.allNumber').remove();
        })
        vName = [];
    }

    var vName = []

    function onCheck(e, treeId, treeNode) {
        //vName = [];

        var getId = treeNode.id; //点击加载获取节点id；
        var checked = $(".checkLow").prop('checked');
        personWithSub = checked == true ? 1 : 0;
        $.ajax({
            type: "post",
            url: "/Shared/GetUserList",
            dataType: "json",
            data: { withSub: personWithSub, organizationId: getId, withUser: false },
            success: function (data) {
                $('.selectPerson ul').html('');
                var listData = data.data;
                var listPerson = '';
                $.each(listData, function (n, val) {
                    listPerson += '<li><input type="checkbox" value=' + val.userId + ' class="checkId" /><span class="checkName">' + val.userName + '</span>-<span>' + val.organizationName + '</span></li>'
                })
                $('.selectPerson ul').html(listPerson);
                if (vName.length > 0) {
                    $.each($('.selectPerson ul li'), function (n, ele) {
                        var isIn = $('.selectPerson ul li').eq(n).children('.checkId').val();
                        for (var i = 0; i < vName.length; i++) {
                            if (isIn == vName[i].id) {
                                $('.selectPerson ul li').eq(n).children('.checkId').attr('checked', true)
                            }
                        }
                    })
                }
                addChecked();
                checkAll();
                checkLow(getId);
            }
        });
    };

    //选择下属
    function checkLow(id) {
        var personOrgId = id;
        $('.checkLow').off('click');
        $('.checkLow').on('click',function () {
            var checked = $(this).prop('checked');
            if (checked == true) {
                $(this).attr('checked', true);
                personWithSub = 1;
                $.ajax({
                    type: "post",
                    url: "/Shared/GetUserList",
                    dataType: "json",
                    data: { withSub: personWithSub, organizationId: personOrgId, withUser:false },
                    success: function (data) {
                        var listPerson = '';
                        if (data.data.length > 0) {
                            for (var i = 0; i < data.data.length; i++) {
                                listPerson += '<li class="addLow"><input type="checkbox" value=' + data.data[i].userId + ' class="checkId" /><span class="checkName">' + data.data[i].userName + '</span>-<span>' + data.data[i].organizationName + '</span></li>'
                            }
                            $('.selectPerson ul').append(listPerson);

                        }
                        addChecked();
                    }
                });
            } else {
                //$(this).attr('checked', false);
                //personWithSub = 0;
                //$('.selectPerson ul li').children('.checkId').attr('checked', false);
                //vName = [];
                //$(".numberSelect ul").html('');
                $(this).attr('checked', false);
                $('.addLow').remove();
            }
        })
    }

    //全选
    function checkAll() {

        $('.checkAll').click(function () {
            var listVid = [];
            $.each(vName, function (i, v) {
                listVid.push(v.id);
            })
            var checked = $(this).prop('checked');
            if (checked == true) {
                $('.selectPerson ul li').children('.checkId').attr('checked', true);
                $.each($('.selectPerson ul li'), function (n, val) {
                    var lid = $('.selectPerson ul li').eq(n).children('.checkId').val();
                    $(this).attr('checked', true);
                    if (listVid.indexOf(lid) == -1) {
                        var tempObj = {
                            id: lid,
                            name: $('.selectPerson ul li').eq(n).children('.checkName').html()
                        }
                        vName.push(tempObj);
                    }
                })
                cleanUser();
            } else {
                $(this).attr('checked', false);
                $('.selectPerson ul li').children('.checkId').attr('checked', false);
                vName = [];
                $(".numberSelect ul").html('');
            }

            moveClick()

        })
    }




    //复选框选择人员
    function addChecked() {
        $('.selectPerson ul input[type="checkbox"]').off('click');
        $('.selectPerson ul input[type="checkbox"]').on('click',function () {
            var checked = $(this).prop('checked');
            var chooseUser = {
                id: $(this).val(),
                name: $(this).next().html()
            }
            if (checked == true) {
                $(this).attr('checked', true);
                vName.push(chooseUser);
            } else {
                $(this).attr('checked', false);
                for (var i = 0; i < vName.length; i++) {
                    if (vName[i].id == chooseUser.id) {
                        vName.splice(i, 1)
                    }
                }

            }
            cleanUser();

            moveClick();


        })
    }

    function cleanUser() {
        $(".numberSelect ul").html('');
        var html = ''
        $.each(vName, function (n, val) {
            html += '<li style="position: relative"><a href="javascript:;">' + val.name + '<span class="move glyphicon glyphicon-remove" style="position:absolute;right:5px;line-height: 40px;" id=' + val.id + '></span></a></li>'
        })
        $(".numberSelect ul").append(html);
    }

    function moveClick() {
        $('.move').click(function () {
            var mIndex = $(this).parents('li').index();
            $(this).parents('li').remove();
            var moveOut = vName.splice(mIndex, 1);
            $.each($('.selectPerson ul li'), function (n, ele) {
                var vCheck = $('.selectPerson ul li').eq(n).children('.checkId').val();
                if (vCheck == moveOut[0].id) {
                    $('.selectPerson ul li').eq(n).children('.checkId').attr('checked', false)
                }
            })
        })
    }


    function beforeClick(id, node, flag) {
        var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
        treeObj.checkNode(node, undefined, undefined, true);

        return false;
    }


    //选择成员结束
    //查看组成员
    $('.showGroup').click(function () {
        showGroup()
    });

    function showGroup() {
        if ($('.speakGroup').hasClass('hide')) {
            $('.speakGroup').removeClass('hide');
            //$.post()
            //var groupNumber = [
            //    {'id': 1, 'name': 'Jack'},
            //    {'id': 2, 'name': 'Merry'},
            //    {'id': 3, 'name': 'Lilei'},
            //    {'id': 4, 'name': 'Hanmeimei'},
            //    {'id': 1, 'name': 'Jack'},
            //    {'id': 2, 'name': 'Merry'},
            //    {'id': 3, 'name': 'Lilei'},
            //    {'id': 4, 'name': 'Hanmeimei'}
            //];
            loadMember(toTalk);

        } else {
            cleanGroup();
        }
    }


    //加载组成员
    function loadMember(msg) {
        var tempMsg = msg;
        $('.groupList ul').html('');
        //var numberList = '';
        var isPower = {};
        var uid = $('#userId').html();
        $.ajax({
            type: "post",
            url: "/IMMessage/GetImGroupUser",
            dataType: "json",
            data: { groupId: tempMsg.groupId },
            success: rsHandler(function (data) {
                //获取当前登录人员在该群组的信息
                $.each(data, function (i, val) {
                    if (val.contactId == uid) {
                        isPower = val;
                        return;
                    }
                })

                if (!isPower.power) {
                    $('.addTo').hide();
                }
                $.each(data, function (n, value) {
                    var headImg = value.headImage ? value.headImage : "/Images/common/portrait.png";
                    var onStyle = 'ss';
                    if (!value.onLine && value.contactId !=uid) {
                        onStyle = "grayFilter";
                    }
                    var numberList = '\
                            <li class="listHover">\
                           <div class="numberLeft ' + onStyle + '" id=' + value.contactId + '>\
                            <img src="' + headImg + '" />\
                           </div> \
                           <div class="numberRight">\
                           <div class="numberName">' + value.contactName + '\
                           </div>\
                            <div class="numberStation">' + value.station + '\
                           </div>\
                           </div>\
                           <span class="adminManage fa fa-user" userId=' + value.contactId + ' power='+value.power+'></span>\
                           <span class="delGroupMember fa fa-times hide" userId=' + value.contactId + '></span>\
                            </li>\
                            ';
                    $('.groupList ul').append(numberList);
  
                    var admin = '';
                    if (value.power == 1) {
                        $('.listHover').eq(n).find('.adminManage').remove();
                    }
                    //只有群主才能增加管理员
                    if (isPower.power != 1) {
                        $('.adminManage').remove();
                    }
                    //当为管理员的时候图标颜色为绿色
                    if (value.power == 2) {
                        $('.adminManage').addClass('adminColor')
                    }


                    ////此处判断是否是群组和管理员
                    if (!isPower.power) {
                        $('.listHover').eq(n).find('.delGroupMember').remove();
                      
                    } 
                    //管理员删除权限
                    if (isPower.power == 2 && value.power) {
                        $('.listHover').eq(n).find('.delGroupMember').remove();
                    }


                    ////判断如果是系统群组则群主不可解散该群
                    if (tempMsg.type == 1 && isPower.power == 1 && value.contactId == uid) {
                        $('.listHover').eq(n).find('.delGroupMember').remove();
                    }

                });

                //增加和取消管理员
                $('.adminManage').click(function (e) {
                    e.stopPropagation();
                    var powerUp = $(this).attr('power');
                     powerUp = powerUp == 2 ? null : 2;
                    var idUp = $(this).attr('userId');;
                    if (isPower.power != 1) {
                        return;
                    }
                    var obj = $(this);
                    $.ajax({
                        type: "post",
                        url: "/IMMessage/SetGroupManager",
                        dataType: "json",
                        data: { userId: idUp, groupId: tempMsg.groupId,power:powerUp},
                        success: rsHandler(function (data) {
                            if (obj.hasClass('adminColor')) {
                                obj.removeClass('adminColor');
                                obj.attr('power', null);
                                ncUnits.alert("删除管理员成功");
                            } else {
                                obj.addClass('adminColor');
                                obj.attr('power', 2);
                                ncUnits.alert("增加管理员成功");
                            }

                        })
                    })



                })

                //删除组成员
                $('.delGroupMember').click(function (e) {
                    e.stopPropagation();
                    var delId = $(this).attr('userId');
                    var index = $(this).parent('.listHover').index();
                    var confirmTitle;
                    //临时群组点击删除时判断是否为群主
                    if (isPower.contactId == delId && isPower.power == 1) {
                        confirmTitle = '是否确认解散该群组？';
                    } else {
                        confirmTitle = '是否确认删除群成员？';
                    }
                    ncUnits.confirm({
                        title: '提示',
                        html: confirmTitle,
                        yes: function (layerID) {
                            layer.close(layerID);
                            $.ajax({
                                type: "post",
                                url: "/IMMessage/QuitGroup",
                                dataType: "json",
                                data: { userId: delId, groupId: tempMsg.groupId },
                                success: rsHandler(function (data) {
                                    if (isPower.contactId == delId && isPower == 1) {
                                        ncUnits.alert("删除成功");
                                        $('.wrapSpeak').addClass('hide');
                                    } else {
                                        ncUnits.alert("删除成功");
                                        $('.listHover').eq(index).remove();
                                        data.splice(index,1)
                                    }
                                    
                                })
                            })
                        
                        }
                     })
                
                })

            })
        });

    }

    //删除群组成员
    $(document).on('mouseover', '.listHover', function (e) {
        e.stopPropagation();
        $(this).find('.delGroupMember').removeClass('hide');
        $(this).siblings().find('.delGroupMember').addClass('hide')

    })
    $(document).on('mouseleave', '.listHover', function (e) {
        e.stopPropagation();
        $(this).find('.delGroupMember').addClass('hide');

    })

    //点击群组成员弹聊天框

 


    function cleanGroup() {
        $('.speakGroup').addClass('hide');
        $('.speakHistory').addClass('hide');
        $('.speakGroup ul').html('');
    }

    //查看组成员结束

    //查看历史记录

    $('.showHistory').click(function () {
        showHistory();
    })
    $('.closeHistory').click(function () {
        removeHistory();
    })

    var searchMsg = {};

    function showHistory() {
        if ($('.speakHistory').hasClass('hide')) {
            $('.speakHistory').removeClass('hide');
            searchMsg.message = '';
            searchMsg.time = '';
            searchMsg.page = 1;
            //searchMsg.type = toTalk.type ? toTalk.type : '1';
            searchMsg.type = toTalk.contactId ? 1 : 2;
            searchMsg.id = toTalk.groupId || toTalk.contactId;
         
            loadHistoryMessage(searchMsg,false,0);
           
        } else {
            removeHistory();
        }
    }

    function removeHistory() {
        $('.speakHistory').addClass('hide');
        $('.historyBody ul').html('');
        cleanHistory();
    }

    //查看历史记录结束

    //qq表情

    $('.emotion').qqFace({
        assign: 'temp', //给输入框赋值
        path: '../Images/chat/arclist/',    //表情图片存放的路径
        show: 'editable'
    });

    $('.emotion').click(function () {
        //$('.div').focus();
        $('.div').focus();
        //getC($('.div'));

    })



    function replace_em(str) {
        str = str.replace(/\</g, '&lt;');
        str = str.replace(/\>/g, '&gt;');
        str = str.replace(/\n/g, '<br/>');
        str = str.replace(/\[em_([0-9]*)\]/g, '<img src="../chat/arclist/$1.gif" border="0" />');
        return str;
    }


    //qq表情结束


    //时间选择开始
 
    var dateConfig = {
        elem: '#historyDate',
        event: 'click',
        format: 'YYYY-MM-DD',
        istoday: true,
        issure: true,
        festival: true,
        choose: function (dates) {
            dateHistory(dates);
        },
        clear: function () {
        }
    }
    laydate(dateConfig);
    function dateHistory(dates) {
        $('.timeDate').text(dates);
        //var dates = dates.replace(/-/g, '/');
        //var date = new Date(dates);
        //date = date.getTime() / 1000;
        searchMsg.message = '';
        searchMsg.time = dates;
        searchMsg.page = 1;
        //searchMsg.type = toTalk.type ? toTalk.type : '';
        searchMsg.type = toTalk.contactId ? 1 : 2;
        searchMsg.id = toTalk.groupId || toTalk.contactId;
        $('.historyBody ul').html('');
        loadHistoryMessage(searchMsg);
    }

    //时间选择结束
    //$('#drag').drag();
    //历史记录搜索
   
    $('.searchOpen').on('click', function(){
        if ($('.searchDiv').hasClass('hide')) {
            $('.searchDiv').removeClass('hide')
        } else {
            $('.searchDiv').addClass('hide')
        }
    })
    $('.searchDiv a').on('click', function () {
        searchMsg.message = $('.searchDiv input[type="text"]').val();
        searchMsg.time = '';
        searchMsg.page = 1;
        //searchMsg.type = toTalk.type?toTalk.type:'';
        searchMsg.type = toTalk.contactId ? 1 : 2;
        searchMsg.id = toTalk.groupId || toTalk.contactId;
        $('.historyBody ul').html('');
        loadHistoryMessage(searchMsg)
    })

    function cleanHistory() {
        $('.searchDiv input[type="text"]').val('');
        $('#historyDate').val('');
        if (!$('.searchDiv').hasClass('hide')) {
            $('.searchDiv').addClass('hide')
        }
        $('.historyBody ul').html('');
        $('.historyBody .noMore').html('');
        $('.historyBody .timeDate').html('');
        searchMsg = {};
    }


    //var historyData = [
    //    { type: 1, message: '11111', name: 'Lilei' },
    //    { type: 2, message: '22222', name: 'Jhon' },
    //    { type: 3, message: '33333', name: 'Tom' },
    //    { type: 1, message: '44444', name: 'Lilei' },
    //    { type: 2, message: '55555', name: 'Jhon' },
    //    { type: 2, message: '66666', name: 'Jhon' },
    //    { type: 1, message: '77777', name: 'Lilei' },
    //    { type: 3, message: '88888', name: 'HanMeimei' },
    //    { type: 4, message: '99999', name: 'Boy' },
    //    { type: 2, message: '00000', name: 'Girl' }


    //]

    //历史记录分页加载

    //var domMouse = document.getElementById('domMouse');
    //domMouse.addEventListener('mousewheel', loadMsg, false);
    //function loadMsg(evt) {
    //    var evt = window.event || event;
    //    var delta = evt.detail ? -evt.detail / 3 : evt.wheelDelta / 120;
        

    //}


    $('#domMouse').scroll(function (e) {
        scollHistory();
    })


    function scollHistory() {
        if (!searchMsg.page) {
            return;
        }
        if (searchMsg.page == 1) {
            searchMsg.page++
            return
        }
        var nScrollTop = $('#domMouse')[0].scrollTop;
        if (nScrollTop <= 0) {
            var nHeight = $('#domMouse').height();
            loadHistoryMessage(searchMsg, true, nHeight);
            searchMsg.page++
        }
    }
   


    //历史记录分页加载结束

    //历史记录搜索结束

    //聊天图片放大


    $(document).on("click", ".talkPic", function () {
        var src = $(this).attr('src');
        bigImg(src)
    });
    $(document).on("click", ".historyImg", function () {
        var src = $(this).attr('src');
        bigImg(src)
    });
    function bigImg(src) {
        var html = '<div class="showBig"></div>';
        var imgDiv = '<div class="imgWrap"><img src=' + src + '  /></div>';
        var windowWidth = $(window).width();
        var windowHeight = $(window).height();
        $('body').append(html);
        $('body').append(imgDiv);
        $('.showBig').css({ 'width': windowWidth, 'height': windowHeight })

    }

    $(document).on("click", ".showBig", function () {
        $(this).remove();
        $('.imgWrap').remove();
    });



    //聊天图片放大结束

    

    function loadList(obj, index) {
        console.log('obj',obj)
        $(obj).children('ul').remove();
        var url = "";
        pos = index;
        // 常用联系人
        if (pos == 0) {
            url = "/IMMessage/GetImContactList";
        } else if (pos == 1) {
            url = "/IMMessage/GetImGroupList";
        } else if (pos == 2) {
            //url = "/IMMessage/GetTempImGroupList";
            url = "/IMMessage/GetOrgAndUserList"
        } else if (pos == 3) {
            url = "/IMMessage/GetConversationList";
        }

        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            success: rsHandler(function (data) {
                //listData = data.data;
                if (data.length === 0) {
                    noPerson(pos)
                    return;
                } else {
                    $(obj).find('.noPerson').remove();
                }
                if (pos == 3) {
                    $('.person').eq(3).find('.last').html(data.length);
                }
                
                //给当前打开窗口赋值
                if (pos == 0 || pos == 1 || pos == 3) {
                    hasMessage = data;
                } else {
                    hasMessage = [];
                }

                var head = "<ul class='listul'>";
                var content = "";
                $.each(data, function (n, value) {
                    if (pos == 4) {
                        content += '<li class="third know" id="' + value.id + '"><a href="javascript:;" class="set doc"><img src="../chat/logo.png"/>' + value.name + '<span class="hide downLoad fa fa-download"></span></a></li>';
                    } else if (pos == 2) {
                    
                    } else if (pos == 3) {
                        var number = value.type ? value.type : '';
                        var onStyle = 'ss';
                        if (!value.onLine && value.type == 1) {
                            onStyle = 'grayFilter'
                        }
                        content += '<li class="second know ' + onStyle + '" number="' + number + '" id="' + value.id + '"><a href="javascript:;" class="set doc"><img class="memPhoto" src="' + value.headImage + '"/>&nbsp;' + value.name + '</a></li>';
                    } else if (pos == 0) {
                        var onStyle = 'ss';
                        if (!value.onLine) {
                            onStyle = 'grayFilter'
                        }
                        var m = 'hjksslowspqwpwqpqpwpqpwqolqwolqwlwpqwpw[q[q[wq'
                        content += '\
                                    <li class="second know '+ onStyle + '" number="' + number + '" id="' + value.contactId + '">\
                                    <a href="javascript:;" class="set doc"><img src="' + value.headImage + '" class="posImg memPhoto"/>\
                                    <div class="linkDetail">\
                                    <div class=linkTop>' + value.contactName + '</div>\
                                    <div class=linkBottom style="color:#bfbfbf">' + value.station + '</div>\
                                    </div>\
                                    <span class="delPerson hide fa fa-times"></span>\
                                    </a>\
                                    </li>\
                                    ';
                    } else {
                        if (value.type == 1) {
                            content += '<li class="second know" number="' + number + '" id="' + value.groupId + '"><a href="javascript:;" class="set doc"><img  class="posImg memPhoto" src="' + value.headImage + '"/><div class="linkDetail">' + value.groupName + '</div></a></li>';
                        } else {
                            content += '<li class="second know" number="' + number + '" id="' + value.groupId + '"><a href="javascript:;" class="set doc"><img  class="posImg memPhoto" src="' + value.headImage + '"/><div class="linkDetail">' + value.groupName + '</div><span class="quit hide fa fa-times" create=' + value.createUser + '></span></a></li>';
                        }
                       
                    }

                });
                var bottom = "</ul>";
                var all = head + content + bottom;
                $(obj).append(all);              
                $(obj).children('ul').eq(1).remove();
              
                newMsg(data);                 //头像闪烁
                $(obj).siblings('.person').children('ul').remove();
                if (pos == 2) {
                    $(obj).children('ul').attr('id', "organzieTree").addClass('ztree');
                    chatTree(data)
                }

                deletePerson(data);
                
            })
        });
    }




    //头像闪烁
    //messageList = [
    //{ 'sendUser': { 'userId': '49' },message:1111111111,type:3 },
    //{ 'sendUser': { 'userId': '77' } ,message:222222222,type:3},
    //]
    function newMsg(data) {
        var listPerson = data 
        var hasNew = [];
        $.each(messageList, function (n, val) {
            var tempId = val.groupId || val.sendUser.userId;
            hasNew.push(parseInt(tempId,10));

        })
        $.each(listPerson, function (n, val) {
            var tempUser = val.groupId || val.contactId || val.id;
            if (hasNew.indexOf(tempUser) > -1) {
                $('.second').eq(n).find('img').addClass('listenNew');
            }
        })


    }

    //最近会话新消息提示
    function latestNew() {

        if (messageList.length > 0) {
            $.each(messageList, function (n, val) {
                if (val.type != -1) {
                    $('.newGif').removeClass('hide');
                    return;
                }
            })
        }
      

    }



    //组织结构
    function chatTree(data) {
       
       //$.each(data, function (n, val) {
       //    if (val.icon == null) {
       //        delete val.icon;
       //    }
        //})

        
       $('.relation li').eq(2).unbind('click')
    
        var folderTree = $.fn.zTree.init($("#organzieTree"), $.extend({
            callback: {
                onCollapse: function (e, id, node) {
                
                   
                },
                onExpand: function (e, id, node) {
                    e.stopPropagation();
   
                },
                beforeClick: function (e,id, node) {
                    //folderTree.checkNode(node, undefined, undefined, true);
                    //e.stopPropagation();
                    //return false;
                   
                },
                onClick: function (e, id, node) {
                    e.stopPropagation();
                    if (node.type == 1) {
                        return;
                    }
                    toTalk = {
                        contactId: node.id,
                        contactName: node.name,
                        headImage: node.icon
                        
                    }
                    //$('.person').eq(2).children('ul').remove();
                    $('.top .showGroup').addClass('hide');
                    chatDlg();

                }
            }
        }, {
            view: {
            //showIcon: false,
            showLine: false,
            selectedMulti: false
            },
        async: {
            enable: true,
            url: "/IMMessage/GetOrgAndUserList",
            autoParam: ["id=parent"],
            dataFilter: function (treeId, parentNode, responseData) {
                //$.each(responseData.data, function (n, val) {
                //    if (val.icon == null) {
                //        delete val.icon;
                //    }
                //})
                return responseData.data;
            }
        }
        }), data);
    }

    function loadHistoryMessage(msg, isload, nHeight) {
        var isMe = $('#userId').html();
        $.ajax({
            type: "post",
            url: "/IMMessage/GetHistoryMessage",
            dataType: "json",
            data: { data: JSON.stringify(msg)},
            success: rsHandler(function (data) {
                var talk = '';
                $.each(data, function (n, val) {
                    var nameStyle = val.userId == isMe ? 'host' : 'guest';
                    val.userName = val.userId == isMe ? '我' : val.userName;
                    val.sendTime = val.sendTime.replace('T', ' ');
                    val.sendTime = val.sendTime.substring(0,19)
                    switch (val.type) {
                        
                        case 3:
                            talk += '\
                                    <li>\
                                    <div class=' + nameStyle + '>' + val.userName + '\
                                    <span style="margin-left:10px;color:#bfbfbf">' + val.sendTime + '</span>\
                                    </div>\
                                    <div class="dataBottom">' + val.message + '\
                                    </div>\
                                    </li>\
                                   ';
                            break;
                        case 4:
                            talk += '\
                                    <li>\
                                    <div class=' + nameStyle + '>' + val.userName + '\
                                    <span style="margin-left:10px;color:#bfbfbf">' + val.sendTime + '</span>\
                                    </div>\
                                    <div class="dataBottom">\
                                     <img class="historyImg" src=' + '/' + $("#filePath").html() + '/' + val.fileName + ' />\
                                    </div>\
                                    </li>\
                                   ';
                            break;
                        case 5:
                            talk += '\
                                    <li>\
                                    <div class=' + nameStyle + '>' + val.userName + '\
                                    <span style="margin-left:10px;color:#bfbfbf">' + val.sendTime + '</span>\
                                    </div>\
                                    <div class="dataBottom">\
                                    <div class="wrapFile" style="width:190px;">\
                                    <div class="fileName">\
                                    <span class="fa fa-file-code-o code"></span>\
                                    <span class="fileIntro fileOut" style="width:65%" title='+val.message+'><p>' + val.message + '</p></span>\
                                    </div>\
                                    <div class="fileDown"><a href="javasipt:;" class="fileDownload" fileName='+val.fileName+' message='+val.message+'><span class="fa fa-download" style="margin-top:6px;"><span></a></div>\
                                    </div>\
                                    </div>\
                                    </li>\
                                   ';
                            break;

                    }
                  

                });
 
     
                if (isload) {
                    $('.historyBody ul').prepend(talk);               
                    if (!data | data.length < 10) {
                        $('.noMore').html('已全部加载完');
                        //$('#domMouse').off('scroll');
                        $('.historyBody').scrollTop(0);
                        return;
                    } else {
                        $('.historyBody').scrollTop(nHeight);
                    }
                
                } else {
                    $('.historyBody ul').append(talk);
                    if (!data | data.length < 10) {
                        $('.noMore').html('已全部加载完');
                        //$('#domMouse').off('scroll');
                        return;
                    }
                    $('.historyBody').scrollTop($('.historyBody')[0].scrollHeight);
                    //$('#domMouse').off('scroll');
                    //$('#domMouse').scroll(function (e) {
                    //    scollHistory();
                    //})

                }
                                


                $(".fileDownload").click(function () {
                    var fileName = $(this).attr('fileName');
                    var message = $(this).attr('message');
                    $.post("/IMMessage/DownloadFile", { displayName:message, saveName:fileName, flag: 0 }, function (data) {
                        if (data == "success") {
                            //loadViewToMain("/IMMessage/DownloadFile?displayName=" + escape(message) + "&saveName=" + fileName + "&flag=1");
                            window.location.href = "/IMMessage/DownloadFile?displayName=" + escape(message) + "&saveName=" + fileName + "&flag=1";
                        }
                        else {
                            ncUnits.alert("下载失败");
                        }
                    });
                });



            })
        });
    }
    //messageList = [
    //    { type: -1, message: '降价大出血，买一送一', sendtime: '2015-08-25T13:14:48.623',sendUser:{id:-1} },
    //    { type: -1, message: '只要一块钱，你买不了吃块，买不了上当', sendtime: '2015-08-25T13:14:48.623', sendUser: { id: -1 } },
    //    { type: -1, message: '江南皮革厂老板带小姨子携款跑路了，皮包挥泪大甩卖', sendtime: '2015-08-25T13:14:48.623', sendUser: { id: -1 } },
    //    { type: -1, message: '江南皮革厂老板带小姨子携款跑路了，皮包挥泪大甩卖', sendtime: '2015-08-24T15:14:48.623', sendUser: { id: -1 } },
    //    { type: -1, message: '江南皮革厂老板带小姨子携款跑路了，皮包挥泪大甩卖', sendtime: '2015-08-23T15:14:48.623', sendUser: { id: -1 } }
    //]

    var isNote = [
    { type: -1, message: '111', sendtime: '2015-08-25T13:14:48.623', sendUser: { id: -1 } },
    { type: -1, message: '222', sendtime: '2015-08-25T13:14:48.623', sendUser: { id: -1 } },
    { type: -1, message: '5555555555', sendtime: '2015-08-25T13:14:48.623', sendUser: { id: -1 } },
    { type: -1, message: '66666666666', sendtime: '2015-08-24T15:14:48.623', sendUser: { id: -1 } },
    { type: -1, message: '888888888888', sendtime: '2015-08-23T15:14:48.623', sendUser: { id: -1 } }
    ]

    //系统消息提示计数
    function noteCount() {
        var iCount = 0;
        $.each(messageList, function (n, val) {
            if (val.sendUser.id == -1) {
                iCount++
            }
        })
        if (iCount == 0) {
            $('.countAll').html('')
        } else {
            $('.countAll').html('(' + iCount + ')');
        }
        
    }
    noteCount();


    //系统消息接收弹窗
    $('.systemNote').on('click', function () {
        if ($('#showNote').length>0) {
            $('.noteList').remove();
            return;
        }
       
        pageLimit = {};

        var html = '\
                   <div class="noteList" id="showNote">\
                    <div class="noteHead"><span class="fa fa-volume-up volume"></span>系统管理员\
                    <span class="fa fa-times closeNote"></span>\
                    </div>\
                    <ul class=listNotes></ul>\
                    <div class="noteBottom"><span class="fa fa-chevron-right pageNote plus"></span><span class="fa fa-chevron-left pageNote minus"></span><span class="pageNum"></span></div>\
                    </div>\
                   ';

        $('body').append(html);
        loadNotes();
        var listMsg = [];
        for (k in messageList) {
            var tempId = messageList[k].sendUser.id;
            if (tempId == -1) {
                listMsg.push(messageList[k]);
                delete messageList[k];
            }

        }
        var pushMessage = [];
        $.each(messageList, function (i, val) {
            if (typeof (val) != 'undefined') {
                pushMessage.push(val)
            }
        })
        messageList = pushMessage;
        noteCount();


    })


    $(document).on('click', '.closeNote', function () {
        $('.noteList').remove();
        pageList = 0;
        pageLimit = {};
      

    })

    var pageList = 0;
    var pageLimit = {};
    //分页加载系统消息
    $(document).on('click', '.minus', function (e) {
        if (pageList <= 1) {
            return;
        }
        pageList--;
        var searchNote = {
            page: pageList,
            id: -1
        }

        $('.pageNum').html('当前页码' + pageList);
       
        //$.ajax({
        //    type: "post",
        //    url: "/IMMessage/GetHistoryMessage",
        //    dataType: "json",
        //    data: searchNote,
        //    success: rsHandler(function (data) {
                    var dataNote = isNote;
                    loadNotes(dataNote, 1);
                    

        //    })
        //})
       
 


    })

    $(document).on('click', '.plus', function (e) {
        if (pageLimit.plus == pageList) {
            return;
        }
        pageList++;
        var searchNote = {
            page: pageList,
            id: -1
        }
    
        $('.pageNum').html('当前页码' + pageList);

        //$.ajax({
        //    type: "post",
        //    url: "/IMMessage/GetHistoryMessage",
        //    dataType: "json",
        //    data: searchNote,
        //    success: rsHandler(function (data) {
        var dataNote = isNote;
        loadNotes(dataNote, 1);
        if (pageList == 3) {
            pageLimit.plus = pageList;
        }

        //    })
        //})

    })





    function loadNotes(dataNote,isload) {
        //将系统通知分类成不同日期
        var timeSet = [];
        var messageListTrue = isload ? dataNote : messageList;
        if (isload) {
            $('.listNotes').html('');
            $.each(messageListTrue, function (n, val) {
                var temp = val.sendtime.substring(0, 10);
                    if (timeSet.indexOf(temp) == -1) {
                        timeSet.push(temp)
                    }
            })

        } else {
            $.each(messageListTrue, function (n, val) {
                var temp = val.sendtime.substring(0, 10);
                if (val.sendUser.id == -1) {
                    if (timeSet.indexOf(temp) == -1) {
                        timeSet.push(temp)
                    }
                }

            })

        }

        if (timeSet.length == 0 && !isload) {
            var notice = '<div style="width:100%;line-height:30px;color:#bfbfbf;text-align:center;">暂无最新消息</div>';
            $('.listNotes').html(notice);
            return

        }
    
        var groupTime = ''
        $.each(timeSet, function (i, value) {
            groupTime += '<li class="groupTime" id=' + value + '><div class="timeUp"><span>' + value + '</span></div><ul class="mianList"></ul></li>'

        })
        $('.listNotes').html(groupTime);
       

        if (isload) {
            $.each(messageListTrue, function (n, val) {
                var t = val.sendtime.substring(0, 10);
                var tid = '#' + t;
                    var note = '\
                        <li>\
                        <div class="noteTop"><span style="color:#58B456;">系统消息</span><span style="margin-left:10px;color:#58B456">' + val.sendtime.substring(11, 19) + '</span></div>\
                         <div class="notemain">'+ val.message + '</div>\
                        </li>\
                        ';
                    $(tid).find('ul').append(note);
            })

        } else {
            $.each(messageListTrue, function (n, val) {
                var t = val.sendtime.substring(0, 10);
                var tid = '#' + t;
                if (val.sendUser.id == -1) {
                    var note = '\
                        <li>\
                        <div class="noteTop"><span style="color:#58B456;">系统消息</span><span style="margin-left:10px;color:#58B456">' + val.sendtime.substring(11, 19) + '</span></div>\
                         <div class="notemain">'+ val.message + '</div>\
                        </li>\
                        ';
                    $(tid).find('ul').append(note);
                }
            })
        }

     

    }





})

