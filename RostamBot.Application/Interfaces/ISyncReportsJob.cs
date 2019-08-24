using System.Threading.Tasks;

namespace RostamBot.Application.Interfaces
{
    public interface ISyncReportsJob
    {
        Task GetMentionsAsync();

        Task GetDirectsAsync();
    }
}
