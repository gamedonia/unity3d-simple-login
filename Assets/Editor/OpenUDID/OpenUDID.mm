//
//  main.m
//  OpenUDID
//
//  Created by Alberto Xaubet Matesanz on 08/10/12.
//  Copyright (c) 2012 Alberto Xaubet Matesanz. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "OpenUDID.h"

#ifdef __cplusplus
extern "C" {
#endif
    char* GetOpenUDID()
    {
        
        NSString *openUDID = [[[UIDevice currentDevice] identifierForVendor] UUIDString];
        
        
        const char* udid = [openUDID UTF8String];
        char* cpy = (char*)malloc(strlen(udid) + 1);
        strcpy(cpy, udid);
        return cpy;
        
    }
#ifdef __cplusplus
}
#endif