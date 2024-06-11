using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerOrderGenerator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI customerOrderDisplayText;
    [SerializeField] private TextMeshProUGUI recipeText;
    [SerializeField] private int profitPerCorrect = 20;
    [SerializeField] private int penaltyPerWrong = 20;

    private HashSet<MakeBurger.BurgerContent> _answerSet;
    private Recipe _recipe;

    private void Start()
    {
        CreateRecipeOfTheDay();
        _answerSet = new HashSet<MakeBurger.BurgerContent>();
    }

    public void CreateRecipeOfTheDay()
    {
        _recipe = new MainRecipe();
        recipeText.text = _recipe.GetRecipe();
    }

    public void GenerateCustomerOrder()
    {
        customerOrderDisplayText.text = _recipe.GenerateCustomerOrder(out _answerSet);
    }

    public int CheckResult(HashSet<MakeBurger.BurgerContent> burgerContents)
    {
        // use the answerSet to determine & return the profit
        int totalNumOfCorrectContents = _answerSet.Count;
        int totalNumOfInputContents = burgerContents.Count;
        _answerSet.IntersectWith(burgerContents);
        int correctCount = _answerSet.Count;
        return (correctCount * profitPerCorrect) - (((totalNumOfInputContents - correctCount) +
                                                     (totalNumOfCorrectContents - correctCount)) * penaltyPerWrong);
    }

    public bool CheckContentAdded(MakeBurger.BurgerContent burgerContent)
    {
        return _answerSet.Contains(burgerContent);
    }
}

abstract class Recipe
{
    protected readonly string[] Patties = {"Beef", "Chicken", "Fish"};
    protected readonly string[] Toppings = {"Cheese", "Cucumber", "Onion", "Lettuce", "Tomato"};
    protected readonly string[] Sauces = {"BBQ", "Chili", "Ketchup", "Mayo", "Mustard"};

    protected const string PinkColorCode = "#FF7979";
    protected const string BlueColorCode = "#83E1F5";
    protected const string YellowColorCode = "#FEF06F";
    protected const string GreenColorCode = "#BAFF90";
    protected const string OrangeColorCode = "#FCA846";

    public abstract string GetRecipe();
    public abstract string GenerateCustomerOrder(out HashSet<MakeBurger.BurgerContent> answerSet);

    protected MakeBurger.BurgerContent MapStringToBurgerContent(string content)
    {
        switch (content)
        {
            case "Beef":
                return MakeBurger.BurgerContent.BeefPatty;
            case "Chicken":
                return MakeBurger.BurgerContent.ChickenPatty;
            case "Fish":
                return MakeBurger.BurgerContent.FishPatty;
            case "Cheese":
                return MakeBurger.BurgerContent.Cheese;
            case "Cucumber":
                return MakeBurger.BurgerContent.Cucumber;
            case "Onion":
                return MakeBurger.BurgerContent.Onion;
            case "Lettuce":
                return MakeBurger.BurgerContent.Salad;
            case "Tomato":
                return MakeBurger.BurgerContent.Tomato;
            case "BBQ":
                return MakeBurger.BurgerContent.BbqSauce;
            case "Chili":
                return MakeBurger.BurgerContent.ChiliSauce;
            case "Ketchup":
                return MakeBurger.BurgerContent.KetchupSauce;
            case "Mayo":
                return MakeBurger.BurgerContent.MayoSauce;
            case "Mustard":
                return MakeBurger.BurgerContent.MustardSauce;
            default:
                throw new Exception();
        }
    }
}

class MainRecipe : Recipe
{
    private readonly string _recipe;
    private readonly HashSet<string> _defaultPatties;
    private readonly HashSet<string> _defaultToppings;
    private readonly string _defaultSauce;

