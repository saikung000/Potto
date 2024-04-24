using UnityEngine;
using UniRx;
using System;

public class BottleController : MonoBehaviour
{
    private float angle;
    [SerializeField]
    private float minAngle;
    [SerializeField]
    private float maxAngle;

    private float power;
    [SerializeField]
    private float minPower;
    [SerializeField]
    private float maxPower;

    public int water;
    [SerializeField]
    private float minMass;
    [SerializeField]
    private float maxMass;

    [SerializeField]
    private Rigidbody2D rig2D;

    [SerializeField]
    private float timeDie;
    [SerializeField]
    private ParticleSystem particleDie;
    [SerializeField]
    private Vector3 walkPosition;
    private Vector3 spawnPosition;
    [SerializeField]
    private float timeSpawn;
    [SerializeField]
    private float timePlay;

    private bool isHitGround;
    private bool isPress;
    private bool isWater;
    private bool isSentWater;

    private CompositeDisposable timerComposition = new CompositeDisposable();
    private CompositeDisposable timerDieComposition = new CompositeDisposable();

    public BottleStage bottleStage;

    public UIBottleView uiView;

    [SerializeField]
    private GameObject waterSprite;
    [SerializeField]
    private Vector3 StartPoint;
    [SerializeField]
    private Animator anim;

    private bool isInCinematic;

	public bool noShowUI;

    private void Start()
    {
        spawnPosition = transform.position;
        _Subscribe_Events();
    }

    public void ChangeToStageSetStart()
    {
        if (isInCinematic)
            return;

        ResetBottle();
        anim.SetTrigger("walking");
        Observable.IntervalFrame(1).Subscribe(x => Walk()).AddTo(timerComposition);
        Observable.Timer(TimeSpan.FromSeconds(timePlay))
            .Subscribe(_ =>
            {
					if(!noShowUI){
					uiView.SetInGameUI(true);
					}
                timerComposition.Clear();
                anim.SetTrigger("idle");
                ChangeToStageSetWater();
                ResetUI();

            });
    }

