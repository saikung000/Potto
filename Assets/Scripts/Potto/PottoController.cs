using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class PottoController : MonoBehaviour
{
    [SerializeField]
    PottoState state;

    [SerializeField]
    Animator[] animState;

    [SerializeField]
    int experience;

    [SerializeField]
    int[] experienceCeling;

    [SerializeField]
    float moveForce;

    [SerializeField]
    float dashForce;

    [SerializeField]
    float flyForce;

    [SerializeField]
    float[] moveTimer;

    [SerializeField]
    float[] chargeBarrierTimer;

    [SerializeField]
    Vector2 moveDirection;

    [SerializeField]
    LayerMask bottleMask;

    [SerializeField]
    Vector3 checkBottleOrigin;

    [SerializeField]
    Vector2 checkBottleOffset;

    [SerializeField]
    Vector3[] originOfBottleCheckerList;

    [SerializeField]
    Vector2[] offsetOfBottleCheckerList;

    [SerializeField]
    LayerMask directionChangerMask;

    [SerializeField]
    Vector3 checkDirectionChangerOrigin;

    [SerializeField]
    Vector2 checkDirectionChangerOffset;
    
    [SerializeField]
    Transform upperTarget;


    public delegate void Func(int level);
    public delegate void FuncDrink();

    public static event Func OnEvolveChanged;
    public static event FuncDrink OnDrink;

    public int Level { get { return currentLevel; } }
    public int Experience { get { return experience; } }
    public bool IsMaxExperience { get { return (currentCelingIndex + 1) == experienceCeling.Length; } }
    public bool IsImmuneFromGainingExperience { get { return isUsingChargeBarrier; } }

    public PottoState State { get { return state; } }


    int currentLevel;
    int currentCelingIndex;

    bool isCanUseDash;
    bool isCanChargeBarrier;

    bool isUsingDash;
    bool isUsingChargeBarrier;

    bool isInCinematic;
    bool isDrinking;

    Vector2 scale;
    Vector2 velocity;

    Vector2 currentBottleOrigin;
    Vector3 currentBottleOffset;

    Collider2D hitByBottle;
    Collider2D hitByDirectionChanger;

	Vector3 startPosition;

	public Transform flyTarget;
	public Transform mainTarget;

    Timer[] timers;
   public Rigidbody2D rigid;

	public delegate void OnOutScene ();
	public static event OnOutScene OnFlyOutScene;


#if UNITY_EDITOR
     void OnDrawGizmosSelected()
     {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position + checkDirectionChangerOrigin, 
            new Vector3(
                checkDirectionChangerOffset.x,
                checkDirectionChangerOffset.y,
                0.0f
            )
        );
        
        Handles.Label(transform.position + checkDirectionChangerOrigin, "Direction changer's trigger area.."); 

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            transform.position + checkBottleOrigin, 
            new Vector3(
                checkBottleOffset.x,
                checkBottleOffset.y,
                0.0f
            )
        );

        Handles.Label(transform.position + checkBottleOrigin, "Bottle's add experience area.."); 
     }
