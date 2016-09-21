/**
 * Created by ZETA on 2015/5/20.
 * 应用到bootstrap的页面通用的JS
 */
$(function () {
    /* tooltips加载 */
    $('[data-toggle="tooltip"]').tooltip();

    /* sortBar加载 */
    $(".sortbar").sortBar();

    /* 紧急度、重要度 开始 */
    fnLevel();
    /* 紧急度、重要度 结束 */
    /* 下拉框 开始 */
    $('.arrowsBBBDiv li').hover(function () {
        $(this).css({ 'background-color': '#58b456' });
        $('a', this).css({ 'color': '#fff' });
    }, function () {
        $(this).css({ 'background-color': '#fff' });
        $('a', this).css({ 'color': '#686868' });
    });
    /* 下拉框 结束 */

    /* 搜索页跳转开始*/
    $(document).on('click', '.searchClick', function (e) {
        searchKeyword()

    })
    $(document).on('keydown', '.autoSearch', function (e) {
        e.stopPropagation();
        if (e.keyCode == 13) {
            searchKeyword()
        }
        
    })
    function searchKeyword() {        
        var word = $.trim($('.autoSearch').val());
        word = word.replace(/\s+/g, "@");
        if (!word) {
            ncUnits.alert('请输入关键字');
            return;
        }
        if (!word) return;
        loadViewToMain('/Search/Search?tagNames=' + word);
    }


    /* 搜索页跳转结束*/



});
