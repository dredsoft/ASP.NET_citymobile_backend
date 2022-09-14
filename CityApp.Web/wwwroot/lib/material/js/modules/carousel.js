// Avoid `console` errors in browsers that lack a console.
(function () {
    var method;
    var noop = function () {};
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeline', 'timelineEnd', 'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());


// Carousel
(function ($, window, document, undefined) {

    "use strict";

    //plugin name
    var carousel = "carousel";

    //get instance
    var self = null;

    //defaults
    var defaults = {
        lazy: true,
        visibleSlides: 1,
        initialSlide: 1
    };

    //main function
    function Carousel(element, options) {
        self = this;
        this.element = $(element);
        this.options = $.extend({}, defaults, options);
        this._defaults = defaults;
        this._name = carousel;
        //init
        this.init();

    }

    //Check for mobile devices
    Carousel.prototype.isMobile = function () {
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    };

    Carousel.prototype.showImage = function ($inViewSlide) {
        var $visibleImg = $inViewSlide.find("img");

        //set src for all images in visible slide
        $visibleImg.each(function () {
            var $this = $(this),
                visibleImgSrc = $inViewSlide.find("img").attr("data-src");
            $visibleImg.attr("src", visibleImgSrc).addClass("lazy-loaded");
        });
    };

    Carousel.prototype.updateButtons = function ($element, $inViewSlide) {
        var $slideShow = $element.find(".carousel-slideshow"),
            $slides = $slideShow.find(".slide"),
            $controls = $element.find(".carousel-controls"),
            $prev = $controls.find(".prev"),
            $next = $controls.find(".next"),
            inViewSlideIndex = $inViewSlide.index(),
            lastSlideIndex = $slides.length - 1;
        //set state for next and previous buttons
        if (inViewSlideIndex == 0) {
            $prev.addClass("disabled").attr("aria-disabled", "true");
        } else {
            $prev.removeClass("disabled").attr("aria-disabled", "false");
        };

        if (inViewSlideIndex == lastSlideIndex) {
            $next.addClass("disabled").attr("aria-disabled", "true");
        } else {
            $next.removeClass("disabled").attr("aria-disabled", "false");
        };
    };

    Carousel.prototype.toggle = function ($this, $element, $slideShow, $inViewSlide, $pagerCurrentDot) {
        var inViewSlideIndex = $inViewSlide.index(),
            $slides = $slideShow.find(".slide"),
            $pager = $element.find(".pager"),
            $pagerCurrentDot = $pager.find(".pager-dot.is-current"),
            dotsLength = $pager.find(".pager-dot").length,
            currentDotIndex = parseInt($pagerCurrentDot.attr("data-index")),
            visibleSlidesNo = this.options.visibleSlides * 100;

        $pagerCurrentDot.removeClass("is-current");

        if ($this.hasClass("prev")) {
            var nextDotIndex = currentDotIndex - 1;
            if (nextDotIndex < 0) {
                nextDotIndex = (dotsLength - 1);
            };
            $pagerCurrentDot = $pager.find(".pager-dot").eq(nextDotIndex);
            $pagerCurrentDot.addClass("is-current");
        } else if ($this.hasClass("next")) {
            var nextDotIndex = currentDotIndex + 1;
            if (nextDotIndex > (dotsLength - 1)) {
                nextDotIndex = 0;
            };
            $pagerCurrentDot = $pager.find(".pager-dot").eq(nextDotIndex);
            $pagerCurrentDot.addClass("is-current");
        } else if ($this.hasClass("pager-dot")) {
            $pagerCurrentDot = $this;
            currentDotIndex = parseInt($pagerCurrentDot.attr("data-index"));
            $pagerCurrentDot.addClass("is-current");
        };

        inViewSlideIndex = $pagerCurrentDot.attr("data-index");
        $slideShow.css("transform", "translateX(" + (-(inViewSlideIndex) * visibleSlidesNo) + "%)");
        $slides.removeClass("in-view");
        $inViewSlide = $slides.eq(inViewSlideIndex);
        $inViewSlide.addClass("in-view");

        var dataContrast = $inViewSlide.attr("data-contrast");
        if(dataContrast == "light"){
            $element.find(".carousel-controls").addClass("light").removeClass("dark");
        }else if(dataContrast == "dark"){
            $element.find(".carousel-controls").addClass("dark").removeClass("light");
        };
        

        self.update($element, $slideShow, $inViewSlide, $pagerCurrentDot);
        /*self.showImage($inViewSlide);
        self.updateButtons($element, $inViewSlide);*/
    };

    Carousel.prototype.update = function ($element, $slideShow, $inViewSlide, $pagerCurrentDot) {
        self.showImage($inViewSlide);
        self.updateButtons($element, $inViewSlide);
    }

    Carousel.prototype.activate = function ($element) {
        var $slideShow = $element.find(".carousel-slideshow"),
            $slides = $slideShow.find(".slide"),
            $inViewSlide = $slideShow.find(".slide.in-view"),
            $controls = $element.find(".carousel-controls"),
            $next = $controls.find(".next"),
            $prev = $controls.find(".prev"),
            $pager = $controls.find(".pager"),
            $pagerDot = $pager.find(".pager-dot"),
            $pagerCurrentDot = $pager.find(".pager-dot.is-current"),
            inViewSlideIndex = $inViewSlide.index(),
            i = this.options.initialSlide;


        if (inViewSlideIndex >= 0) {
            var $currentDot = $pager.find(".pager-dot[data-index=" + inViewSlideIndex + "]");
            $pagerDot.removeClass("is-current");
            $currentDot.addClass("is-current");
            $pagerCurrentDot = $currentDot;
        } else if ($pagerCurrentDot.index() >= 0) {
            $inViewSlide.removeClass("in-view");
            $inViewSlide = $slides.eq($pagerCurrentDot.attr("data-index"));
            $inViewSlide.addClass("in-view");
        } else {
            $inViewSlide = $slides.eq(i);
            $inViewSlide.addClass("in-view");
            $pagerCurrentDot = $pagerDot.eq(i);
            $pagerCurrentDot.addClass("is-current");
        };

        //self.update($element, $slideShow, $inViewSlide, $pagerCurrentDot);

        function toggleSlide() {
            var $this = $(this);
            if (!$this.hasClass("disabled")) {
                self.toggle($this, $element, $slideShow, $inViewSlide, $pagerCurrentDot);
            }
        }

        $prev.on("click", toggleSlide);
        $next.on("click", toggleSlide);
        $pagerDot.on("click", toggleSlide);
        $pagerCurrentDot.click();
    };


    //Init
    Carousel.prototype.init = function () {
        var $element = this.element,
            $slideShow = $element.find(".carousel-slideshow"),
            $slides = $slideShow.find(".slide"),
            $controls = $element.find(".carousel-controls"),
            $prev = $controls.find(".prev"),
            $next = $controls.find(".next"),
            $pager = $controls.find(".pager"),
            $pagerDot = $pager.find(".pager-dot"),
            $pagerCurrentDot = $pager.find(".pager-dot.is-current"),
            isLazy = this.options.lazy;

        var offsetPerc = parseInt(100 / this.options.visibleSlides),
            initialSlideIndex = this.options.initialSlide;

        $element.removeClass("lazy");
        $slides.find("img").addClass("lazy");

        function distribute(offsetPerc, initialSlideIndex, $slideShow, $slides) {
            var slidesNo = $slides.length,
                slideShowOffset = -offsetPerc * (initialSlideIndex),
                i = 0;

            $slideShow.css("transform", "translateX(" + slideShowOffset + "%)");

            $slides.each(function () {
                var $this = $(this),
                    $img = $this.find("img"),
                    dataSrc = $img.attr("src");
                $this.css("left", i * offsetPerc + "%");
                i += 1;
                $img.each(function () {
                    var $this = $(this);
                    $this.attr({
                        "src": "",
                        "data-src": dataSrc
                    });
                });
            });
        };
        distribute(offsetPerc, initialSlideIndex, $slideShow, $slides);

        // Check for the slide in view if predefined, if none, set first slide as active
        self.activate($element);
    };


    /**
     * Create the jquery plugin function
     */
    $.fn.carousel = function (options) {
        return this.each(function () {
            new Carousel(this, options);
        });
    };

})(jQuery, window, document);