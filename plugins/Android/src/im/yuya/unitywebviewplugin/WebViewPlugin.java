package im.yuya.unitywebviewplugin;

import com.unity3d.player.UnityPlayer;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.os.Bundle;
import android.net.Uri;
import android.util.Log;
import android.view.Gravity;
import android.view.MotionEvent;
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
	
	private class WebViewClientSchemeHook extends WebViewClient {
		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			if (Uri.parse(url).getScheme().toString().equals(customScheme)) {
				callMessage();
			}
			
			return false;
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
	
	public WebViewPlugin() {
	}
	
	@SuppressLint("SetJavaScriptEnabled")
	public void Init(final String name, final String scheme) {
		final Activity activity = UnityPlayer.currentActivity;

		gameObjectName = name;
		customScheme   = scheme;
		customSchemeRe = Pattern.compile("^" + customScheme + ":\\/\\/");
		
		activity.runOnUiThread(new Runnable() {
			@SuppressWarnings("deprecation")
			public void run() {
				webView = new WebView(activity);
				WebSettings webSettings = webView.getSettings();
				
				webView.setVisibility(View.GONE);
				webView.setFocusable(true);
				webView.setFocusableInTouchMode(true);
				
				if (layout == null) {
					layout = new FrameLayout(activity);
					
					activity.addContentView(layout, new LayoutParams(
							LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT));
					layout.setFocusable(true);
					layout.setFocusableInTouchMode(true);
				}
				
				layout.addView(webView, new FrameLayout.LayoutParams(
						LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT,
						Gravity.NO_GRAVITY));
				
				webView.setWebViewClient(new WebViewClientSchemeHook());
				webView.setWebChromeClient(new WebChromeClientAlertHook());

				webSettings.setSupportZoom(false);
				webSettings.setJavaScriptEnabled(true);
//				webSettings.setPluginsEnabled(true);
			}
		});
	}
	
	public void Destroy() {
		Activity activity = UnityPlayer.currentActivity;
		
		activity.runOnUiThread(new Runnable() {
			public void run() {
				if (webView != null) {
					layout.removeView(webView);
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
	
	public void callMessage() {
		String message = "javascript:alert(WebViewMediatorInstance.callMessage())";
		
		webView.loadUrl(message);
	}
}
