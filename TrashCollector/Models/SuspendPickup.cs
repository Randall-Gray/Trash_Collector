using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TrashCollector.Models
{
    public class SuspendPickup
    {
        [Key]
        public int SuspendPickupId { get; set; }

        public bool Completed { get; set; }

        [Display(Name = "Suspend Pickup Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Suspend Pickup End Date")]
        public DateTime EndDate { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
