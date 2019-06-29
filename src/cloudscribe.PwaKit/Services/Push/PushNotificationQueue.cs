using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    internal class PushNotificationQueue : IPushNotificationsQueue
    {
        private readonly ConcurrentQueue<PushQueueItem> _queue = new ConcurrentQueue<PushQueueItem>();
        private readonly SemaphoreSlim _messageEnqueuedSignal = new SemaphoreSlim(0);

        public void Enqueue(PushQueueItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _queue.Enqueue(item);

            _messageEnqueuedSignal.Release();
        }

        public async Task<PushQueueItem> DequeueAsync(CancellationToken cancellationToken)
        {
            await _messageEnqueuedSignal.WaitAsync(cancellationToken);

            _queue.TryDequeue(out PushQueueItem message);

            return message;
        }
    }
}
