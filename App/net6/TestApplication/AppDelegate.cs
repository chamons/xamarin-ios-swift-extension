using System.Text.Json;

namespace TestApplication;

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
		var serializeOptions = new JsonSerializerOptions {
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};
		return JsonSerializer.Serialize (v, serializeOptions);
	}
}

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		NSUrl url = NSFileManager.DefaultManager.GetContainerUrl ("group.com.xamarin.sample.TestApplication");
		url = url.Append ("testAppState.json", false);
		System.IO.File.WriteAllText (url.Path, TestData.GetJson ());

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			Text = "Data Written!",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}
