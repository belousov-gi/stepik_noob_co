using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

class Program
{
    static void Main()
    {
        string[] listOfInputStrings = new string[4];
        string inputString;
        
        //amount of departments - 4
        for (int i = 0; i < 4; i++)
        {
            inputString = Console.ReadLine();
            listOfInputStrings[i] = inputString;
        }
        Report.PrintReport(listOfInputStrings);

    }


    class Employee
    {
        public static Employee CreateEmployee(int employeeRank, bool isEmployeeTeamlead, string typeEmployee)
        {
            Employee employee = (typeEmployee) == "manager" ? new Manager(employeeRank, isEmployeeTeamlead, typeEmployee) :
                (typeEmployee) == "marketer" ? new Marketer(employeeRank, isEmployeeTeamlead, typeEmployee) :
                (typeEmployee) == "engineer" ? new Engineer(employeeRank, isEmployeeTeamlead, typeEmployee) :
                (typeEmployee) == "analyst" ? new Analyst(employeeRank, isEmployeeTeamlead, typeEmployee) : null;
            return employee;
        }
    
        private int coffeeConsumption;
        private int sheetsOutcome;
        private int salary;
        public int Rank { get;  init; }
        public bool IsTeamlead { get;  init; }
        public string Department { get;  init; }
    
        public int Salary
        {
            get => salary;
            init
            {
                switch (Rank)
                {
                    case (1):
                        salary = value;
                        break;
                    case (2):
                        salary = (int)Math.Round(value * 1.25);
                        break;
                    case (3):
                        salary = (int)Math.Round(value * 1.50);
                        break;
                    default:
                        salary = value;
                        break;
                }
                salary = IsTeamlead ? (int)Math.Round(salary * 1.5) : salary;
            }
        }
        
        public int CoffeeConsumption 
        {
            get => coffeeConsumption;
            init => coffeeConsumption = IsTeamlead ? value * 2 : value;
        }
    
        public int SheetsOutcome 
        {
            get => sheetsOutcome;
            init => sheetsOutcome = IsTeamlead ? 0 : value;
        }
    
        public Employee(int rank, bool isTeamlead, int salary, int coffeeConsumption,  int sheetOutcome, string department)
        {
            Rank = rank;
            IsTeamlead = isTeamlead;
            Salary = salary;
            CoffeeConsumption = coffeeConsumption;
            SheetsOutcome = sheetOutcome;
            Department = department;
        }
    }
    
    class Manager : Employee
    {
        public Manager(int rank, bool isTeamlead, string department) : base(rank, isTeamlead, 50000, 20, 200, department){}
    }
    
    class Marketer : Employee
    {
        public Marketer(int rank, bool isTeamlead, string department) : base(rank, isTeamlead, 40000, 15, 150, department) {}
    }
    
    class Engineer : Employee
    {
        public Engineer(int rank, bool isTeamlead, string department) : base(rank, isTeamlead, 20000, 5, 50, department) {}
    }
    
    class Analyst : Employee
    {
        public Analyst(int rank, bool isTeamlead, string department) : base(rank, isTeamlead, 80000, 50, 5, department) {}
    }
    
    class Report
    {
        public int AmountEmploeeys { get; init; }
        public int WholeSalary { get; private protected set; }
        public int WholeCoffeeConsumption { get; private protected set; }
        public int WholePages { get; private protected set; }
        public double CostOfOnePage { get; init; }
    
