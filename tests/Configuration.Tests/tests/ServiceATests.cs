using AutoMapper;
using Configuration.Core.Mapper;
using Configuration.Core.Models;
using ConfigurationCase.ConfigurationSource.Abstracts;
using ConfigurationCase.ConfigurationSource.Services;
using ConfigurationCase.Core;
using ConfigurationCase.Core.Caching;
using ConfigurationCase.DAL;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using ServiceA.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ConfigurationCase.Core.Models;
using Configuration.Core.Consts;

namespace Configuration.Tests.tests
{
    public class ServiceATests : InitialDbContextOptions
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IMapper mapper;
        private readonly IRedisCacheService _redisCacheManager;

        private readonly IOptions<AppSettings> _config;
        private readonly IOptions<RedisServerConfig> _redisConfig;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfigurationService _configurationService;
        private ConfigurationController controller;
        private readonly string AppName = "SERVICE-A-TEST";

        public ServiceATests()
        {
            SetContextOptions(new DbContextOptionsBuilder<ConfigurationDbContext>().UseInMemoryDatabase("UnitTestInMemoryDB")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

            _dbContext = new ConfigurationDbContext(_contextOptions);

            var myProfile = new MapProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);


            _config = Options.Create<AppSettings>(new AppSettings() { Name = AppName });
            _redisConfig = Options.Create<RedisServerConfig>(new RedisServerConfig() { PrivateKey = "test-private-key", RedisEndPoint = "localhost", RedisPort = 6379, RedisSsl = false, RedisTimeout = 60, RedisPassword = "" });
            _redisCacheManager = new RedisCacheService(_redisConfig);

            _configurationService = new ConfigurationService(_redisCacheManager, mapper, _dbContext);

            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection", "UnitTestInMemoryDB"},
                //...populate as needed for the test
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            controller = new ConfigurationController(_config, _configuration, _publishEndpoint, _configurationService);
        }


        [Fact]
        public async Task Ensure_Values_Should_Be_Listed()
        {
            // Act

            IEnumerable<ConfigurationDto> configurationDtoList;
            configurationDtoList = new List<ConfigurationDto>()
            {
                new ConfigurationDto() { ApplicationName = AppName, ID = 1, IsActive = true, Name = "SiteName1", Type = Core.Enums.ConfigurationTypeEnum.String, Value = "Test" },
                new ConfigurationDto() { ApplicationName = AppName, ID = 2, IsActive = true, Name = "SiteName2", Type = Core.Enums.ConfigurationTypeEnum.Int, Value = "1" },
            };
            _redisCacheManager.Set(StaticVariables.CacheKey + $"_{AppName}", configurationDtoList);

            // Assert
            var result = await controller.GetValues();

            // Arrange
            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnConfigList = Assert.IsAssignableFrom<IEnumerable<ConfigurationDto>>(okResult.Value);

            Assert.NotNull(returnConfigList);
            Assert.Equal(2, returnConfigList.Count());

        }


        [Fact]
        public async Task Get_Value_By_Key_Properly()
        {
            // Act
            var key = "SiteName1";

            // Assert
            var result = await controller.GetValueByKey(key);

            // Arrange
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.IsAssignableFrom<string>(okResult.Value);

            Assert.Equal("boyner.com", okResult.Value);

        }


        [Fact]
        public async Task Ensure_Configuration_Should_Be_Created_Successfuly()
        {
            //Arrange
            var configuration = new ConfigurationCase.Core.Entities.Configuration
            {
                Name = "Test_Name",
                IsActive = true,
                Type = Core.Enums.ConfigurationTypeEnum.String,
                Value = "Test_Value",
                ApplicationName = "SERVICE-A",
                
            };

            // Act
            await _dbContext.Configuration.AddAsync(configuration);
            await _dbContext.SaveChangesAsync();

            var hasConfiguration = _dbContext.Configuration.Any(p => p.Name == configuration.Name);

            //Assert
            Assert.Equal(4, _dbContext.Configuration.ToList().Count());
            Assert.Equal(4, configuration.ID);

            Assert.True(hasConfiguration);
        }

        [Fact]
        public async Task Ensure_AddConfigurationMethod_ShouldBe_Ok()
        {
            //Arrange
            var configuration = new AddConfigurationDto()
            {
                Name = "Test_Name",
                IsActive = true,
                Type = Core.Enums.ConfigurationTypeEnum.String,
                Value = "Test_Value",

            };

            //Act
            var result = await controller.AddConfiguration(configuration);
            
            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            Assert.Equal(4, _dbContext.Configuration.ToList().Count());
        }


        [Fact]
        public async Task Update_Operation_With_Invalid_ID_Should_Throw_Exception()
        {
            //Arrange
            var configuration = new UpdateConfigurationDto()
            {
                ID = 5,
                Name = "Test_Name",
                IsActive = true,
                Type = Core.Enums.ConfigurationTypeEnum.String,
                Value = "Test_Value",

            };

            //Act
            Func<Task> act = () => controller.UpdateConfiguration(configuration);
            //Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(act);
            Assert.Contains($"ID Bulunamadı", ex.Message);
        }


        [Fact]
        public async Task Valid_ID_Should_Be_Updated()
        {
            //Arrange
            var configuration = new UpdateConfigurationDto()
            {
                ID = 3,
                Name = "Updated_Name",
                IsActive = true,
                Type = Core.Enums.ConfigurationTypeEnum.Int,
                Value = "Updated_Value",

            };

            //Act
            var result = await controller.UpdateConfiguration(configuration);
            var updatedConfiguration = _dbContext.Configuration.FirstOrDefault(x => x.ID == 3);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            Assert.Equal(updatedConfiguration.Value, configuration.Value);
            Assert.Equal(updatedConfiguration.Name, configuration.Name);
            Assert.Equal(updatedConfiguration.Type, configuration.Type);
        }


        [Fact]
        public async Task Delete_Operation_With_Invalid_ID_Should_Throw_Exception()
        {
            //Arrange
            int id = 5;

            //Act
            Func<Task> act = () => controller.DeleteConfiguration(id);
            
            //Assert
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(act);
            Assert.Contains($"ID Bulunamadı", ex.Message);
        }


        [Fact]
        public async Task Delete_Operation_With_Valid_ID_Should_Be_Ok()
        {
            //Arrange
            int id = 2;

            //Act
            var result = await controller.DeleteConfiguration(id);
            var hasConfiguration = _dbContext.Configuration.Any(p => p.ID == id);


            //Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            Assert.Equal(2, _dbContext.Configuration.ToList().Count());
            Assert.False(hasConfiguration);
        }

    }
}
