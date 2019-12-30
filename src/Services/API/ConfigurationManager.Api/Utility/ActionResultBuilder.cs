using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Utility
{
    public interface IActionResultBuilder
    {

        IActionResult Build(ModelStateDictionary dictionary);
        IActionResult Build(Exception exception);
        IActionResult Build(Exception exception, int statusCode);
        IActionResult Build(string error, string description, int statusCode);
    }
    public class ResultBuilder : IActionResultBuilder
    {
        public IActionResult Build(object context)
        {
            throw new NotImplementedException();
        }

        public IActionResult Build(ModelStateDictionary modelState)
        {
            var errors = modelState.Keys.SelectMany(i => modelState[i].Errors).Select(m => m.ErrorMessage).ToArray();

            //var states = modelState.ToDictionary(
            //                kvp => kvp.Key,
            //                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            //            );

            var obj = new { Errors = errors };
            return GetResult(obj, StatusCodes.Status400BadRequest);
        }
        public IActionResult Build(Exception exception)
        {
            return Build(exception, StatusCodes.Status400BadRequest);
        }

        public IActionResult Build(Exception exception, int statusCode)
        {
            var obj = new { IsSuccess = false, Error = exception.Message, Description = exception.StackTrace};
            return GetResult(obj, statusCode);
        }

        public IActionResult Build(string error, string description, int statusCode)
        {
            var obj = new { Error = error, Description = description };
            return GetResult(obj, statusCode);

        }

        public JsonResult GetResult(object obj, int statusCode = StatusCodes.Status400BadRequest)
        {
            return new JsonResult(obj)
            {
                StatusCode = statusCode
            };
        }




    }

}
