using CloudBox.Api.Functions.Photo;
using CloudBox.Api.Functions.User;
using CloudBox.Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
//using System.Drawing.

namespace CloudBox.Api.Controllers.Photo
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private IPhotoFunction _photoFunction;
        private UserOperator _userOperator;

        const string UPLOAD_ROOT_PATH = @"D:\_TmpC\_cloudphoto2";

        public PhotoController(IPhotoFunction photoFunction, 
                                UserOperator userOperator)
        {
            _photoFunction = photoFunction;
            _userOperator = userOperator;
        }

        [HttpPost("upload")]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var userId = _userOperator.GetRequestUser().Id;
            //var userId = 1;

            FileAccessHelper.CreateFolderIfNotExist(Path.Combine(UPLOAD_ROOT_PATH, @$"images\{userId}\"));
            FileAccessHelper.CreateFolderIfNotExist(Path.Combine(UPLOAD_ROOT_PATH, @$"thumnails\{userId}\"));

            // Get the file extension
            var fileExtension = Path.GetExtension(file.FileName);

            // Generate a unique file name
            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + fileExtension;

            // Save the original file to the server's wwwroot/images folder
            var filePath = Path.Combine(UPLOAD_ROOT_PATH, @$"images\{userId}\", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Create the thumbnail
            var thumbnailFileName = fileName;
            var thumbnailFilePath = Path.Combine(UPLOAD_ROOT_PATH, @$"thumnails\{userId}\", thumbnailFileName);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var image = Image.FromStream(fs))
                {
                    var cropSize = image.Size.Height > image.Size.Width ? image.Size.Width : image.Size.Height;
                    var scrX = (image.Size.Width - cropSize) / 2;
                    var scrY = (image.Size.Height - cropSize) / 2;

                    var thumnailSize = 256;
                    var destRect = new Rectangle(0, 0, thumnailSize, thumnailSize);

                    using (var thumbnail = new Bitmap(thumnailSize, thumnailSize))
                    {
                        using (var graphics = Graphics.FromImage(thumbnail))
                        {
                            graphics.Clear(Color.Transparent);
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.DrawImage(image, destRect, scrX, scrY, cropSize, cropSize, GraphicsUnit.Pixel);
                        }

                        // Save the thumbnail to the server's wwwroot/images folder
                        thumbnail.Save(thumbnailFilePath, ImageFormat.Jpeg);
                    }

                    _photoFunction.Create(fileName, image.Width, image.Height);
                }
            }

            // Return the URL of the uploaded image and its thumbnail
            //var imageUrl = Url.Content($"~images/{userId}/" + fileName);
            //var thumbnailUrl = Url.Content($"~thumnails/{userId}/" + thumbnailFileName);
            return Ok();
        }

        [HttpPost("uploadmultifile")]
        public IActionResult UploadImages([FromForm] IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files were uploaded.");

            try
            {
                var userId = _userOperator.GetRequestUser().Id;

                var imagesDirectory = Path.Combine(UPLOAD_ROOT_PATH, $@"images\{userId}\");
                var thumnailsDirectory = Path.Combine(UPLOAD_ROOT_PATH, $@"thumnails\{userId}\");

                FileAccessHelper.CreateFolderIfNotExist(Path.Combine(imagesDirectory));
                FileAccessHelper.CreateFolderIfNotExist(Path.Combine(thumnailsDirectory));

                var entities = new List<Functions.Photo.Photo>();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                        continue;

                    // Get the file extension
                    var fileExtension = Path.GetExtension(file.FileName);

                    // Generate a unique file name
                    var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + fileExtension;

                    // Save the original file to the images directory
                    var originalFilePath = Path.Combine(imagesDirectory, fileName);

                    using (var stream = new FileStream(originalFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Create the thumbnail with preserved transparency
                    var thumbnailFileName = fileName;
                    var thumbnailFilePath = Path.Combine(thumnailsDirectory, thumbnailFileName);

                    using (var image = Image.FromFile(originalFilePath))
                    {
                        var cropSize = image.Size.Height > image.Size.Width ? image.Size.Width : image.Size.Height;
                        var scrX = (image.Size.Width - cropSize) / 2;
                        var scrY = (image.Size.Height - cropSize) / 2;

                        var thumnailSize = 256;
                        var destRect = new Rectangle(0, 0, thumnailSize, thumnailSize);

                        using (var thumbnail = new Bitmap(thumnailSize, thumnailSize))
                        {
                            using (var graphics = Graphics.FromImage(thumbnail))
                            {
                                graphics.Clear(Color.Transparent);
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.DrawImage(image, destRect, scrX, scrY, cropSize, cropSize, GraphicsUnit.Pixel);
                            }

                            // Save the thumbnail to the server's wwwroot/images folder
                            thumbnail.Save(thumbnailFilePath, ImageFormat.Jpeg);
                        }

                        var entity = _photoFunction.Create(fileName, image.Width, image.Height);
                        entities.Add(entity);
                    }
                }

                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading the images: {ex.Message}");
            }
        }

        [HttpPost("getlistphoto")]
        public IActionResult GetPhotoByUserId([FromBody] int userId)
        {
            var photos = _photoFunction.GetListPhoto(userId);
            var response = new GetListPhotoResponse
            {
                ListPhoto = photos
            };
            return Ok(response);
        }

        [HttpGet("getphoto/{userId}/{fileName}")]
        public IActionResult GetPhoto(string userId, string fileName)
        {
            var imagesDirectory = Path.Combine(UPLOAD_ROOT_PATH, $"images/{userId}/");
            var filePath = Path.Combine(imagesDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var imageBytes = System.IO.File.ReadAllBytes(filePath);
            return File(imageBytes, "image/jpeg"); // You can adjust the content type based on your image format
        }

        [HttpGet("getthumnail/{userId}/{fileName}")]
        public IActionResult GetThumnail(string userId, string fileName)
        {
            var imagesDirectory = Path.Combine(UPLOAD_ROOT_PATH, $"thumnails/{userId}/");
            var filePath = Path.Combine(imagesDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var imageBytes = System.IO.File.ReadAllBytes(filePath);
            return File(imageBytes, "image/jpeg"); // You can adjust the content type based on your image format
        }
    }
}