        public static void PrintReport(string[] listOfInputStrings)
        {
            string header = "Департамент     Сотрудников     Тугрики     Кофе     Страницы     Тугр./стр.";
            StringBuilder consoleHeader = new StringBuilder(header);
            StringBuilder dottedLine = new StringBuilder("").Append('-', consoleHeader.Length);
            List<DepartmentInfo> listOfDepartmentInfo = new List<DepartmentInfo>(4);
            List<string> infoAboutDepartments = new List<string>(4);
            StringBuilder consoleString = new StringBuilder();
    
            //collect info about departments
            foreach (var department in listOfInputStrings)
            {
                DepartmentInfo infoForDeparment = Report.GetInfoForDeparment(department);
                
                //To upper first char in department
                string departmentName = infoForDeparment.Deparment.ToString();
                departmentName = Regex.Replace(departmentName, "^[А-Яа-яЁё]", c => c.Value.ToUpper());
               
                consoleString.Clear();
                // stepik doesn't allow to use insert
                consoleString.Append(departmentName)
                    .Append(' ', 16 - consoleString.Length)
                    .Append(infoForDeparment.AmountEmploeeys)
                    .Append(' ', 32 - consoleString.Length)
                    .Append(infoForDeparment.WholeSalary)
                    .Append(' ', 44 - consoleString.Length)
                    .Append(infoForDeparment.WholeCoffeeConsumption)
                    .Append(' ', 53 - consoleString.Length)
                    .Append(infoForDeparment.WholePages)
                    .Append(' ', 66 - consoleString.Length)
                    .Append(infoForDeparment.CostOfOnePage);
    
                infoAboutDepartments.Add(consoleString.ToString());
                listOfDepartmentInfo.Add(infoForDeparment);
            }
            
            //prepare general info about departments
            GeneralInfo generalInfo = new GeneralInfo(listOfDepartmentInfo);
            consoleString = new StringBuilder();
            consoleString.Append("Всего")
                .Append(' ', 16 - consoleString.Length)
                .Append(generalInfo.AmountEmploeeys)
                .Append(' ', 32 - consoleString.Length)
                .Append(generalInfo.WholeSalary)
                .Append(' ', 44 - consoleString.Length)
                .Append(generalInfo.WholeCoffeeConsumption)
                .Append(' ', 53 - consoleString.Length)
                .Append(generalInfo.WholePages)
                .Append(' ', 66 - consoleString.Length)
                .Append(generalInfo.MeanCostOfOnePage);
            
            //print all info to console
            Console.WriteLine(consoleHeader);
            Console.WriteLine(dottedLine);
            foreach (var consoleStr in infoAboutDepartments)
            {
                Console.WriteLine(consoleStr);   
            }
            Console.WriteLine(dottedLine);
            Console.WriteLine(consoleString);
            
        }
        public static DepartmentInfo GetInfoForDeparment(string inputString)
        { 
            List<Employee> allEmployees = new List<Employee>();
            Employee employee;
            int employeeRank;
            bool isEmployeeTeamlead;
            string typeEmployee;
            
            //find department
            string patternDepartment = @"(?<=Департамент\s)[А-яёЁ0-9]+"; 
            
            //find empployeers except teamlead
            string patternGroupOfEmployees = @"(?<!руководитель департамента)\s([\*a-zA-Z\d]+)";
            
            //find teamlead
            string patternTeamlead = @"(?<=руководитель департамента)\s([\*a-zA-Z\d]+)";
            
            var department = Regex.Matches(inputString, patternDepartment)[0].Value;
            var employees = Regex.Matches(inputString, patternGroupOfEmployees);
            var teamlead = Regex.Matches(inputString, patternTeamlead);
            
            
            //parse info about particular group of employees: amount, type, rank 
            string patternInfoEmployee = @"(\d*)[*]*([a-zA-Z]+)(\d)";
    
            for (int i = 0, count = employees.Count; i < count; i++)
            {
                // get all info about this group of employees
                var employeeInfo = Regex.Match(employees[i].Value, patternInfoEmployee).Groups;
                int amountOfEmployeesInGroup;
    
                //Condition for case using "manager2" instead "1*manager2", when digit doesn't exist before type of employee
                if (employeeInfo[1].Value == "")
                {
                    amountOfEmployeesInGroup = 1;
                }
                else
                {
                    amountOfEmployeesInGroup = int.Parse(employeeInfo[1].Value);
                }
                
                employeeRank = int.Parse(employeeInfo[3].Value);
                typeEmployee = employeeInfo[2].Value;
                isEmployeeTeamlead = false;
              
    
    
                for (int j = 1; j <= amountOfEmployeesInGroup; j++)
                {
                    employee = Employee.CreateEmployee(employeeRank, isEmployeeTeamlead, typeEmployee);
                    
                    if (employee != null)
                    {
                        allEmployees.Add(employee);
                    }
                }
            }
            
            //add teamlead to list
            string patternInfoTeamlead = @"([a-zA-Z]+)(\d)";
            var teamleadInfo = Regex.Match(teamlead[0].Value, patternInfoTeamlead).Groups;
            employeeRank = int.Parse(teamleadInfo[2].Value);
            typeEmployee = teamleadInfo[1].Value;
            isEmployeeTeamlead = true;
    
            employee = Employee.CreateEmployee(employeeRank, isEmployeeTeamlead, typeEmployee);
            if (employee != null)
            {
                allEmployees.Add(employee);
            }
    
            DepartmentInfo report = new DepartmentInfo(allEmployees, department);
            return report;
        }
    }
    
    class DepartmentInfo : Report
    {
        public string Deparment { get; init; }
    
        public DepartmentInfo(List<Employee> listOfEmployees, string deparment)
        {
            Deparment = deparment;
            AmountEmploeeys = listOfEmployees.Count;
            WholeSalary = 0;
            WholeCoffeeConsumption = 0;
            WholePages = 0;
            CalculateSalaryCoffeePages(listOfEmployees);
            CostOfOnePage = Math.Round(WholeSalary / (double)WholePages, 2, MidpointRounding.AwayFromZero);
        }
    
        private void CalculateSalaryCoffeePages(List<Employee> listOfEmployees)
        {
            foreach (var employee in listOfEmployees)
            {
                WholeSalary += employee.Salary;
                WholeCoffeeConsumption += employee.CoffeeConsumption;
                WholePages += employee.SheetsOutcome;
            }
        }
    }
    
    class GeneralInfo : Report
    {
        public int AmountEmploeeys { get; private set; }
        public double MeanCostOfOnePage { get; private set; }
        
        public GeneralInfo(List<DepartmentInfo> listOfDepartmensInfo)
        {
            CalculateReportParametrs(listOfDepartmensInfo);
        }
    
        private void CalculateReportParametrs(List<DepartmentInfo> listOfDepartmensInfo)
        {
            foreach (var departmentInfo in listOfDepartmensInfo)
            {
                AmountEmploeeys += departmentInfo.AmountEmploeeys;
                WholeSalary += departmentInfo.WholeSalary;
                WholeCoffeeConsumption += departmentInfo.WholeCoffeeConsumption;
                WholePages += departmentInfo.WholePages;
            }
    
            MeanCostOfOnePage = Math.Round(WholeSalary / (double)WholePages, 2, MidpointRounding.AwayFromZero);
        }
        
    }
}

    

 
    


