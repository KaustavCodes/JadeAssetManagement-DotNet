using Google.Cloud.Storage.V1;
using JadedAssetManagement.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JadedAssetManagement.GoogleFileSystem;

public class GoogleFileSystemManager : IFileSystemBase
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

    public async Task<bool> UploadFileAsync(byte[] fileContent, string relativeDestinationPath)
    {
        try
        {
            string finalPath = Path.Combine(_rootPath, relativeDestinationPath);
            using var fileStream = new MemoryStream(fileContent);
            await _storageClient.UploadObjectAsync(_bucketName, finalPath, null, fileStream);
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
            string finalPath = Path.Combine(_rootPath, relativeFilePath);
            await _storageClient.DeleteObjectAsync(_bucketName, relativeFilePath);
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

    public async Task<AssetTypes> GetFileAsync(string relativeFilePath)
    {
        try
        {
            var storageObject = await _storageClient.GetObjectAsync(_bucketName, relativeFilePath);
            return new AssetTypes
            {
                Name = storageObject.Name,
                Path = storageObject.Name,
                Extension = Path.GetExtension(storageObject.Name)
            };
        }
        catch (Exception ex)
        {
            // Log the exception
            return null;
        }
    }

    public async Task<bool> DeleteDirectoryAsync(string relativeDirectoryPath)
    {
        try
        {
            var objects = _storageClient.ListObjects(_bucketName, relativeDirectoryPath);
            foreach (var storageObject in objects)
            {
                await _storageClient.DeleteObjectAsync(_bucketName, storageObject.Name);
            }
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            return false;
        }
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "/", string searchKey = "")
    {
        var assetList = new List<AssetTypes>();
        try
        {
            var objects = _storageClient.ListObjects(_bucketName, relativePath);
            
            foreach (var storageObject in objects)
            {
                assetList.Add(new AssetTypes
                {
                    Name = storageObject.Name,
                    Path = storageObject.Name,
                    Extension = Path.GetExtension(storageObject.Name)
                });
            }
            return assetList;
        }
        catch (Exception ex)
        {
            // Log the exception
            return assetList;
        }
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0)
    {
        var assetList = new List<AssetTypes>();
        try
        {
            var objects = _storageClient.ListObjects(_bucketName, relativePath);
            
            int pageCounter = 0;
            foreach (var storageObject in objects)
            {
                if (pageCounter == currentPage)
                {
                    assetList.Add(new AssetTypes
                    {
                        Name = storageObject.Name,
                        Path = storageObject.Name,
                        Extension = Path.GetExtension(storageObject.Name)
                    });
                    break;
                }
                pageCounter++;
            }
            return assetList;
        }
        catch (Exception ex)
        {
            // Log the exception
            return assetList;
        }
    }
}