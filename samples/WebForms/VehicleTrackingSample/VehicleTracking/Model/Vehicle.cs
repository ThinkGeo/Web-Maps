using System.Collections.ObjectModel;

namespace ThinkGeo.MapSuite.VehicleTracking
{
    /// <summary>
    /// This class stands for a vehicle.
    /// </summary>
    public class Vehicle
    {
        private int id;
        private string name;
        private bool isInFence;
        private string iconPath;
        private Location location;
        private string motionStateIconVirtualPath;
        private Collection<Location> historyLocations;

        public Vehicle()
            : this(0)
        { }

        public Vehicle(int id)
        {
            Id = id;
            Name = string.Empty;
            Location = new Location();
            historyLocations = new Collection<Location>();
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Location Location
        {
            get { return location; }
            set { location = value; }
        }

        public double Longitude
        {
            get { return location.Longitude; }
        }

        public double Latitude
        {
            get { return location.Latitude; }
        }

        public Collection<Location> HistoryLocations
        {
            get { return historyLocations; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string IconPath
        {
            get { return iconPath; }
            set { iconPath = value; }
        }

        public bool IsInFence
        {
            get { return isInFence; }
            set { isInFence = value; }
        }

        public string MotionStateIconVirtualPath
        {
            get
            {
                if (MotionState == VehicleMotionState.Idle)
                {
                    motionStateIconVirtualPath = "Images/ball_gray.png";
                }
                else
                {
                    motionStateIconVirtualPath = "Images/ball_green.png";
                }
                return motionStateIconVirtualPath;
            }
        }

        public VehicleMotionState MotionState
        {
            get
            {
                VehicleMotionState vehicleState = VehicleMotionState.Idle;

                if (Location.Speed != 0)
                {
                    vehicleState = VehicleMotionState.Motion;
                }
                else
                {
                    int locationIndex = 0;
                    foreach (Location historyLocation in HistoryLocations)
                    {
                        if (locationIndex > 3)
                        {
                            break;
                        }
                        else if (historyLocation.Speed != 0)
                        {
                            vehicleState = VehicleMotionState.Motion;
                            break;
                        }
                        else
                        {
                            locationIndex++;
                        }
                    }
                }

                return vehicleState;
            }
        }

        public int SpeedDuration
        {
            get
            {
                int speedDuration = 0;
                double lastSpeed = Location.Speed;
                foreach (Location location in HistoryLocations)
                {
                    if (location.Speed == lastSpeed)
                    {
                        speedDuration++;
                    }
                    else
                    {
                        break;
                    }
                }
                return speedDuration;
            }
        }


        /// <summary>
        /// If the Vehicle's speed is not 0 in the passed 4 minutes, we say it is in Motion. 
        /// </summary>
        /// <returns>State of current vehicle.</returns>
        public VehicleMotionState GetCurrentState()
        {
            VehicleMotionState vehicleState = VehicleMotionState.Idle;

            if (Location.Speed != 0)
            {
                vehicleState = VehicleMotionState.Motion;
            }
            else
            {
                int locationIndex = 0;
                foreach (Location historyLocation in HistoryLocations)
                {
                    if (locationIndex > 3)
                    {
                        break;
                    }
                    if (historyLocation.Speed != 0)
                    {
                        vehicleState = VehicleMotionState.Motion;
                        break;
                    }
                    locationIndex++;
                }
            }

            return vehicleState;
        }
    }
}
