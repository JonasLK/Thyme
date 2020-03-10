﻿using System;
using System.Collections;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    public enum AbilityName
    {
        OrbDrain,
        TimeStop,
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
    public float curOrbCooldown;
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
    public Color timeStop;
    public Color enhance;
    public Color orb;
    public Color heal;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        actualModel = player.actualPlayer;
    }

    public void Update()
    {
        CheckCooldown();
        //Player Color Change
        if (Input.GetButtonDown("NextAbility"))
        {
            if ((int)curAbility == (int)AbilityName.PlayerHeal)
            {
                curAbility = 0;
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
            case AbilityName.PlayerHeal:
                actualModel.GetComponentInChildren<SkinnedMeshRenderer>().material.color = heal;
                break;
        }
        //Actual Ability
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
                case AbilityName.PlayerHeal:
                    PlayerHeal();
                    break;
            }
        }
    }

    public void CheckCooldown()
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

        if (curHealCooldown > 0)
        {
            curHealCooldown -= Time.deltaTime;
        }
    }

    public void OrbDrain()
    {
        if (!IsInvoking("CheckOrb") && curOrbCooldown <= 0)
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

    public void TimeStop()
    {
        if(!IsInvoking("SlowDownOrb") && curTimeStopCooldown <= 0)
        {
            curTimeStopDur = slowDur;
            Invoke("SlowDownOrb", 0);
            //InvokeRepeating("SlowDownTime", 0, slowDelay);
            StartCoroutine(SlowDurationTimer());
        }
    }

    private void PlayerEnhance()
    {
        if (!IsInvoking("Enhance") && curPlayerEnchanceCooldown <= 0)
        {
            GameManager.instance.particleMan.speedMode.Stop();
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
            GetComponent<PlayerMovement>().playerAnime.speed *= playerMultiplier;
            GetComponent<PlayerMovement>().curMovespeed *= playerMultiplier;
    }

    public void Heal()
    {
        GetComponent<PlayerMovement>().curplayerHp += healAmount;
        curAmountHealed += healAmount;
    }

    public void SlowDownOrb()
    {
        GameManager.instance.particleMan.zaryaOrb.SetActive(true);
        Collider[] c = Physics.OverlapSphere(actualModel.transform.position, radius,interactable);
        foreach (Collider enemy in c)
        {
            if(enemy.tag == "Enemy")
            {
                if (GetComponent<PlayerMovement>().inAir)
                {
                    enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    enemy.GetComponent<Rigidbody>().isKinematic = false;
                    enemy.GetComponent<Chase>().curState = Chase.State.Falling;
                    enemy.GetComponent<EnemyInfo>().inAir = true;
                    enemy.gameObject.transform.position = actualModel.transform.position + actualModel.transform.forward;
                }
                else
                {
                    enemy.gameObject.transform.position = actualModel.transform.position + actualModel.transform.forward;
                }
                StartCoroutine(SlowDownTime(enemy.gameObject));
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

    public IEnumerator HealTimer()
    {
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

    public IEnumerator SlowDurationTimer()
    {
        while (curTimeStopDur > 0)
        {
            curTimeStopDur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curTimeStopDur <= 0)
            {
                GameManager.instance.particleMan.zaryaOrb.SetActive(false);
                CancelInvoke();
                GameManager.gameTime = 1f;
                curTimeStopCooldown = timeStopCooldown;
            }
        }
    }
}
