CSC3020H-Tank-Control
=====================
Author: Benjamin Meier
Date:   May 2013

-----------------------------

Controls:
=========

Tank:
W = Forward
A = Steer Left
D = Steer Right
S = Reverse

Up = Raise Gun
Down = Lower Gun
Left = Turn turret Left
Right = Turn turret Right

Camera:
1 = First Person Camera
2 = Third Person Camera
3 = Orbitting Camera

I = Increase Height
K = Decrease Height
O = Decrease Distance
L = Increase Distance

Implementation Notes:
=====================

There are 3 different Random Object models: Cube, Cylinder, and Cone. These along with 4 different textures give
randomness to the Random Objects in the scene.

The collision detection is performed via the Collidables classes and potential objects are pulled from a Quad Tree
implementation.

There are 3 different directional lights in the scene:
- Blue light towards (1,-1,1)
- White light towards (-1, -0.1, 0)
- Green light manipulated by Tank's Y orientation in the scene

The Fog is a black fog that shows the darkness well and allows objects to move from the shadows, into the light area.

Special Extras
==============

Tank Wheels rotate differently depending on direction the tank is traveling and which way the tank is steering (outside 
wheels vs inside wheels).

The camera fluidly transforms between camera modes using a smoothstep tweaning approach. This makes the camera transition
seem fluid and continuous without too many blatant jumps.



