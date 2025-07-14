using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using smartPiggy.Models;
using smartPiggy.Pages;
using smartPiggy.Servies;
using System.Net.NetworkInformation;
namespace smartPiggy
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			VisibleNewPiggy();
		}

		private void Create_Piggy_Click(object sender, EventArgs e) => CreateNewPiggy();

		private void CreateNewPiggy()
		{
			plus_piggy.IsVisible = false;

			// Основной контейнер
			var piggyCreationContainer = new VerticalStackLayout();

			// Верхняя часть с кнопкой свернуть/развернуть
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
				Text = "Создать новую копилку",
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

			// Форма создания (изначально видима)
			var creationForm = new VerticalStackLayout
			{
				Spacing = 10,
				IsVisible = true,
				Margin = new Thickness(0, 5, 0, 0)
			};

			// Поля формы
			var nameEntry = new Entry { Placeholder = "Название копилки" };
			var purposeEntry = new Entry { Placeholder = "Цель накопления" };
			var taskEntry = new Entry { Placeholder = "Я хочу откладывать..." };
			var amountEntry = new Entry { Placeholder = "1 000", Keyboard = Keyboard.Numeric };
			var periodPicker = new Picker
			{
				Items = { "день", "неделю", "месяц", "год" }
			};
			var startAmountEntry = new Entry { Placeholder = "Сейчас я положу...", Keyboard = Keyboard.Numeric };
			var targetAmountEntry = new Entry { Placeholder = "Сколько нужно набрать", Keyboard = Keyboard.Numeric };

			var saveButton = new Button
			{
				Text = "Создать",
				BackgroundColor = Color.FromArgb("#4CAF50"),
				TextColor = Colors.White,
				Margin = new Thickness(0, 10, 0, 0)
			};

			// Горизонтальный layout для суммы и периода
			var amountLayout = new HorizontalStackLayout
			{
				Spacing = 5,
				VerticalOptions = LayoutOptions.Center
			};
			amountLayout.Children.Add(new Label { Text = "в" });
			amountLayout.Children.Add(periodPicker);

			// Собираем форму
			creationForm.Children.Add(new Label { Text = "Название" });
			creationForm.Children.Add(nameEntry);
			creationForm.Children.Add(new Label { Text = "Цель" });
			creationForm.Children.Add(purposeEntry);
			creationForm.Children.Add(new Label { Text = "Задача (необязательно)" });
			creationForm.Children.Add(taskEntry);

			var taskLayout = new HorizontalStackLayout { Spacing = 5 };
			taskLayout.Children.Add(amountEntry);
			taskLayout.Children.Add(amountLayout);
			creationForm.Children.Add(taskLayout);

			creationForm.Children.Add(new Label { Text = "Начальное значение" });
			creationForm.Children.Add(startAmountEntry);
			creationForm.Children.Add(new Label { Text = "Конечный результат" });
			creationForm.Children.Add(targetAmountEntry);
			creationForm.Children.Add(saveButton);

			// Обработчик кнопки свернуть/развернуть
			toggleButton.Clicked += (s, e) =>
			{
				creationForm.IsVisible = !creationForm.IsVisible;
				toggleButton.Text = creationForm.IsVisible ? "▽" : "△";
				headerFrame.CornerRadius = creationForm.IsVisible ? 10 : 20; // Меняем скругление
			};

			// Обработчик сохранения
			saveButton.Clicked += async (s, e) =>
			{
				if (string.IsNullOrWhiteSpace(nameEntry.Text))
				{
					await DisplayAlert("Ошибка", "Название копилки обязательно", "OK");
					return;
				}

				if (!int.TryParse(startAmountEntry.Text, out var startAmount) ||
					!int.TryParse(targetAmountEntry.Text, out var targetAmount))
				{
					await DisplayAlert("Ошибка", "Проверьте числовые поля", "OK");
					return;
				}

				if (periodPicker.SelectedItem != null || !string.IsNullOrEmpty(taskEntry.Text)
				|| !string.IsNullOrEmpty(amountEntry.Text))
				{
					if (!int.TryParse(amountEntry.Text, out var amount))
					{
						await DisplayAlert("Ошибка", "Проверьте числовые поля", "OK");
						return;
					}
					if (periodPicker.SelectedItem == null)
					{
						await DisplayAlert("Ошибка", "Очистите или полностью введите задачу", "OK");
						return;
					}
					if (string.IsNullOrEmpty(taskEntry.Text))
					{
						await DisplayAlert("Ошибка", "Очистите или полностью введите задачу", "OK");
						return;
					}
					if (string.IsNullOrEmpty(amountEntry.Text))
					{
						await DisplayAlert("Ошибка", "Очистите или полностью введите задачу", "OK");
						return;
					}

				}
				else
				{
					periodPicker.SelectedItem = "";
					taskEntry.Text = null;
					amountEntry.Text = "0";
				}

				try
				{
					var newPiggy = new PiggyTable
					{
						NamePiggy = nameEntry.Text,
						PurposePiggy = purposeEntry.Text,
						TaskPiggy = taskEntry.Text,
						CoutTaskPiggy = decimal.Parse(amountEntry.Text),
						TimeTaskString = periodPicker.SelectedItem.ToString(),
						CoutStartPiggy = startAmount,
						SumPiggy = targetAmount,
						StartDataPiggy = DateTime.Now.Date.ToString("yyyy-MM-dd"),
					};

					await PiggyServies.Post(newPiggy);

					// Очищаем форму
					nameEntry.Text = string.Empty;
					purposeEntry.Text = string.Empty;
					taskEntry.Text = string.Empty;
					amountEntry.Text = string.Empty;
					startAmountEntry.Text = string.Empty;
					targetAmountEntry.Text = string.Empty;

					// Обновляем список копилок
					VisibleNewPiggy();

					// Скрываем форму
					plus_piggy.IsVisible = true;
					addPiggyStackLayout.Children.Remove(piggyCreationContainer);
				}
				catch (Exception ex)
				{
					await DisplayAlert("Ошибка", ex.Message, "OK");
				}
			};

			// Собираем основной контейнер
			piggyCreationContainer.Children.Add(headerFrame);
			piggyCreationContainer.Children.Add(creationForm);
			addPiggyStackLayout.Children.Add(piggyCreationContainer);
		}

		List<PiggyTable> piggies;
		decimal sumklient = 0;
		decimal sumtask = 0;
		private void VisibleNewPiggy()
		{
			visnewpiggyStackLayout.Children.Clear();

			piggies = PiggyServies.Get<List<PiggyTable>>();

			if (piggies == null || piggies.Count == 0)
			{
				visnewpiggyStackLayout.Children.Add(new Label
				{
					Text = "У вас пока нет копилок",
					HorizontalOptions = LayoutOptions.Center,
					TextColor = Colors.Gray
				});
				return;
			}

			foreach (var piggy in piggies)
			{

				sumklient += piggy.CoutStartPiggy;
				sumtask += piggy.SumPiggy;
				sumlb.Text = $"Общее количество набранных средств: {sumklient} / {sumtask}";
				// Основной контейнер для всей копилки
				var piggyContainer = new VerticalStackLayout { ClassId = piggy.IdPiggy.ToString() };

				// Верхняя часть (всегда видимая)
				var topFrame = new Frame
				{
					BorderColor = Colors.Grey,
					BackgroundColor = Color.FromArgb("#F5F5F5"),
					CornerRadius = 10,
					Padding = new Thickness(15, 15, 5, 15),
					Margin = new Thickness(0, 0, 0, 0)
				};

				// Используем Grid для точного позиционирования
				var topGrid = new Grid
				{
					ColumnDefinitions =
			{
				new ColumnDefinition { Width = GridLength.Star }, // Информация
                new ColumnDefinition { Width = GridLength.Auto }, // Кнопка раскрытия
                new ColumnDefinition { Width = GridLength.Auto }, // Кнопка редактирования
                new ColumnDefinition { Width = GridLength.Auto }  // Кнопка удаления
            },
					ColumnSpacing = 5
				};

				// Информационная часть
				var infoLayout = new VerticalStackLayout();
				bool puprsize = piggy.CoutStartPiggy == piggy.SumPiggy;
				// Название
				infoLayout.Children.Add(new Label
				{
					Text = piggy.NamePiggy,
					FontSize = 18,
					FontAttributes = FontAttributes.Bold,
					TextColor = puprsize ? Colors.Green : Colors.Black,
				});

				// Краткая информация
				infoLayout.Children.Add(new Label
				{
					Text = $"Набрано: {piggy.CoutStartPiggy}₽ из {piggy.SumPiggy}₽",
					TextColor = Colors.Green
				});

				topGrid.Add(infoLayout, 0, 0);

				// Кнопка раскрытия/скрытия
				var toggleButton = new Button
				{
					Text = "▽",
					FontSize = 20,
					TextColor = Colors.Blue,
					BackgroundColor = Colors.Transparent,
					Padding = new Thickness(5),
					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.Center
				};
				topGrid.Add(toggleButton, 1, 0);

				// Кнопка редактирования (карандаш)
				var editButton = new Button
				{
					Text = "✏️", // Или используйте иконку
					FontSize = 16,
					TextColor = Colors.Gray,
					BackgroundColor = Colors.Transparent,
					Padding = new Thickness(5),
					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.Center,
					ClassId = piggy.IdPiggy.ToString()
				};
				editButton.Clicked += OnEditButtonClicked;
				topGrid.Add(editButton, 2, 0);

				// Кнопка удаления
				var deleteButton = new Button
				{
					Text = "×",
					FontSize = 20,
					TextColor = Colors.Red,
					BackgroundColor = Colors.Transparent,
					Padding = new Thickness(5),
					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.Center,
					ClassId = piggy.IdPiggy.ToString()
				};
				deleteButton.Clicked += OnDeleteButtonClicked;
				topGrid.Add(deleteButton, 3, 0);

				// Дополнительная информация (изначально скрыта)
				var detailsLayout = new VerticalStackLayout
				{
					IsVisible = false,
					Margin = new Thickness(0, 10, 0, 0)
				};

				// Форма редактирования (изначально скрыта)
				var editFormLayout = new StackLayout
				{
					IsVisible = false,
					Margin = new Thickness(0, 10, 0, 0),
					Spacing = 10
				};

				// Добавляем поля для редактирования
				var nameEntry = new Entry { Text = piggy.NamePiggy, Placeholder = "Название копилки" };
				var sumEntry = new Entry { Text = piggy.SumPiggy.ToString(), Placeholder = "Хочу накопить", Keyboard = Keyboard.Numeric };
				var purposeEntry = new Entry { Text = piggy.PurposePiggy, Placeholder = "Цель" };
				var taskEntry = new Entry { Text = piggy.TaskPiggy, Placeholder = "Задача" };
				var couttaskEntry = new Entry { Text = piggy.CoutTaskPiggy.ToString(), Placeholder = "1000" };
				var timetaskEntry = new Entry { Text = piggy.TimeTaskString, Placeholder = "Я буду откладывать..." };
				var coutstartEntry = new Entry { Text = piggy.CoutStartPiggy.ToString(), Placeholder = "Я положу.." };
				var startdataEntry = DateTime.Now.Date.ToString("yyyy-MM-dd");
				var periodPicker = new Picker
				{
					Items = { "день", "неделю", "месяц", "год" },
					SelectedItem = piggy.TimeTaskString
				};
				var creationForm = new VerticalStackLayout
				{
					Spacing = 10,
					IsVisible = true,
					Margin = new Thickness(0, 5, 0, 0)
				};
				var saveButton = new Button { Text = "Сохранить", BackgroundColor = Colors.Green, TextColor = Colors.White };
				var cancelButton = new Button { Text = "Отмена", BackgroundColor = Colors.Gray, TextColor = Colors.White };

				editFormLayout.Children.Add(new Label { Text = "Редактирование:" });
				var amountLayout = new HorizontalStackLayout
				{
					Spacing = 5,
					VerticalOptions = LayoutOptions.Center
				};
				amountLayout.Children.Add(new Label { Text = "в" });
				amountLayout.Children.Add(periodPicker);

				// Собираем форму
				creationForm.Children.Add(new Label { Text = "Название" });
				creationForm.Children.Add(nameEntry);
				creationForm.Children.Add(new Label { Text = "Цель" });
				creationForm.Children.Add(purposeEntry);
				creationForm.Children.Add(new Label { Text = "Задача (необязательно)" });
				creationForm.Children.Add(taskEntry);

				var taskLayout = new HorizontalStackLayout { Spacing = 5 };
				taskLayout.Children.Add(couttaskEntry);
				taskLayout.Children.Add(amountLayout);
				creationForm.Children.Add(taskLayout);

				creationForm.Children.Add(new Label { Text = "Начальное значение" });
				creationForm.Children.Add(coutstartEntry);
				creationForm.Children.Add(new Label { Text = "Конечный результат" });
				creationForm.Children.Add(sumEntry);


				var buttonsLayout = new HorizontalStackLayout { Spacing = 10 };
				buttonsLayout.Children.Add(saveButton);
				buttonsLayout.Children.Add(cancelButton);
				editFormLayout.Children.Add(creationForm);
				editFormLayout.Children.Add(buttonsLayout);

				// Обработчики кнопок формы
				saveButton.Clicked += async (s, e) =>
				{
					if (periodPicker.SelectedItem != null || !string.IsNullOrEmpty(taskEntry.Text)
				|| !string.IsNullOrEmpty(couttaskEntry.Text))
					{
						if (!int.TryParse(couttaskEntry.Text, out var amount))
						{
							await DisplayAlert("Ошибка", "Проверьте числовые поля", "OK");
							return;
						}
						if (periodPicker.SelectedItem == null)
						{
							await DisplayAlert("Ошибка", "Очистите или полностью введите задачу", "OK");
							return;
						}
						if (string.IsNullOrEmpty(taskEntry.Text))
						{
							await DisplayAlert("Ошибка", "Очистите или полностью введите задачу", "OK");
							return;
						}
						if (string.IsNullOrEmpty(couttaskEntry.Text))
						{
							await DisplayAlert("Ошибка", "Очистите или полностью введите задачу", "OK");
							return;
						}

					}
					else
					{
						periodPicker.SelectedItem = "";
						taskEntry.Text = null;
						couttaskEntry.Text = "0";
					}
					if (int.Parse(coutstartEntry.Text) < int.Parse(sumEntry.Text))
					{
						await DisplayAlert("Ошибка", "Начальное значение не может быть больше конечного", "OK");
						return;
					}
					if (!decimal.TryParse(coutstartEntry.Text, out decimal a) ||
					!decimal.TryParse(sumEntry.Text, out decimal b))
					{
						await DisplayAlert("Ошибка", "Проверьте числовые поля", "OK");
						return;
					}
					var currentPiggy = PiggyServies.Get<PiggyTable>(piggy.IdPiggy.ToString());
					// Сохраняем изменения
					currentPiggy.NamePiggy = nameEntry.Text;
					currentPiggy.PurposePiggy = purposeEntry.Text;
					currentPiggy.CoutTaskPiggy = decimal.Parse(couttaskEntry.Text);
					currentPiggy.TaskPiggy = timetaskEntry.Text;
					currentPiggy.TaskPiggy = taskEntry.Text;
					currentPiggy.TaskPiggy = taskEntry.Text;
					currentPiggy.SumPiggy = decimal.Parse(sumEntry.Text);
					bool answer = await Application.Current.MainPage.DisplayAlert(
				"Удаление копилки",
				$"Изменения обновят Ваш прогресс. Согласны?",
				"Да", "Нет");
					if (answer)
					{
						try
						{
							PiggyServies.Put(currentPiggy, piggy.IdPiggy.ToString());
							editFormLayout.IsVisible = false;
							detailsLayout.IsVisible = false;
							toggleButton.Text = "▽";
							sumklient = 0;
							sumtask = 0;
							Visible();
							nameEntry.Text = string.Empty;
							purposeEntry.Text = string.Empty;
							taskEntry.Text = string.Empty;
							couttaskEntry.Text = string.Empty;
							timetaskEntry.Text = string.Empty;
							coutstartEntry.Text = string.Empty;
						}
						catch (Exception ex)
						{
							await DisplayAlert("Успех", "Копилка изменилась", "ОК");
							throw;
						}
					}

				};

				cancelButton.Clicked += (s, e) =>
				{
					editFormLayout.IsVisible = false;
				};

				// Добавляем детали в скрываемую часть
				if (!string.IsNullOrEmpty(piggy.PurposePiggy))
					detailsLayout.Children.Add(new Label
					{
						Text = $"Цель: {piggy.PurposePiggy}",
						TextColor = Colors.Gray
					});

				bool isOverdue = false;
				if (!string.IsNullOrEmpty(piggy.TaskPiggy))
				{
					detailsLayout.Children.Add(new Label
					{
						Text = $"План: {piggy.TaskPiggy} {piggy.CoutTaskPiggy}₽/{piggy.TimeTaskString}",
						TextColor = Colors.Blue
					});
					detailsLayout.Children.Add(new Label
					{
						Text = $"{TaskTimePiggy(piggy, out isOverdue)}",
						TextColor = isOverdue ? Colors.Red : Colors.Green,
						FontAttributes = isOverdue ? FontAttributes.Bold : FontAttributes.None
					});
				}

				toggleButton.Clicked += (sender, e) =>
				{
					if (detailsLayout.IsVisible)
					{
						detailsLayout.IsVisible = false;
						editFormLayout.IsVisible = false;
						toggleButton.Text = "▽";
					}
					else
					{
						detailsLayout.IsVisible = true;
						toggleButton.Text = "△";
					}
				};

				topFrame.Content = topGrid;

				piggyContainer.Children.Add(topFrame);
				piggyContainer.Children.Add(detailsLayout);
				piggyContainer.Children.Add(editFormLayout);

				var piggyTapGesture = new TapGestureRecognizer();
				piggyTapGesture.Tapped += Navigation_Tapped;
				topFrame.GestureRecognizers.Add(piggyTapGesture);

				visnewpiggyStackLayout.Children.Add(piggyContainer);
			}
		}
		private void OnEditButtonClicked(object sender, EventArgs e)
		{
			var button = (Button)sender;
			var piggyId = button.ClassId;

			// Находим контейнер копилки
			if (button.Parent.Parent.Parent is VerticalStackLayout piggyContainer)
			{
				// Скрываем детали и показываем форму редактирования
				var detailsLayout = piggyContainer.Children[1] as VerticalStackLayout;
				var editFormLayout = piggyContainer.Children[2] as StackLayout;

				detailsLayout.IsVisible = false;
				editFormLayout.IsVisible = true;

				// Меняем стрелку на закрытое состояние
				if (button.Parent is Grid grid && grid.Children[1] is Button toggleButton)
				{
					toggleButton.Text = "▽";
				}
			}
		}
		private async void OnDeleteButtonClicked(object sender, EventArgs e)
		{
			var button = (Button)sender;
			var piggyId = button.ClassId; // Получаем ID как строку (не нужно преобразовывать в int)

			// Находим соответствующую копилку в списке
			var piggyToDelete = piggies.FirstOrDefault(p => p.IdPiggy.ToString() == piggyId);

			if (piggyToDelete == null)
			{
				await Application.Current.MainPage.DisplayAlert("Ошибка", "Копилка не найдена", "OK");
				return;
			}

			// Запрос подтверждения
			bool answer = await Application.Current.MainPage.DisplayAlert(
				"Удаление копилки",
				$"Вы уверены, что хотите удалить копилку '{piggyToDelete.NamePiggy}'?",
				"Да", "Нет");

			if (answer)
			{
				try
				{
					// Удаляем из базы данных
					PiggyServies.Delete<PiggyTable>(piggyToDelete, piggyId);
					sumklient = 0;
					sumtask = 0;
					VisibleNewPiggy();
					await DisplayAlert("Успех", "Копилка удалена", "ОК");
				}
				catch (Exception ex)
				{
					await Application.Current.MainPage.DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
				}
			}
		}

		private async void Navigation_Tapped(object sender, TappedEventArgs e)
		{
			if (sender is View view && view.Parent is VerticalStackLayout tappedStack)
			{
				string stackId = tappedStack.ClassId;

				// 3. Находим соответствующий объект
				var selectedPiggy = piggies.FirstOrDefault(p => p.IdPiggy.ToString() == stackId);

				if (selectedPiggy != null)
				{
					// Создаем TaskCompletionSource для отслеживания закрытия страницы
					var pageClosedTask = new TaskCompletionSource<bool>();
					var infoPage = new InfoPiggy(selectedPiggy);

					// Подписываемся на событие Disappearing страницы
					infoPage.Disappearing += (s, args) => pageClosedTask.SetResult(true);

					// Переходим на страницу
					await Navigation.PushModalAsync(infoPage);

					// Ждем, пока страница закроется
					await pageClosedTask.Task;
				}
				sumklient = 0;
				sumtask = 0;
				// Этот код выполнится только после закрытия InfoPiggy
				VisibleNewPiggy();
			}


		}

		private string TaskTimePiggy(PiggyTable piggy, out bool isOverdue)
		{
			isOverdue = false;
			string datetime = CalculateClass.Calculate(piggy.StartDataPiggy, GetTimeMode(piggy.TimeTaskString));

			// Простая проверка на просрочку (может потребоваться доработка)
			if (datetime.StartsWith("Просрочено"))
			{
				isOverdue = true;
			}

			return datetime;
		}

		private CalculateClass.TimeMode GetTimeMode(string timeTaskString)
		{
			return timeTaskString switch
			{
				"день" => CalculateClass.TimeMode.Day,
				"месяц" => CalculateClass.TimeMode.Month,
				"неделю" => CalculateClass.TimeMode.Week,
				"год" => CalculateClass.TimeMode.Year,
				_ => CalculateClass.TimeMode.Day
			};
		}

		private void Visible() => VisibleNewPiggy();

	}
}
