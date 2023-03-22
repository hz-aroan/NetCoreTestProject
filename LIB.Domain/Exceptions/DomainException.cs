using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(String message)
        : base(message)
    {
    }



    public DomainException(String message, Exception inner)
        : base(message, inner)
    {
    }
}