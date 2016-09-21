/**
 * Created by DELL on 2015/9/22.
 */
//$(function () {
    loadPersonalInfo();
    //获取预约信息列表
    var defaultOpt = {
        year: new Date().getFullYear(),
        month: (new Date().getMonth() + 1) < 10 ? '0' + (new Date().getMonth() + 1) : (new Date().getMonth() + 1),
        day: new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate()
    };

    getList();
    function getList(date){
        reDraw();
        var time = date || defaultOpt.year + '-' + defaultOpt.month + '-' + defaultOpt.day;
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetMeetingRoomAppointment",
            data: { time:time},
            dataType: "json",
            success: rsHandler(function (data) {

                var $html = '';
                $.each(data, function (n, value) {
                    $.each(value.meeting, function (i, val) {
                        var isAttach = val.isHasAttachment ? '':'hideAttach';
                        $html += '\
                                <div class="col-xs-4 meetMsg" mid='+ val.meetingId + ' rid=' + value.room.roomId + '>\
                                   <div><span>时间:</span>'+ val.startTime.substring(11,16) + '-' + val.endTime.substring(11, 16) + '</div>\
                                   <div><span>地点:</span>'+ value.room.roomName + '</div>\
                                   <div><span>原因:</span><span data-toggle="tooltip" title=' + val.content + '>' + val.content + '</span></div>\
                                   <div><span>预约人:</span>' + val.createUser + '</div>\
                                   <div class="downLoadAppoint ' + isAttach + ' " mid=' + val.meetingId + '><i class="fa fa-paperclip"></i><div class="downBottom"></div><div class="trangle"></div></div>\
                                </div>\
                                ';

                    })
                  
                })
                $('.listDate').html($html);
                $('[data-toggle="tooltip"]').tooltip();
                drawDate(data);
              
            })
        })

    }


    //下载附件开始
    $('#meetTabContent').on('click', '.downLoadAppoint', function (e) {
       
        e.stopPropagation();
        var meetingId = $(this).attr('mid');
        var that = $(this);
        if (that.parent('.meetMsg').find('.attachAll').length > 0) {
            that.parent('.meetMsg').find('.attachAll').remove();
            return;
        }
        var p = that.parent('.meetMsg');
        appendAttach(p, meetingId);

    })
    $('#meetContainer').on('click', '.downAttach', function (e) {
        e.stopPropagation();
        var meetingId = $(this).attr('mid');
        var that = $(this);
        if (that.parent('div').find('.attachAll').length > 0) {
            that.parent('div').find('.attachAll').remove();
            return;
        }
        var p = that.parent('div');
        appendAttach(p, meetingId);;
    })

    function appendAttach(p,meetingId) {
        var p = p;
        var meetingId = meetingId;
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetMeetingAttachmentList",
            data: { meetingId: parseInt(meetingId, 10) },
            dataType: "json",
            success: rsHandler(function (data) {
                var $all = '<div class=attachAll><div class="attachHead"><a href="#" class="downloadAll" mid=' + meetingId + '>全部下载</a></div></div>';
                p.append($all);
                var $attach = '';
                $.each(data, function (n, val) {
                    $attach += '<div class="listDownloads"><span class="glyphicon glyphicon-eye-open prview" onclick=preview(9,"' + val.saveName + '","' + val.extension + '")></span><span class="glyphicon glyphicon-download-alt downloadSingle" saveName=' + val.saveName + '></span><span>' + val.attachmentName + '</span></div>'

                })
                p.find('.attachAll').append($attach);

            })
        })

    }
   


    $('#meetContainer').on('click', '.attachAll', function (e) {
        e.stopPropagation();
    })
    $('#meetContainer').on('click', '.downloadAll', function (e) {
        var meetingId = parseInt($(this).attr('mid'));
        downloadAll(meetingId);
    })

    $('#meetContainer').on('click', '.downloadSingle', function (e) {
        var attach = $(this).next().text();
        var save = $(this).attr('saveName');
        downloadSingle(attach,save);
    })
 
    //单个文件下载
    function downloadSingle(attach,save) {
        var attachmentName = attach;
        var saveName = save;
        
        $.post("/MeetingRoom/Download", { displayName: attachmentName, saveName: saveName, flag: 0 }, function (data) {
            if (data == "success") {
                //loadViewToMain("/plan/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1");
                window.location.href = "/MeetingRoom/Download?displayName=" + escape(attachmentName) + "&saveName=" + saveName + "&flag=1";
            }
            return;
        });

    }

    //全部下载
    function downloadAll(meetingId) {
        $.post("/MeetingRoom/MultiDownload", { meetingId: meetingId, flag: 0 }, function (data) {
            if (data == "success") {
                //loadViewToMain("/plan/MultiDownload?planId=" + planId + "&flag=1");
                window.location.href = "/MeetingRoom/MultiDownload?meetingId=" + meetingId + "&flag=1";
            }
            return;
        });
    }


    //下载附件结束

    $('#meetContainer').on('click', '.meetMsg', function (e) {
        $(this).addClass('selected').siblings().removeClass('selected');
        $(this).find('.downLoadAppoint').addClass('downFilter');
        $(this).find('.trangle').addClass('tragleBorder');
        $(this).siblings().find('.downLoadAppoint').removeClass('downFilter');
        $(this).siblings().find('.trangle').removeClass('tragleBorder');
        var id = '#m'+ $(this).attr('mid');
        $(id).addClass('selectedColor').siblings('.drawPic').removeClass('selectedColor');
        $(id).parents('.draw').siblings('.draw').find('.drawPic').removeClass('selectedColor');

    });


    //切换日期
    $('.rightClick').on('click',function(){
        $('.secondArea').animate({'left':'-100%'});
        $('.thirdArea').animate({'left':'0'});
        var moveWidth = $('.drawBody').width();
        $('.drawPic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old-moveWidth+'px'});
        })
    })
    $('.rightToClick').on('click',function(){
        $('.secondArea').animate({'left':'0'});
        $('.firstArea').animate({'left':'-100%'});
        var moveWidth = $('.drawBody').width();
        $('.drawPic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old-moveWidth+'px'});
        })
    })
    $('.leftClick').on('click',function(){
        $('.secondArea').animate({'left':'0'});
        $('.thirdArea').animate({'left':'-100%'});
        var moveWidth = $('.drawBody').width();
        $('.drawPic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old+moveWidth+'px'});
        })
    })
    $('.leftToClick').on('click',function(){
        $('.secondArea').animate({'left':'100%'});
        $('.firstArea').animate({'left':'0%'});
        var moveWidth = $('.drawBody').width();
        $('.drawPic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old+moveWidth+'px'});
        })
    })


    //按日期绘制图形
    function drawDate(data){
        //var d = setData(data);
        var d = data;
        //$('#bydate').css('display','block');
        //var w = $('.areaHead').width()/12;
        var w = 895.5/12;
        var $list = '';
        $.each(d,function(n,val){
            $list +='\
                      <div class="draw">\
                        <div class="col-xs-1 drawHead" title='+val.room.roomName+'>'+val.room.roomName+'</div>\
                        <div class="col-xs-11 drawBody"></div>\
                      </div>\
                    '
        });
        $('.areaBody').html($list);

        $('.areaBody .draw').each(function(idx){
            var $pic = '';
            $.each(d[idx].meeting,function(n,val){
                var width = computeWidth(val.startTime.substring(11, 16), val.endTime.substring(11, 16), w) + 'px';
                var left = computeStart(val.startTime.substring(11, 16), w) + 37 + 'px'
                var id = 'm' + val.meetingId;
                $pic += '\
                        <div id='+id+' class="drawPic" style="width:'+width+';left:'+left+'"></div>\
                        '
            })
            $(this).find('.drawBody').html($pic);

        })

    }

    function reDraw(){
        $('.secondArea').css({'left':'0'});
        $('.firstArea').css({'left':'-100%'});
        $('.thirdArea').css({'left':'100%'});
        $('.areaBody').html('');

    }

    function computeStart(start,w){

        var startLen = start.length;
        var startInt = parseInt(start.substr(0,startLen-2),10);
        var startFloat = parseInt(start.substr(-2),10)/60;
        var start = startInt+startFloat;

        var left = (start-8)*w;
        return left;
    }

    function computeWidth(start,end,w){
        var startLen = start.length;
        var endLen = end.length;
        //console.log('ensssd',end);
        //console.log('ensssd',endLen);
        var startInt = parseInt(start.substr(0,startLen-3),10);
        var startFloat = parseInt(start.substr(-2),10)/60;
        var endInt = parseInt(end.substring(0,startLen-2),10);
        var endFloat = parseInt(end.substr(-2),10)/60;
        var start = startInt+startFloat;
        var end = endInt+endFloat;
        //console.log('start',start);
        //console.log('end',end);
        var width = (end-start)*w;
        return width;

    }

    function setData(data){
        var result = [];
        var ids = [];
        $.each(data,function(n,val){
            var temp = {
                rid:val.room.roomId,
                room:val.room.roomName,
                attach: [{ start: val.meeting.startTime, end: val.meeting.endTime, mid: val.meeting.meetingId }]
                };
            if(n == 0){
                ids.push(val.room.roomId);
                result.push(temp);
            }else{
                var index = ids.indexOf(temp.rid)
                if(index>0){
                    result[index].attach.push(temp.attach[0]);
                }else{
                    result.push(temp);
                    ids.push(val.room.roomId);
                }

            }
        });
        return result;

    }


  //初始化日历
    $.datepicker.setDefaults({
        closeText: "确认", // Display text for close link
        prevText: "上个月", // Display text for previous month link
        nextText: "下个月", // Display text for next month link
        currentText: "本月", // Display text for current month link
        monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"], // Names of months for drop-down and formatting
        monthNamesShort: ["一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二"], // For formatting
        dayNames: ["周日", "周一", "周二", "周三", "周四", "周五", "周六"], // For formatting
        dayNamesShort: ["日", "一", "二", "三", "四", "五", "六"], // For formatting
        dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"], // Column headings for days starting at Sunday
        weekHeader: "周", // Column header for week of the year
        dateFormat: "yy-mm-dd", // See format options on parseDate
        firstDay: 1 // The first day of the week, Sun = 0, Mon = 1, ...
    });

    $( "#meetDate" ).datepicker({
        onSelect:function (dateText, instance){
            getList(dateText);

        }
    });
   
   
    $('#dateChange').on('click', function () {
        getList();
        $("#meetDate").datepicker('setDate', new Date());
              
    });

    $('#siteChange').on('click',function(){
        getSite();
    })



    $('#meetContainer').on('selectstart', '.appointHead,.listApp', function () {
        return false
    })

    function getSite(){
        var time = defaultOpt.year + '-' + defaultOpt.month + '-' + defaultOpt.day;
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetMeetingRoomAppointment",
            data: { time: time },
            dataType: "json",
            success: rsHandler(function (data) {
                siteList(data);


            })
        })



    }

  

    function siteList(siteData) {
        var preDate = getDateLine(1);
        var midDate = getDateLine(2);
        var lastDate = getDateLine(3);
        var $area = '';
        $.each(siteData, function (n, val) {
            var equ = [];
            $.each(val.equipment, function (i, value) {
                equ.push(value.equipmentName);
            })
            var equName = equ.join('，');
            var id = 'collapse' + val.room.roomId;
            var lines = drawSiteLine(val.meeting);//画线
            var listApp = '';
            
            $.each(val.meeting, function (m, d) {
                var hasDownLoad = val.isHasAttachment ? '<i class="fa fa-paperclip downAttach" mid=' + d.meetingId + '></i>' : '';
                listApp += '\
                            <div class="listApp" idx=' + m + '>\
                                <div class="col-xs-1">' + hasDownLoad + '</div>\
                                <div class="col-xs-4 ellipsis">' + d.startTime.substring(11, 16) + '-' + d.endTime.substring(11, 16) + '</div>\
                                <div class="col-xs-4 ellipsis" data-toggle="tooltip" title=' + d.content + '>' + d.content + '</div>\
                                <div class="col-xs-3 ellipsis">' + d.createUser + '</div>\
                            </div>\
                        '

            });
            var seat = val.room.seating ? val.room.seating : '';
            $area += '\
                    <div class="panel panel-default siteList" id="room' + val.room.roomId + '">\
                        <div class="panel-body siteBody">\
                            <div class="col-xs-2 roomName">'+ val.room.roomName + '</div>\
                            <div class="col-xs-9 roomDetail">\
                                <div class="col-xs-8 ellipsis">位置：<span data-toggle="tooltip" title=' + val.room.position + '>' + val.room.position + '</span></div>\
                                <div class="col-sx-4 ellipsis">座位数：' + seat + '</div>\
                                <div class="col-xs-12 ellipsis">描述：<span data-toggle="tooltip" title=' + equName + '>' + equName + '</span></div>\
                            </div>\
                            <div class="col-xs-1 appointRoom">\
                                <button type="button" class="appointBtn newAppoint"  data-toggle="modal">预约</button>\
                            </div>\
                            <div class="dateLine col-xs-12">\
                                <div class="dateLineDraw">\
                                    <div class="preDate dateShow">'+ preDate + '</div>\
                                    <div class="midDate dateShow">'+ midDate + '</div>\
                                    <div class="lastDate dateShow">'+ lastDate + '</div>\
                                    <div class="siteDateLine">'+ lines + '</div>\
                                </div>\
                                <a class="dateMeetMsg" role="button" data-toggle="collapse" href="#'+ id + '" aria-expanded="false" aria-controls="collapseExample">\
                                    <i class="fa fa-chevron-down"></i>\
                                </a>\
                            </div>\
                            <div class="collapse col-xs-12 listDateAppoint" id='+ id + '>\
                           <div class="appointHead" rid='+ val.room.roomId + '>\
                                    <i class="fa fa-chevron-left minusDay"></i>\
                                    <span class="yearNum">'+ defaultOpt.year + '</span>-\
                                    <span class="monNum">'+ defaultOpt.month + '</span>-\
                                    <span class="dayNum">'+ defaultOpt.day + '</span>\
                                    <i class="fa fa-chevron-right plusDay"></i>\
                                </div>\
                                <div class="appointBody">\
                                    <div class="bodyHead">\
                                        <div class=col-xs-1></div>\
                                        <div class="col-xs-4">时间</div>\
                                        <div class="col-xs-4">内容</div>\
                                        <div class="col-xs-3">预约人</div>\
                                    </div>'+ listApp + '\
                                </div>\
                            </div>\
                        </div>\
                    </div>\
                  '
        });
        $('#bySite').html($area);
        $('[data-toggle="tooltip"]').tooltip();
    }

    function drawSiteLine(draws){
        var w = 853.5/12;
        var $result = '';
        $.each(draws,function(n,val){
            var width = computeWidth(val.startTime.substring(11, 16), val.endTime.substring(11, 16), w) + 'px';
            var left = computeStart(val.startTime.substring(11, 16), w) + 37 + 'px'
            $result += '\
                        <div class="drawSitePic" style="width:'+width+';left:'+left+'"></div>\
                        '
        })
        return $result;
    }

    $('#meetContainer').on('click', '.listApp', function () {
        var index = $(this).attr('idx');
        $(this).addClass('listAppGray').siblings('.listApp').removeClass('listAppGray');
        $(this).parents('.siteBody').find('.drawSitePic').eq(index).addClass('siteColor').siblings('.drawSitePic').removeClass('siteColor');
    })

    //切换日期
    $('#meetContainer').on('click', '.rightTo', function () {
        $(this).parents('.dateLineDraw').find('.midDate').animate({'left':'-100%'});
        $(this).parents('.dateLineDraw').find('.lastDate').animate({'left':'0'});
        var moveWidth = $('.siteDateLine').width();
        $(this).parents('.dateLineDraw').find('.drawSitePic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old-moveWidth+'px'});
        })
    })
    $('#meetContainer').on('click', '.rightToUp', function () {
        $(this).parents('.dateLineDraw').find('.midDate').animate({'left':'0'});
        $(this).parents('.dateLineDraw').find('.preDate').animate({'left':'-100%'});
        var moveWidth = $('.siteDateLine').width();
        $(this).parents('.dateLineDraw').find('.drawSitePic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old-moveWidth+'px'});
        })
    })
    $('#meetContainer').on('click', '.leftTo', function () {
        $(this).parents('.dateLineDraw').find('.midDate').animate({'left':'0'});
        $(this).parents('.dateLineDraw').find('.lastDate').animate({'left':'-100%'});
        var moveWidth = $('.siteDateLine').width();
        $(this).parents('.dateLineDraw').find('.drawSitePic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old+moveWidth+'px'});
        })
    })
    $('#meetContainer').on('click', '.leftToUp', function () {
        $(this).parents('.dateLineDraw').find('.midDate').animate({'left':'100%'});
        $(this).parents('.dateLineDraw').find('.preDate').animate({'left':'0'});
        var moveWidth = $('.siteDateLine').width();
        $(this).parents('.dateLineDraw').find('.drawSitePic').each(function(n){
            var old = $(this).position().left;
            $(this).animate({'left':old+moveWidth+'px'});
        })
    })




    function getDateLine(type){
       var result = '';
        switch(type){
            case 1:
                result = '<div class="col-xs-1 col-xs-offset-4">00:00</div>' +
                         '<div class="col-xs-1">01:00</div><div class="col-xs-1">02:00</div><div class="col-xs-1">03:00</div><div class="col-xs-1">04:00</div><div class="col-xs-1">05:00</div><div class="col-xs-1">06:00</div>' +
                         '<div class="col-xs-1">07:00<i class="fa fa-chevron-right rightChevron rightToUp"></i></div>';
                break;
            case 2:
                result = '<div class="col-xs-1">08:00<i class="fa fa-chevron-left leftChevron leftToUp"></i></div>' +
                '<div class="col-xs-1">09:00</div><div class="col-xs-1">10:00</div><div class="col-xs-1">11:00</div><div class="col-xs-1">12:00</div><div class="col-xs-1">13:00</div><div class="col-xs-1">14:00</div><div class="col-xs-1">15:00</div><div class="col-xs-1">16:00</div><div class="col-xs-1">17:00</div><div class="col-xs-1">18:00</div>' +
                '<div class="col-xs-1">19:00<i class="fa fa-chevron-right rightChevron rightTo"></i></div>';
                break;
            default:
                result =  '<div class="col-xs-1">20:00<i class="fa fa-chevron-left leftChevron leftTo"></i></div>' +
                '<div class="col-xs-1">21:00</div><div class="col-xs-1">22:00</div><div class="col-xs-1">23:00</div>'+
                '<div class="col-xs-1">24:00</div>';


        }


        return result;
    }

    //日期选择
    $('#meetContainer').on('click', '.plusDay', function () {
        var index = $(this).parents('.siteList').index();
        var year = parseInt($(this).siblings('.yearNum').html(),10);
        var month = parseInt($(this).siblings('.monNum').html(),10);
        var day = parseInt($(this).siblings('.dayNum').html(),10);;
        var dayGet = new Date(year,month,0);
        var dayCount = dayGet.getDate();
        day++;
        if(day>dayCount){
            day = 1;
            month++;
        }

        $(this).siblings('.dayNum').html(day<10?'0'+day:day);
        if(month>12){
            month = 1;
            year++;
            $(this).siblings('.yearNum').html(year);
        }
        $(this).siblings('.monNum').html(month < 10 ? '0' + month : month);

        var pushData = {
            time:year+'-'+addZero(month)+'-'+addZero(day),
            roomId:$(this).parent('.appointHead').attr('rid')
        };
        getSingle(pushData,index)

    })
    $('#meetContainer').on('click', '.minusDay', function () {
        var index = $(this).parents('.siteList').index();
        var year = parseInt($(this).siblings('.yearNum').html(),10);
        var month = parseInt($(this).siblings('.monNum').html(),10);
        var day = parseInt($(this).siblings('.dayNum').html(),10);;
        day--;
        if(day<1){
            month--;
        }
        if(month<1){
            month = 12;
            year--;
        }
        var dayGet = new Date(year,month,0);
        var dayCount = dayGet.getDate();
        day = day < 1 ? dayCount : day;
        $(this).siblings('.dayNum').html(day < 10 ? '0' + day : day);
        $(this).siblings('.yearNum').html(year);
        $(this).siblings('.monNum').html(month<10?'0'+month:month);
        var pushData = {
            time:year+'-'+addZero(month)+'-'+addZero(day),
            roomId:$(this).parent('.appointHead').attr('rid')
        }
        getSingle(pushData,index)

    })

    function getSingle(pushData, index) {
        var idx = index;
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetAppointmentDetail",
            data: pushData,
            dataType: "json",
            success: rsHandler(function (data) {
                console.log('sss', data);
                reSite();
                $('.siteList').eq(idx).find('.listApp').remove();
                var $single = '';
                $.each(data, function (n, val) {
                    var hasDownLoad = val.isHasAttachment ? '<i class="fa fa-paperclip downAttach"  mid=' + val.meetingId + '></i>' : '';
                    $single += '\
                            <div class="listApp" idx='+ n + '>\
                                <div class="col-xs-1">' + hasDownLoad + '</div>\
                                <div class="col-xs-4 ellipsis">' + val.startTime.substring(11, 16) + '-' + val.endTime.substring(11, 16) + '</div>\
                                <div class="col-xs-4 ellipsis" data-toggle="tooltip" title=' + val.content + '>' + val.content + '</div>\
                                <div class="col-xs-3 ellipsis">' + val.createUser + '</div>\
                            </div>\
                        '
                });
                $('.siteList').eq(idx).find('.appointBody').append($single);
                var lines = drawSiteLine(data);
                $('.siteList').eq(idx).find('.siteDateLine').html(lines);
                $('[data-toggle="tooltip"]').tooltip();

            })
        })
    }

    function reSite() {
        $('.midDate').css({ 'left': '0' });
        $('.preDate ').css({ 'left': '-100%' });
        $('.lastDate').css({ 'left': '100%' });


    }

    function addZero(d){
        d = d<10?'0'+d:d;
        return d;
    }









//})
