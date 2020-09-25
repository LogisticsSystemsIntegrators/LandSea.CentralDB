using Newtonsoft.Json;
using System;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

using BSW.APIResponse;
using XMLAPI.Objects;

namespace XMLAPI.Controllers
{
    public class XMLDocumentsController : ApiController
    {
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse GetXMLDocs(string key, ObjectTypes.XMLType type)
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

                            string xmlDoc = (string)sqlCommand.ExecuteScalar();

                            conn.Close();

                            if (xmlDoc != null && xmlDoc.Length > 0)
                            {
                                // If successful
                                result.Data = JsonConvert.SerializeObject(xmlDoc);
                                result.Success = true;
                                result.Message = "Document successfully retrieved.";
                                result.MessageDetail = "New CargoWise document was successfully retrieved";
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

        [HttpPost]
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

                            int returnMessageID = (int)sqlCommand.ExecuteScalar();

                            conn.Close();

                            if (returnMessageID >= -1)
                            {
                                // If successful
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
