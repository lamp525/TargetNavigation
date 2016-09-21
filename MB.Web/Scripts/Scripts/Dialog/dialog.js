
//公共弹窗
define(['artTemplate', 'imgareaselect'], function (template) {
    //公共弹窗请求url
    var CONSTANT = {
        trans: '/PlanOperate/TurnPlan',//计划转办
        confirm: '/PlanOperate/ConfirmPlan', //计划确认
        commitConfirm: '/PlanOperate/SubmitConfirmPlan', //一般计划提交确认
        commitLoopConfirm: '/PlanOperate/SubmitConfirmLoopPlan', //循环计划提交确认
        delFile: '/FileOperate/DeleteFile',//计划提交确认附件删除
        detail: '/XXXViews/Plan/GetPlanInfo', //获取一般计划详情
        detailLoop: '/XXXViews/Plan/GetLoopPlanInfo', //获取循环计划详情
        searchUser: '/XXXViews/User/UserNameFuzzySearch', //人员模糊检索
        contactUser: '/XXXViews/User/GetTopContacts', //获取常用联系人
        audit: '/PlanOperate/ApprovePlan',//一般计划审核
        auditLoop: '/PlanOperate/ApproveLoopPlan',//循环计划审核
        auditMulti: '/PlanOperate/ApproveMultiPlan',//一般计划批量审核
        auditLoopMulti: '/PlanOperate/ApproveMultiLoopPlan',//循环计划批量审核
        doMethod: '/XXXViews/Plan/GetExecutionInfo',//新建计划完成方式列表获取
        password: '/XXXViews/User/ChangePassword',//密码修改
        fileList: '/FileOperate/GetFileInfo',//获取附件列表
        multiDownload: '/FileOperate/MultiDownload',//多个附件下载
        singleDownload: '/FileOperate/SingleDownload',//附件单个下载
        filePreview: '/FileOperate/Preview',//文件预览
        iconInfo: '/XXXViews/User/GetUserOriginalImage', //获取头像信息
        iconImg: '/FileOperate/UploadFile',//上传头像原始图片,计划提交确认上传附件
        iconSave: '/XXXViews/User/SaveUserHead', //保存裁剪后头像尺寸
        planEdit: '/PlanOperate/AlterPlan',//一般计划修改申请
        planLoopEdit: '/PlanOperate/AlterLoopPlan',//循环计划修改申请
        planStop: '/PlanOperate/StopPlan',//一般计划终止申请
        planLoopStop: '/PlanOperate/StopLoopPlan',//循环计划终止申请
        planSubmit: '/PlanOperate/SubmitPlan',//待提交一般计划提交申请
        planLoopSubmit: '/PlanOperate/SubmitLoopPlan',//待提交循环计划提交申请
        planSubmitMulti: '/PlanOperate/SubmitMultiPlan',//待提交一般计划批量提交申请
        planLoopSubmitMulti: '/PlanOperate/SubmitMultiLoopPlan',//待提交循环计划批量提交申请
        planDel: '/PlanOperate/DeletePlan',//一般计划删除
        planDelMulti: '/PlanOperate/DeleteMultiPlan', //一般计划批量删除
        planLoopDel: '/PlanOperate/DeleteLoopPlan',//循环计划删除
        planLoopDelMulti: '/PlanOperate/DeleteMultiLoopPlan', //循环计划批量删除
        planRevoction: '/PlanOperate/RevokePlan', //一般计划撤销
        planLoopRevoction: '/PlanOperate/RevokeLoopPlan', //循环计划撤销
        getLog: '/XXXViews/Plan/GetPlanLogInfo', //获取一般计划操作日志
        getLoopLog: '/XXXViews/Plan/GetLoopPlanLogInfo', //获取循环计划操作日志
        orgInfo: '/XXXViews/User/GetUserOrgInfo', //获取部门信息
        planAdd: '/XXXViews/Plan/AddOrUpdatePlan', //新建修改一般计划
        planLoopAdd: '/XXXViews/Plan/AddOrUpdateLoopPlan',//新建修改循环计划
        planDefault: '/XXXViews/Plan/GetDefaultPlanInfo', //新建一般计划默认信息
        planLoopDefault: '/XXXViews/Plan/GetDefaultLoopPlanInfo', //新建循环计划默认信息
        comment: '/XXXViews/Plan/GetPlanCommentInfo', //获取评论信息
        upComment: '/XXXViews/Plan/AddPlanComment', //提交评论信息
        getTags: '/XXXViews/Plan/GetMostUsedTag', //获取常用标签
        circleGet: '/XXXViews/Plan/GetLoopPlanSubmitInfo'
    }
    //$(document).off('input', '#comments');
    //$(document).off('propertychange', '#comments');
    //$(document).on('propertychange', '#comments', function () {
    //    var sHeight = $(this)[0].scrollHeight;
    //    console.log('sssheight',sHeight)
    //    if (sHeight <= 63) {
    //        $(this).css({ 'height': '63px', 'overflow-y': 'hidden' })
    //    } else if (sHeight >= 180) {
    //        $(this).css({ 'height': '180px', 'overflow-y': 'auto' })
            
    //    } else {
    //        $(this).css({ 'height': sHeight+'px', 'overflow-y': 'hidden' })
    //    }
    //})
    //$(document).on('input', '#comments', function () {
    //    var sHeight = $(this)[0].scrollHeight;
    //    console.log('sssheight', sHeight)
    //    if (sHeight <= 63) {
    //        $(this).css({ 'height': '63px', 'overflow-y': 'hidden' })
    //    } else if (sHeight >= 180) {
    //        $(this).css({ 'height': '180px', 'overflow-y': 'auto' })

    //    } else {
    //        $(this).css({ 'height': sHeight + 'px', 'overflow-y': 'hidden' })
    //    }
    //})
    function fileUpload(upId,url,callback,tp,param) {
        var jqXHR = null;
        var callback = callback;
        if (tp == 1) {
            var patternImg = /(\.|\/)(jpg|jpeg|bmp|png|gif)$/i;
        } else {
            var patternImg = /(\.|\/)(ppt|xls|doc|pptx|xlsx|docx|7z|zip|rar|dwt|dwg|dws|dxf|jpg|jpeg|txt|pdf|tiff|bmp|png|gif)$/i;
        }
        console.log('ppap',param)
        $(upId).fileupload({
            url: url,
            dataType: 'text',
            formData: param,
            send: function (e, data) {
            },
            add: function (e, data) {
                var isSubmit = true;
                $.each(data.files, function (index, value) {
                    if (value.size > 209715200) {
                        ncUnits.alert("上传文件过大(最大200M)");
                        isSubmit = false;
                        return;
                    } else if (!patternImg.test(value.name)) {
                        ncUnits.alert("上传文件格式不对");
                        isSubmit = false;
                        return;
                    } 
                });

                if (isSubmit) {
                    jqXHR = data.submit();
                }
            },
            complete: function (data, flag) {
                var data = JSON.parse(data.responseText);
                data = data.data[0];
            
                callback(data)

            },
            error: function (e, data) {
                ncUnits.alert('error');
            },
            done: function (e, data) {
                

            },

            progress: function (e, data) {
              
            },
            always: function (e, data) {
            }
        })


    }
    //修改头像
    var IconEdit = function () {
        this.postData = {};
        this.url = {
            'upload': CONSTANT.iconImg,
            'save': CONSTANT.iconSave,
            'img': CONSTANT.iconInfo
        };
        this.imgData = {};
        this.imgselect;
        this.post = {
            "isUploaded": false
        };
        this.el = $('#iconMoal');
        this.el.off('click', '.up-load,.re-upload,.save-upload');
        this.el.on('click', '.up-load,.re-upload', this.upLoad.bind(this));
        this.el.on('click', '.save-upload', this.saveLoad.bind(this));
        this.el.on("hide.bs.modal", function () {
            if (this.imgselect) {
                this.imgselect.cancelSelection()
            }
            this.post = {
                "isUploaded": false
            };
            this.imgData = {};
        }.bind(this));
    }
    IconEdit.prototype = {
        preview: function (selection,newsize,contain,name) {
            if (!selection.width || !selection.height) {
                return;
            }
            var scaleX = contain / selection.width;
            var scaleY = contain / selection.height;
            $(name).css({
                'width': Math.round(scaleX * newsize.w)+'px',
                'height': Math.round(scaleY * newsize.h) + 'px',
                'margin-left': (-Math.round(scaleX * selection.x1))  + 'px',
                'margin-top': (-Math.round(scaleY * selection.y1)) + 'px'
            });
            
            this.post.partWidth = Math.round(newsize.w);
            this.post.partHeight = Math.round(newsize.h);
            this.post.StartPointX = Math.round(selection.x1 * newsize.per);
            this.post.StartPointY = Math.round(selection.y1 * newsize.per);
            this.post.cutWidth = Math.round(selection.width * newsize.per);
            this.post.cutHeight = Math.round(selection.height * newsize.per);
           
        },
        imgLoad: function (src, size, pW, pH,position) {
            console.log('load',src)
            $('#icon-small,#icon-middle,#icon-large').show().attr('src', src);
            var position = position;
            var newSize = {};
            switch (true) {
                case size.width<=pW && size.height<=pH: //图片宽高小于容器宽高
                    newSize.w = size.width;
                    newSize.h = size.height;                    
                    break;
                case size.width>pW && size.width>=size.height: //图片宽大于图片高，且宽大于容器宽
                    newSize.w = pW;
                    var per = size.width / pW;
                    newSize.h = size.height / per;
                    newSize.per = per; //缩放比例
                    break;
                case size.height>=size.width && size.height>pH: //图片宽小于图片高，且高大于容器高
                    newSize.h = pH;
                    var per = size.height/ pH;
                    newSize.w = size.width / per;
                    newSize.per = per; //缩放比例
                    break;

            }
            newSize.per = newSize.per || 1;
            if (position) {               
                position = position.map(function (val) {
                    return val / newSize.per 
                })
            }
           
            $('#icon-image').attr('src', src).show().css({ 'width': newSize.w + 'px', 'height': newSize.h }).siblings('.upload-btn').hide();
            var obj = this;
            var selections = {
                width:position ? position[3]:newSize.w,
                height:position ? position[2] : newSize.h,
                x1: position ? position[0]: 0,
                x2: position ? position[0]+position[3] : newSize.w,
                y1: position ? position[1]: 0,
                y2: position ? position[1] + position[2] : newSize.h,
            }
            this.imgselect = $('#icon-image').imgAreaSelect({
                aspectRatio: newSize.w + ':' + newSize.h,
                handles: true,
                persistent: true,
                fadeSpeed: 200,
                instance: true,
                imageWidth: newSize.width,
                imageHeight: newSize.height,
                onSelectChange: function (img, selection) {
                    obj.preview(selection, newSize, 140, '#icon-large');
                    obj.preview(selection, newSize, 60, '#icon-middle');
                    obj.preview(selection, newSize, 30, '#icon-small');
                },
                x1: selections.x1,
                x2: selections.x2,
                y1: selections.y1,
                y2: selections.y2,
            });

            obj.preview(selections, newSize, 140, '#icon-large');
            obj.preview(selections, newSize, 60, '#icon-middle');
            obj.preview(selections, newSize, 30, '#icon-small');
        },

        imgShow: function (data) {
            this.imgData = data;
            console.log('imgsss', data);
            this.post = {
                imageUrl: this.imgData.filePath,
                originalImage: this.imgData.saveName,
                extension: this.imgData.extension
            }
            if (this.imgData.extension) {
                this.post.format = this.imgData.extension;
            }
            var pWidth = $('.upload-area').width();
            var pHeight = $('.upload-area').height();
            var img_url = this.imgData.filePath + '?' + Date.now();
            var img = new Image();
            img.src = img_url;
            console.log('srx', img.src)

            var result = {};
            if (img.complete) {
                result.width = img.width;
                result.height = img.height;
                this.imgLoad(img.src, result, pWidth, pHeight, this.imgData.position?this.imgData.position:'');
            } else {
                img.onload = function () {                    
                    result.width = img.width;
                    result.height = img.height;
                    console.log('res',result)
                    this.imgLoad(img.src, result, pWidth, pHeight, this.imgData.position ? this.imgData.position : '');
                }.bind(this)
            }
            
        },
        upLoad: function (e) {
            var ele = $(e.currentTarget);
            if (ele.hasClass('re-upload')) {
                $('#uploadIcon').click();
            }
            var param = { type: 5, targetId: -1 };
            var that = this;
            fileUpload('#uploadIcon', this.url['upload'], function (data) {
                that.imgShow.call(that,data);
                that.post.isUploaded = true;
            }, 1, param);
        },
        saveLoad: function () {
            var obj = this;
            $.post(obj.url.save, { Data: JSON.stringify(obj.post) }, function (data) {
                if (data.success == true) {
                    ncUnits.alert("头像保存成功");
                    obj.el.modal('hide');
                    $('.imgPerson').attr('src', data.data + '?' + Date.now())
                    return
                }
                ncUnits.alert("头像保存失败");
            },'json')
        },
        show: function () {
            this.el.modal('show');
            $.post(this.url.img, {}, function (data) {
                if (data.success == true) {
                    var result = {
                        filePath: data.data.imageUrl,
                        position: data.data.imagePosition.split(','),
                        saveName: data.data.originalImage,
                        extension: '.' + data.data.originalImage.substring(data.data.originalImage.lastIndexOf('.') + 1)
                    }
                    var t = setTimeout(function () {
                        this.imgShow(result);
                        clearTimeout(t)
                    }.bind(this), 500)
                    
                }
            }.bind(this),'json')

        },
        hide: function () {
            this.el.modal('hide')
        }

    }



    //修改密码
    var EditPwd = function () {
        this.el = $('#pwdModal');
        this.events = function (e) {
            var ele = $(e.currentTarget);
            if (ele.hasClass('rePwd') || ele.hasClass('newPwd')) {
                this.check();
                return
            }
            if (ele.hasClass('upPwd')) {
                this.commit();
                return
            }
        };
        this.el.on("hide.bs.modal", function () {
            $('.newPwd,.rePwd,.nowPwd').val('');
            $('.info-tip').hide();
        }.bind(this));
        this.el.off('click', '.upPwd');
        this.el.off('input', '.rePwd,.newPwd');
        this.el.on('input', '.rePwd,.newPwd', this.events.bind(this));
        this.el.on('click', '.upPwd', this.events.bind(this));

    }

    EditPwd.prototype = {
        check: function () {
            var newPwd = $('.newPwd').val();
            var rePwd = $('.rePwd').val();
            if (!newPwd || !rePwd) {
                $('.right-tip').hide().siblings('.wrong-tip').hide();
                return;
            }
            if (newPwd == rePwd) {
                $('.right-tip').show().siblings('.wrong-tip').hide();
                return;
            }
            $('.right-tip').hide().siblings('.wrong-tip').show();
        },
        tip: function (newPwd, rePwd, oldPwd) {           
            if (!newPwd || !rePwd || !oldPwd) {
                ncUnits.alert("有输入项为空");
                return false
            }
            if (newPwd != rePwd) {
                ncUnits.alert("确认密码与新密码不一致");
                return false
            }
            return true;

        },
        commit: function () {
            var newPwd = $('.newPwd').val();
            var rePwd = $('.rePwd').val();
            var oldPwd = $('.nowPwd').val();
            if (!this.tip(newPwd, rePwd, oldPwd)) {
                return
            }
            var post = { oldPassword: oldPwd, newPassword: newPwd };
            $.post(CONSTANT.password, {data:JSON.stringify(post)} , function (data) {
                if (data.data == 1 || data.data == 2) {
                    var msg = data.data == 1 ? '原密码错误' : '修改失败';
                    ncUnits.alert(msg);
                    return
                }
                ncUnits.alert("修改成功");
                this.el.modal('hide');
            }.bind(this),'json');
        },
        show: function () {
            this.el.modal('show');

        }
    

    }


    //确认弹窗
    //var Confirm = function () {

    //    var alert = function (opt) {
    //        yes = opt.yes;
    //        console.log('yaaaes', yes)
    //        $('.confirmMsg span').text(opt.title);
    //        if (!opt.backdrop) {
    //            $('#confirmModal').modal({
    //                backdrop: false
    //            })
    //        } else {
    //            $('#confirmModal').modal({
    //                backdrop: true
    //            })
    //        }
    //        $('#confirmModal').modal('show');
    //    }
        
    //    var el = $('#confirmModal');
    //    el.off('click', '.sureConfirm')
    //    el.on('click', '.sureConfirm', function () {
    //        console.log('yaaaes', yes)
    //        yes();
    //        $('#confirmModal').modal('hide');
    //    })
        

    //    return {
    //        alert:alert
        
    //    }
        
    //}
    var Confirm = function () {
        var yes;
        this.el = $('#confirmModal');
        
    }
    Confirm.prototype = {
        sure: function () {
            yes();
            this.el.modal('hide');
        },
        bindEvents: function () {
            this.el.off('click', '.sureConfirm')
            this.el.on('click', '.sureConfirm', this.sure.bind(this))
        },
        alert: function (opt) {
            this.bindEvents();
            yes = opt.yes;            
            if (!opt.backdrop) {
                this.el.modal({
                    backdrop: false
                })
            } else {
                this.el.modal({
                    backdrop: true
                })
            }
            this.el.modal('show')
            this.el.find('.confirmMsg span').text(opt.title);
        }
    }


    //计划转办
    var PlanTrans = function (callback) {
        this.el = $('#transModal');
        this.planId;
        this.responseUser;
        this.callback = callback?callback:'';
        this.confirmUser;
        this.change = function (e) {
            var ele = $(e.currentTarget);
            clearTimeout(this.list.tId);
            this.list.tId = setTimeout(function () {
                this.list.call(this.el, ele);
            }.bind(this), 700)
        };
    
    }
    PlanTrans.prototype = {
        show: function (pid,msg) {
            this.el.modal('show');
            this.bindEvent();
            this.planId = pid;
            this.el.find('.show-confirm').val(msg.confirmUser);
            this.el.find('.show-response').val(msg.responseUser);
            this.el.find('.old-response').attr('src', msg.rImg);
            this.el.find('.old-confirm').attr('src', msg.cImg);
            this.el.find('.complete-time').children('span').text(msg.eTime);
            this.el.find('.event').children().eq(1).text(msg.event).attr('title', msg.event);
            this.el.find('.event').children().eq(0).text(msg.mode).attr('title', msg.mode);
        },
        bindEvent: function () {
            this.el.off('input', '.trans-input');
            this.el.on('input', '.trans-input', this.change.bind(this));
            this.el.off('click', '.show-input');
            this.el.on('click', '.show-input', this.showList.bind(this));
            this.el.off('click', '.list-user');
            this.el.on('click', '.list-user', this.choose.bind(this));
            this.el.off('click', '.sure-trans');
            this.el.on('click', '.sure-trans', this.commit.bind(this));
            this.el.on("hide.bs.modal", function () {
                this.el.find('input').val('');
                this.planId = '';
                this.responseUser = '';
                this.confirmUser = '';
                this.el.find('.img-selected').attr('src', '/Images/common/portrait.png')
            }.bind(this));

        },
        commit: function () {
            if (!this.responseUser) {
                ncUnits.alert("责任人不能为空");
                return;
            }
            if (!this.confirmUser) {
                ncUnits.alert("确认人不能为空");
                return;
            }
            console.log('this.responseUser', this.responseUser)
            $.post(CONSTANT.trans, { planId: Number(this.planId), confirmUser: Number(this.confirmUser), responsibleUser: Number(this.responseUser) }, function (data) {
                if(data.success){
                    this.el.modal('hide');
                    ncUnits.alert("转办成功");
                    if(this.callback){
                        this.callback();
                    }
                    return;
                }
                ncUnits.alert("转办失败");
                
            
            }.bind(this),'json')
        },
        choose: function (e) {
            var ele = $(e.currentTarget);
            var pid = ele.attr('pid');
            var text = ele.find('p').text();
            var p = ele.parents('.wrap-select');
            var src = ele.find('img').attr('src');
            
            if (ele.hasClass('confirm-li') && pid != this.responseUser) {
                this.confirmUser = pid;
                p.find('.show-input').val(text);
                p.find('.img-selected').attr('src', src);
               
            } else if (ele.hasClass('response-li') && pid != this.confirmUser) {
                this.responseUser = pid;
                p.find('.show-input').val(text);
                p.find('.img-selected').attr('src', src);

            } else {
                ncUnits.alert("责任人与确认人不能相同");
            }
           
           
        },
        showList: function (e) {
            var ele = $(e.currentTarget);
            var child = ele.parent().find('.dropdown-menu');
            if (child.is(':hidden')) {
                ele.parent().find('.trans-input').val('');
            }
            var t = setTimeout(function () {
                ele.parent().find('.trans-input').focus();
                clearTimeout(t);
                this.list(ele.parent().find('.trans-input'))
            }.bind(this), 10)

        },        
        list: function (ele) {            
            
            var tp = ele.hasClass('trans-confirm') ? 1 : 2;
            var text = ele.val().replace(/\s+/g, '');
            var url = text ? CONSTANT.searchUser : CONSTANT.contactUser;
            var param = text ? { userName: text } : { type: tp };
            var className = ele.hasClass('trans-confirm') ? 'confirm-li' : 'response-li';
            $.post(url, param, function (data) {
                var list = '';
                if (data.data.length == 0) {
                    list = '<li class="train-tip">---暂无数据---</li>';
                } else {
                    var link = !text ? '<li>常用联系人</li>' : '';
                    $.each(data.data, function (n, val) {
                        list += '<li class="list-user ' + className + '" pid=' + val.userId + '><img src=' + val.headImage + ' class="img-all img-circle" /><p>' + val.userName + '</p></li> '
                    })
                    list = link + list
                }
                ele.parent().next('.list-trans').find('ul').html(list);
            }, 'json')
        }

    }

    //计划 1修改,2确认提交,3终止,4删除
    var PlanOther = function (callback) {
        this.confirm = new Confirm();
        this.callback = callback ? callback : '';
        this.message = {
            'planDel': '确定删除吗？',
            'planSubmitMulti': '确定批量提交吗？',
            'planSubmit': '确定提交吗？',
            'planStop': '确定终止吗？',
            'planEdit': '确定修改吗？',
            'planDelMulti': '确定批量删除吗？',
            'planRevoction':'确定要撤销吗？'
        }
    }
    PlanOther.prototype = {
        commit: function (callback, url, data, alter) {
            $.post(url, data, function () {
                if (alter) {
                    ncUnits.alert("操作成功！");
                }
                
                if (callback && alter) {
                    callback();
                }

            }, 'json')

        },
        init: function (id, type) {
            var postUrl = CONSTANT[type];
            var type = type;
            var info = this.message[type];
            var callback = this.callback;
            var planId = id;
            //if (planId.normal) {
            //    planId = JSON.stringify(id)
            //    var data = { data: planId }

            //} else {
            //    var data = id
            //}

            var commit = this.commit;
            var callback = this.callback;
            this.confirm.alert({
                title: info,
                backdrop:true,
                yes: function () {
                    if (planId.normal) {
                        var post = JSON.stringify(planId.normal)
                        var data = { data: post };                       
                        commit(callback, postUrl, data);
                    }
                    if (planId.loop) {
                        var post = JSON.stringify(planId.loop)
                        var data = { data: post };
                        var url = type == 'planSubmitMulti' ? CONSTANT.planLoopSubmitMulti : CONSTANT.planLoopDelMulti;
                        commit(callback, url, data, 1)
                    }
                    if (!planId.loop && !planId.normal) {
                        var data = id;
                        var last = data.isLoop == 1?CONSTANT[type.substring(0,4)+'Loop' + type.substring(4)]:postUrl;
                        commit(callback, last, data, 1)
                    }

                    

                }

            })
        }


    }


    //系数评定
    var Evaluate = function () {
        this.callback;
        this.el = $('#right-content');
        this.stars = {
            importance: 1,
            urgency: 1,
            difficulty: 1
        };
        this.parent;
        this.planId;
        this.ids = {};
        this.data;

    }
    Evaluate.prototype = {
        starShow: function (e) {
            if (this.data && ((this.data.status == 25 && this.data.stop == 0) || this.data.stop == 10)) return;
            var ele = $(e.currentTarget);
            var flag = ele.parent().attr('flag');
            var index = ele.index();
            ele.addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            ele.nextAll('i').addClass('fa-star-o').removeClass('fa-star');
        },
        starLeave: function (e) {
            if (this.data && ((this.data.status == 25 && this.data.stop == 0) || this.data.stop == 10)) return;
            var ele = $(e.currentTarget);
            var flag = ele.parent().attr('flag');
            var index = ele.index();
            var name = { '1': 'importance', '2': 'urgency' }[flag] || 'difficulty';
            if (!this.stars[name]) {
                ele.parent().find('i').addClass('fa-star-o').removeClass('fa-star');
                return;
            }
            var now = ele.parent().find('i').eq(this.stars[name] - 1);
            now.addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            now.nextAll('i').addClass('fa-star-o').removeClass('fa-star');
        },
        starConfirm: function (e) {
            if (this.data && ((this.data.status == 25 && this.data.stop == 0) || this.data.stop == 10)) return;
            var ele = $(e.currentTarget);
            var flag = ele.parent().attr('flag');           
            var index = ele.index();
            var name = { '1': 'importance', '2': 'urgency' }[flag] || 'difficulty';
            ele.addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            ele.nextAll('i').addClass('fa-star-o').removeClass('fa-star');
            this.stars[name] = index;
        },
        reset: function () {
            this.stars = { importance: 1, urgency: 1, difficulty: 1 };
            this.el.find('i').addClass('fa-star-o').removeClass('fa-star');
            $('#comments').val('');
            this.planId = '';
            this.ids = {};
            this.data = '';
        },
        bindEvent: function () {
            this.el.off('mouseover', '.i-star,.e-star,.h-star')
            this.el.on('mouseover', '.i-star,.e-star,.h-star', this.starShow.bind(this));
            this.el.off('mouseleave', '.i-star,.e-star,.h-star')
            this.el.on('mouseleave', '.i-star,.e-star,.h-star', this.starLeave.bind(this));
            this.el.off('click', '.i-star,.e-star,.h-star');
            this.el.on('click', '.i-star,.e-star,.h-star', this.starConfirm.bind(this));
            this.el.off('click', '.auditBtn')
            this.el.on('click', '.auditBtn', this.pass.bind(this));
        },
        starInit: function () {
            var stars = this.stars;
            for (k in stars) {
                var ele = { importance: $('.importance').children().eq(stars[k]), urgency: $('.ergence').children().eq(stars[k]), difficulty: $('.hard').children().eq(stars[k]) }[k];
                ele.addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
                ele.nextAll('i').addClass('fa-star-o').removeClass('fa-star');
            }
            
        },
        init: function (pid, ids, callback,data) {
            this.bindEvent();
            this.callback = callback;
            this.data = data;
            this.el.find('.right-head').text('系数评定');           
            var html = template('passAudit', []);
            this.el.find('#idea').val('');
            $('#detailPlanModal .right-body').html(html);
            if (data) {
                this.stars = {
                    importance: data.importance ? data.importance : 1,
                    urgency: data.urgency ? data.urgency : 1,
                    difficulty: data.difficulty ? data.difficulty : 1
                };
                this.starInit();
            }
            if (ids.normal && ids.loop) {
                this.ids = ids;
                return
            }
         
            this.planId = pid;
            this.parent = parent;
           
            
        },
        check: function () {
            if (!this.stars.importance || !this.stars.urgency || !this.stars.difficulty) {
                ncUnits.alert("存在空白项！");
                return false
            }
            return true;
        },
        pass: function (e) {
            var ele = $(e.currentTarget);
            this.stars.message = $('#comments').val();
            var postData;
            if (ele.hasClass('pass')) {
                this.stars.isApprove = true;
                if (!this.check()) return false;
            } else {
                this.stars.isApprove = false;
                //this.stars.importance = 1;
                //this.stars.urgency = 1;
                //this.stars.difficulty = 1;
            }
            if (!ele.hasClass('pass') && !$('#comments').val()) {
                ncUnits.alert("审核意见不能为空！");
                return
            }

                       
            if (this.ids.normal || this.ids.loop) {
                this.stars.planIdList = this.ids;
                postData = {
                    loop: [],
                    normal:[]
                }
                $.each(this.ids.loop, function (n, val) {
                    var temp = {
                        loopId: val,
                        isApprove: this.stars.isApprove,
                        importance: this.stars.importance,
                        difficulty: this.stars.difficulty,
                        urgency: this.stars.urgency,
                        msg: this.stars.message
                    }
                    postData.loop.push(temp)
                }.bind(this))
                $.each(this.ids.normal, function (n, val) {
                    var temp = {
                        planId: val,
                        isApprove: this.stars.isApprove,
                        importance: this.stars.importance,
                        difficulty: this.stars.difficulty,
                        urgency: this.stars.urgency,
                        msg: this.stars.message
                    }
                    postData.normal.push(temp)
                }.bind(this))
                this.commit(postData.normal,CONSTANT.auditMulti)
                this.commit(postData.loop, CONSTANT.auditLoopMulti,1)
            } else {
                this.stars.planId = this.planId;
                this.stars.loopId = this.planId;
                postData = this.stars;
                postData.msg = this.stars.message;
                var url = this.data.loopId ? CONSTANT.auditLoop : CONSTANT.audit;
                this.commit(postData, url,1)
            }

          

        },
        commit: function (postData, url, isAlert) {

            $.post(url, { data: JSON.stringify(postData) }, function (data) {
                if (data.success) {
                    $('#detailPlanModal').modal('hide');
                    
                    if (isAlert) {
                        ncUnits.alert("审核成功！");
                        this.callback();
                    }
                    
                   
                    return
                }
                if (isAlert) {
                    ncUnits.alert("审核失败！");
                }
                ;
            }.bind(this), 'json')

        }

    }

    //循坏计划提交
    var CircleConfirm = function () {
        this.el = $('#right-content');
        
        this.common = template('commonCommit', []);
        this.isWorkday = '<div class="modal-rows" style="margin-top:10px;margin-bottom:0"><span>工作日:</span></div>'

    }
    CircleConfirm.prototype = {
        bindEvents: function () {
            this.el.off('click', '.commit-circle')
            this.el.on('click', '.commit-circle',this.commonShow.bind(this))
        },
        commonShow: function (e) {
            var ele = $(e.currentTarget);
            ele.siblings('.zone-detail').find('.add-comon').html(this.common);
            ele.parent('.zone-list').siblings('.zone-list').find('.add-comon').html('');
            this.el.find('.common-commit').prepend(this.isWorkday);
            this.el.find('.info-tip').text(90 + '%');
            this.slide();
        },
        slide: function () {
            $('#complete-info').slider({
                orientation: "horizontal",
                range: "min",
                max: (1 * 100 + 1),
                min: 0,
                value: 90,
                slide: function () {
                    var value = $('#complete-info').slider('value');
                    if (value > 90) {
                        $('#complete-info').slider('value', 90);
                        value = 90;
                    }
                    $('.info-tip').text(value + '%')
                },
                change: function () {
                    var value = $('#complete-info').slider('value');
                    if (value > 90) {
                        $('#complete-info').slider('value', 90);
                        value = 90;
                    }
                    $('.info-tip').text(value + '%');
                }
            });
        },
        init: function (loopId) {
            $.post(CONSTANT.circleGet, { loopId: loopId, mode: 0, pageNum: 1 }, function (data) {


            }, 'json');
            var data = {list:[
                { time: '2016-12-22T12:55:00', acutal: '30分钟', effect: '20分钟',status:1},
                { time: '2016-10-22T12:55:00', acutal: '40分钟', effect: '20分钟',status: 2 },
                { time: '2016-09-21T12:55:00', acutal: '50分钟', effect: '20分钟',status:1 },
                { time: '2016-12-10T12:55:00', acutal: '60分钟', effect: '20分钟',status: 1 },
                { time: '2016-04-20T12:55:00', acutal: '70分钟', effect: '20分钟',status: 3 },
                { time: '2016-12-01T12:55:00', acutal: '80分钟', effect: '20分钟', status:2 }

            ]}
            var content = template('circleCommit', data);
            
            $('#detailPlanModal .right-body').html(content);
            
            this.bindEvents();
        }


    }
    //一般计划提交
    var NormalConfirm = function () {
        this.el = $('#right-content');
        this.content = template('confirmCommit', [])
        this.common = template('commonCommit', [])
    }
    NormalConfirm.prototype = {
        init: function () {
            $('#detailPlanModal .right-body').html(this.content);
            $('#detailPlanModal .right-body').append(this.common);
        }


    }
    //提交计划
    var CommitPlan = function () {
        this.el = $('#right-content');
        this.planId;
        this.confirm = new Confirm();
        this.callback;
        this.data;
        this.normal = new NormalConfirm();
        this.circle = new CircleConfirm();
    }
    CommitPlan.prototype = {
        slide: function () {
            $('#complete-info').slider({
                orientation: "horizontal",
                range: "min",
                max: (1 * 100 + 1),
                min: 0,
                value: 90,
                slide: function () {
                    var value = $('#complete-info').slider('value');
                    if (value > 90) {
                        $('#complete-info').slider('value', 90);
                        value = 90;
                    }
                    $('.info-tip').text(value + '%')
                },
                change: function () {
                    var value = $('#complete-info').slider('value');
                    if (value > 90) {
                        $('#complete-info').slider('value', 90);
                        value = 90;
                    }
                    $('.info-tip').text(value + '%');
                }
            });
        },
        configure: function () {
            
            var p1 = $('.minutes').val() ?  parseInt($('.minutes').val(),10) : 0;
            var p2 = $('.numbers').val() ? parseInt($('.numbers').val(), 10) : 0;
            if (isNaN(p1) || isNaN(p2)) {
                return
            }
            var total = p1 * p2;
            this.el.find('.totals').text(total+'分钟')

        },
        change: function (e) {
            var ele = $(e.currentTarget);
            ele.addClass('choosed').siblings().removeClass('choosed');
            if (ele.hasClass('msgOut')) {
                this.el.find('.intro').hide().siblings('.upload').show();
                return
            }
            this.el.find('.intro').show().siblings('.upload').hide();
        },
        remove: function (e) {
            var ele = $(e.currentTarget);
            var fid = ele.parent().attr('fid');
            this.confirm.alert({
                title: '确认删除吗？',
                backdrop: false,
                yes: function () {
                    $.post(CONSTANT.delFile, { fileId: fid ,type:1}, function (data) {
                        ncUnits.alert("删除成功！");
                        ele.parent().remove();
                    })

                },
                
            })
            
        },
        showList: function (data) {
            console.log('ddd',data)
            //var data = data[0];
            var li = '<li fid=' + data.fileId + ' class="existFile"><p class="ellipsis" title=' + data.displayName + '>' + data.displayName + '</p><i class="fa fa-download file-down"></i><i class="fa fa-remove file-remove"></i></li>';
            this.el.find('.list-upload').append(li);
        },
        bindEvent: function () {
            this.el.off('input', '.minutes,.numbers');
            this.el.on('input', '.minutes,.numbers', this.configure.bind(this));
            this.el.off('click', '#uploadFile')
            this.el.on('click', '#uploadFile', this.upload.bind(this));
            this.el.off('click', '.isEvent')
            this.el.on('click', '.isEvent', this.change.bind(this));
            this.el.off('click', '.comt')
            this.el.on('click', '.comt', this.commit.bind(this));
            this.el.off('click', '.file-remove')
            this.el.on('click', '.file-remove', this.remove.bind(this));
            this.el.off('click', '.file-down')
            this.el.on('click', '.file-down', this.downfile.bind(this));

        },
        //附件下载
        downfile: function (e) {
            var ele = $(e.currentTarget);
            var fileId = ele.parent('li').attr('fid');
            $.post(CONSTANT.singleDownload, { fileId: fileId, type: 1 }, function (data) {
                if (data == "error") {
                    return;
                }
                window.location.href = CONSTANT.singleDownload + "?fileId=" + escape(fileId) + "&type=1";

            });
        },
        //数据初始化
        setDefault: function (data) {
            this.el.find('.info-tip').text(data.progress ? data.progress : 90 + '%');
            $('#complete-info').slider('value', data.progress ? data.progress : 90);
            
            this.el.find('.minutes').val(data.time?data.time:'');
            this.el.find('.numbers').val(data.quantity?data.quantity:'');
            this.el.find('.totals').text((data.time?data.time:0) * (data.quantity?data.quantity:0) + '分钟');
            if (data.isAttach == null) {
                return
            }
            if (data.isAttach) {
                this.el.find('.msgOut').addClass('choosed').siblings('.fileOut').removeClass('choosed');
                this.el.find('.intro').hide().siblings('.upload').show();
            } else {
                this.el.find('.fileOut').addClass('choosed').siblings('.msgOut').removeClass('choosed');
                this.el.find('.upload').hide().siblings('.intro').show().find('#introduction').val(data.result);
            }
        },
        //星星展示
        showStar: function (data) {
            $('.importance').find('i').eq(data.importance?data.importance-1:0).addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            $('.ergence').find('i').eq(data.urgency?data.urgency-1:0).addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            $('.hard').find('i').eq(data.difficulty ? data.difficulty-1 : 0).addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');

        },
        init: function (pid, data, callback) {
            this.bindEvent();
            this.planId = pid;
            this.callback = callback;
            this.data = data;
            if (this.data.loopId) {
                this.circle.init(this.data.loopId);
                
            } else {
                this.normal.init();
                //获取附件列表
                $.post(CONSTANT.fileList, { targetId: this.planId, type: 1 }, function (data) {

                    var li = '';
                    $.each(data.data, function (n, val) {
                        li += '<li fid=' + val.fileId + ' class="existFile"><p class="ellipsis" title=' + val.displayName + '>' + val.displayName + '</p><i class="fa fa-download file-down"></i><i class="fa fa-remove file-remove"></i></li>';
                    })
                    this.el.find('.list-upload').html(li);

                }.bind(this), 'json');
                this.slide();
                this.setDefault(data)
                this.showStar(data)
            }
            
            //if ($('#complete-info').length > 0) {
            //    this.setDefault(data);
            //    this.showStar(data)
            //    return
            //}
            //var html = template('confirmCommit', []);
            //$('#detailPlanModal .right-body').html(html);
            
           
           
         
        },
        reset: function () {
            this.el.find('.list-upload').html('');
            this.el.find('input').val('');
            this.el.find('#introduction').val('');
            this.el.find('.totals').text('0分钟');
            this.el.find('.msgOut').addClass('choosed').siblings().removeClass('choosed');
            this.el.find('.intro').hide().siblings('.upload').show();
            this.el.find('.info-tip').text('90%');
        },
        check: function (data) {
            if (!data.time) {
                ncUnits.alert("请输入时间！");
                return false
            }
            if (!data.quantity) {
                ncUnits.alert("请输入件数！");
                return false
            }
            if (data.isAttach && this.el.find('.existFile').length == 0) {
                ncUnits.alert("请上传文件！");
                return false
            }
            if (!data.isAttach && !data.result) {
                ncUnits.alert("请输入非结果事项！");
                return false
            }
            return true
        },
        commit: function () {
            var post = {
                time: this.el.find('.minutes').val(),
                quantity: this.el.find('.numbers').val(),
                planId: this.planId,
                loopId: this.planId,
                progress: $('#complete-info').slider('value'),
                isAttach: this.el.find('.msgOut').hasClass('choosed')?1:0
            }
            if (!post.isAttach) {
                //post.result = this.el.find('#introduction').val().replace(/\s+/g, '');
                post.result = $.trim(this.el.find('#introduction').val());
            }
            console.log('ppp', post);
            if (!this.check(post)) {
                return
            }
            var url = this.data.loopId?CONSTANT.commitLoopConfirm:CONSTANT.commitConfirm
            $.post(url, { data: JSON.stringify(post) }, function (data) {
                $('#detailPlanModal').modal('hide');
                if (data.success) {
                    ncUnits.alert("计划提交成功！");
                    if (this.callback) {
                        this.callback();
                    }
                    return;
                }
                ncUnits.alert("计划提交失败！");
                
            }.bind(this), 'json')
        },
        upload: function () {
            var pid = this.planId;
            fileUpload('#uploadFile', CONSTANT.iconImg, this.showList.bind(this), 2, { type: 1, targetId: pid });
        }


    }



    //一般计划确定
    var ConfirmPlan = function () {
        this.el = $('#right-content');
        this.planId;
        this.time;
        this.num;
        this.callback;
      

    }
    ConfirmPlan.prototype = {
        bindEvent: function () {
            this.el.off('click', '.confirmPass,.confirmPause');
            this.el.on('click', '.confirmPass,.confirmPause', this.commit.bind(this));
            this.el.off('click', '.down-file');
            this.el.on('click', '.down-file', this.downfile.bind(this));
        },
        slide: function (id) {
            $(id).slider({
                orientation: "horizontal",
                range: "min",
                max: (2 * 100 + 1),
                min: 0,
                value: 100,
                slide: function () {
                    var value = Math.ceil(($(id).slider('value')*10)/100);
                    $(id).next('.info-tip').text(value/10);
                    this.configure();
                }.bind(this),
                change: function () {
                    var value = Math.ceil(($(id).slider('value') * 10) / 100);
                    $(id).next('.info-tip').text(value / 10);
                    this.configure();
                }.bind(this)
            });

        },
        reset: function () {
            $('#complete-number').slider('value', 100)
            $('#complete-time').slider('value', 100)
            $('#complete-quality').slider('value', 100);
            this.el.find('.attachList').children('ul').html('');
            this.el.find('.eventMsg').children('pre').html('');
            this.el.find('.common-text').val('');
        },
        commit: function (e) {
            var ele = $(e.currentTarget);
            var flag = ele.hasClass('confirmPass') ? true : false;
            var data = {
                planId: this.planId,
                completeQuantity:flag?this.el.find('.info-num').text():'0',
                completeQuality:flag? this.el.find('.info-qua').text():'0',
                completeTime:flag? this.el.find('.info-time').text():'0',
                isApprove: flag,
                msg: $.trim(this.el.find('#idea').val())
            }
            if (!flag && !data.msg) {
                ncUnits.alert("请输入意见！");
                return
            }
            console.log('dddd', data)
            $.post(CONSTANT.confirm, {data:JSON.stringify(data)}, function (data) {
                $('#detailPlanModal').modal('hide');
                if (data.success) {
                    ncUnits.alert("计划确认成功！");
                    this.callback();
                    return
                }
                ncUnits.alert("计划确认失败！");
            }.bind(this), 'json')
           
        },
        configure: function () {
            var s1 = Number(this.el.find('.info-num').text());
            var s2 = Number(this.el.find('.info-qua').text());
            var s3 = Number(this.el.find('.info-time').text());
            var total = this.time * s1 * s2 * s3*this.num;
            this.el.find('.total-time').text(Math.ceil(total))
        },
        //获取附件
        getAttach: function () {
            $.post(CONSTANT.fileList, { targetId: this.planId, type: 1 }, function (data) {
                var result = data.data;
                var li = '';
                $.each(result, function (n, val) {
                    li += '<li class="ellipsis"><p>' + val.displayName + '</p><i class="fa fa-download down-file" fid=' + val.fileId + '></i></li>'
                })
                this.el.find('.attachList').children('ul').html(li);
            }.bind(this), 'json')
        },
        //附件下载
        downfile: function (e) {
            var ele = $(e.currentTarget);
            var fileId = ele.attr('fid');
            $.post(CONSTANT.singleDownload, { fileId: fileId, type: 1 }, function (data) {
                if (data == "error") {
                    return;
                }
                window.location.href = CONSTANT.singleDownload + "?fileId=" + escape(fileId) + "&type=1";

            });
        },
        //星星展示
        showStar: function (data) {
            $('.importance').find('i').eq(data.importance ? data.importance - 1 : 0).addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            $('.ergence').find('i').eq(data.urgency ? data.urgency - 1 : 0).addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');
            $('.hard').find('i').eq(data.difficulty ? data.difficulty - 1 : 0).addClass('fa-star').removeClass('fa-star-o').prevAll('i').addClass('fa-star').removeClass('fa-star-o');

        },
        init: function (pid, time, num,isAttach ,result,callback,data) {
            this.callback = callback;
            this.planId = pid;
            this.bindEvent();
            if ($('#complete-quality').length  == 0) {
                var html = template('confirmComplete', []);
                $('#detailPlanModal .right-body').html(html);
                this.slide('#complete-quality');
                this.slide('#complete-number');
                this.slide('#complete-time');
            }                       
            this.planId = pid;
            this.time = time;
            this.num = num;
            this.showStar(data)
            if (isAttach) {                
                this.el.find('.attachList').show().siblings('.eventMsg').hide();
                this.el.find('.attach-intro').show().siblings('.event-intro').hide();
                this.el.find('.attach-is').text('是');
                
            } else {
                this.el.find('.attach-is').text('否');
                this.el.find('.attach-intro').hide().siblings('.event-intro').show();
                
                this.el.find('.eventMsg').show().siblings('.attachList').hide();
                this.el.find('.eventMsg').children('pre').html(result);
            }
            
            this.getAttach();
            this.el.find('.time-init').text(this.time);
            this.el.find('.num-init').text(this.num);
            this.el.find('.total-time').text(this.time * this.num);
            this.el.find('.total-init').text(this.time * this.num);
        }


    }


    //计划详情弹窗

    var DetailPlan = function (callback) {
        this.el = $('#detailPlanModal');
        this.type; 
        this.data;
        this.planId;
        this.isLoop;
        this.loop = { 1: '每日循环', 2: '每周循环', 3: '每月循环', 4: '每年循环' };
        this.callback = callback;
        this.newDetail = template('newDetail', []); //一般计划
        this.loopDetail = template('circleDetail', []);//循环计划
        //系数评定
        this.evaluate = new Evaluate();
        //提交系数
        this.commitPlan = new CommitPlan();
        //计划确认
        this.confirmPlan = new ConfirmPlan();
      
        this.el.on("hide.bs.modal", function () {
            this.evaluate.reset();
            this.commitPlan.reset();
            this.confirmPlan.reset();
            this.el.find('.log-info').show().siblings('.comment-info').hide();
            this.el.find('.log-opt').addClass('selected').siblings().removeClass('selected');
            if (this.el.find('.modal-dialog').hasClass('dialog-class')) {
                var m = setTimeout(function () {
                    this.el.find('.modal-dialog').removeClass('dialog-class');
                    this.el.find('.modal-right').css('left','100%');
                    clearTimeout(m);
                }.bind(this), 150)
                
            }
            
            this.el.find('.list-log').html('')
            this.el.find('.tags').html('');
            this.el.find('.persons').html('');
            this.el.find('.addComment').hide();
            this.el.find('.comment-wrap').addClass('addHide')
            var t = setTimeout(function () {
                this.el.find('.modal-content').show();
                this.el.find('.modal-right').show();
                clearTimeout(t);
            }.bind(this), 150)
           
        }.bind(this));
        
    }
    DetailPlan.prototype = {
        bindEvent: function () {
            this.el.off('click', '.tab-log');
            this.el.on('click', '.tab-log', this.tab.bind(this));
            this.el.off('click', '.fillUp');
            this.el.on('click', '.fillUp', this.comment.bind(this));
            this.el.off('click', '.addComment');
            this.el.on('click', '.addComment', this.showAdd.bind(this));
            this.el.off('keydown', '#commentCommit');
            this.el.on('keydown', '#commentCommit', this.say.bind(this));
        },
        say: function (e) {
            if (e.keyCode == 13) {
                e.preventDefault()
                this.el.find('.fillUp').click();
                return;
            }
        },
        //评论弹出显示
        showAdd: function () {
            this.el.find('.comment-wrap').toggleClass('addHide')

        },
        //提交评论
        comment: function () {
            var text = $.trim($('#commentCommit').val());
            $('#commentCommit').val('').focus();
            if (!text) {
                return;
            }
            $.post(CONSTANT.upComment, { data: JSON.stringify({ suggestion: text, planId: this.planId }) }, function (data) {
                if (data.success == true) {
                    var now = new Date(+new Date() + 8 * 3600 * 1000).toISOString().replace(/T/g, ' ').replace(/\.[\d]{3}Z/, '');//当前格式化时间
                    var icon = $('.icon-center').attr('src');
                    var me = $('.person-show').text();
                    var $html = '\
                        <li class="list-all">\
                            <img src='+icon+' alt="Alternate Text" class="img-circle img-comment" />\
                             <ul class="comment-main">\
                                <li>'+me+'</li>\
                                <li><p class="ellipsis" title='+text+'>' + text + '</p><span>' + now + '</span></li>\
                             </ul>\
                        </li>\
                        '
                    this.el.find(".comment-list").prepend($html);

                }
               
            }.bind(this), 'json')
          
            
        },
        tab: function (e) {
            var ele = $(e.currentTarget);
            ele.addClass('selected').siblings().removeClass('selected');
            if (ele.hasClass('log-opt')) {
                this.el.find('.comment-wrap').addClass('addHide');
                this.el.find('.addComment').hide();
                this.el.find('.log-info').show().siblings('.comment-info').hide();
                this.logInfo();
                this.getlog();
                return
            }

            this.el.find('.log-info').hide().siblings('.comment-info').show();
            this.el.find('.addComment').show();
            this.commentInfo();
        },
        logInfo: function () {
         
        },
        commentInfo: function () {
            $.post(CONSTANT.comment, { planId: this.planId }, function (data) {
                var $list = '';
                $.each(data.data, function (n, val) {
                    $list += '\
                        <li class="list-all">\
                            <img src=' + val.replyUserImage + ' alt="Alternate Text" class="img-circle img-comment" />\
                             <ul class="comment-main">\
                                <li>' + val.replyUserName + '</li>\
                                <li><p class="ellipsis"  title=' + val.suggestion + '>' + val.suggestion + '</p><span>' + val.createTime.substring(0, 19).replace("T", " ") + '</span></li>\
                             </ul>\
                        </li>\
                        '
                })
                this.el.find(".comment-list").html($list);
            }.bind(this), 'json')
           
            

        },
        rend: function (data) {
            this.el.find('.do-method').text(data.executionMode);
            this.el.find('#confirmUser').val(data.confirmUserName);
            this.el.find('#responsibleUser').val(data.responsibleUserName);
            this.el.find('#showEnd').val(data.endTime.substring(0, 10));
            if (data.startTime) {
                this.el.find('#showStart').val(data.startTime.substring(0, 10));
            }
            if (data.loopType) {
                this.el.find('#roundType').val(this.loop[data.loopType]);
            }
            this.el.find('#showPut').val(data.eventOutput);
            this.el.find('.part-level').text(data.organizationInfo);
            this.el.find('.response-icon').attr('src', data.responsibleUserImage);
            this.el.find('.confirm-icon').attr('src', data.confirmUserImage);
            if (data.initial == 1) {
                this.el.find('.tempOpt').eq(0).show().siblings('.tempOpt').hide();
            } else {
                this.el.find('.tempOpt').eq(1).show().siblings('.tempOpt').hide();
            }
            
            
            if (data.keyword) {
                var taglist = '';
                $.each(data.keyword, function (n, val) {
                    taglist += '<li class="person-user tags-add"><p class="ellipsis"  title=' + val + '>' + val + '</p><span>；</span></li>';
                })
                this.el.find('.tags').html(taglist);
            }
            if (data.partnerInfo) {
                var personlist = '';
                $.each(data.partnerInfo, function (n, val) {
                    personlist += '<li class="person-user"><img src=' + val.headImage + ' alt="Alternate Text" class="img-circle img-help"/><p class="ellipsis" title=' + val.userName + '>' + val.userName + '</p><span>；</span></li>';
                })
                this.el.find('.persons').html(personlist);
            }
            
        },
        operate:function(data){
            var type = Number(this.type); //type类型 1审核弹窗 2为提交弹窗 3为计划确认弹窗
            switch (type) {
                case 1:
                    this.evaluate.init(this.planId, '', this.callback, data);
                    break
                case 2:
                    this.commitPlan.init(this.planId, data, this.callback);
                    break
                case 3:
                    this.confirmPlan.init(this.planId, data.time, data.quantity, data.isAttach,data.result,this.callback,data);
                    break
                default:
                    this.el.find('.modal-right').hide();

            }
        },
        //批量审核操作
        multi: function (ids) {
            this.el.find('.modal-content').hide();
            this.el.modal('show');
            this.el.find('.modal-dialog').addClass('dialog-class');
            this.el.find('.modal-right').css('left', '0');
            this.evaluate.init('', ids, this.callback);
        },
        //获取操作日志
        getlog: function () {
            if (this.isLoop == 1) {
                var url = CONSTANT.getLoopLog;
                var param = {loopId:this.planId}
            } else {
                var url = CONSTANT.getLog;
                var param = { planId: this.planId }
            }
            $.post(url, param, function (data) {
                var li = '';
                $.each(data.data, function (n, val) {
                    var msg = val.message ? '<i class="fa fa-commenting-o comment-icon"></i>' : '';
                    val.message = val.message ? val.message.replace(/\r?\n/g, ' ') : '';
                    var tip = val.message ? 'data-toggle="tooltip" data-placement="right" data-title="' + val.message + '" data-html="true"' : '';
                                      
                    li += '<li><span class="person-log">' + val.operateUser + '</span><span class="operate-log" '+tip+'>' + val.operateInfo + ''+msg+'</span><span class="time-log">' + val.operateTime.replace('T', ' ').substring(0, 16
                        ) + '</span></li>';

                })

                this.el.find('.list-log').html(li)
                $('[data-toggle="tooltip"]').tooltip();
                $('.operate-log').on('show.bs.tooltip', function () {
                    var that = $(this);
                    var t = setTimeout(function () {
                        var ele = that.siblings('.tooltip');
                        var top = ele.position().top;
                        var height = ele.outerHeight();
                        if (top < 0) {
                            ele.css('top', '1px');
                            ele.find('.tooltip-arrow').css('top','12px')
                        }
                        clearTimeout(t);
                    }, 50)
                    
                })
            }.bind(this), 'json')
        },
        loadType: function (isLoop) {
            //var isLoop = 1;
            var html = isLoop == 1 ? this.loopDetail : this.newDetail;            
            this.el.find('#detailLoad').html(html)
        },

        show: function (pid, type,isloop) {
            this.el.modal('show');
            this.planId = pid;
            this.type = type;
            this.isLoop = Number(isloop); //0为一般计划，1为循环计划
            if (isloop == 1) {
                var param = { loopId: pid };
                var url = CONSTANT.detailLoop;
                this.el.find('.commont-opt').hide();
            } else {
                var param = { planId: pid };
                var url = CONSTANT.detail;
                this.el.find('.commont-opt').show();
            }
            this.loadType(this.isLoop); //循环计划和一般计划区别
            this.bindEvent();
            //this.logInfo();          
            this.getlog();            
            $.post(url, param, function (data) {
                this.data = data.data;
                this.rend(data.data);
                this.operate(data.data)
            }.bind(this), 'json')


        }

    }


    //新建计划弹窗
    var AddPlan = function () {
        this.el = $('#addPlanModal');
        this.data = {
            partnerInfo: [], //协作人
            keyword: []   //标签
        };
        this.loop = { 1: '每日循环', 2: '每周循环', 3: '每月循环', 4: '每年循环' };
        this.orgInfo
        this.planId;
        this.newPlan = template('newPlan', []); //新建计划
        this.circlePlan = template('circlePlan', []);//循环计划
        
        this.runmode = [];//完成方式
        var end = {
            elem: '#endTimes',
            event: 'click',
            format: 'YYYY-MM-DD',
            isclear: true,
            istoday: true,
            issure: true,
            festival: true,
            istime: false,
            start: '2014-6-15',
            choose: function (date) {
                start.max = date;
            },
            clear: function () {
                start.max = undefined;
            }
        };
        var start = {
            elem: '#startTimes',
            event: 'click',
            format: 'YYYY-MM-DD',
            isclear: true,
            istoday: true,
            issure: true,
            festival: true,
            istime: false,
            start: '2014-6-15',
            choose: function (date) {
                end.min = date
            },
            clear: function () {
                end.min = undefined;
            }
        };
       this.events = function (e) {
            var ele = $(e.currentTarget);
            if (ele.hasClass('tempOpt')) {
                this.isTemp(ele);
                return;
            }
            if (ele.hasClass('output')) {
                this.outPut(ele);
                return;
            }
            if (ele.hasClass('circlePut')) {
                this.circlePut(ele);
                return;
            }
            if (ele.hasClass('ueser-input')) {
                this.showList(ele);
                return
            }
            if (ele.hasClass('search-input')) {
                e.stopPropagation();                
                return
            }
            if (ele.hasClass('helpput')) {
                this.helpPut(ele)
                return
            }
            if (ele.hasClass('savePlan') || ele.hasClass('commitPlan')) {
                this.update(ele)
                return
            }
            if (ele.hasClass('memberput')) {
                this.memberPut(ele)
                return
            }
            if (ele.hasClass('removeList')) {
                this.removeList(ele)
                return
            }
            if (ele.hasClass('partput')) {
                this.choosePart(ele)
                return
            }
            if (ele.hasClass('tagChoose')) {
                this.chooseTag(ele)
                return
            }

            if (ele.hasClass('addMember')) {
                e.stopPropagation();
                ele.parents('.detail-person').click();
                this.showMenu(ele);
                return
            }
            if (ele.attr('id') == 'endTimes' || ele.attr('id') == 'startTimes') {
                var type = ele.attr('id') == 'endTimes' ? end : start;
                var obj = new Date();
                var now = obj.getFullYear() + '-' + (obj.getMonth() + 1) + '-' + obj.getDate();
                type.start = now;                
                laydate(type);
                if (this.planType == 'newPlan') {
                    $('#laydate_box').addClass('laydateStyle').removeClass('laydateOther')
                } else {
                    $('#laydate_box').addClass('laydateOther').removeClass('laydateStyle')
                }
                
                return
            }
        }
        this.change = function (e) {
            var ele = $(e.currentTarget);
            if (ele.hasClass('userTag') || ele.hasClass('userTarget')) return;
            clearTimeout(this.listMember.tId);
            this.listMember.tId = setTimeout(function () {
                this.listMember.call(this.el, ele);
            }.bind(this), 700)
        };
        this.tagAdd = function (e) {
            var ele = $(e.currentTarget);
            if (e.keyCode == 13) {
                this.addTag(ele);
            }

        }
        
        this.el.on('click', '.detail-person *', function (e) {
            e.stopPropagation();
        })
        this.el.on("hide.bs.modal", function () {
            $('#laydate_box').removeClass('laydateOther').removeClass('laydateStyle');
            this.data = { partnerInfo: [], keyword: [] };
            start.max = undefined;
            end.min = undefined;
            this.el.find('input').val('').removeAttr('pid');
            $('.do-method').removeAttr('rid').text('完成方式');
            $('.part-level').find('span').removeAttr('oid').text('请选择部门');
            $('.do-circle').attr('flag', '1').text('每日循环');
            this.planId = '';
            this.planType = '';
            this.el.find('.person-user').remove();
            this.el.find('.responsed-icon').attr('src', '../../Images/index/pic0.png');
            this.el.find('.confirmd-icon').attr('src', '../../Images/index/pic0.png');
            $('.tempOpt').eq(1).addClass('select').siblings().removeClass('select');
            $('.person-user').remove();
        }.bind(this));

    }
    AddPlan.prototype = {
        planType :'',
        //选择部门
        choosePart: function (ele) {
            var text = ele.text();
            $('.part-level span').text(text).attr('oid', ele.attr('oid'));
        },
        //是否是临时计划
        isTemp: function (ele) {
            ele.addClass('select').siblings().removeClass('select');
        },
        //完成方式
        outPut: function (ele) {
            var word = ele.text();
            $('.do-method').text(word).attr('rid',ele.attr('rid'));
        },
        //循环方式
        circlePut: function (ele) {
            var word = ele.text();
            $('.do-circle').text(word).attr('flag', ele.attr('flag'));
        },
        //责任人和确认人列表
        showList: function (ele) {
            var child = ele.find('.user-menu');
            if(child.is(':hidden')){
                ele.find('.search-input').val('');
                //ele.children('.list-contain').find('ul').html('');
                //return;
            }
            var t = setTimeout(function () {
                ele.find(".search-input").focus();
                clearTimeout(t);
                this.listMember(ele.find('.search-input'))
            }.bind(this), 10)
        },
        //选择标签
        chooseTag: function (ele) {
            var ele = ele
            var word = ele.text();
            if (this.data.keyword.indexOf(word) > -1) {
                return
            }
            this.data.keyword.push(word);
            var list = '<li class="person-user tags-add"><p class="ellipsis"  title=' + word + '>' + word + '</p><span>；</span><i class="fa fa-remove removeList removeTag"></i></li>';
            ele.parents('.modal-rows').find('.add-list').before(list);
            this.showMenu(ele.parents('.modal-rows').find('.addMember'), true);
        },
        //搜索标签
        selectTag: function (ele) {
            var ele = ele;
            $.post(CONSTANT.getTags, {}, function (data) {
                if (!data.data) {
                    return;
                }
                var li = '';
                $.each(data.data, function (n, val) {
                    li += '<li class="eventput tagChoose">' + val + '</li>'

                })
                ele.parent('.search-link').siblings('.list-contain').find('ul').html(li);
            }, 'json')

        },
        //搜索成员
        listMember: function (ele) {
            var ele = ele;
            if (ele.hasClass('userTag')) {
                this.selectTag(ele)
                return
            }
            if (ele.hasClass('userTarget')) {

                return
            }
            var tp = ele.hasClass('confirmUser') ? 1 : 2;
            var text = ele.val().replace(/\s+/g, '');
            var url = text ? CONSTANT.searchUser : CONSTANT.contactUser;
            tp = ele.hasClass('helpMember') ? 99 : tp;
            var param = text ? { userName: text } : { type: tp };
            $.post(url, param, function (data) {
                var list = '';
                if (data.data.length == 0) {
                    list = '<li class="msg-tip">---暂无数据---</li>';
                } else {
                    var link = !text ? '<li>常用联系人</li>' : '';
                    var className = ele.hasClass('helpMember') ? 'helpput' : 'memberput';
                    $.each(data.data, function (n, val) {
                        list += '<li class="eventput ' + className + '" pid=' + val.userId + '><img src=' + val.headImage + ' class="img-circle img-lists" alt="Alternate Text" />' + val.userName + '</li>'
                    })
                    list = link+list
                }
                ele.parent().next('.list-contain').find('ul').html(list);
            }, 'json')
        },
        //获取责任人和确认人搜索结果
        memberPut: function (ele) {
            var p = ele.parents('.select-person');
            var g = ele.parents('.userContact');
            var src = ele.find('img').attr('src');
            var text = ele.text();
            var pid = ele.attr('pid');
            if (text == g.siblings('.userContact').find('.memberShow').val()) {
                ncUnits.alert("责任人与确认人不能相同");
                return
            }
            p.find('.memberShow').val(text).attr('pid',pid);
            p.find('.img-select').attr('src', src);
        },
        //增加标签
        addTag: function (ele) {
            var word = ele.val().replace(/\s+/g, '');
            ele.val('').focus();
            if (this.data.keyword.indexOf(word) > -1 || !word) {
                return
            }
            this.data.keyword.push(word);
            var list = '<li class="person-user tags-add"><p class="ellipsis"  title=' + word + '>' + word + '</p><span>；</span><i class="fa fa-remove removeList removeTag"></i></li>'
            ele.parents('.modal-rows').find('.add-list').before(list);
            this.showMenu(ele.parents('.modal-rows').find('.addMember'), true);
        },
        //删除协作人和标签
        removeList: function (ele) {
            var index = ele.parent('.person-user').index();
            var add = ele.parents('.person-lists').find('.addMember');
            ele.parent('.person-user').remove();
            this.showMenu(add);
            if (ele.hasClass('removeTag')) {
                this.data.keyword.splice(index, 1)
                return
            }
            this.data.partnerInfo.splice(index, 1);
            
        },
        //获取协作人和目标搜索结果
        helpPut: function (ele) {
            var src = ele.find('img').attr('src');
            var text = ele.text();
            var pid = Number(ele.attr('pid'));
            if (this.data.partnerInfo.indexOf(pid) > -1) {
                return
            }
            this.data.partnerInfo.push(pid);
            var list = '<li class="person-user"><img src=' + src + ' alt="Alternate Text" class="img-circle img-help"/><p class="ellipsis" title='+text+'>' + text + '</p><span>；</span><i class="fa fa-remove removeList removeHelp"></i></li>'
            ele.parents('.modal-rows').find('.add-list').before(list)
        },
        //协作人和标签搜索框位置
        showMenu: function (ele, flag) {
            
            var p = ele.parent('li');
            console.log(2222, p)
            var pW = p.width();
            var lastIndex = p.position().left;
            var maxWidth = p.parents('.modal-rows').width();
            var menu = p.parents('.detail-person').next();
            var cW = menu.width()+2;
            var configure = lastIndex + pW + 6 + cW;
            console.log('lastIndex', lastIndex)
            if (configure <= maxWidth || !p.siblings('.person-user')) {
                ele.parents('.detail-person').next().css({ 'margin-left': (lastIndex + pW+6) + 'px','margin-top': '-30px' });
            } else {
                ele.parents('.detail-person').next().css({ 'margin-left': (maxWidth - cW) + 'px', 'margin-top': '0px' });
            }
            menu.find('.search-input').focus();
            
            if (flag) return;
            this.listMember(menu.find('.search-input'));
           
        },
        check: function (result) {
            if (!result.organizationId) {
                ncUnits.alert("请选择部门！");
                return false;
            }
            if (!result.eventOutput) {
                ncUnits.alert("请输入计划名称！");
                return false;
            }
            if (!result.responsibleUser) {
                ncUnits.alert("请选择责任人！");
                return false;
            }
            if (!result.confirmUser) {
                ncUnits.alert("请选择确认人！");
                return false;
            }
            if (!result.endTime) {
                ncUnits.alert("请选择结束时间！");
                return false;
            }
            if (!result.executionModeId) {
                ncUnits.alert("请选择完成方式！");
                return false;
            }
            return true;
        },
        //获取操作日志
        getlog: function () {
            if (this.planType == 'circlePlan') {
                var url = CONSTANT.getLoopLog;
                var param = { loopId: this.planId }
            } else {
                var url = CONSTANT.getLog;
                var param = { planId: this.planId }
            }
            $.post(url, param, function (data) {
                var li = '';
                $.each(data.data, function (n, val) {
                    var msg = val.message ? '<i class="fa fa-commenting-o comment-icon"></i>' : '';
                    val.message = val.message ? val.message.replace(/\r?\n/g, ' ') : '';
                    var tip = val.message ? 'data-toggle="tooltip" data-placement="right" title=' + val.message + '' : '';
                    li += '<li><span class="person-log">' + val.operateUser + '</span><span class="operate-log" ' + tip + '>' + val.operateInfo + '' + msg + '</span><span class="time-log">' + val.operateTime.replace('T', ' ').substring(0, 16
                        ) + '</span></li>';

                })
                this.el.find('.list-log').html(li)
                $('[data-toggle="tooltip"]').tooltip();
                $('.operate-log').on('show.bs.tooltip', function () {
                    var that = $(this);
                    var t = setTimeout(function () {
                        var ele = that.siblings('.tooltip');
                        var top = ele.position().top;
                        var height = ele.outerHeight();
                        if (top < 0) {
                            ele.css('top', '1px');
                            ele.find('.tooltip-arrow').css('top', '12px')
                        }
                        clearTimeout(t);
                    }, 50)

                })
            }.bind(this), 'json')
        },
        //提交和保存计划
        update: function (ele) {
            console.log('pssssp', this.data.partnerInfo)
            if (this.planType == 'newPlan') {
                //新建普通计划
                this.data.initial = $('.isTemp .select').attr('flag');
                var url = CONSTANT.planAdd;
            } else {
                //循环计划
                this.data.loopType = $('.do-circle').attr('flag'); //循环类型
                this.data.startTime = $('#startTimes').val() ? $('#startTimes').val() + ' 23:00:00' : '';
                var url = CONSTANT.planLoopAdd
            }         
            this.data.eventOutput = $('#outPut').val().replace(/\s+/g, '');
            this.data.responsibleUser = Number($('#responsibleUsers').attr('pid'));
            this.data.confirmUser = Number($('#confirmUsers').attr('pid'));
            this.data.endTime = $('#endTimes').val()?$('#endTimes').val() + ' 23:59:59':'';
            this.data.executionModeId = $('.do-method').attr('rid');
            this.data.organizationId = $('.part-level').find('span').attr('oid');
            if (this.planId) {
                this.data.planId = this.planId;
            }
            this.data.isSubmit = ele.hasClass('savePlan') ? 0 : 1;
           
            var data = this.data;
          
     
            if (!this.check(data)) return;
            var rIdx = data.partnerInfo.indexOf(data.responsibleUser);
            if (rIdx > -1) {
                data.partnerInfo.splice(rIdx, 1);
            }
            var cIdx = data.partnerInfo.indexOf(data.confirmUser);
            if (cIdx > -1) {
                data.partnerInfo.splice(cIdx, 1);
            }
            data.partnerInfo = data.partnerInfo.map(function (val) {
                return { userId: val }
            })
            $.post(url, { data: JSON.stringify(data) }, function (data) {
                this.el.modal('hide');
                if (data.success) {                    
                    ncUnits.alert('提交成功');
                    planRefresh.refreshView();
                    return;
                }
                ncUnits.alert('提交失败')
            }.bind(this),'json')
        },
        //后台获取数据加载
        init: function () {
            var list = '';
            $.each(this.runmode, function (n, val) {
                list += '<li class="eventput output" rid=' + val.executionId + '>' + val.executionMode + '</li>'
            })
            $('.runmode').html(list);            
            
        },
        //获取部门
        getPart: function () {
            if (this.orgInfo) return;
            $.post(CONSTANT.orgInfo, {}, function (data) {
                console.log('level', data.data);
                this.orgInfo = data.data;
                var li = '';
                $.each(data.data, function (n,val) {
                    li += '<li class="eventput partput" oid='+val.orgId+'>' + val.orgInfo + '</li>'
                })
                this.el.find('.partlist').html(li)
            }.bind(this), 'json')
        },
        edit: function (pid, type, isLoop) {
            this.show(type,1);
            this.planId = pid;
            this.getlog();
            var isLoop = isLoop
            if (isLoop == 1) {
                var param = { loopId: pid };
                var url = CONSTANT.detailLoop;
                this.el.find('.commont-opt').show();
            } else {
                var param = { planId: pid };
                var url = CONSTANT.detail;
                this.el.find('.commont-opt').hide();
            }
            $.post(url, param, function (result) {
                var result = result.data;
                this.el.find('#confirmUsers').val(result.confirmUserName).attr('pid', result.confirmUser);
                this.el.find('#responsibleUsers').val(result.responsibleUserName).attr('pid', result.responsibleUser);
                this.el.find('#endTimes').val(result.endTime.substring(0, 10));
                this.el.find('#outPut').val(result.eventOutput);
                this.el.find('.part-level').find('span').text(result.organizationInfo).attr('oid', result.organizationId);
                this.el.find('.do-method').text(result.executionMode).attr('rid', result.executionModeId);
                this.el.find('.responsed-icon').attr('src', result.responsibleUserImage);
                this.el.find('.confirmd-icon').attr('src', result.confirmUserImage);
                if (isLoop == 1) {
                    this.el.find('#startTimes').val(result.startTime.substring(0, 10));
                    this.el.find('.do-circle').text(this.loop[result.loopType]).attr('flag', result.loopType);
                }
                if (result.initial == 1) {
                    $('.tempOpt').eq(1).addClass('select').siblings().removeClass('select');
                } else {
                    $('.tempOpt').eq(0).addClass('select').siblings().removeClass('select');
                }
                if (result.partnerInfo) {
                    var li = '';
                    this.data.partnerInfo = result.partnerInfo.map(function (val) {
                        return val.userId;
                    })
                    console.log('pp', this.data.partnerInfo)
                    $.each(result.partnerInfo, function (n, val) {
                       
                        li += '<li class="person-user"><img src=' + val.headImage + ' alt="Alternate Text" class="img-circle img-help"/><p class="ellipsis" title=' + val.userName + '>' + val.userName + '</p><span>；</span><i class="fa fa-remove removeList removeHelp"></i></li>'
                    }.bind(this))
                    this.el.find('.col-lists').children('.add-list').before(li);
                }
                if (result.keyword) {
                    var list = '';
                    $.each(result.keyword, function (n, val) {
                        this.data.keyword.push(val)
                        list += '<li class="person-user tags-add"><p class="ellipsis"  title=' + val + '>' + val + '</p><span>；</span><i class="fa fa-remove removeList removeTag"></i></li>';
                    }.bind(this))
                    this.el.find('.tag-lists').children('.add-list').before(list);
                }

            }.bind(this), 'json')
        },
        //循环和一般计划加载不同html
        load: function (type,isEdit) {
            console.log('type', this.planType)            
            if ((type == 'newPlan' && $('.newPlanHtml').length > 0) || (type == 'circlePlan' && $('.circlePlanHtml').length > 0)) {
                if (!isEdit) {
                    this.getDefault();
                } 
                return;
            }
            var html = type == 'newPlan' ? this.newPlan : this.circlePlan;
            var title = type == 'newPlan' ? '新建计划' : '循环计划';
            this.el.find('.modal-title').text(title);
            var remove = type == 'newPlan' ? '.circlePlanHtml' : '.newPlanHtml';
            this.el.find('#load-after').after(html);
            $(remove).remove();
            if (!isEdit) {
                this.getDefault();
            }
        },
        //获取默认信息
        getDefault: function () {
            var url = this.planType == 'newPlan' ? CONSTANT.planDefault : CONSTANT.planLoopDefault;
            $.post(url, {}, function (data) {
                var data = data.data;
                this.el.find('.part-level').find('span').text(data.organizationInfo).attr('oid', data.organizationId);
                this.el.find('#confirmUsers').val(data.confirmUserName).attr('pid', data.confirmUser);
                this.el.find('#responsibleUsers').val(data.responsibleUserName).attr('pid', data.responsibleUser);
                this.el.find('.responsed-icon').attr('src', data.responsibleUserImage);
                this.el.find('.confirmd-icon').attr('src', data.confirmUserImage);
                if (data.loopType) {
                    this.el.find('.do-circle').text(this.loop[result.loopType]).attr('flag', loopType);
                }
                this.el.find('.do-method').text(data.executionMode).attr('rid', data.executionModeId);
            }.bind(this),'json')
        },
        show: function (type,isEdit) {
            console.log("type", type);
            this.el.modal('show');
            this.bindEvents();
            this.planType = type;
            this.load(type, isEdit);
            if (!isEdit) {
                this.el.find('.commentPlan').hide();
            } else {
                this.el.find('.commentPlan').show();
            }
            this.getPart();
            

            if (this.runmode.length > 0) {
                this.init()
                return;
            }
            $.post(CONSTANT.doMethod, {}, function (data) {               
                this.runmode = data.data;
                this.init()
                
            }.bind(this), 'json')
        },
        bindEvents: function () {
            this.el.off('click', '.tagChoose,.tempOpt,.output,.ueser-input,.search-input,.memberput,#endTimes,#startTimes,.addMember,.helpput,.removeList,.commitPlan,.partput,.savePlan,.circlePut');
            this.el.on('click', '.tagChoose,.tempOpt,.output,.ueser-input,.search-input,.memberput,#endTimes,#startTimes,.addMember,.helpput,.removeList,.commitPlan,.partput,.savePlan,.circlePut', this.events.bind(this)); //click事件绑定
            this.el.off('input', '.search-input');
            this.el.on('input', '.search-input', this.change.bind(this));
            this.el.off('keydown', '.userTag');
            this.el.on('keydown', '.userTag', this.tagAdd.bind(this));
        }
    }


    //文件预览
    var Files = function (callback) {
        this.el = $('#fileMoal');
        this.pid;
        this.idx;
        this.isLoop;
        this.callback = callback?callback:'';
        this.confirmPlan = new DetailPlan(this.callback);//确认提交
        this.isImg = [".bmp", ".jpg", ".tiff", ".gif", ".pcx", ".tga", ".raw", ".png"];
        this.events = function (e) {
            var ele = $(e.currentTarget);
            if (ele.hasClass('downfile')) {
                e.stopPropagation();
                var fileId = ele.attr('fid');
                //var saveName = ele.attr('saveName');
                this.downfile(fileId);
                return;

            }
            if (ele.hasClass('downfiles')) {
                e.stopPropagation();
                this.downfiles();
                return;

            }
            if (ele.hasClass('confirmFile')) {
                this.el.modal('hide');
                //ele.trigger('confirm', [this.idx])
                this.confirmPlan.show(this.pid, 3, this.isLoop);
                return
            }
            if (ele.hasClass('previewfile')) {
                var extension = ele.find('.downfile').attr('extension');
                var savename = ele.find('.downfile').attr('saveName');
                var attachmentName = ele.find('.downfile').attr('attachmentname');
                ele.addClass('selected').siblings().removeClass('selected');
                $('.list-title').text(ele.find('p').text());
                this.preview(savename, extension, attachmentName)
                return
            }
        }


    }
    Files.prototype = {
        bindEvent: function () {
            this.el.off('click', '.downfile,.downfiles,.previewfile,.confirmFile');
            this.el.on('click', '.downfile,.downfiles,.previewfile,.confirmFile', this.events.bind(this));
        },
        preview: function (savename, extension, attachmentName) {
            var isImg = this.isImg;
            console.log('atta', attachmentName)
            $('.nopreview').attr('attachmentName', attachmentName).attr('saveName', savename)
            $.post(CONSTANT.filePreview, { saveName: savename, extension: extension }, function (d) {
                $('.imgHref').remove();
                $('.flashShow').remove();
                if (!d.data) {
                    $('.is_exit').show()
                    return;
                }
                var src = d.data + '?' + Date.now();
                if (isImg.indexOf(extension) > -1) {
                    var html = '<a href=' + src + ' class="imgHref" target="_blank"><img src=' + src + '/></a>';
                    $('.preview-main').addClass('img-center')
                } else {
                    var html = '<embed class="flashShow" type="application/x-shockwave-flash" src=' + src + '/>';
                    $('.preview-main').removeClass('img-center')
                }
                $('.is_exit').hide()
                $('.preview-main').append(html);
            }, 'json')

        },
        downfiles: function () {
            var targetId = this.pid
            $.post(CONSTANT.multiDownload, { targetId: targetId, type: 1 }, function (data) {
                if (data == "error") {
                    return;
                }
                window.location.href = CONSTANT.multiDownload + "?targetId=" + targetId + "&type=1";

            });
        },
        downfile: function (fileId) {
            $.post(CONSTANT.singleDownload, { fileId: fileId, type: 1 }, function (data) {
                if (data == "error") {
                    return;
                }
                window.location.href = CONSTANT.singleDownload + "?fileId=" + escape(fileId) + "&type=1";

            });
        },
        showlist: function (result, pid) {
            console.log(11111, result)
            $('.list-title').text(result[0].attachmentName);
            var list = ''
            $.each(result, function (n, val) {
                var select = n == 0 ? 'selected' : '';
                list += '<li class="previewfile ' + select + '"><p class="ellipsis">' + val.displayName + '</p><i class="fa fa-download downfile" fid=' + val.fileId + ' displayName="" saveName=' + val.saveName + ' extension=' + val.extension + '></i></li>'
            })
            $('.file-main').html(list)

        },
        show: function (pid, ele,isLoop) {
            this.el.modal('show');
            this.isLoop = isLoop;
            this.bindEvent();
            if (ele) {
                this.idx = ele.parents('.li-check').attr('idx');
                console.log('eew', this.idx)
            }
            var pid = pid;
            this.pid = pid;
            $.post(CONSTANT.fileList, { targetId: pid, type: 1 }, function (data) {
                var result = data.data;
                if(result.length == 0 || !result) return;
                var extension = result[0].extension;
                var savename = result[0].saveName;
                var attachmentName = result[0].attachmentName;
                this.showlist(result, pid);
                this.preview(savename, extension, attachmentName)

            }.bind(this), 'json')
        }
    };

    return {
        icon: IconEdit,//修改头像
        files: Files, //文件预览
        confirm: Confirm, //确认弹窗
        pwd: EditPwd, //修改密码
        addplan: AddPlan, //添加计划
        detail: DetailPlan,//计划详情
        trans: PlanTrans, //计划转办
        other:PlanOther
    }





  
})