using System.Threading.Tasks;

namespace RostamBot.Application.Interfaces
{
    public interface ISyncMentionsJob
    {
        Task GetMentionsAsync();
    }
}
