var myGlobalInterval;
var manageLoop;
var mapLoop;
var globalIndex = -1;
var pauseGlobalLoop = false;

var getIndex;
function getGlobalIndex() {
    if (typeof getIndex == 'function') {
        return getIndex();
    } else {
        return -1;
    }
}

var manageLoop;

function manageGlobalLoop(flag, interTime) {
    if (isFunction(manageLoop)) {
        manageLoop(flag, interTime);
    } else {
        return -1;
    }
}

var mapLoop;

function runMapLoop(dontIncGlobalIndex) {
    if (typeof mapLoop == 'function') {
        mapLoop(dontIncGlobalIndex);
    } else {
        return -1;
    }
}

function stopLoop() {
    clearInterval(myGlobalInterval)
}
function startLoop(interval) {
    if (animationFlag = 1)
        manageGlobalLoop(true, interval || (1000 - ($("#speedSlider").slider('getValue') * 100)));
}



