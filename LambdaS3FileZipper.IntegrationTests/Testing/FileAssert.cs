using System.IO;
using System.IO.Compression;
using System.Linq;
using LambdaS3FileZipper.IntegrationTests.Logging;
using NUnit.Framework;

namespace LambdaS3FileZipper.IntegrationTests.Testing
{
	public static class FileAssert
	{
		private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

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
		/// <param name="expectedFileCount"></param>
		public static void ZipHasFiles(string filePath, int? expectedFileCount = default)
		{
			HasContent(filePath);

			using var fileStream = File.OpenRead(filePath);
			using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
			if (expectedFileCount.HasValue)
			{
				Assert.That(zipArchive.Entries.Count, Is.EqualTo(expectedFileCount.Value));
			}
			else
			{
				Assert.That(zipArchive.Entries, Is.Not.Empty);
			}
		}

		/// <summary>
		/// Asserts that the ZIP file at <see cref="filePath"/> exists and is non-empty,
		/// and has the provided entry file name
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="entryFileName"></param>
		public static void ZipHasFilesWithName(string filePath, string entryFileName)
		{
			HasContent(filePath);

			using var fileStream = File.OpenRead(filePath);
			using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
			foreach (var entry in zipArchive.Entries)
			{
				Log.Info("Found compressed file entry {EntryFullName}", entry.FullName);
			}

			Assert.That(zipArchive.Entries.Any(entry => entry.FullName == entryFileName), Is.True);
		}
	}
}
