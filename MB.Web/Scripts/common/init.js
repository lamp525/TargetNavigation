/**
 * Created by ZETA on 2015/8/27.
 */
/* 通知信息条 */
Messenger.options = {
    extraClasses: 'messenger-fixed messenger-on-top',
    theme: 'block'
}

var ncUnits = com.ztnc.targetnavigation.unit,
    rsHandler = ncUnits.responseSuccessHandler,
    rcHandler = ncUnits.responseCompleteHandler;

/* 获取加载图的位置 */
//function getLoadingPosition(container){
//    if ($(container).length <= 0) {
//        return;
//    }
//    var loadObj={};
//    var lodi;
//    this.chartX=$(container).offset().left-$(document).scrollLeft();
//    this.chartY=$(container).offset().top-$(document).scrollTop();
//    loadObj.lodiChartX=this.chartX+Math.floor($(container).width()/2)-50+'px';
//    loadObj.lodiChartY=this.chartY+Math.floor($(container).height()/2)-20+'px';
//    lodi = $.layer({
//        time: 0,
//        page:{
//            html:'<span class="xubox_loading xubox_loading_0"></span><span class="xubox_setwin"></span><span class="xubox_botton"></span>'
//        },
//        bgcolor: '',
//        shade: [0],
//        border: [0],
//        fix:false,
//        type : 1,
//        title : false,
//        closeBtn : [0 , false],
//        offset:[loadObj.lodiChartY,loadObj.lodiChartX]
//    });
//    $('#xubox_layer'+lodi).css({'box-shadow':'none','width':'100px','height':'40px'}).find('.xubox_loading').css({'width':'100px','height':'40px'});
//    return lodi;
//}
function getLoadingPosition(container) {
    var $return = $();
    $(container).each(function () {
        var $this = $(this);
        if ($this.css("position") == "static") {
            $this.css("position", "relative");
        }
        var h = $this.height(),
            $pacman = $("<div class='loadingContainer'><div class='loadingBg'></div><div class='pacman'><div></div><div></div><div></div><div></div><div></div></div></div>");
        if (h < 50) {
            $this.css("min-height", "50px")
        }
        $this.append($pacman);
        $return = $return.add($pacman);
    });
    return $return;
}

