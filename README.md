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
For ease of use, bundled with the main command **/notankyou** you can also use the shorthand **/nty** there are several commands you can use

Command format is the following:

/[nty,notankyou] [modulename*] [on,off,toggle]

Module Name is extremely flexible:
* "kardion" "sage" "sge" "kardia"
* "tank" "tankstance"
* "dancepartner" "dancer" "dp" "dnc"
* "faerie" "fairy" "scholar" "sch"

You can omit the [on,off,toggle] to toggle automatically.

# Examples
* /nty dnc
* /notankyou dancer on
* /nty all on
* /nty all
* /notankyou tank off
