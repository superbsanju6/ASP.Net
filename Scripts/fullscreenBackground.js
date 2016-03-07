function fullscreenBackground(element) {
            var bgimg = document.getElementById(element);

            if (bgimg == null) return false;

            var realBody = document.getElementById('realBody');

            var initialHeight = document.documentElement.clientHeight;
            var initialWidth = document.documentElement.clientWidth;

            if (document.compatMode == 'CSS1Compat') {
                if (document.documentElement.clientHeight < realBody.offsetHeight) {
                    bgimg.height = realBody.offsetHeight;
                }
                else {
                    bgimg.height = document.documentElement.clientHeight;
                }
                bgimg.width = document.documentElement.clientWidth;

            }
            else {
                bgimg.style.width = document.body.clientWidth;
                bgimg.style.height = document.body.clientHeight;
            }


            if (document.compatMode == 'CSS1Compat') {
                var finalHeight = document.documentElement.clientHeight;
                var finalWidth = document.documentElement.clientWidth;

                if (finalWidth > bgimg.width) {
                    bgimg.width = finalWidth;
                }

                if ((finalHeight > initialHeight) && (document.documentElement.clientHeight > realBody.offsetHeight)) {
                    bgimg.height = document.documentElement.clientHeight;

                }

                bgimg.style.visibility = "visible";

            }
			}