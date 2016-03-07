// get path of directory ckeditor
var basePath = CKEDITOR.basePath;
basePath = basePath.substr(0, basePath.indexOf("ckeditor/"));   

// load external plugin
(function() {
   CKEDITOR.plugins.addExternal('tgmath',basePath+'myplugins/tgmath/', 'plugin.js');
})();

// config for toolbar, extraPlugins,...
CKEDITOR.editorConfig = function( config )
{
   config.extraPlugins = 'tgmath';
   config.allowedContent = true;
   
	// The toolbar groups arrangement, optimized for two toolbar rows.
   config.toolbarGroups = [
	   { name: 'clipboard', groups: ['clipboard', 'undo'] },
	   { name: 'editing', groups: ['spellchecker'] }, 
	   { name: 'basicstyles', groups: ['basicstyles'] },
	   { name: 'paragraph', groups: ['list', 'indent', 'align'] },
	   { name: 'styles' },
	   { name: 'colors' },
	   { name: 'others'}
   ];

	// Remove some buttons, provided by the standard plugins, which we don't
	// need to have in the Standard(s) toolbar.
   config.removeButtons = 'Underline,Subscript,Superscript';

	// Set the most common block elements.
   config.format_tags = 'p;h1;h2;h3;pre';

	// Make dialogs simpler.
   config.removeDialogTabs = 'image:advanced;link:advanced';
};