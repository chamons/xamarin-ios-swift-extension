# NET6 iOS Sample

This sample has multiple improvements over the legacy Xamarin.iOS samp,le:

## Background Widget Updates

Using `BGAppRefreshTaskRequest` we can update our widget's serialized state with a background thread, instead of only updating when the Application is running.

As the API is a complex, requiring chaining usages of `NSOperation` and `BGAppRefreshTask` in a non-trivial way.

A `SimpleBackgroundTask` wrapper is included in this sample that may be a useful abstraction.

A few notes:

- Both `BGTaskSchedulerPermittedIdentifiers` and `UIBackgroundModes` keys in Info.plist are required for use of the background API
- The `BGAppRefreshTaskRequest` API is not supported in Simulator and will output `Error in scheduling app refresh: The operation couldn’t be completed. (BGTaskSchedulerErrorDomain error 1.)`
- The debugging aid `ForceRefresh` on `SimpleBackgroundTask` uses a debug only selector `_simulateLaunchForTaskWithIdentifier:` that Apple will reject in App Store submissions.

## Widget Reloading (Prototype Swift Support!)

Updating the serialized state in a shared location is insufficient however, as iOS will only invoke your SwiftUI widget code whenever it sees fit by default, which may be hours to days later. The solution is to invoke a `WidgetKit` API such as `WidgetCenter.shared.reloadAllTimelines`. These APIs unfortunately are Swift-only, and can not be called from a C# p/invoke due to the ABI required by Swift.

A new **prototype** technique for embedding simplified Swift APIs is included in this sample, via the CompileSwift.targets file.

With it, files with the .swift extension are compiled into a static library and included in the final native link of your application. Public APIs, which follows some rules to be noted shortly, can be invoked via a p/invoke such as:

```csharp
    [DllImport ("__Internal", EntryPoint = "ReloadWidgets")]
    public extern static int ReloadWidgets ();
```

As this is a **prototype** technique, there are a number of limitations to be aware of:

- Public free functions to be called from C# must be marked with the undocumented `@_cdecl` attribute to define the EntryPoint for the DllImport.
- The arguments and return value of these functions are currently limited to things that can be expressed "like C".
- Today this does not include strings or types such as `UIView *`. Note: This is an area of active research by the macios team.
- There is no debugging support for debugging the Swift code today. Consider using Swift `debugPrint` calls.

Please report back successes or errors to the macios team in [the tracking issue](https://github.com/xamarin/xamarin-macios/issues/15315) so we can improve the technology.
