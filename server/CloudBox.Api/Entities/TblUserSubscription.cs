using System.Net.NetworkInformation;

namespace CloudBox.Api.Entities
{
    public class TblUserSubscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionId { get; set; }
        public uint TotalSize { get; set; }
        public uint RemainSize { get; set; }
    }
}
