using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IDoBuilder
    {
        IPhilipsHueCommand PhilipsHue();
        IHeatingCommand Heating();
    }

    public interface IPhilipsHueCommand
    {
        ICommand On(string area);
        ICommand Off(string area);
        ICommand SwitchOnScene(string scene);
    }

    public interface IHeatingCommand
    {
        ICommand SetTemperature(double temp);
    }
}
