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
using System.Net.Http.Formatting;

using Demo.Library;
using Demo.Objects;

namespace Demo
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private string authToken, bipAuthURL, landSeaOrderURL, bipExAPIURL;

        private void frmMain_Load(object sender, EventArgs e)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();

            bipAuthURL = appSettingsReader.GetValue("BIPAuthURL", typeof(string)).ToString();
            landSeaOrderURL = appSettingsReader.GetValue("LandSeaOrderURL", typeof(string)).ToString();
            bipExAPIURL = appSettingsReader.GetValue("BIPExAPIURL", typeof(string)).ToString();
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
            ofdSelectXML.Filter = "XML files (*.xml) | xml";

            if (ofdSelectXML.ShowDialog() == DialogResult.OK)
            {
                txtSendXMLPath.Text = ofdSelectXML.FileName;
            }

            if (!string.IsNullOrEmpty(txtSendXMLPath.Text.Trim()))
            {
                btnSendXML.Enabled = true;
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
                    string BIPAPIService = landSeaOrderURL;
                    client.BaseAddress = new Uri(BIPAPIService);

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
            string type;

            if (selectedType == "Organization")
            {
                type = "Organization";
            }
            else
            {
                type = "ForwardingShipment";
            }

            string key = txtMessageKey.Text;

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(key))
            {
            }
            else
            {
                string apibody = "{ \"key\": \"" + type + "\", \"type\": \"" + key + "\"}";

                APIRequestResponse requestResponse = new APIRequestResponse();

                using (HttpClient client = new HttpClient())
                {
                    string BIPAPIService = bipExAPIURL;

                    client.BaseAddress = new Uri(BIPAPIService);

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BIPASK", authToken);
                    client.DefaultRequestHeaders.Add("BIPASK", authToken);
                    var res = client.PostAsJsonAsync("GetXMLDocs", apibody).Result;

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
        }
    }
}
