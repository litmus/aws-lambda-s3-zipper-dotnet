using System.Threading.Tasks;

public interface IFileUploader
{
	Task Upload(string bucketName, string resourceName, string compressedFileName);
}