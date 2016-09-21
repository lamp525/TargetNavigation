/* 个人资料设置 开始 */
$('.conR .mine .myself').on('click', function () {
    $.layer({
        type: 1,
        shade: [0],
        area: ['auto', 'auto'],
        title: false,
        border: [0],
        page: { dom: '.perData' }
    });
});
$('.perData .set .modify').click(function () {
    if ($(this).parents('.set').find('.list input:eq(0)').hasClass('inputHit')) {
        if ($(this).parents('.set').hasClass('password')) {
            $(this).parents('.set').find('.list input:eq(0)').val('********');
        }
        $(this).css({ 'background': 'url(../../Images/plan/modify.png) no-repeat' });
        $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
    }
    else {
        if ($(this).parents('.set').hasClass('password')) {
            $(this).parents('.set').find('.list input:eq(0)').val('');
        }
        $(this).css({ 'background': 'url(../../Images/plan/chooseBlue.png) no-repeat' });
        $(this).parents('.set').find('.list input').addClass('inputHit').removeAttr('readonly');
    }
});

$(function () {
    ////获取今日未完成的计划列表
    //$(".planUl .xxc_personplan").click(function () {
    //    var obj = $(this).attr("flag");
    //    $("#xxc_planList").load("/Plan/GetUnCompleteOrOver", { flag: obj }, function () {
    //        //回到默认排序
    //        $('.headL span').eq(0).addClass('spanHit').siblings().removeClass('spanHit');
    //        //清空筛选数据
    //        emptydata();
    //    });
    //});
});
/* 个人资料设置 结束 */