com = {};
com.ztnc = {};
com.ztnc.targetnavigation = {}
com.ztnc.targetnavigation.unit = {};

com.ztnc.targetnavigation.unit.planStatusColor = [
    "#57acdb"    //待提交
    ,"#e00e16"   //待审核
    ,"#be1d9a"   //已审核
    ,"#fbab11"   //待确认
    ,"#58b557"   //已完成
    ,"#49dca7"   //已中止
]

/**
 * ajax请求成功时调用的函数，返回数据格式为:
 *    {
 *        success: $boolean          //请求状态，true：成功   false：失败
 *        ,data: $object             //数据主体
 *        ,login: $boolean           //是否已登录 && Session是否超时
 *        ,access: $boolean          //权限
 *        ,message: $string          //success为false时存储异常信息
 *    }
 * @param callback
 * @returns {Function}
 */
com.ztnc.targetnavigation.unit.responseSuccessHandler = function(callback,failCallback){
    return function(json){
        try{
            if (json.success && json.success.toString() == "true") {
                callback.call(null, json.data);
            } else {
                com.ztnc.targetnavigation.unit.responseErrorHandler.call(null, json.message);
            }
            if(failCallback){
                failCallback.call(null,json.data);
            }
        } catch (e) {
            console.error(e);
        }
    }
}
/**
 * ajax请求完成时调用的函数
 * @param callback
 * @returns {Function}
 */
com.ztnc.targetnavigation.unit.responseCompleteHandler = function(callback){
    return function (xhr, isSuccess) {
        try{
            if (callback) {
                callback.call(this, xhr);
            }
            if (isSuccess == "success") {
                if (this.dataType == "html") {
                    try{
                        var json = JSON.parse(xhr.responseText);
                        if (json.login == false || json.access == false) {
                            location.href = "/"
                        }
                    } catch (e) {

                    }
                } else if (this.dataType == "json" && (!xhr.responseJSON.login || !xhr.responseJSON.access)) {
                    location.href = "/"
                }
            }
        } catch (e) {
            console.error(e);
        }
    }
}
//com.ztnc.targetnavigation.unit.responseHandler = function(callback){
//    return function(json){
//        if(json.login && json.access){
//            if(json.success && json.success.toString() == "true"){
//                callback.call(null,json.data);
//            }else{
//                com.ztnc.targetnavigation.unit.responseErrorHandler.call(null,json.message);
//            }
//        }else{
//            location.href = "/"
//        }
//    }
//}

/**
 * ajax请求失败时异常处理
 * @param message
 */
com.ztnc.targetnavigation.unit.responseErrorHandler = function(message){
    com.ztnc.targetnavigation.unit.alert(message);
}

/**
 * alert提示框
 * @param message
 */
var messager;
com.ztnc.targetnavigation.unit.alert = function(message){
    if(messager && messager.shown){
        messager.update({
            message: message
        });
    }else{
        messager = Messenger().post({
            message: message,
            showCloseButton:"true",
            hideAfter:5
        });
    }
}

/**
 * confirm确认框
 * @param o {
 * title:$string,
 * html:$string,
 * yes:$function(layer_index),
 * no:$function(layer_index)
 * }
 */
com.ztnc.targetnavigation.unit.confirm = function(o){

    var opt = $.extend({
        title:"确认",
        html:"确认该操作?",
        yes:function(){
            layer.close(layer_confirm);
        },
        no:function(){
            layer.close(layer_confirm);
        }
    },o);

    var $confirm = $("#layer_confirm");
    if(!$confirm.length){
        $("body").append('<div class="popUp" id="layer_confirm" style="display:none;width: 300px">' +
        '<div class="title" style="width: 260px">' +
        '确认 ' +
        '</div> ' +
        '<span class="closeWCom"></span> ' +
        '<div class="mainContent" style="min-height: 30px;font-size: 14px">' +
        '</div> ' +
        '<div class="canCon" style="width: 300px"> ' +
        '<span style="width:149px; border-right:1px solid #bbb;">取消</span>' +
        '<span style="width:150px;" id="confirmBtn">确定</span> ' +
        '</div> ' +
        '</div>');
        $confirm = $("#layer_confirm");
    }

    var layer_confirm = $.layer({
        type: 1,
        shade: [.1,"#fff"],
        area: ['auto', 'auto'],
        //title: ['新建计划','background:#58b456;color:#fff;'],
        title:false,
        border: [0],
        page: {dom : '#layer_confirm'},
        move:".title",
        closeBtn:false
    });

    $(".closeWCom",$confirm).off("click");
    $(".closeWCom",$confirm).click(function(){
        layer.close(layer_confirm);
    });

    $(".title",$confirm).html(opt.title);
    $(".mainContent",$confirm).html(opt.html);
    var $no = $(".canCon span:eq(0)",$confirm);
    var $yes = $(".canCon span:eq(1)",$confirm);
    $no.off("click");
    $yes.off("click");
    $no.click(function(){
        opt.no.call(null,layer_confirm);
    });
    $yes.click(function(){
        opt.yes.call(null,layer_confirm);
    });

}