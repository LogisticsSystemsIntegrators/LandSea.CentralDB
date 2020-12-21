using System;

namespace XMLAPI.DataAccess
{
    public class ModelBase:IDisposable
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

        public ModelBase()
        {

        }

        #endregion
    }
}