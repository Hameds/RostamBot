using Hangfire;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RostamBot.Application.Interfaces
{
    public interface ICommandsScheduler
    {
        void Schedule(IRequest request, DateTimeOffset scheduleAt, string description = null);
        void Schedule(IRequest request, TimeSpan delay, string description = null);
        void ScheduleRecurring(IRequest request, string name, string cronExpression, string description = null);
        string SendNow(IRequest request, string description = null);
        string SendNow(IRequest request, string parentJobId, JobContinuationOptions continuationOption, string description = null);
    }
}
