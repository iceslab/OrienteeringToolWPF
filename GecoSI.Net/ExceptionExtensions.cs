//  
//  Copyright (c) 2013-2014 Simon Denier & Yannis Guedel
//  
using System;

namespace GecoSI.Net
{
    public static class ExceptionExtensions
    {
        public static void PrintStackTrace(this Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}