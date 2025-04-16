using Microsoft.AspNetCore.Identity;

namespace MiniReddit;

public class IdentityCoreException : Exception
{
    public IEnumerable<IdentityError> Errors { get; set; }

    public IdentityCoreException(IEnumerable<IdentityError> errors)
    {
        this.Errors = errors;
    }
}
