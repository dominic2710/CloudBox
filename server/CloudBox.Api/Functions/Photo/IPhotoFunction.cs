namespace CloudBox.Api.Functions.Photo
{
    public interface IPhotoFunction
    {
        Photo Create(string title, int width, int height);
        List<Photo> GetListPhoto(int userId);
        Photo? GetPhoto(int photoId);
        //byte[] GetPhotoData(int userId, string fileName);
        //byte[] GetThumnailData(int userId, string fileName);
    }
}
