using BSW.APIResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TestAPI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnGetXML_Click(object sender, EventArgs e)
        {
            try
            {
                string token = string.Empty;
                using (BSW.APIToken.Token t = new BSW.APIToken.Token())
                {
                    var tRes = t.GenerateToken("d3w4ld123", "XMLAPI", 20, "DEWALD", "");
                    if (tRes.Successful)
                    {
                        token = tRes.UserToken;
                    }
                }

                System.Net.ServicePointManager.ServerCertificateValidationCallback += (senderClient, certificate, chain, sslPolicyErrors) => true;
                using (HttpClient client = new HttpClient(new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic }))
                {
                    try
                    {
                        client.BaseAddress = new Uri("http://localhost:51203/");
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("FlowAsk", token);

                        var res = client.GetAsync("api/XMLDocuments/GetXMLDocs").Result;

                        if (res.IsSuccessStatusCode)
                        {
                            //we convert return the results as is - that we get from calling TLCgo
                            var result = res.Content.ReadAsAsync<RequestReponse>().Result;
                            if (result.Success)
                            {
                                MessageBox.Show(result.Data.ToString(), "Success");
                            }
                            else
                            {
                                MessageBox.Show(result.Message, result.MessageDetail);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Message Failed");
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("Message Failed", exp.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Message Failed", ex.Message);
            }
        }
    }
}


