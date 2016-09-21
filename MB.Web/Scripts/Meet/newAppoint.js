/**
 * Created by DELL on 2015/9/28.
 */
$(function () {
    //文件上传
    var partternFile = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|7z|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
    var j = 1;
    var uploadFiles = [];
    var period = [];
    fileUpload();
    function fileUpload() {
        $('#fileuploadAppoint').fileupload({
            url: '/MeetingRoom/UploadDocument',
            dataType: 'text',
            send: function (e, data) {
            },
            add: function (e, data) {
                var isSubmit = true;
                $.each(data.files, function (index, value) {
                    if (value.size > 209715200) {
                        ncUnits.alert("上传文件过大(最大200M)");

                        isSubmit = false;
                        return;
                    } else {

                         if (!partternFile.test(value.name)) {
                                ncUnits.alert("上传文件格式不对");
                                isSubmit = false;
                                return;
                           }

                         var html = '\
                                      <li>\
                                         <span>'+data.files[0].name+'</span><i class="fa fa-remove removeFiles"></i>\
                                      </li>\
                                    ';
                         $('.fileList ul').append(html);
                       
                    }
                });
                if (isSubmit) {
                    data.submit();
                }
            },
            complete: function (data, flag) {
              
            },
            error: function (e, data) {
                ncUnits.alert('error');
            },
            done: function (e, data) {
                var result = JSON.parse(data.result);
                console.log('fisssle', result);
                if (result.data.displayName != null) {
                    ncUnits.alert("上传成功");
                    var temp = {
                        displayName: result.data.displayName,
                        saveName: result.data.saveName,
                        extension: result.data.extension
                    }
                    uploadFiles.push(temp);

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
         
          
                
            },
            always: function (e, data) {
            }
        })


    }


    //文件上传结束
    //删除文件

    $(document).on('click', '.removeFiles', function () {
        var index = $(this).parent('li').index();
        $(this).parent('li').remove();
        uploadFiles.splice(index, 1);
    })




    $('.clockpicker').clockpicker();
    $('.clockpickerTwo').clockpicker();
    //预约弹窗
    var startConfig = {
        elem: '#startDateMeet',
        event: 'click',
        format: 'YYYY-MM-DD',
        istoday: true,
        issure: true,
        festival: true,
        choose: function (dates) {
            $('#endDateMeet').val(dates);
            var rid = $('.roomHidden').val();
            if (!rid) {
                return;
            }
            var time = $('#startDateMeet').val();
            getTime(time, rid);

        },
        clear: function () {
            $('#endDateMeet').val('');
            $('.appointTimeSet .timeBody').find('.timeLimit').remove();
        }
    } ;
    //var endConfig = {
    //    elem: '#endDateMeet',
    //    event: 'click',
    //    format: 'YYYY-MM-DD',
    //    istoday: true,
    //    issure: true,
    //    festival: true,
    //    choose: function (dates) {

    //    },
    //    clear: function () {
    //    }
    //}
    laydate(startConfig);
    //laydate(endConfig);

    var mainTalk = {
        id:[],
        name:[]
    };
    var mainAttend = {
        id:[],
        name:[]
    };
    var now = {
        year: parseInt(new Date().getFullYear(),10),
        month: parseInt(new Date().getMonth() + 1, 10),
        day: parseInt(new Date().getDate(), 10)
    }
   
    //$('#endDateMeet').val(new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + new Date().getDate());

    $('.appointCommon').on('click', '.newAppoint', function () {
        $('#meeting_modal').modal();
        $('#startDateMeet,#endDateMeet').val(now.year + "-" + (now.month < 10 ? '0' + now.month : now.month) + "-" + (now.day < 10 ? '0' + now.day : now.day));
        getRooom();


    });
    $('.addMeeting').on('click', function () {
        $('#meeting_modal').modal();
        $('#startDateMeet,#endDateMeet').val(now.year + "-" + (now.month < 10 ? '0' + now.month : now.month) + "-" + (now.day < 10 ? '0' + now.day : now.day));
        getRooom();

    });

    function getRooom() {
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetSimpleRoomList",
            dataType: "json",           
            success: rsHandler(function (data) {
                var $list = '';
                $.each(data, function (n, val) {
                    $list += '<li><a href="#" rid=' + val.roomId + ' class="selectedRoom">' + val.roomName + '</a></li>';
                })
                $('.roomSelect').html($list);

            })
        })
    }

    function isPeriod(period, upData) {
        var periodlimit = period;
        var startPer = dataFilter(upData.startTime);
        var endPer = dataFilter(upData.endTime);
        var result = '2';
        $.each(periodlimit, function (n,val) {
            var tStart = dataFilter(val.startTime);
            var tEnd = dataFilter(val.endTime);
            if ((startPer >= tStart && startPer <= tEnd) || (endPer >= tStart && endPer <= tEnd) || (startPer<=tStart && endPer>=tEnd)) {
                result = '1';
                return;
            }
        })
        return result;

    }

    function dataFilter(t) {
        var result = parseInt(t.substr(11, 2), 10) + parseInt(t.substr(11, 2), 10) / 60;
        return result;
    }


    $('#meeting_modal_submit').on('click', function () {
        var speechUser = [];
        var joinUser = [];

  
        $('.mainFirst div').each(function (n) {
            speechUser.push(parseInt($(this).attr('mid')));
        })
        $('.mainSecond div').each(function (n) {
            joinUser.push(parseInt($(this).attr('mid')));
        })
        var upData = {
            roomId: $('.roomHidden').val(),
            startTime: $('#startDateMeet').val() + ' ' + $('.clockpicker input').val(),
            endTime: $('#endDateMeet').val() + ' ' + $('.clockpickerTwo input').val(),
            content: $('#contentMeet').val(),
            speechUser: speechUser,
            joinUser: joinUser,
            file: uploadFiles

        };
        console.log('开始时间', dataFilter(upData.startTime));
        console.log('结束时间', dataFilter(upData.endTime));
        console.log('update', upData);
        if (!upData.roomId) {
            ncUnits.alert('请选择会议室');
            return false;
        }
        if (!$('#startDateMeet').val() || !$('.clockpicker input').val()) {
            ncUnits.alert('请输入开始时间');
            return false;
        }
        if (!$('#endDateMeet').val() || !$('.clockpickerTwo input').val()) {
            ncUnits.alert('请输入结束时间');
            return false;
        }
        if (dataFilter(upData.startTime) >= dataFilter(upData.endTime)) {
            ncUnits.alert('结束时间要大于开始时间');
            return false;

        }
    



        //判断预约时间是否已存在
        var isPer = isPeriod(period, upData);
        if (isPer == '1') {
            ncUnits.alert('该时间段已被预约,点击预约情况查看详情')
            return false;
        }


        if (upData.speechUser.length == 0) {
            ncUnits.alert('请输入主讲人员');
            return false;
        }
        if (upData.joinUser.length == 0) {
            ncUnits.alert('请输入参加人员');
            return false;
        }

        var res = justifyByLetter(upData.content,'预约事项');
        if (res === 2) {
            return false;
        }


        $.ajax({
            type: "post",
            url: "/MeetingRoom/SaveAppointment",
            data: { data: JSON.stringify(upData)},
            dataType: "json",
            success: rsHandler(function (data) {
                ncUnits.alert('新建会议成功');
                if ($("#meetDate").length > 0) {
                    var listTime = $("#meetDate").datepicker('getDate');
                    if (listTime) {
                        listTime = listTime.getFullYear() + "-" + (listTime.getMonth() + 1) + "-" + listTime.getDate();
                        if ($('#byDate').hasClass('active') && listTime == $('#startDateMeet').val()) {
                            getList(listTime)

                        }
                    }

                }

                if ($('.myList').length > 0) {
                    myList({ startTime: null, endTime: null, status: 0 });
                    $('.checkType button').first().addClass('btnSelected').siblings().removeClass('btnSelected');
                    $('#typeSelected').val('0');
                    $('#startCondition,#endCondition').val('');
                }

                if ($('#meetContainer').length > 0 && $('#bySite').hasClass('active')) {
                    var id = '#room' + upData.roomId;
                    var selectTime = $(id).find('.yearNum').text() + '-' + $(id).find('.monNum').text() + '-'+ $(id).find('.dayNum').text()
                    if ($('#startDateMeet').val() != selectTime) {
                        $("#meeting_modal").modal("hide");
                        return

                    }
                    var idx = $(id).find('.listApp').last().attr('idx');
                    var tidx =   idx == undefined ?0:parseInt(idx, 10) + 1;
                    var $html ='\
                            <div class="listApp" idx='+ tidx + '>\
                                <div class="col-xs-4 col-xs-offset-1 ellipsis">' + upData.startTime.substring(11, 16) + '-' + upData.endTime.substring(11, 16) + '</div>\
                                <div class="col-xs-4 ellipsis" data-toggle="tooltip" title=' + upData.content + '>' + upData.content + '</div>\
                                <div class="col-xs-3 ellipsis">' + $(".personal_info_name").first().text() + '</div>\
                            </div>\
                                ';
                
                    $(id).find('.appointBody').append($html);
                    var w = 853.5 / 12;
                    var width = computeWidth(upData.startTime.substring(11, 16), upData.endTime.substring(11, 16), w) + 'px';
                    var left = computeStart(upData.startTime.substring(11, 16), w) + 37 + 'px'
                    var $result = '<div class="drawSitePic" style="width:' + width + ';left:' + left + '"></div>';
                    $(id).find('.siteDateLine').append($result);

                }


                
                $("#meeting_modal").modal("hide");

            })
        })

    })


    //判断某变量是否具有非法字符
    function justifyByLetter(txt, name) {
        var reg = /<\w+>/;
        var result = 1;
        if (txt.indexOf('null') >= 0 || txt.indexOf('NULL') >= 0 || txt.indexOf('&nbsp') >= 0 || reg.test(txt) || txt.indexOf('</') >= 0) {
            name = name + "存在非法字符!";
            ncUnits.alert(name);
            result = 2;
        }
        return result;
    }

  


    $('#meeting_modal').on('hide.bs.modal', function (e) {
        mainTalk = {
            id:[],
            name:[]
        };
        mainAttend = {
            id:[],
            name:[]
        };
        $('.fileList ul').html('');
        uploadFiles = [];
        $('.memberTreeSelect').remove();
        $('#meeting_modal .memberList').html('');
        $('#meeting_modal .nameSelect .downName').html('');
        $('#meeting_modal .roomHidden').val('');
        $('#meeting_modal input').val('');
        $('#meeting_modal textarea').val('');
        $('.appointTimeSet').remove();
        $('#startDateMeet').val(new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + new Date().getDate());
        $('#endDateMeet').val(new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + new Date().getDate());

    })

    $('#meeting_modal').on('click', '.selectedRoom', function () {
        $('.roomHidden').val($(this).attr('rid'));
        $('.downName').html($(this).html());
    })

    //查看预约情况
    var time = [
        { start: '06:30', end: "07:30" },
        { start: '08:30', end: "09:30" },
        { start: '12:30', end: "20:30" },
    ]
    $('.appointSee').on('click', function () {
        if ($('.appointTimeSet').length > 0) {
            $('.appointTimeSet').remove();
            return;
        }
        if ($('.memberTreeSelect').length > 0) {
            $('.memberTreeSelect').remove();
        }
        var rid = $('.roomHidden').val();
        var time = $('#startDateMeet').val();
        if (!rid) {
            ncUnits.alert('请选择会议室');
            return;

        }
        if (!time) {
            ncUnits.alert('请选择时间');
            return;

        }

        var pre = getArea(1);
        var middle = getArea(2);
        var last = getArea(3);
        var $see = '<div class="appointTimeSet"><div class="timeHead chevronCommon"><i class="fa fa-chevron-up"></i></div><div class="timeBody">' + pre + '' + middle + '' + last + '</div><div class="timeFoot chevronCommon"><i class="fa fa-chevron-down"></i></div></div>';
        $('#meeting_modal .modal-dialog').append($see);
        
        getTime(time, rid);


    
    })


    $('#meeting_modal').on('click', '.selectedRoom', function () {
        var rid = $(this).attr('rid');
        var time = $('#startDateMeet').val();
        if (!time) {
            return;
        }
        getTime(time, rid);



    })


    function getTime(time,rid) {
        var time = time;
        var rid = rid;
        $('.appointPre').css('top', '-100%');
        $('.appointMiddle').css('top', 0);
        $('.appointLast').css('top', '100%');
        $('.appointTimeSet .timeBody').find('.timeLimit').remove();
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetAppointmentTimeDetail",
            data: { time: time, roomId: rid },
            dataType: 'json',
            success: rsHandler(function (data) {
                period = data;
                console.log('p',data)
                if (data.length == 0) {
                    return;
                }
               
                var $time = '';
                var h = $('.timeBody').height();
                var per = h / 8;
                $.each(data, function (n, val) {
                    var end = dataFilter(val.endTime);
                    var start = dataFilter(val.startTime);
                    var top = (start - 8) * per + 'px';
                    var height = (end - start) * per + 'px';
                    $time += '<div class="timeLimit" style="top:' + top + ';height:' + height + ';line-height:' + height + '"></div>'

                })
                $('.appointTimeSet .timeBody').prepend($time);

            })


        })

    }


    $('.appointSee').on('selectstart',function () {
        return false
    })

    $('#meeting_modal').on('selectstart', '.chevronCommon', function () {
        return false;
    })

    $('#meeting_modal').on('click','.timeHead',function () {
        var p1 = $('.appointPre').position().top;
        var p2 = $('.appointMiddle').position().top;
        var p3 = $('.appointLast').position().top;
        var h = $('.timeBody').height();
        if (p1 < 0 && p2 == 0 && p3 > 0) {
            $('.appointPre').animate({ 'top': 0 });
            $('.appointMiddle').animate({ 'top': '100%' });
            $('.timeLimit').each(function (n) {
                var old = $(this).position().top;
                $(this).animate({ 'top': old + h + 'px' });
            })
        }
        if (p1 < 0 && p2  < 0 && p3 == 0) {
            $('.appointLast').animate({ 'top': '100%' });
            $('.appointMiddle').animate({ 'top': 0 });
            $('.timeLimit').each(function (n) {
                var old = $(this).position().top;
                $(this).animate({ 'top': old + h + 'px' });
            })
        }
        
    })

    $('#meeting_modal').on('click', '.timeFoot', function () {
        var p1 = $('.appointPre').position().top;
        var p2 = $('.appointMiddle').position().top;
        var p3 = $('.appointLast').position().top;
        var h = $('.timeBody').height();
        if (p1  == 0 && p2 > 0 && p3 > 0) {
            $('.appointPre').animate({ 'top': '-100%' });
            $('.appointMiddle').animate({ 'top': 0 });
            $('.timeLimit').each(function (n) {
                var old = $(this).position().top;
                $(this).animate({ 'top': old - h + 'px' });
            })
        }
        if (p1 < 0 && p2  == 0 && p3 > 0) {
            $('.appointLast').animate({ 'top': 0 });
            $('.appointMiddle').animate({ 'top': '-100%' });
            $('.timeLimit').each(function (n) {
                var old = $(this).position().top;
                $(this).animate({ 'top': old - h + 'px' });
            })
        }
    })


    function getArea(type) {
        var $html = '';
        switch (type) {
            case 1:
                $html = '<div class="appointPre appointCommon"><div>00:00</div><div>01:00</div><div>02:00</div><div>03:00</div><div>04:00</div><div>05:00</div><div>06:00</div><div>07:00</div></div>';

                break;
            case 2:
                $html = '<div class="appointMiddle appointCommon"><div>08:00</div><div>09:00</div><div>10:00</div><div>11:00</div><div>12:00</div><div>13:00</div><div>14:00</div><div>15:00</div></div>';

                break;
            default:
                $html = '<div class="appointLast appointCommon"><div>16:00</div><div>17:00</div><div>18:00</div><div>19:00</div><div>20:00</div><div>21:00</div><div>22:00</div><div>23:00</div></div>';
        }
        return $html;

    }



    $('#meeting_modal').on('click', '.plusMain', function () {
        $('.memberTreeSelect').remove();
        if ($('.appointTimeSet').length > 0) {
            $('.appointTimeSet').remove();
        }
        var type = $(this).attr('type');
        var msg = type=='1'?'选择主讲人员':'选择参加人员';
        var $tree = '\
                    <div class="memberTreeSelect" >\
                        <div class="treeIntro">'+msg+'<i class="fa fa-remove"></i></div>\
                        <div class="treeMain"><ul class="ztree" id="treeRoom"></ul></div>\
                        <div class="isLower"><input type="checkbox" id="Meet-haschildren"/>选择下属<input type="checkbox" id="Meet-checkall"/>选择全部</div>\
                        <div class="treeChoose"><ul class="listCheck" type='+type+'></ul></div>\
                    </div>\
                 ';

        $('#meeting_modal .modal-dialog').append($tree);
        var data = getData();
        initTree(data);
    })
    function getData(){
        var result;
        $.ajax({
            type: "post",
            async:false,
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null },
            success: rsHandler(function(data){
                result = data

            })
        })
        return result;
    }



    function initTree(data){
        var type = $('.listCheck').attr('type');
        var folderTree = $.fn.zTree.init($("#treeRoom"), $.extend({
            callback: {
                beforeClick: function (e,id, node) {

                },
                onClick: function (e, id, node) {
                    var checked = $("#Meet-haschildren").prop('checked');
                    var data = {}
                    data.withSub = checked == true ? 1 : 0;
                    data.organizationId = node.id;
                    $('#Meet-haschildren').attr('nid', data.organizationId);
                   getListMember(data,type)
              

                }
            }
        }, {
            view: {
                showIcon: false,
                showLine: false,
                selectedMulti: false
            },
            async: {
                enable: true,
                url: "/Shared/GetOrganizationList",
                autoParam: ["id=parent"],
                dataFilter: function (treeId, parentNode, responseData) {
                    return responseData.data;
                }
            }
        }), data);
    }

    $('#meeting_modal').on('click', '#Meet-haschildren', function () {
        var data = {};
        data.organizationId = parseInt($('#Meet-haschildren').attr('nid'), 10);
        if (!data.organizationId) {
            return;
        }
        var checked = $("#Meet-haschildren").prop('checked');
        data.withSub = checked == true ? 1 : 0;
        var type = $('.listCheck').attr('type');
        getListMember(data,type)
    })

    function getListMember(d, type) {
        var type = type;
        var dataPost = d;
        var selector = type == '1' ? '.mainFirst' : '.mainSecond';
        $.ajax({
            type: 'post',
            url: '/Shared/GetUserList',
            dataType: 'json',
            data: dataPost,
            success: rsHandler(function (data) {
                var data = data
                var $member = '';
                $.each(data, function (n, val) {
                    $member += '\
                                            <li>\
                                                <a href="javascript:;" mid='+ val.userId + ' class="isSelectedMember" ><span>' + val.userName + '</span><i class="fa fa-check hideCheck"></i></a>\
                                            </li>\
                                          ';
                });
                $('.treeChoose ul').html($member);

                var listSelected = type == '1' ? mainTalk : mainAttend;
                if (type == '1') {
                    $('.listCheck li').each(function (n) {
                        var lid = $(this).find('a').attr('mid');
                        if (mainAttend.id.indexOf(lid) > -1) {
                            $(this).remove();
                        }
                    })
                } else {
                    
                    $('.listCheck li').each(function (n) {
                        var lid = $(this).find('a').attr('mid');
                        if (mainTalk.id.indexOf(lid) > -1) {
                            $(this).remove();
                        }
                    })
                }

                listChoose(listSelected);
                $('#Meet-checkall').off('click');
                $('#Meet-checkall').on('click', function () {
                    var checked = $(this).prop('checked');
                    var main = type == '1' ? mainTalk : mainAttend;
                    var $check = '';
                    if (checked) {
                        $('.listCheck li').each(function () {
                            if (checked) { }
                            var lid = $(this).find('a').attr('mid');
                            if (main.id.indexOf(lid) == -1) {
                                $check += '<div class="col-xs-2" mid=' + lid + '>' + $(this).find('span').html() + '<i class="fa fa-remove removeOut" type='+type+'></i></div>';
                                $(this).find('i').removeClass('hideCheck');
                                if (type == '1') {
                                    mainTalk.id.push(lid);
                                    mainTalk.name.push($(this).find('span').text());
                                } else {
                                    mainAttend.id.push(lid);
                                    mainAttend.name.push($(this).find('span').text());
                                }
                            }

                        })

                        $(selector).append($check);
                    } else {
                        var temp = [];
                        $('.listCheck li').each(function () {
                            if (checked) { }
                            var lid = $(this).find('a').attr('mid');
                            temp.push(lid);
                            $(this).find('i').addClass('hideCheck');
                        });
                        $(selector).find('div').each(function (n) {
                            var tid = $(this).attr('mid');
                            if (temp.indexOf(tid)>-1) {
                                $(this).remove();
                                var idx = main.id.indexOf(tid);
                                if (type == '1') {
                                    mainTalk.id.splice(idx, 1);
                                    mainTalk.name.splice(idx, 1);
                                } else {
                                    mainAttend.id.splice(idx,1);
                                    mainAttend.name.splice(idx, 1);
                                }

                            }

                        })

                    }
                    
                })


                $('.isSelectedMember').on('click', function (e) {
                    var mid = $(this).attr('mid');
                    
                    switch (type) {
                        case '1':
                            var idx = mainTalk.id.indexOf(mid);
                            if (idx > -1) {
                                mainTalk.id.splice(idx, 1);
                                mainTalk.name.splice(idx, 1);
                                $(this).find('i').addClass('hideCheck');
                                removeMem(selector, mid);
                            } else {
                                mainTalk.id.push(mid);
                                mainTalk.name.push($(this).html());
                                $(this).find('i').removeClass('hideCheck');
                                var $talk = '<div class="col-xs-2" mid=' + mid + '>' + $(this).find('span').html() + '<i class="fa fa-remove removeOut" type="1"></i></div>';
                                $(selector).append($talk);
                            };

                            break;
                        default:
                            var idx = mainAttend.id.indexOf(mid);
                            if (idx > -1) {
                                mainAttend.id.splice(idx, 1);
                                mainAttend.name.splice(idx, 1);
                                $(this).find('i').addClass('hideCheck');
                                removeMem(selector, mid);
                            } else {
                                mainAttend.id.push(mid);
                                mainAttend.name.push($(this).html());
                                $(this).find('i').removeClass('hideCheck');
                                var $talk = '<div class="col-xs-2" mid=' + mid + '>' + $(this).find('span').html() + '<i class="fa fa-remove removeOut" type="2"></i></div>';
                                $(selector).append($talk);

                            }

                    }



                })

            })
        })

    }



    function listChoose(obj){
        var obj = obj;
        $('.listCheck li').each(function (idx) {
            var tid = $(this).find('a').attr('mid');
            if(obj.id.indexOf(tid)>-1){
                $(this).find('i').removeClass('hideCheck');
            }
        });

    }
    function  removeMem(selector,mid){
        var mid = mid;
        $(selector+' div').each(function(idx){
            if($(this).attr('mid') == mid){
                $(this).remove();
                return;
            }
        })

    }

    $(document).on('click','.removeOut',function(e){
        var type = $(this).attr('type');
        var id = $(this).parent().attr('mid');
        if(type == '1'){
            var index = mainTalk.id.indexOf(id);
            mainTalk.id.splice(index,1);
            mainTalk.name.splice(index,1);

        }else{
            var index = mainAttend.id.indexOf(id);
            mainAttend.id.splice(index,1);
            mainAttend.name.splice(index,1);
        }

        setTreeSelect(id,type);
        $(this).parent().remove();
    });

    function setTreeSelect(id,type){
        var type =type;
        var id = id;
        if($('.treeChoose .listCheck').length ==0){
            return
        }
        var checkType = $('.listCheck').attr('type');
        if (type != checkType) {
            console.log('%c ooo','color:red')
            return;
        };
        $('.listCheck li').each(function(idx){
            var tid = $(this).find('a').attr('mid');
            if(id == tid && !$(this).find('i').hasClass('hideCheck')){
                $(this).find('i').addClass('hideCheck');
                return;
            }
        })

    }

    $(document).on('click','.treeIntro i',function(){
        $('.memberTreeSelect').remove();
    })




})