using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Muggle.Xaml2Pictures.Resources;

namespace Muggle.Xaml2Pictures.Models;

public static class Converter
{
    public static void SaveDrawingGroupWithAutoSize(
        DrawingGroup drawing,
        string filePath,
        BitmapEncoderEnum encoder,
        double dpi,
        int blankBorderWidth)
    {
        ArgumentNullException.ThrowIfNull(drawing);

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException(string.Format(Language.CannotBeNullOrEmpty, nameof(filePath)),
                nameof(filePath));
        }

        var bounds = drawing.Bounds;
        if (bounds.IsEmpty)
        {
            return;
        }

        var factor = dpi / 96.0;

        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {

            if (encoder is BitmapEncoderEnum.Bmp or BitmapEncoderEnum.Jpeg)
            {
                context.DrawRectangle(Brushes.White, null,
                    new Rect(0, 0,
                        bounds.Width + Math.Ceiling(2 * blankBorderWidth / factor),
                        bounds.Height + Math.Ceiling(2 * blankBorderWidth / factor)));
            }

            var transform = new TranslateTransform(
                -bounds.X + blankBorderWidth / factor,
                -bounds.Y + blankBorderWidth / factor);

            var transformedDrawing = new DrawingGroup();
            transformedDrawing.Children.Add(drawing);
            transformedDrawing.Transform = transform;

            context.DrawDrawing(transformedDrawing);
        }

        var width = (int)Math.Ceiling(bounds.Width * factor);
        var height = (int)Math.Ceiling(bounds.Height * factor);

        var renderTarget = new RenderTargetBitmap(
            width + 2 * blankBorderWidth, 
            height + 2 * blankBorderWidth,
            dpi, dpi, PixelFormats.Pbgra32);
        renderTarget.Render(visual);

        BitmapEncoder bitmapEncoder = encoder switch
        {
            BitmapEncoderEnum.Bmp => new BmpBitmapEncoder(),
            BitmapEncoderEnum.Gif => new GifBitmapEncoder(),
            BitmapEncoderEnum.Jpeg => new JpegBitmapEncoder(),
            BitmapEncoderEnum.Png => new PngBitmapEncoder(),
            BitmapEncoderEnum.Tiff => new TiffBitmapEncoder(),
            BitmapEncoderEnum.Wmp => new WmpBitmapEncoder(),
            _ => throw new ArgumentOutOfRangeException(nameof(encoder), encoder, null)
        };

        bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            bitmapEncoder.Save(fileStream);
        }
    }
}