# CocaBot
#### A simple discord bot which uses the SpookVooper API to show info about user

## Installation Proccess

__Windows:__

Follow the first steps of installing Microsoft Visual Studios and Dsharp Nuget packages [here](https://youtu.be/7-tyLCAO4mY) but install version 4.0.0rc1 as version above it might break the bot

Install the SpookVooper.Api Nuget package as well

After that click on Build and Build Solution. After this you will get a exe in bin\Debug\netcoreapp3.1 that will make your bot come to life as long as you set up config.json (same location) with the discord token key!

__Linux:__

Install nuget and dotnet

Make sure config.json is where Bot.cs is and not in bin\Debug\netcoreapp5.0

Add your discord token key to config.json

Then do the following commands (in the main folder for the bot source code):

nuget source add -Name Dsharp -Source https://nuget.emzi0767.com/api/v3/index.json
nuget restore
dotnet run

And that should make the bot run!
If you need help or have a suggestion join [SpookVooper Discord Server](https://discord.gg/spookvooper) and DM me!