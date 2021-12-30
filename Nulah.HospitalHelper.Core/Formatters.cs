using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core
{
    public static class Formatters
    {
        /// <summary>
        /// Returns a persons display name given their first and last display names.
        /// <para>
        /// If <paramref name="displayLastName"/> is null, <paramref name="displayFirstName"/> will be returned,
        /// otherwise "<paramref name="displayFirstName"/> <paramref name="displayLastName"/>" will be returned
        /// </para>
        /// </summary>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <returns></returns>
        public static string PersonNameToDisplayFormat(string displayFirstName, string? displayLastName)
        {
            if (string.IsNullOrWhiteSpace(displayLastName))
            {
                return displayFirstName;
            }

            return $"{displayFirstName} {displayLastName}";
        }
    }
}
