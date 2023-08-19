using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Models
{
    public class PhotoGroupModel: List<PhotoModel>
    {
        public string GroupName { get; set; }
        public PhotoGroupModel(string groupName, List<PhotoModel> photos): base(photos)
        {
            GroupName = groupName;
        }
    }
}