    private void ResetBottle()
    {
		noShowUI = false;
        timerDieComposition.Clear();
        gameObject.layer = 9;
        gameObject.SetActive(true);
        rig2D.isKinematic = true;
        transform.position = spawnPosition;
        waterSprite.transform.localPosition = new Vector3(0, (0 * (0 - StartPoint.y)) / 100 + StartPoint.y, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
        bottleStage = BottleStage.Start;
    }

    private void ResetUI()
    {
        angle = minAngle;
        uiView.SetAngleBar(angle);
        water = 0;
        uiView.SetWaterBar(water);
        power = minPower;
        uiView.SetPowerBar(power, maxPower, minPower);
    }

    public void ChangeToStageSetNone()
    {
        anim.SetTrigger("shoot");
        isSentWater = false;
        Shoot(water, angle, power);
        bottleStage = BottleStage.None;

        Observable.Timer(TimeSpan.FromSeconds(timeDie))
            .Subscribe(_ =>
            {
                Die();
            }).AddTo(timerDieComposition);
    }
    public void ChangeToStageSetWater()
    {
        bottleStage = BottleStage.SetWater;
        isPress = false;
        isWater = false;
    }
    public void ChangeToStageSetAngle()
    {
        bottleStage = BottleStage.SetAngle;
        isPress = false;
        Observable.IntervalFrame(1).Subscribe(x => SetAngle()).AddTo(timerComposition);
    }
    public void ChangeToStageSetPower()
    {
        bottleStage = BottleStage.SetPower;
        isPress = false;
        Observable.IntervalFrame(1).Subscribe(x => SetPower()).AddTo(timerComposition);
    }

    private void Die()
    {
        timerDieComposition.Clear();
        //TODO playerParticle 
        gameObject.SetActive(false);
        Observable.Timer(TimeSpan.FromSeconds(timeSpawn))
            .Subscribe(_ =>
            {
                ChangeToStageSetStart();
            });
    }


    private void FixedUpdate()
    {
        if (!isHitGround &&  bottleStage == BottleStage.None)
        {
            float angle = Mathf.Atan2(rig2D.velocity.y, rig2D.velocity.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 500 * Time.deltaTime);
        }
        if (isPress)
            return;
        switch (bottleStage)
        {
		case BottleStage.SetWater:
			if (Input.GetKeyUp (KeyCode.Space) && isWater) 
			{
				uiView.PlayWaterParticle (false);
				anim.SetTrigger ("waterFinish");
				SetFinish ();
			} else if (Input.GetKeyDown (KeyCode.Space)) 
			{
				uiView.PlayWaterParticle (true);
				isWater = true;
				Observable.IntervalFrame (1).Subscribe (x => {
					
					anim.SetTrigger ("water");			
					FillWater ();

				}).AddTo (timerComposition);
			}
                break;
            case BottleStage.SetAngle:
                if (Input.GetKeyDown(KeyCode.Space))
                    SetFinish();
                break;
            case BottleStage.SetPower:
                if (Input.GetKeyDown(KeyCode.Space))
                    SetFinish();
                break;
            case BottleStage.None:
                break;
        }
    }

    private void SetFinish()
    {
        timerComposition.Clear();
        isPress = true;
        Observable.Timer(TimeSpan.FromSeconds(0.5f))
              .Subscribe(_ =>
              {
                  switch (bottleStage)
                  {
                      case BottleStage.SetWater:
                          ChangeToStageSetAngle();
                          break;
                      case BottleStage.SetAngle:
                          ChangeToStageSetPower();
                          break;
                      case BottleStage.SetPower:
                          ChangeToStageSetNone();
                          break;
                  }
              });
    }

    public void Shoot(int water, float angle, float power)
    {
        rig2D.isKinematic = false;
        isHitGround = false;
        rig2D.mass = ((float)water * (maxMass - minMass)) / 100 + minMass;
        Vector3 dir = Quaternion.AngleAxis(angle-270, Vector3.forward) * Vector3.right;
        rig2D.AddForce(dir * power);

    }

    public void FillWater()
    {
        water = Mathf.Clamp(water += 2, 0, 100);
        waterSprite.transform.localPosition = new Vector3(0, ((float)water * (0 - StartPoint.y)) / 100 + StartPoint.y, 0);
        uiView.SetWaterBar(water);
    }
    public void SetAngle()
    {
        angle = Mathf.PingPong(Time.time * 2, 1) * 90f;
        uiView.SetAngleBar(angle);
    }
    public void SetPower()
    {
        power = (Mathf.PingPong(Time.time * 2, 1) * (maxPower - minPower)) + minPower;
        uiView.SetPowerBar(power, maxPower, minPower);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isHitGround = true;
            Observable.Timer(TimeSpan.FromSeconds(1f))
          .Subscribe(_ =>
          {
              gameObject.layer = 11;
          }).AddTo(timerDieComposition);
        }
        if (collision.transform.tag == "Potto") {
            isHitGround = true;
            gameObject.layer = 11;
        }
    }

    public int AddExpToPotto() {
        isSentWater = true;
        Die();
        return water;
    }

    public void Walk() {
        transform.position = Vector3.MoveTowards(transform.position,walkPosition, 3*Time.deltaTime);
    }


    void _OnEnterCinematic()
    {
        isInCinematic = true;
    }

    void _OnExitCinematic()
    {
        isInCinematic = false;
    }

    void _Subscribe_Events()
    {
        GameController.OnEnterCinematic += _OnEnterCinematic;
        GameController.OnExitCinematic += _OnExitCinematic;
    }

    void _UnSubscribe_Events()
    {
        GameController.OnEnterCinematic -= _OnEnterCinematic;
        GameController.OnExitCinematic -= _OnExitCinematic;
    }
}
