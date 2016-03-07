var masterCurrentTestID;

function MasterPage_EndRequestHandler(sender, args) {
    if (args.get_error() == undefined && sender._postBackSettings.sourceElement != undefined) {
        var postbackTrigger = sender._postBackSettings.sourceElement;
        if (postbackTrigger.className == "expandCommand" || postbackTrigger.className == "editCommand" || postbackTrigger.className == "RadChart" || postbackTrigger.className == "performanceLevel") {
            if (typeof (showExpandWindow) != 'undefined') showExpandWindow();
        }
        else if (postbackTrigger.className == "rdContent" && getCookie('tileName')) {
            var buttonID = getCookie('tileName') + 'SearchButton_smallTile';
            searchSmallTile_Add_MoreResultsLink_Row(buttonID);
            deleteCookie('tileName');

            if (typeof (showExpandWindow) != 'undefined') showExpandWindow();
        }
        else if (postbackTrigger.id.indexOf("SearchButton_smallTile") > -1) {
            searchSmallTile_Add_MoreResultsLink_Row(postbackTrigger.id);
        }
    }
}

function SetSecureAssessmentTitle(dialogTitle) {
    var titleTable = "<Table><tr><td>";
    var imageUrl = '../Images/IconSecure.png';
    var img = '<img src=' + imageUrl + ' style="width: 51px; height: 20px;">';
    titleTable += img + "</td><td> ";
    titleTable += dialogTitle + "</td></tr></table>";
    dialogTitle = titleTable;
    return dialogTitle;
}

function setCookie(name, value, expires, path, domain, secure, subname) {
    if (subname) {
        var value_str = getCookie(name);
        //value_str = value_str.replace(/\?/gi, '\\\?');*/
        //alert('before :' + value_str);
        value = setStrData(value_str, subname, value);
        //alert('after: ' + value);
    }
    var curCookie = name + "=" + escape(value) +
			((expires) ? "; expires=" + expires.toGMTString() : "") +
			((path) ? "; path=" + path : "") +
			((domain) ? "; domain=" + domain : "") +
			((secure) ? "; secure" : "");
    document.cookie = curCookie;
}

function getURLParm(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function getCookie(name, subname) {
    var dc = document.cookie;
    if (subname) {
        //alert('before g: ' + dc);
        dc = getStrData(dc, name);
        name = subname;
        //alert('after g, subname=' + subname  + ': ' + dc);
    }
    var prefix = name + "=";
    var begin;
    if (dc.substring(0, prefix.length) == prefix) {
        //alert(dc.substring(0,prefix.length));
        begin = 0;
    }
    else {
        begin = dc.indexOf("; " + prefix);
        if (begin == -1) {
            begin = dc.indexOf(prefix);
            if (begin != 0) return null;
        } else
            begin += 2;
    }
    var end = dc.indexOf(";", begin);
    if (end == -1)
        end = dc.length;

    return unescape(dc.substring(begin + prefix.length, end));
}

function deleteCookie(name, path, domain) {
    if (getCookie(name)) {
        document.cookie = name + "=" +
		((path) ? "; path=" + path : "") +
		((domain) ? "; domain=" + domain : "") +
		"; expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
}

function onlyNums(obj, e) {
    var keynum = '';
    var keychar = '';
    var numcheck = '';

    if (window.event) {// IE
        keynum = e.keyCode;
    }
    else if (e.which) {// Netscape/Firefox/Opera
        keynum = e.which;
    }

    keychar = String.fromCharCode(keynum);
    numcheck = /\d/;

    return numcheck.test(keychar);
}

function searchSmallTile_SubmitOnEnter(obj, e) {
    var keynum = '';
    var keychar = '';
    var numcheck = '';

    if (window.event) {// IE
        keynum = e.keyCode;
    }
    else if (e.which) {// Netscape/Firefox/Opera
        keynum = e.which;
    }

    if (keynum == 13) {
        document.getElementById(obj.id.replace('SearchText_smallTile', '') + 'SearchButton_smallTile').click();
        return false;
    }

    return true;
}

function isSearchText(buttonObj) {
    var searchTextInput = $('#' + buttonObj.id.replace('SearchButton_smallTile', '') + 'SearchText_smallTile');
    if (searchTextInput.attr('value') == '' || searchTextInput.attr('value') == searchTextInput.attr('defaulttext')) return false;

    return true;
}

function searchSmallTile_Add_MoreResultsLink_Row(buttonID) {
    var searchTileGrid = $('#' + buttonID.replace('SearchButton_smallTile', '') + 'SearchTileGrid');
    var searchMoreLinkSpan = $('#' + buttonID.replace('SearchButton_smallTile', '') + 'SearchMoreLinkSpan');
    if (searchTileGrid && searchMoreLinkSpan && searchMoreLinkSpan.html() != '') {
        $('.rgMasterTable', searchTileGrid).append('<tr id="__101" class="rgAltRow"><td class="searchResults_smallTile">' + searchMoreLinkSpan.html() + '</td></tr>');
        searchMoreLinkSpan.html('');
    }
}

function searchSmallTileAppendMoreLinkOnLoad(clientID, spanID) {
    var searchTileGrid = $('#' + clientID);
    var searchMoreLinkSpan = $('#' + spanID);
    if (searchTileGrid && searchMoreLinkSpan && searchMoreLinkSpan.html() != '') {
        $('.rgMasterTable', searchTileGrid).append('<tr id="__100" class="rgAltRow"><td class="searchResults_smallTile" colspan="4">' + searchMoreLinkSpan.html() + '</td></tr>');
        searchMoreLinkSpan.html('');
    }
}

function setSearchTileNameCookie(tileName) {
    setCookie('tileName', tileName);
}

function dropdownButtonClick(sender, args) {
    if (!sender.get_commandName()) {
        var senderElement = sender.get_element();
        var currentLocation = $telerik.getLocation(senderElement);
        var contextMenu = $find(senderElement.getAttribute('dropdownListID'));
        contextMenu.showAt(currentLocation.x, currentLocation.y + 22);
    }
}

function dropdownItemsClick(sender, args) {
    var senderElement = sender.get_element();
    var itemText;
    if (args.get_item()._attributes && args.get_item()._attributes.getAttribute('buttonText')) itemText = args.get_item()._attributes.getAttribute('buttonText');
    else itemText = args.get_item().get_text();
    var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
    var childButton = $find(senderElement.getAttribute('childButtonID'));
    var defaultChildButtonText = senderElement.getAttribute('defaultChildButtonText');

    dropdownButton.set_text(itemText);
    if (childButton && defaultChildButtonText) childButton.set_text(defaultChildButtonText);
}

function requestChildFilter(sender, args) {
    var senderElement = sender.get_element();
    var dropdownButton = $find(senderElement.getAttribute('dropdownButtonID'));
    //var childButtonID = $find(senderElement.getAttribute('childButtonID')); See comment below.
    var itemText = args.get_item().get_text();
    var classID = senderElement.getAttribute('classID');
    var levelID = senderElement.getAttribute('levelID');
    var panelID = senderElement.getAttribute('xmlHttpPanelID');
    var panel = $find(panelID);

    panel.set_value('{"ClassID":"' + classID + '", "LevelID":"' + levelID + '", "StandardsSet":"' + itemText + '"}');

    /*  BJC: As per David, they will now initially load the standards list filtered with the first selection from each filter, 
    so the second filter button will always be enabled now and therefore, the code below is no longer needed (for now).

    if (childButtonID) enableInactiveButton(childButtonID, dropdownButton, itemText);
    */
}

function enableInactiveButton(inactiveButton, dropdownButton, itemText) {
    if (itemText != '' && !inactiveButton.get_enabled()) {
        inactiveButton.set_enabled(true);
    }
}

function filterBtnClick(sender, args) {
    var senderElement = sender.get_element();
    var currentLocation = $telerik.getLocation(senderElement);
    var contextMenu = $find(senderElement.getAttribute('dropdownListID'));
    contextMenu._targetElement = senderElement;
    contextMenu.showAt(currentLocation.x, currentLocation.y + 18);
}

function toggleView_SmallTile(buttonObj, containerID, viewClass) {
    var extraCommandPanelID = buttonObj.parentNode.id;
    var containerDivID = extraCommandPanelID.substr(0, extraCommandPanelID.indexOf(containerID) + containerID.length);
    var containerDiv = $('#' + containerDivID);
    var selectedButton = $('.toggleViewButtons_selected', containerDiv);

    //hide both views
    $('.listView', containerDiv).css('display', 'none');
    $('.graphicalView', containerDiv).css('display', 'none');

    //change background of selected button and revert previously selected button background
    selectedButton.removeClass('toggleViewButtons_selected');
    selectedButton.addClass('toggleViewButtons');
    buttonObj.className = 'toggleViewButtons_selected';

    //show selected view
    $('.' + viewClass, containerDiv).css('display', '');
}

function toggleView_SmallTile2(buttonObj, containerID, viewClass, otherButton, thisButton) {
    var extraCommandPanelID = buttonObj.parentNode.id;
    var containerDivID = extraCommandPanelID.substr(0, extraCommandPanelID.indexOf(containerID) + containerID.length);
    var containerDiv = $('#' + containerDivID);

    var selectedButton = $('.toggleViewButtons_selected', containerDiv);

    var otherViewBtn = $('#' + otherButton);
    var thisViewBtn = $('#' + thisButton);

    //hide both views
    $('.listView', containerDiv).css('display', 'none');
    $('.graphicalView', containerDiv).css('display', 'none');

    thisViewBtn.css('display', 'none');
    otherViewBtn.css('display', '');

    //change background of selected button and revert previously selected button background
    selectedButton.removeClass('toggleViewButtons_selected');
    selectedButton.addClass('toggleViewButtons');
    buttonObj.className = 'toggleViewButtons_selected';

    //show selected view
    $('.' + viewClass, containerDiv).css('display', '');

    // and save this view in a hidden field (if it exists).
    try {
        var tbxCurrView = $('#currView', containerDiv);
        if (tbxCurrView != null)
            tbxCurrView.val(viewClass);
    }
    catch (e) { }
}

function toggleView_RadTab_SmallTile(sender, args) {
    var toggleViewIndexList = sender._attributes.getAttribute('toggleViewIndexList');
    if (!toggleViewIndexList) return false;

    var indexArray = toggleViewIndexList.split(',');
    var containerDiv = $('#' + sender._attributes.getAttribute('containerDivID'));
    var selectedIndex = sender._selectedIndex;

    if (!containerDiv) return false;

    $('.extraCommandContainer', containerDiv).css('display', 'none');

    for (var i = 0; i < indexArray.length; i++) {
        if (indexArray[i] == selectedIndex) {
            $('.extraCommandContainer', containerDiv).css('display', '');
            break;
        }
    }
}

function isTouchDevice() {
    return "ontouchstart" in window;
}

// Older versions of IE don't support array indexOf
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (obj) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == obj) {
                return i;
            }
        }
        return -1;
    };
}

