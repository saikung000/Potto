using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoozyUI;
using UniRx; 
using System;

public class UIShowcase : MonoBehaviour
{
    [SerializeField]
    BottleController bottleNo;

    [SerializeField]
    PottoController potto;

    [SerializeField]
    UIElement[] showcaseItems;

	[SerializeField]
	private MoveToTargetScript cameraScript;

	[SerializeField]
	private UIElement EndUI;


    void Awake()
    {
        _Subscribe_Events();
    }

    void OnGameStart()
    {
		potto.gameObject.SetActive(false);
		potto.gameObject.SetActive(true);
        if (potto) {
            Show(potto.Level, 1.0f, 2.0f);
        }
    }

    void OnPottoEvolved(int level)
    {
		if (level >= 4) {
			GameController.GameOver ();
		}
        Show(level, 1.0f, 2.0f);
    }
	public void ShowLevel4(){
		Show(4, 1.0f, 2.0f);
	}



    public void Show(int level, float begin, float end)
    {
        StartCoroutine(_Show_Callback(level, begin, end));
        GameController.EnterCinematic(true);
    }

    void _BottleNoWalkIn(int level)
    {
        if (bottleNo) {
			if (level >= 4) {
				bottleNo.noShowUI = true;
				bottleNo.uiView.SetInGameUI (false);
				return;
			}
            bottleNo.ChangeToStageSetStart();
        }
    }

    IEnumerator _Show_Callback(int level, float begin, float end)
    {
        yield return new WaitForSeconds(begin);
        showcaseItems[level - 1].Show(true);

        yield return new WaitForSeconds(end);
        showcaseItems[level - 1].Hide(false);


        GameController.EnterCinematic(false);
		potto.rigid.isKinematic = false;
		potto.rigid.velocity = Vector3.zero;
		if (level == 4) {
			End ();
		}
        _BottleNoWalkIn(level);
    }

    void _Subscribe_Events()
    {
        GameController.OnGameStart += OnGameStart;
        PottoController.OnEvolveChanged += OnPottoEvolved;
    }

	void End ()
	{
		cameraScript.TargetPosition = potto.gameObject.transform;
		cameraScript.removePosition ();
		cameraScript.GoToPosition ();
		potto.FlyOutScene ();

		Observable.Timer(TimeSpan.FromSeconds(5))
			.Subscribe(_ =>
				{
					EndUI.Show(false);
				});
	}
}
