using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DealerUI : MonoBehaviour 
{
	private Dealer _dealer;

    //public Text FaceValueText { get { return _faceValueText; } }
    //[SerializeField]
    //private Text _faceValueText;

    public void Shuffle()
    {
        if (_dealer.DealInProgress == 0)
        {
            StartCoroutine(_dealer.ShuffleCoroutine());
        }
    }


    public void Draw()
    {
        if (_dealer.DealInProgress == 0)
        {
            StartCoroutine(_dealer.DrawCoroutine());
        }
    }

    //---------------------Amshu----------------------//
    public static DealerUI instance = null;

    Manager manager;

    [SerializeField]
    Text p1Score;
    [SerializeField]
    Text p2Score;

    //[SerializeField]
    //GameObject matchScreen;
    [SerializeField]
    GameObject noMatch;
    [SerializeField]
    GameObject DirectHitText;
    [SerializeField]
    GameObject readyUI;
    [SerializeField]
    Text ready;
    //------------------------------------------------//

	private void Awake()
	{
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        _dealer = GameObject.Find("Dealer").GetComponent<Dealer>();
		_dealer.DealerUIInstance = this;

        p1Score.text = "0";
        p2Score.text = "0";

        manager = Manager.instance;
    }

    public void ChangeScore()
    {
        if (manager.currentGameState == GameState.P1)
            p1Score.text = manager.p1Score.ToString();
        else
            p2Score.text = manager.p2Score.ToString();
    }

    public void OnRoundEnd() { StartCoroutine(OnMatchCoroutine()); }
    IEnumerator OnMatchCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log("UI function");
        ReadyScreen();
    }

    public void onDirectHit()
    {
        StartCoroutine(OnDirectHit());
    }
    IEnumerator OnDirectHit()
    {
        DirectHitText.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        DirectHitText.SetActive(false);
        ChangeScore();
    }

    public void NoMatch() { StartCoroutine(NoMatchCoroutine()); }
    IEnumerator NoMatchCoroutine()
    {
        noMatch.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        noMatch.SetActive(false);
    }

    public void ReadyScreen()
    {
        if(manager.currentGameState == GameState.P1)
            ready.text = "Player 1";       
        else
            ready.text = "Player 2";
       
        readyUI.SetActive(true);
    }

    public void OnContinueClicked()
    {
        readyUI.gameObject.SetActive(false);
        //manager.
    }

    public void OnMatchClicked()
    {
        manager.onMatch();
    }

    public void onPlaceClicked()
    {
        manager.Place();
    }

    public void onExitClicked()
    {
        SceneManager.LoadScene(0);
    }

    public void onRematchClicked()
    {
        SceneManager.LoadScene(1);
    }
}
