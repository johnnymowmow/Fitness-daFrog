using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        //Get ActionMethod
        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };
            SetupActivitiesSelectListItems();

            return View(entry);
        }





        //Post Action Method
        //Old way before updating the view to use HTTP helpers.  Public ActionResult Add(DateTime? date, int? activityID, double? duration, Entry.IntensityLevel? intensity, bool? exclude, string notes)
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //Global validation Message - by leaving the first parameter as empty string, it makes it global.
            //ModelState.AddModelError("", "This is a global message.");


            //When not "Strongly typed" Extract the date form field valud  -- string date = Request.Form["Date"];

            //Send the attemptedValues back because they could be null if there is an error converting to the correct type.
            // Old way to send back data - before using the htmlHelpers - ViewBag.Date = ModelState["date"].Value.AttemptedValue;

            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                //REDIRECT instead of returning the view. Send TO NEW PAGE TO DO THE "Post, Redirect, Get" pattern.
                return RedirectToAction("Index");

            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get the requested entry from the repository.
            Entry entry = _entriesRepository.GetEntry((int)id);

            //REturn a status of "not found" if the entry wasn't found
            if (entry == null)
            {
                return HttpNotFound();
            }

            SetupActivitiesSelectListItems();


            //TODO pass the entry into the view.
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            //Validate the entry.
            ValidateEntry(entry);

            //If the entry is valid...
            if (ModelState.IsValid)
            {
                //1) Use the repository to update the entry
                _entriesRepository.UpdateEntry(entry);

                //2) Redirect the user to the "Entries" list page
                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();
            return View(entry);
        }



        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Retrieve entry for the provided id param value
            Entry entry = _entriesRepository.GetEntry((int)id);

            if (entry == null)
            {
                //1) Return "Not Found" if and entry was not found
                return HttpNotFound();
            }

            //2) Pass the entry to the view
            return View(entry);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Delete the Entry
            _entriesRepository.DeleteEntry(id);

            //REdirect to the Entries List page
            RedirectToAction("Index");

            return null;
        }




        private void ValidateEntry(Entry entry)
        {
            //If there aren't any "Duration" Field validation errors
            //then make sure that the duration is greater than 0
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "ID", "Name");
        }


    }
}