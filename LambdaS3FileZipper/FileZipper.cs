using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Extensions;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
	public class FileZipper : IFileZipper
	{
		public async Task<string> Compress(string localDirectory, bool flat = false)
		{
			if (!Directory.Exists(localDirectory))
			{
				throw new IOException($"{localDirectory} does not exist");
			}

			var parentDirectoryName = new DirectoryInfo(localDirectory).Name;
			var zipPath = Path.Combine(Path.GetTempPath(), $"{parentDirectoryName}.zip");

			if (File.Exists(zipPath))
			{
				File.Delete(zipPath);
			}

			if (flat)
			{
				await CreateFlatZip(localDirectory, zipPath);
			}
			else
			{
				await Task.Run(() => ZipFile.CreateFromDirectory(localDirectory, zipPath));
			}
			
			return zipPath;
		}

		public async Task<FileResponse> Compress(string zipFileKey, IEnumerable<FileResponse> filesResponses, CancellationToken cancellationToken)
		{
			var zipMemoryStream = new MemoryStream();

			// In order to allow consumption of the compressed stream:
			// (1) leaveOpen => true
			// (2) ZipArchive object needs to be disposed to prevent compressed stream/ZIP file corruption
			// (disposing runs a number of required finalizers, while keeping stream open :shrug:)
			// For more info, visit: https://stackoverflow.com/a/17939367/1250033
			using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, leaveOpen: true))
			{
				foreach (var fileResponse in filesResponses)
				{
					var zipEntry = zipArchive.CreateEntry(fileResponse.ResourceKey);

					using var zipEntryStream = zipEntry.Open();
					using var fileContentStream = fileResponse.ContentStream;
					await fileContentStream.CopyToAsync(zipEntryStream, 4096, cancellationToken);
				}
			}

			return new FileResponse(resourceKey: zipFileKey, contentStream: zipMemoryStream.Reset());
		}

		private async Task CreateFlatZip(string localDirectory, string zipPath)
		{
			using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
			{
				foreach (var file in GetFilesRecursively(localDirectory))
				{
					var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));

					using(var fileReader = File.OpenRead(file))
					using (var zipStream = zipEntry.Open())
					{
						await fileReader.CopyToAsync(zipStream);
					}
				}
			}
		}

		private static IEnumerable<string> GetFilesRecursively(string directory)
		{
			foreach (var filePath in Directory.GetFiles(directory))
			{
				yield return filePath;
			}

			foreach (var filePath in Directory.GetDirectories(directory).SelectMany(GetFilesRecursively))
			{
				yield return filePath;
			}
		}
	}
}