using Framework.Models;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Framework.Controllers
{
    public class BaseController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [System.Web.Http.HttpPost]
        public IHttpActionResult Entrance()
        {
            //建立兩種回傳資料型態
            var responseJson = new JObject();
            var responseFromData = new HttpResponseMessage(HttpStatusCode.OK);

            string contentType = HttpContext.Current.Request.ContentType.Split(';')[0];
            string functionId = "";
            HttpPostedFile inputFile;
            dynamic input;

            //依照不同ContentType讀取資料
            switch (contentType)
            {
                case "multipart/form-data":
                    string jsonUrlDecode = HttpContext.Current.Request.Form.Get("JSON");
                    string jsonStr = Uri.UnescapeDataString(jsonUrlDecode);
                    input = JObject.Parse(jsonStr);
                    //預設只抓第一個檔案
                    inputFile = HttpContext.Current.Request.Files[0];
                    functionId = (string)input.function;
                    break;
                case "application/json":
                    var stream = HttpContext.Current.Request.InputStream;
                    var readStream = new StreamReader(stream, Encoding.UTF8);
                    string json = readStream.ReadToEnd();
                    input = JObject.Parse(json) as dynamic;
                    inputFile = null;
                    functionId = input.function;
                    break;
                default: //text
                    input = HttpContext.Current.Request;
                    break;
            }

            string return_msg = string.Empty;
            IModel model;

            try
            {
                //依照Function代號進行對應操作
                switch (functionId)
                {
                    case "TEST":
                        model = new TEST();
                        break;
                    default:
                        Global.Test();
                        break;
                }

                responseJson["return_code"] = "S";
                responseJson["return_msg"] = return_msg;

                responseFromData.Headers.Add("return_code", "S");
                responseFromData.Headers.Add("return_msg", return_msg);
            }
            catch (Exception ex)
            {
                responseJson["return_code"] = "S";
                responseJson["return_msg"] = ex.Message;

                responseFromData.Headers.Add("return_code", "S");
                responseFromData.Headers.Add("return_msg", ex.Message);

                log.Error(ex.Message);
            }
            finally
            {
                //儲存API Log
                string HostUrl = $"{Request.RequestUri}{Request.Method}";
                Global.SaveApiLog(HostUrl, functionId, input.ToString(), responseJson.ToString());
            }

            switch (HttpContext.Current.Response.ContentType)
            {
                case "multipart/form-data":
                    return ResponseMessage(responseFromData);
                case "application/json":
                    return Json<dynamic>(responseJson);
                default: // text/html
                    var response = new HttpResponseMessage()
                    {
                        Content = new StringContent("Call API "),
                        RequestMessage = new HttpRequestMessage()
                    };
                    return ResponseMessage(response);
            }
        }
    }
}
