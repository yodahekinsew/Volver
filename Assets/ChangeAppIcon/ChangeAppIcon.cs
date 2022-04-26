////////////////////////////////////////////////////////////////////////////////
//
// @author Yodahe Alemu @yodahekinsew
// https://github.com/yodahekinsew/ChangeAppIcon
// https://yodahe.com
//
////////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

#if UNITY_IOS
using System.Collections;
using System.Runtime.InteropServices;
#endif

public static class ChangeAppIcon
{

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _ChangeIcon(string icon);
#endif

    private static bool initialized = false;

    public static void Init()
    {
        if (initialized) return;
        initialized = true;
    }

    ///<summary>
    /// Change the app icon
    ///</summary>
    public static void ChangeIcon(string icon)
    {
#if UNITY_IOS
        _ChangeIcon(icon);
#endif
    }

}
