function initializeAjax(targetSelector, activatorSelector, inputSelector, ajaxUrl) {
    let circularFlashingColors =
        `<div class="progress">
    <div class="indeterminate"></div>
</div>`;
    function changeDataIn(selector, data) {
        $(selector).empty();
        $(selector).append(data);
    }
    function onSucceess(data, textStatus, jqXHR) {
        if (textStatus != 'success') {
            Materialize.toast(`Sorry, something went bad, please try again. If error will repeat contact an administrator. Cause: ${textStatus}`, 900);
            console.error(textStatus);
        }
        changeDataIn(targetSelector, data);
    };
    function onError() {
        changeDataIn(targetSelector, "");
        Materialize.toast("Sorry, your input is invalid. Please provide valid http://adresss.com .", 1000);
    }
    function fixInput(selector) {
        let inputData = $(selector).val();
        if (!inputData.startsWith("http://") && !inputData.startsWith("https://")) {
            Materialize.toast("Let me fix this URI for you :)", 1600);
            var fixed = "http://" + inputData;
            $(selector).val(fixed)
            return fixed;
        }
    }
    function downloadData() {
        changeDataIn(targetSelector, circularFlashingColors);
        var requestedUrl = fixInput(inputSelector);
        $.ajax({
            url: ajaxUrl,
            success: onSucceess,
            error: onError,
            data: {
                url: requestedUrl
            }
        });
    };
    $(activatorSelector).click(downloadData);
}
$(document).ready(function () {
    let input = "#uriInput";
    (function goButtonInit() {
        let targetSelector = "#topData";
        let activatorSelector = "#goButton";
        let ajaxUrl = "/WebStatistics/SiteAccessTime";
        initializeAjax(targetSelector, activatorSelector, input, ajaxUrl)
        $(input).on("keyup", function (event) {
            if (event.keyCode == 13) {
                $(activatorSelector).get(0).click();
            }
        });
    })();
    (function historyButtonInit() {
        let targetSelector = "#bottomData";
        let activatorSelector = "#historyButton";
        let ajaxUrl = "/WebStatistics/SiteHistory";
        initializeAjax(targetSelector, activatorSelector, input, ajaxUrl)
    })();
    (function minMax() {
        let targetSelector = "#bottomData";
        let activatorSelector = "#minMaxButton";
        let ajaxUrl = "/WebStatistics/MinMaxValues";
        initializeAjax(targetSelector, activatorSelector, input, ajaxUrl)
    })();
});