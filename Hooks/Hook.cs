using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InputHook.Hooks
{
    public class Hook
    {
        private IntPtr mHookPtr = IntPtr.Zero;

        public int HookType;

        public delegate void HookHandler(object sender, HookHandlerArgs args);

        private List<HookHandler> Handlers;
        private Helpers.NativeMethods.HookHandler MainHandler;

        public bool IsInstalled
        {
            get { return mHookPtr != IntPtr.Zero; }
        }

        public Hook(int hookType)
        {
            HookType = hookType;
            Handlers = new List<HookHandler>();
            MainHandler = MainHandlerCallback;
        }

        ~Hook()
        {
            Uninstall();
        }

        private IntPtr Install()
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
            {
                mHookPtr = Helpers.NativeMethods.SetWindowsHookEx(HookType, MainHandler, Helpers.NativeMethods.GetModuleHandle(module.ModuleName), 0);
                Console.WriteLine("Hook " + HookType + " installed. (ptr=" + mHookPtr + ")");
            }
            return mHookPtr;
        }

        private bool Uninstall()
        {
            bool bResult = Helpers.NativeMethods.UnhookWindowsHookEx(mHookPtr);
            Console.WriteLine("Hook " + HookType + " uninstalled. (ptr=" + mHookPtr + ")");
            mHookPtr = IntPtr.Zero;
            return bResult;
        }

        private IntPtr MainHandlerCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            HookHandlerArgs args = new HookHandlerArgs(nCode, wParam, lParam);
            for (int i = 0; i < Handlers.Count; i++)
                Handlers[i]?.Invoke(this, args);
            if (args.freeze)
                return (IntPtr)1;
            return Helpers.NativeMethods.CallNextHookEx(mHookPtr, nCode, wParam, lParam);
        }
        
        public void AddHandler(HookHandler handler)
        {
            Handlers.Add(handler);
            Console.WriteLine("Handler " + handler.Method.Name + " added to hook " + HookType + ".");
            if (!IsInstalled)
                Install();
        }

        public bool RemoveHandler(HookHandler handler)
        {
            bool removed = Handlers.Remove(handler);
            Console.WriteLine("Handler " + handler.Method.Name + " removed from hook " + HookType + ".");
            if (IsInstalled && Handlers.Count == 0)
                Uninstall();
            return removed;
        }

        public bool ContainsHandler(HookHandler handler)
        {
            return Handlers.Contains(handler);
        }

        public static void FreezeCallback(object sender, HookHandlerArgs args)
        {
            args.freeze = true;
        }
    }
}
