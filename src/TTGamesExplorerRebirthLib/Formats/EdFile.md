##STREAMINFO DOCUMENTATION
#####Author: Jay Franco

###About

The StreamInfo file format is one of the most common file formats in the nu2 engine. The reason why is likely how portable it is. It barely makes any assumptions. 

The way I interpret it's use is that the Level EDitor has panes with properties for Gameobjects and such, and the StreamInfo file format was developed to serialize the gameobject and it's properties. This way the nu2 API and the Level Editor can conversate with the same language. 

The StreamInfo file format has pretty much stayed the same for over 2 decades, with the only major change being the switch to the 4CC Resource Header in version 22. 

###File extensions:
.LED (Level EDitor), 
.CD (Character Definition), 
.CPJ (Character ProJect), .CPD
.PRJ (PRoJect)
.APJ (AnimProJect)
.ANIM (ANIMation)
.AS (Animation Set)
.CLIP
.JOB
.SCENE_BAKED
.PREFAB_BAKED
.POD_BAKED
.ANGEL_RESOURCE
.BLOB (Binary Large OBjects)


###File Hierarchy:
######  * == optional

ResourceHeader*

TypeList

ClassList
