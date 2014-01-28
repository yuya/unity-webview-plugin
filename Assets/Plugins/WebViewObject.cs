/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
#region iOS
#if UNITY_IPHONE
    IntPtr webView;

    [DllImport("__Internal")]
    private static extern IntPtr webViewPluginInit(string name, string scheme);
    [DllImport("__Internal")]
    private static extern int webViewPluginDestroy(IntPtr instance);
    [DllImport("__Internal")]
    private static extern void webViewPluginLoadURL(IntPtr instance, string url);
    //    [DllImport("__Internal")]
    //    private static extern void webViewEvaluateJS(IntPtr instance, string str);
    [DllImport("__Internal")]
    private static extern void webViewPluginSetVisibility(IntPtr instance, bool visibility);
    [DllImport("__Internal")]
    private static extern void webViewPluginSetFrame(IntPtr instance, int x, int y, int width, int height);
    [DllImport("__Internal")]
    private static extern void webViewPluginSetMargins(IntPtr instance, int left, int top, int right, int bottom); 

    private GameObject callerObject;
    public void Init(string name, string scheme, string caller) {
        webView      = webViewPluginInit(name, scheme, caller);
        callerObject = GameObject.Find(caller);
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

        webViewPluginLoadURL(webView, url);
    }

    public void HandleMessage(string message) {
        callerObject.SendMessage("CallMessage", (message != null) ? new WebViewObjectMessage(message) : null);
    }

    public void Destroy() {
        webViewPluginDestroy(webView);
    }
#endif
#endregion

#region Android
#if UNITY_ANDROID
    AndroidJavaObject webView;

    private GameObject callerObject;
    public void Init(string name, string scheme, string caller) {
        webView      = new AndroidJavaObject("im.yuya.unitywebviewplugin.WebViewPlugin");
        callerObject = GameObject.Find(caller);

        webView.Call("Init", name, scheme);
    }

    void OnDestroy() {
        if (webView == null) {
            return;
        }

        webView.Call("Destroy");
    }

    public void SetMargins(int left, int top, int right, int bottom) {
        if (webView == null) {
            return;
        }

        webView.Call("SetMargins", left, top, right, bottom);
    }

    public void SetVisibility(bool value) {
        if (webView == null) {
            return;
        }

        webView.Call("SetVisibility", value);
    }

    public void LoadURL(string url) {
        if (webView == null) {
            return;
        }

        webView.Call("LoadURL", url);
    }

    public void EvaluteJS(string str) {
        if (webView == null) {
            return;
        }

        webView.Call("LoadURL", "javascript:" + str);
    }
    public void HandleMessage(string message) {
        callerObject.SendMessage("CallMessage", (message != null) ? new WebViewObjectMessage(message) : null);
    }

    public void Destroy() {
        webView.Call("Destroy");
    }
#endif
#endregion
}
