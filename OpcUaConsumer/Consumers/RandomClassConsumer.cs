using MassTransit;
using SharedModels;

namespace OpcUaConsumer.Consumers;

public class TagModelConsumer : IConsumer<TagModel>
{
    public Task Consume(ConsumeContext<TagModel> context)
    {
        Console.WriteLine("id: {1}, name: {0}, value: {2}", context.Message.NodeId, context.Message.DisplayName, context.Message.Value);
        
        return Task.CompletedTask;
    }
}