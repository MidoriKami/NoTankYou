# No Tank You
Plugin for XivLauncher/Dalamud, shows a warning banner ingame if none of the tanks in your party have their tank stance on.

Why would this be useful you ask?

Ever charged headfirst into a dungeon following you tank as a DPS, and all of a sudden realize a lot of aggro is changing?
And then realize moments later that you are building up a lot of aggro?
Then moments later... you are laying on the ground dead, thinking "oh, tank forgot their stance".

Well fret no more!

This plugin does have some overlap with tools that warn you if YOUR tank stance is off, however those tools don't warn you if OTHER's are off.

### How does it work?
When enabled, if a tank is detected in your party, a warning banner will show if none of the tanks have their tank stance on.

When in a 8-person raid with two tanks, only one of the tanks need to have their stance on.

There is user-configurable delay on instance change to allow a "grace period" before showing the warning.

There is also a configuration option to disable checking while in an alliance raid.

![TankStance](https://user-images.githubusercontent.com/9083275/142379197-9cba7a62-4fe4-46bb-b346-9cdead65f06e.png)

### Why is this useful?
* Avoiding confrontation and death
* Helping a friend when they forget their tank stance
* Realizing that a sprout keeps accidentally toggling stance repeatedly
* Personal reminder to put your own stance on 

# Commands
For ease of use, bundled with the main comman **/notankyou** there are several commands you can use

| Syntax      | Description |
| ----------- | ----------- |
| /notankyou off       | prevents the plugin from showing the warning banner                                                      |
| /notankyou on        | enables the showing of the warning banner                                                                |
| /notankyou force     | forces the warning banner to appear                                                                      |
| /notankyou status    | prints the current settins for the plugin to the debug channel of chat                                   |
| /notankyou blacklist | adds the current instance id to the blacklist, preventing the plugin from running in those instance id's |
| /notankyou whitelist | removes the current instance id from the blacklist                                                       |
| /notankyou debug     | prints the number of tanks, and the stauts of each tank to the chat                                      |

## Notes
The 'force' command will force the banner to show, it will not allow you to move the banner unless the "Show/Move Warning Banner" settings option is selected in the settings menu.

The 'status' and 'debug' commands print to the debug channel in chat, only you will be able to see these messages. By default all log windows include displaying debug messages.

The 'blacklist' and 'whitelist' commands will only add/remove the ids for the instance you are in. These are stored numerically as there isn't an easy way to reference the proper instance name. 
One useful location for these commands are in Bozja/Zadnor, as it generally doesn't matter if none of the tanks in your party have their stance on, generally someone, likely not in your party, will be the main tank.


