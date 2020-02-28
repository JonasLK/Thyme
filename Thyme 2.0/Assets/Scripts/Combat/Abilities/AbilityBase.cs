using System;
using System.Collections;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    public enum AbilityName
    {
        OrbDrain = 0,
        TimeStop = 1,
        PlayerEnhance = 2
    }

    [Header("Ability")]
    public AbilityName curAbility;

    [Header("AbilityProperties")]
    public LayerMask interactable;
    private PlayerMovement player;
    private float curRange;
    RaycastHit hit;

    [Header("Orb")]
    public float orbCooldown;
    public float orbRadius;
    public float orbDelay;
    [Range(1f, 10f)]
    public float orbDur = 5f;
    [Range(1f, 10f)]
    public float orbDam = 1f;

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

    [HideInInspector]
    public float curTimeStopCooldown;
    [HideInInspector]
    public float curPlayerEnchanceCooldown;
    [HideInInspector]
    public float curOrbCooldown;
    [HideInInspector]
    public float curTimeStopDur;
    [HideInInspector]
    public float curPlayerEnhanceDur;
    [HideInInspector]
    public float curOrbDur;

    [Header("Debugging")]
    public GameObject currentHitObject;
    public Transform handModel;
    private GameObject actualModel;
    public Color timeStop;
    public Color enhance;
    public Color orb;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        actualModel = player.actualPlayer;
    }

    public void Update()
    {
        if (curTimeStopCooldown > 0)
        {
            curTimeStopCooldown -= Time.deltaTime;
        }

        if (curOrbCooldown > 0)
        {
            curOrbCooldown -= Time.deltaTime;
        }

        if (curPlayerEnchanceCooldown > 0)
        {
            curPlayerEnchanceCooldown -= Time.deltaTime;
        }

        if (Input.GetButtonDown("NextAbility"))
        {
            if ((int)curAbility == (int)AbilityName.PlayerEnhance)
            {
                curAbility = (AbilityName)0;
            }
            else
            {
                curAbility++;
            }
        }
        switch (curAbility)
        {
            case AbilityName.OrbDrain:
                actualModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = orb;
                break;
            case AbilityName.TimeStop:
                actualModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = timeStop;
                break;
            case AbilityName.PlayerEnhance:
                actualModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = enhance;
                break;
        }
        if (Input.GetButtonDown("Fire3"))
        {
            switch (curAbility)
            {
                case AbilityName.OrbDrain:
                    OrbDrain();
                    break;
                case AbilityName.TimeStop:
                    TimeStop();
                    break;
                case AbilityName.PlayerEnhance:
                    PlayerEnhance();
                    break;
            }
        }
    }

    private void PlayerEnhance()
    {
        if (!IsInvoking() && curPlayerEnchanceCooldown <= 0)
        {
            curPlayerEnhanceDur = enhanceTime;
            Invoke("Enhance",0f);
            StartCoroutine(EnhanceTimer());
        }
    }

    public void Enhance()
    {
            GetComponent<PlayerMovement>().playerAnime.speed *= playerMultiplier;
            GetComponent<PlayerMovement>().curMovespeed *= playerMultiplier;
    }

    public void TimeStop()
    {
        if(!IsInvoking() && curTimeStopCooldown <= 0)
        {
            curTimeStopDur = slowDur;
            Invoke("SlowDownOrb", 0);
            InvokeRepeating("SlowDownTime", 0, slowDelay);
            StartCoroutine(SlowDurationTimer());
        }
    }

    public void SlowDownOrb()
    {
        Collider[] c = Physics.OverlapSphere(actualModel.transform.position, radius,interactable);
        foreach (Collider enemy in c)
        {
            if(enemy.tag == "Enemy")
            {
                if (GetComponent<PlayerMovement>().inAir)
                {
                    enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    enemy.GetComponent<Rigidbody>().isKinematic = false;
                    enemy.GetComponent<Rigidbody>().useGravity = false;
                    enemy.GetComponent<Chase>().curState = Chase.State.Falling;
                    enemy.GetComponent<EnemyInfo>().inAir = true;
                    enemy.gameObject.transform.position = actualModel.transform.position + actualModel.transform.forward;
                }
                else
                {
                    enemy.gameObject.transform.position = actualModel.transform.position + actualModel.transform.forward;
                }
            }
        }
    }

    public void SlowDownTime()
    {
        //Invoke Method
        if(GameManager.gameTime > 0.1f)
        {
            GameManager.gameTime -= timeTick * Time.deltaTime;
        }
    }

    public void OrbDrain()
    {
        if (!IsInvoking() && curOrbDur <= 0)
        {
            curOrbDur = orbDur;
            StartCoroutine(OrbDurationTimer());
            player.SetAbility();
            if (curOrbDur > 0)
            {
                InvokeRepeating("CheckOrb", 0, orbDelay);
            }
        }
    }

    public void CheckOrb()
    {
        //Invoke Method
        Collider[] enemies = Physics.OverlapSphere(handModel.transform.position, orbRadius, interactable);
        foreach (Collider enemy in enemies)
        {
            if (enemy.tag == "Enemy")
            {
                enemy.GetComponent<EnemyInfo>().AdjustHealth(orbDam,false);
                Debug.Log(enemy.name + "Hit");
            }
        }
    }

    public IEnumerator OrbDurationTimer()
    {
        while (curOrbDur > 0)
        {
            curOrbDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curOrbDur <= 0)
            {
                CancelInvoke();
                player.ReturnState();
                curOrbCooldown = orbCooldown;
            }
        }
    }

    public IEnumerator EnhanceTimer()
    {
        while (curPlayerEnhanceDur > 0)
        {
            curPlayerEnhanceDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curPlayerEnhanceDur <= 0)
            {
                CancelInvoke();
                GetComponent<PlayerMovement>().playerAnime.speed = 1;
                GetComponent<PlayerMovement>().curMovespeed = GetComponent<PlayerMovement>().moveSpeed;
                curPlayerEnchanceCooldown = playerEnchanceCooldown;
            }
        }
    }

    public IEnumerator SlowDurationTimer()
    {
        while (curTimeStopDur > 0)
        {
            curTimeStopDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curTimeStopDur <= 0)
            {
                CancelInvoke();
                GameManager.gameTime = 1f;
                curTimeStopCooldown = timeStopCooldown;
            }
        }
    }
}
