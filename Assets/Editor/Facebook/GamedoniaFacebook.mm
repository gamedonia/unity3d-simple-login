//
//  GamedoniaSDKFacebook.mm
//  GamedoniaSDK
//
//  Created by Alberto Xaubet Matesanz on 17/12/13.
//
//
#import "FacebookSDK/FacebookSDK.h"
#include <stdio.h>
#include "GamedoniaFacebook.h"
#include "GamedoniaSDKFacebookHelper.h"
#include "GamedoniaFacebookUtils.h"


void openSessionWithPermissionsOfType(const char** permissions, int size, const char *type, bool allowUI);
void reauthorizeSessionWithPermissionsOfType(const char** permissions, int size, const char *type);

void _init(const char* appID,const char* urlSchemeSuffix, BOOL frictionless) {
    
    // Retrieve application ID
    NSString *_appID = [NSString stringWithUTF8String:appID];

    NSString *_urlSchemeSuffix = nil;
    
    if (strlen(urlSchemeSuffix) > 0) {
        _urlSchemeSuffix = [NSString stringWithUTF8String:urlSchemeSuffix];
    }
    
    [[GamedoniaSDKFacebookHelper sharedInstance] setupWithAppID:_appID urlSchemeSuffix:_urlSchemeSuffix];
}

BOOL _isSessionOpen() {
    
    FBSession *session = [FBSession activeSession];
    BOOL isSessionOpen = [session isOpen];
    
    return isSessionOpen;
}


void _openSessionWithReadPermissions(const char** permissions, int size, BOOL allowUI) {
    openSessionWithPermissionsOfType(permissions, size, "read", allowUI);
}

void _openSessionWithReadPermissions(const char** permissions, int size) {
    openSessionWithPermissionsOfType(permissions, size, "read", true);
}


void _openSessionWithPublishPermissions(const char** permissions, int size) {
    openSessionWithPermissionsOfType(permissions, size, "publish", true);
}

void _openSessionWithPermissions(const char** permissions, int size) {
    openSessionWithPermissionsOfType(permissions, size, "readAndPublish", true);
}

void openSessionWithPermissionsOfType(const char** permissions, int size, const char *type, bool allowUI) {

    
    NSMutableArray *nspermissions = [NSMutableArray array];
    
    for (int i=0;i<size;i++) {
        [nspermissions addObject:[NSString stringWithUTF8String:permissions[i]]];
    }

    // select the right authentication flow
    FBSessionLoginBehavior loginBehavior;
    if (strcmp(type, "read") == 0)
    {
        // system account if no publish permissions
        loginBehavior = FBSessionLoginBehaviorUseSystemAccountIfPresent;
    } else
    {
        // web if publish permissions
        loginBehavior = FBSessionLoginBehaviorWithFallbackToWebView;
    }

    // Start authentication flow
    FBOpenSessionCompletionHandler completionHandler = [GamedoniaSDKFacebookHelper openSessionCompletionHandler];
    NSString *appID = [[GamedoniaSDKFacebookHelper sharedInstance] appID];
    NSString *urlSchemeSuffix = [[GamedoniaSDKFacebookHelper sharedInstance] urlSchemeSuffix];
    FBSession *session = nil;
    @try
    {
        session = [[FBSession alloc] initWithAppID:appID permissions:nspermissions defaultAudience:FBSessionDefaultAudienceFriends urlSchemeSuffix:urlSchemeSuffix tokenCacheStrategy:[FBSessionTokenCachingStrategy nullCacheInstance]];
        [FBSession setActiveSession:session];
        
        if (allowUI || session.state == FBSessionStateCreatedTokenLoaded) {
            [session openWithBehavior:loginBehavior completionHandler:completionHandler];
        }else {
            NSLog(@"FB Session state: %d",session.state);
        }
        
    }
    @catch (NSException *exception)
    {        
        onStatus("OPEN_SESSION_ERROR", [[exception reason] UTF8String]);
    }
    
    
}


void _reauthorizeSessionWithReadPermissions(const char** permissions, int size) {
    reauthorizeSessionWithPermissionsOfType(permissions,size,"read");
}

