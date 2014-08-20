using System;
using System.Collections.Generic;
using Footprints.Models;

namespace Footprints.Common
{
    public class Comparators
    {
    }
    public class JourneyEqualityComparer : IEqualityComparer<Journey>
    {
        public bool Equals(Journey j1, Journey j2)
        {
            return j1.JourneyID == j2.JourneyID;
        }
        public int GetHashCode(Journey journey)
        {
            return journey.JourneyID.GetHashCode();
        }
    }
    public class DestinationEqualityComparer : IEqualityComparer<Destination>
    {
        public bool Equals(Destination d1, Destination d2)
        {
            return d1.DestinationID == d2.DestinationID;
        }
        public int GetHashCode(Destination des)
        {
            return des.DestinationID.GetHashCode();
        }
    }
    public class UserEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User u1, User u2)
        {
            return u1.UserID == u2.UserID;
        }
        public int GetHashCode(User user)
        {
            return user.UserID.GetHashCode();
        }
    }
}