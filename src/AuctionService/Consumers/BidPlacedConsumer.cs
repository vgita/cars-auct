namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDbContext DbContext) : IConsumer<BidPlaced>
{
  public async Task Consume(ConsumeContext<BidPlaced> context)
  {
    Console.WriteLine("--> Consuming BidPlaced event");

    Auction? auction = await DbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

    if (auction is null)
    {
      return;
    }

    if (auction.CurrentHighBid is null
     || context.Message.BidStatus.Contains("Accepted")
     && context.Message.Amount > auction.CurrentHighBid)
    {
      auction.CurrentHighBid = context.Message.Amount;
      await DbContext.SaveChangesAsync();
    }
  }
}
