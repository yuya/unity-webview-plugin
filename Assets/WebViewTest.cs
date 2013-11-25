using UnityEngine;
using System.Collections;

public class WebViewTest : MonoBehaviour {
    public string Url;
    WebViewObject webViewObject;

	// Use this for initialization
	void Start () {
//        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();	
//        Debug.Log (webViewObject);
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init ();
//
//        webViewObject.LoadURL (Url);
//        webViewObject.SetVisibility (true);
	}

//    void OnGUI () {
//        if (GUI.Button (new Rect(10, 10, 150, 100), "I am a button")) {
//            Debug.Log ("HOGE");
//        }
//    }
//
//	// Update is called once per frame
//	void Update () {
//	}
}
