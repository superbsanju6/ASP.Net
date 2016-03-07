var EMPTY_STRING = "";

function applyMouseOverrides()
{
	document.oncontextmenu = function () { return false; }
	document.onmousedown = mouseDown;
}


function mouseDown(e) {
    try { if (event.button == 2) { return false; } }
    catch (e) { if (e.which == 3) { return false; } }
}

// Test for Browser Full Screen Mode (F11)
function isFullScreenMode() {
    var b = false;
    if (screen.height === window.outerHeight) {
        b = true;
    }
    return b;
}

function enterFullScreenIE() {
	//Only attempt Full Screen if IE Browser
	if (BrowserDetect.browser == "Explorer") {
		enterFullScreen();
	}
}

function exitFullScreenIE() {
	//Only attempt Full Screen if IE Browser
	if (BrowserDetect.browser == "Explorer") {
		exitFullScreen();
	}
}

function enterFullScreen() {
	if (!isFullScreenMode()) {
		tgToggleFullScreenF11(); //Go Full Screen!
	}
}

function exitFullScreen() {
	if (isFullScreenMode()) {
		tgToggleFullScreenF11(); //Leave Full Screen!
	}
}

// Invoke the Browser F11 Full Screen Command
function tgToggleFullScreenF11() {
	var el = document.documentElement
                , rfs = // for newer Webkit and Firefox
                el.requestFullScreen
                || el.webkitRequestFullScreen
                || el.mozRequestFullScreen
                || el.msRequestFullScreen;
	if (typeof rfs != "undefined" && rfs) {

		setTimeout(function () {
			rfs.call(el);
		}, 0);


	} else if (typeof window.ActiveXObject != "undefined") {
		// for Internet Explorer
		var wscript = new ActiveXObject("WScript.Shell");
		if (wscript != null) {
			wscript.SendKeys("{F11}");
		}
	}
}



/* clears a given div id by:
 *    1.  removing any child nodes
 *    2.  setting the div innerHtml to and empty string
 *
 * @param {String} divId - the id of the div to clear
 */
function tgClearDiv(divId) {
    try {
        if (divExists(divId)) {
            tgRemoveChildren(divId);
            tgSetInnerHtml(divId, "");
        }
    } catch (ex) {
        alert("tgClearDiv Exception: '" + divId + "':  " + ex);
    }
}

/* get the contents of the innerHTML of a given element
 *
 * @param {String} elid		- the id of the element
 * @return {String} currContents	- the current contents
 */
function tgGetInnerHtmlIE8(elid) {
    var ih = EMPTY_STRING;
    if (divExists(elid)) {
        var elInstance = getElementInstance(elid);
        if (objExists(elInstance)) {
            ih = elInstance.innerHTML;
        }
    }
    return ih;
}

/* get the contents of the innerHTML of a given div
 *
 * @param {String} divId		- the id of the div
 * @return {String} currContents	- the current contents
 */
function tgGetInnerHtmlIE9(divId) {
    var ih = EMPTY_STRING;
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        ih = theDiv.innerHTML;
        theDiv = null;
    }
    return ih;
}

function tgGetInnerHtml(divId) {
    if (BrowserDetect.browser == "Explorer" && BrowserDetect.version == "8") {
        tgGetInnerHtmlIE8(divId);
    } else {
        tgGetInnerHtmlIE9(divId);
    }
}




/* set the contents of the innerHTML of a given div
 *
 * @param {String} elId		- the id of the div
 * @param {String} newContents	- the new contents
 */
function tgSetInnerHtmlIE8(elid, newContents) {
    if (divExists(elid)) {
        var elInstance = getElementInstance(elid);
        if (objExists(elInstance)) {
            elInstance.innerHTML = (newContents + "");
        } 
    }
}

/* set the contents of the innerHTML of a given div
 *
 * @param {String} divId		- the id of the div
 * @param {String} newContents	- the new contents
 */
function tgSetInnerHtmlIE9(divId, newContents) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv.innerHTML = newContents;
        theDiv = null;
    }
}


function tgSetInnerHtml(divId, newContents) {
    if (BrowserDetect.browser == "Explorer" && BrowserDetect.version == "8") {
        tgSetInnerHtmlIE8(divId, newContents);
    } else {
        tgSetInnerHtmlIE9(divId, newContents);
    }
}

