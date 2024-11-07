using Maks.TaskPlanner.Domain.Models;
using Maks.TaskPlanner.DataAccess.Abstractions;
using Moq;


namespace Maks.TaskPlanner.Domain.Logic.Tests
{
	[TestFixture] // Атрибут для тестового класу в NUnit
	public class SimpleTaskPlannerTests
	{
		[Test] // Використовуємо атрибут Test для NUnit
		public void CreatePlan_ShouldSortTasksCorrectly()
		{
			// Arrange: створюємо мок IWorkItemsRepository
			var mockRepo = new Mock<IWorkItemsRepository>();

			var tasks = new WorkItem[]
			{
				new WorkItem { Id = Guid.NewGuid(), Title = "Task A", DueDate = DateTime.Now.AddDays(2), Priority = Models.Enums.Priority.High, IsCompleted = false },
				new WorkItem { Id = Guid.NewGuid(), Title = "Task B", DueDate = DateTime.Now.AddDays(1), Priority = Models.Enums.Priority.Low, IsCompleted = false },
				new WorkItem { Id = Guid.NewGuid(), Title = "Task C", DueDate = DateTime.Now.AddDays(3), Priority = Models.Enums.Priority.Medium, IsCompleted = false }
			};

			mockRepo.Setup(repo => repo.GetAll()).Returns(tasks);

			var planner = new SimpleTaskPlanner(mockRepo.Object);

			// Act: викликаємо метод CreatePlan
			var result = planner.CreatePlan();

			// Assert: перевіряємо, що завдання відсортовані за пріоритетом (за спаданням), потім за датою
			Assert.AreEqual("Task A", result[0].Title); // Пріоритет найвищий
			Assert.AreEqual("Task C", result[1].Title); // Пріоритет середній
			Assert.AreEqual("Task B", result[2].Title); // Пріоритет найнижчий
		}

		[Test]
		public void CreatePlan_ShouldIncludeOnlyUncompletedTasks()
		{
			// Arrange: створюємо мок IWorkItemsRepository
			var mockRepo = new Mock<IWorkItemsRepository>();

			var tasks = new WorkItem[]
			{
				new WorkItem { Id = Guid.NewGuid(), Title = "Task A", DueDate = DateTime.Now.AddDays(2), Priority = Models.Enums.Priority.High, IsCompleted = false },
				new WorkItem { Id = Guid.NewGuid(), Title = "Task B", DueDate = DateTime.Now.AddDays(1), Priority = Models.Enums.Priority.Low, IsCompleted = true },  // Завершена задача
                new WorkItem { Id = Guid.NewGuid(), Title = "Task C", DueDate = DateTime.Now.AddDays(3), Priority = Models.Enums.Priority.Medium, IsCompleted = false }
			};

			mockRepo.Setup(repo => repo.GetAll()).Returns(tasks);

			var planner = new SimpleTaskPlanner(mockRepo.Object);

			// Act: викликаємо метод CreatePlan
			var result = planner.CreatePlan();

			// Assert: перевіряємо, що в плані немає завершених задач
			Assert.IsFalse(result.Any(task => task.Title == "Task B" && task.IsCompleted));
			Assert.AreEqual(2, result.Length); // Маємо 2 незавершених задачі
		}

		[Test]
		public void CreatePlan_ShouldNotIncludeCompletedTasks()
		{
			// Arrange: створюємо мок IWorkItemsRepository
			var mockRepo = new Mock<IWorkItemsRepository>();

			var tasks = new WorkItem[]
			{
				new WorkItem { Id = Guid.NewGuid(), Title = "Task A", DueDate = DateTime.Now.AddDays(2), Priority = Models.Enums.Priority.High, IsCompleted = true },  // Завершена задача
                new WorkItem { Id = Guid.NewGuid(), Title = "Task B", DueDate = DateTime.Now.AddDays(1), Priority = Models.Enums.Priority.Low, IsCompleted = true },  // Завершена задача
            };

			mockRepo.Setup(repo => repo.GetAll()).Returns(tasks);

			var planner = new SimpleTaskPlanner(mockRepo.Object);

			// Act: викликаємо метод CreatePlan
			var result = planner.CreatePlan();

			// Assert: перевіряємо, що результат не містить жодної завершеної задачі
			Assert.IsEmpty(result); // Маємо 0 незавершених задач
		}
	}
}