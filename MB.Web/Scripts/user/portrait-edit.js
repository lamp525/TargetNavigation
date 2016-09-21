$(function() {
    /* 数据交互 开始 */

    /* 数据交互 结束 */
    /* 确定跳转 开始 */
    $('.addPlan').click(function () {
        loadViewToMain("../plan/plan.html");
    });
    /* 确定跳转 结束 */

    // Create variables (in this scope) to hold the API and image size
    var jcrop_api,
        boundx,
        boundy,

    // Grab some information about the preview pane
        $preview = $('#preview-pane'),
        $pcnt = $('#preview-pane .preview-container'),
        $pimg = $('#preview-pane .preview-container img'),
        $preview1 = $('#preview-pane1'),
        $pcnt1 = $('#preview-pane1 .preview-container'),
        $pimg1 = $('#preview-pane1 .preview-container img'),
        $preview2 = $('#preview-pane2'),
        $pcnt2 = $('#preview-pane2 .preview-container'),
        $pimg2 = $('#preview-pane2 .preview-container img'),

        xsize = $pcnt.width(),
        ysize = $pcnt.height(),
        xsize1 = $pcnt1.width(),
        ysize1 = $pcnt1.height(),
        xsize2 = $pcnt2.width(),
        ysize2 = $pcnt2.height();

    $('#target').Jcrop({
        onChange: updatePreview,
        onSelect: updatePreview,
        aspectRatio: xsize / ysize
    },function(){
        // Use the API to get the real image size
        var bounds = this.getBounds();
        boundx = bounds[0];
        boundy = bounds[1];
        // Store the API in the jcrop_api variable
        jcrop_api = this;

        // Move the preview into the jcrop container for css positioning
        $preview.appendTo(jcrop_api.ui.holder);
        $preview1.appendTo(jcrop_api.ui.holder);
        $preview2.appendTo(jcrop_api.ui.holder);

        // 左边照片处理
        //fnPic ();
    });

    function updatePreview(c) {
        if (parseInt(c.w) > 0) {
            var rx = xsize / c.w;
            var ry = ysize / c.h;
            var rx1 = xsize1 / c.w;
            var ry1 = ysize1 / c.h;
            var rx2 = xsize2 / c.w;
            var ry2 = ysize2 / c.h;

            $pimg.css({
                width: Math.round(rx * boundx) + 'px',
                height: Math.round(ry * boundy) + 'px',
                marginLeft: '-' + Math.round(rx * c.x) + 'px',
                marginTop: '-' + Math.round(ry * c.y) + 'px'
            });
            $pimg1.css({
                width: Math.round(rx1 * boundx) + 'px',
                height: Math.round(ry1 * boundy) + 'px',
                marginLeft: '-' + Math.round(rx1 * c.x) + 'px',
                marginTop: '-' + Math.round(ry1 * c.y) + 'px'
            });
            $pimg2.css({
                width: Math.round(rx2 * boundx) + 'px',
                height: Math.round(ry2 * boundy) + 'px',
                marginLeft: '-' + Math.round(rx2 * c.x) + 'px',
                marginTop: '-' + Math.round(ry2 * c.y) + 'px'
            });
        }
        // 剪切图片的大小和位置
        fnPorData ($('#target').attr('src'),jcrop_api.tellSelect());
    }
});
// 请求 剪切图片的大小和位置
var porData = {
    url:'0',
    widths:	'0',		// 状态
    heights:'0',		// 时间 $type:时间条件类型,int值（1：本周，2：本月，0：自选时间(从$starttime至$endtime)）
    xs:'0',		// 人员
    ys:'0'		// 部门分类
};
// 剪切图片的大小和位置
function fnPorData (urls,datas) {
    porData.url=urls;
    porData.widths=datas.w;
    porData.heights=datas.h;
    porData.xs=datas.x;
    porData.ys=datas.y;
    fnPorDataAjax ();
}
function fnPorDataAjax () {
    $.ajax({
        type: "post",
	    url: "/User/SaveHeadImg",
        dataType: "json",
        success:rsHandler(function(data){
            data.url=porData.url;
            data.widths=porData.widths;
            data.heights=porData.heights;
            data.xs=porData.xs;
            data.ys=porData.ys;
        })
    });
}
function HeadImgSelect() {
    $.ajax({
        type: "post",
        url: "/User/HeadImg",
        dataType:"json",
        success: rsHandler(function (data) {
            //alert(data.url);
            $("#imgurl").val(data.url);
        })
    });
}