function getElementInstance(elid) {
    var elInstance = null;
    if (divExists(elid)) {
        var elobj = $("#" + elid);
        if (objExists(elobj)) {
            if (objExists(elobj[0])) {
                elInstance = elobj[0];
                if (!objExists(elobj[0].innerHTML) && objExists(elobj[0].document)) {
                    elInstance = elobj[0].document;
                } else {
                    elInstance = elobj[0];
                }
            } else {
                if (objExists(elobj.context) && objExists(elobj.context.activeElement)) {
                    elInstance = elobj.context.activeElement;
                }
            }
            
        }
        elobj = null;
    }
    return elInstance;
}



/* appends new text to the current innerHTML contents
 *
 * @param {String} elid		- the id of the element to process
 * @param {String} newtxt   - the string to append
 */
function tgAppendInnerHTML(elid, newtxt) {
    var el = document.getElementById(elid);
    var ihtxt = el.innerHTML;
    ihtxt = ihtxt + newtxt;

    tgSetDivContents(elid, ihtxt);
    ihtxt = null;
    el = null;
}

/* prepends new text to the current innerHTML contents
 *
 * @param {String} elid		- the id of the element to process
 * @param {String} newtxt   - the value to prepend
 */
function tgPrependInnerHTML(elid, newtxt) {
    var el = document.getElementById(elid);
    var ihtxt = el.innerHTML;
    ihtxt = newtxt + ihtxt;

    tgSetDivContents(elid, ihtxt);
    ihtxt = null;
    el = null;
}

/*a test for the existence of a div
 *
 * @param {String} divId - the id of the div to test
 * @return {boolean}
 */
function divExists(divId) {
    try {
    var theDiv = document.getElementById(divId);
    if (theDiv && theDiv !== null) {
        return true;
    } else {
        return false;
    }
    } catch (exception) {
        return false;
    }
}

function objExists(obj) {
    try {
        if (obj !== undefined && obj !== null) {
            return true;
        } else {
            return false;
        }
    } catch (exception) {
        return false;
    }
}


/* remove all child nodes for a given div id
 *
 * @param {String} divId - the id of the div to process
 */
function tgRemoveChildren(divId){
    var ele = null;
    try {
        if (divExists(divId)) {
            var ele = document.getElementById(divId);
            if (ele !== null) {
                while(ele.hasChildNodes()) {
                    if (ele.firstChild !== null) {
                        tgPurgeDiv(ele.firstChild.id);
                    }
                    ele.removeChild(ele.firstChild);
                }
            }
        }
    } catch (ex) {
    }
    ele = null;
}


/* remove a div node with a given div id
 *
 * @param {String} divId - the id of the div to remove
 */
function tgRemoveDiv(divId) {
    tgPurgeDiv(divId);
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        //remove the element from the document
        theDiv.remove(true);
        theDiv = null;
    }
}

/* remove a div node with a given div id
 *
 * @param {String} divId - the id of the div to remove
 */
function tgPurgeAndClearContents(divId) {
        var done = tgPurgeDiv(divId);
        if (divExists(divId) && done) {
            tgSetInnerHtml(divId, EMPTY_STRING);
        }
        done = null;
}

/* purges all child nodes, listeners, and functions of a node with a given div id
 *
 * @param {String} divId - the id of the div to remove
 */
function tgPurgeDiv(divId) {
    try {
        if (divExists(divId)) {
            var theDiv = document.getElementById(divId);
            $(divId).unbind();
            $(divId).remove();
            tgPurgeAllFunctions(theDiv);
            theDiv = null;
        }
    } catch(tgPurgeDivEx){
        // do nothing
    }
    return true;
}


/* sets the contents of a div
 *
 * @param {String} divId 		- the id of the div to receive new contents
 * @param {String} newContents	- the new contents
 */
function tgSetDivContents(divId, newContents) {
    tgRemoveChildren(divId);
    tgSetInnerHtml(divId, newContents);
}

/* hide the div with a given id
 *
 * @param {Object} divId	- the id of the div to hide
 */
function tgHideDiv(divId) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv.style.display = 'none';
        theDiv = null;
    }
}

