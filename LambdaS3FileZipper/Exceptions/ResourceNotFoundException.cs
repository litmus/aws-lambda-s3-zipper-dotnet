using System;

namespace LambdaS3FileZipper.Exceptions
{
    public class ResourceNotFoundException : ArgumentException
    {
        public ResourceNotFoundException(string bucket, string resource, string resourceExpressionPattern)
            : base($"No files found for resource `{bucket}/{resource}` that match expression `{resourceExpressionPattern}`")
        {
        }
    }
}
