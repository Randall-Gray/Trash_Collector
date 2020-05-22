using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrashCollector.Models
{
    public class TrashPickupViewModel
    {
        [Key]
        public int Id { get; set; }

        public WeeklyPickup VMWeeklyPickup { get; set; }
        public IEnumerable<DatePickup> VMDatePickups { get; set; }
        public IEnumerable<SuspendPickup> VMSuspendPickups { get; set; }
    }
}
