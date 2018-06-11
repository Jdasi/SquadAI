using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static TempSceneRefs scene = new TempSceneRefs();
    public static int squads_to_spawn { get { return instance.squads_to_spawn_; } }

    private static GameManager instance;

    [SerializeField] GameObject game_over_panel;
    [SerializeField] Text game_over_text;
    [SerializeField] Text paused_text;
    [SerializeField] int squads_to_spawn_;

    private bool game_over;


    public void Quit()
    {
        Application.Quit();
    }


    public void RestartScene()
    {
        AudioManager.StopAllSFX();
        SceneManager.LoadScene(0);
    }


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();

        if (Input.GetKeyDown(KeyCode.R))
            RestartScene();

        if (Input.GetKeyDown(KeyCode.P))
        {
            bool paused = Time.timeScale == 1;

            Time.timeScale = paused ? 0 : 1;
            paused_text.gameObject.SetActive(paused);
        }

        GameLoop();
    }


    void GameLoop()
    {
        if (game_over)
            return;

        bool all_consoles_hacked = GameManager.scene.scene_arranger.active_consoles.TrueForAll(elem => elem.hacked);
        if (all_consoles_hacked)
        {
            PlayerWin(true);
        }

        bool no_player_squads = GameManager.scene.player_squad_control.squad_count == 0;
        if (no_player_squads)
        {
            PlayerWin(false);
        }
    }


    void PlayerWin(bool _win)
    {
        game_over = true;
        game_over_panel.SetActive(true);

        game_over_text.text = "You " + (_win ? "Win!" : "Lose");
    }


    void OnLevelWasLoaded(int _level)
    {
        game_over = false;
        game_over_panel.SetActive(false);
    }

}
