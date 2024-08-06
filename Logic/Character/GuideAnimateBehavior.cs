using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class GuideAnimateBehavior : StateMachineBehaviour
    {
        
        public int[] arrIdleStateId = { 1, 2, 3, 4 };
        private static readonly int IdleState = Animator.StringToHash("IdleState");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (arrIdleStateId.Length <= 0)
            {
                animator.SetInteger(IdleState, 0);
            }
            else
            {
                int index = Random.Range(0, arrIdleStateId.Length);
                animator.SetInteger(IdleState, arrIdleStateId[index]);
            }
        }
        
    }
}