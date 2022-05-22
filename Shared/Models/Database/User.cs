using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Database;

public class User
{
    [Key]
    public string SVID { get; set; }

    [VarChar(64)]
    public string Token { get; set; }

    [BigInt]
    public ulong? Valour { get; set; }

    [BigInt]
    public ulong? Discord { get; set; }

    [VarChar(32)]
    public string ValourName { get; set; }

    public void Deconstruct(out string svid, out string token)
    {
        svid = SVID;
        token = Token;
    }
}