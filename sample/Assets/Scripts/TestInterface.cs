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
        webViewObject.Init(name, (msg)=> {
            Debug.Log("### Initialized!!");
        });
    }

    void OnGUI() {
        if (GUILayout.Button("TAP HERE", GUILayout.MinWidth(200), GUILayout.MinHeight(100))) {
            ActivateWebView();
        }
    }

    private void ActivateWebView() {
        webViewObject.LoadURL(Url);
        webViewObject.SetMargins(12, Screen.height / 2 + 12, 12, 12);
        webViewObject.Show();
    }

    private void DeactivateWebView() {
        webViewObject.LoadURL("about:blank");
        webViewObject.Hide();
    }

    private void DOMContentLoaded() {
        Debug.Log("DOMContentLoaded");
    }

    private void WindowOnLoad() {
        Debug.Log("WindowOnLoad");
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

    private void Note() {

    }

    private void Print() {

    }

    public void CallMessage(WebViewObjectMessage message) {
        switch (message.path) {
        case "/domcontentloaded":
            DOMContentLoaded();
            break;
        case "/load":
            WindowOnLoad();
            break;
        case "/close":
            DeactivateWebView();
            break; 
        case "/spawn":
            Spawn(message.args);
            break;
        };
    }
}
