using smartPiggy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Base
{
	public partial class User
	{
		public int UserId { get; set; }

		public string? Login { get; set; }

		public string? NumberPhone { get; set; }

		public string? Email { get; set; }

		public string? Password { get; set; }

		public virtual ICollection<ExpendiTable> ExpendiTables { get; set; } = new List<ExpendiTable>();

		public virtual ICollection<IncomeTable> IncomeTables { get; set; } = new List<IncomeTable>();

		public virtual ICollection<PiggyTable> PiggyTables { get; set; } = new List<PiggyTable>();

		public virtual ICollection<TaskTable> TaskTables { get; set; } = new List<TaskTable>();
	}
}
