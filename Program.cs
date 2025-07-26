AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("Unhandled exception: " + e.ExceptionObject.ToString());
};

using var timer = new Timer(
    callback: _ => throw new InvalidOperationException("This is a test exception"),
    state: null,
    dueTime: TimeSpan.FromSeconds(10),
    period: Timeout.InfiniteTimeSpan
);

Console.WriteLine("Hello, World! Application will crash in 10 seconds...");
// Keep the application running
Thread.Sleep(Timeout.Infinite);

