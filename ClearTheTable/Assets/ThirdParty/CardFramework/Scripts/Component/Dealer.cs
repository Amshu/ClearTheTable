using UnityEngine;
using System.Collections;
using System.IO;

public class Dealer : MonoBehaviour
{
    public DealerUI DealerUIInstance { get; set; }

    [SerializeField]
    private CardDeck _cardDeck;

    // Slots used for shuffling
    [SerializeField, Space]
    private CardSlot _stackCardSlot;
    [SerializeField]
    private CardSlot _discardHoverStackCardSlot;
    [SerializeField]
    private CardSlot _rightHandCardSlot;
    [SerializeField]
    private CardSlot _leftHandCardSlot;

    [SerializeField, Space]
    private CardSlot _pickupCardSlot;
    [SerializeField]
    private CardSlot _discardStackCardSlot;

    // Face Card Slots
    [SerializeField, Space]
    private CardSlot _fc_slot1;
    [SerializeField]
    private CardSlot _fc_slot2;
    [SerializeField]
    private CardSlot _fc_slot3;
    [SerializeField]
    private CardSlot _fc_slot4;
    [SerializeField]
    private CardSlot _fc_slot5;
    [SerializeField]
    private CardSlot _fc_slot6;

    // Player 1 Card Slots
    [SerializeField]
    private CardSlot _p1_slot1;
    [SerializeField]
    private CardSlot _p1_slot2;
    [SerializeField]
    private CardSlot _p1_slot3;
    [SerializeField]
    private CardSlot _p1_slot4;
    [SerializeField]
    private CardSlot _p1_slot5;
    [SerializeField]
    private CardSlot _p1_slot6;

    // Player 2 card Slots
    [SerializeField, Space]
    private CardSlot _p2_slot1;
    [SerializeField]
    private CardSlot _p2_slot2;
    [SerializeField]
    private CardSlot _p2_slot3;
    [SerializeField]
    private CardSlot _p2_slot4;
    [SerializeField]
    private CardSlot _p2_slot5;
    [SerializeField]
    private CardSlot _p2_slot6;

    [SerializeField, Space]
    private CardSlot _p1_discardPile;
    [SerializeField]
    private CardSlot _p2_discardPile;

    [SerializeField, Space]
    private const float CardStackDelay = .01f;

    /// <summary>
    /// Counter which keeps track current dealing movements in progress.
    /// </summary>
    public int DealInProgress { get; set; }

    private void Awake()
    {
        _cardDeck.InstanatiateDeck("cards");
        StartCoroutine(AtStart());
    }

