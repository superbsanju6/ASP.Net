/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

	config.allowedContent = true;
	config.skin = 'thinkgate';
	config.toolbarGroups = [
		        { name: 'insert' },
		        { name: 'clipboard', groups: ['clipboard', 'undo'] },
		        { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },

		        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		        { name: 'tools' },

		        { name: 'paragraph', groups: ['list', 'indent', 'align'] },
		        { name: 'styles', groups: ['Font', 'FontSize'] },
		        { name: 'colors' },
	            { name: 'document', groups: ['mode'] }
	];

	config.toolbar_assessmentDirectionsEditor = [
        ['Bold', 'Italic', 'Underline', 'Strike'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
        ['Superscript', 'Subscript'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent'],
        ['Undo', 'Cut']
	];

	config.toolbar_assessmentEditor = [
        ['tgmath', 'Mathjax'],
        ['Bold', 'Italic', 'Underline', 'Strike'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
        ['Superscript', 'Subscript'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent'],
        ['Undo', 'Cut', 'Sourcedialog']
	];

	config.toolbar_rubricEditor = [
         { name: 'editing', items: ['SpellChecker'] },
         { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'NumberedList', 'BulletedList'] }
	];

	config.toolbar_assessmentCoverPageEditor = [
        ['Font', 'FontSize'],
        ['Bold', 'Italic', 'Underline'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
        ['TextColor', 'BGColor'],
        ['Undo', 'Redo'],
        ['Cut', 'Copy', 'Paste', 'SpecialChar'],
        ['SpellChecker']
	];

	config.removeButtons = 'Styles,Image,Flash,Smiley,Iframe,PageBreak,ShowBlocks,Format,JustifyBlock,NewPage,Save,Print,Source,Preview';

	// Make dialogs simpler.
	config.removeDialogTabs = 'image:advanced;link:advanced';

	// If set to zero, the TAB key will be used to move the cursor focus to the next element in the page, out of the editor focus
	config.tabSpaces = 4;

	config.stylesSet = 'tgStylesSet';

	config.image_previewText = ' ';

	config.fontSize_sizes = '8/8px;9/9px;10/10px;11/11px;12/12px;14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;36/36px';

	config.plugins =
		'basicstyles,' +
		'bidi,' +
		'blockquote,' +
		'clipboard,' +
		'colorbutton,' +
		'colordialog,' +
		'contextmenu,' +
		'dialogadvtab,' +
		'div,' +
		'enterkey,' +
		'entities,' +
		'filebrowser,' +
		'find,' +
		'flash,' +
		'floatingspace,' +
		'font,' +
		'format,' +
		'forms,' +
		'horizontalrule,' +
		'htmlwriter,' +
		'iframe,' +
		'indentlist,' +
		'indentblock,' +
		'justify,' +
		'language,' +
		'link,' +
		'list,' +
		'liststyle,' +
		'mathjax,' +
		'maximize,' +
		'newpage,' +
		'pagebreak,' +
		'pastefromword,' +
		'pastetext,' +
		'preview,' +
		'print,' +
		'removeformat,' +
		'resize,' +
		'save,' +
		'selectall,' +
		'showblocks,' +
		'showborders,' +
		'smiley,' +
		'sourcearea,' +
		'sourcedialog,' +
		'specialchar,' +
		'stylescombo,' +
		'tab,' +
		'table,' +
		'tabletools,' +
		'templates,' +
		'tgmath,' +
		'toolbar,' +
		'undo,' +
		'wysiwygarea';


	config.equationEditorURL = GetEquationEditorURL();
	config.disableAutoInline = true;
};

//StylesSet config
CKEDITOR.stylesSet.add('tgStylesSet', [
    { name: 'Blue Title', element: 'h3', styles: { color: 'Blue' } },
    { name: 'Red Title', element: 'h3', styles: { color: 'Red' } },
    { name: 'Big', element: 'big' },
    { name: 'Small', element: 'small' },
    { name: 'Typewriter', element: 'tt' },
    { name: 'Computer Code', element: 'code' },
    { name: 'Keyboard Phrase', element: 'kbd' },
    { name: 'Sample Text', element: 'samp' },
    { name: 'Variable', element: 'var' },
    { name: 'Deleted Text', element: 'del' },
    { name: 'Inserted Text', element: 'ins' },
    { name: 'Cited Work', element: 'cite' },
    { name: 'Inline Quotation', element: 'q' },
    { name: 'Borderless Table', element: 'table', styles: { 'border-style': 'hidden', 'background-color': '#E6E6FA' } },
    { name: 'Square Bulleted List', element: 'ul', styles: { 'list-style-type': 'square' } }
]);