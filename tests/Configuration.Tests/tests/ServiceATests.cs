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

namespace Configuration.Tests.tests
{
    public class ServiceATests : InitialDbContextOptions
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IMapper mapper;
        private readonly IRedisCacheService _redisCacheManager;

        private readonly IOptions<AppSettings> _config;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfigurationService _configurationService;
        private ConfigurationController controller;

        public ServiceATests()
        {
            SetContextOptions(new DbContextOptionsBuilder<ConfigurationDbContext>().UseInMemoryDatabase("UnitTestInMemoryDB")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

            _dbContext = new ConfigurationDbContext(_contextOptions);

            var myProfile = new MapProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);

            _configurationService = new ConfigurationService(_redisCacheManager, mapper, _dbContext);

            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection", "UnitTestInMemoryDB"},
                //...populate as needed for the test
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _config = Options.Create<AppSettings>(new AppSettings() {  Name = "SERVICE-A"});

            controller = new ConfigurationController(_config, _configuration, _publishEndpoint, _configurationService);
        }


        [Fact]
        public async Task Ensure_ConfigurationCreated_Successfuly()
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
