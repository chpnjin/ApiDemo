using Core.Models;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ILoggerFactory = log4net.Repository.Hierarchy.ILoggerFactory;

namespace Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _log;

        ////依賴注入log4net
        public BaseController(ILogger<BaseController> log)
        {
            _log = log;
            _log.LogDebug("Test");
        }

        [HttpPost("CallFunc")]
        public JObject CallFunc(JObject obj)
        {
            JObject result = new JObject();
            dynamic request = obj as dynamic;
            IModel model;

            try
            {
                //儲存API Log
                string HostUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
                Global.SaveApiLog(HostUrl, request.function.ToString(), obj.ToString());

                //依照Function代號進行對應操作
                switch (request.function.ToString())
                {
                    default:
                        result.Add("return_msg", "Can not found function id.");
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                result.Add("return_msg", ex.Message);
                _log.LogError(ex.Message);

                return result;
            }
        }
    }
}
