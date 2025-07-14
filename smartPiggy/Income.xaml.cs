using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using smartPiggy.Models;
using System.Net.NetworkInformation;
using smartPiggy.Servies;
using System.Net;
using Newtonsoft.Json;

namespace smartPiggy
{
	public partial class Income : ContentPage
	{
		public Income()
		{
			InitializeComponent();
			VisibleIncome();
		}

		private void CreateNewIncome()
		{
			plus_income.IsVisible = false;

			var mainST = new VerticalStackLayout();
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
				Text = "Создать новую запись доходов",
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

			var nameEntry = new Entry { Placeholder = "Название доходов" };
			var amountEntry = new Entry { Placeholder = "Сумма доходов", Keyboard = Keyboard.Numeric };
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

					var newInc = new IncomeTable
					{
						NameIncome = nameEntry.Text,
						AmountIncome = amount,
						CategoryIncome = pickercategory.SelectedItem.ToString(),
						DayIncome = int.Parse(dayEntry.Text),
						MounthIncome = InvertMonth(pickermounth.SelectedItem.ToString()),
						YearsIncome = int.Parse(yearsEntry.Text),
						TimeCreateincome = TimeOnly.FromDateTime(DateTime.Now)
					};

					await IncomeServies.Post(newInc);
					VisibleIncome();
					nameEntry.Text = string.Empty;
					amountEntry.Text = string.Empty;
					dayEntry.Text = string.Empty;
					pickermounth.SelectedItem = null;
					yearsEntry.Text = string.Empty;


					plus_income.IsVisible = true;
					addnewincome.Children.Remove(mainST);
				}
				catch (Exception ex)
				{
					await DisplayAlert("Ошибка на стороне сервера", $"{ex}", "OK");
					throw;
				}
			};



			mainST.Children.Add(headerFrame);
			mainST.Children.Add(creationForm);
			addnewincome.Children.Add(mainST);

		}

		private void VisibleIncome()
		{
			vismounth.Children.Clear(); // Основной контейнер для расходов
										// Получаем расходы из базы данных
			var incomess = IncomeServies.Get<List<IncomeTable>>();


			if (incomess == null || incomess.Count == 0)
			{
				vismounth.Children.Add(new Label
				{
					Text = "Нет доходов для отображения",
					HorizontalOptions = LayoutOptions.Center,
					TextColor = Colors.Gray
				});
				return;
			}
			var groupedIncomed = incomess
				.GroupBy(e => new DateTime(e.YearsIncome, e.MounthIncome, 1))
				.OrderByDescending(g => g.Key);
			foreach (var monthGroup in groupedIncomed)
			{
				foreach (var inco in incomess)
				{
					var IncoContainer = new VerticalStackLayout { };
					var infostack = new VerticalStackLayout { IsVisible = false };
					var monthName = monthGroup.Key.ToString("MMMM yyyy");
					var mainFrame = new Frame
					{
						BorderColor = Colors.Grey,
						BackgroundColor = Color.FromArgb("#F5F5F5"),
						CornerRadius = 10,
						Padding = new Thickness(5, 5, 5, 5),
						Margin = new Thickness(0, 0, 0, 0),
						Content = IncoContainer
						//{
						//	Text = monthName,
						//	FontAttributes = FontAttributes.Bold,
						//	FontSize = 18,
						//	TextColor = Colors.White
						//}
					};
					IncoContainer.Children.Add(new Label
					{
						Text = monthName,
						FontAttributes = FontAttributes.Bold,
						FontSize = 18,
						TextColor = Colors.Black
					});
					foreach (var incomes in monthGroup.OrderBy(e => e.DayIncome))
					{
						var incFrame = CreateIncomesFrame(incomes);
						IncoContainer.Children.Add(incFrame);
					}
					vismounth.Children.Add(mainFrame);
					IncoContainer.Children.Add(infostack);
					var incTapGesture = new TapGestureRecognizer();
					incTapGesture.Tapped += (s, e) =>
					{
						CreateInfo(infostack, inco);
					};
				}

			}
		}
		private Frame CreateIncomesFrame(IncomeTable inco)
		{
			var infoLayout = new HorizontalStackLayout { ClassId = inco.IncomeId.ToString() };

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
				Text = inco.NameIncome,
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				TextColor = Colors.Black
			});

			nameLayout.Children.Add(new Label
			{
				Text = inco.CategoryIncome,
				FontSize = 14,
				TextColor = Colors.Gray
			});

			infoLayout.Children.Add(nameLayout);

			amountLayout.Children.Add(new Label
			{
				Text = $"+{inco.AmountIncome:N0}₽",
				FontSize = 16,
				FontAttributes = FontAttributes.Bold,
				TextColor = Colors.Green,
				HorizontalTextAlignment = TextAlignment.End
			});

			amountLayout.Children.Add(new Label
			{
				Text = $"{inco.DayIncome}.{inco.MounthIncome}.{inco.YearsIncome}",
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

		private void CreateInfo(VerticalStackLayout mainst, IncomeTable inco)
		{
			if (mainst.IsVisible) mainst.IsVisible = false;
			else mainst.IsVisible = true;

			var nameEntry = new Entry { Placeholder = "Название расходов", Text = inco.NameIncome.ToString() };
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
		private void plus_income_Clicked(object sender, EventArgs e) => CreateNewIncome();

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

