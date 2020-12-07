using System;
using System.Collections.Generic;
using BSW.Data.Provider;

using System.Data;
using System.Data.SqlClient;

namespace XMLAPI.DataAccess
{
    public class CargoWiseFileProvider:List<CargoWiseFileModel>,IProvider,IDisposable
    {

        public bool UpdateETNNumber(string key, string etnNumebr, ref string msg)
        {
            bool result = true;

            try
            {
                List<SqlParameter> listParameters = new List<SqlParameter>();

                listParameters.Add(ProviderManager.CreateParameter("@CargoWiseKey", key, SqlDbType.VarChar));
                listParameters.Add(ProviderManager.CreateParameter("@ETNNumber", etnNumebr, SqlDbType.VarChar));
             
                result = ProviderManager.ExecuteSP("ETNNumberUpdate", listParameters, ref msg);

            }
            catch (Exception ex)
            {
                msg = String.Format("Error in UpdateETNNumber.Insert()!\r\n->{0}", ex.Message);
                result = false;
            }

            return result;
        }

        public bool ReadModelData(SqlDataReader reader, ref string msg)
        {
            try
            {
                CargoWiseFileModel model = new CargoWiseFileModel();


                this.Add(model);

                return true;
            }
            catch (Exception ex)
            {
                msg = String.Format("Error in CargoWiseFileProvider.ReadModelData!\r\n->{0}", ex.Message);
                return false;
            }
        }

        public void ClearModelData()
        {
            Clear();
        }

        #region - Methods : Dispose -

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
                    ClearModelData();
                }
            }
            _disposed = true;
        }

        #endregion
    }
}