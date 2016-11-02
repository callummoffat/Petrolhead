using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Petrolhead.Azure.DataObjects;
using Petrolhead.Azure.Models;
using Petrolhead.Azure.Helpers;
using System.Collections.Generic;
using AutoMapper;
using System;

namespace Petrolhead.Azure.Controllers
{
    public class VehicleController : TableController<VehicleDTO>
    {
        PetrolheadAppContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            context = new PetrolheadAppContext();
            DomainManager = new SimpleMappedEntityDomainManager<VehicleDTO, Vehicle>
                (context, Request, vehicle => vehicle.Id);
        }

        // GET tables/Vehicle
        [ExpandProperty(nameof(Vehicle.Expenses) + "/" + nameof(Expense.Purchases))]
        [ExpandProperty(nameof(Vehicle.Refuels))]
        [ExpandProperty(nameof(Vehicle.Repairs) + "/" + nameof(Repair.Components))]
        public IQueryable<VehicleDTO> GetAllVehicle()
        {
            return Query(); 
        }

        // GET tables/Vehicle/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [ExpandProperty(nameof(Vehicle.Expenses) + "/" + nameof(Expense.Purchases))]
        [ExpandProperty(nameof(Vehicle.Refuels))]
        [ExpandProperty(nameof(Vehicle.Repairs) + "/" + nameof(Repair.Components))]
        public SingleResult<VehicleDTO> GetVehicle(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Vehicle/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<VehicleDTO> PatchVehicle(string id, Delta<VehicleDTO> patch)
        {
            // Look up Vehicle from database so that EF updates the
            // existing entry.
            Vehicle currentVehicle = this.context.Vehicles
               .Include("Expenses.Refuels.Repairs")
               .First(v => v.Id == id);

            VehicleDTO updatedPatchEntity = patch.GetEntity();
            ICollection<ExpenseDTO> updatedExpenses;
            ICollection<RepairDTO> updatedRepairs;
            ICollection<RefuelDTO> updatedRefuels;

            bool requestContainsRelatedEntities = (patch.GetChangedPropertyNames()
                .Contains("Expenses")) ||
                (patch.GetChangedPropertyNames().
                Contains("Refuels")) ||
                (patch.GetChangedPropertyNames().
                Contains("Repairs"));

            if (requestContainsRelatedEntities)
            {
                // Remove unrelated expenses from the database
                for (int i = 0; i < currentVehicle.Expenses.Count
                    && updatedPatchEntity.Expenses != null; i++)
                {
                    ExpenseDTO expenseDTO = updatedPatchEntity.Expenses.FirstOrDefault(x =>
                    x.Id == currentVehicle.Expenses.ElementAt(i).Id);

                    if (expenseDTO == null)
                    {
                        context.Expenses.Remove(currentVehicle.Expenses.ElementAt(i));
                    }
                    else
                    {
                        Expense expense = currentVehicle.Expenses.ElementAt(i);

                        for (int idx = 0; i < expense.Purchases.Count
                            && expenseDTO.Purchases != null; idx++)
                        {
                            PurchaseDTO purchaseDTO = expenseDTO.Purchases.FirstOrDefault(p =>
                            p.Id == expense.Purchases.ElementAt(idx).Id);

                            if (purchaseDTO == null)
                            {
                                context.Purchases.Remove(expense.Purchases.ElementAt(i));
                            }
                        }
                    }
                }

                // Remove unrelated repairs from the database

            for (int i = 0; i < currentVehicle.Repairs.Count && updatedPatchEntity.Repairs != null; i++)
                {
                    RepairDTO repairDTO = updatedPatchEntity.Repairs.
                        FirstOrDefault(rp => rp.Id == currentVehicle.Repairs.ElementAt(i).Id);

                    if (repairDTO == null)
                    {
                        context.Repairs.Remove(currentVehicle.Repairs.ElementAt(i));
                    }
                    else
                    {
                        Repair repair = currentVehicle.Repairs.ElementAt(i);

                        for (int idx = 0; i < repair.Components.Count 
                            && repairDTO.Components != null; idx++)
                        {
                            ComponentDTO componentDTO = repairDTO.Components.FirstOrDefault(c =>
                            c.Id == repair.Components.ElementAt(idx).Id);

                            if (componentDTO == null)
                            {
                                context.Components.Remove(repair.Components.ElementAt(i));
                            }
                        }
                    }
                }

            // Remove unrelated refuels from the database
            for (int i = 0; i < currentVehicle.Refuels.Count 
                    && updatedPatchEntity.Refuels != null; i++)
                {
                    RefuelDTO refuelDTO = updatedPatchEntity.Refuels.FirstOrDefault(rf =>
                    rf.Id == currentVehicle.Refuels.ElementAt(i).Id);

                    if (refuelDTO == null)
                    {
                        context.Refuels.Remove(currentVehicle.Refuels.ElementAt(i));
                    }
                }

                Mapper.Map<VehicleDTO, Vehicle>(updatedPatchEntity, currentVehicle);
                updatedExpenses = updatedPatchEntity.Expenses;
                updatedRefuels = updatedPatchEntity.Refuels;
                updatedRepairs = updatedPatchEntity.Repairs;
            }
            else
            {
                VehicleDTO updatedVehicleDTO = Mapper.Map<Vehicle, VehicleDTO>(currentVehicle);
                patch.Patch(updatedVehicleDTO);
                Mapper.Map<VehicleDTO, Vehicle>(updatedVehicleDTO, currentVehicle);
                updatedExpenses = updatedPatchEntity.Expenses;
                updatedRefuels = updatedPatchEntity.Refuels;
                updatedRepairs = updatedPatchEntity.Repairs;
            }

            if (updatedExpenses != null)
            {
                currentVehicle.Expenses = new List<Expense>();
                foreach (ExpenseDTO expenseDTO in updatedExpenses)
                {
                    // get existing item
                    Expense existingExpense = context.Expenses.
                        FirstOrDefault(x => x.Id == expenseDTO.Id);
                    existingExpense = Mapper.Map<ExpenseDTO, Expense>(expenseDTO, existingExpense);
                    if (existingExpense.CreatedAt == null)
                        existingExpense.CreatedAt = DateTime.Now;
                    existingExpense.Purchases = new List<Purchase>();
                    existingExpense.Vehicle = currentVehicle;
                    foreach (PurchaseDTO purchaseDTO in expenseDTO.Purchases)
                    {
                        Purchase existingPurchase = context.Purchases
                            .FirstOrDefault(p => p.Id == purchaseDTO.Id);
                        existingPurchase = Mapper.Map<PurchaseDTO, Purchase>(purchaseDTO, existingPurchase);
                        if (existingPurchase.CreatedAt == null)
                            existingPurchase.CreatedAt = DateTime.Now;
                        existingPurchase.Expense = existingExpense;
                        existingExpense.Purchases.Add(existingPurchase);
                    }
                    currentVehicle.Expenses.Add(existingExpense);
                    
                }
            }

            if (updatedRepairs != null)
            {
                currentVehicle.Repairs = new List<Repair>();
                foreach (RepairDTO repairDTO in updatedRepairs)
                {
                    Repair existingRepair = context.Repairs.FirstOrDefault(rp => rp.Id == repairDTO.Id);
                    existingRepair = Mapper.Map<RepairDTO, Repair>(repairDTO, existingRepair);
                    if (existingRepair.CreatedAt == null)
                        existingRepair.CreatedAt = DateTime.Now;
                    existingRepair.Components = new List<Component>();
                    existingRepair.Vehicle = currentVehicle;
                    foreach (ComponentDTO componentDTO in repairDTO.Components)
                    {
                        Component existingComponent = context.Components
                            .FirstOrDefault(c => c.Id == componentDTO.Id);
                        existingComponent = Mapper.Map<ComponentDTO, Component>(componentDTO, existingComponent);
                        if (existingComponent.CreatedAt == null)
                            existingComponent.CreatedAt = DateTime.Now;
                        existingComponent.Repair = existingRepair;
                        existingRepair.Components.Add(existingComponent);
                    }

                    currentVehicle.Repairs.Add(existingRepair);
                }
            }

            if (updatedRefuels != null)
            {
                currentVehicle.Refuels = new List<Refuel>();

                foreach (RefuelDTO refuelDTO in updatedRefuels)
                {
                    Refuel existingRefuel = context.Refuels.FirstOrDefault(
                        rf => rf.Id == refuelDTO.Id);
                    existingRefuel = Mapper.Map<RefuelDTO, Refuel>(refuelDTO, existingRefuel);
                    if (existingRefuel.CreatedAt == null)
                        existingRefuel.CreatedAt = DateTime.Now;
                    existingRefuel.Vehicle = currentVehicle;
                    currentVehicle.Refuels.Add(existingRefuel);
                }
            }

            await this.context.SaveChangesAsync();

            VehicleDTO result = Mapper.Map<Vehicle, VehicleDTO>(currentVehicle);
            return result;
        }

        // POST tables/Vehicle
        public async Task<IHttpActionResult> PostVehicle(VehicleDTO item)
        {
            VehicleDTO current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Vehicle/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteVehicle(string id)
        {
             return DeleteAsync(id);
        }
    }
}
