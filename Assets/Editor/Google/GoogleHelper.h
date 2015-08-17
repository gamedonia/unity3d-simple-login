//
//  GoogleHelper.h
//  Unity-iPhone
//
//  Created by Alberto Xaubet Matesanz on 14/7/15.
//
//

#ifndef Unity_iPhone_GoogleHelper_h
#define Unity_iPhone_GoogleHelper_h

#import <GoogleSignIn/GoogleSignIn.h>


@interface GoogleHelper : NSObject <GIDSignInDelegate,GIDSignInUIDelegate>

- (void) onStatus: (NSString*) eventName message:(NSString*) message;
+ (GoogleHelper *) sharedHelper;

@end

#endif
