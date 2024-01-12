using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondShooter.Persistance.Entities;

[Table("ImageFiles")]
public class ImageFile
{
    [Key]
    public Guid ImageFileId { get; set; }
    [StringLength(512)]
    public string RelativePath { get; set; }
    [StringLength(512)]
    public string FileName { get; set; }
    [StringLength(25)]
    public string? Extension { get; set; }
    public Guid Hash { get; set; }
    public Guid PathHash { get; set; }
    public bool Exists { get; set; }
}
