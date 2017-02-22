## Dogey  
[![Discord](https://discordapp.com/api/guilds/158057120493862912/widget.png)](https://discord.gg/0sjlWZiGRvRNZAqx)  

Dogey is a bot built using Discord.Net 1.0 and aims to provide an example implementation of the 5 common choices for database providers. For help you can join [Dogey's guild](https://discord.gg/0sjlWZiGRvRNZAqx) or the discord.net channel in the [Discord API guild](https://discord.gg/BeDSNf2).

#### Provider Example Status

| Provider   |   Status  |
|------------|:-----------:|
| SQLite     |   [Working](https://github.com/Aux/Dogey/issues/1)   |
| MySQL      | [In Progress](https://github.com/Aux/Dogey/issues/2) |
| Redis      |   [Waiting](https://github.com/Aux/Dogey/issues/3)   |
| MongoDB    |   [Waiting](https://github.com/Aux/Dogey/issues/4)   |
| PostgreSQL |   [Waiting](https://github.com/Aux/Dogey/issues/5)   |

#### Structure
##### Projects
- Dogey
Contains initialization logic, modules, and services that don't require access to a database provider to function.
- Dogey.Core
Contains the utility classes and entity contracts used to maintain a structure across multiple database providers.
- Dogey.SQLite/MySQL/Redis
Contains several examples of writing modules and services that integrate with the specified database providers.

##### Entities
Entities for each project are stored in a folder named `Entities`. Each database provider will use the same basic structure for entities, as specified by the core interfaces, but with minor differences based on what is required or more suited for each specific provider. 

##### Modules
Modules for each project are stored in a folder named `Modules`, and are split into classes based on their executing structure. The base command `tag` and all of its sub-commands are in a class named `TagModule`, the base command `youtube` and all of its sub-commands are in a class named `YoutubeModule`, etc...


#### When to use a specific provider

##### SQLite
Information

##### MySQL
Information

##### Redis
Information

##### MongoDB
Information

##### PostgreSQL
Information