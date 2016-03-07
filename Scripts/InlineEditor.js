var contentEditableList;
var contentEditableIdArray = [];
var numberCkEditorNeeded;
var initialCkEditorNumber = 18;
var numberCkEditorScrollTo;
var editorInstances;
var existingContent;
var editedContent;
var currentEditableBody;
var questionInstance;
var itemID;
var undoList;

var createInitialCkeditor = function () {
    if (AssessmentIsProofed) return;
    CKEDITOR.inline("AssessmentDirectionsEditor", { toolbar: 'assessmentDirectionsEditor' });
    CKEDITOR.inline("AdminAssessmentDirectionsEditor", { toolbar: 'assessmentDirectionsEditor' }); /* for administration directions,tool bar will be same in both instructions*/
    contentEditableList = $(".SKEditableBodyText[contenteditable='true']");
    numberCkEditorNeeded = contentEditableList.length;
    if (numberCkEditorNeeded > 0) {
        for (var i = 0; i < (numberCkEditorNeeded < initialCkEditorNumber ? numberCkEditorNeeded : initialCkEditorNumber) ; i++) {
            if (contentEditableIdArray.indexOf(contentEditableList[i].id) == -1) {
                CKEDITOR.inline(contentEditableList[i].id, { toolbar: 'assessmentEditor' });
                contentEditableIdArray[i] = contentEditableList[i].id;
            }
        }
    }
};

function createCkeditorOnScroll() {
    if (contentEditableList) {
        if (numberCkEditorNeeded > initialCkEditorNumber) {
            numberCkEditorScrollTo = initialCkEditorNumber + 6;
            for (var j = initialCkEditorNumber; j < (numberCkEditorNeeded < numberCkEditorScrollTo ? numberCkEditorNeeded : numberCkEditorScrollTo) ; j++) {
                if (contentEditableIdArray.indexOf(contentEditableList[j].id) == -1) {
                    CKEDITOR.inline(contentEditableList[j].id, { toolbar: 'assessmentEditor' });
                    contentEditableIdArray[j] = contentEditableList[j].id;
                }
            }
            initialCkEditorNumber += 6;
        } else {
            return;
        }
    }
};

var textOnClose = '';
$(document).ready(function () {
    var controlName = '';
    if (AssessmentIsProofed) {
        if (parent) {
            var win = getCurrentCustomDialog();
            if (parent.onClientBeforeClose) {
                win.remove_beforeClose(parent.onClientBeforeClose);
            }
        }
    }

    createInitialCkeditor(AssessmentIsProofed);



    CKEDITOR.on('instanceReady', function (event) {

        event.editor.on('focus', function () {
            this.focusManager.focus();
            currentEditor = this;
            currentEditableBody = $(this.element.$).parent();
            existingContent = this.getData();
        });

        event.editor.on('change', function () {
            textOnClose = this.getData();
            controlName = this.name;
        });

        if (parent) {
            var win = getCurrentCustomDialog();
            if (parent.onClientBeforeClose) {
                win.remove_beforeClose(parent.onClientBeforeClose);
            }
            parent.onClientBeforeClose = localClientBeforeClose;
            win.add_beforeClose(parent.onClientBeforeClose);
        }

        function localClientBeforeClose(sender, arg) {
         
            if (textOnClose != '' && textOnClose != 'undefined') {
                if (controlName == "AssessmentDirectionsEditor" || controlName == "AdminAssessmentDirectionsEditor") {

                    try {
                        var htmlObject = $(textOnClose);
                        textOnClose = "<span>" + textOnClose + "</span>";
                        htmlObject = $(textOnClose);
                    }
                    catch (err) {
                    }
                    var content = "";
                    if (htmlObject != undefined || htmlObject != null) {
                        if (htmlObject.find('a').length > 0) {
                            htmlObject.find('a').replaceWith(function () { return this.childNodes; });
                            content = htmlObject.prop('outerHTML');
                        }
                        if (htmlObject.find('font').length > 0) {
                            htmlObject.find('font').replaceWith(function () { return this.childNodes; });
                            content = htmlObject.prop('outerHTML');
                        }
                    }
                    if (content != "") {
                        textOnClose = content;
                    }
                    if (controlName == "AssessmentDirectionsEditor") {
                        assessment_changeField("Directions", textOnClose);


                    }
                    if (controlName == "AdminAssessmentDirectionsEditor") {
                        assessment_changeField("AdministrationDirections", textOnClose);

                    }
                }
                else {
                    assessmentitem_changeField(currentEditableBody, 'Item_Text', textOnClose);
                }
                    arg.set_cancel(true);
                    isServiceComplete();
                }
             
               

               
        }

        function isServiceComplete() {
            setInterval(function () {
                if (isServiceCalled == true) {
                    win.remove_beforeClose(parent.onClientBeforeClose);
                    win.close();
                    setTimeout(isServiceComplete, 0)
                }
                else {
                    isServiceComplete();
            }
            }, 1000);
        }

        event.editor.on('blur', function () {
           // debugger;
            if (this.checkDirty()) 
            {
                editedContent = this.getData();

                if (this.name == "AdminAssessmentDirectionsEditor")
                {
                    if ($('#AdminAssessmentDirectionsEditor').text().length > 8000) {
                        alert("The Administration Instructions entered exceeds the maximum value length.");
                        return false;
                    }
                }
                if (existingContent == editedContent) return;
                currentEditableBody = $(this.element.$).parent();
                questionInstance = currentEditableBody.closest(".questionInstance");
                itemID = questionInstance.attr("id");

                //External Editor doesn't set
                if (typeof undoList !== "undefined") {
                    undoList.Add(UNDO_TYPE.QUESTION_TEXT, itemID, currentEditableBody.attr("update"), existingContent, editedContent);
                }
                if (this.name == "AssessmentDirectionsEditor" || this.name == "AdminAssessmentDirectionsEditor")
                {
                    try{
                        var htmlObject = $(editedContent);
                        editedContent = "<span>" + editedContent + "</span>";
                        htmlObject = $(editedContent);
                    }
                    catch(err) {
                    }
                    var content = "";
                    if (htmlObject != undefined || htmlObject != null) {
                        if (htmlObject.find('a').length > 0) {
                            htmlObject.find('a').replaceWith(function () { return this.childNodes; });
                            content = htmlObject.prop('outerHTML');
                        }
                        if (htmlObject.find('font').length > 0) {
                            htmlObject.find('font').replaceWith(function () { return this.childNodes; });
                            content = htmlObject.prop('outerHTML');
                        }
                    }
                    if (content != "" )
                    {
                        editedContent = content;
                    }
                    if (this.name == "AssessmentDirectionsEditor")
                    {
                        assessment_changeField("Directions", editedContent);
                      
                    }
                    if (this.name == "AdminAssessmentDirectionsEditor")
                    {
                        assessment_changeField("AdministrationDirections", editedContent);
                                           
                    }                    
                }
                else {
                    assessmentitem_changeField(currentEditableBody, 'Item_Text', editedContent);
                }                
            }
            
        });

      

        $("#QuestionHolder").scroll(function () {
            editorInstances = CKEDITOR.instances;
            if (editorInstances) {
                event.editor.focusManager.blur();
            }
        });
    });

});