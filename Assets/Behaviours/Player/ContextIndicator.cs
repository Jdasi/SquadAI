using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject waypoint_indicator;
    [SerializeField] GameObject cover_indicator;
    [SerializeField] GameObject attack_indicator;
    [SerializeField] GameObject hack_indicator;


    public void ChangeIndicator(ContextType _type)
    {
        switch (_type)
        {
            case ContextType.NONE: SwitchToIndicator(null); break;
            case ContextType.FLOOR: SwitchToIndicator(waypoint_indicator); break;
            case ContextType.COVER: SwitchToIndicator(cover_indicator); break;
            case ContextType.ATTACK: SwitchToIndicator(attack_indicator); break;
            case ContextType.HACK: SwitchToIndicator(hack_indicator); break;

            default: SwitchToIndicator(null); break;
        }
    }


    public void SetScreenPosition(Vector3 _position)
    {
        attack_indicator.transform.position = _position;
        hack_indicator.transform.position = _position;
    }


    void SwitchToIndicator(GameObject _indicator)
    {
        SetIndicatorActive(waypoint_indicator, false);
        SetIndicatorActive(cover_indicator, false);
        SetIndicatorActive(attack_indicator, false);
        SetIndicatorActive(hack_indicator, false);

        if (_indicator != null)
            _indicator.SetActive(true);
    }


    void SetIndicatorActive(GameObject _indicator, bool _active)
    {
        if (_indicator == null)
            return;

        _indicator.SetActive(_active);
    }

}
