(function ($) {


    var iOS = navigator.userAgent.match(/iPod|iPad|iPhone/i);
    if (!iOS)
        $(document.documentElement).addClass('RadMenu_Desktop');
    else
        $(document.documentElement).addClass('RadMenu_iOS');
    var lastElement;

    var getEventLocation = function (e) {
        var location = { x: e.pageX, y: e.pageY };

        if (iOS && e.originalEvent.changedTouches.length < 2) {
            location.x = e.originalEvent.changedTouches[0].pageX;
            location.y = e.originalEvent.changedTouches[0].pageY;
        }

        return location;
    };

    var cancelEvents = function (e) {
        e.stopPropagation();
        e.preventDefault();
    };

    // enforces context over function
    var bind = function (context, name) {
        return function (e) {
            if (e && e.preventDefault)
                e.preventDefault();

            return context[name].apply(context, arguments);
        };
    };

    // enforces "top" coordinate of element to be within container
    var constrain = function (element, container) {
        var endTop = parseInt($(element).css("top"));
        var maxScrollTop = container.offsetHeight - element.offsetHeight;

        if (endTop > 0 || maxScrollTop > 0) {
            $(element).animate({
                top: 0
            }, "slow");
        } else if (endTop < maxScrollTop) {
            $(element).animate({
                top: maxScrollTop
            }, "slow");
        }
    };

    // <dragging>
    var dragStartPos = 0;
    var dragStartTop = 0;

    var dragElement = null;

    var hasDragged = false;

    var dragStart = function (e) {
        if (!iOS)
            cancelEvents(e);

        if (this.offsetHeight > $get('RadMenu1').parentNode.offsetHeight) { /* disable scrolling for menus that do not need it */
            if (!iOS) {
                $(document.body)
                .bind("mousemove", compositeDrag)
                .bind("mouseup", dragEnd);
            } else {
                $(document.body)
                .bind("touchmove", compositeDrag)
                .bind("touchend", dragEnd);
            }

            dragElement = $(this).stop();

            dragStartPos = getEventLocation(e).y;
            var top = parseInt(dragElement.css("top")) || 0;
            dragStartTop = top < 0 ? top : 0;
        }

        hasDragged = false;
    };

    var simpleDrag = function (e) {
        cancelEvents(e);
        dragElement
        .css("top", dragStartTop - (dragStartPos - getEventLocation(e).y) + 'px');
    };

    // hit-test
    var compositeDrag = function (e) {
        cancelEvents(e);

        if (Math.abs(dragStartPos - getEventLocation(e).y) > 10) /* threshold value */
        {
            var container = $get("RadMenu_iPhone_Content");

            $("<div id='overlay'></div>")
            .css({
                height: container.offsetHeight,
                width: container.offsetWidth,
                position: "absolute",
                top: 0,
                zIndex: 7100
            })
            .appendTo(container);

            hasDragged = true;

            if (!iOS) {
                $(document.body)
                .unbind("mousemove", compositeDrag)
                .bind("mousemove", simpleDrag);
            } else {
                $(document.body)
                .unbind("touchmove", compositeDrag)
                .bind("touchmove", simpleDrag);

                // lastElement = container;
                // lastElement.addEventListener( "click", cancelEvents, true );
            }
        }
    };

    var dragEnd = function (e) {
        cancelEvents(e);

        if (!iOS) {
            $(document.body)
            .unbind("mousemove", simpleDrag)
            .unbind("mousemove", compositeDrag)
            .unbind("mouseup", dragEnd);
        } else {
            $(document.body)
            .unbind("touchmove", simpleDrag)
            .unbind("touchmove", compositeDrag)
            .unbind("touchend", dragEnd);

            // setTimeout ( function () { lastElement.removeEventListener( "click", cancelEvents, true ); }, 0 );
        }

        constrain(dragElement[0], $get('RadMenu_iPhone_Content'));

        $('#overlay').remove();
    };
    // </dragging>

    $.fn.reverse = [].reverse;

    window.iPodMenu = function (RadMenuInstance) {
        this._animating = false;
        this.menu = RadMenuInstance;
        this._menu = RadMenuInstance.get_element();
        this._currentSelection = $('#currentSelection');        
        this._backButton =
        $('a.backButton', this._menu.parentNode.parentNode)
            .bind("click", bind(this, "navigateBack"));

        /* go to selected item */
        var selectedItem = $('.rmSelected', this._menu);
        var parentItems = selectedItem.parents('.rmItem');
        this._level = parentItems.length - 1;

        var me = this;

        if (this._level >= 0) {
            this._backButton.css('display', 'inline');

            $.each(parentItems.reverse(), function () {
                $('> .rmSlide', this)
                .css({
                    display: 'block',
                    left: me._menu.offsetWidth + 'px',
                    top: -this.offsetTop + 'px'
                })
                .find('> .rmGroup')
                    .css('display', 'block');
            });

            this._lastChild = selectedItem.parent().parent()[0];

            $(this._menu).css('left', -(this._menu.offsetWidth * me._level));
        }

        RadMenuInstance.add_itemClicking(bind(this, "navigateForward"));
        RadMenuInstance.add_itemPopulated(bind(this, "doAnimateForward"));

        /* necessary for 2nd level menus */
        RadMenuInstance.add_itemOpening(
        function (sender, args) {
            args.set_cancel(true);
        });

        RadMenuInstance.add_itemClosing(
        function (sender, args) {
            args.set_cancel(true);
        });

        if (!iOS) {
            /* init drag interface for non-iphone users */
            $(".rmVertical", this._menu)
            .live('mousedown', dragStart);
        } else {
            $(".rmVertical", this._menu)
            .live('touchstart', dragStart);
        }

        $(".rmTemplateLink", this._menu)
        .click(function (e) {
            if (hasDragged || $(this).attr('href') == '#')
                e.preventDefault();
        });
    };

    window.iPodMenu.prototype =
{
    _animateTransition: function (direction, onAnimationEnded, resetMenu) {
        this._animating = true;

        var that = this;
        var finalPosition = 0;

        if (resetMenu !== true) {
            finalPosition = parseInt($telerik.getCurrentStyle(this._menu, 'left', 0), 10) || 0;

            if (direction == Telerik.Web.UI.jSlideDirection.Left)
                finalPosition -= this._menu.offsetWidth;
            else
                finalPosition += this._menu.offsetWidth;
        }

        $(this._menu)
            .animate({
                left: (finalPosition + 'px')
            }, 300, "linear",
            function () {
                if (onAnimationEnded)
                    onAnimationEnded();

                that._animating = false;

                $('.rmActive', this._menu).removeClass('rmActive');
            });
    },

    changeOrientation: function (orientation) {
        var frame = $(this._menu).parents('#iPhoneFrame');

        frame.removeClass('OrientationVertical')
             .removeClass('OrientationHorizontal')
             .addClass('Orientation' + orientation.substr(0, 1).toUpperCase() + orientation.substr(1));

        if (!this._lastChild) {
            this._lastChild = this._menu;
        }

        $('.rmSlide', this._menu).css({
            left: this._menu.offsetWidth + 'px'
        });

        $(this._menu).css({
            left: -this._menu.offsetWidth * (this._level + 1) + 'px'
        });

        if (this._lastChild.offsetHeight < this._menu.parentNode.offsetHeight) {
            if (this._level == -1)
                $('.rmRootGroup', this._lastChild).css('top', '0px');
            else
                $('.rmGroup', this._lastChild).css('top', '0px');
        }
    },

    hideLastChild: function () {
        if (this._level >= 0) {
            this._lastChild =
                    $(this._lastChild)
                        .hide()
                        .parents('.RadMenu, .rmSlide')[0];
        } else {
            $('.rmSlide', this._menu).hide();
            this._lastChild = this._menu;
        }
    },

    navigateBack: function (reset) {
        if (this._animating) return;

        if (reset !== false && reset !== true)
            reset = false;

        if (this._lastChild.parentNode.parentNode.offsetHeight < this._menu.parentNode.offsetHeight) {
            $(this._lastChild.parentNode.parentNode).css({
                top: '0px'
            });
        }

        this._animateTransition(Telerik.Web.UI.jSlideDirection.Right, $.proxy(this.hideLastChild, this), reset);

        if (reset)
            this._level = 0;

        if (--this._level < 0) {
            this._backButton.fadeOut("fast");
            this._currentSelection.html(" ");
        }
        else {
            this._backButton.html((this._level == 0) ? "<span>Grades</span>" : ((this._level == 1) ? "<span>Subjects</span>" : ""));
            this._currentSelection.html(this._lastChild.get_text());
        }
    },

    navigateForward: function (sender, args) {
        if (hasDragged) {
            args.set_cancel(true);
            return;
        }
        else
            if (this._animating)
                return;

        this.clickedItem = args.get_item();
        var clickedElement = this.clickedItem.get_element();

        $('.rmActive', clickedElement.parentNode).removeClass('rmActive');
        $(clickedElement).addClass('rmActive');

        if (this.clickedItem._isWebServiceCallNeeded()) {
            this.menu._loadChildrenFromWebService(this.clickedItem);
            return true;
        }

        this.doAnimateForward(sender, args);
    },

    doAnimateForward: function (sender, args) {

        var childList = this.clickedItem.get_childListElement();

        if (childList) {
            // update back button

            this._backButton.fadeIn("fast");
            this._backButton.html((this._level == -1) ? "<span>Grades</span>" : ((this._level == 0) ? "<span>Subjects</span>" : ""));
            this._currentSelection.html(this._currentSelection.html() + " " + this.clickedItem.get_text());

            // show child list on the right

            childList.style.display = "block";

            this._lastChild = childList.parentNode;

            $(this._lastChild).css({
                display: 'block',
                left: this._menu.offsetWidth + 'px',
                top: -this.clickedItem.get_element().offsetTop - this.clickedItem.get_parent().get_childListElement().offsetTop + 'px'
            });

            // animate left

            this._animateTransition(Telerik.Web.UI.jSlideDirection.Left);

            this._level++;
        }
        else {
            if (this.clickedItem.get_navigateUrl() == '#')
                args.set_cancel(true);
        }
    }
};
})($telerik.$);