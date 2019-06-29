using cloudscribe.PwaKit.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    /// <summary>
    /// multiple implementations can be injected, they should all be Singletons
    /// </summary>
    public interface IPushNotificationRecipientProvider
    {
        string Name { get; }
        Task<IEnumerable<PushDeviceSubscription>> GetRecipients(PushQueueItem pushQueueItem, CancellationToken cancellationToken);

    }
}
