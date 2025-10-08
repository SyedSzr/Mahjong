using UnityEngine;
using Game.State;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Game.Extensions;

namespace Game.Managers
{
    public class PlayerStateManager : MonoBehaviour
    {
        public string PlayFabID;
        public string DisplayName;
        public PlayerState PlayerState;

        JsonSerializerSettings JsonSetting = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private void Start()
        {
            TinySauce.OnGameStarted(PlayerState.Level+1);
            TinySauce.SubscribeOnInitFinishedEvent(onInitFinished);
            Debug.Log("Loading Level");
            StartCoroutine(LoadDataCourutine());

            
        }

        private void onInitFinished(bool arg1, bool arg2)
        {
            Debug.Log("Finished Initialization");
        }

        IEnumerator LoadDataCourutine()
        {
            yield return new WaitForEndOfFrame();
            Load(OnDataLoad);
        }

        private void OnDataLoad(bool obj)
        {
            var Level = DependencyManager.Instance.PlayerStateManager.PlayerState.Level;
            Debug.Log("Level Loaded " + Level);
            DependencyManager.Instance.GameManager.ActionUpdateLevel?.Invoke();
            DependencyManager.Instance.GameManager.ActionUpdateStateItems?.Invoke();

        }

        public void Save()
        {
            var Data = JsonConvert.SerializeObject(PlayerState, Formatting.Indented, JsonSetting);
            PlayerPrefs.SetString("PlayerState", Data);
            DependencyManager.Instance.GooglePlayServicesManager.PostScore(PlayerState.Xp, "CgkIi4zHgLwdEAIQAA");
            //SetUserData(Data);
        }

        public void Load(Action<bool> Callback)
        {
            var Data = PlayerPrefs.GetString("PlayerState", "Default");
            PlayerState = JsonConvert.DeserializeObject<PlayerState>(Data, JsonSetting);

            Callback?.Invoke(true);
            DependencyManager.Instance.GameManager.ActionDataLoaded?.Invoke();

            //GetUserData(LocalLoadData);
            //void LocalLoadData(bool Status, string Data)
            //{
            //    if (Data != "Default")
            //    {
            //        PlayerState = JsonConvert.DeserializeObject<PlayerState>(Data, JsonSetting);
            //    }
            //    Callback?.Invoke(Status);
            //}
        }

        public void GetUserData(Action<bool,string> Callback)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = PlayFabID,
                Keys = null
            },
            result =>
            {
                Debug.Log("Got user data:");
                string PlayerStateJson = "";
                if (result.Data == null || !result.Data.ContainsKey("PlayerState"))
                {
                    Debug.Log("No PlayerState");
                    PlayerStateJson = "Default";
                }
                else
                {
                    PlayerStateJson = result.Data["PlayerState"].Value;
                    Debug.Log("PlayerState: " + PlayerStateJson);
                }
                Callback?.Invoke(true, PlayerStateJson);
            },
            (error) =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
                Callback?.Invoke(false, "");
            });
        }

        public void SetUserData(string JSON)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {"PlayerState", JSON}
                }
            },
            result =>
            {
                Debug.Log("Successfully updated user data");
            },
            error =>
            {
                Debug.Log("Got error setting user data Ancestor to Arthur");
                Debug.Log(error.GenerateErrorReport());
            });
        }

        public void OnDisable()
        {
            Save();
        }

        #region Get States

        //private List<BaseCurrencyState> mCurrencyStates;
        //public List<BaseCurrencyState> CurrencyStates
        //{
        //    get
        //    {
        //        if (mCurrencyStates.IsNullOrEmpty())
        //        {
        //            mCurrencyStates = new List<BaseCurrencyState>();
        //            mCurrencyStates.AddRange(PlayerState.CurrencyStates);
        //            mCurrencyStates.AddRange(PlayerState.RegenerativeCurrencyStates);
        //        }
        //        return mCurrencyStates;
        //    }
        //}

        //public BaseCurrencyState GetCurrencyState(string SettingID)
        //{
        //    return CurrencyStates.FirstOrDefault(x => x.SettingID == SettingID);
        //}

        //public string GetSelectedDotorSkinID()
        //{
        //    return PlayerState.DoctorStates[0].UnlockedSkins.FirstOrDefault(x => x == PlayerState.DoctorStates[0].SelectedSkinID);
        //}

        //public StoreItemState GetSoreItemState(string SettingID)
        //{
        //    var State = PlayerState.StoreItemStates.FirstOrDefault(x => x.SettingID == SettingID);
        //    if (State == null)
        //    {
        //        State = new StoreItemState();
        //        State.SettingID = SettingID;
        //        State.Purchased = true;
        //    }
        //    PlayerState.StoreItemStates.Add(State);
        //    return State;
        //}

        #endregion
    }
}