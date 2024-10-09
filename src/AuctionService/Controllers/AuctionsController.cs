using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
    {
        return await repo.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await repo.GetAuctionByIdAsync(id);

        if (auction is null)
        {
            return NotFound();
        }

        return mapper.Map<AuctionDto>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
    {
        var auction = mapper.Map<Auction>(dto);

        auction.Seller = User.Identity?.Name ?? "unknown";

        repo.AddAuction(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction)); //will create a new message in the outbox and be handled in the same transaction

        var result = await repo.SaveChangesAsync();

        if (!result)
        {
            return BadRequest("Could not save changes tot the DB");
        }

        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto dto)
    {
        var auction = await repo.GetAuctionEntityByIdAsync(id);

        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller != User.Identity?.Name)
        {
            return Forbid();
        }

        auction.Item.Make = dto.Make ?? auction.Item.Make;
        auction.Item.Model = dto.Model ?? auction.Item.Model;
        auction.Item.Color = dto.Color ?? auction.Item.Color;
        auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = dto.Year ?? auction.Item.Year;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction)); //will create a new message in the outbox and be handled in the same transaction

        var result = await repo.SaveChangesAsync();

        if (!result)
        {
            return BadRequest("Could not save changes to the DB");
        }

        return Ok(mapper.Map<AuctionDto>(auction));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await repo.GetAuctionEntityByIdAsync(id);

        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller != User.Identity?.Name)
        {
            return Forbid();
        }

        repo.RemoveAuction(auction);

        await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() }); //will create a new message in the outbox and be handled in the same transaction

        var result = await repo.SaveChangesAsync();

        if (!result)
        {
            return BadRequest("Could not save changes to the DB");
        }

        return NoContent();
    }
}