    IEnumerator AtStart()
    {
        StartCoroutine(StackCardRangeOnSlot(0, _cardDeck.CardList.Count, _stackCardSlot));
        yield return new WaitForSeconds(2f);
        StartCoroutine(ShuffleCoroutine());
        yield return new WaitForSeconds(2f);
        StartCoroutine(ShuffleCoroutine());
        yield return new WaitForSeconds(2f);
        StartCoroutine(ShuffleCoroutine());
        yield return new WaitForSeconds(2f);
        StartCoroutine(ShuffleCoroutine());
        yield return new WaitForSeconds(2f);
        StartCoroutine(DrawCoroutine());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AddToTable());
    }

    private void MoveCardSlotToCardSlot(CardSlot sourceCardSlot, CardSlot targerCardSlot)
    {
        Card card;
        while ((card = sourceCardSlot.TopCard()) != null)
        {
            targerCardSlot.AddCard(card);
        }
    }

    private IEnumerator StackCardRangeOnSlot(int start, int end, CardSlot cardSlot)
    {
        DealInProgress++;
        for (int i = start; i < end; ++i)
        {
            cardSlot.AddCard(_cardDeck.CardList[i]);
            yield return new WaitForSeconds(CardStackDelay);
        }
        DealInProgress--;
    }

    /// <summary>
    /// Shuffle Coroutine.
    /// Moves all card to pickupCardSlot. Then shuffles them back
    /// to cardStackSlot.
    /// </summary>
    public IEnumerator ShuffleCoroutine()
    {
        DealInProgress++;
        //DealerUIInstance.FaceValueText.text = "0";
        //Debug.Log("Shuffling");
        //-------------------Amshu-------------------//
        // Change status of all cards
        Card[] cards = GameObject.FindObjectsOfType<Card>();
        foreach(Card card in cards)
        {
            card.status = GameState.Deck;
        }
        // Clear All Lists

        //-------------------------------------------//

        MoveCardSlotToCardSlot(_stackCardSlot, _pickupCardSlot);
        MoveCardSlotToCardSlot(_discardStackCardSlot, _pickupCardSlot);

        // Face Slots
        MoveCardSlotToCardSlot(_fc_slot1, _pickupCardSlot);
        MoveCardSlotToCardSlot(_fc_slot2, _pickupCardSlot);
        MoveCardSlotToCardSlot(_fc_slot3, _pickupCardSlot);
        MoveCardSlotToCardSlot(_fc_slot4, _pickupCardSlot);
        MoveCardSlotToCardSlot(_fc_slot5, _pickupCardSlot);
        MoveCardSlotToCardSlot(_fc_slot6, _pickupCardSlot);
        // Player 1 Slots
        MoveCardSlotToCardSlot(_p1_slot1, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p1_slot2, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p1_slot3, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p1_slot4, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p1_slot5, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p1_slot6, _pickupCardSlot);
        // Player 2 Slots
        MoveCardSlotToCardSlot(_p2_slot1, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p2_slot2, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p2_slot3, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p2_slot4, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p2_slot5, _pickupCardSlot);
        MoveCardSlotToCardSlot(_p2_slot6, _pickupCardSlot);

        yield return new WaitForSeconds(.4f);
        int halfLength = _cardDeck.CardList.Count / 2;
        for (int i = 0; i < halfLength; ++i)
        {
            _leftHandCardSlot.AddCard(_pickupCardSlot.TopCard());
        }
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < halfLength; ++i)
        {
            _rightHandCardSlot.AddCard(_pickupCardSlot.TopCard());
        }
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < _cardDeck.CardList.Count; ++i)
        {
            if (i % 2 == 0)
            {
                _stackCardSlot.AddCard(_rightHandCardSlot.TopCard());
            }
            else
            {
                _stackCardSlot.AddCard(_leftHandCardSlot.TopCard());
            }
            yield return new WaitForSeconds(CardStackDelay);
        }
        DealInProgress--;
    }

    public IEnumerator DrawCoroutine()
    {
        DealInProgress++;

        // First Card
        if (_p1_slot1.AddCard(_stackCardSlot.TopCard()))
        {
            _p1_slot1.TopCard().status = GameState.P1;
            // Add to p1 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_p2_slot1.AddCard(_stackCardSlot.TopCard()))
        {
            _p2_slot1.TopCard().status = GameState.P2;
            // Add to p2 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        // Second Card
        if (_p1_slot2.AddCard(_stackCardSlot.TopCard()))
        {
            _p1_slot2.TopCard().status = GameState.P1;
            // Add to p1 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_p2_slot2.AddCard(_stackCardSlot.TopCard()))
        {
            _p2_slot2.TopCard().status = GameState.P2;
            // Add to p2 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        // Third Card
        if (_p1_slot3.AddCard(_stackCardSlot.TopCard()))
        {
            _p1_slot3.TopCard().status = GameState.P1;
            // Add to p1 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_p2_slot3.AddCard(_stackCardSlot.TopCard()))
        {
            _p2_slot3.TopCard().status = GameState.P2;
            // Add to p2 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        // Fourth Card
        if (_p1_slot4.AddCard(_stackCardSlot.TopCard()))
        {
            _p1_slot4.TopCard().status = GameState.P1;
            // Add to p1 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_p2_slot4.AddCard(_stackCardSlot.TopCard()))
        {
            _p2_slot4.TopCard().status = GameState.P2;
            // Add to p2 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        // Fifth Card
        if (_p1_slot5.AddCard(_stackCardSlot.TopCard()))
        {
            _p1_slot5.TopCard().status = GameState.P1;
            // Add to p1 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_p2_slot5.AddCard(_stackCardSlot.TopCard()))
        {
            _p2_slot5.TopCard().status = GameState.P2;
            // Add to p2 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        // Sixth Card
        if (_p1_slot6.AddCard(_stackCardSlot.TopCard()))
        {
            _p1_slot6.TopCard().status = GameState.P1;
            // Add to p1 list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_p2_slot6.AddCard(_stackCardSlot.TopCard()))
        {
            _p2_slot6.TopCard().status = GameState.P2;
            // Add to p2 list
            yield return new WaitForSeconds(CardStackDelay);
        }

        /*
        if (_fc_slot5.AddCard(_stackCardSlot.TopCard()))
        {
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_fc_slot6.AddCard(_stackCardSlot.TopCard()))
        {
            yield return new WaitForSeconds(CardStackDelay);
        }
        */

        //if (_discardHoverStackCardSlot.AddCard(_p1_slot3.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //if (_discardStackCardSlot.AddCard(_discardHoverStackCardSlot.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //if (_p1_slot3.AddCard(_p2_slot3.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //if (_p2_slot3.AddCard(_p1_slot2.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //if (_p1_slot2.AddCard(_p2_slot2.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //if (_p2_slot2.AddCard(_p1_slot1.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //if (_p1_slot1.AddCard(_p2_slot1.TopCard()))
        //{
        //    yield return new WaitForSeconds(CardStackDelay);
        //}
        //_p2_slot1.AddCard(_stackCardSlot.TopCard());

        //int collectiveFaceValue = _p1_slot1.FaceValue();
        //collectiveFaceValue += _p2_slot2.FaceValue();
        //collectiveFaceValue += _p1_slot2.FaceValue();
        //collectiveFaceValue += _p2_slot3.FaceValue();
        //collectiveFaceValue += _p1_slot3.FaceValue();
        //collectiveFaceValue += _p2_slot1.FaceValue();
        //DealerUIInstance.FaceValueText.text = collectiveFaceValue.ToString();

        DealInProgress--;
    }

    public IEnumerator AddToTable()
    {
        DealInProgress++;

        // Test for Face Card Slots
        if (_fc_slot1.AddCard(_stackCardSlot.TopCard()))
        {
            _fc_slot1.TopCard().status = GameState.Face;
            // Add to face list

            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_fc_slot2.AddCard(_stackCardSlot.TopCard()))
        {
            _fc_slot2.TopCard().status = GameState.Face;
            // Add to face list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_fc_slot3.AddCard(_stackCardSlot.TopCard()))
        {
            _fc_slot3.TopCard().status = GameState.Face;
            // Add to face list
            yield return new WaitForSeconds(CardStackDelay);
        }
        if (_fc_slot4.AddCard(_stackCardSlot.TopCard()))
        {
            _fc_slot4.TopCard().status = GameState.Face;
            // Add to face list
            yield return new WaitForSeconds(CardStackDelay);
        }

        DealInProgress--;
    }

    public void addToPile()
    {
        StartCoroutine(AddToPile());
    }
    public IEnumerator AddToPile()
    {
        DealInProgress++;

        CardSlot targetPile;
        if (Manager.instance.currentGameState == GameState.P1)
            targetPile = _p1_discardPile;
        else
            targetPile = _p2_discardPile;

        foreach(Card card in Manager.instance.Selected)
        {
            targetPile.AddCard(card);
            yield return new WaitForSeconds(CardStackDelay);
        }
        
        DealInProgress--;
    }

    public void placeOnTable() { StartCoroutine(PlaceOnTable()); }
    IEnumerator PlaceOnTable()
    {
        DealInProgress++;

        if(_fc_slot1.TopCard() == null)
        {
            _fc_slot1.AddCard(Manager.instance.Selected[0]);
        }
        else if (_fc_slot2.TopCard() == null)
        {
            _fc_slot2.AddCard(Manager.instance.Selected[0]);
        }
        else if (_fc_slot3.TopCard() == null)
        {
            _fc_slot3.AddCard(Manager.instance.Selected[0]);
        }
        else if (_fc_slot4.TopCard() == null)
        {
            _fc_slot4.AddCard(Manager.instance.Selected[0]);
        }
        else if (_fc_slot5.TopCard() == null)
        {
            _fc_slot5.AddCard(Manager.instance.Selected[0]);
        }
        else if (_fc_slot6.TopCard() == null)
        {
            _fc_slot6.AddCard(Manager.instance.Selected[0]);
        }
        else
        {
            // You cant place cards on the table
            yield return null;
        }

        yield return new WaitForSeconds(CardStackDelay);
        DealInProgress--;
    }

    public void CheckEmptyHand()
    {
        if(((_p1_slot1.TopCard() == null) && (_p1_slot2.TopCard() == null) 
            && (_p1_slot3.TopCard() == null) && (_p1_slot4.TopCard() == null)
            && (_p1_slot5.TopCard() == null) && (_p1_slot6.TopCard() == null)))
        {
            StartCoroutine(DrawCoroutine());
        }
        if(((_p2_slot1.TopCard() == null) && (_p2_slot2.TopCard() == null)
            && (_p2_slot3.TopCard() == null) && (_p2_slot4.TopCard() == null)
            && (_p2_slot5.TopCard() == null) && (_p2_slot6.TopCard() == null)))
        {
            StartCoroutine(DrawCoroutine());
        }
    }
}