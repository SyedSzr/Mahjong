using UnityEngine;
using Game.Settings;

namespace Game.Managers
{
    public class DependencyManager : MonoBehaviour
    {
        public static DependencyManager Instance;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
            }
        }

        private InAppManager mInAppManager;
        public InAppManager InAppManager
        {
            get
            {
                if(mInAppManager==null)
                {
                    mInAppManager = FindObjectOfType<InAppManager>(true);
                }
                return mInAppManager;
            }
        }
        private LeaderboardManager mLeaderboardManager;
        public LeaderboardManager LeaderboardManager
        {
            get
            {
                if(mLeaderboardManager == null)
                {
                    mLeaderboardManager = FindObjectOfType<LeaderboardManager>(true);
                }
                return mLeaderboardManager;
            }
        }

        private PlayerStateManager mPlayerStateManager;
        public PlayerStateManager PlayerStateManager
        {
            get
            {
                if (mPlayerStateManager == null)
                {
                    mPlayerStateManager = FindObjectOfType<PlayerStateManager>(true);
                }
                return mPlayerStateManager;
            }
        }

        private SoundManager mSoundManager;
        public SoundManager SoundManager
        {
            get
            {
                if (mSoundManager == null)
                {
                    mSoundManager = FindObjectOfType<SoundManager>(true);
                }
                return mSoundManager;
            }
        }


        private GooglePlayServicesManager mGooglePlayServicesManager;
        public GooglePlayServicesManager GooglePlayServicesManager
        {
            get
            {
                if (mGooglePlayServicesManager == null)
                {
                    mGooglePlayServicesManager = FindObjectOfType<GooglePlayServicesManager>(true);
                }
                return mGooglePlayServicesManager;
            }
        }


        private GameConfigurationManager mGameConfigurationManager;
        public GameConfigurationManager GameConfigurationManager
        {
            get
            {
                if (mGameConfigurationManager == null)
                {
                    mGameConfigurationManager = FindObjectOfType<GameConfigurationManager>(true);
                }
                return mGameConfigurationManager;
            }
        }

        private PopupManager mPopupManager;
        public PopupManager PopupManager
        {
            get
            {
                if (mPopupManager == null)
                {
                    mPopupManager = FindObjectOfType<PopupManager>(true);
                }
                return mPopupManager;
            }
        }

        private ScreenManager mScreenManager;
        public ScreenManager ScreenManager
        {
            get
            {
                if (mScreenManager == null)
                {
                    mScreenManager = FindObjectOfType<ScreenManager>(true);
                }
                return mScreenManager;
            }
        }


        private LevelGenerator mLevelGenerator;
        public LevelGenerator LevelGenerator
        {
            get
            {
                if (mLevelGenerator == null)
                {
                    mLevelGenerator = FindObjectOfType<LevelGenerator>(true);
                }
                return mLevelGenerator;
            }
        }

        private MultilayerLevelGenerator mMultilayerLevelGenerator;
        public MultilayerLevelGenerator MultilayerLevelGenerator
        {
            get
            {
                if (mMultilayerLevelGenerator == null)
                {
                    mMultilayerLevelGenerator = FindObjectOfType<MultilayerLevelGenerator>(true);
                }
                return mMultilayerLevelGenerator;
            }
        }


        private MatchManager mMatchManager;
        public MatchManager MatchManager
        {
            get
            {
                if (mMatchManager == null)
                {
                    mMatchManager = FindObjectOfType<MatchManager>(true);
                }
                return mMatchManager;
            }
        }

        private GameManager mGameManager;
        public GameManager GameManager
        {
            get
            {
                if (mGameManager == null)
                {
                    mGameManager = FindObjectOfType<GameManager>(true);
                }
                return mGameManager;
            }
        }

    }
}