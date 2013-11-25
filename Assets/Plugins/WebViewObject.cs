using UnityEngine;
//using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Callback = System.Action<string>;

public class WebViewObject : MonoBehaviour {
    Callback callback;
    
#if UNITY_IPHONE
    IntPtr webView;
    public bool visibility;

    [DllImport("__Internal")]
    private static extern IntPtr _WebViewPlugin_Init();
    [DllImport("__Internal")]
    private static extern void _WebViewPluginSetVisibility(IntPtr instance, bool visibility);
    [DllImport("__Internal")]
    private static extern void _WebViewPluginLoadURL(IntPtr instance, string url);

    public void Init() {
        webView = _WebViewPluginInit();
    }

    public void SetVisibility(bool value) {
        if (webView == IntPtr.Zero) {
            return;
        }

        visibility = value;
        _WebViewPluginSetVisibility(webView, value);
    }

    public void LoadURL(string url) {
        if (webView == null) {
            return;
        }

        _WebViewPluginLoadURL(webView, url);
    }
#endif
}
