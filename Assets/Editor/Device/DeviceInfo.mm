//
//  DeviceInfo.m
//  Unity-iPhone
//
//  Created by Javier Garcia on 23/12/15.
//
//

#import <AdSupport/ASIdentifierManager.h>
#import <sys/utsname.h>
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

char* GetDeviceDevice() {
    const char* device = [[[UIDevice currentDevice] model] UTF8String];
    char* cpy = (char*)malloc(strlen(device) + 1);
    strcpy(cpy, device);
    return cpy;
}

char* GetDevicePlatform() {
    const char* devicePlatform = [[[UIDevice currentDevice] systemName] UTF8String];
    char* cpy = (char*)malloc(strlen(devicePlatform) + 1);
    strcpy(cpy, devicePlatform);
    return cpy;
}

char* GetDeviceBuildVersion() {
    const char* buildVersion = [[[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleShortVersionString"] UTF8String];
    char* cpy = (char*)malloc(strlen(buildVersion) + 1);
    strcpy(cpy, buildVersion);
    return cpy;
}

char* GetDeviceOS() {
    
    const char* deviceOS = [[[UIDevice currentDevice] systemName] UTF8String];
    char* cpy = (char*)malloc(strlen(deviceOS) + 1);
    strcpy(cpy, deviceOS);
    return cpy;
    
}

char* GetDeviceOSVersion() {
    
    const char* deviceOS = [[[UIDevice currentDevice] systemVersion] UTF8String];
    char* cpy = (char*)malloc(strlen(deviceOS) + 1);
    strcpy(cpy, deviceOS);
    return cpy;
}

char* GetDeviceIMEI() {

    //Not allowed by Apple
    const char* imei = "";
    char* cpy = (char*)malloc(strlen(imei) + 1);
    strcpy(cpy, imei);
    return cpy;
    
}

char* GetDeviceIDFA() {

    const char*  idfa = "";
    if([[[UIDevice currentDevice] systemVersion] doubleValue]>=6.0)
    {
        if([[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled]) {
            idfa = [[[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString] UTF8String];
        }
    }
    
    char* cpy = (char*)malloc(strlen(idfa) + 1);
    strcpy(cpy, idfa);
    return cpy;
    
}

char* GetDeviceIDFV() {
    
    const char* udid = "";
    
    if([[[UIDevice currentDevice] systemVersion] doubleValue]>=6.0)
    {
        udid = [[[[UIDevice currentDevice] identifierForVendor] UUIDString] UTF8String];
    }
    
    char* cpy = (char*)malloc(strlen(udid) + 1);
    strcpy(cpy, udid);
    return cpy;
    
}

char* GetDeviceDeviceManufacturer() {

    //Not applicable
    const char* apple = "Apple";
    char* cpy = (char*)malloc(strlen(apple) + 1);
    strcpy(cpy, apple);
    return cpy;
    
}

char* GetDeviceDeviceModel() {
    
    //Not applicable
    const char* deviceModel = [[[UIDevice currentDevice] localizedModel] UTF8String];
    char* cpy = (char*)malloc(strlen(deviceModel) + 1);
    strcpy(cpy, deviceModel);
    return cpy;
    
}

char* GetDeviceDeviceHardware() {
    
    
    struct utsname systemInfo;
    uname(&systemInfo);
    
    const char* machine =  [[NSString stringWithCString:systemInfo.machine encoding:NSUTF8StringEncoding] UTF8String];
    char* cpy = (char*)malloc(strlen(machine) + 1);
    strcpy(cpy, machine);
    return cpy;
    
}

char* GetDeviceDeviceSystem() {
    
    //Not applicable
    const char* androidId = "";
    char* cpy = (char*)malloc(strlen(androidId) + 1);
    strcpy(cpy, androidId);
    return cpy;
}

bool isDeviceAdTrackingEnabled() {
    
    return ([[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled]);
}

bool isDeviceJailBroken () {
    
    return ([[NSFileManager defaultManager] fileExistsAtPath:@"/private/var/lib/apt"] || [[NSFileManager defaultManager] fileExistsAtPath:@"/Applications/Cydia.app"]);
    
}


