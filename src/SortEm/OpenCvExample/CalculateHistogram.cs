using OpenCvSharp;
using System.Diagnostics.Metrics;

namespace OpenCvExample;

public class CalculateHistogram
{
    public void Execute(string inputFile)
    {
        if (inputFile.Contains("_Adjusted") || inputFile.Contains("_Histogram"))
            return;

        Console.WriteLine("->");

        using var src = new Mat(inputFile, ImreadModes.AnyDepth | ImreadModes.AnyColor);
        using var sourceWindow = new Window("Source", image: src, flags: WindowFlags.AutoSize | WindowFlags.FreeRatio);
        using var histogramWindow = new Window("Histogram", flags: WindowFlags.AutoSize | WindowFlags.FreeRatio);

        histogramWindow.Resize(100, 100);

        var brightness = 100;
        var contrast = 100;

        var brightnessTrackbar = sourceWindow.CreateTrackbar(
                trackbarName: "Brightness",
                initialPos: brightness,
                max: 200,
                callback: (position) =>
                {
                    brightness = position;
                    updateImageCalculateHistogram(sourceWindow, histogramWindow, src, brightness, contrast);
                });

        var contrastTrackbar = sourceWindow.CreateTrackbar(
            trackbarName: "Contrast", initialPos: contrast, max: 200,
            callback: (position) =>
            {
                contrast = position;
                updateImageCalculateHistogram(sourceWindow, histogramWindow, src, brightness, contrast);
            });

        brightnessTrackbar.Callback.DynamicInvoke(brightness);
        contrastTrackbar.Callback.DynamicInvoke(contrast);

        sourceWindow.Image?.SaveImage(Path.ChangeExtension(inputFile, "_Adjusted" + Path.GetExtension(inputFile)));
        histogramWindow.Image?.SaveImage(Path.ChangeExtension(inputFile, "_Histogram" + Path.GetExtension(inputFile)));

        sourceWindow.Close();
        histogramWindow.Close();

        //Cv2.WaitKey();
    }

    private static (Mat histogram, Mat adjusted) updateImageCalculateHistogram(Window sourceWindow, Window histogramWindow, Mat src, int brightness, int contrast)
    {
        var adjusted = new Mat();
        updateBrightnessContrast(src, adjusted, brightness, contrast);
        var histogram = Calculate(src, adjusted);

        sourceWindow.Image = adjusted;
        histogramWindow.Image = histogram;

        return (histogram, adjusted);
    }

    private static void updateBrightnessContrast(Mat src, Mat modifiedSrc, int brightness, int contrast)
    {
        brightness -= 100;
        contrast -= 100;

        double alpha, beta;
        if (contrast > 0)
        {
            double delta = 127f * contrast / 100f;
            alpha = 255f / (255f - delta * 2);
            beta = alpha * (brightness - delta);
        }
        else
        {
            double delta = -128f * contrast / 100;
            alpha = (256f - delta * 2) / 255f;
            beta = alpha * brightness + delta;
        }
        src.ConvertTo(modifiedSrc, MatType.CV_8UC3, alpha, beta);
    }

    private static Mat Calculate(Mat src, Mat modifiedSrc)
    {
        const int histogramSize = 64;//from 0 to 63
        using var histogram = new Mat();
        int[] dimensions = [histogramSize]; // Histogram size for each dimension
        Rangef[] ranges = [new Rangef(0, histogramSize)]; // min/max
        Cv2.CalcHist(
            images: [modifiedSrc],
            channels: [0], //The channel (dim) to be measured. In this case it is just the intensity (each array is single-channel) so we just write 0.
            mask: null,
            hist: histogram,
            dims: 1,
            histSize: dimensions,
            ranges: ranges);

        // Get the max value of histogram
        Cv2.MinMaxLoc(histogram, out double minVal, out double maxVal);

        var color = Scalar.All(100);

        // Scales and draws histogram
        var scaledHistogram = (Mat)(histogram * (maxVal != 0 ? src.Rows / maxVal : 0.0));

        var histogramImage = new Mat(new Size(src.Cols, src.Rows), MatType.CV_8UC3, Scalar.All(255));
        var binW = (int)((double)src.Cols / histogramSize);
        for (var j = 0; j < histogramSize; j++)
        {
            histogramImage.Rectangle(
                new Point(j * binW, histogramImage.Rows),
                new Point((j + 1) * binW, histogramImage.Rows - (int)(scaledHistogram.Get<float>(j))),
                color,
                -1);
        }

        return histogramImage;
    }
}