/* show the div with a given id
 *
 * @param {Object} divId		- the id of the div to show
 * @param {Object} displayStyle - (optional) style.display override (i.e.  'inline')
 */
function tgShowDiv(divId, displayStyle) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        if (displayStyle && displayStyle !== null) {
            theDiv.style.display = displayStyle;
        } else {
            theDiv.style.display = 'block';
        }
        theDiv = null;
    }
}


/* sets the title attribute of a given div
 *
 * @param {String} divId 		- the id of the div to receive new contents
 * @param {Object} titleText	- the title text
 */
function tgSetTitle(divId, titleText) {
    var theDiv = document.getElementById(divId);
    if (divExists(divId)) {
        theDiv.title = titleText;
    }
    theDiv = null;
}

/* gets the title attribute of a given div
 *
 * @param  {String} divId 		- the id of the div to receive new contents
 * @return {String} titleText	- the title text
 */
function tgGetTitle(divId) {
    var titleText = "";
    var theDiv = document.getElementById(divId);
    if (divExists(divId)) {
        titleText = theDiv.title;
    }
    theDiv = null;
    return titleText;
}

/*
 *
 * @param  {String} divId 		- the id of the div to receive new contents
 * @param {Object} imageSrc		- the url string of the image src (i.e.  './images/plus.gif')
 */
function tgSetImageSrc(divId, imageSrc) {
    var theDiv = document.getElementById(divId);
    if (divExists(divId)) {
        theDiv.src = imageSrc;
    }
    theDiv = null;
}


function tgAppendChild(parent, child) {
    if (parent !== null && child !== null) {
        parent.appendChild(child);
    }
}
function tgRemoveChild(child) {
    if (child !== null) {
        var parent = child.parentNode; //just get the childs parent
        if (parent && parent !== null) {
            parent.removeChild(child);
        }
        parent = null;
    }
}

function tgAppendChildByID(divId, childId) {
    if (divExists(divId)) {
        var parent = document.getElementById(divId);
        var child = document.getElementById(childId);
        tgAppendChild(parent, child);
        parent = null;
        child = null;
    }
}

function tgRemoveChildByID(childId) {
    try {
        if (divExists(childId)) {
            tgRemoveChild(document.getElementById(childId));
        }
    } catch (ex) {
        //do nothing
    }
}


function unloadJS(scriptName) {
  var head = document.getElementsByTagName('head').item(0);
  var js = document.getElementById(scriptName);
  js.parentNode.removeChild(js);
}

function unloadAllJS() {
  var jsArray = new Array();
  jsArray = document.getElementsByTagName('script');
  for (i = 0; i < jsArray.length; i){
    if (jsArray[i].id){
      unloadJS(jsArray[i].id);
    }else{
      jsArray[i].parentNode.removeChild(jsArray[i]);
    }
  }
}

function unloadBody() {
//	purge(document.body);
}

function tgPurgeAllFunctions(elemToPurge) {
    try {
        if( elemToPurge ) {
            var attributeList = elemToPurge.attributes;
            var elemCount, listLength, attributeName;
            if (attributeList) {
                listLength = attributeList.length;
                for (elemCount = 0; elemCount < listLength; elemCount += 1) {
                    attributeName = attributeList[elemCount].name;
                    if (typeof elemToPurge[attributeName] === 'function') {
                        elemToPurge[attributeName] = null;
                    }
                }
            }
            attributeList = elemToPurge.childNodes;
            if (attributeList) {
                listLength = attributeList.length;
                for (elemCount = 0; elemCount < listLength; elemCount += 1) {
                    tgPurgeAllFunctions(elemToPurge.childNodes[elemCount]);
                }
            }
            attributeList = null;
            elemCount = null;
            listLength = null;
            attributeName = null;
        }
    } catch (purgeEx){
        // do nothing
    }
}

/*
 * Check the class if exists.
 *
 * @param {Object} elem
 * @param {Object} clsName
 * @return {TypeName}
 */
function tgHasClass(elem,clsName) {
    if (elem && elem !== null) {
        return (elem.className.match(new RegExp('(\\s|^)'+clsName+'(\\s|$)')) !== null);
    }
    return false;
}

/*
 * Add a class to the element.
 *
 * @param {Object} elem
 * @param {Object} clsName
 * @memberOf {TypeName}
 */
