using MassTransit;
using System;
using System.Collections.Generic;
using Files.Contracts;
using Files.Repository;
using System.Threading.Tasks;
using Files.Models;
using System.Linq;

namespace Files.Consumers
{
    public class DefaultFileExternalIdConsumer : IConsumer<GetDefaultFileExternalIdRequest>
    {
        private readonly IFilesRepository filesRepository;
        public DefaultFileExternalIdConsumer(IFilesRepository filesRepository) =>
            this.filesRepository = filesRepository;

        public async Task Consume(ConsumeContext<GetDefaultFileExternalIdRequest> context)
        {
            IEnumerable<Files.Models.File> defaultFiles;
            try
            {
                defaultFiles = await filesRepository.GetDefaultFilesAsync();
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.ToString());
            }

            if (defaultFiles.Count() == 0)
                throw new NullReferenceException("Can not find default file in database.");

            await context.RespondAsync(new DefaultFileExternalIdResponse(defaultFiles.First().ExternalId));
        }
    }
}
