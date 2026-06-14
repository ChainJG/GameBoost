using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GameBoost.Infrastructure.UserInput
{
    public static class UserInputNativeMethods
    {
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        private const uint SPI_SETKEYBOARDSPEED = 0x000B;
        private const uint SPI_SETKEYBOARDDELAY = 0x0017;
        private const uint SPI_SETMOUSE = 0x0004;
        private const uint SPI_SETMOUSETRAILS = 0x005D;
        private const uint SPI_SETMOUSESPEED = 0x0071;
        private const uint SPI_SETWHEELSCROLLLINES = 0x0069;

        private const uint Flags =
            SPIF_UPDATEINIFILE | SPIF_SENDCHANGE;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            IntPtr pvParam,
            uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            int[] pvParam,
            uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetDoubleClickTime(
            uint milliseconds);

        public static void SetKeyboardDelay(int value)
        {
            ThrowIfFailed(
                SystemParametersInfo(
                    SPI_SETKEYBOARDDELAY,
                    (uint)Math.Clamp(value, 0, 3),
                    IntPtr.Zero,
                    Flags));
        }

        public static void SetKeyboardSpeed(int value)
        {
            ThrowIfFailed(
                SystemParametersInfo(
                    SPI_SETKEYBOARDSPEED,
                    (uint)Math.Clamp(value, 0, 31),
                    IntPtr.Zero,
                    Flags));
        }

        public static void SetMousePointerSpeed(int value)
        {
            ThrowIfFailed(
                SystemParametersInfo(
                    SPI_SETMOUSESPEED,
                    0,
                    new IntPtr(Math.Clamp(value, 1, 20)),
                    Flags));
        }

        public static void SetEnhancePointerPrecision(bool enabled)
        {
            var values = enabled
                ? new[] { 6, 10, 1 }
                : new[] { 0, 0, 0 };

            ThrowIfFailed(
                SystemParametersInfo(
                    SPI_SETMOUSE,
                    0,
                    values,
                    Flags));
        }

        public static void SetMouseTrails(int value)
        {
            ThrowIfFailed(
                SystemParametersInfo(
                    SPI_SETMOUSETRAILS,
                    (uint)Math.Clamp(value, 0, 7),
                    IntPtr.Zero,
                    Flags));
        }

        public static void SetWheelScrollLines(int value)
        {
            ThrowIfFailed(
                SystemParametersInfo(
                    SPI_SETWHEELSCROLLLINES,
                    (uint)Math.Clamp(value, 0, 20),
                    IntPtr.Zero,
                    Flags));
        }

        public static void SetDoubleClickSpeed(int milliseconds)
        {
            var value = (uint)Math.Clamp(milliseconds, 200, 900);

            if (!SetDoubleClickTime(value))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        private static void ThrowIfFailed(bool success)
        {
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}