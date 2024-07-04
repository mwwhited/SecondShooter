using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondShooter.Persistance.Entities;

[Table("Entries", Schema = "Files")]
public class FileEntry
{
    [Key, Column(TypeName = "UNIQUEIDENTIFIER DEFAULT newsequentialid()")]
    public Guid FileEntryID { get; set; }

    [StringLength(512)]
    public required string RelativePath { get; set; }
    [StringLength(512)]
    public required string FileName { get; set; }
    [StringLength(25)]
    public string? Extension { get; set; }
    public Guid Hash { get; set; }

    [StringLength(100)]
    public required string PathHash { get; set; }
    public bool Exists { get; set; }

    public Collection<FileContent> Contents { get; set; } = [];

    public static void OnModelCreating(EntityTypeBuilder<FileEntry> entity)
    {
        entity.Property(e => e.FileEntryID).ValueGeneratedOnAdd();
    }
}
