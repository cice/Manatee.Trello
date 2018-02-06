using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Trello.FunctionalTests
{
	[TestFixture]
	public class ScriptedTest
	{
		[Test]
		public void Run()
		{
			Board board = null;
			try
			{
				board = Member.Me.Boards.Add($"TestBoard{Guid.NewGuid()}");
			}
			finally
			{
				board?.Delete();
			}
		}
	}
}
