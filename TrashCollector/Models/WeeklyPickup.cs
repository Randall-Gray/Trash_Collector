using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TrashCollector.Models
{
    public class WeeklyPickup
    {
        [Key]
        public int WeeklyPickupId { get; set; }

        public bool Completed { get; set; }

        [ForeignKey("WeekDay")]
        [Display(Name = "Weekly Pickup Day")]
        public int WeekDayId { get; set; }
        public WeekDay WeekDay{ get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
