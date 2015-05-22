inline static void onStatus(const char *eventName, const char *message) {
    
    
    NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:[NSString stringWithUTF8String:eventName],@"eventName", [NSString stringWithUTF8String:message],@"message", nil];
    
    NSError *error = nil;
    NSData *json;
    
    // Dictionary convertable to JSON ?
    if ([NSJSONSerialization isValidJSONObject:dict])
    {
        // Serialize the dictionary
        json = [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:&error];
        
        // If no errors, let's view the JSON
        if (json != nil && error == nil)
        {
            NSString *jsonString = [[NSString alloc] initWithData:json encoding:NSUTF8StringEncoding];
            UnitySendMessage("Gamedonia", "OnFacebookStatus", [jsonString UTF8String]);
            
            //NSLog(@"JSON: %@", jsonString);
            //[jsonString release];
        }
    }
    
    
    //TODO Migrar esto
    /*
     NSString* event = [NSString stringWithUTF8String:eventName];
     NSString* nsmessage =[NSString stringWithUTF8String:message];
     
     NSLog(@"Facebook event received with name: %@ and message: %@",[NSString stringWithUTF8String:eventName],[NSString stringWithUTF8String:eventName]);
     //CCLOG("Facebook event received with name: %s and message: %s",eventName,message);
     
     
     
     if ([event rangeOfString:@"SESSION"].location != NSNotFound) {
     
     bool success = ([event rangeOfString:@"SUCCESS"].location != NSNotFound);
     bool userCancelled = ([event rangeOfString:@"CANCEL"].location != NSNotFound);
     const char *error = ([event rangeOfString:@"ERROR"].location != NSNotFound)? message:NULL;
     
     if ([event rangeOfString:@"OPEN"].location != NSNotFound)  {
     
     if (openSessionListenerInstance != NULL && openSessionListenerMethod != NULL) (openSessionListenerInstance->*openSessionListenerMethod)(success,userCancelled,error);
     openSessionListenerInstance = NULL;
     openSessionListenerMethod = NULL;
     
     }else if ([event rangeOfString:@"REAUTHORIZE"].location != NSNotFound) {
     
     if (reauthorizeSessionListenerInstance != NULL && reauthorizeSessionListenerMethod != NULL) (reauthorizeSessionListenerInstance->*reauthorizeSessionListenerMethod)(success,userCancelled,error);
     reauthorizeSessionListenerInstance = NULL;
     reauthorizeSessionListenerMethod = NULL;
     
     }
     
     
     
     }else if ([event isEqualToString:@"LOGGING"]) {
     NSLog("%@",nsmessage);
     }else {
     
     if (this->callbacks != NULL) {
     CCObject *obj = this->callbacks->objectForKey(eventName);
     if (obj != NULL) {
     GamedoniaSDKFacebookRequestWithGraphPathResponseCallback *callback = dynamic_cast<GamedoniaSDKFacebookRequestWithGraphPathResponseCallback *>(obj);
     CCLOG("A invocar al callback");
     
     Document *data = new Document();
     @try
     {
     
     data->Parse<0>(message);
     
     const char *accessToken = getAccessToken();
     if (accessToken != NULL) {
     Value vAccessToken;
     vAccessToken.SetString(accessToken, strlen(accessToken));
     data->AddMember("accessToken", vAccessToken, data->GetAllocator());
     
     }
     
     }
     @catch (NSException *ex)
     {
     NSLog(@"ERROR - %@", [ex description]);
     }
     
     //Invocamos al callback con los datos
     CCObject * target = callback->getTarget();
     SEL_GamedoniaFacebookRequestWithGraphPathResponse method = callback->getMethod();
     (target->*method)(data);
     
     callbacks->removeObjectForKey(eventName);
     if (callbacks->count() == 0) {
     delete callbacks;
     callbacks = NULL;
     }
     }
     }
     
     }
     */
    
}
