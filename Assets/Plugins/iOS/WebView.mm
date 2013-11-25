#import <UIKit/UIKit.h>

extern UIViewControler *UnityGetGLViewController();
extern "C" void UnityGetGLViewController(const char *, const char *, const char*);

#pragma mark - UIWebView Hacks for IOS

#pragma mark - LodingIndicatorView

#pragma mark - Objective-C Implementation

@interface WEbViewPlugin : NSObject<UIWebViewDelegate>
@property (nonatomic, retain) UIWebView *webView;
@property (nonatomic, copy) NSString *gameObjectName;
@property (nonatomic, copy) NSString *customScheme;
@property (nonatomic, retain) UILabel *label;
@property (nonatomic, retain) LoadingIndicatorView *loadingIndicatorView;
@end

@implementation WebViewPlugin

- (id)initWithGameObjectName:(const char *)gameObjectName_ customScheme(const char*)scheme {
    self = [super init];
    
    if (self) {
        UIView *view = UnityGetGLViewController().view;
        
        self.webView = [[[UIWebView alloc] initWithFrame.view.frame] autorelease];
        self.webView.delegate = self;
        self.webView.hidden = YES;
    }
}

@end

#pragma mark - Unity Plugin

extern "C" {
    void *WebViewPluginInit(const char *gameObjectName, const char *scheme);
    void WebViewPluginLoadURL(void *instance, const char *url);
}

void *WebViewPluginInit(const char *gameObjectName, const char *scheme) {
    id instance = [[WebViewPlugin alloc]];
}
