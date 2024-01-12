using ImageMagick;
using System.Drawing;
using System.IO;

namespace ResortEm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var basePath = @"E:\";
            var folders = Directory.GetDirectories(basePath, "*_iphone", SearchOption.AllDirectories);

            foreach (var folder in folders)
            {
                var files = from f in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                            let fileInfo = new FileInfo(f)
                            //let year = fileInfo.LastWriteTime.Year
                            //let targetPath = Path.Combine(basePath, year.ToString(), $"{year}_Others")
                            //let targetFile = Path.Combine(targetPath, fileInfo.Name)
                            ////orderby year
                            select new
                            {
                                fileInfo = new FileInfo(f),
                                //targetPath,
                                //targetFile,
                            };


                var snapshot = files.ToArray();

               //foreach (var item in files)
                    Parallel.ForEach(snapshot, item =>
                    {
                        try
                        {
                            var time = item.fileInfo.CreationTime;

                            if (time > new DateTime(2023, 1, 1))
                            {
                                try
                                {
                                    using (var image = new MagickImage(item.fileInfo))
                                    {
                                        var exif = image.GetExifProfile();

                                        var value = exif?.Values.FirstOrDefault(e => e.Tag == ExifTag.DateTimeOriginal)?.GetValue()?.ToString();
                                        if (value != null)
                                        {
                                            time = DateTime.ParseExact(value, "yyyy:MM:dd HH:mm:ss", null);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine($"--- nope (MagickImage) --- {ex.Message} ({item.fileInfo.FullName})");
                                }
                            }
                            Console.WriteLine($"{item.fileInfo.FullName} ({time})");

                            if (item.fileInfo.CreationTime != time)
                            {
                                item.fileInfo.CreationTime = time;
                                item.fileInfo.LastWriteTime = time;
                            }

                            var year = time.Year;
                            var targetPath = Path.Combine(basePath, year.ToString(), $"{year}_Others");

                            if (!Directory.Exists(targetPath))
                            {
                                Directory.CreateDirectory(targetPath);
                            }

                            var targetFile = Path.Combine(targetPath, item.fileInfo.Name);

                            var newFile = new FileInfo(targetFile);
                            if (item.fileInfo.FullName == newFile.FullName)
                            {
                                return;
                            }

                            for (var x = 0; newFile.Exists; x++)
                            {
                                if (x > 0)
                                {
                                    newFile = new FileInfo(Path.ChangeExtension(targetFile, "." + x + newFile.Extension));
                                }
                            }

                            item.fileInfo.MoveTo(newFile.FullName);
                            Console.WriteLine($" => {newFile.FullName}");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"--- nope --- {ex.Message} ({item.fileInfo.FullName})");
                        }

                    }
                );


                if (!files.Any() && Directory.Exists(folder))
                {
                    try
                    {
                        Console.WriteLine($" remove {folder}");
                        Directory.Delete(folder, true);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"--- nope (Directory) --- {ex.Message} ({folder})");
                    }
                }
            }

            Console.WriteLine("Hello, World!");
        }
    }
}
