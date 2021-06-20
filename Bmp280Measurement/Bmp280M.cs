using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using System;
using System.Device.I2c;

namespace Bmp280Measurement
{
	public class Bmp280M
    {
        private readonly Bmp280 i2CBmp280;

        public Bmp280M()
        {
            const int busId = 1;
            I2cConnectionSettings i2cSettings = new(busId, Bmp280.DefaultI2cAddress);
            I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            i2CBmp280 = new Bmp280(i2cDevice);
            // set higher sampling
            i2CBmp280.TemperatureSampling = Sampling.LowPower;
            i2CBmp280.PressureSampling = Sampling.UltraHighResolution;            
        }

        public Bmp280ReadResult Read() => i2CBmp280.Read();
    }
}
