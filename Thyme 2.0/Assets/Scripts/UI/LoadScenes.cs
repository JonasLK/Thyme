using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScenes : MonoBehaviour
{
    public GameObject fadeScreen;
    public float extraFadeTime;
    public void LoadScene()
    {
        fadeScreen.GetComponent<Animator>().Play("FadeOut");
        Invoke("LoadGameScene", fadeScreen.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + extraFadeTime);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
