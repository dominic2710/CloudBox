namespace CloudBox.Api.Helpers
{
    public static class FileAccessHelper
    {
        public static void CreateFolderIfNotExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
