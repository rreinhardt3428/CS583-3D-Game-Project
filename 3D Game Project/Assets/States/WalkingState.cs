using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WalkingState : StateMachineBehaviour
{
    // Called when the state is entered
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Player started walking");
    }

    // Called on each frame while in the state
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Custom logic during walking state (optional)
    }

    // Called when the state is exited
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Player stopped walking");
    }
}
