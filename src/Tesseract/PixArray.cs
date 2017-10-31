
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract.Internal;

namespace Tesseract
{
	/// <summary>
	/// Represents an array of <see cref="Pix"/>.
	/// </summary>
	public class PixArray : DisposableBase, IEnumerable<Pix>
	{
		#region Static Constructors
		
		/// <summary>
		/// Loads the multi-page tiff located at <paramref name="filename"/>.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static PixArray LoadMultiPageTiffFromFile(string filename)
		{			
			var	pixaHandle = Interop.LeptonicaApi.Native.pixaReadMultipageTiff( filename );
			if(pixaHandle == IntPtr.Zero)
			{
				throw new IOException(String.Format("Failed to load image '{0}'.", filename));
			}
			
			return new PixArray(pixaHandle);
		}

        public static PixArray Create(int n)
        {
            var pixaHandle = Interop.LeptonicaApi.Native.pixaCreate(n);
            if (pixaHandle == IntPtr.Zero)
            {
                throw new IOException("Failed to create PixArray");
            }

            return new PixArray(pixaHandle);
        }
        
        #endregion

        #region Enumerator implementation

        /// <summary>
        /// Handles enumerating through the <see cref="Pix"/> in the PixArray.
        /// </summary>
        private class PixArrayEnumerator : DisposableBase, IEnumerator<Pix>
		{
			#region Fields
			
			private readonly PixArray array;
			private readonly Pix[] items;
			private Pix current;
			private int index;
			private readonly int version;
			
			#endregion
			
			#region Constructor
			
			public PixArrayEnumerator(PixArray array)
			{
				this.array = array;
				this.version = array.version;
				this.items = new Pix[array.Count];
				this.index = 0;
				this.current = null;
			}
		
			#endregion
			
			#region Enumerator Implementation
						
			/// <inheritdoc/>
			public bool MoveNext()
			{
				VerifyArrayUnchanged();
				VerifyNotDisposed();
				
				if(index < items.Length) {
					if(items[index] == null) {
						items[index] = array.GetPix(index);
					}
					current = items[index];
					index++;
					return true;
				} else {
					index = items.Length + 1;
					current = null;
					return false;
				}
			}
						
			/// <inheritdoc/>
			public Pix Current {
				get {
					VerifyArrayUnchanged();
					VerifyNotDisposed();
										
					return current;
				}
			}
			
			// IEnumerator imp
			
			/// <inheritdoc/>
			void IEnumerator.Reset()
			{
				VerifyArrayUnchanged();
				VerifyNotDisposed();
				
				index = 0;
				current = null;
			}
			
			/// <inheritdoc/>
			object IEnumerator.Current {
				get {
					// note: Only the non-generic requires an exception check according the MSDN docs (Generic version just undefined if it's not currently pointing to an item). Go figure.
					if(index == 0 || index == items.Length + 1) {
						throw new InvalidOperationException("The enumerator is positioned either before the first item or after the last item .");
					}
					
					return Current;
				}
			}
			
			// Helpers
			
			/// <inheritdoc/>
			private void VerifyArrayUnchanged()
			{				
				if(version != array.version) {
					throw new InvalidOperationException("PixArray was modified; enumeration operation may not execute.");
				}
			}
			
			#endregion	
			
			#region Disposal
			
			protected override void Dispose(bool disposing)
			{
				if(disposing) {
					for (int i = 0; i < items.Length; i++) {
						if(items[i] != null) {
							items[i].Dispose();
							items[i] = null;
						}
					}
				}
			}
			
			#endregion		
			
		}
		
		#endregion
		
		#region Fields
		
		/// <summary>
		/// Gets the handle to the underlying PixA structure.
		/// </summary>
		private HandleRef _handle;
		private int _count;
		private int version;
		
		#endregion
		
		#region Constructor
		
