using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BytesAssetManagement.Base;

public static class Helpers
{
    internal static string GetMimeType(string extension)
    {
        // Implementation goes here. For now, returning a generic MIME type.
        return "application/octet-stream";
    }
}