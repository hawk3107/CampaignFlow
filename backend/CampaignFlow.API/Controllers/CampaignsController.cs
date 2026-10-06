using CampaignFlow.API.Data;
using CampaignFlow.API.Analytics;
using CampaignFlow.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampaignFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly CampaignDbContext _context;
    private readonly CampaignAnalyticsService _analyticsService;
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController(
    CampaignDbContext context,
    CampaignAnalyticsService analyticsService,
    ILogger<CampaignsController> logger)
    {
        _context = context;
        _analyticsService = analyticsService;
        _logger = logger;
    }
    // public CampaignsController(CampaignDbContext context)
    // {
    //     _context = context;
    // }

    // GET: api/campaigns
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()
    {
        return await _context.Campaigns.ToListAsync();
    }

    // GET: api/campaigns/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Campaign>> GetCampaign(int id)
    {
        var campaign = await _context.Campaigns.FindAsync(id);

        if (campaign == null)
        {
            return NotFound();
        }

        return campaign;
    }
    
    [HttpGet("{id}/analytics")]
    public async Task<ActionResult<object>> GetCampaignAnalytics(int id)
    {
    var campaign = await _context.Campaigns.FindAsync(id);

    if (campaign == null)
    {
        _logger.LogWarning(
            "Analytics request failed. CampaignId: {CampaignId} was not found.",
            id
        );

        return NotFound();
    }

    var analytics = _analyticsService.Calculate(campaign);

    _logger.LogInformation(
        "Campaign analytics calculated. CampaignId: {CampaignId}, Name: {CampaignName}",
        campaign.Id,
        campaign.Name
    );

    return analytics;
    }

    // POST: api/campaigns
    [HttpPost]
    public async Task<ActionResult<Campaign>> CreateCampaign(Campaign campaign)
    {
    try
    {
        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Campaign created successfully. CampaignId: {CampaignId}, Name: {CampaignName}, Advertiser: {Advertiser}",
            campaign.Id,
            campaign.Name,
            campaign.Advertiser
        );

        return CreatedAtAction(
            nameof(GetCampaign),
            new { id = campaign.Id },
            campaign
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error creating campaign. CampaignName: {CampaignName}, Advertiser: {Advertiser}",
            campaign.Name,
            campaign.Advertiser
        );

        return StatusCode(500, "An error occurred while creating the campaign.");
    }
    }

    // PUT: api/campaigns/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCampaign(int id, Campaign campaign)
    {
    if (id != campaign.Id)
        return BadRequest();

    _context.Entry(campaign).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Campaign updated successfully. CampaignId: {CampaignId}, Name: {CampaignName}, Status: {Status}",
            campaign.Id,
            campaign.Name,
            campaign.Status
        );
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await _context.Campaigns.AnyAsync(c => c.Id == id))
        {
            _logger.LogWarning(
                "Campaign update failed. CampaignId: {CampaignId} was not found.",
                id
            );

            return NotFound();
        }

        throw;
    }

    return NoContent();
    }

    // DELETE: api/campaigns/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
    var campaign = await _context.Campaigns.FindAsync(id);

    if (campaign == null)
    {
        _logger.LogWarning(
            "Campaign deletion failed. CampaignId: {CampaignId} was not found.",
            id
        );

        return NotFound();
    }

    _context.Campaigns.Remove(campaign);
    await _context.SaveChangesAsync();

    _logger.LogInformation(
        "Campaign deleted successfully. CampaignId: {CampaignId}, Name: {CampaignName}",
        campaign.Id,
        campaign.Name
    );

    return NoContent();
    }
}