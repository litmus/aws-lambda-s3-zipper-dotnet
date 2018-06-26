using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using LambdaS3FileZipper.Interfaces;

namespace LambdaS3FileZipper
{
	public class FileZipper : IFileZipper
	{
		public async Task<string> Compress(string localDirectory)
		{
			if (!Directory.Exists(localDirectory))
			{
				throw new IOException($"{localDirectory} does not exist");
			}

			var parentDirectoryName = new DirectoryInfo(localDirectory).Name;
			var zipPath = Path.Combine(Path.GetTempPath(), $"{parentDirectoryName}.zip");

			await Task.Run(() => ZipFile.CreateFromDirectory(localDirectory, zipPath));

			return zipPath;
		}
	}
}