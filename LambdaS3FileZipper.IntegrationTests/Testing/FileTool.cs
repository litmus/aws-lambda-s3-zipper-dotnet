using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Extensions;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper.IntegrationTests.Testing
{
	public static class FileTool
	{
		private static readonly CancellationToken cancellationToken = CancellationToken.None;

		public static void TryDeleteFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public static void TryDeleteDirectory(string directoryPath, bool recursive = true)
		{
			if (Directory.Exists(directoryPath))
			{
				Directory.Delete(directoryPath, recursive);
			}
		}

		public static async Task<FileResponse> CreateTempTextFile(string directory, string fileKey, string fileContent)
		{
			var filePath = Path.Combine(directory, fileKey);
			await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);

			using var fileStream = File.OpenRead(filePath);
			return new FileResponse(resourceKey: fileKey, contentStream: await fileStream.CopyStreamOntoMemory(cancellationToken));
		}

		public static string CreateDirectoryIn(string parentDirectoryPath, string directoryName) =>
			Directory.CreateDirectory(Path.Combine(parentDirectoryPath, directoryName)).FullName;
	}
}
