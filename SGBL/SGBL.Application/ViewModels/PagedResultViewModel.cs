using System;
using System.Collections.Generic;

namespace SGBL.Application.ViewModels
{
    public class PagedResultViewModel<T>
    {
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Número de página actual (1, 2, 3...)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Cantidad de registros por página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Cantidad total de registros (sin paginar)
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Cantidad total de páginas (calculado)
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (PageSize <= 0) return 0;
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
