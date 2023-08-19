namespace CloudBox.Api.Entities
{
    public class TblSubscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public uint Cost { get; set; }
        public uint TotalSize { get; set; }

    }
}
