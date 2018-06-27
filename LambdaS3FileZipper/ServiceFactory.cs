using System;
using Amazon.S3;
using LambdaS3FileZipper.Aws;

namespace LambdaS3FileZipper
{
	public class ServiceFactory
	{
		private static readonly Lazy<Service> Service = new Lazy<Service>(() =>
			new Service(
				new S3FileRetriever(new AwsS3Client(new AmazonS3Client())),
				new FileZipper(),
				new S3FileUploader(new AwsS3Client(new AmazonS3Client()))));

		public static Service BuildDefault() => Service.Value;
	}
}
