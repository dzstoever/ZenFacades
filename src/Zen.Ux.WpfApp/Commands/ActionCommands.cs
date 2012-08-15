using System.Windows.Input;

namespace Zen.Ux.WpfApp.Commands
{
    /// <summary>Class that defines and contains static commands  
    /// </summary>
    public static class ActionCommands
    {
        // Static routed commands (for menuing)
        public static RoutedUICommand LoginCommand { private set; get; }
        public static RoutedUICommand LogoutCommand { private set; get; }
        public static RoutedUICommand ExitCommand { private set; get; }

        public static RoutedUICommand AddCommand { private set; get; }
        public static RoutedUICommand EditCommand { private set; get; }
        public static RoutedUICommand DeleteCommand { private set; get; }
        public static RoutedUICommand ViewJobsCommand { private set; get; }

        public static RoutedUICommand HowDoICommand { private set; get; }
        public static RoutedUICommand IndexCommand { private set; get; }
        public static RoutedUICommand AboutCommand { private set; get; }

        /// <summary>Creates several Routed UI commands with and without shortcut keys.
        /// </summary>
        static ActionCommands()
        {
            // Initialize static commands
            LoginCommand = MakeRoutedUICommand("Login", Key.I, "Ctrl+I");
            LogoutCommand = MakeRoutedUICommand("Logout", Key.O, "Ctrl+O");
            ExitCommand = MakeRoutedUICommand("Exit");

            AddCommand = MakeRoutedUICommand("Add", Key.A, "Ctrl+A");
            EditCommand = MakeRoutedUICommand("Edit", Key.E, "Ctrl+E");
            DeleteCommand = MakeRoutedUICommand("Delete", Key.Delete, "Del");

            ViewJobsCommand = MakeRoutedUICommand("View Jobs");

            HowDoICommand = MakeRoutedUICommand("How Do I", Key.H, "Ctrl+D");
            IndexCommand = MakeRoutedUICommand("Index", Key.N, "Ctrl+N");
            AboutCommand = MakeRoutedUICommand("About");
        }

        /// <summary>Creates a routed command instance without shortcut key.
        /// </summary>
        private static RoutedUICommand MakeRoutedUICommand(string name)
        {
            return new RoutedUICommand(name, name, typeof(ActionCommands));
        }

        /// <summary>Creates a routed command instance with a shortcut key.
        /// </summary>
        private static RoutedUICommand MakeRoutedUICommand(string name, Key key, string displayString)
        {
            var gestures = new InputGestureCollection();
            gestures.Add(new KeyGesture(key, ModifierKeys.Control, displayString));

            return new RoutedUICommand(name, name, typeof(ActionCommands), gestures);
        }
    }
}
