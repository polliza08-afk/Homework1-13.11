using System.Windows;
using WpfTest.Entities;
using WpfTest.Models;

namespace WpfTest;

/// <summary>
/// Стартове вікно: ввід ПІБ, вибір тесту і запуск проходження.
/// </summary>
public partial class BeginWindow : Window
{
    public BeginWindow()
    {
        InitializeComponent(); //Ініціалізація графічних елементів вікна з XAML

        //Підключення до бази даних та завантаження списку тестів
        using var db = new MyDataContext();
        TestsListBox.ItemsSource = db
            .Tests
            .OrderBy(t => t.Title) //Сортування тестів за назвою в алфавітному порядку
            .Select(x => new TestItemModel
            {
                Title = x.Title,
                Description = x.Description,
                QuestionsCount = x.Questions.Count,
                TimeLimitMinutes = x.TimeLimitMinutes.Value
            })
            .ToList();

        //Якщо в списку є тести, автоматично виділяємо перший елемент
        if (TestsListBox.Items.Count > 0)
        {
            TestsListBox.SelectedIndex = 0;
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        //Зчитуємо ПІБ з текстових полів, прибираючи зайві пробіли
        var lastName = txtLastName.Text.Trim();
        var firstName = txtFirstName.Text.Trim();
        var middleName = txtMiddleName.Text.Trim();

        //Перевіряє чи заповнені обов'язкові поля
        //Якщо одне з них порожнє, виводиться повідомлення і виконання переривається.
        if (lastName.Length == 0 || firstName.Length == 0)
        {
            MessageBox.Show("Введіть прізвище та ім'я.", "Увага",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return; //Перериваємо виконання, якщо поля порожні
        }

        //Перевіряє чи обрав користувач тест зі списку
        if (TestsListBox.SelectedItem is not TestItemModel selected)
        {
            MessageBox.Show("Оберіть тест зі списку.", "Увага",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return; //Перериваємо виконання, якщо нічого не обрано
        }

        //Створюємо нове вікно тесту, відкриваємо його і закриваємо поточне
        var testWindow = new TestWindow();
        testWindow.Show();
        Close();
    }
}