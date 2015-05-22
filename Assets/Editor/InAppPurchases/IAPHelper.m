//
//  IAPHelper.m
//  bet2race
//
//  Created by Alberto Xaubet Matesanz on 03/10/12.
//  Copyright (c) 2012 Alberto Xaubet Matesanz. All rights reserved.
//

#import "IAPHelper.h"
#import "SKProduct+priceAsString.h"
#import "NSData+Base64.h"

@implementation IAPHelper

@synthesize productIdentifiers = _productIdentifiers;
@synthesize products = _products;
@synthesize purchasedProducts = _purchasedProducts;
@synthesize request = _request;



- (void)requestProducts:(NSSet *) productIdentifiers {
    
    NSLog(@"Requested produts %d", [productIdentifiers count]);
    self.request = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers];
    _request.delegate = self;
    [_request start];
    
}

- (BOOL) canMakePayments {    
    return [SKPaymentQueue canMakePayments];
}

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {
    
    NSLog(@"Received products results %d", [[response products] count]);
    self.products = response.products;
    self.request = nil;
    
    
    NSMutableDictionary* responseData = [NSMutableDictionary dictionary];
    [responseData setValue:[NSNumber numberWithBool:YES] forKey:@"success"];
    [responseData setValue:@"" forKey:@"message"];
    

    NSString * json =[NSString stringWithFormat:@"[]"];
    NSMutableArray * productsList = [NSMutableArray array];
    
    for (SKProduct * product in [response products]) {
        
        //NSLog(@"Product identifier:%@ , description:%@ ,  priceLocale:%@", [product productIdentifier], [product localizedDescription], [product priceAsString]);
        
        NSMutableDictionary * productDict = [NSMutableDictionary dictionary];
        [productDict setObject:[product productIdentifier] forKey:(@"identifier")];
        [productDict setObject:[product localizedDescription] forKey:(@"description")];
        [productDict setObject:[product priceAsString]forKey:(@"priceLocale")];
        [productsList addObject:productDict];
        
    }
    
    //Handle incorrect product identifiers
    if ([response.invalidProductIdentifiers count] > 0) {
        //[responseData setValue:[NSNumber numberWithBool:NO] forKey:@"success"];
        
        NSMutableString* message = [NSMutableString stringWithString:@"Invalid product identifiers ["];
        int index = 0;
        for (NSString *invalidIdentifier in response.invalidProductIdentifiers) {
            if (index++ != 0) [message appendString:@","];
            [message appendString:invalidIdentifier];
        }
        
        [message appendString:@"]"];
        
        [responseData setValue:message forKey:@"message"];

    }
    
    [responseData setValue:productsList forKey:@"products"];
    
    
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:responseData
                                                       options:0 // Pass 0 if you don't care about the readability of the generated string
                                                         error:&error];
    
    if (! jsonData) {
        NSLog(@"Got an error: %@", error);
    } else {
        json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    //if ([responseData JSONString] != nil) json = [responseData JSONString];
    
    NSLog(@"Received products %@", json);
	UnitySendMessage("Gamedonia", "ProductsRequested", [json UTF8String]);
}

- (id)initWithProductIdentifiers:(NSSet *)productIdentifiers {
    if ((self = [super init])) {
        
        // Store product identifiers
        _productIdentifiers = productIdentifiers;
        
        // Check for previously purchased products
        NSMutableSet * purchasedProducts = [NSMutableSet set];
        for (NSString * productIdentifier in _productIdentifiers) {
            NSLog(@"Product Id: %@", productIdentifier);
            BOOL productPurchased = [[NSUserDefaults standardUserDefaults] boolForKey:productIdentifier];
            if (productPurchased) {
                [purchasedProducts addObject:productIdentifier];
                NSLog(@"Previously purchased: %@", productIdentifier);
            }
            NSLog(@"Not purchased: %@", productIdentifier);
        }
        self.purchasedProducts = purchasedProducts;
        
    }
    return self;
}


- (void)recordTransaction:(SKPaymentTransaction *)transaction {
    // Optional: Record the transaction on the server side...
}

