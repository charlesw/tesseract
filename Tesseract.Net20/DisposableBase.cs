
using System;
using System.Diagnostics;

namespace Tesseract
{
	public abstract class DisposableBase: IDisposable
	{
		protected DisposableBase()
		{
			IsDisposed = false;
		}
		
		~DisposableBase() 
		{
			Dispose(false);
			Trace.TraceWarning(String.Format("{0} was not disposed off.", this));
		}
		
		
		public void Dispose()
		{
			Dispose(true);
			
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
