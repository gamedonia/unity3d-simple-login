//
//  InAppIAPHelper.m
//  bet2race
//
//  Created by Alberto Xaubet Matesanz on 04/10/12.
//  Copyright (c) 2012 Alberto Xaubet Matesanz. All rights reserved.
//

#import "InAppIAPHelper.h"

@implementation InAppIAPHelper

static InAppIAPHelper * _sharedHelper;

+ (InAppIAPHelper *) sharedHelper {
    
    if (_sharedHelper != nil) {
        return _sharedHelper;
    }
    _sharedHelper = [[InAppIAPHelper alloc] init];
    return _sharedHelper;
    
}

- (id)init {

    return self;
    
}

@end
