using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Language
    {
        English,
        Chinese
    }

    [SerializeField] private MakeBurger burgerMaker;
    [SerializeField] private CustomerOrderGenerator customerOrderGenerator;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private AudioSource chachingSound;
    [SerializeField] private AudioSource countDownSound;
    [SerializeField] private AudioSource whistleSound;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private AudioSource wrongAnswerSound;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI doneText;
    [SerializeField] private TextMeshProUGUI customerOrderTitleText;
    [SerializeField] private TextMeshProUGUI recipeText;
    [SerializeField] private TextMeshProUGUI recipeTitleText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI summaryTitleText;
    [SerializeField] private TextMeshProUGUI summaryTargetText;
    [SerializeField] private TextMeshProUGUI summaryMoneyText;
    [SerializeField] private TextMeshProUGUI summaryTotalText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI mainMenuText;
    [SerializeField] private TextMeshProUGUI newHighScoreText;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI tutorialTitleText;
    [SerializeField] private TextMeshProUGUI tutorialContextText;
    [SerializeField] private TextMeshProUGUI quitText;
    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private Image targetLabel;
    [SerializeField] private Image moneyLabel;
    [SerializeField] private Image totalLabel;
    [SerializeField] private Image recipeBackgroundImage;
    [SerializeField] private Image quitDialogueBox;
    [SerializeField] private Image tutorialBox;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button tutorialNextButton;
    [SerializeField] private Button tutorialCloseButton;
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private ParticleSystem confetti;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI moneyText;

    public bool IsGameOver { get; private set; } = true;
    public static Language InGameLanguage { get; private set; } = Language.English;
    public static bool PlayedOneTime { get; private set; } = false;
    private int MoneyEarned { get; set; }
    private int DayPassed { get; set; }
    private int TargetForTheDay { get; set; }
    private int TotalMoneyEarned { get; set; }

    private float _timerTime;
    private const int GameDurationInSec = 90;
    private const int InitialTargetForTheDayVal = 400;
    private const int IncreaseInTargetValPerDay = 200;

    private const string EngTutorialContext =
        "Welcome to WCD!\nThis is a classic burger game where customers give orders (in programming syntax), and you make the burger accordingly.\n\nNote:\n1) You will be penalized for:\n    - missing any required burger content.\n    - adding a non-required burger content.\n2) You are not allowed to remove burger content after you've added it.\n\n<b>Make sure you've memorized the default value for the Burger class variables!</b> You should make your burger according to the <b>final value</b> for the patties, toppings and sauce variable.";

    private const string EngSecondTutorialContext =
        "For example, if the default values of the Burger class variable in the recipe are:\n\npatties = {\"Fish\", \"Chicken\"};\ntoppings = {\"Cucumber\", \"Lettuce\", \"Cheese\"};\nsauce = \"Mayo\";\n\nAnd the given customer's order is:\n\nBurger burger = new Burger();\nburger.setToppings(new String[] {\"Cheese\"});\nburger.setSauce(\"BBQ\");\n\nThe correct answer will be:\n\npatties = {\"Fish\", \"Chicken\"}    // default\ntoppings = {\"Cheese\"}            // overwrite\nsauce = \"BBQ\"                      // overwrite";

    private const string ChiTutorialContext =
        "欢迎来到 WCD！\n这游戏是一款经典的汉堡游戏，不过食客会使用编程语法下单，然后您必须根据食客的订单来制作正確的汉堡。\n\n注：\n1) 您将受到处罚当您的漢堡：\n    - 缺少任何所需的汉堡馅料\n    - 添加非必需的汉堡馅料。\n2) 汉堡馅料添加后不可被删除。\n\n<b>请确保您已记住 Burger 类变量的默认值！</b> 您需根据patties， toppings和sauce类变量的最终值来制作汉堡。";

    private const string ChiSecondTutorialContext =
        "举个例子，如果 Burger 类变量的默认值为：\n\npatties = {\"Fish\", \"Chicken\"};\ntoppings = {\"Cucumber\", \"Lettuce\", \"Cheese\"};\nsauce = \"Mayo\";\n\n以及顾客的订单为：\n\nBurger burger = new Burger();\nburger.setToppings(new String[] {\"Cheese\"});\nburger.setSauce(\"BBQ\");\n\n那么正解将会是：\n\npatties = {\"Fish\", \"Chicken\"}    // 默認值\ntoppings = {\"Cheese\"}\nsauce = \"BBQ\"";

    private void Start()
    {
        Time.timeScale = 1f;

        transitionAnimation.Play("Transition_Out");
        PlayedOneTime = true;

        // get game language from main menu manager
        InGameLanguage = MainMenuManager.InGameLanguage;

        // set text language accordingly
        doneText.text = (InGameLanguage == Language.Chinese) ? "完成" : "Done";
        customerOrderTitleText.text = (InGameLanguage == Language.Chinese) ? "顾客订单：" : "Customer's Order:";
        tutorialText.text = (InGameLanguage == Language.Chinese) ? "教程" : "Tutorial";
        tutorialTitleText.text = (InGameLanguage == Language.Chinese) ? "教程" : "Tutorial";
        tutorialContextText.text = (InGameLanguage == Language.Chinese) ? ChiTutorialContext : EngTutorialContext;
        recipeTitleText.text = (InGameLanguage == Language.Chinese) ? "今日份菜谱" : "Recipe Of The Day";
        readyText.text = (InGameLanguage == Language.Chinese) ? "我准备好了" : "I'm Ready";
        continueText.text = (InGameLanguage == Language.Chinese) ? "继续" : "Continue";
        mainMenuText.text = (InGameLanguage == Language.Chinese) ? "返回主页" : "Main Menu";
        newHighScoreText.text = (InGameLanguage == Language.Chinese) ? "新纪录！" : "New Highscore!";
        yesText.text = (InGameLanguage == Language.Chinese) ? "是" : "Yes";
        noText.text = (InGameLanguage == Language.Chinese) ? "取消" : "No";
        quitText.text = (InGameLanguage == Language.Chinese) ? "返回主頁?" : "Quit?";
        nextText.text = (InGameLanguage == Language.Chinese) ? "下一頁 》" : "Next >>";

        // show recipe
        customerOrderGenerator.CreateRecipeOfTheDay();
        tutorialButton.gameObject.SetActive(true);
        recipeText.gameObject.SetActive(true);
        recipeTitleText.gameObject.SetActive(true);
        recipeBackgroundImage.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);

        // set initial values
        TargetForTheDay = InitialTargetForTheDayVal;
        DayPassed = 0;
        TotalMoneyEarned = 0;

        // hide target text if is story mode
        if (MainMenuManager.IsStoryMode) targetText.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        readyButton.gameObject.SetActive(false);
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        // hide the recipe
        tutorialButton.gameObject.SetActive(false);
        recipeText.gameObject.SetActive(false);
        recipeTitleText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);

        transitionAnimation.Play("Transition_Out");

        // play BGM
        backgroundMusic.volume = 0.7f;
        backgroundMusic.pitch = 1;
        backgroundMusic.Play();

        // start the game
        _timerTime = GameDurationInSec;
        TargetForTheDay = InitialTargetForTheDayVal + (DayPassed * IncreaseInTargetValPerDay);
        targetText.text = ((InGameLanguage == Language.Chinese) ? "目标: $" : "Target: $") + TargetForTheDay;
        IsGameOver = false;
        MoneyEarned = 0;
        moneyText.text = $"${MoneyEarned}";
        DayPassed++;
        dayText.text = (InGameLanguage == Language.English) ? $"Day {DayPassed}" : $"第{DayPassed}天";
        customerOrderGenerator.GenerateCustomerOrder();
        burgerMaker.SetAllButtonsInteractable(true);
        burgerMaker.CreateTray();
    }

    private void Update()
    {
        if (!IsGameOver)
        {
            if (_timerTime > 0)
            {
                _timerTime -= Time.deltaTime;

                if (_timerTime <= 10)
                {
                    backgroundMusic.pitch = 1.2f;
                    if (!countDownSound.isPlaying) countDownSound.Play();
                }
            }
            else
            {
                // if game over,
                GameOver();
            }
        }

        UpdateTimer();
    }

    private void GameOver()
    {
        IsGameOver = true;
        if (countDownSound.isPlaying) countDownSound.Stop();
        StartCoroutine(DecreaseBackgroundMusicVolume());
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator DecreaseBackgroundMusicVolume()
    {
        float initialVolume = backgroundMusic.volume;
        for (float i = initialVolume; i >= 0; i -= initialVolume / 20)
        {
            backgroundMusic.volume = i;
            yield return new WaitForSeconds(.05f);
        }

        backgroundMusic.volume = 0;
        backgroundMusic.Stop();
    }

    private IEnumerator GameOverCoroutine()
    {
        whistleSound.Play();
        yield return new WaitWhile(() => whistleSound.isPlaying);

        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        burgerMaker.RemoveTray();

        if (MainMenuManager.IsStoryMode)
        {
            DialogueManager.DialogueState = DialogueManager.StoryState.End;
            SceneManager.LoadScene("Scenes/Story Scene");
        }
        else
        {
            transitionAnimation.Play("Transition_Out");

            // display day summary
            recipeBackgroundImage.gameObject.SetActive(true);
            summaryTitleText.gameObject.SetActive(true);
            moneyLabel.gameObject.SetActive(true);
            totalLabel.gameObject.SetActive(true);
            targetLabel.gameObject.SetActive(true);

            TotalMoneyEarned += MoneyEarned;

            summaryTargetText.text = (InGameLanguage == Language.English)
                ? $"Today's Target: ${TargetForTheDay}"
                : $"今日の目标：${TargetForTheDay}";
            summaryMoneyText.text = (InGameLanguage == Language.English)
                ? $"Today's Profit: ${MoneyEarned}"
                : $"今日の收入：${MoneyEarned}";
            summaryTotalText.text = (InGameLanguage == Language.English)
                ? $"Total Profit: ${TotalMoneyEarned}"
                : $"总收入：${TotalMoneyEarned}";

            // update highscore record in PlayerPrefs
            if (TotalMoneyEarned > PlayerPrefs.GetInt("highscore"))
            {
                newHighScoreText.gameObject.SetActive(true);
                PlayerPrefs.SetInt("highscore", TotalMoneyEarned);
            }

            // update highscore in database
            FirebaseUtils.UpdateUserHighScore(TotalMoneyEarned);

            if (MoneyEarned >= TargetForTheDay)
            {
                continueButton.gameObject.SetActive(true);

                summaryTitleText.text = (InGameLanguage == Language.English)
                    ? $"Day {DayPassed} Passed!"
                    : $"第{DayPassed}天·成功！";

                winSound.Play();
                confetti.Play();
            }
            else
            {
                mainMenuButton.gameObject.SetActive(true);

                summaryTitleText.text = (InGameLanguage == Language.English)
                    ? $"Day {DayPassed} Failed!"
                    : $"第{DayPassed}天·失败！";

                gameOverSound.Play();
            }
        }
    }

    public void OnTutorialNextButtonPressed()
    {
        tutorialNextButton.gameObject.SetActive(false);
        tutorialCloseButton.gameObject.SetActive(true);

        tutorialContextText.text =
            (InGameLanguage == Language.Chinese) ? ChiSecondTutorialContext : EngSecondTutorialContext;
    }

    public void OnContinueButtonPressed()
    {
        continueButton.gameObject.SetActive(false);
        StartCoroutine(ContinueCoroutine());
    }

    private IEnumerator ContinueCoroutine()
    {
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        // hide summary UI
        newHighScoreText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);
        summaryTitleText.gameObject.SetActive(false);
        moneyLabel.gameObject.SetActive(false);
        totalLabel.gameObject.SetActive(false);
        targetLabel.gameObject.SetActive(false);

        // show recipe
        customerOrderGenerator.CreateRecipeOfTheDay();
        tutorialButton.gameObject.SetActive(true);
        recipeText.gameObject.SetActive(true);
        recipeTitleText.gameObject.SetActive(true);
        recipeBackgroundImage.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);

        transitionAnimation.Play("Transition_Out");
    }

    public void OnMainMenuButtonPressed()
    {
        mainMenuButton.gameObject.SetActive(false);
        StartCoroutine(GoToMainMenu());
    }

    private IEnumerator GoToMainMenu()
    {
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        // hide summary UI
        newHighScoreText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);
        summaryTitleText.gameObject.SetActive(false);
        targetLabel.gameObject.SetActive(false);
        moneyLabel.gameObject.SetActive(false);
        totalLabel.gameObject.SetActive(false);

        SceneManager.LoadScene("Scenes/Main Menu");
    }

    private void UpdateTimer()
    {
        timerText.text = DisplayTimeAsString((int) Math.Ceiling(_timerTime));
    }

    private string DisplayTimeAsString(int sec)
    {
        return $"{sec / 60}:{sec % 60:D2}";
    }

    public void PlayChachingSound()
    {
        chachingSound.Play();
    }

    public void PlayPopSound()
    {
        popSound.Play();
    }

    public void PlayWrongAnswerSound()
    {
        wrongAnswerSound.Play();
    }

    public void GoBackToMainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(GoBackToMainMenuCoroutine());
    }

    private IEnumerator GoBackToMainMenuCoroutine()
    {
        StartCoroutine(DecreaseBackgroundMusicVolume());
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene("Scenes/Main Menu");
    }

    public void ShowQuitDialogueBox()
    {
        quitDialogueBox.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideQuitDialogueBox()
    {
        quitDialogueBox.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowTutorialBox()
    {
        tutorialButton.interactable = false;
        tutorialBox.gameObject.SetActive(true);
        tutorialNextButton.gameObject.SetActive(true);
        tutorialCloseButton.gameObject.SetActive(false);
    }

    public void HideTutorialBox()
    {
        tutorialButton.interactable = true;
        tutorialBox.gameObject.SetActive(false);
        tutorialContextText.text = (InGameLanguage == Language.Chinese) ? ChiTutorialContext : EngTutorialContext;
    }

    public void IncreaseMoneyEarned(int amount)
    {
        int originalVal = MoneyEarned;
        MoneyEarned = MoneyEarned + amount < 0 ? 0 : MoneyEarned + amount;
        StartCoroutine(MoneyTextChangeAnim(originalVal, MoneyEarned));
    }

    private IEnumerator MoneyTextChangeAnim(int originalVal, int newVal)
    {
        if (newVal >= originalVal)
        {
            for (int i = originalVal; i <= newVal; i++)
            {
                moneyText.text = $"${i}";
                yield return new WaitForSeconds(0.005f);
            }
        }
        else
        {
            for (int i = originalVal; i >= newVal; i--)
            {
                moneyText.text = $"${i}";
                yield return new WaitForSeconds(0.005f);
            }
        }
    }
}