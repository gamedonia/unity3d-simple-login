#ifdef __cplusplus
extern "C" {
#endif
    
    char* GetDeviceLanguageCode();
    char* GetDeviceCountryCode();
    double GetDeviceTimeZoneGMTOffset();
    char* GetDeviceDevice();
    char* GetDevicePlatform();
    char* GetDeviceBuildVersion();
    char* GetDeviceOS();
    char* GetDeviceOSVersion();
    char* GetDeviceIMEI();
    char* GetDeviceIDFA();
    char* GetDeviceIDFV();
    char* GetDeviceDeviceManufacturer();
    char* GetDeviceDeviceModel();
    char* GetDeviceDeviceHardware();
    char* GetDeviceDeviceSystem();
    bool isDeviceAdTrackingEnabled();
    bool isDeviceJailBroken ();
    
    /*
    device.idfa = GamedoniaDeviceInfo.GetIDFA ();
    device.idfv = GamedoniaDeviceInfo.GetIDFV ();
    device.androidId = GamedoniaDeviceInfo.GetAndroidId ();
    device.deviceManufacturer = GamedoniaDeviceInfo.GetDeviceManufacturer ();
    device.deviceModel = GamedoniaDeviceInfo.GetDeviceModel ();
    device.deviceHardware = GamedoniaDeviceInfo.GetDeviceHardware ();
    device.deviceSystem = GamedoniaDeviceInfo.GetDeviceSystem ();
    device.adTrackingEnabled = GamedoniaDeviceInfo.isAdTrackingEnabled ();
    device.jailbroken = GamedoniaDeviceInfo.isJailBroken ();
    */
    
#ifdef __cplusplus
}


#endif
