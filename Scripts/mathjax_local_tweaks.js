// Be careful with the format of this configuration.
// The online example has two extra commas that prevent
// it from working with MSIE.
MathJax.Hub.Config({
extensions: ["tex2jax.js","TeX/AMSmath.js","TeX/AMSsymbols.js"],
jax: ["input/TeX","output/HTML-CSS"],
"HTML-CSS": {
availableFonts:[]
},
tex2jax: { inlineMath: [['$','$'],["\\(","\\)"]], processEscapes: true },
imageFont: null
});
MathJax.Ajax.loadComplete("/javascript/mathjax_local_tweaks.js");
