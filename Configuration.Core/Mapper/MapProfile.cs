using AutoMapper;
using Configuration.Core.Models;
using ConfigurationCase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Core.Mapper
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<ConfigurationCase.Core.Entities.Configuration, ConfigurationDto>().ReverseMap();
            CreateMap<ConfigurationCase.Core.Entities.Configuration, UpdateConfigurationDto>().ReverseMap();
        }
    }
}
