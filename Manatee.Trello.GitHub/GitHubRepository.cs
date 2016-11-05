using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.GitHub
{
	public class GitHubRepository : IJsonSerializable
	{
		public string FullName { get; private set; }
		public string Name { get; private set; }
		public string Owner { get; private set; }
		public string OwnerType { get; private set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;
			var obj = json.Object;
			FullName = obj.TryGetString("full_name");
			Name = obj.TryGetString("name");
			Owner = obj.TryGetObject("owner")?.TryGetString("login");
			OwnerType = obj.TryGetObject("owner")?.TryGetString("type");
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new System.NotImplementedException();
		}
	}
}