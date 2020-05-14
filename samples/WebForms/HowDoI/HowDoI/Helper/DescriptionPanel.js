function initDragPan(){
    document.getElementById('DragBar1').onmousedown = handleMouseDown;
    document.getElementById('DragBar2').onmousedown = handleMouseDown;
}

function showDescription(flag){
    if(flag){
        document.getElementById('panelSmall').style.display = 'none';
        document.getElementById('panelBig').style.display = 'block';
        document.getElementById('hidebt').style.display = 'block';
        document.getElementById('showbt').style.display = 'none';        
    }else{
        document.getElementById('panelSmall').style.display = 'block';
        document.getElementById('panelBig').style.display = 'none';
        document.getElementById('hidebt').style.display = 'none';
        document.getElementById('showbt').style.display = 'block'; 
    }
}

var iDiffx = 0;
var iDiffy = 0;
function handleMouseDown(e)
{
    var oEvent = EventUtil.getEvent(e);
    var oDiv = document.getElementById("descriptionPanel");
    
    if(oEvent){
        iDiffx = oEvent.clientX - oDiv.offsetLeft;
        iDiffy = oEvent.clientY - oDiv.offsetTop;
    }
    
    EventUtil.addEventHandler(document.body,"mousemove",handleMouseMove);
    EventUtil.addEventHandler(document.body,"mouseup",handleMouseUp);
    document.getElementById("descriptionPanel").style.cursor = "move";
}
function handleMouseMove(e)
{
    var oEvent = EventUtil.getEvent(e);
    var oDiv = document.getElementById("descriptionPanel");
    oDiv.style.left = parseInt(oEvent.clientX - iDiffx) + 'px';
    oDiv.style.top = parseInt(oEvent.clientY - iDiffy) + 'px';
}
function handleMouseUp(e)
{
    EventUtil.removeEventHandler(document.body,"mousemove",handleMouseMove);
    EventUtil.removeEventHandler(document.body,"mouseup",handleMouseUp);
    document.getElementById("descriptionPanel").style.cursor = "default";
}


var EventUtil = new Object;   
  
EventUtil.addEventHandler = function (oTarget, sEventType, fnHandler){   
    if(oTarget.addEventListener) {//firefox   
        oTarget.addEventListener(sEventType, fnHandler, false);   
    } else if(oTarget.attachEvent) {//IE   
        oTarget.attachEvent("on"+sEventType, fnHandler);   
    } else {//others   
        oTarget["on" + sEventType] = fnHandler;   
    }   
};   
EventUtil.removeEventHandler = function (oTarget, sEventType, fnHandler){   
    if(oTarget.removeEventListener) {//firefox   
        oTarget.removeEventListener(sEventType, fnHandler, false);   
    } else if(oTarget.detachEvent) {//IE   
        oTarget.detachEvent("on"+sEventType, fnHandler);   
    } else {//others   
        oTarget["on" + sEventType] = null;   
    }   
};   
  
EventUtil.formatEvent = function(oEvent){   
    oEvent.charCode = (oEvent.type == "keypress") ? oEvent.keycode : 0;   
    oEvent.eventPhase = 2;   
    oEvent.isChar = (oEvent.charCode > 0);   
    oEvent.pageX = oEvent.clientX + document.body.scrollLeft;   
    oEvent.pageY = oEvent.clientY + document.body.scrollTop;   
       
    oEvent.preventDefault = function(){   
        this.returnValue = false;   
    };   
       
    if(oEvent.type == "mouseout"){   
        oEvent.relatedTarget = oEvent.toElement;   
    } else if (oEvent.Type == "mouseover") {   
        oEvent.relatedTarget = oEvent.fromElement;   
    }   
       
    oEvent.stopPropagation = function(){   
        this.cancelBubble = true;   
    };   
       
    oEvent.target = oEvent.srcElement;   
    oEvent.time = (new Date()).getTime();   
    return oEvent;   
};   
  
EventUtil.getEvent = function (){   
    if(window.event){   
        return this.formatEvent(window.event);   
    } else {   
        return EventUtil.getEvent.caller.arguments[0];   
    }   
};  