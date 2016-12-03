using System.ComponentModel.DataAnnotations;

namespace UmbracoLatch.Core.Models
{
    public class LatchApplicationRequestModel
    {
        [Required(ErrorMessage = "Pleasae enter an application id")]
        public string ApplicationId { get; set; }

        [Required(ErrorMessage = "Please enter a secret")]
        public string Secret { get; set; }
    }
}