void _reauthorizeSessionWithPublishPermissions(const char** permissions, int size) {
    reauthorizeSessionWithPermissionsOfType(permissions, size, "publish");
}


void reauthorizeSessionWithPermissionsOfType(const char** permissions, int size, const char *type) {
    
    
    NSMutableArray *nspermissions = [NSMutableArray array];
    
    for (int i=0;i<size;i++) {
        [nspermissions addObject:[NSString stringWithUTF8String:permissions[i]]];
    }
    
    // Start authentication flow
    FBReauthorizeSessionCompletionHandler completionHandler = [GamedoniaSDKFacebookHelper reauthorizeSessionCompletionHandler];
    
    @try {
        if (strcmp(type, "read") == 0)
        {
            [[FBSession activeSession] requestNewReadPermissions:nspermissions completionHandler:completionHandler];
        }
        else if (strcmp(type, "publish") == 0)
        {
            [[FBSession activeSession] requestNewPublishPermissions:nspermissions defaultAudience:FBSessionDefaultAudienceFriends completionHandler:completionHandler];
        }
    }
    @catch (NSException *exception) {
        onStatus("REAUTHORIZE_SESSION_ERROR", [[exception reason] UTF8String]);
    }
    
    
}

void _closeSessionAndClearTokenInformation() {
    
    [[FBSession activeSession] closeAndClearTokenInformation];
    
}


void _requestWithGraphPath(const char* graphPath, const char* parameters, const char* httpMethod, const char* callbackName) {
    
    NSString *nsgraphPath = [NSString stringWithUTF8String:graphPath];
    
    
    NSDictionary *nsparameters = NULL;
    if (parameters != NULL) {
        NSString* jsonString = [NSString stringWithUTF8String:parameters];
        nsparameters = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding] options:kNilOptions error:nil];
    }else {
        nsparameters = [NSDictionary dictionary];
    }
    
    
    
    NSString * nshttpMethod = [NSString stringWithUTF8String:httpMethod];
   
    NSLog(@"HTTP METHOD: %s",httpMethod);
    NSString *callback = [NSString stringWithUTF8String:callbackName];
    
    FBRequest *request = [FBRequest requestWithGraphPath:nsgraphPath parameters:nsparameters HTTPMethod:nshttpMethod];
    
    FBRequestCompletionHandler completionHandler = [GamedoniaSDKFacebookHelper requestCompletionHandlerWithCallback:callback];
    [request startWithCompletionHandler:completionHandler];
    
    
}

void _getAccessToken(char* destAccessToken,  size_t n) {
    
    FBSession *session = [FBSession activeSession];
    NSString *accessToken = session.accessTokenData.accessToken;
    
    NSLog(@"Access token: %@",accessToken);
    
    if ([accessToken length] == 0) {
        strncpy(destAccessToken, "", n);
    }else {
        const char* strAccessToken = [accessToken UTF8String];
        strncpy(destAccessToken, strAccessToken, n);
    }
    
}

