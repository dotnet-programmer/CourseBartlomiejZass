using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MusicStore.NetFramework.WebApp.App_Start;
using MusicStore.NetFramework.WebApp.DAL;
using MusicStore.NetFramework.WebApp.Infrastructure;
using MusicStore.NetFramework.WebApp.Models;
using MusicStore.NetFramework.WebApp.ViewModels;

namespace MusicStore.NetFramework.WebApp.Controllers
{
	[Authorize]
	public class ManageController : Controller
	{
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private readonly StoreContext _context;
		private readonly IMailService _mailService;
		private ApplicationUserManager _userManager;

		public ManageController(StoreContext context, IMailService mailService)
		{
			_context = context;
			_mailService = mailService;
		}

		public ManageController(ApplicationUserManager userManager) => UserManager = userManager;

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			LinkSuccess,
			Error
		}

		private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

		public ApplicationUserManager UserManager
		{
			get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			private set => _userManager = value;
		}

		// GET: Manage
		public async Task<ActionResult> Index(ManageMessageId? message)
		{
			// INFO - TempData to struktura stworzona specjalnie dla wzorca PRG
			// potrafi zapamiętać dane, ale tylko pomiędzy 2 requestami, po czym jest usuwana
			if (TempData["ViewData"] != null)
			{
				ViewData = (ViewDataDictionary)TempData["ViewData"];
			}

			ViewBag.UserIsAdmin = User.IsInRole("Admin");

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				return View("Error");
			}
			var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
			var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

			var model = new ManageCredentialsViewModel
			{
				Message = message,
				HasPassword = HasPassword(),
				CurrentLogins = userLogins,
				OtherLogins = otherLogins,
				ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1,
				UserData = user.UserData
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeProfile([Bind(Prefix = "UserData")] UserData userData)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				user.UserData = userData;
				var result = await UserManager.UpdateAsync(user);
				AddErrors(result);
			}

			if (!ModelState.IsValid)
			{
				TempData["ViewData"] = ViewData;
				return RedirectToAction("Index");
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword([Bind(Prefix = "ChangePasswordViewModel")] ChangePasswordViewModel model)
		{
			// In case we have simple errors - return
			if (!ModelState.IsValid)
			{
				TempData["ViewData"] = ViewData;
				return RedirectToAction("Index");
			}

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}
				return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
			}
			AddErrors(result);

			// In case we have login errors
			if (!ModelState.IsValid)
			{
				TempData["ViewData"] = ViewData;
				return RedirectToAction("Index");
			}

			var message = ManageMessageId.ChangePasswordSuccess;
			return RedirectToAction("Index", new { Message = message });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SetPassword([Bind(Prefix = "SetPasswordViewModel")] SetPasswordViewModel model)
		{
			// In case we have simple errors - return
			if (!ModelState.IsValid)
			{
				TempData["ViewData"] = ViewData;
				return RedirectToAction("Index");
			}

			if (ModelState.IsValid)
			{
				var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
					if (user != null)
					{
						await SignInAsync(user, isPersistent: false);
					}
					return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
				}
				AddErrors(result);

				if (!ModelState.IsValid)
				{
					TempData["ViewData"] = ViewData;
					return RedirectToAction("Index");
				}
			}

