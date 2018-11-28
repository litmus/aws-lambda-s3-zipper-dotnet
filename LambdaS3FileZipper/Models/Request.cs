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
			string destinationResourceName,
			bool flatZipFile)
		{
			OriginBucketName = originBucketName;
			OriginResourceName = originResourceName;
			DestinationBucketName = destinationBucketName;
			DestinationResourceName = destinationResourceName;
			FlatZipFile = flatZipFile;
		}

		public string OriginBucketName { get; set; }
		public string OriginResourceName { get; set; }
		public string DestinationBucketName { get; set; }
		public string DestinationResourceName { get; set; }
		public bool FlatZipFile { get; set; }
	}
}