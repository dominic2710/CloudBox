using Microsoft.EntityFrameworkCore;

namespace CloudBox.Api.Entities
{
    public class CloudBoxContext : DbContext
    {
        public CloudBoxContext(DbContextOptions<CloudBoxContext> options) : base(options)
        { }

        public virtual DbSet<TblUser> TblUsers { get; set; } = null!;
        public virtual DbSet<TblFileStorageDetail> TblFileStorageDetails { get; set; } = null!;
        public virtual DbSet<TblPermission> TblPermissions { get; set; } = null!;
        public virtual DbSet<TblSercurityGroup> TblSercurityGroups { get; set; } = null!;
        public virtual DbSet<TblSubscription> TblSubscriptions { get; set; } = null!;
        public virtual DbSet<TblUserSubscription> TblUserSubscriptions { get; set; } = null!;
    }
}
