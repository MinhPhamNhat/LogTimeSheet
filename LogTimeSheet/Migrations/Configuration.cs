namespace LogTimeSheet.Migrations
{
    using LogTimeSheet.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LogTimeSheet.Models.SystemContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LogTimeSheet.Models.SystemContext context)
        {
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method
			//  to avoid creating duplicate seed data.
			context.Users.AddOrUpdate(
			  u => u.UserId,
			  new User { UserId = "ADMIN", Role = 0, Position = "ADMIN", Name = "ADMIN", Username = "ADMIN", Password = "ADMIN" },
			  new User { UserId = "PM-Minh", Role = 1, Position = "PM", Name = "Minh", Username = "PM001", Password = "123456" },
			  new User { UserId = "PM-Loi", Role = 1, Position = "PM", Name = "Lợi", Username = "PM002", Password = "123456" },
			  new User { UserId = "PM-Nam", Role = 1, Position = "PM", Name = "Nam", Username = "PM003", Password = "123456" },
				new User
				{
					UserId = "Staff-83LM67",
					Role = 2,
					Position = "BA",
					Name = "Astra Shepherd",
					Username = "staff37867",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-02BX54",
					Role = 2,
					Position = "Developer",
					Name = "Shay Yates",
					Username = "staff37243",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-11GJ52",
					Role = 2,
					Position = "BA",
					Name = "Vance Gordon",
					Username = "staff12416",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-11AT45",
					Role = 2,
					Position = "Tester",
					Name = "Maryam Juarez",
					Username = "staff41768",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-92ZI85",
					Role = 2,
					Position = "Tester",
					Name = "Abraham Nixon",
					Username = "staff41466",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-74NK12",
					Role = 2,
					Position = "BA",
					Name = "Graham French",
					Username = "staff69965",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-92FS72",
					Role = 2,
					Position = "Developer",
					Name = "Ian Pitts",
					Username = "staff41856",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-81DA88",
					Role = 2,
					Position = "BA",
					Name = "Keelie Buckner",
					Username = "staff05952",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-12BD68",
					Role = 2,
					Position = "Tester",
					Name = "Mohammad Bowman",
					Username = "staff60892",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-05CP70",
					Role = 2,
					Position = "BA",
					Name = "Colin Peters",
					Username = "staff20300",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-21RR55",
					Role = 2,
					Position = "BA",
					Name = "Isaac Mcbride",
					Username = "staff17822",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-37RK06",
					Role = 2,
					Position = "Developer",
					Name = "Hayden Bray",
					Username = "staff32778",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-82HE38",
					Role = 2,
					Position = "Developer",
					Name = "Darryl Odom",
					Username = "staff66653",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-05AT34",
					Role = 2,
					Position = "Tester",
					Name = "Silas Brewer",
					Username = "staff65731",
					Password = "123456"
				},
				new User
				{
					UserId = "Staff-32OR42",
					Role = 2,
					Position = "Developer",
					Name = "Shana Spencer",
					Username = "staff33419",
					Password = "123456"
				}
			);
		}
    }
}
