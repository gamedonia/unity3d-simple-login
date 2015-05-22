//
//  GamedoniaUI.m
//  GamedoniaSDKSample
//
//  Created by Alberto Xaubet Matesanz on 10/01/13.
//  Copyright (c) 2013 Edenic Games S.L. All rights reserved.
//

#import "GamedoniaUI.h"
#import "GamedoniaUIHelper.h"

void showAlertDialog(char * params) {
    NSError * e = nil;
    NSMutableDictionary * alertParams = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String: params] dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingMutableContainers error:&e];
    [[GamedoniaUIHelper sharedHelper] showAlertDialogWithParams:alertParams];
}