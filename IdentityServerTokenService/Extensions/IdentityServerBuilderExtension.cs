using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerTokenService.Extensions
{
    internal static class IdentityServerBuilderExtension
    {
        internal static T When<T>(this T source, Func<bool> predicate, Func<T, T> then, Func<T, T> @else = null)
        {
            if (predicate?.Invoke() ?? default(bool))
            {
                then?.Invoke(source);
            }
            else
            {
                @else?.Invoke(source);
            }

            return source;
        }
    }
}
