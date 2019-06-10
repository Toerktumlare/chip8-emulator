# Chip-8 Emulator

This repo contains a Chip-8 emulator written in java.

![Space Invaders](/images/invaders.gif)

Space Invaders

![Pong](/images/pong.png)

Pong

![TicTac](/images/tictac.png)

TicTac

CHIP-8 is an interpreted programming language, developed by [Joseph Weisbecker](https://en.wikipedia.org/wiki/Joseph_Weisbecker). It was initially used on the COSMAC VIP and Telmac 1800 8-bit microcomputers in the mid-1970s. CHIP-8 programs are run on a CHIP-8 virtual machine. It was made to allow video games to be more easily programmed for these computers.

### Memory
The Chip-8 language is capable of accessing up to 4KB (4,096 bytes) of RAM, from location 0x000 (0) to 0xFFF (4095). 

The first 512 bytes, from 0x000 to 0x1FF, are where the original interpreter was located, and should not be used by programs.

Most Chip-8 programs start at location 0x200 (512), but some begin at 0x600 (1536). Programs beginning at 0x600 are intended for the ETI 660 computer.

**Memory Map:**
```
+---------------+= 0xFFF (4095) End of Chip-8 RAM
|               |
|               |
|               |
|               |
|               |
| 0x200 to 0xFFF|
|     Chip-8    |
| Program / Data|
|     Space     |
|               |
|               |
|               |
+- - - - - - - -+= 0x600 (1536) Start of ETI 660 Chip-8 programs
|               |
|               |
|               |
+---------------+= 0x200 (512) Start of most Chip-8 programs
| 0x000 to 0x1FF|
| Reserved for  |
|  interpreter  |
+---------------+= 0x000 (0) Start of Chip-8 RAM
```

## Keyboard

The original Chip-8 had a different keyboard layout but this one uses the following (most standard today) implementation.

1 | 2 | 3| 4
----|----|----|----
Q | W | E | R
A | S | D | F
Z | X | C | V

## Emulator

CHIP-8 has 35 opcodes, which are all two bytes long and stored [big-endian](https://en.wikipedia.org/wiki/Endianness). The opcodes are listed below, in hexadecimal and with the following symbols:

NNN: address

NN: 8-bit constant

N: 4-bit constant

X and Y: 4-bit register identifier

PC : Program Counter

I : 16bit register (For memory address) (Similar to void pointer)

VN: One of the 16 available variables. N may be 0 to F (hexadecimal)

Opcode	| Type	| C Pseudo	 | Explanation
----|----|----|----|
0NNN |	Call|		| Calls RCA 1802 program at address NNN. Not necessary for most ROMs.
00E0|	Display	|disp_clear()|	Clears the screen.
00EE|	Flow|	return;	|Returns from a subroutine.
1NNN|	Flow|	goto NNN;|	Jumps to address NNN.
2NNN|	Flow|	*(0xNNN)()|	Calls subroutine at NNN.
3XNN|	Cond|	if(Vx==NN)|	Skips the next instruction if VX equals NN. (Usually the next instruction is a jump to skip a code block)
4XNN|	Cond|	if(Vx!=NN)|	Skips the next instruction if VX doesn't equal NN. (Usually the next instruction is a jump to skip a code block)
5XY0|	Cond|	if(Vx==Vy)|	Skips the next instruction if VX equals VY. (Usually the next instruction is a jump to skip a code block)
6XNN|	Const|	Vx = NN|	Sets VX to NN.
7XNN|	Const|	Vx += NN|	Adds NN to VX. (Carry flag is not changed)
8XY0|	Assign|	Vx=Vy|	Sets VX to the value of VY.
8XY1|	BitOp|	Vx=Vx&#124;Vy |	Sets VX to VX or VY. (Bitwise OR operation)
8XY2|	BitOp|	Vx=Vx&Vy|	Sets VX to VX and VY. (Bitwise AND operation)
8XY3|	BitOp|	Vx=Vx^Vy|	Sets VX to VX xor VY.
8XY4|	Math|	Vx += Vy|	Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
8XY5|	Math|	Vx -= Vy|	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
8XY6|	BitOp|	Vx>>=1|	Stores the least significant bit of VX in VF and then shifts VX to the right by 1.[2]
8XY7|	Math|	Vx=Vy-Vx|	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
8XYE|	BitOp|	Vx<<=1|	Stores the most significant bit of VX in VF and then shifts VX to the left by 1.[3]
9XY0|	Cond|	if(Vx!=Vy)|	Skips the next instruction if VX doesn't equal VY. (Usually the next instruction is a jump to skip a code block)
ANNN|	MEM|	I = NNN|	Sets I to the address NNN.
BNNN|	Flow|	PC=V0+NNN|	Jumps to the address NNN plus V0.
CXNN|	Rand|	Vx=rand()&NN|	Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.
DXYN|	Disp|	draw(Vx,Vy,N)|	Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
EX9E|	KeyOp|	if(key()==Vx)|	Skips the next instruction if the key stored in VX is pressed. (Usually the next instruction is a jump to skip a code block)
EXA1|	KeyOp|	if(key()!=Vx)|	Skips the next instruction if the key stored in VX isn't pressed. (Usually the next instruction is a jump to skip a code block)
FX07|	Timer|	Vx = get_delay()|	Sets VX to the value of the delay timer.
FX0A|	KeyOp|	Vx = get_key()|	A key press is awaited, and then stored in VX. (Blocking Operation. All instruction halted until next key event)
FX15|	Timer|	delay_timer(Vx)|	Sets the delay timer to VX.
FX18|	Sound|	sound_timer(Vx)|	Sets the sound timer to VX.
FX1E|	MEM|	I +=Vx|	Adds VX to I.
FX29|	MEM|	I=sprite_addr[Vx]|	Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
FX33|	BCD|	set_BCD(Vx); (I+0)=BCD(3); (I+1)=BCD(2); (I+2)=BCD(1);| Stores the binary-coded decimal representation of VX, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
FX55|	MEM|	reg_dump(Vx,&I)|	Stores V0 to VX (including VX) in memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
FX65|	MEM|	reg_load(Vx,&I)|	Fills V0 to VX (including VX) with values from memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.

## Graphics
The original implementation of the Chip-8 language used a 64x32-pixel monochrome display with this format:
```
+-------------------+
|(0,0)        (0,64)|
|                   |
|                   |
|                   |
|(64,0)      (64,64)|
+-------------------+
```

It also stores a set of predefined sprites representing the hexadecimal numbers 0-F. Below is an example of how it stores numbers in memory.

```$xslt
Number: 0                      Number: 1

"0"  | Binary   | Hex         "0"  | Binary   | Hex
-----|----------|-----        -----|----------|-----
**** | 11110000 | 0xF0          *  | 00100000 | 0xF0
*  * | 11110000 | 0x90         **  | 01100000 | 0x90
*  * | 11110000 | 0x90          *  | 00100000 | 0x90
*  * | 11110000 | 0x90          *  | 00100000 | 0x90
**** | 11110000 | 0xF0         *** | 01110000 | 0xF0
```


## Compiling and running
requires maven3 and java 11

compile:
```$xslt
$ mvn clean install
```

run:
```$xslt
$ java -jar target/chip8-emulator-1.0-SNAPSHOT <romfile>

example:
$ java -jar target/chip8-emulator-1.0-SNAPSHOT INVADERS.ch8
```

## ROM files

There are several romfiles that get bundled into the jar during the build. You can find them in `src/main/resources/roms`

## References

* https://en.wikipedia.org/wiki/CHIP-8
* http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
* http://www.multigesture.net/articles/how-to-write-an-emulator-chip-8-interpreter/
