using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondShooter.Persistance.Entities;

[Table("Contents", Schema = "Files")]
public class FileContent
{
    [Key, Column(TypeName= "UNIQUEIDENTIFIER DEFAULT newsequentialid()")]
    public Guid FileContentID { get; set; }

    [Column(TypeName = "UNIQUEIDENTIFIER")]
    public required Guid FileEntryID { get; set; }

    [StringLength(25)]
    public required string ContentType { get; set; }

    [StringLength(25)]
    public required string Category { get; set; }

    [StringLength(100)]
    public required string Container { get; set; }

    [StringLength(512)]
    public required string Reference { get; set; }

    public required long Offset { get; set; }
    public required long Length { get; set; }

    public static void OnModelCreating(EntityTypeBuilder<FileContent> entity)
    {
        entity.Property(e => e.FileContentID).ValueGeneratedOnAdd();
    }
}

