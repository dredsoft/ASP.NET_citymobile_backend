;
(function ($, window, document) {

    // The $ is now locally scoped 
    $(function () {

        // DOM ready!
        //Variables
        $document = $(document),
            $window = $(window),
            $body = $("body"),
            $appBarWrapper = $("div#app-bar-wrapper"),
            $appBarMain = $appBarWrapper.find("#app-bar-main"),
            $appBarExt = $appBarWrapper.find("#app-bar-extension"),
            $appViewTitle = $appBarExt.find(".app-view-title"),
            $appNav = $("div.app-navigation"),
            $newAppNav = $appNav.clone(),
            i = 1;



        // Initial setup
        $appNav.addClass("left-side-nav");
        $newAppNav.addClass("app-bar-nav");
        $newAppNav.appendTo($appNav.parent());
        $appNav.remove();
        $("ul.nav-list").tabs();


        // Event delegation
        $document.on("click", ".nav-list li", function () {
            var $element = $(this);
            getView($element);
        });
        
        $window.on("scroll", updateNav);


    });


    // Functions
    function updateNav(){
        var appBarWrapperOffset = $appBarWrapper.outerHeight() - $appBarMain.outerHeight(),
            perc = ($window.scrollTop() / appBarWrapperOffset*3) -1;
        if($window.scrollTop() > appBarWrapperOffset){
            $appBarMain.addClass("raised");
        }else{
            $appBarMain.removeClass("raised");
            if (perc >= -1){
                while(perc>0){
                    perc = 0;
                }
                perc = Math.abs(perc);
                $appViewTitle.css("opacity", perc);

            }
        };
    };


    function getView($element) {
        var linkURL = $element.attr("data-rel");
        $.ajax({
            // the URL for the request
            url: linkURL,

            // whether this is a POST or GET request
            type: "GET",

            // function to call before we send the AJAX request
            beforeSend: startFn,

            // function to call for success
            success: successFn,

            // function to call on an error
            error: errorFn,

            // code to run regardless of success or failure
            complete: function (xhr, status) {
                //console.log("The request is complete!");
            }
        });
    }

    function startFn() {
        //console.log("fetching view");
    }

    function successFn(result) {
        $("#view").empty();
        $("#view").append($(result)).hide(0).delay(1).show(0);
    }

    function errorFn(xhr, status, strErr) {
        console.log("There was an error!");
    }

}(window.jQuery, window, document)); // Fully reference jQuery after this point.