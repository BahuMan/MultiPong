# MultiPong
pong multiplayer based on [game-of-everything protocol](https://github.com/CoderDojo/cp-zen-platform.git)

##Status

For now, the PongController object creates a polygon based on the number of players.
The walls of this polygon are have colliders.
A "ball" (I used a cube in order to have interesting collisions) is launched and bounces off the walls.

### Todo

* based on #players joined, create playing field dynamically and assign player positions
* set rotation of paddle correctly
* only our "own" paddle should listen to keyinput, all others should receive updates via GoE protocol
* sync positions over GoE protocol