function getCurrentCustomDialogByName(dialogName) {
    var oWnd = $find(dialogName);
    if (oWnd) {
        return oWnd;
    }
    else {
        if (typeof parent != "undefined") {
            return parent.getCurrentCustomDialogByName(dialogName);

        } else {
            return null;
        }
    }
}

// Gets the current RadWindow the user is currently viewing.
function getCurrentCustomDialog() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement && window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
}

// Closes the current RadWindow the user is currently viewing.
function closeCurrentCustomDialog() {
    var oWnd = getCurrentCustomDialog();
    //oWnd.close();
    setTimeout(function () {
        oWnd.close();
    }, 0);
}

function closeCustomDialog(dialogName) {
    var oWnd = $find(dialogName == null ? "RadWindow1Url" : dialogName);
    oWnd.close();
}

function closeParentWindowMsgWise(msg) {
    if (msg == "Your help request has been submitted.")
        closeCustomDialog('RadWindow1Url');
}

function customDialog(options, buttons) {
    masterCurrentTestID = options.testID;
    currentTestID = masterCurrentTestID;
    var defaults = {
        name: null,
        width: 500, /* outer width of rad window */
        height: 100, /* outer height of rad window */
        modal: true, /* is modal? */

        maximizable: false,
        maximize_width: false,
        resizable: false, /* is rad window resizable? */
        movable: false, /* is rad window movable? */
        closeMode: true,/* is close button[X] visible? */
        title: '', /* rad window title */
        content: '', /* rad window content */
        autoSize: false,
        dialog_style: null, /* valid values are confirm and alert */
        animation: null,
        url: null,
        persistFocusOnMaximize: false,
        destroyOnClose: false,
        // event handler 'this' reference is to the current RadWindow.  @content parameter is a reference to the jQuery wrapped content
        onOpen: function (content) {
        }, /* onOpen event handler */
        onClosing: function (content) {
        }, /* onClosing event handler */
        onClosed: function (content) {
        } /* onClosed event handler */
    };
    // merge @options with default settings
    options = $telerik.$.extend({}, defaults, (options) ? options : {});

    // parse the x and y positions
    options.xPos = parseInt(options.xPos);
    options.yPos = parseInt(options.yPos);

    var win;

    // on at least one occasion, what is passed in as a title to our 
    // window might be escaped, so we'll unescape name just in case.
    options.title = unescape(options.title);
    options.xPos = unescape(options.xPos);
    options.yPos = unescape(options.yPos);

    if (options.url) {
        win = window.radopen(options.url, options.name ? options.name : "RadWindow1Url");
    } else {
        //win = window.radopen("", options.name ? options.name : "RadWindow1");
        win = window.radopen(null, null);
    }
    win.set_visibleStatusbar(false);
    //win.set_destroyOnClose(options.destroyOnClose);       // WSH rolling this change back temporarily   // WSH 11/28/12 I've found that changing this from true to false resolves the IE crash that has resulted from closing a radwindow with KB2761451 installed. I'm not 100% comfortable with this. I'm not sure what unintended consequences there may be. Testing will be needed 

    // WSH rolling this change back temporarily   // WSH 11/28/12 I've found that changing this from true to false resolves the IE crash that has resulted from closing a radwindow with KB2761451 installed. I'm not 100% comfortable with this. I'm not sure what unintended consequences there may be. Testing will be needed 
    /*14May2014 Setting destroy on close to true as  this is required to fix TFS bug #18607.( ie. In Safari - Previous selected criteria is not cleared and same session appears on clicking expand on Assessment Result tile). Also did not notice an crash in IE 10 */
    win.set_destroyOnClose(true);  //Kumar: Dont like this change. Need to fix it for later.


    if (!win) {
        alert('Error: Window is not defined in aspx.');
        return null;
    }

    // render the window as modal
    if (options.modal) {
        win.set_modal(options.modal);

        //set whether window will persist focus or not
        win.set_showOnTopWhenMaximized(options.persistFocusOnMaximize);
    }

    //win.set_autoSizeBehaviors("Height");
    // set the window width
    win.set_width(options.width);

    // set the window height
    win.set_height(options.height);

    var bounds = $telerik.getClientBounds();
    var dialogHeight = 3000;
    var dialogWidth = 3000;
    try {
        var obj;
        if (obj = getCurrentCustomDialog()) {
            dialogHeight = obj.get_height() + (isTouchDevice() ? -50 : 0);
            dialogWidth = obj.get_width() + (isTouchDevice() ? -50 : 0);
        }
    } catch (e) {
    }
    if (options.maximize) {
        // this is to make it work as well as possible on iPad
        options.maximize_height = options.maximize_width = true;
        win.maximize();
        var maxHeight = win.get_height() * 0.95;
        var maxWidth = win.get_width() * 0.95;
        if ((options.maxheight) < maxHeight)
            maxHeight = options.maxheight;
        if ((options.maxWidth) < maxWidth)
            maxWidth = options.maxwidth;
        $telerik.removeCssClasses(win._popupElement, ["rwMaximizedWindow"]);
        win.set_width(maxWidth);
        win.set_height(maxHeight);
    }
    /*
    alert('bounds:' + bounds.width + ":" + bounds.height);
    alert('dialog height: ' + dialogHeight);
    alert('wh: ' + $telerik.$(window).height());
    alert('dialog width: ' + dialogWidth);
    alert('ww: ' + $telerik.$(window).width());*/
    if (options.maximize_height) {
        if (options.maxheight < dialogHeight && options.maxheight < $telerik.$(window).height())
            win.set_height(Math.ceil(options.maxheight));
        else {
            if (dialogHeight < $telerik.$(window).height())
                win.set_height(Math.ceil(dialogHeight * 0.95));
            else
                win.set_height(Math.ceil($telerik.$(window).height() * 0.95));
        }
    }
    if (options.maximize_width) {
        if (options.maxwidth < dialogWidth && options.maxheight < $telerik.$(window).width())
            win.set_width(Math.ceil(options.maxwidth));
        else {
            if (dialogWidth < $telerik.$(window).width())
                win.set_width(Math.ceil(dialogWidth * 0.95));
            else
                win.set_width(Math.ceil($telerik.$(window).width() * 0.95));
        }

    }
    if (options.minWidth) {
        win.set_minWidth(1000);
    }
    // set the window xy coordinates or center the window or use default positioning
    /*if ((!isNaN(options.xPos)) && (!isNaN(options.yPos))) win.moveTo(options.xPos, options.yPos);
    else if (!!options.center) win.center();
    */

    // set the window title
    win.set_title(options.title);

    var $content;

    if (options.dialog_style == 'confirm') {
        $content = $telerik.$('<div class="E3Dialog_oride rwDialogPopup radconfirm"></div>').append(options.content);
    }
    else if (options.dialog_style == 'alert')
        $content = $telerik.$('<div class="E3Dialog_oride rwDialogPopup radalert"></div>').append(options.content);
    else
        $content = $telerik.$('<div class="E3Dialog_oride"></div>').append(options.content);
    //var $content = $(options.content);

    var $button;
    if (buttons) {
        for (var j = 0; j < buttons.length; j++) {
            //$content.append("<div><a class=\"rwPopupButton\" tabIndex=\"-1\" onclick=\"customDialog(this, " + buttons[j].callback + ");\" href=\"javascript:void(0);\"><span class=\"rwOuterSpan\"><span class=\"rwInnerSpan\">" + buttons[j].title + "</span></span></a></div>");
            //$content.append("<div></div>").append("<a class=\"rwPopupButton\" tabIndex=\"-1\" onclick=\"customDialog(this, " + buttons[j].callback + ");\" href=\"javascript:void(0);\"><span class=\"rwOuterSpan\"><span class=\"rwInnerSpan\">" + buttons[j].title + "</span></span></a>");
            $button = $telerik.$("<a button_id=\"" + j + "\" class=\"rwPopupButton\" tabIndex=\"-1\" href=\"javascript:void(0);\"></a>");
            $button.bind('click', customDialogCallback);
            $button.callback = buttons[j];
            $content.append("<div></div>").append($button.append("<span class=\"rwOuterSpan\"><span class=\"rwInnerSpan\">" + buttons[j].title + "</span></span>"));
        }
    }

    // Call this function to add a confirm dialog when the window is closing. This code looks crazy but it is in an order so 
    // it will add/remove events correctly if the window stays open and is navigating from page to page.
    win.addConfirmDialog = function (dialogOptions, mainWindow) {
        var dialogDefaults = {
            title: 'Cancel',
            text: 'Proceeding with <b>Cancel</b> discards any entries made and does not create an assessment. Select OK to continue with the Cancel action or Cancel to return to the current window.',
            width: 350,
            height: 100,
            callback: null
        };

        dialogOptions = $telerik.$.extend({}, dialogDefaults, (dialogOptions) ? dialogOptions : {});

        win.confirmBeforeClose = function (sender, arg) {
            function callbackFunction(arg) {
                if (arg) {
                    sender.remove_beforeClose(win.confirmBeforeClose);

                    if (dialogOptions.callback == null) {
                        window.setTimeout(function () { sender.close(); }, 0);
                    } else {
                        dialogOptions.callback();
                    }
                }
            }

            arg.set_cancel(true);
            if (mainWindow == null) {
                mainWindow = getCurrentCustomDialog();
            }

            var confirmWindow = mainWindow.radconfirm(dialogOptions.text,
								callbackFunction,
								dialogOptions.width,
								dialogOptions.height,
								null,
								dialogOptions.title);

            confirmWindow.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);

        };

        function addConfirmBeforeClose() {
            win.add_beforeClose(win.confirmBeforeClose);
            win.remove_pageLoad(addConfirmBeforeClose);
        }

        win.add_pageLoad(addConfirmBeforeClose);
    };

    // This will always make sure that there isn't a confirm dialog left over from a previous navigation on the same window.
    win.remove_beforeClose(win.confirmBeforeClose);

    //oWnd.set_contentElement = $("#proof_dialog");
    // add onClosing and onClosed event handlers
    win.add_beforeClose(function () { options.onClosing.call(win, $content); });
    win.add_close(function () { options.onClosed.call(win, $content); });
    //win.add_close(function () { options.onClosing.call(win, $content); });
    //  win.set_initialBehaviors(Telerik.Web.UI.WindowBehaviors.Maximize);

    // set the window behaviors (Close and/or Resize and/or Move)
    win.set_behaviors(((!!options.movable) ? Telerik.Web.UI.WindowBehaviors.Move : 0) + ((!!options.closeMode) ? Telerik.Web.UI.WindowBehaviors.Close : Telerik.Web.UI.WindowBehaviors.None) + ((!!options.resizable) ? Telerik.Web.UI.WindowBehaviors.Resize : 0) + ((!!options.maximizable) ? Telerik.Web.UI.WindowBehaviors.Maximize : 0));
    if (options.animation) win.set_animation(eval("Telerik.Web.UI.WindowAnimation." + options.animation));

    if (!options.url) {
        //win.setUrl('');
        //win.setUrl(options.url);
        //} else {
        $(win._iframe).remove();         // remove the iframe from the default rad window
    }
    win.set_autoSize(options.autoSize);
    win.show();

    function customDialogCallback(event) {
        var $button_html = $(this);
        var $button_spec = buttons[$button_html.attr('button_id')];
        if ($button_spec.autoClose != false) {
            if (options.url) {
                win.close();
            } else {
                win.close();
            }
        }

        if ($button_spec.callback)
            if (typeof $button_spec.argArray == 'undefined' || !$button_spec.argArray)
                $button_spec.callback.apply(this);
            else
                $button_spec.callback.apply(this, $button_spec.argArray);

        closeParentWindowMsgWise(options.content);
    }

    // add the wrapped content to the rad window - use the set_contentElement(content) function
    setTimeout(function () {
        if (!options.url) {
            $(win._contentCell).html("");       // this is undocumented approach to clear the content since we are reusing this dialog.
            win.set_contentElement($content.get(0));
        }

        // fire the onOpen event handler
        if (options.onOpen) options.onOpen.call(win, $content);
        if (options.center) win.center();

        win.setActive(true);    // bring window in front of modal dimmer when dealing with pseudo-maximized

        //BJC 7/24/2012: Fixes problem in IE8 with top of window positioning above the top of the screen.
        var winBounds = win.getWindowBounds();
        if (winBounds && winBounds.y < 0) win.moveTo(winBounds.x + 'px', '10px');
    }, 0);

    /* Corey Creech 2/24/2014 set the window xy coordinates to the left upper conner of the container
       because Rubric Text was not fully displayed when yPos and xPos have no value  */
    if ((!isNaN(options.xPos)) && (!isNaN(options.yPos))) win.moveTo(options.xPos, options.yPos);

    return win;
}