/* 侧边栏-个人信息-数据加载 */
var setData = {
    num: '30',			// 每页条数
    time: '30',		// 消息提示时间
    password: '2'		// 密码
};
function loadPersonalInfo() {
    var $personal = $("#personal_info");
    $.ajax({
        type: "post",
        url: "/User/GetLoginUser",
        dataType: "json",
        success: rsHandler(function (data) {
            $(".personal_info_id", $personal).html(data.userId);
            $(".personal_info_name", $personal).html(data.userName);
            $(".personal_info_job", $personal).html(data.stationName);
            $(".personal_info_portrait", $personal).attr("src", data.bigImage);
            $(".personal_info_unfinished", $personal).html(data.todayUnfinished+'/'+ data.todayPlanTotal);
            $(".personal_info_overtime", $personal).html(data.overTimePlan);
            $(".personal_info_flow", $personal).html(data.unCompleteFlowCount);
            $(".personal_info_flow", $personal).html(data.process);
            $(".personal_info_invite", $personal).html(data.incentive);
        })
    });
    $(".personal_info_con", $personal).click(function () {
        $('#personal_modal').modal("show");
        $.ajax({
            type: "post",
            url: "/User/GetLoginUser",
            dataType: "json",
            success: rsHandler(function (data) {
                $("#userName").html(data.userName);
                $("#station").html(data.stationName);
                $("#phone").html(data.phone);
                if (data.bigImage != null) {
                    $("#UserImg").attr("src", data.bigImage);
                }
            })
        });
        fnSetDataAjax1();
    });
    function fnSetDataAjax1() {
        $.ajax({
            type: "post",
            url: "/User/GetPersonalSetting",
            dataType: "json",
            success: rsHandler(function (data) {
                //setData.password = data.password;
                $('#personal_modal_countPerPage').val(data.pageSize);
                $('#personal_modal_promptingTime').val(data.refreshTime);
            })
        });
    }
    // 更新个人设定
    function fnSetDataAjax3() {
        var pagesize = $("#personal_modal_countPerPage").val();
        if (10 <= pagesize && pagesize <= 101) {
            var refreshTime = $("#personal_modal_promptingTime").val();
            $.ajax({
                type: "post",
                url: "/User/UpPersonalSetting",
                dataType: "json",
                data: { pagesize: pagesize, refreshTime: refreshTime },
                success: rsHandler(function (data) {
                    //data.pageSize = setData.num;
                    //data.refreshTime = setData.time;
                    //data.password = setData.password;
                    ncUnits.alert("更改个人设定成功");
                })
            });
        }
        else { ncUnits.alert("每页条数的范围必须在10~100"); }
    }
    // 更新密码
    function fnSetDataAjax2() {
        var oldpassword = $("#personal_modal_currentPwd").val();
        var password = $("#personal_modal_newPwd").val();
        $.ajax({
            type: "post",
            url: "/User/UpPwd",
            dataType: "json",
            data: { oldpwd: oldpassword, pwd: password },
            success: rsHandler(function (data) {
                //console.log(data);
                //data.pageSize = setData.num;
                //data.refreshTime = setData.time;
                //data.password = setData.password;
                ncUnits.alert("更改密码成功");
            })
        });
    }


    $('.cancel').click(function () {
        $(this).hide();
        $('#personal_modal_currentPwd').val('');
        $('#personal_modal_newPwd').val('');
        $('#personal_modal_confirmPwd').val('');
        $("#personal_possword_pwd_btn_edit").removeClass('glyphicon-ok').addClass('glyphicon-pencil');
        $("#personal_modal_currentPwd").attr("readonly", true);
        $("#personal_modal_newPwd").attr("readonly", true);
        $("#personal_modal_confirmPwd").attr("readonly", true);
    });




    $("#personal_modal_custom_btn_edit").click(function () {
        if ($("#personal_modal_custom_btn_edit").hasClass('glyphicon-ok')) {
            setData.num = $('#personal_modal_countPerPage input:eq(0)').val();
            setData.time = $('#personal_modal_promptingTime input:eq(0)').val();
            // 个人资料返回数据
            fnSetDataAjax3();
            $(this).removeClass('glyphicon-ok').addClass('glyphicon-pencil');
            $("#personal_modal_promptingTime").attr("readonly", true);
            $("#personal_modal_countPerPage").attr("readonly", true);
            $(this).parents('.set').find('.list input').removeClass('inputHit').attr({ 'readonly': 'readonly' });
        } else {
            $("#personal_modal_countPerPage").removeAttr("readonly");
            $("#personal_modal_promptingTime").removeAttr("readonly");
            $(this).removeClass('glyphicon-pencil').addClass('glyphicon-ok');
        }
    });

    $("#personal_possword_pwd_btn_edit").click(function () {
        $(this).next('.cancel').show();
        if ($("#personal_possword_pwd_btn_edit").hasClass('glyphicon-ok')) {
            // 判断当前密码是否为空
            if ($('#personal_modal_currentPwd').val() != '') {
                // 判断新密码是否为空
                if ($('#personal_modal_newPwd').val() != '') {
                    // 判断确认密码是否为空
                    if ($('#personal_modal_confirmPwd').val() != '') {
                        // 判断当前密码是否正确
                        //if (setData.password == $('#nowPass input:eq(0)').val()) {
                        // 判断新密码和确认密码是否一致
                        if ($('#personal_modal_newPwd').val() == $('#personal_modal_confirmPwd').val()) {
                            //setData.password = $('#personal_modal_currentPwd').val();
                            // 个人资料返回数据
                            $(this).next('.cancel').hide();
                            fnSetDataAjax2();
                            $('#personal_modal_currentPwd').val('');
                            $('#personal_modal_newPwd').val('');
                            $('#personal_modal_confirmPwd').val('');
                            $(this).removeClass('glyphicon-ok').addClass('glyphicon-pencil');
                            $("#personal_modal_currentPwd").attr("readonly",true);
                            $("#personal_modal_newPwd").attr("readonly",true);
                            $("#personal_modal_confirmPwd").attr("readonly",true);
                        } else {
                            validate_reject('新密码和确认密码不一致', $('#personal_modal_confirmPwd'));
                        }
                        //} else {
                        //    validate_reject('当前密码错误', $('#nowPass'));
                        //}

                    } else {
                        validate_reject('确认密码不能为空', $('#personal_modal_confirmPwd'));
                    }
                } else {
                    validate_reject('新密码不能为空', $('#personal_modal_newPwd'));
                }
            } else {
                validate_reject('当前密码不能为空', $('#personal_modal_currentPwd'));
            }
            //$(this).removeClass('glyphicon-ok').addClass('glyphicon-pencil');
        } else {
            $("#personal_modal_currentPwd").removeAttr("readonly");
            $("#personal_modal_newPwd").removeAttr("readonly");
            $("#personal_modal_confirmPwd").removeAttr("readonly");
            $(this).removeClass('glyphicon-pencil').addClass('glyphicon-ok');
        }
    })
}


