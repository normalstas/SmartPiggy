using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using smartPiggy.Models;
using smartPiggy.Pages;
using smartPiggy.Servies;
using System.Linq;
using System.Net.NetworkInformation;
namespace smartPiggy
{
	public partial class Expenditure : ContentPage
	{
		public Expenditure()
		{
			InitializeComponent();
			VisibibleExpi();
		}

		private void CreateNewExp()
		{
			plus_expi.IsVisible = false;
			var expstacklayoutContainer = new VerticalStackLayout();
			var headerFrame = new Frame
			{
				BorderColor = Colors.Grey,
				BackgroundColor = Color.FromArgb("#F5F5F5"),
				CornerRadius = 10,
				Padding = new Thickness(15),
				Margin = new Thickness(0, 0, 0, 5)
			};

			var headerGrid = new Grid
			{
				ColumnDefinitions =
				{
			new ColumnDefinition { Width = GridLength.Star },
			new ColumnDefinition { Width = GridLength.Auto }
				}
			};

			var titleLabel = new Label
			{
				Text = "Создать новую запись расходов",
				FontSize = 16,
				FontAttributes = FontAttributes.Bold
			};

			var toggleButton = new Button
			{
				Text = "▽",
				FontSize = 20,
				TextColor = Colors.Blue,
				BackgroundColor = Colors.Transparent,
				Padding = 0,
				HorizontalOptions = LayoutOptions.End
			};

			headerGrid.Add(titleLabel, 0, 0);
			headerGrid.Add(toggleButton, 1, 0);
			headerFrame.Content = headerGrid;

			var creationForm = new VerticalStackLayout
			{
				Spacing = 10,
				IsVisible = true,
				Margin = new Thickness(0, 5, 0, 0)
			};

			var nameEntry = new Entry { Placeholder = "Название расходов" };
			var amountEntry = new Entry { Placeholder = "Сумма расходов", Keyboard = Keyboard.Numeric };
			var pickercategory = new Picker
			{
				Items = {"","Переводы", "Маркетплейсы", "Копилка", "Супермаркеты", "Кафе и рестораны", "Косметика",
			"ЖКХ, связь и интернет", "Медицина", "Товары для детей", "Транспорт","Бизнес-услуги",
			"Кино и театр", "Налоги", "Развлечения","Другое"},
			};
			var saveButton = new Button
			{
				Text = "Создать",
				BackgroundColor = Color.FromArgb("#4CAF50"),
				TextColor = Colors.White,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var DateNewExpiStackLa = new HorizontalStackLayout { Padding = 10 };
			var dayEntry = new Entry { Placeholder = "День", Keyboard = Keyboard.Numeric };
			var pickermounth = new Picker
			{
				Items = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
					"Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" },
			};
			var yearsEntry = new Entry { Placeholder = "Год", Keyboard = Keyboard.Numeric };
			DateNewExpiStackLa.Children.Add(dayEntry);
			DateNewExpiStackLa.Children.Add(pickermounth);
			DateNewExpiStackLa.Children.Add(yearsEntry);
			creationForm.Children.Add(new Label { Text = "Название" });
			creationForm.Children.Add(nameEntry);
			creationForm.Children.Add(new Label { Text = "Сумма" });
			creationForm.Children.Add(amountEntry);
			creationForm.Children.Add(new Label { Text = "Категория (необязательно)" });
			creationForm.Children.Add(pickercategory);
			creationForm.Children.Add(DateNewExpiStackLa);
			creationForm.Children.Add(saveButton);
			toggleButton.Clicked += (s, e) =>
			{
				creationForm.IsVisible = !creationForm.IsVisible;
				toggleButton.Text = creationForm.IsVisible ? "▽" : "△";
				headerFrame.CornerRadius = creationForm.IsVisible ? 10 : 20; // Меняем скругление
			};

			int currentyears = DateTime.Now.Year;
			saveButton.Clicked += async (s, e) =>
			{
				if (!decimal.TryParse(amountEntry.Text, out decimal amount)
				|| string.IsNullOrEmpty(nameEntry.Text) || pickermounth.SelectedItem == null
				|| !int.TryParse(dayEntry.Text, out int day) || 0 >= day || day >= 32
				|| !int.TryParse(yearsEntry.Text, out int years) || years > currentyears)
				{
					await DisplayAlert("Ошибка", "Проверьте все поля, кроме категории", "ОК");
					return;
				}
					if (pickercategory.SelectedItem == null)
					pickercategory.SelectedItem = "";
				try
				{

					var newExpi = new ExpendiTable
					{
						NameExp = nameEntry.Text,
						AmountExp = amount,
						CategoryExp = pickercategory.SelectedItem.ToString(),
						DayExp = int.Parse(dayEntry.Text),
						MonthExp = InvertMonth(pickermounth.SelectedItem.ToString()),
						YearsExp = int.Parse(yearsEntry.Text),
						TimeCreateExp = TimeOnly.FromDateTime(DateTime.Now)
					};

					await ExpendiServies.Post(newExpi);
					VisibibleExpi();
					nameEntry.Text = string.Empty;
					amountEntry.Text = string.Empty;
					dayEntry.Text = string.Empty;
					pickermounth.SelectedItem = null;
					yearsEntry.Text = string.Empty;


					plus_expi.IsVisible = true;
					addnewexp.Children.Remove(expstacklayoutContainer);
				}
				catch (Exception ex)
				{
					await DisplayAlert("Ошибка на стороне сервера", $"{ex}", "OK");
					throw;
				}
			};



			expstacklayoutContainer.Children.Add(headerFrame);
			expstacklayoutContainer.Children.Add(creationForm);
			addnewexp.Children.Add(expstacklayoutContainer);

		}

