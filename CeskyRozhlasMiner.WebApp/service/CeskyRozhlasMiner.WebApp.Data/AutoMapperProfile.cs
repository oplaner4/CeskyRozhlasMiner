using AutoMapper;
using CeskyRozhlasMiner.Lib.Playlist;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Models;

namespace Microsoft.DSX.ProjectTemplate.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Group, GroupDto>();
            CreateMap<User, UserDto>()
                .ForMember(x => x.Password, opt => opt.Ignore());

            CreateMap<PlaylistSourceStation, PlaylistSourceStationDto>();
            CreateMap<Playlist, PlaylistDto>();
            CreateMap<Song, SongDto>();

            CreateMap<PlaylistSong, Song>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.UpdatedDate, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<PlaylistSong, SongDto>()
                .ForMember(x => x.CreatedDate, opt => opt.Ignore())
                .ForMember(x => x.UpdatedDate, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Token, TokenDto>();
        }
    }
}
