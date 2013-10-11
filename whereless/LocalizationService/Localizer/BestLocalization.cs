using System;
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
    public class BestLocalization : LocalizationAlgorithm
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BestLocalization));
        private static readonly ulong LearningTresh = 15UL;

        public override void Initialize(IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                Location candidateLocation = null;
                var locations = uow.GetAll<Location>();
                if (locations.Count > 0)
                {
                    double min = Double.MaxValue;
                    foreach (var location in locations)
                    {
                        double curr = location.TestInput(input);
                        if (curr.CompareTo(0D) >= 0 && curr.CompareTo(min) < 0)
                        {
                            min = curr;
                            candidateLocation = location;
                        }
                    }
                    if (candidateLocation != null)
                    {
                        currLocation = candidateLocation;

                        currLocation.StartActivities();

                        currLocation.SetUpCurrentTimeStats();

                        currLocation.UpdateStats(input);
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

                    // Learning phase
                    if (currLocation != null && currLocation.GetObservations() <= LearningTresh)
                    {
                        Log.Debug("Learning Phase "+ currLocation.Name + ": step = " + currLocation.GetObservations());
                        currLocation.UpdateStats(input);
                        uow.Commit();
                        return;
                    }
                }

                Log.Debug("Trying to retrieve all locations");
                var locations = uow.GetAll<Location>();
                Log.Debug("All locations retrieved");

                Location candidateLocation = null;
                if (locations.Count > 0)
                {
                    double min = Double.MaxValue;
                    foreach (var location in locations)
                    {
                        double curr = location.TestInput(input);
                        if (curr.CompareTo(0D) >= 0 && curr.CompareTo(min) < 0)
                        {
                            candidateLocation = location;
                            min = curr;
                        }
                    }
                }

                if (candidateLocation != null)
                {
                    unknown = false;
                    if (currLocation == null || !currLocation.Name.Equals(candidateLocation.Name))
                    {
                        currLocation = candidateLocation;
                        currLocation.StartActivities();
                        currLocation.SetUpCurrentTimeStats();
                    }
                    Debug.Assert(currLocation.Name.Equals(candidateLocation.Name),
                                 "currLocation.Name.Equals(candidateLocation.Name)");
                    currLocation.UpdateStats(input);
                }
                else
                {
                    if (unknown)
                    {
                        Debug.Assert(currLocation != null, "currLocation != null");
                        if (currLocation.TestInput(input).CompareTo(0D) >= 0)
                        {
                            currLocation.UpdateStats(input);
                        }
                        else
                        {
                            unknown = false;
                        }
                    }

                    if (!unknown)
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
                Location candidateLocation = uow.GetLocationByName(name);
                if (currLocation.Name.Equals(candidateLocation.Name))
                {
                    return;
                }
                currLocation = candidateLocation;
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

