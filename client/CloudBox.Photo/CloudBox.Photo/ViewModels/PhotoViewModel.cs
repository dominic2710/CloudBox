using CloudBox.Photo.Helpers;
using CloudBox.Photo.Models;
using CloudBox.Photo.Services;
using CloudBox.Photo.Services.Photo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CloudBox.Photo.ViewModels
{
    public class PhotoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private int _minPageSize = 15;
        private int _currentPage = 0;
        private List<PhotoGroupModel> _allPhotoGroupList;
        private ObservableCollection<string> listAlbumThumnail;
        private ObservableCollection<PhotoGroupModel> listPhotoGroup;
        private ObservableCollection<PhotoModel> listPhoto;
        private bool isRefreshing;

        public ObservableCollection<string> ListAlbumThumnail
        {
            get { return listAlbumThumnail; }
            set { listAlbumThumnail = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PhotoGroupModel> ListPhotoGroup
        {
            get { return listPhotoGroup; }
            set { listPhotoGroup = value; OnPropertyChanged(); }
        }
        public ObservableCollection<PhotoModel> ListPhoto
        {
            get { return listPhoto; }
            set { listPhoto = value; OnPropertyChanged(); }
        }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { isRefreshing = value; OnPropertyChanged(); }
        }
        public ICommand UploadPhotoCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand FetchNextPhotoCommand { get; set; }
        public ICommand ViewPhotoCommand { get; set; }
        public PhotoViewModel()
        {
            
            ListAlbumThumnail = new ObservableCollection<string>();

            for (int i = 1; i <= 10; i++)
                ListAlbumThumnail.Add($"cloud_photo_logo.jpg");

            FetchAllPhoto();

            UploadPhotoCommand = new Command(OpenFile);
            RefreshCommand = new Command(FetchAllPhoto);
            FetchNextPhotoCommand = new Command(FetchNextPhoto);
            ViewPhotoCommand = new Command(p => ViewPhoto(p as PhotoModel));
        }

        public void FetchAllPhoto()
        {
            ListPhotoGroup = new ObservableCollection<PhotoGroupModel>();
            Task.Run(async () =>
            {
                IsRefreshing = true;
                _currentPage = 0;
                var response = await Services.ServiceProvider.GetInstance().CallWebApi<int,
                        GetListPhotoResponse>("/api/Photo/getlistphoto", HttpMethod.Post, Global.LOGIN_USERID);

                var photos = response.ListPhoto.OrderByDescending(x => x.CreateDate).ToList();
                _allPhotoGroupList = photos.GroupBy(x => x.TruncateDate)
                                                    .Select(x => new PhotoGroupModel
                                                    (
                                                        x.Key.ToString("dd/MM/yyyy"),
                                                        x.ToList<PhotoModel>()
                                                    ))
                                                    .ToList()
                                                    .OrderByDescending(x => x.GroupName)
                                                    .ToList();

                var minDisplayCount = (_currentPage * _minPageSize) + _minPageSize;
                var displayCount = 0;
                var groupItemCount = 0;
                foreach (var group in _allPhotoGroupList)
                {
                    groupItemCount += 1;
                    displayCount += group.Count();
                    if (displayCount > minDisplayCount) break;
                }

                for (int i = ListPhotoGroup.Count; i < groupItemCount; i++)
                {
                    ListPhotoGroup.Add(_allPhotoGroupList[i]);
                }
                _currentPage++;

                 OnPropertyChanged(nameof(ListPhotoGroup));
            }).GetAwaiter().OnCompleted(()=>
            {
                IsRefreshing = false;
            });
        }

        public void FetchNextPhoto()
        {
            if (IsRefreshing) return;

            var minDisplayCount = (_currentPage * _minPageSize) + _minPageSize;
            var displayCount = 0;
            var groupItemCount = 0;
            foreach (var group in _allPhotoGroupList)
            {
                groupItemCount += 1;
                displayCount += group.Count();
                if (displayCount > minDisplayCount) break;
            }

            for (int i = ListPhotoGroup.Count; i < groupItemCount; i++)
            {
                ListPhotoGroup.Add(_allPhotoGroupList[i]);
            }
            _currentPage++;
        }

        public void ViewPhoto(PhotoModel photo)
        {
            Shell.Current.GoToAsync($"ViewPhotoPage?Url={photo.PhotoUrl}");
        }

        async void OpenFile(object commandParameter)
        {
            try
            {
                var selectedFiles = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images
                });

                if (selectedFiles != null && selectedFiles.Count() > 0)
                {
                    var uploadingRange = new List<PhotoModel>();
                    for (int i=0; i< selectedFiles.Count(); i++)
                    {
                        uploadingRange.Add(new PhotoModel
                        {
                            IsUploading = true,
                        });
                    }
                    ListPhotoGroup.Insert(0, new PhotoGroupModel("Just now", uploadingRange));

                    var formData = new MultipartFormDataContent();

                    foreach (var imageFile in selectedFiles)
                    {
                        var imageStream = await imageFile.OpenReadAsync();
                        var streamContent = new StreamContent(imageStream);
                        formData.Add(streamContent, "files", imageFile.FileName);
                    }

                    var result = await Services.ServiceProvider.GetInstance().UploadPhoto(formData);
                }
                FetchAllPhoto();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
