#include "stdafx.h"
#include <metahost.h>
#include <string>
#include <iostream>
#include <fstream>

#pragma comment(lib, "mscoree.lib")

#import <mscorlib.tlb> auto_rename raw_interfaces_only high_property_prefixes("_get","_put","_putref") rename("ReportEvent", "InteropServices_ReportEvent")

using namespace mscorlib;
using namespace std;

extern "C" __declspec(dllexport) HRESULT ImplantDotNetAssembly(_In_ LPCTSTR lpCurrentPath)
{
    HRESULT hr;
    ICLRMetaHost *pMetaHost = NULL;
    ICLRRuntimeInfo *pRuntimeInfo = NULL;
    ICLRRuntimeHost *pClrRuntimeHost = NULL;

    hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
    if (hr != S_OK) {
        MessageBox(NULL, L"CLRCreateInstance failed!", L"Error", NULL);
    }

    hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
    if (hr != S_OK) {
        MessageBox(NULL, L"GetRuntime failed!", L"Error", NULL);
    }

    BOOL fLoadable;
    hr = pRuntimeInfo->IsLoadable(&fLoadable);
    if (hr != S_OK) {
        MessageBox(NULL, L"IsLoadable failed!", L"Error", NULL);
    }

    if (!fLoadable) {
        MessageBox(NULL, L"Runtime isn't loadable!", L"Error", NULL);
    }

    hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&pClrRuntimeHost));
    if (hr != S_OK) {
        MessageBox(NULL, L"GetInterface failed!", L"Error", NULL);
    }

    hr = pClrRuntimeHost->Start();
    if (hr != S_OK) {
        MessageBox(NULL, L"Start failed!", L"Error", NULL);
    }

    string path = (char*)lpCurrentPath;
    wstring dll_path(path.begin(), path.end());

    DWORD pReturnValue;
    hr = pClrRuntimeHost->ExecuteInDefaultAppDomain(
        dll_path.c_str(),
        L"TTGamesExplorerRebirthHook.HookEngine",
        L"Initialize",
        L"",
        &pReturnValue);

    if (hr != S_OK) {
        char buff[1000];
        snprintf(buff, sizeof(buff), "ExecuteInDefaultAppDomain failed: 0x%x\n%Ls", hr, dll_path.c_str());
        string buffAsStdStr = buff;
        wstring buffAsStdStr2(buffAsStdStr.begin(), buffAsStdStr.end());
        MessageBox(NULL, buffAsStdStr2.c_str(), L"Error", NULL);
    }

    // (optional) unload the .NET runtime; note it cannot be restarted if stopped without restarting the process
    //hr = pClrRuntimeHost->Stop();

    pMetaHost->Release();
    pRuntimeInfo->Release();
    pClrRuntimeHost->Release();

    return hr;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }

    return TRUE;
}