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
 
	File Name:		UpdateCardNameAction.cs
	Namespace:		Manatee.Trello
	Class Name:		UpdateCardNameAction
	Purpose:		Indicates card's name was updated.

***************************************************************************************/
using System.Diagnostics;

namespace Manatee.Trello
{
	/// <summary>
	/// Indicates card's name was updated.
	/// </summary>
	public class UpdateCardNameAction : Action
	{
		/// <summary>
		/// Creates a new instance of the UpdateCardNameAction class.
		/// </summary>
		/// <param name="action"></param>
		public UpdateCardNameAction(Action action)
		{
			Debug.Assert(false, string.Format(
				"{0} is not yet configured.  Please post the JSON returned by http://api.trello.com/actions/{1} to https://trello.com/c/k5q97GRf",
				GetType().Name, action.Id));
			VerifyNotExpired();
		}
	}
}