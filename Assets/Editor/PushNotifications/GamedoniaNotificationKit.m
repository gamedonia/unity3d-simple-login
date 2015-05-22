#include "GamedoniaNotificationKit.h"


void ClearBadge()
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}

void _RegisterForRemoteNotifications (int types)
{
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 80000
    /*UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:(UIRemoteNotificationTypeBadge
                                                                                         |UIRemoteNotificationTypeSound
                                                                                         |UIRemoteNotificationTypeAlert) categories:nil];
     */        
    UIUserNotificationSettings *settings = [UIUserNotificationSettings settingsForTypes:types categories:nil];
    [[UIApplication sharedApplication] registerUserNotificationSettings:settings];
    [[UIApplication sharedApplication] registerForRemoteNotifications];
#else
    //UIRemoteNotificationType myTypes = UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeAlert | UIRemoteNotificationTypeSound;
    NSLog(@">>>> Registering for remote notifications");
    [[UIApplication sharedApplication] registerForRemoteNotificationTypes:types];
#endif
}


