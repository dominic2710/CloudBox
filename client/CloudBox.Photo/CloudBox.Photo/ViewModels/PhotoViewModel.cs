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
        private int _minPageSize = 16;
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
        public PhotoViewModel()
        {
            
            ListAlbumThumnail = new ObservableCollection<string>();

            for (int i = 1; i <= 10; i++)
                ListAlbumThumnail.Add($"cloud_photo_logo.jpg");

            FetchAllPhoto();

            UploadPhotoCommand = new Command(OpenFile);
            RefreshCommand = new Command(FetchAllPhoto);
            FetchNextPhotoCommand = new Command(FetchNextPhoto);
        }

        public void FetchAllPhoto()
        {
            ListPhotoGroup = new ObservableCollection<PhotoGroupModel>();
            //ListPhoto = new ObservableCollection<PhotoModel>();
            Task.Run(async () =>
            {
                IsRefreshing = true;
                var response = await Services.ServiceProvider.GetInstance().CallWebApi<int,
                        GetListPhotoResponse>("/api/Photo/getlistphoto", HttpMethod.Post, 1);

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


                //foreach (var item in groupDate)
                //{
                //    var photoRange = photoToBeDisplay.Where(x => x.TruncateDate == item)
                //                                        .OrderByDescending(x => x.CreateDate)
                //                                        .ToList();
                //    ListPhotoGroup.Add(new PhotoGroupModel(item.ToString("dd/MM/yyyy"),
                //                                        photoRange));
                //}
                //_allPhotoList = 

                //var photoToBeDisplay = _allPhotoList.Take(_minPageSize).ToList();

                ////foreach (var photo in photoToBeDisplay)
                ////    ListPhoto.Add(photo);

                ////OnPropertyChanged(nameof(ListPhoto));

                //var groupDate = photoToBeDisplay

                //foreach (var item in groupDate)
                //{
                //    var photoRange = photoToBeDisplay.Where(x => x.TruncateDate == item)
                //                                        .OrderByDescending(x => x.CreateDate)
                //                                        .ToList();
                //    ListPhotoGroup.Add(new PhotoGroupModel(item.ToString("dd/MM/yyyy"),
                //                                        photoRange));
                //}
                OnPropertyChanged(nameof(ListPhotoGroup));
            }).GetAwaiter().OnCompleted(()=>
            {
                IsRefreshing = false;
            });
        }

        public void FetchNextPhoto()
        {
            if (IsRefreshing) return;

            //var displayedPhotoCount = ListPhoto.Count();
            //var displayedPhotoCount = ListPhotoGroup.Sum(x => x.Count());
            //if (displayedPhotoCount >= _allPhotoGroupList.Count) return;
            //if (_allPhotoGroupList.Count > 0)
            //{
            //    var photoToBeDisplay = _allPhotoGroupList.Skip(displayedPhotoCount).Take(_minPageSize).ToList();

            //    //foreach (var photo in photoToBeDisplay)
            //    //    ListPhoto.Add(photo);

            //    //OnPropertyChanged(nameof(ListPhoto));
            //    var groupDate = photoToBeDisplay.GroupBy(x => x.TruncateDate)
            //                                        .Select(x => x.Key)
            //                                        .OrderByDescending(x => x)
            //                                        .ToList();

            //    foreach (var item in groupDate)
            //    {
            //        var photoRange = photoToBeDisplay.Where(x => x.TruncateDate == item)
            //                                            .OrderByDescending(x => x.CreateDate)
            //                                            .ToList();
            //        var lastedGroup = ListPhotoGroup.Where(x => x.Where(x => x.TruncateDate == item) != null)
            //                                .FirstOrDefault();
            //        if (lastedGroup != null && lastedGroup.Count > 0)
            //        {
            //            lastedGroup.AddRange(photoRange);
            //        }
            //        else
            //        {
            //            ListPhotoGroup.Add(new PhotoGroupModel(item.ToString("dd/MM/yyyy"),
            //                                                photoRange));
            //        }
            //    }
            //    OnPropertyChanged(nameof(ListPhotoGroup));
            //}
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
                    //for (int i = 0; i < selectedFiles.Count(); i++)
                    //{
                    //    uploadingRange[i].Id = result[i].Id;
                    //    uploadingRange[i].IsUploading = false;
                    //    uploadingRange[i].IsDelete = result[i].IsDelete;
                    //    uploadingRange[i].OwnerUserId = result[i].OwnerUserId;
                    //    uploadingRange[i].ThumbnailId = result[i].ThumbnailId;
                    //    uploadingRange[i].CreateDate = result[i].CreateDate;
                    //    uploadingRange[i].Height = result[i].Height;
                    //    uploadingRange[i].Width = result[i].Width;
                    //    uploadingRange[i].Title = result[i].Title;
                    //}
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
