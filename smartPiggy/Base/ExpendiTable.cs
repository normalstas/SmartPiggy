using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Models
{
	public partial class ExpendiTable
	{
		public int ExpId { get; set; }

		public string NameExp { get; set; } = null!;

		public decimal AmountExp { get; set; }

		public string? CategoryExp { get; set; }

		public int DayExp { get; set; }

		public int MonthExp { get; set; }

		public int YearsExp { get; set; }

		public TimeOnly TimeCreateExp { get; set; }
	}
}
