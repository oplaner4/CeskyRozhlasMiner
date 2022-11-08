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
            CreateMap<User, UserDto>();
            CreateMap<PlaylistSourceStation, PlaylistSourceStationDto>();
            CreateMap<Playlist, PlaylistDto>();
            CreateMap<Song, SongDto>();
            CreateMap<PlaylistSong, Song>();
            CreateMap<PlaylistSong, SongDto>();
            CreateMap<Token, TokenDto>();
        }
    }
}
