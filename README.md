# Neon Puzzle Cubes
## A probabilistic algorithmic composition.
[Main Max patcher in the Neon Puzzle Cubes composition.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxMain.png)

Neon Puzzle Cubes is an algorithmic composition that is solely controlled with the eyes. This project utilizes a [Tobii Eyetracker 4C](https://gaming.tobii.com/product/tobii-eye-tracker-4c/) for the eye tracking, Unity for the interface, and Max for the control of the composition and production of the audio.

![Graphic that demonstrates the work-flow of the application.](https://github.com/boomninjavanish/media/neonPuzzleCubes/workflow.png)

## Interface

The interface was designed to be easy to use by anyone who has relatively good eye-sight. The goal was to explore an interface that allows for improvisation. This led to the creation of a means of control which operates instrumental motives using probability.

### Eye Tracking and Unity

Since the Tobii eye tracker is intended for gaming, Tobii provides an excellent [SDK](https://developer.tobii.com/pc-gaming/unity-sdk/) for use with with the Unity gaming engine. This allows one to harness the X and Y location of the user's gaze on the screen. Also, the SDK allows a developer to make objects "gaze aware": once the user is looking at the object, the SDK returns a true boolean value that makes it possible to trigger actions based on this event.

![User interface of the composition.](https://github.com/boomninjavanish/media/neonPuzzleCubes/userInterface.png)

The user interface in Unity consists of four objects laid atop a movie that plays in an infinite loop. Three of the objects are cubes and one object is a sphere flattened into a disk. Each cube is assigned an instrument in Max. Similar to rolling a die, rotating a cube will change what the corresponding instrument plays based on probability. The cubes are rotated by looking at one of four invisible disks that are directly in front of the object and setup to be gaze-aware objects. 

![Interface as seen in the Unity engine IDE.](https://github.com/boomninjavanish/media/neonPuzzleCubes/userInterfaceUnity.png)

When the user gazes at the top or left, the corresponding cube will rotate on that axis and stop when it reaches ninety degrees. Since the Unity script updates every frame refresh, the cube may pass ninety degrees several times until the script sees exactly ninety degrees. This was accidentally discovered as a way to reduce the amount of control the user has over the cube and gives it the appearance of a dice roll.

The disk on the bottom of the interface is a gaze-aware object that allows the user to change scenes during the performance. The scene number is received from Max and is overlaid as text over the scene disk. The scene will change after the user gazes at the disk for more than three seconds. During this time, the disk will fade in color from bright yellow to a dark red to indicate to the user that a change is happening. Once the three seconds have elapsed, a scene change request will be sent to Max via OSC.

## Open Sound Control (OSC)

[OSC](http://opensoundcontrol.org/introduction-osc) is used as a means of communication between the Unity interface and the composition controller in Max. Two UPD port are opened on the local interface (127.0.0.1) for sending and receiving commands and responses.  

![The sub-patch in Max that receives the OSC data and requests from Unity.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxOscReceiver.png)

![The sub-patch in Max that sends OSC commands to Unity.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxOscSender.png)

On each frame update, Unity sends the raw X, Y, Z, and W transform rotation values of each cube to Max. Also, scene requests are sent from Unity to be handled by the composition controller in Max. Once a scene has been changed by the composition controller, Max sends the current scene number back to Unity so that it may be displayed to the user. In addition, the number is used to change other elements of the interface in Unity. 

## Max

### Scene Controller

![The scene controller Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxSceneController.png)

The scene controller contains the clock, which operates as the heartbeat of the entire composition. This clock is also used to track each beat in a 4/4 measure. When a scene change request arrives, the scene controller will wait until the first beat of a measure to switch to the new scene, then report which scene is being changed. Also, the beat number of the 4/4 measure is output for use by other modules.

### Composition Controller

![The composition controller Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxCompController.png)

The composition controller switches the scenes and ensures that only one scene is played at a time. Instructions from each scene are relayed to the various other instrument and playback modules. Each scene is stored as a sub-patch within this module.

![Sample scene Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxScene.png)

Each scene contains melodies, algorithmic playback, instructions, or controls for other modules. Thus, each scene can be thought of as a set of measures in a composition written on sheet music, with each set of outputs being the individual staves that denote an instrument on the sheet music.

### Voter Units 

![Voter Unit Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxVoterUnit.png)

A voter unit is used to transform any cube rotational value into a number between 1-6 so that a motive can be chosen to be performed by the juke joint module. This simulates a role of a die and allows the user to improvise music based on pure chance.

Voter units take the output from each cube (X, Y, Z, and W) and transform the data from a small range to a range of 1 to 1,000. The increased resolution allows for a larger possibility that a different number will be chosen between "rolls". The numbers are then averaged to obtain the median of each of the cube outputs. To eliminate the problem of low probability of obtaining numbers on the low end and the high end of the normal distribution of numbers, the range is again transformed to flatten the bell curve, so that the numbers are more evenly distributed (see [standard deviation](https://en.wikipedia.org/wiki/Standard_deviation) for more information). Therefore, the vote unit is more likely to allow for all possibilities during a performance.

### Juke Joint

![Juke Joint Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxJukeJoint.png)

The juke joint patch contains all of the instruments that will be played by each cube. A set of six interchangeable motives for each instrument were created in Ableton Live. These motives were then output as .wav files and placed into a playlist for each instrument in the juke joint.

When a vote unit decides which clip is to be played, that number is then given to the juke joint and held in a cache. Once it has been determined that the choice is stable, then the new clip in the playlist will start on beat one of the measure as given by the scene controller's clock. 

![Motion Detector Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxMotionDetector.png)

Since the cube may be moving constantly and may cause the playback to shift rapidly, stability is determined by a motion detector sub-patch. This patch will only allow the number to populate in the cache if it detects that the cube has been still for at least two beats.

Each clip is repeated by having the clock play the chosen clip at beat one of the measure. By controlling a gate for the signal from the clock, the juke joint allows for each instrument to be turned off. Since clip playback is not interrupted and will continue until the end of the last beat, transitions within the composition are seamless.

### Drum Machine

![Drum Machine Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxDrumMachine.png)

The drum machine contains six drum loops in a playlist that were created in Ableton Live. The drum loops can be triggered randomly or specifically by the composition controller. Since the drums are played back by Max in a loop, the drums will play until a signal to stop is sent from the composition controller. Also, the drum loops can be pitch-shifted to create a different sounding beat than the original.

### Leia Organa

![Leia Organa Max patch.](https://github.com/boomninjavanish/media/neonPuzzleCubes/maxLeiaOrgana.png)

Leia Organa is a multi-timbral synthesizer which was created using various wave form objects. The composition controller can adjust the reverb mix, as well as a low-pass filter cut-off, in order to change the presence and timbre of the instrument. In addition, the instrument contains another control that plays a note two octaves below the fundamental frequency. This may be used similarly to a pedal effect on a real organ. The loudness of the pedal can be controlled via the pedal amplitude multiplier.

## Composition Structure

The composition starts with a black background and no cube shown on the screen; only the scene control is visible, with an 'X' which indicates nothing is being played. The performer starts Scene Zero by looking at the scene control disk. Meanwhile, nothing appears on the screen while the Leia Organa instrument plays a melody of whole notes. 

Eventually, a drum loop is started in the Drum Machine module with the pitch shifted to the lowest setting. The loops are randomized every four measures. This gives the piece a ghostly or lonely feel.

When Scene One is chosen, the drum loops slowly pitch shift to normal while continuing to randomize every four measures. The background video fades in and the center cube appears. The composition controller opens the first instrument in juke joint so that it may begin to accept numbers from the corresponding voter unit. The main drum loop continues to be played at normal speed until Scene Four.

Scenes two and three each reveal a new cube which introduces a new instrument into the mix. During one of these scenes a final static instrument plays a repeating motif. Finally, scene four removes the drum loop, closes all of the gates for the cube instruments, removes the cubes from view. The background video fades to black while the organ comes back in to play a final note. The static motive plays two more times and then silence.