using System;
using System.Collections;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    public enum AbilityName
    {
        PlayerEnhancedDash,
        //TimeStop,
        PlayerEnhance,
        PlayerHeal
    }

    [Header("Ability")]
    public AbilityName curAbility;

    [Header("AbilityProperties")]
    public LayerMask interactable;
    private PlayerMovement player;
    private float curRange;
    RaycastHit hit;

    [Header("Orb")]
    [SerializeField] public float dashSpeed = 100;
    public float dashCooldown;

    [Header("TimeStop / Graviton")]
    public float timeStopCooldown;
    public float radius;
    public float attractSpeed;
    public float attractDelay;
    public float slowDur;
    public float slowDelay;
    public float timeTick;

    [Header("PlayerEnhance")]
    public float playerEnchanceCooldown;
    public float playerMultiplier;
    public float enhanceTime;

    [Header("PlayerHeal")]
    public float healCooldown;
    public float healDur;
    public float healAmount;
    public float healDelay;
    float curAmountHealed;

    [HideInInspector]
    public float curTimeStopCooldown;
    [HideInInspector]
    public float curPlayerEnchanceCooldown;
    [HideInInspector]
    public float curEnhancedDashCooldown;
    [HideInInspector]
    public float curHealCooldown;
    [HideInInspector]
    public float curTimeStopDur;
    [HideInInspector]
    public float curPlayerEnhanceDur;
    [HideInInspector]
    public float curHealDur;
    [HideInInspector]
    public float curOrbDur;
    [HideInInspector]
    public float mathHeal;

    [Header("Debugging")]
    public GameObject currentHitObject;
    public Transform handModel;
    private GameObject actualModel;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        actualModel = player.actualPlayer;
    }

    public void Update()
    {
        if (DevMode.devMode)
        {
            curHealCooldown = 0;
            curEnhancedDashCooldown = 0;
            curPlayerEnchanceCooldown = 0;
            curTimeStopCooldown = 0;
        }

        CheckCooldown();
        //Player Color Change
        if (Input.GetButtonDown("NextAbility"))
        {
            if ((int)curAbility == (int)AbilityName.PlayerHeal)
            {
                curAbility = AbilityName.PlayerEnhancedDash;
            }
            else
            {
                curAbility++;
            }
        }
        if (Input.GetButtonDown("PrevAbility"))
        {
            if (curAbility == (int)AbilityName.PlayerEnhancedDash)
            {
                curAbility = AbilityName.PlayerHeal;
            }
            else
            {
                curAbility--;
            }
        }
        switch (curAbility)
        {
            case AbilityName.PlayerEnhancedDash:
                GameManager.instance.uiMan.BorderReset();
                GameManager.instance.uiMan.playerDashBorder.enabled = true;
                break;
            case AbilityName.PlayerEnhance:
                GameManager.instance.uiMan.BorderReset();
                GameManager.instance.uiMan.playerEnhanceBorder.enabled = true;
                break;
            case AbilityName.PlayerHeal:
                GameManager.instance.uiMan.BorderReset();
                GameManager.instance.uiMan.playerHealBorder.enabled = true;
                break;
        }
        //Actual Ability
        if (Input.GetButtonDown("Fire3"))
        {
            switch (curAbility)
            {
                case AbilityName.PlayerEnhancedDash:
                    EnhancedDash();
                    break;
                case AbilityName.PlayerEnhance:
                    PlayerEnhance();
                    break;
                case AbilityName.PlayerHeal:
                    PlayerHeal();
                    break;
            }
        }
    }

    public void CheckCooldown()
    {
        if(GameManager.gameTime != 1)
        {
            return;
        }
        if (curTimeStopCooldown > 0)
        {
            curTimeStopCooldown -= Time.deltaTime;
        }

        if (curEnhancedDashCooldown > 0)
        {
            curEnhancedDashCooldown -= Time.deltaTime;
        }

        if (curPlayerEnchanceCooldown > 0)
        {
            curPlayerEnchanceCooldown -= Time.deltaTime;
        }

        if (curHealCooldown > 0)
        {
            curHealCooldown -= Time.deltaTime;
        }
    }

    public void EnhancedDash()
    {
        if (curEnhancedDashCooldown <= 0)
        {
            player.SetAbility();
            player.StartCoroutine(player.DashForward());
        }
    }

    //public void TimeStop()
    //{
    //    if(!IsInvoking("SlowDownOrb") && curTimeStopCooldown <= 0)
    //    {
    //        curTimeStopDur = slowDur;
    //        Invoke("SlowDownOrb", 0);
    //        InvokeRepeating("SlowDownTime", 0, slowDelay);
    //        StartCoroutine(SlowDurationTimer());
    //    }
    //}

    private void PlayerEnhance()
    {
        if (!IsInvoking("Enhance") && curPlayerEnchanceCooldown <= 0)
        {
            ParticleSystem.MainModule speed = GameManager.instance.particleMan.speedMode.main;
            curPlayerEnhanceDur = enhanceTime;
            Invoke("Enhance",0f);
            speed.duration = enhanceTime;
            GameManager.instance.particleMan.speedMode.Play();
            StartCoroutine(EnhanceTimer());
        }
    }

    public void PlayerHeal()
    {
        if (!IsInvoking("Heal") && curHealCooldown <= 0 && GetComponent<PlayerMovement>().curplayerHp < GetComponent<PlayerMovement>().playerHp)
        {
            mathHeal = healDur * healAmount;
            GameManager.instance.particleMan.speedMode.Stop();
            ParticleSystem.MainModule heal = GameManager.instance.particleMan.heal.main;
            curHealDur = healDur;
            heal.duration = healDur;
            InvokeRepeating("Heal", 0f, healDelay);
            GameManager.instance.particleMan.heal.Play();
            StartCoroutine(HealTimer());
        }
    }

    public void Enhance()
    {
            GetComponent<PlayerMovement>().playerAnime.SetFloat("playerMultiplier", playerMultiplier);
            GetComponent<PlayerMovement>().curMovespeed *= playerMultiplier;
    }

    public void Heal()
    {
        GetComponent<PlayerMovement>().curplayerHp += healAmount;
        curAmountHealed += healAmount;
    }

    //public void SlowDownOrb()
    //{
    //    GameManager.instance.particleMan.zaryaOrb.SetActive(true);
    //    Collider[] c = Physics.OverlapSphere(actualModel.transform.position, radius,interactable);
    //    foreach (Collider enemy in c)
    //    {
    //        if(enemy.tag == "Enemy")
    //        {
    //            if (GetComponent<PlayerMovement>().inAir)
    //            {
    //                enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
    //                enemy.GetComponent<Rigidbody>().isKinematic = false;
    //                enemy.GetComponent<Chase>().curState = Chase.State.Falling;
    //                enemy.GetComponent<EnemyInfo>().inAir = true;
    //                enemy.gameObject.transform.position = actualModel.transform.position + actualModel.transform.forward;
    //            }
    //            else
    //            {
    //                enemy.gameObject.transform.position = actualModel.transform.position + actualModel.transform.forward;
    //            }
    //            StartCoroutine(SlowDownTime(enemy.gameObject));
    //        }
    //    }
    //}

    public IEnumerator SlowDownTime(GameObject enemyInRange)
    {
        while (curTimeStopDur > 0)
        {
            yield return new WaitForSeconds(slowDelay);
            enemyInRange.GetComponent<Chase>().curState = Chase.State.SlowingDown;
            if (enemyInRange.GetComponent<EnemyInfo>().curSpeedMultiplier > enemyInRange.GetComponent<EnemyInfo>().minimumSpeedTreshhold)
            {
                enemyInRange.GetComponent<EnemyInfo>().curSpeedMultiplier -= timeTick * Time.deltaTime;
            }
        }
        if(curTimeStopDur <= 0)
        {
            enemyInRange.GetComponent<Chase>().curState = Chase.State.Chase;
        }
    }

    public IEnumerator OrbDurationTimer()
    {
        while (curOrbDur > 0 && GameManager.gameTime == 1)
        {
            curOrbDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curOrbDur <= 0)
            {
                CancelInvoke();
                player.ReturnState();
                curEnhancedDashCooldown = dashCooldown;
            }
        }
    }

    public IEnumerator EnhanceTimer()
    {
        while(GameManager.gameTime != 1)
        {
            yield return null;
        }
        while (curPlayerEnhanceDur > 0)
        {
            curPlayerEnhanceDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curPlayerEnhanceDur <= 0)
            {
                CancelInvoke();
                GetComponent<PlayerMovement>().playerAnime.SetFloat("playerMultiplier", 1);
                GetComponent<PlayerMovement>().curMovespeed = GetComponent<PlayerMovement>().moveSpeed;
                curPlayerEnchanceCooldown = playerEnchanceCooldown;
                GameManager.instance.particleMan.speedMode.Stop();
            }
        }
    }

    public IEnumerator HealTimer()
    {
        while (GameManager.gameTime != 1)
        {
            yield return null;
        }
        while (curHealDur > 0)
        {
            curHealDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curAmountHealed == mathHeal || GetComponent<PlayerMovement>().curplayerHp >= GetComponent<PlayerMovement>().playerHp)
            {
                curHealDur = 0;
                curAmountHealed = 0;
                CancelInvoke();
                GameManager.instance.particleMan.heal.Stop();
                curHealCooldown = healCooldown;
            }
        }
    }

    //public IEnumerator SlowDurationTimer()
    //{
    //    while (curTimeStopDur > 0)
    //    {
    //        curTimeStopDur -= Time.deltaTime;
    //        yield return new WaitForEndOfFrame();
    //        if (curTimeStopDur <= 0)
    //        {
    //            GameManager.instance.particleMan.zaryaOrb.SetActive(false);
    //            CancelInvoke();
    //            GameManager.gameTime = 1f;
    //            curTimeStopCooldown = timeStopCooldown;
    //        }
    //    }
    //}
}
