//
//  GoogleHelper.m
//  Unity-iPhone
//
//  Created by Alberto Xaubet Matesanz on 14/7/15.
//
//

#import <Foundation/Foundation.h>
#import "GoogleHelper.h"


@implementation GoogleHelper

static GoogleHelper * _sharedHelper;


- (void)signIn:(GIDSignIn *)signIn
didSignInForUser:(GIDGoogleUser *)user
     withError:(NSError *)error {
    
    
    
    NSLog(@"IdToken %@",user.authentication.idToken);
    
    if (user) {
        [self onStatus:@"OPEN_SESSION_SUCCESS" message:user.authentication.idToken];
    }else {
        [self onStatus:@"OPEN_SESSION_CANCEL" message:@"Google Sign Canceled"];
    }
    
}

- (void)signIn:(GIDSignIn *)signIn
didDisconnectWithUser:(GIDGoogleUser *)user
     withError:(NSError *)error {
    
    // Perform any operations when the user disconnects from app here.
    // ...
}


- (void)signIn:(GIDSignIn *)signIn presentViewController:(UIViewController *)viewController {

    NSLog(@"En Sign In Present ViewController");
}

- (void)signIn:(GIDSignIn *)signIn dismissViewController:(UIViewController *)viewController {
    
    NSLog(@"En Sign In Dismiss ViewController");
    
}



- (void)handleOpenUrl:(NSNotification *)notification {
    
    NSDictionary* userInfo = [notification userInfo];
    
    
    NSLog(@"Processing notification");
    
    [[GIDSignIn sharedInstance] handleURL:[userInfo objectForKey:@"url"] sourceApplication:[userInfo objectForKey:@"sourceApplication"] annotation:[userInfo objectForKey:@"annotation"]];
    
}


- (void) onStatus: (NSString*) eventName message:(NSString*) message {
    
    
    NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:eventName,@"eventName", message,@"message", nil];
    
    NSError *error = nil;
    NSData *json;
    
    // Dictionary convertable to JSON ?
    if ([NSJSONSerialization isValidJSONObject:dict])
    {
        // Serialize the dictionary
        json = [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:&error];
        
        // If no errors, let's view the JSON
        if (json != nil && error == nil)
        {
            NSString *jsonString = [[NSString alloc] initWithData:json encoding:NSUTF8StringEncoding];
            UnitySendMessage("Gamedonia", "OnGoogleStatus", [jsonString UTF8String]);
            
        }
    }
    
}


+ (GoogleHelper *) sharedHelper {
    
    if (_sharedHelper != nil) {
        return _sharedHelper;
    }
    _sharedHelper = [[GoogleHelper alloc] init];
    return _sharedHelper;
    
}

@end