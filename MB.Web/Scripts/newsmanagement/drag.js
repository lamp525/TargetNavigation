
function dropPrev(treeId, nodes, targetNode) {

    var pNode = targetNode.getParentNode();
    if (pNode && pNode.dropInner === false) {
        return false;
    } else {
        for (var i = 0, l = curDragNodes.length; i < l; i++) {
            var curPNode = curDragNodes[i].getParentNode();
            if (curPNode && curPNode !== targetNode.getParentNode() && curPNode.childOuter === false) {
                return false;
            }
        }
    }
    return true;
}
function dropInner(treeId, nodes, targetNode) {

    if (targetNode && targetNode.dropInner === false) {
        return false;
    } else {
        for (var i = 0, l = curDragNodes.length; i < l; i++) {
            if (!targetNode && curDragNodes[i].dropRoot === false) {
                return false;
            } else if (curDragNodes[i].parentTId && curDragNodes[i].getParentNode() !== targetNode && curDragNodes[i].getParentNode().childOuter === false) {
                return false;
            }
        }
    }
    return true;
}
function dropNext(treeId, nodes, targetNode) {

    var pNode = targetNode.getParentNode();
    if (pNode && pNode.dropInner === false) {
        return false;
    } else {
        for (var i = 0, l = curDragNodes.length; i < l; i++) {
            var curPNode = curDragNodes[i].getParentNode();
            if (curPNode && curPNode !== targetNode.getParentNode() && curPNode.childOuter === false) {
                return false;
            }
        }
    }
    return true;
}

var log, className = "dark", curDragNodes, autoExpandNode;
function beforeDrag(treeId, treeNodes) {

    className = (className === "dark" ? "" : "dark");
    for (var i = 0, l = treeNodes.length; i < l; i++) {
        if (treeNodes[i].drag === false) {
            curDragNodes = null;
            return false;
        } else if (treeNodes[i].parentTId && treeNodes[i].getParentNode().childDrag === false) {
            curDragNodes = null;
            return false;
        }
    }
    curDragNodes = treeNodes;
    return true;
}
function beforeDragOpen(treeId, treeNode) {

    autoExpandNode = treeNode;
    return true;
}

function onDrag(event, treeId, treeNodes) {

    className = (className === "dark" ? "" : "dark");

}
function onDrop(event, treeId, treeNodes, targetNode, moveType, isCopy) {

    className = (className === "dark" ? "" : "dark");
}
function onExpand(event, treeId, treeNode) {

    if (treeNode === autoExpandNode) {
        className = (className === "dark" ? "" : "dark");
    }
}



