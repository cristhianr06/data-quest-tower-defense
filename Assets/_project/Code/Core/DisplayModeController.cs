using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayModeController : MonoBehaviour
{
    public static DisplayModeController Instance;

    private const int WIDTH = 1920;
    private const int HEIGHT = 1080;

    private int currentDisplayIndex = 0;

    private List<DisplayInfo> displays = new();

#if UNITY_STANDALONE_WIN

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool GetWindowRect(System.IntPtr hWnd, out RECT rect);

    [System.Runtime.InteropServices.StructLayout(
        System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        RefreshDisplays();

        StartFullscreenPrimaryMonitor();
    }

    private void RefreshDisplays()
    {
        displays.Clear();

        Screen.GetDisplayLayout(displays);
    }

    /// <summary>
    /// Fullscreen monitor principal al iniciar
    /// </summary>
    private void StartFullscreenPrimaryMonitor()
    {
        currentDisplayIndex = 0;

        Screen.MoveMainWindowTo(
            displays[currentDisplayIndex],
            Vector2Int.zero);

        Screen.SetResolution(
            WIDTH,
            HEIGHT,
            FullScreenMode.FullScreenWindow);
    }

    /// <summary>
    /// Cambia a modo ventana
    /// </summary>
    public void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;

        Screen.SetResolution(
            WIDTH,
            HEIGHT,
            false);
    }

    /// <summary>
    /// Fullscreen en monitor actual
    /// </summary>
    public void SetFullscreen()
    {
        RefreshDisplays();

        currentDisplayIndex = GetCurrentMonitor();

        Screen.MoveMainWindowTo(
            displays[currentDisplayIndex],
            Vector2Int.zero);

        Screen.SetResolution(
            WIDTH,
            HEIGHT,
            FullScreenMode.FullScreenWindow);
    }

    /// <summary>
    /// Detecta monitor donde está la ventana
    /// </summary>
    private int GetCurrentMonitor()
    {
#if UNITY_STANDALONE_WIN

        System.IntPtr window = GetActiveWindow();

        if (GetWindowRect(window, out RECT rect))
        {
            int centerX = (rect.Left + rect.Right) / 2;

            for (int i = 0; i < displays.Count; i++)
            {
                DisplayInfo display = displays[i];

                int minX = display.workArea.x;
                int maxX = display.workArea.x + display.workArea.width;

                if (centerX >= minX && centerX < maxX)
                {
                    return i;
                }
            }
        }

#endif

        return 0;
    }
}