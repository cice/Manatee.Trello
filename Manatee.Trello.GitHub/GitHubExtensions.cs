using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Trello.GitHub
{
	/// <summary>
	/// Extensions to retrieve Custom Fields data.
	/// </summary>
	public static class GitHubExtensions
	{
		private static readonly JsonSerializer Serializer = new JsonSerializer();

		/// <summary>
		/// Gets the repositories which appear as links in the board header.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <returns>The repositories.</returns>
		public static IEnumerable<GitHubRepository> GitHubRepositories(this Board board)
		{
			GitHubPowerUp.Register();

			var data = board.PowerUpData.FirstOrDefault(d => d.PluginId == GitHubPowerUp.PowerUpId);
			if (data == null) return null;

			var json = JsonValue.Parse(data.Value);
			var settings = Serializer.Deserialize<GitHubSettings>(json);
			return settings.Repositories;
		}

		// TODO: convert to an extension on ReadOnlyAttachmentCollection, filtering through IsGitHubItem().
		/// <summary>
		/// Gets the items which have been linked to the card.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <returns>All custom fields defined for the card.</returns>
		public static GitHubAttachmentCollection GitHubAttachments(this Card card)
		{
			GitHubPowerUp.Register();

			var allAttachments = card.Attachments.ToList();
			var issues = allAttachments.Where(GitHubIssue.IsMatch)
			                           .Select(a => new GitHubIssue(a));
			var commits = allAttachments.Where(GitHubCommit.IsMatch)
			                            .Select(a => new GitHubCommit(a));
			var pullRequests = allAttachments.Where(GitHubPullRequest.IsMatch)
			                                 .Select(a => new GitHubPullRequest(a));
			var branches = allAttachments.Where(GitHubBranch.IsMatch)
			                             .Select(a => new GitHubBranch(a));

			return new GitHubAttachmentCollection
				{
					Branches = branches,
					Commits = commits,
					Issues = issues,
					PullRequests = pullRequests
				};
		}

		public static bool IsGitHubItem(this Attachment attachment)
		{
			return GitHubIssue.IsMatch(attachment) ||
			       GitHubPullRequest.IsMatch(attachment) ||
			       GitHubCommit.IsMatch(attachment) ||
			       GitHubBranch.IsMatch(attachment);
		}
	}
}