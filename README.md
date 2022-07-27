
# Puppeteer

A simple way to animate your PNG vtuber avatar.

# Installation

Download the latest release, unzip, and run PngTuber.Pupper.exe. That's it.

LATEST RELEASE: https://github.com/vtuberkoe/PngTuberPuppeteer/releases/download/0.0.3/Puppeteer_0.0.3.zip

# Instruction Manual Tweet Thread

Setup Instructions: https://twitter.com/VtuberKoe/status/1424521764965687297?s=20

How to use with OBS: https://twitter.com/VtuberKoe/status/1447714739585486850?s=20

# Usage

First, you'll need an "avatar" in the correct format. An avatar is made up of the following:

- Root folder - the name of your avatar
  - Emotion folders - the different emotions your avatar can have as subfolders
  - avatarName.ava - an empty JSON file. Might be used in the future.

Included is an example avatar with two emotions - Happy and Surprise.

Each emotion needs 4 images:
- Eyes Open, Mouth Open (1_EO_MO.png)
- Eyes Open, Mouth Closed (2_EO_MC_png)
- Eyes Closed, Mouth Open (3_EC_MO.png)
- Eyes Closed, Mouth Closed (4_EC_MC.png)

**IMPORTANT!** Since 2021-10-11, Pupeeteer now supports GIF and APNG animations. You can use these types of files as well!

First, load your avatar by clicking the "Load Avatar" button.

Locate the avatar folder, and open the .ava file inside it.

Next, select your microphone from the list of microphones. 

The mouth on your avatar will animate based on the "Mouth Open Threshold." By default, it's set to about 33%. Once you start your avatar, adjust this up and down until it feels natural.

Finally, click "Start." 

## Using Puppeteer with OBS

Once you've started your avatar, grab the URL under the preview area. Copy it. It should be http://localhost:65000/viewer.

Create a new source in OBS - a Web Browser source. Paste in the viewer URL.

## Hotkeys

To use global hotkeys, click "Listen for Global Expression Hotkeys."

### Hotkeys:
#### Ctrl+Shift+F1 - First emotion in the list
#### Ctrl+Shift+F2 - Second emotion in the list
#### ...
#### Ctrl+Shift+F10 - Tenth emotion in the list
#### Ctrl+Shift+F11 - Previous Emotion in list
#### Ctrl+Shift+F12 - Next Emotion in list

# Have fun!
