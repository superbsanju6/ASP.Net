var iframeWindow;
var getIframeContents;
var parentEditor = '';
CKEDITOR.plugins.add('tgmath',
   {
   	requires: ['iframedialog'],
   	icons: 'tgmath',
   	hidpi: true,
   	init: function (editor) {
   		var pluginName = 'tgmath';
   		var mypath = this.path;

   		editor.ui.addButton(
		   'tgmath',
		   {
		   	label: "Advanced Content Editor",
		   	command: 'tgmath.cmd'
		   }
		);
   		var cmd = editor.addCommand('tgmath.cmd', { exec: showDialogPlugin });
   		CKEDITOR.dialog.addIframe(
		   'tgmath.dlg',
		   'Advanced Content Editor',
		   editor.config.equationEditorURL,
		   960,
		   500,
		   function () {
		   	// Iframe loaded callback.
		   	var iframe = document.getElementById(this._.frameId);
		   	iframeWindow = iframe.contentWindow;
		   	//var initdata = parentEditor.getData();
			//alert(initdata);
		   	iframeWindow.CKEDITOR.instances.myeditor.setData(parentEditor.getData());
		   },
				  {
				  	onLoad: function (widget) {
				  		var that = this;
				  		parentEditor = this._.editor;
				  		//alert(parentEditor);
				  	},
				  	onShow: function (widget) {
				  		parentEditor = this._.editor;
				  		//alert("parentEditor set");
				  	},
				  	onOk: function (e) {
				  		try {
				  			var val = iframeWindow.CKEDITOR.instances.myeditor.getData();
				  			e.sender._.editor.setData(unescape(val));
				  			if (MathJax) {
				  				MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
				  			}
				  		} catch (ex) {
				  			//alert("onOk Exception: " + ex);
				  		}
				  	}
				  }
		);
   	}
   }
);

function showDialogPlugin(e) {
	e.openDialog('tgmath.dlg');
}