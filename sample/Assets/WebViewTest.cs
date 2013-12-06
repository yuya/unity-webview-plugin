﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WebViewTest : MonoBehaviour {
    WebViewObject webViewObject;
//    private bool _isStarted  = false;

    private string customScheme = "webviewbridge";
    private string currentClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();

    void Awake() {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
    }

    void Start() {
//        Init();
//        _isStarted = true;
    }

//    void Update() {
//        Init();
//    }

    void OnGUI() {
        Init();
    }

    public void Init() {
        if (GUILayout.Button("OPEN URL", GUILayout.MinWidth(200), GUILayout.MinHeight(100))) {
            Debug.Log("### OPEN URL");

//            webViewObject.Init("WebViewObject", "webviewbridge:", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//            webViewObject.Init("WebViewObject", "webviewbridge", "WebViewTest");
//            System.Reflection.MethodBase.GetCurrentMethod()
            webViewObject.Init("WebViewObject", customScheme, currentClass);
//            webViewObject.LoadURL("http://yahoo.co.jp/");
            webViewObject.LoadURL("http://172.21.26.40:8000/");
            webViewObject.SetVisibility(true);
        }
    }

//    public void LogLogCombo(string message) {
//        Debug.Log("===== LogLogCombo =====");
//        Debug.Log(message);
//    }

    public void DOMContentLoaded() {
        Debug.Log("### __DOMContentLoaded__");
    }

    public void WindowOnLoad() {
        Debug.Log("### __Window_OnLoad__");
    }

    public void CloseWebView() {
        Debug.Log("### __Close_Window__");

        webViewObject.SetVisibility(false);
        webViewObject.Destroy();
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