/** 验证失败的tips
 * html:提示文字
 * select:元素选择器
 */

function validate_reject(html,select){
    return layer.tips(html, select, {
        style: ['background-color:#c00; color:#fff', '#c00'],
        maxWidth:185,
        closeBtn:[0, true],
        more:true
    });
}

/**
 * 字符串转html
 * @param htmlStr
 * @returns {XML|string}
 * @constructor
 */
function ToHtmlString(htmlStr) {
    return toTXT(htmlStr).replace(/\&lt\;br[\&ensp\;|\&emsp\;]*[\/]?\&gt\;|\r\n|\n/g, "<br/>");
}

/**
 * html转字符串
 * @param str
 * @returns {XML|string|void|*}
 */
function toTXT(str) {
    var RexStr = /\<|\>|\"|\'|\&|　| /g;
    str = str.replace(RexStr,
        function (MatchStr) {
            switch (MatchStr) {
                case "<":
                    return "&lt;";
                    break;
                case ">":
                    return "&gt;";
                    break;
                case "\"":
                    return "&quot;";
                    break;
                case "'":
                    return "&#39;";
                    break;
                case "&":
                    return "&amp;";
                    break;
                case " ":
                    return "&ensp;";
                    break;
                case "　":
                    return "&emsp;";
                    break;
                default:
                    break;
            }
        }
    )
    return str;
}

/**
 * URI中获取查询字符串
 * @param name
 * @returns {*}
 * @constructor
 */
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null){
        return decodeURIComponent(r[2]);
    }else{
        return null;
    }
}

function removeValue(arr, v) {
    for (var i = 0, len = arr.length; i < len; i++) {
        if (arr[i] == v) {
            arr.splice(i, 1);
            break;
        }
    }
}

// 弹窗高度
function fnPopUpHeight(thisthis) {
    if (parseInt($(thisthis).css('height')) > 700) {
        $(thisthis).parents('.xubox_layer').css({ 'top': '50px', 'height': '700px', 'overflow-x': 'hidden', 'overflow-y': 'scroll' });
    }
}

//紧急度、重要度
function fnLevel() {
    $('.level').hover(function () {
        $('.levelDiv', this).show();
        $('.levelDiv .levelList', this).show();
    }, function () {
        $('.levelDiv', this).hide();
    });
}

