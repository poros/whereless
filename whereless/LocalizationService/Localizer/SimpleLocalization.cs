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
                        if (location.TestInput(input))
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
                    Log.Debug("Trying to retrieve Location" + currLocation.Name);
                    currLocation = uow.GetLocationByName(currLocation.Name);
                    Log.Debug(currLocation == null ? "Nothing Retrieved" : "Retrieved" + currLocation.Name);
                }

                // currLocation may be null if updated by others threads
                //it could also be unknown
                // proximity bias
                if (currLocation != null && currLocation.TestInput(input))
                {
                    currLocation.UpdateStats(input);
                }
                else
                {
                    Location oldLocation = currLocation;
                    currLocation = null;

                    var locations = ModelHelper.GetLocationRepository().GetAll();
                    if (locations.Count > 0)
                    {
                        foreach (var location in locations
                            .Where(location => location != oldLocation))
                        {
                            if (location.TestInput(input))
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
