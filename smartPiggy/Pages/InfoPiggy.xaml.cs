

using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using smartPiggy.Models;
using System.Net.NetworkInformation;
using smartPiggy.Servies;
using System.Net;
using Newtonsoft.Json;

namespace smartPiggy.Pages
{
	public partial class InfoPiggy : ContentPage
	{
		private readonly DonutChartDrawable _donutDrawable;

		public InfoPiggy(PiggyTable piggy)
		{
			InitializeComponent();
			BindingContext = piggy;
			_donutDrawable = new DonutChartDrawable
			{
				StrokeWidth = 15f,
				BackgroundColor = Color.FromArgb("#424242"), // Темный фон
				RemainingColor = Color.FromArgb("#757575"), // Серый (осталось)
				CollectedColor = Color.FromArgb("#4CAF50")  // Зеленый (накоплено)
			};

			donutChartView.Drawable = _donutDrawable;
			donutChartView.HeightRequest = 125;
			donutChartView.WidthRequest = 125;

			UpdateChart(piggy);

		}

		private void UpdateChart(PiggyTable piggy)
		{
			if (piggy == null) return;

			_donutDrawable.Collected = piggy.CoutStartPiggy;
			_donutDrawable.Total = piggy.SumPiggy;

			UpdateLegend(piggy.CoutStartPiggy, piggy.SumPiggy);
			donutChartView.Invalidate();
		}

		private void UpdateLegend(decimal collected, decimal total)
		{
			decimal remaining = total - collected;
			decimal percentage = total > 0 ? collected / total * 100 : 0;

			legendContainer.Children.Clear();
			stdetail.Clear();

			AddLegendItem("Накоплено", $"{percentage:F0}%", _donutDrawable.CollectedColor);
			AddLegendItem("Осталось", $"{remaining:C}", _donutDrawable.RemainingColor);
			UpdateDetail("Накоплено", $"{collected:C}");
			UpdateDetail("Цель", $"{total:C}");
		}

		private void UpdateDetail(string title, string value)
		{
			var label = new Label { Text = title + " : " + value, TextColor = Colors.White, FontSize = 16 };
			stdetail.Children.Add(label);
		}

		private void AddLegendItem(string title, string value, Color color)
		{
			var item = new Grid
			{
				ColumnDefinitions =
			{
				new ColumnDefinition(24),
				new ColumnDefinition(GridLength.Star)
			},
				ColumnSpacing = 8
			};
			item.Children.Clear();
			item.Add(new BoxView
			{
				Color = color,
				CornerRadius = 4,
				HeightRequest = 16,
				WidthRequest = 24,
				VerticalOptions = LayoutOptions.Center
			}, 0, 0);

			item.Add(new Label
			{
				Text = $"{title} - {value}",
				FontSize = 14,
				TextColor = Colors.Black
			}, 1, 0);

			legendContainer.Children.Add(item);
		}

		private async void AddMoneyClick(object sender, EventArgs e)
		{
			if (!int.TryParse(tbnewsum.Text, out int amount) || amount <= 0)
			{
				await DisplayAlert("Ошибка", "Пожалуйста, введите корректную сумму", "OK");
				return;
			}

			var piggy = BindingContext as PiggyTable;
			if (piggy == null) return;

			try
			{
				var currentPiggy = PiggyServies.Get<PiggyTable>(piggy.IdPiggy.ToString());

				// 2. Обновляем нужные поля
				currentPiggy.CoutStartPiggy = currentPiggy.CoutStartPiggy + amount;

				currentPiggy.SumPiggy = piggy.SumPiggy;

				if (currentPiggy.CoutTaskPiggy != null)
				{
					currentPiggy.AmountPiggy += amount;
					if (currentPiggy.AmountPiggy == currentPiggy.CoutTaskPiggy)
					{
						var currentDate = DateTime.Today;
						currentPiggy.AmountPiggy = 0;
						currentPiggy.StartDataPiggy = currentDate.Date.ToString("yyyy-MM-dd");
					}
				}
				// 3. Отправляем обновленные данные
				string response = PiggyServies.Put(currentPiggy, currentPiggy.IdPiggy.ToString());

				// Обновляем UI
				UpdateChart(currentPiggy);
				tbnewsum.Text = string.Empty;
				await DisplayAlert("Успешно", "Сумма добавлена", "OK");
			}
			catch (WebException webEx)
			{
				await DisplayAlert("Ошибка сети", $"{webEx}", "OK");
			}
			catch (Exception ex)
			{

				await DisplayAlert("Ошибка", ex.Message, "OK");
			}
		}

		private void tbnewsum_TextChanged(object sender, TextChangedEventArgs e)
		{
			var piggy = BindingContext as PiggyTable;
			var currentPiggy = PiggyServies.Get<PiggyTable>(piggy.IdPiggy.ToString());
			if (currentPiggy != null && !string.IsNullOrEmpty(tbnewsum.Text))
			{
				if (decimal.TryParse(tbnewsum.Text, out decimal a))
				{
					if (currentPiggy.CoutStartPiggy + a > currentPiggy.SumPiggy)
					{
						var MaxSum = currentPiggy.SumPiggy - currentPiggy.CoutStartPiggy;
						tbnewsum.Text = MaxSum.ToString();
					}
				}
				else
				{
					DisplayAlert("Ошибка", "Вводите правильные значения", "ок");
				}


			}
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}

