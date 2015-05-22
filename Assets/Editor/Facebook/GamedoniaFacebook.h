#ifdef __cplusplus
extern "C" {
#endif
    
    void _init(const char* appID,const char* urlSchemeSuffix, BOOL frictionless);
    BOOL _isSessionOpen();
    void _getAccessToken(char* accessToken, size_t n);
    void _openSessionWithReadPermissions(const char** permissions, int size, BOOL allowUI);
    void _openSessionWithPublishPermissions(const char** permissions, int size);
    void _openSessionWithPermissions(const char** permissions, int size);
    void _reauthorizeSessionWithReadPermissions(const char** permissions, int size);
    void _reauthorizeSessionWithPublishPermissions(const char** permissions, int size);
    void _closeSessionAndClearTokenInformation();
    void _requestWithGraphPath(const char* graphPath, const char* parameters, const char* httpMethod, const char* callback);
    void _dialog(const char* method, const char* parameters, BOOL allowNativeUI, const char* callback);
    void _publishInstall(const char* appId);
    void openSessionWithPermissionsOfType(const char** permissions, int size, const char *type, bool allowUI);
    
#ifdef __cplusplus
}


#endif