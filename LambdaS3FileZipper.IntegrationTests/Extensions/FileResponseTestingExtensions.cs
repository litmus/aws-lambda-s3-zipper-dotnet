using System.IO;
using System.Threading.Tasks;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.IntegrationTests.Extensions
{
	public static class FileResponseTestingExtensions
	{
		public static async Task<string> WriteToTempFile(this FileResponse fileResponse)
		{
			var tempFile = Path.GetTempFileName();
			await fileResponse.WriteTo(tempFile);

			return tempFile;
		}

		public static async Task WriteTo(this FileResponse fileResponse, string filePath)
		{
			using var fileStream = File.OpenWrite(filePath);
			await fileResponse.ContentStream.CopyToAsync(fileStream);
		}
	}
}
