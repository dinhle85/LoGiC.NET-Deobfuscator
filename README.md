# LoGiC.NET-Deobfuscator
## Deobfuscator for LoGiC.NET Obfuscator (https://github.com/AnErrupTion/LoGiC.NET).

### Usage:
- Drag & Drop Protected Application onto Deobfuscator.

### Supported Protections:
```
Int Encoding:
- Removal of Negative Instructions
- Math (Abs & Min)

String Encryption
Control Flow
Anti De4dot
Proxy Calls (Simplifier? Doesn't fix completely but simplfies them kek. Migth not even work, I don't really know.)
Anti Tamper
Watermark
Renamer
Junk
```

### Note:
- Deobfuscated application will most likely have some unreadable methods because of errors (Probably because this deobfuscator doesn't support proxy calls, but I haven't bothered checking why.) and or might not be able to run.
- I will not fix this because LoGiC.NET is unstable and only meant for learning purposes.

## Before:
![bilde](https://user-images.githubusercontent.com/60292167/120541785-9e44ba80-c3ea-11eb-9236-ffcaa0947024.png)

## After:
![bilde](https://user-images.githubusercontent.com/60292167/120541836-af8dc700-c3ea-11eb-8c78-dca727175111.png)

## Credits:
0xd4d - <a href="https://github.com/0xd4d/dnlib">dnlib</a> (The Library I chose to use for making this deobfuscator.) <br>
MindSystemm - <a href="https://github.com/MindSystemm/SuperCalculator">Super-Calculator</a> (De4dot Blocks Cleaning Method.) <br>
AnErruption - <a href="https://github.com/AnErrupTion/LoGiC.NET">LoGiC.NET</a> (For Making LoGiC.NET Obfuscator.) <br>
DevT02 - <a href="https://github.com/DevT02/Junk-Remover">Junk-Remover</a> (Remove Useless Nops.) <br>
