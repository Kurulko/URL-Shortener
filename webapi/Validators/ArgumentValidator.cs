using URL_ShortenerAPI.Exceptions;

namespace URL_ShortenerAPI.Validators;

public static class ArgumentValidator
{
    public static void NotNull(string name, object value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }
    }

    public static void NotNullOrEmpty(string name, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullOrEmptyException(name);
        }
    }

    public static void NonLessOneID(string name, int id)
    {
        if (id < 1)
        {
            throw new InvalidIDException(name);
        }
    }
}
