using AutoMapper.QueryableExtensions;

namespace AuctionService.Data;

public interface IAuctionRepository
{
  Task<List<AuctionDto>> GetAuctionsAsync(string? date);
  Task<AuctionDto?> GetAuctionByIdAsync(Guid id);
  Task<Auction?> GetAuctionEntityByIdAsync(Guid id);
  void AddAuction(Auction auction);
  void RemoveAuction(Auction auction);
  Task<bool> SaveChangesAsync();
}

public class AuctionRepository(AuctionDbContext context, IMapper mapper) : IAuctionRepository
{
  public void AddAuction(Auction auction)
  {
    context.Auctions.Add(auction);
  }

  public async Task<AuctionDto?> GetAuctionByIdAsync(Guid id)
  {
    return await context.Auctions
           .ProjectTo<AuctionDto>(mapper.ConfigurationProvider)
           .FirstOrDefaultAsync(a => a.Id == id);
  }

  public async Task<Auction?> GetAuctionEntityByIdAsync(Guid id)
  {
    return await context.Auctions
          .Include(a => a.Item)
          .FirstOrDefaultAsync(a => a.Id == id);
  }


  public async Task<List<AuctionDto>> GetAuctionsAsync(string? date)
  {
    var query = context.Auctions
           .Include(a => a.Item)
           .OrderBy(x => x.Item.Make).AsQueryable();

    if (!string.IsNullOrWhiteSpace(date))
    {
      query = query.Where(q => q.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
    }

    var auctions = await query.ToListAsync();

    return mapper.Map<List<AuctionDto>>(auctions);
  }

  public void RemoveAuction(Auction auction)
  {
    context.Auctions.Remove(auction);
  }

  public async Task<bool> SaveChangesAsync()
  {
    return await context.SaveChangesAsync() > 0;
  }
}
