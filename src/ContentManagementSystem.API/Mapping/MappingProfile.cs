using AutoMapper;
using ContentManagementSystem.API.DTOs;
using ContentManagementSystem.Core.Commands;
using ContentManagementSystem.Core.DTOs;
using ContentManagementSystem.Core.Entities;
using ContentManagementSystem.Core.Enums;

namespace ContentManagementSystem.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Program mappings
        CreateMap<Program, ProgramDto>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.ToString()))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.ProgramCategories.Select(pc => pc.Category)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProgramTags.Select(pt => pt.Tag)))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Where(c => c.IsApproved)));

        CreateMap<CreateProgramRequest, CreateProgramCommand>();
        CreateMap<UpdateProgramRequest, UpdateProgramCommand>();
        CreateMap<SearchProgramsRequest, SearchCriteria>();

        // Category mappings
        CreateMap<Category, CategoryDto>();

        // Tag mappings
        CreateMap<Tag, TagDto>();

        // Comment mappings
        CreateMap<Comment, CommentDto>();

        // Search result mappings
        CreateMap<SearchResult<Program>, SearchResult<ProgramDto>>();
    }
}