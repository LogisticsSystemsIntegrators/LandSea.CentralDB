using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Http;

using Demo.Library;
using Demo.Objects;
using Newtonsoft.Json.Linq;
using BSW.APIResponse;

namespace Demo
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private string authToken, bipAuthURL, landSeaOrderURL, bipAPIURL;

        private void frmMain_Load(object sender, EventArgs e)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();

            bipAuthURL = appSettingsReader.GetValue("BIPAuthURL", typeof(string)).ToString();
            landSeaOrderURL = appSettingsReader.GetValue("LandSeaOrderURL", typeof(string)).ToString();
            bipAPIURL = appSettingsReader.GetValue("BIPAPIURL", typeof(string)).ToString();

            //Testing only
            txtUsername.Text = "LandseaAPIUser";
            txtPassword.Text = "ujD@wxxg_z3Eg9aK";
        }

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            // See Library folder for Authentication.cs code
            Authentication auth = new Authentication();

            authToken = auth.AuthenticationToken(bipAuthURL, txtUsername.Text, txtPassword.Text);
            lblResponseResult.Text = authToken;
        }

        private void btnSelectXML_Click(object sender, EventArgs e)
        {
            ofdSelectXML.InitialDirectory = @"C:\";
            ofdSelectXML.RestoreDirectory = true;

            ofdSelectXML.CheckFileExists = true;
            ofdSelectXML.CheckPathExists = true;

            ofdSelectXML.DefaultExt = "xml";
            ofdSelectXML.Filter = "XML files (*.xml) | *.xml";

            if (ofdSelectXML.ShowDialog() == DialogResult.OK)
            {
                txtSendXMLPath.Text = ofdSelectXML.FileName;
            }

            if (!string.IsNullOrEmpty(txtSendXMLPath.Text.Trim()))
            {
                btnSendXML.Enabled = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdateETN_Click(object sender, EventArgs e)
        {
            APIRequestResponse apiResponse = new APIRequestResponse();
            string tokenReturnValue = string.Empty;
            string tokenvalue = string.Empty;
            System.Net.WebRequest req = System.Net.WebRequest.Create("http://129.232.164.178/BIPEXAPI/Authentication?username=LandseaAPIUser&password=ujD@wxxg_z3Eg9aK");
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "GET";

            string tokenResponse = string.Empty;
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null)
            {
                MessageBox.Show("Authentication Failed");
                return;
            }

            using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
            {
                tokenResponse = sr.ReadToEnd().Trim();
            }
            if (!string.IsNullOrEmpty(tokenResponse))
            {
                tokenReturnValue = "Data";
                tokenvalue = JObject.Parse(tokenResponse)[tokenReturnValue].ToString();
            }

            ETNNumberModel model = new ETNNumberModel
            {
                CaroWiseKey = "S0000100655",
                ETNNumber = "DOC16"
            };
            // Update ETN Numbers linked to Cargo Wise key
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (senderDetail, certificate, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic }))
            {
                string landseaAPIURL = "http://129.232.164.178/LandSeaGlobal/LandseaAPI/api/ETNNumber/";
                //string landseaAPIURL = "http://localhost:51203/api/ETNNumber/";
                client.BaseAddress = new Uri(landseaAPIURL);

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", tokenvalue);
                client.DefaultRequestHeaders.Add("BIPASK", authToken);
                HttpResponseMessage res = client.PutAsJsonAsync("Update",model).Result;

                if (res.IsSuccessStatusCode)
                {
                    apiResponse = res.Content.ReadAsAsync<APIRequestResponse>().Result;
                    MessageBox.Show(apiResponse.ToString());
                }
                else
                {
                    string resultString = res.Content.ReadAsStringAsync().Result;
                    MessageBox.Show(resultString);
                   
                }
            }


        }

        private void btnSendXML_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                APIRequestResponse requestResponse = new APIRequestResponse();
                string sourceFile = txtSendXMLPath.Text;
                string sendString = System.IO.File.ReadAllText(sourceFile);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(bipAPIURL);

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", authToken);
                    client.DefaultRequestHeaders.Add("BIPASK", authToken);
                    var res = client.PostAsJsonAsync("LandSeaOrder/", sendString).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        requestResponse = res.Content.ReadAsAsync<APIRequestResponse>().Result;
                    }
                    else
                    {
                        string resultString = res.Content.ReadAsStringAsync().Result;
                    }
                }

                MessageBox.Show("XML message send successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("XML message send failed.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetXML_Click(object sender, EventArgs e)
        {
            string selectedType = gbxGetXML.Controls.OfType<RadioButton>().FirstOrDefault(n => n.Checked).Text;
            string type = string.Empty;

            if (selectedType == "Organization")
            {
                type = "Organization";
            }
            else if (selectedType == "Shipment")
            {
                type = "ForwardingShipment";
            }

            //string key = txtMessageKey.Text;

            //if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(key))
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Please specify both type and message number.", "Failure");
            }
            else
            {
                APIRequestResponse requestResponse = new APIRequestResponse();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(landSeaOrderURL);

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", authToken);
                    client.DefaultRequestHeaders.Add("BIPASK", authToken);
                    //HttpResponseMessage res = client.GetAsync("GetXMLDocs?key=" + key + "&type=" + type).Result;
                    HttpResponseMessage res = client.GetAsync("GetXMLDocs?type=" + type).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        requestResponse = res.Content.ReadAsAsync<APIRequestResponse>().Result;
                    }
                    else
                    {
                        string resultString = res.Content.ReadAsStringAsync().Result;
                    }
                }

                if (requestResponse.Data == null)
                {
                    rtxXMLMessageResult.Text = requestResponse.Message + Environment.NewLine + requestResponse.MessageDetail;
                }
                else
                {
                    rtxXMLMessageResult.Text = requestResponse.Data.ToString();
                }
            }
        }

        private void btnMarkAsProcessed_Click(object sender, EventArgs e)
        {
            string processedMessageID = txtProcessedMessageID.Text;

            if (string.IsNullOrEmpty(processedMessageID))
            {
                MessageBox.Show("Please specify message ID.", "Failure");
            }
            else
            {
                try
                {
                    APIRequestResponse requestResponse = new APIRequestResponse();

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(landSeaOrderURL);

                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", authToken);
                        client.DefaultRequestHeaders.Add("BIPASK", authToken);
                        HttpResponseMessage res = client.GetAsync("SAPProcessed?messageID=" + processedMessageID).Result;

                        if (res.IsSuccessStatusCode)
                        {
                            requestResponse = res.Content.ReadAsAsync<APIRequestResponse>().Result;
                        }
                        else
                        {
                            string resultString = res.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
