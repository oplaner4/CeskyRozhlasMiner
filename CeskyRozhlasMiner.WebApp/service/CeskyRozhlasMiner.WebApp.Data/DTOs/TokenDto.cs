using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class TokenDto : AuditDto<int>
    {
        /// <summary>
        /// This field is used only in the request, not in response.
        /// </summary>
        public string Value { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationHelper<TokenDto> helper = new ValidationHelper<TokenDto>(this);
            return helper.CheckStringNotEmptyAndCorrectLength(nameof(Value)).ToList();
        }
    }
}