//文件预览
function preview(type, name, extension) {
    $.ajax({
        type: "post",
        url: "/Shared/ConvertFile",
        dataType: "json",
        data: {
            type: type,
            saveName: name,
            extension: extension
        },
        success: rsHandler(function (data) {
            var oeTags = '<embed width="800" height="500" src=' + data + ' type="application/x-shockwave-flash"/>';
            $.layer({
                type: 1,
                shade: [0.5, '#000'],
                area: ['auto', 'auto'],
                title: false,
                border: [0],
                page: {
                    html: oeTags
                }
            });
        })
    });
}

//加载视图到主容器
var $mainContainer = $("#mainContentContainer");
var $mainNav = $("#mainNav");
function loadViewToMain(url) {
    $(".modal.in").modal("hide");
    layer.closeAll();
    var uri = url.split("?");
    var $tar = $mainNav.find("[href='" + uri[0] + "']");
    if ($tar.length > 0) {
        $mainNav.find(".active").removeClass("active");
        $tar.parent().addClass("active");
    }
    console.log('url',url)
    $mainContainer.load(url, function () {
        if ($("body").hasClass("modal-open")) {
            $("body").removeClass("modal-open").css({
                "padding-right": 0
            })
        }
        $(".modal-backdrop.fade.in").remove();
	    sessionStorage.setItem("currentUrl",url);
    });
}


//新建标签
function LabelAdd() {
    if (!(this instanceof LabelAdd)) {
        return new LabelAdd(opt, data);

    }
    var check = {
        isRepeat: function (value) {
            var result = false,value = value;
            $('.list-lab').each(function () {
                if ($(this).find('span').text() == value) {
                    result = true;
                    return;
                }
            })
            return result;
        }

    }

    this.template = function (data, target) {
        var data = data;
        
        if (!data) return;
        var temp = '';
        $.each(data, function (n, val) {
            temp += '<li class="list-lab"><span>' + val + '</span><i class="fa fa-times remove-lab"></i></li>';

        })
        $(target).before(temp);
    }
    this.add = function (that) {
        var that = $(that);
        if (that.siblings('input[type="text"]').is(":hidden")) {
            that.siblings('input[type="text"]').show();
        } else {
            var val = that.siblings('input[type="text"]').val();
            that.siblings('input[type="text"]').hide().val('');
            if (!val || check.isRepeat(val)) {
                return;
            }
            val = val.replace(/(^\s{1,})|(\s{1,}$)|(\s{1,})/g, '');
            var html = '<li class="list-lab"><span>' + val + '</span><i class="fa fa-times remove-lab"></i></li>';
            that.parent('li').before(html);

        }

    };
    this.get = function (target) {
        var result = [];
        $(target).find('.list-lab').each(function () {
            result.push($(this).find('span').text());


        })
        return result;
    };
    this.empty = function (input) {
        $('.list-lab').remove();
        $('.new_label').hide().val('');
    };

    this.autocomplete = function (data) {
        console.log('auto', data);
        $('.new_label').autocompleter({
            source: data
        });

    }

    $('#mainContentContainer').on('click', '.remove-lab', function () {
        $(this).parent('li').remove();
        var index = $(this).parents('li').index();

    })


}
var lab = new LabelAdd();
function tags(func) {
    var tagsList = [];
    $.ajax({
        url: "/Shared/GetMostUsedTag",
        type: "post",
        dataType: "json",
        success: rsHandler(function (data) {
            tagsList = $.map(data, function (n) {
                return {
                    label: n
                }
            })
            func(tagsList);
        })
    });

}

/**
 * 获取一个月的周数（月首月尾不满一个周的日子也算本月的一周）
 * @param y
 * @param m
 * @returns {number}
 */
function getWeekCount(y, m) {
    var first = new Date(y, m, 1).getDay();
    var last = 32 - new Date(y, m, 32).getDate();
    return Math.ceil((first + last) / 7);
}



$(function(){
    $.ajaxSetup({
        cache: false,
        complete: rcHandler(function () {
            //console.log("这是ajax的默认complete事件,触发请求为:" + this.url);
        }),
        error: function (xhr, text) {
            ncUnits.alert(text);
        }
    });
})