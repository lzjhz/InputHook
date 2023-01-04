using System;

namespace InputHook.Hooks
{
    public class HookHandlerArgs
    {
        public int nCode;
        public IntPtr wParam;
        public IntPtr lParam;
        public bool freeze;

        public HookHandlerArgs(int nCode, IntPtr wParam, IntPtr lParam)
        {
            this.nCode = nCode;
            this.wParam = wParam;
            this.lParam = lParam;
            this.freeze = false;
        }
    }
}
