using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadControl : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] List<Squaddie> squaddies;

    [Header("References")]
    [SerializeField] ContextScanner context_scanner;


    void Start()
    {

    }


    void Update()
    {
        if (Input.GetButtonDown("Command"))
            IssueContextCommand();
    }


    void IssueContextCommand()
    {
        switch (context_scanner.current_context)
        {
            case ContextType.FLOOR:
            {
                IssueWaypointCommand();
            } break;

            case ContextType.COVER:
            {
                IssueCoverCommand();
            } break;
        }
    }


    void IssueWaypointCommand()
    {
        foreach (Squaddie squaddie in squaddies)
        {
            squaddie.IssueWaypoint(context_scanner.indicator_position);
        }
    }


    void IssueCoverCommand()
    {

    }

}
