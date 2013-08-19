﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Board.cs
	Namespace:		Manatee.Trello
	Class Name:		Board
	Purpose:		Represents a board on Trello.com.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Json;
using Manatee.Trello.Json;
using Manatee.Trello.Rest;

namespace Manatee.Trello
{
	///<summary>
	/// Represents a board.
	///</summary>
	public class Board : ExpiringObject, IEquatable<Board>, IComparable<Board>
	{
		private IJsonBoard _jsonBoard;
		private readonly ExpiringList<Action, IJsonAction> _actions;
		private readonly ExpiringList<Card, IJsonCard> _archivedCards;
		private readonly ExpiringList<List, IJsonList> _archivedLists;
		private readonly ExpiringList<InvitedMember, IJsonMember> _invitedMembers;
		private readonly LabelNames _labelNames;
		private readonly ExpiringList<List, IJsonList> _lists;
		private readonly ExpiringList<BoardMembership, IJsonBoardMembership> _memberships;
		private Organization _organization;
		private readonly BoardPreferences _preferences;
		private readonly BoardPersonalPreferences _personalPreferences;

		///<summary>
		/// Enumerates all actions associated with this board.
		///</summary>
		public IEnumerable<Action> Actions { get { return _actions; } }
		/// <summary>
		/// Enumerates all archived cards on this board.
		/// </summary>
		public IEnumerable<Card> ArchivedCards { get { return _archivedCards; } }
		/// <summary>
		/// Enumerates all archived lists on this board.
		/// </summary>
		public IEnumerable<List> ArchivedLists { get { return _archivedLists; } }
		///<summary>
		/// Gets or sets the board's description.
		///</summary>
		public string Description
		{
			get
			{
				VerifyNotExpired();
				return (_jsonBoard == null) ? null : _jsonBoard.Desc;
			}
			set
			{
				Validator.Writable();
				if (_jsonBoard == null) return;
				if (_jsonBoard.Desc == value) return;
				_jsonBoard.Desc = value ?? string.Empty;
				Parameters.Add("desc", _jsonBoard.Desc);
				Put();
			}
		}
		/// <summary>
		/// Gets a unique identifier (not necessarily a GUID).
		/// </summary>
		public override string Id
		{
			get { return _jsonBoard != null ? _jsonBoard.Id : base.Id; }
			internal set
			{
				if (_jsonBoard != null)
					_jsonBoard.Id = value;
				base.Id = value;
			}
		}
		/// <summary>
		/// Enumerates all members who have received invitations to this board.
		/// </summary>
		public IEnumerable<Member> InvitedMembers { get { return _invitedMembers; } }
		///<summary>
		/// Gets or sets whether this board is closed.
		///</summary>
		public bool? IsClosed
		{
			get
			{
				VerifyNotExpired();
				return (_jsonBoard == null) ? null : _jsonBoard.Closed;
			}
			set
			{
				Validator.Writable();
				Validator.Nullable(value);
				if (_jsonBoard == null) return;
				if (_jsonBoard.Closed == value) return;
				_jsonBoard.Closed = value;
				Parameters.Add("closed", _jsonBoard.Closed.ToLowerString());
				Put();
			}
		}
		///<summary>
		/// Gets or sets whether this board is pinned to the user's Boards menu.
		///</summary>
		public bool? IsPinned
		{
			get
			{
				VerifyNotExpired();
				return (_jsonBoard == null) ? null : _jsonBoard.Pinned;
			}
			private set
			{
				Validator.Writable();
				Validator.Nullable(value);
				if (_jsonBoard == null) return;
				if (_jsonBoard.Pinned == value) return;
				_jsonBoard.Pinned = value;
				Parameters.Add("pinned", _jsonBoard.Pinned.ToLowerString());
				Put();
			}
		}
		///<summary>
		/// Gets or sets whether the user is subscribed to this board.
		///</summary>
		public bool? IsSubscribed
		{
			get
			{
				VerifyNotExpired();
				return (_jsonBoard == null) ? null : _jsonBoard.Subscribed;
			}
			set
			{
				Validator.Writable();
				Validator.Nullable(value);
				if (_jsonBoard == null) return;
				if (_jsonBoard.Subscribed == value) return;
				_jsonBoard.Subscribed = value;
				Parameters.Add("subscribed", _jsonBoard.Subscribed.ToLowerString());
				Put();
			}
		}
		///<summary>
		/// Gets the board's set of label names.
		///</summary>
		public LabelNames LabelNames { get { return _labelNames; } }
		///<summary>
		/// Gets the board's open lists.
		///</summary>
		public IEnumerable<List> Lists { get { return _lists; } }
		internal ExpiringList<List, IJsonList> ListsList { get { return _lists; } }
		///<summary>
		/// Gets the board's members and their types.
		///</summary>
		public IEnumerable<BoardMembership> Memberships { get { return _memberships; } }
		///<summary>
		/// Gets or sets the board's name.
		///</summary>
		public string Name
		{
			get
			{
				VerifyNotExpired();
				return (_jsonBoard == null) ? null : _jsonBoard.Name;
			}
			set
			{
				Validator.Writable();
				Validator.NonEmptyString(value);
				if (_jsonBoard == null) return;
				if (_jsonBoard.Name == value) return;
				_jsonBoard.Name = value;
				Parameters.Add("name", _jsonBoard.Name);
				Put();
			}
		}
		/// <summary>
		/// Gets or sets the organization, if any, to which this board belongs.
		/// </summary>
		public Organization Organization
		{
			get
			{
				VerifyNotExpired();
				if (_jsonBoard == null) return null;
				return ((_organization == null) || (_organization.Id != _jsonBoard.IdOrganization)) && (Svc != null)
				       	? (_organization = string.IsNullOrWhiteSpace(_jsonBoard.IdOrganization)
				       	                   	? null
				       	                   	: Svc.Retrieve<Organization>(_jsonBoard.IdOrganization))
				       	: _organization;
			}
			set
			{
				Validator.Writable();
				Validator.Entity(value, true);
				if (_jsonBoard == null) return;
				if (value == null)
				{
					_jsonBoard.IdOrganization = null;
				}
				else
				{
					if (_jsonBoard.IdOrganization == value.Id) return;
					_jsonBoard.IdOrganization = value.Id;
				}
				Parameters.Add("idOrganization", _jsonBoard.IdOrganization ?? string.Empty);
				Put();
			}
		}
		///<summary>
		/// Gets the set of preferences for this board unique to the current member.
		///</summary>
		public BoardPersonalPreferences PersonalPreferences { get { return _personalPreferences; } }
		///<summary>
		/// Gets the set of preferences for this board.
		///</summary>
		public BoardPreferences Preferences { get { return _preferences; } }
		///<summary>
		/// Gets the URL for this board.
		///</summary>
		public string Url { get { return (_jsonBoard == null) ? null : _jsonBoard.Url; } }

