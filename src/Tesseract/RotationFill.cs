
using System;

namespace Tesseract
{
	/// <summary>
	/// What colour pixels should be used for the outside?
	/// </summary>
	public enum RotationFill : int
	{
		/// <summary>
		/// Bring in white pixels from the outside.
		/// </summary>
		White = 1,
		/// <summary>
		/// Bring in black pixels from the outside.
		/// </summary>
		Black = 2
	}
}
