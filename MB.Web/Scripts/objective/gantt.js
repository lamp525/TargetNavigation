/**
 * Created by DELL on 2015/9/8.
 */
var ge;
//this is the hugly but very friendly global var for the gantt editor
$(function() {

    //load templates
    $("#ganttemplates").loadTemplates();

    // here starts gantt initialization
    ge = new GanttMaster();
    var workSpace = $("#workSpace");
    workSpace.css({width:$(window).width() - 20,height:$(window).height() - 100});
    ge.init(workSpace);
    //inject some buttons (for this demo only)
    $(".ganttButtonBar div").append("<button onclick='clearGantt();' class='button'>clear</button>")
        .append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
        .append("<button onclick='nextYear();' class='button'>next</button>");
    $(".ganttButtonBar h1").html("<a href='http://twproject.com' title='Twproject the friendly project and work management tool' target='_blank'><img width='80%' src='res/twBanner.jpg'></a>");
    $(".ganttButtonBar div").addClass('buttons');
    //overwrite with localized ones
    loadI18n();


    //fill default Teamwork roles if any
    if (!ge.roles || ge.roles.length == 0) {
        setRoles();
    }

    //fill default Resources roles if any
    if (!ge.resources || ge.resources.length == 0) {
        setResource();
    }

    var defaultOpt = {
        view:'m',
        year:new Date().getFullYear(),
        month:new Date().getMonth()+1
    };


    $('#gantList').css('display', 'none');
    
    $('#gantListTab').on('click', function () {
        loadFromLocalStorage({ year: defaultOpt.year, month: defaultOpt.month }, defaultOpt);
        $('#punckListTab').removeClass("TabActiveColor");
        $('#gantList').css('display', 'block');
        $('.listObj').addClass('hidden');
        ge.reset();
        
    })
    $('#punckListTab').on('click', function () {
        $('.listObj').removeClass('hidden');
    })

    $('#punckListTab').on('click', function () {
        //$(this).addClass("TabActiveColor");
        $('#gantList').css('display', 'none');
    })

    //simulate a data load from a server.
    

    $('.viewUp').on('click',function(){
        if(defaultOpt.month == 12 && defaultOpt.view == 'm'){
            return;
        }
        if(defaultOpt.view == 'q'){
            defaultOpt.year++;
            loadFromLocalStorage({ year: defaultOpt.year }, defaultOpt)
        }else{
            defaultOpt.month++;
            loadFromLocalStorage({ year: defaultOpt.year, month: defaultOpt.month }, defaultOpt)
            monthClick(defaultOpt);
        }
 
    });
    $('.viewDown').on('click',function(){
        if(defaultOpt.month == 1  && defaultOpt.view == 'm'){
            return;
        }
        if(defaultOpt.view == 'q'){

            defaultOpt.year--;
            loadFromLocalStorage({ year: defaultOpt.year }, defaultOpt)
        }else{
            defaultOpt.month--;
            loadFromLocalStorage({ year: defaultOpt.year, month: defaultOpt.month }, defaultOpt)
            monthClick(defaultOpt);

        }
    })

    $('.viewDown').on('selectstart', function () {
        return false;
    })
    $('.viewUp').on('selectstart', function () {
        return false;
    })
    $('.setYear').on('click',function(){
        defaultOpt.view = 'q';
        loadFromLocalStorage({ year: defaultOpt.year }, defaultOpt);
        $('.viewDown').removeClass('grayColor');
        $('.viewUp').removeClass('grayColor')
        $('#workSpace').trigger('zoomYear.gantt');
    });
    $('.setMonth').on('click',function(){
        defaultOpt.view = 'm';
        monthClick(defaultOpt);
        loadFromLocalStorage({ year: defaultOpt.year, month: defaultOpt.month }, defaultOpt)
        $('#workSpace').trigger('zoomMonth.gantt');
    });
    monthClick(defaultOpt);

    /*/debug time scale
     $(".splitBox2").mousemove(function(e){
     var x=e.clientX-$(this).offset().left;
     var mill=Math.round(x/(ge.gantt.fx) + ge.gantt.startMillis)
     $("#ndo").html(x+" "+new Date(mill))
     });*/

    //$(window).resize(function(){
    //    workSpace.css({width:$(window).width() - 1,height:$(window).height() - workSpace.position().top});
    //    workSpace.trigger("resize.gantt");
    //}).oneTime(150,"resize",function(){$(this).trigger("resize")});

});

function monthClick(opt){
    if(opt.month == 12 && opt.view == 'm'){
        $('.viewUp').addClass('grayColor');
    }else if(opt.month == 1 && opt.view == 'm'){
       $('.viewDown').addClass('grayColor')
    }else{
        $('.viewDown').removeClass('grayColor');
        $('.viewUp').removeClass('grayColor')
    }
}




function loadGanttFromServer(time) {

    loadFromLocalStorage(time);
   

}


function saveGanttOnServer() {
    if(!ge.canWrite)
        return;


    //this is a simulation: save data to the local storage or to the textarea
    saveInLocalStorage();

}


//-------------------------------------------  Create some demo data ------------------------------------------------------
function setRoles() {
    ge.roles = [
        {
            id:"tmp_1",
            name:"Project Manager"
        },
        {
            id:"tmp_2",
            name:"Worker"
        },
        {
            id:"tmp_3",
            name:"Stakeholder/Customer"
        }
    ];
}

function setResource() {
    var res = [];
    for (var i = 1; i <= 10; i++) {
        res.push({id:"tmp_" + i,name:"Resource " + i});
    }
    ge.resources = res;
}


function editResources(){

}

function clearGantt() {
    ge.reset();
}