		internal static string TypeKey { get { return "boards"; } }
		internal static string TypeKey2 { get { return "idBoard"; } }
		internal override string Key { get { return TypeKey; } }
		internal override string Key2 { get { return TypeKey2; } }

		///<summary>
		/// Creates a new instance of the Board class.
		///</summary>
		public Board()
		{
			_jsonBoard = new InnerJsonBoard();
			_actions = new ExpiringList<Action, IJsonAction>(this, Action.TypeKey) {Fields = "id"};
			_archivedCards = new ExpiringList<Card, IJsonCard>(this, Card.TypeKey) {Filter = "closed", Fields = "id"};
			_archivedLists = new ExpiringList<List, IJsonList>(this, List.TypeKey) {Filter = "closed", Fields = "id"};
			_invitedMembers = new ExpiringList<InvitedMember, IJsonMember>(this, InvitedMember.TypeKey) {Fields = "id"};
			_labelNames = new LabelNames(this);
			_lists = new ExpiringList<List, IJsonList>(this, List.TypeKey) {Fields = "id"};
			_memberships = new ExpiringList<BoardMembership, IJsonBoardMembership>(this, BoardMembership.TypeKey);
			_personalPreferences = new BoardPersonalPreferences(this);
			_preferences = new BoardPreferences(this);
		}