function createAssessmentOptionsCallback() {
    parent.customDialog({
        name: 'RadWindowAddAssessment',
        url: '../Dialogues/Assessment/CreateAssessmentOptions.aspx',
        title: 'Options to Create Assessment',
        maximize: true, maxwidth: 550, maxheight: 450
    });
}

function expandAssessmentResults(sender, args) {
    var url = args.command.get_name();
    var category = $find('AssessmentResults_RadTabStrip').get_selectedTab().get_text();
    var term = document.getElementById("AssessmentResults_term").value;
    var type = document.getElementById("AssessmentResults_type").value;
    var level = document.getElementById("AssessmentResults_level").value;
    var levelID = document.getElementById("AssessmentResults_levelID").value;

    url = url + "?category=" + category + "&term=" + term + "&type=" + type + "&level=" + level + "&levelID=" + levelID;
    customDialog({ url: url, maximize: true, maxwidth: 950, maxheight: 6750 });
}

function openExpandEditRadWindow(sender, args) {
    doOpenExpandEditRadWindow(args.command.get_name());
}

function doOpenExpandEditRadWindow(u) {
    var windowName = u.substring(u.lastIndexOf('/') + 1, u.lastIndexOf('.aspx'));
    customDialog({ url: u, width: 1050, height: 675, name: windowName });
}

