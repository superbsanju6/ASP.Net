/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

'use strict';

CKEDITOR.dialog.add( 'mathjax', function( editor ) {

	var preview,
		lang = editor.lang.mathjax;

	return {
		title: lang.title,
		minWidth: 350,
		minHeight: 100,
		contents: [
			{
				id: 'info',
				elements: [
					{
						id: 'equation',
						type: 'textarea',
						label: lang.dialogInput,

						onLoad: function( widget ) {
							var that = this;

							if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 7 ) ) {
								this.getInputElement().on( 'keyup', function() {
									// Add \( and \) for preview.
									preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
								} );
							}
						},

						setup: function( widget ) {
							// Remove \( and \).
							this.setValue( CKEDITOR.plugins.mathjax.trim( widget.data.math ) );
						},

						commit: function( widget ) {
							// Add \( and \) to make TeX be parsed by MathJax by default.
							widget.setData('math', '\\(' + this.getValue() + '\\)');
							
							//IE8 needs a little push to properly update font size and color ...
							//if ((CKEDITOR.env.ie && CKEDITOR.env.version == 8)) {
								//arguments[0].editor.undoManager.undo();
								//arguments[0].editor.undoManager.redo();
							//}
						}
					},
					{
						id: 'documentation',
						type: 'html',
						html:
							'<div style="width:100%;text-align:right;margin:-8px 0 10px">' +
								'<a class="cke_mathjax_doc" href="' + editor.config.equationEditorURL + 'help.cshtml" target="_black" style="cursor:pointer;color:#00B2CE;text-decoration:underline">' +
									lang.docLabel +
								'</a>' +
							'</div>'
					},
					( !( CKEDITOR.env.ie && CKEDITOR.env.version == 7 ) ) && {
						id: 'preview',
						type: 'html',
						html:
							'<div style="max-width:600px;max-height:200px;text-align:center;overflow:auto;">' +
								'<iframe style="border:0;width:0;height:0;font-size:16px" scrolling="no" frameborder="0" allowTransparency="true" src="' + CKEDITOR.plugins.mathjax.fixSrc + '"></iframe>' +
							'</div>',

						onLoad: function( widget ) {
							var iFrame = CKEDITOR.document.getById( this.domId ).getChild( 0 );
							preview = new CKEDITOR.plugins.mathjax.frameWrapper( iFrame, editor );
						},

						setup: function( widget ) {
							preview.setValue( widget.data.math );
						}
					}
				]
			}
		]
	};
} );