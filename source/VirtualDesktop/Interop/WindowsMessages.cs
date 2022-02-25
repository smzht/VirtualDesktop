using System;
// ReSharper disable InconsistentNaming

namespace WindowsDesktop.Interop
{
	/// <summary>
	/// Windows Messages
	/// Defined in winuser.h from Windows SDK v6.1
	/// Documentation pulled from MSDN.
	/// </summary>
	public enum WindowsMessages : uint
	{
		/// <summary>
		/// The WM_KILLFOCUS message is sent to a window immediately before it loses the keyboard focus. 
		/// </summary>
		WM_KILLFOCUS = 0x0008,
	}
}
