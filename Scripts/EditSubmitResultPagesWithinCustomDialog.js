/************************************************************************
It is assumed that this page will perform postbacks.  So the function 
below will be executed upon initial creation and also upon those 
occasions when we return from the postback (i.e. when user has 
clicked "add" button after providing necessary data to add a record).

The addResultPanel only exists (i.e. Visible="true") if user has 
clicked the "add" button. Therefore, we only want to add confirmation 
to the customDialog's close button if the addResultPanel isn't 
displayed yet.
***********************************************************************/

function addWindowBeforeCloseEvent() {
    var addResultPanel = $('#resultPanel')[0];
    if (!addResultPanel) {
        getCurrentCustomDialog().add_beforeClose(onClientBeforeClose);
    }
}

function refreshParentWindow() {
    parent.window.location.reload();
}


/*********************************************************************************************************************
The following js methods are used in cases where one sets up, within a custom dialog window (see customDialog() in 
master.js) an aspx for editing objects, validating, submitting to codebehind in order to save. Examples include:
ChgTeacherPasswordAdmin.aspx
Any aspx pages that use AddNew.Master

*********************************************************************************************************************/



function autoSizeWindow() {
    getCurrentCustomDialog().set_autoSizeBehaviors(Telerik.Web.UI.WindowAutoSizeBehaviors.Height);
    getCurrentCustomDialog().autoSize(false);
    getCurrentCustomDialog().center();
}

function closeWindow() {
    setTimeout(function () {
        getCurrentCustomDialog().close();
    }, 0);
}

function isStringNullOrEmpty(str) {
    if (str == null || str == "") {
        return true;
    }

    return false;
}

function onClientBeforeClose(sender, arg) {

    function confirmCallback(arg) {
        if (arg == true) {
            sender.remove_beforeClose(onClientBeforeClose);

            // Have to do this for IE since it doesn't dispose IFrames properly.
            setTimeout(function () {
                sender.close();
            }, 0);
        }
    }

    if (userHasAddedData && userHasAddedData()) {
        arg.set_cancel(true);
        var wnd = radconfirm('Are you sure you want to cancel?', confirmCallback, 330, 100, null, 'Cancel Action');
        wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
    } else {
        confirmCallback(true);
    }
}


