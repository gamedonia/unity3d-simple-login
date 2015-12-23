//
//  Silent.m
//  Unity-iPhone
//
//  Created by Alberto Xaubet Matesanz on 9/12/15.
//
//

#include "Silent.h"
#import "SSKeychain.h"
#import <Foundation/Foundation.h>
#import <Security/Security.h>

char* GetPlatformDeviceId() {
    

    NSString* service = [NSString stringWithFormat:@"%@-%@",@"GD",[[NSBundle mainBundle] bundleIdentifier]];

    NSString* password = [SSKeychain passwordForService:service account:@"silentId"];
    
    if (password == nil) {
        password = [[[UIDevice currentDevice] identifierForVendor] UUIDString];
        [SSKeychain setPassword:password forService:service account:@"silentId"];
    }
    
    const char* udid = [password UTF8String];
    char* cpy = (char*)malloc(strlen(udid) + 1);
    strcpy(cpy, udid);
    return cpy;
    
}