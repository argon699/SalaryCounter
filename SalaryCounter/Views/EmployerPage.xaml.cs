using Microsoft.Maui;

namespace SalaryCounter.Views;

public partial class EmployerPage : ContentPage
{
    private readonly AppRepository _db;
    public EmployerPage(AppRepository db)
	{
        _db = db;
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetEmployers();
        pickerCommand.Items.Add("Добавить");
        pickerCommand.Items.Add("Удалить");
        pickerCommand.Items.Add("Изменить");
    }
    private void Clicked(object sender, EventArgs e)
    {
        if (pickerCommand.SelectedItem == null)
            return;
        if (pickerCommand.SelectedItem.Equals("Добавить"))
        {
            if (!FirstNameEntry.Text.Equals("") && !LastNameEntry.Text.Equals(""))
            { 
                var employer = new Employer()
            {
                FirstName = FirstNameEntry.Text,
                LastName = LastNameEntry.Text
            };
            _db.CreateEmployer(employer);
        } else AppShell.Current.DisplayAlert("Добавление", "Есть пустые поля", "OK");
            GetEmployers();
        }
        if (pickerCommand.SelectedItem.Equals("Изменить"))
        {
            if (collectionView.SelectedItem is null)
                return;

            var employer = collectionView.SelectedItem as Employer;
            if (employer is null)
                return;
            if (!FirstNameEntry.Text.Equals("") && !LastNameEntry.Text.Equals(""))
            {
                employer.FirstName = FirstNameEntry.Text;
                employer.LastName = LastNameEntry.Text;
                _db.UpdateEmployer(employer);
            }
            else AppShell.Current.DisplayAlert("Изменение", "Есть пустые поля", "OK");
            GetEmployers();
        }
        if (pickerCommand.SelectedItem.Equals("Удалить"))
        {
            if (collectionView.SelectedItem is null)
                return;

            var employer = collectionView.SelectedItem as Employer;
            if (employer is null)
                return;
            if(_db.FindEmployers(employer.Id).Count()<1)
                _db.DeleteEmployer(employer);
            else AppShell.Current.DisplayAlert("Удаление", "Сначала удалите задачи, в которых участвует сотрудник", "OK");
            GetEmployers();
        }
    }
    private void Search(object sender, EventArgs e)
    {
        collectionView.ItemsSource = _db.GetEmployers();
        var source = new List<Employer>();
        foreach (var item in collectionView.ItemsSource)
        {
            if (item is Employer collectionItem)
            {
                if (collectionItem.LastName.Contains(SearchEntry.Text))
                    source.Add(collectionItem);
            }
        }
        collectionView.ItemsSource = source;
    }
    private void GetEmployers()
    {
        FirstNameEntry.Text = string.Empty;
        LastNameEntry.Text = string.Empty;
        SearchEntry.Text = string.Empty;
        collectionView.ItemsSource = _db.GetEmployers();
    }
}