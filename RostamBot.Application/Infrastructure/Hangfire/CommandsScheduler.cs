using Hangfire;
using MediatR;
using Newtonsoft.Json;
using RostamBot.Application.Interfaces;
using System;

namespace RostamBot.Application.Infrastructure.Hangfire
{
    public class CommandsScheduler : ICommandsScheduler
    {
        private readonly ICommandsExecutor commandsExecutor;

        public CommandsScheduler(ICommandsExecutor commandsExecutor)
        {
            this.commandsExecutor = commandsExecutor;
        }

        public string SendNow(IRequest request, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(request, description);

            return BackgroundJob.Enqueue(() => commandsExecutor.ExecuteCommand(mediatorSerializedObject));
        }

        public string SendNow(IRequest request, string parentJobId, JobContinuationOptions continuationOption, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(request, description);
            return BackgroundJob.ContinueJobWith(parentJobId, () => commandsExecutor.ExecuteCommand(mediatorSerializedObject), continuationOption);
        }

        public void Schedule(IRequest request, DateTimeOffset scheduleAt, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(request, description);

            BackgroundJob.Schedule(() => commandsExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);
        }
        public void Schedule(IRequest request, TimeSpan delay, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(request, description);
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => commandsExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
        }

        public void ScheduleRecurring(IRequest request, string name, string cronExpression, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(request, description);

            RecurringJob.AddOrUpdate(name, () => commandsExecutor.ExecuteCommand(mediatorSerializedObject), cronExpression, TimeZoneInfo.Local);
        }


        private MediatorSerializedObject SerializeObject(object mediatorObject, string description)
        {
            string fullTypeName = mediatorObject.GetType().FullName;
            string data = JsonConvert.SerializeObject(mediatorObject, new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                //ContractResolver = new PrivateJsonDefaultContractResolver()
            });

            return new MediatorSerializedObject(fullTypeName, data, description);
        }
    }
}
