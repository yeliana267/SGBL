
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("libros")]
    public class Book : BaseEntity<int>
    {
        [Column("idlibro")]
        [Key]
        public override int Id { get; set; }
        [Column("titulo")]
        public string Title { get; set; }
        [Column("isbn")]
        public int Isbn { get; set; }
        [Column("descripcion")]
        public string Description { get; set; }
        [Column("año_publicacion")]
        public int PublicationYear { get; set; }
        [Column("paginas")]
        public int Pages { get; set; }
        [Column("copias_total")]
        public int  TotalCopies {  get; set; }
        [Column("copias_disponibles")]
        public int AvailableCopies { get; set; }
        [Column("ubicacion")]
        public string Ubication { get; set; }
        [Column("estado")]
        public int Status { get; set; }

    }
}
