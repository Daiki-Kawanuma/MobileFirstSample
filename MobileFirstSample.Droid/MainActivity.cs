using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Worklight;
using Worklight.Xamarin.Android;
using Worklight.Push;
using Worklight.Xamarin.Android.Push;

namespace MobileFirstSample.Droid
{
	[Activity(Label = "MobileFirstSample.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App(new AndroidInitializer { Activity = this }));
		}
	}

	public class AndroidInitializer : IPlatformInitializer
	{
		public Activity Activity { get; set; }

		public void RegisterTypes(IUnityContainer container)
		{
			container.RegisterInstance<IWorklightClient>(WorklightClient.CreateInstance(Activity));
			container.RegisterInstance<IWorklightPush>(WorklightPush.Instance);
		}
	}
}
