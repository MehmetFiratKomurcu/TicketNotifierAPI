using System.Threading.Tasks;

namespace TicketNotifier.Checkers.Interfaces
{
    public interface ICheckEvent
    {
        Task Run();
    }
}