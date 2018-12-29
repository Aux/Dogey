namespace Dogey
{
    public enum EarningType
    {
        None = -1,       // Not calculated
        Activity,    // By message activity
        Trade,      // By user to user trading
        Gift,       // By admin create points
        Trivia      // By trivia answer
    }
}
