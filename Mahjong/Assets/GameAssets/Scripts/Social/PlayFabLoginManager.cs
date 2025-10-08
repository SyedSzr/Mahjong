using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Game.Managers;
using System;

public class PlayFabLoginManager : MonoBehaviour
{
    void Start()
    {
        LoginWithDeviceId();
        //DependencyManager.Instance.PlayerStateManager.Load(LoadedData);

    }

    void LoginWithDeviceId()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("✅ PlayFab login successful!");
        Debug.Log("PlayFab ID: " + result.PlayFabId);
        //DependencyManager.Instance.PlayerStateManager.Load(LoadedData);
    }

    private void LoadedData(bool obj)
    {
        Debug.Log("Data Loaded");

    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("❌ PlayFab login failed: " + error.GenerateErrorReport());
    }
}
