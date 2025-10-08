using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Managers;
using DG.Tweening;
using System;
using Game.Popups;
using System.Collections;
using Firebase.Analytics;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;

    public TileComponent selectedTile = null;
    public int Score;
    public bool Combo;
    public int RightMoveCount;
    public int MatchCount;
    public List<TileComponent> activeTiles;
    public Action<TileComponent, TileComponent> OnMatchSuccess;
    public Action<TileComponent, TileComponent> OnMatchFail;
    public Action OnNoMatchesAvailable;
    public Action<int> ActionUpdateScore;
    public Action<int> ActionUpdateMatchCount;
    public GameObject impactFXPrefab; // assign in inspector

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OnMatchSuccess += OnMatchSuccessCallBack;
        OnNoMatchesAvailable += NoMatchAvailable;
    }

    private void NoMatchAvailable()
    {
        var popup = DependencyManager.Instance.PopupManager.GetPopup<PopupMessage>();
        popup.Setup("Oops!", "No Match Available", Shuffle);
    }

    public void RemoveTile(TileComponent tile)
    {
        if (tile == null) return;

        if (activeTiles.Contains(tile))
        {
            activeTiles.Remove(tile);
            tile.Hide(); // Already in your TileComponent (disables sprite + collider etc.)
            DependencyManager.Instance.MultilayerLevelGenerator.OnTileRemoved?.Invoke(new List<TileComponent> { tile });
        }
    }

    void Shuffle()
    {
        DependencyManager.Instance.MultilayerLevelGenerator.ShuffleTilesInScene();
    }


    //private void OnMatchSuccessCallBack(TileComponent Tile1, TileComponent Tile2)
    //{
    //    DependencyManager.Instance.GameManager.ActionTargetUpdate?.Invoke();
    //    DependencyManager.Instance.MultilayerLevelGenerator.BringTileToTop(Tile1);
    //    DependencyManager.Instance.MultilayerLevelGenerator.BringTileToTop(Tile2);
    //    // Find horizontal midpoint (X between tiles, Y = average)
    //    float centerX = (Tile1.transform.position.x + Tile2.transform.position.x) / 2f;
    //    float centerY = (Tile1.transform.position.y + Tile2.transform.position.y) / 2f;
    //    Vector3 midpoint = new Vector3(centerX, centerY, Tile1.transform.position.z);

    //    // Decide left & right tiles
    //    TileComponent leftTile = (Tile1.transform.position.x < Tile2.transform.position.x) ? Tile1 : Tile2;
    //    TileComponent rightTile = (leftTile == Tile1) ? Tile2 : Tile1;
    //    RightMoveCount++;
    //    MatchCount += 1;
    //    ActionUpdateMatchCount?.Invoke(MatchCount);
    //    float stopGap = 0.4f;        // gap between tiles at collision
    //    float recoilDistance = 1.0f; // bounce distance
    //    float moveTime = 0.2f;
    //    float recoilTime = 0.2f;

    //    // Stop positions → just before touching, always horizontal
    //    Vector3 stopPosLeft = new Vector3(midpoint.x - stopGap, midpoint.y, midpoint.z);
    //    Vector3 stopPosRight = new Vector3(midpoint.x + stopGap, midpoint.y, midpoint.z);

    //    stopPosLeft.z = -5;
    //    stopPosRight.z = -5;

    //    // Recoil positions (swing back horizontally)
    //    Vector3 recoilPosLeft = stopPosLeft + Vector3.left * recoilDistance;
    //    Vector3 recoilPosRight = stopPosRight + Vector3.right * recoilDistance;

    //    // Reset rotations
    //    leftTile.transform.rotation = Quaternion.identity;
    //    rightTile.transform.rotation = Quaternion.identity;

    //    // Build sequence
    //    var seq = DOTween.Sequence();

    //    // Move both tiles horizontally toward midpoint
    //    seq.Append(leftTile.transform.DOMove(stopPosLeft, moveTime).SetEase(Ease.OutSine));
    //    seq.Join(rightTile.transform.DOMove(stopPosRight, moveTime).SetEase(Ease.OutSine));

    //    // Tilt for anticipation
    //    seq.Join(leftTile.transform.DORotate(new Vector3(0, 0, -10f), moveTime * 0.5f));
    //    seq.Join(rightTile.transform.DORotate(new Vector3(0, 0, 10f), moveTime * 0.5f));

    //    // Collision impact
    //    seq.AppendCallback(() =>
    //    {
    //        TriggerImpactFX(midpoint);
    //        Camera.main.DOShakePosition(0.25f, 0.2f, 15, 90, false);
    //    });

    //    // Update Score
    //    Score += 10 * RightMoveCount;
    //    ActionUpdateScore?.Invoke(Score);
    //    DependencyManager.Instance.GameManager.Score = Score;

    //    // Recoil horizontally
    //    seq.Append(leftTile.transform.DOMove(recoilPosLeft, recoilTime).SetEase(Ease.OutQuad));
    //    seq.Join(rightTile.transform.DOMove(recoilPosRight, recoilTime).SetEase(Ease.OutQuad));
    //    seq.Join(leftTile.transform.DORotate(new Vector3(0, 0, 15f), recoilTime).SetEase(Ease.OutBack));
    //    seq.Join(rightTile.transform.DORotate(new Vector3(0, 0, -15f), recoilTime).SetEase(Ease.OutBack));

    //    // Settle back (reduce tilt, slight inward)
    //    seq.Append(leftTile.transform.DOMove(stopPosLeft, 0.05f).SetEase(Ease.InOutSine));
    //    seq.Join(rightTile.transform.DOMove(stopPosRight, 0.05f).SetEase(Ease.InOutSine));
    //    seq.Join(leftTile.transform.DORotate(Vector3.zero, 0.05f).SetEase(Ease.InOutSine));
    //    seq.Join(rightTile.transform.DORotate(Vector3.zero, 0.05f).SetEase(Ease.InOutSine));
    //    DependencyManager.Instance.GameManager.ActionMatchedTile?.Invoke(Tile1.TileSetting.ID);

    //    // Finish → hide tiles
    //    seq.OnComplete(() =>
    //    {
    //        Tile1.Hide();
    //        Tile2.Hide();

    //        DependencyManager.Instance.MultilayerLevelGenerator.OnTileRemoved(new List<TileComponent> { Tile1, Tile2 });


    //    });
    //}
    private void OnMatchSuccessCallBack(TileComponent Tile1, TileComponent Tile2)
    {
        var gameManager = DependencyManager.Instance.GameManager;
        var levelGen = DependencyManager.Instance.MultilayerLevelGenerator;

        gameManager.ActionTargetUpdate?.Invoke();

        // Bring tiles visually to top
        levelGen.BringTileToTop(Tile1);
        levelGen.BringTileToTop(Tile2);

        // Midpoint for meeting point
        float centerX = (Tile1.transform.position.x + Tile2.transform.position.x) / 2f;
        float centerY = (Tile1.transform.position.y + Tile2.transform.position.y) / 2f;
        Vector3 midpoint = new Vector3(centerX, centerY, -5);

        // Identify left/right tiles
        TileComponent leftTile = (Tile1.transform.position.x < Tile2.transform.position.x) ? Tile1 : Tile2;
        TileComponent rightTile = (leftTile == Tile1) ? Tile2 : Tile1;

        RightMoveCount++;
        MatchCount++;
        ActionUpdateMatchCount?.Invoke(MatchCount);

        // 🎬 Trigger visual animation
        StartCoroutine(BevelPullAndDissolveEffect(leftTile, rightTile, midpoint, () =>
        {
            // ✅ After animation completes
            int comboBonus = Mathf.Clamp(RightMoveCount - 1, 0, 5);
            Score += 10 + (comboBonus * 5);
            ActionUpdateScore?.Invoke(Score);
            gameManager.Score = Score;

            gameManager.ActionMatchedTile?.Invoke(Tile1.TileSetting.ID);

            Tile1.Hide();
            Tile2.Hide();
            levelGen.OnTileRemoved(new List<TileComponent> { Tile1, Tile2 });
        }));
    }
    private IEnumerator BevelPullAndDissolveEffect(TileComponent leftTile, TileComponent rightTile, Vector3 midpoint, System.Action onComplete)
    {
        DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile?.Invoke();

        // --- Config ---
        float expandTime = 0.2f;     // outward anticipation
        float pauseTime = 0.15f;      // pause before movement
        float moveTime = 0.35f;      // curved motion time
        float impactScaleTime = 0.15f;
        float dissolveTime = 0.25f;
        float overshootDistance = 1.5f; // bounce past midpoint
        float curveHeightFactor = 0.7f;  // curve depth

        // --- Setup ---
        leftTile.transform.rotation = Quaternion.identity;
        rightTile.transform.rotation = Quaternion.identity;

        SpriteRenderer leftRenderer = leftTile.GetComponent<SpriteRenderer>();
        SpriteRenderer rightRenderer = rightTile.GetComponent<SpriteRenderer>();

        Vector3 leftStart = leftTile.transform.position;
        Vector3 rightStart = rightTile.transform.position;
        float distance = Vector3.Distance(leftStart, rightStart);
        float curveOffset = Mathf.Clamp(distance * curveHeightFactor, 0.3f, 1.0f);

        // Path points for 8-shape motion
        Vector3 leftCurve = new Vector3(midpoint.x - distance * 0.3f, midpoint.y + curveOffset, -5);
        Vector3 rightCurve = new Vector3(midpoint.x + distance * 0.3f, midpoint.y - curveOffset, -5);

        Vector3 leftOvershoot = midpoint + Vector3.left * overshootDistance;
        Vector3 rightOvershoot = midpoint + Vector3.right * overshootDistance;

        Vector3[] leftPath = new Vector3[] { leftStart, leftCurve, leftOvershoot };
        Vector3[] rightPath = new Vector3[] { rightStart, rightCurve, rightOvershoot };

        var seq = DOTween.Sequence();

        // 🔹 Step 1 — Expand outward (anticipation)
        seq.Append(leftTile.transform.DOScale(1.15f, expandTime).SetEase(Ease.OutQuad));
        seq.Join(rightTile.transform.DOScale(1.15f, expandTime).SetEase(Ease.OutQuad));

        // Short pause after expansion
        seq.AppendInterval(pauseTime);

        // 🌀 Step 2 — 8-shape curved motion toward midpoint
        seq.Append(leftTile.transform.DOPath(leftPath, moveTime, PathType.CatmullRom)
            .SetEase(Ease.InOutCubic));
        seq.Join(rightTile.transform.DOPath(rightPath, moveTime, PathType.CatmullRom)
            .SetEase(Ease.InOutCubic));

        // Start fade & partial shrink during curved motion
        seq.Join(leftRenderer.DOFade(0.6f, moveTime * 0.8f).SetEase(Ease.OutQuad));
        seq.Join(rightRenderer.DOFade(0.6f, moveTime * 0.8f).SetEase(Ease.OutQuad));
        seq.Join(leftTile.transform.DOScale(0.9f, moveTime * 0.9f).SetEase(Ease.OutQuad));
        seq.Join(rightTile.transform.DOScale(0.9f, moveTime * 0.9f).SetEase(Ease.OutQuad));

        // 💥 Step 3 — Impact flash + overshoot bounce
        seq.AppendCallback(() =>
        {
            TriggerImpactFX(midpoint);
            Camera.main.DOShakePosition(0.15f, 0.15f, 20, 100, false);

            leftTile.transform.DOPunchScale(Vector3.one * 0.25f, 0.15f, 8, 0.7f);
            rightTile.transform.DOPunchScale(Vector3.one * 0.25f, 0.15f, 8, 0.7f);
        });

        seq.Append(leftTile.transform.DOMove(midpoint, 0.1f).SetEase(Ease.OutBack));
        seq.Join(rightTile.transform.DOMove(midpoint, 0.1f).SetEase(Ease.OutBack));

        // 🌟 Step 4 — Scale burst + fade out
        seq.Append(leftTile.transform.DOScale(1.2f, impactScaleTime).SetEase(Ease.OutQuad));
        seq.Join(rightTile.transform.DOScale(1.2f, impactScaleTime).SetEase(Ease.OutQuad));

        seq.Append(leftTile.transform.DOScale(Vector3.zero, dissolveTime).SetEase(Ease.InBack));
        seq.Join(rightTile.transform.DOScale(Vector3.zero, dissolveTime).SetEase(Ease.InBack));

        seq.Join(leftRenderer.DOFade(0f, dissolveTime));
        seq.Join(rightRenderer.DOFade(0f, dissolveTime));

        seq.OnComplete(() => onComplete?.Invoke());

        yield return seq.WaitForCompletion();
    }


    private void TriggerImpactFX(Vector3 position)
    {
        PlaSfx("Match");
        Vibration.Vibrate(60);

        if (impactFXPrefab == null)
        {
            Debug.LogWarning("ImpactFX prefab not assigned!");
            return;
        }

        // Instantiate prefab
        GameObject fx = GameObject.Instantiate(impactFXPrefab, position, Quaternion.identity);

        // Auto destroy after effect ends (particle lifetime + margin)
        Destroy(fx, 1f);
    }


    public void PlaSfx(string ID)
    {
        var Clip = DependencyManager.Instance.GameManager.GetClipByID(ID);
        DependencyManager.Instance.SoundManager.PlaySFX(Clip);
    }

    public void ClearAllActiveTile()
    {
        foreach (var item in activeTiles)
        {
            Destroy(item.gameObject);
        }
        activeTiles.Clear();
    }

    public void RegisterTile(TileComponent tile)
    {
        if (!activeTiles.Contains(tile))
            activeTiles.Add(tile);
    }

    public bool isLevelCompleted;
    public void UnregisterTile(TileComponent tile)
    {
        if (activeTiles.Contains(tile))
            activeTiles.Remove(tile);


        if (activeTiles.Count<=0)
        {
            DependencyManager.Instance.MatchManager.LevelCompleted();
        }
        
    }

    public void LevelCompleted()
    {
        if (isLevelCompleted)
            return;
        isLevelCompleted = true;
        RightMoveCount = 1;
        DependencyManager.Instance.GameManager.UpdateLevel() ;
        DependencyManager.Instance.PlayerStateManager.PlayerState.Xp += Score;
        DependencyManager.Instance.PlayerStateManager.Save();//.Level += 1;
        DependencyManager.Instance.PopupManager.GetPopup<PopupLevelCompleted>().Show();
        TinySauce.OnGameFinished(true, Score);
        TinySauce.OnGameFinished(true, Score, (DependencyManager.Instance.PlayerStateManager.PlayerState.Level+1));
        FirebaseAnalytics.LogEvent("level_complete", new Parameter("level_index", (DependencyManager.Instance.PlayerStateManager.PlayerState.Level + 1)),
                                               new Parameter("score", DependencyManager.Instance.PlayerStateManager.PlayerState.Xp));

    }

    public List<TileComponent> GetActiveTiles()
    {
        return activeTiles.FindAll(t => t != null && t.gameObject.activeInHierarchy);
    }

    public void OnTileClicked(TileComponent tile)
    {
        // Blocked tile → shake & penalty
        if (!MahjongMatchRules.IsFree(tile))
        {
            HandleBlockedTile(tile);
            return;
        }

        // First selection
        if (selectedTile == null)
        {
            SelectTile(tile);
            return;
        }

        // Clicked the same tile → deselect
        if (tile == selectedTile)
        {
            DeselectTile(tile);
            return;
        }

        // Valid match
        if (MahjongMatchRules.IsMatch(selectedTile, tile))
        {
            HandleMatchSuccess(tile);
            return;
        }

        // Wrong match → unselect previous and make clicked tile the new first tile
        HandleWrongMatchAndSelectNew(tile);
    }

    private void SelectTile(TileComponent tile)
    {
        DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile?.Invoke();
        selectedTile = tile;
        tile.Highlight(true);
        PlaSfx("SelectA");
        Vibration.Vibrate(30);
    }

    private void DeselectTile(TileComponent tile)
    {
        tile.Highlight(false);
        if (selectedTile == tile) selectedTile = null;
    }

    private void HandleBlockedTile(TileComponent tile)
    {
        tile.Shake();
        RightMoveCount = 1;
        if (Score > 0) Score -= 10;
        ActionUpdateScore?.Invoke(Score);

    }

    private void HandleMatchSuccess(TileComponent tile)
    {
        PlaSfx("SelectB");
        Vibration.Vibrate(30);
        OnMatchSuccess?.Invoke(selectedTile, tile);
        selectedTile = null; // clear selection if both tiles are removed/consumed
    }

    private void HandleWrongMatchAndSelectNew(TileComponent tile)
    {
        // keep previous for the fail callback
        var previous = selectedTile;

        // unselect visuals (call the existing unselect action so UI/other systems update)
        DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile?.Invoke();

        // make clicked tile the new 'first' selection
        selectedTile = tile;
        tile.Highlight(true);
        PlaSfx("SelectA");
        Vibration.Vibrate(30);

        RightMoveCount = 1;

        // notify listeners that a match failed between previous and current
        OnMatchFail?.Invoke(previous, tile);
    }


    public bool HasFreeMatches()
    {
        var matches = MahjongMatchRules.GetAllFreeMatches(GetActiveTiles());
        return matches.Count > 0;
    }

    [ContextMenu("Check for Matches or Shuffle")]
    public IEnumerator CheckOrPromptShuffle()
    {
        yield return new WaitForSeconds(.10f);
        if (!HasFreeMatches())
        {
            var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupMessage>();
            Popup.Setup("No Matches", "No available moves. Please shuffle to proceed.", ForceShuffle);
            Popup.Show();
        }
        else
        {
            Debug.Log("Matches are available!");
        }
    }

    public void HighlightAllMatches()
    {
        DependencyManager.Instance.MultilayerLevelGenerator.ActionUnselectAllTile?.Invoke();
        var allTiles = GetActiveTiles();

        foreach (var tile in activeTiles)
        {
            tile.Highlight(false);
        }

        var matches = MahjongMatchRules.GetAllFreeMatches(activeTiles);
        if (matches.Count == 0)
        {
            var Popup = DependencyManager.Instance.PopupManager.GetPopup<PopupMessage>();
            Popup.Setup("No Matches", "No available moves. Please shuffle to proceed.", ForceShuffle);
            Popup.Show();
            return;
        }
        DependencyManager.Instance.PlayerStateManager.PlayerState.Hint -= 1;
        DependencyManager.Instance.GameManager.ActionUpdateStateItems?.Invoke();
        var Index = UnityEngine.Random.Range(0, matches.Count);
        matches[Index].Item1.Highlight(true);
        matches[Index].Item2.Highlight(true);
    }

    private void ForceShuffle()
    {
        var Shuffle = DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle;
        if (Shuffle >= 1)
        {
            Debug.Log(Shuffle);
            DependencyManager.Instance.PlayerStateManager.PlayerState.Shuffle -= 1;
            DependencyManager.Instance.GameManager.ActionUpdateStateItems?.Invoke();
            DependencyManager.Instance.MultilayerLevelGenerator.ShuffleTilesInScene();

        }
        else
        {
            DependencyManager.Instance.PopupManager.GetPopup<PopupShop>().Show();
        }
    }

    public void OnHintButtonClicked()
    {
        HighlightAllMatches();
    }
}
