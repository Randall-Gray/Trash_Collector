using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrashCollector.Models
{
    public class WeekDay
    {
        [Key]
        public int WeekDayId { get; set; }

        public string Day { get; set; }
    }
}
