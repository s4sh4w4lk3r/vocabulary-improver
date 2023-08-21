namespace ViApi.Types.Common;

public static class RatingEdtior
{
    private const int MAX_RATING = 10;
    private const int MIN_RATING = 0;
    public static int IncreaseRating(int rating)
    {
        if (rating < MAX_RATING)
        {
            return rating++;
        }
        else return rating;
    }
    public static int DecreaseRating(int rating)
    {
        if (rating < MIN_RATING)
        {
            return rating--;
        }
        else return rating;
    }
}
