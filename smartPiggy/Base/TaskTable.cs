using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Models
{
	public partial class TaskTable
	{
		public int TaskId { get; set; }

		public string TitleTask { get; set; } = null!;

		public string CategoryTask { get; set; } = null!;

		public bool CheckedTask { get; set; }

		public string StartDateTask { get; set; } = null!;

		public string EndDateTask { get; set; } = null!;
	}
}
