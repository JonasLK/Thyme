using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float gameTime = 1f;
    [Header("Mode")]
    public GameMode controlMode;

    [Header("Managers")]
    public SoundMan soundMan;
    public Waves waveMan;
    public PointManager pointMan;
    public NewUiManager uiMan;
    public ParticleMan particleMan;
    public PillarMan pillarMan;

    [Header("Extra")]
    public OrbScale timeOrb;
    public CamShake shake;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static bool IsPlaying(Animator anim,int layer,string tag)
    {
        if(anim.GetCurrentAnimatorStateInfo(layer).IsTag(tag))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Update()
    {
        if(gameTime < 0)
        {
            gameTime = 0;
        }
    }
}

public enum GameMode
{
    controller,
    pc
}
