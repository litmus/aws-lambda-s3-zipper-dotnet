using System;

namespace LambdaS3FileZipper.Exceptions
{
    public class ResourceNotFoundException : ArgumentException
    {
        public ResourceNotFoundException(string bucket, string resource, string resourceExpressionPattern)
            : base("No files found for given bucket/resource that match the expression")
        {
            Bucket = bucket;
            Resource = resource;
            ResourceExpressionPattern = resourceExpressionPattern;
        }

        public string Bucket { get; set; }
        public string Resource { get; set; }
        public string ResourceExpressionPattern { get; set; }
    }
}
