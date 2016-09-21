
/**
 * Created by DELL on 2015/9/28.
 */
$(function(){
    loadPersonalInfo();
    roomList();

    function roomList() {
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetMeetingRoomList",
            dataType: "json",
            success: rsHandler(function (data) {
                var $list = '';
                $.each(data, function (n, val) {
                    var equ = [];
                    $.each(val.equipment, function (i, value) {
                        equ.push(value.equipmentName);
                    })
                    var equName = equ.join('，');
                    var seat = val.room.seating ? val.room.seating : '';
                    $list += '\
                    <tr>\
                        <td class="overHidden col-xs-2">'+ val.room.roomName + '</td>\
                        <td class="overHidden col-xs-2"><div class="showEquName"  data-toggle="tooltip" title=' + val.room.position + '>' + val.room.position + '</div></td>\
                        <td class="overHidden col-xs-2">' + seat + '</td>\
                        <td class="overHidden col-xs-2"><div class="showEquName"  data-toggle="tooltip" title=' + equName + '>' + equName + '</div></td>\
                        <td class="overHidden col-xs-2"><div class="showEquName"  data-toggle="tooltip" title=' + val.room.comment + '>' + val.room.comment + '</div></td>\
                        <td class="col-xs-2">\
                            <a href="#" rid='+ val.room.roomId + ' class="editRoom hideOpt" >修改</a>\
                            <a href="#" rid='+ val.room.roomId + ' class="deleteRoom hideOpt">删除</a>\
                        </td>\
                    </tr>\
                 '
                })
                $('#manageRoom tbody').html($list);
                $('[data-toggle="tooltip"]').tooltip()


            })
        })
       

    };

    $('#manageContainer').on('click', '.editRoom', function () {

        var idx = $(this).parents('tr').index();
        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetMeetingRoomInfo",
            data:{roomId:$(this).attr('rid')},
            dataType: "json",
            success: rsHandler(function (data) {
                editRoom(data, idx)

            })
        })
       
    });
    $('#manageContainer').on('click', '.deleteRoom', function () {
        var idx = $(this).parents('tr').index();
        var that = $(this);
        ncUnits.confirm({
            title: '提示',
            html: '是否确认删除该会议室？',
            yes: function (layerID) {
              
                layer.close(layerID);

                $.ajax({
                    type: "post",
                    url: "/MeetingRoom/DeleteMeetingRoom",
                    dataType: "json",
                    data: { roomId: that.attr('rid')},
                    success: rsHandler(function (data) {
                     
                        if (data == true) {
                            ncUnits.alert("删除成功!");
                 
                            that.parents('tr').remove();
                          
                        } else {
                            ncUnits.alert("删除失败!");
                        }
                    })
                });
            }
        });


    });

    $(document).on('mouseover', '#manageRoom tbody tr', function () {
        $(this).addClass('trBack');
        $(this).find('a').removeClass('hideOpt');
    })
    $(document).on('mouseleave', '#manageRoom tbody tr', function () {
        $(this).removeClass('trBack');
        $(this).find('a').addClass('hideOpt');
    })



    //编辑会议室
    function editRoom(room,index){
        var room = room;
        var index = index;
        var equ = [];
        $.each(room.equipment, function (i, value) {
            equ.push(value.equipmentId);
        });
        
        getEquipment();
        $('.selectEqu').each(function (n) {
            var id = parseInt($(this).val(),10);
            if (equ.indexOf(id) > -1) {
             
                $(this).attr('checked', true);
            }
        })

        $('#manage_modal_label').html('修改');
        $('#manage_modal').modal();
        $('.roomName').val(room.room.roomName);
        $('.roomPosition').val(room.room.position);
        $('.roomSeating').val(room.room.seating);
        $('.roomComment').val(room.room.comment);
        $('.roomId').val(room.room.roomId);
        $('#manage_modal_submit').attr('index',index)

    }

    //新增会议室
    $('.newRoom').on('click',function(){
        getEquipment();
        $('.roomId').val('');
        $('#manage_modal #manage_modal_label').html('新增');
        $('#manage_modal .setInput').val("");
        $('#manage_modal').modal();
        $('#manage_modal_submit').removeAttr('index');
    })



    $('#manage_modal_submit').on('click',function(){
        var postData = getPost();
        var index = parseInt($('#manage_modal_submit').attr('index'));
        postData.result.room.roomId = postData.result.room.roomId ? postData.result.room.roomId : null;
        if (!postData.result.room.roomName) {
            ncUnits.alert('请输入会议室名');
            return;
        }
        var res;
     
        $('.setInput').each(function (n) {
            var txt = $(this).val();
            var name = $(this).parent().siblings('.manage_room_head').text();
            name = name.replace(':', '');
            res = justifyByLetter(txt, name);
            if (res === 2) {
                return false;
            }
           
        })
        if (res === 2) {
            
            return;
        }
        if (!postData.result.room.position) {
            ncUnits.alert('请输入会议室位置');
            return;
        }

        $.ajax({
            type: "post",
            url: "/MeetingRoom/SaveMeetingRoom",
            data: { data: JSON.stringify(postData.result)},
            dataType: "json",
            success: rsHandler(function (data) { 
                if (data) {
                    postData.result.room.roomId = data;
                }
                if (data == 2) {
                    ncUnits.alert('会议室名重复！');
                } else {
                    if (index >= 0) {
                        setTable(index, postData);
                        ncUnits.alert('修改成功');
                    } else {
                        addTable(postData)
                        ncUnits.alert('新增成功');
                    }
                }
            })       
        })
       

        $("#manage_modal").modal("hide");
        $('#manage_modal .selectEqu').attr('checked',false);
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


    function setTable(idx, data) {
       
        $('#manageRoom tbody tr').eq(idx).find('td').first().html(data.result.room.roomName);
        $('#manageRoom tbody tr').eq(idx).find('td').eq(1).html(data.result.room.position);
        $('#manageRoom tbody tr').eq(idx).find('td').eq(2).html(data.result.room.seating);
        $('#manageRoom tbody tr').eq(idx).find('td').eq(3).find('.showEquName').html(data.equName.join(','));
        $('#manageRoom tbody tr').eq(idx).find('td').eq(3).find('.showEquName').attr('title',data.equName.join(','));
        $('#manageRoom tbody tr').eq(idx).find('td').eq(4).html(data.result.room.comment);
    }

    function addTable(data) {
        var $table ='\
                    <tr>\
                        <td class="overHidden">'+data.result.room.roomName+'</td>\
                        <td class="overHidden"><div class="showEquName"  data-toggle="tooltip" title=' + data.result.room.position + '>' + data.result.room.position + '</div></td>\
                        <td class="overHidden">'+data.result.room.seating+'</td>\
                        <td class="overHidden"><div class="showEquName"  data-toggle="tooltip" title=' + data.equName.join(',') + '>' + data.equName.join(',') + '</div></td>\
                        <td class="overHidden"><div class="showEquName"  data-toggle="tooltip" title=' + data.result.room.comment + '>' + data.result.room.comment + '</div></td>\
                        <td>\
                            <a href="#" rid='+data.result.room.roomId+' class="editRoom hideOpt" >修改</a>\
                            <a href="#" rid='+data.result.room.roomId+' class="deleteRoom hideOpt">删除</a>\
                        </td>\
                    </tr>\
                 ';
        $('#manageRoom tbody').append($table);
        $('[data-toggle="tooltip"]').tooltip()
    }


    function getPost(){
        var data = {};
        data.equName = [];
        data.result = {
            room:{
                roomId: $('.roomId').val(),
                roomName: $('.roomName').val(),
                position: $('.roomPosition').val(),
                comment: $('.roomComment').val(),
                seating: $('.roomSeating').val()
            },
            equipmentId:[]
        };
        $('.selectEqu').each(function(n){

            if(this.checked){
                data.result.equipmentId.push($(this).val());
                data.equName.push($(this).next().html());
            }
        })

        return data;
    }

    function getEquipment(equ) {
        var equ = equ;

        $.ajax({
            type: "post",
            url: "/MeetingRoom/GetEquipmentList",
            dataType: "json",
            async: false,
            success: rsHandler(function (data) {
                var $list = '';
                $.each(data, function (n, val) {
                    $list += '\
                        <div class="col-xs-3">\
                            <input type="checkbox" class="selectEqu"  value=' + val.equipmentId + ' id=' + val.equipmentId + '><label for=' + val.equipmentId + '>' + val.equipmentName + '</label>\
                        </div>\
                    '
                })
                $('.checkEqu').html($list);
              
                
            })
        })
   
       
    }


























})