function openAssessmentObjectIdentificationEditRadWindow(sender, args) {
    customDialog({ url: args.command.get_name(), maximize: true, maxwidth: 650, maxheight: 600 });
}

function openAssessmentObjectContentEditRadWindow(sender, args) {
    customDialog({ url: args.command.get_name(), name: "AssessmentPage", maximize: true });
}

function openAssessmentObjectContentExpandRadWindow(sender, args) {
    var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
    var winHeight = adjustedHeight < 625 ? adjustedHeight : 625; //to compensate for clients with very low resolution
    var dialogTitle = "Configure Assessment";
    var hasSecure = hasPermission.toLowerCase() == "true" ? true : false;
    var isSecureflag = isSecuredFlag.toLowerCase() == "true" ? true : false;
    var secureAssessment = SecureType.toLowerCase() == "true" ? true : false;
    if (hasSecure == true && isSecureflag == true) {
        if (secureAssessment == true) {
            dialogTitle = SetSecureAssessmentTitle(dialogTitle);
        }
    }
    customDialog({ title: dialogTitle, url: args.command.get_name(), maximize: true, maxwidth: 800 });
}

function openAssessmentObjectConfigurationEditRadWindow(sender, args) {
    var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
    var winHeight = adjustedHeight < 675 ? adjustedHeight : 675; //to compensate for clients with very low resolution
    customDialog({ url: args.command.get_name(), maximize: false, width: 800, height: 700 });
}

function openStaffIdentificationEditRadWindow(sender, args) {
    var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
    var winHeight = adjustedHeight < 750 ? adjustedHeight : 750; //to compensate for clients with very low resolution
    customDialog({ url: args.command.get_name(), height: 665, width: 630 });
}

function openTeacherStaffIdentificationEditRadWindow(sender, args) {
    var adjustedHeight = Math.ceil($telerik.$(parent.window).height() * 96 / 100);
    var winHeight = adjustedHeight < 675 ? adjustedHeight : 675; //to compensate for clients with very low resolution
    customDialog({ url: args.command.get_name(), height: 675, width: 675 });
}

function openPieChartExpandedWindow(sender, args) {
    var senderElement = sender.get_element();
    var level = senderElement.getAttribute('level');
    var levelID = senderElement.getAttribute('levelID');
    var controlURL = senderElement.getAttribute('controlURL') + '?level=' + level + '&levelID=' + levelID;
    customDialog({ url: controlURL, maximize: true, maxwidth: 950, maxheight: 675 });
}

