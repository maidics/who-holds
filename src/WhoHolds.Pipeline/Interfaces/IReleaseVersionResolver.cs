namespace WhoHolds.Pipeline.Interfaces;

public interface IReleaseVersionResolver
{
    string Resolve(string tagName);
}
