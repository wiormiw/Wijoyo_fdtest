namespace Wijoyo_fdtest.Infrastructure.Storage;

public class MinioSettings
{
    public string Endpoint { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string BucketName { get; set; } = "";
}