function adjustCells(input) {
    switch (input.value) {
        case "correct":
            if (input.checked) {
                $('[answerType="green"]', $('.rgDataDiv')).each(function () {
                    $(this).css('background-color', 'green');
                });
            }
            else {
                $('[answerType="green"]', $('.rgDataDiv')).each(function () {
                    $(this).css('background-color', '');
                });
            }
            break;
        case "incorrect":
            if (input.checked) {
                $('[answerType="red"]', $('.rgDataDiv')).each(function () {
                    $(this).css('background-color', $(this).attr('answerType'));
                });
                $('[answerType="yellow"]', $('.rgDataDiv')).each(function () {
                    $(this).css('background-color', $(this).attr('answerType'));
                    $(this).css('color', 'black');
                });
                $('[answerType="yellow"] a').each(function () {
                    $(this).css('color', 'black');
                });

            }
            else {
                $('[answerType="red"]', $('.rgDataDiv')).each(function () {
                    $(this).css('background-color', '');
                });
                $('[answerType="yellow"]', $('.rgDataDiv')).each(function () {
                    $(this).css('background-color', '');
                    $(this).css('color', 'white');
                });
                $('[answerType="yellow"] a').each(function () {
                    $(this).css('color', 'white');
                });
            }
            break;
        case "standards":
            if (input.checked) document.getElementById('standardsRow').parentNode.style.display = '';
            else document.getElementById('standardsRow').parentNode.style.display = 'none';
            break;
        case "studentid":
            if (input.checked) {
                $('[studentid="studentid"]').each(function () {
                    $(this).css('display', 'none');
                });
            }
            else {
                $('[studentid="studentid"]').each(function () {
                    $(this).css('display', '');
                });
            }
            break;
        case "studentname":
            if (input.checked) {
                $('[studentname="studentname"]').each(function () {
                    $(this).html('')
                    $(this).removeAttr("onclick");
                    $(this).removeAttr("style");
                });
            }
            else {
                $('[studentname="studentname"]').each(function () {
                    $(this).html($(this).attr('sname'))
                    $(this).attr("onClick", $(this).attr('onClickHide'));
                    $(this).attr("style", $(this).attr('styleHide'));
                });
            }
            break;
        default:
            if (input.checked) document.getElementById('rigorRow').parentNode.style.display = '';
            else document.getElementById('rigorRow').parentNode.style.display = 'none';
            break;

    }
}

function sortRadGridWithCustomHeaderLinks_OnClick(obj, e) {
    var el = e.target || e.srcElement;
    if (el.tagName == 'A') return;

    var aTag = obj.getElementsByTagName('a');
    if (aTag.length > 0) eval(aTag[0].getAttribute('href').replace('javascript:', ''));
}

function showHideSortArrow(obj, bgColor) {
    if (obj.getAttribute('selectedSortColumn') == 'true') return false;

    if (obj.style.background == '') {
        obj.style.background = 'url(../Images/downarrow.gif) no-repeat right center';
    }
    else {
        obj.style.background = '';
    }

    if (bgColor && bgColor != '') obj.style.backgroundColor = bgColor;
}

function requestCourseFilter_StandardsSearchSmallTile(sender, args) {
    var senderElement = sender.get_element();
    var itemText = args.get_item().get_text();
    var classID = senderElement.getAttribute('classID');
    var levelID = senderElement.getAttribute('levelID');
    var panelID = senderElement.getAttribute('xmlHttpPanelID');
    var panel = $find(panelID);

    panel.set_value('{"ClassID":"' + classID + '", "LevelID":"' + levelID + '", "StandardsSet":"' + itemText + '"}');
}

function loadFilter_StandardsSearchSmallTile(sender, args) {
    //load panel
    var senderElement = sender.get_element();
    var results = args.get_content();
    var dropdownObjectID = senderElement.getAttribute('objectToLoadID');
    var dropdownObject = $find(dropdownObjectID);
    var gridObj = $('#' + senderElement.getAttribute('gridClientID'));

    //Clear all context menu items
    clearAllDropdownItems_StandardsSearchSmallTile(dropdownObject);

    /*Add each new context menu item
    results[i].Key(value) = full text
    results[i].Value(button display) = short text...
    */
    for (var i = 0; i < results.length; i++) {
        addDropdownItem_StandardsSearchSmallTile(dropdownObject, results[i].Key, results[i].Value);
    }

    if (results.length == 1) {
        dropdownObject._selectedIndex = 0;
        dropdownObject.set_text(results[0].Value);
        gridObj.css('display', '');
        __doPostBack(dropdownObjectID);
    }
    else {
        dropdownObject.set_text('');
        dropdownObject._applyEmptyMessage();
        gridObj.css('display', 'none');
        __doPostBack(dropdownObjectID);
    }
}

function addDropdownItem_StandardsSearchSmallTile(dropdownObject, itemTextandValue, buttonText) {
    if (!dropdownObject || !itemTextandValue || !buttonText) {
        return false;
    }

    /*indicates that client-side changes are going to be made and 
    these changes are supposed to be persisted after postback.*/
    dropdownObject.trackChanges();

    //Instantiate a new client item
    var item = new Telerik.Web.UI.RadComboBoxItem();

    //Set its text and add the item
    item.set_text(itemTextandValue);
    item.set_value(itemTextandValue);
    dropdownObject.get_items().add(item);

    //submit the changes to the server.
    dropdownObject.commitChanges();
}

function clearAllDropdownItems_StandardsSearchSmallTile(dropdownObject) {
    var allItems = dropdownObject.get_items().get_count();
    if (allItems.length < 1) {
        return false;
    }

    /*indicates that client-side changes are going to be made and 
    these changes are supposed to be persisted after postback.*/
    dropdownObject.trackChanges();

    //clear all items
    dropdownObject.get_items().clear();

    //submit the changes to the server.
    dropdownObject.commitChanges();

    return false;
}

function standardsSearchTrigger(sender, args) {
    __doPostBack(sender.get_id());
}


/* Hal 3-21-12
Show the AssessmentAdministration window only if we have a teacher id and class id.
*/
function viewAsssessment_adminClick(assessmentID, classID, testName, category, level, isGroup, isSecure) {
    if (assessmentID != null && classID == null) {
        customDialog({ name: "AssessmentAssignmentSearch_ExpandedWindow", url: ('../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=false&assessmentID=' + assessmentID + '&testcategory=' + category + '&level=' + level + '&isSecure=' + isSecure), maximize: true, maxwidth: 950, maxheight: 675 });
    }
    else if (assessmentID == null) {
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    }
    else
        customDialog({ url: ('../Dialogues/Assessment/AssessmentAdministration.aspx?xID=' + assessmentID.toString() + '&yID=' + classID.toString() + '&IsGroup=' + isGroup.toString()), autoSize: true });
}


