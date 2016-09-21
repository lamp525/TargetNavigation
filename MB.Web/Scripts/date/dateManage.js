//@ sourceURL=dateManage.js
/**
 * Created by ZETA on 2015/10/12.
 */
$(function () {

    var $dateSetting = $("#dateSetting"),
        $thisYear = $("#thisYear"),
        $lastYear = $("#dateContainer .yearSelector.lastYear"),
        $nextYear = $("#dateContainer .yearSelector.nextYear"),
        date = new Date(),
        thisyear = date.getFullYear(),
        dragging = false,
        $allDate = $(),
        $selecteds = $(),
        $startDate, tipsIndex,
        save_holiday = [],
        save_workday = [],
        save_defday = [];

    $thisYear.html(thisyear + "年");
    $lastYear.html(thisyear - 1);
    $nextYear.html(thisyear + 1);
    $dateSetting.disableSelection();

    renderDateSetting(thisyear);

    $lastYear.click(function () {
        thisyear--;
        $thisYear.html(thisyear + "年");
        $lastYear.html(thisyear - 1);
        $nextYear.html(thisyear + 1);

        renderDateSetting(thisyear);
    });
    $nextYear.click(function () {
        thisyear++;
        $thisYear.html(thisyear + "年");
        $lastYear.html(thisyear - 1);
        $nextYear.html(thisyear + 1);

        renderDateSetting(thisyear);
    });

    $("#saveDateSetting").click(function () {
        $.ajax({
            type: "post",
            url: "/Holiday/AddNewHoliday",
            dataType: "json",
            data: {
                data: JSON.stringify({
                    holiday: save_holiday,
                    workday: save_workday,
                    defday: save_defday
                })
            },
            success: rsHandler(function (data) {
                ncUnits.alert("保存成功！");
            })
        });
    });

    $(document).mouseup(function(){
        init();
    });
    $dateSetting.delegate(".calendar tbody","mouseup",function(e){
        e.stopPropagation();
    });
    $dateSetting.delegate(".calendar tbody td:has(.date)",{
        "mousedown": mousedownHandler,
        "mouseup": mouseupHandler,
        "mouseenter": mouseenterHandler
    });

    function renderDateSetting(year){
        $dateSetting.empty();

        var tableRows = [],
            row = ""

        for(var month = 0; month < 12; month ++){
            if(month % 4 == 0){
                if(row.length){
                    row += "</div>";
                    tableRows.push(row);
                }
                row = "<div class='row'>";
            }

            var day = 1;
            date.setFullYear(year,month,day);
            var weekday = date.getDay();
            date.setFullYear(year,month + 1,0);
            var dayCount = date.getDate();
            var trs = [],
                tr = "";
            for(var i = 0; i < weekday; i ++){
                if(tr.length == 0){
                    tr += "<tr>";
                }
                tr += "<td></td>";
            }
            for(; day <= dayCount; day ++,weekday = (weekday + 1) % 7){
                if(weekday == 0){
                    if(tr.length){
                        tr += "</tr>";
                        trs.push(tr);
                    }
                    tr = "<tr>";
                }

                var dateStr = "<div class='date" + ((weekday == 0 || weekday == 6) ? " weekend'" : "'") + " value='" + (year + "-" + ((month + 1) >= 10 ? (month + 1) : ("0" + (month + 1))) + "-" + (day >= 10 ? day : ("0" + day))) + "'>" + day + "</div>";

                tr += "<td>" + dateStr + "</td>";
            }
            tr += "</tr>";
            trs.push(tr);
            row += "<div class='col-xs-3'><div class='calendarTitle'>" + (month + 1) + "月</div><div class='calendarBox'><table class='calendar'><thead><tr><td>日</td><td>一</td><td>二</td><td>三</td><td>四</td><td>五</td><td>六</td></tr></thead><tbody>" + trs.join("") + "</tbody></table></div></div>"
        }
        row += "</div>";
        tableRows.push(row);

        $dateSetting.append(tableRows.join(""));

        $allDate = $(".date",$dateSetting).parent("td");

        $(".row",$dateSetting).each(function(){
            var $this = $(this);
            var lineNum = 5;
            $this.find(".calendar tbody").each(function(){
                lineNum = _.max([$(this).find("tr").length,lineNum]);
            });
            $this.addClass("line" + lineNum);
        });

        $.ajax({
            type: "post",
            url: "/Holiday/GetHolidayByYear",
            dataType: "json",
            data: {
                data: thisyear
            },
            success: rsHandler(function (data) {
                $.each(data.holiday,function(i,v){
                    setHolidayClass($("[value='" + v + "']",$dateSetting));
                })
                $.each(data.workday,function(i,v){
                    setWorkdayClass($("[value='" + v + "']",$dateSetting));
                })
            })
        });
    }

    function mousedownHandler(){
        init();

        dragging = true;
        var $this = $(this);
        $startDate = $this;
        selectDate($this);
    }

    function mouseupHandler(){
        dragging = false;
        var $this = $(this);
        tipsIndex = $.layer({
            type: 4,
            tips: {
                msg: "<div id='holiday' class='dayType'>休</div><div id='workday' class='dayType'>班</div><div id='defday' class='dayType'>消</div>",
                follow: this,
                isGuide: false,
                style: ["background-color: #fff; border: 1px solid #fbac0f; padding: 5px;"]
            },
            shade: [0],
            closeBtn: false,
            success: function(l){
                $("#holiday",l).mouseup(isHoliday);
                $("#workday", l).mouseup(isWorkday);
                $("#defday", l).mouseup(isDefday);
            }
        });
    }

    function mouseenterHandler(){
        if(dragging){
            var $this = $(this);

            var startIndex = $allDate.index($startDate);
            var thisIndex = $allDate.index($this);
            var nowSelected = startIndex < thisIndex ? $allDate.slice(startIndex, thisIndex + 1) : $allDate.slice(thisIndex, startIndex + 1);

            var addSelected = nowSelected.not($selecteds);
            var deleteSelected = $selecteds.not(nowSelected);

            addSelected.each(function(){
                selectDate($(this));
            });
            deleteSelected.each(function(){
                unselectedDate($(this));
            });
        }
    }

    function selectDate($dateTD){
        $dateTD.addClass("selected");
        $selecteds = $selecteds.add($dateTD);
    }

    function unselectedDate($dateTD){
        $dateTD.removeClass("selected");
        $selecteds = $selecteds.not($dateTD);
    }

    function isHoliday(e){
        e.stopPropagation();
        $selecteds.children().each(function(){
            var $this = $(this),
                v = $this.attr("value");
            removeValue(save_workday, v);
            removeValue(save_defday, v);
            save_holiday.push(v);
            setHolidayClass($this);
        });
        init();
    }

    function isWorkday(e){
        e.stopPropagation();
        $selecteds.children().each(function(){
            var $this = $(this),
                v = $this.attr("value");
            removeValue(save_holiday, v);
            removeValue(save_defday, v);
            save_workday.push(v);
            setWorkdayClass($this);
        });
        init();
    }

    function isDefday(e){
        e.stopPropagation();
        $selecteds.children().each(function () {
            var $this = $(this),
                v = $this.attr("value");
            removeValue(save_workday, v);
            removeValue(save_holiday, v);
            save_defday.push(v);
            setDefdayClass($this);
        });
        init();
    }

    function setHolidayClass($date){
        $date.removeClass("workday").addClass("holiday");
    }
    function setWorkdayClass($date){
        $date.removeClass("holiday").addClass("workday");
    }
    function setDefdayClass($date) {
        $date.removeClass("holiday workday");
    }

    function init(){
        if(tipsIndex){
            layer.close(tipsIndex);
            tipsIndex = undefined;
        }
        unselectedDate($selecteds);
    }
});