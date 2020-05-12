﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
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

            return View(entry);
        }



        //Post Action Method
        //Old way before updating the view to use HTTP helpers.  Public ActionResult Add(DateTime? date, int? activityID, double? duration, Entry.IntensityLevel? intensity, bool? exclude, string notes)
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //Extract the date form field valud  -- string date = Request.Form["Date"];

            //Send the attemptedValues back because they could be null if there is an error converting to the correct type.
            // Old way to send back data - before using the htmlHelpers - ViewBag.Date = ModelState["date"].Value.AttemptedValue;

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                //ToDo Display the Entries List page.
            }


            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }
    }
}