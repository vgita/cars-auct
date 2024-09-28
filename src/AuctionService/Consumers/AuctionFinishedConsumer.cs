namespace AuctionService.Consumers;

public class AuctionFinishedConsumer(AuctionDbContext DbContext) : IConsumer<AuctionFinished>
{
  public async Task Consume(ConsumeContext<AuctionFinished> context)
  {
    Console.WriteLine("--> Consuming Auction Finished event");

    Auction? auction = await DbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

    if (auction is null)
    {
      return;
    }

    if (context.Message.ItemSold)
    {
      auction.Winner = context.Message.Winner;
      auction.SoldAmount = context.Message.Amount;
    }

    auction.Status = auction.SoldAmount > auction.ReservePrice
      ? Status.Finished : Status.ReserveNotMet;

    await DbContext.SaveChangesAsync();
  }
}
