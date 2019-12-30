using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationManager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ConfigurationManager.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IConfiguration configuration;

        public HomeController( IOptionsSnapshot<AppSettings> settings, IConfiguration configuration)
        {
            _settings = settings;
            this.configuration = configuration;
        }

        public IActionResult Configuration()
        {
            var setting = _settings.Value;
            if (configuration["BASE_URL"] != null)
            {
                setting.BaseUrl = configuration["BASE_URL"];
            }
            return Json(setting);
        }
    }
}
