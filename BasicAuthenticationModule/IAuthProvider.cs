using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Principal;

namespace BasicAuthenticationModule
{
	/// <summary>
	/// An authentication and authorization provider for very simple applications
	/// Should probably be either implemented with a database backend, 
	/// or using a web.config custom section
	/// Implementors of this interface should provide a default no args constructor to be used
	/// by the AuthenticationModule
	/// </summary>
	/// <remarks>
	/// Taken from: http://blog.smithfamily.dk/2008/08/27/ImplementingBasicAuthenticationInASPNET20.aspx
	/// </remarks>
	public interface IAuthProvider : IDisposable
	{
		/// <summary>
		/// Determines whether or not the current request requires HTTP-Basic authentication.
		/// </summary>
		/// <param name="request">The request to check</param>
		/// <returns>true if required;false otherwise</returns>
		bool BasicAuthRequired(HttpRequest request);

		/// <summary>
		/// Validates the username and password and returns whether or not the combination is a valid user
		/// </summary>
		/// <param name="userName">The username to validate</param>
		/// <param name="password">The password to match</param>
		/// <param name="user">The user object created</param>
		/// <returns>true if the combination is a valid user;false otherwise</returns>
		bool IsValidUser(string userName, string password, out IBasicUser user);
	}
}