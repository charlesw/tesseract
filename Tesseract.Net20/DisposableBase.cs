
using System;
using System.Diagnostics;
using System.IO;

namespace Tesseract
{
	public abstract class DisposableBase: IDisposable
	{

	    /// <summary>
	    /// Leptonica does not support reading images from memory
	    /// on Windows. Therefor as a workaround we need to write
	    /// </summary>
	    protected static string IoTempPath = null;

		protected DisposableBase()
		{
			IsDisposed = false;
		}
		
		~DisposableBase() 
		{
			Dispose(false);
			Debug.Fail(String.Format("{0} was not disposed off.", this));
		}
		
		
		public void Dispose()
		{
			Dispose(true);
			
            if (!string.IsNullOrEmpty(IoTempPath))
            {
                try
                {
                    File.Delete(IoTempPath);
                    IoTempPath = null;
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Unable to delete temporary image {0}", IoTempPath));
                }
            }

			IsDisposed = true;			
            GC.SuppressFinalize(this);

            if (Disposed != null) {
                Disposed(this, EventArgs.Empty);
            }

		}
		
		public bool IsDisposed { get; private set; }
        
        public event EventHandler<EventArgs> Disposed;

		
		protected virtual void VerifyNotDisposed()
		{
			if(IsDisposed) throw new ObjectDisposedException(ToString());
		}
		
		protected abstract void Dispose(bool disposing);
	}
}
