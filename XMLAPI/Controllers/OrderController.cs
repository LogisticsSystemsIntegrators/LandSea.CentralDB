using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Cors;
using BSW.APIResponse;
using System.Web.Http;
using XMLAPI.Objects;
using XMLAPI;

using BIP.BaseAdapter;
using BIP.DataModel;

namespace XMLAPI.Controllers
{
    public class OrderController : Controller
    {
        // Current LandSeaOrderController process flow
        private string bipAPIURL = string.Empty;
        private string bipToken = string.Empty;
        private string logFilePath = string.Empty;
        private int ActiveProfileID = 0;
        private int ProfileProcessID = 0;

        [System.Web.Mvc.HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse Post([FromBody] string value)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                #region Token Validation
                string msg = string.Empty;
                string cwRequestResult = string.Empty;

                if (string.IsNullOrEmpty(value))
                {
                    result.Success = false;
                    result.Message = "Landsea Global Order Upsert Failed!";
                    result.MessageDetail = "Landsea Global Order detail is missing";
                    return result;
                }

                if (!LoadSettings())
                {
                    result.Success = false;
                    result.Message = "Landsea Global Order Upsert Failed!";
                    result.MessageDetail = "Failed to read configuration settings";
                    return result;
                }

                if (!GenerateBIPToken())
                {
                    result.Success = false;
                    result.Message = "Landsea Global Order Upsert Failed!";
                    result.MessageDetail = "Internal security token generation failed";
                    return result;
                }

                ProcessLogs.bipToken = bipToken;
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.webApiUrl = bipAPIURL;

                #endregion Token Validation

                if (string.IsNullOrEmpty(value))
                {
                    result.Success = false;
                    result.Message = "'Value' parameter is required and cannot be blank";
                    ProcessLogs.WriteToLogFile("Value parameter is required and cannot be blank - Landsea Global Order Upsert");
                    return result;
                }


                if (!string.IsNullOrEmpty(bipToken))
                {
                    ProcessLogs.UpdateProfileHistory("New message post request received", BIP.Enum.EventLogType.Information, ActiveProfileID);
                    //we need to convert the string into byte array - and create a new message
                    byte[] msgBytes = System.Text.Encoding.UTF8.GetBytes(value);

                    BaseMessage bMessage = new BaseMessage();
                    bMessage.ProfileID = ActiveProfileID;
                    bMessage.bipToken = bipToken;
                    bMessage.AttachmentID = 0;
                    bMessage.XMLContext = string.Empty;
                    bMessage.webApiUrl = bipAPIURL;

                    bMessage.Context = msgBytes;
                    bMessage.MessageStatus = InternalStatus.Processing;
                    bMessage.ReProcessed = false;
                    bMessage.ProfileProcessID = ProfileProcessID;
                    bMessage.CreatedBy = "Landsea Global Order Upsert";
                    bMessage.ListPromotedValues = new List<PromoteProperties>();

                    if (bMessage.ListPromotedValues != null)
                    {
                        bMessage.ListPromotedValues.AddRange(bMessage.ListPromotedValues);
                    }

                    List<MessageHistoryModel> newHistory = new List<MessageHistoryModel>();
                    newHistory.Add(new MessageHistoryModel
                    {
                        EventDesc = "Message received from API controller.",
                        EventTypeID = (byte)BIP.Enum.EventLogType.Information,
                        MessageStatusID = 1,
                        ProfileProcessID = ProfileProcessID,
                        DoneBy = "Landsea Global Order Upsert"
                    });

                    bool saveResult = true;

                    using (BIP.MessageUtils.UpdateMessage sMessage = new BIP.MessageUtils.UpdateMessage())
                    {
                        saveResult = sMessage.SaveMessageDetail(bMessage, newHistory, BIP.Enum.MessageType.Incomming, ref msg);
                    }

                    if (!saveResult)
                    {
                        result.Success = false;
                        result.Message = "Request Failed - no message was created in BIP";
                        result.MessageDetail = msg;
                    }
                    else
                    {
                        result.Success = true;
                        result.Message = "Request was successfully received by BIP";
                        result.MessageDetail = "New Message was successfully created and will be processed.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Request was not Processed";
                result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(ex);

                ProcessLogs.UpdateProfileHistory("Landsea Global Order Upsert: " + ExceptionDetail.GetExceptionFullMessage(ex), BIP.Enum.EventLogType.Error, ActiveProfileID);
            }

            return result;
        }

        #region Private methods

        private bool LoadSettings()
        {
            try
            {
                bipAPIURL = ConfigurationManager.AppSettings["BIPApiURL"];
                logFilePath = ConfigurationManager.AppSettings["LogFilePath"].ToString();

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OrderUpsertProfileID"]))
                {
                    ActiveProfileID = int.Parse(ConfigurationManager.AppSettings["OrderUpsertProfileID"]);
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OrderUpsertProfileProcessID"]))
                {
                    ProfileProcessID = int.Parse(ConfigurationManager.AppSettings["OrderUpsertProfileProcessID"]);
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

        #endregion Private methods
    }
}