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

#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

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
@property (nonatomic, copy)   NSString  *customScheme;
@end

@implementation WebViewPlugin

- (id)initWithGameObjectName:(const char *)name customScheme:(const char *)scheme {
    self = [super init];
    
    if (self) {
        UIView *view = UnityGetGLViewController().view;
        _webView     = [[[UIWebView alloc] initWithFrame:view.frame] autorelease];

        _webView.delegate = self;
        _webView.hidden   = YES;
        _webView.opaque   = NO;
        _webView.scrollView.decelerationRate = UIScrollViewDecelerationRateNormal;
        
        [view addSubview:_webView];
        
        self.gameObjectName = [NSString stringWithUTF8String:name];
        self.customScheme   = (scheme != NULL) ? [NSString stringWithUTF8String:scheme] : nil;
    }
    
    return self;
}

- (void)dealloc {
    _webView.delegate = nil;
    [_webView stopLoading];
    [_webView removeFromSuperview];
    
    _webView            = nil;
    self.gameObjectName = nil;
    
    [super dealloc];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType {
    NSString *url = [[request URL] absoluteString];
    
    if ([url hasPrefix:self.customScheme]) {
        UnitySendMessage([self.gameObjectName UTF8String], "HandleMessage", [self shiftQueue]);
        
        return NO;
    }
    else {
        return YES;
    }
}

- (void)loadURL:(const char *)url {
    NSString     *urlStr = [NSString stringWithUTF8String:url];
    NSURL        *nsurl  = [NSURL URLWithString:urlStr];
    NSURLRequest *req    = [NSURLRequest requestWithURL:nsurl];
    
    [_webView loadRequest:req];
}

- (void)evaluateJS:(const char *)str {
    NSString *js = [NSString stringWithUTF8String:str];
    
    [_webView stringByEvaluatingJavaScriptFromString:js];
}

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

- (char *)shiftQueue {
    const char *message = [_webView stringByEvaluatingJavaScriptFromString:@"WebViewMediator.shiftQueue()"].UTF8String;
    
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
    void *webViewPluginInit(const char *gameObjectName, const char *scheme);
    void webViewPluginDestroy(void *instance);
    void webViewPluginLoadURL(void *instance, const char *url);
    void webViewPluginEvaluteJS(void *instance, const char *str);
    void webViewPluginSetVisibility(void *instance, BOOL visibility);
    void webViewPluginSetFrame(void *instance, NSInteger x, NSInteger y, NSInteger width, NSInteger height);
    void webViewPluginSetMargins(void *instance, int left, int top, int right, int bottom);
}

void *webViewPluginInit(const char *gameObjectName, const char *scheme) {
    id instance = [[WebViewPlugin alloc] initWithGameObjectName:gameObjectName customScheme:scheme];
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

void webViewPluginEvaluteJS(void *instance, const char *str) {
    WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
    [webViewPlugin evaluateJS:str];
}

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
