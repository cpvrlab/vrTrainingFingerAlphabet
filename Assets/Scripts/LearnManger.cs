using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LearningState
{
    New = 0,
    Learned = 1,
    Masterd = 2
}
public struct LetterState
{
    public char Letter;
    public LearningState LearnState;
    public int SkippedAmount;

    public LetterState(char letter, LearningState state)
    {
        Letter = letter;
        LearnState = state;
        SkippedAmount = 0;
    }
}

// TODO, Test, and make scene more nice!
// TODO this is trash
public static class LearnManger
{

    public static char[] AllLetters;
    public static int AmountOfSkip;

    // TODO make that you can spell words?

    private static LetterState[] _LetterStates;
    private static int _NewLetterAmount;
    private static int _LearndLetterAmount;
    private static int _MasterdLetterAmount;
    private static LearningState _CurrentLearnState;
    public static LetterState _CurrentLetter;

    public static void Init()
    {
        _LetterStates = new LetterState[AllLetters.Length];
        for(int i = 0; i < AllLetters.Length; i++)
        {
            _LetterStates[i] = new LetterState(AllLetters[i], LearningState.New);
        }
        _NewLetterAmount = AllLetters.Length;
        _LearndLetterAmount = 0;
        _MasterdLetterAmount = 0;

        _CurrentLetter = GetRandomLetter(LearningState.New);
        _CurrentLearnState = LearningState.New;
    }
    

    public static char GetNewLetterChar( LearningState goal, bool skiped = true)
    {
        if(GetStateAmount(goal - 1) > 0)
            _CurrentLearnState = goal - 1;

        if (_LetterStates.Length < 1)
            Init();

        if(skiped)
        {
            _CurrentLetter.SkippedAmount++;
            if(_CurrentLetter.SkippedAmount > AmountOfSkip)
            {
                ChangeToNextState(goal);
            }
        }
        else
        {
            ChangeToNextState(goal);
        }
        
        _CurrentLetter = GetRandomLetter(_CurrentLearnState);

        return _CurrentLetter.Letter;
    }



    
    private static LetterState GetRandomLetter(LearningState stateType)
    {
        switch(stateType)
        {
            case LearningState.New:
                return _LetterStates[Random.Range(0, _NewLetterAmount)];
            case LearningState.Learned:
                return _LetterStates[Random.Range(0, _LearndLetterAmount) + _NewLetterAmount];
            case LearningState.Masterd:
                return _LetterStates[Random.Range(0, _MasterdLetterAmount) + _NewLetterAmount + _LearndLetterAmount];
            default:
                return _LetterStates[Random.Range(0, _LetterStates.Length)];
        }
    }

    private static void ChangeToNextState(LearningState state)
    {
        if (_CurrentLetter.LearnState.Equals(state))
            return;
        _CurrentLetter.LearnState = state;
        IncreaseAmount(_CurrentLearnState);
        SortCurrentInLetters();
    }

    // TODO swap current with biggest smaller
    private static void SortCurrentInLetters()
    {
        // insertion sort
        int indexCurrent = GetIndex<LetterState>(_CurrentLetter, _LetterStates);

        for(int i = 1; i < _LetterStates.Length; i++)
        {
            if(_LetterStates[i].LearnState > _CurrentLetter.LearnState)
            {
                SwapLetterStates(_CurrentLetter, indexCurrent, _LetterStates[i-1], i-1);
            }

            if(i == _LetterStates.Length-1)
            {
                SwapLetterStates(_CurrentLetter, indexCurrent, _LetterStates[i], i);
            }

        }
    }

    private static void IncreaseAmount(LearningState state)
    {
        switch(state)
        {
            case LearningState.New:
                _NewLetterAmount++;
                break;
            case LearningState.Learned:
                _LearndLetterAmount++;
                _NewLetterAmount--;
                break;
            case LearningState.Masterd:
                _MasterdLetterAmount++;
                _LearndLetterAmount--;
                break;
        }

        if(_NewLetterAmount < 1)
        {
            _CurrentLearnState = LearningState.Learned;
        }
        if(_LearndLetterAmount < 1)
        {
            _CurrentLearnState = LearningState.Masterd;
        }
    }

    private static int GetStateAmount(LearningState state)
    {
        switch (state)
        {
            case LearningState.New:
                return _NewLetterAmount;
            case LearningState.Learned:
                return _LearndLetterAmount;
            case LearningState.Masterd:
                return _MasterdLetterAmount;
        }
        return 0;
    }

    private static void SwapLetterStates(LetterState element1,
                                         int index1, 
                                         LetterState element2, 
                                         int index2)
    {
        _LetterStates[index1] = element2;
        _LetterStates[index2] = element1;

    }

    private static int GetIndex<T>(T element, T[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(element))
                return i;
        }
        return -1;
    }
    /// <summary>
    /// Displays a new letter on the AlphabetScreen
    /// </summary>
    /// <returns>The new letter that is displayed on the AlphabetScreen</returns>
    //private static char GetNewLetter()
    //{
    //    char rndLetter = GetRandomAlphabetLetter();
    //    if (_CurrentLetter == rndLetter)
    //    {
    //        return GetNewLetter();
    //    }
    //    _CurrentLetter = rndLetter;
    //    CurrentLetterText.text = _CurrentLetter.ToString();

    //    return _CurrentLetter;
    //}


}
