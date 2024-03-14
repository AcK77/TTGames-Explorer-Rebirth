<div align="center">
<h1>
  <br>
  <img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Logo.png" width="150">
  <br>
  <b>TTGames Explorer</b> <i>Rebirth</i>
  <br>
</h1>
TTGames Explorer Rebirth is an utility that will allow you to extract different data from TTGames's Lego games.<br>
This tool was originally written 10 years ago, abandoned, then rewrited from scratch.
</div>

<h3>Supported files</h3>

- `.dat` archives to extract choosen files from them (or extract everything).
- `.fpk` `.pac` `.pak` archives to extract choosen files from them (or extract everything).
- `.ats` `.cfg` `.csv` `.git` `.ini` `.lua` `.scp` `.sf` `.sub` `.subopt` `.txt` `.xml` text files to view colored text and copy/paste text elsewhere.
- `.cmo` `.dds` `.tex` texture files to view and extract them as PNG file.
- `.nxg_textures` archive texture files  to view and extract them as DDS or PNG file.
- `.ft2` font files to extract the DDS file and look at mapped region.
- `.tsh` textures set files to extract the DDS file and look at mapped region.
- `.adp` `.cbx` `.mp3` `.ogg` `.wav` sound files to play them and convert some of them to WAV format.
- `.pc_shaders` shaders collection files to extract each compiled shader file and look at the disassembled data.
- `.ghg` `.gsc` 3d model files to view the model using OpenGL.

<h3>Supported Encryption/Compression/Hashes</h3>

- `LZ2K` which is used to compress files in archive files.
- `Deflate_v1.0` which is used to compress files in archive files (`.an4` files aren't supported).
- `FNV1a` which is used to generate some IDs used by games.
- `PAK Checksum` which is used to calculate a checksum over the whole `.pak` files.
- `RC4` which is used to encrypt some files.

<h3>Credits</h3>

- Trevor Natiuk for unlz2k code which helped me along of my RE (https://github.com/pianistrevor/unlz2k - No license).
- Christopher Whitley for RC4 C# implementation (https://github.com/manbeardgames/RC4 - MIT License).
- Luigi Auriemma for QuickBMS which I used as reference here and there (https://aluigi.altervista.org/quickbms.htm).
- Connor Harrison for CBXDecoder which helped me to decode CBX files (https://github.com/connorh315/CBXDecoder - No license).
- spacehamster for DXDecompiler (https://github.com/spacehamster/DXDecompiler - MIT License).
- PavelTorgashov for FastColoredTextBox (https://github.com/PavelTorgashov/FastColoredTextBox - LGPLv3).
- Jay Franco for some help on file formats.
- Sluicer for their amazing research on 3d models 10 years ago.
- Everyone which shared informations, codes, help about TTGames LEGO files.

<h3>Screenshots</h3>
<div align="center">
(outdated)<br>
 <a href="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen01.png" target="_blank"><img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen01.png" width="200"></a>
 <a href="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen02.png" target="_blank"><img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen02.png" width="200"></a>
 <a href="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen03.png" target="_blank"><img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen03.png" width="200"></a>
 <br>
 <a href="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen04.png" target="_blank"><img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen04.png" width="200"></a>
 <a href="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen05.png" target="_blank"><img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen05.png" width="200"></a>
 <a href="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen06.png" target="_blank"><img src="https://raw.githubusercontent.com/AcK77/TTGames-Explorer-Rebirth/master/distribution/misc/Screen06.png" width="200"></a>
</div>
