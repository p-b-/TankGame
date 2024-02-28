**Tank Game**

Beginnings of a wireframe 3d tank game, written in C#.

This is not a serious project.  It does all drawing via Windows paint events.  There is no z-buffer.

The transformations are not done via a matrix, but individual translations/rotations from model, to world, to view, and finally via a simple projection formula.

This was similar to how games used to be written in the past, apart from the reliance on Windows to draw the triangles.  In decades past, you'd have the render the triangles directly to memory using something like a span buffer if filling the triangles in.

This is not how to write 3d games.  

**Controls**

Click the window to capture the mouse, press Escape to release capture.

Left/Right arrow-keys: rotate left and right
Up/Down arrow-keys: move forward and backward
A/D: turret left and right
S/X: turret down and up
Space: fire

P: Enter frame replay mode for debugging
O: Exit frame replay mode
 U: Redraw previous frame
 I: Redraw next frame

**Outstanding tasks**

* Collision detection to work correctly
* Enemy tank intelligence
* Spaceships
* Gun emplacements
* Scoring
* Levels
* Multi-player
 

**Issues**
* Framerate Slowdown when lots of entities on screen
* Glitching - possibly caused by drawing frames based on data that is changing in a different thread.
* Current collision detection slow, and tank can get stuck
