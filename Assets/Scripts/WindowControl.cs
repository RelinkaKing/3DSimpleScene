using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowControl : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(Screen.width,Screen.height,false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    [DllImport("user32.dll")]
    static extern int GetDpiForWindow(IntPtr hWnd);

    public float GetDisplayScaleFactor(IntPtr windowHandle)
    {
        try
        {
            return GetDpiForWindow(windowHandle) / 96f;
        }
        catch
        {
            // Or fallback to GDI solutions above
            return 1;
        }
    }

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
    public static extern System.IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


    [DllImport("user32.dll")]
    private static extern bool EnableWindow(IntPtr hwnd, bool enable);
    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr handle, int x, int y, int width,
    int height, bool redraw);

    public void WindowMin()
    {
        ShowWindow(FindWindow(null, "PPTPlugin"), 7);
    }
}
