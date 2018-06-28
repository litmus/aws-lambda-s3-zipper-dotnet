using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;

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