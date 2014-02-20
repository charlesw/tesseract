
using System;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract.Internal;

namespace Tesseract
{
	/// <summary>
	/// Represents an array of <see cref="Pix"/>.
	/// </summary>
	public class PixArray : DisposableBase
	{
		#region Static Constructors
		
		/// <summary>
		/// Loads the multi-page tiff located at <paramref name="filename"/>.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static PixArray LoadMultiPageTiffFromFile(string filename)
		{			
			var	pixaHandle = Interop.LeptonicaApi.pixaReadMultipageTiff( filename );
			if(pixaHandle == IntPtr.Zero)
			{
				throw new IOException(String.Format("Failed to load image '{0}'.", filename));
			}
			
			return new PixArray(pixaHandle);
		}
		
		#endregion
		
		#region Fields
		
		/// <summary>
		/// Gets the handle to the underlying PixA structure.
		/// </summary>
		private HandleRef _handle;
		private int _count;
		
		#endregion
		
		#region Constructor
		
		private PixArray(IntPtr handle)
		{
			_handle = new HandleRef(this, handle);
			
			// These will need to be updated whenever the PixA structure changes (i.e. a Pix is added or removed) though at the moment that isn't a problem.
			_count = Interop.LeptonicaApi.pixaGetCount(_handle);
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets the number of <see cref="Pix"/> contained in the array.
		/// </summary>
		public int Count
		{
			get {
				VerifyNotDisposed();
				return _count; 
			}
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Gets the <see cref="Pix"/> located at <paramref name="index"/> using the specified <paramref name="accessType"/>.
		/// </summary>
		/// <param name="index">The index of the pix (zero based).</param>
		/// <param name="accessType">The <see cref="PixArrayAccessType" /> used to retrieve the <see cref="Pix"/>, only Clone or Copy are allowed.</param>
		/// <returns>The retrieved <see cref="Pix"/>.</returns>
		public Pix GetPix(int index, PixArrayAccessType accessType = PixArrayAccessType.Clone)
		{
			Guard.Require("accessType", accessType == PixArrayAccessType.Clone || accessType == PixArrayAccessType.Copy, "Access type must be either copy or clone but was {0}.", accessType);
			Guard.Require("index", index >= 0 && index < Count, "The index {0} must be between 0 and {1}.", index, Count);
			
			VerifyNotDisposed();
			
			var pixHandle = Interop.LeptonicaApi.pixaGetPix(_handle, index, accessType);
			if(pixHandle == IntPtr.Zero) {
				throw new InvalidOperationException(String.Format("Failed to retrieve pix {0}.", pixHandle));
			}
			return Pix.Create(pixHandle);
		}
						
		protected override void Dispose(bool disposing)
		{
			IntPtr handle = _handle.Handle;
			Interop.LeptonicaApi.pixaDestroy(ref handle);
			_handle = new HandleRef(this, handle);
		}
		
		#endregion
	}
}
