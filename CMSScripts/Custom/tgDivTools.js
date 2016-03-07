//Hi
var EMPTY_STRING = "";

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

/* get the contents of the innerHTML of a given div
 *
 * @param {String} divId		- the id of the div
 * @return {String} currContents	- the current contents
 */
function tgGetInnerHtml(divId) {
	var ih = EMPTY_STRING;
	if (divExists(divId)) {
		var theDiv = document.getElementById(divId);
		ih = theDiv.innerHTML;
		theDiv = null;
	}
	return ih;
}

/* set the contents of the innerHTML of a given div
 *
 * @param {String} divId		- the id of the div
 * @param {String} newContents	- the new contents
 */
function tgSetInnerHtml(divId, newContents) {
	if (divExists(divId)) {
		var theDiv = document.getElementById(divId);
		theDiv.innerHTML = newContents;
		theDiv = null;
	}
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

// Fast javascript function to clear all the options in an HTML select element
// Provide the id of the select element
// References to the old <select> object will become invalidated!
// This function returns a reference to the new select object.
function tgClearSelectOptionsFast(id) {
	var selectObj = document.getElementById(id);
	var selectParentNode = selectObj.parentNode;
	var newSelectObj = selectObj.cloneNode(false); // Make a shallow copy
	selectParentNode.replaceChild(newSelectObj, selectObj);
	return newSelectObj;
}