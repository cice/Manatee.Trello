using System.Collections.Generic;
using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.GitHub
{
	internal class GitHubSettings : IJsonSerializable
	{
		public IEnumerable<GitHubRepository> Repositories { get; private set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;
			Repositories = serializer.Deserialize<List<GitHubRepository>>(json.Object["repos"]);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new System.NotImplementedException();
		}
	}
}