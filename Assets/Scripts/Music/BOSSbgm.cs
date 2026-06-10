using UnityEngine;

public class BOSSbgm : MonoBehaviour
{   public  AudioSource AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public  void PlayBGM()
    {
        AudioSource.Play();
    }
}
