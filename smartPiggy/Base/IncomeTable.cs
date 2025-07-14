using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Models
{
	public partial class IncomeTable
	{
		public int IncomeId { get; set; }

		public string NameIncome { get; set; } = null!;

		public decimal AmountIncome { get; set; }

		public string? CategoryIncome { get; set; }

		public int DayIncome { get; set; }

		public int MounthIncome { get; set; }

		public int YearsIncome { get; set; }

		public TimeOnly TimeCreateincome { get; set; }
	}
}
