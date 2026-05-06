using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance;

    [SerializeField] private Animator animator;

    public bool FadeOutFinished { get; private set; }
    public bool FadeInFinished { get; private set; }

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FadeOut()
    {
        FadeOutFinished = false;
        animator.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        FadeInFinished = false;
        animator.SetTrigger("FadeIn");
    }


    //for animator
    public void OnFadeOutEnd()
    {
        FadeOutFinished = true;
        Debug.Log("Fade out finished");
    }

    public void OnFadeInEnd()
    {
        FadeInFinished = true;
        Debug.Log("Fade in finished");
    }
}