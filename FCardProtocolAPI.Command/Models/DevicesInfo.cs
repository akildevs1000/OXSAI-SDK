namespace FCardProtocolAPI.Command.Models
{
    public class DevicesInfo
    {
        public string SN { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }
    public class FileDevicesInfo
    {
        public List<DevicesInfo> DevicesInfos { get; set; }
    }
}
