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
 
	File Name:		MemberPreferences.cs
	Namespace:		Manatee.Trello
	Class Name:		MemberPreferences
	Purpose:		Represents available preference settings for a member
					on Trello.com.

***************************************************************************************/
using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Json;
using Manatee.Trello.Json;
using Manatee.Trello.Rest;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents available preference settings for a member.
	/// </summary>
	public class MemberPreferences : ExpiringObject
	{
		private IJsonMemberPreferences _jsonMemberPreferences;

		/// <summary>
		/// Enables/disables color-blind mode.
		/// </summary>
		public bool? ColorBlind
		{
			get
			{
				VerifyNotExpired();
				return (_jsonMemberPreferences == null) ? null : _jsonMemberPreferences.ColorBlind;
			}
			set
			{
				Validator.Writable();
				Validator.Nullable(value);
				if (_jsonMemberPreferences == null) return;
				if (_jsonMemberPreferences.ColorBlind == value) return;
				_jsonMemberPreferences.ColorBlind = value;
				Parameters.Add("value", _jsonMemberPreferences.ColorBlind.ToLowerString());
				Put("colorBlind");
			}
		}
		/// <summary>
		/// Gets or sets the number of minutes between summary emails.
		/// </summary>
		public MemberPreferenceSummaryPeriodType? MinutesBetweenSummaries
		{
			get
			{
				VerifyNotExpired();
				return (_jsonMemberPreferences == null) ? null : (MemberPreferenceSummaryPeriodType?) _jsonMemberPreferences.MinutesBetweenSummaries;
			}
			set
			{
				Validator.Writable();
				Validator.Nullable(value);
				Validator.Enumeration(value.Value);
				if (_jsonMemberPreferences == null) return;
				if (_jsonMemberPreferences.MinutesBetweenSummaries == (int?) value) return;
				_jsonMemberPreferences.MinutesBetweenSummaries = (int?) value;
				Parameters.Add("value", _jsonMemberPreferences.MinutesBetweenSummaries);
				Put("minutesBetweenSummaries");
			}
		}
		/// <summary>
		/// Enables/disables summary emails.
		/// </summary>
		internal bool? SendSummaries
		{
			get
			{
				VerifyNotExpired();
				return (_jsonMemberPreferences == null) ? null : _jsonMemberPreferences.SendSummaries;
			}
			set
			{
				Validator.Writable();
				Validator.Nullable(value);
				if (_jsonMemberPreferences == null) return;
				if (_jsonMemberPreferences.SendSummaries == value) return;
				_jsonMemberPreferences.SendSummaries = value;
				Parameters.Add("value", _jsonMemberPreferences.SendSummaries.ToLowerString());
				Put("sendSummaries");
			}
		}
		/// <summary>
		/// Gets or sets the number of minutes before a deadline to notify the member.
		/// </summary>
		internal int? MinutesBeforeDeadlineToNotify
		{
			get
			{
				VerifyNotExpired();
				return (_jsonMemberPreferences == null) ? null : _jsonMemberPreferences.MinutesBeforeDeadlineToNotify;
			}
			set
			{
				Validator.Writable();
				Validator.Nullable(value);
				if (_jsonMemberPreferences == null) return;
				if (_jsonMemberPreferences.MinutesBeforeDeadlineToNotify == value) return;
				_jsonMemberPreferences.MinutesBeforeDeadlineToNotify = value;
				Parameters.Add("value", _jsonMemberPreferences.MinutesBeforeDeadlineToNotify);
				Put("minutesBeforeDeadlineToNotify");
			}
		}

		internal static string TypeKey { get { return "prefs"; } }
		internal static string TypeKey2 { get { return "prefs"; } }
		internal override string Key { get { return TypeKey; } }
		internal override string Key2 { get { return TypeKey2; } }

		/// <summary>
		/// Creates a new instance of the MemberPreferences class.
		/// </summary>
		public MemberPreferences()
		{
			_jsonMemberPreferences = new InnerJsonMemberPreferences();
		}
		internal MemberPreferences(Member owner)
			: this()
		{
			Owner = owner;
		}

		/// <summary>
		/// Retrieves updated data from the service instance and refreshes the object.
		/// </summary>
		public override bool Refresh()
		{
			var endpoint = EndpointGenerator.Default.Generate(Owner, this);
			var request = RequestProvider.Create(endpoint.ToString());
			var obj = Api.Get<IJsonMemberPreferences>(request);
			if (obj == null) return false;
			ApplyJson(obj);
			return true;
		}

		/// <summary>
		/// Propigates the service instance to the object's owned objects.
		/// </summary>
		protected override void PropigateService() {}

		internal override void ApplyJson(object obj)
		{
			if (obj is IRestResponse)
				_jsonMemberPreferences = ((IRestResponse<IJsonMemberPreferences>)obj).Data;
			else
				_jsonMemberPreferences = (IJsonMemberPreferences)obj;
		}

		private void Put(string extension)
		{
			if (Svc == null)
			{
				Parameters.Clear();
				return;
			}
			var endpoint = EndpointGenerator.Default.Generate(Owner, this);
			endpoint.Append(extension);
			var request = RequestProvider.Create(endpoint.ToString());
			foreach (var parameter in Parameters)
			{
				request.AddParameter(parameter.Key, parameter.Value);
			}
			Api.Put<IJsonMemberPreferences>(request);
			Parameters.Clear();
		}
	}
}