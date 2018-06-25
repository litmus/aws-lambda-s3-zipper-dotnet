namespace LambdaS3FileZipper.Models
{
	public class Request
	{
		public Request(
			string originBucketName,
			string originResourceName,
			string destinationBucketName,
			string destinationResourceName)
		{
			OriginBucketName = originBucketName;
			OriginResourceName = originResourceName;
			DestinationBucketName = destinationBucketName;
			DestinationResourceName = destinationResourceName;
		}

		public string OriginBucketName { get; }
		public string OriginResourceName { get; }
		public string DestinationBucketName { get; }
		public string DestinationResourceName { get; }
	}
}