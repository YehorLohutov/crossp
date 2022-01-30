using System.Collections.Generic;
//using DotNetCore.CAP;
using Users.Contracts;
using Files.Repository;
using Files.Models;
using System.Threading.Tasks;
using System.Threading;
using MassTransit;

namespace Files.MessagesSubscribers
{
    //public class CreateFileOwner : IConsumer<UserCreatedMessage>
    //{
    //    private readonly IDatabaseRepository databaseRepository = default;

    //    public CreateFileOwner(IDatabaseRepository databaseRepository)
    //    {
    //        this.databaseRepository = databaseRepository;
    //    }

    //    public async Task Consume(ConsumeContext<UserCreatedMessage> context)
    //    {

    //        //string messageId = capHeader.GetValueOrDefault(DotNetCore.CAP.Messages.Headers.MessageId);
    //        //string consumer = nameof(CreateFileOwner);

    //        //if (await databaseRepository.MessageHasBeenProcessedAsync(messageId, consumer))
    //        //    return;

    //        FileOwner owner = new FileOwner(context.Message.UserExternalId);
    //        await databaseRepository.InsertOwnerAsync(owner);

    //        //owner.Files.AddRange(await databaseRepository.GetDefaultFilesAsync());

    //        //await databaseRepository.ConfirmMessageProcessingAsync(messageId, consumer);

    //        await databaseRepository.SaveChangesAsync();
    //        await Task.CompletedTask;
    //    }
    //}
}
