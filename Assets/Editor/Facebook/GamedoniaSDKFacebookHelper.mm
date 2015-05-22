//
//  GamedoniaSDKFacebookHelper.m
//  GamedoniaSDK
//
//  Created by Alberto Xaubet Matesanz on 17/12/13.
//
//

#import "GamedoniaSDKFacebookHelper.h"
#include "GamedoniaFacebookUtils.h"
#include "AppDelegateListener.h"

@implementation GamedoniaSDKFacebookHelper

@synthesize appID = _appID;
@synthesize urlSchemeSuffix = _urlSchemeSuffix;
@synthesize frictionless = _frictionless;
@synthesize friendCache = _friendCache;


static GamedoniaSDKFacebookHelper *sharedInstance = nil;

+ (GamedoniaSDKFacebookHelper *)sharedInstance
{
    if (sharedInstance == nil)
    {
        sharedInstance = [[super allocWithZone:NULL] init];
    }
    
    return sharedInstance;
}

- (void)setupWithAppID:(NSString *)appID urlSchemeSuffix:(NSString *)urlSchemeSuffix
{

    // Save parameters
    _appID = appID;
    _urlSchemeSuffix = urlSchemeSuffix;
    _friendCache = [[FBFrictionlessRecipientCache alloc] init];
    //_frictionless = frictionless;
    NSMutableString *logMessage = [NSMutableString stringWithFormat:@"Initializing with application ID %@", _appID];
    if (_urlSchemeSuffix)
    {
        [logMessage appendFormat:@" and URL scheme suffix %@", _urlSchemeSuffix];
    }
    NSLog(logMessage);
    
    // Open session if a token is in cache.
    FBSession *session = nil;
    @try
    {
        session = [[FBSession alloc] initWithAppID:appID permissions:nil urlSchemeSuffix:urlSchemeSuffix tokenCacheStrategy:[FBSessionTokenCachingStrategy defaultInstance]];
    }
    @catch (NSException *exception)
    {
                
        onStatus("LOGGING",[[exception reason] UTF8String]);
        return;
    }
    
    [FBSession setActiveSession:session];
    if (session.state == FBSessionStateCreatedTokenLoaded)
    {
        NSLog(@"Opening session from cached token");
        
        @try
        {
            [session openWithBehavior:FBSessionLoginBehaviorUseSystemAccountIfPresent completionHandler:[GamedoniaSDKFacebookHelper openSessionCompletionHandler]];
        }
        @catch (NSException *exception)
        {
            //TODO
            onStatus("LOGGING", [[exception reason] UTF8String]);
            return;
        }
    }
    
    [FBSession renewSystemCredentials:NULL];
    
    
    //Nos registramos para el centro de notificaciones
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleNotification:) name:kUnityOnOpenURL object:nil];

}

- (void) handleNotification:(NSNotification *)notification
{
    NSLog(@"Notification received Unity OpenURL");
    
    NSDictionary* userInfo = [notification userInfo];
    NSURL* url = [userInfo objectForKey:@"url"];
    
    NSLog(@"URL: %@", url);
    
    FBSession *session = [FBSession activeSession];
    [session handleOpenURL:url];
    //[self handleOpenUrl:url];
}

- (bool)handleOpenUrl:(NSString *)url {
    
        NSURL *_url = [NSURL URLWithString:url];
        
        // Give the URL to the Facebook session
        FBSession *session = [FBSession activeSession];
        return [session handleOpenURL:_url];
    
}

+ (FBOpenSessionCompletionHandler)openSessionCompletionHandler
{
    return ^(FBSession *session, FBSessionState status, NSError *error) {
 
        if (error) {            
            if ([error respondsToSelector:@selector(fberrorShouldNotifyUser)] && error.fberrorShouldNotifyUser) {
                // if the error is application turned off from ios6 settings
                if ([[error userInfo][FBErrorLoginFailedReason] isEqualToString:FBErrorLoginFailedReasonSystemDisallowedWithoutErrorValue]) {
                    //[AirFacebook dispatchEvent:@"OPEN_SESSION_ERROR" withMessage:@"APPLICATION_TURNED_OFF"];
                    onStatus("OPEN_SESSION_ERROR","APPLICATION_TURNED_OFF");
                } else {
                    //[AirFacebook dispatchEvent:@"OPEN_SESSION_ERROR" withMessage:error.fberrorUserMessage];
                    onStatus("OPEN_SESSION_ERROR",[error.fberrorUserMessage UTF8String]);
                }
            } else if ([error respondsToSelector:@selector(fberrorCategory)] && error.fberrorCategory == FBErrorCategoryUserCancelled) {
                NSLog(@"Login error : User Cancelled (Error details : %@ )", error.description);
                //[AirFacebook dispatchEvent:@"OPEN_SESSION_CANCEL" withMessage:@"OK"];
                onStatus("OPEN_SESSION_CANCEL","OK");
            } else {
                //[AirFacebook log:@"Unexpected Error on login (Error details : %@ )", error.description];
                //[AirFacebook dispatchEvent:@"OPEN_SESSION_ERROR" withMessage:[error description]];
                onStatus("OPEN_SESSION_ERROR",[[error description] UTF8String]);
            }
        }
        
        if (status == FBSessionStateOpen)
        {
            NSLog(@"Session opened with permissions: %@", [session.permissions componentsJoinedByString:@", "]);
            //[AirFacebook dispatchEvent:@"OPEN_SESSION_SUCCESS" withMessage:@"OK"];
            onStatus("OPEN_SESSION_SUCCESS","OK");
            [[[GamedoniaSDKFacebookHelper sharedInstance] friendCache] prefetchAndCacheForSession:nil];
        }
        else if (status == FBSessionStateClosed)
        {
            NSLog(@"Session closed");
        }
    };
}

