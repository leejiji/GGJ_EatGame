using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAnimation : MonoBehaviour
{
    public void AnimationPlay(Animator animator)
    {
        string ButtonName = EventSystem.current.currentSelectedGameObject.name;

        if (ButtonName != "PrevBtn")
        {
            animator.Play(ButtonName, -1, 0f);
        }
        else
            animator.Play("MainAni", -1, 0f);
    }
}
