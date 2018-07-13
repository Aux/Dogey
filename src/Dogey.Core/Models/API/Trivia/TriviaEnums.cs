namespace Dogey
{
    public enum TriviaReponseCode
    {
        Success = 0,
        NoResult = 1,
        InvalidParameter = 2,
        TokenNotFound = 3,
        TokenEmpty = 4, // Session Token has returned all possible questions for the specified query. Resetting the Token is necessary.
    }

    public enum TriviaDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum TriviaType
    {
        Unknown = -1,
        Boolean,
        Multiple
    }

    public enum TriviaCategory
    {
        Any = -1,
        General = 9,
        Books = 10,
        Film = 11,
        Music = 12,
        Theatre = 13,
        Television = 14,
        VideoGames = 15,
        BoardGames = 16,
        Nature = 17,
        Computers = 18,
        Math = 19,
        Mythology = 20,
        Sports = 21,
        Geography = 22,
        History = 23,
        Politics = 24,
        Art = 25,
        Celebrities = 26,
        Animals = 27,
        Vehicles = 28,
        Comics = 29,
        Gadgets = 30,
        Anime = 31,
        Cartoons = 32
    }
}
