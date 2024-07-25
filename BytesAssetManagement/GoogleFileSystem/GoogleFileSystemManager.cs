using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BytesAssetManagement.GoogleFileSystem;

public class GoogleFileSystemManager
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public GoogleFileSystemManager(string projectId, string bucketName, string credentialsPath)
    {
        // Set the environment variable to specify the credentials file.
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        _storageClient = StorageClient.Create();
        _bucketName = bucketName;
    }

    public async Task<bool> UploadFileAsync(string filePath, string destinationBlobName)
    {
        try
        {
            using var fileStream = File.OpenRead(filePath);
            await _storageClient.UploadObjectAsync(_bucketName, destinationBlobName, null, fileStream);
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
            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix);
            await foreach (var storageObject in objects)
            {
                fileList.Add(storageObject.Name);
            }
        }
        catch (Exception ex)
        {
            // Log the exception
        }
        return fileList;
    }
}