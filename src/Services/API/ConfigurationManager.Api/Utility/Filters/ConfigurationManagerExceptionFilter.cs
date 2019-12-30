using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Utility.Filters
{
    public class ConfigurationManagerExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IActionResultBuilder jsonResultService;
        public ConfigurationManagerExceptionFilter(
            IActionResultBuilder jsonResultService
        )
        {
            this.jsonResultService = jsonResultService ?? throw new ArgumentNullException(nameof(jsonResultService));
        }
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            context.Result = jsonResultService.Build(context.Exception);
        }
    }
}
