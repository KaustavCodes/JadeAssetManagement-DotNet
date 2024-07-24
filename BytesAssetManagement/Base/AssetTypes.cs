namespace BytesAssetManagement.Base;

public class AssetTypes
{
    public string Name { get; set; }

    public bool IsFolder { get; set; }

    public string Path { get; set; }

    public string Extension { get; set; }

    public string MimeType { get; set; }

    public long SizeInBytes { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }
}