- (void)provideContent:(NSString *)productIdentifier {
    
    NSLog(@"Toggling flag for: %@", productIdentifier);
    [[NSUserDefaults standardUserDefaults] setBool:TRUE forKey:productIdentifier];
    [[NSUserDefaults standardUserDefaults] synchronize];
    [_purchasedProducts addObject:productIdentifier];
    
    [[NSNotificationCenter defaultCenter] postNotificationName:kProductPurchasedNotification object:productIdentifier];
    
}

- (void)completeTransaction:(SKPaymentTransaction *)transaction {
    
    NSLog(@"completeTransaction...");
    
    [self recordTransaction: transaction];
    [self provideContent: transaction.payment.productIdentifier];
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
    NSLog(@"Buy successed, the %@ product with transaction identifier %@ was purchased", transaction.payment.productIdentifier, transaction.transactionIdentifier);
    
    NSString *receipt = [transaction.transactionReceipt base64EncodedString];
    
    NSString * json =[NSString stringWithFormat:@"{'success':true, 'status':'success', 'identifier':'%@', 'message':'', 'transactionId':'%@', 'receipt':'%@'}", transaction.payment.productIdentifier, transaction.transactionIdentifier, receipt];
    UnitySendMessage("Gamedonia", "ProductPurchased", [json UTF8String]);
    
}

- (void)restoreTransaction:(SKPaymentTransaction *)transaction {
    
    NSLog(@"restoreTransaction...");
    
    [self recordTransaction: transaction];
    [self provideContent: transaction.originalTransaction.payment.productIdentifier];
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
}

- (void)failedTransaction:(SKPaymentTransaction *)transaction {
    
    NSString * json;
    
    if (transaction.error.code != SKErrorPaymentCancelled)
    {
        NSLog(@"Transaction error: %@", transaction.error.description);
    }
    
    [[NSNotificationCenter defaultCenter] postNotificationName:kProductPurchaseFailedNotification object:transaction];
    
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    NSLog(@"Buying failed, Could not buy the %@ product caused by (%d):%@", transaction.payment.productIdentifier, transaction.error.code, transaction.error.localizedDescription);
    
    if (transaction.error.code != SKErrorPaymentCancelled) {
        json =[NSString stringWithFormat:@"{'success':false, 'status':'error', 'identifier':'%@', 'message':'%@', 'transactionId':'', 'receipt':''}", transaction.payment.productIdentifier, transaction.error.localizedDescription];    
    } else {
        json =[NSString stringWithFormat:@"{'success':false, 'status':'cancel', 'identifier':'%@', 'message':'%@', 'transactionId':'', 'receipt':''}", transaction.payment.productIdentifier, transaction.error.localizedDescription];        
    }
    
    UnitySendMessage("Gamedonia", "ProductPurchased", [json UTF8String]);
    
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
            default:
                break;
        }
    }
}


- (void)buyProductIdentifier:(NSString *)productIdentifier {
    
    NSLog(@"Buying %@...", productIdentifier);
    
    //SKPayment *payment = [SKPayment  paymentWithProductIdentifier:productIdentifier];
    SKProduct * product = [self getSKProductByIdentifier:productIdentifier];
    if (product != nil) {
        SKPayment *payment = [SKPayment paymentWithProduct:product];
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    }else {
        NSLog(@"Buying failed, no products found in apple store");
        NSString * json =[NSString stringWithFormat:@"{'success':false, 'status':'error', 'identifier':'', 'message':'No products found in apple store', 'transactionId':'', 'receipt':''}"];
        UnitySendMessage("Gamedonia", "ProductPurchased", [json UTF8String]);
    }
    
}

- (SKProduct *)getSKProductByIdentifier:(NSString *)productIdentifier {
    NSLog(@"getSKProductByIdentifier %d", [[self products] count]);
    
    for (SKProduct * product in self.products) {
        NSLog(@"product %@", [product productIdentifier]);
        if ([[product productIdentifier] isEqualToString:productIdentifier]) return product;
    }
    
    return nil;
}

@end
