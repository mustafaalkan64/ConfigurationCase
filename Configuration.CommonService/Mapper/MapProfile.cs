using AutoMapper;
using Configuration.Core.Models;
using ConfigurationCase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.CommonService.Mapper
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<ConfigurationTb, ConfigurationDto>().ReverseMap();
            CreateMap<ConfigurationTb, UpdateConfigurationDto>().ReverseMap();
        }
    }
}
