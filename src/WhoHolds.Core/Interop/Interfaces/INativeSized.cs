namespace WhoHolds.Core.Interop.Interfaces;

internal interface INativeSized<TSelf>
    where TSelf : unmanaged, INativeSized<TSelf> // constraints implementation
{
    static abstract int Size { get; } // used for testing and guards
}
