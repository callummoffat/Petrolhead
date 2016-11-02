using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Petrolhead.Azure.DataObjects;
using Petrolhead.Azure.Models;
using Owin;

namespace Petrolhead.Azure
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Vehicle, VehicleDTO>()
                .ForMember(vehicleDTO => vehicleDTO.Expenses, map => map.MapFrom(vehicle => vehicle.Expenses))
                .ForMember(vehicleDTO => vehicleDTO.Refuels, map => map.MapFrom(vehicle => vehicle.Refuels))
                .ForMember(vehicleDTO => vehicleDTO.Repairs, map => map.MapFrom(vehicle => vehicle.Repairs));

                cfg.CreateMap<VehicleDTO, Vehicle>()
                .ForMember(vehicle => vehicle.Expenses, map => map.MapFrom(vehicleDTO => vehicleDTO.Expenses))
                .ForMember(vehicle => vehicle.Refuels, map => map.MapFrom(vehicleDTO => vehicleDTO.Refuels))
                .ForMember(vehicle => vehicle.Repairs, map => map.MapFrom(vehicleDTO => vehicleDTO.Repairs));

                cfg.CreateMap<Expense, ExpenseDTO>()
                .ForMember(expenseDTO => expenseDTO.Purchases, map => map.MapFrom(expense => expense.Purchases));

                cfg.CreateMap<ExpenseDTO, Expense>()
                .ForMember(expense => expense.Purchases, map => map.MapFrom(expenseDTO => expenseDTO.Purchases));

                cfg.CreateMap<Repair, RepairDTO>()
                .ForMember(repairDTO => repairDTO.Components, map => map.MapFrom(repair => repair.Components));
                cfg.CreateMap<RepairDTO, Repair>()
                .ForMember(repair => repair.Components, map => map.MapFrom(repairDTO => repairDTO.Components));

                cfg.CreateMap<Refuel, RefuelDTO>();
                cfg.CreateMap<RefuelDTO, Refuel>();

                cfg.CreateMap<Component, ComponentDTO>();
                cfg.CreateMap<ComponentDTO, Component>();


                cfg.CreateMap<Purchase, PurchaseDTO>();
                cfg.CreateMap<PurchaseDTO, Purchase>();


            });

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new PetrolheadAppInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public sealed class PetrolheadAppInitializer : DropCreateDatabaseIfModelChanges<PetrolheadAppContext>
    {
        protected override void Seed(PetrolheadAppContext context)
        {

            List<Refuel> refuels = new List<Refuel>()
            {
                new Refuel()
                {
                    Id = "RF1",
                    Name = "Seattle Refuel",
                    Description = "On the way to Microsoft!",
                    Location = "Redmond, Washington",
                    DateRefueled = new DateTime(2003, 1, 1),
                    AmountRefueledBy = 125.76M,
                    Cost = 210.99M,
                },
                new Refuel()
                {
                    Id = "RF2",
                    Name = "Whakatane Refuel",
                    Description = "On the way to Ohope",
                    Location = "Whakatane, New Zealand",
                    DateRefueled = new DateTime(2009, 10, 6),
                    AmountRefueledBy = 125.76M,
                    Cost = 210.99M
                }
            };

            List<Component> components = new List<Component>()
            {
                new Component()
                {
                    Id = "C1",
                    Name = "Wheel",
                    ComponentModel = "W-2D",
                    ComponentType = "Wheel",
                    Description = "The wheels on the bus go round and round",
                    DateFailed = new DateTime(2000, 1, 1),
                    DateRepaired = new DateTime(2000, 1, 3),
                    Cost = 550
                },
                new Component()
                {
                    Id = "C2",
                    Name = "Engine",
                    ComponentModel = "E-27X",
                    ComponentType = "Engine",
                    Description = "VROOM, VROOM!",
                    DateFailed = new DateTime(2000, 1, 1),
                    DateRepaired = new DateTime(2000, 1, 3),
                    Cost = 3269.99M
                },
            };

            List<Repair> repairs = new List<Repair>()
            {
                new Repair()
                {
                    Id = "RP1",
                    Name = "The Mother of All Breakdowns",
                    Description = "Vehicle went ka-boom!",
                    DateOfTransaction = new DateTime(2000, 1, 4),
                    Cost = 3819.99M,
                    Components = components,
                }
            };

            List<Purchase> purchases = new List<Purchase>()
            {
                 new Purchase()
                {
                    Id = "P1",
                    Name = "New Car",
                    Description = "Best. Car. Ever!",
                    DatePurchased = new DateTime(2005, 10, 1),
                    PurchasedAtLocation = "Auckland, New Zealand",
                    Cost = 49999.99M,
                },
                new Purchase()
                {
                    Id = "P2",
                    Name = "Fuzzy Dice",
                    Description = "Every car needs a pair of these!",
                    DatePurchased = new DateTime(2005, 10, 1),
                    PurchasedAtLocation = "Auckland, New Zealand",
                    Cost = 4.99m,
                },

            };

            List<Expense> expenses = new List<Expense>()
            {
                new Expense()
                {
                    Id = "X1",
                    Name = "Stationwagon",
                    Description = "A necessary bankruptcy",
                    DateOfTransaction = new DateTime(2005, 10, 1),
                    Cost = 50004.98M,
                    Purchases = purchases,
                }
            };

            List<Vehicle> vehicles = new List<Vehicle>()
            {
                new Vehicle()
                {
                    Id = "V1",
                    Name = "Ol' Deathtrap",
                    Description = "Held together by duct tape, literally.",
                    Manufacturer = "Holden",
                    Make = "Commodore",
                    Model = "V6",
                    ModelIdentifier = "23-D",
                    EngineType = "V6",
                    ChassisType = "Stationwagon",
                    NumberOfSeats = 4,
                    NumberOfWheels = 4,
                    NumberPlate = "ABC123",
                    Mileage = 500028,
                    YearManufactured = new DateTime(2002, 1, 1),
                    YearPurchased = new DateTime(2005, 1, 1),
                    DateOfNextRegistration = new DateTime(2016, 12, 10),
                    DateOfNextWarrant = new DateTime(2017, 6, 11),
                    Expenses = expenses,
                    Refuels = refuels,
                    Repairs = repairs

                }
            };

            foreach (Vehicle v in vehicles)
            {
                context.Set<Vehicle>().Add(v);
            }
            base.Seed(context);
        }
    }
}

