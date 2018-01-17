﻿namespace Manatee.Trello
{
	public interface IMemberSearchResult
	{
		/// <summary>
		/// Gets the returned member.
		/// </summary>
		IMember Member { get; }

		/// <summary>
		/// Gets a value indicating the similarity of the member to the search query.
		/// </summary>
		int? Similarity { get; }
	}

	/// <summary>
	/// Represents a result from a member search.
	/// </summary>
	public class MemberSearchResult : IMemberSearchResult
	{
		/// <summary>
		/// Gets the returned member.
		/// </summary>
		public IMember Member { get; private set; }
		/// <summary>
		/// Gets a value indicating the similarity of the member to the search query.
		/// </summary>
		public int? Similarity { get; private set; }

		internal MemberSearchResult(Member member, int? similarity)
		{
			Member = member;
			Similarity = similarity;
		}
	}
}
