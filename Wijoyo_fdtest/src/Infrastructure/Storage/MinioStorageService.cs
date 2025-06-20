using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Application.Common.Models;

namespace Wijoyo_fdtest.Infrastructure.Storage;

public class MinioStorageService : IStorageService
{
    private readonly MinioSettings _settings;
    private readonly IMinioClient _minioClient;

    public MinioStorageService(IOptions<MinioSettings> settings)
    {
        _settings = settings.Value;

        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint.Replace("http://", "").Replace("https://", ""))
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.Endpoint.StartsWith("https"))
            .Build();
    }

    public async Task<string> UploadFileAsync(FileUploadRequest file)
    {
        // Ensure the bucket exists
        bool bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_settings.BucketName));
        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_settings.BucketName));
        }

        string policyJson = $@"
        {{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""Allow"",
                    ""Principal"": {{
                        ""AWS"": [""*""]
                    }},
                    ""Action"": [""s3:GetObject""],
                    ""Resource"": [""arn:aws:s3:::{_settings.BucketName}/*""]
                }}
            ]
        }}";

        await _minioClient.SetPolicyAsync(new SetPolicyArgs()
            .WithBucket(_settings.BucketName)
            .WithPolicy(policyJson));

        var objectName = $"{Guid.NewGuid()}_{file.FileName}";

        // Upload the file
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName)
            .WithStreamData(file.Content)
            .WithObjectSize(file.Content.Length)
            .WithContentType(file.ContentType));

        // Return object URL or name
        return $"{_settings.Endpoint}/{_settings.BucketName}/{objectName}";
    }
}