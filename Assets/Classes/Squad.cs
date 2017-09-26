using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad
{
    public int num_squaddies { get { return squaddies.Count; } }

    private List<Squaddie> squaddies = new List<Squaddie>();


    public void Update()
    {
        squaddies.RemoveAll(elem => elem == null);
    }


    public void AddSquaddie(Squaddie _squaddie)
    {
        if (squaddies.Contains(_squaddie))
            return;

        squaddies.Add(_squaddie);
    }


    public void IssueContextCommand(ContextCommand _command)
    {
        switch (_command.type)
        {
            case ContextType.FLOOR:
            {
                IssueMoveCommand(_command);
            } break;

            case ContextType.COVER:
            {
                IssueCoverCommand(_command);
            } break;
        }
    }


    void IssueMoveCommand(ContextCommand _command)
    {
        /* TODO: Determine indidivdual destinations based on squaddie count
         * and squad settings.
         */

        foreach (Squaddie squaddie in squaddies)
            squaddie.IssueWaypoint(_command.target);
    }


    void IssueCoverCommand(ContextCommand _command)
    {

    }

}
