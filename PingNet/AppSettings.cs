namespace PingNet
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// IP address range. First three octets only (e.g 192.168.1)
        /// </summary>
        public string IPAddressRange { get; set; }

        /// <summary>
        /// Path to the web browser on the host machine.
        /// </summary>
        public string BrowserPath { get; set; }
    }
}
