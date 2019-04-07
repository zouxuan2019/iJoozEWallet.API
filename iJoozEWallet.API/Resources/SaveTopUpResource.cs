using System.ComponentModel.DataAnnotations;

namespace iJoozEWallet.API.Resources
{
    public class SaveTopUpResource
    {
        [Required] public double TopUpCredit { get; set; }

        [MaxLength(100)] public string TopUpSource { get; set; }

        [Required] public int UserId { get; set; }
    }
}