function tgAddClass(elem,clsName) {
    if (!this.tgHasClass(elem, clsName))
        elem.className += " " + clsName;
}

/*
 * Replace the class of the element
 *
 * @param {Object} elem
 * @param {Object} clsName
 * @memberOf {TypeName}
 */
function tgReplaceClass(elem, clsName) {
    try {
        if (!this.tgHasClass(elem, clsName))
            elem.className = clsName;
    } catch (ex) { }
}

/*
 * Remove a class from element.
 *
 * @param {Object} elem
 * @param {Object} clsName
 */
function tgRemoveClass(elem,clsName) {
    if (tgHasClass(elem,clsName)) {
        var reg = new RegExp('(\\s|^)'+clsName+'(\\s|$)');
        elem.className=elem.className.replace(reg,' ');
    }
}
/*
 * Removes all classes from element.
 *
 * @param {Object} elem
 */
function tgClearClass(elem) {
    if (elem && elem !== null) {
        if (elem.className && elem.className !== null) {
            elem.className = '';
        }
    }
}
function tgCenterDiv(divname) {
    jq("#" + divname).center();
}



/*********************************************
 *
 * IFrame specific helper functions
 *
 *********************************************/
var ABOUT_BLANK_URL = 'about:blank';

// This is an iframe configuration model
// Use it to build various iframe configurations
var iframeConfigModel = {
    doc: document,
    parent: null,
    id: null,
    name: null,
    src: null,
    width: '100%',
    height: 470,
    scrolling: 'no',
    frameBorder: 0,
    align: 'center',
    valign: 'top',
    marginwidth: 0,
    marginheight: 0,
    hspace: 0,
    vspace: 0
}

// Create an iframe configuration using the iframeConfigModel and ...
//    theDoc   - the base document (document or parent.document). Required.
//    parentId - the id of the iframe's parent element (i.e. a wrapper div).  Nullable (use existing).
//    iframeId - the id of the iframe. Required.
//    url      - the url to be used as the src of the iframe.  Nullable (use existing).
//
// If the iframe is pre-existing, then the configuration attributes will be set to the current values.
//
function createIframeConfig(theDoc, parentId, iframeId, url) {

    var config = iframeConfigModel; //load the iframe config model

    config.doc = theDoc;
    config.id = iframeId;
    config.name = iframeId;

    var theIframe = null;
    if (iframeExists(theDoc, iframeId)) {
        theIframe = getIFrame(theDoc, iframeId);
        config.parent = theIframe.parentNode.id;
        if (stringIsEmpty(theIframe.src)) {
            config.src = ABOUT_BLANK_URL;
        } else {
            config.src = theIframe.src;
        }

        config.width = tgSafeAssign(config.width, theIframe.width);
        config.height = tgSafeAssign(config.height, theIframe.height);
        config.scrolling = tgSafeAssign(config.scrolling, theIframe.scrolling);
        config.align = tgSafeAssign(config.align, theIframe.align);
        config.valign = tgSafeAssign(config.valign, theIframe.valign);
        config.hspace = tgSafeAssign(config.hspace, theIframe.hspace);
        config.vspace = tgSafeAssign(config.vspace, theIframe.vspace);
        config.marginwidth = tgSafeAssign(config.marginwidth,
                theIframe.marginwidth);
        config.marginheight = tgSafeAssign(config.marginheight,
                theIframe.marginheight);
    }

    if (parentId !== null) {
        config.parent = parentId;
    }
    if (url !== null) {
        config.src = url;
    }
    theIframe = null;

    return config;
}

// protects against setting a 'target' var to 'undefined'
function tgSafeAssign(target, source) {
    return (source !== undefined) ? source : target;
}

// Create an iframe using an iframe configuration
function tgCreateIFrame(config) {
    var theDoc = config.doc;

    var iframeHeaderCell = theDoc.getElementById(config.parent);

    var iframeHeader = theDoc.createElement('IFRAME');

    iframeHeader.id = config.id;
    iframeHeader.name = config.name;
    iframeHeader.src = config.src;
    iframeHeader.width = config.width;
    iframeHeader.height = config.height;
    iframeHeader.scrolling = config.scrolling;
    iframeHeader.frameBorder = config.frameBorder;
    iframeHeader.align = config.align;
    iframeHeader.valign = config.valign;
    iframeHeader.marginwidth = config.marginwidth;
    iframeHeader.marginheight = config.marginheight;
    iframeHeader.hspace = config.hspace;
    iframeHeader.vspace = config.vspace;
    //iframeHeaderCell.appendChild(iframeHeader);

    iframeHeader = null;
    iframeHeaderCell = null;
}

