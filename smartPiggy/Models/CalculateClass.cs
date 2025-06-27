using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Models
{
	public static class CalculateClass
	{
		public enum TimeMode { Day, Week, Month, Year }

		public static string Calculate(string startDateStr, TimeMode mode)
		{
			if (!DateTime.TryParse(startDateStr, out DateTime startDate))
				return "Ошибка формата даты!";

			DateTime deadlineDate = CalculateDeadline(startDate, mode);
			DateTime currentDate = DateTime.Today;
			int totalDays = (deadlineDate - currentDate).Days;

			if (totalDays > 0)
			{
				return FormatRemainingTime(totalDays);
			}
			else if (totalDays < 0)
			{
				return FormatOverdueTime(currentDate - deadlineDate);
			}
			return "Дедлайн сегодня!";
		}

		private static DateTime CalculateDeadline(DateTime startDate, TimeMode mode)
		{
			return mode switch
			{
				TimeMode.Day => startDate.AddDays(1),
				TimeMode.Week => startDate.AddDays(7),
				TimeMode.Month => startDate.AddMonths(1),
				TimeMode.Year => startDate.AddYears(1),
				_ => startDate
			};
		}

		private static string FormatRemainingTime(int totalDays)
		{
			if (totalDays < 7)
				return $"Осталось: {totalDays} {DayWord(totalDays)}";

			int weeks = totalDays / 7;
			int days = totalDays % 7;
			return $"Осталось: {weeks} {WeekWord(weeks)}{(days > 0 ? $" и {days} {DayWord(days)}" : "")}";
		}

		private static string FormatOverdueTime(TimeSpan overdue)
		{
			int totalDays = overdue.Days;
			return $"Просрочено: {totalDays} {DayWord(totalDays)}";
		}

		// Методы для склонения слов
		private static string DayWord(int n) => (n % 10 == 1 && n % 100 != 11) ? "день" :
											   (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? "дня" : "дней";

		private static string WeekWord(int n) => (n % 10 == 1 && n % 100 != 11) ? "неделя" :
												(n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? "недели" : "недель";

		private static string MonthWord(int n) => (n % 10 == 1 && n % 100 != 11) ? "месяц" :
												 (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? "месяца" : "месяцев";

		private static string YearWord(int n) => (n % 10 == 1 && n % 100 != 11) ? "год" :
												(n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? "года" : "лет";

	}
}