		///<summary>
		/// Adds a new list to the board in the specified location
		///</summary>
		///<param name="name">The name of the list.</param>
		///<param name="position">The desired position of the list.  Default is Bottom.  Invalid positions are ignored.</param>
		///<returns>The new list.</returns>
		public List AddList(string name, Position position = null)
		{
			if (Svc == null) return null;
			Validator.Writable();
			Validator.NonEmptyString(name);
			var list = new List();
			var endpoint = EndpointGenerator.Default.Generate(list);
			var request = RequestProvider.Create(endpoint.ToString());
			request.AddParameter("name", name);
			request.AddParameter("idBoard", Id);
			if ((position != null) && position.IsValid)
				request.AddParameter("pos", position);
			list.ApplyJson(Api.Post<IJsonList>(request));
			list.Svc = Svc;
			if (Svc.Configuration.Cache != null)
				Svc.Configuration.Cache.Add(list);
			_lists.MarkForUpdate();
			_actions.MarkForUpdate();
			return list;
		}
		///<summary>
		/// Adds a member to the board or updates the permissions of a member already on the board.
		///</summary>
		///<param name="member">The member</param>
		///<param name="type">The permission level for the member</param>
		public void AddOrUpdateMember(Member member, BoardMembershipType type = BoardMembershipType.Normal)
		{
			if (Svc == null) return;
			Validator.Writable();
			Validator.Entity(member);
			var endpoint = EndpointGenerator.Default.Generate(this, member);
			var request = RequestProvider.Create(endpoint.ToString());
			request.AddParameter("type", type.ToLowerString());
			Api.Put<IJsonMember>(request);
			_memberships.MarkForUpdate();
			_actions.MarkForUpdate();
		}
		/// <summary>
		///  Adds a new or existing member to the board or updates the permissions of a member already on the board.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="type">The permission level for the member</param>
		/// <param name="emailAddress"></param>
		public Member AddOrUpdateMember(string emailAddress, string fullName, BoardMembershipType type = BoardMembershipType.Normal)
		{
			if (Svc == null) return null;
			Validator.Writable();
			Validator.NonEmptyString(emailAddress);
			Validator.NonEmptyString(fullName);
			var member = Svc.SearchMembers(emailAddress, 1).FirstOrDefault();
			if (member != null)
			{
				AddOrUpdateMember(member, type);
				_memberships.MarkForUpdate();
				_memberships.MarkForUpdate();
				_actions.MarkForUpdate();
			}
			return member;
		}
		///<summary>
		/// Marks the board as viewed by the current member.
		///</summary>
		public void MarkAsViewed()
		{
			if (Svc == null) return;
			Validator.Writable();
			var endpoint = EndpointGenerator.Default.Generate(this);
			endpoint.Append("markAsViewed");
			var request = RequestProvider.Create(endpoint.ToString());
			Api.Post<IJsonBoard>(request);
			_actions.MarkForUpdate();
		}
		/// <summary>
		/// Extends an invitation to the board to another member.
		/// </summary>
		/// <param name="member">The member to invite.</param>
		/// <param name="type">The level of membership offered.</param>
		internal void InviteMember(Member member, BoardMembershipType type = BoardMembershipType.Normal)
		{
			Validator.Writable();
			Validator.Entity(member);
			Log.Error(new NotSupportedException("Inviting members to boards is not yet supported by the Trello API."));
		}
		///<summary>
		/// Removes a member from the board.
		///</summary>
		///<param name="member"></param>
		public void RemoveMember(Member member)
		{
			if (Svc == null) return;
			Validator.Writable();
			Validator.Entity(member);
			var endpoint = EndpointGenerator.Default.Generate(this, member);
			var request = RequestProvider.Create(endpoint.ToString());
			Api.Delete<IJsonBoard>(request);
			_memberships.MarkForUpdate();
			_actions.MarkForUpdate();
		}
		/// <summary>
		/// Rescinds an existing invitation to the board.
		/// </summary>
		/// <param name="member"></param>
		internal void RescindInvitation(Member member)
		{
			Validator.Entity(member);
			Log.Error(new NotSupportedException("Inviting members to boards is not yet supported by the Trello API."));
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Board other)
		{
			return Id == other.Id;
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is Board)) return false;
			return Equals((Board) obj);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(Board other)
		{
			var order = string.Compare(Name, other.Name);
			return order;
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return Name;
		}
		/// <summary>
		/// Retrieves updated data from the service instance and refreshes the object.
		/// </summary>
		public override bool Refresh()
		{
			var endpoint = EndpointGenerator.Default.Generate(this);
			var request = RequestProvider.Create(endpoint.ToString());
			request.AddParameter("fields", "name,desc,closed,idOrganization,pinned,url,subscribed");
			request.AddParameter("actions", "none");
			request.AddParameter("cards", "none");
			request.AddParameter("lists", "none");
			request.AddParameter("members", "none");
			request.AddParameter("checklists", "none");
			request.AddParameter("organization", "false");
			request.AddParameter("myPrefs", "false");
			var obj = Api.Get<IJsonBoard>(request);
			if (obj == null) return false;
			ApplyJson(obj);
			return true;

		}

		/// <summary>
		/// Propigates the service instance to the object's owned objects.
		/// </summary>
		protected override void PropigateService()
		{
			_actions.Svc = Svc;
			_archivedCards.Svc = Svc;
			_archivedLists.Svc = Svc;
			_labelNames.Svc = Svc;
			_lists.Svc = Svc;
			_memberships.Svc = Svc;
			_personalPreferences.Svc = Svc;
			_preferences.Svc = Svc;
			if (_organization != null) _organization.Svc = Svc;
		}

		internal override void ApplyJson(object obj)
		{
			if (obj is IRestResponse)
				_jsonBoard = ((IRestResponse<IJsonBoard>) obj).Data;
			else
				_jsonBoard = (IJsonBoard) obj;
		}

		private void Put()
		{
			if (Svc == null)
			{
				Parameters.Clear();
				return;
			}
			var endpoint = EndpointGenerator.Default.Generate(this);
			var request = RequestProvider.Create(endpoint.ToString());
			foreach (var parameter in Parameters)
			{
				request.AddParameter(parameter.Key, parameter.Value);
			}
			Api.Put<IJsonBoard>(request);
			Parameters.Clear();
			_actions.MarkForUpdate();
		}
	}
}