BIY: BOOM IT YOURSELF — Medieval Stone Timer Pedestal / Unity 6

Main asset: BIY_MedievalStone_TimerPedestal.fbx
A true binary Autodesk FBX 7.4 file (not a renamed OBJ), metres, +Y up, front faces -Z.
Short stylized octagonal ivory stone, four chunky gray carved feet, low bronze
candle seat with four clasp tabs. TWO EMPTY front plates for gameplay visuals.
No candle, no blue or red connector, no baked numbers / type symbol.

QUICK IMPORT
1. Drag the FBX file into Assets in Unity 6; put into the scene.
2. If URP defaults to a different shader, extract/remap FBX materials via the
   Model Importer > Materials or use URP/Lit. The source FBX stores material
   color definitions directly, with no texture paths.
3. Add your candle prefab at child anchor ANCHOR_Candle_Position.
4. Put a world-space TextMeshPro icon or sprite at ANCHOR_Type_Badge,
   and a second world-space TextMeshPro countdown at ANCHOR_Countdown_Text.
   The panels themselves have no decal/text baked in.
5. Rotate TextMeshPro to face -Z; offset forward (toward -Z) 0.001-0.005m
   to prevent z-fighting. Badge and display slope with the stone surface;
   tip text by ~-16 degrees around X if necessary for perfect alignment.

MESHS / MATERIALS
Stone_CarvedBody_WithFeet: low-poly ivory stones and four four-sided feet
Bronze_Socket_And_MetalTrims: aged bronze, minimal trims
CandleSocket_Interior: dark shallow interior (not a through-hole)
NodeType_Badge_Empty: dark matte badge background
Countdown_Screen_Empty: nearly-black countdown background
3 empty transforms: candle, type, countdown anchor.

Included reference OBJ/MTL fallback is for checking/import if necessary.
