//
//  ChangeAppIcon.mm
//  https://yodahe.com
//
//  Created by Yodahe Alemu on 28/10/2020.
//  Copyright © 2020 Yodahe Alemu. All rights reserved.
//
#import <Foundation/Foundation.h>
#import <AudioToolbox/AudioToolbox.h>
#import <UIKit/UIKit.h>

#import "ChangeAppIcon.h"

@interface ChangeAppIcon ()

@end

@implementation ChangeAppIcon



//////////////////////////////////////////

#pragma mark - ChangeIcon


+ (void) setAlternateIconName:(NSString*) iconName
{
    //anti apple private method call analyse
    if ([[UIApplication sharedApplication] respondsToSelector:@selector(supportsAlternateIcons)] &&
        [[UIApplication sharedApplication] supportsAlternateIcons])
    {
        NSLog(@"Changing the app icon silently maybe");
        NSLog(iconName);
        NSMutableString *selectorString = [[NSMutableString alloc] initWithCapacity:40];
        [selectorString appendString:@"_setAlternate"];
        [selectorString appendString:@"IconName:"];
        [selectorString appendString:@"completionHandler:"];

        SEL selector = NSSelectorFromString(selectorString);
        IMP imp = [[UIApplication sharedApplication] methodForSelector:selector];
        void (*func)(id, SEL, id, id) = (void (*)(__strong id, SEL, __strong id, __strong id))imp;
        if (func)
        {
            func([UIApplication sharedApplication], selector, iconName, ^(NSError * _Nullable error) {});
        }
    }
}

@end
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#pragma mark - "C"

extern "C" {
    
    //////////////////////////////////////////
    // ChangeIcon
    
    // This takes a char* you get from Unity and converts it to an NSString* to use in your objective c code. You can mix c++ and objective c all in the same file.
    static NSString* CreateNSString(const char* string)
    {
        if (string != NULL)
            return [NSString stringWithUTF8String:string];
        else
            return [NSString stringWithUTF8String:""];
    }

    void    _ChangeIcon (const char* icon) {
        [ChangeAppIcon setAlternateIconName: CreateNSString(icon)];
    }

}

