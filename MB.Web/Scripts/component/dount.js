/**
 * Created by ZETA on 2015/4/12.
 * 双层色环形图
 * @param cx 圆心x坐标
 * @param cy 圆心y坐标
 * @param ir 内半径
 * @param mr 中半径
 * @param or 外半径
 * @param data 数据:[[value,color,id,text],[]...]
 * @param clickHandle click事件
 * @returns {*}
 */

Raphael.fn.dountChart = function (cx, cy, ir, mr, or, data,clickHandle) {
    var paper = this,
        rad = Math.PI / 180,
        chart = this.set();
    function loop(cx, cy, ir, mr, or, startAngle, endAngle, color) {
        paper.setStart();
        var c = Raphael.getRGB(color);
        if(Math.abs(startAngle - endAngle) == 360){
            paper.circle(cx,cy,or).attr({fill:color,"stroke-width": 0});
            paper.circle(cx,cy,mr).attr({fill:Raphael.rgb(c.r * 0.8,c.g * 0.8,c.b * 0.8),"stroke-width": 0});
            paper.circle(cx,cy,ir).attr({fill:"#fff","stroke-width": 0});
        }else{
            var x11 = cx + ir * Math.cos(-startAngle * rad),
                x12 = cx + ir * Math.cos(-endAngle * rad),
                y11 = cy + ir * Math.sin(-startAngle * rad),
                y12 = cy + ir * Math.sin(-endAngle * rad),

                x21 = cx + mr * Math.cos(-startAngle * rad),
                x22 = cx + mr * Math.cos(-endAngle * rad),
                y21 = cy + mr * Math.sin(-startAngle * rad),
                y22 = cy + mr * Math.sin(-endAngle * rad),

                x31 = cx + or * Math.cos(-startAngle * rad),
                x32 = cx + or * Math.cos(-endAngle * rad),
                y31 = cy + or * Math.sin(-startAngle * rad),
                y32 = cy + or * Math.sin(-endAngle * rad);

            paper.path(["M", x11, y11, "A", ir, ir, 0, +(endAngle - startAngle > 180), 0, x12, y12, "L", x32, y32, "A", or, or, 0, +(endAngle - startAngle > 180), 1, x31, y31, "z"]).attr({fill: color,"stroke-width": 0});
            paper.path(["M", x11, y11, "A", ir, ir, 0, +(endAngle - startAngle > 180), 0, x12, y12, "L", x22, y22, "A", mr, mr, 0, +(endAngle - startAngle > 180), 1, x21, y21, "z"]).attr({fill: Raphael.rgb(c.r * 0.8,c.g * 0.8,c.b * 0.8),"stroke-width": 0});

        }
        return  paper.setFinish();
    }
    var angle = 0,
        total = 0,
        start = 0,
        process = function (j) {
            var d = data[j]
                ,value = d[0],
                angleplus = 360 * value / total,
                popangle = angle + (angleplus / 2),
                //color = Raphael.hsb(start, .75, 1),
                ms = 500,
                delta = 30,
                //bcolor = Raphael.hsb(start, 1, 1),
                p = loop(cx, cy, ir, mr, or, angle, angle + angleplus, d[1]),
                txt = paper.text(cx + (mr + or) / 2 * Math.cos(-popangle * rad), cy + (mr + or) / 2 * Math.sin(-popangle * rad), d[0]).attr({fill: "#fff", stroke: "none", "font-size": 14});
            var se = paper.set();
            se.push(p,txt);
            se.hover(function () {
                p.stop().animate({transform: "s1.15 1.15 " + cx + " " + cy}, ms, "elastic");
                txt.stop().animate({transform: "s1.15 1.15 " + cx + " " + cy}, ms, "elastic");
                if(d[3]){
                    layer.tips(d[3] + "：" + d[0],$(txt.node),{
                        guide:0
                        ,isGuide:false
                        ,style:["color:#fff;background-color:#000;opacity:0.5",""]
                    });
                }
            },function () {
                p.stop().animate({transform: ""}, ms, "elastic");
                txt.stop().animate({transform: ""}, ms);
                layer.closeTips();
            });
            if(clickHandle && typeof clickHandle == "function"){
                se.click(function(){
                    clickHandle(d);
                })
            }

            angle += angleplus;
            chart.push(p);
            chart.push(txt);
            start += .1;
        };
    for (var i = 0, ii = data.length; i < ii; i++) {
        total += data[i][0];
    }
    if(ii == 0){
        paper.circle(cx,cy,(ir+or)/2).attr({stroke: "#a0a0a0", "stroke-width": or-ir})
    }else{
        for (i = 0; i < ii; i++) {
            process(i);
        }
    }
    //paper.text(cx,cy,'计划 ' + total + ' 项').attr({"font-family": "微软雅黑","font-size": 16,fill:"#686868"});
    return chart;
};


