using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.Configuration;
using System.Reflection;

namespace BasicAuthenticationModule
{
	/// <summary>
	/// HttpModule that provides Basic authentication for asp.net applications
	/// </summary>
	/// <remarks>
	/// Based on:
	///		- http://blog.smithfamily.dk/2008/08/27/ImplementingBasicAuthenticationInASPNET20.aspx
	///		- http://www.codeproject.com/KB/aspnet/mybasicauthentication.aspx
	///		- http://cacheandquery.com/blog/2011/03/customizing-asp-net-mvc-basic-authentication/
	/// </remarks>
	public class BasicAuthenticationModule : IHttpModule
	{
		private static object _lock = new object();
		private static bool _initialized = false;
		private HttpApplication _application = null;

		/// <summary>
		/// Gets or sets the auth provider.
		/// </summary>
		/// <remarks>
		/// Publicly accessible so it can be set from code instead of from configuration file.
		/// </remarks>
		/// <value>The auth provider.</value>
		public static IAuthProvider AuthProvider { get; set; }
		public static string Realm { get; set; }

		#region IHttpModule Members
		/// <summary>
		/// Initializes the <see cref="BasicAuthenticationModule"/> class.
		/// Instantiates the IAuthProvider configured in the web.config
		/// </summary>
		private static void Initialize()
		{
			lock (_lock)
			{
				var provider = ConfigurationManager.AppSettings["BasicAuthenticationModule.AuthProvider"];

				if (!string.IsNullOrEmpty(provider))
				{
					Type providerType = Type.GetType(provider, true);
					AuthProvider = Activator.CreateInstance(providerType, false) as IAuthProvider;
				}

				// Realm string..
				if (string.IsNullOrEmpty(Realm))
					Realm = ConfigurationManager.AppSettings["BasicAuthenticationModule.Realm"]; ;

				if (string.IsNullOrEmpty(Realm))
					Realm = System.Net.Dns.GetHostName();

				_initialized = true;
			}
		}

		public BasicAuthenticationModule()
		{
			if (!_initialized)
				Initialize();
		}

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
		public void Init(HttpApplication context)
		{
			context.AuthenticateRequest += context_AuthenticateRequest;
			context.EndRequest += context_EndRequest;
			_application = context;
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
			//AuthProvider.Dispose();
			//AuthProvider = null;
			_application.AuthenticateRequest -= context_AuthenticateRequest;
			_application.EndRequest -= context_EndRequest;
		}

		/// <summary>
		/// Sends the Unauthorized header to the user, telling the user to provide a valid username and password
		/// </summary>
		/// <param name="context">The context.</param>
		private void SendAuthHeader(HttpApplication context)
		{
			context.Response.Clear();
			context.Response.StatusCode = 401;
			context.Response.StatusDescription = "Unauthorized";
			context.Response.AddHeader("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
			context.Response.Write("401 Sorry, authentication required.");
			context.Response.End();

			context.CompleteRequest();
		}

		/// <summary>
		/// Tries to authenticate the user
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private bool TryAuthenticate(HttpApplication context)
		{
			if (AuthProvider == null)
				throw new ApplicationException("No authentication provider configured for BasicAuthenticationModule.");

			//var authHeader = context.Request.ServerVariables["HTTP_AUTHORIZATION"];
			string authHeader = context.Request.Headers["Authorization"];
			if (!string.IsNullOrEmpty(authHeader))
			{
				if (authHeader.StartsWith("basic ", StringComparison.InvariantCultureIgnoreCase))
				{

					string userNameAndPassword = Encoding.Default.GetString(
						Convert.FromBase64String(authHeader.Substring(6)));
					string[] parts = userNameAndPassword.Split(new[] { ':' }, 2);
					IBasicUser bu = null;

					if (!AuthProvider.IsValidUser(parts[0], parts[1], out bu))
						return false;

					if (!bu.IsAuthenticated)
						return false;

					context.Context.User = new GenericPrincipal(bu, new string[] { });
				}

			}
			return false;
		}

		/// <summary>
		/// Handles the AuthenticateRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_AuthenticateRequest(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;

			if (app.Context.Request.IsAuthenticated)
				return;

			if (!TryAuthenticate(app))
			{
				//if (AuthProvider.BasicAuthRequired(app.Request))
				//	SendAuthHeader(app);
			}
		}

		/// <summary>
		/// Handles the EndRequest event of the application context.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_EndRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;

			if (app.Response.StatusCode == 401 && AuthProvider.BasicAuthRequired(app.Request))
			{
				app.Response.AddHeader("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
			}
		}
		#endregion
	}
}
