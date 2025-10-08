using UnityEngine;
using Game.Settings;
using Game.Managers;
using System.Linq;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TileComponent : MonoBehaviour
{
    public int Layer;
    public TileSetting TileSetting;
    private bool interactable = true;
    private SpriteRenderer SpriteRenderer;
    public TileComponent LeftNeighbor;
    public TileComponent RightNeighbor;
    public List<TileComponent> AboveNeighbors=new();
    public Vector3 DefaultPosition;
    public UnityEngine.Rendering.Universal.Light2D light2D;
    public bool isFree;
    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile += DeselectAll;
        DependencyManager.Instance.GameManager.ActionTileStatus += UpdateFreeStatus;
       DefaultPosition = transform.position;
    }

    private void UpdateFreeStatus()
    {
        isFree = MahjongMatchRules.IsFree(this);
    }

    public void Shake()
    {
        transform.DOKill();
        transform.position = DefaultPosition;
        DependencyManager.Instance.MatchManager.PlaSfx("Error");
        transform.DOShakePosition(.2f);
        Vibration.Vibrate(50);
    }
    private void DeselectAll()
    {
        DependencyManager.Instance.MatchManager.selectedTile = null;
        Highlight(false);
    }

    public void Setup()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        var Icon = DependencyManager.Instance.GameConfigurationManager.IconData.IconSettings.FirstOrDefault(x => x.ID == TileSetting.ID).Icon;
        SpriteRenderer.sprite = Icon;
    }

    public bool IsBlockedByAbove()
    {
        return AboveNeighbors.Exists(n => n != null && n.gameObject.activeInHierarchy);
    }


    public void SetInteractable(bool isOn)
    {
        interactable = isOn;
    }
    public bool IsInteractable()
    {
        return interactable && gameObject.activeInHierarchy;
    }


    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Click was on UI, ignore this
            //Debug.Log(EventSystem.current.gameObject.name);
            return;
        }
        if (!interactable) return;

        DependencyManager.Instance.MatchManager.OnTileClicked(this);
    }


public void Highlight(bool enable)
    {
        if (SpriteRenderer != null)
        {
            SpriteRenderer.color = enable
                ? new Color32(47, 149, 255, 255) // #00FF54 (R,G,B,A)
                : Color.white;
             SetLightEnabled(enable);
           
        }
    }

    private Tween pulseTween;

    public void SetLightEnabled(bool enable)
    {
        if (pulseTween != null) pulseTween.Kill(); // stop any existing tween
        light2D.gameObject.SetActive(enable);
        if (enable)
        {
            // Pulse between 0 and 0.5 intensity
            pulseTween = DOTween.To(
                () => light2D.volumeIntensity,
                x => light2D.volumeIntensity = x,
                1,
                1f
            )
            .SetLoops(-1, LoopType.Yoyo)   // infinite up & down
            .SetEase(Ease.InOutSine);      // smooth easing
        }
        else
        {
            // Tween back to 0 and stop
            pulseTween = DOTween.To(
                () => light2D.volumeIntensity,
                x => light2D.volumeIntensity = x,
                0f,
                0.5f
            );
        }
    }

    private void OnDestroy()
    {
        if (DependencyManager.Instance.GameManager)
        {
            DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile -= DeselectAll;
            DependencyManager.Instance.GameManager.ActionTileStatus -= UpdateFreeStatus;
        }
    }

    public void Hide()
    {
        //Debug.Log("Hide");
        DependencyManager.Instance.GameManager.ActionTileStatus -= UpdateFreeStatus;

        Destroy(gameObject);
        DependencyManager.Instance.MatchManager.UnregisterTile(this);
    }
}
