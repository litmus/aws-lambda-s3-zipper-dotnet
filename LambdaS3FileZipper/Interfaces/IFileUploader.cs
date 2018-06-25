public interface IFileUploader
{
    Task Upload(string bucketName, string resourceName, string compressedFileName)
}