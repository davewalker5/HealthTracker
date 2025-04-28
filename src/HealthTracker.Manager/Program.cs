using HealthTracker.Data;
using HealthTracker.Logic.Factory;
using HealthTracker.Manager.Logic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using HealthTracker.Manager.Entities;

namespace HealthTracker.Terminal
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                // Get the version number and write the application title
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
                Console.WriteLine($"\nHealth Tracker Database Management Application v{info.FileVersion}");

                // Parse the command line
                CommandLineParser parser = new(new HelpTabulator());
                parser.Add(CommandLineOptionType.Help, true, "--help", "-h", "Show command line help", "", 0, 0);
                parser.Add(CommandLineOptionType.AddUser, true, "--add", "-a", "Add a new user", "username password", 2, 2);
                parser.Add(CommandLineOptionType.SetPassword, true, "--set-password", "-sp", "Set the password for a user", "username password", 2, 2);
                parser.Add(CommandLineOptionType.DeleteUser, true, "--delete", "-d", "Delete a user", "username", 1, 1);
                parser.Add(CommandLineOptionType.Update, true, "--update", "-u", "Apply the latest database migrations", "", 0, 0);
                parser.Parse(args);

                // If help's been requested, show help and exit
                if (parser.IsPresent(CommandLineOptionType.Help))
                {
                    parser.Help();
                }
                else
                {
                    // Configure the business logic factory
                    var context = new HealthTrackerDbContextFactory().CreateDbContext([]);
                    var factory = new HealthTrackerFactory(context, null, null);

                    // Handle new user additions
                    if (parser.IsPresent(CommandLineOptionType.AddUser))
                    {
                        var values = parser.GetValues(CommandLineOptionType.AddUser);
                        await factory.Users.AddAsync(values[0], values[1]);
                        Console.WriteLine($"Added new user {values[0]}");
                    }

                    // Handle user password updates
                    if (parser.IsPresent(CommandLineOptionType.SetPassword))
                    {
                        var values = parser.GetValues(CommandLineOptionType.SetPassword);
                        await factory.Users.SetPasswordAsync(values[0], values[1]);
                        Console.WriteLine($"Updated the password for user {values[0]}");
                    }

                    // Handle existing user deletions
                    if (parser.IsPresent(CommandLineOptionType.DeleteUser))
                    {
                        var values = parser.GetValues(CommandLineOptionType.DeleteUser);
                        await factory.Users.DeleteAsync(values[0]);
                        Console.WriteLine($"Deleted user {values[0]}");
                    }

                    // Handle database updates
                    if (parser.IsPresent(CommandLineOptionType.Update))
                    {
                        context.Database.Migrate();
                        Console.WriteLine($"Applied the latest database migrations");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
