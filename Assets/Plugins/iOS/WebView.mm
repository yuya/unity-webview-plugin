#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

//NSString *customScheme     = @"webviewbridge:";
//NSInteger *customSchemeLen = [customScheme length];

char *MakeStringCopy (const char *string) {
    if (string == NULL) {
        return NULL;
    }
    
    char *res = (char *)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

#pragma mark - Objective-C Implementation

@interface WebViewPlugin : NSObject<UIWebViewDelegate>
@property (nonatomic, retain) UIWebView *webView;
@property (nonatomic, copy)   NSString  *gameObjectName;
@end

@implementation WebViewPlugin

- (id)initWithGameObjectName:(const char *)name {
    self = [super init];
    
    if (self) {
        UIView *view      = UnityGetGLViewController().view;
        _webView          = [[UIWebView alloc] initWithFrame:view.frame];
        _webView.delegate = self;
        _webView.hidden   = YES;
        
        [view addSubview:_webView];
        
        _gameObjectName   = [NSString stringWithUTF8String:name];
    }
    
    return self;
}

- (void)dealloc {
    NSLog(@"##### DEALLOC #####");
    _webView.delegate = nil;
    [_webView removeFromSuperview];
    
    _webView        = nil;
    _gameObjectName = nil;
    
    [super dealloc];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType {
    NSString *url = [[request URL] absoluteString];
    
    //    if ([url hasPrefix:customScheme]) {
    //        NSLog(@"NSURL HAS PREFIX WEBVIEWBRIDGE!!");
    //        UnitySendMessage([_gameObjectName UTF8String], "BridgeMessage", [[url substringFromIndex:customSchemeLen] UTF8String]);
    //
    //        return NO;
    //    }
    if ([url hasPrefix:@"webviewbridge:"]) {
        NSLog(@"NSURL HAS PREFIX WEBVIEWBRIDGE!!");
        //        NSLog(_gameObjectName);
        //        NSLog([[url substringFromIndex:14] UTF8String]);
        //        UnitySendMessage("WebViewObject", "BridgeMessage", [self webViewPluginPollMessage:_webView]);
        //        UnitySendMessage("WebViewObject", "BridgeMessage", "MOMONGA IS YUMMY!!");
        UnitySendMessage("WebViewObject", "CallMessage", [self callMessage]);
        //        UnitySendMessage([_gameObjectName UTF8String], "BridgeMessage", [[url substringFromIndex:14] UTF8String]);
        
        return NO;
    }
    else {
        return YES;
    }
}

- (void)loadURL:(const char *)url {
    NSString *urlStr  = [NSString stringWithUTF8String:url];
    NSURL *nsurl      = [NSURL URLWithString:urlStr];
    NSURLRequest *req = [NSURLRequest requestWithURL:nsurl];
    
    [_webView loadRequest:req];
}

//- (void)evaluateJS:(const char *)str {
//    NSString *js = [NSString stringWithUTF8String:str];
//
//    [_webView stringByEvaluatingJavaScriptFromString:js];
//}

- (void)setVisibility:(BOOL)visibility {
    _webView.hidden = visibility ? NO : YES;
}

- (void)setFrame:(NSInteger)x positionY:(NSInteger)y width:(NSInteger)width height:(NSInteger)height {
    UIView* view  = UnityGetGLViewController().view;
    CGRect frame  = _webView.frame;
    CGRect screen = view.bounds;
    
    frame.origin.x    =  x + ((screen.size.width  - width)  / 2);
    frame.origin.y    = -y + ((screen.size.height - height) / 2);
    frame.size.width  = width;
    frame.size.height = height;
    
    _webView.frame = frame;
}

- (NSString *)stringByEvaluatingJS:(NSString *)str {
    return [_webView stringByEvaluatingJavaScriptFromString:str];
}

- (void)setMargins:(int)left top:(int)top right:(int)right bottom:(int)bottom {
    UIView* view  = UnityGetGLViewController().view;
    CGRect frame  = _webView.frame;
    CGRect screen = view.bounds;
    CGFloat scale = 1.0f / view.contentScaleFactor;
    
    frame.size.width  = screen.size.width  - scale * (left + right);
    frame.size.height = screen.size.height - scale * (top + bottom);
    frame.origin.x    = scale * left;
    frame.origin.y    = scale * top;
    
    _webView.frame = frame;
}

- (char *)callMessage {
    const char *message = [_webView stringByEvaluatingJavaScriptFromString:@"WebViewMediatorInstance.callMessage()"].UTF8String;
    
    if (message) {
        return MakeStringCopy(message);
    }
    else {
        return NULL;
    }
}

@end

#pragma mark - Unity Plugin

extern "C" {
    void *webViewPluginInit(const char *gameObjectName);
    void webViewPluginDestroy(void *instance);
    void webViewPluginLoadURL(void *instance, const char *url);
    //    void webViewEvaluteJS(void *instance, const char *str);
    void webViewPluginSetVisibility(void *instance, BOOL visibility);
    void webViewPluginSetFrame(void *instance, NSInteger x, NSInteger y, NSInteger width, NSInteger height);
    void webViewPluginSetMargins(void *instance, int left, int top, int right, int bottom);
    const char *webViewPluginCallMessage(void *instance);
    
    void hoge_();
    void webViewPluginAlert();
}

static WebViewPlugin *webViewInstance;

void *webViewPluginInit(const char *gameObjectName) {
    id instance     = [[WebViewPlugin alloc] initWithGameObjectName:gameObjectName];
    webViewInstance = instance;
    
    return (void *)instance;
}

void webViewPluginDestroy(void *instance) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    
    [webViewPlugin release];
}

void webViewPluginLoadURL(void *instance, const char *url) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    
    [webViewPlugin loadURL:url];
}

//void webViewEvaluteJS(void *instance, const char *str) {
//    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
//
//    [webViewPlugin evaluateJS:str];
//}

void webViewPluginSetVisibility(void *instance, BOOL visibility) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    
    [webViewPlugin setVisibility:visibility];
}

void webViewPluginSetFrame(void *instance, NSInteger x, NSInteger y, NSInteger width, NSInteger height) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    float screenScale            = [UIScreen instancesRespondToSelector:@selector(scale)] ? [UIScreen mainScreen].scale : 1.0f;
    
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
        if (screenScale == 2.0) {
            screenScale = 1.0f;
        }
    }
    
    [webViewPlugin setFrame:x/screenScale positionY:y/screenScale width:width/screenScale height:height/screenScale];
}

void webViewPluginSetMargins(void *instance, int left, int top, int right, int bottom) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    
    [webViewPlugin setMargins:left top:top right:right bottom:bottom];
}

const char *webViewPluginCallMessage(void *instance) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    
    return [webViewPlugin callMessage];
}

void webViewPluginAlert() {
    UIAlertView *alert = [[UIAlertView alloc] init];
    
    alert.title = @"TITLE_TITLE";
    alert.message = @"MESSAGE_MESSAGE";
    [alert addButtonWithTitle:@"OK"];
    
    [alert show];
}

void hoge_() {
    UIAlertView *alert = [[UIAlertView alloc] init];
    alert.title        = @"お知らせ";
    alert.message      = @"完了しました";
    
    [alert addButtonWithTitle:@"確認"];
    [alert show];
}
