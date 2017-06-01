"use strict";

function initializeAjax(targetSelector, activatorSelector, inputSelector, ajaxUrl, successCallback) {
    let circularFlashingColors =
        `<div class="progress">
<div class="indeterminate"></div>
</div>`;

    function changeDataIn(selector, data) {
        $(selector).empty();
        $(selector).append(data);
    }

    function onSuccess(data, textStatus, jqXHR) {
        if (textStatus !== 'success') {
            Materialize.toast(`Sorry, something went bad, please try again. If error will repeat contact an administrator. Cause: ${textStatus}`, 900);
            console.error(textStatus);
        }
        var dataToChange = data;
        if (typeof successCallback === 'function') {
            dataToChange = successCallback(targetSelector, data);
        }
        else {
            changeDataIn(targetSelector, dataToChange);
        }
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
            $(selector).html(fixed)
            return fixed;
        }
        else {
            return inputData;
        }
    }

    function downloadData() {
        changeDataIn(targetSelector, circularFlashingColors);
        var requestedUrl = fixInput(inputSelector);

        $.ajax({
            url: ajaxUrl,
            success: onSuccess,
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
            if (event.keyCode === 13) {
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

    (function realtimeButtonInit() {
        let targetSelector = "#topData";
        let activatorSelector = "#realtimeButton";
        let ajaxUrl = "/WebStatistics/SingleSiteRaw";
        function createChart(selector) {
            $(selector).html("<canvas id=\"graph\"> </canvas>");
            let ctx = document.getElementById('graph').getContext('2d');
            let chart = new Chart(ctx, {
                type: 'bar',
                data: {
                    datasets: [{
                        label: 'miliseconds',
                    }]
                }
            });
            return chart;
        }
        function onSuccess(selector, data) {
            let target = $(targetSelector);
            var chart;
            if (target.attr("chart") !== 'is') {
                target.empty();
                chart = createChart(targetSelector);
                console.log(data);
                addData(chart, data.uri, data.descendants);
            }
            else {
                addData(chart, data.Uri, data.Descendants);
            }
        }
        initializeAjax(targetSelector, activatorSelector, input, ajaxUrl, onSuccess)
    })();

    (function minMaxButtonInit() {
        let targetSelector = "#bottomData";
        let activatorSelector = "#minMaxButton";
        let ajaxUrl = "/WebStatistics/MinMaxValues";
        initializeAjax(targetSelector, activatorSelector, input, ajaxUrl)
    })();
});

function addData(chart, label, data) {
    chart.data.labels.push(label);
    chart.data.datasets[0].data.push(data);
    chart.update();
}