using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource popSound;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private TextMeshProUGUI ChinaButtonText;
    [SerializeField] private TextMeshProUGUI UKButtonText;
    [SerializeField] private TextMeshProUGUI storyModeText;
    [SerializeField] private TextMeshProUGUI endlessModeText;
    [SerializeField] private TextMeshProUGUI leaderboardTitle;
    [SerializeField] private TextMeshProUGUI leaderboardContext;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Button ChinaButton;
    [SerializeField] private Button UKButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private Image leaderboardBox;

    public static GameManager.Language InGameLanguage { get; private set; }
    public static bool IsStoryMode { get; private set; }
    private Image _chinaImage;
    private Image _ukImage;

    private void Start()
    {
        if (GameManager.PlayedOneTime) transitionAnimation.Play("Transition_Out");

        InGameLanguage = GameManager.InGameLanguage;
        _chinaImage = ChinaButton.GetComponent<Image>();
        _ukImage = UKButton.GetComponent<Image>();
        switch (InGameLanguage)
        {
            case GameManager.Language.English:
                ChangeToEnglish();
                break;
            case GameManager.Language.Chinese:
                ChangeToChinese();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void PlayStoryMode()
    {
        StartCoroutine(DecreaseBackgroundMusicVolume());
        StartCoroutine(PlayStoryModeCoroutine());
    }

    private IEnumerator PlayStoryModeCoroutine()
    {
        IsStoryMode = true;
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        DialogueManager.DialogueState = DialogueManager.StoryState.Start;
        SceneManager.LoadScene("Scenes/Story Scene");
    }

    public void PlayEndlessMode()
    {
        StartCoroutine(DecreaseBackgroundMusicVolume());
        StartCoroutine(PlayEndlessModeCoroutine());
    }

    private IEnumerator PlayEndlessModeCoroutine()
    {
        IsStoryMode = false;
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene("Scenes/Game Scene");
    }

    private IEnumerator DecreaseBackgroundMusicVolume()
    {
        float initialVolume = backgroundMusic.volume;
        for (float i = initialVolume; i >= 0; i -= initialVolume / 20)
        {
            backgroundMusic.volume = i;
            yield return new WaitForSeconds(.02f);
        }

        backgroundMusic.volume = 0;
    }

    public void ChangeToEnglish()
    {
        InGameLanguage = GameManager.Language.English;

        rankText.text = "Rank";
        leaderboardTitle.text = "Leaderboard";
        ChinaButtonText.text = "ZH";
        UKButtonText.text = "EN";
        storyModeText.text = "Story Mode";
        endlessModeText.text = "Endless Mode";

        if (_chinaImage != null && _ukImage != null)
        {
            Color tempColor = _chinaImage.color;
            tempColor.a = .4f;
            _chinaImage.color = tempColor;

            tempColor = _ukImage.color;
            tempColor.a = 1f;
            _ukImage.color = tempColor;
        }
    }

    public void ChangeToChinese()
    {
        InGameLanguage = GameManager.Language.Chinese;

        rankText.text = "排行";
        leaderboardTitle.text = "排行榜";
        ChinaButtonText.text = "中";
        UKButtonText.text = "英";
        storyModeText.text = "故事模式";
        endlessModeText.text = "无尽模式";

        if (_chinaImage != null && _ukImage != null)
        {
            Color tempColor = _chinaImage.color;
            tempColor.a = 1f;
            _chinaImage.color = tempColor;

            tempColor = _ukImage.color;
            tempColor.a = .4f;
            _ukImage.color = tempColor;
        }
    }

    public void OnRankButtonPressed()
    {
        leaderboardButton.interactable = false;
        UKButton.interactable = false;
        ChinaButton.interactable = false;
        leaderboardContext.text = (InGameLanguage == GameManager.Language.Chinese)
            ? "<pos=35%>加载中..."
            : "<pos=35%>Loading...";
        leaderboardBox.gameObject.SetActive(true);

        // request for leaderboard data
        FirebaseUtils.GetLeaderboard();
    }

    public void OnCloseLeaderboardButtonPressed()
    {
        leaderboardButton.interactable = true;
        UKButton.interactable = true;
        ChinaButton.interactable = true;
        leaderboardBox.gameObject.SetActive(false);
    }

    public void UpdateLeaderboardContext(string leaderboardJson)
    {
        string context = Leaderboard.GenerateLeaderboardContext(leaderboardJson);
        leaderboardContext.text = context;
    }

    public void PlayPopSound()
    {
        popSound.Play();
    }
}

public class Player
{
    public string Name { get; set; }
    public int Score { get; set; }
    public bool IsCurrentUser { get; set; }
}

public class Leaderboard
{
    private static readonly string HighlightColor = "#FF4949";

    public static string GenerateLeaderboardContext(string leaderboardJson)
    {
        Player[] players = JsonConvert.DeserializeObject<Player[]>(leaderboardJson);

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < players.Length; i++)
        {
            stringBuilder.Append(
                players[i].IsCurrentUser
                    ? $"<color={HighlightColor}>{i + 1}<pos=10%>{Truncate(players[i].Name, 25)}<pos=80%>{players[i].Score}</color>\n"
                    : $"{i + 1}<pos=10%>{Truncate(players[i].Name, 25)}<pos=80%>{players[i].Score}\n");
        }

        return stringBuilder.ToString();
    }

    private static string Truncate(string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value.Substring(0, maxChars - 3) + "...";
    }
}