## Dogey  
[![Discord](https://discordapp.com/api/guilds/158057120493862912/widget.png)](https://discord.gg/0sjlWZiGRvRNZAqx)  [![Build Status](https://travis-ci.org/Aux/Dogey.svg?branch=master)](https://travis-ci.org/Aux/Dogey)  

Dogey is a bot for [Discord](https://discordapp.com) that aims to provide example implementations of 5 of the common choices for database providers in applications. For help you can join [Dogey's guild](https://discord.gg/0sjlWZiGRvRNZAqx) or the discord.net channel in the [Discord API guild](https://discord.gg/BeDSNf2).

#### Provider Example Status

| Provider   |   Status  |
|------------|:-----------:|
| SQLite     |   [Working](https://github.com/Aux/Dogey/issues/1)   |
| MySQL      | [In Progress](https://github.com/Aux/Dogey/issues/2) |
| Redis      |   [Waiting](https://github.com/Aux/Dogey/issues/3)   |
| MongoDB    |   [Waiting](https://github.com/Aux/Dogey/issues/4)   |
| PostgreSQL |   [Waiting](https://github.com/Aux/Dogey/issues/5)   |

#### Requirements
- [Visual Studio 2017](https://www.microsoft.com/net/core#windowsvs2017)

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


#### Provider Information

##### SQLite
###### Description
SQLite works with functional and direct calls to an 'SQLite Database' file rather than communicating with a server over ports or sockets. These files tend to be generated and stored locally alongside the application, which makes it a great option for quick and efficient storage of data.
###### Pros
- Extremely easy to set up
- Minimal response times

###### Cons
- Allows only one concurrent write operation at a time

##### MySQL
###### Description
MySQL is a popular choice for large-scale databases. Despite not trying to implement the full SQL standard, MySQL offers a lot of functionality to the users. As a stand-alone database server, applications talk to MySQL daemon process to access the database itself, unlike SQLite.
###### Pros
- Relatively easy installation
- Supported by many hosting providers
- Many management tools created by 3rd-parties
- High security

###### Cons
- Stagnant development of new versions
- Not completely sql compliant

##### PostgreSQL
Information

##### Redis
Information

##### MongoDB
Information
