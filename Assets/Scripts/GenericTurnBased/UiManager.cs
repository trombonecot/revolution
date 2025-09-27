using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public TMP_Text turnNumber;
    public TMP_Text sideName;
    public TMP_Text influence;
    public TMP_Text influenceIncrement;
    public TMP_Text health;
    public Button nextTurn;

    public Token activeToken = null;


    // BUTTONS
    private bool buttonsInit;
    public Button[] buttons;

    public static UiManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static void SetActiveToken(Token token)
    {
        if (Instance == null)
        {
            return;
        }

        Instance.activeToken = token;

        if (token != null)
        {
            Instance.health.text = $"{token.health}/{token.maxHealth}";
        } else
        {
            Instance.health.text ="";
        }
    }
    public static Token GetActiveToken()
    {
        if (Instance == null)
        {
            return null;
        }

        return Instance.activeToken;
    }

    void Start()
    {
        nextTurn.onClick.AddListener(() => onNextTurn());
    }

    void Update()
    {
        PlayerSide playerSide = GameManager.GetActiveSide();

        if (playerSide)
        {
            this.sideName.text = playerSide.tokenName;
        }

        this.turnNumber.text = GameManager.GetTurnNumer().ToString();
        this.influence.text = InfluenceManager.GetCurrentInfluence().ToString();
        this.influenceIncrement.text = InfluenceManager.GetCurrentInfluenceIncrement().ToString();

        if (!buttonsInit)
        {
            for (int i = 0; i < buttons.Length; i++)
            {

                InitializeButton(i);
            }
        } else if (!activeToken)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                ResetButton(i);
            }
            
        }
    }

    protected void InitializeButton(int i)
    {
        if (activeToken != null && (activeToken.buttonActions.Length > i))
        {
            Action action = activeToken.buttonActions[i];

            buttons[i].GetComponentInChildren<TMP_Text>().text = action.actionName;
            buttons[i].gameObject.SetActive(true);

            buttons[i].onClick.AddListener(() => action.OnActivate(activeToken, null));
            buttonsInit = true;
        }
        else
        {
            ResetButton(i);
        }
    }

    protected void ResetButton(int i )
    {
        buttons[i].gameObject.SetActive(false);
        buttons[i].onClick.RemoveAllListeners();
        buttonsInit = false;
    }

    void onNextTurn()
    {
        GameManager.GoToNextTurn();
    }
}
