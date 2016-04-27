# MultiPong
pong multiplayer based on [game-of-everything protocol](https://github.com/CoderDojo/cp-zen-platform.git)

##Prerequisite

This game connects to a helper application called "Game of Everything" (see https://github.com/Squarific/Game-Of-Everything).
It uses websockets and the JSON format for all messages, and it will quietly ignore any messages it doesn't understand.

##Status

When the hosting player presses "fire", the game starts for all players currently joined.
For now, a polygon is created with the number of edges twice the number of players.
A "ball" (I used a cube in order to have interesting collisions) is launched and bounces off the walls.
The position of all the edges as well as min/max position for the player paddle are communicated to all players.

## Todo

* only our "own" paddle should listen to keyinput, all others should receive updates via GoE protocol
* each "owner" of the paddle should broadcast positions over GoE protocol
* server should restart game when new player joins
* win/lose conditions
