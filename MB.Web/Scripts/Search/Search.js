/**
 * Created by DELL on 2015/10/21.
 */
//(function () {

var searchData;

  function ViewHttp() {
      var req;     
      return function (url, keyword, func, scope) {
          if (req != null) {
              req.abort();
          }
          var query = { type: url, keyword: keyword };
          var func = func;
          var scope = scope;
              req =  $.ajax({
                        type: "post",
                        url: "/Search/GetSearchResult",
                        data: { data:JSON.stringify(query)},
                        dataType: "json",
                        success: rsHandler(function (data) {
                            searchData = data
                            func.call(scope,data)
                    
                        })
                 })


      }
  }



    //Model负责数据处理
    var Model = function (str) {
        //this.http = new Http();
        this.controllers = str.controllers;
        this.scope = str.scope;
        //this.keywords = [];
        //this.keywords.push(tagNames);
        this.keywords = tagNames.split('@');
        this.url = 1;
        this.controller = 'planShowController';
        this.http = new ViewHttp();

    };

    //Model.prototype.get = function (url, keyword,mm) {
    //    console.log('wqqw',Http(url,keyword))
    //    return Http(url,keyword,mm);
    //};
    //拆分关键词到输入框
    Model.prototype.keyWordSplit = function (words) {
        this.keywords = words;
        var data = this.keywords.join(' ');
        this.controllers['keyWordSplitController'].call(this.scope, data)
    }
    Model.prototype.keyWordRemove = function (idx) {
        this.keywords.splice(idx, 1);
        this.toKeyword();
    }
    Model.prototype.keyWord = function (words) {
        this.keywords = words.replace(/(^\s+)|(\s+$)/g, "");
        this.keywords = this.keywords.replace(/\s+/g, ' ');
        if (!this.keywords) return;
        this.keywords = this.keywords.split(' ');
        this.toKeyword();
    }
    Model.prototype.toKeyword = function () {
        this.controllers['keyWordController'].call(this.scope, this.keywords);
        if (this.keywords.length == 0) return;
        this.updateView()
    }


    //从服务器获取数据更新视图
    Model.prototype.updateView = function (info) {
        this.url = info?info.url:this.url;
        this.controller = info ? info.controller : this.controller;
        //var data = Http(this.url, this.keywords)
        //this.controllers[this.controller].call(this.scope, data);
        //var http = Http();
        this.http(this.url, this.keywords, this.controllers[this.controller], this.scope);
    };
    //初始化视图
    Model.prototype.initData = function () {
        var key, data;
        var mm = 'you';
        //var http = Http();
        for (key in this.scope.init) {
            //data = key == 'keyWordController' ? this.keywords : Http(this.scope.init[key], this.keywords);
            if (key == 'keyWordController') {
                this.controllers[key].call(this.scope, this.keywords);
            } else {
                this.http(this.scope.init[key], this.keywords, this.controllers[key], this.scope);
            }
           
        }

    };

   






    //View负责视图更新和事件点击
    var View = function () {
        this.events = {
            'click #planSearch': 'planSearchAction',
            'click #newsSearch': 'newsSearchAction',
            'click #flotSearch': 'flotSearchAction',
            'click #wordSearch': 'wordSearchAction',
            'click #targetSearch': 'targetSearchAction',
            'click .searchBtn': 'keyWordSplitAction',
            'keydown .searchKey': 'keyWordAction',
            'click .removeKeywords': 'removeKeywordsAction',
            'click .downLoadWord': 'downLoadAction',
            'click .clickAll': 'clickAllAction',
            'click .searchIcon': 'searchAction'
        };
        //初始化的视图控制器
        this.init = {
             'planShowController': 1
            ,'keyWordController': ''
        };
        this.el = $('#searchContainer');
        this.model = new Model({
            scope: this,
            controllers: {
                planShowController: this.planShowController,
                targetShowController: this.targetShowController,
                newsShowController: this.newsShowController,
                flotShowController: this.flotShowController,
                wordShowController: this.wordShowController,
                keyWordController: this.keyWordController,
                keyWordSplitController: this.keyWordSplitController
            }
        })

    };

    //
    View.prototype.templateHtml = function (data, target, wrap) {
        if (!data) {
            $(wrap).html('');
            return;
        }
        var tpl = document.getElementById(target).innerHTML;
        var tHtml = _.template(tpl);
        $(wrap).html(tHtml({ list: data }));
        if (wrap === '#planList') {
            this.planOperate(data);
            return
        }
        if (wrap === "#targetList") {
            this.targetOperate(data);
        }
    };
    
    View.prototype.planOperate = function (data) {
        $.each(data, function (n, item) {
            if (!item.isSubordinatePlan) {  //我的计划
                if (item.IsCollPlan != 1) {
                    if ((item.status == 0 || item.status == 15) && item.stop == 0) {
                        $('#planList .planListAll').eq(n).find('.submitTo,.transimitPlan').removeClass('planHide');
                    } else if (((item.status == 10 || item.status == 25) && item.stop == 0) || item.stop == 10) {
                        $('#planList .planListAll').eq(n).find('.transimitPlan').removeClass('planHide');
                        if (item.stop == 10) {
                            $('#planList .planListAll').eq(n).find('.revocation').removeClass('planHide').attr('operate',1);
                        } else {
                            if (item.status == 10) {
                                $('#planList .planListAll').eq(n).find('.revocation').removeClass('planHide').attr('operate', 0);
                            } else {
                                $('#planList .planListAll').eq(n).find('.revocation').removeClass('planHide').attr('operate', 20);
                            }                           
                        }
                    } else if ((item.status == 20 || item.status == 40) && item.stop == 0) {
                        $('#planList .planListAll').eq(n).find('.submit,.transimitPlan,.suspend,.modification,.decPlan').removeClass('planHide');

                    } else if (item.status == 30 && item.stop == 0) {
                        $('#planList .planListAll').eq(n).find('.transimitPlan,.revocation').removeClass('planHide');
                        $('#planList .planListAll').eq(n).find('.revocation').attr('operate',20)
                    } else if (item.stop == 90) {
                        $('#planList .planListAll').eq(n).find('.restart').removeClass('planHide');
                    } 
                }
            }

            if (item.isSubordinatePlan) {    //下属计划
                if ((item.status == 0 || item.status == 15) && item.stop == 0) {   
                    $('#planList .planListAll').eq(n).find('.transimitPlan').removeClass('planHide');
                } else if (((item.status == 10 || item.status == 25) && item.stop == 0) || item.stop == 10) {
                    $('#planList .planListAll').eq(n).find('.audit,.transimitPlan').removeClass('planHide');
                } else if ((item.status == 20 || item.status == 40) && item.stop == 0) {
                    $('#planList .planListAll').eq(n).find('.transimitPlan').removeClass('planHide');
                    if (item.initial != 0) {
                        $('#planList .planListAll').eq(n).find('.revocation').removeClass('planHide').attr('operate',10);
                    }
                } else if (item.status == 30 && item.stop == 0) {
                    $('#planList .planListAll').eq(n).find('.transimitPlan,.confirmPlan').removeClass('planHide');
                }
            }

        })

    }
    View.prototype.targetOperate = function (data) {
        $.each(data, function (n, v) {
            if (!v.isSubordinateObjective) {  //我的目标
                if (v.authorizedUser == null || loginId == v.authorizedUser) {                 //1、登陆用户为目标责任人且目标未授权给他人的场合： 3、登陆用户为目标被授权人的的场合：
                    if (v.status == 1) {                                            //待提交：授权、提交、删除、展开、详情。
                        if (v.authorizedUser == null) {
                            $('#targetList .wrapAll').eq(n).find('.liPower,.liCommit,.liDel').removeClass('planHide');
                        }
                        else {
                            $('#targetList .wrapAll').eq(n).find('.liCommit,.liDel').removeClass('planHide');
                        }
                    } else if (v.status == 2 || v.status == 4) {                    // 待审核：撤销、展开、详情。 1-4、待确认：撤销、展开、详情。
                        $('#targetList .wrapAll').eq(n).find('.liRevocation').removeClass('planHide');
                    } else if (v.status == 3 || v.status == 6) {                   //进行中（已超时）：修改、提交、展开、详情
                        $('#targetList .wrapAll').eq(n).find('.liModify,.liConfirm').removeClass('planHide');
                    } 
                } else {                                                              //a： 取消授权、展开、详情。
                    $('#targetList .wrapAll').eq(n).find('.liPowerCancel').removeClass('planHide');
                }
            }
            if (v.isSubordinateObjective) {   //下属目标
                if (v.status == 2) {            // 待审核：修改、审核、展开、详情。
                    $('#targetList .wrapAll').eq(n).find('.liCheck,.liModify').removeClass('planHide');
                } else if (v.status == 3 || v.status == 6) {                //进行中（已超时）：修改、分解、展开、详情。
                    $('#targetList .wrapAll').eq(n).find('.liModify').removeClass('planHide');
                } else if (v.status == 4) {        //待确认：确认、展开、详情。
                    $('#targetList .wrapAll').eq(n).find('.liSure').removeClass('planHide');
                }
            }
        })

    }

   
    View.prototype.circlePer = function (wrap) {

        $(wrap+" .knob").each(function () {
            var text = $(this).data("text");
            $(this).knob({
                width: 80,
                height: 80,
                min: 0,
                thickness: .3,
                bgColor: '#e0e0e0',
                inputColor: '#888',
                format: function (v) {
                    if (text) {
                        return text;
                    } else {
                        return v + "%";
                    }
                },
                release: function (v) {
                    if (text) {
                        return false;
                    }
                    if (parseInt(v) > 90 || parseInt(v) < 0) {
                        v = 90;
                    }
                    var planIdNew = this.$.attr("term");
                    $.ajax({
                        type: "post",
                        url: "/plan/UpdateProcess",
                        dataType: "json",
                        data: { planId: planIdNew, newProcess: v },
                        success: rsHandler(function (data) {
                            ncUnits.alert("已更新进度！");
                        })
                    });
                }
            });
        });

    }

    //视图控制器
    View.prototype.delWordController = function () {


    }


    View.prototype.planShowController = function (data) {
        var data = data, target = "planTarget", wrap = "#planList";
        this.templateHtml(data, target, wrap);
        //this.circlePer('.planListAll', data);
        this.circlePer('.planListAll')
        $('.wordAction').hide();

    };

    View.prototype.targetShowController = function (data) {
        var data = data, target = "actionTarget", wrap = "#targetList";
        this.templateHtml(data, target, wrap);
        //this.circlePer('.targetListAll', data)
        this.circlePer('.targetListAll')

        $('.wordAction').hide();

    };
    View.prototype.newsShowController = function (data) {
        var data = data, target = "newsTarget", wrap = "#newsList";
        this.templateHtml(data, target, wrap);
        $('.wordAction').hide();
    };
    View.prototype.flotShowController = function (data) {
        var data = data, target = "flotTarget", wrap = "#flotList";
        this.templateHtml(data, target, wrap);
        this.circlePer('.flotListAll', data);
        $('.wordAction').hide();

    };
    View.prototype.wordShowController = function (data) {
        $('.wordAction').show();
        var data = data, target = "wordTarget", wrap = "#wordList";
        this.templateHtml(data, target, wrap);
        

    };
    View.prototype.keyWordController = function (data) {
        if (data.length == 0) {
            this.keyWordSplitController('');
            return;
        }
        var data = data, target = "searchWord", wrap = "#searchTitle";
        this.templateHtml(data, target, wrap);
    }
    View.prototype.keyWordSplitController = function (data) {
        $('.searchBtn').remove();
        $('.searchKey').removeClass('hideKeywords').val(data).focus();
    }
    



    //点击事件获取数据
    View.prototype.planSearchAction = function () {
        this.model.updateView({ url: 1, controller: 'planShowController' });
    };
    View.prototype.targetSearchAction = function () {
        this.model.updateView({ url: 2, controller: 'targetShowController' });
    };
    View.prototype.newsSearchAction = function () {
        this.model.updateView({ url: 4, controller: 'newsShowController' });
    };
    View.prototype.flotSearchAction = function () {
        this.model.updateView({ url: 7, controller: 'flotShowController' });

    };
    View.prototype.wordSearchAction = function () {
        $('.clickAll').attr('checked', false);
        this.model.updateView({ url: 5, controller: 'wordShowController' });

    };
    View.prototype.keyWordSplitAction = function () {
        var words = [];
        $('.searchBtn').each(function (v) {
            words.push($(this).find('span').text());
        });
        this.model.keyWordSplit(words);
    }
   //搜索input框enter键搜索
    View.prototype.keyWordAction = function (e) {
        if (e.keyCode == 13) {
            var words = $('.searchKey').val();
            this.model.keyWord(words)
        }

    }
    View.prototype.removeKeywordsAction = function (e) {
        var el = $(e.currentTarget);
        e.stopPropagation();
        var index = el.attr('idx');
        this.model.keyWordRemove(index);

    }

    View.prototype.clickAllAction = function (e) {
        var el = $(e.currentTarget);
        if (el.attr('checked')) {
            $('.chooseDown').attr('checked', true);
            return;
        }
        $('.chooseDown').attr('checked', false);
       


    }

    View.prototype.searchAction = function () {
        if ($('.searchKey ').is(':hidden')) {
            this.model.toKeyword();
            return;
        }
         var words = $('.searchKey').val();
         this.model.keyWord(words);


    }
    View.prototype.downLoadAction = function(){
        var ids = [];
        $('.chooseDown').each(function (n) {
            if (this.checked) {
                var temp = {
                    id: parseInt($(this).attr('wid'), 10),
                    isCompany: $(this).attr('isCompany')
                }
                ids.push(temp)
            }
        });
        if (ids.length == 0) {
            ncUnits.alert('请选择文档');
            rerturn;
        }
        $.post("/Shared/MultiDocumentDownload", { data: JSON.stringify(ids),flag:0}, function (data) {
            if (data == "success") {
                //loadViewToMain("/DocumentManagement/MultiDownload?flag=1");
                window.location.href = "/Shared/MultiDocumentDownload?flag=1";
            }
            else {
                ncUnits.alert("下载失败!");
            }

        });
    }


    //事件绑定
    View.prototype.bindEvents = function () {
        var events = this.events;
        if (!events) {
            return;
        }
        //正则拆分事件字符串
        var delegateEventSplitter = /^(\S+)\s*(.*)$/;
        var k, method, eventName, selector, match;
        for (k in events) {
            method = events[k];
            match = k.match(delegateEventSplitter);
            eventName = match[1];
            selector = match[2];
            method = this[events[k]];
            //将view的this指向事件
            method = method.bind(this);
            this.el.off(eventName, selector)
            this.el.on(eventName, selector, method);
        }

    };
    
    View.prototype.updateList = function () {
        this.model.updateView();
    }

    View.prototype.show = function () {
        this.model.initData();
        this.bindEvents();

    }

    var view = new View();
     view.show();







//})()