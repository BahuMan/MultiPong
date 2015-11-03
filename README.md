# MultiPong
pong multiplayer based on [game-of-everything protocol](https://github.com/CoderDojo/cp-zen-platform.git)

##Status

For now, the PongController object creates a polygon based on the number of players.
The walls of this polygon are have colliders.
A "ball" (I used a cube in order to have interesting collisions) is launched and bounces off the walls.

### Todo

* create player paddels
* create goals (player loses if the ball touches "their" goal)
* option to host or join game
* sync positions over GoE protocol
