using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public enum StoryState
    {
        Start,
        End
    }

    private readonly string[] _cnStartDialogue =
    {
        "“我准备好了！让我们开始做汉堡吧！”",
        "这是你在粮食短缺中志愿为 FSKTM 学生烹饪的第一天。",
        "为了测试你的技能，学院特地派你到临时厨房准备汉堡。",
        "一到厨房，你发现这里和你想象中的不太一样。",
        "面对简陋的烹饪设备和穿着生存装备的同学们，你不禁想，“这真的是厨房吗？”",
        "尽管有这些挑战，工作仍然必须完成。"
    };

    private readonly string[] _cnEndDialogue =
    {
        "恭喜你！你已经成为制作汉堡的高手了——这在这些时候是至关重要的技能！",
        "学院对你迅速适应新环境的能力印象深刻。",
        "“我本来担心你之前专注于学术可能会让这次转变变得困难，但看来我完全不必担心。”",
        "……你正要说些什么，但决定让你的汉堡来说话。",
    };

    private readonly string[] _enStartDialogue =
    {
        "\"I've got this! Let's make some burgers!\"",
        "It's your first day volunteering to cook for the FSKTM students in the midst of the food shortage.",
        "To test your skills, the faculty has sent you to the makeshift kitchen to prepare burgers.",
        "Upon arrival in the kitchen, you find that it doesn't quite match your expectations.",
        "Faced with makeshift cooking equipment and a staff of fellow students in survival gear, you wonder, \"Is this really the kitchen?\"",
        "Despite the challenges, the work must be done."
    };

    private readonly string[] _enEndDialogue =
    {
        "Congratulations! You've become a master at making burgers - a crucial skill in these times!",
        "The faculty is impressed with how quickly you've adapted to the new environment.",
        "\"I was worried that your previous focus on academics might make this transition difficult, but it seems I had nothing to worry about.\"",
        "... You were about to comment, but decided to let your burgers do the talking.",
    };


    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private AudioSource typingSound;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private Button skipButton;

    private const float EnglishTypingSpeed = .03f;
    private const float ChineseTypingSpeed = .08f;
    private readonly Queue<string> _dialogueQueue = new Queue<string>();

    private GameManager.Language InGameLanguage { get; set; } = GameManager.Language.English;
    public static StoryState DialogueState { get; set; } = StoryState.Start;

    private void Start()
    {
        transitionAnimation.Play("Transition_Out");

        InGameLanguage = MainMenuManager.InGameLanguage;

        dialogueBox.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);

        switch (DialogueState)
        {
            case StoryState.Start:
                ShowStartDialogue();
                break;
            case StoryState.End:
                ShowEndDialogue();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void LoadStartDialogue()
    {
        string[] lines = { };
        switch (InGameLanguage)
        {
            case GameManager.Language.English:
                lines = _enStartDialogue;
                continueText.text = "Continue >>";
                skipText.text = "Skip";
                break;
            case GameManager.Language.Chinese:
                lines = _cnStartDialogue;
                continueText.text = "继续 》";
                skipText.text = "跳过";
                break;
        }

        foreach (var line in lines)
        {
            _dialogueQueue.Enqueue(line);
        }
    }

    private void LoadEndDialogue()
    {
        string[] lines = { };
        switch (InGameLanguage)
        {
            case GameManager.Language.English:
                lines = _enEndDialogue;
                continueText.text = "Continue >>";
                skipText.text = "Skip";
                break;
            case GameManager.Language.Chinese:
                lines = _cnEndDialogue;
                continueText.text = "继续 》";
                skipText.text = "跳过";
                break;
        }

        foreach (var line in lines)
        {
            _dialogueQueue.Enqueue(line);
        }
    }

    private IEnumerator TypeOneDialogue()
    {
        dialogueText.text = string.Empty;
        dialogueText.gameObject.SetActive(true);

        if (typingSound.time != 0)
        {
            typingSound.UnPause();
        }
        else
        {
            typingSound.Play();
        }

        if (_dialogueQueue.Count > 0)
        {
            string line = _dialogueQueue.Dequeue();
            foreach (var letter in line)
            {
                dialogueText.text += letter;
                yield return (InGameLanguage == GameManager.Language.English)
                    ? new WaitForSeconds(EnglishTypingSpeed)
                    : new WaitForSeconds(ChineseTypingSpeed);
            }

            continueButton.SetActive(true);
        }

        typingSound.Pause();
    }

    private void ShowStartDialogue()
    {
        LoadStartDialogue();
        dialogueBox.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    private void ShowEndDialogue()
    {
        LoadEndDialogue();
        skipButton.gameObject.SetActive(true);
        dialogueBox.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    private IEnumerator ProceedNextLine()
    {
        continueButton.SetActive(false);

        popSound.Play();
        yield return new WaitWhile(() => popSound.isPlaying);

        dialogueText.gameObject.SetActive(false);

        if (_dialogueQueue.Count <= 0)
        {
            // all story displayed
            StartCoroutine(SwitchScene());
        }
        else
        {
            StartCoroutine(TypeOneDialogue());
        }
    }

    private IEnumerator SwitchScene()
    {
        dialogueBox.gameObject.SetActive(false);
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene(DialogueState == StoryState.Start ? "Scenes/Game Scene" : "Scenes/Main Menu");
    }

    public void OnContinueButtonPressed()
    {
        StartCoroutine(ProceedNextLine());
    }

    public void OnSkipButtonPressed()
    {
        skipButton.gameObject.SetActive(false);
        StartCoroutine(SkipStory());
    }

    private IEnumerator SkipStory()
    {
        popSound.Play();
        yield return new WaitWhile(() => popSound.isPlaying);
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene(DialogueState == StoryState.Start ? "Scenes/Game Scene" : "Scenes/Main Menu");
    }
}