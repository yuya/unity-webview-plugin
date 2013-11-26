using UnityEngine;

public class WebViewTest : MonoBehaviour {
    public string Url;
    WebViewObject webViewObject;

    void start() {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init();
    }

    void OnGUI() {
        if (GUILayout.Button("OPEN URL", GUILayout.MinWidth(200), GUILayout.MinHeight(100))) {
            Debug.Log("OPEN URL");
            webViewObject.LoadURL("http://yahoo.co.jp/");
        }
        if (GUILayout.Button("ALERT", GUILayout.MinWidth(200), GUILayout.MinHeight(100))) {
            Debug.Log("ALERT");
            webViewObject.Alert();
        }
    }
}
