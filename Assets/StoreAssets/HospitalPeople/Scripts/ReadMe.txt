About the shader for the Low Poly Hospital People.

The shader takes care of hiding certain parts of the mesh based on it's vertex color. These parameters are controllved via the shuffler script.
An example is the head area when there is hair is hidden, this can be easilty avoided if the hair covers the head completely, however, this effect
will be used later on for hats so that parts of the hair mesh can be hidden by the script talking to the shader. LPEP_Shuffler talks to the parameters
of LPEDP_BodyShader such as the isBald or hasHat boolean/switch toggles. The shuffler also takes care of color changes in the hair and skin tones. Sub objects
have been tagged so that the script can easilty talk to the components of the characters. Be usre to use the prefabs only to avoid issues.

If you wish to use this script for your own characters for shuffling their features, be sure to comment out or remove these requests to the shader, or, 
if you want a similar system, take a look at the vertex colors of the meshes by exporting them to blender and making a vertex color material to see how they
are used.


RGB 0,255,170 - Head scalp Vertex color (used for hiding that part of the mesh when isBald is checked).
RGB 0,255,255 - Face Area (you may not need to hide the face but this is the color used here)
RGB 255,64,64 - The upper hair Vertex color, this will be hidden via the shader in a future update when Hats are used (my not be used in the hospital people as they normally have no hats)
RGB 255,0,128 - Hair lower area (this is the area the hair will be seen when wearing a hat).

Hats may be included in a pack related to this one and this shader may be updated to reflect that so can work across multiple packs as they are created.