			var message = ManageMessageId.SetPasswordSuccess;
			return RedirectToAction("Index", new { Message = message });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
			// Request a redirect to the external login provider to link a login for the current user
			=> new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());

		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
			if (loginInfo == null)
			{
				return RedirectToAction("Index", new { Message = ManageMessageId.Error });
			}
			var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
			return result.Succeeded ? RedirectToAction("Index", new { Message = ManageMessageId.LinkSuccess }) : RedirectToAction("Index", new { Message = ManageMessageId.Error });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			ManageMessageId? message;
			var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}
				message = ManageMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ManageMessageId.Error;
			}
			return RedirectToAction("Index", new { Message = message });
		}

		private async Task SignInAsync(ApplicationUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
		}

		public ActionResult OrdersList()
		{
			bool isAdmin = User.IsInRole("Admin");
			ViewBag.UserIsAdmin = isAdmin;

			IEnumerable<Order> userOrders;

			// For admin users - return all orders
			if (isAdmin)
			{
				userOrders = _context.Orders
					.Include("OrderItems")
					.OrderByDescending(o => o.DateCreated)
					.ToArray();
			}
			else
			{
				var userId = User.Identity.GetUserId();
				userOrders = _context.Orders
					.Where(o => o.UserId == userId)
					.Include("OrderItems")
					.OrderByDescending(o => o.DateCreated)
					.ToArray();
			}

			return View(userOrders);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public OrderState ChangeOrderState(Order order)
		{
			Order orderToModify = _context.Orders.Find(order.OrderId);
			orderToModify.OrderState = order.OrderState;
			_context.SaveChanges();

			if (orderToModify.OrderState == OrderState.Shipped)
			{
				// Schedule confirmation
				//string url = Url.Action("SendStatusEmail", "Manage", new { orderid = orderToModify.OrderId, lastname = orderToModify.LastName }, Request.Url.Scheme);

				//BackgroundJob.Enqueue(() => Helpers.CallUrl(url));

				//IMailService mailService = new HangFirePostalMailService();
				//mailService.SendOrderShippedEmail(orderToModify);

				_mailService.SendOrderShippedEmail(orderToModify);

				//dynamic email = new Postal.Email("OrderShipped");
				//email.To = orderToModify.Email;
				//email.OrderId = orderToModify.OrderId;
				//email.FullAddress = string.Format("{0} {1}, {2}, {3}", orderToModify.FirstName, orderToModify.LastName, orderToModify.Address, orderToModify.CodeAndCity);
				//email.Send();
			}

			return order.OrderState;
		}

		[AllowAnonymous]
		public ActionResult SendStatusEmail(int orderId, string lastName)
		{
			// This could also be used (but problems when hosted on Azure Websites)
			// if (Request.IsLocal)

			var orderToModify = _context.Orders.Include("OrderItems").Include("OrderItems.Album").SingleOrDefault(o => o.OrderId == orderId && o.LastName == lastName);

			if (orderToModify == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			OrderShippedEmail email = new OrderShippedEmail
			{
				To = orderToModify.Email,
				OrderId = orderToModify.OrderId,
				FullAddress = string.Format("{0} {1}, {2}, {3}", orderToModify.FirstName, orderToModify.LastName, orderToModify.Address, orderToModify.CodeAndCity)
			};
			email.Send();

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[AllowAnonymous]
		public ActionResult SendConfirmationEmail(int orderId, string lastName)
		{
			// orderid and lastname as a basic form of auth

			// Also might be called by scheduler (ie. Azure scheduler), pinging endpoint and using some kind of queue / db

			// This could also be used (but problems when hosted on Azure Websites)
			// if (Request.IsLocal)

			var order = _context.Orders.Include("OrderItems").Include("OrderItems.Album").SingleOrDefault(o => o.OrderId == orderId && o.LastName == lastName);

			if (order == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			OrderConfirmationEmail email = new OrderConfirmationEmail
			{
				To = order.Email,
				Cost = order.TotalPrice,
				OrderNumber = order.OrderId,
				FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity),
				OrderItems = order.OrderItems,
				CoverPath = AppConfig.PhotosFolderRelative
			};
			email.Send();

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[Authorize(Roles = "Admin")]
		public ActionResult AddProduct(int? albumId, bool? confirmSuccess)
		{
			ViewBag.EditMode = albumId.HasValue;

			var result = new EditProductViewModel
			{
				Genres = _context.Genres.ToArray(),
				ConfirmSuccess = confirmSuccess,
				Album = albumId.HasValue ? _context.Albums.Find(albumId) : new Album()
			};

			return View(result);
		}

		[HttpPost]
		public ActionResult AddProduct(HttpPostedFileBase file, EditProductViewModel model)
		{
			// Saving existing entry
			if (model.Album.AlbumId > 0)
			{
				_context.Entry(model.Album).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("AddProduct", new { confirmSuccess = true });
			}
			// Creating new entry
			else
			{
				var f = Request.Form;
				// Verify that the user selected a file
				if (file != null && file.ContentLength > 0)
				{
					// Generate filename
					var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
					var path = Path.Combine(Server.MapPath(AppConfig.PhotosFolderRelative), filename);
					file.SaveAs(path);

					// Save info to DB
					model.Album.CoverFileName = filename;
					model.Album.DateAdded = DateTime.Now;

					_context.Entry(model.Album).State = EntityState.Added;
					_context.SaveChanges();

					return RedirectToAction("AddProduct", new { confirmSuccess = true });
				}
				else
				{
					ModelState.AddModelError("", "Nie wskazano pliku.");
					var genres = _context.Genres.ToArray();
					model.Genres = genres;
					return View(model);
				}
			}
		}

		public ActionResult HideProduct(int albumId)
		{
			var album = _context.Albums.Find(albumId);
			album.IsHidden = true;
			_context.SaveChanges();
			return RedirectToAction("AddProduct", new { confirmSuccess = true });
		}

		public ActionResult UnhideProduct(int albumId)
		{
			var album = _context.Albums.Find(albumId);
			album.IsHidden = false;
			_context.SaveChanges();
			return RedirectToAction("AddProduct", new { confirmSuccess = true });
		}

		private bool HasPassword()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			return user != null && user.PasswordHash != null;
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("password-error", error);
			}
		}
	}
}