using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject waypoint_indicator;
    [SerializeField] GameObject cover_indicator;


    public void ChangeIndicator(ContextType _type)
    {
        switch (_type)
        {
            case ContextType.NONE:
            {
                waypoint_indicator.SetActive(false);
                cover_indicator.SetActive(false);
            } break;

            case ContextType.FLOOR:
            {
                waypoint_indicator.SetActive(true);
                cover_indicator.SetActive(false);
            } break;

            case ContextType.COVER:
            {
                waypoint_indicator.SetActive(false);
                cover_indicator.SetActive(true);
            } break;
        }
    }

}
