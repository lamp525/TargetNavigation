$( function() {
/* 导航条一级菜单高亮 开始 */
//fnNav (0);
/* 导航条一级菜单高亮 结束 */



    //新闻图片轮播
    newsBannerRander("/News/AjaxImageNewsData");

    /* 搜索 开始 */
        $('.newsSearch').focus( function () {
            if ( $(this).val()=='搜索' ) {
                $(this).val('');
                $(this).css({'color':'#686868'});
            }
        });
        $('.newsSearch').blur( function () {
            if ( $(this).val()=='' ) {
                $(this).val('搜索');
                $(this).css({'color':'#e7e7e7'});
            }
        });
    /* 搜索 结束 */
    
    
    /* 获取新闻类别（newsNavList） 开始 */
        var url = '/News/getDirectory';

    NavListLoader('new',url);
    /* 获取新闻类别（newsNavList） 结束 */

    /* 获取重要新闻 开始 */
    function importantNewsLoader(parent,url){
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data:{
                count:5
            },
            success:rsHandler(function(data){
                var html = '';
                $.each(data,function(index,value){
                    html+='<li><span class="tagOfNew"><a href="'+value.href+'">'+value.tag+'</a></span>'+
                            '<span class="summaryOfNew">'+value.summary+'</span>'+
                            '<span class="dateOfNew">'+value.date+'</span></li>';
                });
                $(parent+' ul').append(html);                                                                    
            })
        });
    }
    importantNewsLoader('.importantNewsListL','');
    importantNewsLoader('.importantNewsListR','');
    /* 获取重要新闻 结束 */
    /* 获取其他新闻列表 开始 */
    
    generalLoader('/News/GetNew',{DriID:0}, 'New')
    /* 获取其他新闻列表 结束 */
});