    public MainRecipe()
    {
        string defaultPattiesText = GeneratePattyArrayContent(out _defaultPatties);
        string defaultToppingsText = GenerateToppingArrayContent(out _defaultToppings);
        _defaultSauce = Sauces[Random.Range(0, Sauces.Length)];

        _recipe =
            $"<color={PinkColorCode}>class</color> <color={BlueColorCode}>Burger</color> {{\n\t<color={PinkColorCode}>private</color> <color={BlueColorCode}>String[]</color> <color={OrangeColorCode}>patties</color> = {{{defaultPattiesText}}};\n\t<color={PinkColorCode}>private</color> <color={BlueColorCode}>String[]</color> <color={OrangeColorCode}>toppings</color> = {{{defaultToppingsText}}};\n\t<color={PinkColorCode}>private</color> <color={BlueColorCode}>String</color> <color={OrangeColorCode}>sauce</color> = <color={YellowColorCode}>\"{_defaultSauce}\"</color>;\n\n\t<color={PinkColorCode}>public</color> <color={GreenColorCode}>Burger</color>() {{\n\n\t}}\n\n\t<color={PinkColorCode}>public</color> <color={PinkColorCode}>void</color> <color={GreenColorCode}>setPatties</color>(<color={BlueColorCode}>String[]</color> <color={OrangeColorCode}>patties</color>) {{\n\t\t<color={PinkColorCode}>this</color>.patties = <color={OrangeColorCode}>patties</color>;\n\t}}\n\n\t<color={PinkColorCode}>public</color> <color={PinkColorCode}>void</color> <color={GreenColorCode}>setToppings</color>(<color={BlueColorCode}>String[]</color> <color={OrangeColorCode}>toppings</color>) {{\n\t\t<color={PinkColorCode}>this</color>.toppings = <color={OrangeColorCode}>toppings</color>;\n\t}}\n\n\t<color={PinkColorCode}>public</color> <color={PinkColorCode}>void</color> <color={GreenColorCode}>setSauce</color>(<color={BlueColorCode}>String</color> <color={OrangeColorCode}>sauce</color>) {{\n\t\t<color={PinkColorCode}>this</color>.sauce = <color={OrangeColorCode}>sauce</color>;\n\t}}\n}}";
    }

    public override string GetRecipe()
    {
        return _recipe;
    }

