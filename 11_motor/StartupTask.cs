using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace _11_motor
{
    public sealed class StartupTask : IBackgroundTask
    {
        //Debug.WriteLine("========================================\n");
        //printf("|                Motor                 |\n");
        //printf("|    ------------------------------    |\n");
        //printf("|     Motor pin 1 connect to GPIO0     |\n");
        //printf("|     Motor pin 2 connect to GPIO1     |\n");
        //printf("|     Motor enable connect to GPIO3    |\n");
        //printf("|                                      |\n");
        //printf("|         Controlling a motor          |\n");
        //printf("|                                      |\n");
        //printf("|                            SunFounder|\n");
        //printf("========================================\n");
        //printf("\n");
        //printf("\n");

        private const int MOTOR_PIN1 = 17;
        private const int MOTOR_PIN2 = 18;
        private const int MOTOR_ENABLE = 27;
        private GpioPin MotorPin1;
        private GpioPin MotorPin2;
        private GpioPin MotorEnable;

        private DispatcherTimer timer;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += Timer_Tick;
            InitGPIO();
            if (MotorPin1 != null)
            {
                timer.Start();
            }

            // described in http://aka.ms/backgroundtaskdeferral
            //
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                MotorPin1 = null;
                MotorPin2 = null;
                MotorEnable = null;
                return;
            }

            MotorPin1 = gpio.OpenPin(MOTOR_PIN1);
            MotorPin1.SetDriveMode(GpioPinDriveMode.Output);
            MotorPin2 = gpio.OpenPin(MOTOR_PIN2);
            MotorPin2.SetDriveMode(GpioPinDriveMode.Output);
            MotorEnable = gpio.OpenPin(MOTOR_ENABLE);
            MotorEnable.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(object sender, object e)
        {
            Motor(1);
            Task.Delay(3000);
            Motor(0);
            Task.Delay(3000);
            Motor(-1);
            Task.Delay(3000);
            Motor(0);
            Task.Delay(3000);
        }

        private void Motor(int direction)
        {
            if (direction == 1) //Clockwise
            {
                MotorPin1.Write(GpioPinValue.High);
                MotorPin2.Write(GpioPinValue.Low);
                MotorEnable.Write(GpioPinValue.High);
            }
            if (direction == -1) //CounterClockwise
            {
                MotorPin1.Write(GpioPinValue.Low);
                MotorPin2.Write(GpioPinValue.High);
                MotorEnable.Write(GpioPinValue.High);
            }
            if (direction == 0) //Stop
            {
                MotorEnable.Write(GpioPinValue.Low);
            }
        }
    }
}
