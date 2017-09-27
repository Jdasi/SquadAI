using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad
{
    public int num_squaddies { get { return squaddies.Count; } }
    public SquadSettings settings;

    private List<Squaddie> squaddies = new List<Squaddie>();


    public Squad(SquadSettings _settings)
    {
        settings = _settings;
    }


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


    public void RemoveSquaddie()
    {
        if (num_squaddies == 0)
            return;

        Object.Destroy(squaddies[squaddies.Count - 1].gameObject);
        squaddies.Remove(squaddies[squaddies.Count - 1]);
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
        float padded_squaddie = settings.squaddie_size + settings.squaddie_spacing;
        float line_width = padded_squaddie * num_squaddies;

        for (int i = 0; i < num_squaddies; ++i)
        {
            Vector3 waypoint = _command.target + (_command.indicator_right * (padded_squaddie * i));
            waypoint -= _command.indicator_right * ((line_width - padded_squaddie) / 2);

            squaddies[i].IssueWaypoint(waypoint);
        }
    }


    void IssueCoverCommand(ContextCommand _command)
    {

    }

}
