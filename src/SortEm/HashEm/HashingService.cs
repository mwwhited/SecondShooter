using HashEm.Data;
using LiteDB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace HashEm;

public class HashingService(
    HashingDbContext db,
    ILogger<HashingService> log,
    IOptions<HashingOptions> options
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = options.Value;

        ////note: mark all as hashed
        //await DoHashingAsync(config, cancellationToken);

        //note: check if still exist
        await CheckExistingAsync(config, cancellationToken);

        //// remove duplicates 
        //await RemoveDuplicatesAsync(config, cancellationToken);
    }

    private async Task DoHashingAsync(HashingOptions config, CancellationToken cancellationToken)
    {
        var files = from path in Directory.EnumerateFiles(config.SearchRoot, "*.*", SearchOption.AllDirectories)

                    let file = Path.GetFileName(path)
                    let extension = Path.GetExtension(path).ToLower()
                    let fullPath = Path.GetFullPath(path)
                    let realativePath = ".\\" + fullPath.Substring(config.SearchRoot.Length).Trim('/', '\\')

                    where !path.StartsWith(config.MetaDataBasePath, StringComparison.InvariantCultureIgnoreCase)

                    where !file.Equals("Thumbs.db", StringComparison.InvariantCultureIgnoreCase)
                    where !file.Equals("ehthumbs_vista.db", StringComparison.InvariantCultureIgnoreCase)

                    where !path.StartsWith(Path.Combine(config.SearchRoot, "lightroom"), StringComparison.InvariantCultureIgnoreCase)

                    where !path.StartsWith(Path.Combine(config.SearchRoot, "$RECYCLE.BIN"), StringComparison.InvariantCultureIgnoreCase)
                    where !path.StartsWith(Path.Combine(config.SearchRoot, "System Volume Information"), StringComparison.InvariantCultureIgnoreCase)

                    select new ImageFile
                    {
                        Extension = extension,
                        FileName = fullPath,
                        RealativePath = realativePath,
                    };

        var x = 0;
        await Parallel.ForEachAsync(files, cancellationToken, async (file, token) =>
        {
            if (token.IsCancellationRequested)
            {
                log.LogWarning("Canceled!");
                return;
            }
            x++;

            var exists = db.ImageFiles.Find(i => i.RealativePath == file.RealativePath).FirstOrDefault();
            if (exists == null)
            {
                log.LogInformation($"Hash: {{{nameof(file.RealativePath)}}} [{{x}}]", file.RealativePath, x);

                file.PathHash = await HashStringAsync(file.RealativePath);
                file.Hash = await HashFileAsync(Path.Combine(config.SearchRoot, file.RealativePath));

                file.Id = file.PathHash;

                db.ImageFiles.Insert(file);
            }
            else
            {
                log.LogWarning($"Skip: {{{nameof(file.RealativePath)}}} [{{x}}]", file.RealativePath, x);
            }

            x--;
        });
    }

    private async Task CheckExistingAsync(HashingOptions config, CancellationToken cancellationToken)
    {
        var files = from img in db.ImageFiles.FindAll()
                        //where !img.Exists.HasValue
                    where img.Exists.HasValue && img.Exists.Value
                    select img
            ;
        var x = 0;
        await Parallel.ForEachAsync(files, cancellationToken, async (file, token) =>
        {
            if (token.IsCancellationRequested)
            {
                log.LogWarning("Canceled!");
                return;
            }
            x++;

            var fullPath = Path.Combine(config.SearchRoot, file.RealativePath);
            var exists = Path.Exists(fullPath);

            await Task.Yield();

            x--;

            if (!file.Exists.HasValue || file.Exists.Value != exists)
            {
                file.Exists = exists;
                db.ImageFiles.Update(file);

                log.LogInformation($"Exists: {{{nameof(file.RealativePath)}}} [{{x}}] [{{{nameof(exists)}}}]", file.RealativePath, x, exists);
            }

        });
    }

    private async Task RemoveDuplicatesAsync(HashingOptions config, CancellationToken cancellationToken)
    {
        var query = from img in db.ImageFiles.FindAll().AsQueryable()
                    where img.Exists.HasValue && img.Exists.Value
                    group img by img.Hash into hashgroup
                    let dups = new
                    {
                        hash = hashgroup.Key,
                        first = hashgroup.OrderBy(i => i.RealativePath).First().RealativePath,
                        others = hashgroup.OrderBy(i => i.RealativePath).Skip(1).Select(i => i.RealativePath).ToArray(),
                    }
                    where dups.others.Any()
                    select dups;
        var duplicates = query.ToArray();

        var files = (from item in duplicates
                     from dup in item.others
                     select Path.Combine(config.SearchRoot, dup)).ToList();

        var cnt = files.Count;
        log.LogWarning("{cnt}!", cnt);

        var x = 0;
        await Parallel.ForEachAsync(files, cancellationToken, async (duplicate, token) =>
        {
            if (token.IsCancellationRequested)
            {
                log.LogWarning("Canceled!");
                return;
            }
            x++;

            //var fullPath = Path.Combine(config.SearchRoot, duplicate.RealativePath);
            //var exists = duplicate.Exists = Path.Exists(fullPath);
            //db.ImageFiles.Update(duplicate);
            log.LogInformation($"Remove this: {{{nameof(duplicate)}}} [{{x}}]", duplicate, x);
            File.Delete(duplicate);
            x--;
            await Task.Yield();

        });
    }


    public static async ValueTask<Guid> HashFileAsync(string path)
    {
        using var fs = File.OpenRead(path);
        var hash = await MD5.HashDataAsync(fs);
        return new Guid(hash);
    }

    public static async ValueTask<Guid> HashStringAsync(string value)
    {
        using var buffer = new MemoryStream(Encoding.UTF8.GetBytes(value));
        Memory<byte> outBuffer = new byte[16];
        await MD5.HashDataAsync(buffer, outBuffer);
        return new Guid(outBuffer.ToArray());
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
         Task.CompletedTask;
}
