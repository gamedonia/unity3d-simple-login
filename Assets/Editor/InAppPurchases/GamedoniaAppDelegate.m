//
//  GamedoniaAppDelegate.m
//  Unity-iPhone
//
//  Created by Alberto Xaubet Matesanz on 23/1/15.
//
//

#import "GamedoniaAppDelegate.h"
#import <objc/runtime.h>


void GamedoniaDidRegisterForRemoteNotificationsWithDeviceToken(id self, SEL _cmd, id application, id devToken);
void GamedoniaDidFailToRegisterForRemoteNotificationsWithError(id self, SEL _cmd, id application, id error);

static void exchangeMethodImplementations(Class class, SEL oldMethod, SEL newMethod, IMP impl, const char *signature) {
    // Check whether method exists in the class
    Method method = class_getInstanceMethod(class, oldMethod);
    if (method) {
        // if method exists add a new method and exchange it with current
        class_addMethod(class, newMethod, impl, signature);
        method_exchangeImplementations(class_getInstanceMethod(class, oldMethod), class_getInstanceMethod(class, newMethod));
    }
    else {
        // if method does not exist, simply add as original method
        class_addMethod(class, oldMethod, impl, signature);
    }
}

@implementation UIApplication (GamedoniaAppDelegate)


+ (void)load {
    //NSLog(@"%s", __FUNCTION__);
    method_exchangeImplementations(class_getInstanceMethod(self, @selector(setDelegate:)), class_getInstanceMethod(self, @selector(setGamedoniaDelegate:)));
    
    //UIApplication *app = [UIApplication sharedApplication];
    NSLog(@"Initializing application...");
}


- (void)setGamedoniaDelegate:(id <UIApplicationDelegate> )delegate {
    
    static Class delegateClass = nil;
    
    
    if (delegateClass == [delegate class]) {
        [self setGamedoniaDelegate:delegate];
        return;
    }
    
    delegateClass = [delegate class];
    
    
    exchangeMethodImplementations(delegateClass, @selector(application:didRegisterForRemoteNotificationsWithDeviceToken:),
                                  @selector(application:GamedoniaDidRegisterForRemoteNotificationsWithDeviceToken:),
                                  (IMP)GamedoniaDidRegisterForRemoteNotificationsWithDeviceToken,
                                  "v@:::");
    
    exchangeMethodImplementations(delegateClass, @selector(application:didFailToRegisterForRemoteNotificationsWithError:),
                                  @selector(application:GamedoniaDidFailToRegisterForRemoteNotificationsWithError:),
                                  (IMP)GamedoniaDidFailToRegisterForRemoteNotificationsWithError,
                                  "v@:::");
    
    /*
     exchangeMethodImplementations(delegateClass, @selector(application:didFinishLaunchingWithOptions:),
     @selector(application:IBPushDidFinishLaunchingWithOptions:),
     (IMP)IBPushDidFinishLaunchingWithOptions,
     "B@:::");
     */
    
    /*
    exchangeMethodImplementations(delegateClass, @selector(application:didReceiveRemoteNotification:),
                                  @selector(application:GamedoniaDidReceiveRemoteNotification:),
                                  (IMP)GamedoniaDidReceiveRemoteNotification,
                                  "v@:::");
    
    
    exchangeMethodImplementations(delegateClass, @selector(application:didReceiveRemoteNotification:fetchCompletionHandler:),
                                  @selector(application:IBPushDidReceiveRemoteNotificationFetchCompletionHandler:),
                                  (IMP)IBPushDidReceiveRemoteNotificationFetchCompletionHandler,
                                  "v@::::");
    
    exchangeMethodImplementations(delegateClass, @selector(application:didReceiveLocalNotification:),
                                  @selector(application:IBPushDidReceiveLocalNotification:),
                                  (IMP)IBPushDidReceiveLocalNotification,
                                  "v@:::");
    */
    
    [self setGamedoniaDelegate:delegate];
}

void GamedoniaDidRegisterForRemoteNotificationsWithDeviceToken(id self, SEL _cmd, id application, id deviceToken) {
    //NSLog(@"%s", __FUNCTION__);
    
    /*
    if ([self respondsToSelector:@selector(application:GamedoniaDidRegisterForRemoteNotificationsWithDeviceToken:)]) {
        [self application:application GamedoniaDidRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
    }*/
    
    const unsigned *tokenBytes = [deviceToken bytes];
    NSString *hexToken = [NSString stringWithFormat:@"%08x%08x%08x%08x%08x%08x%08x%08x",
                          ntohl(tokenBytes[0]), ntohl(tokenBytes[1]), ntohl(tokenBytes[2]),
                          ntohl(tokenBytes[3]), ntohl(tokenBytes[4]), ntohl(tokenBytes[5]),
                          ntohl(tokenBytes[6]), ntohl(tokenBytes[7])];
    
    
    //Unity Send Message con la informacion
    UnitySendMessage("Gamedonia","DidRegisterForRemoteNotifications", [hexToken UTF8String]);
    
}

void GamedoniaDidFailToRegisterForRemoteNotificationsWithError(id self, SEL _cmd, id application, id error) {
    
    NSString *errorCode = [NSString stringWithFormat:@"%d", [error code]];
    UnitySendMessage("Gamedonia","DidFailToRegisterForRemoteNotificationsWithError", [errorCode UTF8String]);
}

@end
