using Kudoshi.Utilities;
using UnityEngine;

/// <summary>
/// This used to be a mouse manager but it doesn't really hold any state. So currently just going to put it as a Util function first
/// </summary>
public static class MouseUtil
{
    // Cache previous cursor state for use
    public static bool CursorWasVisible = true;
    public static CursorLockMode CursorPrevLockState = CursorLockMode.None;
    // Call this method to hide and lock the cursor
    public static void HideCursor(bool savePrevConfig = true)
    {
        if (savePrevConfig)
        {
            CursorWasVisible = Cursor.visible;
            CursorPrevLockState = Cursor.lockState;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void ResetCursor()
    {
        Cursor.visible = CursorWasVisible;
        Cursor.lockState = CursorPrevLockState;
    }

    // Call this method to show and unlock the cursor
    public static void ShowCursor(bool savePrevConfig = true)
    {
        if (savePrevConfig)
        {
            CursorWasVisible = Cursor.visible;
            CursorPrevLockState = Cursor.lockState;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
