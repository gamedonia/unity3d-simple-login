//
//  GamedoniaUIHelper.m
//  GamedoniaSDKSample
//
//  Created by Alberto Xaubet Matesanz on 10/01/13.
//  Copyright (c) 2013 Edenic Games S.L. All rights reserved.
//

#import "GamedoniaUIHelper.h"
#import "GDAlertView.h"

@implementation GamedoniaUIHelper

static GamedoniaUIHelper * _sharedHelper;

+ (GamedoniaUIHelper *) sharedHelper {
    
    if (_sharedHelper != nil) {
        return _sharedHelper;
    }
    _sharedHelper = [[GamedoniaUIHelper alloc] init];
    return _sharedHelper;
    
}

- (id)init {
    
    return self;
    
}

- (void) showAlertDialogWithParams:(NSDictionary *)params {
    GDAlertView * alert = [[GDAlertView alloc] initWithParams:params];
    alertViewSettings =
    (NSDictionary *)CFBridgingRelease(CFPropertyListCreateDeepCopy(kCFAllocatorDefault,
                                                           (CFPropertyListRef)(params),
                                                           kCFPropertyListImmutable));
    //alertViewSettings = [NSDictionary dictionaryWithDictionary: params];
    [alert setDelegate:self];
    [alert show];
}

- (void) alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    
    GDAlertView * alert = (GDAlertView *)alertView;
    NSString *title = [alert buttonTitleAtIndex:buttonIndex];
    NSDictionary * btn = [self findBtnByTitle:title];
    
    NSLog(@"Clicked btn %@ %@ %@", [btn objectForKey:@"title"], [btn objectForKey:@"target"], [btn objectForKey:@"function"]);
    UnitySendMessage([[btn objectForKey:@"target"] UTF8String], [[btn objectForKey:@"function"] UTF8String], [@"" UTF8String]);

}

- (NSDictionary *) findBtnByTitle:(NSString *)title {
    
    //Miramos si es el boton principal de cancel
    if([title isEqualToString:[alertViewSettings objectForKey:@"cancelButtonTitle"]]) {
        NSDictionary * btn = [NSDictionary dictionaryWithObjectsAndKeys:[alertViewSettings objectForKey:@"cancelButtonTitle"], @"title",[alertViewSettings objectForKey:@"cancelTarget"],@"target",[alertViewSettings objectForKey:@"cancelFunction"],@"function", nil];
        return btn;
    }
    
    for (NSDictionary *btn in [alertViewSettings objectForKey:@"otherButtons"]) {
        if([title isEqualToString:[btn objectForKey:@"title"]]) {
            return btn;
        }
    }
    
    return nil;
    
}
@end