		private void plus_expi_Clicked(object sender, EventArgs e) => CreateNewExp();

		private void VisibibleExpi()
		{
			vismounth.Children.Clear(); // Основной контейнер для расходов
			// Получаем расходы из базы данных
			var expenses = ExpendiServies.Get<List<ExpendiTable>>();

			
			if (expenses == null || expenses.Count == 0)
			{
				vismounth.Children.Add(new Label
				{
					Text = "Нет расходов для отображения",
					HorizontalOptions = LayoutOptions.Center,
					TextColor = Colors.Gray
				});
				return;
			}

			// Группируем расходы по месяцам
			var groupedExpenses = expenses
				.GroupBy(e => new DateTime(e.YearsExp, e.MonthExp, 1))
				.OrderByDescending(g => g.Key);

			foreach (var monthGroup in groupedExpenses)
			{
				foreach (var expi in expenses)
				{
					var ExpContainer = new VerticalStackLayout { };
					var infostack = new VerticalStackLayout { IsVisible = false };
					var monthName = monthGroup.Key.ToString("MMMM yyyy");
					var mainFrame = new Frame
					{
						BorderColor = Colors.Grey,
						BackgroundColor = Color.FromArgb("#F5F5F5"),
						CornerRadius = 10,
						Padding = new Thickness(5, 5, 5, 5),
						Margin = new Thickness(0, 0, 0, 0),
						Content = ExpContainer
						//{
						//	Text = monthName,
						//	FontAttributes = FontAttributes.Bold,
						//	FontSize = 18,
						//	TextColor = Colors.White
						//}
					};
					ExpContainer.Children.Add(new Label
					{
						Text = monthName,
						FontAttributes = FontAttributes.Bold,
						FontSize = 18,
						TextColor = Colors.Black
					});
					foreach (var expense in monthGroup.OrderBy(e => e.DayExp))
					{
						var expenseFrame = CreateExpenseFrame(expense);
						ExpContainer.Children.Add(expenseFrame);
					}
					vismounth.Children.Add(mainFrame);
					ExpContainer.Children.Add(infostack);
					var expTapGesture = new TapGestureRecognizer();
					expTapGesture.Tapped += (s, e) =>
					{
						CreateInfo(infostack, expi);
					};
				}
				
			}
		}

		private Frame CreateExpenseFrame(ExpendiTable expi)
		{
			var infoLayout = new HorizontalStackLayout { ClassId = expi.ExpId.ToString() };

			var nameLayout = new VerticalStackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Start,
				WidthRequest = 150
			};

