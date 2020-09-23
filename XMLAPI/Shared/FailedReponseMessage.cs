using System;

namespace XMLAPI
{
    public class FailedResponseMessage : IDisposable
    {
        #region IDisposable/Construction Members

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }
            }
            _disposed = true;
        }

        #endregion
        public string Message { get; set; }
    }
    
}