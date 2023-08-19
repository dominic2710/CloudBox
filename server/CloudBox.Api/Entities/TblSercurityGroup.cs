namespace CloudBox.Api.Entities
{
    public class TblSercurityGroup
    {
        public int Id { get; set; }
        public string SercurityGroupName { get; set; } = null!;
        public string PermistionCode { get; set; } = null!;
    }
}
