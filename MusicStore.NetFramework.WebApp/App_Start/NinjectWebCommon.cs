[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MusicStore.NetFramework.WebApp.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MusicStore.NetFramework.WebApp.App_Start.NinjectWebCommon), "Stop")]

namespace MusicStore.NetFramework.WebApp.App_Start
{
	using System;
	using System.Web;

	using Microsoft.Web.Infrastructure.DynamicModuleHelper;
	using MusicStore.NetFramework.WebApp.Infrastructure;
	using Ninject;
	using Ninject.Web.Common;
	using Ninject.Web.Common.WebHost;

	// INFO - dodanie kontenera Dependency Injection - NuGet Ninject

	public static class NinjectWebCommon
	{
		private static readonly Bootstrapper _bootstrapper = new Bootstrapper();

		/// <summary>
		/// Starts the application.
		/// </summary>
		public static void Start()
		{
			DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
			DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
			_bootstrapper.Initialize(CreateKernel);
		}

		/// <summary>
		/// Stops the application.
		/// </summary>
		public static void Stop() => _bootstrapper.ShutDown();

		/// <summary>
		/// Creates the kernel that will manage your application.
		/// </summary>
		/// <returns>The created kernel.</returns>
		private static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			try
			{
				kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
				kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
				RegisterServices(kernel);
				return kernel;
			}
			catch
			{
				kernel.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Load your modules or register your services here!
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		private static void RegisterServices(IKernel kernel)
		{
			//kernel.Bind<IMailService>().To<BackgroundPostalMailService>();

			// INFO - Dependency Injection - ta metoda wskazuje, ¿e wszêdzie gdzie s¹ u¿ywane obiekty IMailService to klas¹ konkretn¹ bêdzie PostalMailService
			//kernel.Bind<IMailService>().To<PostalMailService>();

			kernel.Bind<IMailService>().To<HangFirePostalMailService>();
			kernel.Bind<ISessionManager>().To<SessionManager>();
			kernel.Bind<ICacheProvider>().To<DefaultCacheProvider>();
		}
	}
}