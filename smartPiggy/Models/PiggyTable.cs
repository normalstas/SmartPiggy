using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Models
{
	public partial class PiggyTable
	{
		public int IdPiggy { get; set; }

		public string NamePiggy { get; set; } = null!;

		public string PurposePiggy { get; set; } = null!;

		public string? TaskPiggy { get; set; }

		public decimal? CoutTaskPiggy { get; set; }

		public string? TimeTaskString { get; set; }

		public decimal CoutStartPiggy { get; set; }

		public decimal SumPiggy { get; set; }

		public string? StartDataPiggy { get; set; }

		public decimal? AmountPiggy { get; set; }
	}


}
