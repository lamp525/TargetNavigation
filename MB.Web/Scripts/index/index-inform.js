$( function() {
/* 导航条一级菜单高亮 开始 */
//fnNav (0);
/* 导航条一级菜单高亮 结束 */



	/* 搜索 开始 */
		$('.informsSearch').focus( function () {
			if ( $(this).val()=='搜索' ) {
				$(this).val('');
				$(this).css({'color':'#686868'});
			}
		});
		$('.informsSearch').blur( function () {
			if ( $(this).val()=='' ) {
				$(this).val('搜索');
				$(this).css({'color':'#e7e7e7'});
			}
		});
	/* 搜索 开始 */
	
	
	
	/* 获取通知类别 开始 */
    var url1='../../test/data/index/index_company_newsNavListData.json',
        url2='../../test/data/index/index_company_newsCategoryUpData.json',
        url3='../../test/data/index/index_company_newsCategoryDownData.json';

    NavListLoader('inform',url1,url2,url3);
    /* 获取通知类别 结束 */
    /* 获取其他通知列表 开始 */
    
    generalLoader('.generalInformsList','../../test/data/index/index_company_generalNewsData.json','Inform')
    /* 获取其他通知列表 结束 */
});





















