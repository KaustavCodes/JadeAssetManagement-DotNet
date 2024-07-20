using JadedAssetManagement.Base;
using JadedAssetManagement.FileSystem;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Jaded File Manager!");

var configuration = new FileSystemConfig()
{
    PageSize = 20,
    RootPath = Path.Combine(Directory.GetCurrentDirectory(), "Files")
};

FileSystemManager fileSystemManager = new(configuration);

//Testing all file listing
var files = await fileSystemManager.ListFilesAllFiles();

Console.WriteLine("All File Listing");
foreach (var file in files)
{
    Console.WriteLine(file.Name + " (" + file.Path + ")");
}

//Testing Single File Listing
AssetTypes singelFile = await fileSystemManager.GetFileAsync("test2.png");
Console.WriteLine("");
Console.WriteLine("Single File Listing");
if(singelFile != null)
{
    Console.WriteLine(singelFile.Name + " (" + singelFile.Path + ")");
}

//Testing Paged File Listing

var pagedFiles = await fileSystemManager.ListFilesPaged("", 1, "", 1);
Console.WriteLine("");
Console.WriteLine("File Listing Paged (PG 1):");
foreach (var file in pagedFiles)
{
    Console.WriteLine(file.Name + " (" + file.Path + ")");
}


var pagedFiles2 = await fileSystemManager.ListFilesPaged("", 2, "", 1);
Console.WriteLine("");
Console.WriteLine("File Listing Paged (PG 2):");
foreach (var file in pagedFiles2)
{
    Console.WriteLine(file.Name + " (" + file.Path + ")");
}


var searchedFiles = await fileSystemManager.ListFilesPaged("", 1, "searchme");
Console.WriteLine("");
Console.WriteLine("Serch File Listing Paged : Looking for file with name searchme:");
foreach (var file in searchedFiles)
{
    Console.WriteLine(file.Name + " (" + file.Path + ")");
}


//Upload File Testing
byte[] fileContent = File.ReadAllBytes("SourceFiles/uplaodFile.png");
bool isUploaded = await fileSystemManager.UploadFileAsync(fileContent, "uploadedFile.png");
Console.WriteLine("");
Console.WriteLine("File Upload Status: " + isUploaded);

// //File Deletion Testing
// bool isDeleted = await fileSystemManager.DeleteFileAsync("Delete/singleDelete.jpg");

// Console.WriteLine("File Delete Status: " + isDeleted);


// //Directory Deletion Testing
// bool isDirectoryDeleted = await fileSystemManager.DeleteDirectoryAsync("Delete");
// Console.WriteLine("Directory Delete Status: " + isDirectoryDeleted);

