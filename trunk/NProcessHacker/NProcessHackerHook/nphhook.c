#include <windows.h>
#include "../kphhook.h"

BOOL WINAPI DllMain(
    HINSTANCE hinstDLL,
    DWORD fdwReason,
    LPVOID lpvReserved
    )
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        KphHookInit();
        break;
    case DLL_PROCESS_DETACH:
        KphHookDeinit();
        break;
    default:
        break;
    }

    return TRUE;
}
