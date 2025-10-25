namespace UIFramework.Window
{
    public struct WindowHistoryEntry
    {
        public IWindowController Controller;

        public WindowHistoryEntry(IWindowController controller)
        {
            this.Controller = controller;
        }
    }
}