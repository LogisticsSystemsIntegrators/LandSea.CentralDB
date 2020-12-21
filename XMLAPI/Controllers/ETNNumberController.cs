using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XMLAPI.DataAccess;
using BSW.APIResponse;
using System.Web.Http.Cors;
using System.Configuration;
using BIP.BaseAdapter;
using BIP.MessageUtils;
using System.Data;
using System.IO;
using BIP.DataModel;
using BIP.Enum;

namespace XMLAPI.Controllers
{
    public class ETNNumberController : ApiController
    {
        private string bipAPIURL = string.Empty;
        private string bipToken = string.Empty;
        private string logFilePath = string.Empty;
        private int ActiveProfileID = 0;
        private int ProfileProcessID = 0;

        [HttpPut]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse Update([FromBody]ETNNumberModel etnDetail)
        {
            RequestReponse result = new RequestReponse();
            try
            {
                string msg = string.Empty;

                if (string.IsNullOrEmpty(etnDetail.CaroWiseKey))
                {
                    result.Success = false;
                    result.Message = "Update ETN Number Failed!";
                    result.MessageDetail = "CargoWise Key value is missing";
                    return result;
                }
                
                if(!LoadSettings())
                {
                    result.Success = false;
                    result.Message = "ETA Number Update Failed failed!";
                    result.MessageDetail = "Failed to read configuration settings";
                    return result;
                }

                if(!GenerateBIPToken())
                {
                    result.Success = false;
                    result.Message = "ETA Number Update Failed failed!";
                    result.MessageDetail = "Internal security token generation failed";
                    return result;
                }

                ProcessLogs.bipToken = bipToken;
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.webApiUrl = bipAPIURL;
                

                using (CargoWiseFileProvider provider = new CargoWiseFileProvider())
                {
                    if (!provider.UpdateETNNumber(etnDetail, ref msg))
                    {
                        result.Success = false;
                        result.Message = "ETA Number Update Failed!";
                        result.MessageDetail = msg;
                    }
                    else
                    {
                        result.Success = true;
                        result.Message = "ETA Number successfully updated.";
                        result.MessageDetail = string.Empty;
                    }
                }


                //we need to create a new message in BIP
                BaseMessage bmessage = new BaseMessage();
                List<MessageHistoryModel> newHistory = new List<MessageHistoryModel>();

                using (DataSet ds = new DataSet("ETNNUmberUpdate"))
                {
                    using (DataTable dt = new DataTable("RequestDetail"))
                    {
                        dt.Columns.Add("Key", typeof(string));
                        dt.Columns.Add("ETNNumber", typeof(string));
                        dt.Columns.Add("GIBInvoiceNumber", typeof(string));
                        dt.Columns.Add("ResultMsg", typeof(string));
                        dt.Columns.Add("ResultDetailMsg", typeof(string));
                        dt.AcceptChanges();

                        DataRow dr = dt.NewRow();
                        dr["Key"] = etnDetail.CaroWiseKey;
                        dr["ETNNumber"] = etnDetail.ETNNumber;
                        dr["GIBInvoiceNumber"] = etnDetail.GIBInvoiceNumber;
                        dr["ResultMsg"] = result.Message;
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
                        EventDesc = "Request received from LandSea XML API Service - Update ETN Number",
                        ProfileProcessID = ProfileProcessID,
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
                        ProfileProcessID = ProfileProcessID,
                        EventTypeID = (byte)EventLogType.Error,
                        MessageStatusID = 2,
                        DoneBy = "LandSea API Service"
                    });
                }
                bmessage.PromoteValue("CargoWiseKey", etnDetail.CaroWiseKey);
                bmessage.PromoteValue("ETNNumber", etnDetail.ETNNumber);
                bmessage.PromoteValue("GIBInvoiceNumber", etnDetail.GIBInvoiceNumber);
                bmessage.webApiUrl = bipAPIURL;
                bmessage.bipToken = bipToken;
                bmessage.AttachmentID = 0;
                bmessage.XMLContext = string.Empty;
                bmessage.ProfileID = ActiveProfileID;
                bmessage.CreatedBy = "LandSea XML API Service";
                bmessage.ReProcessed = false;
                bmessage.PublishMessageID = null;
                bmessage.ProfileProcessID = ProfileProcessID;

                bool saveResult = true;
                using (UpdateMessage sMessage = new BIP.MessageUtils.UpdateMessage())
                {
                    saveResult = sMessage.SaveMessageDetail(bmessage, newHistory, BIP.Enum.MessageType.Incomming, ref msg);
                }

                if(!saveResult)
                {
                    result.Success = false;
                    result.Message = "Failed to Update BIP process";
                    result.MessageDetail = msg;
                }


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);

                ProcessLogs.UpdateProfileHistory(result.Message, BIP.Enum.EventLogType.Error, ActiveProfileID);
            }

            return result;
        }


        #region - Private methods -

        private bool LoadSettings()
        {
            try
            {
                bipAPIURL = ConfigurationManager.AppSettings["BIPApiURL"];
                logFilePath = ConfigurationManager.AppSettings["LogFilePath"].ToString();

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProfileIDETNNumberUpdate"]))
                    ActiveProfileID = int.Parse(ConfigurationManager.AppSettings["ProfileIDETNNumberUpdate"]);
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProfileProcessIDETNNumberUpdate"]))
                    ProfileProcessID = int.Parse(ConfigurationManager.AppSettings["ProfileProcessIDETNNumberUpdate"]);

            }
            catch (Exception exp)
            {
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.writeToLogFile("Error Reading Settings:" + exp.Message);
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
                ProcessLogs.writeToLogFile("Generating BIP Token:" + ExceptionDetail.GetExceptionFullMessage(ex));
                return false;
            }
            return true;
        }

        #endregion

    }
}