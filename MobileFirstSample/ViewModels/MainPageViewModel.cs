using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;
using System.Diagnostics;

namespace MobileFirstSample.ViewModels
{
	public class MainPageViewModel : BindableBase
	{
		public ReactiveProperty<bool> IsVisibleIndicator { get; set; }
		public ReactiveProperty<CommandItem> SelectedItem { get; set; }
		public List<CommandItem> CommandItemList { get; set; }

		public MainPageViewModel()
		{
			IsVisibleIndicator = new ReactiveProperty<bool> { Value = false };
			SelectedItem = new ReactiveProperty<CommandItem>();
			SelectedItem.PropertyChanged += (sender, e) =>
			{
				if (SelectedItem.Value != null)
				{
					Debug.WriteLine(SelectedItem.Value.CommandName);
				}
				SelectedItem.Value = null;
			};

			CommandItemList = new List<CommandItem>
			{
				new CommandItem { CommandName = "Call unprotected adapter", Image = "invoke.png" },
				new CommandItem { CommandName = "Call protected adapter", Image = "invoke.png" },
				new CommandItem { CommandName = "Register Device", Image = "subscribe.png" },
				new CommandItem { CommandName = "Unregister Device", Image = "unsubscribe.png" },
				new CommandItem { CommandName = "Subscribe to Push", Image = "subscribe.png" },
				new CommandItem { CommandName = "Unsubscribe from Push", Image = "unsubscribe.png" },
				new CommandItem { CommandName = "Get All Subscriptions", Image = "issubscribed.png" },
				new CommandItem { CommandName = "Get All Tags", Image = "issubscribed.png" },
				new CommandItem { CommandName = "JSONStore destroy", Image = "logactivity.png" },
				new CommandItem { CommandName = "Open JSONStore Collection", Image = "logactivity.png" },
				new CommandItem { CommandName = "Add Data to JSONStore", Image = "logactivity.png" },
				new CommandItem { CommandName = "Retrieve all Data", Image = "logactivity.png" },
				new CommandItem { CommandName = "Retrieve Filtered Data", Image = "logactivity.png" }
			};
		}
	}
}

