using Firebase;
using Firebase.Analytics;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("✅ Firebase initialized successfully.");

                // Example analytics event
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
            }
            else
            {
                Debug.LogError($"❌ Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
}
