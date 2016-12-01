using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Worklight;
using Worklight.Push;

namespace MobileFirstSample
{
	public class SampleClient
	{
		#region Fields
		private string _appRealm = "UserLogin";
		#endregion

		#region Properties
		public IWorklightClient Client { get; private set; }
		public IWorklightPush Push { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is push supported.
		/// </summary>
		/// <value><c>true</c> if this instance is push supported; otherwise, <c>false</c>.</value>
		public bool IsPushSupported
		{
			get
			{
				try
				{
					return Push.IsPushSupported;
				}
				catch
				{
					return false;
				}
			}
		}
		#endregion

		#region Constuctors
		public SampleClient(IWorklightClient client, IWorklightPush push)
		{
			Client = client;
			Push = push;
			SecurityCheckChallengeHandler customChallengeHandler = new CustomChallengeHandler(_appRealm);
			Client.RegisterChallengeHandler(customChallengeHandler);
			push.Initialize();
		}
		#endregion

		#region Async functions
		public async Task<WorklightResult> UnprotectedInvokeAsync()
		{
			var result = new WorklightResult();
			try
			{
				var uriBuilder = new StringBuilder()
					.Append("/adapters")
					.Append("/ResourceAdapter") //Name of the adapter
					.Append("/publicData");    // Name of the adapter procedure

				WorklightResourceRequest resourceRequest = Client.ResourceRequest(new Uri(uriBuilder.ToString(), 
		                                                             UriKind.Relative), 
				                                                     "GET");

				WorklightResponse response = await resourceRequest.Send();

				result.Success = response.Success;
				//result.Message = (resp.Success) ? "Connected" : resp.Message;
				result.Response = response.ResponseText;

			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> ProtectedInvokeAsync()
		{
			var result = new WorklightResult();
			try
			{
				StringBuilder uriBuilder = new StringBuilder()
					.Append("/adapters")
					.Append("/ResourceAdapter") //Name of the adapter
					.Append("/balance");    // Name of the adapter procedure

				WorklightResourceRequest resourceRequest = Client.ResourceRequest(new Uri(uriBuilder.ToString(), UriKind.Relative),
																	 "GET",
																	 "accessRestricted");

				WorklightResponse response = await resourceRequest.Send();

				result.Success = response.Success;
				//result.Message = (resp.Success) ? "Connected" : resp.Message;
				result.Response = response.Success ? "Your account balance is " + response.ResponseText : response.Message;

			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> RegisterAsync()
		{
			var result = new WorklightResult();

			try
			{
				MFPPushMessageResponse response = await Push.RegisterDevice(new JObject());
				result.Success = response.Success;
				result.Message = "Registered";
				result.Response = (response.ResponseJSON != null) ? response.ResponseJSON.ToString() : "";
			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> UnregisterAsync()
		{
			var result = new WorklightResult();

			try
			{
				MFPPushMessageResponse response = await Push.UnregisterDevice();
				result.Success = response.Success;
				result.Message = "Unregistered";
				result.Response = (response.ResponseJSON != null) ? response.ResponseJSON.ToString() : "";
			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> SubscribeAsync()
		{
			var result = new WorklightResult();

			try
			{
				MFPPushMessageResponse response = await Push.Subscribe(new string[] { "Xamarin" });
				result.Success = response.Success;
				result.Message = "Subscribed";
				result.Response = (response.ResponseJSON != null) ? response.ResponseJSON.ToString() : "";
			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> UnSubscribeAsync()
		{
			var result = new WorklightResult();

			try
			{
				MFPPushMessageResponse response = await Push.Unsubscribe(new string[] { "Xamarin" });

				result.Success = response.Success;
				result.Message = "Unsubscribed";
				result.Response = (response.ResponseJSON != null) ? response.ResponseJSON.ToString() : "";
			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> GetSubscriptionsAsync()
		{
			var result = new WorklightResult();

			try
			{
				MFPPushMessageResponse response = await Push.GetSubscriptions();

				result.Success = response.Success;
				result.Message = "All Subscriptions";
				result.Response = (response.ResponseJSON != null) ? response.ResponseJSON.ToString() : "";
			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		public async Task<WorklightResult> GetTagsAsync()
		{
			var result = new WorklightResult();

			try
			{
				MFPPushMessageResponse response = await Push.GetTags();

				result.Success = response.Success;
				result.Message = "All tags";
				result.Response = (response.ResponseJSON != null) ? response.ResponseJSON.ToString() : "";
			}
			catch (Exception exception)
			{
				result.Success = false;
				result.Message = exception.Message;
			}

			return result;
		}

		#endregion
	}
}
