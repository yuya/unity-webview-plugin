using UnityEngine;
using System.Collections;

public class TestInterface : MonoBehaviour {
    public string Url;
    private string note;

    public GUISkin    guiSkin;
    public GameObject redBoxPrefab;
    public GameObject blueBoxPrefab;
    WebViewObject webViewObject;

    void Start() {        
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(name, "CustomUserAgent/1.0.0");
    }

    void OnGUI() {
        if (GUILayout.Button("TAP HERE", GUILayout.MinWidth(200), GUILayout.MinHeight(100))) {
            ShowWebView();
        }
    }

    private void ShowWebView() {
        webViewObject.LoadURL(Url);
        webViewObject.SetMargins(12, Screen.height / 2 + 12, 12, 12);
        webViewObject.Show();
    }

    private void HideWebView() {
        webViewObject.LoadURL("about:blank");
        webViewObject.Hide();
    }

    private IEnumerator StartActivityIndicator() {
#if UNITY_IPHONE
        Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.White);
#elif UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif
        Handheld.StartActivityIndicator();
        
        yield return new WaitForSeconds(0);
    }

    private void ShowIndicator() {
        StartCoroutine(StartActivityIndicator());
    }

    private void HideIndicator() {
        Handheld.StopActivityIndicator();
    }

    public void LoadURLWithIndicator(string url) {
        if (string.IsNullOrEmpty(url)) {
            return;
        }

        StartCoroutine(StartActivityIndicator());
        webViewObject.LoadURL(url);
    }

    private void Spawn(Hashtable args) {
        GameObject prefab;
        GameObject box;

        if (args.ContainsKey("color")) {
            prefab = (args["color"] as string == "red") ? redBoxPrefab : blueBoxPrefab;
        }
        else {
            prefab = Random.value < 0.5 ? redBoxPrefab : blueBoxPrefab;
        }

        box = Instantiate(prefab, redBoxPrefab.transform.position, Random.rotation) as GameObject;

        if (args.ContainsKey("scale")) {
            box.transform.localScale = Vector3.one * float.Parse(args["scale"] as string);
        }
    }

    private void WindowOnLoad() {
        Debug.Log("WindowOnLoad");
    }

    private void onError() {
        Debug.Log("OnErorr");
    }

    public void CallMessage(WebViewObjectMessage message) {
        switch (message.path) {
        case "/spawn":
            Spawn(message.args);
            break;
        case "/close":
            HideWebView();
            break; 
        case "/show_indicator":
            ShowIndicator();
            break;
        case "/hide_indicator":
            HideIndicator();
            break;
        case "/load":
            WindowOnLoad();
            break;
        case "/on_error":
            onError();
            break;
        };
    }
}
