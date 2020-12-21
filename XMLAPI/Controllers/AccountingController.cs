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
using XMLAPI.CargoWisePaymentService;
using System.Xml.Serialization;
using System.Text;

namespace XMLAPI.Controllers
{
    public class AccountingController : ApiController
    {
        private string bipAPIURL = string.Empty;
        private string bipToken = string.Empty;
        private string logFilePath = string.Empty;
        private int ActiveProfileID = 0;
        private int ProfileProcessID = 0;
        private string paymentURL = string.Empty;
        private string paymentUser = string.Empty;
        private string paymentPWD = string.Empty;

        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse UpdateInvoicePaymentDetails([FromBody]string paymentDetails)
        {
            RequestReponse result = new RequestReponse();
            try
            {
                string msg = string.Empty;
                string cwRequestResult = string.Empty;

                if (string.IsNullOrEmpty(paymentDetails))
                {
                    result.Success = false;
                    result.Message = "Update Invoice Payment Details Failed!";
                    result.MessageDetail = "Payment Details is missing";
                    return result;
                }

                if (!LoadSettings())
                {
                    result.Success = false;
                    result.Message = "Update Invoice Payment Details Failed!";
                    result.MessageDetail = "Failed to read configuration settings";
                    return result;
                }

                if (!GenerateBIPToken())
                {
                    result.Success = false;
                    result.Message = "Update Invoice Payment Details Failed!";
                    result.MessageDetail = "Internal security token generation failed";
                    return result;
                }

                ProcessLogs.bipToken = bipToken;
                ProcessLogs.logFilePath = logFilePath;
                ProcessLogs.webApiUrl = bipAPIURL;

                //we need to load the xml into a strcured object - we will use datatable 
                using (DataSet dsRequest = new DataSet())
                {
                    try
                    {
                        using (var strReader = new StringReader(paymentDetails))
                        {
                            dsRequest.ReadXml(strReader);
                        }
                    }
                    catch(Exception excp)
                    {
                        result.Success = false;
                        result.Message = "Invalid Payment Details!";
                        result.MessageDetail = ExceptionDetail.GetExceptionFullMessage(excp);
                        ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                        return result;
                    }
                    //check if we got some thing
                    if (dsRequest.Tables.Count == 0)
                    {
                        result.Success = false;
                        result.Message = "Update Invoice Payment Details Failed!";
                        result.MessageDetail = "Invalid payment detail recevied";
                        ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                        return result;
                    }
                    if (!dsRequest.Tables.Contains("Payment"))
                    {
                        result.Success = false;
                        result.Message = "Update Invoice Payment Details Failed!";
                        result.MessageDetail = "Invalid payment detail recevied";
                        ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                        return result;
                    }


                    UpdateInvoicePaymentDetailsServiceSoapClient serviceClient = new UpdateInvoicePaymentDetailsServiceSoapClient("UpdateInvoicePaymentDetailsServiceSoap");

                    //Update the CargoWise service url and accoutn details
                    serviceClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(paymentURL);
                    SecuritySOAPHeader securityHeader = new SecuritySOAPHeader() { UserName = paymentUser, Password = paymentPWD };

                    //we need to loop thru all the request, we may get more than 1 per request
                    using (DataTable dtPayment = dsRequest.Tables["Payment"])
                    {
                        //we need to check some of the requiered Columns
                        if(!dtPayment.Columns.Contains("CompanyCode"))
                        {
                            result.Success = false;
                            result.Message = "Update Invoice Payment Details Failed!";
                            result.MessageDetail = "Company Code node was not supplied";
                            ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                            return result;
                        }
                        if (!dtPayment.Columns.Contains("OrgCode"))
                        {
                            result.Success = false;
                            result.Message = "Update Invoice Payment Details Failed!";
                            result.MessageDetail = "Organization Code node was not supplied";
                            ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                            return result;
                        }
                        if (!dtPayment.Columns.Contains("LedgerType"))
                        {
                            result.Success = false;
                            result.Message = "Update Invoice Payment Details Failed!";
                            result.MessageDetail = "Account Ledger Type node was not supplied ('AR' or 'AP')";
                            ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                            return result;
                        }
                        if (!dtPayment.Columns.Contains("TransactionType"))
                        {
                            result.Success = false;
                            result.Message = "Update Invoice Payment Details Failed!";
                            result.MessageDetail = "Transaction Type node was not supplied ('INV' or 'CRD')";
                            ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                            return result;
                        }
                        if (!dtPayment.Columns.Contains("Amount"))
                        {
                            result.Success = false;
                            result.Message = "Update Invoice Payment Details Failed!";
                            result.MessageDetail = "Amount node was not supplied";
                            ProcessLogs.UpdateProfileHistory(result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
                            return result;
                        }

                        foreach (DataRow drP in dtPayment.Rows)
                        {
                                                       
                            UpdateInvoicePaymentDetailsRequest cwRequest = new UpdateInvoicePaymentDetailsRequest()
                            {
                                OrgCode = Convert.ToString(drP["OrgCode"]).Trim(),
                                CompanyCode = Convert.ToString(drP["CompanyCode"]).Trim(),
                                AccLedger = Convert.ToString(drP["LedgerType"]).Trim(),
                                TransactionType = Convert.ToString(drP["TransactionType"]).Trim(),
                                AmountPaidInCompanyCurrency = decimal.Parse(Convert.ToString(drP["Amount"])),
                            };

                            //optional fields and values
                            if (dtPayment.Columns.Contains("TransactionNo"))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(drP["TransactionNo"])))
                                    cwRequest.TransactionNumber = Convert.ToString(drP["TransactionNo"]);
                            }
                            if (dtPayment.Columns.Contains("JobNumber"))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(drP["JobNumber"])))
                                    cwRequest.JobTransactionNumber = Convert.ToString(drP["JobNumber"]);
                            }
                            if (dtPayment.Columns.Contains("IntReference"))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(drP["IntReference"])))
                                    cwRequest.InternalReference = Convert.ToString(drP["IntReference"]);
                            }
                            if (dtPayment.Columns.Contains("PaymentReference"))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(drP["PaymentReference"])))
                                    cwRequest.PaymentReference = Convert.ToString(drP["PaymentReference"]);
                            }
                            if (dtPayment.Columns.Contains("PaidUpDate"))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(drP["PaidUpDate"])))
                                    cwRequest.PaymentDate = DateTime.Parse(Convert.ToString(drP["PaidUpDate"]));
                            }

                            UpdateInvoicePaymentDetailsResponse response = serviceClient.UpdateInvoicePaymentDetails(securityHeader, cwRequest);


                            XmlSerializer serializer = new XmlSerializer(typeof(UpdateInvoicePaymentDetailsResponse)); // SuppressCodeSmell Reason = This is sample client application which does not use ZArchitecture
                            using (System.IO.StringWriter writer = new StringWriter())
                            {
                                serializer.Serialize(writer, response);
                                cwRequestResult += writer.GetStringBuilder().ToString();
                            }
                        }

                    }
                }

                //we need to remove the Unicode marker
                cwRequestResult = cwRequestResult.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

                result.Data = cwRequestResult;

                //we need to create a new message in BIP
                BaseMessage bmessage = new BaseMessage();
                List<MessageHistoryModel> newHistory = new List<MessageHistoryModel>();

                bmessage.Context = Encoding.UTF8.GetBytes(cwRequestResult);

                if (result.Success)
                {
                    bmessage.PublishMessageID = (int)InternalStatus.Processing;
                    bmessage.MessageStatus = InternalStatus.Processing;
                    newHistory.Add(new MessageHistoryModel
                    {
                        EventDesc = "Request received from LandSea XML API Service - Update Invoice Payment",
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

                if (!saveResult)
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

                ProcessLogs.UpdateProfileHistory(result.Message + " " + result.MessageDetail, BIP.Enum.EventLogType.Error, ActiveProfileID);
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

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["InvoicePamentUpdateProfileID"]))
                {
                    ActiveProfileID = int.Parse(ConfigurationManager.AppSettings["InvoicePamentUpdateProfileID"]);
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["InvoicePaymentUpdateProcessID"]))
                {
                    ProfileProcessID = int.Parse(ConfigurationManager.AppSettings["InvoicePaymentUpdateProcessID"]);
                }

                paymentURL = ConfigurationManager.AppSettings["CargoWisePaymentURL"];
                paymentUser = ConfigurationManager.AppSettings["CargoWisePaymentUser"];
                paymentPWD = ConfigurationManager.AppSettings["CargoWisePaymentPWD"];
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