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

			var directoryName = Path.GetDirectoryName(localDirectory);
			var zipPath = Path.Combine(localDirectory, $"{directoryName}.zip");

			await Task.Run(() => ZipFile.CreateFromDirectory(localDirectory, zipPath));

			return zipPath;
		}
	}
}