using System.IO;
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
	}
}
