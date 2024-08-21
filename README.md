# CoatOfArmsCoreNew
Coat Of Arms Generator (written in .NET 8, C# WinForms)

I contend we should build flexible "purpose-built" programs. This program is the second in a series of my purpose-built programs that illustrate "expandable programs". Expandable programs are those whose functionalty grows as new data is introduced. In this particular case, the coat-of-arms generator's functionalty grows as more data (in this case images - grows). So as you add an shield-shape or an "ordinary" or a "charge", the number of possible coat-of-arms generated expands.

Designed to be a flexible creator of coat-of-arms. The program randomly picks a shield shape, how to divide the shield (if any) (called divisions or ordinaries), and symbol (if any) (called charges). Also, it randomly chooses a color from a palete of true heraldric colors for each piece being careful about not overlaying black on black for example. To add new symbol designs or ordinaries or shield shapes, just add a new file in the subdirectories provided. Just ensure it follows the rules about the picture (for example on symbols, paint any changeable portions black). To simplify the program rather than drawing the geometric ordinaries, we just add the ordinary image in the file and perform color changes.

Pseudo code lines for the Coat Of Arms generation are:

    randomly pick ordinary image from directory
    switch ordinary black to random color
    if not solid, switch ordinary white to random color
    resize Ordinary because many shield shapes do not conform to uniform white space border
    randomly pick shape from directory
    frame the shield shape over the resized ordinary chopping off extra
    overlay randomly picked symbol
    switch symbol black to random color

ImageActionToolbox.cs holds the assumed directory structures. Currently the assumed directory structure is C:\CoatOfArmsCore\ holding the EXE and subdirectories underneath for the ChargesFiles, OrdinaryFiles, and ShieldShapeFiles.

See the other repository CoatOfArmsCore to see the generated EXE.
