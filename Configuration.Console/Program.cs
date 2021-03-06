// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CmdLine;

namespace Its.Configuration.Console
{
    public class Program
    {
        private static void Main()
        {
            ConsoleParameters parameters = null;

            // parse the command line
            try
            {
                parameters = CommandLine.Parse<ConsoleParameters>();
            }
            catch (CommandLineHelpException helpException)
            {
                // User asked for help 
                System.Console.WriteLine(helpException.ArgumentHelp.GetHelpText(System.Console.BufferWidth));
                Environment.Exit(1);
            }
            catch (CommandLineException exception)
            {
                System.Console.Error.WriteLine("!! {0}\n", exception.ArgumentHelp.Message);
                System.Console.Error.WriteLine(exception.ArgumentHelp.GetHelpText(System.Console.BufferWidth));
                Environment.Exit(1);
            }

            try
            {
                RunCommand(parameters);
                Environment.Exit(0);
            }
            catch (Exception exception)
            {
                System.Console.Error.WriteLine(exception.ToString());
                Environment.Exit(1);
            }
        }

        public static void RunCommand(ConsoleParameters parameters)
        {
            Validate(parameters);

            switch (parameters.Command.ToLowerInvariant())
            {
                case "encrypt":
                    System.Console.WriteLine(Encrypt(parameters));
                    break;
                case "decrypt":
                    System.Console.WriteLine(Decrypt(parameters));
                    break;
                default:
                    System.Console.Error.WriteLine("Command {0} not supported.", parameters.Command);
                    Environment.Exit(1);
                    break;
            }
        }

        private static void Validate(ConsoleParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.FileSpec) && !string.IsNullOrWhiteSpace(parameters.Text))
            {
                System.Console.Error.WriteLine("You cannot specify both the /f and /t switches.");
                Environment.Exit(1);
            }

            if (string.IsNullOrWhiteSpace(parameters.FileSpec) && string.IsNullOrWhiteSpace(parameters.Text))
            {
                System.Console.Error.WriteLine("You must specify either the /f or /t switch.");
                Environment.Exit(1);
            }
        }

        public static string Encrypt(ConsoleParameters parameters)
        {
            var plaintext = GetText(parameters);
            return plaintext.Encrypt(new X509Certificate2(parameters.Certificate, parameters.Password));
        }

        public static string Decrypt(ConsoleParameters parameters)
        {
            var cipherText = GetText(parameters);
            return cipherText.Decrypt(new X509Certificate2(parameters.Certificate, parameters.Password));
        }

        private static string GetText(ConsoleParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.FileSpec))
            {
                return File.ReadAllText(parameters.FileSpec);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Text))
            {
                return parameters.Text;
            }

            return string.Empty;
        }
    }
}