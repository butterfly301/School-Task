using System.Collections;
using UnityEngine;

public class ScreenFade : MonoBehaviour
{
    
    public Animator animator;
    public IEnumerator FadeAnimation(float duration)
    {
        animator.SetBool("FadeIn", true);
        animator.SetBool("FadeOut", false);
        yield return new WaitForSeconds(duration);
        animator.SetBool("FadeIn", false);
        animator.SetBool("FadeOut", true);
    }
}
