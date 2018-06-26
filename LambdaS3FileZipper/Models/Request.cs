namespace LambdaS3FileZipper.Models
{
	public class Request
	{
        // Empty constructor required for serialization
        public Request()
        {

        }

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

		public string OriginBucketName { get; set; }
		public string OriginResourceName { get; set; }
		public string DestinationBucketName { get; set; }
		public string DestinationResourceName { get; set; }
	}
}