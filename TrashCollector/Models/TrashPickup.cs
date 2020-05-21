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
        public int TrashPickupId { get; set; }

        [Display(Name="Pickup Type")]
        public int PickupType { get; set; }

        [Display(Name = "Week Day Trash Pickup")]
        public string WeekDay { get; set; }

        [Display(Name = "Extra Trash Pickup Date")]
        public DateTime ExtraPickupDate { get; set; }

        [Display(Name="Suspend Pickup Start Date")]
        public DateTime StartSuspendDate { get; set; }

        [Display(Name = "Suspend Pickup End Date")]
        public DateTime EndSuspendDate { get; set; }

        public bool Completed { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
