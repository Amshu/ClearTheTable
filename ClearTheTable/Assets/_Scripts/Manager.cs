using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : MonoBehaviour
{
    public static Manager instance = null;

    [SerializeField]
    Dealer dealer;

    public GameState currentGameState { get; set; }
    public GameObject camPivot;
    bool switching = false;
    public float timeTakenDuringLerp = 3.0f;

    [SerializeField]
    public List<Card> Selected = new List<Card>();
    [SerializeField]
    List<Card> Face = new List<Card>();
    [SerializeField]
    List<Card> P1Pile = new List<Card>();
    [SerializeField]
    List<Card> P2Pile = new List<Card>();

    [SerializeField]
    public int p1Score { get; set;}
    [SerializeField]
    public int p2Score { get; set; }
    public int RoundNo { get; set; }

    void Awake()
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

        // Setup
        currentGameState = GameState.P1;
        StartCoroutine(CamSwitch());
        p1Score = 0;
        p2Score = 0;
        RoundNo = 1;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentGameState != GameState.GameOver)
        {
            RaycastCheck();
        }
    }

    // Fuction to raycast from the screen to world
    void RaycastCheck()
    {
        RaycastHit hit;
        Ray ray = camPivot.GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                //Debug.Log(hit.collider.name + " was hit.");

                // If the line hits the wall
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Card"))
                {
                    //Debug.Log(hit.collider.GetComponent<Card>().status);
                    //Debug.Log(hit.collider.GetComponent<Card>().cardValue);

                    // Debug raycast to check
                    Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

                    onCardSelect(hit.collider.gameObject.GetComponent<Card>());
                }
            }
        }
    }

    // When clicked on card
    void onCardSelect(Card card)
    {
        if (card.status == GameState.Face || card.status == currentGameState)
        {
            if(Selected.Contains(card.GetComponent<Card>()))
            {
                //card.isSelected = false;
                card.GetComponent<Outline>().enabled = false;
                // Remove from selected List
                Selected.Remove(card);
                // Audio Cue
                SoundManager.instance.PlayClip(2);
            }
            else
            {
                //card.isSelected = true;
                card.GetComponent<Outline>().enabled = true;
                // Add to selected list
                Selected.Add(card);
                // Audio Cue
                SoundManager.instance.PlayClip(1);
            }
        }
    }

    // When Match is clicked
    public void onMatch()
    {
        Combinations result = CheckCombinations();
        Debug.Log(result + "-----------");
        // If wrong combination
        if (result <= Combinations.Placed)
        {
            DealerUI.instance.OnRoundEnd(result);
            // Deselect the cards
            foreach (Card card in Selected)
            {
                card.GetComponent<Outline>().enabled = false;
            }
            Selected.Clear();
        }

        //Debug.Log(result);
        else if (result > Combinations.Placed)
        {
            // Direct Hits
            if ((result == Combinations.PairD || result == Combinations.PairDC)
                || (result == Combinations.AddD || result == Combinations.AddDC))
            {
                if (currentGameState == GameState.P1)
                    p1Score++;
                else
                    p2Score++;
            }
            // Clear the table
            if ((result == Combinations.PairC || result == Combinations.PairDC)
                || (result == Combinations.AddC || result == Combinations.AddDC))
            {
                if (currentGameState == GameState.P1)
                    p1Score++;
                else
                    p2Score++;
            }
            StartCoroutine(AfterMatch());
            DealerUI.instance.OnRoundEnd(result);
        }
        
    }

    IEnumerator AfterMatch()
    {
        yield return new WaitForSeconds(1.2f);

        if (currentGameState == GameState.P1)
        {
            // Change status and add to pile
            foreach (Card card in Selected)
            {
                if (card.status == GameState.Face)
                {
                    Face.Remove(card);
                }
                card.GetComponent<Outline>().enabled = false;
                card.status = GameState.P1Pile;
                P1Pile.Add(card);   
            }
        }
        else
        {
            // Change status and add to pile
            foreach (Card card in Selected)
            {
                if (card.status == GameState.Face)
                {
                    Face.Remove(card);
                }
                card.GetComponent<Outline>().enabled = false;
                card.status = GameState.P2Pile;
                P2Pile.Add(card);
            }
        }
        Selected.Clear();
    }

    Combinations CheckCombinations()
    {
        // If the size is 2 then its a same card or direct hit
        if(Selected.Count == 2)
        {
            return (IsPair());
        }
        // If not try for an additive combination
        else if (Selected.Count > 2)
        {
            return (IsAdditive());
        }
        return Combinations.NotEnoughSelected;
    }

    Combinations IsPair()
    {
        Combinations result = Combinations.Blank;
        // Check if card numbers is similar
        if (Selected[0].cardValue.x == Selected[1].cardValue.x)
        {
            dealer.addToPile();
            result = Combinations.Pair;
            if (IsDirectHit())
            {
                result = Combinations.PairD;
            }
            if (IsClearTheTable())
            {
                if (result == Combinations.PairD)
                    result = Combinations.PairDC;
                else
                    result = Combinations.PairC;
                    
            }
            return result;
        }
        return Combinations.Wrong;
    }

    Combinations IsAdditive()
    {
        Combinations result = Combinations.Blank;
        int playerCardCheck = 0;
        int checkCard = -1;
        int check = 0;

        foreach (Card card in Selected)
        {
            if(card.cardValue.x < 11)
            {
                // Find the players card
                if (card.status == currentGameState)
                {
                    playerCardCheck++;
                    // If there are more than 1 hand cards
                    if (playerCardCheck >= 2)
                    {
                        return Combinations.Wrong;
                    }
                    checkCard = (int)card.cardValue.x;
                }
                else
                {
                    // Make sure its not a face card
                    check += (int)card.cardValue.x;
                }
            }
        }
        //Debug.Log(check);
        //Debug.Log(checkCard.cardValue.x);
        // Check if player card adds up to the other cards
        if (checkCard == check)
        {
            dealer.addToPile();
            result = Combinations.Add;
            if (IsDirectHit())
            {
                result = Combinations.AddD;
            }
            if (IsClearTheTable())
            {
                if(result == Combinations.AddD)
                {
                    result = Combinations.AddDC;
                }
                else
                {
                    result = Combinations.AddC;
                }
            }
            return result;
        }
        return Combinations.Wrong;
    }

    bool IsDirectHit()
    {
        foreach(Card card in Selected)
        {
            if(card.status == GameState.Face && card.Armed)
            {
                return true;
            }
        }
        return false;
    }

    bool IsClearTheTable()
    {
        if (dealer.CheckClearTheTable())
            return true;

        return false;
    }

    public void Place()
    {
        // If only one card is selected and its from the players hand
        if(Selected.Count == 1 && Selected[0].status == currentGameState)
        {
            dealer.placeOnTable();
            StartCoroutine(AfterPlaced());
            DealerUI.instance.OnRoundEnd(Combinations.Placed);
        }
        else if(Selected.Count > 1)
        {
            DealerUI.instance.OnRoundEnd(Combinations.TooManySelected);
        }
        else
        {
            DealerUI.instance.OnRoundEnd(Combinations.Blank);
        }
        
    }
    IEnumerator AfterPlaced()
    {
        yield return new WaitForSeconds(0.05f);

        Selected[0].Armed = true;
        Selected[0].ArmedBy = currentGameState;
        Selected[0].WhenArmed = RoundNo;
        Selected[0].status = GameState.Face;

        Selected[0].GetComponent<Outline>().enabled = false;

        Face.Add(Selected[0]);
        Selected.Clear();
        //SwitchPlayer();
    }

    // To switch player
    public void SwitchPlayer()
    {
        RoundNo++;

        CheckForArmed();

        dealer.CheckEmptyHand();

        Selected.Clear();
        if (currentGameState == GameState.P1)
            currentGameState = GameState.P2;
        else
            currentGameState = GameState.P1;
        StartCoroutine(CamSwitch());
    }

    // To change camera
    IEnumerator CamSwitch()
    {
        //Debug.Log(RoundNo);
        //yield return new WaitForSeconds(1.0f);

        Quaternion target;
        //Debug.Log(currentGameState);

        if (currentGameState == GameState.P1)
        {
            //Debug.Log("P1 Rotation");
            target = Quaternion.identity;
        }
        else
        {
            //Debug.Log("P2 Rotation");
            target = new Quaternion(0, -180, 0, 1);
        }

        camPivot.transform.rotation = target;

        yield return null;

        /*
        switching = true;

        float _timeStartedLerping = Time.time;
        while (switching)
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            Debug.Log("Current Rotation" + camPivot.transform.rotation);
            Debug.Log("Target Rotation" + target);

            Quaternion currRotation = camPivot.transform.rotation;

            camPivot.transform.rotation = Quaternion.Lerp(currRotation, target, percentageComplete);

            if ((int)camPivot.transform.rotation.y == (int)target.y)
            {
                switching = false;
                Debug.Log("-------------Stop Switching");
                Debug.Log("Current Rotation" + camPivot.transform.rotation);
                Debug.Log("Target Rotation" + target);
            }

            yield return new WaitForSeconds(0.01f);
        }
        */
    }

    void CheckForArmed()
    {
        foreach(Card card in Face)
        {
            if(RoundNo - card.WhenArmed > 1)
            {
                card.Armed = false;
            }
        }
    }

    public void OnGameOver()
    {
        currentGameState = GameState.GameOver;

        if(P1Pile.Count > P2Pile.Count)
        {
            p1Score++;
        }
        if (P1Pile.Count < P2Pile.Count)
        {
            p2Score++;
        }

        if (p1Score > p2Score)
        {
            DealerUI.instance.OnGameOver(1);
        }
        else if(p1Score < p2Score)
        {
            DealerUI.instance.OnGameOver(2);
        }
        else
        {
            DealerUI.instance.OnGameOver(0);
        }
    }
}
