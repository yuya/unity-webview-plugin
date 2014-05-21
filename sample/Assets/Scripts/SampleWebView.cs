/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 * Copyright (C) 2014 Yuya Hashimoto
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

public class SampleWebView : MonoBehaviour {
    public string Url;
    WebViewObject webViewObject;

    void Awake() {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
    }

    void Start() {
        webViewObject.Init(name, (msg)=> {
            Debug.Log("### Initialized!!");
        });

        webViewObject.SetVisibility(true);
        webViewObject.LoadURL(Url);
    }


    private void DOMContentLoaded() {
        Debug.Log("DOMContentLoaded");
    }

    private void WindowOnLoad() {
        Debug.Log("WindowOnLoad");
    }

    private void CloseWebView() {
        Debug.Log("CloseWebView");

        webViewObject.SetVisibility(false);
        Destroy(webViewObject);
    }

    public void CallMessage(WebViewObjectMessage message) {
        switch (message.path) {
        case "domcontentloaded":
            DOMContentLoaded();
            break;
        case "load":
            WindowOnLoad();
            break;
        case "close":
            CloseWebView();
            break; 
        };
    }
}