
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tesseract
{
	/// <summary>
	/// Description of BitmapToPixConverter.
	/// </summary>
	public class BitmapToPixConverter
    {
        public BitmapToPixConverter()
		{
		}

        public Pix Convert(Bitmap img)
        {           
            var pixDepth = GetPixDepth(img.PixelFormat);
            var pix = Pix.Create(img.Width, img.Height, pixDepth);

            BitmapData imgData = null;
            PixData pixData = null;
            try {

                // TODO: Setup colormap, if required
                if ((img.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed) {
                    throw new NotImplementedException("Indexed bitmaps are not currently supported.");
                }

                // transfer data
                imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                pixData = pix.GetData();

                TransferData(imgData, pixData);

                return pix;
            } catch (Exception) {
                pix.Dispose();
                throw;
            } finally {
                if (imgData != null) {
                    img.UnlockBits(imgData);
                }
            }
        }

        private int GetPixDepth(PixelFormat pixelFormat)
        {
            switch (pixelFormat) {
                case PixelFormat.Format1bppIndexed:
                    return 1;
                case PixelFormat.Format4bppIndexed:
                    return 4;
                case PixelFormat.Format8bppIndexed:
                    return 8;
                case PixelFormat.Format16bppGrayScale:
                    return 16;
                case PixelFormat.Canonical:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    return 32;
                default:
                    throw new Exception("Invalid value for PixelFormat");
            }
        }

        private unsafe void TransferData(BitmapData imgData, PixData pixData)
        {
            var imgFormat = imgData.PixelFormat;
            var imgDepth = Bitmap.GetPixelFormatSize(imgFormat);
            var height = imgData.Height;
            var width = imgData.Width;

            if (imgDepth == 16) {
                for (int y = 0; y < height; y++) {
                    ushort* imgLine = (ushort*)imgData.Scan0 + (y * (imgData.Stride >> 1));
                    uint* pixLine = (uint*)pixData.Data + (y * pixData.WordsPerLine);

                    if (imgFormat == PixelFormat.Format16bppRgb555) {
                        for (int x = 0; x < width; x++) {
                            ushort imgPixel = BitmapHelper.GetDataUInt16(imgLine, x);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.ConvertRgb555ToRGBA(imgPixel));
                        }
                    } else if (imgFormat == PixelFormat.Format16bppRgb565) {
                        for (int x = 0; x < width; x++) {
                            ushort imgPixel = BitmapHelper.GetDataUInt16(imgLine, x);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.ConvertRgb565ToRGBA(imgPixel));
                        }
                    } else if (imgFormat == PixelFormat.Format16bppArgb1555) {
                        for (int x = 0; x < width; x++) {
                            ushort imgPixel = BitmapHelper.GetDataUInt16(imgLine, x);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.ConvertArgb1555ToRGBA(imgPixel));
                        }
                    } else if (imgFormat == PixelFormat.Format16bppGrayScale) {
                        for (int x = 0; x < width; x++) {
                            ushort imgPixel = BitmapHelper.GetDataUInt16(imgLine, x);
                            PixData.SetDataTwoByte(pixLine, x, imgPixel);
                        }
                    }
                }
            } else if(imgDepth == 1 || imgDepth == 4 || imgDepth == 8) {
                for (int y = 0; y < height; y++) {
                    byte* imgLine = (byte*)imgData.Scan0 + (y * imgData.Stride);
                    uint* pixLine = (uint*)pixData.Data + (y * pixData.WordsPerLine);

                    if (imgFormat == PixelFormat.Format8bppIndexed) {
                        for (int x = 0; x < width; x++) {
                            byte* pixelPtr = imgLine + x;
                            byte red = *pixelPtr;
                            byte green = *(pixelPtr + 1);
                            byte blue = *(pixelPtr + 2);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                        }
                    }
                }
            } else if(imgDepth == 24 || imgDepth == 32) {
                for (int y = 0; y < height; y++) {
                    byte* imgLine = (byte*)imgData.Scan0 + (y * imgData.Stride);
                    uint* pixLine = (uint*)pixData.Data + (y * pixData.WordsPerLine);
                    if (imgFormat == PixelFormat.Format24bppRgb) {
                        for (int x = 0; x < lineLength; x += 3) {
                            byte* pixelPtr = imgLine + x;
                            byte red = *pixelPtr;
                            byte green = *(pixelPtr + 1);
                            byte blue = *(pixelPtr + 2);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                        }
                    } else if (imgFormat == PixelFormat.Format32bppArgb) {
                        for (int x = 0; x < lineLength; x += 4) {
                            byte* pixelPtr = imgLine + x;
                            byte alpha = *pixelPtr;
                            byte red = *(pixelPtr + 1);
                            byte green = *(pixelPtr + 2);
                            byte blue = *(pixelPtr + 3);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, alpha));
                        }
                    } else if (imgFormat == PixelFormat.Format32bppPArgb) {
                        for (int x = 0; x < lineLength; x += 4) {
                            byte* pixelPtr = imgLine + x;
                            byte alpha = *pixelPtr;
                            byte red = *(pixelPtr + 1);
                            byte green = *(pixelPtr + 2);
                            byte blue = *(pixelPtr + 3);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, alpha));
                        }
                    } else if (imgFormat == PixelFormat.Format32bppRgb) {
                        for (int x = 0; x < lineLength; x += 4) {
                            byte* pixelPtr = imgLine + x;
                            byte red = *pixelPtr;
                            byte green = *(pixelPtr + 1);
                            byte blue = *(pixelPtr + 2);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                        }
                    } else if (imgFormat == PixelFormat.Canonical) {
                        for (int x = 0; x < lineLength; x += 4) {
                            byte* pixelPtr = imgLine + x;
                            byte red = *pixelPtr;
                            byte green = *(pixelPtr + 1);
                            byte blue = *(pixelPtr + 2);
                            byte alpha = *(pixelPtr + 3);
                            PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, alpha));
                        }
                    } 
                }
            } 
        }
	}
}