			var amountLayout = new VerticalStackLayout
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.Center
			};

			nameLayout.Children.Add(new Label
			{
				Text = expi.NameExp,
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				TextColor = Colors.Black
			});

			nameLayout.Children.Add(new Label
			{
				Text = expi.CategoryExp,
				FontSize = 14,
				TextColor = Colors.Gray
			});

			infoLayout.Children.Add(nameLayout);

			amountLayout.Children.Add(new Label
			{
				Text = $"-{expi.AmountExp:N0}₽",
				FontSize = 16,
				FontAttributes = FontAttributes.Bold,
				TextColor = Colors.Red,
				HorizontalTextAlignment = TextAlignment.End
			});

			amountLayout.Children.Add(new Label
			{
				Text = $"{expi.DayExp}.{expi.MonthExp}.{expi.YearsExp}",
				FontSize = 14,
				TextColor = Colors.Gray,
				HorizontalTextAlignment = TextAlignment.End
			});

			infoLayout.Children.Add(amountLayout);

			return new Frame
			{
				Content = infoLayout,
				BorderColor = Colors.LightGray,
				CornerRadius = 10,
				Padding = 3,
				HasShadow = false,
				BackgroundColor = Colors.White,
				Margin = new Thickness(3, 2)
			};
		}

		private async void OnEditExpenseClicked(object sender, EventArgs e)
		{

		}

		private async void OnDeleteExpenseClicked(object sender, EventArgs e)
		{

		}

		private void CreateInfo(VerticalStackLayout mainst, ExpendiTable expi)
		{
			if(mainst.IsVisible) mainst.IsVisible = false;
			else mainst.IsVisible = true;

			var nameEntry = new Entry { Placeholder = "Название расходов", Text = expi.NameExp.ToString() };
			var amountEntry = new Entry { Placeholder = "Сумма расходов", Keyboard = Keyboard.Numeric };
			var pickercategory = new Picker
			{
				Items = {"","Переводы", "Маркетплейсы", "Копилка", "Супермаркеты", "Кафе и рестораны", "Косметика",
			"ЖКХ, связь и интернет", "Медицина", "Товары для детей", "Транспорт","Бизнес-услуги",
			"Кино и театр", "Налоги", "Развлечения","Другое"},
			};
			var saveButton = new Button
			{
				Text = "Создать",
				BackgroundColor = Color.FromArgb("#4CAF50"),
				TextColor = Colors.White,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var DateNewExpiStackLa = new HorizontalStackLayout { Padding = 10 };
			var dayEntry = new Entry { Placeholder = "День", Keyboard = Keyboard.Numeric };
			var pickermounth = new Picker
			{
				Items = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
					"Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" },
			};
			var yearsEntry = new Entry { Placeholder = "Год", Keyboard = Keyboard.Numeric };
			DateNewExpiStackLa.Children.Add(dayEntry);
			DateNewExpiStackLa.Children.Add(pickermounth);
			DateNewExpiStackLa.Children.Add(yearsEntry);
			mainst.Children.Add(new Label { Text = "Название" });
			mainst.Children.Add(nameEntry);
			//creationForm.Children.Add(new Label { Text = "Сумма" });
			//creationForm.Children.Add(amountEntry);
			//creationForm.Children.Add(new Label { Text = "Категория (необязательно)" });
			//creationForm.Children.Add(pickercategory);
			//creationForm.Children.Add(DateNewExpiStackLa);
			//creationForm.Children.Add(saveButton);
		}
		private int InvertMonth(string month)
		{
			switch (month)
			{
				case "Январь": return 1;
				case "Февраль": return 2;
				case "Март": return 3;
				case "Апрель": return 4;
				case "Май": return 5;
				case "Июнь": return 6;
				case "Июль": return 7;
				case "Август": return 8;
				case "Сентябрь": return 9;
				case "Октябрь": return 10;
				case "Ноябрь": return 11;
				case "Декабрь": return 12;
				default:
					break;
			}
			return 0;
		}
	}
}

