using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakeBurger : MonoBehaviour
{
    private enum State
    {
        Spawn,
        AddingPatty,
        AddingTopping,
        AddingSauce,
        Ready,
    }

    public enum BurgerContent
    {
        BeefPatty,
        ChickenPatty,
        FishPatty,
        Cheese,
        Cucumber,
        Onion,
        Salad,
        Tomato,
        BbqSauce,
        ChiliSauce,
        KetchupSauce,
        MayoSauce,
        MustardSauce,
    }

    [SerializeField] private float offsetY = .5f;
    [SerializeField] private float moveDist = 4.45f;
    [SerializeField] private GameObject topBunPrefab;
    [SerializeField] private GameObject trayWithBunPrefab;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CustomerOrderGenerator customerOrderGenerator;
    [SerializeField] private Button doneButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI[] itemLabels;
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private Sprite[] pattyOptions;
    [SerializeField] private Sprite[] toppingOptions;
    [SerializeField] private Sprite[] sauceOptions;
    [SerializeField] private GameObject[] pattyPrefabs;
    [SerializeField] private GameObject[] toppingPrefabs;
    [SerializeField] private GameObject[] saucePrefabs;

    private const float TrayMovingTime = .35f;
    private const float OffsetZ = 0.01f;
    private GameObject _currentBurger;
    private State GameState { get; set; }
    private bool IsMoving { get; set; }
    private HashSet<BurgerContent> _burgerContents;

    private void Start()
    {
        _burgerContents = new HashSet<BurgerContent>();
    }

    private void Update()
    {
        if (gameManager.IsGameOver)
        {
            SetAllButtonsInteractable(false);
        }
    }

    public void SetAllButtonsInteractable(bool interactable)
    {
        doneButton.interactable = interactable;
        leftButton.interactable = interactable;
        rightButton.interactable = interactable;
        foreach (Button button in itemButtons)
        {
            button.interactable = interactable;
        }
    }

    public void CreateTray()
    {
        _currentBurger = Instantiate(trayWithBunPrefab, transform.position, transform.rotation);
        GameState = State.Spawn;
        MoveTrayToRight();
    }

    public void RemoveTray()
    {
        if (_currentBurger == null) return;
        // remove tray
        Destroy(_currentBurger);
        // clear burger content
        _burgerContents.Clear();
    }

    private void MoveTrayToLeft()
    {
        StartCoroutine(MoveTray(Vector3.left));
    }

    private void MoveTrayToRight()
    {
        StartCoroutine(MoveTray(Vector3.right));
    }

    private IEnumerator MoveTray(Vector3 direction)
    {
        if (_currentBurger != null)
        {
            IsMoving = true;
            // disable all option button
            foreach (var button in itemButtons)
            {
                button.interactable = false;
            }

            Transform burgerTransform = _currentBurger.GetComponent<Transform>();
            Vector3 currPos = burgerTransform.position;
            Vector3 dest = currPos + (direction * moveDist);

            float elapsedTime = 0;
            while (elapsedTime < TrayMovingTime)
            {
                burgerTransform.position = Vector3.Lerp(currPos, dest, (elapsedTime / TrayMovingTime));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (direction == Vector3.left) MoveToPrevState();
            else MoveToNextState();

            IsMoving = false;
            burgerTransform.position = dest;
            UpdateMachineOption();
        }
    }

    private void MoveToPrevState()
    {
        GameState = GameState switch
        {
            State.AddingTopping => State.AddingPatty,
            State.AddingSauce => State.AddingTopping,
            State.Ready => State.AddingSauce,
            _ => throw new ArgumentOutOfRangeException()
        };
        doneButton.interactable = GameState == State.Ready;
    }

    private void MoveToNextState()
    {
        GameState = GameState switch
        {
            State.Spawn => State.AddingPatty,
            State.AddingPatty => State.AddingTopping,
            State.AddingTopping => State.AddingSauce,
            State.AddingSauce => State.Ready,
            _ => throw new ArgumentOutOfRangeException()
        };
        doneButton.interactable = GameState == State.Ready;
    }

    private void UpdateMachineOption()
    {
        switch (GameState)
        {
            case State.AddingPatty:
                for (int i = 0; i < 5; i++)
                {
                    itemButtons[i].image.sprite = pattyOptions[i];

                    itemLabels[i].text = i switch
                    {
                        0 => "Beef",
                        1 => "Chicken",
                        2 => "Fish",
                        _ => String.Empty
                    };

                    Color temp = itemButtons[i].image.color;
                    if (pattyOptions[i] == null)
                    {
                        temp.a = 0f;
                        itemButtons[i].image.color = temp;
                        itemButtons[i].interactable = false;
                    }
                    else
                    {
                        temp.a = 1f;
                        itemButtons[i].image.color = temp;
                        itemButtons[i].interactable = true;
                    }
                }

                // set item button intractability based on the current burger content
                if (_burgerContents.Contains(BurgerContent.BeefPatty)) itemButtons[0].interactable = false;
                if (_burgerContents.Contains(BurgerContent.ChickenPatty)) itemButtons[1].interactable = false;
                if (_burgerContents.Contains(BurgerContent.FishPatty)) itemButtons[2].interactable = false;

                break;
            case State.AddingTopping:
                for (int i = 0; i < 5; i++)
                {
                    itemButtons[i].image.sprite = toppingOptions[i];

                    itemLabels[i].text = i switch
                    {
                        0 => "Cheese",
                        1 => "Cucumber",
                        2 => "Onion",
                        3 => "Lettuce",
                        4 => "Tomato",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    Color temp = itemButtons[i].image.color;
                    if (toppingOptions[i] == null)
                    {
                        temp.a = 0f;
                        itemButtons[i].image.color = temp;
                        itemButtons[i].interactable = false;
                    }
                    else
                    {
                        temp.a = 1f;
                        itemButtons[i].image.color = temp;
                        itemButtons[i].interactable = true;
                    }
                }

                // set item button intractability based on the current burger content
                if (_burgerContents.Contains(BurgerContent.Cheese)) itemButtons[0].interactable = false;
                if (_burgerContents.Contains(BurgerContent.Cucumber)) itemButtons[1].interactable = false;
                if (_burgerContents.Contains(BurgerContent.Onion)) itemButtons[2].interactable = false;
                if (_burgerContents.Contains(BurgerContent.Salad)) itemButtons[3].interactable = false;
                if (_burgerContents.Contains(BurgerContent.Tomato)) itemButtons[4].interactable = false;

                break;
            case State.AddingSauce:
                for (int i = 0; i < 5; i++)
                {
                    itemButtons[i].image.sprite = sauceOptions[i];

                    itemLabels[i].text = i switch
                    {
                        0 => "BBQ",
                        1 => "Chili",
                        2 => "Ketchup",
                        3 => "Mayo",
                        4 => "Mustard",
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    Color temp = itemButtons[i].image.color;
                    if (sauceOptions[i] == null)
                    {
                        temp.a = 0f;
                        itemButtons[i].image.color = temp;
                        itemButtons[i].interactable = false;
                    }
                    else
                    {
                        temp.a = 1f;
                        itemButtons[i].image.color = temp;
                        itemButtons[i].interactable = true;
                    }
                }

                // set item button intractability based on the current burger content
                // only one type of sauce is allowed
                if (_burgerContents.Contains(BurgerContent.BbqSauce) ||
                    _burgerContents.Contains(BurgerContent.ChiliSauce) ||
                    _burgerContents.Contains(BurgerContent.KetchupSauce) ||
                    _burgerContents.Contains(BurgerContent.MayoSauce) ||
                    _burgerContents.Contains(BurgerContent.MustardSauce))
                {
                    foreach (var button in itemButtons)
                    {
                        button.interactable = false;
                    }
                }

                break;
            default:
                for (int i = 0; i < 5; i++)
                {
                    itemButtons[i].image.sprite = null;
                    itemLabels[i].text = String.Empty;

                    Color temp = itemButtons[i].image.color;
                    temp.a = 0f;
                    itemButtons[i].image.color = temp;
                    itemButtons[i].interactable = false;
                }

                break;
        }
    }

    private void PlayItemButtonClickSound(bool isCorrect)
    {
        if (isCorrect)
        {
            gameManager.PlayPopSound();
        }
        else
        {
            gameManager.PlayWrongAnswerSound();
        }
    }

    private void OnItemButtonClick(int buttonIndex, BurgerContent? patty, BurgerContent topping, BurgerContent sauce)
    {
        Vector3 offset = new Vector3(0, (_burgerContents.Count + 1) * offsetY, (_burgerContents.Count + 1) * -OffsetZ);

        switch (GameState)
        {
            case State.AddingPatty:
                if (patty == null) break;
                if (_burgerContents.Add((BurgerContent) patty))
                {
                    GameObject temp = Instantiate(pattyPrefabs[buttonIndex], _currentBurger.transform.position + offset,
                        _currentBurger.transform.rotation);
                    temp.transform.parent = _currentBurger.transform;
                    itemButtons[buttonIndex].interactable = false;
                }

                PlayItemButtonClickSound(customerOrderGenerator.CheckContentAdded((BurgerContent) patty));

                break;
            case State.AddingTopping:
                if (_burgerContents.Add(topping))
                {
                    GameObject temp = Instantiate(toppingPrefabs[buttonIndex],
                        _currentBurger.transform.position + offset,
                        _currentBurger.transform.rotation);
                    temp.transform.parent = _currentBurger.transform;
                    itemButtons[buttonIndex].interactable = false;
                }

                PlayItemButtonClickSound(customerOrderGenerator.CheckContentAdded(topping));

                break;
            case State.AddingSauce:
                if (_burgerContents.Add(sauce))
                {
                    GameObject temp = Instantiate(saucePrefabs[buttonIndex], _currentBurger.transform.position + offset,
                        _currentBurger.transform.rotation);
                    temp.transform.parent = _currentBurger.transform;

                    // only one type of sauce is allowed
                    foreach (var button in itemButtons)
                    {
                        button.interactable = false;
                    }
                }

                PlayItemButtonClickSound(customerOrderGenerator.CheckContentAdded(sauce));

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnFirstItemClick()
    {
        OnItemButtonClick(0, BurgerContent.BeefPatty, BurgerContent.Cheese, BurgerContent.BbqSauce);
    }

    public void OnSecondItemClick()
    {
        OnItemButtonClick(1, BurgerContent.ChickenPatty, BurgerContent.Cucumber, BurgerContent.ChiliSauce);
    }

    public void OnThirdItemClick()
    {
        OnItemButtonClick(2, BurgerContent.FishPatty, BurgerContent.Onion, BurgerContent.KetchupSauce);
    }

    public void OnForthItemClick()
    {
        OnItemButtonClick(3, null, BurgerContent.Salad, BurgerContent.MayoSauce);
    }

    public void OnFifthItemClick()
    {
        OnItemButtonClick(4, null, BurgerContent.Tomato, BurgerContent.MustardSauce);
    }

    public void OnLeftButtonPressed()
    {
        if (GameState != State.AddingPatty && !IsMoving)
        {
            MoveTrayToLeft();
        }
    }

    public void OnRightButtonPressed()
    {
        if (GameState != State.Ready && !IsMoving)
        {
            MoveTrayToRight();
        }
    }

    public void OnDoneButtonPressed()
    {
        // disable done button
        doneButton.interactable = false;
        // check burger content & update money earned
        int earn = customerOrderGenerator.CheckResult(_burgerContents);
        gameManager.IncreaseMoneyEarned(earn);
        // play destroy animation
        StartCoroutine(DestroyAnim());
        // generate new customer order
        customerOrderGenerator.GenerateCustomerOrder();
        // spawn a new tray
        CreateTray();
    }

    private IEnumerator DestroyAnim()
    {
        // save reference of the current burger
        GameObject temp = _currentBurger;

        // add burger top bun
        Vector3 offset = new Vector3(0, (_burgerContents.Count + 1) * offsetY, (_burgerContents.Count + 1) * -OffsetZ);
        Instantiate(topBunPrefab,
            _currentBurger.transform.position + offset,
            _currentBurger.transform.rotation).transform.parent = _currentBurger.transform;

        // clear burger content
        _burgerContents.Clear();

        // move the burger
        Transform burgerTransform = temp.GetComponent<Transform>();
        Vector3 currPos = burgerTransform.position;
        Vector3 dest = currPos + (Vector3.right * moveDist);
        float elapsedTime = 0;
        while (elapsedTime < TrayMovingTime)
        {
            burgerTransform.position = Vector3.Lerp(currPos, dest, (elapsedTime / TrayMovingTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        burgerTransform.position = dest;

        // destroy the current burger
        Destroy(temp);
    }
}