+ (FBReauthorizeSessionCompletionHandler)reauthorizeSessionCompletionHandler
{
    return ^(FBSession *session, NSError *error) {
        
        if (error)
        {
            if (error.fberrorShouldNotifyUser) {
                // show sdk message
                NSLog(@"Error when reauthorizing session: %@", [error description]);
                onStatus("REAUTHORIZE_SESSION_ERROR", [[error description] UTF8String]);
            } else {
                if (error.fberrorCategory == FBErrorCategoryUserCancelled){
                    // User Cancelled
                    NSLog(@"User cancelled when reauthorizing session");
                    onStatus("REAUTHORIZE_SESSION_CANCEL", "OK");
                } else {
                    NSLog(@"Error when reauthorizing session: %@", [error description]);
                    onStatus("REAUTHORIZE_SESSION_ERROR", [[error description] UTF8String]);
                }
            }
        }
        else
        {
            NSLog(@"Session reauthorized with permissions: %@", session.permissions);            
            [[[GamedoniaSDKFacebookHelper sharedInstance] friendCache] prefetchAndCacheForSession:nil];
            onStatus("REAUTHORIZE_SESSION_SUCCESS", "OK");
        }
    };
}

+ (FBRequestCompletionHandler)requestCompletionHandlerWithCallback:(NSString *)callback
{
    return [^(FBRequestConnection *connection, id result, NSError *error) {
        if (error)
        {
            
            // If user doesn't have the publish permission, ask them
            if (error.fberrorCategory == FBErrorCategoryPermissions) {
                NSLog(@"Requesting publish permissions");
                //[AirFacebook dispatchEvent:@"ACTION_REQUIRE_PERMISSION" withMessage:@"publish_actions"];
                onStatus("ACTION_REQUIRE_PERMISSION", "publish actions");
                return;
            } else if (callback)
			{
                NSDictionary* parsedResponseKey = [error.userInfo objectForKey:FBErrorParsedJSONResponseKey];
                if (parsedResponseKey && [parsedResponseKey objectForKey:@"body"])
                {
                    NSDictionary* body = [parsedResponseKey objectForKey:@"body"];
                    NSError *jsonError = nil;
                    NSData *resultData = [NSJSONSerialization dataWithJSONObject:body options:0 error:&jsonError];
                    if (jsonError)
                    {
                        //[AirFacebook log:[NSString stringWithFormat:@"Request error -> JSON error: %@", [jsonError description]]];
                        NSLog(@"Request error -> JSON error: %@",[jsonError description]);
                    } else
                    {
                        NSString *resultString = [[NSString alloc] initWithData:resultData encoding:NSUTF8StringEncoding];
                        onStatus([callback UTF8String], [resultString UTF8String]);
                        //FREDispatchStatusEventAsync(AirFBCtx, (const uint8_t *)[callback UTF8String], (const uint8_t *)[resultString UTF8String]);
                    }
                }
                return;
			}
            
            NSLog(@"Request error: %@", [error description]);
            
        }
        else
        {
            NSError *jsonError = nil;
            NSData *resultData = [NSJSONSerialization dataWithJSONObject:result options:0 error:&jsonError];
            if (jsonError)
            {
                 NSLog(@"Request JSON error: %@", [jsonError description]);
            }
            else
            {
                NSString *resultString = [[NSString alloc] initWithData:resultData encoding:NSUTF8StringEncoding];
                onStatus([callback UTF8String], [resultString UTF8String]);
                //[AirFacebook dispatchEvent:callback withMessage:resultString];
            }
            
        }
    } copy];
}

+ (FBOSIntegratedShareDialogHandler)shareDialogHandlerWithCallback:(NSString *)callback
{
    return [^(FBOSIntegratedShareDialogResult result, NSError *error) {
        NSString *resultString = nil;
        switch (result)
        {
            case FBOSIntegratedShareDialogResultCancelled:
                resultString = @"{ \"cancelled\": true}";
                break;
                
            case FBOSIntegratedShareDialogResultError:
                resultString = [NSString stringWithFormat:@"{ \"error\": \"%@\" }", [error description]];
                
            default:
                resultString = @"{}";
                break;
        }
        //[AirFacebook dispatchEvent:callback withMessage:resultString];
        onStatus([callback UTF8String], [resultString UTF8String]);
    } copy];
}

@end
