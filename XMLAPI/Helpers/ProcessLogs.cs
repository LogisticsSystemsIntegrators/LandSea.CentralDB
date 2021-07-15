using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using BIP.Enum;
using BIP.DataModel;
using BSW.APIResponse;
using System.IO;

namespace XMLAPI
{
    public static class ProcessLogs
    {
        public static string webApiUrl { get; set; }
        public static string bipToken { get; set; }
        public static string logFilePath { get; set; }

        public static bool UpdateProfileHistory(string eventDesc, EventLogType etype, int activeProfileID)
        {
            bool result = true;
            try
            {
                if (activeProfileID <= 0)
                {
                    UpdateEventLog(eventDesc,etype);
                    return result;
                }
                else
                {

                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    using (HttpClient client = new HttpClient(new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic }))
                    {
                        client.BaseAddress = new Uri(webApiUrl);
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", bipToken);

                        //update the event log
                        List<ProfileHistoryModel> lstevent = new List<ProfileHistoryModel>();
                        lstevent.Add(new ProfileHistoryModel
                        {
                            EventTypeID = (short)etype,
                            DoneBy = "LandSea XML API Service",
                            EventDesc = eventDesc,
                            ProfileID = activeProfileID
                        });

                        var res = client.PutAsJsonAsync("api/ProfileHistory/AddHistoryEntry/", lstevent).Result;
                        if (!res.IsSuccessStatusCode)
                        {
                            WriteToLogFile(res.Content.ReadAsStringAsync().Result);
                        }
                        else
                        {
                            //get the result and check if the resutl was success
                            RequestReponse presult = res.Content.ReadAsAsync<RequestReponse>().Result;
                            if (!presult.Success)
                            {
                                WriteToLogFile(presult.Message + " Detail: " + presult.MessageDetail);
                            }
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                result = false;
                UpdateEventLog(BSW.APIResponse.ExceptionDetail.GetExceptionFullMessage(exp), EventLogType.Error);
            }

            return result;
        }

        public static void UpdateEventLog(string desc, EventLogType etype)
        {
            try
            {

                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                using (HttpClient client = new HttpClient(new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic }))
                {

                    client.BaseAddress = new Uri(webApiUrl);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", bipToken);

                    //update the event log
                    List<EventLogModel> lstevent = new List<EventLogModel>();
                    lstevent.Add(new EventLogModel
                    {
                        EventTypeID = (short)etype,
                        EventProcess = "LandSea XML API Service",
                        EventDesc = desc,
                        ServiceName = "Rest API or Soap Service",
                        ServerName = Environment.MachineName
                    });

                    var res = client.PutAsJsonAsync("api/EventLog/AddLogEntry/", lstevent).Result;
                    if (!res.IsSuccessStatusCode)
                    {
                        WriteToLogFile(res.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        //get the result and check if the resutl was success
                        RequestReponse result = res.Content.ReadAsAsync<RequestReponse>().Result;
                        if (!result.Success)
                        {
                            WriteToLogFile(result.Message + " Detail: " + result.MessageDetail);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                WriteToLogFile(BSW.APIResponse.ExceptionDetail.GetExceptionFullMessage(exp));
            }
        }

        public static void WriteToLogFile(string logMessage)
        {
           
            string strLogMessage = string.Empty;

            if (string.IsNullOrEmpty(logFilePath))
                logFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/LogFiles");

            string strLogFile = logFilePath + @"\LandSeaXMLAPI_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
            StreamWriter swLog;

            try
            {
                if (!Directory.Exists(logFilePath))
                    Directory.CreateDirectory(logFilePath);

                strLogMessage = string.Format("Processing - {0} : {1} ", DateTime.Now, logMessage);

                if (!File.Exists(strLogFile))
                {
                    swLog = new StreamWriter(strLogFile);
                }
                else
                {
                    swLog = File.AppendText(strLogFile);
                }
                swLog.WriteLine(strLogMessage);

                //we need to make sure we close the log file
                swLog.Close();
                swLog.Dispose();
            }
            catch (Exception exp)
            {

            }

        }
    }
}