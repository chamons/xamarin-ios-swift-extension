using Foundation;
using System;
using UIKit;

namespace TestApplication
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		UITextView text;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Imagine this using https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/backgrounding/ios-backgrounding-techniques/updating-an-application-in-the-background
			// instead of just on launch

			text = new UITextView (new CoreGraphics.CGRect (200, 200, 200, 100));
			text.Text = "Before";
			View.AddSubview (text);

			NSUrl url = NSFileManager.DefaultManager.GetContainerUrl ("group.com.xamarin.sample.TestApplication");
			url = url.Append ("testAppState.json", false);
			System.IO.File.WriteAllText (url.Path, TestData.GetJson ());
			text.Text = "After";

			text.Text = System.IO.File.Exists (url.Path) ? ("Exists - " + url.Path) : "??";
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}