﻿using Manatee.Trello.Internal.DataAccess;

namespace Manatee.Trello
{
	/// <summary>
	/// Extension methods for members.
	/// </summary>
	public static class MemberExtensions
	{
		/// <summary>
		/// Gets all open cards for a member.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <returns>A <see cref="ReadOnlyCardCollection"/> containing the member's cards.</returns>
		public static ReadOnlyCardCollection Cards(this Member member)
		{
			return new ReadOnlyCardCollection(EntityRequestType.Member_Read_Cards, () => member.Id, member.Auth);
		}
	}
}
