using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;

using BIP.DataModel;
using BIP.Enum;
using BIP.BaseAdapter;
using BIP.MessageUtils;
using BSW.APIResponse;

namespace XMLAPI.Controllers
{
    public class XMLDocumentsController : ApiController
    {
        private string bipAPIURL = string.Empty;
        private string bipToken = string.Empty;
        private string logFilePath = string.Empty;
        private int GetXMLDocsProfileID = 0;
        private int SAPProcessedProfileID = 0;

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse GetXMLDocs([FromUri]string type)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                // XML document result will be passed in here as a string
                string messageDetail = string.Empty;

                string centralDBURL = ConfigurationManager.ConnectionStrings["LandseaDB"].ToString();

                if (!LoadSettings())
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Failed to read configuration settings.";
                    return result;
                }

                if (string.IsNullOrEmpty(type))
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Document Type is missing.";
                    return result;
                }

                if (!GenerateBIPToken())
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Internal security token generation failed";
                    return result;
                }

                ProcessLogs.bipToken = bipToken;
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.webApiUrl = bipAPIURL;

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

                                ProcessLogs.UpdateProfileHistory(string.Join(" - ", result.Message, result.MessageDetail), BIP.Enum.EventLogType.Information, GetXMLDocsProfileID);
                            }
                            else
                            {
                                result.Success = true;
                                result.Message = "No documents available.";
                                result.MessageDetail = "No new CargoWise XML documents available at this moment. Please try again later.";

                                ProcessLogs.UpdateProfileHistory(string.Join(" - ", result.Message, result.MessageDetail), BIP.Enum.EventLogType.Warning, GetXMLDocsProfileID);
                            }
                        }

                        #region BIP Message
                        //we need to create a new message in BIP
                        BaseMessage bmessage = new BaseMessage();
                        List<MessageHistoryModel> newHistory = new List<MessageHistoryModel>();

                        using (DataSet ds = new DataSet("XMLDocs"))
                        {
                            using (DataTable dt = new DataTable("XMLDocsGet"))
                            {
                                dt.Columns.Add("RequestedType", typeof(string));
                                dt.Columns.Add("ResultMsg", typeof(string));
                                dt.Columns.Add("ResultData", typeof(string));
                                dt.Columns.Add("ResultDetailMsg", typeof(string));
                                dt.AcceptChanges();

                                DataRow dr = dt.NewRow();
                                dr["RequestedType"] = type;
                                dr["ResultMsg"] = result.Message;
                                dr["ResultData"] = result.Data;
                                dr["ResultDetailMsg"] = result.MessageDetail;

                                dt.Rows.Add(dr);
                                dt.AcceptChanges();

                                ds.Tables.Add(dt);
                                ds.AcceptChanges();

                                using (TextWriter write = new StringWriter())
                                {
                                    //convert the results into xml   
                                    ds.WriteXml(write);
                                    //also convert the xml into byte arry
                                    bmessage.Context = new byte[write.ToString().Length * sizeof(char)];
                                    System.Buffer.BlockCopy(write.ToString().ToCharArray(), 0, bmessage.Context, 0, bmessage.Context.Length);
                                }
                            }
                        }

                        if (result.Success)
                        {
                            bmessage.PublishMessageID = (int)InternalStatus.Processing;
                            bmessage.MessageStatus = InternalStatus.Processing;
                            newHistory.Add(new MessageHistoryModel
                            {
                                EventDesc = "Request received from LandSea XML API Service - Get XML Documents",
                                ProfileProcessID = GetXMLDocsProfileID,
                                EventTypeID = (byte)EventLogType.Information,
                                MessageStatusID = 1,
                                DoneBy = "LandSea API Service"
                            });
                        }
                        else
                        {
                            bmessage.PublishMessageID = (int)InternalStatus.Suspended;
                            bmessage.MessageStatus = InternalStatus.Suspended;
                            newHistory.Add(new MessageHistoryModel
                            {
                                EventDesc = result.Message + " Detail: " + result.MessageDetail,
                                ProfileProcessID = GetXMLDocsProfileID,
                                EventTypeID = (byte)EventLogType.Error,
                                MessageStatusID = 2,
                                DoneBy = "LandSea API Service"
                            });
                        }
                        bmessage.PromoteValue("SentType", type);
                        bmessage.webApiUrl = bipAPIURL;
                        bmessage.bipToken = bipToken;
                        bmessage.AttachmentID = 0;
                        bmessage.XMLContext = string.Empty;
                        bmessage.ProfileID = GetXMLDocsProfileID;
                        bmessage.CreatedBy = "LandSea XML API Service";
                        bmessage.ReProcessed = false;
                        bmessage.PublishMessageID = null;
                        bmessage.ProfileProcessID = GetXMLDocsProfileID;

                        bool saveResult = true;
                        using (UpdateMessage sMessage = new BIP.MessageUtils.UpdateMessage())
                        {
                            saveResult = sMessage.SaveMessageDetail(bmessage, newHistory, BIP.Enum.MessageType.Incomming, ref messageDetail);
                        }

                        if (!saveResult)
                        {
                            result.Success = false;
                            result.Message = "Failed to Update BIP process";
                            result.MessageDetail = messageDetail;
                        }
                        #endregion BIP Message
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                        result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);

                        ProcessLogs.UpdateProfileHistory(result.Message, BIP.Enum.EventLogType.Error, GetXMLDocsProfileID);
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

                ProcessLogs.UpdateProfileHistory(result.Message, BIP.Enum.EventLogType.Error, GetXMLDocsProfileID);
            }

            return result;
        }

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse GetXMLDocs([FromUri]string key, [FromUri]string type)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                // XML document result will be passed in here as a string
                string messageDetail = string.Empty;

                string centralDBURL = ConfigurationManager.ConnectionStrings["LandseaDB"].ToString();

                if (!LoadSettings())
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Failed to read configuration settings.";
                    return result;
                }

                if (string.IsNullOrEmpty(key))
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Document Message ID (Key) is missing.";
                    return result;
                }

                if (string.IsNullOrEmpty(type))
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Document Type is missing.";
                    return result;
                }

                if (!GenerateBIPToken())
                {
                    result.Success = false;
                    result.Message = "Get XML Documents failed!";
                    result.MessageDetail = "Internal security token generation failed";
                    return result;
                }

                ProcessLogs.bipToken = bipToken;
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.webApiUrl = bipAPIURL;

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

                                ProcessLogs.UpdateProfileHistory(string.Join(" - ", result.Message, result.MessageDetail), BIP.Enum.EventLogType.Information, GetXMLDocsProfileID);
                            }
                            else
                            {
                                result.Success = true;
                                result.Message = "No documents available.";
                                result.MessageDetail = "No new CargoWise XML documents available at this moment. Please try again later.";

                                ProcessLogs.UpdateProfileHistory(string.Join(" - ", result.Message, result.MessageDetail), BIP.Enum.EventLogType.Warning, GetXMLDocsProfileID);
                            }
                        }

                        #region BIP Message
                        //we need to create a new message in BIP
                        BaseMessage bmessage = new BaseMessage();
                        List<MessageHistoryModel> newHistory = new List<MessageHistoryModel>();

                        using (DataSet ds = new DataSet("XMLDocs"))
                        {
                            using (DataTable dt = new DataTable("XMLDocsGet"))
                            {
                                dt.Columns.Add("RequestedMessageID", typeof(string));
                                dt.Columns.Add("RequestedType", typeof(string));
                                dt.Columns.Add("ResultMsg", typeof(string));
                                dt.Columns.Add("ResultData", typeof(string));
                                dt.Columns.Add("ResultDetailMsg", typeof(string));
                                dt.AcceptChanges();

                                DataRow dr = dt.NewRow();
                                dr["RequestedMessageID"] = key;
                                dr["RequestedType"] = type;
                                dr["ResultMsg"] = result.Message;
                                dr["ResultData"] = result.Data;
                                dr["ResultDetailMsg"] = result.MessageDetail;

                                dt.Rows.Add(dr);
                                dt.AcceptChanges();

                                ds.Tables.Add(dt);
                                ds.AcceptChanges();

                                using (TextWriter write = new StringWriter())
                                {
                                    //convert the results into xml   
                                    ds.WriteXml(write);
                                    //also convert the xml into byte arry
                                    bmessage.Context = new byte[write.ToString().Length * sizeof(char)];
                                    System.Buffer.BlockCopy(write.ToString().ToCharArray(), 0, bmessage.Context, 0, bmessage.Context.Length);
                                }
                            }
                        }

                        if (result.Success)
                        {
                            bmessage.PublishMessageID = (int)InternalStatus.Processing;
                            bmessage.MessageStatus = InternalStatus.Processing;
                            newHistory.Add(new MessageHistoryModel
                            {
                                EventDesc = "Request received from LandSea XML API Service - Get XML Documents",
                                ProfileProcessID = GetXMLDocsProfileID,
                                EventTypeID = (byte)EventLogType.Information,
                                MessageStatusID = 1,
                                DoneBy = "LandSea API Service"
                            });
                        }
                        else
                        {
                            bmessage.PublishMessageID = (int)InternalStatus.Suspended;
                            bmessage.MessageStatus = InternalStatus.Suspended;
                            newHistory.Add(new MessageHistoryModel
                            {
                                EventDesc = result.Message + " Detail: " + result.MessageDetail,
                                ProfileProcessID = GetXMLDocsProfileID,
                                EventTypeID = (byte)EventLogType.Error,
                                MessageStatusID = 2,
                                DoneBy = "LandSea API Service"
                            });
                        }

                        bmessage.PromoteValue("RequestedMessageID", type);
                        bmessage.PromoteValue("SentType", type);
                        bmessage.webApiUrl = bipAPIURL;
                        bmessage.bipToken = bipToken;
                        bmessage.AttachmentID = 0;
                        bmessage.XMLContext = string.Empty;
                        bmessage.ProfileID = GetXMLDocsProfileID;
                        bmessage.CreatedBy = "LandSea XML API Service";
                        bmessage.ReProcessed = false;
                        bmessage.PublishMessageID = null;
                        bmessage.ProfileProcessID = GetXMLDocsProfileID;

                        bool saveResult = true;
                        using (UpdateMessage sMessage = new BIP.MessageUtils.UpdateMessage())
                        {
                            saveResult = sMessage.SaveMessageDetail(bmessage, newHistory, BIP.Enum.MessageType.Incomming, ref messageDetail);
                        }

                        if (!saveResult)
                        {
                            result.Success = false;
                            result.Message = "Failed to Update BIP process";
                            result.MessageDetail = messageDetail;
                        }
                        #endregion BIP Message
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
        public RequestReponse SAPProcessed([FromUri]int messageID)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                // XML document result will be passed in here as a string
                string messageDetail = string.Empty;

                string centralDBURL = ConfigurationManager.ConnectionStrings["LandseaDB"].ToString();

                if (!LoadSettings())
                {
                    result.Success = false;
                    result.Message = "XML Documents Update SAP Processed failed!";
                    result.MessageDetail = "Failed to read configuration settings.";
                    return result;
                }

                if (string.IsNullOrEmpty(messageID.ToString()))
                {
                    result.Success = false;
                    result.Message = "XML Documents Update SAP Processed failed!";
                    result.MessageDetail = "Message ID is missing.";
                    return result;
                }

                if (!GenerateBIPToken())
                {
                    result.Success = false;
                    result.Message = "XML Documents Update SAP Processed failed!";
                    result.MessageDetail = "Internal security token generation failed";
                    return result;
                }

                ProcessLogs.bipToken = bipToken;
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.webApiUrl = bipAPIURL;

                #region Set CargoWise XML as Processed by SAP
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

                                ProcessLogs.UpdateProfileHistory(string.Join(" - ", result.Message, result.MessageDetail), BIP.Enum.EventLogType.Information, SAPProcessedProfileID);
                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "SAP Message Processed flag update failed.";
                                result.MessageDetail = "SAP Message Processed flag update failed - please ensure the Message ID is correct.";

                                ProcessLogs.UpdateProfileHistory(string.Join(" - ", result.Message, result.MessageDetail), BIP.Enum.EventLogType.Error, SAPProcessedProfileID);
                            }
                        }

                        #region BIP Message
                        //we need to create a new message in BIP
                        BaseMessage bmessage = new BaseMessage();
                        List<MessageHistoryModel> newHistory = new List<MessageHistoryModel>();

                        using (DataSet ds = new DataSet("SAPProcessed"))
                        {
                            using (DataTable dt = new DataTable("SAPProcessed"))
                            {
                                dt.Columns.Add("RequestedMessageID", typeof(string));
                                dt.Columns.Add("ResultMsg", typeof(string));
                                dt.Columns.Add("ResultData", typeof(string));
                                dt.Columns.Add("ResultDetailMsg", typeof(string));
                                dt.AcceptChanges();

                                DataRow dr = dt.NewRow();
                                dr["RequestedMessageID"] = messageID;
                                dr["ResultMsg"] = result.Message;
                                dr["ResultData"] = result.Data;
                                dr["ResultDetailMsg"] = result.MessageDetail;

                                dt.Rows.Add(dr);
                                dt.AcceptChanges();

                                ds.Tables.Add(dt);
                                ds.AcceptChanges();

                                using (TextWriter write = new StringWriter())
                                {
                                    //convert the results into xml   
                                    ds.WriteXml(write);
                                    //also convert the xml into byte arry
                                    bmessage.Context = new byte[write.ToString().Length * sizeof(char)];
                                    System.Buffer.BlockCopy(write.ToString().ToCharArray(), 0, bmessage.Context, 0, bmessage.Context.Length);
                                }
                            }
                        }

                        if (result.Success)
                        {
                            bmessage.PublishMessageID = (int)InternalStatus.Processing;
                            bmessage.MessageStatus = InternalStatus.Processing;
                            newHistory.Add(new MessageHistoryModel
                            {
                                EventDesc = "Request received from LandSea XML API Service - Get XML Documents",
                                ProfileProcessID = SAPProcessedProfileID,
                                EventTypeID = (byte)EventLogType.Information,
                                MessageStatusID = 1,
                                DoneBy = "LandSea API Service"
                            });
                        }
                        else
                        {
                            bmessage.PublishMessageID = (int)InternalStatus.Suspended;
                            bmessage.MessageStatus = InternalStatus.Suspended;
                            newHistory.Add(new MessageHistoryModel
                            {
                                EventDesc = result.Message + " Detail: " + result.MessageDetail,
                                ProfileProcessID = SAPProcessedProfileID,
                                EventTypeID = (byte)EventLogType.Error,
                                MessageStatusID = 2,
                                DoneBy = "LandSea API Service"
                            });
                        }

                        bmessage.PromoteValue("RequestedMessageID", messageID.ToString());
                        bmessage.webApiUrl = bipAPIURL;
                        bmessage.bipToken = bipToken;
                        bmessage.AttachmentID = 0;
                        bmessage.XMLContext = string.Empty;
                        bmessage.ProfileID = SAPProcessedProfileID;
                        bmessage.CreatedBy = "LandSea XML API Service";
                        bmessage.ReProcessed = false;
                        bmessage.PublishMessageID = null;
                        bmessage.ProfileProcessID = SAPProcessedProfileID;

                        bool saveResult = true;
                        using (UpdateMessage sMessage = new BIP.MessageUtils.UpdateMessage())
                        {
                            saveResult = sMessage.SaveMessageDetail(bmessage, newHistory, BIP.Enum.MessageType.Incomming, ref messageDetail);
                        }

                        if (!saveResult)
                        {
                            result.Success = false;
                            result.Message = "Failed to Update BIP process";
                            result.MessageDetail = messageDetail;
                        }
                        #endregion BIP Message
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                        result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);

                        ProcessLogs.UpdateProfileHistory(result.Message, BIP.Enum.EventLogType.Error, SAPProcessedProfileID);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                #endregion Set CargoWise XML as Processed by SAP
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);

                ProcessLogs.UpdateProfileHistory(result.Message, BIP.Enum.EventLogType.Error, SAPProcessedProfileID);
            }

            return result;
        }

        #region Private BIP settings

        private bool LoadSettings()
        {
            try
            {
                bipAPIURL = ConfigurationManager.AppSettings["BIPApiURL"];
                logFilePath = ConfigurationManager.AppSettings["LogFilePath"].ToString();

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["XMLDocsGet"]))
                {
                    GetXMLDocsProfileID = int.Parse(ConfigurationManager.AppSettings["XMLDocsGet"]);
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["XMLDocsSAPProcessedUpdate"]))
                {
                    SAPProcessedProfileID = int.Parse(ConfigurationManager.AppSettings["XMLDocsSAPProcessedUpdate"]);
                }
            }
            catch (Exception exp)
            {
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.WriteToLogFile("Error Reading Settings:" + exp.Message);
                return false;
            }

            return true;
        }

        private bool GenerateBIPToken()
        {
            try
            {
                using (BSW.APIToken.Token t = new BSW.APIToken.Token())
                {
                    var result = t.GenerateToken("1", "Landsea-XMLAPIService", 10, "Landsea - XMLAPIService", "");
                    if (result.Successful)
                    {
                        bipToken = result.UserToken;
                    }
                    else
                    {
                        bipToken = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.WriteToLogFile("Generating BIP Token:" + ExceptionDetail.GetExceptionFullMessage(ex));
                return false;
            }
            return true;
        }
        #endregion Private BIP settings
    }
}
