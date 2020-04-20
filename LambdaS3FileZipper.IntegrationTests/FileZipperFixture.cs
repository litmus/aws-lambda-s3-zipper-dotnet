using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Extensions;
using LambdaS3FileZipper.IntegrationTests.Extensions;
using LambdaS3FileZipper.Models;
using NUnit.Framework;
using FileAssert = LambdaS3FileZipper.IntegrationTests.Testing.FileAssert;

namespace LambdaS3FileZipper.IntegrationTests
{
	[TestFixture]
	public class FileZipperFixture
	{
		private readonly FileZipper fileZipper;

		private CancellationToken cancellationToken;

		public FileZipperFixture()
		{
			fileZipper = new FileZipper();

			cancellationToken = CancellationToken.None;
		}

		[Test]
		public async Task Compress_ShouldZipFolder()
		{
			var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(tempDirectory);

			var tempFile = Path.Combine(tempDirectory, "compress.txt");
			await File.WriteAllTextAsync(tempFile, "Compress test.");

			try
			{
				var zipFile = await fileZipper.Compress(tempDirectory);

				Assert.True(File.Exists(zipFile));
			}
			finally
			{
				if (Directory.Exists(tempDirectory))
				{
					Directory.Delete(tempDirectory, recursive: true);
				}
		
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}
			}
		}

		[Test]
		public async Task Compress_ShouldZipFilesInMemory()
		{
			var tempDirectory = Guid.NewGuid().ToString();
			var tempDirectoryPath = Path.Combine(Path.GetTempPath(), tempDirectory);
			Directory.CreateDirectory(tempDirectoryPath);

			var zipFileKey = $"{tempDirectory}.zip";
			var zipFilePath = Path.Combine(Path.GetTempPath(), zipFileKey);

			try
			{
				var tempFileNames = new[] {"text-file-0.txt", "text-file-1.txt"};
				var fileResponses = await Task.WhenAll(tempFileNames
					.Select(fileName => CreateTempTextFile(tempDirectoryPath, fileName, fileContent: fileName))
					.ToArray());

				var zipFileResponse = await fileZipper.Compress(zipFileKey, fileResponses, cancellationToken);

				await zipFileResponse.WriteTo(zipFilePath);
				Console.WriteLine("Created zipFilePath at {0}", zipFilePath);
				Debugger.Break();

				FileAssert.HasContent(zipFilePath);
			}
			finally
			{
				if (Directory.Exists(tempDirectoryPath)) { Directory.Delete(tempDirectoryPath, recursive: true); }
				if (File.Exists(zipFilePath)) { File.Delete(zipFilePath); }
			}
		}

		private async Task<FileResponse> CreateTempTextFile(string directory, string fileKey, string fileContent)
		{
			var filePath = Path.Combine(directory, fileKey);
			await File.WriteAllTextAsync(filePath, fileContent, cancellationToken);

			using var fileStream = File.OpenRead(filePath);
			return new FileResponse(resourceKey: fileKey, contentStream: await fileStream.CopyStreamOntoMemory(cancellationToken));
		}
	}
}