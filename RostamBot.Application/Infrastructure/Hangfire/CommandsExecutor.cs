using MediatR;
using Newtonsoft.Json;
using RostamBot.Application.Features.SuspiciousActivity.Commands;
using RostamBot.Application.Interfaces;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace RostamBot.Application.Infrastructure.Hangfire
{
    public class CommandsExecutor : ICommandsExecutor
    {
        private readonly IMediator mediator;
        public CommandsExecutor(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [DisplayName("Processing command {0}")]
        public Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject)
        {
            var type = Assembly.GetAssembly(typeof(ChangeBlockStatus)).GetType(mediatorSerializedObject.FullTypeName);

            if (type != null)
            {
                dynamic req = JsonConvert.DeserializeObject(mediatorSerializedObject.Data, type);

                return mediator.Send(req as IRequest);
            }

            return null;
        }
    }
}
