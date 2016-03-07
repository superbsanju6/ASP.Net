
var iframeWindow;
var getIframeContents;

CKEDITOR.plugins.add('tgmath',
   {
	  requires : ['iframedialog'],
	  init : function(editor) {
		 var pluginName = 'tgmath';
		 var mypath = this.path;
		 alert(mypath);
		 editor.ui.addButton(
			'tgmath.btn',
			{
			   label : "TGMath Plug-in",
			   command : 'tgmath.cmd',
			   icon : 'tgmath.gif'
			}
		 );
		 var cmd = editor.addCommand('tgmath.cmd', {exec:showDialogPlugin});
		 CKEDITOR.dialog.addIframe(
			'tgmath.dlg',
			'Math Editor',
			'http://localhost/MathEquationEditor/',
			960,
			400,
			function()
				   {
						// Iframe loaded callback.
						var iframe = document.getElementById(this._.frameId);
						iframeWindow = iframe.contentWindow;
				   },
 
				   {
						onOk : function(e)
						{
							try {
								var val = iframeWindow.myta.value; // calls frameVar from iframe
								e.sender._.editor.insertHtml("$$" + unescape(val) + "$$");
								MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
							} catch ( ex) {
								alert("onOk Exception: " + ex);
							}
							//var iframeid = e.sender._.contents.iframe.undefined.domId;
							//var val = document.getElementById(iframeid).contentWindow.myta.value;
							//this._.editor.insertHtml("$$" + unescape(val) + "$$");
							
						}
				   }
		 );
	  }
   }
);

function showDialogPlugin(e){
   e.openDialog('tgmath.dlg');
}