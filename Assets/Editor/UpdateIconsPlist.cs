using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using System.IO;

public class UpdateIconsPlist
{
    private static List<string> appIcons = new List<string>()
    {
        "Aero", "Bedroom", "Grayscale", "Iceking", "Mauve", "Moss",
        "Omnitrix", "Popsicle", "Ramen", "Sunset", "CottonCandy",
        "Watermelon", "Midnight", "Autumn", "River", "Diner", "Sweater",
        "Jungle", "Moon", "Stone"
    };

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // Add icons
            PlistElementDict iconsDict = rootDict.CreateDict("CFBundleIcons");
            PlistElementDict altIconsDict = iconsDict.CreateDict("CFBundleAlternateIcons");

            PlistElementDict ipadIconsDict = rootDict.CreateDict("CFBundleIcons~ipad");
            PlistElementDict ipadAltIconsDict = ipadIconsDict.CreateDict("CFBundleAlternateIcons");

            for (int i = 0; i < appIcons.Count; i++)
            {
                PlistElementDict iconDict = altIconsDict.CreateDict(appIcons[i]);
                PlistElementArray iconName = iconDict.CreateArray("CFBundleIconFiles");
                iconName.AddString(appIcons[i] + "-20");
                iconName.AddString(appIcons[i] + "-29");
                iconName.AddString(appIcons[i] + "-40");
                iconName.AddString(appIcons[i] + "-60");
                iconName.AddString(appIcons[i] + "-76");
                iconName.AddString(appIcons[i] + "-83.5");
                iconName.AddString(appIcons[i] + "-1024");

                PlistElementDict ipadIconDict = ipadAltIconsDict.CreateDict(appIcons[i]);
                PlistElementArray ipadIconName = ipadIconDict.CreateArray("CFBundleIconFiles");
                ipadIconName.AddString(appIcons[i] + "-20");
                ipadIconName.AddString(appIcons[i] + "-29");
                ipadIconName.AddString(appIcons[i] + "-40");
                ipadIconName.AddString(appIcons[i] + "-60");
                ipadIconName.AddString(appIcons[i] + "-76");
                ipadIconName.AddString(appIcons[i] + "-83.5");
                ipadIconName.AddString(appIcons[i] + "-1024");
            }

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
