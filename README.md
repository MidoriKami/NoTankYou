# No Tank You
[![Download count](https://img.shields.io/endpoint?url=https://qzysathwfhebdai6xgauhz4q7m0mzmrf.lambda-url.us-east-1.on.aws/NoTankYou)](https://github.com/MidoriKami/NoTankYou)

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

This interface is searchable, you can search by partial name or module type. 
There are three types of modules currently available.

NoTankYou's Configuration UI is built with KamiToolKit which allows it to be fully rendered by the game itself, instead of as an overlay.

This configuation window seamlessly integrates with the game and its window systems.

<img width="696" height="588" alt="ffxiv_dx11_y1DWWZl2Vm" src="https://github.com/user-attachments/assets/545ec258-b71b-4bcd-88fb-08fcc6af75b2" />

# Warning Displays

NoTankYou has two overlays you can use to display the warnings, you can go for the subtle approach by integrating with the PartyList user interface, or you can go with the classic in-your-face warning banner in the middle of your screen.

Party List Overlay Appearance            |  Warning Banner Appearance
:-------------------------:|:-------------------------:
<img width="431" height="100" alt="ffxiv_dx11_EG3vn4qvc6" src="https://github.com/user-attachments/assets/79d7448f-3a1a-40a2-8d66-06960bb17caf" />  | <img width="531" height="126" alt="ffxiv_dx11_aXiIRRAENl" src="https://github.com/user-attachments/assets/fcc987c8-95f9-4029-a0b1-1a2ddb26c6f2" />

## Party List

The party list feature by default animates to help grab your attention without being too obnoxious, this can be disabled in the settings.

https://github.com/user-attachments/assets/1c2a9399-5e74-4934-8598-43dfd8469b15

This display mode also includes a handy mouse over tooltip to explain what is causing the warning to show.

This will include a Action tooltip showing the details of the required action.

<img width="487" height="279" alt="ffxiv_dx11_nofR6HTClA" src="https://github.com/user-attachments/assets/3b3894dc-8e8a-4209-bba3-9c94fb34e185" />

## Warning Banner

The warning banner is a overlay element that displays a list of warnings, ordered by warning priority.

This list can be moved and resized allowing NoTankYou to show more, or less warnings at once depending on your preferences.

<img width="558" height="552" alt="ffxiv_dx11_g1aOaOyOri" src="https://github.com/user-attachments/assets/13ee004a-0ed9-49f3-a828-2edd61349301" />

This warning will also pulse to grab your attention, this can be disabled in the settings.

https://github.com/user-attachments/assets/f0909687-cef0-4ab2-88cb-ef70a03ece5e

Mousing over the action icon will display an Action tooltip showing the details of the required action.

<img width="886" height="302" alt="ffxiv_dx11_VQkrzvB1PK" src="https://github.com/user-attachments/assets/f5657835-7c3a-4a4b-9d80-7f92991733b5" />
