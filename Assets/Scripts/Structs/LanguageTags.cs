
[System.Serializable]
public struct LanguageTags
{
    public string[] LanguageTag; // { "en", "de", "fr", "nl" }

    #region Tags

    #region StartScene

    public string[] DISCLAMER_TITLE;
    public string[] DISCLAMER_TEXT;

    public string[] START_TITEL;
    public string[] START_LEARN;
    public string[] START_TEST;
    public string[] START_SETTINGS;

    #endregion
    #region LearnAndTestScene

    public string[] ALPHABET_TEXT;

    public string[] CONTROLPANEL_SKIP;
    public string[] CONTROLPANEL_EXIT;

    public string[] DEBUG_LETTERTITLE;

    #endregion

    #endregion

}
