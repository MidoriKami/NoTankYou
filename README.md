# No Tank You
Plugin for XivLauncher/Dalamud, shows a warning banner ingame **you or your party members** are forgetting to do role-specific actions.
Tank Stance, Dance Partner, Scholar Faerie, Summoner Pet, Sage Kardion.

Why would this be useful you ask?

Ever charged headfirst into a dungeon following you tank as a DPS, and all of a sudden realize a lot of aggro is changing?
And then realize moments later that you are building up a lot of aggro?
Then moments later... you are laying on the ground dead, thinking "oh, tank forgot their stance".

Teaching a friend that is just picking up Scholar? Now you can see if they accidentally dismiss their Faerie mid-combat.

Playing as a DPS, and want those **big numbers** but your Dancer doesn't realize they have Dance Partner, now you have a notice!

The primary focus of this plugin is helping to inform you that **others** may not be doing role critical things that they should be doing in all situations.

## How does it work?
When enabled, if a party member with a role-specific action hasn't done that role specific action, you will see a warning displayed on your screen.
Only you can see this warning, and no messages are sent to other party members. 

Warnings will not show if there are no players with a role-specific action in the party.
(Ex. If there are no summoners, you will not see the Summoner Pet Warning.)

![image](https://user-images.githubusercontent.com/9083275/147808967-3b83bd41-a1ac-4947-a161-14ab469eb32e.png)

## Modules

### Tank Stance
This module checks there is at least 1 tank in the party. (Blue Mage with *Aetherical Mimicry: Tank* is not considered a tank)

If there is at least 1 tank in the party and none of those tanks have a tank stance on, the Tank Stance warning will display.

If a tank is dead, they will not be considered to be a tank for the duration of their death.

![image](https://user-images.githubusercontent.com/9083275/147809510-9894555e-d801-4ae7-b97c-8ca165a2005e.png)

### Dance Partner
This module checks for dancers that are over lvl 60 for the status effect `closed position`

![image](https://user-images.githubusercontent.com/9083275/147809651-6d50350d-bb57-4103-b4bd-106d45008fde.png)

### Scholar Faerie
This module checks for a Scholars Faerie pet. Due to how the game code works, there is a pre-programed `half second` delay between faerie checks.

This module does account for when your Faerie becomes Seraph, and when it is consumed by Dissipation.

A warning is only shown if the Scholar never summoned their Faerie in the first place.

![image](https://user-images.githubusercontent.com/9083275/147809735-c9175e62-1c97-4473-9015-82a6be118032.png)

### Sage Kardion
This module checks for the `Kardia` effect on the Sage.

Kardion is the name of the effect that is put on the other player.

![image](https://user-images.githubusercontent.com/9083275/147809761-18ce83c0-b864-48e6-9dce-1749e3b4196f.png)

### Summoner Pet
This module checks for a Summoners Pet. Due to how the game code works, there is a pre-programed `half second` delay between pet checks.

This module does account for when your pet transitions between the various pet stages.

A warning is only shown if no pet has been detected for more than a `half second`.

![image](https://user-images.githubusercontent.com/9083275/147809864-fe55d64c-3537-4f7e-b3ac-d858043c5314.png)

# Commands
You can invoke commands using `/notankyou` and `/nty`

### Command Format 
#### /notankyou [primary command] [secondary command]

### Module Commands

These commands enable or disable the associated module. 
This is a quick way to disable a module that is potentially causing a problem or to quickly toggle a module if you just want to check if a warning is showing or not.

Primary commands
* all
* everything
* kardion
* sage
* sge
* kardia
* tank
* tankstance
* dancepartner
* dancer
* dp
* partner
* dnc
* faerie
* fairy
* scholar
* sch

Secondary commands
* on
* off
* toggle
* t
* tog

### Mode Commands

There are a total of three modes, `Party Mode` `Solo Mode - Duties Only` and `Solo Mode - Everywhere`

When in Party Mode, you must be in a party of at least 2 people. *Trusts do not count as a party*

When in Solo Mode, only your statuses are checked to display a warning, this is effectively a way to remind yourself to use your role-critical actions.

Primary commands
* mode

Secondary commands
* party
* solo
* trust
* t
* toggle

### Blacklist/Whitelist Commands

Blacklist commands add a mapID to the blacklist, whitelist commands remove a mapID from the black list.

`/blacklist add ###` and `/whitelist remove ###` do the same thing.

`/blacklist remove ###` and `/whitelist add ###` do the same thing.

This is useful to prevent this plugin from running in specific areas. PVP and Golden Saucer areas are automatically disabled internally.

Primary Commands
* blacklist
* bl
* whitelist
* wl

Secondary Commands
* here
* add
* remove
