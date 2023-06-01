using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalaryCounter.Views;

public partial class EmployerTaskPage : ContentPage
{
    private readonly AppRepository _db;
    public EmployerTaskPage(AppRepository db)
	{
        _db = db;
        InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetTasks();
        pickerCommand.Items.Add("Добавить");
        pickerCommand.Items.Add("Удалить");
        pickerCommand.Items.Add("Изменить");
        pickerCommand.Items.Add("Отметить выполненным");
    }
    private void Clicked(object sender, EventArgs e)
    {
        if (pickerCommand.SelectedItem == null)
            return;
        if (pickerCommand.SelectedItem.Equals("Добавить"))
        {
            if (AddName.Text == null || AddRate.Text == null || AddEmployer.Text==null)
            {
                AppShell.Current.DisplayAlert("Добавление", "Не все ячейки заполнены", "OK");
                return;
            }
            decimal i = 0;
            string s = AddRate.Text;
            bool result = decimal.TryParse(s, out i);
            if (!result) 
            {
                AppShell.Current.DisplayAlert("Добавление", "Тариф не decimal", "OK");
                return;
            }
            if (AddName.Equals("") || AddRate.Equals("") || AddEmployer.Equals(""))
            {
                AppShell.Current.DisplayAlert("Добавление", "Не все ячейки заполненны", "OK");
                return;
            }
            int idTask = _db.FindLastEmployerTask() + 1;
            string[] split = { "," };
            string[] employerStr = AddEmployer.Text.Split(split, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in employerStr)
            {
                int i2 = 0;
                string s2 = item;
                bool result2 = int.TryParse(s2, out i2);
                if (!result2) 
                { 
                    AppShell.Current.DisplayAlert("Добавление", "Неправильно заполнено поле сотрудников", "OK");
                    return;
                }
            }
            foreach (string str in employerStr)
            {
                var temp = new EmployersList()
                {
                    IdEmployer = Convert.ToInt32(str),
                    IdTask = idTask
                };
                _db.CreateEmployersList(temp);
            }
            var task = new EmployerTask()
            {
                Name = AddName.Text,
                Rate = Convert.ToDecimal(AddRate.Text),
                EmployersId = _db.FindLastEmployerTask() + 1,
                StartTime = DatePickerStart.Date,
                EndTime = DatePickerEnd.Date,
            };
            _db.CreateEmployerTask(task);
            GetTasks();
        }
        if (pickerCommand.SelectedItem.Equals("Удалить"))
        {
            if (collectionView.SelectedItem is null)
                return;

            var template = collectionView.SelectedItem as TaskVisible;
            if (template is null)
                return;
            _db.DeleteEmployerTask(template.Id);
            GetTasks();
        }
        if (pickerCommand.SelectedItem.Equals("Изменить"))
        {
            if (collectionView.SelectedItem is null)
                return;

            var template = collectionView.SelectedItem as TaskVisible;
            if (template is null)
                return;
            string[] split = { "," };
            string[] employerStr = AddEmployer.Text.Split(split, StringSplitOptions.RemoveEmptyEntries);
            _db.DeleteEmployersList(template.Id);
            foreach (string str in employerStr)
            {
                var temp = new EmployersList()
                {
                    IdEmployer = Convert.ToInt32(str),
                    IdTask = template.Id
                };
                _db.CreateEmployersList(temp);
            }
            var task = new EmployerTask() 
            {
                Id = template.Id,
                Name = AddName.Text,
                Rate = Convert.ToDecimal(AddRate.Text),
                EmployersId = template.Id,
                EndTime = DatePickerEnd.Date,
                StartTime = DatePickerStart.Date,
                ResultTime = template.ResultTime
            };
            _db.UpdateEmployerTask(task);
        }
            GetTasks();
            
        if (pickerCommand.SelectedItem.Equals("Отметить выполненным"))
        {
            if (collectionView.SelectedItem is null)
                return;

            var template = collectionView.SelectedItem as TaskVisible;
            DateTime temp = DateTime.Now;
            if (template is null)
                return;
            _db.UpdateEmployerTask(template.Id, temp);
            GetTasks();
        }
    }
    private void Search(object sender, EventArgs e)
    {
        collectionView.ItemsSource = Enumerable.Reverse(_db.GetTasks()).ToList();
        var source = new List<TaskVisible>();
        foreach (var item in collectionView.ItemsSource)
        {
            if (item is TaskVisible collectionItem)
            {
                if (collectionItem.Name.Contains(SearchEntry.Text))
                    source.Add(collectionItem);
            }
        }
        collectionView.ItemsSource = source;

    }
    private void GetTasks()
    {
        collectionView.ItemsSource = Enumerable.Reverse(_db.GetTasks()).ToList();
        foreach (var item in collectionView.ItemsSource)
        {
            if (item is TaskVisible collectionItem)
            {
                if (collectionItem.ResultTime == DateTime.MinValue)
                    collectionItem.DR = false;
            }
        }
        AddName.Text = string.Empty;
        AddRate.Text = string.Empty;
        AddEmployer.Text = string.Empty;
        SearchEntry.Text = string.Empty;
    }
}