using CloudBox.Photo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Services.Photo
{
    public class GetListPhotoResponse: BaseResponse
    {
        public IEnumerable<PhotoModel> ListPhoto { get; set; }
    }
}
