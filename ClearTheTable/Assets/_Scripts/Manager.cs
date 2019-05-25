using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : MonoBehaviour
{
    public static Manager instance = null;

    public GameState currentGameState { get; set; }
    public GameObject camPivot;
    bool switching = false;
    public float timeTakenDuringLerp = 3.0f;

    [SerializeField]
    List<Card> Selected = new List<Card>();
    [SerializeField]
    List<Card> P1Pile = new List<Card>();
    [SerializeField]
    List<Card> P2Pile = new List<Card>();

    [SerializeField]
    public int p1Score { get; set;}
    [SerializeField]
    public int p2Score { get; set; }

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
        DontDestroyOnLoad(gameObject);

        // Setup
        currentGameState = GameState.P1;

        p1Score = 0;
        p2Score = 0;

        DealerUI.instance.ChangeScore();
    }


    public void SwitchPlayer()
    {
        Selected.Clear();
        if (currentGameState == GameState.P1)
            currentGameState = GameState.P2;
        else
            currentGameState = GameState.P1;
    }

    IEnumerator CamSwitch()
    {
        Quaternion target = Quaternion.identity;
        if (currentGameState != GameState.P1)
            target = new Quaternion(0, -180, 0, 1);
        switching = true;

        float _timeStartedLerping = Time.time;
        while (switching)
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            Debug.Log("Checking");
            Quaternion currRotation = camPivot.transform.rotation;
            camPivot.transform.rotation = Quaternion.Slerp(currRotation, target, percentageComplete);
            yield return new WaitForEndOfFrame();

            if (camPivot.transform.rotation == target)
            {
                switching = false;
                Debug.Log("Stop Switching");
            }    
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

                    // Debug raycast to check
                    Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

                    onCardSelect(hit.collider.gameObject.GetComponent<Card>());
                }
            }
        }
    }

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
            }
            else
            {
                //card.isSelected = true;
                card.GetComponent<Outline>().enabled = true;
                // Add to selected list
                Selected.Add(card);
            }
        }
    }

    public void onMatch()
    {
        bool IsACombination = false;
        switch (CheckCombination())
        {
            // If there is no match
            case -1:
                DealerUI.instance.NoMatch();
                Selected.Clear();
                foreach (Card card in Selected)
                {
                    card.GetComponent<Outline>().enabled = false;
                }
                break;
            // Incase of direct hit
            case 0:
                DealerUI.instance.OnMatch(0);

                if(currentGameState == GameState.P1)
                {
                    p1Score++;
                    // Change status and add to pile
                    foreach(Card card in Selected)
                    {
                        card.GetComponent<Outline>().enabled = false;
                        card.status = GameState.P1Pile;
                        P1Pile.Add(card);
                    }
                    Selected.Clear();
                    SwitchPlayer();
                }
                else
                {
                    p2Score++;
                    // Change status and add to pile
                    foreach (Card card in Selected)
                    {
                        card.status = GameState.P2Pile;
                        P2Pile.Add(card);
                    }
                    Selected.Clear();
                    SwitchPlayer();
                }
                break;
            // In case of 2 pair
            case 1:

                break;
            // In case of additive
            case 2:

                break;
        }
    }

    int CheckCombination()
    {
        // If the size is 2 then its a same card or direct hit
        if(Selected.Count == 2)
        {
            return (IsDirectHit());
        }
        // If not try for an additive combination
        if (IsAdditive())
        {
            return 2;
        }
        return -1;
    }

    int IsDirectHit()
    {
        // One card must be on hand and another on the table
        if (Selected[0].status != Selected[1].status)
        {
            // Check if card numbers is similar
            if (Selected[0].cardValue.x == Selected[1].cardValue.x)
            {
                // Check if either one is armed
                if (Selected[0].armed || Selected[1].armed)
                {
                    return 0;
                }
                return 1;
            }   
        }
        return -1;
    }

    bool IsAdditive()
    {
        int playerCardCheck = 0;
        Card checkCard = Selected[0];
        int check = 0;

        foreach(Card card in Selected)
        {
            // Find the players card
            if(card.status == currentGameState)
            {
                playerCardCheck++;
                // If there are more than 1 hand cards
                if(playerCardCheck > 1)
                {
                    return false;
                }
                checkCard = card;
            }
            else
            {
                check += (int)card.cardValue.x;
            }
        }

        // Check if player card adds up to the other cards
        if ((int)checkCard.cardValue.x == check)
        {
            return true;
        }

        return false;
    }
}
