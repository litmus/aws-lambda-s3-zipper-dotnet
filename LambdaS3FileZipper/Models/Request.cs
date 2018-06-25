namespace LambdaS3FileZipper.Models
{
	public class Request
	{
		public Request(string bucketName, string resourceName)
		{
			BucketName = bucketName;
			ResourceName = resourceName;
		}

		public string BucketName { get; }
		public string ResourceName { get; }
	}
}