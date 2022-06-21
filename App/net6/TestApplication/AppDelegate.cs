using System.Text.Json;
using System.Runtime.InteropServices;

namespace TestApplication;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

    [DllImport ("__Internal", EntryPoint = "ReloadWidgets")]
    public extern static int ReloadWidgets ();

	SimpleBackgroundTask? WidgetUpdateTask;

	// This Application uses BGAppRefreshTaskRequest via a SimpleBackgroundTask wrapper
	// to serialize state in a shared json file stored in a container that is accessible by 
	// the SwiftUI based Widget
	//
	// Consider serializating your state on application exit, as it may be a 
	// significant amount of time before the OS runs your refresh task
	//
	// NOTE - SimpleBackgroundTask WILL NOT RUN on iOS simulator. Another good reason to not depend
	// only on background updating of your shared state
	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		WidgetUpdateTask = new SimpleBackgroundTask ("com.xamarin.sample.TestApplication.UpdateWidget", WriteWidgetState, verbose: true);
		WidgetUpdateTask.Schedule ();
		// This is a debugging aid, forcing an update after 5 seconds
		WidgetUpdateTask.ScheduleForceRefresh (5000);

		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);
		
		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			Text = $"Loaded",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();
		return true;
	}

	public override void DidEnterBackground (UIApplication application)
	{
		WriteWidgetState ();
	}

	void WriteWidgetState ()
	{
		NSUrl url = NSFileManager.DefaultManager.GetContainerUrl ("group.com.xamarin.sample.TestApplication");
		url = url.Append ("testAppState.json", false);
		System.IO.File.WriteAllText (url.Path, TestData.GetJson ());
		ReloadWidgets ();
	}
}

public partial class Values
{
	public Dictionary<string, Datum> Data { get; set; }
}

public partial class Datum
{
	public string Value { get; set; }
	public string Delta { get; set; }
}

public static class TestData
{
	public static string GetJson ()
	{
		// Go go fake business logic!
		Values v = new Values {
			Data = new Dictionary<string, Datum> {
				{ "2020-07-01",  new Datum { Value = "50.34", Delta = "-1.68"} },
				{ "2020-07-02",  new Datum { Value = "51.99", Delta = "-0.03"} },
				{ "2020-07-03",  new Datum { Value = "51.56", Delta = "-0.46"} },
			}
		};

		var rng = new Random ();
		v.Data [DateTime.Now.ToString("yyyy-MM-dd")] = new Datum { Value = rng.NextDouble ().ToString ("0.##"), Delta = rng.NextDouble ().ToString ("0.##")};

		var serializeOptions = new JsonSerializerOptions {
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};
		return JsonSerializer.Serialize (v, serializeOptions);
	}
}