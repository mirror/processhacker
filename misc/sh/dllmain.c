#include "apilog.h"

BOOL APIENTRY DllMain(
    HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
    )
{
	switch (ul_reason_for_call)
	{
	    case DLL_PROCESS_ATTACH:
            AlInit();
            return TRUE;
	    case DLL_PROCESS_DETACH:
            AlDeinit();
		    return FALSE;
	}

	return TRUE;
}
