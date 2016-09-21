/**
 * Created by ZETA on 2015/8/12.
 */
$(function(){
    $.fn.extend({
        flowChart: function(data){
            this.empty();
            var that = this,
                width = this.width(),
                height = this.height(),
                paper = Raphael(this.get(0),width,height),
                lineList = [];


            var originX = 200,
                originY = 50,
                perIntervalY = 100,
                r = 20,
                lineWidth = 10,
                spaceEnd = 5,
                spaceStart = spaceEnd + lineWidth*Math.sqrt(3)/2,
                triangleSide = 25,
                overlapLength = (triangleSide - lineWidth)*Math.sqrt(3)/2,
                triangleHeight = triangleSide*Math.sqrt(3)/ 2,
                curveBaseWidth = 20,
                curveRadiu = 15,
                pathwaySpace = 20,
                map = {},
                circleList = [],
                nodeLength = data.nodeInfo.length;
            if(nodeLength > 4){
                var newH = (2*r + perIntervalY)*(nodeLength - 1) + 2*originY;
                that.height(newH);
                paper.setSize(width,newH);
            }
            $.each(data.nodeInfo,function(i,node){
                var x = originX,
                    y = (perIntervalY + 2 * r) * i + originY,
                    color = node.testRatio == 100 ? "#58b456" : "#fbac0f",
                    c = Raphael.getRGB(color);
                var circle = paper.circle(x,y,r).attr({fill: color, "stroke-width": 0}).data("coord",[x,y]).hover(function(){
                    this.glowSet = this.glow({color:Raphael.rgb(c.r * 0.6,c.g * 0.6,c.b * 0.6)});
                    layer.tips(node.operate,$(this.node),{
                        guide:0
                        ,style:["color:#fff;background-color:#000;opacity:0.5",""]
                    });
                },function(){
                    this.glowSet.remove();
                    layer.closeTips();
                });
                circle.pathway = [];
                paper.text(x - r - spaceStart,y,node.nodeName).attr({"font-family": "微软雅黑,Helvetica Neue,Helvetica,Arial,sans-serif","font-size": "12px","fill": "#686868","text-anchor": "end"})
                map[node.nodeId] = i;
                circleList[i] = circle;
            });
            $.each(data.linkInfo,function(i,link){
                var entryIndex = map[link.nodeEntryId],
                    exitIndex = map[link.nodeExitId],
                    entry = circleList[entryIndex],
                    exit = circleList[exitIndex],
                    originEntry = entry.data("coord"),
                    originExit = exit.data("coord"),
                    difference = exitIndex - entryIndex,
                    isunder = difference > 0,
                    lineStr = "",
                    color = link.testFlag ? (link.status ? "#58b456" : "#ADD2A4") : (link.status ? "#fbac0f" : "#e3e3e3"),
                    c = Raphael.getRGB(color);

                if(Math.abs(difference) == 1){
                    if(isunder){
                        lineStr = "M" + (originEntry[0] - lineWidth/2) + " " + (originEntry[1] + r + spaceStart) + "h" + lineWidth + "v" + (originExit[1] - originEntry[1] - 2*r - spaceEnd - spaceStart - triangleHeight)
                        + "h" + (triangleSide - lineWidth)/2 + "l-" + triangleSide/2 + " " + triangleHeight + "l-" + triangleSide/2 + " -" + triangleHeight + "h" + (triangleSide - lineWidth)/2 + "z";
                    }else{
                        lineStr = "M" + (originEntry[0] - lineWidth/2) + " " + (originEntry[1] - r - spaceStart) + "h" + lineWidth + "v-" + (originEntry[1] - originExit[1] - 2*r - spaceEnd - spaceStart - triangleHeight)
                        + "h" + (triangleSide - lineWidth)/2 + "l-" + triangleSide/2 + " -" + triangleHeight + "l-" + triangleSide/2 + " " + triangleHeight + "h" + (triangleSide - lineWidth)/2 + "z";
                    }
                }else if(Math.abs(difference) >= 2){
                    var intervalY = Math.abs(originExit[1] - originEntry[1]),
                        passCircle = circleList.slice(Math.min(entryIndex,exitIndex),Math.max(entryIndex,exitIndex) + 1),
                        pathwayIndex = getPathwayIndex(passCircle);
                    if(isunder){
                        lineStr = "M" + (originEntry[0] + r + spaceStart) + " " + (originEntry[1] + lineWidth/2) + "v-" + lineWidth + "h" + (curveBaseWidth + pathwayIndex*pathwaySpace) + "a" + curveRadiu + " " + curveRadiu + " 0 0 1 " + curveRadiu + " " + curveRadiu
                        + "v" + (intervalY + lineWidth - 2*curveRadiu) + "a" + curveRadiu + " " + curveRadiu + " 0 0 1 -" + curveRadiu + " " + curveRadiu + "h-" + (curveBaseWidth + pathwayIndex*pathwaySpace -overlapLength) + "v" + (triangleSide - lineWidth)/2
                        + "l-" + triangleHeight + " -" + triangleSide/2 + "l" + triangleHeight + " -" + triangleSide/2 + "v" + (triangleSide - lineWidth)/2 + "h" + (curveBaseWidth + pathwayIndex*pathwaySpace -overlapLength)
                        + "a" + (curveRadiu - lineWidth) + " " + (curveRadiu - lineWidth) + " 0 0 0 " + (curveRadiu - lineWidth) + " -" + (curveRadiu - lineWidth) + "v-" + (intervalY + lineWidth - 2*curveRadiu)
                        + "a" + (curveRadiu - lineWidth) + " " + (curveRadiu - lineWidth) + " 0 0 0 -" + (curveRadiu - lineWidth) + " -" + (curveRadiu - lineWidth) + "z";
                    }else{
                        lineStr = "M" + (originEntry[0] + r + spaceStart) + " " + (originEntry[1] - lineWidth/2) + "v" + lineWidth + "h" + (curveBaseWidth + pathwayIndex*pathwaySpace) + "a" + curveRadiu + " " + curveRadiu + " 0 0 0 " + curveRadiu + " -" + curveRadiu
                        + "v-" + (intervalY + lineWidth - 2*curveRadiu) + "a" + curveRadiu + " " + curveRadiu + " 0 0 0 -" + curveRadiu + " -" + curveRadiu + "h-" + (curveBaseWidth + pathwayIndex*pathwaySpace -overlapLength) + "v-" + (triangleSide - lineWidth)/2
                        + "l-" + triangleHeight + " " + triangleSide/2 + "l" + triangleHeight + " " + triangleSide/2 + "v-" + (triangleSide - lineWidth)/2 + "h" + (curveBaseWidth + pathwayIndex*pathwaySpace -overlapLength)
                        + "a" + (curveRadiu - lineWidth) + " " + (curveRadiu - lineWidth) + " 0 0 1 " + (curveRadiu - lineWidth) + " " + (curveRadiu - lineWidth) + "v" + (intervalY + lineWidth - 2*curveRadiu)
                        + "a" + (curveRadiu - lineWidth) + " " + (curveRadiu - lineWidth) + " 0 0 1 -" + (curveRadiu - lineWidth) + " " + (curveRadiu - lineWidth) + "z";
                    }
                    setPathway(passCircle,pathwayIndex);
                }
                lineList.push(paper.path(lineStr).attr({fill: color,'stroke-width': 0}).hover(function(e){
                    var _that = this;
                    this.glowSet = this.glow({color:Raphael.rgb(c.r * 0.6,c.g * 0.6,c.b * 0.6)});
                    var elePosition = $(e.target).offset(),
                        parentPosition = $(e.target).parent().offset();
                    layer.tips(link.condition,this.node,{
                        style:["left:" + (e.offsetX - elePosition.left + parentPosition.left + 20) + "px;top:" + (e.offsetY - elePosition.top + parentPosition.top) + "px;color:#fff;background-color:#000;opacity:0.5",""]
                    });
                    $.each(lineList,function(i,ele){
                        if(ele != _that){
                            ele.toBack();
                        }
                    })
                },function(){
                    this.glowSet.remove();
                    layer.closeTips();
                }));
            });

            /* function 开始 */
            function getPathwayIndex(list){
                var index = 0;
                while(true){
                    var used = false;
                    $.each(list,function(i,ele){
                        used = used || ele.pathway[index];
                    });
                    if(used){
                        index ++;
                    }else{
                        return index;
                    }
                }
            }

            function setPathway(list,index){
                var pass = list.slice(1,list.length - 1);
                $.each(pass,function(i,ele){
                    ele.pathway[index] = true;
                })
            }
            /* function 结束 */
        }
    });
});