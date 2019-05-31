using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int sceneCount = 5;

    [SerializeField] CanvasGroup _fadePanel;
    int _sceneNum = 0;
    float _time = 0;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadScene();
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > 60.0f)
        {
            _time = 0.0f;
            LoadScene();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _time = 0.0f;
            LoadScene();
        }
    }

    void LoadScene()
    {
        _sceneNum++;
        if (_sceneNum > sceneCount)
        {
            _sceneNum = 1;
        }
        StartCoroutine(LoadSceneCoroutine(_sceneNum));
    }

    IEnumerator LoadSceneCoroutine(int num)
    {
        var unloadId = SceneManager.GetActiveScene().buildIndex;
        int count = 20;

        for (int i = 0; i < count; i++)
        {
            _fadePanel.alpha += 1 / (float)count;
            yield return null;
        }

        var loadAsync = SceneManager.LoadSceneAsync(num);
        while (!loadAsync.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < count; i++)
        {
            _fadePanel.alpha -= 1 / (float)count;
            yield return null;
        }
    }
}
