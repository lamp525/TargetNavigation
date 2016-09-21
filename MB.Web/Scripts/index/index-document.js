$( function() {
/* 导航条一级菜单高亮 开始 */
//fnNav (0);
/* 导航条一级菜单高亮 结束 */



    /* 搜索 开始 */
        $('.documentsSearch').focus( function () {
            if ( $(this).val()=='搜索' ) {
                $(this).val('');
                $(this).css({'color':'#686868'});
            }
        });
        $('.documentsSearch').blur( function () {
            if ( $(this).val()=='' ) {
                $(this).val('搜索');
                $(this).css({'color':'#e7e7e7'});
            }
        });
    /* 搜索 开始 */
    /* 获取文档类别 开始 */
    var url1='../../test/data/index/index_company_newsNavListData.json',
        url2='../../test/data/index/index_company_newsCategoryUpData.json',
        url3='../../test/data/index/index_company_newsCategoryDownData.json';

    NavListLoader('document',url1,url2,url3);
    /* 获取文档类别 结束 */
    /* 获取其他新闻列表 开始 */
    //加载普通新闻、通知、文档列表
    function generalDocumentLoader(parent,url,type){
        $.ajax({
            type: "post",
            url: url,
            dataType: "json",
            data:{
                count:8
            },
            success:rsHandler(function(data){
                var html = '';
                $.each(data,function(index,value){
                    html+='<li><span class="summaryOf'+type+'">'+value.summary+'</span>'+
                                '<span class="dateOf'+type+'">'+value.date+'</span>'+
                                '<div class="liHover"><em class="bg"></em><em class="down">下载</em>'+
                                '<em class="updown"></em><input type="hidden" value="true" /></div></li>';


                });
                $(parent+' ul').append(html);  
                $(parent+' ul li').hover(function () {
                        $('.liHover',this).toggle();
                });                                                                  
            })
        });
    }
    generalDocumentLoader('.generalDocumentsList','../../test/data/index/index_company_generalNewsData.json','Document')
    /* 获取其他新闻列表 结束 */

    
});