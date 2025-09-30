using AutoMapper;
using LibraryManagement.DTOs.Author;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.Member;
using LibraryMangment.Model;

namespace LibraryManagement.AutoMappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Mapping Author
            CreateMap<Author, AuthorDto>().ReverseMap()
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));
            CreateMap<Author, AuthorCreateDto>().ReverseMap();

            //Mapping Book
            CreateMap<Book, BookDto>().ReverseMap()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.AuthorName));
            CreateMap<Book, CreateBookDto>().ReverseMap();
            CreateMap<Book, UpdateBookDto>().ReverseMap();

            //Mapping BorrowHistory();

            CreateMap<BorrowsHistory, BorrowsHistoryDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name));
            CreateMap<BorrowsHistoryCreateDto, BorrowsHistory>();

            //Mapping Member
            CreateMap<Member, MemberDto>();
            CreateMap<MemberCreateDto, Member>();
            
        }
    }
}
