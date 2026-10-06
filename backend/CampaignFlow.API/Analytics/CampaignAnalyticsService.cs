using CampaignFlow.API.Models;

namespace CampaignFlow.API.Analytics;

public class CampaignAnalyticsService
{
    public object Calculate(Campaign campaign)
    {
        var ctr = campaign.Impressions > 0
            ? (decimal)campaign.Clicks / campaign.Impressions * 100
            : 0;

        var conversionRate = campaign.Clicks > 0
            ? (decimal)campaign.Conversions / campaign.Clicks * 100
            : 0;

        var cpc = campaign.Clicks > 0
            ? campaign.Spend / campaign.Clicks
            : 0;

        var costPerConversion = campaign.Conversions > 0
            ? campaign.Spend / campaign.Conversions
            : 0;

        return new
        {
            campaign.Id,
            campaign.Name,
            CTR = Math.Round(ctr, 2),
            ConversionRate = Math.Round(conversionRate, 2),
            CPC = Math.Round(cpc, 2),
            CostPerConversion = Math.Round(costPerConversion, 2)
        };
    }
}