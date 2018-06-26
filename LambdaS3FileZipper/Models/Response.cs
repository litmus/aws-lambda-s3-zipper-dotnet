namespace LambdaS3FileZipper.Models
{
	public class Response
	{
        // Empty constructor needed for seriliazation 
        public Response()
        {

        }

		public Response(string url)
		{
			Url = url;
		}

		public string Url;
	}
}