using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace GenericRepository.Mongo.Tests
{
    internal static class BsonClassMapHelper
    {
        public static void Clear()
        {
            var cm = BsonClassMap.GetRegisteredClassMaps().First();
            var fi = typeof(BsonClassMap).GetField("__classMaps", BindingFlags.Static | BindingFlags.NonPublic);
            var classMaps = (Dictionary<Type, BsonClassMap>)fi.GetValue(cm);
            classMaps.Clear();
        }
    }
}