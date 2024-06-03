using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondShooter.Persistance.Entities;

[Table("ImageFiles")]
public class ImageFile
{
    [Column("_id"), Key]
    public Guid ImageFileId { get; set; }
    [StringLength(512)]
    public required  string RelativePath { get; set; }
    [StringLength(512)]
    public required string FileName { get; set; }
    [StringLength(25)]
    public string? Extension { get; set; }
    public Guid Hash { get; set; }
    public Guid PathHash { get; set; }
    public bool Exists { get; set; }
}
