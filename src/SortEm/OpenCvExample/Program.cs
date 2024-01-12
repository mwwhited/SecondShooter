namespace OpenCvExample;

internal class Program
{
    static void Main(string[] args)
    {
        // https://github.com/VahidN/OpenCVSharp-Samples/blob/master/OpenCVSharpSample10/Program.cs

        var files = Directory.GetFiles(@".\", "*.jpg");
        foreach (var file in files)
        {
            Console.WriteLine(file);
            new CalculateHistogram().Execute(file);
        }

        Console.WriteLine("fin!");
    }
}
