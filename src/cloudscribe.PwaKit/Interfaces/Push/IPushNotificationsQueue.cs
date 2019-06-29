using cloudscribe.PwaKit.Models;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IPushNotificationsQueue
    {
        void Enqueue(PushQueueItem item);

        Task<PushQueueItem> DequeueAsync(CancellationToken cancellationToken);

    }
}
