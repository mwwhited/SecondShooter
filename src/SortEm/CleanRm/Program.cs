namespace CleanRm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var folders = new[]
            {
                @"C:\Users\mwwhi\Pictures\iCloud Photos\Photos",
            };

            var files = from folder in folders
                        from file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                        let fileInfo = new FileInfo(file)
                        where fileInfo.CreationTime < new DateTime(2023, 9, 1)
                        select fileInfo
                ;


            foreach(var file in files)
            {
                try
                {
                    Console.WriteLine($"Remove move: {file}");
                  //  file.Delete();
                }
                catch (Exception ex) {
                    Console.Error.WriteLine($"(error) Remove move: {file}: {ex.Message}");
                }
            }

        }
    }
}