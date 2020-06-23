
# VR-Trainingsapplikation-App für Fingeralphabet
<p align="center">
<img src="https://cdn.sidequestvr.com/file/29914/bannerSideQuest2.png" width="60%">
    
</p>
<h3 align="center">
IPA-Abschlussarbeit von Cédric Girardin

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

</h3>


## Overview
This is the final project (IPA) of our apprentice Cédric Girardin at the computer science department of the Berne University of Applied Science.\
The VR-Application runs on a Oculus-Quest and uses its Hand tracking feature. The Application can validate up to 23 Hand signs from the German Alphabet.

The Readme file explains shortly folowing topics:
* Setup
* Documentation
* Plugins
* Handform validation
* Handdata
* Domainmodel



## Setup
1. Clone the unity project
```sh
git clone https://...
```
2. Open Unity Hub->Add, Select the Project where its cloned
3. Select Unity Version (2019.3.06f) and open the project

```
project
└───Assets
│   └───Documents
│   └───Materials
│   └───Oculus
│   └───Prefabs
│   └───Resources
│   └───Scenes
│   └───Scripts
│   └───TestFiles
│   └───TextMesh Pro
│   └───XR
```

## Documentation
The Documentation is in German and uses the HERMES 5.1 Project method.

```
└───Assets
│   └───Documents
│       │   IPA_CédricGirardin.pdf
```


## Plugins
The Unity Project uses folowing plugins:

| Name | Link |
| ------ | ------ |
| ui_ux_in_vr_2019_2 | https://github.com/soma1294/ui_ux_in_vr_2019_2 |
| Oculus Integration | https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022 |

## Handform validation
To validate a Handform, it loads a saved hand into the scene, and compares it values with the current hand.\
The Validation compares folowing values:

| Handsubdivision | Value Type | BoneIds |
| ------ | ------ | ------ |
| Handorientation | Vector.up | Hand_WristRoot |
| Fingerangle | localEulerAngles Z | Intermediate BonesId’s and distal BoneId’s |
| Fingertipdistances | Diffrence of Vector.position | Hand_tipId’s |
The diffrences then gets weighted and it determents if its right or wrong.
This process can be  observed in realtime in the insight scene (insight the oculus quest).
![structimage](https://www.researchgate.net/profile/Paul_Fishwick/publication/228888064/figure/fig2/AS:647923578712064@1531488427021/Example-of-converting-an-array-of-a-struct-in-C-into-JavaScript.png)


## Handdata
In the insight scene is a save button.
When pressed it creates a Handformstruct:\
![structimage](https://www.researchgate.net/profile/Paul_Fishwick/publication/228888064/figure/fig2/AS:647923578712064@1531488427021/Example-of-converting-an-array-of-a-struct-in-C-into-JavaScript.png)

This Handformstruct will then be saved in the handdata.txt file.
Note: I wanted to save it in a json file but I had insufficent time.\
The Textfile is as followed formated:

| Char | Description |
| ------ | ------ |
| \ | Split the Attributs of the handstruct |
| {} | Segment the OVRBone Objects |
| : | Segment the OVRBone Attributs |
| () | Segment the Transform values |
| , | Segment the float values |
The Textfile gets then splited and converts the values back to a handformstruct...

## Domainmodel
![structimage](https://www.researchgate.net/profile/Paul_Fishwick/publication/228888064/figure/fig2/AS:647923578712064@1531488427021/Example-of-converting-an-array-of-a-struct-in-C-into-JavaScript.png)
