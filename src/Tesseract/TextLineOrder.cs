
namespace Tesseract
{
	/// <summary>
	/// The text lines are read in the given sequence.
	/// </summary>
	/// <remarks>
	/// <para>
	/// For example in English the order is top-to-bottom. Chinese vertical text lines
	/// are read right-to-left. While Mongolian is written in vertical columns
	/// like Chinese but read left-to-right.
	/// </para>
	/// <para>
	/// Note that only some combinations makes sense for example <see cref="WritingDirection.LeftToRight"/> implies
	/// <see cref="TextLineOrder.TopToBottom" />.
	/// </para>
	/// </remarks>
    public enum TextLineOrder : int
    {
    	/// <summary>
    	/// The text lines form vertical columns ordered left to right.
    	/// </summary>
        LeftToRight,
        
    	/// <summary>
    	/// The text lines form vertical columns ordered right to left.
    	/// </summary>
        RightToLeft,   
        
    	/// <summary>
    	/// The text lines form horizontal columns ordered top to bottom.
    	/// </summary>
        TopToBottom
    }
}
