using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Database;

public class SVID : VarChar
{
    public SVID() : base(38) { }
}

public class VarChar : ColumnAttribute
{
    public VarChar(int length)
    {
        TypeName = $"VARCHAR({length})";
    }
}

public class BigInt : ColumnAttribute
{
    public BigInt()
    {
        TypeName = $"BIGINT";
    }
}