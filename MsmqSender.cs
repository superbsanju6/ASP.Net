using System;
using Newtonsoft.Json;
using Thinkgate.Services.Contracts.UserSyncService;
using Thinkgate.UserSyncMessageSvcRef;
namespace Thinkgate
{
	public class MsmqSender
	{
		private const string MsmqBindingName = "NetMsmqBinding_IMessageService";

		/// <summary>
		/// Send the UserSyncMessage to the usersync queue
		/// </summary>
		/// <param name="userSyncMessage"></param>
		public void SendMessage(UserSyncMessage userSyncMessage)
		{
			var channel = new MessageServiceClient(MsmqBindingName);
			try
			{
				channel.ProcessUserSyncMessage(new Message { JsonMessage = JsonConvert.SerializeObject(userSyncMessage) });
				channel.Close();
			}
			catch (Exception)
			{
				channel.Abort();
			}
		}
	}
}