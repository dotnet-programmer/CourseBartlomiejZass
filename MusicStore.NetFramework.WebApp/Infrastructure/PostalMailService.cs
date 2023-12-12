using MusicStore.NetFramework.WebApp.ViewModels;

namespace MusicStore.NetFramework.WebApp.Infrastructure
{
	public class PostalMailService : IMailService
	{
		public void SendOrderConfirmationEmail(Models.Order order)
		{
			// Strongly typed - without background
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
		}

		public void SendOrderShippedEmail(Models.Order order)
		{
			OrderShippedEmail email = new OrderShippedEmail
			{
				To = order.Email,
				OrderId = order.OrderId,
				FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity)
			};
			email.Send();
		}
	}
}