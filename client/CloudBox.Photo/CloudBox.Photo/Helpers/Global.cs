using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Helpers
{
    public static class Global
    {
        public static ImageSource IMAGE_LOADING = ImageSource.FromFile("image_icon.jpg");
        public static ImageSource IMAGE_LOAD_FAILED = ImageSource.FromFile("image_fail.png");
    }
}