/* Hal 4-18-12
Show the print assessment dialog.
*/
function viewAsssessment_printClick(assessmentID, testName) {
    if (assessmentID == null)
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    else
        customDialog({ url: ('../Dialogues/Assessment/AssessmentPrint.aspx?xID=' + assessmentID + '&yID=' + escape(testName)), maximize: true, maxwidth: 520, maxheight: 400 });
}

function searchAsssessment_printClick(assessmentID, testName) {
    if (assessmentID == null)
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    else
        customDialog({ url: ('../../Dialogues/Assessment/AssessmentPrint.aspx?xID=' + assessmentID + '&yID=' + escape(testName)), maximize: true, maxwidth: 520, maxheight: 400 });
}

function searchAsssessment_editClick(assessmentID, testName, url) {
    if (assessmentID == null)
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    else
        if (parent.customDialog && typeof (parent.customDialog) != 'undefined')
            parent.customDialog({ url: (url), title: testName, maximize: true });
}

function functionIsInDevelopment() {
    var wnd = radalert('Function is in development.', 300, 100);
    wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
}

function showProfile() {
    var profile_data = $("#__asptrace");
    if (!profile_data) return;
    top.consoleRef = window.open('', 'myconsole',
				'width=350,height=250'
						+ ',menubar=0'
								+ ',toolbar=1'
										+ ',status=0'
												+ ',scrollbars=1'
														+ ',resizable=1');
    top.consoleRef.document.writeln(profile_data.html());
    top.consoleRef.document.close();
}

//PLH - Bad hack to hide the navigation pane in legacy when bridging back to the test targetting screen. Should only be temporary until targeting is complete in E3. 
function OpenURL(sender, args) {
    var newWin = window.open(args.command.get_name(), '_blank');
    newWin.focus();

    var i = 0;
    var formLoaded = false;
    while (formLoaded == false) {
        try {
            if (newWin.document.readyState == 'complete') {
                formLoaded = true;
            }
            i++;

            if (i > 100000) {
                break;
            }
        }
        catch (ex) {
        }
    }

    if (formLoaded == true) {
        newWin.document.getElementById('Execute7099|modalmenu').style.display = 'none';
        newWin.document.getElementById('Targets').click();
    }
}

function searchAsssessment_adminClick(assessmentID, testName, category, issecure) {
    if (assessmentID == null || testName == null) {
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
								[{ title: 'Cancel' }]);
    }
    else {
        var win = parent.customDialog({ name: "AssessmentAssignmentSearch_ExpandedWindow", url: ('../Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=true&assessmentID=' + assessmentID + '&testcategory=' + category + '&isSecure=' + issecure), title: testName, maximize: true, maxwidth: 950, maxheight: 675 });
        setTimeout(function () {
            win.setActive(true);
        }, 0);
    }
}

function searchAsssessmentAssignment_printClick(classTestEvents, assessmentCategory) {
    if (classTestEvents == null) {
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    }
    else if ((classTestEvents.split(',').length - 1) > 50) {
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: '', content: 'You selected over 50 classes. This may result in a timeout. Refine your search criteria.', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    }
    else {
        customDialog({ url: ('../../Dialogues/Assessment/AssessmentPrintBubbleSheets.aspx?cteIDList=' + classTestEvents.toString() + '&assessmentCategory=' + assessmentCategory.toString()), autoSize: true });
    }
}

