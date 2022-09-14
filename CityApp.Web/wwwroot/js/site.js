// Write your Javascript code.
// extend jquery range validator to work for required checkboxes
var defaultRangeValidator = $.validator.methods.range;
$.validator.methods.range = function (value, element, param) {
    if (element.type === 'checkbox') {
        // if it's a checkbox return true if it is checked
        return element.checked;
    } else {
        // otherwise run the default validation function
        return defaultRangeValidator.call(this, value, element, param);
    }
}

var blockUI = function (customMessage) { // Add more params/settings/options as needed

    var message = "<img src=\"/images/spinner.svg\" />";
    if (customMessage)
    {
        message = customMessage;
    }
    
    $.blockUI({ message: message })
};

$.blockUI.defaults.css.border = 'none';
$.blockUI.defaults.css.background = 'none';
$.blockUI.defaults.baseZ = 1051;

var unblockUI = function (message) {

    $.unblockUI();
};

$("form.with-loader").submit(function () {
    blockUI();
});