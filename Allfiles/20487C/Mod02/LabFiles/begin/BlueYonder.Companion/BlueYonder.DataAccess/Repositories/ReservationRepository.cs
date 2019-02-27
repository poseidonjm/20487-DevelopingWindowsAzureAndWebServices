﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BlueYonder.DataAccess.Interfaces;
using BlueYonder.Entities;

namespace BlueYonder.DataAccess.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        TravelCompanionContext context;

        public ReservationRepository(string connectionName)
        {
            context = new TravelCompanionContext(connectionName);
        }

        public ReservationRepository()
        {
            context = new TravelCompanionContext();
        }

        public ReservationRepository(TravelCompanionContext dbContext)
        {
            context = dbContext;
        }

        public Reservation GetSingle(int entityKey)
        {
            //TODO :Lab 02 Exercise 1, Task 4.2 : Implement the GetSingle Method
            var query = from r in context.Reservations
                        where r.ReservationId == entityKey
                        select r;
            return query.SingleOrDefault();
        }

        //TODO :Lab 02 Exercise 1, Task 4.5 : unmark the GetAll Method
        public IQueryable<Reservation> GetAll()
        {
            return context.Reservations.AsQueryable<Reservation>();
        }

        public IQueryable<Reservation> FindBy(Expression<Func<Reservation, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }

        //TODO :Lab 02 Exercise 1, Task 4.5 : unmark the Add Method

        public void Add(Reservation entity)
        {
            context.Reservations.Add(entity);
        }

        //TODO :Lab 02 Exercise 1, Task 4.5 : unmark the Delete Method
        public void Delete(Reservation entity)
        {

            context.Reservations.Find(entity.ReservationId);
            if (entity.DepartFlightScheduleID != 0)
                context.Entry(entity.DepartureFlight).State = System.Data.Entity.EntityState.Deleted;
            if (entity.ReturnFlightScheduleID != 0)
                context.Entry(entity.ReturnFlight).State = System.Data.Entity.EntityState.Deleted;
            context.Reservations.Remove(entity);

        }

        public void Edit(Reservation entity)
        {
            //TODO :Lab 02 Exercise 1, Task 4.3 : Implement the Edit Method
            var originalEntity = context.Reservations.Find(entity.ReservationId);
            context.Entry(originalEntity).CurrentValues.SetValues(entity);
        }

        public void Save()
        {
            context.SaveChanges();
        }
        
        public void Dispose()
        {
            //TODO :Lab 02 Exercise 1, Task 4.4 : Implement the Dispose Method      
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }
       
    }
}
