using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WebViewTest : MonoBehaviour {
    WebViewObject webViewObject;
//    WebViewObjectMessage message;
//    private WebViewObject webViewObject;
    private WebViewObjectMessage message;
    private bool _isStarted  = false;

    void Awake() {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
//        webViewObject = webViewObject.Instance.Init();

//        WebViewObject.Instance.Init();
    }

    void Start() {
//        Init();
        _isStarted = true;
    }

    void OnGUI() {
        Init();
    }

    public void Init() {
        if (GUILayout.Button("OPEN URL", GUILayout.MinWidth(200), GUILayout.MinHeight(100))) {
            Debug.Log("OPEN URL");

//            webViewObject = webViewObject.Instance.Init();
//            webViewObject.Init();
//            webViewObject.LoadURL("http://yahoo.co.jp/");

//            WebViewObject.Instance.LoadURL("http://172.21.26.40:8000/");
//            WebViewObject.Instance.SetVisibility(true);

            webViewObject.Init();
            webViewObject.LoadURL("http://172.21.26.40:8000/");
            webViewObject.SetVisibility(true);
        }
    }
}
