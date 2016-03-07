/* Load this script using conditional IE comments if you need to support IE 7 and IE 6. */

window.onload = function() {
	function addIcon(el, entity) {
		var html = el.innerHTML;
		el.innerHTML = '<span style="font-family: \'icomoon\'">' + entity + '</span>' + html;
	}
	var icons = {
			'icon-undo' : '&#xe000;',
			'icon-redo' : '&#xe001;',
			'icon-checkmark' : '&#xe002;',
			'icon-checkmark-2' : '&#xe003;',
			'icon-close' : '&#xe004;',
			'icon-cancel-circle' : '&#xe005;',
			'icon-blocked' : '&#xe006;',
			'icon-erase' : '&#xe007;',
			'icon-eraser' : '&#xf12d;'
		},
		els = document.getElementsByTagName('*'),
		i, attr, c, el;
	for (i = 0; ; i += 1) {
		el = els[i];
		if(!el) {
			break;
		}
		attr = el.getAttribute('data-icon');
		if (attr) {
			addIcon(el, attr);
		}
		c = el.className;
		c = c.match(/icon-[^\s'"]+/);
		if (c && icons[c[0]]) {
			addIcon(el, icons[c[0]]);
		}
	}
};