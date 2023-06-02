namespace CloudBox.Api.Entities
{
    public class TblUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Status { get; set; } = null!;
        public byte[]? Avatar { get; set; } = null!;
        public int SercurityGroupId { get; set; }
        public DateTime LastLogonTime { get; set; }
    }
}
