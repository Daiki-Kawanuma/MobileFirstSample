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
		private string _pushAlias = "myAlias2";
		private string _appRealm = "UserLogin";
		private JObject _metadata = JObject.Parse(" {\"platform\" : \"Xamarin\" } ");
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
		public SampleClient(IWorklightClient wlc, IWorklightPush push)
		{
			Client = wlc;
			Push = push;
			SecurityCheckChallengeHandler customCH = new CustomChallengeHandler(_appRealm);
			Client.RegisterChallengeHandler(customCH);
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

				WorklightResourceRequest rr = Client.ResourceRequest(new Uri(uriBuilder.ToString(), 
		                                                             UriKind.Relative), 
				                                                     "GET");

				WorklightResponse resp = await rr.Send();

				result.Success = resp.Success;
				//result.Message = (resp.Success) ? "Connected" : resp.Message;
				result.Response = resp.ResponseText;

			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
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

				WorklightResourceRequest rr = Client.ResourceRequest(new Uri(uriBuilder.ToString(), UriKind.Relative),
																	 "GET",
																	 "accessRestricted");

				WorklightResponse resp = await rr.Send();

				result.Success = resp.Success;
				//result.Message = (resp.Success) ? "Connected" : resp.Message;
				result.Response = resp.Success ? "Your account balance is " + resp.ResponseText : resp.Message;

			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<WorklightResult> SubscribeAsync()
		{
			var result = new WorklightResult();

			try
			{
				var resp = await Push.Subscribe(new string[] { "Xamarin" });
				Push.NotificationReceived += handleNotification;
				result.Success = resp.Success;
				result.Message = "Subscribed";
				result.Response = (resp.ResponseJSON != null) ? resp.ResponseJSON.ToString() : "";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<WorklightResult> RegisterAsync()
		{
			var result = new WorklightResult();

			try
			{
				var resp = await Push.RegisterDevice(new JObject());
				result.Success = resp.Success;
				result.Message = "Registered";
				result.Response = (resp.ResponseJSON != null) ? resp.ResponseJSON.ToString() : "";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<WorklightResult> UnregisterAsync()
		{
			var result = new WorklightResult();

			try
			{
				var resp = await Push.UnregisterDevice();
				result.Success = resp.Success;
				result.Message = "Unregistered";
				result.Response = (resp.ResponseJSON != null) ? resp.ResponseJSON.ToString() : "";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}
		public static void handleNotification(object sender, EventArgs e)
		{
			PushEventArgs eventArgs = (PushEventArgs)e;
			Debug.WriteLine("Notification received. Payload is " + eventArgs.Payload +
							  ". URL is " + eventArgs.Url);
			//HomePage._this.ShowAlert("Notified", eventArgs.Alert, "OK");
		}

		public async Task<WorklightResult> UnSubscribeAsync()
		{
			var result = new WorklightResult();

			try
			{
				var resp = await UnsubscribePush();

				result.Success = resp.Success;
				result.Message = "Unsubscribed";
				result.Response = (resp.ResponseJSON != null) ? resp.ResponseJSON.ToString() : "";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<WorklightResult> GetSubscriptionsAsync()
		{
			var result = new WorklightResult();

			try
			{
				var resp = await GetSubscriptions();

				result.Success = resp.Success;
				result.Message = "All Subscriptions";
				result.Response = (resp.ResponseJSON != null) ? resp.ResponseJSON.ToString() : "";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<WorklightResult> GetTagsAsync()
		{
			var result = new WorklightResult();

			try
			{
				var resp = await GetTags();

				result.Success = resp.Success;
				result.Message = "All tags";
				result.Response = (resp.ResponseJSON != null) ? resp.ResponseJSON.ToString() : "";
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;
		}

		#endregion

		#region Worklight Methods

		/// <summary>
		/// Unsubscribes from push notifications
		/// </summary>
		/// <returns>The push.</returns>
		private async Task<MFPPushMessageResponse> UnsubscribePush()
		{
			try
			{
				return await Push.Unsubscribe(new string[] { "xamarin " });
			}
			catch (Exception ex)
			{
				return null;
			}

		}

		/// <summary>
		/// Subscribes to push notifications
		/// </summary>
		/// <param name="callBack">Call back.</param>
		private async Task<MFPPushMessageResponse> SubscribePush()
		{
			Debug.WriteLine("Subscribing to push");
			return await Push.Subscribe(new string[] { "xamarin" });
		}

		private async Task<MFPPushMessageResponse> GetSubscriptions()
		{
			Debug.WriteLine("Getting list of subscriptions");
			return await Push.GetSubscriptions();
		}

		private async Task<MFPPushMessageResponse> GetTags()
		{
			Debug.WriteLine("Getting list of all tags");
			return await Push.GetTags();
		}


		/// <summary>
		/// Invokes the procedured
		/// </summary>
		/// <returns>The proc.</returns>
		private async Task<WorklightResult> InvokeProc()
		{
			var result = new WorklightResult();

			try
			{
				Client.Analytics.Log("trying to invoking procedure");
				System.Diagnostics.Debug.WriteLine("Trying to invoke proc");
				WorklightProcedureInvocationData invocationData = new WorklightProcedureInvocationData("SampleHTTPAdapter", "getFeed", new object[] { "technology" });
				WorklightResponse task = await Client.InvokeProcedure(invocationData);
				Client.Analytics.Log("invoke response : " + task.Success);
				StringBuilder retval = new StringBuilder();

				result.Success = task.Success;

				if (task.Success)
				{
					JArray jsonArray = (JArray)task.ResponseJSON["rss"]["channel"]["item"];
					foreach (JObject title in jsonArray)
					{
						JToken titleString;
						title.TryGetValue("title", out titleString);
						retval.Append(titleString.ToString());
						retval.AppendLine();
					}
				}
				else
				{
					retval.Append("Failure: " + task.Message);
				}

				result.Message = retval.ToString();
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return result;

		}
		#endregion
	}
}
