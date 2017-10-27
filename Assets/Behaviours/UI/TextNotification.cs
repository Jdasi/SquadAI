using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextNotification : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float rise_speed;
    [SerializeField] float duration;

    [Header("References")]
    [SerializeField] Text notification_text;
    [SerializeField] FadableGraphic fade;


    public void Init(Vector3 _pos, float _rise_speed, float _duration)
    {
        fade.Init();

        transform.position = _pos;
        rise_speed = _rise_speed;
        duration = _duration;
    }


    public void SetNotificationText(string _text, Color _color)
    {
        notification_text.text = _text;
        notification_text.color = _color;
    }


    void Start()
    {
        StartCoroutine(NotificationRoutine());
    }


    void Update()
    {
        transform.position += Vector3.up * rise_speed * Time.deltaTime;
    }


    IEnumerator NotificationRoutine()
    {
        Destroy(this.gameObject, duration);

        yield return new WaitForSeconds(duration / 2);

        fade.FadeOut(duration / 2);
    }

}
