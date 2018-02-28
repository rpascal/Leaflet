using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeafletTesting.Web.ViewModels
{
    public class BaseViewModel : IValidatableObject
    {
        public BaseViewModel()
        {
            IsActive = true;
        }

        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "Name can be no longer than 50 characters.")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public bool IsInUse { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Example usage
            //if (true)     Some invalid property
            //    yield return new ValidationResult("Unspecified error", new string[] { "Property name" });

            //if (true)     Another invalid property
            //    yield return new ValidationResult("Unspecified error", new string[] { "Another property name" });

            return new ValidationResult[] { };
        }
    }
}