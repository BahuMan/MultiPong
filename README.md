# MultiPong
pong multiplayer based on [game-of-everything protocol](https://github.com/Squarific/Game-Of-Everything)

##Prerequisite

This game connects to a helper application called "Game of Everything" (see https://github.com/Squarific/Game-Of-Everything).
It uses websockets and the JSON format for all messages, and it will quietly ignore any messages it doesn't understand.

##Status

You can choose your nickname, choose host or join, and then connect. (No check is done for duplicate nicknames)
When the hosting player presses "fire", the game starts for all players currently joined.
The position of all the edges as well as min/max position for the player paddle are communicated to all players.
A "ball" (I used a cube in order to have interesting collisions) is launched and bounces off the walls.

## Todo

* test that only the local paddle listens to keyboard input
* test that paddle positions are correctly broadcast, and interpreted by other players
* server should restart game when new player joins
* win/lose conditions
