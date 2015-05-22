//
//  GamedoniaUIHelper.h
//  GamedoniaSDKSample
//
//  Created by Alberto Xaubet Matesanz on 10/01/13.
//  Copyright (c) 2013 Edenic Games S.L. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface GamedoniaUIHelper : NSObject <UIAlertViewDelegate> {
    NSDictionary * alertViewSettings;
}

-(void) showAlertDialogWithParams:(NSDictionary *)params;
-(void) alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex;

+ (GamedoniaUIHelper *) sharedHelper;

@end
