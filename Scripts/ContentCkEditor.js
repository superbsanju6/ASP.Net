var existingContent;
var editedContent;
var currentEditableBody;
var questionInstance;
var itemID;
var undoList;



$(document).ready(function () {

    var contentEditableList = $(".SKEditableBodyText[contenteditable='true']");
    var numberCkEditorNeeded = contentEditableList.length;
    var contentEditableIdArray = [];
    if (numberCkEditorNeeded > 0) {
        for (var i = 0; i < numberCkEditorNeeded ; i++) {
            if (contentEditableIdArray.indexOf(contentEditableList[i].id) == -1) {
                contentEditableIdArray[i] = contentEditableList[i].id;
                CKEDITOR.inline(contentEditableList[i].id);
            }
        }
    }

    CKEDITOR.on('instanceReady', function(event) {
        //var math = MathJax.Hub.getAllJax(id)[0];

        event.editor.on('focus', function() {
            //math.Remove();
            existingContent = this.getData();
            setCursorPos(this);
        });

        event.editor.on('onkeyup', function() {
            setCursorPos(this);
        });

        event.editor.on('blur', function () {            
            if (this.checkDirty()) {
                editedContent = this.getData();
                if (existingContent == editedContent) return;

                if (editedContent.length > 2147483647 && this.name == "AdminAssessmentDirectionsEditor") {
                    alert("The Administration Instructions entered exceeds the maximum value length.");
                    return false;
                }
                currentEditableBody = $(this.element.$).parent();
                questionInstance = currentEditableBody.closest(".questionInstance");
                itemID = questionInstance.attr("id");

                //External Editor doesn't set
                if (undoList !== undefined) {
                    undoList.Add(UNDO_TYPE.QUESTION_TEXT, itemID, currentEditableBody.attr("update"), existingContent, editedContent);
                }
                if (this.name == "AssessmentDirectionsEditor" || this.name == "AdminAssessmentDirectionsEditor") {
                    if (this.name == "AssessmentDirectionsEditor")
                    { assessment_changeField("Directions", editedContent); }
                    if (this.name == "AdminAssessmentDirectionsEditor") {
                        assessment_changeField("AdministrationDirections", editedContent);
                    }

                }
                else {
                    assessmentitem_changeField(currentEditableBody, 'Item_Text', editedContent);
                }                
            }
        });

    });
});