MathJax.Hub.Config({
    extensions: ["tex2jax.js"],
    jax: ["input/TeX", "output/HTML-CSS"],
    tex2jax: {
        inlineMath: [['$', '$'], ['\(', '\)'], ['\\(', '\\)'], ['\\[', '\\]']],
        displayMath:[['$$', '$$'], ['\(', '\)'], ['\\(', '\\)'], ['\\[', '\\]']],
        processEscapes: true
    },
    "HTML-CSS": {
        preferredFont: "TeX",
        availableFonts: ["STIX", "TeX"],
        minScaleAdjust: 100,
        EqnChunk: 50,
        matchFontHeight: true,
        undefinedFamily: "STIXGeneral, 'Arial Unicode MS', serif",
        webFont: "TeX"
    }
});