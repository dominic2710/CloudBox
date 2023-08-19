namespace CloudBox.Api.Entities
{
    public class TblThumnail
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
