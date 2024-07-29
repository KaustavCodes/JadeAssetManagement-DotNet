using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BytesAssetManagement.GoogleFileSystem;

public class GoogleFileSystemManager
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    private int _pageSize = 10;

    private string _rootPath = "";

    public GoogleFileSystemManager(string projectId, string bucketName, string credentialsPath)
    {
        // Set the environment variable to specify the credentials file.
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        }

        _storageClient = StorageClient.Create();
        _bucketName = bucketName;
    }

    public GoogleFileSystemManager(IConfiguration configuration)
    {
        string credentialsPath = configuration["GoogleFileSystem:CredentialsPath"];
        if(string.IsNullOrEmpty(credentialsPath))
        {
            throw new ArgumentNullException("CredentialsPath is missing in the configuration");
        }

        if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        }
        _storageClient = StorageClient.Create();
        _bucketName = configuration["GoogleFileSystem:BucketName"];
    }

    public async Task<bool> UploadFileAsync(string filePath, string destinationBlobName)
    {
        try
        {
            string finalPath = Path.Combine(_rootPath, destinationBlobName);
            using var fileStream = File.OpenRead(filePath);
            await _storageClient.UploadObjectAsync(_bucketName, finalPath, null, fileStream);
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            string finalPath = Path.Combine(_rootPath, blobName);
            await _storageClient.DeleteObjectAsync(_bucketName, blobName);
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            return false;
        }
    }

    public async Task<List<string>> ListFilesAsync(string prefix = "")
    {
        var fileList = new List<string>();
        try
        {
            var options = new ListObjectsOptions
            {
                PageSize = _pageSize
            };

            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix, options);
            await foreach (var storageObject in objects)
            {
                fileList.Add(storageObject.Name);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return fileList;
    }

    public async Task<string> GetFileAsync(string blobName)
    {
        try
        {
            string finalName = Path.Combine(_rootPath, blobName);
            var storageObject = await _storageClient.GetObjectAsync(_bucketName, blobName);
            return storageObject.Name;
        }
        catch (Google.GoogleApiException e) when (e.Error.Code == 404)
        {
            // File not found
            return null;
        }
        catch (Exception ex)
        {
            // Log the exception
            throw ex;
        }
    }
}