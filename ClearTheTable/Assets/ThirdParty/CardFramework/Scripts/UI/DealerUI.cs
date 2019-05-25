using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DealerUI : MonoBehaviour 
{
	private Dealer _dealer;

    //---------------------Amshu----------------------//
    public static DealerUI instance = null;

    [SerializeField]
    Text p1Score;
    [SerializeField]
    Text p2Score;

    [SerializeField]
    GameObject matchScreen;
    [SerializeField]
    GameObject noMatch;
    [SerializeField]
    GameObject DirectHitText;
    [SerializeField]
    GameObject readyUI;
    [SerializeField]
    Text ready;
    //------------------------------------------------//

	//public Text FaceValueText { get { return _faceValueText; } }
	//[SerializeField]
	//private Text _faceValueText;

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
	}


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


    public void ChangeScore()
    {
        if (Manager.instance.currentGameState == GameState.P1)
            p1Score.text = Manager.instance.p1Score.ToString();
        else
            p2Score.text = Manager.instance.p2Score.ToString();
    }

    public IEnumerator OnMatch(int i)
    {
        if(i == 0)
        {
            DirectHitText.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            DirectHitText.SetActive(false);
            ChangeScore();
        }
        DealerUI.instance.ReadyScreen();
    }

    public IEnumerator NoMatch()
    {
        noMatch.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        noMatch.SetActive(false);
    }

    public void ReadyScreen()
    {
        if(Manager.instance.currentGameState == GameState.P1)
            ready.text = "Player 2";       
        else
            ready.text = "Player 1";
       
        readyUI.SetActive(true);
    }
}
