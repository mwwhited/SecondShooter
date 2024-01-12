namespace SortEm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var log = $"{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            using var writer = new StreamWriter(log, new FileStreamOptions
            {
                Share = FileShare.Read,
                Access = FileAccess.Write,
                Mode = FileMode.OpenOrCreate,
            })
            {
                AutoFlush = true,
            };

            var baseTarget = @"M:\media\Photos\";
            var sources = new[]
            {
                @"M:\ICloud",
            };
            var errors = new List<Exception>();
            foreach (var source in sources)
            {
                var baseSource = source;
                if (!baseSource.EndsWith('\\'))
                    baseSource += '\\';

                writer.WriteLine($"{nameof(baseTarget)} = {baseTarget}");
                writer.WriteLine($"{nameof(baseSource)} = {baseSource}");
                writer.WriteLine($"{nameof(DateTime)} = {DateTime.Now}");

                var files =
                    Directory.EnumerateFiles(baseSource, "*.*", SearchOption.AllDirectories)
                    .Where(f => !f.EndsWith(".db", StringComparison.InvariantCultureIgnoreCase));

                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    var modified = info.LastWriteTime;

                    var baseName = file.Replace(baseSource, "");
                    var newPath = Path.GetFullPath(Path.Combine(baseTarget, modified.Year.ToString(), $"{modified:yyyyMM}_iPhone", baseName));

                    try
                    {
                        if (!newPath.StartsWith(baseTarget, StringComparison.InvariantCultureIgnoreCase))
                            throw new ApplicationException("Output file not in target");

                        var dir = Path.GetDirectoryName(newPath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            writer.WriteLine($"(create) {nameof(dir)} = {dir}");
                        }

                        if (File.Exists(newPath))
                        {
                            writer.WriteLine($"(skip) {baseName} => {newPath}");
                            continue;
                        }

                        Console.WriteLine($"{baseName} => {newPath}");
                         throw new Exception("Test");
                        info.MoveTo(newPath);
                        writer.WriteLine($"(moved) {baseName} => {newPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"(error) {baseName} => {newPath} \"{ex.Message}\"");
                        writer.WriteLine($"(error) {baseName} => {newPath} \"{ex.Message}\"");
                        errors.Add(ex);
                    }
                }
            }

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}