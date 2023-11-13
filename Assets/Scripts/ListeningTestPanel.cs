using UnityEngine;

[RequireComponent(typeof(WarmUpController))]
public class ListeningTestPanel : MonoBehaviour
{
    public Transform ListeningTestIntroduction;
    public Transform ListeningTest;
    public Transform Ending1;
    public Transform Ending2;
    public Transform WarmUp1;
    public Transform WarmUp2;
    public Transform WarmUp3;
    public Transform WarmUp4;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        DisableAll();
        ListeningTestIntroduction.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisableAll()
    {
        ListeningTestIntroduction.gameObject.SetActive(false);
        ListeningTest.gameObject.SetActive(false);
        Ending1.gameObject.SetActive(false);
        Ending2.gameObject.SetActive(false);
        WarmUp1.gameObject.SetActive(false);
        WarmUp2.gameObject.SetActive(false);
        WarmUp3.gameObject.SetActive(false);
        WarmUp4.gameObject.SetActive(false);
    }

    public void SetWarmup(int i)
    {
        DisableAll();

        switch (i) 
        {
            case 0:
                WarmUp1.gameObject.SetActive(true);
                break;
            case 1:
                WarmUp2.gameObject.SetActive(true);
                break;
            case 2:
                WarmUp3.gameObject.SetActive(true);
                break;
            case 3:
                WarmUp4.gameObject.SetActive(true);
                break;
        }
    }

    public void ShowListeningTestInfo()
    {
        DisableAll();
        ListeningTest.gameObject.SetActive(true);
    }

    public void ShowListeningTestIntroduction()
    {
        DisableAll();
        ListeningTestIntroduction.gameObject.SetActive(true);
    }

    public void ShowEnding()
    {
        DisableAll();
        Ending1.gameObject.SetActive(true);
    }

    public void ShowEnding2()
    {
        DisableAll();
        Ending2.gameObject.SetActive(true);
    }
}
