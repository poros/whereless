﻿using System;
using System.Collections.Generic;
using whereless.Model.Entities;
using whereless.Model.ValueObjects;

namespace whereless.Model.Factory
{
    public class MplZipGn : IEntitiesFactory
    {
        //public virtual Type LocationType
        //{
        //    get { return typeof(MultiPlacesLocation); }
        //}

        //public virtual Type PlaceType
        //{
        //    get { return typeof (ZIndexPlace); }
        //}

        //public virtual Type NetworkType
        //{
        //    get { return typeof(GaussianNetwork); }
        //}

        public virtual Location CreateLocation(String name)
        {
            return new MultiPlacesLocation(name);
        }

        public virtual Location CreateLocation(String name, IList<IMeasure> measures)
        {
            return new MultiPlacesLocation(name, measures);
        }

        public virtual Place CreatePlace()
        {
            return new ZIndexPlace();
        }

        public virtual Place CreatePlace(IList<IMeasure> measures)
        {
            return new ZIndexPlace(measures);
        }

        public virtual Network CreateNetwork(IMeasure measure)
        {
            return new GaussianNetwork(measure);
        }

        public virtual Activity CreateActivity(string activityName, string pathfile, string argument, string activityType)
        {
            return new Activity(activityName, pathfile, argument, activityType);
        }
    }
}
