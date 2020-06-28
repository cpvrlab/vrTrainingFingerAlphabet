
# VR-Trainingsapplikation-App für Fingeralphabet
<p align="center">
<a href="http://www.youtube.com/watch?feature=player_embedded&v=Ahs1l0INA-w" target="_blank"><img src="http://img.youtube.com/vi/Ahs1l0INA-w/0.jpg" 
alt="VR-Trainingsapplikation-App für Fingeralphabet Thumbnail" width="480" height="360" border="10" /></a>
    
</p>
<h3 align="center">
IPA-Abschlussarbeit von Cédric Girardin

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

</h3>


## Overview
This is the final project (IPA) of our apprentice Cédric Girardin at the computer science department of the Berne University of Applied Science.\
The VR-Application runs on a Oculus-Quest and uses its Hand tracking feature. The Application can validate up to 23 Hand signs from the German Alphabet.

This Project was made in 2 weeks/ 10 Work days: 30.03.2020-08.05.2020 (Yes, the documentation was also made within does 2 weeks).

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
git clone https://github.com/cpvrlab/vrTrainingFingerAlphabet.git
```
2. Open Unity Hub->Add, Select the Project where its cloned
3. Select Unity Version (2019.3.06f) and open the project

## Documentation

The [Documentation](https://github.com/cpvrlab/vrTrainingFingerAlphabet/blob/master/Assets/Documents/IPA_C%C3%A9dricGirardin.pdf) is in German and uses the HERMES 5.1 Project method.

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
| JSON .NET For Unity | https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347#description |


## Handform validation
To validate a Handform, it loads a saved hand into the scene, and compares it values with the current hand.\
The Validation compares folowing values:

| Handsubdivision | Value Type | BoneIds |
| ------ | ------ | ------ |
| Handorientation | Vector.up | Hand_WristRoot |
| Fingerangle | localEulerAngles Z | Intermediate BonesId’s and distal BoneId’s |
| Fingertipdistances | Diffrence of Vector.position | Hand_tipId’s |

The diffrences then gets weighted and it determents if its right or wrong.
This process can be  observed in realtime in the insight scene (insight the oculus quest).\

<img src="https://github.com/cpvrlab/vrTrainingFingerAlphabet/blob/master/Assets/Documents/DocumentImages/Screenshots/OculusScreenshots/ValidationWithoutWeights.jpg" width="40%">


## Handdata
In the insight scene is a save button.
When pressed it creates a Handformstruct:\
![structimage](https://github.com/cpvrlab/vrTrainingFingerAlphabet/blob/master/Assets/Documents/DocumentImages/HandformStruct.png)

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
![structimage](https://github.com/cpvrlab/vrTrainingFingerAlphabet/blob/master/Assets/Documents/DocumentImages/KlassendiagrammRealisierung2.png)

