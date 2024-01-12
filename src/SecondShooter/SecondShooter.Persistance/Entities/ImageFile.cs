using System.ComponentModel.DataAnnotations.Schema;

namespace SecondShooter.Persistance.Entities;

[Table("ImageFiles")]
public class ImageFile
{
    public Guid Id { get; set; }
    public string RelativePath { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    public Guid Hash { get; set; }
    public Guid PathHash { get; set; }
    public bool Exists { get; set; }
}
