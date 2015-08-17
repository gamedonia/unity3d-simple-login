//
//  GamedoniaGoogle.m
//  Unity-iPhone
//
//  Created by Alberto Xaubet Matesanz on 14/7/15.
//
//

#import <Foundation/Foundation.h>
#import <GoogleSignIn/GoogleSignIn.h>
#import "GoogleHelper.h"
#include "AppDelegateListener.h"

#include "GamedoniaGoogle.h"

void _initGoogle(char* clientId) {
    NSLog(@"Inicializando!!! Google +");
    
    NSDictionary* infoDict = [[NSBundle mainBundle] infoDictionary];
    NSString* iosClientId = [infoDict objectForKey:@"GoogleClientId"];
    
    [GIDSignIn sharedInstance].clientID = iosClientId;
    [GIDSignIn sharedInstance].serverClientID = [NSString stringWithUTF8String:clientId];
    [GIDSignIn sharedInstance].shouldFetchBasicProfile = YES;
    [GIDSignIn sharedInstance].delegate = [GoogleHelper sharedHelper];
    [GIDSignIn sharedInstance].uiDelegate = [GoogleHelper sharedHelper];
    

    //Nos registramos para el centro de notificaciones
    [[NSNotificationCenter defaultCenter] addObserver:[GoogleHelper sharedHelper] selector:@selector(handleOpenUrl:) name:kUnityOnOpenURL object:nil];
}

void _signIn() {
    
    
    int sysVer = [[[UIDevice currentDevice] systemVersion] intValue];
    
    if (sysVer >= 7) {
        [[GIDSignIn sharedInstance] signIn];
    }else {
        [[GoogleHelper sharedHelper] onStatus:@"OPEN_SESSION_ERROR" message:@"Google + Sign in requires iOS 7 or greater"];
    }
    
    
}
