namespace Dogey
{
    public enum EarningType
    {
        None = -1,       // Not calculated
        Message,    // By message id calc
        Trade,      // By user to user trading
        Gift,       // By admin create points
        Trivia      // By trivia answer
    }
}
