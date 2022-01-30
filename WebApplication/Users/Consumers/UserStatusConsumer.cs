using MassTransit;
using System.Threading.Tasks;
using Users.Contracts;
using Users.Repository;

namespace Users.Consumers
{
    public class UserStatusConsumer : IConsumer<GetUserStatusRequest>
    {
        private readonly IUsersRepository usersRepository;
        public UserStatusConsumer(IUsersRepository usersRepository) =>
            this.usersRepository = usersRepository;
        
        public async Task Consume(ConsumeContext<GetUserStatusRequest> context)
        {
            bool exist = await usersRepository.UserExists(context.Message.UserExternalId);
            await context.RespondAsync(new UserStatusResponse(context.Message.UserExternalId, exist));
        }
    }
}
