using Prism.Unity;
using MobileFirstSample.Views;
using Xamarin.Forms;

namespace MobileFirstSample
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer) { }

		protected async override void OnInitialized()
		{
			InitializeComponent();
			await NavigationService.NavigateAsync("NavigationPage/MainPage");
		}

		protected override void RegisterTypes()
		{
			Container.RegisterTypeForNavigation<NavigationPage>();
			Container.RegisterTypeForNavigation<MainPage>();
		}
	}
}

