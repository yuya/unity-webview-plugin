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

package im.yuya.unitywebviewplugin;

import com.unity3d.player.UnityPlayer;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.net.Uri;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.widget.FrameLayout;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.JsResult;

public class WebViewPlugin {
    private WebView webView;
    private String  gameObjectName;
    private String  customScheme;
    private Pattern customSchemeRe;

    private static FrameLayout layout = null;
    
    private class WebViewClientNetworkHook extends WebViewClient {
        @Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
		    if (Uri.parse(url).getScheme().toString().equals(customScheme)) {
		        ShiftQueue();
		    }

			return false;
		}

        public void onPageFinished(WebView view, String url) {
	        UnityPlayer.UnitySendMessage(gameObjectName, "DetectNetworkStatus", "pass");
        }

        public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
            if (!Uri.parse(failingUrl).getScheme().toString().equals(customScheme)) {
                UnityPlayer.UnitySendMessage(gameObjectName, "DetectNetworkStatus", "fail");
            }
        }
    }
    
    private class WebChromeClientAlertHook extends WebChromeClient {
        @Override
        public boolean onJsAlert(WebView view, String url, String message, JsResult result) {
            Matcher matcher = customSchemeRe.matcher(message);

            if (matcher.lookingAt()) {
                UnityPlayer.UnitySendMessage(gameObjectName, "HandleMessage", matcher.replaceFirst(""));
                
                try {
                    return true;
                }
                finally {
                    result.confirm();
                }
            }
            else {
                return false;
            }
        }    
    }
    
    public WebViewPlugin() {}
    
    @SuppressLint("SetJavaScriptEnabled")
    public void Init(final String name) {
        final Activity activity = UnityPlayer.currentActivity;

        gameObjectName = name;
        customScheme   = "webviewbridge";
        customSchemeRe = Pattern.compile("^" + customScheme + ":\\/\\/");
        
        activity.runOnUiThread(new Runnable() {
            @SuppressWarnings("deprecation")
            public void run() {
                webView = new WebView(activity);
                WebSettings webSettings = webView.getSettings();

                webView.setVisibility(View.GONE);
                webView.setFocusable(true);
                webView.setFocusableInTouchMode(true);
                webView.setVerticalScrollbarOverlay(true);
                webView.setWebViewClient(new WebViewClientNetworkHook());
                webView.setWebChromeClient(new WebChromeClientAlertHook());

                webSettings.setSupportZoom(false);
                webSettings.setJavaScriptEnabled(true);
                
                if (layout == null) {
                    layout = new FrameLayout(activity);
                    
                    activity.addContentView(layout, new LayoutParams(
                            LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT));
                    layout.setFocusable(true);
                    layout.setFocusableInTouchMode(true);
                }
                
                layout.addView(webView, new FrameLayout.LayoutParams(
                        LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT,
                        Gravity.NO_GRAVITY))
                ;
            }
        });
    }
    
    public void Destroy() {
        Activity activity = UnityPlayer.currentActivity;
        
        activity.runOnUiThread(new Runnable() {
            public void run() {
                if (webView != null) {
                    layout.removeView(webView);
                    webView.setWebViewClient(null);
                    webView.setWebChromeClient(null);
                    webView.destroy();
                    webView = null;
                }
            }
        });
    }
    
    public void LoadURL(final String url) {
        final Activity activity = UnityPlayer.currentActivity;
        
        activity.runOnUiThread(new Runnable() {
            public void run() {
                webView.loadUrl(url);
            }
        });
    }
    
    public void EvaluteJs(final String str) {
        final Activity activity = UnityPlayer.currentActivity;
        
        activity.runOnUiThread(new Runnable() {
            public void run() {
                webView.loadUrl("javascript:" + str);
            }
        });
    }
    
    public void SetVisibility(final boolean visibility) {
        Activity activity = UnityPlayer.currentActivity;
        
        activity.runOnUiThread(new Runnable() {
            public void run() {
                if (visibility) {
                    webView.setVisibility(View.VISIBLE);
                    layout.requestFocus();
                    webView.requestFocus();
                }
                else {
                    webView.setVisibility(View.GONE);
                }
            }
        });
    }
    
    public void SetMargins(int left, int top, int right, int bottom) {
        @SuppressWarnings("deprecation")
        final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
                LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT, Gravity.NO_GRAVITY);
        Activity activity = UnityPlayer.currentActivity;
        
        params.setMargins(left, top, right, bottom);
        activity.runOnUiThread(new Runnable() {
            public void run() {
                webView.setLayoutParams(params);
            }
        });
    }
    
    private void ShiftQueue() {
        String message = "javascript:alert(WebViewMediator.ShiftQueue())";
        
        if (webView != null) {
            webView.loadUrl(message);
        }
    }
}
