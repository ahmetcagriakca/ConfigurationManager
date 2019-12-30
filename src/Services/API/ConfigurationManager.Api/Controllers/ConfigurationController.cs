using ConfigurationManager.Api.Model;
using ConfigurationManager.Api.Utility;
using ConfigurationManager.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private IConfigurationReader configurationReader;
        private readonly IApplicationConfigurationRepository applicationConfigurationRepository;
        private readonly ConfigurationEnvironment configurationEnvironment;

        public ConfigurationController(
            IConfigurationReader configurationReader,
            IApplicationConfigurationRepository applicationConfigurationRepository,
            ConfigurationEnvironment configurationEnvironment)
        {
            this.configurationReader = configurationReader;
            this.applicationConfigurationRepository = applicationConfigurationRepository;
            this.configurationEnvironment = configurationEnvironment;
        }
        /// <summary>
        /// Get configuration from configuration manager
        /// </summary>
        /// <param name="name">Configuration name</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get(string name)
        {

            var result = String.IsNullOrEmpty(name) ? null : configurationReader.GetValue<string>(name);
            return Ok(new BaseServiceResponse()
            {
                ResultValue = result
            });
        }

        /// <summary>
        /// Get configuration from db
        /// </summary>
        /// <param name="name">Configuration Key</param>
        /// <returns></returns>
        [HttpGet("GetConfigurationValueFromDb")]
        public async Task<IActionResult> GetConfigurationValueFromDb(string name)
        {
            var result =
                await applicationConfigurationRepository.GetByNameAsync(name, configurationEnvironment.ApplicationName);
            return Ok(new BaseServiceResponse()
            {
                ResultValue = result.Value
            });
        }

        /// <summary>
        /// Update existing configuration value from db
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("UpdateApplicationConfiguration")]
        public async Task<IActionResult> UpdateApplicationConfiguration(string name, string value, string type)
        {
            var configuration = await
                applicationConfigurationRepository.GetByNameAsync(name, configurationEnvironment.ApplicationName);
            if (configuration == null)
            {
                throw new Exception("Configuration is not found");
            }
            configuration.Value = value;
            configuration.Type = type;
            var result =
                await applicationConfigurationRepository.UpdateAsync(configuration.Id, configuration);
            return Ok(new BaseServiceResponse()
            {
                ResultValue = result.Value
            });
        }
    }
}
