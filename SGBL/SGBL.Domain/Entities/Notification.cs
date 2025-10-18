

using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("notificaciones")]
    public class Notification: BaseEntity<int>
    {
        public override int Id { get; set; }
        public int IdUser { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public int IdBook {  get; set; }
        public int IdLoan {  get; set; }
        public DateTime ReadDate {  get; set; } 
    }
}
