## Dogey
Dogey is a bot for use with Discord. He is currently not available to be added to personal servers, but you can join http://voice.auxesis.tv to test some commands.


#### Commands
This is a list of commands available to any server Dogey has permission to speak in.
{} - Required parameter
[] - Optional parameter
... - Indicates repeating parameters are allowed

##### Dogey Commands
Commands listed here serve no purpose other than to doge.

*~doge {veryparameter}...*
Generate a doge with the given text.
*~wag [user]*
Who's a good boy?


##### Custom Commands
Custom commands are per-server. Any command added on 'Doges Anonymous' will not be available or cause issues with commands on 'Tactical Pickle' or any other server.

*~commands*
Display a list of all available custom commands for the current server.

*~create {command} {message}
Create a new custom command, and set it's initial return text.

*~{command} [index]*
Returns a random text from the list of available messages, optionally specify a message.

*~{command}.add {message}*
Add a new return text to a custom command. To add new messages to a command called *lonce*, simply type *~lonce.add Is lonce even a word?*

*~{command}.del {index}*
Delete an existing message from a custom command. The number listed in each message is its index.

*~{command}.count*
Find how many messages a custom command has stored.


##### Search Commands
A bunch of commands pretty much every bot has, by now these are required for most bots to be useful right?

*~gif {search}*
Return the first gif from imgur that matches the provided text.

*~imgur {search}*
Return the first image from imgur tagged with the provided words.

*~vimeo {search}*
Return the first video from vimeo that matches the provided text.

*~youtube {search}*
Return the first video from youtube that matches the provided text.

*~vine {search}*
Return the first video from vine that matches the provided text.


##### Twitch Commands
Some commands for Twitch information, will be expanded later.

*~twitch online {channel}*
Lets you know if this user is currently streaming.


##### Chat Commands
A bunch of commands to view information about chat and messages.

*~messages [user]*
Get the total number of messages this user has posted in this channel.


####Libraries
**Discord.Net**
**Newtonsoft.Json**
