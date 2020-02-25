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
    private float curRange, curDuration;
    RaycastHit hit;

    [Header("Orb")]
    public float orbCooldown;
    public float orbRadius;
    public float orbDelay;
    [Range(1f, 10f)]
    public float orbDur = 5f;
    [Range(1f, 10f)]
    public float orbDam = 1f;

    [Header("TimeStop")]
    public float timeStopCooldown;
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
        if (!IsInvoking())
        {
            Invoke("Enhance",0f);
            Invoke("Normalize",enhanceTime);
        }
    }

    public void Enhance()
    {
        GetComponent<PlayerMovement>().playerAnime.speed *= playerMultiplier;
        GetComponent<PlayerMovement>().curMovespeed *= playerMultiplier;
    }

    public void Normalize()
    {
        GetComponent<PlayerMovement>().playerAnime.speed = 1;
        GetComponent<PlayerMovement>().curMovespeed = GetComponent<PlayerMovement>().moveSpeed;
    }

    public void TimeStop()
    {
        if(!IsInvoking())
        {
            curDuration = slowDur;
            InvokeRepeating("SlowDownTime", 0, slowDelay);
            StartCoroutine(DurationTimer());
        }
    }

    public void SlowDownTime()
    {
        //Invoke Method
        if(GameManager.gameTime > 0.1f)
        {
            GameManager.gameTime -= timeTick * Time.deltaTime;
            print("SLOW MODE");
        }
    }

    public void OrbDrain()
    {
        if (!IsInvoking() && curDuration <= 0)
        {
            curDuration = orbDur;
            StartCoroutine(DurationTimer());
            player.SetAbility();
            if (curDuration > 0)
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

    public IEnumerator DurationTimer()
    {
        while (curDuration > 0)
        {
            curDuration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curDuration < 0)
            {
                CancelInvoke();
                player.ReturnState();
                GameManager.gameTime = 1f;
            }
        }
    }
}
