#import "PlayAdsSDKWrapper.h"
#import "PlayAdsSDK.h"

extern void UnityPause(bool pause);

@interface PlayAdsSDKBridge : NSObject <PlayAdsSDKDelegate>

@property (nonatomic, retain) NSString  *instanceName;

+ (void)startWithApp:(NSString*)appicationID secretToken:(NSString*)applicationSecret;
+ (void)show:(NSString*)typeString;
+ (void)cache:(NSString*)typeString;
+ (void)version;

@end

static PlayAdsSDKBridge *_sharedInstance;

@implementation PlayAdsSDKBridge

+ (PlayAdsSDKBridge*)sharedInstance
{
    if(_sharedInstance == nil)
    {
        _sharedInstance = [[PlayAdsSDKBridge alloc] init];
    }
    return _sharedInstance;
}

+ (void)startWithApp:(NSString*)applicationID secretToken:(NSString*)applicationSecret
{
    [PlayAdsSDK startWithAppID:applicationID
                   secretToken:applicationSecret
                      delegate:[PlayAdsSDKBridge sharedInstance]];
}

+ (void)cache:(NSString*)typeString
{
    [PlayAdsSDK cacheType:[PlayAdsSDKBridge getType:typeString]];
}

+ (void)show:(NSString*)typeString loadingScreen:(bool)showLoadingScreen
{
    [PlayAdsSDK showLoadingScreen:showLoadingScreen];
    [PlayAdsSDK showType:[PlayAdsSDKBridge getType:typeString]];
}

+ (PlayAdsAdType)getType:(NSString*)typeString
{
    PlayAdsAdType result = nil;
    
    if([typeString isEqualToString:@"Scratch"])
    {
        result = PlayAdsAdTypeScratch;
    }
    else if([typeString isEqualToString:@"Interstitial"])
    {
        result = PlayAdsAdTypeInterstitial;
    }
    else if([typeString isEqualToString:@"Video"])
    {
        result = PlayAdsAdTypeVideo;
    }
    else if([typeString isEqualToString:@"SlotMachine"])
    {
        result = PlayAdsAdTypeSlotMachine;
    }
    else if([typeString isEqualToString:@"Memory"])
    {
        result = PlayAdsAdTypeMemory;
    }
    else if([typeString isEqualToString:@"CoverFlow"])
    {
        result = PlayAdsAdTypeCoverFlow;
    }
    else if([typeString isEqualToString:@"GameList"])
    {
        result = PlayAdsAdTypeGamesList;
    }
    else
    {
        result = PlayAdsAdTypeSmart;
    }
    
    return result;
}

+ (void)version
{
    [[PlayAdsSDKBridge sharedInstance] version];
}

- (void)dealloc
{
    [_instanceName release];
    [super dealloc];
}

- (void)playAdsStartDidEnd
{
    UnitySendMessage([self.instanceName UTF8String], "SDKStartedCallback", "");
}

- (void)playAdsStartDidFailWithError:(NSError *)error
{
    UnitySendMessage([self.instanceName UTF8String],
                     "SDKStartFailedCallback",
                     [[NSString stringWithFormat:@"%@", [error localizedDescription]] UTF8String]);
}

- (void)playAdsAdReady
{
    UnitySendMessage([self.instanceName UTF8String], "AdReadyCallback", "");
}

- (void)playAdsAdDidShow
{
    UnitySendMessage([self.instanceName UTF8String], "AdShownCallback", "");
    UnityPause(true);
}

- (void)playAdsAdDidFailWithError:(NSError *)error
{
    UnityPause(false);
    UnitySendMessage([self.instanceName UTF8String],
                     "AdFailedCallback",
                     [[NSString stringWithFormat:@"%@", [error localizedDescription]] UTF8String]);
}

- (void)playAdsAdDidClose
{
    UnityPause(false);
    UnitySendMessage([self.instanceName UTF8String], "AdClosedCallback", "");
}

- (void)version
{
    UnitySendMessage([self.instanceName UTF8String],
                     "SDKVersionCallback",
                     [[NSString stringWithFormat:@"%@", [PlayAdsSDK version]] UTF8String]);
}

@end


#ifdef __cplusplus

extern "C"
{
    void PlayAdsSDKStart(const char* appID, const char* secret, const char* instanceName)
    {
        NSString *applicationID = [NSString stringWithCString:appID encoding:nil];
        NSString *applicationSecret = [NSString stringWithCString:secret encoding:nil];
        NSString *unityInstanceName = [NSString stringWithCString:instanceName encoding:nil];
        
        [PlayAdsSDKBridge sharedInstance].instanceName = unityInstanceName;
        [PlayAdsSDKBridge startWithApp:applicationID
                           secretToken:applicationSecret];
    }

    void PlayAdsSDKCache(const char* typeString)
    {
        NSString *typeNSString = [NSString stringWithCString:typeString encoding:nil];
        [PlayAdsSDKBridge cache:typeNSString];
    }

    void PlayAdsSDKShow(const char* typeString, const bool showLoadingScreen)
    {
        NSString *typeNSString = [NSString stringWithCString:typeString encoding:nil];
        [PlayAdsSDKBridge show:typeNSString loadingScreen:showLoadingScreen];
    }

    void PlayAdsSDKGetVersion()
    {
        [PlayAdsSDKBridge version];
    }
}

#endif