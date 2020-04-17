using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
			var zipStream = new MemoryStream();
			var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create);
			foreach (var fileResponse in filesResponses)
			{
				var zipEntry = zipArchive.CreateEntry(fileResponse.ResourceKey);

				using var zipEntryStream = zipEntry.Open();
				using var fileContentStream = fileResponse.ContentStream;
				await fileContentStream.CopyToAsync(zipEntryStream, 4096, cancellationToken);
			}

			return new FileResponse(resourceKey: zipFileKey, contentStream: zipStream.Reset());
		}

		private async Task CreateFlatZip(string localDirectory, string zipPath)
		{
			using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
			{
				foreach (var file in GetFiles(localDirectory))
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

		private IEnumerable<string> GetFiles(string directory)
		{
			var files = new List<string>();

			foreach (var dir in Directory.GetDirectories(directory))
			{
				files.AddRange(Directory.GetFiles(dir));
				files.AddRange(GetFiles(dir));
			}

			return files;
		}
	}
}