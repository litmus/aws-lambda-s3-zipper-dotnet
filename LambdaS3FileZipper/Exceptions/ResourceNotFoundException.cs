using System;

namespace LambdaS3FileZipper.Exceptions
{
    public class ResourceNotFoundException : ArgumentException
    {
        public ResourceNotFoundException(string bucket, string resource) : base($"No files found under {bucket}:{resource}")
        {
        }
    }
}
