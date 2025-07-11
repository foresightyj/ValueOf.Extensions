using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ValueOf.Extensions.Examples.Models;

[TypeConverter(typeof(ValueOfTypeConverter<string, EmailAddress>))]
public sealed class EmailAddress : ValueOf<string, EmailAddress>
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