using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrashCollector.Models
{
    public class TrashPickupViewModel
    {
        public WeeklyPickup VMWeeklyPickup { get; set; }
        public IEnumerable<DatePickup> VMDatePickups { get; set; }
        public IEnumerable<SuspendPickup> VMSuspendPickups { get; set; }
    }
}
