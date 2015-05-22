//
//  GDAlertView.m
//  GamedoniaSDKSample
//
//  Created by Alberto Xaubet Matesanz on 10/01/13.
//  Copyright (c) 2013 Edenic Games S.L. All rights reserved.
//

#import "GDAlertView.h"

@implementation GDAlertView


- (id)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        // Initialization code
    }
    return self;
}

- (id) initWithParams:(NSDictionary *)params
{
    
    self = [super initWithTitle:[params objectForKey:@"title"]
                  message:[params objectForKey:@"message"]
                  delegate:nil
                  cancelButtonTitle:[params objectForKey:@"cancelButtonTitle"]
                  otherButtonTitles:nil
    ];
    
    for (NSDictionary *btn in [params objectForKey:@"otherButtons"]) {
        [self addButtonWithTitle:[btn objectForKey:@"title"]];
    }    
    
    return self;
}

@end
