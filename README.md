# Jaded Asset Management

[![.NET](https://github.com/YourGitHubUsername/JadedAssetManagement/actions/workflows/dotnet.yml/badge.svg)](https://github.com/YourGitHubUsername/JadedAssetManagement/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/JadedAssetManagement.svg)](https://www.nuget.org/packages/JadedAssetManagement) 
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

JadedAssetManagement is a powerful, unified asset management library built on .NET 8.  Simplify the way you store and manage your images and files across a variety of storage platforms with a single, intuitive module.

**Key Features (Coming Soon):**

* **Local Storage:** Seamlessly manage assets in your local file system. (In Progress)
* **Cloud Storage:**
    * **AWS S3:** Leverage the scalability and reliability of Amazon S3.
    * **Azure Storage:** Integrate with Microsoft's cloud storage solutions.
    * **Google Cloud Platform Storage:** Harness the power of Google's cloud infrastructure.


## Project Status

JadedAssetManagement is currently under active development. Stay tuned for exciting updates and releases!

**Roadmap:**

1. **Local Storage:** Initial release with robust local asset management.
2. **AWS S3 Storage:** Expand functionality to Amazon S3.
3. **Azure Storage:** Add support for Azure storage solutions.
4. **Google Cloud Platform Storage:** Complete the suite with Google Cloud integration.

## Usage

### Basic Setup

```bash
var configuration = new FileSystemConfig() 
{
    PageSize = 20,
    RootPath = Path.Combine(Directory.GetCurrentDirectory(), "Files")
};
FileSystemManager fileSystemManager = new(configuration);


Alternative method to instanciate the class:
```bash
var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
FileSystemManager fileSystemManager = new FileSystemManager(configuration);


## File Listing
### All Files
```bash
var files = await fileSystemManager.ListFilesAllFiles();


### Single File
```bash
AssetTypes singleFile = await fileSystemManager.GetFileAsync("test2.png");

### Paged Listing
```bash
var pagedFiles = await fileSystemManager.ListFilesPaged("", 1, "", 1); // Page 1
var pagedFiles2 = await fileSystemManager.ListFilesPaged("", 2, "", 1); // Page 2


## File Upload
```bash
byte[] fileContent = File.ReadAllBytes("SourceFiles/uplaodFile.png");
bool isUploaded = await fileSystemManager.UploadFileAsync(fileContent, "uploadedFile.png");


## File & Directory Deletions
```bash
bool isDeleted = await fileSystemManager.DeleteFileAsync("Delete/singleDelete.jpg");
bool isDirectoryDeleted = await fileSystemManager.DeleteDirectoryAsync("Delete");



## Important Note


// For NuGet Package installation from Package Manager Console
Install-Package JadedAssetManagement

// For .Net CLI installation
dotnet add package JadedAssetManagement
