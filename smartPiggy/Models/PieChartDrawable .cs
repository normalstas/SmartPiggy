using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smartPiggy.Models
{
	public class DonutChartDrawable : IDrawable
	{
		public decimal Collected { get; set; }
		public decimal Total { get; set; }
		public Color CollectedColor { get; set; } = Color.FromArgb("#4CAF50"); // Зеленый (накоплено)
		public Color RemainingColor { get; set; } = Color.FromArgb("#9E9E9E"); // Темно-серый (осталось)
		public Color BackgroundColor { get; set; } = Color.FromArgb("#424242"); // Темный фон
		public float StrokeWidth { get; set; } = 15f;

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			if (Total <= 0) return;

			float centerX = dirtyRect.Width / 2;
			float centerY = dirtyRect.Height / 2;
			float radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 2) - (StrokeWidth / 2);

			// 1. Рисуем темный фон
			canvas.FillColor = BackgroundColor;
			canvas.FillCircle(centerX, centerY, radius + StrokeWidth / 2);

			// 2. Рисуем серую линию (оставшаяся сумма)
			canvas.StrokeColor = RemainingColor;
			canvas.StrokeSize = StrokeWidth;
			canvas.DrawCircle(centerX, centerY, radius);

			// 3. Рисуем зеленую линию (накопленная сумма)
			if (Collected > 0)
			{
				float sweepAngle = (float)(360 * (double)(Collected / Total));
				canvas.StrokeColor = CollectedColor;
				canvas.StrokeSize = StrokeWidth;
				canvas.DrawArc(
					centerX - radius,
					centerY - radius,
					radius * 2,
					radius * 2,
					90,  // Начинаем сверху
					90 + sweepAngle, // По часовой стрелке
					false, false);
			}
		}
	}
}
