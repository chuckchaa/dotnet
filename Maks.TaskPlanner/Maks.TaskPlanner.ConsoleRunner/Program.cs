using Maks.TaskPlanner.Domain.Logic;
using Maks.TaskPlanner.Domain.Models;
using Maks.TaskPlanner.Domain.Models.Enums;

internal static class Program
{
	public static void Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;

		List<WorkItem> workItems = new List<WorkItem>();
		bool addMoreItems = true;

		// Введення користувачем WorkItem через консольний інтерфейс
		while (addMoreItems)
		{
			Console.WriteLine("Введіть інформацію про новий WorkItem:");

			
			Console.Write("Title: ");
			string title = Console.ReadLine();

		
			Console.Write("Description: ");
			string description = Console.ReadLine();

	
			DateTime creationDate = DateTime.MinValue;
			bool validCreationDate = false;

			while (!validCreationDate)
			{
				Console.Write("Creation Date (dd.MM.yyyy): ");
				string creationDateInput = Console.ReadLine();

		
				if (string.IsNullOrEmpty(creationDateInput))
				{
					Console.WriteLine("Введення не може бути порожнім. Спробуйте ще раз.");
					continue;
				}

				try
				{
					creationDate = DateTime.ParseExact(creationDateInput, "dd.MM.yyyy",
						System.Globalization.CultureInfo.InvariantCulture);
					validCreationDate = true;
				}
				catch (FormatException)
				{
					Console.WriteLine("Невірний формат дати. Спробуйте ще раз.");
				}
			}

			
			DateTime dueDate = DateTime.MinValue; 
			bool validDueDate = false;

			while (!validDueDate)
			{
				Console.Write("Due Date (dd.MM.yyyy): ");
				string dueDateInput = Console.ReadLine();

				
				if (string.IsNullOrEmpty(dueDateInput))
				{
					Console.WriteLine("Введення не може бути порожнім. Спробуйте ще раз.");
					continue; 
				}

				
				if (DateTime.TryParseExact(dueDateInput, "dd.MM.yyyy",
					System.Globalization.CultureInfo.InvariantCulture,
					System.Globalization.DateTimeStyles.None, out dueDate))
				{
					validDueDate = true;
				}
				else
				{
					Console.WriteLine("Невірний формат дати. Спробуйте ще раз."); 
				}
			}

		
			bool validPriority = false;
			Priority priority = Priority.None; 

			while (!validPriority)
			{
				try
				{
					Console.Write("Priority (None, Low, Medium, High, Urgent): ");
					priority = Enum.Parse<Priority>(Console.ReadLine(), ignoreCase: true);
					validPriority = true; 
				}
				catch (ArgumentException)
				{
					Console.WriteLine("Невірний пріоритет. Будь ласка, введіть одне з наступних значень: None, Low, Medium, High, Urgent.");
				}
			}

			
			bool validComplexity = false;
			Complexity complexity = Complexity.None; 

			while (!validComplexity)
			{
				try
				{
					Console.Write("Complexity (None, Minutes, Hours, Days, Weeks): ");
					complexity = Enum.Parse<Complexity>(Console.ReadLine(), ignoreCase: true);
					validComplexity = true; 
				}
				catch (ArgumentException)
				{
					Console.WriteLine("Невірна складність. Будь ласка, введіть одне з наступних значень: None, Minutes, Hours, Days, Weeks.");
				}
			}

			bool isCompleted = false; 
			bool validIsCompleted = false;

			while (!validIsCompleted)
			{
				Console.Write("Is Completed (true/false): ");
				string isCompletedInput = Console.ReadLine();

				
				if (string.IsNullOrEmpty(isCompletedInput))
				{
					Console.WriteLine("Введення не може бути порожнім. Спробуйте ще раз.");
					continue; 
				}

				if (bool.TryParse(isCompletedInput, out isCompleted))
				{
					validIsCompleted = true; 
				}
				else
				{
					Console.WriteLine("Невірне введення. Спробуйте ще раз. Введіть 'true' або 'false'."); 
				}
			}

			
			WorkItem workItem = new WorkItem
			{
				Title = title,
				Description = description,
				CreationDate = creationDate,
				DueDate = dueDate,
				Priority = priority,
				Complexity = complexity,
				IsCompleted = isCompleted
			};

			workItems.Add(workItem);

			
			Console.Write("Додати ще один WorkItem? (yes/no): ");
			string response = Console.ReadLine().ToLower();
			addMoreItems = response == "yes";
		}

	
		SimpleTaskPlanner planner = new SimpleTaskPlanner();
		WorkItem[] sortedWorkItems = planner.CreatePlan(workItems.ToArray());

		
		Console.WriteLine("\nВпорядковані WorkItems:");
		foreach (var item in sortedWorkItems)
		{
			Console.WriteLine(item.ToString());
		}
	}
}