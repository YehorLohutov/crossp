using MassTransit;
using System.Collections.Generic;
using Ads.Repository;
using Ads.Models;
using System.Threading.Tasks;
using Files.Contracts;
using System.Linq;
using MassTransit.Mediator;

namespace Ads.Consumers
{
    public class FileDeletedConsumer : IConsumer<FileDeletedMessage>
    {
        private readonly IAdsRepository adsRepository;
        private readonly IMediator mediator;
        public FileDeletedConsumer(IAdsRepository adsRepository, IMediator mediator)
        {
            this.adsRepository = adsRepository;
            this.mediator = mediator;
        }
        public async Task Consume(ConsumeContext<FileDeletedMessage> context)
        {
            IEnumerable<Ad> adsWhichUsingDeletedFile = 
                await adsRepository.GetAdsWhichUsingFileAsync(context.Message.FileExternalId);   

            if (adsWhichUsingDeletedFile.Count() == 0)
                return;

            IRequestClient<GetDefaultFileExternalIdRequest> client = mediator.CreateRequestClient<GetDefaultFileExternalIdRequest>();
            Response<DefaultFileExternalIdResponse> response = await client.GetResponse<DefaultFileExternalIdResponse>(new GetDefaultFileExternalIdRequest());
            string defaultFileExternalId = response.Message.FileExternalId;

            foreach(Ad ad in adsWhichUsingDeletedFile)
                ad.FileExternalId = defaultFileExternalId;
            
            await adsRepository.SaveChangesAsync();
        }
    }
}
