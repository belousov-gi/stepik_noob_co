using System;
using System.Collections.Generic;
using System.Linq;
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
        string inputString;
        
        //by default 4
        int amountOfDepartments = 4;

        int numberOfAntyCrisisSolution = int.Parse(Console.ReadLine() ?? throw new Exception("Invalid number of solution"));
        
        string[] listOfInputStrings = new string[amountOfDepartments];

        for (int i = 0; i < amountOfDepartments; i++)
        {
            inputString = Console.ReadLine();
            listOfInputStrings[i] = inputString;
        }
        Report.PrintReport(listOfInputStrings, numberOfAntyCrisisSolution);
    }


    class Employee
    {
        public static Employee CreateEmployee(int employeeRank, bool isEmployeeTeamlead, string typeEmployee, int antyCrisisSolution)
        {
            Employee employee =
                (typeEmployee) == "manager" ? new Manager(employeeRank, isEmployeeTeamlead) :
                (typeEmployee) == "marketer" ? new Marketer(employeeRank, isEmployeeTeamlead) :
                (typeEmployee) == "engineer" ? new Engineer(employeeRank, isEmployeeTeamlead) :
                (typeEmployee) == "analyst" ? new Analyst(employeeRank, isEmployeeTeamlead, antyCrisisSolution) :
                throw new Exception($"Invalid type of employee: {typeEmployee}");
            return employee;
        }
        
        private int coffeeConsumption;
        private int defaultCoffeeConsumption;
        private int sheetsOutcome;
        private int salary;
        private int defaultSalary;
        private int rank;
        private bool isTeamLead;
        public int Rank
        {
            get => rank;
            set
            {
                if (value <= 1){ rank = 1;}
                else if (value <= 3) { rank = value; }
                else { rank = 3;}
                CurrentSalary = DefaultSalary;
            } 
        }

        public bool IsTeamlead
        {
            get => isTeamLead;
            set
            {
                isTeamLead = value;
                CurrentSalary = DefaultSalary;
                CurrentCoffeeConsumption = DefaultCoffeeConsumption;
                CurrentSheetsOutcome = DefaultSheetsOutcome;
            }
        }
        public Enum Type { get;}

        private protected int DefaultSalary
        {
            get => defaultSalary;
            init
            {
                defaultSalary = value;
                CurrentSalary = value;
            }
        }

        public int CurrentSalary
        {
            get => salary;
            private set
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

        private protected int DefaultCoffeeConsumption
        {
            get => defaultCoffeeConsumption;
            init
            {
                defaultCoffeeConsumption = value;
                CurrentCoffeeConsumption = value;
            }
        }

        public int CurrentCoffeeConsumption 
        {
            get => coffeeConsumption;
            private protected set => coffeeConsumption = IsTeamlead ? value * 2 : value;
        }

        private protected int DefaultSheetsOutcome { get; init; }

        public int CurrentSheetsOutcome 
        {
            get => sheetsOutcome;
            private protected set => sheetsOutcome = IsTeamlead ? 0 : value;
           
            
        }
    
        public Employee(int rank, bool isTeamlead, int defaultSalary, int defaultCoffeeConsumption,  int defaultSheetOutcome, Enum type)
        {
            DefaultSalary = defaultSalary;
            DefaultCoffeeConsumption = defaultCoffeeConsumption;
            DefaultSheetsOutcome = defaultSheetOutcome;
            Rank = rank;
            IsTeamlead = isTeamlead;
            Type = type;
        }

        public enum TypeOfEmployeers
        {
            Manager,
            Marketer,
            Engineer,
            Analyst,
        }
    }
    
    class Manager : Employee
    {
        public Manager(int rank, bool isTeamlead) : base(rank, isTeamlead, 50000, 20, 200, TypeOfEmployeers.Manager ){}
    }
    
    class Marketer : Employee
    {
        public Marketer(int rank, bool isTeamlead) : base(rank, isTeamlead, 40000, 15, 150, TypeOfEmployeers.Marketer) {}
    }
    
    class Engineer : Employee
    {
        public Engineer(int rank, bool isTeamlead) : base(rank, isTeamlead, 20000, 5, 50, TypeOfEmployeers.Engineer) {}
    }
    
    class Analyst : Employee
    {
        public Analyst(int rank, bool isTeamlead, int antyCrisisSolution) : 
               base(rank, isTeamlead, 80000, 50, 5, TypeOfEmployeers.Analyst)
        {
            if (antyCrisisSolution == 2)
            {
                DefaultSalary = 110000;
                DefaultCoffeeConsumption = 75;
            }
        }
    }
    
    class Report
    {
        public int AmountEmploeeys { get; set; }
        public int WholeSalary { get; private protected set; }
        public int WholeCoffeeConsumption { get; private protected set; }
        public int WholePages { get; private protected set; }
        public double CostOfOnePage { get; init; }
    
        public static void PrintReport(string[] listOfInputStrings, int antyCrisisSolution)
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
                DepartmentInfo infoForDeparment = GetInfoForDeparment(department, antyCrisisSolution);
                
                //To upper first char in department
                string departmentName = infoForDeparment.Department;
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
        public static DepartmentInfo GetInfoForDeparment(string inputString, int antyCrisisSolution)
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
                
                //Condition for case using "manager2" instead "1*manager2", when digit isn't existing before type of employee
                int amountOfEmployeesInGroup;
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
                    employee = Employee.CreateEmployee(employeeRank, isEmployeeTeamlead, typeEmployee, antyCrisisSolution);
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
    
            var teamLead = Employee.CreateEmployee(employeeRank, isEmployeeTeamlead, typeEmployee, antyCrisisSolution);
            if (teamLead != null)
            {
                allEmployees.Add(teamLead);
            }
            
            // logic with anty-crisis solutions
            if (antyCrisisSolution == 1)
            {
                List<Employee> engineersList = allEmployees.FindAll(x=> Employee.TypeOfEmployeers.Engineer.Equals(x.Type) );
                allEmployees.RemoveAll(x => Employee.TypeOfEmployeers.Engineer.Equals(x.Type));
                
                engineersList = engineersList.OrderBy(x=>x.Rank).ToList();
                int amountOfEngineersForFiring = (int)Math.Ceiling(engineersList.Count * 0.4);

                for (int counter = 0, index = 0, lenghtArray = engineersList.Count; counter < amountOfEngineersForFiring && index < lenghtArray; counter++)
                {
                    if (!engineersList[index].IsTeamlead)
                    {
                        engineersList.RemoveAt(index); 
                    }
                    else
                    {
                        counter--;
                        index++;
                    }
                }
                allEmployees.AddRange(engineersList);
            }

            else if (antyCrisisSolution == 2)
            {
                if (!teamLead.Type.Equals(Employee.TypeOfEmployeers.Analyst))
                {
                    List<Employee> analystList = allEmployees.FindAll(x=> Employee.TypeOfEmployeers.Analyst.Equals(x.Type) );

                    if (analystList.Count > 0)
                    {
                        allEmployees.RemoveAll(x => Employee.TypeOfEmployeers.Analyst.Equals(x.Type));
                        analystList = analystList.OrderBy(x=>x.Rank).ToList();
                        var analystTeamlead = analystList[analystList.Count - 1];
                        analystTeamlead.IsTeamlead = true;
                        teamLead.IsTeamlead = false;
                        allEmployees.AddRange(analystList);
                    }
                }
            }
            
            else if (antyCrisisSolution == 3)
            {
                List<Employee> managersList = allEmployees.FindAll(x=> Employee.TypeOfEmployeers.Manager.Equals(x.Type) );
                allEmployees.RemoveAll(x => Employee.TypeOfEmployeers.Manager.Equals(x.Type));
                
                managersList = managersList.OrderBy(x=>x.Rank).ToList();
                
                var managersFirstRank = managersList.FindAll(x=> x.Rank == 1 );
                managersList.RemoveAll(x=> x.Rank == 1);
                
                var managersSecondRank = managersList.FindAll(x=> x.Rank == 2 );
                managersList.RemoveAll(x=> x.Rank == 2);
                
                
                List<List<Employee>> wholeListForRising = new List<List<Employee>>(){managersFirstRank, managersSecondRank};
                int amountOfManagersForRising;

                foreach (var groupOfManagers in wholeListForRising)
                {
                    amountOfManagersForRising = (int)Math.Ceiling(groupOfManagers.Count * 0.5);
                    for (int i = 0; i < amountOfManagersForRising; i++)
                    {
                        groupOfManagers[i].Rank++;
                    }
                    managersList.AddRange(groupOfManagers);
                }
                
                allEmployees.AddRange(managersList);
            }

            
            DepartmentInfo info = new DepartmentInfo(allEmployees, department);
            return info;
        }
    }
    
    class DepartmentInfo : Report
    {
        public string Department { get;}
    
        public DepartmentInfo(List<Employee> listOfEmployees, string deparment)
        {
            Department = deparment;
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
                WholeSalary += employee.CurrentSalary;
                WholeCoffeeConsumption += employee.CurrentCoffeeConsumption;
                WholePages += employee.CurrentSheetsOutcome;
            }
        }
    }
    
    class GeneralInfo : Report
    {
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

    

 
    


