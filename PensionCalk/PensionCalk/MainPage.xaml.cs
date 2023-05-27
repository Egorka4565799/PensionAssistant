namespace PensionCalk;
using CalculatorAssistent;

public partial class MainPage : ContentPage
{
    Calculator calc;
    double Prognoz_Pension;

    public MainPage()
	{
		InitializeComponent();
	}

	void DateSelectedPeriod()
	{

		List<Job> Jobs = new List<Job>(); 

		for(int i = 0; i<3; i++)
		{
            try
            {
                DateTime startDate = DatePeriodStart1.Date;
                DateTime endDate = DatePeriodEnd1.Date;
                double salary = Double.Parse(SalaryPeriod1.Text);

                Job job = new Job(startDate, endDate, salary);
                Jobs.Add(job);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при преобразовании строки в double или другие исключения
                Console.WriteLine("Ошибка: " + ex.Message);
            }

        }

        //-------------

        List<Extra> extras = new List<Extra>();

        for (int i = 0; i<1; i++)
        {
            try
            {
                double Age = Double.Parse(OldMilitary1.Text);
                double Mount = Double.Parse(MounMilitary1.Text);
                double Day = Double.Parse(DayMilitary1.Text);
                double salary = Double.Parse(SalaryMilitary1.Text);

                int select = SelectMilitary1.SelectedIndex;
                String s ='s'+ select.ToString();
               

                Extra extra = new Extra(Age, Mount, Day, salary, s);
                extras.Add(extra);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при преобразовании строки в double или другие исключения
                Console.WriteLine("Ошибка: " + ex.Message);
            }

        }

        //-------------
        String child_s =  Children.SelectedIndex.ToString();

        double ages = Double.Parse(PeriodAge.Text);
        double mounts = Double.Parse(PeriodM.Text);
        double days = Double.Parse(PeriodD.Text);
        double all_salary = Double.Parse(PeriodS.Text);


        calc = new Calculator(ages, mounts, days, all_salary, child_s, Jobs, extras);
    }

  

    void OnPensionClicked(object sender, EventArgs e)
	{
        DateSelectedPeriod();

        if (calc.DoCalc())
        {
            Prognoz_Pension = calc.Pension;

            ResultAge.Text = $"Ваш стаж: Лет={calc.X1} Месяцев={calc.X2} Дней={calc.X1}";
            ResultPension.Text = $"Ваша пенсия: {Prognoz_Pension}";
        }

    }
}

