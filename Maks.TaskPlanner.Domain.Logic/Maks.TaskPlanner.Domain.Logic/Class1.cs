using Maks.TaskPlanner.Domain.Models;

namespace Maks.TaskPlanner.Domain.Logic
{
	public class SimpleTaskPlanner
	{
		public WorkItem[] CreatePlan(WorkItem[] tasks)
		{
			var sortedTasks = tasks
				.OrderByDescending(task => task.Priority)   // Спершу за спаданням пріоритету
				.ThenBy(task => task.DueDate)               // Потім за зростанням дати виконання
				.ThenBy(task => task.Title)                 // за алфавітом
				.ToArray();

			return sortedTasks;
		}
	}
}