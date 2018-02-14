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
        ICommandHandler On(string area);
        ICommandHandler Off(string area);
        ICommandHandler SwitchOnScene(string scene);
    }

    public interface IHeatingCommand
    {
        ICommandHandler SetTemperature(double temp);
    }
}
