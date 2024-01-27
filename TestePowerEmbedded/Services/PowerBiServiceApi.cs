using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using TestePowerEmbedded.ViewModels;

namespace TestePowerEmbedded.Services;

public class PowerBiServiceApi
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private const string PowerBiApiScope = "https://analysis.windows.net/powerbi/api/.default";
    private readonly string _powerBiApiUrl;
    
    public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition)
    {
        _tokenAcquisition = tokenAcquisition;
        _powerBiApiUrl = configuration["PowerBi:ServiceRootUrl"];
    }
    
    public async Task<EmbeddedReportViewModel> GetReport(Guid workspaceId, Guid reportId)
    {
        var client = await GetPowerBiClient();
        var report = await client.Reports.GetReportInGroupAsync(workspaceId, reportId);
        
        var datasetId = report.DatasetId;
        var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, datasetId);
        var embedTokenResponse = await client.Reports.GenerateTokenAsync(workspaceId, reportId, tokenRequest);
        var embedToken = embedTokenResponse.Token;    
        
        return new EmbeddedReportViewModel
        {
            Id = report.Id.ToString(),
            Name = report.Name,
            EmbedUrl = report.EmbedUrl,
            EmbedToken = embedToken
        };
    }
    
    private async Task<PowerBIClient> GetPowerBiClient()
    {
        var accessToken = await GetAccessTokenAsync();
        var tokenCredentials = new TokenCredentials(accessToken, "Bearer");
        var client = new PowerBIClient(new Uri(_powerBiApiUrl), tokenCredentials);
        return client;
    }
    
    private async Task<string> GetAccessTokenAsync()
    {
        var scopes = new[] { PowerBiApiScope };
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
        return accessToken;
    }
}