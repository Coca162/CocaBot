using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Database;

public class Transaction
{
    [Key]
    public int Count { get; set; }

    [SVID]
    public string FromAccount { get; set; }

    [SVID]
    public string ToAccount { get; set; }

    public decimal Amount { get; set; }

    [VarChar(255)]
    public string Detail { get; set; }

    public bool Force { get; set; }

    public bool? Success { get; set; }

    public ApplicableTax Tax { get; set; }

    public long Timestamp { get; set; }
}

public enum ApplicableTax
{
    None = 0, Corporate = 1, Payroll = 2, CapitalGains = 3, Sales = 4
}