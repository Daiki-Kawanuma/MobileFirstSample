using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;
using System.Diagnostics;
using System.Threading.Tasks;
using Prism.Services;
using Worklight;
using Worklight.Push;
using Newtonsoft.Json.Linq;

namespace MobileFirstSample.ViewModels
{
	public class MainPageViewModel : BindableBase
	{
		public ReactiveProperty<bool> IsVisibleIndicator { get; set; }
		public ReactiveProperty<CommandItem> SelectedItem { get; set; }
		public List<CommandItem> CommandItemList { get; set; }

		private readonly INavigationService _navigationService;
		private readonly IPageDialogService _pageDidalogService;
		private readonly SampleClient _sampleClient;

		public MainPageViewModel(INavigationService navigationService,
								 IPageDialogService pageDailogService,
								 IWorklightClient worklightClient,
								 IWorklightPush worklightPush)
		{
			_navigationService = navigationService;
			_pageDidalogService = pageDailogService;

			_sampleClient = new SampleClient(worklightClient, worklightPush);

			IsVisibleIndicator = new ReactiveProperty<bool> { Value = false };
			SelectedItem = new ReactiveProperty<CommandItem>();
			SelectedItem.PropertyChanged += (sender, e) =>
			{
				if (SelectedItem.Value != null)
				{
					//SelectedItem.Value.Command();
				}
				SelectedItem.Value = null;
			};

			CommandItemList = new List<CommandItem>
			{
				new CommandItem { CommandName = "Call unprotected adapter", Image = "invoke.png", Command = OnUnprotectedInvoke },
				new CommandItem { CommandName = "Call protected adapter", Image = "invoke.png", Command =  OnProtectedInvoke },
				new CommandItem { CommandName = "Register Device", Image = "subscribe.png", Command = OnRegister },
				new CommandItem { CommandName = "Unregister Device", Image = "unsubscribe.png", Command = OnUnregister },
				new CommandItem { CommandName = "Subscribe to Push", Image = "subscribe.png", Command = OnSubscribe },
				new CommandItem { CommandName = "Unsubscribe from Push", Image = "unsubscribe.png", Command = OnUnSubscribe },
				new CommandItem { CommandName = "Get All Subscriptions", Image = "issubscribed.png", Command = OnGetSubscriptions },
				new CommandItem { CommandName = "Get All Tags", Image = "issubscribed.png", Command = OnGetTags },
				new CommandItem { CommandName = "JSONStore destroy", Image = "logactivity.png", Command = OnDestroyJSONStore },
				new CommandItem { CommandName = "Open JSONStore Collection", Image = "logactivity.png", Command = OnJSONStoreOpenCollection },
				new CommandItem { CommandName = "Add Data to JSONStore", Image = "logactivity.png", Command = OnAddDataToJSONStore },
				new CommandItem { CommandName = "Retrieve all Data", Image = "logactivity.png", Command = OnRetrieveAllDataFromJSONStore },
				new CommandItem { CommandName = "Retrieve Filtered Data", Image = "logactivity.png", Command = OnRetrieveFilteredDataFromJSONStore }
			};
		}

		private async void OnUnprotectedInvoke()
		{
			ShowWorking();
			var result = await _sampleClient.UnprotectedInvokeAsync();
			HandleResult(result, "Unprotected adapter invocation Result", true);
		}

		private async void OnProtectedInvoke()
		{
			ShowWorking();
			var result = await _sampleClient.ProtectedInvokeAsync();
			HandleResult(result, "Protected adapter invocation Result", true);
		}

		private async void OnRegister()
		{
			ShowWorking();
			var result = await _sampleClient.RegisterAsync();
			HandleResult(result, "Register Device Result", true);
		}

		private async void OnUnregister()
		{
			ShowWorking();
			var result = await _sampleClient.UnregisterAsync();
			HandleResult(result, "Unregister Device Result", true);
		}

		private async void OnSubscribe()
		{
			ShowWorking();
			var result = await _sampleClient.SubscribeAsync();
			HandleResult(result, "Subscribe Result");
		}

		private async void OnUnSubscribe()
		{
			ShowWorking();
			var result = await _sampleClient.UnSubscribeAsync();
			HandleResult(result, "Unsubcribe Result", true);
		}

		private async void OnGetSubscriptions()
		{
			ShowWorking();
			var result = await _sampleClient.GetSubscriptionsAsync();
			HandleResult(result, "Get Subscriptions Result", true);
		}

		private async void OnGetTags()
		{
			ShowWorking();
			var result = await _sampleClient.GetTagsAsync();
			HandleResult(result, "Get Tags Result", true);
		}

		private void OnIsPushEnabled()
		{
			_pageDidalogService.DisplayAlertAsync("Push Status",
												  $"Push {(_sampleClient.IsPushSupported ? "is" : "isn't")} supported",
												  "OK");
		}

