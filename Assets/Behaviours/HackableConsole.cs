using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ConsoleState
{
    IDLE,
    BEING_HACKED,
    HACKED
}

public class HackableConsole : MonoBehaviour
{
    public bool hacked { get { return state == ConsoleState.HACKED; } }

    public ConsoleState state;

    [Header("Parameters")]
    [SerializeField] float time_to_hack;
    [SerializeField] float time_to_reset;
    [SerializeField] Color screen_idle_color;
    [SerializeField] Color screen_hack_color;
    [SerializeField] Color screen_disabled_color;

    [Header("References")]
    public Transform hack_point;
    [SerializeField] GameObject text_panel;
    [SerializeField] Image screen_img;
    [SerializeField] Image hack_progress_img;
    [SerializeField] Text state_text;

    [Header("Notification Settings")]
    [SerializeField] Vector3 notification_spawn;
    [SerializeField] float notification_speed;
    [SerializeField] float notification_duration;
    [SerializeField] GameObject notifcation_prefab;

    private float hack_timer;
    private float hack_progress;


    public void Hack(SquaddieAI _squaddie)
    {
        if (hacked)
            return;

        hack_timer = time_to_reset;
    }


    void Start()
    {
        HandleStateChange(state);
    }


    void Update()
    {
        ConsoleState prev_state = state;
        UpdateState();

        hack_progress_img.fillAmount = hack_progress / time_to_hack;

        if (state != prev_state)
            HandleStateChange(state);
    }


    void UpdateState()
    {
        switch (state)
        {
            case ConsoleState.IDLE: IdleState(); break;
            case ConsoleState.BEING_HACKED: HackingState(); break;
            case ConsoleState.HACKED: HackedState(); break;
        }
    }


    void IdleState()
    {
        if (hack_timer > 0)
            state = ConsoleState.BEING_HACKED;
    }


    void HackingState()
    {
        hack_timer -= Time.deltaTime;
        hack_progress += Time.deltaTime;

        if (hack_timer <= 0)
        {
            state = ConsoleState.IDLE;
            hack_progress = 0;
        }
        else if (hack_progress >= time_to_hack)
        {
            state = ConsoleState.HACKED;
        }
    }


    void HackedState()
    {
        // Crickets ...
    }


    void HandleStateChange(ConsoleState _state)
    {
        state_text.text = _state.ToString();

        switch (_state)
        {
            case ConsoleState.IDLE:
            {
                screen_img.color = screen_idle_color;

                SpawnNotification("HACK CANCELLED", Color.white);
            } break;

            case ConsoleState.BEING_HACKED:
            {
                screen_img.color = screen_hack_color;

                SpawnNotification("HACKING CONSOLE", Color.red);
            } break;

            case ConsoleState.HACKED:
            {
                screen_img.color = screen_disabled_color;
                hack_progress = 0;

                text_panel.SetActive(false);

                SpawnNotification("CONSOLE HACKED", Color.yellow);
            } break;
        }
    }


    void SpawnNotification(string _text, Color _color)
    {
        GameObject clone = Instantiate(notifcation_prefab, notification_spawn, Quaternion.identity);
        TextNotification note = clone.GetComponent<TextNotification>();

        note.Init(transform.position + notification_spawn, notification_speed, notification_duration);
        note.SetNotificationText(_text, _color);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + notification_spawn, 0.5f);
    }

}
