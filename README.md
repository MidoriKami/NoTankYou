# No Tank You
[![Download count](https://img.shields.io/endpoint?url=https://vz32sgcoal.execute-api.us-east-1.amazonaws.com/NoTankYou)](https://github.com/MidoriKami/NoTankYou)

Plugin for XivLauncher/Dalamud, shows a warning banner ingame you or your party members are forgetting to do role-specific actions.

Why would this be useful you ask?

Ever charged headfirst into a dungeon following you tank as a DPS, and all of a sudden realize a lot of aggro is changing? And then realize moments later that you are building up a lot of aggro? Then moments later... you are laying on the ground dead, thinking "oh, tank forgot their stance".

Teaching a friend that is just picking up Scholar? Now you can see if they accidentally dismiss their Faerie mid-combat.

Playing as a DPS, and want those **big numbers** but your Dancer doesn't realize they have Dance Partner, now you have a notice!

The primary focus of this plugin is helping to inform you that **others** may not be doing role critical things that they should be doing in all situations.

Alternatively, you can use this plugin to remind yourself to do these role specific tasks!

## How does it work?
When enabled, if a party member with a role-specific action hasn't done that role specific action, you will see a warning displayed on your screen.
Only you can see this warning, and no messages are sent to other party members. 

Warnings will not show if there are no players with a role-specific action in the party.
(Ex. If there are no summoners, you will not see the Summoner Pet Warning)

## How to use it
Using the ingame command `/nty` will bring up the main configuration window as seen below.
Configuration is split into three categories, Display, Modules, Settings.

You can use the command `/nty help` to view a list of all available commands.

![image](https://user-images.githubusercontent.com/9083275/177445912-c3b3bd08-91ce-4681-b1c5-177ee0ae0e7a.png)

# Display

The display tab is how you will enable and configure how NoTankYou will show you the warnings for your party members

Currently there are three display options available, **Banner**, **Party Frame**, and **Tippy**

## Banner Overlay

The Banner Overlay displays a warning banner with the warning info, player name of the player triggering the warning, and an icon indicating what the warning is regarding.

The hide in sanctuaries option will prevent the warning banner for showing while you are in any area the game considers to be a sanctuary.

This overlay can be configured to be anywhere on your screen with configurable scale, and display options. You can hide individual parts of the warning banner.

The Banner Overlay will by default show up to 8 warnings at once, one for each party member, you can configure it to display fewer warnings.

![image](https://user-images.githubusercontent.com/9083275/177445370-b9237ca6-25f7-4e6c-b301-e8ce161e0e07.png)

![image](https://user-images.githubusercontent.com/9083275/177447625-465c87ff-e41c-42c5-9e28-5c0de41c22c6.png)

## Party Frame Overlay

The Party Frame Overlay displays warnings next to the players name in the Party List.

The hide in sanctuaries option will prevent the warning banner for showing while you are in any area the game considers to be a sanctuary.

You can configure individual parts to show or hide, and you can enable or disable the flashing effect.

For accessability options for changing the color are provided.

![YoloMouse_JVgwSjNQUg](https://user-images.githubusercontent.com/9083275/177447974-c2bfc46d-ac92-4bfe-a1bf-42f413074d02.gif)

![image](https://user-images.githubusercontent.com/9083275/177448164-26836c51-5088-4980-9070-fe706fabf44d.png)

## Tippy Display

The Tippy Display requires the `Tippy` plugin to also be installed, if it is not installed you will see the following warning:

![image](https://user-images.githubusercontent.com/9083275/177448338-fc48120a-df3d-4cc1-9c97-6224fdf28da9.png)

With the Tippy Plugin installed NoTankYou will send warnings to be displayed via Tippy, the little semi-helpful rascal!

![image](https://user-images.githubusercontent.com/9083275/177448484-1d380e28-87c6-4ef2-b4f1-eef3bbbc91b3.png)

![image](https://user-images.githubusercontent.com/9083275/177448853-b6ed4eae-8a06-490c-b879-7d052befe775.png)

# Modules

The Modules tab lets you enable and disable warnings for specific classes or categories.

![image](https://user-images.githubusercontent.com/9083275/177449217-c5c04ea2-a8ad-4bb4-bf6c-65c304d7de08.png)

## Dancer, Sage, Scholar, Summoner

Dancer, Sage, Scholar, and Summoner only have basic configuration options.

Enable - Turn the warning on or off
Solo Mode - Evaluate warnings for the player only, or evaluate the entire party
Duties Only - Only trigger warnings for this module if you are in a duty
Priority - Warning Priority, only the highest priority warning will be displayed per player

Higher priority number is a higher priority, a warning with value 5 will be displayed before a warning with value 3

![image](https://user-images.githubusercontent.com/9083275/177450914-e75ab16c-c0d0-4e1a-af90-8688b25358a9.png)


## Blue Mage

Blue Mage has the same basic settings as Dancer, Sage, Scholar, and Summoner.

Blue Mage also has options to enable specific warnings for Aetherial Mimicry, Mighty Guard, and Basic Instinct.

Aetherial Mimicry can not be used with the Duties Only option, as this warning is only valid while not in a duty.

Basic Instinct will only display if you are alone and not in a party.

![image](https://user-images.githubusercontent.com/9083275/177450924-45fef447-8784-4a20-9a6a-7180056302f3.png)

## Tank

Tank has the same basic settings as Dancer, Sage, Scholar, and Summoner.

Tank also has options to disable warnings while in an Alliance raid, and an option to check other alliance members for tank stance.

![image](https://user-images.githubusercontent.com/9083275/177450943-32e6dd40-af50-4c6d-a669-1efd4ee82f30.png)

## Food

The food module will evaluate yourself or partymembers for the "Well Fed" status effect, this feature is intended to be used by statics or consistent groups in difficult content.

Options are provided to only show food warnings in specific types of duties, suppress warnings once combat has started, and a configurable early warning time to consider food "about to expire"

![image](https://user-images.githubusercontent.com/9083275/177450681-c24d2a7d-1e0f-437a-a378-6f5fba68abe4.png)

## Free Company

The Free Company module can be configured to warn you when one or more free company buffs are missing. This feature is intended specifically for people running free companies to help they use their buffs before doing coordinated play.

This module can be configured to warn when a specific number of any buffs are missing, or can be configured to warn when specific buffs are missing.

![image](https://user-images.githubusercontent.com/9083275/177450868-2bf03c8c-566d-480a-89f6-eb94f33f55b0.png)

# Blacklist

NoTankYou includes a zone blacklist that allows you to explicity state which zones you do not want **Any** warnings to appear. This is particularly useful for areas such as Bozja/Zadnor/Eureka

Note: NoTankYou is entirely disabled while you are in PvP automatically

![image](https://user-images.githubusercontent.com/9083275/177451100-fcc41b28-1f69-40fb-9e1d-0e8194afdb37.png)


