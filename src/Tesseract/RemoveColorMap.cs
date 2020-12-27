namespace Tesseract
{
    /// <summary>
    /// The type of colormap removal. <see cref="Pix.RemoveColorMap"/>
    /// </summary>
    public enum RemoveColorMap
    {
        /// <summary>
        /// Remove colormap for conv to 1 bpp
        /// </summary>
        Binary = 0,
        /// <summary>
        /// Remove colormap for conv to 8 bpp
        /// </summary>
        Grayscale = 1,
        /// <summary>
        /// Remove colormap for conv to 32 bpp
        /// </summary>
        FullColor = 2,
        /// <summary>
        /// Remove colormap and alpha
        /// </summary>
        WithAlpha = 3,
        /// <summary>
        /// Remove depending on src format
        /// </summary>
        BasedOnSrc = 4
    }
}