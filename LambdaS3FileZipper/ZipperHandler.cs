using System;

namespace LambdaS3FileZipper
{
    public class ZipperHandler : IHandler
    {
	    private readonly IFileRetriever fileRetriever;
	    private readonly IFileZipper fileZipper;
	    private readonly IFileUploader fileUploader;

		public ZipperHandler(IFileRetriever fileRetriever, IFileZipper fileZipper, IFileUploader fileUploader)
		{
			this.fileRetriever = fileRetriever;
			this.fileZipper = fileZipper;
			this.fileUploader = fileUploader;
		}

	    public Response Handle(Request request)
	    {
		    throw new NotImplementedException();
	    }
    }
}
