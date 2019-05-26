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
    Text NoCombinationsText;
    [SerializeField]
    GameObject DirectHitText;
    [SerializeField]
    GameObject ClearTheTableText;

    [SerializeField]
    GameObject readyUI;
    [SerializeField]
    Text ready;

    [SerializeField]
    GameObject GameOverPanel;
    [SerializeField]
    Text OnGameOverText;
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
        //DontDestroyOnLoad(gameObject);

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

    public void OnRoundEnd(Combinations result) { StartCoroutine(OnRoundEndCoroutine(result)); }
    IEnumerator OnRoundEndCoroutine(Combinations result)
    {
        // If combinations are wrong
        if (result < Combinations.Placed)
        {
            if (result == Combinations.Blank)
            {
                NoCombinationsText.text = "No cards selected";
            }
            else if (result == Combinations.NotEnoughSelected)
            {
                NoCombinationsText.text = "Please select 2 or more cards";
            }
            else if(result == Combinations.TooManySelected)
            {
                NoCombinationsText.text = "Please select only one card to place";
            }
            else if (result == Combinations.Wrong)
            {
                NoCombinationsText.text = "Invalid combination";
            }

            NoCombinationsText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            NoCombinationsText.gameObject.SetActive(false);
        }

        // If placed
        else if (result == Combinations.Placed)
        {
            yield return new WaitForSeconds(1.0f);
            //Debug.Log("UI function");
            ReadyScreen();
        }

        // If combinations are valid
        else if (result > Combinations.Placed)
        {
            if ( (result == Combinations.PairD || result == Combinations.PairDC) 
                || (result == Combinations.AddD || result == Combinations.AddDC))
            {
                DirectHitText.SetActive(true);
                yield return new WaitForSeconds(2.0f);
                DirectHitText.SetActive(false);
                ChangeScore();
            }
            if ( (result == Combinations.PairC || result == Combinations.PairDC)
                || (result == Combinations.AddC || result == Combinations.AddDC))
            {
                ClearTheTableText.SetActive(true);
                yield return new WaitForSeconds(2.0f);
                ClearTheTableText.SetActive(false);
                //ChangeScore();
            }
            yield return new WaitForSeconds(1.0f);
            //Debug.Log("UI function");
            ReadyScreen();
        }
    }

    public void OnGameOver(int i)
    {
        if (i == 1)
        {
            OnGameOverText.text = "Player 1 takes all your gold!";
        }
        else if (i == 2)
        {
            OnGameOverText.text = "Player 2 takes all your gold!";
        }
        else
        {
            OnGameOverText.text = "The battle has ended in a draw!";
        }
        GameOverPanel.SetActive(true);
    }

    public void ReadyScreen()
    {
        if(manager.currentGameState == GameState.P1)
            ready.text = "Player 2";       
        else
            ready.text = "Player 1";
       
        readyUI.SetActive(true);
    }

    public void OnContinueClicked()
    {
        manager.SwitchPlayer();
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
