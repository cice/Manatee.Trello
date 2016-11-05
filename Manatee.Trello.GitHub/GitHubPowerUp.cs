using Manatee.Trello.Json;

namespace Manatee.Trello.GitHub
{
	public class GitHubPowerUp : PowerUpBase
	{
		internal const string PowerUpId = "55a5d916446f517774210004";

		private static bool _isRegistered;

		internal GitHubPowerUp(IJsonPowerUp json, TrelloAuthorization auth)
			: base(json, auth) {}

		internal static void Register()
		{
			if (!_isRegistered)
			{
				_isRegistered = true;
				TrelloConfiguration.RegisterPowerUp(PowerUpId, (j, a) => new GitHubPowerUp(j, a));
			}
		}
	}
}
