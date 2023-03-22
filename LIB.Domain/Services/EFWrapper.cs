using Infrastructure.SQL.Main;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.Domain.Contracts;
using LIB.Domain.Exceptions;

namespace LIB.Domain.Services;

public class EFWrapper : IEFWrapper
{
    private readonly IDbContextFactory<MainDbContext> DbctxFactory;
    public EFWrapper(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public MainDbContext GetContext()
    {
        return DbctxFactory.CreateDbContext();
    }



    public void SafeFinish(MainDbContext context, String errorMessage=null)
    {
        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            var message = errorMessage ?? "Error happened finishing the operation!";
            throw new DomainException(errorMessage, ex);
        }
    }
}
