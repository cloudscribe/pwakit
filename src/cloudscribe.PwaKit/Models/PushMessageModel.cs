using System.Collections.Generic;

namespace cloudscribe.PwaKit.Models
{
    public class PushMessageModel
    {
        public PushMessageModel()
        {
            Vibrate = new List<int>()
            {
                100, 50, 100 //a phone would vibrate for 100 milliseconds, pause for 50 milliseconds, and then vibrate again for 100 milliseconds
            };
        }

        /// <summary>
        /// this can be used to send messages to serviceworker that don't popup any notification by setting to something other than "Visible"
        /// the idea being that additional js can be added to service worker to handle non visual methods
        /// ie silently updating the pre-cache
        /// </summary>
        public string MessageType { get; set; } = "Visible";

        public string Title { get; set; } = "My Title";

        /// <summary>
        /// main message body up to 2 lines of text
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The icon option is essentially a small image you can show next to the title and body text.
        /// </summary>
        public string Icon { get; set; } = "/media/images/push-notification-icon.png";

        /// <summary>
        /// The image option can be used to display a larger image to the user. This is particularly useful to display a preview image to the user.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The badge is a small monochrome icon that is used to portray a little more information to the user about where the notification is from.
        /// At the time of writing the badge is only used on Chrome for Android.
        /// </summary>
        public string Badge { get; set; }

        /// <summary>
        /// The vibrate option specifies a vibration pattern for a phone receiving the notification. Example [100, 50, 100], a phone would vibrate for 100 milliseconds, pause for 50 milliseconds, and then vibrate again for 100 milliseconds.
        /// </summary>
        public List<int> Vibrate { get; set; }

        /// <summary>
        /// url string to an mp3. At the time of writing no browser has support for this option.
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// auto, ltr, or rtl
        /// </summary>
        public string Dir { get; set; } = "auto";



        public string Tag { get; set; }

        public string Data { get; set; }

        public bool? RequireInteraction { get; set; }

        public bool? Renotify { get; set; }

        public bool? Silent { get; set; }

        /// <summary>
        /// Timestamp allows you to tell the platform the time when an event occurred that resulted in the push notification being sent.
        /// The timestamp should be the number of milliseconds since 00:00:00 UTC, which is 1 January 1970 (i.e. the unix epoch).
        /// </summary>
        public long? Timestamp { get; set; }

        /// <summary>
        /// You can defined actions to display buttons with a notification.
        /// At the time of writing only Chrome and Opera for Android support actions.
        /// </summary>
        public List<PushActionModel> Actions { get; set; }
    }
}
