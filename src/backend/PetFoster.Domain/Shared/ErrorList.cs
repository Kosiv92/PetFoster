using PetFoster.Domain.Shared;
using System.Collections;

public class ErrorList : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public ErrorList(IEnumerable<Error> errors)
    {
        _errors = [..errors];
    }

    public IEnumerator<Error> GetEnumerator() 
        => _errors.GetEnumerator();    

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public static implicit operator ErrorList(List<Error> errors) 
        => new ErrorList(errors);

    public static implicit operator ErrorList(Error error)
        => new ErrorList([error]);
}