function loadI18n() {
    GanttMaster.messages = {
        "CANNOT_WRITE":                  "CANNOT_WRITE",
        "CHANGE_OUT_OF_SCOPE":"NO_RIGHTS_FOR_UPDATE_PARENTS_OUT_OF_EDITOR_SCOPE",
        "START_IS_MILESTONE": "已小于父目标起始日期",
        'MOVE_IS_MILESTONE': "请在黄色区间拖拽",
        "END_IS_MILESTONE": "END_IS_MILESTONE",
        "CHILD_LESS_MILESTONE": "收缩时请不要小于紫色虚线",
        "END_LESS_MILESTONE": "小于子目标最大结束日期",
        "END_MORE_MILESTONE": "大于父目标最大结束日期",
        "END_START_MILESTONE": "结束日期不能小于起始日期",
        "TASK_HAS_CONSTRAINTS":"TASK_HAS_CONSTRAINTS",
        "GANTT_ERROR_DEPENDS_ON_OPEN_TASK":"GANTT_ERROR_DEPENDS_ON_OPEN_TASK",
        "GANTT_ERROR_DESCENDANT_OF_CLOSED_TASK":"GANTT_ERROR_DESCENDANT_OF_CLOSED_TASK",
        "TASK_HAS_EXTERNAL_DEPS":"TASK_HAS_EXTERNAL_DEPS",
        "GANTT_ERROR_LOADING_DATA_TASK_REMOVED":"GANTT_ERROR_LOADING_DATA_TASK_REMOVED",
        "ERROR_SETTING_DATES":"ERROR_SETTING_DATES",
        "CIRCULAR_REFERENCE":"CIRCULAR_REFERENCE",
        "CANNOT_DEPENDS_ON_ANCESTORS":"CANNOT_DEPENDS_ON_ANCESTORS",
        "CANNOT_DEPENDS_ON_DESCENDANTS":"CANNOT_DEPENDS_ON_DESCENDANTS",
        "INVALID_DATE_FORMAT":"INVALID_DATE_FORMAT",
        "TASK_MOVE_INCONSISTENT_LEVEL":"TASK_MOVE_INCONSISTENT_LEVEL",

        "GANTT_QUARTER_SHORT":"trim.",
        "GANTT_SEMESTER_SHORT":"sem."
    };
}



//-------------------------------------------  Get project file as JSON (used for migrate project from gantt to Teamwork) ------------------------------------------------------
function getFile() {
    $("#gimBaPrj").val(JSON.stringify(ge.saveProject()));
    $("#gimmeBack").submit();
    $("#gimBaPrj").val("");

    /*  var uriContent = "data:text/html;charset=utf-8," + encodeURIComponent(JSON.stringify(prj));
     neww=window.open(uriContent,"dl");*/
}


//-------------------------------------------  LOCAL STORAGE MANAGEMENT (for this demo only) ------------------------------------------------------
Storage.prototype.setObject = function(key, value) {
    this.setItem(key, JSON.stringify(value));
};


Storage.prototype.getObject = function(key) {
    return this.getItem(key) && JSON.parse(this.getItem(key));
};


function loadFromLocalStorage(time, defaultOpt) {
    
    //ret = num==1?JSON.parse($("#tal").val()):JSON.parse($("#ta").val());
    var dt = defaultOpt;
    $.ajax({
        type: "post",
        url: "/ObjectiveIndex/GetGanttChartObjectiveList",
        dataType: "json",
        data: time,
        success: rsHandler(function (data) {
          
            //if (data == "ok") {
               
                dt.month < 10 ? '0' + dt.month : dt.month;
                var date = dt.year + '-' + dt.month + '-' + '01' + ' 00:00:00';
                date = date.substring(0,19);    
                date = date.replace(/-/g,'/'); 
                var timestamp = new Date(date).getTime();
                $('#workSpace').attr('tamp',timestamp);
                var ret = { "selectedRow": 0, "canWrite": true, "canWriteOnParent": true,};
                ret.tasks = data.length>0?data:[];
                $.each(ret.tasks,function(n,val){
                    val.level = 0;
                    val.end = parseInt(val.end, 10)+86400000;
                    val.start = parseInt(val.start, 10);
                    val.status = { 1: 'STATUS_FAILED', 2: 'STATUS_SUSPENDED', 3: 'STATUS_CHECKED', 4: 'STATUS_DONE', 5: 'STATUS_ACTIVE' }[val.status] || 'STATUS_STATUS_UNDEFINED',
                    val.progress = val.process;
                    val.code = n + 1;
                    val.startIsMilestone = false;
                    val.endIsMilestone = false;
                    val.canWrite = true;
                    val.collapsed = false;
                    
                })
               
   
                //ret.tasks.splice(5, 1);
                ge.loadProject(ret);
                ge.checkpoint();
                $('.taskEditRow').click(function () {
                    //var masster = new GanttMaster();
                    //var task = new Task();
                    //var taskMsg = master.getTask($(this).attr("taskId"));
                    if ($('.ganttLimitSVG').attr('tid') != $(this).attr('taskid')) {
                        console.log('触发taskEditRow')
                        $('.ganttLimitSVG').remove();
                        
                        //var x = Math.round((parseInt(taskMsg.childEnd, 10) + 86400000 - self.startMillis) * self.fx);
                        //self.svg.line(gridGroup, x, 0, x, "100%", { class: "ganttTodaySVG ganttLimitSVG", tid: task.id });
     
                    }

                })
            //} 
        })
    });

}


function saveInLocalStorage() {
    var prj = ge.saveProject();
    if (localStorage) {
        localStorage.setObject("teamworkGantDemo", prj);
    } else {
        $("#ta").val(JSON.stringify(prj));
    }
    console.log('prj',prj)
}

$(document).on('mousedown','.taskLinkStartSVG',function(){
    var prj = ge.saveProject();


})



