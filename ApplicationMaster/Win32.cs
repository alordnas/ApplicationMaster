using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Casamia
{
	class Win32
	{
		// Posted when the user presses the left mouse button while the cursor is within the nonclient area of a window  
		public const int WM_NCLBUTTONDOWN = 0x00A1;
		// Sent to a window in order to determine what part of the window corresponds to a particular screen coordinate  
		public const int WM_NCHITTEST = 0x0084;
		/// <summary>  
		/// Indicates the position of the cursor hot spot.  
		/// </summary>  
		public enum HitTest : int
		{
			/// <summary>  
			/// On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep to indicate an error).  
			/// </summary>  
			HTERROR = -2,

			/// <summary>  
			/// In a window currently covered by another window in the same thread (the message will be sent to underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).  
			/// </summary>  
			HTTRANSPARENT = -1,

			/// <summary>  
			/// On the screen background or on a dividing line between windows.  
			/// </summary>  
			HTNOWHERE = 0,

			/// <summary>  
			/// In a client area.  
			/// </summary>  
			HTCLIENT = 1,

			/// <summary>  
			/// In a title bar.  
			/// </summary>  
			HTCAPTION = 2,

			/// <summary>  
			/// In a window menu or in a Close button in a child window.  
			/// </summary>  
			HTSYSMENU = 3,

			/// <summary>  
			/// In a size box (same as HTSIZE).  
			/// </summary>  
			HTGROWBOX = 4,

			/// <summary>  
			/// In a size box (same as HTGROWBOX).  
			/// </summary>  
			HTSIZE = 4,

			/// <summary>  
			/// In a menu.  
			/// </summary>  
			HTMENU = 5,

			/// <summary>  
			/// In a horizontal scroll bar.  
			/// </summary>  
			HTHSCROLL = 6,

			/// <summary>  
			/// In the vertical scroll bar.  
			/// </summary>  
			HTVSCROLL = 7,

			/// <summary>  
			/// In a Minimize button.  
			/// </summary>  
			HTMINBUTTON = 8,

			/// <summary>  
			/// In a Minimize button.  
			/// </summary>  
			HTREDUCE = 8,

			/// <summary>  
			/// In a Maximize button.  
			/// </summary>  
			HTMAXBUTTON = 9,

			/// <summary>  
			/// In a Maximize button.  
			/// </summary>  
			HTZOOM = 9,

			/// <summary>  
			/// In the left border of a resizable window (the user can click the mouse to resize the window horizontally).  
			/// </summary>  
			HTLEFT = 10,

			/// <summary>  
			/// In the right border of a resizable window (the user can click the mouse to resize the window horizontally).  
			/// </summary>  
			HTRIGHT = 11,

			/// <summary>  
			/// In the upper-horizontal border of a window.  
			/// </summary>  
			HTTOP = 12,

			/// <summary>  
			/// In the upper-left corner of a window border.  
			/// </summary>  
			HTTOPLEFT = 13,

			/// <summary>  
			/// In the upper-right corner of a window border.  
			/// </summary>  
			HTTOPRIGHT = 14,

			/// <summary>  
			/// In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).  
			/// </summary>  
			HTBOTTOM = 15,

			/// <summary>  
			/// In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).  
			/// </summary>  
			HTBOTTOMLEFT = 16,

			/// <summary>  
			/// In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).  
			/// </summary>  
			HTBOTTOMRIGHT = 17,

			/// <summary>  
			/// In the border of a window that does not have a sizing border.  
			/// </summary>  
			HTBORDER = 18,

			/// <summary>  
			/// In a Close button.  
			/// </summary>  
			HTCLOSE = 20,

			/// <summary>  
			/// In a Help button.  
			/// </summary>  
			HTHELP = 21,
		};
		// Sent to a window when the size or position of the window is about to change  
		public const int WM_GETMINMAXINFO = 0x0024;
		// Retrieves a handle to the display monitor that is nearest to the window  
		public const int MONITOR_DEFAULTTONEAREST = 2;
		// RECT structure, Rectangle used by MONITORINFOEX  
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		// MONITORINFOEX structure, Monitor information used by GetMonitorInfo function  
		[StructLayout(LayoutKind.Sequential)]
		public class MONITORINFOEX
		{
			public int cbSize;
			public RECT rcMonitor; // The display monitor rectangle  
			public RECT rcWork; // The working area rectangle  
			public int dwFlags;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
			public char[] szDevice;
		}

		// Point structure, Point information used by MINMAXINFO structure  
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}
		// MINMAXINFO structure, Window's maximum size and position information  
		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize; // The maximized size of the window  
			public POINT ptMaxPosition; // The position of the maximized window  
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		}
		// Retrieves a handle to the display monitor  
		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);
		// Get the working area of the specified monitor  
		[DllImport("user32.dll")]
		public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX monitorInfo);
		// Sends the specified message to a window or windows  
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		internal static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

		internal static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam, MainWindow window)
		{
			// MINMAXINFO structure  
			Win32.MINMAXINFO mmi = (Win32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Win32.MINMAXINFO));

			// Get handle for nearest monitor to this window  
			WindowInteropHelper wih = new WindowInteropHelper(window);
			IntPtr hMonitor = Win32.MonitorFromWindow(wih.Handle, Win32.MONITOR_DEFAULTTONEAREST);

			// Get monitor info  
			Win32.MONITORINFOEX monitorInfo = new Win32.MONITORINFOEX();
			monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
			Win32.GetMonitorInfo(new HandleRef(window, hMonitor), monitorInfo);

			// Get HwndSource  
			HwndSource source = HwndSource.FromHwnd(wih.Handle);
			if (source == null)
				// Should never be null  
				throw new Exception("Cannot get HwndSource instance.");
			if (source.CompositionTarget == null)
				// Should never be null  
				throw new Exception("Cannot get HwndTarget instance.");

			// Get transformation matrix  
			Matrix matrix = source.CompositionTarget.TransformFromDevice;

			// Convert working area  
			Win32.RECT workingArea = monitorInfo.rcWork;
			Point dpiIndependentSize =
				matrix.Transform(new Point(
						workingArea.Right - workingArea.Left,
						workingArea.Bottom - workingArea.Top
						));

			// Convert minimum size  
			Point dpiIndenpendentTrackingSize = matrix.Transform(new Point(
				window.MinWidth,
				window.MinHeight
				));

			// Set the maximized size of the window  
			mmi.ptMaxSize.x = (int)dpiIndependentSize.X;
			mmi.ptMaxSize.y = (int)dpiIndependentSize.Y;

			// Set the position of the maximized window  
			mmi.ptMaxPosition.x = 0;
			mmi.ptMaxPosition.y = 0;

			// Set the minimum tracking size  
			mmi.ptMinTrackSize.x = (int)dpiIndenpendentTrackingSize.X;
			mmi.ptMinTrackSize.y = (int)dpiIndenpendentTrackingSize.Y;

			Marshal.StructureToPtr(mmi, lParam, true);
		}

		internal static IntPtr WmNCHitTest(IntPtr lParam, ref bool handled, MainWindow window)
		{
			// Update cursor point  
			// The low-order word specifies the x-coordinate of the cursor.  
			// #define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))  
			window.mousePoint.X = (int)(short)(lParam.ToInt32() & 0xFFFF);
			// The high-order word specifies the y-coordinate of the cursor.  
			// #define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))  
			window.mousePoint.Y = (int)(short)(lParam.ToInt32() >> 16);

			// Do hit test  
			handled = true;
			if (Math.Abs(window.mousePoint.Y - window.Top) <= window.cornerWidth
				&& Math.Abs(window.mousePoint.X - window.Left) <= window.cornerWidth)
			{ // Top-Left  
				return new IntPtr((int)HitTest.HTTOPLEFT);
			}
			else if (Math.Abs(window.ActualHeight + window.Top - window.mousePoint.Y) <= window.cornerWidth
				&& Math.Abs(window.mousePoint.X - window.Left) <= window.cornerWidth)
			{ // Bottom-Left  
				return new IntPtr((int)HitTest.HTBOTTOMLEFT);
			}
			else if (Math.Abs(window.mousePoint.Y - window.Top) <= window.cornerWidth
				&& Math.Abs(window.ActualWidth + window.Left - window.mousePoint.X) <= window.cornerWidth)
			{ // Top-Right  
				return new IntPtr((int)HitTest.HTTOPRIGHT);
			}
			else if (Math.Abs(window.ActualWidth + window.Left - window.mousePoint.X) <= window.cornerWidth
				&& Math.Abs(window.ActualHeight + window.Top - window.mousePoint.Y) <= window.cornerWidth)
			{ // Bottom-Right  
				return new IntPtr((int)HitTest.HTBOTTOMRIGHT);
			}
			else if (Math.Abs(window.mousePoint.X - window.Left) <= window.customBorderThickness)
			{ // Left  
				return new IntPtr((int)HitTest.HTLEFT);
			}
			else if (Math.Abs(window.ActualWidth + window.Left - window.mousePoint.X) <= window.customBorderThickness)
			{ // Right  
				return new IntPtr((int)HitTest.HTRIGHT);
			}
			else if (Math.Abs(window.mousePoint.Y - window.Top) <= window.customBorderThickness)
			{ // Top  
				return new IntPtr((int)HitTest.HTTOP);
			}
			else if (Math.Abs(window.ActualHeight + window.Top - window.mousePoint.Y) <= window.customBorderThickness)
			{ // Bottom  
				return new IntPtr((int)HitTest.HTBOTTOM);
			}
			else
			{
				handled = false;
				return IntPtr.Zero;
			}
		}
	}
}
