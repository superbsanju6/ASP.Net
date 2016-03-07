var Helper = function() {
    var block = function(selector, text, callback) {
            $(selector).block({
                message: "<img src='" + "../../Images/Loading.gif' align='absmiddle'>&nbsp;" + text,
                onBlock: callback,
                centerX: true,
                centerY: true,
                css: {
                    border: '3px solid #cccccc',
                    padding: "7px",
                    width: "auto"
                },
                blockMsgClass: "ui-corner-all"
            });

            //position the blockUI message to always be in the center of the screen
            //regardless of how tall the page content is
            var yPosition = ($(window).height() / 2);
            $(".blockElement").css("top", yPosition);
        },
        unblock = function(selector) {
            $(selector).unblock();
        };

    return {
        "Block": block,
        "Unblock": unblock,

    };
}();