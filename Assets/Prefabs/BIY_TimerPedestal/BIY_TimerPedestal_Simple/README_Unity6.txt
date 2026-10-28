BIY: Boom It Yourself | Timer Pedestal (Simplified)

This is a GENUINE FBX 7.4 BINARY file, not a renamed OBJ or GLB.

Import: drag BIY_TimerPedestal_Simple.fbx into Assets in Unity 6.
It has imported diffuse-color materials; for URP, you may wish to extract/remap the materials to URP/Lit if your project uses URP.

Design: oversized, broad faceted ivory ceramic body with a narrow cool-slate base, simple pewter-edged blank badge and countdown faces, plus a recessed top socket. No red/blue connectors, no candle, no numerals, no small bolts.

Meshes: 01_LowPoly_Ceramic_Body, 02_NodeType_Frame, 03_NodeType_Blank_Surface, 04_Countdown_Frame, 05_Countdown_Blank_Surface.

ANCHOR_NodeType_Badge: near the face for a dynamic icon / sprite.
ANCHOR_Countdown_Text: near the front surface for 3D TextMeshPro.
ANCHOR_Candle: near top socket, center axis.

+Y up, front towards -Z. Approx 0.8m wide and 0.27m tall (resize entire model uniformly to suit scene). The front plate surfaces are at z=-0.386 and z=-0.400 respectively; make the TMP text face out of the pedestal (toward -Z). Rotate the TMP object by 180 degrees around Y if its initial forward direction is opposite.

No textures to manage. The low-poly surfaces use simple FBX-embedded material colors. For maximum Unity drawcall efficiency, assign the same project-wide URP/Lit material for repeated trims or merge static parts if you prefer.

FALLBACK: An OBJ + MTL version of the identical mesh is provided if the FBX importer rejects the primary model. OBJ has the separate named mesh groups and basic colors, but no Unity anchor empties.
