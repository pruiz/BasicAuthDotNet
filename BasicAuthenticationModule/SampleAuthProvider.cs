using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BasicAuthenticationModule
{

	/// <summary>
	/// Sample IAuthProvider that will authenticate all users, and only allow access to user with a username of bjorn
	/// </summary>
	public class SampleAuthProvider : IAuthProvider
	{

		#region IAuthProvider Members

		/// <summary>
		/// Validates the username and password and returns whether or not the combination is a valid user
		/// </summary>
		/// <param name="userName">The username to validate</param>
		/// <param name="password">The password to match</param>
		/// <param name="user">The user object created</param>
		/// <returns>
		/// true if the combination is a valid user;false otherwise
		/// </returns>
		public bool IsValidUser(string userName, string password, out IBasicUser user)
		{
			user = new BasicUser();
			user.UserName = userName;
			//user.Password = password;

			return true;
		}

		/// <summary>
		/// Determines whether or not the current request requires HTTP-Basic authentication.
		/// </summary>
		/// <param name="request">The request to check</param>
		/// <returns>true if required;false otherwise</returns>
		public bool BasicAuthRequired(HttpRequest request)
		{
			return true;
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			//This is intentional, since we don't have any resources to free in this very simple sample IAuthProvider
		}

		#endregion
	}
}
