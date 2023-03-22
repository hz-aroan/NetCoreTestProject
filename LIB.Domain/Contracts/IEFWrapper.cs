using Infrastructure.SQL.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Contracts;

public interface IEFWrapper
{
    MainDbContext GetContext();

    void SafeFinish(MainDbContext context, String errorMessage = null);
}