    public override string GenerateCustomerOrder(out HashSet<MakeBurger.BurgerContent> answerSet)
    {
        StringBuilder stringBuilder = new StringBuilder();
        HashSet<MakeBurger.BurgerContent> set = new HashSet<MakeBurger.BurgerContent>();

        // add constructor code
        stringBuilder.Append(
            $"<color={BlueColorCode}>Burger</color> burger = " +
            $"<color={PinkColorCode}>new</color> <color={GreenColorCode}>Burger</color>();\n");

        // add random setter combination
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
            {
                // no setter
                // add default burger content;
                foreach (var defaultPatty in _defaultPatties)
                {
                    set.Add(MapStringToBurgerContent(defaultPatty));
                }

                foreach (var defaultTopping in _defaultToppings)
                {
                    set.Add(MapStringToBurgerContent(defaultTopping));
                }

                set.Add(MapStringToBurgerContent(_defaultSauce));

                break;
            }
            case 1:
            {
                // one setter
                // pick one setter from three type of setters
                int temp = Random.Range(0, 3);
                switch (temp)
                {
                    case 0:
                    {
                        // first type of setter (setPatties)
                        string pattyContentsText = GeneratePattyArrayContent(out var hashSet);
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setPatties</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{pattyContentsText}}});\n");

                        foreach (var patty in hashSet)
                        {
                            set.Add(MapStringToBurgerContent(patty));
                        }

                        foreach (var defaultTopping in _defaultToppings)
                        {
                            set.Add(MapStringToBurgerContent(defaultTopping));
                        }

                        set.Add(MapStringToBurgerContent(_defaultSauce));

                        break;
                    }
                    case 1:
                    {
                        // second type of setter (setToppings)
                        string toppingContentsText = GenerateToppingArrayContent(out var hashSet);
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setToppings</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{toppingContentsText}}});\n");

                        foreach (var topping in hashSet)
                        {
                            set.Add(MapStringToBurgerContent(topping));
                        }

                        foreach (var defaultPatty in _defaultPatties)
                        {
                            set.Add(MapStringToBurgerContent(defaultPatty));
                        }

                        set.Add(MapStringToBurgerContent(_defaultSauce));

                        break;
                    }
                    case 2:
                    {
                        // third type of setter (setSauce)
                        string sauce = Sauces[Random.Range(0, Sauces.Length)];
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setSauce</color>(<color={YellowColorCode}>\"{sauce}\"</color>);\n");

                        foreach (var defaultPatty in _defaultPatties)
                        {
                            set.Add(MapStringToBurgerContent(defaultPatty));
                        }

                        foreach (var defaultTopping in _defaultToppings)
                        {
                            set.Add(MapStringToBurgerContent(defaultTopping));
                        }

                        set.Add(MapStringToBurgerContent(sauce));

                        break;
                    }
                }

                break;
            }
            case 2:
            {
                // two setter
                // pick two setters from three type of setters
                int temp = Random.Range(0, 3);
                switch (temp)
                {
                    case 0:
                    {
                        // first setter (setPatties)
                        string pattyContentsText = GeneratePattyArrayContent(out var hashSet);
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setPatties</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{pattyContentsText}}});\n");

                        foreach (var patty in hashSet)
                        {
                            set.Add(MapStringToBurgerContent(patty));
                        }

                        // second setter (setToppings)
                        string toppingContentsText = GenerateToppingArrayContent(out hashSet);
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setToppings</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{toppingContentsText}}});\n");

                        foreach (var topping in hashSet)
                        {
                            set.Add(MapStringToBurgerContent(topping));
                        }

                        set.Add(MapStringToBurgerContent(_defaultSauce));

                        break;
                    }
                    case 1:
                    {
                        // first setter (setPatties)
                        string pattyContentsText = GeneratePattyArrayContent(out var hashSet);
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setPatties</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{pattyContentsText}}});\n");

                        foreach (var patty in hashSet)
                        {
                            set.Add(MapStringToBurgerContent(patty));
                        }

                        // third setter (setSauce)
                        string sauce = Sauces[Random.Range(0, Sauces.Length)];
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setSauce</color>(<color={YellowColorCode}>\"{sauce}\"</color>);\n");

                        set.Add(MapStringToBurgerContent(sauce));

                        foreach (var defaultTopping in _defaultToppings)
                        {
                            set.Add(MapStringToBurgerContent(defaultTopping));
                        }

                        break;
                    }
                    case 2:
                    {
                        // second setter (setToppings)
                        string toppingContentsText = GenerateToppingArrayContent(out var hashSet);
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setToppings</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{toppingContentsText}}});\n");

                        foreach (var topping in hashSet)
                        {
                            set.Add(MapStringToBurgerContent(topping));
                        }

                        // third setter (setSauce)
                        string sauce = Sauces[Random.Range(0, Sauces.Length)];
                        stringBuilder.Append(
                            $"burger.<color={GreenColorCode}>setSauce</color>(<color={YellowColorCode}>\"{sauce}\"</color>);\n");

                        set.Add(MapStringToBurgerContent(sauce));

                        foreach (var defaultPatty in _defaultPatties)
                        {
                            set.Add(MapStringToBurgerContent(defaultPatty));
                        }

                        break;
                    }
                }

                break;
            }
            case 3:
            {
                // first type of setter (setPatties)
                string pattyContentsText = GeneratePattyArrayContent(out var hashSet);
                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setPatties</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{pattyContentsText}}});\n");

                foreach (var patty in hashSet)
                {
                    set.Add(MapStringToBurgerContent(patty));
                }

                // second setter (setToppings)
                string toppingContentsText = GenerateToppingArrayContent(out hashSet);
                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setToppings</color>(<color={PinkColorCode}>new</color> <color={BlueColorCode}>String[]</color> {{{toppingContentsText}}});\n");

                foreach (var topping in hashSet)
                {
                    set.Add(MapStringToBurgerContent(topping));
                }

                // third setter (setSauce)
                string sauce = Sauces[Random.Range(0, Sauces.Length)];
                stringBuilder.Append(
                    $"burger.<color={GreenColorCode}>setSauce</color>(<color={YellowColorCode}>\"{sauce}\"</color>);\n");

                set.Add(MapStringToBurgerContent(sauce));

                break;
            }
        }

        answerSet = set;
        return stringBuilder.ToString();
    }

    private string GenerateToppingArrayContent(out HashSet<string> content)
    {
        HashSet<string> set = new HashSet<string>();
        string toppingContentsText;

        // Generate default toppings randomly, max 3 toppings
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
            {
                // one default value
                string topping = Toppings[Random.Range(0, Toppings.Length)];
                toppingContentsText = ColorText("\"" + topping + "\"", YellowColorCode);

                set.Add(topping);

                break;
            }
            case 1:
            {
                // two default values
                int[] values = GenerateRandomNumbers(0, Toppings.Length, 2);
                string[] toppings =
                {
                    ColorText("\"" + Toppings[values[0]] + "\"", YellowColorCode),
                    ColorText("\"" + Toppings[values[1]] + "\"", YellowColorCode)
                };
                toppingContentsText = String.Join(", ", toppings);

                set.Add(Toppings[values[0]]);
                set.Add(Toppings[values[1]]);

                break;
            }
            case 2:
            {
                // three default values
                int[] values = GenerateRandomNumbers(0, Toppings.Length, 3);
                string[] toppings =
                {
                    ColorText("\"" + Toppings[values[0]] + "\"", YellowColorCode),
                    ColorText("\"" + Toppings[values[1]] + "\"", YellowColorCode),
                    ColorText("\"" + Toppings[values[2]] + "\"", YellowColorCode)
                };
                toppingContentsText = String.Join(", ", toppings);

                set.Add(Toppings[values[0]]);
                set.Add(Toppings[values[1]]);
                set.Add(Toppings[values[2]]);

                break;
            }
            default:
                throw new Exception();
        }

        content = set;
        return toppingContentsText;
    }

    private string GeneratePattyArrayContent(out HashSet<string> content)
    {
        HashSet<string> set = new HashSet<string>();
        string pattyContentsText;

        // Generate default patties randomly, max 2 patties
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0:
            {
                // one default value
                string patty = Patties[Random.Range(0, Patties.Length)];
                pattyContentsText = ColorText("\"" + patty + "\"", YellowColorCode);

                set.Add(patty);

                break;
            }
            case 1:
            {
                // two default values
                int[] values = GenerateRandomNumbers(0, Patties.Length, 2);
                string[] patties =
                {
                    ColorText("\"" + Patties[values[0]] + "\"", YellowColorCode),
                    ColorText("\"" + Patties[values[1]] + "\"", YellowColorCode)
                };
                pattyContentsText = String.Join(", ", patties);

                set.Add(Patties[values[0]]);
                set.Add(Patties[values[1]]);

                break;
            }
            default:
                throw new Exception();
        }

        content = set;
        return pattyContentsText;
    }

    /// <summary>
    /// Generate n unique number(s) within the range [start, end) randomly.
    /// </summary>
    /// <param name="start">the starting number.</param>
    /// <param name="end">the ending number (exclusive).</param>
    /// <param name="n">the number of unique numbers that needs to be randomly generated.</param>
    /// <returns>Return n unique number(s) within the range [start, end), or null if the input is invalid. </returns>
    private int[] GenerateRandomNumbers(int start, int end, int n)
    {
        if (start > end || n <= 0 || end - start < n) return null;

        int[] arr = new int[end - start];
        for (int i = start, index = 0; i < end; i++)
        {
            arr[index++] = i;
        }

        for (int i = 0; i < arr.Length; i++)
        {
            int rand = Random.Range(0, arr.Length);
            (arr[i], arr[rand]) = (arr[rand], arr[i]);
        }

        int[] res = new int[n];
        Array.Copy(arr, res, n);
        return res;
    }

    /// <summary>
    /// Color a given text by surrounding the text with color tag.
    /// </summary>
    /// <param name="text">Original text to be colored.</param>
    /// <param name="color">Hex code of the color.</param>
    /// <returns>A processed color text.</returns>
    private string ColorText(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
}