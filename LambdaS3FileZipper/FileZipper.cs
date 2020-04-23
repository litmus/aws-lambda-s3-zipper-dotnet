using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaS3FileZipper.Extensions;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Logging;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
	public class FileZipper : IFileZipper
	{
		private readonly ILog log;

		public FileZipper()
		{
			this.log = LogProvider.GetCurrentClassLogger();
		}

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

		public async Task<FileResponse> Compress(
			string zipFileKey,
			IEnumerable<FileResponse> filesResponses,
			bool flat = false,
			CancellationToken cancellationToken = default)
		{
			var zipMemoryStream = new MemoryStream();

			// In order to allow consumption of the compressed stream:
			// (1) leaveOpen => true
			// (2) ZipArchive object needs to be disposed to prevent compressed stream/ZIP file corruption
			// (disposing runs a number of required finalizers, while keeping stream open :shrug:)
			// For more info, visit: https://stackoverflow.com/a/17939367/1250033
			using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, leaveOpen: true))
			{
				var total = filesResponses.Count();
				var current = 0;

				foreach (var fileResponse in filesResponses)
				{
					var stopwatch = Stopwatch.StartNew();
					var entryName = flat ? Path.GetFileName(fileResponse.ResourceKey) : fileResponse.ResourceKey;
					var entry = zipArchive.CreateEntry(entryName);

					using var zipEntryStream = entry.Open();
					using var fileContentStream = fileResponse.ContentStream;
					await fileContentStream.CopyToAsync(zipEntryStream, 4096, cancellationToken);

					Interlocked.Increment(ref current);
					log.Trace("Compressed file {File} ({Current}/{Total}) | {Time}ms", fileResponse.ResourceKey, current, total, stopwatch.ElapsedMilliseconds);
				}
			}

			return new FileResponse(resourceKey: zipFileKey, contentStream: zipMemoryStream.Reset());
		}

		private async Task CreateFlatZip(string localDirectory, string zipPath)
		{
			using var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
			foreach (var filePath in GetFilesRecursively(localDirectory))
			{
				var zipEntry = zipArchive.CreateEntry(Path.GetFileName(filePath));

				using var fileReader = File.OpenRead(filePath);
				using var zipStream = zipEntry.Open();
				await fileReader.CopyToAsync(zipStream);
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