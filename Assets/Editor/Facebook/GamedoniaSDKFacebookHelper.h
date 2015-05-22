//
//  GamedoniaSDKFacebookHelper.h
//  GamedoniaSDK
//
//  Created by Alberto Xaubet Matesanz on 17/12/13.
//
//

#import "FacebookSDK/FacebookSDK.h"
#import <Foundation/Foundation.h>


typedef void (^FBOpenSessionCompletionHandler)(FBSession *session, FBSessionState status, NSError *error);
typedef void (^FBReauthorizeSessionCompletionHandler)(FBSession *session, NSError *error);
typedef void (^FBRequestCompletionHandler)(FBRequestConnection *connection, id result, NSError *error);

@interface GamedoniaSDKFacebookHelper : NSObject


+ (id)sharedInstance;

//+ (void)dispatchEvent:(NSString *)event withMessage:(NSString *)message;

- (void)setupWithAppID:(NSString *)appID urlSchemeSuffix:(NSString *)urlSchemeSuffix;
- (void)handleNotification:(NSNotification*)notification;
- (bool)handleOpenUrl:(NSString *)url;

/*
void openSessionWithReadPermissions(CCArray *permissions, CCObject *target,SEL_GamedoniaFacebookOpenSessionResponse pSelector);
void openSessionWithReadPermissions(CCArray *permissions, CCObject *target,SEL_GamedoniaFacebookOpenSessionResponse pSelector, bool allowUI);
void openSessionWithPublishPermissions(CCArray *permissions, CCObject *target,SEL_GamedoniaFacebookOpenSessionResponse pSelector);
void openSessionWithPermissions(CCArray *permissions, CCObject *target,SEL_GamedoniaFacebookOpenSessionResponse pSelector);
*/

+ (FBOpenSessionCompletionHandler)openSessionCompletionHandler;
+ (FBReauthorizeSessionCompletionHandler)reauthorizeSessionCompletionHandler;
+ (FBRequestCompletionHandler)requestCompletionHandlerWithCallback:(NSString *)callback;
+ (FBOSIntegratedShareDialogHandler)shareDialogHandlerWithCallback:(NSString *)callback;

+ (void)log:(NSString *)string, ...;

@property (nonatomic, readonly) NSString *appID;
@property (nonatomic, readonly) NSString *urlSchemeSuffix;
@property (nonatomic, readonly) bool frictionless;
@property (nonatomic, readonly) FBFrictionlessRecipientCache *friendCache;

@end
