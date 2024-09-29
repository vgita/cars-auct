namespace NotificationService.Consumers;

public class BidPlacedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("==> auction finished message received");

        await hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}
