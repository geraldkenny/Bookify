using AutoMapper;
using Bookify.DTO;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookify.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            // ReverseMap maps the destination to source type
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, AddBookDTO>().ReverseMap();
        }
    
    }
}
