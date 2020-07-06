using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LearnManger
{
    // private static List<char> _AllLetters = new List<char>();

    private static List<char> _NewLetters = new List<char>();
    private static List<char> _LearnedLetters = new List<char>();
    private static List<char> _MasterdLetters = new List<char>();

    public static List<char>[] _AllLetters = { _NewLetters, _LearnedLetters, _MasterdLetters };

    private static int[] _LearnOrder = { 0, 1, 2 };
    private static int[] _TestOrder = { 1, 2, 0 };

    public static bool IsInitalized = false;

    private static char _CurrentLetter;

    public static void Init(char[] alphabet)
    {
        foreach (char letter in alphabet)
            _AllLetters[0].Add(letter);

        _AllLetters[1].Clear();
        _AllLetters[2].Clear();

        IsInitalized = true;
    }

    #region GetLetter
    public static char GetLetterMode(bool TestMode)
    {
        if(TestMode)
        {
            _CurrentLetter = GetLetter(_TestOrder);
        }
        else
        {
            _CurrentLetter = GetLetter(_LearnOrder);
        }
        return _CurrentLetter;

    }

    private static char GetLetter(int[] order)
    {
        if (IsInitalized)
        {
            char letter;

            for (int i = 0; i < _AllLetters.Length; i++)
            {
                if (GetRndLetterFromList(_AllLetters[order[i]], out letter))
                    return letter;
            }
        }

        return GetLetter(order);
    }

    private static bool GetRndLetterFromList(List<char> list, out char t)
    {
        t = ' ';
        if (list.Count > 0)
        {
            t = list[Random.Range(0, list.Count)];
            return _CurrentLetter != t;

        }
        return false;
    }
    #endregion

    #region IncreaseScore

    public static void IncreaseLetterScore(char letter, bool Testmode)
    {
        if(Testmode)
        {
            MasterdLetter(letter);
        }
        else
        {
            LearendLetter(letter);
        }
    }

    private static void LearendLetter(char letter)
    {
        _AllLetters[0].Remove(letter);
        _AllLetters[1].Add(letter);
    }

    private static void MasterdLetter(char letter)
    {
        _AllLetters[1].Remove(letter);
        _AllLetters[2].Add(letter);
    }

    #endregion
}
