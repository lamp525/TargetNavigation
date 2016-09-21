/**
 * Created by DELL on 2015/9/28.
 */

function myList(query) {
    var query = query || { startTime: null, endTime: null, status: 0 };
    $.ajax({
        type: "post",
        url: "/MeetingRoom/GetMyAppointment",
        data: { data: JSON.stringify(query) },
        dataType: "json",
        success: rsHandler(function (data) {
            var $my = '';
            $.each(data, function (n, val) {
                var status = val.isComplete ? '已完成' : '未进行';
                $my += '\
                      <div class="col-xs-4 wrapList">\
                         <div class="wrapDetail">\
                             <div class="detailLine">\
                                <span class="labelOne"></span>\
                                <div class="labelTwo">'+ status + '</div>\
                                <span class="labelThree"></span>\
                             </div>\
                             <div class="detailLine ellipsis"><span>会议室名称:</span>' + val.roomName + '</div>\
                             <div class="detailLine ellipsis"><span>时间:</span>' + val.startTime.substr(0, 10) + '&nbsp&nbsp' + val.startTime.substr(11) + ' -' + val.endTime.substr(11) + '</div>\
                             <div class="detailLine ellipsis"><span>事项:</span><span data-toggle="tooltip" title=' + val.content + '>' + val.content + '<span></div>\
                             <div class="detailLine ellipsis"><span>人数:</span>' + val.member + '</div>\
                             <div class="showLine hideOpt"><a href="#" class="appointPlan" aid='+ val.meetingId + '>计划</a><a href="#" class="appointCancel" aid=' + val.meetingId + '>取消</a></div>\
                         </div>\
                      </div>\
                    '
            })
            $('.myList').html($my);
            $('[data-toggle="tooltip"]').tooltip()

        })
    })




};
$(function(){
    loadPersonalInfo();
    var startTime = {
        elem: '#startCondition',
        event: 'click',
        format: 'YYYY-MM-DD',
        istoday: true,
        issure: true,
        festival: true,
        choose: function (dates) {

        },
        clear: function () {
        }
    } ;
    var endTime = {
        elem: '#endCondition',
        event: 'click',
        format: 'YYYY-MM-DD',
        istoday: true,
        issure: true,
        festival: true,
        choose: function (dates) {

        },
        clear: function () {
        }
    }
    laydate(startTime);
    laydate(endTime);

    myList();


    $(document).on('mouseover','.wrapDetail',function(){
        $(this).find('.showLine').stop().animate({'height':'40px'});
    });
    $(document).on('mouseleave','.wrapDetail',function(){
        $(this).find('.showLine').stop().animate({'height':'0px'});
    });

    //顶部条件筛选
    $('.checkType button').on('click',function(){
        $(this).addClass('btnSelected').siblings().removeClass('btnSelected');
        $('#typeSelected').val($(this).attr('sid'));
        var postData = {
            start: $('#startCondition').val() ? $('#startCondition').val() : null,
            end: $('#endCondition').val() ? $('#endCondition').val() : null,
            status: parseInt($(this).attr('sid'), 10)
        }

        myList(postData);
    });
    $('.confirmCondition').on('click',function(){
        var postData = {
            start:$('#startCondition').val()?$('#startCondition').val():null,
            end:$('#endCondition').val()?$('#endCondition').val():null,
            status:parseInt($('#typeSelected').val(),10)
        }
        myList(postData);
    })



    //取消预约
    $('#appointContainer').on('click', '.appointCancel', function () {
        var index = $(this).parents('.wrapList').index();
        var mid = $(this).attr('aid');
        $('#appoint_modal').modal();
        $('#appoint_modal_submit').attr('idx',index);
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetMeetingInfo",
            data: { meetingId:mid},
            dataType: "json",
            success: rsHandler(function (data) {
        
                $('#appoint_modal .appointTime').find('span').first().html(data.startTime.replace("T", " "));
                $('#appoint_modal .appointTime').find('span').last().html(data.endTime.replace("T", " "));
                $('#appoint_modal .appointContent').html(data.content);
                $('#appoint_modal .appointMain').html(data.speakerName.join('，'));
                $('#appoint_modal .appointAttend').html(data.participantUser.join('，'));
                $('#appoint_modal .appointId').val(data.meetingId);
           
            })
        })

    });

    $('#appoint_modal_submit').on('click',function(){
        var mid = $('#appoint_modal .appointId').val();
        var index = $(this).attr('idx');
        $.ajax({
            type: "post",
            url: "/MeetingRoom/CancelAppointment",
            data: {meetingId:parseInt(mid)},
            dataType: "json",
            success: rsHandler(function (data) {
                ncUnits.alert('已取消预约');
                $('.wrapList').eq(index).remove();
                $('#appoint_modal').modal('hide');
           
            })
        })
       
    });
    var planCreate = addPlan();
    $('#appPlan_modal').on('click', '.addPlan', function () {
        var mid = $(this).attr('mid');
        planCreate.addPlan(mid);
        $('#appPlan_modal').modal('hide');
    })


    //计划
    $('#appointContainer').on('click', '.appointPlan', function () {
        var mid = $(this).attr('aid');
        $('#appPlan_modal #appPlan_modal_submit').attr('mid',mid)
     $('#appPlan_modal').modal();
     $('#appPlan_modal .modal-body .row').html('');
     $.ajax({
         type: "post",
         url: "/MeetingRoom/GetPlanList",
         data: { meetingId:parseInt(mid,10)},
         dataType: "json",
         success: rsHandler(function (data) {
             console.log('ss', data);
             var $plan = '';
             $.each(data, function (n, val) {
                 val.smallImage = val.smallImage ? val.smallImage : '../../Images/index/news/news0.png';
                 $plan += '\
                   <div class="col-xs-6 planList">\
                      <div class="wrapPlan">\
                        <div class="wrapTop">\
                            <div class="planIcon">\
                                <img src='+ val.smallImage + ' alt=""/>\
                            </div>\
                            <div class="planMsg">\
                                <div>执行方式:'+ val.executionMode + '</div>\
                                <div class="ellipsis">输出对象:<span data-toggle="tooltip" title=' + val.eventOutput + '>' + val.eventOutput + '</div>\
                                <div>计划完成时间:'+ val.endTime.replace("T"," ") + '</div>\
                                <div>确认人:'+ val.confirmUser + '</div>\
                            </div>\
                            <div class="responseUser">\
                                <span>'+ val.responsibleUser + '</span><span>' + val.createTime.substring(0,10) + '</span>\
                            </div>\
                        </div>\
                      </div>\
                   </div>\
                ';
                 $('#appPlan_modal .modal-body .row').html($plan);
                 $('[data-toggle="tooltip"]').tooltip()
             })


         })
     })

     //var data = [
     //    {planId:1,executionMode:'修改完成', eventOutput:'《流程管理》',responsibleUser:'张三','smallImage':'',confirmUser:'李四',endTime:'2015-06-16 16:00',createTime:'2015-06-14'},
     //    {planId:2,executionMode:'开发完成', eventOutput:'《模块管理》',responsibleUser:'李四','smallImage':'',confirmUser:'李四',endTime:'2015-06-17 12:00',createTime:'2015-06-11'},
     //    {planId:3,executionMode:'修改完成', eventOutput:'《表单管理》',responsibleUser:'王五','smallImage':'',confirmUser:'李四',endTime:'2015-06-18 05:00',createTime:'2015-06-11'},
     //    {planId:4,executionMode:'开发完成', eventOutput:'《项目管理》',responsibleUser:'赵六','smallImage':'',confirmUser:'李四',endTime:'2015-06-19 06:00',createTime:'2015-06-12'},
     //    {planId:5,executionMode:'修改完成', eventOutput:'《计划管理》',responsibleUser:'钱一','smallImage':'',confirmUser:'李四',endTime:'2015-06-16 18:00',createTime:'2015-06-15'}
     //];


 })











})