using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class State : ScriptableObject
{
    public string display_text;
    public Action[] actions;
    public Transition[] transitions;
    public Color state_color = Color.grey;


    public void UpdateState(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.state_time_elapsed += Time.deltaTime;

        DoActions(_squaddie);
        CheckTransitions(_squaddie);

        _squaddie.knowledge.prev_state_time_elapsed =
            _squaddie.knowledge.state_time_elapsed;
    }


    void DoActions(SquaddieAI _squaddie)
    {
        foreach (Action action in actions)
        {
            if (!action.PreconditionsMet(_squaddie))
                continue;

            action.Act(_squaddie);
        }
    }


    void CheckTransitions(SquaddieAI _squaddie)
    {
        foreach (Transition transition in transitions) 
        {
            bool decision_success = transition.decision.Decide(_squaddie);
            State target_state = decision_success ? transition.true_state : transition.false_state;

            if (target_state == null)
                continue;

            TransitionTrigger[] triggers = decision_success ?
                transition.true_triggers : transition.false_triggers;

            foreach (TransitionTrigger trigger in triggers)
                trigger.Trigger(_squaddie);

            _squaddie.TransitionToState(target_state);
        }
    }

}
