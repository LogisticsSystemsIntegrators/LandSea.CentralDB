using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using System.Data.SqlClient;

using BSW.APIResponse;

namespace XMLAPI.Controllers
{
    public class XMLDocumentsController : ApiController
    {
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse GetXMLDocs(string type)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                // XML document result will be passed in here as a string
                string messageDetail = string.Empty;

                string centralDBURL = ConfigurationManager.AppSettings["LandseaCD"].ToString();

                if (string.IsNullOrEmpty(centralDBURL))
                {
                    result.Success = false;
                    result.Message = "Landsea Global Central Database - internal configuration problem";
                    result.MessageDetail = "Landsea Global - Landsea Global Central Database connection was not configured correctly.";
                    return result;
                }

                #region Get CargoWise XML Documents
                using (SqlConnection conn = new SqlConnection(centralDBURL))
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Open();

                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("CargoWiseFileProcess", conn))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;

                            sqlCommand.Parameters.AddWithValue("@XMLType", type);

                            SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
                            DataTable dtResult = new DataTable();

                            sda.Fill(dtResult);

                            conn.Close();

                            if (dtResult.Rows.Count > 0)
                            {
                                int messageID = int.Parse(dtResult.Rows[0]["MessageID"].ToString());
                                string xmlDoc = dtResult.Rows[0]["Message"].ToString();

                                var resultData = "{ \"MessageID\": " + messageID.ToString() + ", \"Message\": \"" + xmlDoc + "\" }";

                                // If successful
                                result.Data = resultData;
                                result.Success = true;
                                result.Message = "Document successfully retrieved.";
                                result.MessageDetail = "New CargoWise document was successfully retrieved.";
                            }
                            else
                            {
                                result.Success = true;
                                result.Message = "No documents available.";
                                result.MessageDetail = "No new CargoWise XML documents available at this moment. Please try again later.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                        result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                #endregion Get CargoWise XML Documents
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);
            }

            return result;
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse GetXMLDocs(string key, string type)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                // XML document result will be passed in here as a string
                string messageDetail = string.Empty;

                string centralDBURL = ConfigurationManager.AppSettings["LandseaCD"].ToString();

                if (string.IsNullOrEmpty(centralDBURL))
                {
                    result.Success = false;
                    result.Message = "Landsea Global Central Database - internal configuration problem";
                    result.MessageDetail = "Landsea Global - Landsea Global Central Database connection was not configured correctly.";
                    return result;
                }

                #region Get CargoWise XML Documents
                using (SqlConnection conn = new SqlConnection(centralDBURL))
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Open();

                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("CargoWiseFileProcess", conn))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;

                            sqlCommand.Parameters.AddWithValue("@XMLType", type);
                            sqlCommand.Parameters.AddWithValue("@KeyValue", key);

                            SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
                            DataTable dtResult = new DataTable();

                            sda.Fill(dtResult);

                            int messageID = int.Parse(dtResult.Rows[0]["MessageID"].ToString());
                            string xmlDoc = dtResult.Rows[0]["Message"].ToString();

                            conn.Close();

                            if (xmlDoc != null && xmlDoc.Length > 0)
                            {
                                var resultData = "{ \"MessageID\": " + messageID.ToString() + ", \"Message\": \"" + xmlDoc + "\" }";

                                // If successful
                                result.Data = resultData;
                                result.Success = true;
                                result.Message = "Document successfully retrieved.";
                                result.MessageDetail = "New CargoWise document was successfully retrieved.";
                            }
                            else
                            {
                                result.Success = true;
                                result.Message = "No documents available.";
                                result.MessageDetail = "No new CargoWise XML documents available at this moment. Please try again later.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                        result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                #endregion Get CargoWise XML Documents
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);
            }

            return result;
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse SAPProcessed(int messageID)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                // XML document result will be passed in here as a string
                string messageDetail = string.Empty;

                string centralDBURL = ConfigurationManager.AppSettings["LandseaCD"].ToString();

                if (string.IsNullOrEmpty(centralDBURL))
                {
                    result.Success = false;
                    result.Message = "Landsea Global Central Database - internal configuration problem";
                    result.MessageDetail = "Landsea Global - Landsea Global Central Database connection was not configured correctly.";
                    return result;
                }

                #region Get CargoWise XML Documents
                using (SqlConnection conn = new SqlConnection(centralDBURL))
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Open();

                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("CargoWiseFileLandseaProcessed", conn))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;

                            sqlCommand.Parameters.AddWithValue("@MessageID", messageID);

                            object returnMessageIDObject = sqlCommand.ExecuteScalar();

                            int returnMessageID = (int)returnMessageIDObject;

                            var resultData = "{ \"messageID\": " + returnMessageID + " }";

                            conn.Close();

                            if (returnMessageID > 0)
                            {
                                // If successful
                                result.Data = resultData;
                                result.Success = true;
                                result.Message = "SAP Message Processed flag successfully updated.";
                                result.MessageDetail = "SAP Message Processed flag successfully updated.";
                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "SAP Message Processed flag update failed.";
                                result.MessageDetail = "SAP Message Processed flag update failed - please ensure the Message ID is correct.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                        result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                #endregion Get CargoWise XML Documents
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);
            }

            return result;
        }


    }
}
