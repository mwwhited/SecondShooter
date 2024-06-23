using ImageMagick;

namespace SecondShooter.Cli;

internal class Program
{
    static void Main(string[] args)
    {
        var path = @"M:\media\Photos\2024\20240408_Eclipse";
        var outpath = @"M:\out\Photos\2024\20240408_Eclipse";
        var files = Directory.GetFiles(path, "*.nef", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var inputPath = Path.GetFullPath(file);
            var outputPath = Path.ChangeExtension(inputPath.Replace(path, outpath), ".jpg");

            var dir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using var image = new MagickImage(file);
            image.Quality = 90;
            image.Write(outputPath);

            Console.WriteLine($"Written: {outputPath}");
        }
    }
}
