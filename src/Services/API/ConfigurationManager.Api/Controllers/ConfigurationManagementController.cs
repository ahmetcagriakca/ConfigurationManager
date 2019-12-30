using ConfigurationManager.Api.Model;
using ConfigurationManager.Api.Utility;
using ConfigurationManager.Core;
using ConfigurationManager.Core.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Controllers
{
    /// <summary>
    /// Configuration management services
    /// </summary>
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationManagementController : ControllerBase
    {
        private IConfigurationReader configurationReader;
        private readonly IApplicationConfigurationRepository applicationConfigurationRepository;
        private readonly ConfigurationEnvironment configurationEnvironment;

        public ConfigurationManagementController(
            IConfigurationReader configurationReader,
            IApplicationConfigurationRepository applicationConfigurationRepository,
            ConfigurationEnvironment configurationEnvironment,
            ILogger<ConfigurationManagementController> logger)
        {
            this.configurationReader = configurationReader;
            this.applicationConfigurationRepository = applicationConfigurationRepository;
            this.configurationEnvironment = configurationEnvironment;
        }

        /// <summary>
        /// Get configurations
        /// </summary>
        /// <param name="name">Configuration Key</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result =
                await applicationConfigurationRepository.GetAllAsync();
            return Ok(new BaseServiceResponse()
            {
                ResultValue = result
            });
        }

        /// <summary>
        /// Add new application configuration 
        /// </summary>
        /// <param name="key">Configuration Key</param>
        /// <returns></returns>
        [HttpPost("AddConfiguration")]
        public async Task<IActionResult> AddConfiguration(string name, string value, string type, string applicationName)
        {
            var applicationConfiguration = new ApplicationConfiguration()
            {
                Name = name,
                Value = value,
                Type = type,
                ApplicationName = applicationName
            };
            var result =
                await applicationConfigurationRepository.AddAsync(applicationConfiguration);
            return Ok(new BaseServiceResponse());
        }

        /// <summary>
        /// Update existing configuration value from db
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        [HttpPut("UpdateConfiguration")]
        public async Task<IActionResult> UpdateConfiguration(int id, string name, string value, string type, string applicationName, bool isActive)
        {
            var configuration = await
                applicationConfigurationRepository.GetByIdAsync(id);
            if (configuration == null)
            {
                throw new Exception("Configuration is not found");
            }
            configuration.Name = name;
            configuration.Type = type;
            configuration.Value = value;
            configuration.ApplicationName = applicationName;
            configuration.IsActive = isActive;
            var result =
                await applicationConfigurationRepository.UpdateAsync(configuration.Id, configuration);
            return Ok(new BaseServiceResponse());

        }

        /// <summary>
        /// Delete configuration with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteConfiguration")]
        public async Task<IActionResult> DeleteConfiguration(int id)
        {
            await applicationConfigurationRepository.DeleteAsync(id);
            return Ok(new BaseServiceResponse());
        }
    }
}
