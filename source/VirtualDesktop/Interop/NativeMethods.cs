using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDesktop.Interop
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern uint RegisterWindowMessage(string lpProcName);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ChangeWindowMessageFilterEx(IntPtr hwnd, uint message, uint action, ref CHANGEFILTERSTRUCT pChangeFilterStruct);

		[DllImport("user32.dll")]
		public static extern bool CloseWindow(IntPtr hWnd);

		public static void ForceSetForegroundWindow(IntPtr targetHandle)
		{
			const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
			const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
			const int SPIF_SENDCHANGE = 0x2;

			var targetThreadId = GetWindowThreadProcessId(targetHandle, out var _);
			var currentActiveThreadId = GetWindowThreadProcessId(GetForegroundWindow(), out var _);

			if (targetThreadId == currentActiveThreadId)
			{
				SetForegroundWindow(targetHandle);
				BringWindowToTop(targetHandle);
			}
			else
			{
				var dummy = IntPtr.Zero;
				var timeout = IntPtr.Zero;
				AttachThreadInput((uint)targetThreadId, (uint)currentActiveThreadId, true);
				try
				{
					SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, timeout, 0);
					SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, dummy, SPIF_SENDCHANGE);
					SetForegroundWindow(targetHandle);
					BringWindowToTop(targetHandle);
				}
				finally
				{
					if (timeout != IntPtr.Zero)
					{
						SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, timeout, SPIF_SENDCHANGE);
					}
					AttachThreadInput((uint)targetThreadId, (uint)currentActiveThreadId, false);
				}
			}
		}

		public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);
	}

	public enum MessageFilter : uint
	{
		MSGFLT_RESET = 0,
		MSGFLT_ALLOW = 1,
		MSGFLT_DISALLOW = 2
	};

	public enum MessageFilterInfo : uint
	{
		MSGFLTINFO_NONE = 0,
		MSGFLTINFO_ALREADYALLOWED_FORWND = 1,
		MSGFLTINFO_ALREADYDISALLOWED_FORWND = 2,
		MSGFLTINFO_ALLOWED_HIGHER = 3,
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct CHANGEFILTERSTRUCT
	{
		public uint cbSize;
		public MessageFilterInfo ExtStatus;
	}
}
