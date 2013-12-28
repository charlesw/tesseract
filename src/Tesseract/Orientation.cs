using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
	/// <summary>
	/// Represents orientation that the page would need to be rotated so that .
	/// </summary>
	/// <remarks>
	/// Orientation is defined as to what side of the page would need to correspond to the 'up' direction such that the characters will
	/// be read able. Another way of looking at this what direction you need to rotate you head so that "up" aligns with Orientation, 
	/// then the characters will appear "right side up" and readable.
	/// 
	/// In short:
	/// <list type="bullet">
	/// 	<item>PageUp - Page is correctly alligned with up and no rotation is needed.</item>
	/// 	<item>PageRight - Page needs to be rotated so the right hand side is up, 90 degress counter clockwise, to be readable.</item>
	/// 	<item>PageDown - Page needs to be rotated so the bottom side is up, 180 degress counter clockwise, to be readable.</item>
	/// 	<item>PageLeft - Page needs to be rotated so the left hand side is up, 90 degress clockwise, to be readable.</item>
	/// </list>
	/// </remarks>
    public enum Orientation
    {
    	/// <summary>
    	/// Page is correctly alligned with up and no rotation is needed.
    	/// </summary>
        PageUp, 
        /// <summary>
        /// Page needs to be rotated so the right hand side is up, 90 degress counter clockwise, to be readable.
        /// </summary>
        PageRight, 
        /// <summary>
        /// Page needs to be rotated so the bottom side is up, 180 degress counter clockwise, to be readable.
        /// </summary>
        PageDown, 
        /// <summary>
        /// Page needs to be rotated so the left hand side is up, 90 degress clockwise, to be readable.
        /// </summary>
        PageLeft
    }
}
