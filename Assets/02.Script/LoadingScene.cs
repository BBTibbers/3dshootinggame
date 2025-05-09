using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{

    public int NextSceneIndex = 2;

    public Slider ProgressSlider;

    public TextMeshProUGUI ProgressText;

    private void Start()
    {
        StartCoroutine(LoadNextScene_Coroutine());
    }

    private IEnumerator LoadNextScene_Coroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(NextSceneIndex);
        ao.allowSceneActivation = false;

        while (ao.isDone == false)
        {
            ProgressSlider.value = ao.progress;
            ProgressText.text = (ao.progress * 100).ToString("F0") + "%";
            if (ao.progress >= 0.9f)
            {
                ProgressText.text = "Press any key to continue...";
                if (Input.anyKeyDown)
                {
                    ao.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
