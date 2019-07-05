namespace cloudscribe.PwaKit.Models
{
    public class PushClientSettings
    {
        public string Subject { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Subject) && !string.IsNullOrWhiteSpace(PublicKey) && !string.IsNullOrWhiteSpace(PrivateKey);
        }
    }
}
