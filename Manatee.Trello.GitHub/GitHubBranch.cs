using System.Text.RegularExpressions;

namespace Manatee.Trello.GitHub
{
	public class GitHubBranch
	{
		private static readonly Regex Pattern = new Regex(@"^https:\/\/github\.com\/(?<user>[a-z0-9_.-]+)\/(?<repo>[a-z0-9_.-]+)\/tree\/", RegexOptions.IgnoreCase);

		public string Name { get; }
		public string Repository { get; }
		public string User { get; }
		public string Url { get; set; }

		internal GitHubBranch(Attachment a)
		{
			Name = a.Name;
			Url = a.Url;

			var match = Pattern.Match(a.Url);
			Repository = match.Groups["repo"].Value;
			User = match.Groups["user"].Value;
		}

		internal static bool IsMatch(Attachment attachment)
		{
			return Pattern.IsMatch(attachment.Url);
		}
	}
}