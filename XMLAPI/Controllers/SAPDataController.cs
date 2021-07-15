using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Cors;
using System.Web.Http;
using System.Xml;

using BSW.APIResponse;

namespace XMLAPI.Controllers
{
    public class SAPDataController : ApiController
    {
        // Central point - data will split from here to either OrderController (current BIP API LandSeaController) or AccountingController (existing logic)
        [System.Web.Mvc.HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public RequestReponse ProcessXML([FromBody] string values)
        {
            RequestReponse result = new RequestReponse();

            try
            {
                using (DataSet dsRequest = new DataSet())
                {
                    using (var strReader = new StringReader(values))
                    {
                        dsRequest.ReadXml(strReader);

                        if (dsRequest.Tables.Contains("Shipment") || dsRequest.Tables.Contains("CurrencyExchangeRate"))
                        {
                            OrderController orderController = new OrderController();
                            result = orderController.Post(values);
                        }
                        else
                        {
                            AccountingController accountingController = new AccountingController();
                            result = accountingController.UpdateInvoicePaymentDetails(values);
                        }
                    }
                }
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