using MusicStore.NetFramework.WebApp.Models;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public interface IMailService
	{
		void SendOrderConfirmationEmail(Order order);
		void SendOrderShippedEmail(Order order);
	}
}