using Maks.TaskPlanner.DataAccess;
using Maks.TaskPlanner.DataAccess.Abstractions;
using Maks.TaskPlanner.Domain.Logic;
using Maks.TaskPlanner.Domain.Models;
using Maks.TaskPlanner.Domain.Models.Enums;

internal static class Program
{
	public static void Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;

		FileWorkItemsRepository repository = new FileWorkItemsRepository();
		bool keepRunning = true;

		while (keepRunning)
		{
			Console.WriteLine("\nОберіть дію:");
			Console.WriteLine("[A]dd work item");
			Console.WriteLine("[B]uild a plan");
			Console.WriteLine("[M]ark work item as completed");
			Console.WriteLine("[R]emove a work item");
			Console.WriteLine("[Q]uit the app");

			Console.Write("\nВаша відповідь: ");
			string choice = Console.ReadLine()?.ToUpper();

			switch (choice)
			{
				case "A":
					AddWorkItem(repository);
					break;
				case "B":
					BuildPlan(repository);
					break;
				case "M":
					MarkWorkItemAsCompleted(repository);
					break;
				case "R":
					RemoveWorkItem(repository);
					break;
				case "Q":
					keepRunning = false;
					break;
				default:
					Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
					break;
			}
		}
	}

	private static void AddWorkItem(IWorkItemsRepository repository)
	{
		Console.WriteLine("\nВведіть інформацію про новий WorkItem:");

		Console.Write("Title: ");
		string title = Console.ReadLine();

		Console.Write("Description: ");
		string description = Console.ReadLine();

		DateTime creationDate = ReadDate("Creation Date (dd.MM.yyyy): ");
		DateTime dueDate = ReadDate("Due Date (dd.MM.yyyy): ");

		Priority priority = ReadEnum<Priority>("Priority (None, Low, Medium, High, Urgent): ");
		Complexity complexity = ReadEnum<Complexity>("Complexity (None, Minutes, Hours, Days, Weeks): ");

		bool isCompleted = ReadBoolean("Is Completed (true/false): ");

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

		repository.Add(workItem);
		repository.SaveChanges();
		Console.WriteLine("WorkItem додано успішно!");
	}

	private static void BuildPlan(IWorkItemsRepository repository)
	{
		SimpleTaskPlanner planner = new SimpleTaskPlanner(repository);
		WorkItem[] sortedWorkItems = planner.CreatePlan();

		Console.WriteLine("\nВпорядковані WorkItems:");
		foreach (var item in sortedWorkItems)
		{
			Console.WriteLine(item.ToString());
		}
	}

	private static void MarkWorkItemAsCompleted(IWorkItemsRepository repository)
	{
		Console.Write("Введіть ID WorkItem для позначення як завершеного: ");
		if (Guid.TryParse(Console.ReadLine(), out Guid id))
		{
			WorkItem workItem = repository.Get(id);
			if (workItem != null)
			{
				workItem.IsCompleted = true;
				repository.Update(workItem);
				repository.SaveChanges();
				Console.WriteLine("WorkItem позначено як завершений.");
			}
			else
			{
				Console.WriteLine("WorkItem з таким ID не знайдено.");
			}
		}
		else
		{
			Console.WriteLine("Невірний формат ID.");
		}
	}

	private static void RemoveWorkItem(IWorkItemsRepository repository)
	{
		Console.Write("Введіть ID WorkItem для видалення: ");
		if (Guid.TryParse(Console.ReadLine(), out Guid id))
		{
			if (repository.Remove(id))
			{
				repository.SaveChanges();
				Console.WriteLine("WorkItem видалено.");
			}
			else
			{
				Console.WriteLine("WorkItem з таким ID не знайдено.");
			}
		}
		else
		{
			Console.WriteLine("Невірний формат ID.");
		}
	}

	private static DateTime ReadDate(string prompt)
	{
		DateTime date;
		bool validDate = false;
		do
		{
			Console.Write(prompt);
			string input = Console.ReadLine();
			validDate = DateTime.TryParseExact(input, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
			if (!validDate)
				Console.WriteLine("Невірний формат дати. Спробуйте ще раз.");
		} while (!validDate);

		return date;
	}

	private static T ReadEnum<T>(string prompt) where T : struct
	{
		T result;
		bool validEnum = false;
		do
		{
			Console.Write(prompt);
			string input = Console.ReadLine();
			validEnum = Enum.TryParse(input, true, out result) && Enum.IsDefined(typeof(T), result);
			if (!validEnum)
				Console.WriteLine($"Невірне значення. Будь ласка, введіть одне з наступних значень: {string.Join(", ", Enum.GetNames(typeof(T)))}.");
		} while (!validEnum);

		return result;
	}

	private static bool ReadBoolean(string prompt)
	{
		bool result;
		bool validBoolean = false;
		do
		{
			Console.Write(prompt);
			string input = Console.ReadLine();
			validBoolean = bool.TryParse(input, out result);
			if (!validBoolean)
				Console.WriteLine("Невірне значення. Введіть 'true' або 'false'.");
		} while (!validBoolean);

		return result;
	}
}
