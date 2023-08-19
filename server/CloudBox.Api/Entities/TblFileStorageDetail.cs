using System.Runtime.InteropServices;

namespace CloudBox.Api.Entities
{
    public class TblFileStorageDetail
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ObjectType { get; set; } = null!;
        public string StorageLocation { get; set; } = null!;
        public uint ObjectSize { get; set; } = 0!;
        public string ParentId { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