void _dialog(const char* method, const char* parameters, BOOL allowNativeUI, const char* callbackName) {
    
    
    NSString *nsmethod = [NSString stringWithUTF8String:method];
    
    
    NSDictionary *nsparameters = NULL;
    if (parameters != NULL) {
        NSString* jsonString = [NSString stringWithUTF8String:parameters];
        nsparameters = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding] options:kNilOptions error:nil];
    }else {
        nsparameters = [NSDictionary dictionary];
    }
    
    //NSString* jsonString = [NSString stringWithUTF8String:parameters];
    //NSDictionary *nsparameters = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding] options:kNilOptions error:nil];
    
    
    
    NSString *callback = [NSString stringWithUTF8String:callbackName];
    
    
    // If possible, open new-style Facebook sharing sheet
    FBSession *session = [FBSession activeSession];
    BOOL canPresentNativeDialog = [FBDialogs canPresentOSIntegratedShareDialogWithSession:session];
    BOOL isFeedDialog = [nsmethod isEqualToString:@"feed"];
    BOOL isRequestDialog = [nsmethod isEqualToString:@"apprequests"];
    BOOL hasNoRecipient = ([nsparameters objectForKey:@"to"] == nil || [[nsparameters objectForKey:@"to"] length] == 0);
    
    NSLog(@"displaying facebook feed dialog : allowNativeUI - %@, canPresentNativeDialog - %@, isFeedingDialog - %@, hasNoRecipient - %@",allowNativeUI ? @"YES" : @"NO",
          canPresentNativeDialog ? @"YES" : @"NO",
          isFeedDialog ? @"YES" : @"NO",
          hasNoRecipient ? @"YES" : @"NO");
    
    if (allowNativeUI && canPresentNativeDialog && isFeedDialog && hasNoRecipient)
    {
        UIViewController *rootViewController = [[[UIApplication sharedApplication] keyWindow] rootViewController];
        NSString *initialText = [nsparameters objectForKey:@"name"];
        UIImage *image = nil;
        NSURL *url = [NSURL URLWithString:[nsparameters objectForKey:@"link"]];
        FBOSIntegratedShareDialogHandler handler = [GamedoniaSDKFacebookHelper shareDialogHandlerWithCallback:callback];
        
        // If there is an image, try to download it
        NSString *picture = [nsparameters objectForKey:@"picture"];
        if (picture && picture.length > 0)
        {
            NSURL *pictureURL = [NSURL URLWithString:picture];
            NSURLRequest *request = [NSURLRequest requestWithURL:pictureURL cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:2];
            NSURLResponse *response = nil;
            NSData *pictureData = [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:NULL];
            image = [UIImage imageWithData:pictureData];
        }
        
        [FBDialogs presentOSIntegratedShareDialogModallyFrom:rootViewController initialText:initialText image:image url:url handler:handler];
    }
    else // Else, open old-style Facebook dialog
    {
        if (isFeedDialog)
        {
            [FBWebDialogs presentFeedDialogModallyWithSession:nil
                                                   parameters:nsparameters
                                                      handler:
             ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
                 
                 if (error) {
                     // TODO handle errors on a low level using FB SDK
                     NSString *data = [NSString stringWithFormat:@"{ \"error\" : \"%@\"}", [error description]];
                     onStatus([callback UTF8String], [data UTF8String]);
                     //[AirFacebook dispatchEvent:callback withMessage:data];
                 } else {
                     if (result == FBWebDialogResultDialogNotCompleted) {
                         NSLog(@"User canceled story publishing.");
                         //[AirFacebook dispatchEvent:callback withMessage:@"{ \"cancel\" : true}"];
                         onStatus([callback UTF8String], "{ \"cancel\" : true}");
                     } else {
                         NSString *queryString = [resultURL query];
                         NSString *data = queryString ? [NSString stringWithFormat:@"{ \"params\" : \"%@\"}", queryString] : @"{ \"cancel\" : true}";
                         onStatus([callback UTF8String], [data UTF8String]);
                         //[AirFacebook dispatchEvent:callback withMessage:data];
                     }
                 }
             }
             ];
        } else if (isRequestDialog)
        {
            
            //if (![[AirFacebook sharedInstance] frictionless]) {
                [FBWebDialogs presentRequestsDialogModallyWithSession:nil message:[nsparameters objectForKey:@"message"] title:nil parameters:nsparameters handler:
                 ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
                     
                     if (error) {
                         // TODO handle errors on a low level using FB SDK
                         NSString *data = [NSString stringWithFormat:@"{ \"error\" : \"%@\"}", [error description]];
                         onStatus([callback UTF8String], [data UTF8String]);
                         //[AirFacebook dispatchEvent:callback withMessage:data];
                     } else {
                         if (result == FBWebDialogResultDialogNotCompleted) {
                             NSLog(@"User canceled story publishing.");
                             //[AirFacebook dispatchEvent:callback withMessage:@"{ \"cancel\" : true}"];
                             onStatus([callback UTF8String], "{ \"cancel\" : true}");
                         } else {
                             NSString *queryString = [resultURL query];
                             NSString *data = queryString ? [NSString stringWithFormat:@"{ \"params\" : \"%@\"}", queryString] : @"{ \"cancel\" : true}";
                             onStatus([callback UTF8String], [data UTF8String]);
                             //[AirFacebook dispatchEvent:callback withMessage:data];
                         }
                     }
                 }
                 ];
                //friendCache:[[AirFacebook sharedInstance] friendCache]];
            /*}else {
                //Frictionless
                NSLog(@"Friction less request");
                if (ms_friendCache == NULL) {
                    ms_friendCache = [[FBFrictionlessRecipientCache alloc] init];
                }
                
                [ms_friendCache prefetchAndCacheForSession:nil];
                
                [FBWebDialogs presentRequestsDialogModallyWithSession:nil message:[parameters objectForKey:@"message"] title:nil parameters:parameters handler:
                 ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
                     
                     if (error) {
                         // TODO handle errors on a low level using FB SDK
                         NSString *data = [NSString stringWithFormat:@"{ \"error\" : \"%@\"}", [error description]];
                         [AirFacebook dispatchEvent:callback withMessage:data];
                     } else {
                         if (result == FBWebDialogResultDialogNotCompleted) {
                             NSLog(@"User canceled story publishing.");
                             [AirFacebook dispatchEvent:callback withMessage:@"{ \"cancel\" : true}"];
                         } else {
                             NSString *queryString = [resultURL query];
                             NSString *data = queryString ? [NSString stringWithFormat:@"{ \"params\" : \"%@\"}", queryString] : @"{ \"cancel\" : true}";
                             [AirFacebook dispatchEvent:callback withMessage:data];
                         }
                     }
                 }
                                                          friendCache:ms_friendCache];
            }*/
        } else
        {
            
            [FBWebDialogs presentDialogModallyWithSession:nil dialog:nsmethod parameters:nsparameters
                                                  handler:
             ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
                 
                 if (error) {
                     // TODO handle errors on a low level using FB SDK
                     NSString *data = [NSString stringWithFormat:@"{ \"error\" : \"%@\"}", [error description]];
                     onStatus([callback UTF8String], [data UTF8String]);
                     //[AirFacebook dispatchEvent:callback withMessage:data];
                 } else {
                     if (result == FBWebDialogResultDialogNotCompleted) {
                         NSLog(@"User canceled story publishing.");
                         onStatus([callback UTF8String], "{ \"cancel\" : true}");
                         //[AirFacebook dispatchEvent:callback withMessage:@"{ \"cancel\" : true}"];
                     } else {
                         NSString *queryString = [resultURL query];
                         NSString *data = queryString ? [NSString stringWithFormat:@"{ \"params\" : \"%@\"}", queryString] : @"{ \"cancel\" : true}";
                         onStatus([callback UTF8String], [data UTF8String]);
                         //[AirFacebook dispatchEvent:callback withMessage:data];
                     }
                 }
             }
             ];
            
        }
    }
    
}

void _publishInstall(const char* appId) {
    
}


/*
bool GamedoniaSDKFacebook::handleOpenUrl(const char *url) {
    
    NSURL *_url = [NSURL URLWithString:[NSString stringWithUTF8String:(char*)url]];
    
    // Give the URL to the Facebook session
    FBSession *session = [FBSession activeSession];
    return [session handleOpenURL:_url];
    
}
 */

/*

GamedoniaSDKFacebookRequestWithGraphPathResponseCallback::GamedoniaSDKFacebookRequestWithGraphPathResponseCallback(CCObject *target, SEL_GamedoniaFacebookRequestWithGraphPathResponse method) {
    this->target = target;
    this->method = method;
    
}

GamedoniaSDKFacebookRequestWithGraphPathResponseCallback::~GamedoniaSDKFacebookRequestWithGraphPathResponseCallback() {
    
}

CCObject *GamedoniaSDKFacebookRequestWithGraphPathResponseCallback::getTarget() {
    return this->target;
}

SEL_GamedoniaFacebookRequestWithGraphPathResponse GamedoniaSDKFacebookRequestWithGraphPathResponseCallback::getMethod() {
    return this->method;
}*/