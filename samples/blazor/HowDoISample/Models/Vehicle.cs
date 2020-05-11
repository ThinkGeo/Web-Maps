using System.Collections.ObjectModel;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    /// <summary>
    /// This class stands for a vehicle.
    /// </summary>
    public class Vehicle
    {
        private int id;
        private Location location;
        private string vehicleName;
        private string vehicleIconVirtualPath;
        private bool isInFence;
        private string motionStateIconVirtualPath;
        private Collection<Location> historyLocations;

        public Vehicle()
            : this(0)
        { }

        public Vehicle(int id)
        {
            Id = id;
            VehicleName = string.Empty;
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

        public Collection<Location> HistoryLocations
        {
            get { return historyLocations; }
        }

        public string VehicleName
        {
            get { return vehicleName; }
            set { vehicleName = value; }
        }

        public string VehicleIconVirtualPath
        {
            get { return vehicleIconVirtualPath; }
            set { vehicleIconVirtualPath = value; }
        }

        public bool IsInFence
        {
            get { return isInFence; }
            set { isInFence = value; }
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

        public string MotionStateIconVirtualPath
        {
            get
            {
                if (MotionState == VehicleMotionState.Idle)
                {
                    motionStateIconVirtualPath = "images/ball_gray.png";
                }
                else
                {
                    motionStateIconVirtualPath = "images/ball_green.png";
                }
                return motionStateIconVirtualPath;
            }
        }

        /// <summary>
        /// If the Vehicle's speed is not 0 in the passed 4 minutes, we say it is in Motion. 
        /// </summary>
        /// <returns>State of current vehicle.</returns>
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

        public string MotionText
        {
            get { return MotionState == VehicleMotionState.Motion ? "In Motion" : "Idle"; }
        }
    }
}
