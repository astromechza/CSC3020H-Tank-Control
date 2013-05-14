CSC3020H-Tank-Control
=====================
Author: Benjamin Meier
Date:   May 2013

-----------------------------

Controls:
=========

Keyboard / Mouse: 
-----------------
W = Forward
A = Steer Left
D = Steer Right
S = Reverse

Up = Raise Gun
Down = Lower Gun
Left = Turn turret Left
Right = Turn turret Right

Mouse = Turn turret, raise gun, lower gun

1 = First Person Camera
2 = Third Person Camera
3 = Orbitting Camera

I = Increase Height
K = Decrease Height
O = Decrease Distance
L = Increase Distance
U = Increase Looseness
J = Decrease Looseness
(Looseness is the rate at which the camera keeps up with its target position)

~ = Toggle draw collision boxes

Escape = Exit

Controller:
-----------
Left Analog = Tank movement
Right Analog = Turret / Gun movement

Y = Change Camera

Right Trigger = Increase Height
Right Shoulder = Decrease Height
Left Trigger = Increase Distance
Left Shoulder = Decrease Distance
Dpad Up = Increase Looseness
Dpad Down = Decrease Looseness

B = Toggle draw collision boxes

Back = Exit

Implementation Notes:
=====================

There are 3 different Random Object models: Cube, Cylinder, and Cone. These along with 4 different textures give
randomness to the Random Objects in the scene.

The collision detection is performed via the Collidables classes and potential objects are pulled from a Quad Tree
implementation.

There are 3 different directional lights in the scene:
- Blue
- Red
- White

The Fog is a black fog that shows the darkness well and allows objects to move from the shadows, into the light area.

The collision detection checks 12 points on the edge of the Tank's Object Aligned Bounding Box ( Press ~ or (B) to view).
Each point is checked using intersection of Circle and intersection of Rectangle methods.


Special Extras
==============

Tank Wheels rotate differently depending on direction the tank is traveling and which way the tank is steering (outside 
wheels vs inside wheels).

The camera fluidly transforms between camera modes using a smoothstep tweaning approach. This makes the camera transition
seem fluid and continuous without too many blatant jumps.