// Destroy an existing iframe
function tgDestroyIFrame(theDoc, iframeId) {
    var theIframe = null;
    if (iframeExists(theDoc, iframeId)) {
        theIframe = getIFrame(theDoc, iframeId);
        var parnodeid = '#' + theIframe.parentNode.id;
        var frameid = "#" + iframeId;
        
        try {
            $(frameid).remove();
            removeChildSafe(theIframe);

            $(parnodeid).remove;
        } catch (ex) { }

        theIframe = null;
        parnodeid = null;
    }
}

// Refresh contents of an existing iframe (without creating history)
function tgRefreshIframe(theDoc, iframeId) {
    //var theIframe = null;
    if (iframeExists(theDoc, iframeId)) {
        //theIframe = getIFrame(theDoc, iframeId); //removing - not used
        var iframeConfig = createIframeConfig(theDoc, null, iframeId, null);
        tgDestroyIFrame(theDoc, iframeId);
        tgCreateIFrame(iframeConfig);
        currentSrc = null;
        parentId = null;
    }
    //theIframe = null;
}

// Reset an existing iframe to a new src url
function tgResetIFrame(theDoc, iframeId, url) {
    //var theIframe = null;
    if (iframeExists(theDoc, iframeId)) {
        //theIframe = getIFrame(theDoc, iframeId); //removing - not used
        var iframeConfig = createIframeConfig(theDoc, null, iframeId, url);
        tgDestroyIFrame(theDoc, iframeId);
        tgCreateIFrame(iframeConfig);
    }
}

// Simply clears an iframe by setting the src url to about:blank
function tgClearIFrame(theDoc, iframeId, altClearURL) {
    if (altClearURL && altClearURL !== null) {
        tgResetIFrame(theDoc, iframeId, altClearURL);
    } else {
        tgResetIFrame(theDoc, iframeId, ABOUT_BLANK_URL);
    }
}

// Toggles an iframe in a hide/show manner.
function tgToggleIFrame(theDoc, iframeId, url) {
    var theIframe = null;
    if (iframeExists(theDoc, iframeId)) {
        theIframe = getIFrame(theDoc, iframeId);
        var currentSrc = theIframe.src;
        if (!(currentSrc == ABOUT_BLANK_URL)) {
            tgClearIFrame(theDoc, iframeId);
        } else {
            tgResetIFrame(theDoc, iframeId, url);
        }
    }

    theIframe = null;
}

// a simple iframe getter
function getIFrame(theDoc, iframeId) {
    return theDoc.getElementById(iframeId);
}

// a simple iframe parent node getter
function getIframeParentNode(theDoc, iframeId) {
    var parentNode = null;
    var theIframe = null;
    if (iframeExists(theDoc, iframeId)) {
        theIframe = getIFrame(theDoc, iframeId);
        parentNode = theIframe.parentNode;
    }
    theIframe = null;
    return parentNode;
}

// a simple iframe parent node id getter
function getIframeParentId(theDoc, iframeId) {
    var parentId = null;
    var parentNode = getIframeParentNode(theDoc, iframeId);
    if (parentNode && parentNode !== null) {
        parentId = parentNode.id;
    }
    parentNode = null;
    return parentId;
}

function iframeExists(theDoc, iframeId) {
    var theIframe = getIFrame(theDoc, iframeId);
    if (theIframe && theIframe !== null) {
        return true;
    } else {
        return false;
    }
}

function divExists(divId) {
    var theDiv = document.getElementById(divId);
    if (theDiv && theDiv !== null) {
        return true;
    } else {
        return false;
    }
}

function tgClearDiv(divId) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        //        purge(theDiv);
        removeChildSafe(theDiv);
        theDiv.innerHTML = "";
        theDiv = null;
    }
}

