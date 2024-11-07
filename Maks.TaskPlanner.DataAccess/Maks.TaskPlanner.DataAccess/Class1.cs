using Maks.TaskPlanner.DataAccess.Abstractions;
using Maks.TaskPlanner.Domain.Models;
using Newtonsoft.Json;
using System.Xml;

namespace Maks.TaskPlanner.DataAccess
{
	public class FileWorkItemsRepository : IWorkItemsRepository
	{
		private const string FileName = "work-items.json"; // Назва файлу
		private readonly Dictionary<Guid, WorkItem> _workItems; // In-memory словник для зберігання задач

		public FileWorkItemsRepository()
		{
			// Ініціалізація словника з даних файлу work-items.json, якщо файл існує
			if (File.Exists(FileName))
			{
				string json = File.ReadAllText(FileName);

				// Десеріалізація JSON в масив WorkItem
				var items = string.IsNullOrEmpty(json) ?
					new List<WorkItem>() :
					JsonConvert.DeserializeObject<List<WorkItem>>(json);

				// Конвертуємо масив у словник для зберігання в пам'яті
				_workItems = items?.ToDictionary(item => item.Id) ?? new Dictionary<Guid, WorkItem>();
			}
			else
			{
				_workItems = new Dictionary<Guid, WorkItem>();
			}
		}

		public Guid Add(WorkItem workItem)
		{
			// Створення копії workItem та присвоєння нового Guid
			var newWorkItem = workItem.Clone();
			newWorkItem.Id = Guid.NewGuid();

			// Додавання копії у словник
			_workItems[newWorkItem.Id] = newWorkItem;
			return newWorkItem.Id;
		}

		public WorkItem Get(Guid id)
		{
			// Повертає WorkItem за його Id
			return _workItems.TryGetValue(id, out var item) ? item : null;
		}

		public WorkItem[] GetAll()
		{
			// Повертає масив усіх задач
			return new List<WorkItem>(_workItems.Values).ToArray();
		}

		public bool Update(WorkItem workItem)
		{
			// Оновлює WorkItem, якщо такий Id існує
			if (!_workItems.ContainsKey(workItem.Id))
				return false;

			_workItems[workItem.Id] = workItem;
			return true;
		}

		public bool Remove(Guid id)
		{
			// Видаляє WorkItem за Id, якщо він існує
			return _workItems.Remove(id);
		}

		public void SaveChanges()
		{
			// Перетворює словник у масив та серіалізує його у JSON, записуючи в файл
			var items = new List<WorkItem>(_workItems.Values);
			string json = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(FileName, json);
		}
	}
}