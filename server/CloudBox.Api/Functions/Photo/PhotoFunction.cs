using CloudBox.Api.Entities;
using CloudBox.Api.Functions.User;
using CloudBox.Api.Helpers;

namespace CloudBox.Api.Functions.Photo
{
    public class PhotoFunction: IPhotoFunction
    {
        private UserOperator _userOperator;
        private readonly CloudBoxContext _context;
        private readonly IConfiguration configuration;
        public PhotoFunction(UserOperator userOperator,
                            CloudBoxContext context,
                            IConfiguration configuration)
        {
            _userOperator = userOperator;
            _context = context;
            this.configuration = configuration; 
        }

        public List<Photo> GetListPhoto(int userId)
        {
            var entities = _context.TblPhotos.Where(x=>x.OwnerUserId == userId).ToList();
            return entities.Select(ToPhotoModel).ToList();
        }

        public Photo? GetPhoto(int photoId)
        {
            var entity = _context.TblPhotos.Where(x => x.Id == photoId).FirstOrDefault();
            return entity != null ? ToPhotoModel(entity) : null;
        }

        //public byte[] GetPhotoData(int userId, string fileName)
        //{
        //    var filePath = Path.Combine(configuration["PhotoStoreLocation"], 
        //                                "images", 
        //                                userId.ToString(), 
        //                                fileName);
        //    byte[] imageData;
        //    using (var stream = new FileStream(filePath, FileMode.Open))
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            stream.CopyTo(memoryStream);
        //            imageData = memoryStream.ToArray();
        //        }
        //    }
        //    return imageData;
        //}

        public byte[] GetThumnailData(int userId, string fileName)
        {
            var filePath = Path.Combine(configuration["PhotoStoreLocation"],
                                        "thumnails",
                                        userId.ToString(),
                                        fileName);
            if (!File.Exists(filePath)) return new byte[0];

            byte[] imageData;
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
            return imageData;
        }

        public Photo Create(string title, int width, int height)
        {
            var thumnail = new TblThumnail
            {
                Title = title,
                Width = width,
                Height = height,
                CreateDate = DateTime.Now,
            };
            _context.TblThumnails.Add(thumnail);
            _context.SaveChanges();

            var entity = new TblPhoto
            {
                Title = title,
                IsDelete = false,
                OwnerUserId = 1,
                ThumbnailId = thumnail.Id,
                Width = width,
                Height = height,
                CreateDate = DateTime.Now
            };
            _context.TblPhotos.Add(entity);
            var count = _context.SaveChanges();

            return ToPhotoModel(entity);
        }

        private Photo ToPhotoModel(TblPhoto entity)
        {
            return new Photo
            {
                Id = entity.Id,
                Title = entity.Title,
                Width = entity.Width,
                Height = entity.Height,
                CreateDate = entity.CreateDate,
                IsDelete = entity.IsDelete,
                OwnerUserId = entity.OwnerUserId,
                ThumbnailId = entity.ThumbnailId,
                ThumnailData = GetThumnailData(entity.OwnerUserId, entity.Title),
            };
        }
    }
}
