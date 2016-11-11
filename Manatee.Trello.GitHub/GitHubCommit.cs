using System.Text.RegularExpressions;

namespace Manatee.Trello.GitHub
{
	public class GitHubCommit
	{
		private static readonly Regex Pattern = new Regex(@"^https:\/\/github\.com\/(?<user>[a-z0-9_.-]+)\/(?<repo>[a-z0-9_.-]+)\/commit\/(?<id>[a-f0-9]+)", RegexOptions.IgnoreCase);

		public string Id { get; }
		public string Name { get; }
		public string Repository { get; }
		public string User { get; }
		public string Url { get; set; }

		internal GitHubCommit(Attachment a)
		{
			Name = a.Name;
			Url = a.Url;

			var match = Pattern.Match(a.Url);
			Id = match.Groups["id"].Value;
			Repository = match.Groups["repo"].Value;
			User = match.Groups["user"].Value;
		}

		internal static bool IsMatch(Attachment attachment)
		{
			return Pattern.IsMatch(attachment.Url);
		}
	}
}