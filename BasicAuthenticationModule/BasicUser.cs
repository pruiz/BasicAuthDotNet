using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace BasicAuthenticationModule
{
	public class BasicUser : IBasicUser
	{
		/// <summary>
		/// Gets or sets the username of the user
		/// </summary>
		/// <value>The username of the user.</value>
		public string UserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password
		{
			get;
			set;
		}

		#region IIdentity Members

		/// <summary>
		/// Gets the type of authentication used.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The type of authentication used to identify the user.
		/// </returns>
		public virtual string AuthenticationType
		{
			get
			{
				return "BasicAuthenticationModule";
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the user has been authenticated.
		/// </summary>
		/// <value></value>
		/// <returns>true if the user was authenticated; otherwise, false.
		/// </returns>
		public bool IsAuthenticated
		{
			get
			{
				return UserName != null;
			}
		}

		/// <summary>
		/// Gets the name of the current user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The name of the user on whose behalf the code is running.
		/// </returns>
		public string Name
		{
			get
			{
				return UserName;
			}
		}

		#endregion
	}
}
