//@ sourceURL=systemSettingManage.js
/**
 * Created by ZETA on 2015/11/12.
 */
$(function(){
    //页面变量
    var SystemUploadPath   = {
        //新闻通知分页
        pageNum:0,

        //用户列表
        userPage:0,

        //设置量质时的最大值
      completeQuantity:0,

         completeQuality:0,
         completeTime:0,

        //设置错误信息文件
         MessagePath:0,

        //密码错误次数验证码
         InputErrorValidate:0,

        //上传文件路径
        NewsUpLoadPath:"",

         DocumentUpLoadPath:"",
      PlanUpLoadPath:"",
         MineUpLoadPath:"",
        HeadImageUpLoadPath:"",
        IMUploadPath:"",
        ObjectiveUploadPath:"",
        FlowIndexUploadPath:"",
        MeetingUpLoadPath:"",
         ConvertFilePath:"",

        //IM通讯服务
        IMHost:"",

        //模板路径
         PlanTemplate:"",

        //量质时系数
         maxQuantity:0,

       maxQuality:0,
        maxTime:0,
        }
    //控制输入
    //数字
    $("#InputErrorValidate2,#completeQuantity2,#completeQuality2,#completeTime2,#pageNum2,#userPage2").on("input", function () {
        var reg1 = /^\+?[1-9][0-9]*$/;
        var $value1 = $(this);
        vals1 = $value1.val().match(reg1);

        $value1.val(vals1 ? vals1[0] : "");
        flag = true;
    });
    var flag = true;
    //文件绝对路径
    $("#DocumentUpLoadPath2,#PlanUpLoadPath2,#MeetingUpLoadPath2,#FlowIndexUploadPath2,#MineUpLoadPath2,#ObjectiveUploadPath2").on("blur", function () {
        
        var reg2 = /^[a-zA-Z]:(((\\(?! )[^\/:*?<>""|\\]+[^.])+\\?)|(\\)?)+[^.]*$/;
       
        var $value2 = $(this);
        vals2 = $value2.val().match(reg2);
        //$value2.val(vals2 ? vals2[0] : "");
        if (vals2==null && $value2.val() != "") {
            flag = false;
            //$value2.val($value2.val().substr($value2.val().length - 1, 1));
            ncUnits.alert("文件路径格式不正确 例如:C:\\A\\B（文件名不能包含以下任何字符:\/:*?'<>|.）");
            return false;
        } else {
            flag = true;
        }
    });
    //文件相对路径
    $("#NewsUpLoadPath2,#HeadImageUpLoadPath2,#IMUploadPath2,#ConvertFilePath2").on("blur", function () {
        var reg3 = /^[^\?<>""|\\]+[^.]$/;
        var $value3 = $(this);
        if (!reg3.test($value3.val())) {
            flag = false;
            ncUnits.alert("文件名不能包含以下任何字符:\/:*?'<>|或名称结尾不能包含.");
            return false;
        } else {
            flag = true;
        }
       
        //$value3.val(vals3 ? vals3[0] : "");
    });

    //文件名加后缀验证
    $("#MessagePath2,#PlanTemplate2").on("blur", function () {
        var reg4 = /^[^\/:*?<>""|\\]+?\.(?:xml|txt|doc|xlsx)$/i;
        vals4 = $(this).val().match(reg4);
        if (vals4==null && $(this).val() != "") {
            flag = false;
            ncUnits.alert("文件没有后缀名 例:.xml|.txt|.doc|.xlsx(文件名不能包含以下任何字符:\/:*?'<>|.)");
            return false;
        } else {
            flag = true;
        }
    });

    //IM通讯路径url验证
    $("#IMHost2").on("blur", function () {
        var re = "^(([A-Za-z]+://)([0-9]{1,3}.){3}[0-9]{1,3})(:[0-9]{1,4})+?[/]$";
        var reg5 = new RegExp(re);
        var $value5 = $(this);
        if (!reg5.test($value5.val())) {
            flag = false;
            ncUnits.alert("IM通讯路径url验证不正确 例如:ws://10.10.10.2:9200/");
            return false;
        } else {
            flag = true;
        }
    });

    $(".set-item").each(function(){
        var $this = $(this),
            $display = $this.find(".set-display"),
            $text = $this.find(".set-text"),
            $change = $this.find(".set-change"),
            $delete = $this.find(".set-delete"),
            changeUrl = "/SystemSetting/SystemSet";
          


        //switch(this.id){
        //    case "setFileUploadPath":
        //        changeUrl = "/SystemSetting/SystemSet";
                
        //        break;
        //    case "setPlanFilePath":
        //        changeUrl = "/SystemSetting/SystemSet";
                
        //        break;
        //    case "setMeetingDatumPath":
        //        changeUrl = "/SystemSetting/SystemSet";
                
        //        break;
        //    case "setFlowFilePath":
        //        changeUrl = "../../test/data/fail.json";
                
        //        break;
        //}

       

        var v="", dv="";

        $text.blur(function () {
            if (dv == "")
            {
                dv = this.defaultValue;
            }
            v = this.value;
            
            $t = $(this).parents("tr").find("td:eq(2)").children("a");

            

            if (this.value == "")
            {
                flag = true;
                this.value = dv;
                //$text.hide();
                //$display.show();
                //$t.text("修改");
            }
            else
            {
                SystemUploadPath[$(this).parents("tr").attr("id")] = this.value;
            }
            
           
           
            if (v == dv) {
                if (flag) {
                    $text.val(dv);
                    $display.text(dv);
                    //$text.hide();
                    //$display.show();
                    //$t.text("修改");
                } else {
                    return false;
                }

            }
           
        });

        var trueBtn = false;
        $change.click(function () {
            console.log('change')
         
          

            
       
                if ($text.is(":hidden")) {
                    $text.show();
                    $display.hide();
                    $(this).text("确认");
                }
                else {

                    //var reg2 = /^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\.]+)+\\?)|(\\)?)\s*$/;
                    //flag = true;
                    //vals2 = $text.val().match(reg2);
                    //if (!vals2 && $text.val() != "") {
                    //    flag = false;
                    //    //$value2.val($value2.val().substr($value2.val().length - 1, 1));
                    //    ncUnits.alert("文件路径格式不正确 例如:C:\\A\\B(文件名不能包含以下任何字符:\/:*?'<>|.)");
                    //    return false;
                    //}
                    if (flag) {
                        $.ajax({
                            type: "post",
                            url: changeUrl,
                            dataType: "json",
                            data: { data: JSON.stringify(SystemUploadPath) },
                            success: rsHandler(function (data) {
                                dv = $text.val();
                                $display.text($text.val());
                                trueBtn = true;
                                ncUnits.alert("修改成功");
                            }, function (data) {
                                $text.val(dv);
                            })
                        });
                        $text.hide();
                        $display.show();
                        $(this).text("修改");
                    }
                }
            

        });
       
    })
});