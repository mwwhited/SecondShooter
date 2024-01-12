using LiteDB;
using Microsoft.Extensions.Options;

namespace HashEm.Data;

public class HashingDbContext
{
    public HashingDbContext(
    IOptions<HashingOptions> options
    )
    {
        db = GetDatabase(options.Value);

        ImageFiles = db.GetCollection<ImageFile>(nameof(ImageFiles));

        EnsureIndexes(this);
    }

    private readonly LiteDatabase db;

    public ILiteCollection<ImageFile> ImageFiles { get; }

    internal static LiteDatabase GetDatabase(HashingOptions options) =>
        new LiteDatabase(options.ConnectionString);

    internal static void EnsureIndexes(HashingDbContext context)
    {
        context.ImageFiles.EnsureIndex(i => i.Id);
        context.ImageFiles.EnsureIndex(i => i.RealativePath);
    }
}
