using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WebViewTest : MonoBehaviour {
    WebViewObject webViewObject;
//    private bool _isStarted  = false;

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
            Debug.Log("OPEN URL");

//            webViewObject.Init("WebViewObject", "webviewbridge:", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            webViewObject.Init("WebViewObject", "webviewbridge:", "WebViewTest");
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
        Debug.Log("DOMContentLoaded");
    }

    public void WindowOnLoad() {
        Debug.Log("Window OnLoad");
    }

    public void CloseWebView() {
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
