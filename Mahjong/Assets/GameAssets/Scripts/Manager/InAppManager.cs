using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing.Extension;

namespace Game.Managers
{
    public class InAppManager : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;
        private static IExtensionProvider m_StoreExtensionProvider;

        private Action<string> ActionSuccessCallback = null;
        private Action<string> ActionFailCallback = null;
        private List<string> ProductIDs;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            InAppPurchaseInitialize();

        }

        public void InAppPurchaseInitialize()
        {
            var storeData = DependencyManager.Instance.GameConfigurationManager.ShopData;
            ProductIDs = storeData.ShopItemSettings
                .Where(x => x.Consumable)
                .Select(x => x.PackegeName)
                .ToList();

            StartCoroutine(InitializePurchasingAsync());
        }

        private IEnumerator InitializePurchasingAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions().SetEnvironmentName("production");
                var task = UnityServices.InitializeAsync(options);
                yield return new WaitUntil(() => task.IsCompleted);

                if (task.IsFaulted)
                {
                    Debug.LogError("Unity Services Initialization Failed: " + task.Exception);
                    yield break;
                }
            }

            if (!IsInitialized())
            {
                InitializePurchasing(ProductIDs);
            }
        }

        public void InitializePurchasing(List<string> productIDs)
        {
            this.ProductIDs = productIDs;

            // Create purchasing module with fake store for editor
            var module = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

            var builder = ConfigurationBuilder.Instance(module);

            // Add all consumable products
            foreach (var id in ProductIDs)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    builder.AddProduct(id, ProductType.Consumable);
                }
            }

            // Add non-consumable product if exists (from original script)
            var storeData = DependencyManager.Instance.GameConfigurationManager.ShopData;
            var nonConsumable = storeData.ShopItemSettings.FirstOrDefault(x => !x.Consumable);
            if (nonConsumable != null)
            {
                Debug.Log(nonConsumable.PackegeName);
                builder.AddProduct(nonConsumable.PackegeName, ProductType.NonConsumable);
            }

            // Standard initialization that works across all versions
            UnityPurchasing.Initialize(this, builder);
        }
        public void PurchaseItem(string BundleAndroid, string BundleIOS, Action<string> ActionSuccessCallback, Action<string> ActionFailCallback)
        {
            this.ActionSuccessCallback = ActionSuccessCallback;
            this.ActionFailCallback = ActionFailCallback;

            if (Application.platform == RuntimePlatform.Android)
            {
                BuyProduct(BundleAndroid);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                BuyProduct(BundleIOS);
            }
            else
            {
                this.ActionSuccessCallback?.Invoke("Sucessfully Purchased Item ");
            }
        }

        public bool IsInitialized()
        {
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        private Product mProduct;
        public Product Product(string ID)
        {
            if (IsInitialized())
            {
                if (mProduct == null)
                {
                    mProduct = m_StoreController.products.WithID(ID);
                }
            }
            return mProduct;
        }

        string GetProductId(string ProductIdAndroid, string ProductIdIOS)
        {
            return Application.platform == RuntimePlatform.IPhonePlayer ? ProductIdIOS : ProductIdAndroid;
        }

        public string GetProductPrice(string ProductIdAndroid, string ProductIdIOS)
        {
            var productId = GetProductId(ProductIdAndroid, ProductIdIOS);
            return GetPrice(productId);
        }

        public string GetPrice(string ProductId)
        {
            if (!IsInitialized())
                return "$0.00";
            var product = m_StoreController.products.WithID(ProductId);
            return product?.metadata.localizedPriceString ?? "$0.00";
        }

        void BuyProduct(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log($"Purchasing product asynchronously: '{product.definition.id}'");
                    m_StoreController.InitiatePurchase(product);
                }
                else
                {
                    ActionFailCallback?.Invoke("FAIL. Not purchasing product, either not found or not available");
                    Debug.Log(productId+" BuyProductID: FAIL. Not purchasing product, either not found or not available");
                }
            }
            else
            {
                ActionFailCallback?.Invoke("BuyProductID FAIL. Not initialized.");
                Debug.Log(productId+" BuyProductID FAIL. Not initialized.");
            }
        }


        private Action<bool> CallBack;
        public void Restore(Action<bool> CallBack)
        {
            this.CallBack = CallBack;
            this.CallBack?.Invoke(HasNoAds());
            
        }
       
        bool HasNoAds()
        {
            var noAdsProduct = m_StoreController.products.WithID("com.game.noads");
            return noAdsProduct != null && noAdsProduct.hasReceipt;
        }

        public void RestorePurchases()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result, error) =>
                {
                    Debug.Log("RestorePurchases continuing: " + result);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogWarning("RestorePurchases error: " + error);
                    }
                });

            }
            else
            {
                Debug.Log("RestorePurchases is not supported on this platform. Current = " + Application.platform);
            }
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason: " + error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log(error + "---------" + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            ActionSuccessCallback?.Invoke("Successfully purchased");
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', Reason: {failureReason}");
            ActionFailCallback?.Invoke("Not purchasing product");
        }
    }
}
