using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeAreaBounds : MonoBehaviour {

    private RectTransform curRectTrans;

	private RectTransform rectTrans;

	[HideInInspector]
    public RectTransform rectTransTop;
	[HideInInspector]
	public RectTransform rectTransBottom;

	public bool IgnoreHomeBtnAreaInLandscape = true;


	private Rect lastSafeArea = new Rect(0, 0, 0, 0);

	private bool SafeAreaApplied=false;
    public bool NotchFlagForTestingOnEditor = false;

	void Start () {
        curRectTrans = this.GetComponent<RectTransform>();

		rectTrans = this.GetComponent<RectTransform> ();


		GameObject topObj = new GameObject("Top",new Type[] { typeof(RectTransform)});

		//topObj.AddComponent<Image> ().color = new Color (0, 0, 0,100);


		rectTransTop = topObj.GetComponent<RectTransform> ();
		rectTransTop.SetParent (rectTrans);
		rectTransTop.localScale = Vector3.one;
		rectTransTop.anchorMin = new Vector2 (1,0);
		rectTransTop.anchorMax = new Vector2 (1,1);
		rectTransTop.pivot = new Vector2 (0,0.5f);

		rectTransTop.anchoredPosition3D = Vector3.zero;

		GameObject bottomObj = new GameObject("Bottom", new Type[] { typeof(RectTransform) });

		//bottomObj.AddComponent<Image> ().color = new Color (0, 0, 0, 100);

		rectTransBottom = bottomObj.GetComponent<RectTransform> ();
		rectTransBottom.SetParent (rectTrans);

		rectTransBottom.localScale = Vector3.one;
		rectTransBottom.anchorMin = new Vector2 (0,0);
		rectTransBottom.anchorMax = new Vector2 (0,1);
		rectTransBottom.pivot = new Vector2 (1,0.5f);

		rectTransBottom.anchoredPosition3D = Vector3.zero;

		topObj.SetActive (false);
		bottomObj.SetActive (false);
	}

	void ApplySafeArea(Rect area)
	{

		if (IgnoreHomeBtnAreaInLandscape) {
			area.y = 0;
			area.height = Screen.height;
		}

		var anchorMin = area.position;
		var anchorMax = area.position + area.size;
		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;

		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;

		rectTrans.anchorMin = anchorMin;
		rectTrans.anchorMax = anchorMax;

		if (area.position != Vector2.zero) {

			//Top
			rectTransTop.gameObject.SetActive (true);
			rectTransTop.sizeDelta = new Vector2 (area.size.x, area.y * 2.0f);


			//Bottom
			rectTransBottom.gameObject.SetActive (true);
			rectTransBottom.sizeDelta = new Vector2 (area.size.x, (Screen.height - area.height - area.y) * 2.0f);

		}

		lastSafeArea = area;
		SafeAreaApplied = true;

	}

	// Update is called once per frame
	void Update () 
	{

#if UNITY_EDITOR
		//Testing Notch Margin
		if (NotchFlagForTestingOnEditor)
        {
            Rect safeArea = new Rect(350, 100, Screen.width-100, Screen.height);
            if (safeArea != lastSafeArea)
            {
                ApplySafeAreaForTesting(safeArea);
            }
        }
#endif

#if UNITY_IOS
        try
        {
			
			if(!SafeAreaApplied){
			Rect safeArea = Screen.safeArea;
			if (safeArea != lastSafeArea)
				ApplySafeArea(safeArea);
			}
		} catch (Exception ex) {
			Debug.Log("Exception in Applying SafeArea. SafeAreaBounds::Update. " + ex.Message);
		}
		#endif
	}

    private void ApplySafeAreaForTesting(Rect area)
    {
		if (IgnoreHomeBtnAreaInLandscape)
		{
			area.y = 0;
			area.height = Screen.height;
		}

		//print ("ApplySafeArea ");

		var anchorMin = area.position;
        var anchorMax =  area.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;

        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        curRectTrans.anchorMin = anchorMin;
        curRectTrans.anchorMax = anchorMax;

        lastSafeArea = area;

		if (area.position != Vector2.zero)
		{

			//Top
			rectTransTop.gameObject.SetActive(true);
			rectTransTop.sizeDelta = new Vector2(area.size.x, area.y * 2.0f);


			//Bottom
			rectTransBottom.gameObject.SetActive(true);
			rectTransBottom.sizeDelta = new Vector2(area.size.x, (Screen.height - area.height - area.y) * 2.0f);

		}

	} //F.E.
}
