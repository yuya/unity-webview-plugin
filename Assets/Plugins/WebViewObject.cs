using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Callback = System.Action<string>;

public class WebViewObjectMessage {
    public string path;
    public Hashtable args;

    public WebViewObjectMessage(string message) {
        string[] split = message.Split("?"[0]);

        path = split[0];
        args = new Hashtable();

        if (split.Length > 1) {
            foreach (string pair in split[1].Split("&"[0])) {
                string[] keys = pair.Split("="[0]);
                args[keys[0]] = WWW.UnEscapeURL(keys[1]);
            }
        }
    }
}

public class WebViewObject : MonoBehaviour {
    Callback callback;
    IntPtr webView;

    [DllImport("__Internal")]
    private static extern IntPtr webViewPluginInit(string gameObject);
    [DllImport("__Internal")]
    private static extern int webViewPluginDestroy(IntPtr instance);
    [DllImport("__Internal")]
    private static extern void webViewPluginLoadURL(IntPtr instance, string url);
//    [DllImport("__Internal")]
//    private static extern void webViewEvaluateJS(IntPtr instance, string str);
    [DllImport("__Internal")]
    private static extern void webViewPluginAlert();
    [DllImport("__Internal")]
    private static extern void webViewPluginSetVisibility(IntPtr instance, bool visibility);
    [DllImport("__Internal")]
    private static extern void webViewPluginSetFrame(IntPtr instance, int x, int y, int width, int height);
    [DllImport("__Internal")]
    private static extern void webViewPluginSetMargins(IntPtr instance, int left, int top, int right, int bottom);
    [DllImport("__Internal")]
    private static extern void hoge_();

//    private static WebViewObject _instance = null;
//    public static WebViewObject Instance {
//        get {
//            if (_instance == null) {
//                GameObject gameObject = new GameObject("WebViewObject");
//                DontDestroyOnLoad(gameObject);
//
//                _instance = gameObject.AddComponent<WebViewObject>();
//            }
//
//            return _instance;
//        }
//    }

    public void Init(Callback cb = null) {
        callback = cb;
        webView  = webViewPluginInit(name);
    }

    void OnDestroy() {
        if (webView == IntPtr.Zero) {
            return;
        }

        webViewPluginDestroy(webView);
    }

    public void SetCenterPositionWithScale(Vector2 center, Vector2 scale) {
        if (webView == IntPtr.Zero) {
            return;
        }

        webViewPluginSetFrame(webView, (int)center.x, (int)center.y, (int)scale.x, (int)scale.y);
    }

    public void SetMargins(int left, int top, int right, int bottom) {
        if (webView == IntPtr.Zero) {
            return;
        }

        webViewPluginSetMargins(webView, left, top, right, bottom);
    }

    public void SetVisibility(bool value) {
        if (webView == IntPtr.Zero) {
            return;
        }

        webViewPluginSetVisibility(webView, value);
    }

    public void LoadURL(string url) {
        if (webView == IntPtr.Zero) {
            return;
        }

        Debug.Log(url);
        webViewPluginLoadURL(webView, url);
    }

//    public WebViewObjectMessage CallMessage(string message) {
//        Debug.Log(message);
//
//        return (message != null) ? new WebViewObjectMessage(message) : null;
//    }

    public void CallMessage(string message) {
        Debug.Log(message);

//        return (message != null) ? new WebViewObjectMessage(message) : null;
    }

    public void Alert() {
        webViewPluginAlert();
    }

    public void hoge() {
        hoge_();
    }

//    public void EvaluteJS(string str) {
//        if (webView == IntPtr.Zero) {
//            return;
//        }
//
//        webViewEvaluateJS(webView, str);
//    }
}
