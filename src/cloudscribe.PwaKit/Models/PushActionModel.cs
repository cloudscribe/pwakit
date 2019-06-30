namespace cloudscribe.PwaKit.Models
{

    /// <summary>
    /// You can defined actions to display buttons with a notification.
    /// At the time of writing only Chrome and Opera for Android support actions.
    /// </summary>
    public class PushActionModel
    {
        /// <summary>
        /// essentially the ID of the action. The ID is used when detecting that the action button had been clicked
        /// </summary>
        public string Action { get; set; }

        public string Title { get; set; }
        public string Icon { get; set; }
    }
}
