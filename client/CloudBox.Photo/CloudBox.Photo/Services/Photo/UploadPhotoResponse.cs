using CloudBox.Photo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Services.Photo
{
    public class UploadPhotoResponse: BaseResponse
    {
        List<PhotoModel> ListPhotos { get; set; }
    }
}
