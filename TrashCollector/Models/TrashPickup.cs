using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TrashCollector.Models
{
    public class TrashPickup
    {
        [Key]
        public int TrashPickupId { get; set; }

        public bool Completed { get; set; }

        [Display(Name = "Trash Pickup Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name = "Address Line 1")]
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }

        [NotMapped]
        public int DatePickupId { get; set; }

        [NotMapped]
        public int WeeklyPickupId { get; set; }
    }
}
