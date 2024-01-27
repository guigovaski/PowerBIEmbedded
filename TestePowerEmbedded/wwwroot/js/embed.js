(() => {
    var reportContainer = document.getElementById("embed-container");
    
    var reportId = window.viewModel.reportId;
    var embedUrl = window.viewModel.embedUrl;
    var token = window.viewModel.embedToken;

    var models = window['powerbi-client'].models;

    var config = {
        type: 'report',
        id: reportId,
        embedUrl: embedUrl,
        accessToken: token,
        permissions: models.Permissions.All,
        tokenType: models.TokenType.Embed,
        viewMode: models.ViewMode.View,
        settings: {
            panes: {
                filters: { expanded: false, visible: true },
                pageNavigation: { visible: false }
            }
        }
    };

    powerbi.embed(reportContainer, config);
    
    const heightBuffer = 12;
    const newHeight = $(window).height() - ($("header").height() + heightBuffer);
    
    $("#embed-container").height(newHeight);
    
    $(window).resize(function () {
        var newHeight = $(window).height() - ($("header").height() + heightBuffer);
        $("#embed-container").height(newHeight);
    });
})();