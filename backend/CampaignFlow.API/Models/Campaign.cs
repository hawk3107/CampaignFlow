namespace CampaignFlow.API.Models;

public class Campaign
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Advertiser { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public decimal Spend { get; set; }

    public int Impressions { get; set; }

    public int Clicks { get; set; }

    public int Conversions { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}