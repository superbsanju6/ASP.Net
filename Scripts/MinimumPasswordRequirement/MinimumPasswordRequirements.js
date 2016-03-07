var PasswordRequirement = function () {
    showDialog = function (html) {
        $("<div/>").dialog({
            modal: true,
            width: 600,
            height: 185,
            resizable: false,
            title: "Minimum Password Requirements"
        }).html(html);
    },
    showDialogWithPosition = function (html) {
        $("<div/>").dialog({
            modal: true,
            width: 600,
            height: 185,
            position: [100, 150],
            resizable: false,
            title: "Minimum Password Requirements"
        }).html(html);
    };
    return {
        "ShowDialog": showDialog,
        "ShowDialogWithPosition": showDialogWithPosition
    };
}();