#endif

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        timers = GetComponents<Timer>();

        _Subscribe_Events();
		startPosition = transform.position;
    }

	void OnEnable(){
		currentLevel = 1;
		currentCelingIndex = 0;
		experience = 0;
		rigid.velocity = Vector3.zero;
		rigid.isKinematic = true;
		transform.position = startPosition;
	}

    void Start()
    {
    }

    void Update()
    {
        if (!isInCinematic) {
            _PottoHandler();
        }

        _AnimationHandler();
    }

    void FixedUpdate()
    {
        if (!isInCinematic) {
            _ChangeOriginOfBottleChecker();
            _State_Involve_Physics_Handler();
            _CheckBottle();

            if (isDrinking) {
                _StandStill();
            }
        }
        else {
			if (Level == 4) {
				_FinalFormHandler ();
			}else 
            _StandStill();
        }
    }

    void Destroy()
    {
        _UnSubscribe_Events();
    }

	public void EndGame()
	{
		upperTarget = flyTarget;
		StartCoroutine (WaitEndGame());
	}
	IEnumerator WaitEndGame()
	{
		yield return new WaitForSeconds (1f);
		if (PottoController.OnFlyOutScene != null) {
			PottoController.OnFlyOutScene();
		}
	}
	public void FlyOutScene(){
		upperTarget = mainTarget;
	}

    void _AnimationHandler()
    {
        if (!isDrinking) {
            switch (state) {
                case PottoState.Idle:
                    animState[currentLevel - 1].Play("walking");
                    break;

                case PottoState.Move:
                    if (isUsingDash && !timers[1].IsFinished) {
                        animState[currentLevel - 1].Play("dashing");
                    }
                    else {
                        animState[currentLevel - 1].Play("walking");
                    }
                    break;

                case PottoState.Agressive:
                    if (isUsingChargeBarrier) {
                        animState[currentLevel - 1].Play("getdown");
                    }
                    else {
                        animState[currentLevel - 1].Play("walking");
                    }
                    break;

                case PottoState.FinalForm:
                        animState[currentLevel - 1].Play("noarmlegflying");
                    break;
            }
        }
        else {
            animState[currentLevel - 1].Play("drinking");
        }
    }

    void _CheckBottle()
    {
        hitByBottle = Physics2D.OverlapBox(
                transform.position + checkBottleOrigin, 
                checkBottleOffset,
                0.0f,
                bottleMask
        );

        if (hitByBottle) {
            var bottleControl = hitByBottle.gameObject.GetComponent<BottleController>();
            var exp = bottleControl.AddExpToPotto();
            AddExperience(exp);
        }
    }

    void _PottoHandler()
    {
        _ToggleSprite();
        _DecideState();
        _CheckEvole();
        _StateHandler();
        _DashHandler();
        _ChargeBarrierHandler();
        _DrinkingHandler();
    }

    void _ToggleSprite()
    {
        for (int i = 0; i < animState.Length; i++) {
            animState[i].gameObject.SetActive((i + 1) == currentLevel);
        }
    }

    void _DecideState()
    {
        state = (PottoState)currentCelingIndex;
    }

    void _CheckEvole()
    {
        if (currentCelingIndex + 1 < experienceCeling.Length) {
            if (experience > experienceCeling[currentCelingIndex]) {
                _Evolve();
            }
        }
    }

    void _Evolve()
    {
        experience -= experienceCeling[currentCelingIndex];
        currentCelingIndex += 1;
        currentLevel += 1;

        if (OnEvolveChanged != null) {
            OnEvolveChanged(currentLevel);
        }
    }

    void _StateHandler()
    {
        switch (state) {
            case PottoState.Idle:
                _IdleHandler();
                break;

            default:
                break;
        }
    }

    void _ChangeOriginOfBottleChecker()
    {
        checkBottleOrigin = originOfBottleCheckerList[currentLevel - 1];
    }

    void _ChangeOffsetOfBottleChecker()
    {
        checkBottleOffset = offsetOfBottleCheckerList[currentLevel - 1];
    }

    void _State_Involve_Physics_Handler()
    {
        if (PottoState.Move == state) {
            _MoveHandler();
        }
        else if (PottoState.Agressive == state) {
            _AgressiveHandler();
        }
        else if (PottoState.FinalForm == state) {
            _FinalFormHandler();
        }
        else {
            _StandStill();
        }
    }

    void _IdleHandler()
    {
        _StandStill();
    }

    void _MoveHandler()
    {
        _ChangeDirectionHandler();

        if (isCanUseDash) {
            if (isUsingDash) {

                _Dash();

                timers[1].MaxSeconds = moveTimer[1];
                timers[1].CountDown();
            }
            else {
                if (!timers[1].IsStart || timers[1].IsFinished) {
                    _Move();
                }
            }
        }
        else {
            _Move();
        }
    }

    void _ChangeDirectionHandler()
    {
        hitByDirectionChanger = Physics2D.OverlapBox(
                transform.position + checkDirectionChangerOrigin, 
                checkDirectionChangerOffset,
                0.0f,
                directionChangerMask
        );

        if (hitByDirectionChanger) {
            var dirChangerObj = hitByDirectionChanger.gameObject.GetComponent<DirectionChanger>();
            moveDirection = dirChangerObj.Direction;
        }
    }

    void _AgressiveHandler()
    {
        _ChargeBarrierHandler();
    }

    void _FinalFormHandler()
    {
        rigid.velocity = (upperTarget.position - transform.position) * flyForce * Time.deltaTime;
    }

    void _DashHandler()
    {
        isCanUseDash = (state == PottoState.Move) && (experience > 100);

        if (IsMaxExperience) {
            return;
        }

        if (!isCanUseDash) {
            isUsingDash = false;
            return;
        }

        if (timers[0].IsStart) {
            if (timers[0].IsFinished) {
                isUsingDash = true;
            }
        }
        else {
            timers[0].MaxSeconds = moveTimer[0];
            timers[0].CountDown();
        }
    }

    void _ChargeBarrierHandler()
    {
        isCanChargeBarrier = (PottoState.Agressive == state) && (experience > 100);

        if (IsMaxExperience) {
            return;
        }

        if (!isCanChargeBarrier) {
            isUsingChargeBarrier = false;
            _MoveHandler();
        }

        if (timers[0].IsStart) {
            if (timers[0].IsFinished && !timers[1].IsStart) {
                isUsingChargeBarrier = true;

                timers[1].MaxSeconds = chargeBarrierTimer[1];
                timers[1].CountDown();
            }
        }
        else {
            if (timers[1].IsFinished && !timers[0].IsStart) {
                isUsingChargeBarrier = false;

                timers[0].MaxSeconds = chargeBarrierTimer[0];
                timers[0].CountDown();
            }
        }

        _ChargeBarrier();
    }

    void _DrinkingHandler()
    {
        if (OnDrink != null) {
            OnDrink();
            OnDrink -= _OnDrink;
        }
    }

    void _OnDrink()
    {
        StartCoroutine(_Drinking_Callback());
    }

    IEnumerator _Drinking_Callback()
    {
        animState[currentLevel - 1].Play("drinking");
        yield return new WaitForSeconds(1.0f);
        animState[currentLevel - 1].Play(-1);
        isDrinking = false;
    }

    void _Move()
    {
        velocity.x = (moveDirection.x * moveForce) * Time.deltaTime;
        velocity.y = rigid.velocity.y;

        rigid.velocity = velocity;
    }

    void _Dash()
    {
        rigid.AddForce(moveDirection * dashForce, ForceMode2D.Impulse);
        isUsingDash = false;
    }

    void _StandStill()
    {
        velocity = Vector2.zero;
        velocity.y = rigid.velocity.y;

        rigid.velocity = velocity;
    }

    void _ChargeBarrier()
    {
        if (isCanChargeBarrier) {
            if (isUsingChargeBarrier) {
                _StandStill();
            }
            else {
                _MoveHandler();
            }
        }
    }

    void _Drinking()
    {
        animState[currentLevel - 1].Play("drinking");
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

    public void AddExperience(int value)
    {
        if (!isUsingChargeBarrier) {
            experience += Mathf.Abs(value);
            OnDrink += _OnDrink;
            isDrinking = true;
        }
    }
}