		private PixArray(IntPtr handle)
		{
			_handle = new HandleRef(this, handle);
			version = 1;
			
			// These will need to be updated whenever the PixA structure changes (i.e. a Pix is added or removed) though at the moment that isn't a problem.
			_count = Interop.LeptonicaApi.Native.pixaGetCount(_handle);
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
        /// Add the specified pix to the end of the pix array.
        /// </summary>
        /// <remarks>
        /// PixArrayAccessType.Insert is not supported as the managed Pix object will attempt to release the pix when
        /// it goes out of scope creating an access exception.
        /// </remarks>
        /// <param name="pix">The pix to add.</param>
        /// <param name="copyflag">Determines if a clone or copy of the pix is inserted into the array.</param>
        /// <returns></returns>
        public bool Add(Pix pix, PixArrayAccessType copyflag = PixArrayAccessType.Clone)
        {
            Guard.RequireNotNull("pix", pix);
            Guard.Require("copyflag", copyflag == PixArrayAccessType.Clone || copyflag == PixArrayAccessType.Copy,
                "Copy flag must be either copy or clone but was {0}.", copyflag);

            int result = Interop.LeptonicaApi.Native.pixaAddPix(_handle, pix.Handle, copyflag);
            if (result == 0)
            {
                _count = Interop.LeptonicaApi.Native.pixaGetCount(_handle);
            }
            return result == 0;
        }

        /// <summary>
        /// Removes the pix located at index.
        /// </summary>
        /// <remarks>
        /// Notes:
        /// * This shifts pixa[i] --> pixa[i - 1] for all i > index.
        /// * Do not use on large arrays as the functionality is O(n).
        /// * The corresponding box is removed as well, if it exists.
        /// </remarks>
        /// <param name="index">The index of the pix to remove.</param>
        public void Remove(int index)
        {
            Guard.Require("index", index >= 0 && index < Count, "The index {0} must be between 0 and {1}.", index, Count);

            VerifyNotDisposed();
            if(Interop.LeptonicaApi.Native.pixaRemovePix(_handle, index) == 0)
            {
                _count = Interop.LeptonicaApi.Native.pixaGetCount(_handle);
            }
        }

        /// <summary>
        /// Destroys ever pix in the array.
        /// </summary>
        public void Clear()
        {
            VerifyNotDisposed();
            if (Interop.LeptonicaApi.Native.pixaClear(_handle) == 0)
            {
                _count = Interop.LeptonicaApi.Native.pixaGetCount(_handle);
            }
        }

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
			
			var pixHandle = Interop.LeptonicaApi.Native.pixaGetPix(_handle, index, accessType);
			if(pixHandle == IntPtr.Zero) {
				throw new InvalidOperationException(String.Format("Failed to retrieve pix {0}.", pixHandle));
			}
			return Pix.Create(pixHandle);
		}

		/// <summary>
		/// Returns a <see cref="IEnumerator{Pix}"/> that iterates the the array of <see cref="Pix"/>.
		/// </summary>
		/// <remarks>
		/// When done with the enumerator you must call <see cref="Dispose"/> to release any unmanaged resources.
		/// However if your using the enumerator in a foreach loop, this is done for you automatically by .Net. This also means
		/// that any <see cref="Pix"/> returned from the enumerator cannot safely be used outside a foreach loop (or after Dispose has been
		/// called on the enumerator). If you do indeed need the pix after the enumerator has been disposed of you must clone it using 
		/// <see cref="Pix.Clone()"/>. 
		/// </remarks>
		/// <returns>A <see cref="IEnumerator{Pix}"/> that iterates the the array of <see cref="Pix"/>.</returns>
		public IEnumerator<Pix> GetEnumerator()
		{
			return new PixArrayEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new PixArrayEnumerator(this);
		}					
		
		protected override void Dispose(bool disposing)
		{
			IntPtr handle = _handle.Handle;
			Interop.LeptonicaApi.Native.pixaDestroy(ref handle);
			_handle = new HandleRef(this, handle);
		}
		
		#endregion
	}
}
