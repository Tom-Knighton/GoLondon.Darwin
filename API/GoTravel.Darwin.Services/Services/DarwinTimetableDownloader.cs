using System.IO.Compression;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using S3Object = Amazon.S3.Model.S3Object;

namespace GoTravel.Darwin.Services.Services;

public class DarwinTimetableDownloader(IConfiguration config, ILogger<DarwinConsumer> log)
{

    public async Task DownloadTimetable()
    {
        var darwinConfig = config.GetSection("Darwin").GetSection("S3");
        var access = darwinConfig["AccessKey"];
        var secret = darwinConfig["SecretKey"];
        var bucket = darwinConfig["Bucket"];
        var prefix = darwinConfig["Prefix"];
        var region = RegionEndpoint.EUWest1;

        using var client = new AmazonS3Client(access, secret, region);
        var request = new ListObjectsV2Request
        {
            BucketName = bucket,
            Prefix = prefix
        };

        var response = await client.ListObjectsV2Async(request);
        var files = FilesFromToday(response.S3Objects);
        var fileToDownload = files.FirstOrDefault(f => f.Key.Contains("_v8"));

        if (fileToDownload is null)
        {
            log.LogError("Failed to find _v8 timetable");
            throw new Exception("Failed to find _v8 timetable");
        }

        var file = await client.GetObjectAsync(bucket, fileToDownload.Key);
        if (file is null)
        {
            log.LogError("Failed to download _v8 file");
            throw new Exception("Failed to download _v8 file");
        }

        using var stream = new MemoryStream();
        await file.ResponseStream.CopyToAsync(stream);
        using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();

        
    }
    
    private IEnumerable<S3Object> FilesFromToday(ICollection<S3Object> files)
    {
        var dateString = DateTime.UtcNow.ToString("yyyyMMdd");
        var filesFromToday = files.Where(x => x.Key.StartsWith($"PPTimetable/{dateString}"));
        var fromToday = filesFromToday.ToList();
        if (fromToday.Count != 0) return fromToday;
        
        dateString = DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");
        filesFromToday = files.Where(x => x.Key.StartsWith($"PPTimetable/{dateString}"));
        if (fromToday.Count != 0)
        {
            log.LogCritical("No Darwin timetable files found for today or yesterday");
        }

        return filesFromToday;
    }
}