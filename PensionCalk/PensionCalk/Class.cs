using Microsoft.VisualBasic;
using System;
using System.ComponentModel.Design.Serialization;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace CalculatorAssistent
{
    public class Job
    {
        public DateTime Date_start { get; set; }
        public DateTime Date_end { get; set; }
        public double Salary { get; set; }
        public Job(DateTime _start, DateTime _end, double _salary)
        { 
            Date_start = _start;
            Date_end = _end;
            Salary = _salary;
        }
    }

    public class Extra
    {
        public DateTime Date_start { get; set; }
        public DateTime Date_end { get; set; }
        public double Years { get; set; }

        public double Months { get; set; }

        public double Days { get; set; }

        public double Salary { get; set; }
        public string S { get; set; }
        public Extra(double years, double months, double days, double _salary, string _s)
        {
            
            Years = years;
            Months = months;
            Days = days;
            Salary = _salary;
            S = _s;
        }
    }


    public class Calculator
    {
        public double all_years { get; set; }
        public double all_months { get; set; }
        public double all_days { get; set; }

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double X3 { get; set; }


        string s_child;
        double salary;
        double years;
        double months;
        double days;
        List<Job> jobs;
        List<Extra> extras;

   

        public Calculator (double _years, double _months, double _days, double _salary, string _s_child, 
           
          List<Job> _jobs, List<Extra> _extras)
           
        {

            years = _years;
            months = _months;
            days = _days;
            salary = _salary;
            jobs = _jobs;
            extras = _extras;
            s_child = _s_child;
        }

        public double Pension { get; set; }


        public bool DoCalc()
        {
            bool res = false;
            try
            {
                

                all_years = years;
                all_months = months;
                all_days = days;

                
                X1 = all_years;
                X2 = all_months;
                X3 = all_days;

                if (X1 != Math.Round(X1)) // если число лет ДРОБНОЕ
                {
                    X1 -= 0.5;
                    X2 += 6;
                }

                if (X2 != Math.Round(X2)) // если число месяцев ДРОБНОЕ
                {
                    X2 -= 0.5;
                    X3 += 15;
                }

                if (X3 != Math.Round(X3)) // если число дней ДРОБНОЕ
                {
                    X3 += 0.5;
                }

                while (X3 > 30)
                {
                    X3 -= 30;
                    X2 += 1;
                }

                while (X2 >= 12)
                {
                    X2 -= 12;
                    X1 += 1;
                }

                while (years > 0)
                {
                    years -= 1;
                    months += 12;
                }

                
                double ipk, ipk_all;
                CalcIpk(salary, salary, years, months, out ipk, out ipk_all);

                foreach ( var job in jobs )
                {
                    double ipk_i, ipk_all_i;
                    CalcIpk(job.Salary, job.Salary, job.Date_start, 
                        job.Date_end, out ipk_i, out ipk_all_i);
                    ipk += ipk_i;
                    ipk_all += ipk_all_i;
                }

                

                double ipk_dop = 0;

                double north_exp = 0;

                foreach (var extra in extras)
                {
                    double ipk_i, ipk_all_i;
                    if (extra.S == "s1")
                    {
                        extra.Years *= 2;
                        extra.Months *= 2;
                        extra.Days *= 2;
                    }
                    else if (extra.S == "s3")
                    {
                        extra.Years *= 1.5;
                        extra.Months *= 1.5;
                        extra.Days *= 1.5;
                    }
                    else if (extra.S == "s4")
                    {
                        extra.Years *= 3;
                        extra.Months *= 3;
                        extra.Days *= 3;
                    }

                    if (extra.S == "s3")
                    {
                        north_exp += extra.Years;
                    }

                    CalcIpk(extra.Salary, extra.Salary, extra.Years,
                        extra.Months, out ipk_i, out ipk_all_i);
                    ipk += ipk_i;
                    ipk_all += ipk_all_i;

                    if (extra.S == "s1" || extra.S == "s2" || extra.S == "s11" ||
                        extra.S == "s12" || extra.S == "s14")
                    {
                        ipk_dop += 1.8 * extra.Years;
                    }

                }


                if (s_child == "1")
                {
                    ipk_dop += 1.8;
                }

                else if (s_child == "2")
                {
                    ipk_dop += 3.6;
                }

                else if (s_child == "3")
                {
                    ipk_dop += 5.4;
                }

                
                ipk_all += ipk_dop;
                double fixed_payout = 7567.33;

                if (north_exp >= 15)
                {
                    fixed_payout *= 1.5;
                }

                Pension = ipk_all * 123.76 + fixed_payout;
                Pension = Math.Round(Pension, 2);

                res = true;
            }
            catch
            {
                res = false;
            }
            return res;
        }

        private void CalcIpk(double salary1, double salary2, double years, double months, out double ipk, out double ipk_all)
        {
            double sv_date = salary1 * 12 - (salary2 * 12 * 0.84);
            double date_years = years + (months / 12);
            ipk = sv_date / 306720 * 10;
            ipk_all = ipk * date_years;
            return;
        }

        private void CalcIpk(double salary1, double salary2, DateTime start, DateTime end, out double ipk, out double ipk_all)
        {
            var date = DateTimeSpan.CompareDates(start, end);
            CalcIpk(salary1, salary2, date.Years, date.Months, out ipk, out ipk_all);
            return;
        }
    }


    public struct DateTimeSpan
    {
        public double Years { get; }
        public double Months { get; }
        public double Days { get; }

        public DateTimeSpan(double years, double months, double days)
        {
            Years = years;
            Months = months;
            Days = days;
        }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            double years = 0;
            double months = 0;
            double days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            double officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears((int)(years + 1)) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears((int)years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths((int)(months + 1)) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths((int)months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }
}
 
