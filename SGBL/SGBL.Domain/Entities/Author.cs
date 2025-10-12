
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("autores")]
    public class Author: BaseEntity<int>
    {
        [Column("idautor")]
        [Key]
        public override int Id { get; set; }
        [Column("nombre")]
        public string Name { get; set; }
        [Column("biografia")]
        public string Biography { get; set; }
        [Column("fecha_nacimiento")]
        public DateTime BirthDate { get; set; }
        [Column("nacionalidad")]
        public int Nationality { get; set; }
    }
}
