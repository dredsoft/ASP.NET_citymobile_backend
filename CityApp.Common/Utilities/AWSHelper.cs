

namespace CityApp.Common.Utilities
{
    public static class AWSHelper
    {
        public static string GetS3Url(string key, string s3BucketUrl)
        {
            return $"{s3BucketUrl}{key}";
        }
    }
}
