#ifdef __cplusplus
extern "C" {
#endif
    
    bool CanMakePayments();
    void RequestProducts(char ** productIdentifiers, int size);
    void BuyProduct(char * productIdentifier);
#ifdef __cplusplus
}
#endif