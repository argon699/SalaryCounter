using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace SalaryCounter.Views;

public partial class ReportPage : ContentPage
{
    private readonly AppRepository _db;
    public static int HolderTaskId;
    public ReportPage(AppRepository db)
	{
       // chart.IsVisible = false;
        _db = db;
        InitializeComponent();
	}
    private void Clear() 
    {
        pickerEmployer.SelectedItem = null;
        pickerTask.SelectedItem = null;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        pickerCommand.Items.Add("Подсчёт эффективности работника");
        pickerCommand.Items.Add("Подсчёт всех работников");
        pickerCommand.Items.Add("Генерация зарплаты по задаче");
        pickerCommand.Items.Add("Генерация трудозатрат");
        pickerCommand.Items.Add("Генерация трудозатрат по задаче");
        pickerCommand.Items.Add("Оценка сотрудника");
        var pickerEmp = _db.GetEmployers();
        List<string> Emp = new();
        List<string> Tas = new();
        foreach (var item in pickerEmp)
        {
            Emp.Add(item.LastName);
            //pickerEmployer.Items.Add(item.LastName);
        }
        var pickerTas = _db.GetEmployerTasks();
        foreach (var item in pickerTas)
        {
            Tas.Add(item.Id.ToString());
            //pickerTask.Items.Add(item.Id.ToString());
        }
        pickerTask.ItemsSource = Tas;
        pickerEmployer.ItemsSource =Emp;
        //chart.Source = "C:\\Users\\pilyu\\Desktop\\Отчет2.png";
    }
    private void Clicked(object sender, EventArgs e)
    {
        if (pickerCommand.SelectedItem.Equals("Подсчёт эффективности работника"))
        {
            if (pickerEmployer.SelectedItem != null)
            {
                int idEmployer = _db.FindEmployerId(pickerEmployer.SelectedItem.ToString());
                var Efficiency = _db.FindEmployerEfficiency(idEmployer);
                var Employer = _db.FindEmployer(idEmployer);
                var filePath = "C:\\Users\\pilyu\\Desktop\\Отчет.csv";
                FileStream fileStream = File.Open(filePath, FileMode.Open);
                fileStream.SetLength(0);
                fileStream.Close();
                File.AppendAllText(filePath, "Имя;Фамилия;Эффективность\n", Encoding.UTF8);
                string str = $"{Employer.FirstName};{Employer.LastName};{Efficiency}\n";
                File.AppendAllText(filePath, str, Encoding.UTF8);
                AppShell.Current.DisplayAlert("Генерация", "Файл csv обновлен", "OK");
                OnAppearing();
            }
            else
            {
                AppShell.Current.DisplayAlert("Генерация", "Выберите сотрудника", "OK");
            } 
                Clear();
        }
        if (pickerCommand.SelectedItem.Equals("Генерация зарплаты по задаче"))
        {
            if (pickerTask.SelectedItem != null)
            {
                HolderTaskId = Convert.ToInt32(pickerTask.SelectedItem);
                Shell.Current.GoToAsync($"CountSalaryTask");
            }
            else
            {
                AppShell.Current.DisplayAlert("Генерация", "Выберите задачу", "OK");
            }
                Clear();
        }
        if (pickerCommand.SelectedItem.Equals("Подсчёт всех работников"))
        {
            var filePath = "C:\\Users\\pilyu\\Desktop\\Отчет.csv";
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "Имя;Фамилия;Эффективность\n", Encoding.UTF8);

            var Employers = _db.GetEmployers();
            foreach (var employer in Employers)
            { 
             var Efficiency = _db.FindEmployerEfficiency(employer.Id);
             var Employer = _db.FindEmployer(employer.Id);

             string str = $"{Employer.FirstName};{Employer.LastName};{Efficiency}\n";
             File.AppendAllText(filePath, str, Encoding.UTF8);
            }
            
            AppShell.Current.DisplayAlert("Генерация", "Файл csv обновлен", "OK");
            //OnAppearing();


            Clear();
        }
        if (pickerCommand.SelectedItem.Equals("Генерация трудозатрат"))
        {
            var Result = _db.FindEmployerLaborCost();

            var filePath = "C:\\Users\\pilyu\\Desktop\\Отчет.csv";
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "id Задачи,Название,Трудозатраты\n", Encoding.UTF8);

            foreach (var result in Result) 
            {
                File.AppendAllText(filePath, result, Encoding.UTF8);
            }
           
            AppShell.Current.DisplayAlert("Генерация", "Файл csv обновлен", "OK");
            //OnAppearing();
            Clear();        
        }
        if (pickerCommand.SelectedItem.Equals("Генерация трудозатрат по задаче"))
        {
            if (pickerTask.SelectedItem == null)
            {
                AppShell.Current.DisplayAlert("Генерация", "Выберите задачу", "OK");
                return;
            }
            var Result = _db.FindEmployerLaborCostTask(Convert.ToInt32(pickerTask.SelectedItem));

            var filePath = "C:\\Users\\pilyu\\Desktop\\Отчет.csv";
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
            File.AppendAllText(filePath, "id Задачи,Название,Трудозатраты\n", Encoding.UTF8);

            foreach (var result in Result)
            {
                File.AppendAllText(filePath, result, Encoding.UTF8);
            }
           
            AppShell.Current.DisplayAlert("Генерация", "Файл csv обновлен", "OK");
            //OnAppearing();
            Clear();
        }
        if (pickerCommand.SelectedItem.Equals("Оценка сотрудника"))
        {
            if (pickerEmployer.SelectedItem == null)
            {
                AppShell.Current.DisplayAlert("Генерация", "Выберите сотрудника", "OK"); 
                return;
            }
            int idEmployer = _db.FindEmployerId(pickerEmployer.SelectedItem.ToString());
            var Efficiency = _db.FindTerms(idEmployer); 
            var Employer = _db.FindEmployer(idEmployer);
            var filePath = "C:\\Users\\pilyu\\Desktop\\Отчет.csv";
            FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.SetLength(0); 
            fileStream.Close();
            File.AppendAllText(filePath, "Имя;Фамилия;Вовремя;Просрочено\n", Encoding.UTF8); 
            string str = $"{Employer.FirstName};{Employer.LastName};{Efficiency[0]};{Efficiency[1]}\n";
            File.AppendAllText(filePath, str, Encoding.UTF8); 
            AppShell.Current.DisplayAlert("Генерация", "Файл csv обновлен", "OK");
            Clear();
        }
        
    }
   
}