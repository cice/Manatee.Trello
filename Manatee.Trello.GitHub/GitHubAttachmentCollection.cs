using System.Collections.Generic;
using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.GitHub
{
	public class GitHubAttachmentCollection : IJsonSerializable
	{
		internal string Id { get; private set; }
		public IEnumerable<GitHubIssue> Issues { get; private set; }
		public IEnumerable<GitHubBranch> Branches { get; private set; }
		public IEnumerable<GitHubPullRequest> PullRequests { get; private set; }
		public IEnumerable<GitHubCommit> Commits { get; private set; }

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