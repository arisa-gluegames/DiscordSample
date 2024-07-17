using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class CoreUtils
{
    /// <summary>
    /// Checks if the word provided matches any from the words list, regardless of case
    /// </summary>
    /// <param name="word"></param>
    /// <param name="words"></param>
    /// <returns></returns>
    public static bool ListContainsSameWord(string word, List<string> words)
    {
        for (int i = 0; i < words.Count; i++)
        {
            if (IsSameWord(word, words[i]))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the word provided matches any from the words list, regardless of case
    /// </summary>
    /// <param name="word"></param>
    /// <param name="words"></param>
    /// <returns></returns>
    public static bool ListContainsSameChar(char character, List<char> characters)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (IsSameWord(character.ToString(), characters[i].ToString()))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Compares two strings and returns true if any string is the same
    /// </summary>
    /// <param name="stringToCompare"></param>
    /// <param name="comparedWord"></param>
    /// <returns></returns>
    public static bool StringContainsString(string stringToCompare, string comparedWord)
    {
        string[] wordsToCompare = stringToCompare.Split(' ');
        string[] comparedWords = comparedWord.Split(" ");
        for(int j = 0; j < wordsToCompare.Length; j++)
        {
            for (int i = 0; i < comparedWords.Length; i++)
            {
                if (IsSameWord(wordsToCompare[j], comparedWords[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Compares two string if they are equal (case-insensitive)
    /// </summary>
    /// <param name="input"></param>
    /// <param name="word"></param>
    /// <returns></returns>
    public static bool IsSameWord(string input, string word, bool ignoreSpecialCharacters = false)
    {
        if (ignoreSpecialCharacters)
        {
            return string.Equals(input, word, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            int specialLimit = CountNonAlphaCharacters(word);
            string removedSpecial = RemoveSpecialOccurrences(word, specialLimit);
            bool sameOriginalWord = string.Equals(input, word, StringComparison.OrdinalIgnoreCase);
            bool sameModifiedWord = string.Equals(input, removedSpecial, StringComparison.OrdinalIgnoreCase);
            return sameOriginalWord || sameModifiedWord;
        }
    }

    public static int CountMatchingLetters(string str1, string str2)
    {
        int minLength = Math.Min(str1.Length, str2.Length);
        int matchCount = 0;

        str1 = str1.ToLower();
        str2 = str2.ToLower();

        for (int i = 0; i < minLength; i++)
        {
            if (str1[i] == str2[i])
            {
                matchCount++;
            }
        }

        return matchCount;
    }

    public static int CountNonAlphaCharacters(string input)
    {
        Regex regex = new("[^a-zA-Z]");
        MatchCollection matches = regex.Matches(input);
        return matches.Count;
    }

    public static string RemoveSpecialOccurrences(string input, int occurrencesToRemove)
    {
        string pattern = @"[^a-zA-Z0-9\s]";
        for (int i = 0; i < occurrencesToRemove; i++)
        {
            Regex regex = new(pattern);
            Match match = regex.Match(input);
            if (!match.Success)
                break;
            input = input.Remove(match.Index, 1);
        }
        return input;
    }

    /// <summary>
    /// Replaces part of a string with a given string
    /// </summary>
    /// <param name="input"></param>
    /// <param name="word"></param>
    /// <returns></returns>
    public static string ReplaceString(string input, string stringToReplace, string replacement)
    {
        // Use the Replace method to replace occurrences of stringToReplace with the replacement string
        return input.Replace(stringToReplace, replacement, StringComparison.OrdinalIgnoreCase);
    }

    public static char GenerateRandomAlphabet()
    {
        // Generate a random number between 0 and 25 (inclusive) to represent the alphabet index
        int randomIndex = UnityEngine.Random.Range(0, 26);
        char randomChar = (char)('A' + randomIndex);

        return randomChar;
    }

    public static List<char> GetAlphabetCharacters(string input, int minimumCharacters)
    {
        List<char> chars = new();
        foreach (char c in input)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                chars.Add(c);
            }
        }
        if (chars.Count < minimumCharacters)
        {
            int difference = minimumCharacters - chars.Count;
            for (int i = 0; i < difference; i++)
            {
                chars.Add(CoreUtils.GenerateRandomAlphabet());
            }
        }
        Shuffle(chars);
        return chars;
    }

    public static List<char> GetUniqueCharacters(List<char> chars)
    {
        HashSet<char> uniqueChars = new();
        foreach (char c in chars)
        {
            char lowerCaseChar = char.ToLowerInvariant(c);
            uniqueChars.Add(lowerCaseChar);
        }
        List<char> unique = new(uniqueChars);
        Shuffle(unique);
        return unique;
    }


    public static void Shuffle<T>(List<T> list)
    {
        Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public static int MapFloatToInt(float value, float inMin, float inMax, int outMin, int outMax)
    {
        return (int)UnityEngine.Mathf.Lerp(outMin, outMax, UnityEngine.Mathf.InverseLerp(inMin, inMax, value));
    }
}
