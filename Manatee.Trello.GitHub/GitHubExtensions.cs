using System.Collections.Generic;
using System.Linq;
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
		public static IEnumerable<GitHubRepository> GitHubSettings(this Board board)
		{
			GitHubPowerUp.Register();
			var data = board.PowerUpData.FirstOrDefault(d => d.PluginId == GitHubPowerUp.PowerUpId);
			if (data == null) return null;

			var json = JsonValue.Parse(data.Value);
			var settings = Serializer.Deserialize<GitHubSettings>(json);
			return settings.Repositories;
		}

		/// <summary>
		/// Gets the items which have been linked to the card.
		/// </summary>
		/// <param name="card">The card.</param>
		/// <returns>All custom fields defined for the card.</returns>
		public static GitHubAttachmentCollection GitHubAttachments(this Card card)
		{
			GitHubPowerUp.Register();
			var data = card.PowerUpData.FirstOrDefault(d => d.PluginId == GitHubPowerUp.PowerUpId);
			if (data == null) return null;

			// This will return null if the power-up isn't registered.
			var powerUp = TrelloConfiguration.Cache.Find<GitHubPowerUp>(p => p.Id == GitHubPowerUp.PowerUpId);
			if (powerUp == null) return null;

			var json = JsonValue.Parse(data.Value);
			var attachments = Serializer.Deserialize<GitHubAttachmentCollection>(json);

			return attachments;
		}
	}
}