using System;
using LambdaS3FileZipper.Interfaces;
using LambdaS3FileZipper.Models;

namespace LambdaS3FileZipper
{
    public class Handler : IHandler
    {
	    private readonly IFileRetriever fileRetriever;
	    private readonly IFileZipper fileZipper;
	    private readonly IFileUploader fileUploader;

		public Handler(IFileRetriever fileRetriever, IFileZipper fileZipper, IFileUploader fileUploader)
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
