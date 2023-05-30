using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Text.RegularExpressions;

namespace Test;

public class RepoTestBase
{
    public ILogger Log;
    protected ISender Dispatcher;
    internal IHost? Host;
    internal IDbContextFactory<MainDbContext> DbctxFactory;

    internal ICurrencyHandlingService CurrencyHandlingService;


    public void Init()
    {
        Host = TestHostConfiguration.Configure();
        DbctxFactory = Host.Services.GetRequiredService<IDbContextFactory<MainDbContext>>();
        Log = Host.Services.GetService<ILogger<RepoTestBase>>();
        Dispatcher = Host.Services.GetService<ISender>();
        CurrencyHandlingService = Host.Services.GetService<ICurrencyHandlingService>();
    }


    


    internal void ClearDatabase()
    {
        using var ctx = DbctxFactory.CreateDbContext();
        ctx.Database.ExecuteSqlRaw("DELETE FROM dbo.BasketItem");
        ctx.Database.ExecuteSqlRaw("DELETE FROM dbo.BasketHead");
        ctx.Database.ExecuteSqlRaw("DELETE FROM dbo.Product");
        ctx.Database.ExecuteSqlRaw("DELETE FROM dbo.Event");
    }


    public void ExecuteWithLog(Type expectedExceptionType, Action codeBlock)
    {
        try
        {
            codeBlock();
            Log.Log(LogLevel.Error, $"Expected exception was not thrown: Expected type: {expectedExceptionType.FullName}");
            Assert.Fail("Exception was expected but didn't happen");
        }
        catch (Exception e)
        {
            if (e.GetType() != expectedExceptionType)
            {
                Log.Log(LogLevel.Error,$@"Expected exception with a specific type - other type was thrown
                        Expected type: {expectedExceptionType.FullName}
                        Current type: {e.GetType().FullName}
                        Current message: {e.Message}");
                Assert.Fail("Different type of exception was expected");
            }
        }
    }



    public void ExceptionExpected(Type expectedExceptionType, String expectedMessagePattern, Action codeBlock)
    {
        try
        {
            codeBlock();
            Log.Log(LogLevel.Error, $@"Expected exception was not thrown: 
                Expected type: {expectedExceptionType.FullName}
                Expected message: {expectedMessagePattern}");
            Assert.Fail("Exception was expected but didn't happen");
        }
        catch (Exception e)
        {
            if (e.GetType() != expectedExceptionType)
            {
                Log.Log(LogLevel.Error, $@"Expected exception with a specific type - other type was thrown
                        Expected type: {expectedExceptionType.FullName}
                        Current type: {e.GetType().FullName}
                        Expected message: {expectedMessagePattern}
                        Current message: {e.Message}");
                Assert.Fail("Different type of exception was expected");
            }
            else if (IsMessageMatches(e.Message, expectedMessagePattern) == false)
            {
                Log.Log(LogLevel.Error, $@"Expected exception with a specific type was thrown, but different message
                        Expected type: {expectedExceptionType.FullName}
                        Expected message: {expectedMessagePattern}
                        Current message: {e.Message}");
                Assert.Fail("Exception with different message was expected");
            }
        }
    }



    public void SuccessExpected(Action codeBlock)
    {
        try
        {
            codeBlock();
            Log.Log(LogLevel.Information, " * test code block finished successfully.");
        }
        catch (Exception e)
        {
            Log.Log(LogLevel.Error, e.ToString());
            throw;
        }
    }



    internal static Boolean IsMessageMatches(String currentMessage, String pattern)
    {
        var regex = Resolve(pattern);
        var result = regex.IsMatch(currentMessage ?? "");
        return result;
    }



    internal static Regex Resolve(String pattern, Boolean caseSensitivity = false)
    {
        const String integerPattern = @"[0-9]*";
        const String numberPattern = @"[\-?0-9\.\,]*";
        const String datePattern = @"\d{4}[-/.]\d{1,2}[-/.]\d{1,2}";
        const String timePattern = @"\d{1,2}:\d{1,2}:\d{1,2}";
        const String stringPattern = @".+";
        const String dateTimePattern = datePattern + @"[\ |T]" + timePattern;
        const String whiteSpacesPattern = @"\s*";

        RegexOptions options = RegexOptions.IgnorePatternWhitespace | (caseSensitivity ? 0 : RegexOptions.IgnoreCase);

        if (pattern.StartsWith("[REGEX]"))
            return new Regex(pattern.Substring(7), options);

        var resolvedPattern = " " + pattern
                                      .Replace("\\", "\\\\")
                                      .Replace("?", "\\?")
                                      .Replace(".", "\\.")
                                      .Replace("*", "\\*")
                                      .Replace("+", "\\+")
                                      .Replace("{TODAY}", DateTime.Now.ToString("yyyy-MM-dd"))
                                      .Replace("{INTEGER}", integerPattern)
                                      .Replace("{INT}", integerPattern)
                                      .Replace("{NUMBER}", numberPattern)
                                      .Replace("{NUM}", numberPattern)
                                      .Replace("{DATE}", datePattern)
                                      .Replace("{STRING}", stringPattern)
                                      .Replace("{DATETIME}", dateTimePattern)
                                      .Replace("(", "\\(")
                                      .Replace(")", "\\)")
                                  + " ";

        var patternWithWhiteSpaces = resolvedPattern.Replace(" ", whiteSpacesPattern);

        return new Regex(patternWithWhiteSpaces, options);
    }
}

