using System.Runtime.InteropServices;
using System.Windows;
using InputHook.Hooks;

namespace InputHook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Hook keyboardHook;
        Hook mouseHook;

        public MainWindow()
        {
            InitializeComponent();

            keyboardHook = new Hook(HookTypes.WH_KEYBOARD_LL);
            mouseHook = new Hook(HookTypes.WH_MOUSE_LL);

            bool ctrlPressed = false, altPressed = false;
            keyboardHook.AddHandler(delegate (object sender, HookHandlerArgs args)
            {
                if (args.nCode >= 0)
                {
                    int eventType = args.wParam.ToInt32();
                    if (eventType == MessageTypes.WM_KEYDOWN || eventType == MessageTypes.WM_SYSKEYDOWN || eventType == MessageTypes.WM_KEYUP)
                    {
                        int vkCode = Marshal.ReadInt32(args.lParam);

                        if (vkCode == 162) // Ctrl
                            ctrlPressed = eventType == MessageTypes.WM_KEYDOWN || eventType == MessageTypes.WM_SYSKEYDOWN;
                        else if (vkCode == 164) // Alt
                            altPressed = eventType == MessageTypes.WM_KEYDOWN || eventType == MessageTypes.WM_SYSKEYDOWN;

                        // Uninstall hooks when Ctrl+Alt+F is pressed
                        if (ctrlPressed && altPressed && eventType == MessageTypes.WM_KEYUP && vkCode == 70) // Ctrl+Alt+F
                        {
                            ResetHooks();
                        }

#if VERBOSE
                    Console.WriteLine("eventType=" + eventType + ", vkCode=" + vkCode);
#endif
                    }
                }
            });
        }

        public void ResetHooks()
        {
            if (keyboardHook.ContainsHandler(Hook.FreezeCallback))
                keyboardHook.RemoveHandler(Hook.FreezeCallback);
            if (mouseHook.ContainsHandler(Hook.FreezeCallback))
                mouseHook.RemoveHandler(Hook.FreezeCallback);
        }

        public void ApplyHooks()
        {
            if (freezeKeyboardCheckbox.IsChecked == true)
            {
                if (!keyboardHook.ContainsHandler(Hook.FreezeCallback))
                    keyboardHook.AddHandler(Hook.FreezeCallback);
            }
            else
            {
                if (keyboardHook.ContainsHandler(Hook.FreezeCallback))
                    keyboardHook.RemoveHandler(Hook.FreezeCallback);
            }
            if (freezeMouseCheckbox.IsChecked == true)
            {
                if (!mouseHook.ContainsHandler(Hook.FreezeCallback))
                    mouseHook.AddHandler(Hook.FreezeCallback);
            }
            else
            {
                if (mouseHook.ContainsHandler(Hook.FreezeCallback))
                    mouseHook.RemoveHandler(Hook.FreezeCallback);
            }
        }

        private void OnApply(object sender, RoutedEventArgs e)
        {
            ApplyHooks();
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            ResetHooks();
        }
    }
}
