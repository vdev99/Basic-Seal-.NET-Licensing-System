using AutoMapper;
using BasicSealBackend.Dtos;
using BasicSealBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<SoftwareLicenses, LicenseDto>();
            CreateMap<Softwares, ApplicationsRespDto>();
            CreateMap<SoftwareVersions, SoftVersion>();
            CreateMap<SoftwareVersions, SoftwareVersionsRespDto>();/*
                        .ForPath(dest => dest.version.res, opt => opt.MapFrom(src => src.Id))
                        .ForPath(dest => dest.version, opt => opt.MapFrom(src => src.Hash))
                        .ForPath(dest => dest.version, opt => opt.MapFrom(src => src.Version));*/
            CreateMap<SoftwareLicenses, SoftwareLicenseDto>();
            CreateMap<SoftwareLicenseDto, SoftwareLicenses>();
        }
    }
}
