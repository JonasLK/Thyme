using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NewUiManager : MonoBehaviour
{
    #region Functional UI
    [Header("Functional UI")]
    public List<TextMeshProUGUI> pylonsChargeText;

    public TextMeshProUGUI playerStatus;
    public TextMeshProUGUI playerHp;

    public TextMeshProUGUI pylonsAmount;
    public TextMeshProUGUI bossHealth;

    public Image playerEnhanceDashCD;
    public Image playerEnhanceCD;
    public Image playerHealCD;

    public Image playerDashBorder;
    public Image playerEnhanceBorder;
    public Image playerHealBorder;

    public TextMeshProUGUI playerEnhanceDashCDTimer;
    public TextMeshProUGUI playerEnhanceCDTimer;
    public TextMeshProUGUI playerHealCDTimer;
   
    public Image playerEnhanceDur;
    public Image playerHealDur;

    public TextMeshProUGUI playerEnhanceDurTimer;
    public TextMeshProUGUI playerHealDurTimer;

    public Image dashIcon;
    public Image enhanceIconCD;
    public Image healIconCD;

    public Image enhanceIconDur;
    public Image healIconDur;

    [Header("GameOverScreen")]
    public GameObject gameOverScreen;
    public GameObject gameOver;
    public GameObject gameWon;

    [Header("MiscColor")]
    public Color enhance;
    public Color dash;
    public Color heal;

    [Header("Misc")]
    public GameObject bossStats;
    public GameObject pauseScreen;
    public GameObject tutorial;
    public GameObject controller;
    public GameObject pC;
    #endregion
    [Header("ScreenFade")]
    public GameObject fadeScreen;
    public float extraFadeTime;

    [Header("HitScreen")]
    public GameObject BloodScreen;
    public float dur, mag;

    PillarMan p;
    AbilityBase playerAbilities;
    BossInfo bInfo;

    private void Start()
    {
        fadeScreen.GetComponent<Animator>().Play("FadeIn");
        if (tutorial.activeSelf == false)
        {
            tutorial.SetActive(true);
        }
        p = GameManager.instance.pillarMan;
        playerAbilities = GameManager.instance.player.GetComponent<AbilityBase>();
        bInfo = GameManager.instance.bInfo;
        for (int i = 0; i < p.pylons.Count; i++)
        {
            pylonsChargeText[i].text = "Remaining: " + Mathf.Round(p.pylons[i].chargeTimer).ToString() + " / " + p.pylons[i].chargeTimerMax.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(tutorial.activeSelf == true)
        {
            CloseTutorial();
            CheckTutorialInput();
            return;
        }
        DebugCheck();
        PylonCharge();
        BossStats();
        PlayerStatusCheck();
        AbilityCDColor();
        AbilityCDText();
        AbilityDurColor();
        AbilityDurText();
        GameOver();
        PauseGame();
    }

    public void CloseTutorial()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            tutorial.SetActive(false);
        }
    }

    private void CheckTutorialInput()
    {
        if(GameManager.instance.controlMode == GameMode.Controller)
        {
            controller.SetActive(true);
            pC.SetActive(false);
        }
        else if (GameManager.instance.controlMode == GameMode.Pc)
        {
            controller.SetActive(false);
            pC.SetActive(true);
        }
    }

    private void DebugCheck()
    {
        if(GameManager.instance.debug)
        {
            playerStatus.gameObject.SetActive(true);
            playerStatus.text = "Current Control: Debug";
        }
        else
        {
            playerStatus.gameObject.SetActive(false);
        }
    }

    private void PylonCharge()
    {
        pylonsChargeText[p.curPylon].text = "Remaining: " + Mathf.Round(p.pylons[p.curPylon].chargeTimer).ToString() + " / " + p.pylons[p.curPylon].chargeTimerMax.ToString();
    }

    private void BossStats()
    {
        if (GameManager.instance.gameStart)
        {
            bossStats.SetActive(true);
            pylonsAmount.text = "Obelisk Amount: " + GameManager.instance.pillarMan.donePylon.Count.ToString() + "/" + GameManager.instance.pillarMan.pylons.Count.ToString();
            if(bInfo.curHealth >= 0)
            {
                bossHealth.text = "Boss Health: " + bInfo.curHealth.ToString();
            }
            else
            {
                bossHealth.text = "Boss Health: 0";
            }
        }
    }

    private void PlayerStatusCheck()
    {
        {
            if (playerAbilities.GetComponent<PlayerMovement>().curState != PlayerMovement.PlayerState.Death || Mathf.Round(playerAbilities.GetComponent<PlayerMovement>().curplayerHp) <= 0)
            {
                playerHp.text = "Current Hp: " + Mathf.Round(playerAbilities.GetComponent<PlayerMovement>().curplayerHp);
            }
            else
            {
                playerHp.text = "Current Hp: 0";
            }
        }

    }

    private void AbilityCDColor()
    {
        playerEnhanceDashCD.color = dash;
        playerEnhanceCD.color = enhance;
        playerHealCD.color = heal;
    }

    private void AbilityCDText()
    {
        if(Mathf.RoundToInt(playerAbilities.curEnhancedDashCooldown) == 0)
        {
            playerEnhanceDashCDTimer.text = "";
            dashIcon.enabled = true;
        }
        else
        {
            playerEnhanceDashCDTimer.text = Mathf.RoundToInt(playerAbilities.curEnhancedDashCooldown).ToString();
            dashIcon.enabled = false;
        }
        if (Mathf.RoundToInt(playerAbilities.curPlayerEnchanceCooldown) == 0)
        {
            playerEnhanceCDTimer.text = "";
            enhanceIconCD.enabled = true;
        }
        else
        {
            playerEnhanceCDTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnchanceCooldown).ToString();
            enhanceIconCD.enabled = false;
        }
        if (Mathf.RoundToInt(playerAbilities.curHealCooldown) == 0)
        {
            playerHealCDTimer.text = "";
            healIconCD.enabled = true;
        }
        else
        {
            playerHealCDTimer.text = Mathf.RoundToInt(playerAbilities.curHealCooldown).ToString();
            healIconCD.enabled = false;
        }
    }

    private void AbilityDurColor()
    {
        playerEnhanceDur.color = enhance;
        playerHealDur.color = heal;
    }

    private void AbilityDurText()
    {
        if (Mathf.RoundToInt(playerAbilities.curPlayerEnhanceDur) == 0)
        {
            playerEnhanceDurTimer.text = "";
            enhanceIconDur.enabled = true;
        }
        else
        {
            playerEnhanceDurTimer.text = Mathf.RoundToInt(playerAbilities.curPlayerEnhanceDur).ToString();
            enhanceIconDur.enabled = false;
        }
        if (Mathf.RoundToInt(playerAbilities.curHealDur) == 0)
        {
            playerHealDurTimer.text = "";
            healIconDur.enabled = true;
        }
        else
        {
            playerHealDurTimer.text = Mathf.RoundToInt(playerAbilities.curHealDur).ToString();
            healIconDur.enabled = false;
        }
    }

    public void BorderReset()
    {
        playerDashBorder.enabled = false;
        playerEnhanceBorder.enabled = false;
        playerHealBorder.enabled = false;
    }

    public void SetBloodScreen()
    {
        BloodScreen.SetActive(true);
        GameManager.instance.shake.StartCoroutine(GameManager.instance.shake.CustomScreenShake(dur,mag));
    }

    public void ResetBloodScreen()
    {
        BloodScreen.SetActive(false);
    }

    private void GameOver()
    {
        if (bInfo != null && playerAbilities.GetComponent<PlayerMovement>().curplayerHp >= 0)
        {
            return;
        }
        ScreenFreeze();
        gameOverScreen.SetActive(true);
        if (bInfo == null)
        {
            gameWon.SetActive(true);
        }
        if (playerAbilities.GetComponent<PlayerMovement>().curplayerHp <= 0)
        {
            gameOver.SetActive(true);
        }
    }

    public void Restart()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartFade()
    {
        Time.timeScale = 1;
        GameManager.gameTime = 1;
        fadeScreen.GetComponent<Animator>().Play("FadeOut");
        Invoke("GoToMenu", fadeScreen.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + extraFadeTime);
    }

    void GoToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void PauseGame()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(pauseScreen.activeSelf == false)
            {
                ScreenFreeze();
                pauseScreen.SetActive(true);
            }
            else
            {
                ResumeGame();
                pauseScreen.SetActive(false);
            }
        }
    }

    private void ScreenFreeze()
    {
        Time.timeScale = 0;
        GameManager.gameTime = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        GameManager.gameTime = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
