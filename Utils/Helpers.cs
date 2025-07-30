using Farmer.Data.API.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Farmer.Data.API.Utils
{
    public class Helpers
    {
        private static PagedResponseModel<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilterModel validFilter, int totalRecords, IUriService uriService, string route)
        {
            var respose = new PagedResponseModel<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            respose.NextPage =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilterModel(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null;
            respose.PreviousPage =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilterModel(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null;
            respose.FirstPage = uriService.GetPageUri(new PaginationFilterModel(1, validFilter.PageSize), route);
            respose.LastPage = uriService.GetPageUri(new PaginationFilterModel(roundedTotalPages, validFilter.PageSize), route);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;
            return respose;
        }


        public static PagedResponseModel<List<T>> GeneratePagedResponse<T>(List<T> data, PaginationFilterModel validFilter, IUriService uriService, string route)
        {
            var pagedData = data
.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
.Take(validFilter.PageSize).ToList();
            var totalRecords = data.Count;
            var pagedReponse = CreatePagedReponse<T>(pagedData, validFilter, totalRecords, uriService, route);
            return pagedReponse;
        }


        public static string Clean(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 1. Decompose accented chars into base + diacritic
            var normalized = input.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder(input.Length);
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);

                // skip diacritics
                if (category == UnicodeCategory.NonSpacingMark)
                    continue;

                // skip control, format, private use, unassigned
                if (category == UnicodeCategory.Control
                 || category == UnicodeCategory.Format
                 || category == UnicodeCategory.PrivateUse
                 || category == UnicodeCategory.OtherNotAssigned)
                    continue;

                // keep letters, digits, punctuation, separators (spaces)
                if (char.IsLetterOrDigit(c)
                 || char.IsPunctuation(c)
                 || char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }

                // otherwise drop (e.g. currency symbols, math symbols, etc.)
            }

            // 2. Recompose and return
            return sb
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// CleanStrict: removes everything except letters, digits & single spaces, 
        /// forces ASCII, and lower-cases.
        /// </summary>
        public static string CleanStrict(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Step 1: base cleaning (remove unseen control/unassigned)
            var intermediate = Clean(input);

            var sb = new StringBuilder(intermediate.Length);
            foreach (var c in intermediate)
            {
                // keep only ASCII letters or digits or whitespace
                if (c <= 127
                 && (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                {
                    sb.Append(c);
                }
            }

            // Step 2: collapse multiple whitespace into one space
            var collapsed = Regex.Replace(sb.ToString(), @"\s+", " ").Trim();

            // Step 3: normalize case
            return collapsed.Replace(" ", "").ToLowerInvariant();
        }
    }
}
