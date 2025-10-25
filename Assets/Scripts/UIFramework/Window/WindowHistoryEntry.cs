namespace UIFramework.Window
{
    public struct WindowHistoryEntry
    {
        public readonly IWindowController Controller;

        public WindowHistoryEntry(IWindowController controller)
        {
            this.Controller = controller;
        }
    }
}