function searchAsssessmentAssignment_adminClick(assessmentID, classID, contentType, scoreType, isGroup, isKentico,isSecureAssessment,category) {
    var urlPrefix = '';
    if (isKentico) {
        urlPrefix = '../';
    }

    if (assessmentID == null || classID == null) {
        customDialog({ maximize: true, maxwidth: 300, maxheight: 100, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
						[{ title: 'Cancel' }]);
    }
    else {
        if (contentType == 'noItemContent') {
            var win = parent.customDialog({
                name: 'RadWindowAssessmentInputScores',
                url: urlPrefix + '../Dialogues/Assessment/AssessmentOfflineScores.aspx?assessmentID=' + assessmentID + '&classID=' + classID + '&scoreType=' + scoreType,
                title: 'Offline Test - 3rd Party Assessment on Fractions',
                maximize: true, maxwidth: 600, maxheight: 520
            });

            var win = $find('RadWindowAssessmentAssignmentSearch');
            if (win) setTimeout(function () { win.close() }, 0);
        }
        else {
            var winTitle = "Assessment Administration";
            if (isGroup == 'True') {
                winTitle = "Assessment Administration --GROUP";
            }
            if (isSecureAssessment.toUpperCase()=="TRUE")
                winTitle = SetSecureAssessmentTitle(winTitle);
            parent.parent.customDialog({ url: (urlPrefix + '../Dialogues/Assessment/AssessmentAdministration.aspx?xID=' + assessmentID.toString() + '&yID=' + classID.toString() + '&IsGroup=' + isGroup.toString() + '&IsSecure=' + isSecureAssessment + '&Category=' + category), title: winTitle, width: 1006, height: 730 });
        }
    }
}

function centerWindowsInManager() {
    var oManager = GetRadWindowManager();

    if (oManager == null) {
        return;
    }

    var windows = oManager.get_windows();
    for (var i = 0; i < windows.length; i++) {
        windows[i].center();
    }
}

function ajaxWCFService(options) {
    var defaults = {
        type: "POST", // GET or POST or PUT or DELETE verb
        url: null, // Location of the service
        data: null, // Data sent to server
        contentType: "application/json; charset=utf-8", // Content type sent to server
        dataType: "json", // Expected data format from server
        processdata: true, // True or False
        success: null,
        error: function (result) {
            var messageStart = '';
            var messageEnd = '';

            if (typeof (radalert) == "function") {
                messageStart = '<span style="color: red;">';
                messageEnd = '</span><hr />';
            }

            var errorStatus = result.status + ' (' + result.statusText + ')';
            var message = messageStart + this.type + ': ' + this.url + ' ' + errorStatus + messageEnd;

            if (this.data != null) {
                message += '<div style="height:400px; overflow: auto;">';
                message += '\ndata: ' + this.data;
                message += '</div>';
            }

            if (typeof (radalert) == "function") {
                var wnd = radalert(message, 600, 160, "Error " + errorStatus);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            } else {
                alert(message);
            }
        }
    };

    // merge @options with default settings
    options = $telerik.$.extend({}, defaults, (options) ? options : {});

    $.ajax({
        type: options.type,
        url: options.url,
        data: options.data,
        contentType: options.contentType,
        dataType: options.dataType,
        processdata: options.processdata,
        success: function (result) {
            if (typeof (window[options.success]) == "function") {
                window[options.success](result.d);
            }
        },
        error: options.error
    });
}

function viewAssessments_editLink_onClick(url, title) {
    if (url != null) {
        var urlPassed = url.split("xID=");
        url = urlPassed[0] + "xID=" + encodeURIComponent(urlPassed[1]);
    }
    customDialog({ url: url, minWidth: 1010, title: title, Name: "AssessmentPage", maximize: true });
}

function RadCombobox_CustomMultiSelectDropdown_StopPropagation(e) {
    //cancel bubbling
    e.cancelBubble = true;
    if (e.stopPropagation) {
        e.stopPropagation();
    }
}

var RadCombobox_CustomMultiSelectDropdown_reloadingTable = false;
function RadCombobox_CustomMultiSelectDropdown_evaluateCheckedItem(item, clientID, idKeyword) {
    var selectedDropdown = $find(clientID);
    var itemChecked = item.checked;
    var checkboxID = idKeyword + 'Checkbox';
    var labelID = idKeyword + 'Label';
    var itemValue = $get(item.id.replace(checkboxID, labelID)).innerHTML;
    var allItemsChecked = true;
    var itemsCheckedCount = 0;
    var checkbox;
    var labelValue;
    var items = selectedDropdown.get_items();

    if (itemChecked && itemValue == 'All Standards') {
        for (var i = 1; i < items.get_count() ; i++) {
            checkbox = $get(selectedDropdown.get_id() + '_i' + i + '_' + checkboxID);
            labelValue = $get(selectedDropdown.get_id() + '_i' + i + '_' + labelID).innerHTML;
            if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
            checkbox.checked = true;
        }

        selectedDropdown.set_text('All Standards');
    }
    else if ((itemValue == 'All Standards') && !itemChecked) {
        for (var i = 1; i < items.get_count() ; i++) {
            checkbox = $get(selectedDropdown.get_id() + '_i' + i + '_' + checkboxID);
            labelValue = $get(selectedDropdown.get_id() + '_i' + i + '_' + labelID).innerHTML;
            if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;
            checkbox.checked = false;
        }

        selectedDropdown.set_text('All Standards');
    }
    else {
        for (var i = 1; i < items.get_count() ; i++) {
            checkbox = $get(selectedDropdown.get_id() + '_i' + i + '_' + checkboxID);
            labelValue = $get(selectedDropdown.get_id() + '_i' + i + '_' + labelID).innerHTML;
            if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

            if (!checkbox.checked) {
                allItemsChecked = false;
            }
            else {
                itemsCheckedCount++;
            }
        }

        if (allItemsChecked) {
            checkbox = $get(selectedDropdown.get_id() + '_i0_' + checkboxID);
            checkbox.checked = true;
            selectedDropdown.set_text('All Standards');
        }
        else {
            checkbox = $get(selectedDropdown.get_id() + '_i0_' + checkboxID);
            checkbox.checked = false;

            switch (itemsCheckedCount) {
                case 0:
                    selectedDropdown.set_text('All Standards');
                    break;
                case 1:
                    for (var i = 1; i < items.get_count() ; i++) {
                        checkbox = $get(selectedDropdown.get_id() + '_i' + i + '_' + checkboxID);
                        labelValue = $get(selectedDropdown.get_id() + '_i' + i + '_' + labelID).innerHTML;
                        if (!checkbox || labelValue.toLowerCase().indexOf('<img') > -1) continue;

                        if (checkbox.checked) {
                            var labelValue = $get(checkbox.id.replace(checkboxID, labelID)).innerHTML;
                            selectedDropdown.set_text(labelValue);
                        }
                    }
                    break;
                default:
                    selectedDropdown.set_text('Multiple');
                    break;
            }
        }
    }

    if (itemValue.toLowerCase().indexOf('<img') > -1) {
        if (allItemsChecked || itemsCheckedCount > 0) {
            if (RadCombobox_CustomMultiSelectDropdown_reloadingTable) return false;
            RadCombobox_CustomMultiSelectDropdown_reloadingTable = true;
            __doPostBack(clientID);
        }
        else {
            //radalert('Please select an standard.', 300, 100);
        }
    }
}

function lessonPlans_OnclientSelectedIndexChanged(sender, args) {
    __doPostBack(sender.get_id(), '');
}

function OnCmsDocumentDialogClosed(sender) {
    window.__doPostBack();
}
function openCmsDialogWindow(wnd) {
    wnd.show();
}
var pResourceTitle = "";
var pTypeKey = '';

function openCmsDialogWindows(wnd, title, typekey, mydoc) {
    pResourceTitle = title;
    pTypeKy = typekey;
    if (mydoc != null && typeof mydoc != "undefined") {
        window.$find(mydoc).set_checked(true);
    }
    wnd.show();
}
function closeCmsDialogUsingCommandArgument(sender, arg) {

    
    var type = "#" + pResourceTitle.replace("btnOkType", "ddlType");
    var subtype = "#" + pResourceTitle.replace("btnOkType", "ddlSubType");
    var formtype = "#" + pResourceTitle.replace("btnOkType", "ddlFormType");

    if ($(type).length > 0) {

        if (pTypeKey == "84") {
            $(type).val("0");
            $(subtype).empty();
            $(subtype).append('<option value=0>-- Select --</option>');
        }
        $(subtype).val("0");
        $(formtype).empty();
        $('#divFormType').css('display', 'none');
    }

    var wnd = window.$find(arg.get_commandArgument());


    // get all the checked radbuttons
    var checkedRadios = $(".RadButton").filter(function (index) {
        return window.$find(this.id).get_checked();
    });

    // now find the selected radbutton (of type radio) whose name is 'where' and store the ID to a hidden input
    var checkedWhereRadio;
    var len = checkedRadios.length;
    for (var i = 0; i < len; i++) {
        if ($find(checkedRadios[i].id)._groupName == 'CreateWhere') {
            $find(checkedRadios[i].id).set_checked(false);
            if (checkedRadios[i].id == 'ctl00_MainContent_ctlDoublePanel_rotator1_container5_tileContainerDiv3_C_ctl00_wndAddDocument_C_rdoMyDocuments') {
                $find(checkedRadios[i].id).set_checked(true);
                break;
            }
        }
    }
    wnd.close();
}
function closeCmsDialog(wnd) {
    wnd.close();
}

function OnClientBeforeClose(sender, args) {
    args.set_cancel(true);
    function confirmCallback(arg) {
        if (arg) {
            sender.remove_beforeClose(OnClientBeforeClose);
            sender.close();
            sender.add_beforeClose(OnClientBeforeClose);
        }
    }
    radconfirm("Are you sure you want to close this window?", confirmCallback);
}

function closeCmsDialogDoPostBack(wnd) {
    window.location.href = window.location.href;
    wnd.close();
}

function validateTypeSubtye(hdnCmsDocumentLocation, wnd) {

    var type = "#" + pResourceTitle.replace("btnOkType", "ddlType");
    var subtype = "#" + pResourceTitle.replace("btnOkType", "ddlSubType");
    var formtype = "#" + pResourceTitle.replace("btnOkType", "ddlFormType");

    if ($(type).length > 0 && $(subtype).length > 0) {

        var subtypevalue = $(subtype).val();
        if ($("#hdnSubTypes").length > 0) {
            $("#hdnSubTypes").val(subtypevalue);
        }

        var formtypevalue = $(formtype).val();
        $("#hdnFormTypes").val(formtypevalue);


        if ($(type).val() == "0") {
            alert("Please Select Type and SubType.");
        }
        else if ($(subtype).val() == "0") {
            alert("Please Select SubType.");
        }
        else {
            triggerBtnOkTypeClick(hdnCmsDocumentLocation, wnd);
        }

    }

}

function triggerBtnOkTypeClick(hdnCmsDocumentLocation, wnd) {

    setCmsDocumentLocation(hdnCmsDocumentLocation, wnd);

    var okBtn = "#" + pResourceTitle.replace("btnOkType", "hdnBtnOkType");
    $(okBtn).click();

}


function setCmsDocumentLocations(hdnCmsDocumentLocation, wnd, divToHide, divToShow) {

    document.getElementById(divToHide).style.display = 'none';
    document.getElementById(divToShow).style.display = 'block';

    // get all the checked radbuttons
    var checkedRadios = $(".RadButton").filter(function (index) {
        return window.$find(this.id).get_checked();
    });

    // now find the selected radbutton (of type radio) whose name is 'where' and store the ID to a hidden input
    var checkedWhereRadio;
    var len = checkedRadios.length;
    for (var i = 0; i < len; i++) {
        if ($find(checkedRadios[i].id)._groupName == 'CreateWhere') {
            checkedWhereRadio = checkedRadios[i];
            var hdn = wnd.BrowserWindow.WebForm_GetElementById(hdnCmsDocumentLocation);
            hdn.value = window.$find(checkedWhereRadio.id).get_commandArgument();
            break;
        }
    }
}



function setupCmsDocumentLocations(hdnCmsDocumentLocation, wnd, divToHide, divToShow, SharedClientID, divtoshowshared, AddExistingradio, AddNewRadio, skipTypeSubTypeDialog) {
    var sharedval = $find(SharedClientID);

    var addRb = $find(AddNewRadio);
    addRb.set_checked(true);

    if (sharedval != null) {
        if (sharedval.get_checked() == true) {
            var Existingrb = $find(AddExistingradio);
            Existingrb.set_checked(true);
            if (skipTypeSubTypeDialog.toLowerCase() == 'true')
                triggerBtnOkTypeClick(hdnCmsDocumentLocation, wnd);
            else
            setCmsDocumentLocations(hdnCmsDocumentLocation, wnd, divToHide, divtoshowshared);
        }

        else {
            setCmsDocumentLocations(hdnCmsDocumentLocation, wnd, divToHide, divToShow);
        }
    }
    else
    {
        setCmsDocumentLocations(hdnCmsDocumentLocation, wnd, divToHide, divToShow);
}
}

function setCmsDocumentLocationShared(hdnCmsDocumentLocation, wnd, defaultSelectionID, title, divToHide, divToShow, SharedClientID, divtoshowshared) {
    var sharedval = $find(SharedClientID);
    var sharedState = false;
    if (sharedval != null) {
        sharedState = sharedval.get_checked();
    }
    setCmsDocumentLocation(hdnCmsDocumentLocation, wnd, defaultSelectionID, title);
    document.getElementById(divToHide).style.display = 'none';
    if (sharedState == true) {
        document.getElementById(divtoshowshared).style.display = 'block';
    }
    else {
        document.getElementById(divToShow).style.display = 'block';
    }
}

function setCmsDocumentLocation(hdnCmsDocumentLocation, wnd, defaultSelectionID, title) {

    // get all the checked radbuttons
    var checkedRadios = $(".RadButton").filter(function (index) {
        return window.$find(this.id).get_checked();
    });

    if (defaultSelectionID != null && typeof defaultSelectionID != "undefined") {

        var subtype = "#" + pResourceTitle.replace("btnOkType", "ddlSubType");
        $(subtype).val("0");
        
        var ddltype = "#" + pResourceTitle.replace("btnOkType", "ddlType");
        if (title == 'Resource')
        {
            $(ddltype).val("0");
            $(subtype).empty();
            var newOption = $('<option value="-- Select --">-- Select --</option>');
            $(subtype).append(newOption);
        }       

        $('#divFormType').css('display', 'none');       
        

        // now find the selected radbutton (of type radio) whose name is 'where' and store the ID to a hidden input
        var checkedWhereRadio;
        var len = checkedRadios.length;
        for (var i = 0; i < len; i++) {
            if ($find(checkedRadios[i].id)._groupName == 'CreateWhere') {
                if (checkedRadios[i].id != defaultSelectionID) {
                    $find(checkedRadios[i].id).set_checked(false);
                    $find(defaultSelectionID).set_checked(true);
                    var hdn = wnd.BrowserWindow.WebForm_GetElementById(hdnCmsDocumentLocation);
                    hdn.value = window.$find(defaultSelectionID).get_commandArgument();
                    break;
                }
            }
        }
    } else {
    // now find the selected radbutton (of type radio) whose name is 'where' and store the ID to a hidden input
    var checkedWhereRadio;
    var len = checkedRadios.length;
    for (var i = 0; i < len; i++) {
        if ($find(checkedRadios[i].id)._groupName == 'CreateWhere') {
            checkedWhereRadio = checkedRadios[i];
            var hdn = wnd.BrowserWindow.WebForm_GetElementById(hdnCmsDocumentLocation);
            hdn.value = window.$find(checkedWhereRadio.id).get_commandArgument();
            break;
        }
    }
}
}


function OpenLegacyRTISearch(sender, args) {
    var newWin = window.open(args.command.get_name(), '_blank');
    newWin.focus();
}

function assessmentItemsExportToExcel(sender, args) {
    var excelButton = document.getElementById("exportToExcelIcon");
    if (excelButton != null) {
        excelButton.click();
    }
}

function generateReport(sender, args) {
    customDialog({ url: args.command.get_name(), maximize: true, maxwidth: 350, maxheight: 300 });
}

function SendResultsToWindow(imagePath) {
    //get the InsertImage function in the parent dialogue window depanding on the dialogue name. Name of the dialogue window will be passed as the URL parm NewAndReturnType.
    var DialogueToSendResultsTo;
    DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(getURLParm('NewAndReturnType'));

    if (typeof imagePath == "undefined")
        imagePath = document.getElementById('ImageSrc').innerHTML;

    try {
        if (DialogueToSendResultsTo != null) {
            DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertImage(imagePath);
        }
        else {
            var parentDialogue = getURLParm('NewAndReturnType') == "ContentEditorITEM" ? parent.ContentEditorITEM : parent.ContentEditorADDENDUM;
            if (typeof parentDialogue != "undefined") {
                parentDialogue.InsertImage(imagePath);
            }
            else {
                parent.InsertImage(imagePath);
            }
        }
    }
    catch (e) {
        alert('Error inserting Image');
    }
    closeCurrentCustomDialog();
}