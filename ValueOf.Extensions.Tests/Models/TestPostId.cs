namespace ValueOf.Extensions.Tests.Models;

public sealed class TestPostId : ValueOf<long, TestPostId>
{
    protected override void Validate()
    {
        var postId = Value;
        if (postId <= 0)
            throw new ArgumentOutOfRangeException(nameof(postId) + $" must be positive integer but is: {postId}");
    }
}