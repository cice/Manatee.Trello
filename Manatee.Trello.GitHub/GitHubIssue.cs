using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.GitHub
{
	public class GitHubIssue : IJsonSerializable
	{
		void IJsonSerializable.FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new System.NotImplementedException();
		}
		JsonValue IJsonSerializable.ToJson(JsonSerializer serializer)
		{
			throw new System.NotImplementedException();
		}
	}
}