function tgRemoveDiv(divId) {
    tgPurgeDiv(divId);
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv = $("#" + divId);
        //remove the element from the document
        theDiv.remove();
        theDiv = null;
    }
}

function tgPurgeAndClearContents(divId) {

        var done = tgPurgeDiv(divId);

        if (divExists(divId) && done) {
            var theDiv = document.getElementById(divId);
            theDiv.innerHTML = "";
            theDiv = null;
        }

        done = null;

}

function tgPurgeDiv(divId) {
    try {
        if (divExists(divId)) {
            var theDiv = document.getElementById(divId);
            // remove all event listeners	
            $("#" + divId).detach();
            $("#" + divId).remove();
            tgPurgeAllFunctions(theDiv);
            theDiv = null;
        }
    } catch (tgPurgeDivEx) {
        // do nothing
    }
    return true;
}


function tgSetDivContents(divId, newContents) {
    //tgClearDiv(divId);

    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv.innerHTML = newContents;
        theDiv = null;
    }

}

function tgHideDiv(divId) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv.style.display = 'none';
        theDiv = null;
    }
}

function tgShowDiv(divId, displayStyle) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        if (displayStyle && displayStyle !== null) {
            theDiv.style.display = displayStyle;
        } else {
            theDiv.style.display = 'block';
        }
        theDiv = null;
    }
}

function tgAppendChild(divId, childId) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv.appendChild(childId);
        theDiv = null;
    }
}

function tgRemoveChild(divId, childId) {
    if (divExists(divId)) {
        var theDiv = document.getElementById(divId);
        theDiv.removeChild(childId);
        theDiv = null;
    }
}


function tgSetTitle(divId, titleText) {
    var theDiv = document.getElementById(divId);
    if (divExists(divId)) {
        theDiv.title = titleText;
    }
    theDiv = null;
}

function tgSetElementSrc(divId, newsrc) {
    var theDiv = document.getElementById(divId);
    if (divExists(divId)) {
        theDiv.src = newsrc;
    }
    theDiv = null;
}


function removeChildSafe(el) {
    //before deleting el, recursively delete all of its children.
    while (el.childNodes.length > 0) {
        removeChildSafe(el.childNodes[el.childNodes.length - 1]);
    }
    el.parentNode.removeChild(el);
    discardElement(el);
}

function discardElement(el) {
    var bin = document.getElementById("IELeakGarbageBin");

    if (!bin) {
        bin = document.createElement("DIV");
        bin.id = "IELeakGarbageBin";
        document.body.appendChild(bin);
    }

    bin.appendChild(el);
    bin.innerHTML = "";
}

function unloadJS(scriptName) {
    var head = document.getElementsByTagName('head').item(0);
    var js = document.getElementById(scriptName);
    js.parentNode.removeChild(js);
}

function unloadAllJS() {
    var jsArray = new Array();
    jsArray = document.getElementsByTagName('script');
    for (i = 0; i < jsArray.length; i) {
        if (jsArray[i].id) {
            unloadJS(jsArray[i].id)
        } else {
            jsArray[i].parentNode.removeChild(jsArray[i]);
        }
    }
}

function unloadBody() {
    //	purge(document.body);
}

function tgPurgeAllFunctions(elemToPurge) {
    try {
        if (elemToPurge) {
            var attributeList = elemToPurge.attributes;
            var elemCount, listLength, attributeName;
            if (attributeList) {
                listLength = attributeList.length;
                for (elemCount = 0; elemCount < listLength; elemCount += 1) {
                    attributeName = attributeList[elemCount].name;
                    if (typeof elemToPurge[attributeName] === 'function') {
                        elemToPurge[attributeName] = null;
                    }
                }
            }
            attributeList = elemToPurge.childNodes;
            if (attributeList) {
                listLength = attributeList.length;
                for (elemCount = 0; elemCount < listLength; elemCount += 1) {
                    tgPurgeAllFunctions(elemToPurge.childNodes[elemCount]);
                }
            }
            attributeList = null;
            elemCount = null;
            listLength = null;
            attributeName = null;
        }
    } catch (purgeEx) {
        // do nothing
    }
}

function tgAppendInnerHTML(elid, newtxt) {
    var el = document.getElementById(elid);
    var ihtxt = el.innerHTML;
    ihtxt = ihtxt + newtxt;

    tgSetDivContents(elid, ihtxt);
    ihtxt = null;
    el = null;
}

