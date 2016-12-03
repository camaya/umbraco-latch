using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UmbracoLatch.Core.Models
{
    public class LatchOperationRequestModel : IValidatableObject
    {

        public LatchOperationRequestModel()
        {
            Users = Enumerable.Empty<int>();
            Nodes = Enumerable.Empty<int>();
        }

        [Required(ErrorMessage = "Please enter an operation name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select the operation type")]
        public string Type { get; set; }
        public string Action { get; set; }
        public bool ApplyToAllUsers { get; set; }
        public IEnumerable<int> Users { get; set; }
        public bool ApplyToAllNodes { get; set; }
        public IEnumerable<int> Nodes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowedTypes = new string[]
            {
                LatchConstants.OperationTypes.Login,
                LatchConstants.OperationTypes.Content,
                LatchConstants.OperationTypes.Media,
                LatchConstants.OperationTypes.Dictionary,
            };
            if (!allowedTypes.Contains(Type))
            {
                yield return new ValidationResult(string.Format("Please select a valid operation type: {0}", string.Join(", ", allowedTypes)));
            }

            var isLoginType = Type.Equals(LatchConstants.OperationTypes.Login, StringComparison.InvariantCultureIgnoreCase);
            if (!isLoginType && string.IsNullOrEmpty(Action))
            {
                yield return new ValidationResult("Please select the operation action");
            }

            var allowedActions = new string[]
            {
                LatchConstants.OperationActions.Delete,
                LatchConstants.OperationActions.Publish,
                LatchConstants.OperationActions.Unpublish
            };
            if (!string.IsNullOrEmpty(Action) && !allowedActions.Contains(Action))
            {
                yield return new ValidationResult(string.Format("Please select a valid action: {0}", string.Join(", ", allowedActions)));
            }

            if (!ApplyToAllUsers && (Users != null && !Users.Any()))
            {
                yield return new ValidationResult("You must provide a list of users when the operation does not apply to all of them.");
            }

            if (!ApplyToAllNodes && (Nodes != null &&!Nodes.Any()))
            {
                yield return new ValidationResult("You must provide a list of nodes when the operation does not apply to all of them.");
            }
        }

    }
}