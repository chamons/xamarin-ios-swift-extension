using System.Text.Json;
using BackgroundTasks;

#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace TestApplication;

// A simple wrapper around BGAppRefreshTaskRequest.
// 
// RefreshAction should complete within seconds at most
// Consider porting this wrapper to BGProcessingTaskRequest
// if more time is required.
// 
// NOTE:
//   Requires BGTaskSchedulerPermittedIdentifiers and UIBackgroundModes keys in Info.plist
public class SimpleBackgroundTask
{
    string BackgroundTaskID;
    Action RefreshAction;
    bool Verbose;
    NSDate? EarliestBeginDate;

    public SimpleBackgroundTask (string backgroundTaskID, Action refreshAction, NSDate? earliestBeginDate = null, bool verbose = false)
    {
        BackgroundTaskID = backgroundTaskID;
        RefreshAction = refreshAction;
        Verbose = verbose;
        EarliestBeginDate = earliestBeginDate;

        BGTaskScheduler.Shared.Register (BackgroundTaskID, null, task => HandleAppRefresh ((BGAppRefreshTask)task));
    }

    public void Schedule ()
	{
		var request = new BGAppRefreshTaskRequest (BackgroundTaskID);
        if (EarliestBeginDate != null) {
            request.EarliestBeginDate = EarliestBeginDate;
        }
		BGTaskScheduler.Shared.Submit (request, out NSError error);

        if (Verbose) {
            if (error != null) {
                Console.Error.WriteLine ($"Error in scheduling app refresh: {error}");
            } else {
                Console.Error.WriteLine ("Scheduled app refresh");
            }
        }
	}

	void HandleAppRefresh (BGAppRefreshTask task)
	{
		Schedule ();

		var queue = new NSOperationQueue ();
		var operation = new WidgetRefreshOperation (RefreshAction, Verbose);
		task.ExpirationHandler = () => { 
			queue.CancelAllOperations();
		};
		operation.CompletionBlock = () => {
			task.SetTaskCompleted (!operation.IsCancelled);
		};

		queue.AddOperation (operation);
	}

#if DEBUG // Apple will reject application which include _simulateLaunchForTaskWithIdentifier selector
    public async void ScheduleForceRefresh (int ms)
    {
        Task.Run(async () => {
            await Task.Delay(ms);
            ForceRefresh ();
        });
    }

    // https://developer.apple.com/documentation/backgroundtasks/starting_and_terminating_tasks_during_development
	[DllImport ("/usr/lib/libobjc.dylib", EntryPoint="objc_msgSend")]
	public extern static void void_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1);

	public void ForceRefresh ()
	{
		using (var taskId = new NSString (BackgroundTaskID)) { 
			var method = new ObjCRuntime.Selector ("_simulateLaunchForTaskWithIdentifier:");
			void_objc_msgSend_IntPtr (BGTaskScheduler.Shared.Handle, method.Handle, taskId.Handle);
		}
	}
#endif

    class WidgetRefreshOperation : NSOperation
    {
        Action RefreshAction;
        bool Verbose;

        public WidgetRefreshOperation (Action refreshAction, bool verbose = false)
        {
            RefreshAction = refreshAction;
            Verbose = verbose;
        }

        public override void Main ()
        {
            if (Verbose)
                Console.Error.WriteLine ("Widget Refresh Running");
            RefreshAction ();
            if (Verbose)
                Console.Error.WriteLine ("Widget Refresh Complete");
        }
    }
}