using System.Text.RegularExpressions;

namespace ValueOf.Extensions.Tests.Models;

public class TestEmail : ValueOf<string, TestEmail>
{
    private static readonly Regex EmailPatt = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

    protected override void Validate()
    {
        var email = Value;
        if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
        if (!EmailPatt.IsMatch(email))
            throw new ArgumentException(nameof(email) + $" has invalid format, value: {Value}");
    }
}