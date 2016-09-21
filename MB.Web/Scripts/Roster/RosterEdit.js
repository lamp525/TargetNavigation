//@ sourceURL=RosterEdit.js
/**
 * Created by DELL on 15-9-16.
 */
$(function () {
    if ("undefined" == typeof type) {
        console.log("type未定义");
    }
    var typeNum = type;
    type = undefined;

    if ("undefined" == typeof RuserId) {
        console.log("RuserId未定义");
    }
    var RosterId = RuserId;
    RuserId = undefined;

    console.log(RosterId+",,,,,"+typeNum);
    console.log("xxxxx");
    loadPersonalInfo();
   
    var count=2;
    var targetId=null,targetName=null;
    var deleteStationArray=[];
    var postChooseFlag=null, termFlag=null,oId=0,sId=0;
    //输入控制
    $("#salary").bind("input", function () {
        var reg = /^\d+(\.\d{0,2})?/;
        var values = $(this).val().substring(0, $(this).val().length - 1);
        var  $value= $(this);
        vals = $value.val().match(reg);

        $value.val(vals ? vals[0] : "");

        if ($value.val().indexOf(".") == -1 && $value.val().length >= 17) {
            $(this).val(values);
            ncUnits.alert("工资位数不能超过16位");
            return;
        }
        else if ($value.val().indexOf(".") != -1 && $value.val().length >= 20) {
            $(this).val(values);
            ncUnits.alert("工资位数不能超过19位");
            return;
        }
    });
    $("#marriage,#sex,#workStatus,#userType,#education").closest(".dropdown").find("li a").click(function(){
        dropDownEvent($(this));
    });
    $("#identityCard,#bankCard").click(function () {
        this.focus();
    })



    //身份证验证
    $("#identityCard").blur(function () {
        if ($(this).val() != "") {
            var regIden = /^(\d{18,18}|\d{15,15}|\d{17,17}x)$/;
           

            valsIden = regIden.test($(this).val());
            if (valsIden) {
                if ($(this).val().length > 13) {
                    $("#birthday").val($(this).val().substr(6, 4) + "-" + $(this).val().substr(10, 2) + "-" + $(this).val().substr(12, 2));
                }
                var identityCardNum = $(this).val();
                $.ajax({
                    type: "post",
                    url: "/Roster/GetTownByidentityCard",
                    dataType: "json",
                    data: { identityCard: identityCardNum },
                    success: rsHandler(function (data) {
                        if (data[0].strName + data[1].strName + data[2].strName == null) {
                            if (data.length == 3) {
                                $("#ProvinceCitydistrict").val(data[0].strName + data[1].strName + data[2].strName);
                                $("#ProvinceCitydistrict").attr("provinceId", data[0].strId);//provinceId
                                $("#ProvinceCitydistrict").attr("cityId", data[1].strId);//cityId
                                $("#ProvinceCitydistrict").attr("districtId", data[2].strId);//districtId
                            } else if (data.length == 2) {
                                $("#ProvinceCitydistrict").val(data[0].strName + data[1].strName);
                                $("#ProvinceCitydistrict").attr("provinceId", data[0].strId);//provinceId
                                $("#ProvinceCitydistrict").attr("cityId", data[1].strId);//cityId
                            } else if (data.length == 1) {
                                $("#ProvinceCitydistrict").val(data[0].strName);
                                $("#ProvinceCitydistrict").attr("provinceId", data[0].strId);//provinceId

                            }
                        }
                        else {
                            $("#ProvinceCitydistrict").val("");
                        }
                    })
                });
            } else {
                ncUnits.alert("身份证格式不正确");
                return false;
            }
        } 
      
    });

    //银行卡验证
    $("#bankCard").blur(function () {
        if ($(this).val() != "") {
            var bankNum = $(this).val();
            $.ajax({
                type: "post",
                url: "/Roster/GetBankName",
                dataType: "json",
                data: { bankNum: bankNum },
                success: rsHandler(function (data) {
                    $("#bankName").val(data.strName);
                })
            });
        }
    
    });

    //工号重复验证
    $("#userNumber").blur(function () {
        if (typeNum == 2) {
            if ($(this).val() != "") {
                $.ajax({
                    type: "post",
                    url: "/Roster/UserNumBerIsHave",
                    dataType: "json",
                    data: { num: $(this).val() },
                    success: rsHandler(function (data) {
                        if (data) {
                           
                            ncUnits.alert("工号重复");
                            $("#userNumber").val("");
                            return false;
                        }
                    })
                });
            }
        }
    });

    //姓名重复验证
    $("#userName").blur(function () {
        if(typeNum==2){
            if ($(this).val() != "")
            {
                var nval = $(this).val();
                $.ajax({
                    type: "post",
                    url: "/Roster/GetRosterList",
                    dataType: "json",
                    data: {userName:$(this).val()},
                    success: rsHandler(function (data) {
                        if (data.length > 0)
                        {
                            ncUnits.confirm({
                                title: '提示',
                                html: '姓名为' +nval+ '的员工已存在,是否确认创建该员工？',
                                yes: function (layer_confirm) {
                                    layer.close(layer_confirm);
                                },
                                no: function (layer_confirm) {
                                    $("#userName").val("");
                                    layer.close(layer_confirm);
                                }
                            });
                        }
                    })
                });
            }
        }
        
    });

    function rNumber() {
        $('.stationAddTr').each(function (n) {
            $(this).find('td').eq(2).html('职位' + (n + 1)); 
            $(this).find('td').eq(3).find(".postChoose_input").attr("term",n+2);


        })

    }

    $(".menuAdd").click(function () {
        typeNum = 2;
        RosterId = 0;
        //clearEditInfo();
        //clearShowInfo();
        $("#passwordChange").css("display", "none");
        $("#passwordChange_show").css("display", "none");
        $("#EditBtn").trigger("click");
    });
   
    //加载编辑页面信息
    if (typeNum == 1) {
        loadInfo(null);      
    }
    else if (typeNum == 2) {
        $("#passwordChange").css("display", "none");
        $("#passwordChange_show").css("display", "none");
    }//新建
    else if (typeNum == 3) {
        loadInfo(RosterId);
    }//从主页面进入编辑页面
    function loadInfo(userIdArgu) {
        //loadAllPerson();
        console.log(12345667789);
        $.ajax({
            type:"post",
            url: "/Roster/GetRosterInfo",
            dataType: "json",
            data: { userId: userIdArgu },
            success: rsHandler(function (data) {
                clearEditInfo();
                if (typeNum == 1) {
                    RosterId = data.userId;
                }             
                $("#userNumber").val(data.userNumber);
                $("#userName").val(data.userName);
              
                if (data.sex) {
                    $("#sex").closest(".dropdown").find("a[term=1]").trigger("click");
                }
                else {
                    $("#sex").closest(".dropdown").find("a[term=0]").trigger("click");
                }
                $("#nation").val(data.nation);
                $("#political").val(data.political);
                if (data.marriage) {
                    $("#marriage").closest(".dropdown").find("a[term=1]").trigger("click");
                }
                else {
                    $("#marriage").closest(".dropdown").find("a[term=0]").trigger("click");
                }             
                $("#identityCard").val(data.identityCard);
                $("#mobile1").val(data.mobile1);
                $("#mobile2").val(data.mobile2);
                $("#address").val(data.address);
                $("#workPlace").val(data.workPlace);
                if (data.entryTime != null) {
                    $("#entryTime").val(data.entryTime.split("T")[0]);
                }

                $("#probationaryPeriod").val(data.probationaryPeriod);
                if(data.positiveDate!=null){
                    $("#positiveDate").val(data.positiveDate.split("T")[0]);
                }
                
                $("#term").val(data.term);
                if(data.expiredDate!=null){
                    $("#expiredDate").val(data.expiredDate.split("T")[0]);
                }
               
                $("#comment").val(data.comment);
                if(data.birthday!=null){
                    $("#birthday").val(data.birthday.split("T")[0]);
                }
              
                $("#nature").val(data.nature);
                $("#nativePlace").val(data.nativePlace);
             
                $("#ProvinceCitydistrict").attr("provinceId", data.provinceId)
                $("#ProvinceCitydistrict").attr("cityId", data.cityId)
                $("#ProvinceCitydistrict").attr("districtId", data.districtId)           
                if (data.province + data.city + data.district == 0) {
                    $("#ProvinceCitydistrict").val("");
                }
                else {
                    $("#ProvinceCitydistrict").val(data.province + data.city + data.district);
                }
                $("#school").val(data.school);
                $("#professional").val(data.professional);
                $("#education").closest(".dropdown").find("a[term="+data.education+"]").trigger("click");
                if(data.firstWork!=null){
                    $("#firstWork").val(data.firstWork.split("T")[0]);
                }
                $("#qualification").val(data.qualification);
                $("#cornet").val(data.cornet);

                $("#emergencyNumber").val(data.emergencyNumber);
                $("#bankCard").val(data.bankCard);
                $("#bankName").val(data.bankName);
                $("#workStatus").closest(".dropdown").find("a[term=" + data.workStatus + "]").trigger("click");
                $("#userType").closest(".dropdown").find("a[term=" + data.userType + "]").trigger("click");
                if (data.quitTime != null) {
                    $("#quitTime").val(data.quitTime.split("T")[0]);
                }
            
                $("#salary").val(data.salary);
                $("#email").val(data.email);
                $("#qq").val(data.qq);
                if(data.orgList.length>=1){
                    $(".org_input").val(data.orgList[0].OrganizationName);
                    $(".org_input").attr("orgId", data.orgList[0].organizationId);
                    $(".postChoose_input").val(data.orgList[0].stationName);
                    $(".postChoose_input").attr("orgId", data.orgList[0].organizationId);
                    $(".postChoose_input").attr("stationId", data.orgList[0].stationId);
                    data.orgList.splice(0, 1);
                    console.log('data.orgList', data.orgList.length)
                    count = data.orgList.length;
                    $.each(data.orgList, function (i, value) {
                        //i++;
                        //count = i;
                        console.log(i);
                       
                            $(".add").addClass("no");
                            $(".add").css("display", "none");
                        
                        var $tr = $("<tr class='stationAddTr'> </tr>"),
                            $nameTd=$("<td style='width:14%' class='name'>部门/事业部 :</td>"),
                            $orgTd = $("<td style='width:32%'> <div class='input-group'><input type='text' style='cursor:pointer' class='form-control org_input' value=" + value.OrganizationName + " term=" + (i+1) + " orgId=" + value.organizationId + "><span class='input-group-addon org' style='cursor:pointer'></span></div> </td>"),
                            $postNameTd=$("<td style='width:14%'  class='name'>职位"+(i+1)+" :</td>"),
                            $postChooseTd = $("<td style='width:20%'> <div class='input-group'> <input type='text' style='cursor:pointer' class='form-control postChoose_input'  value=" + value.stationName + " term=" + (i+1) + " stationId=" + value.stationId + "  orgId=" + value.organizationId + "> <span class='input-group-addon postChoose' style='cursor:pointer'></span></div></td>"),
                            $operateTd=$("<td style='width:20%'></td>"),
                            $add = $("<a class='add'style='cursor:pointer'></a>"),
                            $delete = $("<a class='delete'style='cursor:pointer'></a>");
                        $add.click(function(){
                            AddEvent($(this));
                        });
                        $delete.click(function () {
                            count--;
                            
                            deleteStationArray.push(value.stationId);
                            if($(this).siblings(".add").hasClass("no")==false){
                                $(this).closest("tr").prev().find(".add").css("display","inline-block").removeClass("no");
                            }
                            $(this).closest("tr").remove();
                            rNumber();
                        });

          
                        $operateTd.append($delete,$add);
                        $tr.append($nameTd,$orgTd,$postNameTd,$postChooseTd,$operateTd);
                        $tr.insertBefore("#orgAndPost");
                        $(".org_input").css("cursor", "pointer");
                        $(".org").css("cursor", "pointer");
                        $(".postChoose").css("cursor", "pointer");
                        $(".postChoose_input").css("cursor", "pointer");

                        $(".org_input").click(function () {
                            if (typeof ($(this).attr("orgid")) != "undefined") {
                                oId = $(this).attr("orgid");
                            } else {
                                oId = 0;
                            }
                           
                            sId = 0;
                            termflag = $(this).attr("term");
                            postChooseFlag = ".org_input";

                            $(".postChoose_input[term=" + termFlag + "]").val("");
                            $(".postChoose_input[term=" + termFlag + "]").removeAttr("stationid");

                            $("#department_modal").modal('show');
                          
                        });
                      
                        $(".org").click(function () {
                            $(this).siblings("input").trigger("click");
                        });
                       
                        $(".postChoose").click(function () {
                            $(".postChoose_input").trigger("click");
                        });
                       
                        $(".postChoose_input").click(function () {
                            //if (termFlag == null) {
                                termFlag = $(this).attr("term");
                            //}
                            postChooseFlag = ".postChoose_input";
                            if (typeof ($(".org_input[term=" + termFlag + "]").attr("orgid")) != "undefined") {
                                oId = $(".org_input[term=" + termFlag + "]").attr("orgid");
                            } else {
                                oId = 0;
                            }
                            if (typeof ($(this).attr("stationid")) != "undefined") {
                                sId = $(this).attr("stationid");
                            } else {
                                sId = 0;
                            }
                            $('#station_modal').modal('show');
                        });
                        
                    })
                }
            })
        });
    }
   
    function time(id){
        //时间插件
        var start = {
            elem: id,
            format: 'YYYY/MM/DD',
            max: '2099-06-16', //最大日期
            istime: false,
            istoday: false,
            choose: function(datas){
                end.min = datas; //开始日选好后，重置结束日的最小日期
                end.start = datas //将结束日的初始值设定为开始日
            },
            clear:function(){
                end.min = undefined;
            }
        };
        var end = {
            elem: id,
            format: 'YYYY/MM/DD',
            max: '2099-06-16',
            istime: false,
            istoday: false,
            choose: function(datas){
                start.max = datas; //结束日选好后，重置开始日的最大日期
            },
            clear:function(){
                start.max = undefined;
            }
        };
        laydate(start);
        laydate(end);
    }
    $("#entryTime,#positiveDate,#expiredDate,#quitTime,#firstWork").click(function () {
        $(this).css("cursor", "pointer");
        time("#"+$(this).attr("id"));
    });
    $("#entryTime,#positiveDate,#expiredDate,#quitTime,#firstWork").siblings("span").click(function () {
        $(this).css("cursor", "pointer");
        $(this).siblings("input").trigger("click");
    });
    //数据保存确定
    $("#information_sure").click(function () {
        //判断是否为空值
        var returnFlag = false;
      
        if ($("#userName").val() == "") {
            ncUnits.alert("姓名不能为空");
            //validate_reject("输入不能为空", $("#userName"));
            returnFlag = true;
        }
        if ($("#userNumber").val() == "") {
            ncUnits.alert("用户工号不能为空");
            //validate_reject("输入不能为空", $("#userNumber"));
            returnFlag = true;
        }
        $(".postChoose_input").each(function () {
            if (typeof ($(this).attr("stationId")) == "undefined") {
                ncUnits.alert("未填写职位");
                //validate_reject("输入不能为空", $(this));
                returnFlag = true;
            }
        });
        if ($("#workStatus").attr("term") == "")
        {
            ncUnits.alert("员工在职状态不能为空");
            //validate_reject("输入不能为空", $("#workStatus"));
            returnFlag = true;
          
        }
        if ($("#userType").attr("term") == "") {
            ncUnits.alert("员工用户类型不能为空");
            //validate_reject("输入不能为空", $("#workStatus"));
            returnFlag = true;

        }
        if (returnFlag == true) {
         
            return;
        }
        var returnData={
            userId:0,                    //用户ID
            userNumber:"",             //工号
            userName:"",               //姓名
            sex:false,                       //性别
            nation:"",                 //民族
            political:"",              //政治面貌
            marriage:false,                 //婚否
            mobile1:"",                //手机1
            mobile2:"",                //手机2
            address:"",                //实际住址
            workPlace:"",              //工作地点
            entryTime:"",            //入职时间
            probationaryPeriod:0,        //试用期
            positiveDate:"",         //转正时间
            term:0,                      //合同期
            expiredDate:"",          //合同到期日
            comment:"",                //备注
            birthday:"",             //生日
            nature:"",                 //户口性质
            nativePlace:"",            //籍贯
            province:"",               //省份
            city:"",                   //城市
            district:"",               //地区
            school:"",                 //毕业院校
            professional:"",           //专业
            education:0,                 //学历
            firstWork:"",            //首次参加工作时间
            qualification:"",          //资格证书
            cornet:"",                 //短号
            emergencyNumber:"",        //紧急联系号码
            bankCard:"",               //银行卡号
            bankName:"",               //银行卡名
            workStatus: 0,                //在职状态
            userType: 0,                //员工状态
            quitTime:"",             //离职时间
            salary:"",                //现在工资
            qq:"",
            email: "",
            provinceId: 0,
            cityId: 0,
            districtId: 0,
            identityCard: 0,

            deleteStation:[],               //删除岗位
            newStation:[]                   //添加岗位
        }
        returnData.userId = RosterId;
       
        returnData.userNumber = $("#userNumber").val();
      
        returnData.userName = $("#userName").val();
        if ($("#sex").attr("term") == 0)
        {
            returnData.sex=false;
        } else {
            returnData.sex = true;
        }
        if($("#marriage").attr("term")==0){
            returnData.marriage=false;
        }else{
            returnData.marriage = true;
        }
        returnData.nation = $("#nation").val();
        var political = $("#political").val();
        var reg = /[`~!@#$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (political != "") {
            if (political.indexOf('null') >= 0 || political.indexOf('NULL') >= 0 || political.indexOf('&nbsp') >= 0 || reg.test(political) || political.indexOf('</') >= 0) {
                ncUnits.alert("政治面貌存在非法字符!");
                return;
            }
        }
        returnData.political = political;
       
        returnData.mobile1=$("#mobile1").val();
        returnData.mobile2 = $("#mobile2").val();
        var address = $("#address").val();
        //var regs = /[`~!@$%^&*()_+<>?:"{},.\/;'[\]]/im;
        if (address != "") {
            if (address.indexOf('null') >= 0 || address.indexOf('NULL') >= 0 || address.indexOf('&nbsp') >= 0 || reg.test(address) || address.indexOf('</') >= 0) {
                ncUnits.alert("住址存在非法字符!");
                return;
            }
        }
      
        returnData.address = address;
        var workp = $("#workPlace").val();
        if (workp != "") {
            if (workp.indexOf('null') >= 0 || workp.indexOf('NULL') >= 0 || workp.indexOf('&nbsp') >= 0 || reg.test(workp) || workp.indexOf('</') >= 0) {
                ncUnits.alert("工作地址存在非法字符!");
                return;
            }
        }
        returnData.workPlace = workp;
        returnData.entryTime=$("#entryTime").val();
        returnData.probationaryPeriod=$("#probationaryPeriod").val();
        returnData.positiveDate=$("#positiveDate").val();
        returnData.term=$("#term").val();
        returnData.expiredDate = $("#expiredDate").val();

        var comment = $("#comment").val();
        if (comment != "") {
            if (comment.indexOf('null') >= 0 || comment.indexOf('NULL') >= 0 || comment.indexOf('&nbsp') >= 0 || reg.test(comment) || comment.indexOf('</') >= 0) {
                ncUnits.alert("备注存在非法字符!");
                return;
            }
        }
        returnData.comment = comment;

        returnData.birthday = $("#birthday").val();

        var vt = /^(?:(?!0000)[0-9]{4}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$/;
        if (!returnData.birthday.match(vt)&&returnData.birthday!="") {
            ncUnits.alert("出生日期格式错误");
            return;
        }

        var nature = $("#nature").val();
        if (nature != "") {
            if (nature.indexOf('null') >= 0 || nature.indexOf('NULL') >= 0 || nature.indexOf('&nbsp') >= 0 || reg.test(nature) || nature.indexOf('</') >= 0) {
                ncUnits.alert("户口性质存在非法字符!");
                return;
            }
        }
        returnData.nature = nature;
        var nativePlace = $("#nativePlace").val();
        if (nativePlace != "") {
            if (nativePlace.indexOf('null') >= 0 || nativePlace.indexOf('NULL') >= 0 || nativePlace.indexOf('&nbsp') >= 0 || reg.test(nativePlace) || nativePlace.indexOf('</') >= 0) {
                ncUnits.alert("籍贯存在非法字符!");
                return;
            }
        }
        returnData.nativePlace = nativePlace;
        returnData.provinceId = $("#ProvinceCitydistrict").attr("provinceId")
        returnData.cityId = $("#ProvinceCitydistrict").attr("cityId")
        returnData.districtId = $("#ProvinceCitydistrict").attr("districtId")
        
        var school = $("#school").val();
        if (school != "") {
            if (school.indexOf('null') >= 0 || school.indexOf('NULL') >= 0 || school.indexOf('&nbsp') >= 0 || reg.test(school) || school.indexOf('</') >= 0) {
                ncUnits.alert("毕业院校存在非法字符!");
                return;
            }
        }
        returnData.school = school;

        var professional = $("#professional").val();
        if (professional != "") {
            if (professional.indexOf('null') >= 0 || professional.indexOf('NULL') >= 0 || professional.indexOf('&nbsp') >= 0 || reg.test(professional) || professional.indexOf('</') >= 0) {
                ncUnits.alert("专业存在非法字符!");
                return;
            }
        }
        returnData.professional = professional;
        returnData.education=$("#education").attr("term");
        returnData.firstWork = $("#firstWork").val();

        var qualification = $("#qualification").val();
        if (qualification != "") {
            if (qualification.indexOf('null') >= 0 || qualification.indexOf('NULL') >= 0 || qualification.indexOf('&nbsp') >= 0 || reg.test(qualification) || qualification.indexOf('</') >= 0) {
                ncUnits.alert("资格证书存在非法字符!");
                return;
            }
        }
        returnData.qualification = qualification;
        returnData.cornet=$("#cornet").val();
        returnData.emergencyNumber= $("#emergencyNumber").val();
        returnData.bankCard= $("#bankCard").val();
        returnData.bankName= $("#bankName").val();
        returnData.workStatus = $("#workStatus").attr("term");
        returnData.userType =$("#userType").attr("term");
        returnData.quitTime=$("#quitTime").val();
        returnData.salary= $("#salary").val();
        returnData.qq = $("#qq").val();

        var email = $("#email").val();
        if (email != "")
        {
            var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (!filter.test(email)) {
                ncUnits.alert("邮箱格式有误!");
              
                return;
            }
        }
        returnData.email = email;
        returnData.identityCard = $("#identityCard").val();
        returnData.newStation;
        $(".postChoose_input").each(function(){
            returnData.newStation.push(parseInt($(this).attr("stationId")));
        })
        returnData.deleteStation = deleteStationArray.slice(0, 1);
        $.ajax({
            type:"post",
            url: "/Roster/SaveRosterInfo",
            data: {data:JSON.stringify(returnData)},
            dataType: "json",
            success: rsHandler(function (data) {
                typeNum = 3;
                ncUnits.alert("保存成功");
                //取消验证样式
                layer.closeTips();
                RosterId = data;
                $("#EditCancelBtn").trigger("click");
                $("#personInfo_table").empty();
                $("#personInfo_select").val("");
                loadAllPerson();
               
            })
        });
    });
    $("#information_Cancle").click(function () {
        $("#EditCancelBtn").trigger("click");
    });
   
    $(".add").click(function () {
      
        AddEvent($(this));
    });

    function AddEvent(value) {
        console.log('count', count)
        count++;
        value.addClass("no");
        value.css("display", "none");
        console.log('222',count)
        var $tr = $("<tr class='stationAddTr'> </tr>"),
            $nameTd=$("<td style='width:14%' class='name'>部门/事业部 :</td>"),
            $orgTd=$("<td style='width:32%'> <div class='input-group'><input type='text' class='form-control org_input'term='"+(count+1)+"'><span class='input-group-addon org'></span></div> </td>"),
            $postNameTd=$("<td style='width:14%'  class='name'>职位"+count+" :</td>"),
            $postChooseTd = $("<td style='width:20%'> <div class='input-group'> <input type='text' class='form-control postChoose_input' term='" +(count+1) + "'> <span class='input-group-addon postChoose'></span></div></td>"),
            $operateTd=$("<td style='width:20%'></td>"),
            $add=$("<a class='add' style='cursor:pointer'></a>"),
            $delete = $("<a class='delete'style='cursor:pointer'></a>");
       
        $add.click(function () {
           
            AddEvent($(this));
        });
       
        $delete.click(function () {
            count--;
            if($(this).siblings(".add").hasClass("no")==false){
                $(this).closest("tr").prev().find(".add").css("display","inline-block").removeClass("no");
            }
            $(this).closest("tr").remove();
            rNumber();
        });
        $operateTd.append($delete,$add);
        $tr.append($nameTd,$orgTd,$postNameTd,$postChooseTd,$operateTd);
        $tr.insertBefore("#orgAndPost");
        $(".org_input").css("cursor", "pointer");
        $(".org_input").off("click");
        $(".org_input").click(function (e) {
            if (typeof ($(this).attr("orgid")) != "undefined") {
                oId = $(".org_input[term=" + termFlag + "]").attr("orgid");
            } else {
                oId = 0;
            }
            sId = 0;
            $("#department_modal").modal('show');
            termFlag=$(this).attr("term");
            postChooseFlag = ".org_input";

            $(".postChoose_input[term=" + termFlag + "]").val("");
            $(".postChoose_input[term=" + termFlag + "]").removeAttr("stationid");
        });
        $(".org").css("cursor", "pointer");
        $(".org").click(function () {
            $(this).siblings("input").trigger("click");
        });
        $(".postChoose").css("cursor", "pointer");
        $(".postChoose").off("click");
        $(".postChoose").click(function () {
            var temp = $(this).attr('term');
            var ele = $(".org_input[term=" + temp + "]")
            ele.trigger("click");
        });
        $(".postChoose_input").css("cursor", "pointer");
        $(".postChoose_input").off("click");
        $(".postChoose_input").on('click', function () {
            //if (termFlag == null) {
                termFlag = $(this).attr("term");
            //}
            postChooseFlag = ".postChoose_input";
            if (typeof ($(".org_input[term=" + termFlag + "]").attr("orgid")) != "undefined") {
                oId = $(".org_input[term=" + termFlag + "]").attr("orgid");
            } else {
                oId = 0;
            }
            if (typeof ($(this).attr("stationid")) != "undefined") {
                sId = $(this).attr("stationid");
            } else {
                sId = 0;
            }
            $('#station_modal').modal('show');
        });
        
    }

    function dropDownEvent(value){
        var x =  $(value).parents("ul").prev().find("span:eq(0)");
        x.text( $(value).text() );
        var term  = $(value).attr("term");
        x.attr("term",term);
    }
    /*------------------------------------------弹窗 事件 开始---------------------------------*/
    //组织架构
    var department_modal;
    $('#information-content').off('click', ".org_input")
    $('#information-content').on('click', ".org_input", function () {
       
        if (typeof ($(this).attr("orgid")) != "undefined") {
            oId = $(this).attr("orgid");
        } else {
            oId = 0;
        }
     
        sId = 0;
        $("#department_modal").modal('show');
        termFlag=$(this).attr("term");
        postChooseFlag = ".org_input";

        $(".postChoose_input[term=" + termFlag + "]").val("");
        $(".postChoose_input[term=" + termFlag + "]").removeAttr("stationid");
    });
    $('#information-content').on('click',".org",function () {
        $(this).siblings("input").trigger("click");
    });

    $("#department_modal").on('show.bs.modal', function () {
        //取消验证提示样式
        //layer.closeTips();
        if (oId == 0) {
            $("#department_modal_chosen").empty();
            $("#department_modal_chosen_count").text(0);
        }
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            data: { parent: null },
            success: rsHandler(function (data) {
                department_modal = $.fn.zTree.init($("#department_modal_folder"), $.extend({
                    callback: {
                        beforeClick: function (id, node) {
                            department_modal.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {
                            $("#department_modal_chosen").empty();
                            for(var i=0;i< department_modal.getCheckedNodes().length;i++){
                                if(department_modal.getCheckedNodes()[i].id!=node.id){
                                    department_modal.checkNode(department_modal.getCheckedNodes()[i], false, false);
                                }
                            }
                            if (node.checked) {
                                var $checked = $("<li term=" + node.id + "><span>" + node.name + "</span></li>"),
                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                $("#department_modal_chosen").append($checked.append($close));
                                $close.click(function () {
                                    department_modal.checkNode(node, undefined, undefined, true);
                                });
                                node.mappingLi = $checked;

                            } else {
                                node.mappingLi.remove();
                            }
                            $("#department_modal_chosen_count").html($("#department_modal_chosen>li").length);
                        },
                        onNodeCreated: function (e, id, node) {
                            $.each($("#department_modal_chosen li"), function () {
                                if (oId != $(this).attr("term"))
                                {
                                    $("#department_modal_chosen").empty();
                                    $("#department_modal_chosen_count").text(0);
                                  
                                }
                            });
                            if (oId == node.id)
                            {
                                node.checked = true;
                                node.checkedOld = true;
                                $("#" + node.tId + "_check").removeClass("checkbox_false_full").addClass("checkbox_true_full");
                                $("#department_modal_chosen_count").html("1");
                                var $checked = $("<li term=" + node.id + "><span>" + node.name + "</span></li>"),
                                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                $("#department_modal_chosen").empty();
                                $("#department_modal_chosen").append($checked.append($close));
                                $close.click(function () {
                                    $("#department_modal_chosen_count").text(0);
                                    department_modal.checkNode(node, undefined, undefined, true);
                                   
                                });
                            }
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false
                    },
                    check: {
                        enable: true,
                        chkStyle: "checkbox",
                        chkboxType: { "Y": "", "N": "" }
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
        //组织架构搜索
        $("#department_modal_search").selection({
            url: "/Shared/GetOrgListByName",
            hasImage: false,
            selectHandle: function (data) {
                $("#department_select").val(data.name);
                var n = department_modal.getNodeByParam("id", data.id);
                if (n && !n.checked) {
                    department_modal.checkNode(n, undefined, undefined, true);
                } else {
                    var flag = true;
                    if ($("#department_modal_chosen li").length > 0) {
                        $("#department_modal_chosen li").each(function () {
                            if ($(this).attr('term') == data.id) {
                                flag = false;
                            }
                        });
                    }

                    if (flag == true) {
                        ////取消原来的选择
                        $("#department_modal_chosen").empty();
                        for(var i=0;i< department_modal.getCheckedNodes().length;i++){
                            if(department_modal.getCheckedNodes()[i].id!=data.id){
                                department_modal.checkNode(department_modal.getCheckedNodes()[i], false, false);
                            }
                        }
                        ////////////////////////////////////////////////
                        var $checked = $("<li term=" + data.id + "><span>" + data.name + "</span></li>"),
                            $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                        $("#department_modal_chosen").append($checked.append($close));
                        $close.click(function () {
                            var nodeId = $(this).parent().attr("term");
                            n = department_modal.getNodeByParam("id", parseInt(nodeId));
                            if (n) {
                                department_modal.checkNode(n);
                            }
                            $(this).parent().remove();
                            $("#department_modal_chosen_count").text($("#department_modal_chosen li").length);
                        });
                    }
                }
                $("#department_modal_chosen_count").text($("#department_modal_chosen li").length);
            }
        });
        //确定组织架构选择
        $("#department_modal_submit").click(function () {
            if ($("#department_modal_chosen li").length == 0)
            {
                ncUnits.alert("未选择部门");
                return;
            }
            $('#department_modal_chosen li').each(function () {
                var name = $(this).find("span:eq(0)").text();
                targetId=$(this).attr('term');
                targetName=name;
            });
            oId = targetId;
            $(postChooseFlag+"[term="+termFlag+"]").val(targetName);
            $(postChooseFlag + "[term=" + termFlag + "]").attr("orgId", targetId);
            $('#department_modal').modal('hide');
        });
        //取消组织架构选择
        $('#department_modal_cancel').click(function () {
            $("#department_modal_chosen_count").text(0);
            $("#department_modal_chosen").empty();
        });
        //  $("#department_modal .modal-content").load("/Shared/GetDepartmentHtml", function () {                });
    });
    /*岗位 开始*/
    var stationWithSub=0;
    var stationOrganizationId;
    $("#information-content").on('click','.postChoose',function (e) {
        e.stopPropagation();
     
        var idx = $(this).prev().attr("term");
        var postChoose = $(".postChoose_input[term=" + idx + "]")
        postChoose.trigger("click");
    });

    $("#information-content").on('click','.postChoose_input',function (e) {
        //if (termFlag == null) {
            termFlag = $(this).attr("term");
        //}
         e.stopPropagation();
        postChooseFlag = ".postChoose_input";
        if (typeof ($(".org_input[term=" + termFlag + "]").attr("orgid")) != "undefined") {
            oId = $(".org_input[term=" + termFlag + "]").attr("orgid");
        } else {
            oId = 0;
        }

        if (typeof ($(this).attr("stationid")) != "undefined") {
            sId = $(this).attr("stationid");
        } else {
            sId = 0;
        }
        $('#station_modal').modal('show');
    });
    $('#station_modal').on('show.bs.modal', function () {
        //取消验证提示样式
        //layer.closeTips();
        if (sId == 0) {
            $("#station_modal_chosen_count").text(0);
            $(".station_list ul").remove();
        }
        $.ajax({
            type: "post",
            url: "/Shared/GetOrganizationList",
            dataType: "json",
            success: rsHandler(function (data) {
                var folderTree = $.fn.zTree.init($("#station_modal_folder"), $.extend({
                    callback: {
                        beforeClick: function (id, node) {
                            folderTree.checkNode(node, undefined, undefined, true);
                            return false;
                        },
                        onCheck: function (e, id, node) {
                            stationOrganizationId = node.id;
                           
                            $.ajax({
                                type:"post",
                                url: "/Shared/GetStationList",
                                dataType:"json",
                                data:{withSub:stationWithSub,organizationId:stationOrganizationId},
                                success:rsHandler(function(data){
                                    if(data.length>0){
                                        $(".station_list ul").remove();
                                        $.each(data, function (i, v) {
                                          
                                            var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox' style='cursor:pointer;'></li><li term=" + v.stationId + " orgId='" + v.organizationId + "'><span>" + v.stationName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                            $(".station_list").append($stationHtml);
                                        });
                                        appendstation();
                                    }
                                })
                            });
                        },
                        onNodeCreated: function (e, id, node) {
    
                            //console.log('nid',node.id)
                            if (oId == node.id) {
                                node.checked = true;
                                node.checkedOld = true;
                                $("#" + node.tId + "_check").removeClass("radio_false_full").addClass("radio_true_full");
                                stationOrganizationId = node.id;
                                $.ajax({
                                    type: "post",
                                    url: "/Shared/GetStationList",
                                    dataType: "json",
                                    data: { withSub: stationWithSub, organizationId: stationOrganizationId },
                                    success: rsHandler(function (data) {
                                        if (data.length > 0) {
                                            $(".station_list ul").remove();
                                            $.each(data, function (i, v) {
                                                //var soName2 = v.stationName + "-" + v.organizationName;
                                                //if (soName2.length > 18) {
                                                //    soName2 = soName2.substring(0, 15) + "...";
                                                //}
                                                var $stationHtml = $("<ul class='list-inline'><li><input type='checkbox' style='cursor:pointer;'></li><li term=" + v.stationId + " orgId='" + v.organizationId + "'><span>" + v.stationName + "</span>-<span>" + v.organizationName + "</span></li></ul>");
                                                $(".station_list").append($stationHtml);
                                            });
                                                if (sId != 0) {
                                                    //$(".station_list ul li[term=" + sId + "]").prev().find("input[type='checkbox']").prop("checked", true);
                                                    $(".station_list ul li[term=" + sId + "]").prev().find("input[type='checkbox']").trigger("click");
                                                    $('#station_modal_chosen_count').text("1");
                                                    $("#station_modal_chosen").append("");
                                                    var $checked = $("<li term=" + sId + "  orgId='" + oId + "'><span>" + $(".station_list ul li[term=" + sId + "]").find("span:eq(0)").text() + "</span></li>"),
                       $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                                                    $("#station_modal_chosen").empty();
                                                    $("#station_modal_chosen").append($checked.append($close));
                                                    $close.click(function () {
                                                        var $thisId = $(this).parent().attr('term');

                                                        $(this).parent().remove();
                                                        $(".station_list ul").each(function () {
                                                            if ($(this).find("li:eq(1)").attr('term') == $thisId) {
                                                                $(this).find("input[type='checkbox']").attr("checked", false);
                                                                $('#station_modal_chosen_count').text("0");
                                                            }
                                                        });
                                                    });
                                                } else {
                                                    $("#station_modal_chosen").empty();
                                                    $('#station_modal_chosen_count').text("0");
                                                  
                                                }

                                           
                                            appendstation();
                                        }
                                    })
                                });
                            } else {
                                
                                $("#station_modal_chosen").empty();
                                $(".station_list ul").remove();
                                $('#station_modal_chosen_count').text("0");
                            }
                          

                           
                        }
                    }
                }, {
                    view: {
                        showIcon: false,
                        showLine: false
                    },
                    check: {
                        enable: true,
                        chkStyle: "radio",
                        chkboxType: { "Y": "", "N": "" }
                    },
                    async: {
                        enable: true,
                        url: "/Shared/GetOrganizationList",
                        autoParam: ["id=parent"],
                        dataFilter: function (treeId, parentNode, responseData) {
                            return responseData.data;
                        }
                    }
                }), data);
            })
        });
    });

    //加载点击复选框的事件
    function appendstation(){
        $(".station_list input[type='checkbox']").click(function(){
            var checked = $(this).prop('checked');
            var orgId = $(this).parents(".list-inline").find("li:eq(1)").attr('orgId');
            var stationId=$(this).parents(".list-inline").find("li:eq(1)").attr('term');
            var stationName = $(this).parents(".list-inline").find("li:eq(1) span:eq(0)").text();
            var OrgName = $(this).parents(".list-inline").find("li:eq(1) span:eq(1)").text();
            if(checked==true){
                var  $this= $(this);
                $(".station_list input[type='checkbox']:checked").each(function(){
                    $(this).prop("checked",false);
                });
                $("#station_modal_chosen").empty();

                $this.prop("checked",true);
                $('#station_modal_chosen_count').text($(".station_list input[type='checkbox']:checked").length);
                var $checked = $("<li term=" + stationId + "  orgId='" + orgId + "' OrgName='" + OrgName + "'><span>" + stationName + "</span></li>"),
                    $close = $("<a class='close' aria-label='Close'><span aria-hidden='true'>&times;</span></a>");
                $("#station_modal_chosen").append($checked.append($close));
                $close.click(function(){
                    var $thisId=$(this).parent().attr('term');

                    $(this).parent().remove();
                    $(".station_list ul").each(function(){
                        if($(this).find("li:eq(1)").attr('term')==$thisId){
                            $(this).find("input[type='checkbox']").attr("checked",false);
                            $('#station_modal_chosen_count').text($(".station_list input[type='checkbox']:checked").length);
                        }
                    });
                });
            }else {
                $(this).attr('checked',false);
                $('#station_modal_chosen_count').text($(".station_list input[type='checkbox']:checked").length);
                $("#station_modal_chosen li").each(function(){
                    if($(this).attr('term')==stationId){
                        $(this).remove();
                    }
                });
            }
        });
    }

   

    //确定
    $("#station_modal_submit").click(function () {
        if ($("#station_modal_chosen_count").text() == 0) {
            ncUnits.alert("未选择职位");
            return;
        }
        var orgName, orgId;
        $("#station_modal_chosen li").each(function(){
            targetId=$(this).attr('term');
            targetName = $(this).find('span:eq(0)').text();
            orgName = $(this).attr('OrgName');
            orgId = $(this).attr('orgId');  
        });
        $(".org_input[term=" + termFlag + "]").attr("orgId",orgId);
        $(".org_input[term=" + termFlag + "]").val(orgName);
        $(postChooseFlag+"[term="+termFlag+"]").val(targetName);
        $(postChooseFlag +"[term=" + termFlag + "]").attr("stationId", targetId);
        $("#station_modal_chosen li").remove();
        $('#station_modal').modal('hide');
    });

    //取消
    $("#station_cancel").click(function(){
        $("#station_modal_chosen li").remove();
    });
    /*岗位 结束*/
    /*-----------------------弹窗 事件 结束------------------------------------------*/
    /*------------右侧 搜索  事件 开始-----------------------*/
    //人员搜索
    //$("#personInfo_search").selection({
    //    url: "/Shared/GetUserListByName",
    //    hasImage: true,
    //    selectHandle: function (data) {
    //        $("#personInfo_select").val(data.name);
    //        $("#personInfo_table").empty();
              
    //        var  $tr = $("<tr><td style='width:25%'></td> " +
    //               "<td style='width:25%'>姓名</td> <td style='width:25%'>职位</td> <td style='width:25%'>电话</td></tr>");
    //        $("#personInfo_table").append($tr);
    //        searchPerson(data.name);
    //    }
    //});

    $("#personInfo_select_click").click(function () {
        $("#personInfo_table").empty();
        if ($("#personInfo_select").val() == "") {
            //$tr = $("<tr><td style='width:25%'></td> " +
            //      "<td style='width:25%'>姓名</td> <td style='width:25%'>职位</td> <td style='width:25%'>电话</td></tr>");
            //$("#personInfo_table").append($tr);
            loadAllPerson();
        }
        else {
            //$tr = $("<tr><td style='width:25%'></td> " +
            //   "<td style='width:25%'>姓名</td> <td style='width:25%'>职位</td> <td style='width:25%'>电话</td></tr>");
            //$("#personInfo_table").append($tr);
            searchPerson($("#personInfo_select").val());
        }
    });
    //右侧搜索
   
    function searchPerson(name) {     
        $.ajax({
            type: "post",
            url: "/Roster/GetRosterList",
            data: { userName: name },
            dataType: "json",
            success: rsHandler(function (data) {
                $.each(data, function (i, v) {
                    //if (v.mobile2 != null) {
                    //    var $tr = $("<tr term='" + v.userId + "'></tr>");
                    //    var  $td = $("<td style='width:25%'><img class='img-thumbnail img-circle x32 pull-right personal_info_portrait' src=" + v.headImage + "></td> " +
                    //        "<td style='width:25%'>" + v.userName + "</td> <td style='width:25%'>" + v.station[0].stationName + "</td> <td style='width:25%'>" + v.mobile2 + "</td>");
                    //    $tr.click(function () {
                    //        userId = $(this).attr("term");
                    //        $("#EditCancelBtn").trigger("click");
                    //        typeNum = 4;
                    //    });
                    //    $("#personInfo_table").append($tr.append($td));
                    //}
                    var orgN = [], stationN = [];
                    $.each(v.station, function (i, value) {
                        orgN.push(value.OrganizationName);
                        stationN.push(value.stationName);
                    });
                    var m1 = "";
                    if (v.mobile1 != null) {
                        m1 = v.mobile1;
                    }
                    var m2 = "";
                    if (v.mobile2 != null) {
                        m2 = v.mobile2;
                    }
                    $t = $("<tr><td style='width:25%'></td> " +
               "<td style='width:25%'>姓名</td> <td style='width:25%'>职位</td> <td style='width:25%'>电话</td></tr>");
                        var $tr = $("<tr term='" + v.userId + "'></tr>");
                        var $td = $("<td style='width:25%'><img class='img-thumbnail img-circle x32 pull-right personal_info_portrait' src=" + v.headImage + " style='max-width:38px;'></td> " +
                               "<td style='width:25%;padding-top:15px;' class='text-overflow'>" + v.userName + "</td> <td style='width:25%;padding-top:15px;'> <span  class='text-overflow' style='width:70px;display:inline-block;' title=" + stationN.join(",") + ">" + stationN.join(",") + "</span></td> <td style='width:25%;padding-top:15px;'>" + m1 + "&nbsp&nbsp" + m2 + "</td>");
                        $tr.append($td);
                        $tr.click(function () {
                          
                            $("#password_Changed").text("");
                            RosterId = $(this).attr("term");
                            $("#EditCancelBtn").trigger("click");
                            typeNum = 4;
                        });
                        $("#personInfo_table").append($t,$tr);
                       
                    
                });
            })
        });
         
    }
    
    //右侧默认加载全部人员清单的数据
    loadAllPerson();
    function  loadAllPerson(){
        $.ajax({
            type: "post",
            url: "/Roster/GetRosterList",
            data: {},
            dataType: "json",
            success: rsHandler(function (data) {
                $t = $("<tr><td style='width:25%'></td> " +
               "<td style='width:25%'>姓名</td> <td style='width:25%'>职位</td> <td style='width:25%'>电话</td></tr>");
                $("#personInfo_table").append($t);
                $.each(data, function (i, v) {
                    //if (v.mobile2 != null) {
                    //    var $tr = $("<tr term='" + v.userId + "'></tr>");
                    //   var  $td = $("<td style='width:25%'><img class='img-thumbnail img-circle x32 pull-right personal_info_portrait' src=" + v.headImage + "></td> " +
                    //        "<td style='width:25%'>" + v.userName + "</td> <td style='width:25%'> <span class='text-overflow' style='width:70px;display:inline-block;' title=" + v.station[0].stationName + ">" + v.station[0].stationName + "</span></td> <td style='width:25%'>" + v.mobile2 + "</td>");
                    //   $tr.click(function () {
                    //       userId = $(this).attr("term");
                    //       $("#EditCancelBtn").trigger("click");
                    //       typeNum = 4;
                    //   });
                    //   $("#personInfo_table").append($tr.append($td));
                    //}
                    var orgN = [], stationN = [];
                    $.each(v.station, function (i, value) {
                        orgN.push(value.OrganizationName);
                        stationN.push(value.stationName);
                    });
                    var m3 = "";
                    if (v.mobile1 != null) {
                        m3 = v.mobile1;
                    }
                    var m4 = "";
                    if (v.mobile2 != null) {
                        m4= v.mobile2;
                    }
                   
                        var $tr = $("<tr term='"+ v.userId +"'></tr>");
                        var $td = $("<td style='width:25%'><img class='img-thumbnail img-circle x32 pull-right personal_info_portrait' src=" + v.headImage + " style='max-width:38px;'></td> " +
                            "<td style='width:25%;padding-top:15px;' class='text-overflow'>" + v.userName + "</td> <td style='width:25%;padding-top:15px;'> <span  class='text-overflow' style='width:70px;display:inline-block;' title=" + stationN.join(",") + ">" + stationN.join(",") + "</span></td> <td style='width:25%;padding-top:15px;'>" + m3 + "&nbsp&nbsp" + m4 + "</td>");
                        $tr.append($td);
                        $tr.click(function () {
                            $("#password_Changed").text("");
                            RosterId = $(this).attr("term");
                            $("#EditCancelBtn").trigger("click");
                            typeNum = 4;
                        });
                        $("#personInfo_table").append($tr.append($td));
                  
                });
            })
        });
       
    }
    /*------------右侧 搜索  事件 结束-----------------------*/
    //编辑密码修改
    $("#passwordChange").click(function () {
        $.ajax({
            type: "post",
            url: "/Roster/UpdatePassWord",
            data: { userId: RosterId },
            dataType: "json",
            success: rsHandler(function (data) {
                $("#password_Changed").text("最近修改的密码:" + data);
            })
        });
    })
    //显示密码修改
    $("#passwordChange_show").click(function () {
        $.ajax({
            type: "post",
            url: "/Roster/UpdatePassWord",
            data: { userId: RosterId },
            dataType: "json",
            success: rsHandler(function (data) {
                $("#password_Changed").text("最近修改的密码:"+data);
            })
        });
    })
 
  
    var userTypeArray = ["实习", "试用","正式"];
    var workStatusArray = ["离职", "在职", "退休"];
    
    var educationAray=["高中","大专","本科","硕士","博士"];
    var sexArray=["女","男"];
    var marriageArray=["未婚","已婚"];
    
    function loadInfo_show(RosterId) {
        if (typeNum != 2)
        {
            $("#passwordChange_show").css("display", "block");
        }
        $.ajax({
            type:"post",
            url:"/Roster/GetRosterInfo",
            data: { userId: RosterId },
            dataType: "json",
            success: rsHandler(function (data) {
                clearShowInfo();
                if (data.userNumber != null) {
                    $("#show_userNumber").text(data.userNumber);
                }              
                if (data.userName  != null) {
                    $("#show_userName").text(data.userName);
                   
                }          
                if (data.sex) { $("#show_sex").text(sexArray[1]); }
                else { $("#show_sex").text(sexArray[0]); }
                if (data.nation  != null) {
                    $("#show_nation").text(data.nation);
                }
                if (data.political  != null) {
                    $("#show_political").text(data.political);
                }        
                if (data.marriage) {
                    $("#show_marriage").text(marriageArray[1]);
                }
                else { $("#show_marriage").text(marriageArray[0]); }
                if (data.identityCard != null) {
                    $("#show_identityCard").text(data.identityCard);
                }
                if (data.mobile1 != null) {
                    $("#show_mobile1").text(data.mobile1);
                }
                if (data.mobile2 != null) {
                    $("#show_mobile2").text(data.mobile2);
                }
                if (data.address != null) {
                    $("#show_address").text(data.address);
                    $("#show_address").attr("title", data.address);
                }
                if (data.workPlace != null) {
                    $("#show_workPlace").text(data.workPlace);
                    $("#show_workPlace").attr("title", data.workPlace);
                }          
                if (data.entryTime != null) {
                    $("#show_entryTime").text(data.entryTime.split("T")[0]);
                }
                if (data.probationaryPeriod != null) {
                    $("#show_probationaryPeriod").text(data.probationaryPeriod+"月");
                }
               
                if (data.positiveDate != null) {
                    $("#show_positiveDate").text(data.positiveDate.split("T")[0]);
                }
                if (data.term != null) {
                    $("#show_term").text(data.term+"年");
                }    
                if (data.expiredDate != null) {
                    $("#show_expiredDate").text(data.expiredDate.split("T")[0]);
                }
                if (data.comment != null) {
                    $("#show_comment").text(data.comment);
                    $("#show_comment").attr("title", data.comment);
                }
               
                if (data.birthday != null) {
                    $("#show_birthday").text(data.birthday.split("T")[0]);
                }          
                if(data.nature!=null){
                    $("#show_nature").text(data.nature);
                }
                if(data.nativePlace!=null){
                    $("#show_nativePlace").text(data.nativePlace);
                    $("#show_nativePlace").attr("title",data.nativePlace);
                }
                if (data.school != null) {
                  
                        $("#show_school").text(data.school);
                        $("#show_school").attr("title",data.school);
                    
                }
                if (data.province + data.city + data.district == 0) {
                    $("#show_ProvinceCitydistrict").text("");
                }
                else {
                    $("#show_ProvinceCitydistrict").text(data.province + data.city + data.district);
                }
                if (data.professional != null) {
                   
                        $("#show_professional").text(data.professional);
                        $("#show_professional").attr("title",data.professional);
                    
                }
                
                $("#show_education").text(educationAray[data.education]);
                if (data.firstWork != null) {
                    $("#show_firstWork").text(data.firstWork.split("T")[0]);
                }
                if (data.qualification != null) {
                   
                        $("#show_qualification").text(data.qualification);
                        $("#show_qualification").attr("title",data.qualification);
                   
                }
                if(data.cornet!=null){
                    $("#show_cornet").text(data.cornet);
                }
                if(data.emergencyNumber){
                    $("#show_emergencyNumber").text(data.emergencyNumber);
                }
                if(data.bankCard!=null){
                    $("#show_bankCard").text(data.bankCard);
                }
                if (data.bankName != null) {
                    $("#show_bankName").text(data.bankName);
                }
               
                $("#show_workStatus").text(workStatusArray[data.workStatus]);
                $("#show_userType").text(userTypeArray[data.userType]);
                if (data.quitTime != null) {
                    $("#show_quitTime").text(data.quitTime.split("T")[0]);
                }     
                if(data.salary!=null){
                    $("#show_salary").text(data.salary);
                }
                if(data.email!=null){
                    $("#show_email").text(data.email);
                }
                if(data.qq!=null){
                    $("#show_qq").text(data.qq);
                }
               
                if(data.orgList.length>=1){
                    $(".show_org").text(data.orgList[0].OrganizationName);
                    $(".show_postChoose").text(data.orgList[0].stationName);
                    $(".show_postChoose").attr("title",data.orgList[0].stationName);
                    data.orgList.splice(0,1);
                    $.each(data.orgList, function (i, value) {
                        i++;
                        var $tr = $("<tr class='stationAddTr'> </tr>"),
                            $nameTd=$("<td style='width:14%' class='name'>部门/事业部 :</td>"),
                            $orgTd=$("<td style='width:32%'> <span  class='span-control show_org'>"+value.OrganizationName+"</span></td>"),
                            $postNameTd=$("<td style='width:14%'  class='name'>职位"+i+" :</td>"),
                            $postChooseTd = $("<td style='width:20%' class='text-overflow'> <span class='span-control show_postChoose' title=" + value.stationName + ">" + value.stationName + "</span></td>"),
                            $operateTd=$("<td style='width:20%'></td>");
                        $tr.append($nameTd,$orgTd,$postNameTd,$postChooseTd,$operateTd);
                        $tr.insertBefore("#show_orgAndPost");
                        
                    })
                }
            })
        });
    }
    /*-------------------取消编辑按钮 事件-----------------------------*/
    $("#EditCancelBtn").click(function () {
        //if (typeNum != 2) {
        if (typeNum != 2) {
            $("#passwordChange_show").css("display", "block");
            $("#passwordChange").css("display", "block");
           
        }
            loadInfo_show(RosterId);           
            $("#con_informationShow").addClass("active");
            $("#con_informationEdit").removeClass("active");
            $("#EditCancelBtn").css("display", "none");
            $("#EditBtn").css("display", "block");
        //}
        
    })
    $("#EditBtn").click(function () {
        //if (typeNum != 2) {
        if (typeNum != 2) {
            $("#passwordChange_show").css("display", "block");
            $("#passwordChange").css("display", "block");
        }
        $(".add").removeClass("no");
        $(".add").css("display", "inline-block");
            loadInfo(RosterId);
            $("#con_informationShow").removeClass("active");
            $("#con_informationEdit").addClass("active");
            $("#EditBtn").css("display", "none");
            $("#EditCancelBtn").css("display", "block");
        //}   
    })
    function clearEditInfo() {
        $('.stationAddTr').remove();
        $("#userNumber").val("");
        $("#userName").val("");
      
        $("#sex").closest(".dropdown").find("a[term=1]").trigger("click");       
        $("#nation").val("");
        $("#political").val("");        
        $("#marriage").closest(".dropdown").find("a[term=0]").trigger("click");      
        $("#identityCard").val("");
        $("#mobile1").val("");
        $("#mobile2").val("");
        $("#address").val("");
        $("#workPlace").val("");
        $("#entryTime").val("");
        $("#probationaryPeriod").val("");
        $("#positiveDate").val("");
        $("#term").val("");
        $("#expiredDate").val("");
        $("#comment").val("");
        $("#birthday").val("");
        $("#nature").val("");
        $("#nativePlace").val("");
        $("#ProvinceCitydistrict").attr("provinceId", "")
        $("#ProvinceCitydistrict").attr("cityId", "")
        $("#ProvinceCitydistrict").attr("districtId", "")
        $("#ProvinceCitydistrict").val("");
        $("#school").val("");
        $("#professional").val("");
        $("#education").closest(".dropdown").find("a[term=0]").trigger("click");
        $("#firstWork").val("");
        $("#qualification").val("");
        $("#cornet").val("");
        $("#emergencyNumber").val("");
        $("#bankCard").val("");
        $("#bankName").val("");
        $("#workStatus").closest(".dropdown").find("a[term='']").trigger("click");
        $("#userType").closest(".dropdown").find("a[term='']").trigger("click");
        $("#quitTime").val("");       
        $("#salary").val("");
        $("#email").val("");
        $("#qq").val("");
        $(".org_input").val("");
        $(".org_input").attr("orgId","");
        $(".postChoose_input").val("");
        $(".postChoose_input").attr("orgId","");
    }
    function clearShowInfo() {
        $('.stationAddTr').remove();
        $("#show_userNumber").text("");
        $("#show_userName").text("");
      
        $("#show_sex").text("");
        $("#show_nation").text("");
        $("#show_political").text("");
        $("#show_marriage").text("");
        $("#show_identityCard").text("");
        $("#show_mobile1").text("");
        $("#show_mobile2").text("");
        $("#show_address").text("");
        $("#show_address").attr("title", "");
        $("#show_workPlace").text("");
        $("#show_workPlace").attr("title", "");      
        $("#show_entryTime").text("");
        $("#show_probationaryPeriod").text("");
        $("#show_positiveDate").text("");
        $("#show_term").text("");
        $("#show_expiredDate").text("");
        $("#show_comment").text("");
        $("#show_comment").attr("title", "");
        $("#show_birthday").text("");
        $("#show_nature").text("");
        $("#show_nativePlace").text("");
        $("#show_ProvinceCitydistrict").text("");
        $("#show_school").text("");
        $("#show_professional").text("");
        $("#show_education").text("");
        $("#show_firstWork").text("");
        $("#show_qualification").text("");
        $("#show_cornet").text("");
        $("#show_emergencyNumber").text("");
        $("#show_bankCard").text("");
        $("#show_bankName").text("");
        $("#show_workStatus").text("");
        $("#show_userType").text("");
        $("#show_quitTime").text("");
        $("#show_salary").text("");
        $("#show_email").text("");
        $("#show_qq").text("");
        $(".show_org").text("");
        $(".show_postChoose").text("");
    }

});
