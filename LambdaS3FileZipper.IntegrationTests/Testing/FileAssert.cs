using System.IO;
using System.IO.Compression;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Testing
{
	public static class FileAssert
	{
		/// <summary>
		/// Asserts that file at <see cref="filePath"/> exists
		/// </summary>
		/// <param name="filePath"></param>
		public static void Exists(string filePath)
		{
			var file = new FileInfo(filePath);
			Assert.That(file.Exists, Is.True);
		}

		/// <summary>
		/// Asserts that file at <see cref="filePath"/> exists and is non-empty
		/// </summary>
		/// <param name="filePath"></param>
		public static void HasContent(string filePath)
		{
			var file = new FileInfo(filePath);
			Assert.That(file.Exists, Is.True);
			Assert.That(file.Length, Is.GreaterThan(0));
		}

		/// <summary>
		/// Asserts that the ZIP file at <see cref="filePath"/> exists and is non-empty,
		/// and has at least one file compressed
		/// </summary>
		/// <param name="filePath"></param>
		public static void ZipHasFiles(string filePath)
		{
			HasContent(filePath);

			using var fileStream = File.OpenRead(filePath);
			using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
			Assert.That(zipArchive.Entries, Is.Not.Empty);
		}
	}
}
