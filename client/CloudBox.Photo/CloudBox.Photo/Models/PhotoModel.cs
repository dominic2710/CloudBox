using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Models
{
    public class PhotoModel:BaseModel
    {
        private string title;
        public int Id { get; set; }
        public string Title 
        {
            get { return title; }
            set { title = value; }
        }
        public int OwnerUserId { get; set; }
        public int ThumbnailId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime TruncateDate
        {
            get { return new DateTime(CreateDate.Year, CreateDate.Month, CreateDate.Day); }
        }
        public bool IsUploading { get; set; }
        public bool IsDownloading { get; set; }
        public string Url
        {
            //set { 
            //    Url = value; 
            //    //OnPropertyChanged(); 
            //}
            get { return $"/api/photo/getthumnail/{OwnerUserId}/{Title}"; }
        }

    }
}
