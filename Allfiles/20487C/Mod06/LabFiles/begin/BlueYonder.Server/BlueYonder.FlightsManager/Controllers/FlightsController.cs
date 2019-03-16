using BlueYonder.Companion.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace BlueYonder.FlightsManager.Controllers
{
    public class FlightsController : Controller
    {
        string _companionService = WebConfigurationManager.AppSettings["webapi:BlueYonderCompanionService"];

        private static HttpClient proxy = new HttpClient();
        //proxy.Timeout = TimeSpan.FromSeconds(360);

        public async Task<ActionResult> Index()
        {
            IEnumerable<LocationDTO> locations = null;

            //specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            //using (var proxy = new HttpClient())
            //{
            //BaseAddress = new Uri(_companionService),
            //  Timeout = TimeSpan.FromSeconds(360)
            //   })
            //{

            //locations = proxy.GetAsync("Locations?orderby=Country,City").Result.Content.
            //   ReadAsAsync<IEnumerable<LocationDTO>>().Result;
            HttpResponseMessage response = null;
                try {
                response = await proxy.GetAsync("https://blueyonder-companion-jm2.azurewebsites.net/Locations");
                //response = await proxy.GetAsync("http://localhost:49698/Locations");
            } catch (Exception e) {
                    return Content("An exception occurred: " + e.Message);
                    //response = await proxy.GetAsync("https://blueyonder-companion-jm.azurewebsites.net/Locations");
                }
                
                if (response.IsSuccessStatusCode)
                {
                    locations = await response.Content.ReadAsAsync<IEnumerable<LocationDTO>>();
                    return View(locations);
                }
                return Content("An error occurred: " + response.Content);
            //}
            
                        
        }

        public ActionResult Schedules(int from, int to)
        {
            IEnumerable<FlightWithSchedulesDTO> flights = null;

            using (var proxy = new HttpClient() { BaseAddress = new Uri(_companionService) })
            {                
                flights = proxy.GetAsync(
                    string.Format("Flights?source={0}&destination={1}", from, to)).
                    Result.Content.ReadAsAsync<IEnumerable<FlightWithSchedulesDTO>>().Result;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return PartialView(flights);
        }

        public ActionResult UpdateActualDeparture(int id, int scheduleId, string date, string time)
        {
            DateTime newDeparture = DateTime.Parse(date + " " + time);
            using (var proxy = new HttpClient() { BaseAddress = new Uri(_companionService) })
            {
                FlightScheduleDTO schedule = proxy.GetAsync(
                    string.Format("Flights/{0}?scheduleId={1}", id, scheduleId)).
                    Result.Content.ReadAsAsync<FlightScheduleDTO>().Result;

                schedule.ActualDeparture = newDeparture;

                proxy.PutAsJsonAsync(
                    string.Format("Flights/{0}?scheduleId={1}", id, scheduleId),
                    schedule).Wait();
            }
            return Content("updated");
        }
    }
}
