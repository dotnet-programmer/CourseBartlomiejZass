using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MusicStore.NetFramework.WebApp.App_Start;
using MusicStore.NetFramework.WebApp.Models;
using MusicStore.NetFramework.WebApp.ViewModels;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	// INFO - atrybut RequireHttps - wymagaj adresu https
	[RequireHttps]
	public class AccountController : Controller
	{
		private ApplicationUserManager _userManager;
		private ApplicationSignInManager _signInManager;

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationUserManager UserManager
		{
			get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			private set => _userManager = value;
		}

		public ApplicationSignInManager SignInManager
		{
			get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			private set => _signInManager = value;
		}

		private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

		// GET: Account/Login
		public ActionResult Login(string returnUrl)
		{
			// INFO - parametr returnUrl - potrzebne do przekierowania/powrotu na stronę z której kliknięto "Zaloguj" po udanym zalogowaniu się
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// This doen't count login failures towards lockout only two factor authentication
			// To enable password failures to trigger lockout, change to shouldLockout: true
			var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					// sprawdzenie czy zwracany adres na pewno jest poprawny i należy do tej aplikacji
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
				case SignInStatus.Failure:
				default:
					// INFO - ręczne dodanie błędu do ModelState
					ModelState.AddModelError("loginerror", "Nieudana próba logowania.");
					return View(model);
			}
		}

		[AllowAnonymous]
		public ActionResult Register() => View();

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email, UserData = new UserData() };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					return RedirectToAction("Index", "Home");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		// INFO - logowanie za pomocą zewnętrznych serwisów - facebook, google

		#region Facebook_Google

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
			// INFO - specjalny resultat dziedziczący po ActionResult
			// powoduje przekierowanie do zewnętrznego serwisu wraz z wysłaniem specjalnych tokenów uwierzytelniających
			// po przejściu na inną stronę logowania, serwis musi wiedzieć gdzie wrócić - do tego służy ExternalLoginCallback
			=> new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, create account with external provider login
					// in reality, we might ask for providing e-mail (+ confirming it)
					// we also need some error checking logic (ie. verification if user doesn't already exist)

					var user = new ApplicationUser
					{
						UserName = loginInfo.Email,
						Email = loginInfo.Email,
						UserData = new UserData { Email = loginInfo.Email }
					};

					var registrationResult = await UserManager.CreateAsync(user);
					if (registrationResult.Succeeded)
					{
						// INFO - powiązanie konta lokalnego z kontem zewnętrznym (facebook, google)
						registrationResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
						if (registrationResult.Succeeded)
						{
							await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
							return RedirectToLocal(returnUrl);
						}
						else
						{
							throw new Exception("External provider association error");
						}
					}
					else
					{
						throw new Exception("Registration error");
					}
			}
		}

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		#endregion Facebook_Google
	}
}