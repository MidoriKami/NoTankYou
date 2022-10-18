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

![image](https://user-images.githubusercontent.com/9083275/194430187-f60ae9d5-04d0-43dd-be18-7ab2c30dacbf.png)

## Banner Overlay

The Banner Overlay displays a warning banner with the warning info, player name of the player triggering the warning, and an icon indicating what the warning is regarding.

This overlay can be configured to be anywhere on your screen with configurable scale, and display options. You can hide individual parts of the warning banner.

The Banner Overlay will by default show up to 8 warnings at once, one for each party member, you can configure it to display fewer warnings.

Configuration Window             |  Warning Appearance
:-------------------------:|:-------------------------:
![image](https://user-images.githubusercontent.com/9083275/194428607-7b3db649-aec7-4acd-adf1-af1d2d26d03a.png) | ![image](https://user-images.githubusercontent.com/9083275/194427783-45b4d2f2-3778-4dfd-8f90-5e8b5cf53b03.png)

## Party Frame Overlay

The Party Frame Overlay displays warnings next to the players name in the Party List.

The hide in sanctuaries option will prevent the warning banner for showing while you are in any area the game considers to be a sanctuary.

You can configure individual parts to show or hide, and you can enable or disable the flashing effect.

For accessability options for changing the color are provided.

Configuration Window             |  Warning Appearance
:-------------------------:|:-------------------------:
![image](https://user-images.githubusercontent.com/9083275/194430654-166fd1c5-8029-4da2-82ae-da9febddd832.png) |  ![YoloMouse_JVgwSjNQUg](https://user-images.githubusercontent.com/9083275/177447974-c2bfc46d-ac92-4bfe-a1bf-42f413074d02.gif)

# Blacklist

NoTankYou includes a zone blacklist that allows you to explicity state which zones you do not want **Any** warnings to appear. This is particularly useful for areas such as Bozja/Zadnor/Eureka

Note: NoTankYou is entirely disabled while you are in PvP automatically

![image](https://user-images.githubusercontent.com/9083275/194430817-9ec4aaad-01fa-46d1-80fa-00a3b7073b5c.png)


