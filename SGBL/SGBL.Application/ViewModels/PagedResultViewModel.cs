using SGBL.Application.Dtos.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBL.Application.ViewModels
{
   public class PagedResultViewModel
    {
      
            public List<BookViewModel> Items { get; set; } = new();
            public int TotalItems { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
        
    }
}
