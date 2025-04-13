using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string[] sceneNames;
    int curSceneIndex;
    Goal[] goals;
    Spawner[] spawners;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        curSceneIndex = Array.FindIndex(sceneNames, x => x.Equals(SceneManager.GetActiveScene().name));
        if (curSceneIndex == -1)
        {
            curSceneIndex = 0;
        }
        StartStage();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartStage();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void StartStage()
    {
        spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        goals = FindObjectsByType<Goal>(FindObjectsSortMode.None);
    }

    public void CheckClear()
    {
        if (Array.TrueForAll(goals, goal => goal.isEntered))
        {
            Debug.Log("Clear");
            GoNextStage();
        }
    }

    public void GoNextStage()
    {
        SceneManager.LoadScene(sceneNames[++curSceneIndex]);
    }

    //public void SpawnPlayer()
    //{
    //    Array.ForEach(spawners, spawn => spawn.Spawn());
    //}
}
