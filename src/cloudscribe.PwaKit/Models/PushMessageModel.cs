using System;
using System.Collections.Generic;
using System.Text;

namespace cloudscribe.PwaKit.Models
{
    public class PushMessageModel
    {
        public string Title { get; set; } = "My Title";
        public string Body { get; set; }
        public string Icon { get; set; } = "/media/images/push-notification-icon.png";
    }
}
