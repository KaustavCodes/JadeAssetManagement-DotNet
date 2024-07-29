using BytesAssetManagement.Base;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;

namespace BytesAssetManagement.AwsFileSystem;

public class AwsFileSystemManager : IFileSystemBase
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private int _pageSize = 10;

    private string _rootPath = "";

    /// <summary>
    /// This constructor accepts a dynamic object and expects a parameter called PageSize.
    /// </summary>
    /// <param name="configuration">PageSize as integer. eg 10</param>
    public AwsFileSystemManager(AwsFileSystemConfig configuration)
    {
        if (configuration.PageSize != null)
            _pageSize = configuration.PageSize;
        
        if (configuration.RootPath != null)
            _rootPath = configuration.RootPath;
        
        _s3Client = new AmazonS3Client(configuration.AccessKey, configuration.SecretKey, Amazon.RegionEndpoint.GetBySystemName(configuration.Region));
            _bucketName = configuration.BucketName;
    }

    public AwsFileSystemManager(IConfiguration configuration)
    {
        _pageSize = Convert.ToInt32(configuration["JadedFileSystemConfig:PageSize"]);
        _rootPath = configuration["JadedFileSystemConfig:RootPath"].ToString();
        _s3Client = new AmazonS3Client(configuration["JadedFileSystemConfig:AccessKey"], configuration["JadedFileSystemConfig:SecretKey"], Amazon.RegionEndpoint.GetBySystemName(configuration["JadedFileSystemConfig:Region"]));
    }

    public async Task<bool> DeleteDirectoryAsync(string relativeDirectoryPath)
    {
        try
        {
            // Ensure the directory path ends with a '/' to represent a folder structure in S3
            if (!relativeDirectoryPath.EndsWith("/"))
            {
                relativeDirectoryPath += "/";
            }

            // List all objects with the specified prefix (directoryPath)
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = relativeDirectoryPath
            };

            ListObjectsV2Response listResponse;
            do
            {
                listResponse = await _s3Client.ListObjectsV2Async(listRequest);

                // Delete the objects found with the specified prefix
                foreach (var s3Object in listResponse.S3Objects)
                {
                    await _s3Client.DeleteObjectAsync(new Amazon.S3.Model.DeleteObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = s3Object.Key
                    });
                }

                // Set the continuation token to continue listing more objects
                listRequest.ContinuationToken = listResponse.NextContinuationToken;
            } while (listResponse.IsTruncated); // Continue while there are more objects to list

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync(string relativeFilePath)
    {
        try
        {
            var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = relativeFilePath
            };
            var response = await _s3Client.DeleteObjectAsync(deleteRequest);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            // Log exception
            return false;
        }
    }

    public async Task<AssetTypes> GetFileAsync(string relativeFilePath)
    {
        try
        {
            // Get the object from S3
            var response = await _s3Client.GetObjectAsync(_bucketName, relativeFilePath);

            // Create an instance of AssetTypes and map the properties
            var asset = new AssetTypes
            {
                Name = System.IO.Path.GetFileName(relativeFilePath),
                IsFolder = false, // Assuming this method is only used for files
                Path = relativeFilePath,
                Extension = System.IO.Path.GetExtension(relativeFilePath),
                MimeType = response.Headers.ContentType,
                SizeInBytes = (int)response.ResponseStream.Length, // Be cautious with large files
                DateCreated = response.Metadata["x-amz-meta-datecreated"] != null ? DateTime.Parse(response.Metadata["x-amz-meta-datecreated"]) : DateTime.MinValue,
                DateModified = response.LastModified
            };

            return asset;
        }
        catch (AmazonS3Exception ex)
        {
            // Handle specific S3 exceptions
            throw;
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw;
        }
    }

    /// <summary>
    /// List all files in the specified directory. Here we do not provide Created Date, Modified Date, and Size to avoid unnecessary S3 requests.
    /// </summary>
    /// <param name="relativePath">Path after the root path already provided after initialization.</param>
    /// <param name="searchKey">Filter the records by this search keyword</param>
    /// <returns></returns>
    public async Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "/", string searchKey = "")
    {
        var path = Path.Combine(_rootPath, relativePath);
        var client = new AmazonS3Client();
        var request = new ListObjectsV2Request
        {
            BucketName = "your-bucket-name",
            Prefix = path.TrimStart('/'),
            ContinuationToken = null
        };

        var assets = new List<AssetTypes>();

        ListObjectsV2Response response;
        do
        {
            response = await client.ListObjectsV2Async(request);
            foreach (var obj in response.S3Objects)
            {
                if (string.IsNullOrEmpty(searchKey) || obj.Key.Contains(searchKey))
                {
                    string extension = System.IO.Path.GetExtension(path);
                    assets.Add(new AssetTypes()
                    {
                        Name = System.IO.Path.GetFileName(path),
                        IsFolder = false, // Assuming this method is only used for files
                        Path = path,
                        Extension = extension,
                        MimeType = Helpers.GetMimeType(extension),
                        SizeInBytes = -1, // Be cautious with large files
                        DateCreated = DateTime.MinValue,
                        DateModified = DateTime.MinValue
                    });
                }
            }
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);

        return assets;
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0)
    {
        var path = Path.Combine(_rootPath, relativePath);
        var client = new AmazonS3Client();
        var request = new ListObjectsV2Request
        {
            BucketName = "your-bucket-name",
            Prefix = path.TrimStart('/'),
            MaxKeys = pageSize,
            ContinuationToken = null
        };

        var assets = new List<AssetTypes>();
        int pageCounter = 0;

        ListObjectsV2Response response;
        do
        {
            response = await client.ListObjectsV2Async(request);
            if (pageCounter == currentPage)
            {
                foreach (var obj in response.S3Objects)
                {
                    if (string.IsNullOrEmpty(searchKey) || obj.Key.Contains(searchKey))
                    {
                        assets.Add(new AssetTypes { /* Populate your AssetTypes object based on the S3 object */ });
                    }
                }
                break;
            }
            request.ContinuationToken = response.NextContinuationToken;
            pageCounter++;
        } while (response.IsTruncated && pageCounter <= currentPage);

        return assets;
    }

    public async Task<bool> UploadFileAsync(byte[] fileContent, string relativeDestinationPath)
    {
        var path = Path.Combine(_rootPath, relativeDestinationPath);
        try
        {
            using (var stream = new MemoryStream(fileContent))
            {
                var putRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = path,
                    InputStream = stream
                };
                var response = await _s3Client.PutObjectAsync(putRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }
        catch (Exception ex)
        {
            // Log exception
            return false;
        }
    }
}