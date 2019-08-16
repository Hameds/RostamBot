using RostamBot.Application.Infrastructure.Hangfire;
using System.Threading.Tasks;

namespace RostamBot.Application.Interfaces
{
    public interface ICommandsExecutor
    {
        Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject);
    }
}
