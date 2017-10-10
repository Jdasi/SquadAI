using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;
    public Color state_color = Color.grey;


    public void UpdateState(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.state_time_elapsed += Time.deltaTime;

        DoActions(_squaddie);
        CheckTransitions(_squaddie);
    }


    void DoActions(SquaddieAI _squaddie)
    {
        foreach (Action action in actions)
        {
            action.Act(_squaddie);
        }
    }


    void CheckTransitions(SquaddieAI _squaddie)
    {
        foreach (Transition transition in transitions) 
        {
            bool decisionSucceeded = transition.decision.Decide(_squaddie);

            State target_state = decisionSucceeded ? transition.true_state : transition.false_state;
            _squaddie.TransitionToState(target_state);
        }
    }

}
