#include "GamedoniaStoreKit.h"
#include "InAppIAPHelper.h"


bool CanMakePayments()
{
    return [[InAppIAPHelper sharedHelper] canMakePayments];
}

void RequestProducts(char ** productIdentifiers, int size)
{
    
    NSMutableSet * identifiers = [NSMutableSet set];
    for (int i=0;i<size;i++) {
        NSString * productIdentifier = [NSString stringWithUTF8String: productIdentifiers[i]];
        //NSLog(productIdentifier);
        [identifiers addObject:(productIdentifier)];
    }
    
    [[InAppIAPHelper sharedHelper] requestProducts: identifiers];
     
}

void BuyProduct(char * productIdentifier)
{

    NSString * str = [NSString stringWithUTF8String: productIdentifier];
    NSLog(@"%@", str);
    [[SKPaymentQueue defaultQueue] addTransactionObserver:[InAppIAPHelper sharedHelper]];
    [[InAppIAPHelper sharedHelper] buyProductIdentifier: str];

}