		private void OnDestroyJSONStore()
		{
			_pageDidalogService.DisplayAlertAsync("Destroy JSONStore Status",
												  $"JSONStore was {(_sampleClient.Client.JSONStoreService.JSONStore.Destroy() ? "successfully" : "not")} destroyed",
												  "OK");
		}

		private void OnJSONStoreOpenCollection()
		{
			var collectionList = new List<WorklightJSONStoreCollection>();
			var collection = _sampleClient.Client.JSONStoreService.JSONStoreCollection("people");
			var searchFieldDict = new Dictionary<string, WorklightJSONStoreSearchFieldType>();
			//var searchFied = _sampleClient.Client.JSONStoreService.JSONStore.se;
			searchFieldDict.Add("name", WorklightJSONStoreSearchFieldType.String);
			collection.SearchFields = searchFieldDict;
			searchFieldDict = collection.SearchFields;
			collectionList.Add(collection);

			_pageDidalogService.DisplayAlertAsync("JSONStore Open Collection Status",
												  $"JSONStore Person Collection was {(_sampleClient.Client.JSONStoreService.JSONStore.OpenCollections(collectionList) ? "successfully" : "not")} opened",
												  "OK");
		}

		private void OnAddDataToJSONStore()
		{
			var data = new JArray();
			var val = JObject.Parse(" {\"name\" : \"Chethan\", \"laptop\" : 23.546} ");
			var val2 = JObject.Parse(" {\"name\" : \"Ajay\", \"laptop\" : [ \"hellow\", 234.23, true] } ");
			var val3 = JObject.Parse(" {\"name\" : \"Srihari\", \"laptop\" : true} ");
			data.Add(val);
			data.Add(val2);
			data.Add(val3);
			var collection = _sampleClient.Client.JSONStoreService.JSONStore.GetCollectionByName("people");
			if (collection != null)
				collection.AddData(data);
			else
				_pageDidalogService.DisplayAlertAsync("JSONStore addData",
													  "Open JSONstore collection before attempting to add data",
													  "OK");
		}

		private void OnRetrieveAllDataFromJSONStore()
		{
			var collection = _sampleClient.Client.JSONStoreService.JSONStore.GetCollectionByName("people");
			if (collection != null)
			{
				var outArr = collection.FindAllDocuments();
				_pageDidalogService.DisplayAlertAsync("All JSONStore Data",
													  $"JSONStore Person data is: {outArr.ToString()}",
													  "OK");
			}
			else
				_pageDidalogService.DisplayAlertAsync("JSONStore RetrieveData",
													  "Open JSONstore collection before attempting to retrieve data",
													  "OK");
		}

		private void OnRetrieveFilteredDataFromJSONStore()
		{
			//			JsonObject outArr = App.WorklightClient.client.JSONStoreService.JSONStoreCollection("people").FindDocumentByID(1);
			var queryParts = new WorklightJSONStoreQueryPart[1];
			queryParts[0] = _sampleClient.Client.JSONStoreService.JSONStoreQueryPart();
			queryParts[0].AddLike("name", "Chethan");
			var collection = _sampleClient.Client.JSONStoreService.JSONStore.GetCollectionByName("people");
			if (collection != null)
			{
				var outArr = collection.FindDocumentsWithQueryParts(queryParts);
				_pageDidalogService.DisplayAlertAsync("Filtered JSONStore Data",
													  $"JSONStore Person data is {(outArr != null ? outArr.ToString() : "not available")}",
													  "OK");
			}
			else
				_pageDidalogService.DisplayAlertAsync("JSONStore RetrieveData",
													  "Open JSONstore collection before attempting to retrieve data",
													  "OK");
		}

		private async void ShowWorking(int timeout = 15)
		{
			IsVisibleIndicator.Value = true;

			await Task.Delay(TimeSpan.FromSeconds(timeout));

			if (IsVisibleIndicator.Value == true)
			{
				IsVisibleIndicator.Value = false;
				await _pageDidalogService.DisplayAlertAsync("Timeout", "Call timed out", "OK");
			}
		}

		private void HandleResult(WorklightResult result, string title, bool ShowOnSuccess = false)
		{
			IsVisibleIndicator.Value = false;

			if (result.Success)
			{
				if (ShowOnSuccess)
				{
					var navigationParameters = new NavigationParameters
					{
						{ "title", title },
						{"result", result}
					};
					_navigationService.NavigateAsync("ResultPage", navigationParameters);
				}
				else
				{
					_pageDidalogService.DisplayAlertAsync(title, result.Message, "OK");
				}
			}
			else
			{
				var navigationParameters = new NavigationParameters
					{
						{ "title", title },
						{"result", result}
					};
				_navigationService.NavigateAsync("ResultPage", navigationParameters);
			}
		}
	}
}