function tgPrependInnerHTML(elid, newtxt) {
    var el = document.getElementById(elid);
    var ihtxt = el.innerHTML;
    ihtxt = newtxt + ihtxt;

    tgSetDivContents(elid, ihtxt);
    ihtxt = null;
    el = null;
}


function trimToEmpty(str) {
    var result = "";
    if (!stringIsEmpty(str)) {
        try {
            var wrkstr = str + "";
            result = wrkstr.trim();
        } catch (ex) {
            result = str;
        }
    }
    return result;
}

function string_contains(haystack, needle) {
    if (haystack.indexOf(needle) == -1) {
        return false;
    } else {
        return true;
    }
}

function stringIsEmpty(str) {
    if (str && str !== null && str !== "") {
        return false;
    } else {
        return true;
    }
}

String.prototype.replaceAll = function (token, newToken, ignoreCase) {
    var _token;
    var str = this + "";
    var i = -1;

    if (typeof token === "string") {

        if (ignoreCase) {

            _token = token.toLowerCase();

            while ((
                i = str.toLowerCase().indexOf(
                    token, i >= 0 ? i + newToken.length : 0
                )) !== -1
            ) {
                str = str.substring(0, i) +
                    newToken +
                    str.substring(i + token.length);
            }

        } else {
            return this.split(token).join(newToken);
        }

    }
    return str;
};


var BrowserDetect = {
    init: function () {
        this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
        this.version = this.searchVersion(navigator.userAgent)
            || this.searchVersion(navigator.appVersion)
            || "an unknown version";
        this.OS = this.searchString(this.dataOS) || "an unknown OS";
    },
    searchString: function (data) {
        for (var i = 0; i < data.length; i++) {
            var dataString = data[i].string;
            var dataProp = data[i].prop;
            this.versionSearchString = data[i].versionSearch || data[i].identity;
            if (dataString) {
                if (dataString.indexOf(data[i].subString) != -1)
                    return data[i].identity;
            }
            else if (dataProp)
                return data[i].identity;
        }
    },
    searchVersion: function (dataString) {
        var index = dataString.indexOf(this.versionSearchString);
        if (index == -1) return;
        return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
    },
    dataBrowser: [
        {
            string: navigator.userAgent,
            subString: "Chrome",
            identity: "Chrome"
        },
        {
            string: navigator.userAgent,
            subString: "OmniWeb",
            versionSearch: "OmniWeb/",
            identity: "OmniWeb"
        },
        {
            string: navigator.vendor,
            subString: "Apple",
            identity: "Safari",
            versionSearch: "Version"
        },
        {
            prop: window.opera,
            identity: "Opera",
            versionSearch: "Version"
        },
        {
            string: navigator.vendor,
            subString: "iCab",
            identity: "iCab"
        },
        {
            string: navigator.vendor,
            subString: "KDE",
            identity: "Konqueror"
        },
        {
            string: navigator.userAgent,
            subString: "Firefox",
            identity: "Firefox"
        },
        {
            string: navigator.vendor,
            subString: "Camino",
            identity: "Camino"
        },
        {		// for newer Netscapes (6+)
            string: navigator.userAgent,
            subString: "Netscape",
            identity: "Netscape"
        },
        {
            string: navigator.userAgent,
            subString: "MSIE",
            identity: "Explorer",
            versionSearch: "MSIE"
        },
        {
            string: navigator.userAgent,
            subString: "Gecko",
            identity: "Mozilla",
            versionSearch: "rv"
        },
        { 		// for older Netscapes (4-)
            string: navigator.userAgent,
            subString: "Mozilla",
            identity: "Netscape",
            versionSearch: "Mozilla"
        }
    ],
    dataOS: [
        {
            string: navigator.platform,
            subString: "Win",
            identity: "Windows"
        },
        {
            string: navigator.platform,
            subString: "Mac",
            identity: "Mac"
        },
        {
            string: navigator.userAgent,
            subString: "iPhone",
            identity: "iPhone/iPod"
        },
        {
            string: navigator.platform,
            subString: "Linux",
            identity: "Linux"
        }
    ]

};
BrowserDetect.init();
