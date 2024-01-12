using System.Diagnostics;

namespace HashEm.Data;

[DebuggerDisplay("{RealativePath} ({Id})")]
public class ImageFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RealativePath { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public Guid Hash { get; set; } 
    public Guid PathHash { get; set; }

    public bool? Exists { get; set; }
}
