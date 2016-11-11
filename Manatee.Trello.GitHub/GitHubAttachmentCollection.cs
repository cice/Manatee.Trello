using System.Collections.Generic;

namespace Manatee.Trello.GitHub
{
	public class GitHubAttachmentCollection
	{
		public IEnumerable<GitHubIssue> Issues { get; internal set; }
		public IEnumerable<GitHubBranch> Branches { get; internal set; }
		public IEnumerable<GitHubPullRequest> PullRequests { get; internal set; }
		public IEnumerable<GitHubCommit> Commits { get; internal set; }
	}
}