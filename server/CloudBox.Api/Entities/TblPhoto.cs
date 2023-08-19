namespace CloudBox.Api.Entities
{
    public class TblPhoto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int OwnerUserId { get; set; }
        public int ThumbnailId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
