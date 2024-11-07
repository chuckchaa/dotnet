using Maks.TaskPlanner.Domain.Models;
using Maks.TaskPlanner.DataAccess.Abstractions;
using Moq;


namespace Maks.TaskPlanner.Domain.Logic.Tests
{
	[TestFixture] // ������� ��� ��������� ����� � NUnit
	public class SimpleTaskPlannerTests
	{
		[Test] // ������������� ������� Test ��� NUnit
		public void CreatePlan_ShouldSortTasksCorrectly()
		{
			// Arrange: ��������� ��� IWorkItemsRepository
			var mockRepo = new Mock<IWorkItemsRepository>();

			var tasks = new WorkItem[]
			{
				new WorkItem { Id = Guid.NewGuid(), Title = "Task A", DueDate = DateTime.Now.AddDays(2), Priority = Models.Enums.Priority.High, IsCompleted = false },
				new WorkItem { Id = Guid.NewGuid(), Title = "Task B", DueDate = DateTime.Now.AddDays(1), Priority = Models.Enums.Priority.Low, IsCompleted = false },
				new WorkItem { Id = Guid.NewGuid(), Title = "Task C", DueDate = DateTime.Now.AddDays(3), Priority = Models.Enums.Priority.Medium, IsCompleted = false }
			};

			mockRepo.Setup(repo => repo.GetAll()).Returns(tasks);

			var planner = new SimpleTaskPlanner(mockRepo.Object);

			// Act: ��������� ����� CreatePlan
			var result = planner.CreatePlan();

			// Assert: ����������, �� �������� ���������� �� ���������� (�� ���������), ���� �� �����
			Assert.AreEqual("Task A", result[0].Title); // �������� ��������
			Assert.AreEqual("Task C", result[1].Title); // �������� �������
			Assert.AreEqual("Task B", result[2].Title); // �������� ���������
		}

		[Test]
		public void CreatePlan_ShouldIncludeOnlyUncompletedTasks()
		{
			// Arrange: ��������� ��� IWorkItemsRepository
			var mockRepo = new Mock<IWorkItemsRepository>();

			var tasks = new WorkItem[]
			{
				new WorkItem { Id = Guid.NewGuid(), Title = "Task A", DueDate = DateTime.Now.AddDays(2), Priority = Models.Enums.Priority.High, IsCompleted = false },
				new WorkItem { Id = Guid.NewGuid(), Title = "Task B", DueDate = DateTime.Now.AddDays(1), Priority = Models.Enums.Priority.Low, IsCompleted = true },  // ��������� ������
                new WorkItem { Id = Guid.NewGuid(), Title = "Task C", DueDate = DateTime.Now.AddDays(3), Priority = Models.Enums.Priority.Medium, IsCompleted = false }
			};

			mockRepo.Setup(repo => repo.GetAll()).Returns(tasks);

			var planner = new SimpleTaskPlanner(mockRepo.Object);

			// Act: ��������� ����� CreatePlan
			var result = planner.CreatePlan();

			// Assert: ����������, �� � ���� ���� ���������� �����
			Assert.IsFalse(result.Any(task => task.Title == "Task B" && task.IsCompleted));
			Assert.AreEqual(2, result.Length); // ���� 2 ������������ ������
		}

		[Test]
		public void CreatePlan_ShouldNotIncludeCompletedTasks()
		{
			// Arrange: ��������� ��� IWorkItemsRepository
			var mockRepo = new Mock<IWorkItemsRepository>();

			var tasks = new WorkItem[]
			{
				new WorkItem { Id = Guid.NewGuid(), Title = "Task A", DueDate = DateTime.Now.AddDays(2), Priority = Models.Enums.Priority.High, IsCompleted = true },  // ��������� ������
                new WorkItem { Id = Guid.NewGuid(), Title = "Task B", DueDate = DateTime.Now.AddDays(1), Priority = Models.Enums.Priority.Low, IsCompleted = true },  // ��������� ������
            };

			mockRepo.Setup(repo => repo.GetAll()).Returns(tasks);

			var planner = new SimpleTaskPlanner(mockRepo.Object);

			// Act: ��������� ����� CreatePlan
			var result = planner.CreatePlan();

			// Assert: ����������, �� ��������� �� ������ ����� ��������� ������
			Assert.IsEmpty(result); // ���� 0 ������������ �����
		}
	}
}