BIY - Rusting WIZARD TIMER PEDESTAL (pedestal ONLY)

Includes: assembled OBJ, three modular OBJ parts, MTL, and 9 procedural diffuse PNG textures.
No candle and no colored input/output connector are included.

UNITY 6 IMPORT
1) Drag the full BIY_TimerPedestal folder into Assets/ in your Unity project.
2) Select TimerPedestal.obj in Project. In Inspector > Materials, use Import via Material Description / Standard (depending on Unity version).
3) If Unity loses textures, extract/create materials using the .mtl names and assign the same-named *.png as Base Map in URP/Lit. Textures are unlit color art, not metallic maps.
4) Add MeshCollider or BoxCollider for picking, as needed.
   For separately controllable pieces, DO NOT use the assembled OBJ. Instead put TimerPedestal_Body.obj, TimerPedestal_CountdownPlate.obj, and TimerPedestal_TypeBadge.obj under one empty GameObject at local (0,0,0), rotation 0, scale 1. These modular OBJs align exactly.
5) The OBJ uses Y up, faces toward -Z, base at Y=0. Approx size 1.85m width x 0.83m tall. Resize the imported prefab uniformly to fit your existing BIY node scale.

TIMER UI / RUNTIME INTEGRATION
- The modular TimerPedestal_TypeBadge.obj has a 3D hourglass and badge geometry; replace it with a different type badge as needed.
- 24_TimerType_Badge_Base / 25_TimerType_Badge_Slate / 26_TimerType_Icon_Hourglass are independent meshes. Disable / replace the hourglass if using different node types.
- 22_Countdown_Black_Glass is a blank glossy/dark display for a dynamic countdown. Do NOT print a static digit into the texture. Place TextMeshPro (3D text) as a child of the node, and update it from your existing countdown logic.
- Suggested world-local TMPro center: (0, 0.194, -0.971) looking outward toward -Z, with ~13 degree upward tilt (rotation about X ~ -13 degrees depending text orientation). Check facing in Scene view.
- Candle mounting center is (0, 0.70, 0). Top ring inner opening diameter ~0.64 units; resize candle separately.
- Colored input/output port connectors are deliberately absent. Attach your existing connector meshes to the sides at +/-X.

Materials
- Wood_Walnut, Wood_Light, Wood_Top = warm, rough wooden body
- Brass_Worn, Brass_Bright = muted aged hardware
- Slate_Badge = replaceable inset type-symbol plate
- Dark_Iron = edge straps / nails / backings
- Countdown_Glass = plain dark timer screen
- Dark_Shadow = intentional seams / recesses

INFO
Designed procedurally specifically for BIY. Low-poly angular, rustic aesthetic rather than a polished gothic prop. Mesh pieces are intentionally named and separated for easier runtime visibility swaps. Object count and triangles can be inspected in any 3D software. Keep the OBJ, MTL and PNG textures together in the same directory.
