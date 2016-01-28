//
//  DeviceInfo.m
//  Unity-iPhone
//
//  Created by Javier Garcia on 23/12/15.
//
//

#include "DeviceInfo.h"


char* GetDeviceLanguageCode() {
    
    NSLocale *locale = [NSLocale currentLocale];
    NSString *language = [locale objectForKey: NSLocaleLanguageCode];
    const char* code = [language UTF8String];
    char* cpy = (char*)malloc(strlen(code) + 1);
    strcpy(cpy, code);
    return cpy;
    
}

char* GetDeviceCountryCode() {
    
    NSLocale *locale = [NSLocale currentLocale];
    NSString *country = [locale objectForKey: NSLocaleCountryCode];
    const char* code = [country UTF8String];
    char* cpy = (char*)malloc(strlen(code) + 1);
    strcpy(cpy, code);
    return cpy;
    
}

double GetDeviceTimeZoneGMTOffset() {
    NSInteger seconds = [[NSTimeZone localTimeZone] secondsFromGMT];
    double result = seconds / 3600;
    return result;
}