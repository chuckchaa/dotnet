using Maks.TaskPlanner.Domain.Models;
using Maks.TaskPlanner.DataAccess.Abstractions;

namespace Maks.TaskPlanner.Domain.Logic
{
	public class SimpleTaskPlanner
	{
		private readonly IWorkItemsRepository _repository;

		// Конструктор приймає залежність від IWorkItemsRepository
		public SimpleTaskPlanner(IWorkItemsRepository repository)
		{
			_repository = repository;
		}

		public WorkItem[] CreatePlan()
		{
			// Отримуємо задачі з репозиторію, ігноруючи виконані
			var tasks = _repository.GetAll()
				.Where(task => !task.IsCompleted); // Фільтр для виключення завершених задач

			var sortedTasks = tasks
				.OrderByDescending(task => task.Priority)   // Спершу за спаданням пріоритету
				.ThenBy(task => task.DueDate)               // Потім за зростанням дати виконання
				.ThenBy(task => task.Title)                 // за алфавітом
				.ToArray();

			return sortedTasks;
		}
	}
}