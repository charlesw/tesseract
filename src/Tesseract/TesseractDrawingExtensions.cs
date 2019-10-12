#if SYSTEM_DRAWING_SUPPORT

using System;
using System.Drawing;

namespace Tesseract
{
    public static class TesseractDrawingExtensions
    {
        
        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="Process(Pix, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, PageSegMode? pageSegMode = null)
        {
            return engine.Process(image, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="Process(Pix, String, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, string inputName, PageSegMode? pageSegMode = null)
        {
            return engine.Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="TesseractEngine.Process(Pix, Rect, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, Rect region, PageSegMode? pageSegMode = null)
        {
            return engine.Process(image, null, region, pageSegMode);
        }

        /// <summary>
        /// Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        /// Please consider <see cref="TesseractEngine.Process(Pix, String, Rect, PageSegMode?)"/> instead. This is because
        /// this method must convert the bitmap to a pix for processing which will add additional overhead.
        /// Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this TesseractEngine engine, Bitmap image, string inputName, Rect region, PageSegMode? pageSegMode = null)
        {
            var pix = PixConverter.ToPix(image);
            var page = engine.Process(pix, inputName, region, pageSegMode);
            new TesseractEngine.PageDisposalHandle(page, pix);
            return page;
        }

        public static Color ToColor(this PixColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static PixColor ToPixColor(this Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// gets the number of Bits Per Pixel (BPP)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static int GetBPP(this System.Drawing.Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed: return 1;
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed: return 4;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed: return 8;
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565: return 16;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb: return 24;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb: return 32;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb: return 48;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb: return 64;
                default: throw new ArgumentException(String.Format("The bitmap's pixel format of {0} was not recognised.", bitmap.PixelFormat), "bitmap");
            }
        }
    }
}

#endif
