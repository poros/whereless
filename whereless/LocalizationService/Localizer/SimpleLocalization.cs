﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using whereless.Model;
using whereless.Model.Entities;
using whereless.Model.ValueObjects;

namespace whereless.LocalizationService.Localizer
{
    public class SimpleLocalization : LocalizationAlgorithm
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleLocalization));

        public override void Initialize(IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                var locations = uow.GetAll<Location>();
                if (locations.Count > 0)
                {
                    foreach (var location in locations)
                    {
                        if (location.TestInput(input) >= 0)
                        {
                            currLocation = location;

                            currLocation.StartActivities();

                            currLocation.SetUpCurrentTimeStats();

                            location.UpdateStats(input);
                            break;
                        }
                    }
                }
                uow.Commit();
            }

            if (currLocation == null)
            {
                currLocation = ModelHelper.EntitiesFactory
                                      .CreateLocation(LocationLocalizer.Unknown, input);
                unknown = true;
            }
        }

        public override void Localize(IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                // currLocation may be updated by other threads
                if (!unknown)
                {
                    Log.Debug("Trying to retrieve Location " + currLocation.Name);
                    currLocation = uow.GetLocationByName(currLocation.Name);
                    Log.Debug(currLocation == null ? "Nothing Retrieved " : "Retrieved " + currLocation.Name);
                }

                // currLocation may be null if updated by others threads
                //it could also be unknown
                // proximity bias
                if (currLocation != null && !unknown && currLocation.TestInput(input) >= 0)
                {
                        currLocation.UpdateStats(input);
                } 
                else
                {
                    String oldLocationName = null;
                    if (currLocation != null)
                    {
                        oldLocationName = currLocation.Name;
                    }
                    if (!unknown)
                    {
                        currLocation = null;
                    }

                    Log.Debug("Trying to retrieve all locations");
                    var locations = uow.GetAll<Location>();
                    Log.Debug("All locations retrieved");

                    if (locations.Count > 0)
                    {
                        foreach (var location in locations
                            .Where(location => oldLocationName == null || !location.Name.Equals(oldLocationName)))
                        {
                            if (location.TestInput(input) >= 0)
                            {
                                unknown = false;
                                currLocation = location;

                                currLocation.StartActivities();
                                
                                currLocation.SetUpCurrentTimeStats();

                                location.UpdateStats(input);
                                break;
                            }
                        }
                    }

                    if (unknown)
                    {
                        Debug.Assert(currLocation != null, "currLocation != null");
                        if (currLocation.TestInput(input) >= 0)
                        {
                            currLocation.UpdateStats(input);
                        }
                        else
                        {
                            currLocation = null;
                        }
                    }
                    if (currLocation == null)
                    {
                        currLocation = ModelHelper.EntitiesFactory
                                                  .CreateLocation(LocationLocalizer.Unknown, input);
                        unknown = true;
                    }
                }
                uow.Commit();
            }
        }

        public override void ForceLocation(string name, IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                currLocation = uow.GetLocationByName(name);
                Debug.Assert(currLocation != null, "currLocation != null");

                currLocation.ForceLocation(input);

                currLocation.StartActivities();

                unknown = false;
                uow.Commit();
            }
        }

        public override void ForceUnknown(IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                currLocation = ModelHelper.EntitiesFactory
                                  .CreateLocation(LocationLocalizer.Unknown, input);
                unknown = true;
                uow.Commit();
            }
        }

        public override void RegisterLocation(string name, IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                if (!unknown)
                {
                    currLocation = ModelHelper.EntitiesFactory
                                               .CreateLocation(name, input);
                }
                else
                {
                    currLocation.Name = name;
                    unknown = false;
                }
                uow.Save(currLocation);
                uow.Commit();
            }
        }
    }
}
