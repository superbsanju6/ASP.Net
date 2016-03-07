var lastShownButton;
var currentRotatorIndex = 0;

function showItemByIndex(index, rotatorClientID) {
    // gets reference to the rotator object
    var oRotator = $find(rotatorClientID);

    // Sets currently shown item by its index
    oRotator.set_currentItemIndex(index);
        
    //if (lastShownButton)
      //  lastShownButton.removeClass("btnHighlight");        

    var currentButton = getButtonByIndex(index);
    //currentButton.addClass("btnHighlight");    
    lastShownButton = currentButton;
}

function OnClientItemShown(oRotator, args) {
    var currentIndex = args.get_item().get_index();
    currentRotatorIndex = currentIndex;

    /*
    if (lastShownButton)
        lastShownButton.removeClass("btnHighlight");

    var currentButton = getButtonByIndex(currentIndex);
    currentButton.addClass("btnHighlight");
    lastShownButton = currentButton;
    */
}

function getButtonByIndex(index) {
    var buttonIdSelector = String.format("#Button{0}:first", index);
    var currentButton = $telerik.$(buttonIdSelector)

    return currentButton;
}

// TOUCH-EVENTS SINGLE-FINGER SWIPE-SENSING JAVASCRIPT
// Courtesy of PADILICIOUS.COM and MACOSXAUTOMATION.COM
			
// this script can be used with one or more page elements to perform actions based on them being swiped with a single finger
		
var triggerElementID = null; // this variable is used to identity the triggering element
var fingerCount = 0;
var startX = 0;
var startY = 0;
var curX = 0;
var curY = 0;
var deltaX = 0;
var deltaY = 0;
var horzDiff = 0;
var vertDiff = 0;
var minLength = 72; // the shortest distance the user may swipe
var swipeLength = 0;
var swipeAngle = null;
var swipeDirection = null;
var touchStartEvent = null;
var rotatorID = null;	

// The 4 Touch Event Handlers
			
// NOTE: the touchStart handler should also receive the ID of the triggering element
// make sure its ID is passed in the event call placed in the element declaration, like:
// <div id="picture-frame" ontouchstart="touchStart(event,'picture-frame');"  ontouchend="touchEnd(event);" ontouchmove="touchMove(event);" ontouchcancel="touchCancel(event);">

function touchStart(event, passedName, rID) {
    //touchStartEvent = event;
	// disable the standard ability to select the touched object
	//event.preventDefault();
	// get the total number of fingers touching the screen
	fingerCount = event.touches.length;
	// since we're looking for a swipe (single finger) and not a gesture (multiple fingers),
	// check that only one finger was used
	if ( fingerCount == 1 ) {
		// get the coordinates of the touch
		startX = event.touches[0].pageX;
		startY = event.touches[0].pageY;
		// store the triggering element ID
		triggerElementID = passedName;
		rotatorID = rID;
	} else {
		// more than one finger touched so cancel
		touchCancel(event);
	}
}
		
function touchMove(event) {
	event.preventDefault();
	if ( event.touches.length == 1 ) {
		curX = event.touches[0].pageX;
		curY = event.touches[0].pageY;
	} else {
		touchCancel(event);
	}
}
			
function touchEnd(event) {
	event.preventDefault();
	// check to see if more than one finger was used and that there is an ending coordinate
	if ( fingerCount == 1 && curX != 0 ) {
		// use the Distance Formula to determine the length of the swipe
		swipeLength = Math.round(Math.sqrt(Math.pow(curX - startX,2) + Math.pow(curY - startY,2)));
		// if the user swiped more than the minimum length, perform the appropriate action
		if (swipeLength >= minLength) {
			caluculateAngle();
			determineSwipeDirection();
			processingRoutine();
			touchCancel(event); // reset the variables
        }
        else {            
			touchCancel(event);
		}
    }
    else {        
        touchCancel(event);
    }
}
		
function touchCancel(event) {
    // reset the variables back to default values  
    //if(touchStartEvent != null)  
        //$find(triggerElementID).trigger(touchStartEvent);

	fingerCount = 0;
	startX = 0;
	startY = 0;
	curX = 0;
	curY = 0;
	deltaX = 0;
	deltaY = 0;
	horzDiff = 0;
	vertDiff = 0;
	swipeLength = 0;
	swipeAngle = null;
	swipeDirection = null;
	triggerElementID = null;
	rotatorID = null;
	//touchStartEvent = null;
}
			
function caluculateAngle() {
	var X = startX-curX;
	var Y = curY-startY;
	var Z = Math.round(Math.sqrt(Math.pow(X,2)+Math.pow(Y,2))); //the distance - rounded - in pixels
	var r = Math.atan2(Y,X); //angle in radians (Cartesian system)
	swipeAngle = Math.round(r*180/Math.PI); //angle in degrees
	if ( swipeAngle < 0 ) { swipeAngle =  360 - Math.abs(swipeAngle); }
}
			
function determineSwipeDirection() {
	if ( (swipeAngle <= 45) && (swipeAngle >= 0) ) {
		swipeDirection = 'left';
	} else if ( (swipeAngle <= 360) && (swipeAngle >= 315) ) {
		swipeDirection = 'left';
	} else if ( (swipeAngle >= 135) && (swipeAngle <= 225) ) {
		swipeDirection = 'right';
	} else if ( (swipeAngle > 45) && (swipeAngle < 135) ) {
		swipeDirection = 'down';
	} else {
		swipeDirection = 'up';
	}
}
			
function processingRoutine() {
    var swipedElement = document.getElementById(triggerElementID);    
        
    if (swipeDirection == 'left') {
        showNextItem($find(rotatorID), Telerik.Web.UI.RotatorScrollDirection.Left);
    } else if (swipeDirection == 'right') {
        showNextItem($find(rotatorID), Telerik.Web.UI.RotatorScrollDirection.Right);
	} else if ( swipeDirection == 'up' ) {
		
	} else if ( swipeDirection == 'down' ) {

    }

    return false;
}

function showNextItem(rotator, direction) {
    //alert("rotator = " + rotator + " direction = " + direction);
    rotator.showNext(direction);
}