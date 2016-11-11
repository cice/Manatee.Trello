using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Trello.GitHub;
using Manatee.Trello.ManateeJson;
using Manatee.Trello.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Trello.Test
{
	[TestClass]
	public class DevTest
	{
		[TestMethod]
		public void TestMethod1()
		{
			Run(() =>
				{
					var board = new Board(TrelloIds.BoardId);
					OutputCollection("Repositories", board.GitHubRepositories().Select(r => r.Name));
					var card = new Card(TrelloIds.CardId);
					var github = card.GitHubAttachments();
					OutputCollection("Pull Requests", github.PullRequests.Select(a => a.Url));
					OutputCollection("Branches", github.Branches.Select(a => a.Url));
					OutputCollection("Commits", github.Commits.Select(a => a.Url));
					OutputCollection("Issues", github.Issues.Select(a => a.Url));
				});
		}

		private static void Run(System.Action action)
		{
			var serializer = new ManateeSerializer();
			TrelloConfiguration.Serializer = serializer;
			TrelloConfiguration.Deserializer = serializer;
			TrelloConfiguration.JsonFactory = new ManateeFactory();
			TrelloConfiguration.RestClientProvider = new WebApiClientProvider();

			TrelloAuthorization.Default.AppKey = TrelloIds.AppKey;
			TrelloAuthorization.Default.UserToken = TrelloIds.UserToken;

			action();

			TrelloProcessor.Flush();
		}

		private static void OutputCollection<T>(string section, IEnumerable<T> collection)
		{
			Console.WriteLine();
			Console.WriteLine($"{section} ({collection.Count()})");
			foreach (var item in collection)
			{
				Console.WriteLine($"    {item}");
			}
		}
	}
}
