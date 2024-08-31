namespace AuctionService.Data;

public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
{
  public DbSet<Auction> Auctions { get; set; }
  public DbSet<Item> Items { get; set; }
}
